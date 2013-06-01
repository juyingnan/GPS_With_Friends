using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

using GWF_WebServices.Types;
using GWF_WebServices.Types.Constants;
using GWF_WebServices.Entities.UserEntities;
using GWF_WebServices.Utils.ValidationUtils;
using GWF_WebServices.Models;

//Databasse
using System.Data.SqlClient;
using System.Data.OleDb;

namespace GWF_WebServices
{
    /// <summary>
    /// Summary description for GWF_Services
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class UserAction : System.Web.Services.WebService
    {
        private  Dictionary<String, GWF_User> allUserList = new Dictionary<String, GWF_User>();

        [WebMethod]
        public string LogIn(string email, string password)
        {
            GWF_User user = new GWF_User();

            if (!email.IsValidEmail())
            {
                return GWFInfoCode.GWF_E_INVALID_EMAIL_FORMAT.ToString();
            }

            user.user_email = email;

            if (!user.IsRegistered())
            {
                return GWFInfoCode.GWF_E_LOGIN_ERROR.ToString();
            }

            user.user_passwordMD5 = password;

            if (!user.IsValidPassword())
            {
                return GWFInfoCode.GWF_E_INVALID_EMAIL_PWD_PAIR.ToString();
            }

            user.currentState = new OnlineState();

            // Get user_id from database
            GWF_DBContext ctx = new GWF_DBContext();
            var query = from u in ctx.GWF_User where u.user_email == user.user_email select u.user_id;
            string user_id = query.First();
            this.allUserList.Add(user_id, user);

            return GWFInfoCode.GWF_I_LOGIN_SUCCESS.ToString();
        }

        [WebMethod]
        public string FastSignUp(string email, string password, string nickName)
        {
            //return GWFInfoCode.GWF_E_INVALID_EMAIL_FORMAT.ToString();
            //GWFUser user = new GWFUser();
            GWF_User user = new GWF_User();

            if (!email.IsValidEmail())
            {
                return GWFInfoCode.GWF_E_INVALID_EMAIL_FORMAT.ToString();
            }

            user.user_email= email;
            // TODO Add name validation
            user.user_name = nickName;
            user.user_passwordMD5 = password;

            if (user.IsRegistered())
            {
                return GWFInfoCode.GWF_E_SIGNUP_ERROR.ToString();
            }

            user.user_id = Guid.NewGuid().ToString();

            GWF_DBContext dbContext = new GWF_DBContext();
            dbContext.GWF_User.Add(user);
            dbContext.SaveChanges();

            return GWFInfoCode.GWF_I_SIGNUP_SUCCESS.ToString();
        }

        [WebMethod]
        public string LogOut(string uid)
        {
            GWF_User user = new GWF_User();
            user.user_id = uid;
            GWFMessage userInfoMsg = new GWFMessage();

            if (!this.allUserList.ContainsKey(user.user_id))
            {
                return GWFInfoCode.GWF_E_LOGOUT_ERROR.ToString();
            }
            this.allUserList.Remove(user.user_id);

            return GWFInfoCode.GWF_I_LOGOUT_SUCCESS.ToString();
        }


        [WebMethod]
        public string Follow(string uid)
        {
            GWF_User user = new GWF_User();
            user.user_id = uid;
            GWFMessage userInfoMsg = new GWFMessage();
            userInfoMsg.content = user;
            userInfoMsg.type = GWFMessageType.POST;

            return GWFInfoCode.GWF_I_FOLLOW_SUCCESS.ToString();
        }

        [WebMethod]
        public string UnFollow(string uid)
        {
            GWF_User user = new GWF_User();
            user.user_id = uid;
            GWFMessage userInfoMsg = new GWFMessage();
            userInfoMsg.content = user;
            userInfoMsg.type = GWFMessageType.POST;

            return GWFInfoCode.GWF_I_UNFOLLOW_SUCCESS.ToString();
        }
    }
}
