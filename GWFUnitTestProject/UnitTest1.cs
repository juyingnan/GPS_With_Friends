//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using GPSWithFriends;

//namespace GWFUnitTestProject
//{
//    [TestClass]
//    public class UnitTest1
//    {
//        [TestMethod]
//        public void TestGetCoordinateStringOutput()
//        {
//            MainPage main = new MainPage();
//            string result;
//            result=main.GetCoordinateString(new Geocoordinate(90,120));
//            string expect = "Lat: {90.0000}, Long: {120.0000}, Acc: 5m";
//            Assert.AreEqual(expect, result, false);
//        }

//        [TestMethod]
//        public void TestSendFriendRequestInputLengthIsZero()
//        {
//            bool result;
//            result = MainPage.SendFriendRequest("");
//            Assert.IsFalse(result);
//        }

//        [TestMethod]
//        public void TestSendFriendRequestInputLengthIsNotZero()
//        {
//            bool result;
//            result = MainPage.SendFriendRequest("test");
//            Assert.IsTrue(result);
//        }

//        [TestMethod]
//        public void TestSendFriendRequestInputLengthIsNull()
//        {
//            bool result;
//            result = MainPage.SendFriendRequest(null);
//            Assert.IsFalse(result);
//        }


//    }
//}
