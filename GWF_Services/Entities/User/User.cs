using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text.RegularExpressions;

using GWF_Services.Entities.User.UserState;

namespace GWF_Services.Entities.User
{
    public class emailAddress 
    {
        public emailAddress(string emailAddr)
        {
            string[] splited = emailAddr.Split('@');
            this.name = splited[0];
            this.provider = splited[1].Split('.')[0];
            this.appendix = splited[1].Substring(splited[1].IndexOf('.'));
        }

        public string name
        {
            get;
            set;
        }

        public string provider
        {
            get;
            set;
        }

        public string appendix
        {
            get;
            set;
        }

        public override string ToString()
        {
            return name + "@" + provider + appendix;
        }
    }
    public class User
    {
        public User()
        {
            currentState = new OfflineState();
        }

        public string uid
        {
            get;
            set;
        }

        public emailAddress emailAccount
        {
            get;
            set;
        }

        public string nickName
        {
            get;
            set;
        }

        public UserState currentState
        {
            get;
            set;
        }
    }
}