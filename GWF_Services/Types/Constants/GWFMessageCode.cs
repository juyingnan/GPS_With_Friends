using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GWF_Services.Types.Constants
{
    public enum GWFErrorCode
    {
        GWF_E_LOGIN_ERROR,
        GWF_E_SIGNUP_ERROR,

        GWF_E_INVALID_USER_INFO,
        GWF_E_INVALID_EMAIL_FORMAT
    }

    public enum GWFInfoCode
    {
        GWF_I_LOGIN_SUCCESS,
        GWF_I_SIGNUP_SUCCESS,
        GWF_I_LOGOUT_SUCCESS
    }
}