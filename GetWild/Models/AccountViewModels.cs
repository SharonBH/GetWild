using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using InShapeModels;
using Newtonsoft.Json;
using Utilities;

namespace GetWild.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "אימייל")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "קוד")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "זכור את הדפדפן?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "אימייל")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "חובה למלא כתובת אימייל")]
        [Display(Name = "אימייל")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "סיסמא")]
        public string Password { get; set; }

        [Display(Name = "זכור אותי?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "חובה למלא כתובת אימייל")]
        [EmailAddress]
        [Display(Name = "כתובת אימייל")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "אורך השדה {0} חייב להיות לפחות {2} תווים.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "סיסמא")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "אימות סיסמא")]
        [Compare("Password", ErrorMessage = "שדות לא זהים - סיסמא ואימות סיסמא.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "שם פרטי")]
        public string Fname { get; set; }

        [Required]
        [Display(Name = "שם משפחה")]
        public string Lname { get; set; }

        [Required]
        [Display(Name = "טלפון")]
        public string mobile { get; set; }


        [Display(Name = "כתובת")]
        public string Address { get; set; }

        [Display(Name = "תמונה")]
        public string ProfileIMG { get; set; }

        [Display(Name = "תאריך לידה")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }

        [Display(Name = "קבלת הודעות")]
        public bool ReceiveSMS { get; set; }

        public DateTime JoinDate { get; set; }

        public DateTime ProfileUpdateDate { get; set; }

        public string Userid { get; set; }

        [Display(Name = "מין")]
        public Gender Gender { get; set; }

        [Display(Name = "קבוצת גיל")]
        public AgeGroup AgeGroup { get; set; }

        [Required]
        [Display(Name = "סוג מתאמן")]
        public ParticipantType UserType { get; set; }
        //public ProfileViewModel Profile { get; set; }

        public bool SignedHealthTandC { get; set; }

        public int StudioId { get; set; }

        [Display(Name = "סימון")]
        public bool Marked { get; set; }

        public CompanyConfiguration CurrentCompany { get; set; }

    }

    public class RegisterInstructorViewModel : RegisterViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "שכר לאימון")]
        public double Rate { get; set; }

        [Display(Name = "נסיעות (יומי)")]
        public double DailyRate { get; set; }


        [Required]
        [Display(Name = "צבע")]
        public string ColorCode { get; set; }

        public DateTime DateUpdated { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "תמונה")]
        public HttpPostedFileBase ImageUpload { get; set; }
    }


    public class HealthTandCViewModel
    {
        [JsonIgnore]
        [Required(ErrorMessage = "חובה למלא כתובת אימייל (email)")]
        [EmailAddress]
        [Display(Name = "כתובת אימייל")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "שם פרטי")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "שם משפחה")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "טלפון")]
        public string PhoneNumber { get; set; }

        [JsonIgnore]
        public string ProfileIMG { get; set; }

        [Required]
        [Display(Name = "תאריך לידה")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }

        [Required]
        [Display(Name = "עיר")]
        public string Address { get; set; }

        [JsonIgnore]
        [Display(Name = "תאריך")]
        [DataType(DataType.Date)]
        public DateTime? SignedDate { get; set; }

        [Required]
        [Display(Name = "חתימה")]
        [UIHint("SignaturePad")]
        public byte[] Signature { get; set; }

        //[Required]
        [Display(Name = "עיסוק במהלך היום")]
        public string Occupation { get; set; }

        [JsonIgnore]
        public string Userid { get; set; }

        [Required]
        [Display(Name = "תעודת זהות")]
        //[StringLength(9, MinimumLength = 9,ErrorMessage = "תעודת זהות לא נכונה")]
        [RegularExpression("[0-9]{8,9}", ErrorMessage = "תעודת זהות לא נכונה, תעודת זהות 9 ספרות (TZ)")]
        public long? CitizenId { get; set; }

        [JsonIgnore]
        public string SignatureIMGPath { get; set; }

        [Display(Name = "מין")]
        public Gender Gender { get; set; }
    }



    public class ProfileViewModel
    {
        [Display(Name = "גובה")]
        public int? Height { get; set; }

        [Display(Name = "משקל")]
        public decimal? Weight { get; set; }

        public decimal? OrigWeight { get; set; }

        [Display(Name = "BMI")]
        public decimal? BMI { get; set; }

        [Display(Name = "אחוזי שומן")]
        public decimal? Fat { get; set; }

        [Display(Name = "ידיים")]
        public decimal? Dim_Hands { get; set; }

        [Display(Name = "רגליים")]
        public decimal? Dim_Legs { get; set; }

        [Display(Name = "מותניים")]
        public decimal? Dim_Waist { get; set; }

        [Display(Name = "שוקיים")]
        public decimal? Dim_Thighs { get; set; }

        //[Display(Name = "אחר")]
        //public int? D_Other { get; set; }

        [Display(Name = "יד שמאל")]
        public decimal? Fat_HandL { get; set; }
        
        [Display(Name = "יד ימין")]
        public decimal? Fat_HandR { get; set; }

        [Display(Name = "רגל ימין")]
        public decimal? Fat_LegR { get; set; }

        [Display(Name = "רגל שמאל")]
        public decimal? Fat_LegL { get; set; }

        [Display(Name = "בטן")]
        public decimal? Fat_Belly { get; set; }

        [Display(Name = "מסת שריר")]
        public decimal? Mucsle { get; set; }

        public string UserId { get; set; }

        public DateTime? Date { get; set; }

        public int Id { get; set; }

        [Display(Name = "עד כה ירדת")]
        public decimal WeightChange { get; set; }

        [Display(Name = "שוקיים")]
        public string Picture { get; set; }

        public string ProgressIMGPath { get { return @"/images/Members/" + Picture; } }

        public string datestr
        {
            get { return Date.Value.ToString("dd/MM/yy"); }
        }

    }

    public class SubscriptionViewModel
    {
        public string UserId { get; set; }

        public int Id { get; set; }

        [Display(Name = "תאריך רכישה")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateSubscribed { get; set; }

        [Display(Name = "תאריך תפוגה")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateExpire { get; set; }

        [Required]
        [Range(0, 100)]
        [Display(Name = "מספר אימונים")]
        public int NumClasses { get; set; }

        [Display(Name = "סכום ששולם")]
        public double AmountPaid { get; set; }

        [Display(Name = "פעיל")]
        public bool Active { get; set; }

        [Display(Name = "יתרת אימונים")]
        public int CurrentBalance { get; set; }

        public int ClassesDone { get; set; }

        public List<SubscriptionDetailViewModel> Details { get; set; }

        public int SubscriptionTypeId { get; set; }

        public SubscriptionTypeViewModel SubscriptionType { get; set; }

        public List<SubscriptionTypeViewModel> SubscriptionTypesList { get; set; }

        public int LateCacelation { get; set; }
        public string Expiration
        {
            get
            {
                return DateExpire.HasValue
                    ? //string.Format("{0} - {1}", DateExpire.Value.ToShortDateString(), DateAdded.ToShortDateString())
                    //string.Format("המנוי בתוקף עד <b>{0}</b> ({1} יום)", DateExpire.Value.ToShortDateString(), ExpirationDays)
                    "המנוי בתוקף עד:" + " <b>" + DateExpire.Value.ToShortDateString() + "</b>" + "(" + ExpirationDays + "יום)"
                    : "המנוי ללא תאריך תפוגה";
            }
        }

        public string ExpirationDays
        {
            get
            {
                return DateExpire.HasValue
                    ? (DateExpire.Value - DateTime.UtcNow.ToLocal().Date).TotalDays.ToString()
                    : "המנוי ללא תאריך תפוגה";
            }
        }

        public int SubscriptionProgress
        {
            get { return Math.Min((int)((CurrentBalance / (double)NumClasses) * 100), 100); }
        }


        public string SubscriptionProgressClass
        {
            get
            {
                if (CurrentBalance >= 5) return "success";
                if (CurrentBalance >= 3) return "warning";
                return "danger";
            }
        }


        public int HeartRate
        {
            get { return NumClasses == 0 ? 0 : Convert.ToInt32((NumClasses) / NumClasses * 5); }
        }

        public List<SysMessage> Messages { get; set; }

        public bool Frozen { get; set; }

    }

    public class SubscriptionDetailViewModel
    {
        public int Id { get; set; }

        public int SubscriptionId { get; set; }
        public DateTime Date { get; set; }


        [Display(Name = "סוג שינוי")]
        public int ChangeTypeId { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "מספר")]
        public int Value  { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "תאור")]
        public string Note { get; set; }

        public string User { get; set; }

        public string UserId { get; set; }

        public List<BalanceChangeModel> ChangeTypes;
    }


    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "אימייל")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "סיסמא")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "אימות סיסמא")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "אימייל")]
        public string Email { get; set; }
    }

    public class RegisterPlusViewModel
    {
        public RegisterViewModel RegistrationDetails { get; set; }
        public SubscriptionViewModel SubscriptionDetails { get; set; }

        public bool HaveSubscription { get; set; }

        [Display(Name = "שלח סמס הצטרפות?")]
        public bool SendWelcomeSMS { get; set; }

    }
}
