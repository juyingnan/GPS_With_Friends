using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using GWF_Services.Entities.UserEntities;

namespace GWF_Services.Utils.UserUtils
{
    public class OnlineUserManager: IUserManager
    {
        private UserManager allUserList;
        private ArrayList onlineUserList;

        private OnlineUserManager()
        {
            this.allUserList = UserManager.instance;
            this.onlineUserList = new ArrayList();
        }

        public static readonly OnlineUserManager instance = new OnlineUserManager();

        public void addUser(User user)
        {
            this.onlineUserList.Add(user.uid);
        }

        public User getUser(string uid)
        {
            return this.allUserList.getUser(uid);
        }

        public bool removeUser(string uid)
        {
            if (!this.userExists(uid))
            {
                return false;
            }
            this.onlineUserList.Remove(uid);
            return true;
        }

        public bool updateUser(User user)
        {
            // Do nothing;
            return true;
        }

        public bool userExists(string uid)
        {
            if (!this.allUserList.userExists(uid))
            {
                return false;
            }

            return this.onlineUserList.Contains(uid);
        }
    }
}