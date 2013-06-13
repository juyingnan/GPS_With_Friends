using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

using System.Text.RegularExpressions;

namespace GWF_WCF_WebRole.Contracts
{
    [CollectionDataContract(Name = "users", Namespace = "")]
    public class CUsers : List<CUser>
    {
    }

    [DataContract(Name = "user", Namespace = "")]
    public class CUser
    {
        [DataMember(Name = "id", Order = 1)]
        public string id;

        [DataMember(Name = "email", Order = 2)]
        public string email;

        [DataMember(Name = "name", Order = 3)]
        public string name;

        [DataMember(Name = "last_location", Order = 4)]
        public CLocation last_location;

        [DataMember(Name = "current_state", Order = 5)]
        public CUserState current_state;

        [DataMember(Name = "feed_id", Order = 6)]
        public string feed_id;

        [DataMember]
        public string group_name;
    }

    


    
}