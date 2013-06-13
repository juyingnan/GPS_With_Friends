using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GWF_WCF_WebRole.Contracts
{
    [CollectionDataContract]
    public class CGroups : Dictionary<string, CGroup>
    {
    }

    [DataContract]
    public class CGroup
    {
        [DataMember]
        public string id;

        [DataMember]
        public CUsers members;
    }
}