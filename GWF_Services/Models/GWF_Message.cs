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
    
    public partial class GWF_Message
    {
        public int ID { get; set; }
        public string feed_id { get; set; }
        public int content_type { get; set; }
        public string content { get; set; }
        public System.DateTime timestamp { get; set; }
        public string is_processed { get; set; }
        public string from_uid { get; set; }
    
        public virtual GWF_User_Feed GWF_User_Feed { get; set; }
    }
}