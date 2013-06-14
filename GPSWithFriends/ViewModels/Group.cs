using Microsoft.Phone.Maps.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class Group<T> : ObservableCollection<T>
    {
        public Group(string name, IEnumerable<T> items)
        {
            this.Title = name;
            if (items != null)
            {
                foreach (T item in items)
                {
                    this.Add(item);
                }
            }
        }

        //public override bool Equals(object obj)
        //{
        //    Group<T> that = obj as Group<T>;

        //    return (that != null) && (this.Title.Equals(that.Title));
        //}

        public string Title
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Title;
        }
    }

    public static class EnumerableExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            ObservableCollection<T> observableCollection = new ObservableCollection<T>();
            foreach (T item in collection)
            {
                observableCollection.Add(item);
            }

            return observableCollection;
        }
    }

    public class GroupInfo:INotifyPropertyChanged
    {
        public GroupInfo(string t)
        {
            Title = t;
        }
        public GroupInfo(string t, string[] uids)
        {
            Title = t;
            FriendEmails = uids;
        }


        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (value != title)
                {
                    title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private bool isLoaded;
        public bool IsLoaded
        {
            get
            {
                return isLoaded;
            }
            set
            {
                if (value != isLoaded)
                {
                    isLoaded = value;
                    NotifyPropertyChanged("IsLoaded");
                }
            }
        }

        private string[] friendEmails;
        public string[] FriendEmails
        {
            get
            {
                return friendEmails;
            }
            set
            {
                if (value != friendEmails)
                {
                    friendEmails = value;
                    NotifyPropertyChanged("FriendEmails");
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