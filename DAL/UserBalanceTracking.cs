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
    
    public partial class UserBalanceTracking
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public System.DateTime Date { get; set; }
        public int ChangeTypeId { get; set; }
        public int Value { get; set; }
        public int Balance { get; set; }
        public string Note { get; set; }
        public string UserUpdated { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    
        public virtual BalanceChangeType BalanceChangeType { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual UserSubscription UserSubscription { get; set; }
    }
}