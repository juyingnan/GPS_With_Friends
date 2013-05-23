using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GWF_Services.Entities.User
{
    public interface UserState
    {
        void handle();
    }
}