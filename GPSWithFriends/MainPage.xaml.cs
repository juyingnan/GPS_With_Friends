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


namespace GPSWithFriends
{
    public partial class MainPage : PhoneApplicationPage
    {
        //GPS
        Geolocator myGeoLocator = new Geolocator();
        Friend Me = App.ViewModel.Me;
        //for Route Query
        RouteQuery MyQuery = null;
        MapRoute MyMapRoute = null;

        Server.GPSwfriendsClient proxy = new Server.GPSwfriendsClient();
        
        // Constructor
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
            myGeoLocator.MovementThreshold = 50;

            //map extension controls data binding
            this.FriendsLocationMarkerList.ItemsSource = App.ViewModel.Friends;
            MyLocationMarker.DataContext = this.Me; 
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
                GPSLocationTextblock.Text = "Location: " + string.Format("Lat: {0:0.0000}, Long: {1:0.0000}, Acc: {2}m",
                 position.Coordinate.Latitude, position.Coordinate.Longitude, position.Coordinate.Accuracy);
                Me.Latitude = position.Coordinate.Latitude;
                Me.Longitude = position.Coordinate.Longitude;
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
                            //proxy.setLocationCompleted += proxy_setLocationCompleted;
                            proxy.setLocationAsync(Me.Uid, Me.Latitude, Me.Longitude);
                        }
                        catch (TimeoutException e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    }
                    MyLocationMarker.Visibility = System.Windows.Visibility.Visible;
                    MyMap.SetView(new GeoCoordinate(Me.Latitude, Me.Longitude), MyMap.ZoomLevel, MapAnimationKind.Parabolic);
                }
            }
        }

        //void proxy_setLocationCompleted(object sender, Server.setLocationCompletedEventArgs e)
        //{
        //    //throw new NotImplementedException();
        //}

        /// <summary>
        /// Click a Listbox Item and the map will focus on that friend
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FriendsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //get index
            ListBox listbox = sender as ListBox;
            int index = listbox.SelectedIndex;
            if (index > -1 && index < App.ViewModel.Friends.Count)
            {
                Friend friend = App.ViewModel.Friends[index];
                if (friend.isLocated())
                {
                    MyMap.SetView(new GeoCoordinate(friend.Latitude, friend.Longitude), MyMap.ZoomLevel, MapAnimationKind.Parabolic);
                }
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
                        result = emailInputBox.Text;
                        if (result.Length > 0 && !result.Equals(Me.Email))  //make sure there is input and the address doesn't belong to ME
                            SendFriendRequest(result);
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

        /// <summary>
        /// Send the email address of Add friend request
        /// </summary>
        /// <param name="email">email of the friend that you want to add</param>
        public void SendFriendRequest(string email)
        {
            // 1. get uid
            // 2. add the friend into group

            // 1. get uid
            proxy.getUserCompleted += proxy_getUserCompleted;
            try
            {
                proxy.getUserAsync(email);
            }
            catch (Exception)
            {
            }
        }

        void proxy_getUserCompleted(object sender, Server.getUserCompletedEventArgs e)
        {
            // 1 continued
            if (e.Error == null)
            {
                proxy.addMemberCompleted += proxy_addMemberCompleted;
                try
                {
                    // 2. add the friend into group
                    proxy.addMemberAsync(e.Result.uid, App.ViewModel.CurrentGroup.Gid);
                }
                catch (Exception)
                {
                }
            }
        }

        void proxy_addMemberCompleted(object sender, Server.addMemberCompletedEventArgs e)
        {
            // 2 continued
            if (e.Error == null)
            {
                if (e.Result.success == true)
                {
                    //refresh to get the latest data
                    App.ViewModel.RefreshData();
                }
            }
        }

        /// <summary>
        /// Select a request to handle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RequestsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listbox = sender as ListBox;
            int index = listbox.SelectedIndex;            
            if (index > -1 && index < App.ViewModel.Requests.Count)
            {
                Request request = App.ViewModel.Requests[index];
                RequestHandle(request);
            }
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
                //2. get friend from cloud
                //3. add friend

                //temp
                App.ViewModel.Friends.Add(new Friend
                {
                    IsFriend = true,
                    Email = request.SenderEmail,
                    NickName = request.SenderName,
                    Distance="???",
                    ImagePath = "/Assets/fakePor.png"
                });
                //throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Select a friend to see detail page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FriendsManageListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //get index
            ListBox listbox = sender as ListBox;
            int index = listbox.SelectedIndex;
            if (index > -1 && index < App.ViewModel.Friends.Count)
            {
                App.ViewModel.CurrentFriend = App.ViewModel.AllGroup.Friends[index];
                this.NavigationService.Navigate(new Uri("/DetailPage.xaml", UriKind.Relative));
            }
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
                this.MyMap.SetView(locationRectangle, new Thickness(10));
            }
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
            if (Me.isLocated()) temp.Add(Me);

            LocationRectangle locationRectangle = LocationRectangle.CreateBoundingRectangle(from Friend in temp select Friend.Geocoordinate);

            this.MyMap.SetView(locationRectangle, new Thickness(20));
        }

        /// <summary>
        /// Zoom in the map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {   
            //make sure zoomlevel <=19
            if (MyMap.ZoomLevel <=18)
                MyMap.ZoomLevel += 1;
            MyMap.SetView(MyMap.Center, MyMap.ZoomLevel, MapAnimationKind.Linear);
        }

        /// <summary>
        /// zoom out the map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Minus_Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //make sure zoomlevel >=1
            if (MyMap.ZoomLevel >= 2)
                MyMap.ZoomLevel -= 1;
            MyMap.SetView(MyMap.Center, MyMap.ZoomLevel, MapAnimationKind.Linear);
        }

        /// <summary>
        /// Switch group to next
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarIconSwitchGroupButton_Click(object sender, EventArgs e)
        {
            //get index
            int index = App.ViewModel.Groups.IndexOf(App.ViewModel.CurrentGroup);
            //make sure the index++ is within the bound
            if (index + 1 < App.ViewModel.Groups.Count)
            {
                index++;                
            }
            //last group -> 0
            else if (index + 1 == App.ViewModel.Groups.Count)
            {
                index = 0;
            }

            //group switch
            App.ViewModel.CurrentGroup = App.ViewModel.Groups[index];
            App.ViewModel.Friends.Clear();
            foreach (var item in App.ViewModel.CurrentGroup.Friends)
            {
                if (item != null)
                    App.ViewModel.Friends.Add(item);
            }
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

        /// <summary>
        /// remove a friend from the group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveFromGroupItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //get index
                int selectedIndex = App.ViewModel.Friends.IndexOf((sender as MenuItem).DataContext as Friend);
                proxy.removeMemberCompleted += proxy_removeMemberCompleted;
                //remove via uid and gid
                proxy.removeMemberAsync(App.ViewModel.Friends[selectedIndex].Uid, App.ViewModel.CurrentGroup.Gid);
            }
            catch (Exception)
            {
                MessageBox.Show("Something gotta wrong.");
            }
        }

        void proxy_removeMemberCompleted(object sender, Server.removeMemberCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result.success == true)
                {
                    App.ViewModel.RefreshData();
                }
            }
        }
        
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