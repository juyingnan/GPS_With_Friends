using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GWF_WebServices;
using GWF_WebServices.Types.Constants;
using GWF_WebServices.Models;
using System.Web;
using System.Data.Entity;
using System.Linq;
namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAddFriends()
        {
            UserAction uaTest = new UserAction();
            string res_fail = GWFInfoCode.GWF_E_ADD_FRIEND_ERROR.ToString();
            string res_suc = GWFInfoCode.GWF_I_ADD_FRIEND_SUCCESS.ToString();
            string actual_res = "";

            string email1 = "test1" + DateTime.Now.Millisecond + "@test.com";
            string email2 = "test2" + DateTime.Now.Millisecond + "@test.com";

            this.TestSignUp(GWFInfoCode.GWF_I_SIGNUP_SUCCESS.ToString(), email1, "1", "1");
            this.TestSignUp(GWFInfoCode.GWF_I_SIGNUP_SUCCESS.ToString(), email2, "2", "2");

            GWF_DBContext ctx = new GWF_DBContext();
            var query = from u in ctx.GWF_Users where u.user_email == email1 select u;
            GWF_User user1 = query.First();
            query = from u in ctx.GWF_Users where u.user_email == email2 select u;
            GWF_User user2 = query.First();

            actual_res = uaTest.AddFriend(user1.user_id, user2.user_id);
            Assert.AreEqual(GWFInfoCode.GWF_I_ADD_FRIEND_SUCCESS.ToString(), actual_res);
        }

        [TestMethod]
        public void TestSignUpWREmail()
        {
            string email = "testn" + DateTime.Now.Millisecond; 
            // Test incorrect email format
            this.TestSignUp(GWFInfoCode.GWF_E_INVALID_EMAIL_FORMAT.ToString(), email, "1", "1");

        }

        [TestMethod]
        public void TestSignUpDuplicated()
        {
            string email = "123@123.com"; 
            // Test incorrect email format
            // Test duplicated sign up
            this.TestSignUp(GWFInfoCode.GWF_E_SIGNUP_ERROR.ToString(), email, "123", "123");

        }
        [TestMethod]
        public void TestSignUpSuccess()
        {
            string email = "testn" + DateTime.Now.Millisecond; 
            email += "@test.com";
            // Test correct email format
            this.TestSignUp(GWFInfoCode.GWF_I_SIGNUP_SUCCESS.ToString(), email, "1", "1");
        }


        [TestMethod]
        public void TestLoginNotRegistered()
        {
            string email = "123@123.com";
            string pwd = "123";
            string email_notin = "test" + DateTime.Now.Millisecond + "@test.com";
            // Not signed up
            this.TestLogin(GWFInfoCode.GWF_E_LOGIN_ERROR.ToString(), email_notin, pwd);
        }

        [TestMethod]
        public void TestLoginWRPwd()
        {
            string email = "123@123.com";
            string pwd = "123";
            string email_notin = "test" + DateTime.Now.Millisecond + "@test.com";
            // Incorrect password
            this.TestLogin(GWFInfoCode.GWF_E_INVALID_EMAIL_PWD_PAIR.ToString(), email, pwd + DateTime.Now.Millisecond);
        }

        [TestMethod]
        public void TestLoginWREmail()
        {
            string email = "123@123.com";
            string pwd = "123";
            string email_notin = "test" + DateTime.Now.Millisecond + "@test.com";
            // Invalid email format
            this.TestLogin(GWFInfoCode.GWF_E_INVALID_EMAIL_FORMAT.ToString(), email_notin + "@123123", pwd);
        }

        [TestMethod]
        public void TestLoginSuccess()
        {
            string email = "123@123.com";
            string pwd = "123";
            // Successfully logged in
            this.TestLogin(GWFInfoCode.GWF_I_LOGIN_SUCCESS.ToString(), email, pwd);
        }

        public void TestSignUp(string expect, string email, string pwd, string name)
        {
            string actual_res = "";
            UserAction uaTest = new UserAction();
            actual_res = uaTest.FastSignUp(email, pwd, name);
            Assert.AreEqual(expect, actual_res);
        }

        public void TestLogin(string expect, string email, string pwd)
        {
            string actual_res = "";

            UserAction uaTest = new UserAction();
            actual_res = uaTest.LogIn(email, pwd);
            Assert.AreEqual(expect, actual_res);
        }

    }
}
