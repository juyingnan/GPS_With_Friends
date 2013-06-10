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
        public GroupInfo(string t, int[] uids)
        {
            Title = t;
            FriendUids = uids;
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

        private int[] friendUids;
        public int[] FriendUids
        {
            get
            {
                return friendUids;
            }
            set
            {
                if (value != friendUids)
                {
                    friendUids = value;
                    NotifyPropertyChanged("FriendUids");
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