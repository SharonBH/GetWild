﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class InShapeEntities : DbContext
    {
        public InShapeEntities()
            : base("name=InShapeEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BalanceChangeType> BalanceChangeTypes { get; set; }
        public virtual DbSet<ClassType> ClassTypes { get; set; }
        public virtual DbSet<UserBalanceTracking> UserBalanceTrackings { get; set; }
        public virtual DbSet<SysAlert> SysAlerts { get; set; }
        public virtual DbSet<UserProcessing> UserProcessings { get; set; }
        public virtual DbSet<ProfileTracking> ProfileTrackings { get; set; }
        public virtual DbSet<ClassDailySlot> ClassDailySlots { get; set; }
        public virtual DbSet<UserSMSs> UserSMSses { get; set; }
        public virtual DbSet<MSGType> MSGTypes { get; set; }
        public virtual DbSet<UserDailyTick> UserDailyTicks { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<SmsSender> SmsSenders { get; set; }
        public virtual DbSet<SubscriptionType> SubscriptionTypes { get; set; }
        public virtual DbSet<SMSSetting> SMSSettings { get; set; }
        public virtual DbSet<Studio> Studios { get; set; }
        public virtual DbSet<StudioRoom> StudioRooms { get; set; }
        public virtual DbSet<Subscriber> Subscribers { get; set; }
        public virtual DbSet<FrozenSubscription> FrozenSubscriptions { get; set; }
        public virtual DbSet<ClassWaitList> ClassWaitLists { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Tip> Tips { get; set; }
        public virtual DbSet<Class_Instructors> Class_Instructors { get; set; }
        public virtual DbSet<InstructorSalary> InstructorSalaries { get; set; }
        public virtual DbSet<UserMobileDevice> UserMobileDevices { get; set; }
        public virtual DbSet<StudioExpens> StudioExpenses { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<StudioPlacement> StudioPlacements { get; set; }
        public virtual DbSet<ClassEnrollment> ClassEnrollments { get; set; }
        public virtual DbSet<ClassAvailablePlacement> ClassAvailablePlacements { get; set; }
        public virtual DbSet<InstructorDetail> InstructorDetails { get; set; }
        public virtual DbSet<ClassTypeDetail> ClassTypeDetails { get; set; }
        public virtual DbSet<LastClass> LastClasses { get; set; }
        public virtual DbSet<EnrollmentComment> EnrollmentComments { get; set; }
        public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<DailyExport> DailyExports { get; set; }
    
        public virtual int DailySystemAlerts()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DailySystemAlerts");
        }
    
        public virtual int ExpireSubscriptions()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ExpireSubscriptions");
        }
    
        public virtual int SetLastClass()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SetLastClass");
        }
    
        public virtual int CalcClassRating(Nullable<int> classId)
        {
            var classIdParameter = classId.HasValue ?
                new ObjectParameter("classId", classId) :
                new ObjectParameter("classId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("CalcClassRating", classIdParameter);
        }
    
        public virtual int ResetLateCancel()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ResetLateCancel");
        }
    }
}
