using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GPSWithFriends.Resources;

namespace GPSWithFriends.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Friends = new ObservableCollection<Friend>();
            this.Requests = new ObservableCollection<Request>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<Friend> Friends { get; private set; }
        public ObservableCollection<Request> Requests { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
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
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            // Sample data; replace with real data
            this.Friends.Add(new Friend() { Name = "Joshua", Status = "UO, 30 seconds ago", Distance = "7287 km", ImagePath = "/Assets/fakePor.png", Email = "Jushua@gmail.com" });
            this.Friends.Add(new Friend() { Name = "Kate", Status = "Beijing, 20 minutes ago", Distance = "1 km", ImagePath = "/Assets/fakePor.png", Email = "Kate@gmail.com" });
            this.Friends.Add(new Friend() { Name = "Bao", Status = "Unreachable", Distance = "???", ImagePath = "/Assets/fakePor.png", Email = "Bao@gmail.com" });
            this.Friends.Add(new Friend() { Name = "Stranger", Status = "Unreachable", Distance = "???", ImagePath = "/Assets/fakePor.png", Email = "Stranger@gmail.com" });

            this.Requests.Add(new Request() { Content = "Yu wants to friend you", Time="8/5/2013 13:04" });
            this.Requests.Add(new Request() { Content = "Hongye wants to friend you", Time = "7/5/2013 12:44" });
            this.Requests.Add(new Request() { Content = "Kevin wants to friend you", Time = "7/5/2013 10:21" });

            this.IsDataLoaded = true;
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