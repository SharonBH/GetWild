//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserSMSs
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public System.DateTime Date { get; set; }
        public string SMS { get; set; }
        public int TypeId { get; set; }
        public Nullable<int> RefId { get; set; }
        public string Response { get; set; }
    
        public virtual MSGType MSGType { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
    }
}