using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GPSWithFriends.ViewModels;

namespace GPSWithFriends
{
    public partial class DetailPage : PhoneApplicationPage
    {
        Server.UserActionSoapClient proxy = new Server.UserActionSoapClient();

        bool isStillFriend = false;

        public DetailPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel.CurrentFriend;
            //set group list picker
            GroupListPicker.ItemsSource = App.ViewModel.Groups;
            foreach (var item in App.ViewModel.Groups)
            {
                //find the group of currentFriend
                if (item.Contains(App.ViewModel.CurrentFriend))
                {
                    GroupListPicker.SelectedItem = item;
                    break;
                }
            }
            isStillFriend = App.ViewModel.CurrentFriend.IsFriend;
            proxy.AddFriendRequestCompleted += proxy_AddFriendRequestCompleted;
            proxy.DeleteFriendCompleted += proxy_DeleteFriendCompleted;
            proxy.ChangeUserGroupCompleted += proxy_ChangeUserGroupCompleted;
        }


        private void AddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;

            //send the friend request
            try
            {
                proxy.AddFriendRequestAsync(App.ViewModel.Me.Uid, App.ViewModel.CurrentFriend.Email);
            }
            catch (Exception)
            {

            }
        }

        void proxy_AddFriendRequestCompleted(object sender, Server.AddFriendRequestCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result.Contains("ERROR"))
                {
                    MessageBox.Show("Add friend request failed. Please try again.");
                }
                else
                {
                    isStillFriend = false;
                    this.NavigationService.GoBack();
                }
            }
            else
                MessageBox.Show("Server connection error. Please try again later.");
            AddFriendButton.IsEnabled = true;
        }

        private void RemoveFriendButton_Click(object sender, RoutedEventArgs e)
        {
            //Double check
            MessageBoxResult result = MessageBox.Show("Would you really like to remove " + App.ViewModel.CurrentFriend.NickName + "?", "Remove friend", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                Button button = sender as Button;
                button.IsEnabled = false;

                //send the remove friend request
                try
                {
                    proxy.DeleteFriendAsync(App.ViewModel.Me.Uid,
                        App.ViewModel.CurrentFriend.Email,
                        App.ViewModel.CurrentFriend.Group);
                }
                catch (Exception)
                {

                }
            }
        }

        void proxy_DeleteFriendCompleted(object sender, Server.DeleteFriendCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result.Contains("ERROR"))
                {
                    MessageBox.Show("Remove friend request failed. Please try again.");
                }
                else
                {
                    App.ViewModel.RefreshData();
                    isStillFriend = false;
                    this.NavigationService.GoBack();
                }
            }
            else
                MessageBox.Show("Server connection error. Please try again later.");
            RemoveFriendButton.IsEnabled = true;
        }


        private void GroupListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                if (isStillFriend)
                {
                    if (!App.ViewModel.CurrentFriend.Group.Equals(App.ViewModel.Groups[GroupListPicker.SelectedIndex].Title))
                        proxy.ChangeUserGroupAsync(App.ViewModel.Me.Uid,
                            App.ViewModel.CurrentFriend.Group,
                            App.ViewModel.Groups[GroupListPicker.SelectedIndex].Title,
                            App.ViewModel.CurrentFriend.Email);
                }
            }
        }

        void proxy_ChangeUserGroupCompleted(object sender, Server.ChangeUserGroupCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result.Contains("ERROR"))
                {
                    MessageBox.Show("Remove friend request failed. Please try again.");
                }
                App.ViewModel.RefreshData();
            }
            else
                MessageBox.Show("Server connection error. Please try again later.");
        }
    }
}