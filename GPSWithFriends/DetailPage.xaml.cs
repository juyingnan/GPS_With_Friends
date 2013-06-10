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
        }

        private void AddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;

            //send the friend request

            //test
            App.ViewModel.Friends.Add(App.ViewModel.CurrentFriend);
            App.ViewModel.CurrentFriend.IsFriend = true;
            App.ViewModel.RefreshGroup();

            this.NavigationService.GoBack();
            //MainPage.SendFriendRequest(App.ViewModel.CurrentFriend.Email);
        }

        private void RemoveFriendButton_Click(object sender, RoutedEventArgs e)
        {
            //Double check
            MessageBoxResult result = MessageBox.Show("Would you really like to remove "+App.ViewModel.CurrentFriend.NickName+"?", "Remove friend", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                Button button = sender as Button;
                button.IsEnabled = false;
                //send unfriend request

                //test
                App.ViewModel.Friends.Remove(App.ViewModel.CurrentFriend);
                App.ViewModel.CurrentFriend.IsFriend = false;
                App.ViewModel.RefreshGroup();

                this.NavigationService.GoBack();
            }            
        }

        private void GroupListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
           if (e.NavigationMode==NavigationMode.Back)
            {
                App.ViewModel.CurrentFriend.Group = App.ViewModel.Groups[GroupListPicker.SelectedIndex].Title;
                App.ViewModel.RefreshGroup();
            }
        }
    }
}