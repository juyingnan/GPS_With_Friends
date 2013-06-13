using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using GWF_WCF_WebRole.Contracts;

namespace GWF_WCF_WebRole.Services.Interfaces
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IUserService" in both code and config file together.
    [ServiceContract]
    public interface IUserService
    {

        [WebGet(UriTemplate = "/users/{uid}", ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        public CUser GetUser(string uid);

        [WebGet(UriTemplate = "/users", ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        public CUsers GetUsers(List<string> uid_list);

        [WebInvoke(UriTemplate = "/users/{uid}?_MD5={passwordMD5}",
            Method = "DELETE",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        public GWFInfoCode DeleteUser(string uid, string passwordMD5);

        [WebInvoke(UriTemplate = "/users/{uid}?_MD5={passwordMD5}",
            Method = "PUT",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        public GWFInfoCode UpdatePassword(string uid, string passwordMD5);

        [WebInvoke(UriTemplate = "/users/{uid}?_nick_name={nick_name}",
            Method = "PUT",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        public GWFInfoCode UpdateNickName(string uid, string nick_name);

        [WebInvoke(UriTemplate = "/users?_action=SignUp&_email={email}&_MD5={passwordMD5}",
            Method = "PUT",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        public GWFInfoCode SignUp(string email, string passwordMD5);

        [WebGet(UriTemplate = "/users?_action=SignIn&_email={email}&_MD5={passwordMD5}", ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        public GWFInfoCode SignIn(string email, string passwordMD5);

        [WebGet(UriTemplate = "/users/{uid}?_action=SignOff", ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        public GWFInfoCode SignOff(string uid);

    }
}
