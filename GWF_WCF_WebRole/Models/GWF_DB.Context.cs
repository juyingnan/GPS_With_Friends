﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GWF_WCF_WebRole.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class GWF_DBContext : DbContext
    {
        public GWF_DBContext()
            : base("name=GWF_DBContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<GWF_Device> GWF_Devices { get; set; }
        public DbSet<GWF_FriendRelation> GWF_FriendRelations { get; set; }
        public DbSet<GWF_geo_Location> GWF_geo_Locations { get; set; }
        public DbSet<GWF_Group> GWF_Groups { get; set; }
        public DbSet<GWF_Message> GWF_Messages { get; set; }
        public DbSet<GWF_Profile> GWF_Profiles { get; set; }
        public DbSet<GWF_User> GWF_Users { get; set; }
        public DbSet<GWF_User_Feed> GWF_User_Feeds { get; set; }
        public DbSet<GWF_User_Group> GWF_User_Groups { get; set; }
    }
}
