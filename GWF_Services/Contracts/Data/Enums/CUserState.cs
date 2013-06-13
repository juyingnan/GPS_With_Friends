using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GWF_WCF_WebRole.Contracts
{
    [DataContract]
    public enum CUserState
    {
        [EnumMember]
        ONLINE,
        
        [EnumMember]
        OFFLINE
    }
}