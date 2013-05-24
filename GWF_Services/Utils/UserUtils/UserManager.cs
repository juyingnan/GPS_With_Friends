using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using GWF_Services.Entities.UserEntities;

namespace GWF_Services.Utils.UserUtils
{
    public class UserManager: IUserManager
    {
        public static readonly UserManager instance = new UserManager();

        private Dictionary<string, GWFUser> userList;

        private UserManager()
        {
            this.userList = new Dictionary<string, GWFUser>();
        }

        public void addUser(GWFUser user)
        {
            this.userList.Add(user.uid, user);
        }

        public GWFUser getUser(string uid)
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

        public bool updateUser(GWFUser user)
        {
            bool success = false;
            if (!this.userExists(user.uid))
            {
                return success;
            }
            success = true;
            this.userList[user.uid] = user;
            return success;
        }

        public bool userExists(string uid)
        {
            return this.userList.ContainsKey(uid);
        }

    }
}