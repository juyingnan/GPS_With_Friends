using Microsoft.Phone.Maps.Controls;
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
    public class Group : INotifyPropertyChanged
    {
       public Group()
        {
        }

        private int gid;
        public int Gid
        {
            get
            {
                return gid;
            }
            set
            {
                if (value != gid)
                {
                    gid = value;
                    NotifyPropertyChanged("Gid");
                }
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value != name)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        private int owner;
        public int Owner
        {
            get
            {
                return owner;
            }
            set
            {
                if (value != owner)
                {
                    owner = value;
                    NotifyPropertyChanged("Owner");
                }
            }
        }

        private Friend[] friends;
        public Friend[] Friends
        {
            get
            {
                return friends;
            }
            set
            {
                if (value != friends)
                {
                    friends = value;
                    NotifyPropertyChanged("Friends");
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