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
        GWF_E_FOLLOW_ERROR,
        GWF_E_UNFOLLOW_ERROR,

        GWF_E_INVALID_USER_INFO,
        GWF_E_INVALID_EMAIL_FORMAT,
        GWF_E_INVALID_EMAIL_PWD_PAIR,


        GWF_I_LOGIN_SUCCESS,
        GWF_I_SIGNUP_SUCCESS,
        GWF_I_LOGOUT_SUCCESS,
        GWF_I_FOLLOW_SUCCESS,
        GWF_I_UNFOLLOW_SUCCESS
    }
}