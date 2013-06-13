using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GWF_WCF_WebRole.Contracts
{
    [CollectionDataContract]
    public class CLocations: Dictionary<string, CLocations>
    {
    }

    [DataContract]
    public class CLocation
    {
        [DataMember]
        public DateTime last_time;
        [DataMember]
        public double latitude;
        [DataMember]
        public double longitude;
    }
}