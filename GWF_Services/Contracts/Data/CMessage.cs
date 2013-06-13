using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GWF_WCF_WebRole.Contracts
{
    [CollectionDataContract]
    public class CMessages : List<CMessage>
    {
    }

    [DataContract]
    public class CMessage
    {
        [DataMember]
        public string feed_id { get; set; }
        [DataMember]
        public string from_uid { get; set; }
        [DataMember]
        public string from_email { get; set; }
        [DataMember]
        public CMessageType content_type { get; set; }
        [DataMember]
        public string content { get; set; }
        [DataMember]
        public System.DateTime timestamp { get; set; }
        [DataMember]
        public string hint
        {
            get;
            set;
        }
    }
}