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
        public Message(int fuid, int tuid, string content, DateTime time, bool iscleared)
        {
            FromUid = fuid;
            ToUid = tuid;
            Content = content;
            LastUpdateTime = time;
            IsCleared = iscleared;
        }

        public Message()
        {
            Content = "";
            IsCleared = true;
        }     

        private int fromUid;
        public int FromUid
        {
            get
            {
                return fromUid;
            }
            set
            {
                if (value != fromUid)
                {
                    fromUid = value;
                    NotifyPropertyChanged("FromUid");
                }
            }
        }

        private int toUid;
        public int ToUid
        {
            get
            {
                return toUid;
            }
            set
            {
                if (value != toUid)
                {
                    toUid = value;
                    NotifyPropertyChanged("ToUid");
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