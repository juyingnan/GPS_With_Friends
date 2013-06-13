using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GWF_WCF_WebRole.Contracts;
using GWF_WebServices.Models;
using GWF_WebServices.Entities;

namespace GWF_WebServices.Utils.UserUtils
{
    public class EntityUtils
    {
        public static CMessage ToCMessage(GWF_Message g_msg)
        {
            CMessage c_msg = new CMessage();
            c_msg.content_type = (CMessageType)g_msg.content_type;
            c_msg.content = g_msg.content;
            c_msg.feed_id = g_msg.feed_id;
            c_msg.from_uid = g_msg.from_uid;
            c_msg.from_email = g_msg.from_email;
            c_msg.timestamp = g_msg.timestamp;

            return c_msg;
        }

        public static CMessages ToCMessages(List<GWF_Message> g_msgs)
        {
            CMessages c_msgs = new CMessages();
            foreach (GWF_Message g_msg in g_msgs)
            {
                c_msgs.Add(ToCMessage(g_msg));
            }
            return c_msgs;
        }

        public static CUser ToCUser(GWF_User g_user)
        {
            CUser c_user = new CUser();
            c_user.id = g_user.user_id;
            c_user.name = g_user.user_name;
            c_user.email = g_user.user_email;

            return c_user;
        }

        public static CUsers ToCUsers(List<GWF_User> g_users)
        {
            CUsers c_users = new CUsers();
            foreach (GWF_User g_user in g_users)
            {
                c_users.Add(ToCUser(g_user));
            }
            return c_users;
        }

        public static CLocation ToCLocation(GWF_geo_Location g_loc)
        {
            CLocation c_loc = new CLocation();
            c_loc.last_time = g_loc.timestamp;
            c_loc.latitude = g_loc.latitude;
            c_loc.longitude = g_loc.longitude;

            return c_loc;
        }
    }
}