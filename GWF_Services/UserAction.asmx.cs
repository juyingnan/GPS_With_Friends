using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

using GWF_Services.Types;
using GWF_Services.Types.Constants;
using GWF_Services.Entities.UserEntities;
using GWF_Services.Utils.UserUtils;

//Databasse
using System.Data.SqlClient;
using System.Data.OleDb;

namespace GWF_Services
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
        private  Dictionary<String, GWFUser> allUserList = new Dictionary<String, GWFUser>();

        //[WebMethod]
        //public string Declaration(User user)
        ////public string Declaration(User user, OnlineState onlineState, OfflineState offlineState, HiddenState hiddenState)
        //{
        //    return "Don not use this fucking method!";
        //}

        [WebMethod]
        public string LogIn(string email, string password)
        {
            GWFUser user = new GWFUser();
            
            EmailRegexUtils emailVal = new EmailRegexUtils();
            if (!emailVal.IsValidEmail(email))
            {
                return GWFInfoCode.GWF_E_INVALID_EMAIL_FORMAT.ToString();
            }
            user.emailAccount = new EmailAddress(email);
            user.passwordMD5 = password;

            GWFMessage userInfoMsg = new GWFMessage();
            userInfoMsg.content = user;
            userInfoMsg.type = GWFMessageType.POST;

            GWFMessage resp = this.LogIn(userInfoMsg);
            string retStr = ((GWFInfoCode)(resp.content)).ToString();

            return retStr;
        }

        [WebMethod]
        public string FastSignUp(string email, string password, string nickName)
        {
            //return GWFInfoCode.GWF_E_INVALID_EMAIL_FORMAT.ToString();
            GWFUser user = new GWFUser();

            EmailRegexUtils emailVal = new EmailRegexUtils();
            if (!emailVal.IsValidEmail(email))
            {
                return GWFInfoCode.GWF_E_INVALID_EMAIL_FORMAT.ToString();
            }
            user.emailAccount = new EmailAddress(email);
            // TODO Add name validation
            user.nickName = nickName;
            user.passwordMD5 = password;

            GWFMessage userInfoMsg = new GWFMessage();
            userInfoMsg.type = GWFMessageType.POST;
            userInfoMsg.content = user;

            GWFMessage resp = this.SignUp(userInfoMsg);
            string retStr = ((GWFInfoCode)(resp.content)).ToString();

            return retStr;
        }

        [WebMethod]
        public string LogOut(string uid)
        {
            GWFUser user = new GWFUser();
            user.uid = uid;
            GWFMessage userInfoMsg = new GWFMessage();
            userInfoMsg.content = user;
            userInfoMsg.type = GWFMessageType.POST;

            GWFMessage resp = this.LogOut(userInfoMsg);
            string retStr = ((GWFInfoCode)(resp.content)).ToString();

            return retStr;
        }


        [WebMethod]
        public string Follow(string uid)
        {
            GWFUser user = new GWFUser();
            user.uid = uid;
            GWFMessage userInfoMsg = new GWFMessage();
            userInfoMsg.content = user;
            userInfoMsg.type = GWFMessageType.POST;

            GWFMessage resp = this.Follow(userInfoMsg);
            string retStr = ((GWFInfoCode)(resp.content)).ToString();

            return retStr;

        }

        [WebMethod]
        public string UnFollow(string uid)
        {
            GWFUser user = new GWFUser();
            user.uid = uid;
            GWFMessage userInfoMsg = new GWFMessage();
            userInfoMsg.content = user;
            userInfoMsg.type = GWFMessageType.POST;

            GWFMessage resp = this.UnFollow(userInfoMsg);
            string retStr = ((GWFInfoCode)(resp.content)).ToString();

            return retStr;
        }

        public GWFMessage LogIn(GWFMessage userInfo)
        {
            GWFUser user = (GWFUser)userInfo.content;
            if (!this.isRegistered(user))
            {
                return new GWFMessage(GWFInfoCode.GWF_E_LOGIN_ERROR);
            }

            SqlConnection conn = this.connectToDB();
            String queryString = String.Format("SELECT * FROM GWFUser WHERE user_email='{0}' AND user_passwordMD5='{1}';", user.emailAccount.ToString(), user.passwordMD5);

            SqlCommand comm = new SqlCommand(queryString, conn);
            SqlDataReader reader = comm.ExecuteReader();
            if (!reader.HasRows)
            {
                this.disconnectToDB(conn);
                return new GWFMessage(GWFInfoCode.GWF_E_INVALID_EMAIL_PWD_PAIR);
            }
            if (reader.Read())
            {
                user.uid = reader.GetString(1);
            }

            this.disconnectToDB(conn);
            user.currentState = new OnlineState();
            this.allUserList.Add(user.uid, user);
            return new GWFMessage(GWFInfoCode.GWF_I_LOGIN_SUCCESS);
        }

        public GWFMessage LogOut(GWFMessage userInfo)
        {
            GWFUser user = (GWFUser) userInfo.content;
            this.allUserList.Remove(user.uid);
            return new GWFMessage(GWFInfoCode.GWF_I_LOGOUT_SUCCESS);
        }

        public GWFMessage SignUp(GWFMessage newUserInfoMsg)
        {
            GWFUser user = (GWFUser) newUserInfoMsg.content;
            if (this.isRegistered(user))
            {
                return new GWFMessage(GWFInfoCode.GWF_E_SIGNUP_ERROR);
            }
            user.uid = Guid.NewGuid().ToString();

            SqlConnection conn = this.connectToDB();
            String queryString = String.Format("INSERT INTO GWFUser (user_id, user_name, user_email, user_passwordMD5) VALUES ('{0}', '{1}', '{2}', '{3}');", user.uid, user.nickName, user.emailAccount, user.passwordMD5);
            this.executeSQLQuery(conn, queryString);
            this.disconnectToDB(conn);

            return new GWFMessage(GWFInfoCode.GWF_I_SIGNUP_SUCCESS);
        }

        public GWFMessage Follow(GWFMessage targetUserInfoMsg)
        {
            GWFUser user = (GWFUser)targetUserInfoMsg.content;
            if (!this.isRegistered(user))
            {
                return new GWFMessage(GWFInfoCode.GWF_E_FOLLOW_ERROR);
            }

            // TODO Add follower
            return new GWFMessage(GWFInfoCode.GWF_I_FOLLOW_SUCCESS);
        }

        public GWFMessage UnFollow(GWFMessage targetUserInfoMsg)
        {
            GWFUser user = (GWFUser)targetUserInfoMsg.content;
            if (!this.isRegistered(user))
            {
                return new GWFMessage(GWFInfoCode.GWF_E_UNFOLLOW_ERROR);
            }

            return new GWFMessage(GWFInfoCode.GWF_I_UNFOLLOW_SUCCESS);
        }

        private bool isRegistered(GWFUser user)
        {
            bool isRegistered = false;

            SqlConnection conn = this.connectToDB();
            String queryString = String.Format("SELECT * FROM GWFUser WHERE user_email = '{0}';", user.emailAccount);
            SqlCommand comm = new SqlCommand(queryString, conn);
            SqlDataReader reader = comm.ExecuteReader();
            isRegistered = reader.HasRows;
            this.disconnectToDB(conn);

            return isRegistered;
        }

        private System.Data.SqlClient.SqlConnection connectToDB()
        {
            System.Data.SqlClient.SqlConnection conn =
        new System.Data.SqlClient.SqlConnection();
            // TODO: Modify the connection string and include any
            // additional required properties for your database.
            conn.ConnectionString =
            "user id=bunny;" +
            "password=Test1340;" +
            "database=GPSWithFriends_db;" +
            "Server=pavh1f5kvg.database.windows.net;";
            try
            {
                conn.Open();
                // Insert code to process data.
                return conn;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to connect to data source: " + e.Message);
            }
            finally
            {
                //conn.Close();
            }
        }

        private void executeSQLQuery(System.Data.SqlClient.SqlConnection conn, string queryString)
        {
            SqlCommand comm = new SqlCommand(queryString, conn);
            //comm.Connection.Open();
            comm.ExecuteNonQuery();
        }
        private void disconnectToDB(System.Data.SqlClient.SqlConnection conn)
        {
            conn.Close();
        }
    }
}
