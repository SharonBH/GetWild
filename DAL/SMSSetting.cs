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
    
    public partial class SMSSetting
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string DisplyName { get; set; }
        public int Balance { get; set; }
        public bool Active { get; set; }
        public string URL { get; set; }
    
        public virtual Company Company { get; set; }
    }
}
