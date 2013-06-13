using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GWF_WCF_WebRole.Contracts
{
    [CollectionDataContract]
    public class CProfiles : Dictionary<string, CProfile>
    {
    }    [DataContract]
    public class CProfile
    {
        [DataMember]
        public string user_id;
        [DataMember]
        public string Firstname;
        [DataMember]
        public string Lastname;
        [DataMember]
        public string PictureURL;
        [DataMember]
        public string PictureBigURL;
        [DataMember]
        public string PictureSmallURL;
        [DataMember]
        public DateTime Birthday;
    }
}