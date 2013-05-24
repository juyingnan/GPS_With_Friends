﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

using GWF_Services.Types;
using GWF_Services.Types.Constants;
using GWF_Services.Entities.UserEntities;

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
        private  Dictionary<String, User> allUserList = new Dictionary<String, User>();

        //[WebMethod]
        //public string Declaration(User user)
        ////public string Declaration(User user, OnlineState onlineState, OfflineState offlineState, HiddenState hiddenState)
        //{
        //    return "Don not use this fucking method!";
        //}

        [WebMethod]
        public GWFMessage LogIn(GWFMessage userInfo)
        {
            User user = (User)userInfo.content;
            if (!this.isRegistered(user))
            {
                return new GWFMessage(GWFErrorCode.GWF_E_LOGIN_ERROR);
            }

            user.currentState = new OnlineState();
            this.allUserList.Add(user.uid, user);
            return new GWFMessage(GWFInfoCode.GWF_I_LOGIN_SUCCESS);
        }

        [WebMethod]
        public GWFMessage LogOut(GWFMessage userInfo)
        {
            User user = (User) userInfo.content;
            this.allUserList.Remove(user.uid);
            return new GWFMessage(GWFInfoCode.GWF_I_LOGOUT_SUCCESS);
        }

        [WebMethod]
        public GWFMessage SignUp(GWFMessage newUserInfoMsg)
        {
            User user = (User) newUserInfoMsg.content;
            if (this.isRegistered(user))
            {
                return new GWFMessage(GWFErrorCode.GWF_E_SIGNUP_ERROR);
            }
            user.uid = Guid.NewGuid().ToString();

            SqlConnection conn = this.connectToDB();
            String queryString = String.Format("INSERT INTO GWFUser (guid, name, email) VALUES ('{0}', '{1}', '{2}');", user.uid, user.nickName, user.emailAccount);
            this.executeSQLQuery(conn, queryString);
            this.disconnectToDB(conn);

            return new GWFMessage(GWFInfoCode.GWF_I_SIGNUP_SUCCESS);
        }

        [WebMethod]
        public GWFMessage Follow(GWFMessage targetUserInfoMsg)
        {
            User user = (User)targetUserInfoMsg.content;
            if (!this.isRegistered(user))
            {
                return new GWFMessage(GWFErrorCode.GWF_E_FOLLOW_ERROR);
            }

            // TODO Add follower
            return new GWFMessage(GWFInfoCode.GWF_I_FOLLOW_SUCCESS);
        }

        [WebMethod]
        public GWFMessage UnFollow(GWFMessage targetUserInfoMsg)
        {
            User user = (User)targetUserInfoMsg.content;
            if (!this.isRegistered(user))
            {
                return new GWFMessage(GWFErrorCode.GWF_E_UNFOLLOW_ERROR);
            }

            return new GWFMessage(GWFInfoCode.GWF_I_UNFOLLOW_SUCCESS);
        }

        private bool isRegistered(User user)
        {
            bool isRegistered = false;

            SqlConnection conn = this.connectToDB();
            String queryString = String.Format("SELECT COUNT(*) FROM GWFUser;");
            SqlCommand comm = new SqlCommand(queryString, conn);
            SqlDataReader reader = comm.ExecuteReader();
            isRegistered = !reader.HasRows;
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
             "integrated security=SSPI;data source=SQL Server Name;" +
             "persist security info=False;initial catalog=northwind";
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
        }

        private void executeSQLQuery(System.Data.SqlClient.SqlConnection conn, string queryString)
        {
            SqlCommand comm = new SqlCommand(queryString, conn);
            comm.Connection.Open();
            comm.ExecuteNonQuery();
        }
        private void disconnectToDB(System.Data.SqlClient.SqlConnection conn)
        {
            conn.Close();
        }
    }
}
