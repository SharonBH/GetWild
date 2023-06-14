using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InShapeModels
{
    public enum Gender
    {
        נקבה = 1,
        זכר = 2,
        מעורב = 9
    }

    public enum AgeGroup
    {
        ילדים = 1,
        נוער = 2,
        מבוגרים = 30
    }

    public enum ShortDay
    {
        א = 0,
        ב = 1,
        ג = 2,
        ד = 3,
        ה = 4,
        ו = 5,
        ש = 6
    }

    public enum MessageType
    {
        //[Display(Name = "הכל")]
        //All = 0,
        [Display(Name = "תוקף מנוי")]
        Expire = 1,
        [Display(Name = "פעילות")]
        Inactive = 4,
        [Display(Name = "תזכורת - אימון")]
        BeforeStart = 3,
        [Display(Name = "רישום לאתר")]
        Welcome = 6,
        [Display(Name = "סיום מנוי")]
        Finished = 5,
        [Display(Name = "רשימת המתנה")]
        WaitList = 7,
        [Display(Name = "רשימת המתנה - כולם")]
        BroadcastWaitList = 8,
        [Display(Name = "תזכורת מאמן")]
        InstructorReminder = 9,
        [Display(Name = "שליחה מהאתר")]
        MassSMS = 999
    }

    public enum SmsListType
    {
        All = 1,
        Active = 2,
        Inactive = 3,
        ByClass = 4,
        ByDay = 5,
        UserReport = 6, //דוח משתמשים
        WeeklyReport = 7, //דוח שבועי
        NoEnrollmentReport = 8, //דוח עצלנים
        Frozen = 9 //מנויים קפואים
    }

    public enum UserType
    {
        [Display(Name = "מנהל על")]
        admin            = 99,
        [Display(Name = "מנהל")]
        Instructor       = 1,
        [Display(Name = "מאמן")]
        ClassInstructor  = 11,
        [Display(Name = "מתאמן רגיל")]
        User             = 2,
        [Display(Name = "כרטיסיות")]
        FreeUser         = 3,
        [Display(Name = "מתאמן דמו")]
        DemoUser         = 4,
        [Display(Name = "מתאמן פוטנציאל")]
        TrailUser        = 5
    }

    public enum ParticipantType
    {
        [Display(Name = "מתאמן רגיל")]
        User = 2,
        [Display(Name = "כרטיסיות")]
        FreeUser = 3,
        [Display(Name = "מתאמן דמו")]
        DemoUser = 4,
        [Display(Name = "מתאמן פוטנציאל")]
        TrailUser = 5
    }

    public enum AdminType
    {
        [Display(Name = "מנהל על")]
        admin = 99,
        [Display(Name = "מנהל")]
        Instructor = 1,
        [Display(Name = "מאמן")]
        ClassInstructor = 11
    }

    //public static class UserTypeExtensions
    //{
    //    public static string ToFriendlyString(this UserType me)
    //    {
    //        switch (me)
    //        {
    //            case UserType.admin:
    //                return "מנהל על";
    //            case UserType.Instructor:
    //                return "מנהל";
    //            case UserType.ClassInstructor:
    //                return "מדריך";
    //            case UserType.User:
    //                return "מתאמן";
    //            case UserType.FreeUser:
    //                return "מתאמן חינם";
    //            case UserType.DemoUser:
    //                return "מתאמן דמו";
    //            case UserType.TrailUser:
    //                return "מתאמן פוטנציאל";
    //            default:
    //                throw new ArgumentOutOfRangeException(nameof(me), me, null);
    //        }
    //    }
    //}
}
