using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Globalization;
using System.Text.RegularExpressions;

using GWF_WebServices.Models;

namespace GWF_WebServices.Utils.ValidationUtils
{
    public static class StringValidationExtensions
    {
        public static bool IsValidEmail(this string strIn)
        {
            Regex regex = new Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                        + "@"
                                        + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");
            return regex.IsMatch(strIn);
        }
    }

    public static class UserValidationExtensions
    {
        public static bool IsRegistered(this GWF_User user)
        {
            bool isRegistered = false;
            GWF_DBEntities ctx = new GWF_DBEntities();
            int uc = (from s in ctx.GWF_Users where s.user_email == user.user_email select s).Count();
            isRegistered = uc > 0;

            return isRegistered;
        }

        public static bool IsValidPassword(this GWF_User user)
        {
            bool isValid = false;
            GWF_DBEntities ctx = new GWF_DBEntities();
            int uc = (from s in ctx.GWF_Users where (s.user_email == user.user_email && s.user_passwordMD5 == user.user_passwordMD5) select s).Count();
            isValid = uc > 0;

            return isValid;
        }
    }
}