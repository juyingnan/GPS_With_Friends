//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GWF_WebServices.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class GWF_FriendRelation
    {
        public int ID { get; set; }
        public string user_id { get; set; }
        public string user_id2 { get; set; }
    
        public virtual GWF_User GWF_User { get; set; }
    }
}
