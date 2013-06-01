using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using GWF_WebServices.Models;

namespace GWF_WebServices.Utils.ValidationUtils
{
    public class UserManager: IUserManager
    {
        public static readonly UserManager instance = new UserManager();

        private Dictionary<string, GWF_User> userList;

        private UserManager()
        {
            this.userList = new Dictionary<string, GWF_User>();
        }

        public void addUser(GWF_User user)
        {
            this.userList.Add(user.user_id, user);
        }

        public GWF_User getUser(string uid)
        {
            return this.userList[uid];
        }

        public bool removeUser(string uid)
        {
            bool success = false;
            if (!this.userExists(uid))
            {
                return success;
            }
            success = true;
            this.userList.Remove(uid);
            return success;
        }

        public bool updateUser(GWF_User user)
        {
            bool success = false;
            if (!this.userExists(user.user_id))
            {
                return success;
            }
            success = true;
            this.userList[user.user_id] = user;
            return success;
        }

        public bool userExists(string uid)
        {
            return this.userList.ContainsKey(uid);
        }

    }
}