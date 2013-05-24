using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text.RegularExpressions;

using GWF_Services.Entities.UserEntities;

namespace GWF_Services.Entities.UserEntities
{
    public class EmailAddress 
    {
        public EmailAddress()
        {
        }
        public EmailAddress(string emailAddr)
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
    public class GWFUser
    {
        public GWFUser()
        {
            currentState = new OfflineState();
        }

        public string uid
        {
            get;
            set;
        }

        public string passwordMD5
        {
            get;
            set;
        }

        public EmailAddress emailAccount
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