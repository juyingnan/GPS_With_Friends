using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using GWF_WCF_WebRole.Services.Interfaces;
using GWF_WCF_WebRole.Contracts;

namespace GWF_WCF_WebRole
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "UserService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select UserService.svc or UserService.svc.cs at the Solution Explorer and start debugging.
    public class UserService : IUserService
    {
        public CUser GetUser(string uid)
        {
            throw new NotImplementedException();
        }

        public CUsers GetUsers(List<string> uid_list)
        {
            throw new NotImplementedException();
        }

        public GWFInfoCode DeleteUser(string uid, string passwordMD5)
        {
            throw new NotImplementedException();
        }

        public GWFInfoCode UpdatePassword(string uid, string passwordMD5)
        {
            throw new NotImplementedException();
        }

        public GWFInfoCode UpdateNickName(string uid, string nick_name)
        {
            throw new NotImplementedException();
        }

        public GWFInfoCode SignUp(string email, string passwordMD5)
        {
            throw new NotImplementedException();
        }

        public GWFInfoCode SignIn(string email, string passwordMD5)
        {
            throw new NotImplementedException();
        }

        public GWFInfoCode SignOff(string uid)
        {
            throw new NotImplementedException();
        }
    }

}
