using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GPSWithFriends.Resources;
using System.Collections.Generic;
using System.Windows;

namespace GPSWithFriends.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        const string ME = "Me";
        const string JUNCTION = "Let's Meet here.";
        const string DEFAULT_GROUP_NAME = "My Friends";

        public MainViewModel()
        {
            this.Friends = new ObservableCollection<Friend>();
            this.Requests = new ObservableCollection<Request>();
            this.GroupInfos = new ObservableCollection<GroupInfo>();
            this.Groups = new ObservableCollection<Group<Friend>>();
            Me = new Friend() { Latitude = 181, Longitude = 181, NickName = ME };
            MeetingPoint = new Friend { Latitude = 181, Longitude = 181, NickName = JUNCTION };

            proxy.GetGroupNamesCompleted += proxy_GetGroupNamesCompleted;
            proxy.GetGroupInfoFromNameCompleted += proxy_GetGroupInfoFromNameCompleted;
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<Friend> Friends { get; private set; }
        public ObservableCollection<Request> Requests { get; private set; }
        public ObservableCollection<Group<Friend>> Groups { get; private set; }
        public ObservableCollection<GroupInfo> GroupInfos { get; private set; }

        Server.UserActionSoapClient proxy = new Server.UserActionSoapClient();

        private Friend currentFriend;
        public Friend CurrentFriend
        {
            get
            {
                return currentFriend;
            }
            set
            {
                if (value != currentFriend)
                {
                    currentFriend = value;
                    NotifyPropertyChanged("CurrentFriend");
                }
            }
        }

        private Friend me;
        public Friend Me
        {
            get
            {
                return me;
            }
            set
            {
                if (value != me)
                {
                    me = value;
                    NotifyPropertyChanged("Me");
                }
            }
        }

        private Friend meetingPoint;
        public Friend MeetingPoint
        {
            get
            {
                return meetingPoint;
            }
            set
            {
                if (value != meetingPoint)
                {
                    meetingPoint = value;
                    NotifyPropertyChanged("MeetingPoint");
                }
            }
        }


        /// <summary>
        /// Sample property that returns a localized string
        /// </summary>
        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.SampleProperty;
            }
        }

        public bool IsDataLoaded
        {
            get;
            set;
        }

        public ObservableCollection<Group<Friend>> GetFriendsGroups()
        {
            return GetItemGroups(Friends, c => c.Group);
        }

        public static ObservableCollection<Group<T>> GetItemGroups<T>(IEnumerable<T> itemList, Func<T, string> getKeyFunc)
        {
            IEnumerable<Group<T>> groupList = from item in itemList
                                              group item by getKeyFunc(item) into g
                                              orderby g.Key
                                              select new Group<T>(g.Key, g);
            
            return groupList.ToObservableCollection();
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            ClearData();

            ///load real data from server
            /// 1. get my groupinfos via my uid
            /// 2. get my friends via group names
            /// 

            // 1 . get my group uid via my uid
            proxy.GetGroupNamesAsync(me.Uid);
            IsDataLoaded = true;
        }

        public void ClearData()
        {
            Friends.Clear();
            Groups.Clear();
            GroupInfos.Clear();
            Requests.Clear();
        }

        void proxy_GetGroupNamesCompleted(object sender, Server.GetGroupNamesCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                foreach (var item in e.Result)
                {
                    // 2. get my friends via group names
                    GroupInfos.Add(new GroupInfo(item));
                    proxy.GetGroupInfoFromNameAsync(me.Uid, item);
                }
            }
            else
                MessageBox.Show("Server connection error and failed to get Group Names. Please try again later.");
        }

        void proxy_GetGroupInfoFromNameCompleted(object sender, Server.GetGroupInfoFromNameCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                foreach (var item in e.Result)
                {
                    Friends.Add(new Friend()
                    {
                        Email = item.email,
                        ImagePath = "/Assets/fakePor.png",
                        IsFriend = true,
                        IsHidden = false,
                        LastMessage = new Message("", "", "", DateTime.Now, true),
                        NickName = item.name,
                        Group = item.group_name
                    });
                    if (item.last_location != null)
                    {
                        Friends.Last().LastUpdateTime = item.last_location.last_time;
                        Friends.Last().Latitude = item.last_location.latitude;
                        Friends.Last().Longitude = item.last_location.longitude;
                    }
                    RefreshGroup();
                }
            }
            else
                MessageBox.Show("Server connection error and failed to get Groups & friends. Please try again later.");
        }

        public void RefreshData()
        {
            IsDataLoaded = false;
            LoadData();
        }

        public void RefreshGroup()
        {
            //get temp groups
            var tempGroup = GetFriendsGroups();

            //add temp to groups
            Groups.Clear();
            foreach (var item in tempGroup)
            {
                Groups.Add(item);
            }

            //synchronize group and groupinfo
            SyncGroupAndGroupInfo();
        }

        private void SyncGroupAndGroupInfo()
        {
            foreach (var groupInfo in GroupInfos)
            {
                groupInfo.FriendEmails = null;
                foreach (var group in Groups)
                {
                    if (groupInfo.Title.Equals(group.Title))
                    {
                        groupInfo.FriendEmails = new string[group.Count];
                        for (int i = 0; i < group.Count; i++)
                        {
                            groupInfo.FriendEmails[i] = group[i].Email.ToString();
                        }
                    }
                }
            }

            //check GroupInfos contains DEFAULT_GROUP_NAME:"My Friends"
            bool isContainedUngrouped = false;
            foreach (var item in GroupInfos)
            {
                if (item.Title.Equals(DEFAULT_GROUP_NAME))
                    isContainedUngrouped = true;
            }
            if (!isContainedUngrouped)
                GroupInfos.Add(new GroupInfo(DEFAULT_GROUP_NAME, null));

            //Add empty froups
            foreach (var groupInfo in GroupInfos)
            {
                bool isContained = false;
                foreach (var group in Groups)
                {
                    if (group.Title.Equals(groupInfo.Title))
                        isContained = true;
                }
                if (!isContained)
                    Groups.Add(new Group<Friend>(groupInfo.Title, null));
            }

            //Put My Friend First!
            //Group<Friend> temp = null;
            //foreach (var group in Groups)
            //{
            //    if (group.Title.Equals(DEFAULT_GROUP_NAME))
            //        temp = group;
            //}
            //if (temp != null)
            //{
            //    Groups.Remove(temp);
            //    Groups.Insert(0, temp);
            //}
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