using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GWF_WCF_WebRole.Contracts
{
    [DataContract]
    public enum CMessageType
    {
        [EnumMember]
        DEFAULT_TYPE = 0,

        [EnumMember]
        IM_MESSAGE = 1,

        [EnumMember]
        REQUEST = 2,

        [EnumMember]
        NOTIFICATION = 3,

        [EnumMember]
        LOC_UPDATE = 4
    }
}