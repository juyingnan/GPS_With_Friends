using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GPSWithFriends.ViewModels
{
    public class Request : INotifyPropertyChanged
    {
        private string senderName;
        public string SenderName
        {
            get
            {
                return senderName;
            }
            set
            {
                if (value != senderName)
                {
                    senderName = value;
                    NotifyPropertyChanged("SenderName");
                }
            }
        }

        private string senderEmail;
        public string SenderEmail
        {
            get
            {
                return senderEmail;
            }
            set
            {
                if (value != senderEmail)
                {
                    senderEmail = value;
                    NotifyPropertyChanged("SenderEmail");
                }
            }
        }

        private string content;
        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                if (value != content)
                {
                    content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }

        private string receiverEmail;
        public string ReceiverEmail
        {
            get
            {
                return receiverEmail;
            }
            set
            {
                if (value != receiverEmail)
                {
                    receiverEmail = value;
                    NotifyPropertyChanged("ReceiverEmail");
                }
            }
        }

        private bool isAccepted;
        public bool IsAccepted
        {
            get
            {
                return isAccepted;
            }
            set
            {
                if (value != isAccepted)
                {
                    isAccepted = value;
                    NotifyPropertyChanged("IsAccepted");
                }
            }
        }

        private string time;
        public string Time
        {
            get
            {
                return time;
            }
            set
            {
                if (value != time)
                {
                    time = value;
                    NotifyPropertyChanged("Time");
                }
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}