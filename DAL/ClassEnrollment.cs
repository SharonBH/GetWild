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
    
    public partial class ClassEnrollment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ClassEnrollment()
        {
            this.EnrollmentComments = new HashSet<EnrollmentComment>();
        }
    
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int SubscriptionId { get; set; }
        public System.DateTime DateEnrolled { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> DateCanceled { get; set; }
        public bool IsVerified { get; set; }
        public bool IsSmsSent { get; set; }
        public Nullable<double> Rating { get; set; }
        public Nullable<int> ClassAvailablePlacementId { get; set; }
        public bool IsLateCancel { get; set; }
    
        public virtual Class Class { get; set; }
        public virtual ClassAvailablePlacement ClassAvailablePlacement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EnrollmentComment> EnrollmentComments { get; set; }
        public virtual UserSubscription UserSubscription { get; set; }
    }
}
