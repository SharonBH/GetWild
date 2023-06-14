using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace InShapeModels
{
    public class UserSubscriptionModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get { return FirstName + " " + LastName; } }

        public string PhoneNumber { get; set; }

        public DateTime DateSubscribed { get; set; }

        public DateTime? DateExpire { get; set; }

        public int NumClasses { get; set; }

        public double? AmountPaid { get; set; }

        public bool Active { get; set; }

        public int CurrentBalance { get; set; }

        public int ClassesDone { get; set; }

        public string UserCreated { get; set; }

        public int SubscriptionTypeId { get; set; }

        public List<SubscriptionDetailModel> Type { get; set; }

        public SubscriptionTypeModel SubscriptionType { get; set; }

        public bool Frozen { get; set; }

        //public FrozenSubscriptionModel FrozenSubscription { get; set; }

        public bool IsFirst { get; set; }

        public int LateCacelation { get; set; }

        public List<UserRole> Roles { get; set; }

        public Gender Gender { get; set; }

    }

    public class UserSubscriptionModelForNav
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get { return FirstName + " " + LastName; } }

        public string PhoneNumber { get; set; }

        public DateTime DateSubscribed { get; set; }

        public DateTime? DateExpire { get; set; }

        public int NumClasses { get; set; }

        public double? AmountPaid { get; set; }

        public bool Active { get; set; }

        public int CurrentBalance { get; set; }

        public int ClassesDone { get; set; }

        public string UserCreated { get; set; }

        public int SubscriptionTypeId { get; set; }

        //public List<SubscriptionDetailModel> Type { get; set; }

        //public SubscriptionTypeModel SubscriptionType { get; set; }

        public bool Frozen { get; set; }

        //public FrozenSubscriptionModel FrozenSubscription { get; set; }

        public bool IsFirst { get; set; }

        //public List<UserRole> Roles { get; set; }

    }


    public class UserRole
    {
        [Key]
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }


    public class UserSMSSubscriptionModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get { return FirstName + " " + LastName; } }

        public DateTime JoinDate { get; set; }

        public string PhoneNumber { get; set; }

        public bool? Active { get; set; }

        public DateTime? SubscriptionExpireDate { get; set; }

        public DateTime? SubscriptionStartDate { get; set; }

        public int? NumClasses { get; set; }

        public int? CurrentBalance { get; set; }

        public int WeeklyClasses { get; set; }

        public int? ClassesDone { get { return NumClasses - CurrentBalance; } }

        public DateTime? NextClassDate { get; set; }

        public DateTime? LastClassDate { get; set; }

        public bool Selected { get; set; }

        public UserType UserType { get; set; }

        public string Password { get; set; }

        public string Username { get; set; }
    }


    public class AboutToExpireMessageModel
    {
        public string Message { get; set; }

        public List<UserSubscriptionModel> AboutToExpireSubscriptionModels { get; set; }
    }

    public class SubscriptionDetailModel
    {
        public int Id { get; set; }

        public int SubscriptionId { get; set; }
        public DateTime Date { get; set; }

        public BalanceChangeModel ChangeType { get; set; }

        public int Value { get; set; }

        public string Note { get; set; }

        public string User { get; set; }

    }

    public class BalanceChangeModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Multiplier { get; set; }

        public bool ValueAsDays { get; set; }
    }

    public class DailyClassEnrollmentModel
    {
        public DateTime Date { get; set; }

        public bool IsReactive { get; set; }

        public List<StudioClassModel> Classes { get; set; }
    }

    public class ClassEnrollmentModel
    {
        public int Id { get; set; }

        public int SubscriptionId { get; set; }

        public int ClassId { get; set; }

        public DateTime DateEnrolled { get; set; }

        public DateTime? DateCanceled { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsSmsSent { get; set; }

        public UserSubscriptionModel UserSubscription { get; set; }

        public StudioClassModel Class { get; set; }

        public bool IsVerified { get; set; }

        public string UserTypestr { get; set; }

        public bool HaveClassToday => Class != null; //&& App.CurrentCompany.UseInstructors;  //todo change UseInstructors

        public string UserTypeName
        {
            get
            {
                var type = (ParticipantType)Convert.ToInt32(UserTypestr);
                return type.GetDisplayName();
            }
        }

        public bool IsTrailUser => ((ParticipantType)Convert.ToInt32(UserTypestr)) == ParticipantType.TrailUser;

        public double? Rating { get; set; }

        public int ClassesDoneinLastXdays { get; set; }

        public int DaysSinceLastClass
        {
            get
            {
                var days = DateTime.UtcNow.ToLocal().Date - LastClass;
                return days.HasValue ? days.Value.Days : 1000;
            }
        }

        public DateTime? LastClass { get; set; }

        [Required]
        public AvailablePlacementsModel SelectedPlacement { get; set; }

        //public string StudioPlacementName { get; set; }
        //public byte? ClassPlacementNumber { get; set; }

        public string CSSClass
        {
            get
            {
                if (IsTrailUser) return "TrailUser";
                else if (UserSubscription.DateSubscribed.AddDays(14).Date >= DateTime.UtcNow.Date && UserSubscription.IsFirst) return "NewUser";
                else if (UserSubscription.DateExpire.HasValue && UserSubscription.DateExpire.Value.AddDays(-14).Date <= DateTime.UtcNow.Date) return "ExpireUser";
                else if (DaysSinceLastClass >= App.Configuration.DaysSinceLastClassMarker) return "LastClassMarker";
                return "primary";
            }
        }

        public string Comment { get; set; }
        public string CommentBy { get; set; }

        public bool CommentByAdmin { get; set; }
        public DateTime? CommentDate { get; set; }

        public string Commentformated { get { return CommentDate.HasValue ? $"{CommentBy}({CommentDate.Value.ToShortDateString()}): {Comment}" : string.Empty; } }
    }

    public class CalendarClassEnrollmentModel
    {
        public int Id { get; set; }

        public int SubscriptionId { get; set; }

        public bool SubscriptionActive { get; set; }

        public int ClassId { get; set; }

        public DateTime DateEnrolled { get; set; }

        public DateTime? DateCanceled { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsSmsSent { get; set; }

        //public UserSubscriptionModel UserSubscription { get; set; }

        //public StudioClassModel Class { get; set; }

        public bool IsVerified { get; set; }

        public string UserTypestr { get; set; }

        //public bool HaveClassToday => Class != null; //&& App.CurrentCompany.UseInstructors;  //todo change UseInstructors

        public string UserTypeName
        {
            get
            {
                var type = (ParticipantType)Convert.ToInt32(UserTypestr);
                return type.GetDisplayName();
            }
        }

        public bool IsTrailUser => ((ParticipantType)Convert.ToInt32(UserTypestr)) == ParticipantType.TrailUser;

        public double? Rating { get; set; }

        public int ClassesDoneinLastXdays { get; set; }

        public int DaysSinceLastClass
        {
            get
            {
                var days = DateTime.UtcNow.ToLocal().Date - LastClass;
                return days.HasValue ? days.Value.Days : 1000;
            }
        }

        public DateTime? LastClass { get; set; }

        //[Required]
        //public AvailablePlacementsModel SelectedPlacement { get; set; }

        //public string StudioPlacementName { get; set; }
        //public byte? ClassPlacementNumber { get; set; }

        public string ClassName { get; set; }
        public DateTime ClassDate { get; set; }

        public int ClassParticipants { get; set; }

        public int? StudioPlacementId { get; set; }
        public string StudioPlacementName { get; set; }
        public byte? ClassPlacementNumber { get; set; }

        public string PlacementDisplyName { get { return StudioPlacementId != 999 && StudioPlacementId != 1003 && StudioPlacementId != 1007 ? $"{StudioPlacementName}: {ClassPlacementNumber}" : $"{StudioPlacementName}: {ClassPlacementNumber + 11}"; } } //{ClassPlacementNumber + 11}"; } }

    }

    public class EnrollmentCommentModel
    {
        public int? Id { get; set; }
        public int? EnrollmentId { get; set; }
        public string UserId { get; set; }
        public int? ClassId { get; set; }
        public string Comment { get; set; }
        public string CommentBy { get; set; }
        public string UserCreated { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? IsDeleted { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class MessageEnrollmentsModel
    {

        public MessageEnrollmentsModel()
        {
            EnrollmentModels = new List<ClassEnrollmentModel>();
        }

        public string Message { get; set; }

        public List<ClassEnrollmentModel> EnrollmentModels { get; set; }
    }

    public class SysMessage
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public DateTime Date { get; set; }

        public bool IsRead { get; set; }

        public int TypeId { get; set; }
    }


    public class UsersList
    {
        public bool IncludeFrozen { get; set; }

        public int PageNo { get; set; }
        public ParticipantType UserType { get; set; }

        public CompanyConfiguration CurrentCompany { get; set; }

        public List<UserWithSubscription> UserWithSubscriptions { get; set; }
    }

    public class UserWithSubscription : InShapeUser
    {
        

        public string ProfileIMGPath { get { return string.IsNullOrEmpty(ProfileIMG) ? @"/images/Members/no-profile.jpg" : @"/images/Members/" + ProfileIMG; } }

       public DateTime? SubscriptionExpireDate { get; set; }

        public DateTime? PayEndDate { get; set; }

        public string DaysSincePayEnd
        {
            get
            {
                var days = DateTime.UtcNow.ToLocal().Date - PayEndDate;
                return days.HasValue && days.Value.Days > 0 ? $" ({days.Value.Days})" :string.Empty;
            }
        }

        public DateTime? SubscriptionStartDate { get; set; }

        public int? NumClasses { get; set; }

        public int? CurrentBalance { get; set; }

        public int WeeklyClasses { get; set; }
         
        //public int? ClassesDone { get { return NumClasses - CurrentBalance; } }

        public int ClassesDone { get; set; }

        public int ClassesMissed { get; set; }

        public DateTime? NextClassDate { get; set; }

        public DateTime? LastClassDate { get; set; }

        public int DaysSinceLastClass { get {
                if (LastClassDate == null) return 999;
                return (DateTime.UtcNow.ToLocal().Date - LastClassDate.Value).Days;
            } }

        public string LastClassType { get; set; }

        public string NextClassType { get; set; }
        public bool Ticked { get; set; }

        public bool Frozen { get; set; }

        public string CssClass
        {
            get
            {
                return Utils.GetCssforTypeActive(Enum.GetName(typeof(UserType), UserType), Active.HasValue && Active.Value, Frozen);
                //var css = new StringBuilder();
                //css.Append(Active.HasValue && Active.Value ? UserType == UserType.TrailUser ? "ActiveTrailUser" : "ActiveUser" : "");
                //if (Frozen) css.Append(" FrozenUser");
                ////if (UserType == UserType.TrailUser) css.Append(" TrailUser");
                ////if (Frozen) css.Append(" InctiveUser");
                //css.Append(!Active.HasValue || !Active.Value ? UserType == UserType.TrailUser ? "InctiveTrailUser" : "InctiveUser" : "");
                //return css.ToString();
            }
        }

        public string PayingClass
        {
            get
            {
                return Active.HasValue && Active.Value && PayEndDate.HasValue && PayEndDate.Value.Date < DateTime.UtcNow.Date.ToLocal() ? "text-danger" : "";
                //var css = new StringBuilder();
                //css.Append(Active.HasValue && Active.Value ? UserType == UserType.TrailUser ? "ActiveTrailUser" : "ActiveUser" : "");
                //if (Frozen) css.Append(" FrozenUser");
                ////if (UserType == UserType.TrailUser) css.Append(" TrailUser");
                ////if (Frozen) css.Append(" InctiveUser");
                //css.Append(!Active.HasValue || !Active.Value ? UserType == UserType.TrailUser ? "InctiveTrailUser" : "InctiveUser" : "");
                //return css.ToString();
            }
        }

        public int ExtraDays { get
            {
                if (PayEndDate == null || PayEndDate.Value.Date >= DateTime.UtcNow.Date.ToLocal()) return 0;
                return (DateTime.UtcNow.Date.ToLocal() - PayEndDate.Value.Date).Days;
            }
        }

        public string SubscriptionType { get; set; }

    }

    public class InShapeUser
    {
        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get { return FirstName + " " + LastName; } }

        public Gender Gender { get; set; }

        public AgeGroup GetAgeGroup(int AdultAge, int TeenAge)
        {
            if (DOB == null) return AgeGroup.מבוגרים;
            var today = DateTime.UtcNow.ToLocal();
            int age = today.Year - DOB.Value.Year;

            if (today.Month > DOB.Value.Month || (today.Month == DOB.Value.Month && today.Day > DOB.Value.Day))
                age++;

            return age > AdultAge ? AgeGroup.מבוגרים : age > TeenAge ? AgeGroup.נוער : AgeGroup.ילדים;

        }

        public UserType UserType { get; set; }

        public DateTime JoinDate { get; set; }

        public string ProfileIMG { get; set; }

        public string PhoneNumber { get; set; }

        public bool? Active { get; set; }

        public int StudioId { get; set; }

        public int CompanyId { get; set; }

        public string Email { get; set; }

        public DateTime? DOB { get; set; }

        public string Address { get; set; }

        public bool ReceiveSMS { get; set; }
    }

    public class WaitListEnrollment
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int SubscriptionId { get; set; }
        public DateTime DateEnrolled { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DateCanceled { get; set; }
        public bool IsSmsSent { get; set; }
        public DateTime? DateSmsSent { get; set; }

        public bool IsBroadcastSmsSent { get; set; }
        public DateTime? DateBroadcastSmsSent { get; set; }


        public bool CanEnroll { get { return (IsSmsSent || IsBroadcastSmsSent) && !IsDeleted; } }
    }

}
