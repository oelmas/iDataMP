//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Domain
{
    using System;
    using System.Collections.Generic;
    
    public partial class User_Types
    {
        public User_Types()
        {
            this.Users = new HashSet<User>();
        }
    
        public int id { get; set; }
        public string role_name { get; set; }
    
        public virtual ICollection<User> Users { get; set; }
    }
}
