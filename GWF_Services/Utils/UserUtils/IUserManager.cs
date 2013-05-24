﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GWF_Services.Entities.UserEntities;

namespace GWF_Services.Utils.UserUtils
{
    interface IUserManager
    {
        void addUser(GWFUser user);
        GWFUser getUser(String uid);
        bool removeUser(String uid);
        bool updateUser(GWFUser user);
        bool userExists(string uid);
    }
}
