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
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }


        CUser IUserService.GetUserFromId(string uid)
        {
            throw new NotImplementedException();
        }

        CUser IUserService.GetUserFromEmail(string email)
        {
            throw new NotImplementedException();
        }

        CUsers IUserService.GetUsers(List<string> uid_list)
        {
            throw new NotImplementedException();
        }

        GWFInfoCode IUserService.DeleteUser(string uid, string passwordMD5)
        {
            throw new NotImplementedException();
        }

        GWFInfoCode IUserService.UpdatePassword(string uid, string passwordMD5)
        {
            throw new NotImplementedException();
        }

        GWFInfoCode IUserService.UpdateNickName(string uid, string nick_name)
        {
            throw new NotImplementedException();
        }

        GWFInfoCode IUserService.SignUp(string email, string passwordMD5)
        {
            throw new NotImplementedException();
        }

        GWFInfoCode IUserService.SignIn(CUser user, string passwordMD5)
        {
            throw new NotImplementedException();
        }

        GWFInfoCode IUserService.SignOff(string uid)
        {
            throw new NotImplementedException();
        }
    }

}
