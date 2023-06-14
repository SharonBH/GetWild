using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using InShapeModels;

namespace BLL
{
    public static class ReportBLL
    {
        public static string ClassReport(int classid, int StudioId, out string ClassName)
        {
            var c = StudioRepo.GetClasses(classid, true, StudioId);
            ClassName = c.First().Name;
            if (c == null) return string.Empty;
            var reportdata = ReportRepo.GetClassReportUsers(classid);
            var report = new StringBuilder();
            //add header
            report.AppendLine("\uFEFFשם מתאמנת, " + ClassName);
            foreach (var user in reportdata)
            {
                report.AppendLine(string.Format("{0},{1}", user.Name, user.Participate.HasValue && user.Participate.Value == 1 ? "X" : "" ));
            }
            return report.ToString();
        }

        //public static string RoomReport(int roomid, out string RoomName)
        //{
        //    var r = StudioRepo.GetStudioRooms(roomid);
        //    RoomName = r.First().Name;
        //    if (r == null) return string.Empty;
        //    var classes = StudioRepo.GetSClassesForReporting(roomid);

        //    //string reportheader = "\uFEFFשם מתאמנת, " + string.Join(",", classes.Select(x => x.Name).ToArray());
        //    var report = new StringBuilder();
        //    report.AppendLine("\uFEFFשם מתאמנת, " + string.Join(",", classes.Select(x => x.Name).ToArray()));
        //    var reportusers = new Array List<ClassReportUsers>(); 
        //    foreach (var c in classes)
        //    {
        //        reportusers.Add(ReportRepo.GetClassReportUsers(c.Id));
        //    }

        //    return report.ToString();
        //}

        public static bool ProcessUser(string UserId, string Note)
        {
            return UserRepo.ProcessUser(UserId, Note);
        }

        public static bool ConfirmSalary(string userId, double Adjustment, DateTime date, string note, string userConfirmed)
        {
            return UserRepo.ConfirmSalary(userId, Adjustment, date, note, userConfirmed);
        }

        //public static List<UserWithSubscription> GetWeeklyReporbyDay(int id)
        //{
        //    return ReportRepo.GetWeeklyReporbyDay(id);
        //}

        public static WeeklyReportModel GetWeeklySummaryReport(int currentweekno, int StudioId)
        {
            var startdate = Utilities.Utils.FirstDateOfWeek(DateTime.UtcNow.Year, currentweekno);
            var result = ReportRepo.GetWeeklySummaryReport(startdate, StudioId);
            result.WeekNo = currentweekno;
            result.StartDate = startdate;
            result.EndDate = startdate.AddDays(6);
            CalculateTotals(result);
            return result;
        }

        public static List<DailyStatsModel> GetWeeklySummaryFooter(int currentweekno, int StudioId)
        {
            var startdate = Utilities.Utils.FirstDateOfWeek(DateTime.UtcNow.Year, currentweekno);
            var result = ReportRepo.GetWeeklySummaryFooter(startdate, StudioId);
            //result.WeekNo = currentweekno;
            //result.StartDate = startdate;
            //result.EndDate = startdate.AddDays(6);
            //CalculateTotals(result);
            return result;
        }

        public static WeeklyReportModel GetMonthlySummaryReport(DateTime fromdate, DateTime todate, int StudioId)
        {
            var StartDate = fromdate; //new DateTime(date.Year, date.Month, 1);
            var EndDate = fromdate == todate ? StartDate.AddMonths(1) : todate; //StartDate.AddMonths(1);
            var result = ReportRepo.GetMonthlySummaryReport(StartDate, EndDate, StudioId);
            result.StartDate = StartDate;
            result.EndDate = EndDate;
            CalculateMonthlyTotals(result); 
            return result;
        }

        public static InstructorReport GetInstructorMonthlyReport(DateTime month, int StudioId)
        {
            var result = new InstructorReport { ReportDate = month };
            var StartDate = month; //new DateTime(date.Year, date.Month, 1);
            //var EndDate = fromdate == todate ? StartDate.AddMonths(1) : todate; //StartDate.AddMonths(1);
            result.InstructorReportList = ReportRepo.GetInstructorMonthlyReport(StartDate, StudioId);
            //result.StartDate = StartDate;
            //CalculateMonthlyTotals(result);
            return result;
        }

        public static ExpensesReport GetExpensesMonthlyReport(DateTime month, int StudioId)
        {
            var result = new ExpensesReport { ReportDate = month};
            var StartDate = month; //new DateTime(date.Year, date.Month, 1);
            //var EndDate = fromdate == todate ? StartDate.AddMonths(1) : todate; //StartDate.AddMonths(1);
            result.ExpensesReportList = ReportRepo.GetExpensesMonthlyReport(StartDate, StudioId);
            //result.StartDate = StartDate;
            //CalculateMonthlyTotals(result);
            return result;
        }

        private static void CalculateTotals(WeeklyReportModel result)
        {
            foreach (var dailySlot in result.DailySlots.Where(x=> x.Id != -1))
            {
                dailySlot.Participants =
                    result.WeeklyReportDetails.Where(x => x.DailySlotId == dailySlot.Id || x.Time.Hour == dailySlot.StartTime.Hours).Sum(x => x.Participants + x.ExtraParticipants);
                var classes =
                    result.WeeklyReportDetails.Where(x => x.DailySlotId == dailySlot.Id || x.Time.Hour == dailySlot.StartTime.Hours)
                        //.GroupBy(y => y.Date.Date)
                        .Count();
                dailySlot.DailyClasses = classes;
                dailySlot.AVGParticipants = classes == 0 ? 0 : dailySlot.Participants / (double)classes;
            }
        }

        private static void CalculateMonthlyTotals(WeeklyReportModel result)
        {
            foreach (var dailySlot in result.DailySlots)
            {
                dailySlot.Participants =
                    result.WeeklyReportDetails.Where(x => x.DailySlotId == dailySlot.Id).Sum(x => x.Participants + x.ExtraParticipants);
                var count =
                    result.WeeklyReportDetails.Sum(x => x.Participants + x.ExtraParticipants);
                dailySlot.AVGParticipants = count == 0 ? 0 : dailySlot.Participants * 100 / (double)count;
            }
        }

        public static UsersGraphModel GetUsersGraph(DateTime? fromdate, DateTime? todate, int StudioId)
        {
            return ReportRepo.GetuserUsersGraph(fromdate, todate, StudioId);
        }

    }
}
