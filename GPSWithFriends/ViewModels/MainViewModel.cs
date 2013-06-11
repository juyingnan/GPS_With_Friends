using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GPSWithFriends.Resources;
using System.Collections.Generic;

namespace GPSWithFriends.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        const string ME = "Me";
        const string DEFAULT_GROUP_NAME = "My Friends";

        public MainViewModel()
        {
            this.Friends = new ObservableCollection<Friend>();
            this.Requests = new ObservableCollection<Request>();
            this.GroupInfos = new ObservableCollection<GroupInfo>();
            this.Groups = new ObservableCollection<Group<Friend>>();
            Me = new Friend() { Latitude = 181, Longitude = 181, NickName = ME };
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<Friend> Friends { get; private set; }
        public ObservableCollection<Request> Requests { get; private set; }
        public ObservableCollection<Group<Friend>> Groups { get; private set; }
        public ObservableCollection<GroupInfo> GroupInfos { get; private set; }

        //Server.GPSwfriendsClient proxy = new Server.GPSwfriendsClient();

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
            // Sample data; replace with real data
            this.Friends.Add(new Friend() { NickName = "Wang Cong", Status = "updated in 16:20", Distance = "1.5 km", ImagePath = "/Assets/fakePor.png", Email = "Jushua@gmail.com", Latitude = 39.7677, Longitude = 116.3602,IsFriend=true,Uid=1, LastMessage=new Message(1,0,"Hi, I'm here",DateTime.Now,false)});
            this.Friends.Add(new Friend() { NickName = "Yu Zhe", Status = "updated in 16:22", Distance = "1 km", ImagePath = "/Assets/fakePor.png", Email = "Kate@gmail.com", Latitude = 39.7588, Longitude = 116.3510, IsFriend = true, Uid = 2, LastMessage = new Message(1, 0, "Hi, I'm here", DateTime.Now, true) });
            this.Friends.Add(new Friend() { NickName = "Kate.Xu", Status = "updated in 15:30", Distance = "1.3 km", ImagePath = "/Assets/fakePor.png", Email = "Bao@gmail.com", Latitude = 39.7532, Longitude = 116.3452, IsFriend = true, Uid = 3, LastMessage= new Message(1, 0, "Hi, I'm here", DateTime.Now, false) });
            this.Friends.Add(new Friend() { NickName = "Rye", Status = "updated in 16:12", Distance = "3 km", ImagePath = "/Assets/fakePor.png", Email = "Stranger@gmail.com", Latitude = 39.7532, Longitude = 116.3602, IsFriend = true, Uid = 4, LastMessage = new Message(1, 0, "Hi, I'm here", DateTime.Now, true) });
            
            this.Requests.Add(new Request() { Content = "Yu wants to friend you", Time = "8/5/2013 13:04", SenderName = "Yu", SenderEmail = "Yu@163.com" });
            this.Requests.Add(new Request() { Content = "Hongye wants to friend you", Time = "7/5/2013 12:44", SenderName = "Hongye", SenderEmail = "Hongye@163.com" });
            this.Requests.Add(new Request() { Content = "Kevin wants to friend you", Time = "7/5/2013 10:21", SenderName = "Kevin", SenderEmail = "Kevin@163.com" });

            //get group info from web
            GroupInfos.Add(new GroupInfo(DEFAULT_GROUP_NAME, new int[] { 1, 2 }));
            GroupInfos.Add(new GroupInfo("Classmates", new int[] { 3 }));
            GroupInfos.Add(new GroupInfo("Other", new int[] { 4 }));
            GroupInfos.Add(new GroupInfo("Test", null));

            //set group to friends
            foreach (var friend in Friends)
            {
                foreach (var groupInfo in GroupInfos)
                {
                    if (groupInfo.FriendUids != null)
                    {
                        if (groupInfo.FriendUids.Contains(friend.Uid))
                        {
                            friend.Group = groupInfo.Title;
                            break;
                        }
                    }
                }
            }            

            //get group for longlistselector           
            RefreshGroup();

            IsDataLoaded = true;

            ///load real data from server
            /// 1. get my uid
            /// 2. get my groups via my uid
            /// 3. for each group, get friends
            /// 4. set currentFriends & currentGroup
            /// 5. Set AllGroup

            // 1. get my uid
            //proxy.getUserCompleted += proxy_getUserCompleted;
            try
            {
                //proxy.getUserAsync(Me.Email);
            }
            catch (Exception)
            {
            }
        }

        //void proxy_getUserCompleted(object sender, Server.getUserCompletedEventArgs e)
        //{
        //    // 1 continued
        //    if (e.Error == null)
        //    {
        //        Me.Uid = e.Result.uid;
        //        // 2. get my groups via my uid
        //        //proxy.getGroupsCompleted += proxy_getGroupsCompleted;
        //        try
        //        {
        //            //proxy.getGroupsAsync(Me.Uid);
        //        }
        //        catch (Exception)
        //        {
        //        }
        //    }
        //}

        //void proxy_getGroupsCompleted(object sender, Server.getGroupsCompletedEventArgs e)
        //{
        //    // 2 continued
        //    if (e.Error == null && e.Result != null)
        //    {

        //        Groups.Clear();
        //        // 3. for each group, get friends
        //        foreach (var item in e.Result)
        //        {
        //            Friend[] tempFriends = null;
        //            if (item.users.Length > 0)
        //            {
        //                tempFriends = new Friend[item.users.Length];
        //                for (int i = 0; i < item.users.Length; i++)
        //                {
        //                    if (item.users[i].uid != Me.Uid)
        //                    {
        //                        tempFriends[i] = new Friend()
        //                        {
        //                            NickName = item.users[i].fName,
        //                            Uid = item.users[i].uid,
        //                            Status = "Last updated at " + item.users[i].lastLoc.date,
        //                            IsFriend = true,
        //                            ImagePath = "/Assets/fakePor.png",
        //                            Latitude = item.users[i].lastLoc.latitude,
        //                            Longitude = item.users[i].lastLoc.longitude
        //                        };
        //                    }
        //                }
        //            }
        //            Groups.Add(new Group() { Gid = item.gid, Name = item.name, Owner = item.owner, Friends = tempFriends });
        //        }

        //        // 4. set Friends for Current Group
        //        if (Groups.Count > 0)
        //        {
        //            CurrentGroup = this.Groups[0];
        //            Friends.Clear();
        //            foreach (var item in CurrentGroup.Friends)
        //            {
        //                if (item != null)
        //                    Friends.Add(item);
        //            }
        //        }

        //        // 5. Set AllGroup
        //        List<Friend> tempAll = new List<Friend>();
        //        foreach (var item in Groups)
        //        {
        //            foreach (var user in item.Friends)
        //            {
        //                if (user != null)
        //                {
        //                    bool isContained = false;
        //                    foreach (var u in tempAll)
        //                    {
        //                        if (u.Uid == user.Uid)
        //                            isContained = true;
        //                    }
        //                    if (!isContained)
        //                        tempAll.Add(user);
        //                }
        //            }
        //        }

        //        Friend[] allFriend = new Friend[tempAll.Count];
        //        for (int i = 0; i < tempAll.Count; i++)
        //        {
        //            allFriend[i] = tempAll[i];
        //        }
        //        AllGroup.Friends = allFriend;

        //        this.IsDataLoaded = true;
        //    }
        //}

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
                groupInfo.FriendUids = null;
                foreach (var group in Groups)
                {
                    if (groupInfo.Title.Equals(group.Title))
                    {
                        groupInfo.FriendUids = new int[group.Count];
                        for (int i = 0; i < group.Count; i++)
                        {
                            groupInfo.FriendUids[i] = group[i].Uid;
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
            Group<Friend> temp = null;
            foreach (var group in Groups)
            {
                if (group.Title.Equals(DEFAULT_GROUP_NAME))
                    temp = group;
            }
            if (temp != null)
            {
                Groups.Remove(temp);
                Groups.Insert(0, temp);
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