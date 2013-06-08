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
    public class Friend : INotifyPropertyChanged
    {
        public Friend()
        {
            nickName = "";
            status = "";
            distance = "";
            email = "";
            isFriend = false;
            imagePath = "";
            latitude = 181;
            longitude = 181;
            if (this.isLocated())
            {
                CalculteGeoCoordinate();
                CalculateDistance();
            }
        }

        public void CalculateDistance()
        {
            double temp;
            if (App.ViewModel.Me.isLocated() && this.isLocated())
            {
                temp = this.Geocoordinate.GetDistanceTo(App.ViewModel.Me.Geocoordinate);
                this.distance = ConvertDoubleToDistance(temp);
            }
        }

        private string ConvertDoubleToDistance(double temp)
        {
            if (temp < 0)
                return "???";
            else if (temp < 1000)
                return ((int)temp).ToString() + " m";
            else return (temp / 1000).ToString("f1") + " km";
        }

        private void CalculteGeoCoordinate()
        {
            if (latitude > -90 && latitude < 90 && longitude > -180 && longitude < 180)
            {
                ///Geocoordinate.Latitude = latitude;
                //Geocoordinate.Longitude = longitude;
                Geocoordinate = new GeoCoordinate(latitude, longitude);
            }
        }

        public bool isLocated()
        {
            if (latitude < 90 && longitude < 180 && latitude > -90 && longitude > -180)
                return true;
            else return false;
        }

        private string nickName;
        public string NickName
        {
            get
            {
                return nickName;
            }
            set
            {
                if (value != nickName)
                {
                    nickName = value;
                    NotifyPropertyChanged("NickName");
                }
            }
        }

        private int uid;
        public int Uid
        {
            get
            {
                return uid;
            }
            set
            {
                if (value != uid)
                {
                    uid = value;
                    NotifyPropertyChanged("Uid");
                }
            }
        }

        private string status;
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                if (value != status)
                {
                    status = value;
                    NotifyPropertyChanged("Status");
                }
            }
        }

        private string distance;
        public string Distance
        {
            get
            {
                return distance;
            }
            set
            {
                if (value != distance)
                {
                    distance = value;
                    NotifyPropertyChanged("Distance");
                }
            }
        }

        private string email;
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                if (value != email)
                {
                    email = value;
                    NotifyPropertyChanged("Email");
                }
            }
        }

        private bool isFriend;
        public bool IsFriend
        {
            get
            {
                return isFriend;
            }
            set
            {
                if (value != isFriend)
                {
                    isFriend = value;
                    NotifyPropertyChanged("IsFriend");
                }
            }
        }

        private string imagePath;
        public string ImagePath
        {
            get
            {
                return imagePath;
            }
            set
            {
                if (value != imagePath)
                {
                    imagePath = value;
                    NotifyPropertyChanged("ImagePath");
                }
            }
        }

        private double latitude;
        public double Latitude
        {
            get
            {
                return latitude;
            }
            set
            {
                if (value != latitude)
                {
                    latitude = value;
                    NotifyPropertyChanged("Latitude");
                    if (this.isLocated())
                    {
                        CalculteGeoCoordinate();
                        CalculateDistance();
                    }
                }
            }
        }

        private double longitude;
        public double Longitude
        {
            get
            {
                return longitude;
            }
            set
            {
                if (value != longitude)
                {
                    longitude = value;
                    NotifyPropertyChanged("Longitude");
                    if (this.isLocated())
                    {
                        CalculteGeoCoordinate();
                        CalculateDistance();
                    }
                }
            }
        }

        private GeoCoordinate geocoordinate;
        [TypeConverter(typeof(GeoCoordinateConverter))]
        public GeoCoordinate Geocoordinate
        {
            get
            {
                return geocoordinate;
            }
            set
            {
                if (value != geocoordinate)
                {
                    geocoordinate = value;
                    NotifyPropertyChanged("Geocoordinate");
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