using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Text;
using System.Web;

namespace Utilities
{
    public static class Utils
    {
         
        //private static readonly string CancellationThreshold = ConfigurationManager.AppSettings["CanelationThresholdMins"];
        //public static int CancellationThresholdMins = Convert.ToInt32(CancellationThreshold) *-1;

    public static int GetIso8601WeekOfYear(DateTime time)
        {
            DayOfWeek day = CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(time);
            //if (day >= DayOfWeek.Sunday && day <= DayOfWeek.Tuesday)
            //{
            //    time = time.AddDays(3);
            //}

            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        public static int GetIso8601WeekOfYear()
        {
            var time = DateTime.UtcNow.ToLocal();
            DayOfWeek day = CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(time);
            //if (day >= DayOfWeek.Sunday && day <= DayOfWeek.Tuesday)
            //{
            //    time = time.AddDays(3);
            //}

            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        public static DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            CultureInfo ci = CultureInfo.CurrentCulture;
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)ci.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            DateTime firstWeekDay = jan1.AddDays(daysOffset);
            int firstWeek = ci.Calendar.GetWeekOfYear(jan1, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
            if (firstWeek <= 1 || firstWeek > 50)
            {
                weekOfYear -= 1;
            }
            return firstWeekDay.AddDays(weekOfYear * 7);
        }

        
        public static DateTime FirstDateOfWeek()
        {
            return DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
        }

        public static string GetWeekHeader(int weekOfYear)
        {
            var start = FirstDateOfWeek(DateTime.Now.Year, weekOfYear);
            var end = start.Date.AddDays(6);
            return string.Format("{0} עד {1}", start.ToString("MMMM dd"), end.ToString("MMMM dd"));
        }

        public static readonly string[] ValidImageTypes =
        {
            "image/gif",
            "image/jpeg",
            "image/pjpeg",
            "image/png"
        };

        //public static string GetTextForGender(string gender)
        //{
        //    switch (gender)
        //    {
        //        case "Female":
        //            return App.CurrentCompany.UseAgeforGender ? "מבוגרים" : "נקבה";
        //        case "Male":
        //            return App.CurrentCompany.UseAgeforGender ? "ילדים" : "זכר";
        //        case "Both":
        //            return "מעורב";
        //        default:
        //            return "";
        //    }
        //}

        public static bool SaveImage(byte[] imageData, string imagePath, bool scale = true)
        {
            var result = false;
            MemoryStream ms = new MemoryStream(imageData);
            Image originalImage = Image.FromStream(ms);

            if (originalImage.PropertyIdList.Contains(0x0112))
            {
                int rotationValue = originalImage.GetPropertyItem(0x0112).Value[0];
                switch (rotationValue)
                {
                    case 1: // landscape, do nothing
                        break;

                    case 8: // rotated 90 right
                        // de-rotate:
                        originalImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate270FlipNone);
                        break;

                    case 3: // bottoms up
                        originalImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate180FlipNone);
                        break;

                    case 6: // rotated 90 left
                        originalImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate90FlipNone);
                        break;
                }
            }
            try
            {
                if (scale) ScaleImage(originalImage, 400, 800).Save(imagePath);
                else originalImage.Save(imagePath);
                result = true;
            }
            catch (Exception ex)
            {
                Logger.WriteError("Error on Sign HealthTandC", ex);
            }
            return result;
            //originalImage.Save(imagePath);
        }

        //public static void ImageFromBase64(string base64Img)
        //{
        //    var bytes = Convert.FromBase64String(base64Img);
        //    using (var img = new FileStream(@"result.svg", FileMode.Create))
        //    {
        //        img.Write(bytes, 0, bytes.Length);
        //        img.Flush();
        //    }
        //}

        private static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        //public static DateTime ConvertDateTimeToLocal(DateTime DateTimeinUTC)
        //{
        //    var timeUtc = DateTimeinUTC; //DateTime.UtcNow;
        //    TimeZoneInfo IsrealTime = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
        //    return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, IsrealTime);
        //}

        //public static DateTime ConvertLocalDateTimeToUTC(DateTime LocalDateTime)
        //{
        //    TimeZoneInfo IsrealTime = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
        //    return TimeZoneInfo.ConvertTimeToUtc(LocalDateTime, IsrealTime);
        //}

        //public static DateTime ConvertNowToLocal()
        //{
        //    var timeUtc = DateTime.UtcNow;
        //    TimeZoneInfo IsrealTime = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
        //    return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, IsrealTime);
        //}

        public static string stringGetFilePath()
        {
            return Path.Combine(HttpContext.Current.Server.MapPath("/"), "Configuration.config");
        }

        public static bool ValidateID(string idNumberString)
   {
       if (idNumberString.Length< 9)
           return false;
  
       int int1 = Convert.ToInt32(idNumberString.Substring(0, 1)) * 1;
       int int2 = Convert.ToInt32(idNumberString.Substring(1, 1)) * 2;
       int int3 = Convert.ToInt32(idNumberString.Substring(2, 1)) * 1;
       int int4 = Convert.ToInt32(idNumberString.Substring(3, 1)) * 2;
       int int5 = Convert.ToInt32(idNumberString.Substring(4, 1)) * 1;
       int int6 = Convert.ToInt32(idNumberString.Substring(5, 1)) * 2;
       int int7 = Convert.ToInt32(idNumberString.Substring(6, 1)) * 1;
       int int8 = Convert.ToInt32(idNumberString.Substring(7, 1)) * 2;
       int int9 = Convert.ToInt32(idNumberString.Substring(8, 1)) * 1;
  
       if (int1 > 9) int1 = (int1 % 10) + 1;
       if (int2 > 9) int2 = (int2 % 10) + 1;
       if (int3 > 9) int3 = (int3 % 10) + 1;
       if (int4 > 9) int4 = (int4 % 10) + 1;
       if (int5 > 9) int5 = (int5 % 10) + 1;
       if (int6 > 9) int6 = (int6 % 10) + 1;
       if (int7 > 9) int7 = (int7 % 10) + 1;
       if (int8 > 9) int8 = (int8 % 10) + 1;
       if (int9 > 9) int9 = (int9 % 10) + 1;
  
       int sumOfAllInts = int1 + int2 + int3 + int4 + int5 + int6 + int7 + int8 + int9;
  
       sumOfAllInts = sumOfAllInts % 10;
       return sumOfAllInts == 0;
   }

        public static string GetCssforTypeActive(string UserType, bool Active, bool frozen)
        {
            if (frozen) return "FrozenUser";
            var css = new StringBuilder();
            css.Append(Active ? UserType == "TrailUser" ? "ActiveTrailUser" : "ActiveUser" : "");
            //if (UserType == UserType.TrailUser) css.Append(" TrailUser");
            //if (Frozen) css.Append(" InctiveUser");
            css.Append(!Active ? UserType == "TrailUser" ? "InctiveTrailUser" : "InctiveUser" : "");
            //if (Frozen) css.Append(" FrozenUser");
            return css.ToString();
        }

        public static string GetColorforTypeActive(string UserType, bool Active)
        {
            if (UserType == "Frozen") return "#800000";
            var css = new StringBuilder();
            css.Append(Active ? UserType == "TrailUser" ? "#006400" : "#0000ff" : "");
            //if (UserType == UserType.TrailUser) css.Append(" TrailUser");
            //if (Frozen) css.Append(" InctiveUser");
            css.Append(!Active ? UserType == "TrailUser" ? "#111111" : "#ff4500" : "");
            //if (Frozen) css.Append(" FrozenUser");
            return css.ToString();
        }

        //public static int GetAgeGroupByAge(int age)
        //{
        //    if (age == 0) return 1;
        //    return age >= App.CurrentCompany.AdultAge ? 3 : age >= App.CurrentCompany.TeenAge ? 2 : 1;
        //}

    }

    public static class Extentions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType().GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                .Name;
        }

        public static DateTime ToLocal(this DateTime dt)
        {
            TimeZoneInfo IsrealTime = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(dt, IsrealTime);
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime StartOfWeek(this DateTime dt)
        {
            int diff = (7 + (dt.DayOfWeek - DayOfWeek.Sunday)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        //public static IQueryable<T> FilterByCompany<T>(this IQueryable<T> source)
        //{
        //    Logger.WriteDebug($"FilterByCompany: {App.CurrentCompany.Id}");
        //    var properties = typeof (T).GetProperties();
        //    if (properties.Any(p => p.Name == "CompanyId"))
        //        return source.Where("CompanyId == " + App.CurrentCompany.Id); 
        //    if (properties.Any(p => p.Name == "Studio"))
        //        return source.Where("Studio.CompanyId == " + App.CurrentCompany.Id);
        //    if (properties.Any(p => p.Name == "StudioRoom"))
        //        return source.Where("StudioRoom.Studio.CompanyId == " + App.CurrentCompany.Id);
        //    if (properties.Any(p => p.Name == "UserSubscription"))
        //        return source.Where("UserSubscription.AspNetUser.Studio.CompanyId == " + App.CurrentCompany.Id);
        //    if (properties.Any(p => p.Name == "AspNetUser"))
        //        return source.Where("AspNetUser.Studio.CompanyId == " + App.CurrentCompany.Id);
        //    return source.Take(0);
        //}

        public static IQueryable<T> FilterByUser<T>(this IQueryable<T> source, int StudioId)
        {
            //Logger.WriteDebug($"FilterByUser: {StudioId}");
            var properties = typeof(T).GetProperties();
            if (properties.Any(p => p.Name == "StudioId"))
                return source.Where("StudioId == " + StudioId);
            if (properties.Any(p => p.Name == "StudioRoom"))
                return source.Where("StudioRoom.StudioId == " + StudioId);
            if (properties.Any(p => p.Name == "UserSubscription"))
                return source.Where("UserSubscription.AspNetUser.StudioId == " + StudioId);
            if (properties.Any(p => p.Name == "AspNetUser"))
                return source.Where("AspNetUser.StudioId == " + StudioId);
            return source.Take(0);
        }

    }

    //public class CurrentUser
    //{
    //    public string Email { get; set; }
        
    //    public string PhoneNumber { get; set; }
        
    //    //public ICollection<TRole> Roles { get; }
        
    //    public string Id { get; set; }

    //    public string UserName { get; set; }

    //    public string FirstName { get; set; }

    //    public string LastName { get; set; }

    //    public DateTime? DOB { get; set; }

    //    //public AgeGroup AgeGroup { get; set; }

    //    public string Address { get; set; }

    //    public DateTime JoinDate { get; set; }

    //    public DateTime? ProfileUpdateDate { get; set; }

    //    public string ProfileIMG { get; set; }

    //    public bool ReceiveSMS { get; set; }

    //    //public Profile UserProfile { get; set; }
    //    public string FullMame { get { return FirstName + " " + LastName; } }

    //    public string ProfileIMGPath { get { return string.IsNullOrEmpty(ProfileIMG) ? @"/images/Members/no-profile.jpg" : @"/images/Members/" + ProfileIMG; } }

    //    //public Gender Gender { get; set; }

    //    public bool AcceptedTandC { get; set; }

    //    public bool SignedHealthTandC { get; set; }

    //    public DateTime? SignedDate { get; set; }


    //    public long? CitizenId { get; set; }

    //    public string Occupation { get; set; }

    //    public int StudioId { get; set; }

    //    public bool Active { get; set; }
    //}

}
