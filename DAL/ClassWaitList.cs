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
    
    public partial class ClassWaitList
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int SubscriptionId { get; set; }
        public System.DateTime DateEnrolled { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> DateCanceled { get; set; }
        public bool IsSmsSent { get; set; }
        public Nullable<System.DateTime> DateSmsSent { get; set; }
        public bool IsBroadcastSmsSent { get; set; }
        public Nullable<System.DateTime> DateBroadcastSmsSent { get; set; }
    
        public virtual Class Class { get; set; }
        public virtual UserSubscription UserSubscription { get; set; }
    }
}