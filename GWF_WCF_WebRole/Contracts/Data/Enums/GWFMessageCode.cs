using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GWF_WCF_WebRole.Contracts
{
    [DataContract]
    public enum GWFInfoCode
    {
        [EnumMember]
        GWF_E_LOGIN_ERROR,

        [EnumMember]
        GWF_E_SIGNUP_ERROR,

        [EnumMember]
        GWF_E_LOGOUT_ERROR,

        [EnumMember]
        GWF_E_ADD_FRIEND_ERROR,

        [EnumMember]
        GWF_E_DELETE_FRIEND_ERROR,

        [EnumMember]
        GWF_E_GET_FRIENDS_ERROR,

        [EnumMember]
        GWF_E_CREATE_GROUP_ERROR,

        [EnumMember]
        GWF_E_GROUP_NAME_EXISTS,

        [EnumMember]
        GWF_E_UPDATE_GROUP_ERROR,

        [EnumMember]
        GWF_E_DELETE_GROUP_ERROR,

        [EnumMember]
        GWF_E_SEND_MESSAGE_ERROR,

        [EnumMember]
        GWF_E_INVALID_USER_INFO,

        [EnumMember]
        GWF_E_INVALID_EMAIL_FORMAT,

        [EnumMember]
        GWF_E_INVALID_EMAIL_PWD_PAIR,

        [EnumMember]
        GWF_I_LOGIN_SUCCESS,

        [EnumMember]
        GWF_I_SIGNUP_SUCCESS,

        [EnumMember]
        GWF_I_LOGOUT_SUCCESS,

        [EnumMember]
        GWF_I_ADD_FRIEND_SUCCESS,

        [EnumMember]
        GWF_I_DELETE_FRIEND_SUCCESS,

        [EnumMember]
        GWF_I_GET_FRIENDS_SUCCESS,

        [EnumMember]
        GWF_I_CREATE_GROUP_SUCCESS,

        [EnumMember]
        GWF_I_UPDATE_GROUP_SUCCESS,

        [EnumMember]
        GWF_I_DELETE_GROUP_SUCCESS,

        [EnumMember]
        GWF_I_SEND_MESSAGE_SUCCESS
    }
}