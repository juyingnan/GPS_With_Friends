using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Services;

using GWF_WebServices.Types;
using GWF_WebServices.Types.Constants;
using GWF_WebServices.Entities.UserEntities;
using GWF_WebServices.Utils.ValidationUtils;
using GWF_WebServices.Models;
using GWF_WCF_WebRole.Contracts;

//Databasse
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Collections.Concurrent;
using GWF_WebServices.Utils.UserUtils;

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
        private static string default_group_name = "My Friends";
        private static ConcurrentDictionary<String, GWF_User> allUserList = new ConcurrentDictionary<String, GWF_User>();

        [WebMethod]
        public string LogIn(string email, string password)
        {
            email = email.ToLower();
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
            var query = from u in ctx.GWF_Users where u.user_email == user.user_email select u.user_id;
            string user_id = query.First();
            allUserList[user_id] = user;
            if (user_id == null)
            {
                return "uid is null";
            }

            return "uid:" + user_id;
        }

        [WebMethod]
        public string FastSignUp(string email, string password, string nickName)
        {
            //return GWFInfoCode.GWF_E_INVALID_EMAIL_FORMAT.ToString();
            //GWFUser user2 = new GWFUser();
            email = email.ToLower();
            GWF_User user = new GWF_User();

            if (!email.IsValidEmail())
            {
                return GWFInfoCode.GWF_E_INVALID_EMAIL_FORMAT.ToString();
            }

            user.user_email = email;
            // TODO Add name validation
            user.user_name = nickName;
            user.user_passwordMD5 = password;

            if (user.IsRegistered())
            {
                return GWFInfoCode.GWF_E_SIGNUP_ERROR.ToString();
            }

            user.user_id = Guid.NewGuid().ToString();

            GWF_DBContext ctx = new GWF_DBContext();

            ctx.GWF_Users.Add(user);

            GWF_User_Feed uf = new GWF_User_Feed();
            uf.feed_id = Guid.NewGuid().ToString();
            uf.user_id = user.user_id;

            ctx.GWF_User_Feeds.Add(uf);

            ctx.SaveChanges();
            // Create default group
            this.CreateGroup(user.user_id, default_group_name);

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
        //public List<GWF_Message> GetMessages(string uid)
        public CMessages GetMessages(string uid)
        {
            GWF_DBContext ctx = new GWF_DBContext();
            // First check the IM_MESSAGE
            // FIXME: only if we support multiple feed for each user2
            string feed_id = ctx.GWF_User_Feeds.SingleOrDefault(uf => uf.user_id == uid).feed_id;

            int loop_count = 0;

            //while (loop_count < 15)
            {
                List<GWF_Message> messages = ctx.GWF_Messages.Where(m => m.feed_id == feed_id && m.is_processed == "N").ToList();
                //GWF_Message messages = ctx.GWF_Messages.Where(m => m.feed_id == feed_id && m.is_processed == "N").SingleOrDefault();
                if (messages != null)
                {
                    //GWF_Message msg = messages;
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
                                //msg.from_email = ctx.GWF_Users.Where(u => u.user_id == msg.from_uid).SingleOrDefault().user_email;
                                break;
                            case GWFMessageType.LOC_UPDATE:
                                break;
                            default:
                                break;
                        }
                        msg.from_email = ctx.GWF_Users.Where(u => u.user_id == msg.from_uid).SingleOrDefault().user_email;
                        msg.is_processed = "Y";
                    }
                    ctx.SaveChanges();
                    return EntityUtils.ToCMessages(messages);
                    //return new List<GWF_Message>();
                }
                loop_count++;
            }

            return new CMessages();
        }

        [WebMethod]
        public string SendMessage(string from_uid, string to_email2, string content)
        {
            to_email2 = to_email2.ToLower();
            GWF_DBContext ctx = new GWF_DBContext();
            // Get recipient's user_id
            GWF_User user2 = ctx.GWF_Users.Where(u => u.user_email == to_email2).SingleOrDefault();
            if (user2 == null)
            {
                return GWFInfoCode.GWF_E_SEND_MESSAGE_ERROR.ToString();
            }
            string user2_feed_id = ctx.GWF_User_Feeds.Where(uf => uf.user_id == user2.user_id).SingleOrDefault().feed_id;
            GWF_Message message = new GWF_Message();
            message.timestamp = DateTime.Now;
            message.feed_id = user2_feed_id;
            message.content_type = (int)GWFMessageType.IM_MESSAGE;
            message.content = content;
            message.is_processed = "N";
            message.from_uid = from_uid;

            ctx.GWF_Messages.Add(message);
            ctx.SaveChanges();

            return GWFInfoCode.GWF_I_SEND_MESSAGE_SUCCESS.ToString();
        }

        [WebMethod]
        public string AddFriendRequest(string uid1, string email2)
        {
            email2 = email2.ToLower();
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
            GWF_DBContext ctx = new GWF_DBContext();
            user2 = ctx.GWF_Users.Where(u => u.user_email == email2).SingleOrDefault();
            GWF_User user = ctx.GWF_Users.Where(u => u.user_id == uid1).SingleOrDefault();
            // Firstly push message to user2's feed
            string user2_feed_id = ctx.GWF_User_Feeds.Where(uf => uf.user_id == user2.user_id).SingleOrDefault().feed_id;

            GWF_Message message = new GWF_Message();
            message.timestamp = DateTime.Now;
            message.feed_id = user2_feed_id;
            message.content = "Add friend request from user:\"" + user.user_email + "\"";
            message.content_type = (int)GWFMessageType.REQUEST;
            message.is_processed = "N";
            message.from_uid = uid1;
            message.from_email = user.user_email;

            ctx.GWF_Messages.Add(message);

            ctx.SaveChanges();

            return GWFInfoCode.GWF_I_ADD_FRIEND_SUCCESS.ToString();
        }

        [WebMethod]
        public string AnswerAddFriend(string uid1, string email2, bool accept)
        {
            email2 = email2.ToLower();
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

            GWF_DBContext ctx = new GWF_DBContext();

            GWF_User user1 = ctx.GWF_Users.Where(u => u.user_id == uid1).SingleOrDefault();
            user2 = ctx.GWF_Users.Where(u => u.user_email == email2).SingleOrDefault();
            GWF_Message message = new GWF_Message();
            message.feed_id = ctx.GWF_User_Feeds.Where(uf => uf.user_id == user2.user_id).SingleOrDefault().feed_id;
            message.content_type = (int)GWFMessageType.NOTIFICATION;
            message.timestamp = DateTime.Now;
            message.is_processed = "N";
            message.from_uid = uid1;
            //message.from_email = user1.user_email;

            if (accept)
            {
                message.content = "[*ANS_ADDF*]Add Friend Request Accepted by user:\"" + user1.user_email + "\"";
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
        public string UpdateLocation(string uid, double lati, double longi)
        {
            GWF_DBContext ctx = new GWF_DBContext();
            GWF_geo_Location loc = new GWF_geo_Location();
            loc.user_id = uid;
            loc.latitude = lati;
            loc.longitude = longi;
            loc.timestamp = DateTime.Now;

            // Add locations to feed
            ctx.GWF_geo_Locations.Add(loc);
            // Get friends user_id
            List<string> friends_uid = ctx.GWF_FriendRelations.Where(uf => uf.user_id == uid).Select(u => u.user_id2).ToList();
            // Get friends' feed_id
            List<string> feed_ids = new List<string>();
            foreach (string fuid in friends_uid)
            {
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
                GWF_User u = ctx.GWF_Users.SingleOrDefault(usr => usr.user_id == uid);
                message.content = "(" + u.user_email + "," + lati.ToString() + "," + longi.ToString() + ")";
                message.is_processed = "N";
                ctx.GWF_Messages.Add(message);
            }

            ctx.SaveChanges();

            return GWFInfoCode.GWF_I_SEND_MESSAGE_SUCCESS.ToString();
        }

        [WebMethod]
        public string DeleteFriend(string uid1, string email2, string group_name)
        {
            email2 = email2.ToLower();
            GWF_DBContext ctx = new GWF_DBContext();
            // Get friend uid
            GWF_User user2 = ctx.GWF_Users.Where(f => f.user_email == email2).SingleOrDefault();

            if (user2 == null || !this.AreFriends(uid1, user2.user_id))
            {
                return GWFInfoCode.GWF_E_DELETE_FRIEND_ERROR.ToString();
            }

            GWF_FriendRelation relation1 = ctx.GWF_FriendRelations.Where(fr => fr.user_id == uid1 && fr.user_id2 == user2.user_id).SingleOrDefault();
            GWF_FriendRelation relation2 = ctx.GWF_FriendRelations.Where(fr => fr.user_id == user2.user_id && fr.user_id2 == uid1).SingleOrDefault();
            ctx.GWF_FriendRelations.Remove(relation1);
            ctx.GWF_FriendRelations.Remove(relation2);
            // TODO Delete friend from my group
            // Check if group exists
            GWF_User_Group my_user_group = ctx.GWF_User_Groups.Where(ug => ug.user_id == uid1 && ug.group_name == group_name).SingleOrDefault();
            if (my_user_group == null) {
                return "user does not has group " + group_name;
            }
            string my_gid = my_user_group.group_id;

            GWF_Group g = ctx.GWF_Groups.Where(gi => gi.group_id == my_gid && gi.member_user_id == user2.user_id).SingleOrDefault();
            if (g == null)
            {
                return "friend not in group " + group_name;
            }

            ctx.GWF_Groups.Remove(g);

            // Delete friend from friend's group
            // 1. find all groups' id this user belongs 
            List<string> ingids = ctx.GWF_Groups.Where(ig => ig.member_user_id == uid1).Select(igid => igid.group_id).ToList();
            // 2. find all groups' id that friend has
            List<string> fugids = ctx.GWF_User_Groups.Where(ug => ug.user_id == user2.user_id).Select(ugid => ugid.group_id).ToList();

            string ingid = "0";
            // 3. find the group id there must has one
            foreach (string id in fugids)
            {
                if (ingids.Contains(id))
                {
                    ingid = id;
                }
            }

            // 4. delete myself from friend's group
            GWF_Group fg = ctx.GWF_Groups.Where(mfg => mfg.group_id == ingid && mfg.member_user_id == uid1).SingleOrDefault();
            ctx.GWF_Groups.Remove(fg);
            ctx.SaveChanges();
            // 5. tell the client to refresh
            GWF_Message message = new GWF_Message();
            message.feed_id = ctx.GWF_User_Feeds.Where(uf => uf.user_id == user2.user_id).SingleOrDefault().feed_id;
            message.content_type = (int)GWFMessageType.NOTIFICATION;
            message.timestamp = DateTime.Now;
            message.is_processed = "N";
            message.from_uid = uid1;
            //message.from_email = user1.user_email;
            message.content = "[*DELF_INFO*]Friend deleted user:\"" + email2 + "\"";
            ctx.GWF_Messages.Add(message);
            ctx.SaveChanges();

            return GWFInfoCode.GWF_I_DELETE_FRIEND_SUCCESS.ToString();
        }

        [WebMethod]
        public CUsers GetFriends(string uid)
        {
            GWF_DBContext ctx = new GWF_DBContext();
            IEnumerable<string> friend_uids = ctx.GWF_FriendRelations.Where(uf => uf.user_id == uid).Select(uf => uf.user_id2);

            CUsers friends = new CUsers();
            foreach (string uid2 in friend_uids)
            {
                GWF_User user = ctx.GWF_Users.SingleOrDefault(u => u.user_id == uid2);
                CUser cu = EntityUtils.ToCUser(user);
                cu.last_location = this.GetLatestLocation(cu.id);

                friends.Add(cu);
            }

            return friends;
        }

        [WebMethod]
        public List<string> GetGroupNames(string uid)
        {
            GWF_DBContext ctx = new GWF_DBContext();
            List<string> group_names = ctx.GWF_User_Groups.Where(ug => ug.user_id == uid).Select(un => un.group_name).ToList();

            return group_names;

        }

        [WebMethod]
        public CUsers GetGroupInfoFromName(string uid, string name)
        {
            GWF_DBContext ctx = new GWF_DBContext();
            GWF_User_Group user_group = ctx.GWF_User_Groups.Where(ug => ug.user_id == uid && ug.group_name == name).SingleOrDefault();
            if (user_group == null)
            {
                return null;
            }
            List<string> members = ctx.GWF_Groups.Where(gi => gi.group_id == user_group.group_id).Select(m => m.member_user_id).ToList();
            List<GWF_User> member_users = new List<GWF_User>();
            CUsers m_users = new CUsers();
            foreach (string m in members)
            {
                GWF_User user = ctx.GWF_Users.Where(u => u.user_id == m).SingleOrDefault();
                user.user_passwordMD5 = "";
                CUser cu = EntityUtils.ToCUser(user);
                cu.group_name = name;
                cu.last_location = this.GetLatestLocation(cu.id);
                m_users.Add(cu);
            }
            return m_users;
        }

        [WebMethod]
        public string CreateGroup(string uid, string group_name)
        {
            GWF_DBContext ctx = new GWF_DBContext();

            if (ctx.GWF_User_Groups.Where(ug => ug.user_id == uid && ug.group_name == group_name).Count() > 0)
            {
                return GWFInfoCode.GWF_E_GROUP_NAME_EXISTS.ToString();
            }

            GWF_User_Group user_group = new GWF_User_Group();
            user_group.user_id = uid;
            user_group.group_id = Guid.NewGuid().ToString();
            user_group.group_name = group_name;
            ctx.GWF_User_Groups.Add(user_group);
            ctx.SaveChanges();

            return GWFInfoCode.GWF_I_CREATE_GROUP_SUCCESS.ToString();
        }

        [WebMethod]
        public string ChangeUserGroup(string uid, string old_group_name, string new_group_name, string email2)
        {
            email2 = email2.ToLower();
            GWF_DBContext ctx = new GWF_DBContext();
            CUser c_u = this.GetUserFromEmail(uid, email2);
            string uid2 = c_u.id;

            string new_group_id = ctx.GWF_User_Groups.Where(g => g.user_id == uid && g.group_name == new_group_name).SingleOrDefault().group_id;
            string old_group_id = ctx.GWF_User_Groups.Where(g => g.user_id == uid && g.group_name == old_group_name).SingleOrDefault().group_id;

            if (new_group_id == null)
            {
                return new_group_name + "does not exist.";
            }
            if (old_group_id == null)
            {
                return old_group_name + "does not exist.";
            }
            // Delete from old
            GWF_Group old_g_group = ctx.GWF_Groups.Where(g => g.group_id == old_group_id && g.member_user_id == uid2).SingleOrDefault();
            if (old_g_group == null)
            {
                return "user " + email2 + " is not in group " + old_group_name;
            }
            ctx.GWF_Groups.Remove(old_g_group);

            // Add to new
            GWF_Group new_g_group = new GWF_Group();
            new_g_group.group_id = new_group_id;
            new_g_group.member_user_id = uid2;
            ctx.GWF_Groups.Add(new_g_group);

            ctx.SaveChanges();
            return GWFInfoCode.GWF_I_CHANGE_GROUP_SUCCESS.ToString();

        }

        [WebMethod]
        public string DeleteGroup(string uid, string group_name)
        {
            if (group_name == default_group_name)
            {
                return GWFInfoCode.GWF_E_DELETE_GROUP_ERROR.ToString();
            }
            GWF_DBContext ctx = new GWF_DBContext();
            GWF_User_Group u_group = ctx.GWF_User_Groups.Where(ug => ug.user_id == uid && ug.group_name == group_name).SingleOrDefault();

            // TODO Change all member to default group
            var res = ctx.GWF_Groups.Where(ug => ug.group_id == u_group.group_id);
            if (res.Count() <= 0)
            {
                ctx.GWF_User_Groups.Remove(u_group);
                ctx.SaveChanges();
                return GWFInfoCode.GWF_I_DELETE_GROUP_SUCCESS.ToString();
            }
            List<GWF_Group> members = res.ToList();
            foreach (GWF_Group m in members)
            {
                string email = ctx.GWF_Users.Where(u => u.user_id == m.member_user_id).SingleOrDefault().user_email;
                this.ChangeUserGroup(uid, group_name, default_group_name, email);
            }


            ctx = new GWF_DBContext();
            GWF_User_Group new_u_group = ctx.GWF_User_Groups.Where(ug => ug.user_id == uid && ug.group_name == group_name).SingleOrDefault();
            ctx.GWF_User_Groups.Remove(new_u_group);

            ctx.SaveChanges();

            return GWFInfoCode.GWF_I_DELETE_GROUP_SUCCESS.ToString();
        }

        [WebMethod]
        public CUser GetUserFromEmail(string uid, string email)
        {
            email = email.ToLower();
            GWF_DBContext ctx = new GWF_DBContext();
            GWF_User g_f = ctx.GWF_Users.Where(u => u.user_email == email).SingleOrDefault();
            if (g_f == null)
            {
                return null;
            }
            CUser c_u = EntityUtils.ToCUser(g_f);
            if (ctx.GWF_FriendRelations.Where(fr => fr.user_id == uid && fr.user_id2 == g_f.user_id).Count() <= 0)
            {
                c_u.IsFriendOfCaller = false;
            }
            else
            {
                c_u.IsFriendOfCaller = true;
            }
            return c_u;
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

            GWF_DBContext ctx = new GWF_DBContext();

            // Add to default group
            GWF_Group default_group1 = new GWF_Group();
            GWF_User_Group ug1 = ctx.GWF_User_Groups.Where(g => g.user_id == uid1 && g.group_name == default_group_name).SingleOrDefault();
            string dg1_id = ug1.group_id;
            default_group1.member_user_id = uid2;
            default_group1.group_id = dg1_id;
            ctx.GWF_Groups.Add(default_group1);

            GWF_Group default_group2 = new GWF_Group();
            GWF_User_Group ug2 = ctx.GWF_User_Groups.Where(g => g.user_id == uid2 && g.group_name == default_group_name).SingleOrDefault();
            string dg2_id = ug2.group_id;
            default_group2.member_user_id = uid1;
            default_group2.group_id = dg2_id;
            ctx.GWF_Groups.Add(default_group2);


            GWF_FriendRelation fr1 = new GWF_FriendRelation();
            fr1.user_id = uid1;
            fr1.user_id2 = uid2;

            ctx.GWF_FriendRelations.Add(fr1);
            GWF_FriendRelation fr2 = new GWF_FriendRelation();
            fr2.user_id = uid2;
            fr2.user_id2 = uid1;
            ctx.GWF_FriendRelations.Add(fr2);
            ctx.SaveChanges();

            return GWFInfoCode.GWF_I_ADD_FRIEND_SUCCESS.ToString();
        }

        public bool AreFriends(string uid1, string uid2)
        {
            GWF_DBContext ctx = new GWF_DBContext();
            var query = from uf in ctx.GWF_FriendRelations where uf.user_id == uid1 && uf.user_id2 == uid2 select uf;
            int c = query.Count();
            return c > 0;
        }

        public CLocation GetLatestLocation(string uid)
        {
            GWF_DBContext ctx = new GWF_DBContext();
            var locs = ctx.GWF_geo_Locations.Where(gl => gl.user_id == uid);
            if (locs.Count() <= 0)
            {
                return null;
            }
            GWF_geo_Location g_loc = locs.OrderByDescending(s => s.timestamp).First();

            return EntityUtils.ToCLocation(g_loc);
        }
    }
}
