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
    
    public partial class Tip
    {
        public int Id { get; set; }
        public int StudioId { get; set; }
        public string Short { get; set; }
        public string Long { get; set; }
        public bool IsDeleted { get; set; }
    
        public virtual Studio Studio { get; set; }
    }
}
