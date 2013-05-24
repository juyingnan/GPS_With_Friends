using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace GPSWithFriends
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void LOGINBUTTON_Click(object sender, RoutedEventArgs e)
        {
            Server.UserActionSoapClient proxy = new Server.UserActionSoapClient();
            proxy.LogInCompleted += proxy_LogInCompleted;
            proxy.LogInAsync(LOGINEMAILTEXTBOX.Text, LOGINPASSWORDTEXTBOX.Password);
        }

        void proxy_LogInCompleted(object sender, Server.LogInCompletedEventArgs e)
        {
            if (e.Result == "GWF_I_LOGIN_SUCCESS")
                this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            else
                TempResult.Text = e.Result;
        }

        private void REGISTERBUTTON_Click(object sender, RoutedEventArgs e)
        {
            //Server.GWFMessage signUpMessage = new Server.GWFMessage();
            //signUpMessage.type=Server.GWFMessageType.POST;
            //GWF_Services.Entities.UserEntities.User user = new GWF_Services.Entities.UserEntities.User();
            //GWF_Services.Entities.UserEntities.EmailAddress ed = new GWF_Services.Entities.UserEntities.EmailAddress("bunny@microsoft.com");
            //user.emailAccount=ed;
            //user.nickName="Jushen";
            //signUpMessage.content=user;

            string email = "bunny_gg@hotmail.com";
            string nickName = "Bunny";
            string password = "test";

            Server.UserActionSoapClient proxy = new Server.UserActionSoapClient();
            proxy.FastSignUpCompleted += proxy_FastSignUpCompleted;
            try
            {
                proxy.FastSignUpAsync(email, password, nickName);
            }
            catch (Exception)
            {
                TempResult.Text = "姚睿尧傻×";
            }            
        }

        void proxy_FastSignUpCompleted(object sender, Server.FastSignUpCompletedEventArgs e)
        { 
            TempResult.Text = e.Result;            
        }

        //void proxy_SignUpCompleted(object sender, Server.SignUpCompletedEventArgs e)
        //{
        //    Server.GWFMessage result = e.Result;
        //    string temp="";
        //    switch (result.type)
        //    {
        //        case GPSWithFriends.Server.GWFMessageType.DEFAULT_TYPE:
        //            break;
        //        case GPSWithFriends.Server.GWFMessageType.QUERY:
        //            break;
        //        case GPSWithFriends.Server.GWFMessageType.POST:
        //            break;
        //        case GPSWithFriends.Server.GWFMessageType.PUSH:
        //            break;
        //        case GPSWithFriends.Server.GWFMessageType.INFO:
        //            temp = ((GWF_Services.Types.Constants.GWFInfoCode)result.content).ToString();
        //            break;
        //        case GPSWithFriends.Server.GWFMessageType.ERROR:
        //            temp = ((GWF_Services.Types.Constants.GWFErrorCode)result.content).ToString();
        //            break;
        //        default:
        //            break;
        //    }
        //    TempResult.Text = temp;
        //}        
    }
}