using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using InShapeModels;
using Utilities;

namespace DAL
{
    public static class ReportRepo
    {
        public static List<ClassReportUsers> GetClassReportUsers(int classid)
        {
            using (var context = new InShapeEntities())
            {
                var classidParameter = new SqlParameter("@classid", classid);

                var result = context.Database
                    .SqlQuery<ClassReportUsers>("GetReportData @classId", classidParameter)
                    .ToList();

                return result;
            }
        }

        public static List<UserWithSubscription> GetWeeklyReporbyDay(int id)
        {
            throw new NotImplementedException();
        }

        public static WeeklyReportModel GetWeeklySummaryReport(DateTime startDate, int StudioId)
        {
            //var start = DateTime.UtcNow.ToLocal();
            var endDate = startDate.AddDays(7);
            var result = new WeeklyReportModel();
            result.WeeklyDates = new List<DailyStatsModel>();
            var TrailRolIid = ((int)UserType.TrailUser).ToString();
            var c = UserType.ClassInstructor.ToString("d");
            using (var context = new InShapeEntities())
            {
                result.WeeklyReportDetails = AutoMapper.Mapper.Map<List<StudioClassReportModel>>(
                    context.Classes.Where(x => x.Date >= startDate && x.Date < endDate && !x.IsDeleted)
                    .FilterByUser(StudioId)
                    //.OrderBy(x=>x.Date)
                    //.Include("ClassEnrollments")
                        //.ProjectTo<StudioClassModel>()
                        .ToList());
                //result.WeeklyReportDetails.ForEach(x=> x.UsePlacements = context.ClassAvailablePlacements.Any(cl => cl.ClassId == x.Id && !cl.IsDeleted));
                //var end = DateTime.UtcNow.ToLocal();
                //Logger.WriteDebug($"time: {(end - start).Seconds} seconds");
                result.DailySlots =
                    context.ClassDailySlots.Where(x => x.IsDeleted == false)
                        .OrderBy(y => y.StartTime)
                        .FilterByUser(StudioId)
                        .ProjectTo<DailySlotModel>()
                        .ToList();
                result.StudioRooms =
                    context.StudioRooms.Where(x => x.IsDeleted == false)
                    .FilterByUser(StudioId).ProjectTo<StudioRoomModel>().ToList();
                //result.ClassTypes =
                //    context.ClassTypes.ProjectTo<ClassTypeModel>().ToList();
                //end = DateTime.UtcNow.ToLocal();
                //Logger.WriteDebug($"time: {(end - start).Seconds} seconds");
                result.Instructors = context.InstructorDetails
                    .Where(x => x.AspNetUser.AspNetUserRoles.Any(r => r.RoleId == c))
                    .Include("AspNetUser")
                    .FilterByUser(StudioId)
                    .ProjectTo<InstructorDetailsModel>().ToList();
                //for (int i = 0; i < 7; i++)
                //{
                //    var dailystats = new DailyStatsModel();
                //    dailystats.Date = startDate.AddDays(i);
                //    var from = startDate.AddDays(i).Date;
                //    var to = startDate.AddDays(i+1).Date;
                //    dailystats.MissedParticipants =
                //        context.ClassEnrollments.Where(
                //            e => e.IsDeleted == false && e.Class.Date >= from && e.Class.Date < to).FilterByUser(StudioId)
                //            .Count(v => !v.IsVerified);
                //    dailystats.TrailParticipants =
                //        context.ClassEnrollments.FilterByUser(StudioId).Count(e => e.IsDeleted == false && e.Class.Date >= from && e.Class.Date < to &&
                //                e.UserSubscription.AspNetUser.AspNetUserRoles.Any(r => r.RoleId == TrailRolIid));
                //    result.WeeklyDates.Add(dailystats);
                //}
            }
            //var end2 = DateTime.UtcNow.ToLocal();
            //Logger.WriteDebug($"time: {(end2 - start).Seconds} seconds");
            //CalculateDailyStats(result, startDate, TrailRolIid);
            Logger.WriteDebug($"Got {result.WeeklyReportDetails.Count} Classes, for Studio: {StudioId}");
            //end2 = DateTime.UtcNow.ToLocal();
            //Logger.WriteDebug($"time: {(end2-start).Seconds} seconds");
            //GetDailys(result, startDate);
            result.WeeklyDates = GetWeeklySummaryFooter(startDate, StudioId);
            return result;
        }

        private static void GetDailys(WeeklyReportModel result, DateTime startDate)
        {
            for (int i = 0; i < 7; i++)
            {
                var dailystats = new DailyStatsModel();
                dailystats.Date = startDate.AddDays(i);
                result.WeeklyDates.Add(dailystats);
            }
        }

        private static void CalculateDailyStats(WeeklyReportModel result, DateTime startDate, string trailRolIid)
        {
            for (int i = 0; i < 7; i++)
            {
                var dailystats = new DailyStatsModel();
                dailystats.Date = startDate.AddDays(i);
                var from = startDate.AddDays(i).Date;
                var to = startDate.AddDays(i + 1).Date;
                //dailystats.MissedParticipants = result.WeeklyReportDetails.Where(e => e.Date >= from && e.Date < to && e.Enrollments != null)
                //    .SelectMany(x => x.Enrollments.Where(z => !z.IsVerified && !z.IsDeleted)).Count();
                ////context.ClassEnrollments.Where(
                ////    e => e.IsDeleted == false && e.Class.Date >= from && e.Class.Date < to).FilterByUser(StudioId)
                ////    .Count(v => !v.IsVerified);
                //dailystats.TrailParticipants = result.WeeklyReportDetails.Where(e => e.Date >= from && e.Date < to && e.Enrollments != null)
                //    .SelectMany(x => x.Enrollments.Where(z => !z.IsDeleted
                //&& z.UserSubscription.Roles.Any(r => r.RoleId == trailRolIid)
                //)).Count();
                ////context.ClassEnrollments.FilterByUser(StudioId).Count(e => e.IsDeleted == false && e.Class.Date >= from && e.Class.Date < to &&
                //        e.UserSubscription.AspNetUser.AspNetUserRoles.Any(r => r.RoleId == TrailRolIid));
                result.WeeklyDates.Add(dailystats);
            }
        }


        public static List<DailyStatsModel> GetWeeklySummaryFooter(DateTime startDate, int StudioId)
        {
            using (var context = new InShapeEntities())
            {
                var startDateParameter = new SqlParameter("@StartDate", startDate);
                var StudioIdParameter = new SqlParameter("@StudioId", StudioId);
                var result = context.Database
                    .SqlQuery<DailyStatsModel>("GetWeeklyFooter @StartDate, @StudioId", startDateParameter, StudioIdParameter)
                    .ToList();
                for (int i = 0; i < 7; i++)
                {
                    var date = startDate.AddDays(i);
                    if (!result.Any(r=>r.Date == date))
                    {
                        var dailystats = new DailyStatsModel();
                        dailystats.Date = date;
                        result.Add(dailystats);
                    }
                }
                return result.OrderBy(d=>d.Date).ToList();
            }
        }

        public static WeeklyReportModel GetMonthlySummaryReport(DateTime startDate, DateTime EndDate, int StudioId)
        {
            EndDate = EndDate.AddDays(1);
            var result = new WeeklyReportModel();
            result.WeeklyDates = new List<DailyStatsModel>();
            using (var context = new InShapeEntities())
            {
                result.WeeklyReportDetails = AutoMapper.Mapper.Map<List<StudioClassReportModel>>(
                    context.Classes.Where(x => x.Date >= startDate && x.Date < EndDate && !x.IsDeleted)
                    .FilterByUser(StudioId)
                    //.ProjectTo<StudioClassModel>()
                    .ToList());
                result.DailySlots =
                    context.ClassDailySlots.Where(x => x.IsDeleted == false)
                    .FilterByUser(StudioId)
                    .OrderBy(y => y.StartTime)
                    .ProjectTo<DailySlotModel>()
                    .ToList();
                result.StudioRooms =
                    context.StudioRooms.Where(x => x.IsDeleted == false).FilterByUser(StudioId).ProjectTo<StudioRoomModel>().ToList();
                result.ClassTypes =
                    context.ClassTypes.Where(x => x.IsDeleted == false).FilterByUser(StudioId).ProjectTo<ClassTypeModel>().ToList();
                for (int i = 0; i < EndDate.Date.AddDays(-1).Day; i++)
                {
                    var dailystats = new DailyStatsModel();
                    dailystats.Date = startDate.AddDays(i);
                    var from = startDate.AddDays(i).Date;
                    var to = startDate.AddDays(i + 1).Date;
                    dailystats.MissedParticipants =
                        context.ClassEnrollments.Where(
                            e => e.IsDeleted == false && e.Class.Date >= from && e.Class.Date < to)
                            .Count(v => !v.IsVerified);
                    result.WeeklyDates.Add(dailystats);
                }
            }
            return result;
        }

        public static List<InstructorReportModel> GetInstructorMonthlyReport(DateTime month, int StudioId)
        {
            //var result = new List<InstructorReportModel>();
            using (var context = new InShapeEntities())
            {
                var fromDateParameter = new SqlParameter("@FromDate", month);
                //var toDateParameter = new SqlParameter("@ToDate", todate ?? DateTime.UtcNow.ToLocal().Date);
                var companyParameter = new SqlParameter("@StudioId", StudioId);
                var result = context.Database
                    .SqlQuery<InstructorReportModel>("GetInstructorReport @FromDate, @StudioId",
                        fromDateParameter, companyParameter).AsQueryable().ToList();

                return result;
            }
        }

        public static List<ExpensesModel> GetExpensesMonthlyReport(DateTime month, int StudioId)
        {
            var fromdate = new DateTime(month.Year,month.Month,1);
            var enddate = new DateTime(month.AddMonths(1).Year, month.AddMonths(1).Month, 1);
            using (var context = new InShapeEntities())
            {
                return
                    context.StudioExpenses.Where(x => !x.IsDeleted && x.Date >= fromdate && x.Date < enddate).FilterByUser(StudioId)
                        .ProjectTo<ExpensesModel>().ToList();
            }
        }
        

        public static UsersGraphModel GetuserUsersGraph(DateTime? fromdate, DateTime? todate, int StudioId)
        {
            UsersGraphModel ug = new UsersGraphModel();
            using (var context = new InShapeEntities())
            {
                var fromDateParameter = fromdate != null ? new SqlParameter("@FromDate", fromdate.Value) : new SqlParameter("@FromDate", DBNull.Value);
                var toDateParameter = new SqlParameter("@ToDate", todate ?? DateTime.UtcNow.ToLocal().Date);
                var companyParameter = new SqlParameter("@StudioId", StudioId);
                var result = context.Database
                    .SqlQuery<DateUsers>("GetUserGraph @FromDate, @ToDate, @StudioId", fromDateParameter, toDateParameter, companyParameter);

                ug.UsersPerDay = result.OrderBy(x=>x.Date).ToList();
                ug.ActiveUsers = context.AspNetUsers.Where(u => u.AspNetUserRoles.Any(x => x.RoleId == "2")).FilterByUser(StudioId).Count(x => x.Active);
                ug.InActiveUsers = context.AspNetUsers.Where(u => u.AspNetUserRoles.Any(x => x.RoleId == "2")).FilterByUser(StudioId).Count(x => !x.Active);
            }
            return ug;
        }
    }
}
                                                                                                                                  