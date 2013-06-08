using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace GPSWithFriends
{
    public partial class RegisterPage : PhoneApplicationPage
    {
        private IsolatedStorageSettings _appSettings = IsolatedStorageSettings.ApplicationSettings;

        Server.GPSwfriendsClient proxy = new Server.GPSwfriendsClient();

        public RegisterPage()
        {
            InitializeComponent();
        }

        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            proxy.registerCompleted += proxy_registerCompleted;
            if (ContentCheck())
            {
                try
                {
                    proxy.registerAsync(RegisterEmailTextBox.Text, RegisterPasswordBox.Password, RegisterNickNameTextBox.Text, RegisterNickNameTextBox.Text);
                    SUBMITBUTTON.IsEnabled = false;
                    progressBar.Visibility = System.Windows.Visibility.Visible;
                }
                catch (Exception)
                {
                    SUBMITBUTTON.IsEnabled = true;
                    progressBar.Visibility = System.Windows.Visibility.Collapsed;
                    MessageBox.Show("Connection failed.");
                }
            }
            else
                MessageBox.Show("Input incomplete or password not match. Please try again.");
        }

        void proxy_registerCompleted(object sender, Server.registerCompletedEventArgs e)
        {
            if (e.Result.success)
            {
                SaveLastLoginUser(RegisterEmailTextBox.Text);
                this.NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show("Register failed. Please try again.");
            }
            SUBMITBUTTON.IsEnabled = true;
            progressBar.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void SaveLastLoginUser(string lastLoginUser)
        {
            if (_appSettings.Contains("LAST_LOGIN_USERNAME"))
                _appSettings["LAST_LOGIN_USERNAME"] = lastLoginUser;
            else
                _appSettings.Add("LAST_LOGIN_USERNAME", lastLoginUser);
        }

        private bool Register(string p1, string p2, string p3)
        {
            return true;
        }

        public bool ContentCheck()
        {
            if (RegisterEmailTextBox.Text.Length > 0 &&
                RegisterNickNameTextBox.Text.Length > 0 &&
                RegisterPasswordBox.Password.Length > 0 &&
                RegisterPasswordBox.Password.Equals(RegisterPasswordAgainBox.Password))
                return true;
            else
                return false;
        }
    }
}