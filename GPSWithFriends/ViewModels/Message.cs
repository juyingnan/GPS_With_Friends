using Microsoft.Phone.Maps.Controls;
using System;
using System.ComponentModel;
using System.Device.Location;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GPSWithFriends.ViewModels
{
    public class Message : INotifyPropertyChanged
    {
        public Message(string femail, string temail, string content, DateTime time, bool iscleared)
        {
            FromEmail = femail;
            ToEmail = temail;
            Content = content;
            LastUpdateTime = time;
            IsCleared = iscleared;
        }

        public Message()
        {
            Content = "";
            IsCleared = true;
        }     

        private string fromEmail;
        public string FromEmail
        {
            get
            {
                return fromEmail;
            }
            set
            {
                if (value != fromEmail)
                {
                    fromEmail = value;
                    NotifyPropertyChanged("FromEmail");
                }
            }
        }

        private string toEmail;
        public string ToEmail
        {
            get
            {
                return toEmail;
            }
            set
            {
                if (value != toEmail)
                {
                    toEmail = value;
                    NotifyPropertyChanged("ToEmail");
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

        private DateTime lastUpdateTime;
        public DateTime LastUpdateTime
        {
            get
            {
                return lastUpdateTime;
            }
            set
            {
                if (value != lastUpdateTime)
                {
                    lastUpdateTime = value;
                    NotifyPropertyChanged("LastUpdateTime");
                }
            }
        }

        private bool isCleared=true;
        public bool IsCleared
        {
            get
            {
                return isCleared;
            }
            set
            {
                if (value != isCleared)
                {
                    isCleared = value;
                    NotifyPropertyChanged("IsCleared");
                }
            }
        }

        //public override string ToString()
        //{
        //    return this.Content;
        //}

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