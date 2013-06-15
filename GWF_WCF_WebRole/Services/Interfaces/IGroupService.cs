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
    public interface IGroupService
    {
        [WebInvoke(UriTemplate = "/users/{uid}/groups/name/{group_name}",
            Method = "POST",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        GWFInfoCode CreateGroup(string uid, string group_name);

        [WebGet(UriTemplate = "/users/{uid}/groups/id/{group_id}", ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        CGroup GetGroup(string uid, string group_id);

        [WebInvoke(UriTemplate = "/users/{uid}/groups/id/{group_id}/{uid2}",
            Method = "POST",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        GWFInfoCode AddToGroup(string uid, string group_id, string uid2);

        [WebInvoke(UriTemplate = "/users/{uid}/groups/id/{group_id}/{uid2}",
            Method = "DELETE",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        GWFInfoCode RemoveFromGroup(string uid, string group_id, string uid2);

        [WebInvoke(UriTemplate = "/users/{uid}/groups/id/{from_gid}/{uid2}?_action=ChangeGroup&_to_gid=to_gid",
            Method = "PUT",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        GWFInfoCode ChangeGroup(string uid, string from_gid, string to_gid, string uid2);
    }
}
