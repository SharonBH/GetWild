using CsvHelper;
using CsvHelper.Configuration;
using DAL;
using InShapeModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace BLL
{
    public class ExportCSV
    {
        public static byte[] ExportUserWithSubscription(int StudioId)
        {
            return WriteCsvToMemory(UserBll.GetUsersWithSubscription4Export(StudioId,true, -1).UserWithSubscriptions);
            
        }

        public static byte[] ExportWeeklyUserWithSubscription(int StudioId,int id, int? weekno, int ut = 0, bool includeForzen = false)
        {
            var users = UserBll.GetUsersWithSubscription(id, weekno ?? -1, StudioId, ut);
            if (!includeForzen) users.UserWithSubscriptions.RemoveAll(x => x.Frozen);
            return WriteCsvToMemory(users.UserWithSubscriptions);

        }

        public static byte[] ExportUserWithTicketSubscription(int StudioId, bool includefrozen, int ut)
        {
            return WriteCsvToMemory(UserBll.GetUsersWithTicketSubscription(StudioId, includefrozen, -1, ut).UserWithSubscriptions);

        }

        public static byte[] ExportUserWithNoEnrollments(int StudioId)
        {
            return WriteCsvToMemory(UserBll.GetUsersWithNoEnrollments(StudioId).UserWithSubscriptions);

        }

        public static byte[] ExportFrozenUsers(int StudioId)
        {
            return WriteFrozenToMemory(UserBll.GetFrozenReport(StudioId));

        }


        static  byte[] WriteCsvToMemory(IEnumerable<UserWithSubscription> records)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                csvWriter.Configuration.RegisterClassMap<UserWithSubscriptionClassMap>();
                csvWriter.WriteRecords(records);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }

        static byte[] WriteFrozenToMemory(IEnumerable<FrozenSubscriptionModel> records)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                csvWriter.Configuration.RegisterClassMap<FrozenExportClassMap>();
                csvWriter.WriteRecords(records);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }

        static byte[] WriteEnrollmentsToMemory(List<ClassEnrollmentModel> records)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                csvWriter.Configuration.RegisterClassMap<DailyEnrollmentstionsClassMap>();
                csvWriter.WriteRecords(records);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }



        static byte[] WriteDailyExportToMemory(List<DailyExportModel> records)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                csvWriter.Configuration.RegisterClassMap<DailyExportClassMap>();
                csvWriter.WriteRecords(records);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }

        public static byte[] ExportEnrollmentBydate(int StudioId, DateTime date, int? userRole, bool activated = false, bool isLateCancel = false)
        {
            var enrollments = ClassRepo.GetDailyExport(date, StudioId, userRole);
            if (activated) enrollments.RemoveAll(e => e.DaysSinceLastClass < 15);
            if (isLateCancel) enrollments.RemoveAll(e => !e.IsLateCancel);
            else enrollments.RemoveAll(e => e.IsLateCancel);
            return WriteDailyExportToMemory(enrollments);
        }
    }


    public sealed class UserWithSubscriptionClassMap : ClassMap<UserWithSubscription>
    {
        public UserWithSubscriptionClassMap()
        {

            Map(m => m.FullName).Name("שם");
            Map(m => m.FirstName).Name("פרטי");
            Map(m => m.LastName).Name("משפחה");
            Map(m => m.DOB).TypeConverterOption.Format("dd/MM/yyyy").Name("תאריך לידה");
            //Map(m => m.GetAgeGroup).Name("גיל");
            Map(m => m.Gender).Name("מין");
            Map(m => m.PhoneNumber).Name("טלפון");
            Map(m => m.Email).Name("דואל");
            Map(m => m.JoinDate).TypeConverterOption.Format("dd/MM/yyyy").Name("רישום"); //.Default("אין");
            Map(m => m.UserType).ConvertUsing(m => { var type = m.UserType.GetDisplayName(); return type; }).Name("סוג");
            Map(m => m.SubscriptionType).Default("-").Name("מנוי");
            Map(m => m.Active).ConvertUsing(m => { var act = m.Active.HasValue && m.Active.Value ? "כן" : "לא"; return act; }).Name("פעיל");
            Map(m => m.SubscriptionExpireDate).TypeConverterOption.Format("dd/MM/yyyy").Name("תפוגת מנוי");
            Map(m => m.PayEndDate).TypeConverterOption.Format("dd/MM/yyyy").Name("סיום תשלום");
            Map(m => m.ExtraDays).Default("-").Name("ימי חריגה");
            Map(m => m.NumClasses).Default("-").Name("אימונים");
            Map(m => m.ClassesDone).Default("-").Name("נוצל");
            //Map(m => m.SubscriptionStartDate).TypeConverterOption.Format("dd/MM/yyyy");
            Map(m => m.LastClassDate).Default("-").Name("אימון אחרון");
            Map(m => m.LastClassType).Default("-").Name("אימון אחרון");
            Map(m => m.WeeklyClasses).Default("-").Name("אימונים שבוע");
            Map(m => m.DaysSinceLastClass).Name("ימים ללא אימון");
            Map(m => m.NextClassDate).Default("-").Name("אימון הבא");
            Map(m => m.NextClassType).Default("-").Name("אימון הבא");
            Map(m => m.Address).Default("-").Name("כתובת");
            Map(m => m.Frozen).Default("-").Name("קפוא");
            Map(m => m.WeeklyClasses).Default("-");
        }

    }

    public sealed class DailyEnrollmentstionsClassMap : ClassMap<ClassEnrollmentModel>
    {
        public DailyEnrollmentstionsClassMap()
        {

            Map(m => m.UserSubscription.FirstName).Name("שם פרטי");
            Map(m => m.UserSubscription.LastName).Name("שם משפחה");
            Map(m => m.UserSubscription.PhoneNumber).Name("טלפון");
            Map(m => m.UserSubscription.Gender).Name("מין");
            Map(m => m.LastClass).Name("אימון אחרון");
            Map(m => m.UserTypeName).Name("מנוי");
        }

    }

    public sealed class DailyExportClassMap : ClassMap<DailyExportModel>
    {
        public DailyExportClassMap()
        {

            Map(m => m.FirstName).Name("שם פרטי");
            Map(m => m.LastName).Name("שם משפחה");
            Map(m => m.PhoneNumber).Name("טלפון");
            Map(m => m.Gender).Name("מין");
            Map(m => m.LastClass).Name("אימון");
            Map(m => m.Date).Name("תאריך");
            Map(m => m.NextClass).Name("אימון הבא");
            Map(m => m.NextClassDate).Name("הבא תאריך");
            Map(m => m.UserTypeName).Name("מנוי");
        }

    }

    public sealed class FrozenExportClassMap : ClassMap<FrozenSubscriptionModel>
    {
        public FrozenExportClassMap()
        {

            Map(m => m.UserSubscription.FirstName).Name("שם פרטי");
            Map(m => m.UserSubscription.LastName).Name("שם משפחה");
            Map(m => m.UserSubscription.PhoneNumber).Name("טלפון");
            Map(m => m.UserSubscription.Gender).Name("מין");
            Map(m => m.FrozenDetails).Name("פרטי הקפאה");
            Map(m => m.FreezeToDate).Name("סיום הקפאה");
            Map(m => m.DaysFrozen).Name("ימים בהקפאה");
            Map(m => m.UserSubscription.SubscriptionType.Name).Name("מנוי");
        }

    }


}