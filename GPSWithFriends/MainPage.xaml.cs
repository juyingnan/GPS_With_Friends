using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GPSWithFriends.Resources;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Tasks;
using System.Device.Location;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Phone.Maps.Controls;
using GPSWithFriends.ViewModels;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Toolkit;

using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Foundation;
using System.Collections;
using System.Windows.Threading;

namespace GPSWithFriends
{
    public partial class MainPage : PhoneApplicationPage
    {
        const int INTERVAL_BETWEEN_INQUERY_MESSAGE = 30;
        const int MOVEMENT_THRESHOLD = 50;
        const int TEST_TIMER_TIMERSPAN = 3;
        const int MAP_RECTANGLE_THICKNESS = 10;
        const int MAP_MAX_ZOOMLEVEL = 19;
        const int MAP_MIN_ZOOMLEVEL = 1;
        const double MAP_MIN_LOCATE_ZOOMLEVEL = 15;
        const string DEFAULT_GROUP_NAME = "My Friends";

        //GPS
        Geolocator myGeoLocator = new Geolocator();
        Friend Me = App.ViewModel.Me;
        //for Route Query
        RouteQuery MyQuery = null;
        MapRoute MyMapRoute = null;

        //Message Timer
        DispatcherTimer TimerForMessage = new DispatcherTimer();

        Server.UserActionSoapClient proxy = new Server.UserActionSoapClient();
        
        /// <summary>
        /// Constructor
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            //Necessary codes to initiate the toolkit map control
            this.MapExtensionsSetup(this.MyMap);

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

            myGeoLocator.DesiredAccuracy = PositionAccuracy.High;
            myGeoLocator.MovementThreshold = MOVEMENT_THRESHOLD;

            //map extension controls data binding
            this.FriendsLocationMarkerList.ItemsSource = App.ViewModel.Friends;
            MyLocationMarker.DataContext = this.Me;

            //update message
            TimerForMessage.Tick += TimerForMessage_Tick;
            TimerForMessage.Interval = TimeSpan.FromSeconds(INTERVAL_BETWEEN_INQUERY_MESSAGE);
            TimerForMessage.Start();
            proxy.GetMessagesCompleted += proxy_GetMessagesCompleted;
            proxy.GetMessagesAsync(Me.Uid);

            proxy.SendMessageCompleted += proxy_SendMessageCompleted;
            proxy.AnswerAddFriendCompleted += proxy_AnswerAddFriendCompleted;
            proxy.CreateGroupCompleted += proxy_CreateGroupCompleted;
            proxy.DeleteGroupCompleted += proxy_DeleteGroupCompleted;
            proxy.GetUserFromEmailCompleted += proxy_GetUserFromEmailCompleted;
        }

        void TimerForMessage_Tick(object sender, EventArgs e)
        {
            //receive messages
            proxy.GetMessagesAsync(Me.Uid);
        }

        void proxy_GetMessagesCompleted(object sender, Server.GetMessagesCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                foreach (var item in e.Result)
                {
                    switch (item.content_type)
                    {
                        case Server.CMessageType.DEFAULT_TYPE:
                            break;
                        case Server.CMessageType.IM_MESSAGE:                            
                            foreach (var friend in App.ViewModel.Friends)
                            {
                                if (friend.Email.Equals(item.from_email))
                                {
                                    friend.LastMessage = new Message()
                                      {
                                          Content = item.content,
                                          IsCleared = false,
                                          LastUpdateTime = item.timestamp,
                                          FromEmail = item.from_email,
                                          ToEmail = Me.Email
                                      };
                                    break;
                                }
                            }
                            break;
                        case Server.CMessageType.LOC_UPDATE:
                            foreach (var friend in App.ViewModel.Friends)
                            {
                                if (friend.Email.Equals(item.from_email))
                                {
                                    string[] info = item.content.Trim(new char[]{'(',')'}).Split(',');
                                    if (info != null && info.Length == 3)    //judge the format "(email,lat,lon)"
                                    {
                                        try
                                        {
                                            
                                        friend.Latitude = Double.Parse(info[1]);//lat
                                        friend.Longitude = Double.Parse(info[2]); //lon
                                        }
                                        catch (Exception)
                                        {
                                            
                                        }
                                    }
                                    break;
                                }
                            } 
                            break;
                        case Server.CMessageType.NOTIFICATION:
                            if (item.content.StartsWith("[*ANS_ADDF*]"))
                            {
                                App.ViewModel.RefreshData();
                            }
                            if (item.content.StartsWith("[*DELF_INFO*]"))
                            {
                                App.ViewModel.RefreshData();
                            }
                            break;
                        case Server.CMessageType.REQUEST:
                            App.ViewModel.Requests.Add(new Request()
                            {
                                Content = item.content,
                                SenderEmail = item.from_email,
                                IsAccepted = false,
                                Time = item.timestamp.ToString()
                            });
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        //Necessary codes to initiate the toolkit map control
        public void MapExtensionsSetup(Map map)
        {
            ObservableCollection<DependencyObject> children = MapExtensions.GetChildren(map);
            var runtimeFields = this.GetType().GetRuntimeFields();

            foreach (DependencyObject i in children)
            {
                var info = i.GetType().GetProperty("Name");

                if (info != null)
                {
                    string name = (string)info.GetValue(i);

                    if (name != null)
                    {
                        foreach (FieldInfo j in runtimeFields)
                        {
                            if (j.Name == name)
                            {
                                j.SetValue(this, i);
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }            

            //if register page is visited before, backentry will be more than one
            while (this.NavigationService.CanGoBack)
            {
                this.NavigationService.RemoveBackEntry();
            }
        }        

        private async void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            SetProperMapZoomLevel();
            await LocateMe();
            SetProperMapZoomLevel();
        }

        /// <summary>
        /// Locate myself, show the marker and upload my location
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task LocateMe()
        {
            try
            {
                Geoposition position = await myGeoLocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(1), timeout: TimeSpan.FromSeconds(30));
                GPSLocationTextblock.Text = "Location of Me: " + string.Format("Lat: {0:0.0000}, Lon: {1:0.0000}",
                 position.Coordinate.Latitude, position.Coordinate.Longitude);
                Me.Latitude = position.Coordinate.Latitude;
                Me.Longitude = position.Coordinate.Longitude;
                //refresh distance when I am located at first time
                foreach (var item in App.ViewModel.Friends)
                {
                    item.CalculateDistance();
                }
            }
            catch (UnauthorizedAccessException)
            {
                GPSLocationTextblock.Text = "Location is disabled in phone settings.";
            }
            catch (Exception ex)
            {
                GPSLocationTextblock.Text = ex.Message;
            }
            finally
            {
                if (Me.isLocated())
                {
                    if (App.ViewModel.IsDataLoaded) //to ensure Me.uid has been set
                    {                        
                        try
                        {
                            proxy.UpdateLocationAsync(Me.Uid, Me.Latitude, Me.Longitude);
                        }
                        catch (TimeoutException e)
                        {
                            MessageBox.Show(e.Message);
                        }
                        finally
                        {
                            MyLocationMarker.Visibility = System.Windows.Visibility.Visible;
                            LocateFriend(Me);
                        }
                    }
                }
            }
        }

        private void FriendsListBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Friend friend = ((sender as LongListSelector).SelectedItem as Friend);
            if (friend != null)
            {
                LocateFriend(friend);
            }

            //to avoid bug when list is empty
            (sender as LongListSelector).SelectedItem = null;

            //void proxy_setLocationCompleted(object sender, Server.setLocationCompletedEventArgs e)
            //{
            //    //throw new NotImplementedException();
            //}

            ///// <summary>
            ///// Click a Listbox Item and the map will focus on that friend
            ///// </summary>
            ///// <param name="sender"></param>
            ///// <param name="e"></param>
            //private void FriendsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            //{
            //    //get index
            //    IList addedItems = e.AddedItems;
            //    if (addedItems == null || addedItems.Count < 1)
            //    {
            //        return;
            //    }

            //    Friend friend = addedItems[0] as Friend;
            //    if (friend.isLocated())
            //    {
            //        GPSLocationTextblock.Text = "Location of "+friend.NickName+": " + string.Format("Lat: {0:0.00}, Long: {1:0.00}",friend.Latitude, friend.Longitude);
            //        MyMap.SetView(new GeoCoordinate(friend.Latitude, friend.Longitude), MyMap.ZoomLevel, MapAnimationKind.Parabolic);
            //    }
            //}

        }

        private void Pushpin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Friend friend = ((sender as Pushpin).DataContext as Friend);
            if (friend != null)
            {
                LocateFriend(friend);
            }
        }

        private void LocateFriend(Friend friend)
        {
            if (friend.isLocated())
            {
                GPSLocationTextblock.Text = "Location of " + friend.NickName + ": " + string.Format("Lat: {0:0.00}, Long: {1:0.00}", friend.Latitude, friend.Longitude);
                double zoomLevel = MyMap.ZoomLevel;
                if (zoomLevel < MAP_MIN_LOCATE_ZOOMLEVEL)
                    zoomLevel = MAP_MIN_LOCATE_ZOOMLEVEL;
                MyMap.SetView(new GeoCoordinate(friend.Latitude, friend.Longitude), zoomLevel, MapAnimationKind.Parabolic);
            }
        }

        private void PushpinMessage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                Friend friend = ((sender as StackPanel).DataContext as Friend);
                if (friend != null)
                {
                    //send message
                    InputMessageToFriend(friend);
                }
            }
            catch (Exception)
            {
            }
        }

        private void FriendList_ClearMessage(object sender, RoutedEventArgs e)
        {
            //get index
            int selectedIndex = App.ViewModel.Friends.IndexOf((sender as MenuItem).DataContext as Friend);
            Friend friend = App.ViewModel.Friends[selectedIndex];

            //clear old message
            ClearMessage(friend);
        }

        public void ClearMessage(Friend friend)
        {
            friend.ClearMessage();

            //update to web
        }

        private void FriendList_SendMessage(object sender, RoutedEventArgs e)
        {
            //get index
            int selectedIndex = App.ViewModel.Friends.IndexOf((sender as MenuItem).DataContext as Friend);
            Friend friend = App.ViewModel.Friends[selectedIndex];

            //send message
            InputMessageToFriend(friend);
            LocateFriend(friend);
        }

        private void InputMessageToFriend(Friend friend)
        {
            //get group name
            TextBox messageInputBox = new TextBox();
            TiltEffect.SetIsTiltEnabled(messageInputBox, true);
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = "Send Message:",
                Message = "Please input the message.",
                Content = messageInputBox,
                LeftButtonContent = "Send",
                RightButtonContent = "Cancel",
                IsFullScreen = false,
            };

            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        string result = messageInputBox.Text;
                        if (result.Length != 0)
                        {
                            //send message
                            SendFriendMessage(friend, result);

                            //clear old message
                            ClearMessage(friend);
                        }
                        break;
                    case CustomMessageBoxResult.RightButton:
                        // Do something.
                        break;
                    case CustomMessageBoxResult.None:
                        // Do something.
                        break;
                    default:
                        break;
                }
            };

            messageBox.Show();
        }

        private void SendFriendMessage(Friend friend, string result)
        {
            proxy.SendMessageAsync(Me.Uid, friend.Email, result);
        }

        void proxy_SendMessageCompleted(object sender, Server.SendMessageCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result.Contains("ERROR"))
                    MessageBox.Show("Fail to send messag. Please try again.");
            }
            else
                MessageBox.Show("Server connection error. Please try again later.");

        }

        /// <summary>
        /// Get the route from me to a friend
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FriendList_Route(object sender, RoutedEventArgs e)
        {
            try
            {
                //get index
                int selectedIndex = App.ViewModel.Friends.IndexOf((sender as MenuItem).DataContext as Friend);
                Friend friend = App.ViewModel.Friends[selectedIndex];

                //Add start and end points
                List<GeoCoordinate> MyCoordinates = new List<GeoCoordinate>();
                MyCoordinates.Add(new GeoCoordinate(Me.Latitude, Me.Longitude));
                MyCoordinates.Add(new GeoCoordinate(friend.Latitude, friend.Longitude));

                //send the query
                MyQuery = new RouteQuery();
                MyQuery.Waypoints = MyCoordinates;
                MyQuery.QueryCompleted += MyQuery_QueryCompleted;
                try
                {
                    MyQuery.QueryAsync();
                }
                catch (Exception)
                {
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Something gotta wrong.");
            }
        }

        private void MyQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {
            if (e.Error == null)
            {
                Route MyRoute = e.Result;
                if (MyMapRoute != null)
                {
                    //clear last route
                    MyMap.RemoveRoute(MyMapRoute);
                }
                MyMapRoute = new MapRoute(MyRoute);
                MyMap.AddRoute(MyMapRoute);
                MyQuery.Dispose();

                //set the map view to fit the route
                LocationRectangle locationRectangle = LocationRectangle.CreateBoundingRectangle(MyRoute.Geometry);
                this.MyMap.SetView(locationRectangle, new Thickness(MAP_RECTANGLE_THICKNESS));
            }
        }

        /// <summary>
        /// Invite a friend via email address
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InviteButton_Click(object sender, RoutedEventArgs e)
        {
            InputFriendEmail();
        }

        /// <summary>
        /// Pop up a MessageBox with Input Textbox to input the email address of a friend
        /// </summary>
        public void InputFriendEmail()
        {
            TextBox emailInputBox = new TextBox()
            {
                //Set inputscope to Email
                InputScope = new System.Windows.Input.InputScope()
                {
                    Names = {new System.Windows.Input.InputScopeName() 
                    { NameValue = System.Windows.Input.InputScopeNameValue.EmailSmtpAddress }}
                }
            };
            TiltEffect.SetIsTiltEnabled(emailInputBox, true);
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = "Please input Email Address:",
                Message = "Please input email address of the friend that you want to add.",
                Content = emailInputBox,
                LeftButtonContent = "View",
                RightButtonContent = "Cancel",
                IsFullScreen = false,
            };

            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        string result = "";
                        result = emailInputBox.Text;
                        if (result.Length > 0 && !result.Equals(Me.Email))  //make sure there is input and the address doesn't belong to ME
                        {
                            proxy.GetUserFromEmailAsync(Me.Uid, result);
                            InviteButton.IsEnabled = false;
                            InviteProgressBar.IsEnabled = true;
                        }
                        else
                            MessageBox.Show("Please input a email address of a friend");
                        break;
                    case CustomMessageBoxResult.RightButton:
                        // Do something.
                        break;
                    case CustomMessageBoxResult.None:
                        // Do something.
                        break;
                    default:
                        break;
                }
            };
            messageBox.Show();
        }

        void proxy_GetUserFromEmailCompleted(object sender, Server.GetUserFromEmailCompletedEventArgs e)
        {
            InviteButton.IsEnabled = true;
            InviteProgressBar.IsEnabled = false;

            if (e.Error == null)
            {
                if (e.Result == null)
                    MessageBox.Show("The email you input doesn't exist, please try again.");
                else
                {
                    if (e.Result.IsFriendOfCaller)
                        MessageBox.Show("He/She has already been your friend.");
                    else
                    {
                        App.ViewModel.CurrentFriend = new Friend()
                        {
                            Email = e.Result.email,
                            ImagePath = "/Assets/fakePor.png",
                            NickName = e.Result.name,
                            IsFriend = e.Result.IsFriendOfCaller
                        };

                        this.NavigationService.Navigate(new Uri("/DetailPage.xaml", UriKind.Relative));
                    }
                }
            }
            else
                MessageBox.Show("Server connection error. Please try again later.");
        }  

        private void RequestsListBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Request request = (sender as ListBox).SelectedItem as Request;
            if (request != null)
            {
                RequestHandle(request);
            }
            ///// <summary>
            ///// Select a request to handle
            ///// </summary>
            ///// <param name="sender"></param>
            ///// <param name="e"></param>
            //private void RequestsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            //{
            //    ListBox listbox = sender as ListBox;
            //    int index = listbox.SelectedIndex;            
            //    if (index > -1 && index < App.ViewModel.Requests.Count)
            //    {
            //        Request request = App.ViewModel.Requests[index];

            //    }
            //}
        }

        public void RequestHandle(Request request)
        {
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = "Request Accept?",
                Message = request.Content,
                LeftButtonContent = "Accept",
                RightButtonContent = "Reject",
                IsFullScreen = false,
            };

            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        RequestDone(request, true);
                        break;
                    case CustomMessageBoxResult.RightButton:
                        RequestDone(request, false);
                        break;
                    case CustomMessageBoxResult.None:
                        // Do something.
                        break;
                    default:
                        break;
                }
            };

            messageBox.Show();
        }

        public void RequestDone(Request request, bool p)
        {
            //1. send request handle info to cloud
            App.ViewModel.Requests.Remove(request);
            if (p)
            {
                try
                {
                    proxy.AnswerAddFriendAsync(Me.Uid, request.SenderEmail, p);
                }
                catch (Exception)
                {

                }

                //refresh friendlist
            }
        }

        void proxy_AnswerAddFriendCompleted(object sender, Server.AnswerAddFriendCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result.Contains("ERROR"))
                    MessageBox.Show("Fail to answer the request.");
                else
                    App.ViewModel.RefreshData();    //success, refresh to get new friends
            }
            else
                MessageBox.Show("Server connection error. Please try again later.");
        }

        private void FriendsManageListBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.ViewModel.CurrentFriend = (sender as LongListSelector).SelectedItem as Friend;
            if (App.ViewModel.CurrentFriend != null)
            {
                this.NavigationService.Navigate(new Uri("/DetailPage.xaml", UriKind.Relative));
            }

            (sender as LongListSelector).SelectedItem = null;
            ///// <summary>
            ///// Select a friend to see detail page
            ///// </summary>
            ///// <param name="sender"></param>
            ///// <param name="e"></param>
            //private void FriendsManageListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            //{
            //    //get index
            //    IList addedItems = e.AddedItems;
            //    if (addedItems == null || addedItems.Count < 1)
            //    {
            //        return;
            //    }

            //    App.ViewModel.CurrentFriend = addedItems[0] as Friend;
            //    this.NavigationService.Navigate(new Uri("/DetailPage.xaml", UriKind.Relative));
            //}
        }

        /// <summary>
        /// Locate me
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ApplicationBarIconLocateMeButton_Click(object sender, EventArgs e)
        {
            await LocateMe();
        }

        /// <summary>
        /// set the map view to show all friends in the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarIconShowAllButton_Click(object sender, EventArgs e)
        {
            SetProperMapZoomLevel();
        }

        /// <summary>
        /// set the map view to show all friends in the list
        /// </summary>
        private void SetProperMapZoomLevel()
        {
            //Find all located friends in the list
            List<Friend> temp = new List<Friend>();

            foreach (var item in App.ViewModel.Friends)
            {
                if (item.isLocated())
                    temp.Add(item);
            }

            //including Me
            if (Me.isLocated())
                temp.Add(Me);

            if (temp.Count > 0)
            {
                LocationRectangle locationRectangle = LocationRectangle.CreateBoundingRectangle(from Friend in temp select Friend.Geocoordinate);
                this.MyMap.SetView(locationRectangle, new Thickness(MAP_RECTANGLE_THICKNESS));
            }
        }

        /// <summary>
        /// Zoom in the map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {   
            //make sure zoomlevel <=19
            if (MyMap.ZoomLevel + 1 <= MAP_MAX_ZOOMLEVEL)
                MyMap.ZoomLevel += 1;
            MyMap.SetView(MyMap.Center, MyMap.ZoomLevel, MapAnimationKind.Parabolic);
        }

        /// <summary>
        /// zoom out the map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Minus_Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //make sure zoomlevel >=1
            if (MyMap.ZoomLevel - 1 >= MAP_MIN_ZOOMLEVEL)
                MyMap.ZoomLevel -= 1;
            MyMap.SetView(MyMap.Center, MyMap.ZoomLevel, MapAnimationKind.Parabolic);
        }

        /// <summary>
        /// refresh the friend list data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarIconRefreshButton_Click(object sender, EventArgs e)
        {
            App.ViewModel.RefreshData();
        }

        ///// <summary>
        ///// remove a friend from the group
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void RemoveFromGroupItem_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        //get index
        //        int selectedIndex = App.ViewModel.Friends.IndexOf((sender as MenuItem).DataContext as Friend);
                
        //        //test
        //        App.ViewModel.Friends.Remove(App.ViewModel.Friends[selectedIndex]);
        //        App.ViewModel.RefreshGroup();

        //        //remove via uid and gid
        //        //proxy.removeMemberCompleted += proxy_removeMemberCompleted;
        //        //proxy.removeMemberAsync(App.ViewModel.Friends[selectedIndex].Uid, App.ViewModel.CurrentGroup.Gid);
        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("Something gotta wrong.");
        //    }
        //}

        private void ApplicationBarIconRequestRefreshButton_Click(object sender, EventArgs e)
        {
            proxy.GetMessagesAsync(Me.Uid);
        }

        private void ApplicationBarIconAllAcceptButton_Click(object sender, EventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you really like to accept all requests?", "Accept all", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                List<Request> tempRequests = new List<Request>();
                foreach (var item in RequestsListBox.ItemsSource)
                {
                    Request request = item as Request;
                    if (request != null)
                        tempRequests.Add(request);
                }

                foreach (var item in tempRequests)
                {
                    RequestDone(item, true);
                }
            }          
        }

        private void ApplicationBarIconAllRejectButton_Click(object sender, EventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you really like to reject all requests?", "Reject all", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                List<Request> tempRequests = new List<Request>();
                foreach (var item in RequestsListBox.ItemsSource)
                {
                    Request request = item as Request;
                    if (request != null)
                        tempRequests.Add(request);
                }

                foreach (var item in tempRequests)
                {
                    RequestDone(item, false);
                }
            }  
        }

        private void ApplicationBarIconFriendManageRefreshButton_Click(object sender, EventArgs e)
        {
            App.ViewModel.RefreshData();
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((sender as Pivot).SelectedIndex)
            {
                case 0:
                    this.ApplicationBar = this.Resources["AppBar_Home"] as ApplicationBar;
                    break;
                case 1:
                    this.ApplicationBar = this.Resources["AppBar_Invite"] as ApplicationBar;
                    break;
                case 2:
                    this.ApplicationBar = this.Resources["AppBar_Me"] as ApplicationBar;
                    break;
                default:
                    break;
            }
        }

        private void LogOutButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Double check
            MessageBoxResult result =MessageBox.Show("Would you like to Log out?","Log Out", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                App.ViewModel.ClearData();
                this.NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
            }
        }

        private void ApplicationBarIconFriendManageAddGroupButton_Click(object sender, EventArgs e)
        {
            //check group quantity
            //if >=5, no new group
            if (App.ViewModel.GroupInfos.Count >= 5)
            {
                MessageBox.Show("Group quantity is limited to be less than 5.");
            }
            else
            {
                //get group name
                TextBox groupInputBox = new TextBox();
                TiltEffect.SetIsTiltEnabled(groupInputBox, true);
                CustomMessageBox messageBox = new CustomMessageBox()
                {
                    Caption = "Group Name:",
                    Message = "Please input the new group name.",
                    Content = groupInputBox,
                    LeftButtonContent = "Add",
                    RightButtonContent = "Cancel",
                    IsFullScreen = false,
                };

                messageBox.Dismissed += (s1, e1) =>
                {
                    switch (e1.Result)
                    {
                        case CustomMessageBoxResult.LeftButton:
                            string result = "";
                            result = groupInputBox.Text;
                            bool isExisted = false;
                            foreach (var groupinfo in App.ViewModel.GroupInfos)
                            {
                                if (groupinfo.Title.Equals(result))
                                    isExisted = true;
                            }
                            if (result.Length > 0 && !isExisted)  //make sure there is input and the no repeated name
                            //if!=null
                            {
                                proxy.CreateGroupAsync(Me.Uid, result);                                
                            }
                            else
                                MessageBox.Show("Input is empty or there has already been a group with that name.");
                            break;
                        case CustomMessageBoxResult.RightButton:
                            // Do something.
                            break;
                        case CustomMessageBoxResult.None:
                            // Do something.
                            break;
                        default:
                            break;
                    }
                };

                messageBox.Show();
            }
        }

        void proxy_CreateGroupCompleted(object sender, Server.CreateGroupCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result.Contains("ERROR"))
                {
                    MessageBox.Show("Failed to add group.");
                }
                App.ViewModel.RefreshData();
            }
            else
                MessageBox.Show("Server connection error. Please try again later.");
        }

        private void ApplicationBarIconFriendManageRemoveGroupButton_Click(object sender, EventArgs e)
        {
            //get group name
            ListPicker groupListPicker = new ListPicker();
            groupListPicker.ItemsSource = App.ViewModel.Groups;
            TiltEffect.SetIsTiltEnabled(groupListPicker, true);
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = "Select a group:",
                Message = "Please select a group to remove",
                Content = groupListPicker,
                LeftButtonContent = "Remove",
                RightButtonContent = "Cancel",
                IsFullScreen = false,
            };

            messageBox.Dismissing += (s1, e1) =>
            {
                if (groupListPicker.ListPickerMode == ListPickerMode.Expanded)
                {
                    e1.Cancel = true;
                }
            };

            //messageBox.Dismissing += (s1, e2) =>
            //{
            //    if (groupListPicker.ListPickerMode == ListPickerMode.Full)
            //    {
            //        e2.Cancel = true;
            //    }
            //};

            messageBox.Dismissed += (s1, e3) =>
            {
                switch (e3.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        //upgrouped can not be removed
                        if (App.ViewModel.Groups[groupListPicker.SelectedIndex].Title.Equals(DEFAULT_GROUP_NAME))
                        {
                            MessageBox.Show("This group cannot be removed.");
                        }
                        else
                        {
                            //Double check
                            MessageBoxResult result = MessageBox.Show("Would you really like to remove this group?", "Remove group", MessageBoxButton.OKCancel);

                            if (result == MessageBoxResult.OK)
                            {
                                proxy.DeleteGroupAsync(Me.Uid, App.ViewModel.Groups[groupListPicker.SelectedIndex].Title);
                            }                            
                        }
                        break;
                    case CustomMessageBoxResult.RightButton:
                        // Do something.
                        break;
                    case CustomMessageBoxResult.None:
                        // Do something.
                        break;
                    default:
                        break;
                }
            };

            messageBox.Show();
        }

        void proxy_DeleteGroupCompleted(object sender, Server.DeleteGroupCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result.Contains("ERROR"))
                {
                    MessageBox.Show("Failed to remove group.");
                }
                App.ViewModel.RefreshData();
            }
            else
                MessageBox.Show("Server connection error. Please try again later.");
        }

        //void proxy_removeMemberCompleted(object sender, Server.removeMemberCompletedEventArgs e)
        //{
        //    if (e.Error == null)
        //    {
        //        if (e.Result.success == true)
        //        {
        //            App.ViewModel.RefreshData();
        //        }
        //    }
        //}
        
        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}