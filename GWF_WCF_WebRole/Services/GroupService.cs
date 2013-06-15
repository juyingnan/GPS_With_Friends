using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GWF_WCF_WebRole.Services.Interfaces;
namespace GWF_WCF_WebRole.Services
{
    public class GroupService: IGroupService
    {
        public Contracts.GWFInfoCode CreateGroup(string uid, string group_name)
        {
            throw new NotImplementedException();
        }

        public Contracts.CGroup GetGroup(string uid, string group_id)
        {
            throw new NotImplementedException();
        }

        public Contracts.GWFInfoCode AddToGroup(string uid, string group_id, string uid2)
        {
            throw new NotImplementedException();
        }

        public Contracts.GWFInfoCode RemoveFromGroup(string uid, string group_id, string uid2)
        {
            throw new NotImplementedException();
        }

        public Contracts.GWFInfoCode ChangeGroup(string uid, string from_gid, string to_gid, string uid2)
        {
            throw new NotImplementedException();
        }
    }
}