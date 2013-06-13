using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;

using GWF_WCF_WebRole.Contracts;

namespace GWF_WCF_WebRole.Services.Interfaces
{
    [ServiceContract]
    interface IMessageService
    {
        [WebGet(UriTemplate = "/feeds/{feed_id}/Messages", ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        public CMessages GetMessages(string feed_id);

        [WebInvoke(UriTemplate = "/users/{to_uid}?_action=SendMessage&_from_uid={from_uid}&_content={content}",
            Method = "POST",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        public GWFInfoCode SendIMMessage(string from_uid, string to_uid, string content);

        [WebInvoke(UriTemplate = "/users/{to_uid}?_action=AddFriendRequest&_from_uid={from_uid}",
            Method = "POST",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        public GWFInfoCode AddFriendRequest(string from_uid, string to_uid);

        [WebInvoke(UriTemplate = "/users/{to_uid}?_action=AnswerFriendRequest&_from_uid={from_uid}",
            Method = "POST",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        public GWFInfoCode AnswerFriendRequest(string from_uid, string to_uid);

        [WebInvoke(UriTemplate = "/locations/{uid}?_action=SetLatestLocation&_lat={latitude}&_long={longitude}",
            Method = "POST",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        public GWFInfoCode SetLatestLocation(string uid, double latitude, double longitude);
    }
}
