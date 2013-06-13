using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GWF_WebServices.Types.Constants
{
    public enum GWFInfoCode
    {
        GWF_E_LOGIN_ERROR,
        GWF_E_SIGNUP_ERROR,
        GWF_E_LOGOUT_ERROR,
        GWF_E_ADD_FRIEND_ERROR,
        GWF_E_DELETE_FRIEND_ERROR,
        GWF_E_GET_FRIENDS_ERROR,
        //GWF_E_CREATE_GROUP_ERROR,
        GWF_E_GROUP_NAME_EXISTS,
        GWF_E_UPDATE_GROUP_ERROR,
        GWF_E_DELETE_GROUP_ERROR,
        GWF_E_SEND_MESSAGE_ERROR,
        GWF_E_GET_MESSAGE_TIMEOUT,

        GWF_E_INVALID_USER_INFO,
        GWF_E_INVALID_EMAIL_FORMAT,
        GWF_E_INVALID_EMAIL_PWD_PAIR,


        GWF_I_LOGIN_SUCCESS,
        GWF_I_SIGNUP_SUCCESS,
        GWF_I_LOGOUT_SUCCESS,
        GWF_I_ADD_FRIEND_SUCCESS,
        GWF_I_DELETE_FRIEND_SUCCESS,
        GWF_I_GET_FRIENDS_SUCCESS,
        GWF_I_CREATE_GROUP_SUCCESS,
        GWF_I_CHANGE_GROUP_SUCCESS,
        GWF_I_DELETE_GROUP_SUCCESS,
        GWF_I_SEND_MESSAGE_SUCCESS
    }
}