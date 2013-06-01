using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GWF_WebServices.Models;

namespace GWF_WebServices.Utils.ValidationUtils
{
    interface IUserManager
    {
        void addUser(GWF_User user);
        GWF_User getUser(String uid);
        bool removeUser(String uid);
        bool updateUser(GWF_User user);
        bool userExists(string uid);
    }
}
