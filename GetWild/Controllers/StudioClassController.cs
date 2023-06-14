using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using BLL;
using GetWild.Models;
using InShapeModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Utilities;

namespace GetWild.Controllers
{
    [Authorize(Roles = "Instructor, ClassInstructor, admin")]
    public class StudioClassController : InShapeMVCController
    {
        //private ApplicationDbContext db = new ApplicationDbContext();

        // GET: StudioClass
        
        public ActionResult Index(int? weekno, bool inclAVG = false)
        {
            //ViewBag.EnableOutroll = true;
            var isInstructor = User.IsInRole("ClassInstructor");
            var week = weekno ?? Utils.GetIso8601WeekOfYear(DateTime.Now);
            ViewBag.WeekNo = week;
            ViewBag.Header = Utils.GetWeekHeader(ViewBag.WeekNo);
            var classes = StudioBLL.GetClassesByWeek(week, CurrentUser.StudioId, inclAVG);
            if (isInstructor)
            {
                var user = User.Identity.GetUserId();
                classes.RemoveAll(c => !c.Instructors.Any(i => i.UserId == user));
            }
            ViewBag.ShowPublishbtn = classes.Count(c => !c.Published) > 0;
            ClassesList list = new ClassesList { Classes = classes.OrderBy(x => x.Date), CurrentCompany = CurrentCompany };
            return View(list);
        }

        //public ActionResult GetClasseswithAVG(int? weekno)
        //{
        //    //ViewBag.EnableOutroll = true;
        //    var isInstructor = User.IsInRole("ClassInstructor");
        //    var week = weekno ?? Utils.GetIso8601WeekOfYear(DateTime.Now);
        //    ViewBag.WeekNo = week;
        //    ViewBag.Header = Utils.GetWeekHeader(ViewBag.WeekNo);
        //    var classes = StudioBLL.GetClassesByWeek(week, true);
        //    if (isInstructor)
        //    {
        //        var user = User.Identity.GetUserId();
        //        classes.RemoveAll(c => !c.Instructors.Any(i => i.UserId == user));
        //    }
        //    ViewBag.ShowPublishbtn = classes.Count(c => !c.Published) > 0;
        //    return View(classes.OrderBy(x => x.Date));
        //}

        public ActionResult GetNextClasses()
        {
            ClassesList classes = new ClassesList { Classes = StudioBLL.GetNextClasses(DateTime.UtcNow.Date.ToLocal(), CurrentUser.StudioId, 1).OrderBy(x => x.Date), CurrentCompany = CurrentCompany };
            //var classes = StudioBLL.GetNextClasses(DateTime.UtcNow.Date.ToLocal(), 1).OrderBy(x => x.Date);
            return PartialView("NextClassesTablePartial", classes);
        }

        public ActionResult GetWeeklyByDate(string date)
        {
            DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var week = Utils.GetIso8601WeekOfYear(dt);
            ViewBag.WeekNo = week;
            ViewBag.Header = Utils.GetWeekHeader(ViewBag.WeekNo);
            return View("Index",StudioBLL.GetClassesByWeek(week, CurrentUser.StudioId));
        }

        public ActionResult GetClassById(int classid, int? selectedPlacement)
        {
            var studioClassModel = StudioBLL.GetClasses(classid, true, CurrentUser.StudioId, true).FirstOrDefault();
            var model = Mapper.Map<ClassViewModel>(studioClassModel);
            model.CurrentCompany = CurrentCompany;
            if (model == null) return PartialView("BOEnrollToClassPartial-plc", null);
            if (selectedPlacement.HasValue) model.SelectedPlacementId = selectedPlacement.Value;
            return PartialView("BOEnrollToClassPartial-plc", model);
        }

        // GET: StudioClass/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    StudioClassModel studioClassModel = StudioBLL.GetClasses(id.Value, true).FirstOrDefault();
        //    if (studioClassModel == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(studioClassModel);
        //}

        [Authorize(Roles = "Instructor,admin")]
        public ActionResult Create(int? id, string source = "Index", int DailySlot = 0, int RoomId = 0, string Date = "")
        {
           
            Date = string.IsNullOrEmpty(Date) ? DateTime.UtcNow.ToLocal().ToString("dd/MM/yyyy") : Date;
            if (id == null)
            {
                var model = new ClassViewModel
                    {
                        ClassTypes = ClassBLL.GetClassTypes(0, CurrentUser.StudioId),
                        Placements = StudioBLL.GetStudioPlacements(0, CurrentUser.StudioId),
                        //ClassPlacements = new List<ClassPlacementModel>(),
                        StudioRooms = StudioBLL.GetStudioRooms(0, CurrentUser.StudioId),
                        TimeSlots = ClassBLL.GetClassTimeSlots(0, CurrentUser.StudioId),
                        Instructors = UserBll.GetInstructorList(CurrentUser.StudioId),
                        SourcePage = source,
                        DailySlotId = DailySlot,
                        StudioRoomId = RoomId,
                        Published = CurrentCompany.AutoPublish,
                        //Id = 0,
                        CurrentCompany = CurrentCompany,
                        Date = DateTime.ParseExact(Date,"dd/MM/yyyy",null)
                    };
                //foreach (var item in model.Placements)
                //{
                //    model.ClassPlacements.Add(new ClassPlacementModel { StudioPlacementId = item.Id, StudioPlacementName = item.Name });
                //}
                model.ClassTypesDetails = ClassBLL.GetClassTypesDetailsByType(model.ClassTypeId);
                if (CurrentCompany.UsePlacements)
                {
                    var places = ClassBLL.GetClassAvailablePlacements(0, CurrentUser.StudioId);
                    model.ClassAvailablePlacements = places.Where(x => x.TypeId == 1).ToList();
                    model.KangooClassAvailablePlacements = places.Where(x => x.TypeId == 2).ToList();
                    model.NumbersClassAvailablePlacements = places.Where(x => x.TypeId == 0).ToList();
                    return View("CreateWithPlacements", model);
                }
                return View(model);
            }
            StudioClassModel studioClassModel = StudioBLL.GetClasses(id.Value, true, CurrentUser.StudioId).First();
            if (studioClassModel == null)
            {
                return HttpNotFound();
            }
            var classmodel = Mapper.Map<ClassViewModel>(studioClassModel);
            classmodel.SourcePage = source;
            classmodel.ClassTypes = ClassBLL.GetClassTypes(0, CurrentUser.StudioId);
            classmodel.ClassTypesDetails = ClassBLL.GetClassTypesDetailsByType(classmodel.ClassTypeId, true);
            classmodel.TimeSlots = ClassBLL.GetClassTimeSlots(0, CurrentUser.StudioId);
            classmodel.StudioRooms = StudioBLL.GetStudioRooms(0, CurrentUser.StudioId);
            classmodel.Instructors = UserBll.GetInstructorList(CurrentUser.StudioId);
            classmodel.Placements = StudioBLL.GetStudioPlacements(0, CurrentUser.StudioId);
            classmodel.CurrentCompany = CurrentCompany;
            //classmodel.InstructorIds = classmodel.InstructorIds;
            if (classmodel.DailySlotId == -1) classmodel.Time = classmodel.Date;
            if (CurrentCompany.UsePlacements)
            {
                classmodel.KangooClassAvailablePlacements = classmodel.ClassAvailablePlacements.Where(x => x.TypeId == 2).ToList();
                classmodel.NumbersClassAvailablePlacements = classmodel.ClassAvailablePlacements.Where(x => x.TypeId == 0).ToList();
                classmodel.ClassAvailablePlacements.RemoveAll(x => x.TypeId != 1);
                //classmodel.ClassAvailablePlacements = ClassBLL.GetClassAvailablePlacements(classmodel.Id.Value);
                classmodel.UsePlacements = classmodel.AutoShowPlacements;
                classmodel.UseKangoo = classmodel.AutoShowKangooPlacements;
                classmodel.UseNumbers = classmodel.AutoShowNumbersPlacements;
                return View("CreateWithPlacements", classmodel);
            }
            return View(classmodel);

        }

        // POST: StudioClass/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Instructor,admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClassViewModel ClassViewModel)
        {
            if (!ModelState.IsValid) return View(ClassViewModel);
            var classmodel = Mapper.Map<StudioClassModel>(ClassViewModel);
            if (ClassViewModel.ClassTypeDetailsId == -1) classmodel.ClassTypeDetailsId = null;
            if (!ClassViewModel.UsePlacements) classmodel.ClassAvailablePlacements.Clear();
            if (ClassViewModel.UseKangoo) classmodel.ClassAvailablePlacements = ClassViewModel.KangooClassAvailablePlacements;
            else if (ClassViewModel.UseNumbers) classmodel.ClassAvailablePlacements = ClassViewModel.NumbersClassAvailablePlacements;
            if (ClassViewModel.DailySlotId != null && ClassViewModel.DailySlotId != -1 && ClassViewModel.DailySlotId != 1004 && ClassViewModel.DailySlotId != 1024)
            {
                var time = ClassBLL.GetClassTimeSlots(ClassViewModel.DailySlotId.Value, CurrentUser.StudioId).FirstOrDefault();
                if (time != null)
                {
                    classmodel.Date = classmodel.Date.Add(time.StartTime);
                    classmodel.Duration = time.Duration;
                }
            }
            else
            {
                var time = ClassViewModel.Time;
                if (time != null)
                {
                    classmodel.Date = classmodel.Date.Add(time.Value.TimeOfDay);
                    classmodel.Duration = ClassViewModel.Duration ?? 60;
                }
                //classmodel.DailySlotId = -1;
            }

            //classmodel.ClassPlacements.ForEach(x => x.ClassId = classmodel.Id);

            var userid = User.Identity.GetUserId();
                if (classmodel.Id == 0) StudioBLL.CreateClass(classmodel, userid, CurrentUser.StudioId, CurrentCompany.AutoPublish, CurrentCompany.UseClassNamefromType);
                else
                {
                    StudioBLL.UpdateClass(classmodel, userid, CurrentCompany.UseClassNamefromType);
                }

            switch (ClassViewModel.SourcePage)
            {
                case "Admin":
                    return RedirectToAction("Index", "Admin");
                case "WeeklyReport":
                    return RedirectToAction("WeeklySummaryReport", "Report", new { weekno = Utils.GetIso8601WeekOfYear(classmodel.Date)});
                case "Index":
                    return RedirectToAction("Index", new { weekno = Utils.GetIso8601WeekOfYear(classmodel.Date) });
                default:
                    return RedirectToAction("Index", new { weekno = Utils.GetIso8601WeekOfYear(classmodel.Date) });
            }
            //return ClassViewModel.SourcePage == "WeeklySummaryReport" ? RedirectToAction("WeeklySummaryReport", "Report") : RedirectToAction("Index");
        }

        [Authorize(Roles = "Instructor,admin")]
        public ActionResult DeleteConfirmed(int id, string source = "Index")
        {
            var deleted = StudioBLL.DeleteClass(id);
            if (!deleted) return Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." });
            switch (source)
            {
                case "Admin":
                    return RedirectToAction("Index", "Admin");
                case "WeeklyReport":
                    return RedirectToAction("WeeklySummaryReport", "Report");
                case "Index":
                    return RedirectToAction("Index");
                default:
                    return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Instructor,admin")]
        public JsonResult CopyCalander(string weekno)
        {
            var currentweekno = string.IsNullOrEmpty(weekno) ? Utils.GetIso8601WeekOfYear(DateTime.UtcNow.ToLocal()) : int.Parse(weekno);
            var result = StudioBLL.CopyWeeklyCalander(currentweekno, CurrentUser.StudioId, CurrentCompany.AutoPublish);
            return Json(result ? new { Response = "Success", Message = "הלוז השבועי שוכפל בהצלחה!." } : new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." });
        }

        [HttpPost]
        [Authorize(Roles = "Instructor,admin")]
        public JsonResult PublishCalander()
        {
            var currentweekno = Utils.GetIso8601WeekOfYear(DateTime.UtcNow.ToLocal())+1;
            var result = StudioBLL.PublishWeeklyCalander(currentweekno, CurrentUser.StudioId);
            return Json(result ? new { Response = "Success", Message = "הלוז השבועי פורסם בהצלחה!." } : new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." });
        }

        [HttpPost]
        [Authorize(Roles = "Instructor,admin")]
        public JsonResult UpdateClassLinks()
        {
            ServiceBLL.UpdateClassesURLs(CurrentUser.StudioId);
            return Json(new { Response = "Success", Message = "הבקשה נשלחה!" });
        }


        //public ActionResult ClassEnrollmentCancelConfirmed(int id, string userId)
        //{
        //    ClassBLL.OutrolltUserToClass(id);
        //    return RedirectToAction("ManageUserEnrollment", new { userid = userId });
        //}

        [Authorize(Roles = "Instructor,admin")]
        public ActionResult ManageUserEnrollment(string userid)
        {
            if (userid == null || string.IsNullOrEmpty(userid)) return new PartialViewResult();
            var enrollments = ClassBLL.GetEnrollmentsByUserDate(userid, DateTime.MinValue,false);
            ViewBag.UserId = userid;
            ViewBag.OldEnrollments = false;
            return PartialView("UserEnrolmentsPartial", enrollments.Where(e=> e.SubscriptionActive).ToList());
        }

        [Authorize(Roles = "Instructor,admin")]
        public ActionResult ManageUserOldEnrollment(string userid)
        {
            if (userid == null || string.IsNullOrEmpty(userid)) return new PartialViewResult();
            //var enrollments = ClassBLL.GetOldEnrollmentsByUser(userid);
            var enrollments = ClassBLL.GetEnrollmentsByUserDate(userid, DateTime.MinValue, false);
            ViewBag.UserId = userid;
            ViewBag.OldEnrollments = true;
            return PartialView("UserEnrolmentsPartial", enrollments.Where(e => !e.SubscriptionActive).ToList());
        }


        [HttpPost]
        public ActionResult AdminEnrollToClass(int classId, int? classAvailablePlacementId, string userId, bool WeeklyReport = false, bool UserManage = false)
        {
            var result = ClassBLL.EnrollUserToClass(classId, classAvailablePlacementId ?? 0, userId, false, CurrentCompany.NumAdvncedEnrollments, CurrentCompany.LateRegistration, CurrentCompany.LateCancelPenalty, true);
            return !result.Result ? Json(new { Response = "Error", Message = result.Error }) :
                UserManage ? RedirectToAction("ManageUserEnrollment", new { userid = userId }) : WeeklyReport ? RedirectToAction("GetWeeklyTable", "Report") : RefreshClasses(result.Date);
        }

        public ActionResult AdminEnrollToClassPlacement(int classId, int classAvailablePlacementId, string userId, bool WeeklyReport = false)
        {
            var result = ClassBLL.EnrollUserToClass(classId, classAvailablePlacementId, userId, false, CurrentCompany.NumAdvncedEnrollments, CurrentCompany.LateRegistration, CurrentCompany.LateCancelPenalty, true);
            var us = result.Error.Split(':');
            return !result.Result ? Json(new { Response = "Error", Message = result.Error }) :
                Json(new { Response = "Success", Message = result.Error, FullName = us[0], SubsciptionId = us[1] });

        }

        [HttpPost]
        public ActionResult AdminMarkNoShow(int EnrollmentId, bool Value)
        {
            var result = ClassBLL.AdminMarkNoShow(EnrollmentId, Value);
            return !result.Result
                ? Json(new {Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר."})
                : Json(new {Response = "Success", Message = "השינויים נשמרו בהצלחה!."});
            //return Json(new {Response = "true", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר."});
        }
        

        [HttpPost]
        public ActionResult AdminOutrollFromClass(int classId, int subscriptionId, bool WeeklyReport = false)
        {
            var result = ClassBLL.AdminCancelEnrollment(classId, subscriptionId, User.Identity.GetUserId());
            return !result.Result ? Json(new { Response = "Error", Message = result.Error }) :
                WeeklyReport ? RedirectToAction("GetWeeklyTable", "Report", new { weekno = result.Date }) : RefreshClasses(result.Date);
        }

        [HttpPost]
        public ActionResult AdminOutrollFromClassParticipants(int classId, int subscriptionId, bool WeeklyReport = false)
        {
            var result = ClassBLL.AdminCancelEnrollment(classId, subscriptionId, User.Identity.GetUserId());
            return !result.Result
                ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." })
                : Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה!." });
            //return !result.Result ? Json(new { Response = "Error", Message = result.Error }) :
            //    WeeklyReport ? RedirectToAction("GetWeeklyTable", "Report") : RefreshClasses(result.Date);
        }
        

        [HttpPost]
        public ActionResult AdminRemoveFromWaitList(int classId, int subscriptionId, bool WeeklyReport = false)
        {
            var result = ClassBLL.AdminRemoveFromWaitList(classId, subscriptionId, User.Identity.GetUserId());
            return !result.Result ? Json(new { Response = "Error", Message = result.Error }) :
                WeeklyReport ? RedirectToAction("GetWeeklyTable", "Report", new { weekno = result.Date }) : RefreshClasses(result.Date);
        }

        public PartialViewResult GetClassEnrollment(int id, bool WeeklyReport = false)
        {
           List<ClassEnrollmentModel> enrollments = ClassBLL.GetEnrollmentsByClass(id, true);
            //ViewBag.EnableOutroll = StudioBLL.GetClasses(id,true).First().Date >= DateTime.Now;
            ViewBag.WeeklyReport = WeeklyReport;
            return PartialView("ClassEnrolment", enrollments);
        }

        public ViewResult ManagePlacements(int classid, bool WeeklyReport = false)
        {
            var enrollments = ClassBLL.GetClassAvailablePlacements(classid, CurrentUser.StudioId, true, true);
            //ViewBag.EnableOutroll = StudioBLL.GetClasses(id,true).First().Date >= DateTime.Now;
            ViewBag.WeeklyReport = WeeklyReport;
            return View("EnrolementsWithPlacements", enrollments);
        }

        public ActionResult SaveUserComment(string UserId, string Note, int ClassId, int EnrollmentId, int CommentId)
        {
            var comment = new EnrollmentCommentModel{ Id = CommentId, ClassId = ClassId, EnrollmentId = EnrollmentId, UserId = UserId, UserCreated = CurrentUser.Id, Comment = Note, CreateDate = DateTime.UtcNow.ToLocal()};
            var result = ClassBLL.SaveUserComment(comment);
            return !result
                ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." })
                : Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה!." });
        }

        public ActionResult GetUserComment(string UserId, int EnrollmentId, int ClassId)
        {
            var comment = ClassBLL.GetUserComment(UserId, EnrollmentId);
            return PartialView("_CommentPopup", comment?? new EnrollmentCommentModel{ Id=0, Comment=string.Empty, EnrollmentId= EnrollmentId, UserId = UserId, ClassId = ClassId });
        }

        public ActionResult GetUserCommentInline(string UserId, int EnrollmentId, int ClassId)
        {
            var comment = ClassBLL.GetUserComment(UserId, EnrollmentId);
            return PartialView("_Comment_inline", comment ?? new EnrollmentCommentModel { Id = 0, Comment = string.Empty, EnrollmentId = EnrollmentId, UserId = UserId, ClassId = ClassId });
        }

        public PartialViewResult GetClassWaitListEnrollment(int id, bool WeeklyReport = false)
        {
            List<ClassEnrollmentModel> enrollments = ClassBLL.GetWaitListEnrollmentsByClass(id);
            //ViewBag.EnableOutroll = StudioBLL.GetClasses(id,true).First().Date >= DateTime.Now;
            ViewBag.WeeklyReport = WeeklyReport;
            return PartialView("ClassWaitListEnrolment", enrollments);
        }

        public PartialViewResult GetClassEnrollmentBydate(string date, int? userRole, bool? removeEmptyClasses)
        {
            DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //var user = CurrentUser; //System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            DailyClassEnrollmentModel enrollments = ClassBLL.GetEnrollmentsByDate(dt, CurrentUser.StudioId, userRole, removeEmptyClasses ?? false);
           //ViewBag.EnableOutroll = StudioBLL.GetClasses(id,true).First().Date >= DateTime.Now;
            return PartialView("DailyClassEnrolment", enrollments);
        }

        public PartialViewResult GetCommentsBydate(string date, int? userRole, bool? removeEmptyClasses)
        {
            DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //var user = CurrentUser; //System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            DailyClassEnrollmentModel enrollments = ClassBLL.GetCommentsByDate(dt, CurrentUser.StudioId, userRole, removeEmptyClasses ?? false);
            //ViewBag.EnableOutroll = StudioBLL.GetClasses(id,true).First().Date >= DateTime.Now;
            return PartialView("DailyComments", enrollments);
        }

        public PartialViewResult GetActivatedEnrollmentBydate(string date, int? userRole, bool? removeEmptyClasses)
        {
            DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //var user = CurrentUser; //System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            DailyClassEnrollmentModel enrollments = ClassBLL.GetActivatedEnrollmentsByDate(dt, CurrentUser.StudioId, userRole, removeEmptyClasses ?? true);
            //ViewBag.EnableOutroll = StudioBLL.GetClasses(id,true).First().Date >= DateTime.Now;
            return PartialView("DailyClassEnrolment", enrollments);
        }
        
        public PartialViewResult GetLateCancelEnrollmentsBydate(string date)
        {
            DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //var user = CurrentUser; //System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            DailyClassEnrollmentModel enrollments = ClassBLL.GetLateCancelEnrollmentsByDate(dt, CurrentUser.StudioId);
            //ViewBag.EnableOutroll = StudioBLL.GetClasses(id,true).First().Date >= DateTime.Now;
            return PartialView("DailyClassEnrolment", enrollments);
        }

        public PartialViewResult GetClassEnrollmentByWeek(int weekno, int? userRole, bool removeEmptyClasses)
        {
            //DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DailyClassEnrollmentModel enrollments = ClassBLL.GetEnrollmentsByWeek(weekno, CurrentUser.StudioId, userRole, removeEmptyClasses);
            //ViewBag.EnableOutroll = StudioBLL.GetClasses(id,true).First().Date >= DateTime.Now;
            return PartialView("DailyClassEnrolment", enrollments);
        }

        public PartialViewResult GetClassesbydate(string date)
        {
            //var userid = User.Identity.GetUserId();
            //var user = CurrentUser; //System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            //var user = UserManager.Users.FirstOrDefault(x => x.Id == userid);
            DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //var classes = StudioBLL.GetDailyClassesForCalander(dt, CurrentUser.StudioId);
            ClassesList classes = new ClassesList { Classes = StudioBLL.GetDailyClassesForCalander(dt, CurrentUser.StudioId), CurrentCompany = CurrentCompany };
            return PartialView("ClassesTablePartial", classes);
        }

        public PartialViewResult GetClassByInstructor(string id, string month)
        {
            var dt = month == null ? new DateTime(DateTime.UtcNow.ToLocal().Year, DateTime.UtcNow.ToLocal().Month - 1, 1)
                                   : DateTime.ParseExact(month, "MM/yyyy", CultureInfo.InvariantCulture);
            var classes = StudioBLL.GetClassByInstructor(id,dt);
            return PartialView("InstructorClasses", classes);
        }
        

        public ActionResult RefreshClasses(DateTime Classdate)
        {
            var week = Utils.GetIso8601WeekOfYear(Classdate); //weekno ?? Utils.GetIso8601WeekOfYear(DateTime.Now);
            //var classes = StudioBLL.GetClassesByWeek(week, CurrentUser.StudioId).OrderBy(x => x.Date); //StudioBLL.GetClasses(0, false);
            ClassesList classes = new ClassesList { Classes = StudioBLL.GetClassesByWeek(week, CurrentUser.StudioId).OrderBy(x => x.Date), CurrentCompany = CurrentCompany };
            return PartialView("ClassesTablePartial", classes);
        }

    }
}
