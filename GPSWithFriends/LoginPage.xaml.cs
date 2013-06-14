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
using System.Text;

namespace GPSWithFriends
{
    public partial class LoginPage : PhoneApplicationPage
    {
        const int INPUT_MAX_LENGTH_WITH_TIPS = 10;

        private IsolatedStorageSettings _appSettings = IsolatedStorageSettings.ApplicationSettings;

        //Server.UserActionSoapClient proxy = new Server.UserActionSoapClient("UserActionSoap", "http://gpswithfriends.cloudapp.net:80/UserAction.asmx");
        Server.UserActionSoapClient proxy = new Server.UserActionSoapClient();

        public LoginPage()
        {
            InitializeComponent();
            ReadLastLoginUser();
        }
       
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            while (this.NavigationService.CanGoBack)
            {
                this.NavigationService.RemoveBackEntry();
            }
            ReadLastLoginUser();
        }


        private void LOGINBUTTON_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string md5String = MD5Core.GetHashString(LoginPasswordBox.Password);

                proxy.LogInCompleted += proxy_LogInCompleted;
                proxy.LogInAsync(LoginUsernameTextBox.Text, md5String);

                LOGINBUTTON.IsEnabled = false;
                REGISTERBUTTON.IsEnabled = false;
                progressBar.Visibility = System.Windows.Visibility.Visible;
            }
            catch (Exception)
            {
                MessageBox.Show("Connection Failed");
                LOGINBUTTON.IsEnabled = true;
                REGISTERBUTTON.IsEnabled = true;
                progressBar.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void proxy_LogInCompleted(object sender, Server.LogInCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result.StartsWith("GWF_E")) //login failed
                {
                    MessageBox.Show("Login Failed. Please try again.");
                    LoginPasswordBox.Password = "";
                    LoginPasswordBox.Focus();
                }
                else
                {
                    SaveLastLoginUser(LoginUsernameTextBox.Text);
                    App.ViewModel.Me.Email = LoginUsernameTextBox.Text;
                    string uid = e.Result.Split(':')[1];
                    if (uid != null)
                    {
                        App.ViewModel.Me.Uid = uid;
                        this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                    }
                    else
                        MessageBox.Show("UID error.");
                }
            }
            LOGINBUTTON.IsEnabled = true;
            REGISTERBUTTON.IsEnabled = true;
            progressBar.Visibility = System.Windows.Visibility.Collapsed;
            
        }

        //private void proxy_authenticateCompleted(object sender, Server.authenticateCompletedEventArgs e)
        //{
        //    if (e.Error == null)
        //    {
        //        if (e.Result.success)
        //        {
        //            SaveLastLoginUser(LoginUsernameTextBox.Text);
        //            App.ViewModel.Me.Email = LoginUsernameTextBox.Text;
        //            this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        //        }
        //        else
        //        {
        //            MessageBox.Show("Login Failed. Please try again.");
        //            LoginPasswordBox.Password = "";
        //            LoginPasswordBox.Focus();
        //        }
        //    }
        //    LOGINBUTTON.IsEnabled = true;
        //    REGISTERBUTTON.IsEnabled = true;
        //    progressBar.Visibility = System.Windows.Visibility.Collapsed;
        //}


        private void ReadLastLoginUser()
        {
            //READ LAST LOGIN USER NAME
            string tempLastLoginUserName;

            if (_appSettings.Count > 0 && _appSettings.Contains("LAST_LOGIN_USERNAME"))
            {
                if (_appSettings.TryGetValue<string>("LAST_LOGIN_USERNAME", out tempLastLoginUserName))
                    LoginUsernameTextBox.Text = tempLastLoginUserName;
                else LoginUsernameTextBox.Text = "";
            }
            else
                LoginUsernameTextBox.Text = "";
        }

        private void SaveLastLoginUser(string lastLoginUser)
        {
            if (_appSettings.Contains("LAST_LOGIN_USERNAME"))
                _appSettings["LAST_LOGIN_USERNAME"] = lastLoginUser;
            else
                _appSettings.Add("LAST_LOGIN_USERNAME", lastLoginUser);
        }

        private void REGISTERBUTTON_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/RegisterPage.xaml", UriKind.Relative));
        }

        private void LoginUsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text.Length < INPUT_MAX_LENGTH_WITH_TIPS)
                EmailInputTooltipTextBlock.Visibility = System.Windows.Visibility.Visible;
            else EmailInputTooltipTextBlock.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void LoginPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if ((sender as PasswordBox).Password.Length < INPUT_MAX_LENGTH_WITH_TIPS)
                PasswordInputTooltipTextBlock.Visibility = System.Windows.Visibility.Visible;
            else PasswordInputTooltipTextBlock.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}