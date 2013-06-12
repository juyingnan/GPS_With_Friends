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
using System.Collections.Concurrent;

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
    /// FKUBunny
    public class UserAction : System.Web.Services.WebService
    {
        private static ConcurrentDictionary<String, GWF_User> allUserList = new ConcurrentDictionary<String, GWF_User>();

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
            GWF_DBEntities ctx = new GWF_DBEntities();
            var query = from u in ctx.GWF_Users where u.user_email == user.user_email select u.user_id;
            string user_id = query.First();
            allUserList.AddOrUpdate(user_id, user, null);

            return user_id;
        }

        [WebMethod]
        public string FastSignUp(string email, string password, string nickName)
        {
            //return GWFInfoCode.GWF_E_INVALID_EMAIL_FORMAT.ToString();
            //GWFUser user2 = new GWFUser();
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

            GWF_DBEntities ctx = new GWF_DBEntities();

            ctx.GWF_Users.Add(user);

            GWF_User_Feed uf = new GWF_User_Feed();
            uf.feed_id = Guid.NewGuid().ToString();
            uf.user_id = user.user_id;

            ctx.GWF_User_Feeds.Add(uf);

            ctx.SaveChanges();

            return GWFInfoCode.GWF_I_SIGNUP_SUCCESS.ToString();
        }

        [WebMethod]
        public string LogOut(string uid)
        {
            GWF_User user = new GWF_User();
            user.user_id = uid;
            GWFMessage userInfoMsg = new GWFMessage();

            if (!allUserList.ContainsKey(user.user_id))
            {
                return GWFInfoCode.GWF_E_LOGOUT_ERROR.ToString();
            }

            allUserList.TryRemove(user.user_id, out user);

            return GWFInfoCode.GWF_I_LOGOUT_SUCCESS.ToString();
        }

        /**
         * Assume that the two user2 exist.
         */

        [WebMethod]
        public List<GWF_Message> GetMessages(string uid)
        {
            GWF_DBEntities ctx = new GWF_DBEntities();
            // First check the IM_MESSAGE
            // FIXME: only if we support multiple feed for each user2
            string feed_id = ctx.GWF_User_Feeds.SingleOrDefault(uf => uf.user_id == uid).feed_id;
            List<GWF_Message> messages = ctx.GWF_Messages.Where(m => m.feed_id == feed_id && m.is_processed == "N").ToList();
<<<<<<< HEAD

            foreach (GWF_Message msg in messages)
            {
                switch ((GWFMessageType)msg.content_type)
                {
                    case GWFMessageType.DEFAULT_TYPE:
                        break;
                    case GWFMessageType.IM_MESSAGE:
                        break;
                    case GWFMessageType.NOTIFICATION:
                        break;
                    case GWFMessageType.REQUEST:
                        // if accepted
                        break;
                    case GWFMessageType.LOC_UPDATE:
                        break;
                    default:
                        break;
                }
                msg.is_processed = "Y";
            }
            ctx.SaveChanges();

=======

            foreach (GWF_Message msg in messages)
            {
                switch ((GWFMessageType)msg.content_type)
                {
                    case GWFMessageType.DEFAULT_TYPE:
                        break;
                    case GWFMessageType.IM_MESSAGE:
                        break;
                    case GWFMessageType.NOTIFICATION:
                        break;
                    case GWFMessageType.REQUEST:
                        // if accepted
                        break;
                    default:
                        break;
                }
                msg.is_processed = "Y";
            }
           ctx.SaveChanges();

>>>>>>> 40b8671987faa250f97d648567786278098522dc
            return messages;
        }

        [WebMethod]
        public string SendMessage(string from_uid, string to_email2, string content)
        {
            GWF_DBEntities ctx = new GWF_DBEntities();
            // Get recipient's user_id
            GWF_User user2 = ctx.GWF_Users.Where(u => u.user_email == to_email2).SingleOrDefault();
            string user2_feed_id = ctx.GWF_User_Feeds.Where(uf => uf.user_id == user2.user_id).SingleOrDefault().feed_id;
            GWF_Message message = new GWF_Message();
            message.timestamp = DateTime.Now;
            message.feed_id = user2_feed_id;
            message.content_type = (int)GWFMessageType.IM_MESSAGE;
            message.content = content;
            message.is_processed = "N";
<<<<<<< HEAD
            message.from_uid = from_uid;

=======
>>>>>>> 40b8671987faa250f97d648567786278098522dc
            ctx.GWF_Messages.Add(message);
            ctx.SaveChanges();

            return GWFInfoCode.GWF_I_SEND_MESSAGE_SUCCESS.ToString();
        }

        [WebMethod]
        public string AddFriendRequest(string uid1, string email2)
        {
            if (!email2.IsValidEmail())
            {
                return GWFInfoCode.GWF_E_INVALID_EMAIL_FORMAT.ToString();
            }
            GWF_User user2 = new GWF_User();
            user2.user_email = email2;
            // Check if user2 exists
            if (!user2.IsRegistered())
            {
                return GWFInfoCode.GWF_E_ADD_FRIEND_ERROR.ToString();
            }
            
            // Get uid from email
            GWF_DBEntities ctx = new GWF_DBEntities();
            user2 = ctx.GWF_Users.Where(u => u.user_email == email2).SingleOrDefault();
            // Firstly push message to user2's feed
            string user2_feed_id = ctx.GWF_User_Feeds.Where(uf => uf.user_id == user2.user_id).SingleOrDefault().feed_id;

            GWF_Message message = new GWF_Message();
            message.timestamp = DateTime.Now;
            message.feed_id = user2_feed_id;
            message.content = "Add friend request from user2:[\"" + user2.user_name + "\", \"" + user2.user_email + "\"]";
            message.content_type = (int)GWFMessageType.REQUEST;
            message.is_processed = "N";
<<<<<<< HEAD
            message.from_uid = uid1;
=======
>>>>>>> 40b8671987faa250f97d648567786278098522dc

            ctx.GWF_Messages.Add(message);

            ctx.SaveChanges();
            
            return GWFInfoCode.GWF_I_ADD_FRIEND_SUCCESS.ToString();
        }

        [WebMethod]
        public string AnswerAddFriend(string uid1, string email2, bool accept)
        {
            if (!email2.IsValidEmail())
            {
                return GWFInfoCode.GWF_E_INVALID_EMAIL_FORMAT.ToString();
            }

            GWF_User user2 = new GWF_User();
            user2.user_email = email2;
            // Check if user2 exists
            if (!user2.IsRegistered())
            {
                return GWFInfoCode.GWF_E_ADD_FRIEND_ERROR.ToString();
            }

            GWF_DBEntities ctx = new GWF_DBEntities();

            GWF_User user1 = ctx.GWF_Users.Where(u => u.user_id == uid1).SingleOrDefault();
            user2 = ctx.GWF_Users.Where(u => u.user_email == email2).SingleOrDefault();
            GWF_Message message = new GWF_Message();
            message.feed_id = ctx.GWF_User_Feeds.Where(uf => uf.user_id == user2.user_id).SingleOrDefault().feed_id;
            message.content_type = (int)GWFMessageType.NOTIFICATION;
            message.timestamp = DateTime.Now;
            message.is_processed = "N";
<<<<<<< HEAD
            message.from_uid = uid1;
=======
>>>>>>> 40b8671987faa250f97d648567786278098522dc

            if (accept)
            {
                message.content = "Add Friend Request Accepted by user:[\"" + user1.user_name + "\", \"" + user1.user_email + "\"]";
                this.AddFriend(uid1, user2.user_id);
                ctx.GWF_Messages.Add(message);
                ctx.SaveChanges();
            }
            else
            {
                //message.content = "Add Friend Request Accepted by user:[\"" + user1.user_name + "\", \"" + user1.user_email + "\"]";
                // do nothing
            }

            return GWFInfoCode.GWF_I_ADD_FRIEND_SUCCESS.ToString();

        }

        [WebMethod]
<<<<<<< HEAD
        public string UpdateLocation(string uid, double lati, double longi)
        {
            GWF_DBEntities ctx = new GWF_DBEntities();
            GWF_geo_Location loc = new GWF_geo_Location();
            loc.user_id = uid;
            loc.latitude = lati;
            loc.longitude = longi;

            // Add locations to feed
            ctx.GWF_geo_Locations.Add(loc);
            // Get friends user_id
            List<string> friends_uid = ctx.GWF_FriendRelations.Where(uf => uf.user_id == uid).Select(u => u.user_id2).ToList();
            // Get friends' feed_id
            List<string> feed_ids = new List<string>();
            foreach (string fuid in friends_uid) {
                feed_ids.Add(ctx.GWF_User_Feeds.Where(uf => uf.user_id == fuid).SingleOrDefault().feed_id);
            }

            // Send messages to feeds
            foreach (string fid in feed_ids)
            {
                GWF_Message message = new GWF_Message();
                message.timestamp = DateTime.Now;
                message.feed_id = fid;
                message.content_type = (int)GWFMessageType.LOC_UPDATE;
                message.from_uid = uid;
                message.content = "(" + lati.ToString() + "," + longi.ToString() + ")";
                message.is_processed = "N";
                ctx.GWF_Messages.Add(message);
            }

            ctx.SaveChanges();

            return GWFInfoCode.GWF_I_SEND_MESSAGE_SUCCESS.ToString();
        }

        [WebMethod]
=======
>>>>>>> 40b8671987faa250f97d648567786278098522dc
        public string DeleteFriend(string uid1, string email2)
        {
            GWF_DBEntities ctx = new GWF_DBEntities();
            // Get friend uid
            GWF_User user2 = ctx.GWF_Users.Where(f => f.user_email == email2).SingleOrDefault();

            if (!this.AreFriends(uid1, user2.user_id))
            {
                return GWFInfoCode.GWF_E_DELETE_FRIEND_ERROR.ToString();
            }

            GWF_FriendRelation relation = ctx.GWF_FriendRelations.Where(fr => fr.user_id == uid1 && fr.user_id2 == user2.user_id).SingleOrDefault();
            ctx.GWF_FriendRelations.Remove(relation);
            ctx.SaveChanges();

            return GWFInfoCode.GWF_I_DELETE_FRIEND_SUCCESS.ToString();
        }

        [WebMethod]
        public List<GWF_User> GetFriends(string uid)
        {
            GWF_DBEntities ctx = new GWF_DBEntities();
            IEnumerable<string> friend_uids = ctx.GWF_FriendRelations.Where(uf => uf.user_id == uid).Select(uf => uf.user_id2);

            List<GWF_User> friends = new List<GWF_User>();
            foreach (string uid2 in friend_uids)
            {
                GWF_User user = ctx.GWF_Users.SingleOrDefault(u => u.user_id == uid2);
                user.user_passwordMD5 = "";
                friends.Add(user);
            }

            return friends;
        }

        [WebMethod]
        public List<string> GetGroupNames(string uid)
        {
            GWF_DBEntities ctx = new GWF_DBEntities();
            List<string> group_names = ctx.GWF_User_Groups.Where(ug => ug.user_id == uid).Select(un => un.group_name).ToList();

            return group_names;

        }

        [WebMethod]
        public List<GWF_User> GetGroupInfoFromName(string uid, string name)
        {
            GWF_DBEntities ctx = new GWF_DBEntities();
            GWF_User_Group user_group = ctx.GWF_User_Groups.Where(ug => ug.user_id == uid && ug.group_name == name).SingleOrDefault();
                List<string> members = ctx.GWF_Groups.Where(gi => gi.group_id == user_group.group_id).Select(m => m.member_user_id).ToList();
                List<GWF_User> member_users = new List<GWF_User>();
                foreach (string m in members)
                {
                    GWF_User user = ctx.GWF_Users.Where(u => u.user_id == uid).SingleOrDefault();
                    user.user_passwordMD5 = "";
                    member_users.Add(user);
                }
            return member_users;
        }

        [WebMethod]
        public string CreateGroup(string uid, string group_name)
        {
            GWF_DBEntities ctx = new GWF_DBEntities();

            if (ctx.GWF_User_Groups.Where(ug => ug.user_id == uid && ug.group_name == group_name).Count() > 0) 
            {
                return GWFInfoCode.GWF_E_GROUP_NAME_EXISTS.ToString();
            }

            GWF_User_Group user_group = new GWF_User_Group();
            user_group.user_id = uid;
            user_group.group_id = Guid.NewGuid().ToString();
            user_group.group_name = group_name;
<<<<<<< HEAD

            ctx.GWF_User_Groups.Add(user_group);
            ctx.SaveChanges();

            return GWFInfoCode.GWF_I_CREATE_GROUP_SUCCESS.ToString();
        }

        [WebMethod]
        public string UpdateGroup(string uid, string group_name, List<GWF_Group> group)
        {
            GWF_DBEntities ctx = new GWF_DBEntities();
            foreach (GWF_Group g in group) 
            {
                ctx.GWF_Groups.Add(g);
            }

            ctx.SaveChanges();
            return GWFInfoCode.GWF_I_UPDATE_GROUP_SUCCESS.ToString();
            
        }

=======

            ctx.GWF_User_Groups.Add(user_group);
            ctx.SaveChanges();

            return GWFInfoCode.GWF_I_CREATE_GROUP_SUCCESS.ToString();
        }

        [WebMethod]
        public string UpdateGroup(string uid, string group_name, List<GWF_Group> group)
        {
            GWF_DBEntities ctx = new GWF_DBEntities();
            foreach (GWF_Group g in group) 
            {
                ctx.GWF_Groups.Add(g);
            }

            ctx.SaveChanges();
            return GWFInfoCode.GWF_I_UPDATE_GROUP_SUCCESS.ToString();
            
        }

>>>>>>> 40b8671987faa250f97d648567786278098522dc
        [WebMethod]
        public string DeleteGroup(string uid, string group_name)
        {
            GWF_DBEntities ctx = new GWF_DBEntities();
            GWF_User_Group u_group = ctx.GWF_User_Groups.Where(ug => ug.user_id == uid && ug.group_name == group_name).SingleOrDefault();

            ctx.GWF_User_Groups.Remove(u_group);
            ctx.SaveChanges();

            return GWFInfoCode.GWF_I_DELETE_GROUP_SUCCESS.ToString();
        }

        public string AddFriend(string uid1, string uid2)
        {
            // Actually add friend
            if (this.AreFriends(uid1, uid2))
            {
                // Already friends
                // Do nothing
                return GWFInfoCode.GWF_I_ADD_FRIEND_SUCCESS.ToString();
            }

            GWF_DBEntities ctx = new GWF_DBEntities();
            ctx.GWF_FriendRelations.Add(new GWF_FriendRelation { user_id = uid1, user_id2 = uid2 });
            ctx.SaveChanges();

            return GWFInfoCode.GWF_I_ADD_FRIEND_SUCCESS.ToString();
        }
        
        public bool AreFriends(string uid1, string uid2)
        {
            GWF_DBEntities ctx = new GWF_DBEntities();
            var query = from uf in ctx.GWF_FriendRelations where uf.user_id == uid1 && uf.user_id2 == uid2 select uf;
            int c = query.Count();
            return c > 0;
        }
    }
}
