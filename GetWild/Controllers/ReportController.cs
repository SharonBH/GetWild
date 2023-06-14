using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BLL;
using GetWild.Models;
using InShapeModels;
using Microsoft.AspNet.Identity;
using Utilities;

namespace GetWild.Controllers
{
    [Authorize(Roles = "Instructor, ClassInstructor, admin")]
    public class ReportController : InShapeMVCController
    {

        // GET: Studio
        public ActionResult Index()
        {
            var rs = new ReportByClassModel {StudioRooms = StudioBLL.GetStudioRooms(0, CurrentUser.StudioId) };
            return View(rs);
        }

        [Authorize(Roles = "Instructor, admin")]
        public ActionResult UserReport()
        {
            var rs = UserBll.GetUsersReport(5, CurrentUser.StudioId);
            return View(rs);
        }

        [Authorize(Roles = "Instructor, admin")]
        public ActionResult FrozenReport()
        {
            var rs = UserBll.GetFrozenReport(CurrentUser.StudioId);
            return View(rs);
        }

        [Authorize(Roles = "Instructor, admin")]
        public ActionResult UpdateUnfreeze(int subscriptionId)
        {
            var result = UserBll.UpdateSubscriptionUnFreeze(subscriptionId, User.Identity.GetUserId());
            var rs = UserBll.GetFrozenReport(CurrentUser.StudioId);
            return PartialView("_FrozenList", rs);
        }


        public ActionResult ProcessUser(string UserId, string Note)
        {
            ReportBLL.ProcessUser(UserId, Note);
            return PartialView("UserReportList", UserBll.GetUsersReport(3, CurrentUser.StudioId));
        }

        public ActionResult ProcessInstructorSalary(string UserId, double Adjustment, string date, string Note)
        {
            DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var result = ReportBLL.ConfirmSalary(UserId, Adjustment, dt, Note, User.Identity.GetUserId());
            if (result) return PartialView("InstructorMonthlyTable", ReportBLL.GetInstructorMonthlyReport(dt, CurrentUser.StudioId));
            return Json(new {Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר."});
            //return PartialView("InstructorMonthlyTable", ClassBLL.);
        }
        


        //public ActionResult weeklyReport(int id)
        //{

        //    return PartialView("UserReportList", ReportBLL.GetWeeklyReporbyDay(id));
        //}


        [AllowAnonymous]
        public ActionResult GetClassesForRoom(int roomid)
        {
            return PartialView("ClassReportList", StudioBLL.GetSClassesForReporting(roomid));
        }

        [Authorize(Roles = "Instructor, admin")]
        public FileContentResult ExportClass(int classid)
        {
            string Classname;
            string csv = ReportBLL.ClassReport(classid, CurrentUser.StudioId, out Classname);
            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", Classname + ".csv");
        }


        public ActionResult GetWeeklyTable(int? weekno)
        {
            var isInstructor = User.IsInRole("ClassInstructor");
            var currentweekno = weekno ?? Utilities.Utils.GetIso8601WeekOfYear(DateTime.UtcNow.ToLocal());
            var classes = ReportBLL.GetWeeklySummaryReport(currentweekno, CurrentUser.StudioId);
            classes.CurrentCompany = CurrentCompany;
            if (isInstructor)
            {
                classes.WeeklyReportDetails.RemoveAll(c => !c.Instructors.Any(i => i.UserId == CurrentUser.Id));
            }
            ViewBag.ShowPublishbtn = classes.WeeklyReportDetails.Count(c => !c.Published) > 0;
            return PartialView("WeeklySummaryTable", classes);
        }

        public ActionResult GetWeeklyFooter(int? weekno)
        {
            //var isInstructor = User.IsInRole("ClassInstructor");
            var currentweekno = weekno ?? Utilities.Utils.GetIso8601WeekOfYear(DateTime.UtcNow.ToLocal());
            var classes = ReportBLL.GetWeeklySummaryFooter(currentweekno, CurrentUser.StudioId);
            //classes.CurrentCompany = CurrentCompany;
            //if (isInstructor)
            //{
            //    classes.WeeklyReportDetails.RemoveAll(c => !c.Instructors.Any(i => i.UserId == CurrentUser.Id));
            //}
            //ViewBag.ShowPublishbtn = classes.WeeklyReportDetails.Count(c => !c.Published) > 0;
            return PartialView("WeeklySummaryFooter", classes);
        }

        public ActionResult GetWeeklyNoTable(int? weekno)
        {
            var currentweekno = weekno ?? Utilities.Utils.GetIso8601WeekOfYear(DateTime.UtcNow.ToLocal());
            return PartialView("WeeklySummaryNoTable", ReportBLL.GetWeeklySummaryReport(currentweekno, CurrentUser.StudioId));
        }


        public ActionResult WeeklySummaryReport()
        {
            return View();
        }

        [Authorize(Roles = "Instructor, admin")]
        public ActionResult SMSSendReport()
        {
            var model = new SmsReport
            {
                Date = DateTime.UtcNow.ToLocal().Date
            };
            model.SentSMS = SMSBLL.GetSmsLog(model.Date, CurrentUser.StudioId);
            return View(model);
        }

        [Authorize(Roles = "Instructor, admin")]
        public ActionResult GetSMSList(DateTime? date)
        {
            //DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var model = new SmsReport
            {
                Date = date ?? DateTime.UtcNow.ToLocal().Date
            };
            model.SentSMS = SMSBLL.GetSmsLog(model.Date, CurrentUser.StudioId);
            return PartialView("SMSSendReportList", model.SentSMS);
        }


        public ActionResult GetMonthlyTable(DateTime? fromdate, DateTime? todate)
        {
            return PartialView("MonthlySummaryTable", ReportBLL.GetMonthlySummaryReport(fromdate ?? new DateTime(DateTime.UtcNow.ToLocal().Year, DateTime.UtcNow.ToLocal().Month, 1), todate ?? DateTime.UtcNow.ToLocal().Date, CurrentUser.StudioId));
        }

        [CompanyAuthorization("UseExpenses")]
        public ActionResult ExpensesMonthlyReport(string month)
        {
            ViewBag.Date = string.IsNullOrEmpty(month)
                ? DateTime.UtcNow.ToLocal().AddMonths(-1).ToString("MM/yyyy")
                : month;
            return View();
        }

        [CompanyAuthorization("UseExpenses")]
        public ActionResult GetExpensesMonthlyTable(string month)
        {
            var dt = month == null ? new DateTime(DateTime.UtcNow.ToLocal().Year,DateTime.UtcNow.ToLocal().Month-1, 1) 
                                   : DateTime.ParseExact(month, "MM/yyyy", CultureInfo.InvariantCulture);
            return PartialView("ExpensesMonthlyTable", ReportBLL.GetExpensesMonthlyReport(dt, CurrentUser.StudioId));
        }

        [CompanyAuthorization("UseInstructors")]
        public ActionResult InstructorMonthlyReport()
        {
            return View();
        }

        [CompanyAuthorization("UseInstructors")]
        public ActionResult GetInstructorsMonthlyTable(string month)
        {
            var dt = month == null ? new DateTime(DateTime.UtcNow.ToLocal().AddMonths(-1).Year, DateTime.UtcNow.ToLocal().AddMonths(-1).Month, 1)
                                   : DateTime.ParseExact(month, "MM/yyyy", CultureInfo.InvariantCulture);
            return PartialView("InstructorMonthlyTable", ReportBLL.GetInstructorMonthlyReport(dt, CurrentUser.StudioId));
        }


        public ActionResult MonthlySummaryReport()
        {
            return View();
        }


        public ActionResult GetGraph(string fromdate, string todate)
        {
            DateTime? dt1 = null, dt2 = null;
            if (!string.IsNullOrEmpty(fromdate)) dt1 = DateTime.ParseExact(fromdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(todate))  dt2 = DateTime.ParseExact(todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var graphdata = ReportBLL.GetUsersGraph(dt1, dt2, CurrentUser.StudioId);
            return PartialView("UsersChart", graphdata);
        } 

    }
}
