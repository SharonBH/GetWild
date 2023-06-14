using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using BLL;
using GetWild.Models;
using InShapeModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Utilities;

namespace GetWild.Controllers
{
    [Authorize]
    public class GymController : InShapeMVCController
    {
        private ApplicationUserManager _userManager;
        //private const string ProfileUploadDir = "~/images/Members";
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Gym
        public ActionResult Index()
        {
            //var userid = User.Identity.GetUserId();
            //var user = UserManager.Users.FirstOrDefault(x => x.Id == userid);
            if (CurrentUser == null) RedirectToAction("Index", "Home");
            var userInfo = new GymUserViewModel { User = CurrentUser };
            ViewBag.ShowTandC = !userInfo.User.AcceptedTandC; //RedirectToAction("TandC");
            //ViewBag.ShowHealthTandC = !userInfo.User.SignedHealthTandC; //RedirectToAction("TandC");
            if (!userInfo.User.SignedHealthTandC && CurrentCompany.HealthTandCEnabled) return RedirectToAction("SignHealthTandC");
            var expiresoon = SystemBLL.GetMessagesForUser(CurrentUser.Id).FirstOrDefault(x=>x.TypeId == 1);
            ViewBag.ProfileIMG = userInfo.User.ProfileIMGPath;
            if (expiresoon == null) return View();
            ViewBag.PopupMSG = expiresoon.Message.Replace("[[שם]]", CurrentUser.FirstName);
            ViewBag.ShowPopup = true;
            return View();
        }

        //public ActionResult TandC()
        //{
        //    return View();
        //}

        public ActionResult GetRightNav()
        {
            //var userid = User.Identity.GetUserId();
            UserSubscriptionModelForNav subscription = UserBll.GetSubscriptionForUser(CurrentUser.Id);
            var model = Mapper.Map<SubscriptionViewModel>(subscription);
            model.Messages = SystemBLL.GetMessagesForUser(CurrentUser.Id);
            return PartialView("_RightNav", model);
        }

        public ActionResult GetHealthTandCPopup()
        {
            //var userid = User.Identity.GetUserId();
            //var user = UserManager.Users.FirstOrDefault(x => x.Id == userid);
            var model = new HealthTandCViewModel { FirstName = CurrentUser.FirstName, LastName = CurrentUser.LastName, DOB = CurrentUser.DOB, Email = CurrentUser.Email
                                                   , PhoneNumber = CurrentUser.PhoneNumber, Address = CurrentUser.Address, Userid = CurrentUser.Id, CitizenId = CurrentUser.CitizenId};
            return PartialView("HealthTandCPopup", model);
        }

        public ActionResult SignHealthTandC()
        {
            //var userid = User.Identity.GetUserId();
            //var user = UserManager.Users.FirstOrDefault(x => x.Id == userid);
            var model = new HealthTandCViewModel { FirstName = CurrentUser.FirstName, Gender = CurrentUser.Gender, LastName = CurrentUser.LastName, DOB = CurrentUser.DOB,
                                                   Email = CurrentUser.Email, PhoneNumber = CurrentUser.PhoneNumber, Address = CurrentUser.Address, Userid = CurrentUser.Id,
                                                   CitizenId = CurrentUser.CitizenId };


            return View("HealthTandCFull", model);
        }

        [HttpPost]
        public ActionResult SignHealthTandC (HealthTandCViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("HealthTandCFull");
            }
            //var userid = User.Identity.GetUserId();
            if (CurrentUser.Id != model.Userid)
                return View("HealthTandCFull");
            var filename = CurrentUser.Id + "_signature.png";
            var imagePath = Path.Combine(Server.MapPath(App.Configuration.ProfileUploadDir), filename);
            Logger.WriteInfo($"Begore Getuser - userid: {model.Userid}");
            //var user = UserManager.Users.FirstOrDefault(x => x.Id == CurrentUser.Id);

            if (CurrentUser != null)
            {
                CurrentUser.FirstName = model.FirstName;
                CurrentUser.LastName = model.LastName;
                CurrentUser.PhoneNumber = model.PhoneNumber;
                CurrentUser.Address = model.Address;
                CurrentUser.DOB = model.DOB;
                CurrentUser.CitizenId = model.CitizenId;
                CurrentUser.Gender = model.Gender;
                CurrentUser.SignedHealthTandC = true;
                CurrentUser.SignedDate = DateTime.UtcNow.ToLocal();
            }
            var result = Utilities.Utils.SaveImage(model.Signature, imagePath);
            if (result)
            {
                var result2 = UserManager.Update(CurrentUser);
                result = result2.Succeeded;
                Logger.WriteInfo($"Success - userid: {model.Userid}");
                return RedirectToAction("Index");
            }
            else
            {
                Logger.WriteError($"error saving image: {imagePath}");
                return View("HealthTandCFull");
            }
            
        }

        public ActionResult UserInfo()
        {
            //var userid = User.Identity.GetUserId();
            var userInfo = new GymUserViewModel {User = CurrentUser };
            var subscription = UserBll.GetSubscription(CurrentUser.Id);
            userInfo.UserSubscription = Mapper.Map<SubscriptionViewModel>(subscription);
            var profile = UserBll.GetUserProfile(CurrentUser.Id);
            userInfo.UserProfile = Mapper.Map<ProfileViewModel>(profile);

            var firstprofile = UserBll.GetFirstUserProfile(CurrentUser.Id);
            userInfo.UserFirstProfile = Mapper.Map<ProfileViewModel>(firstprofile);

            return View("User-info", userInfo);
        }

        //public ActionResult GetCalander(int? weekNo, int? roomId)
        //{
        //    var currentweekno = Utilities.Utils.GetIso8601WeekOfYear();
        //    if (!weekNo.HasValue) weekNo = currentweekno;
        //    if (!roomId.HasValue) roomId = 7;
        //    var model = new GymCalanderViewModel {WeekNo = weekNo.Value, CurrentWeekNo = currentweekno,RoomId = roomId.Value, Date = Utilities.Utils.FirstDateOfWeek(DateTime.UtcNow.ToLocal().Year, weekNo.Value),Classes = new List<ClassViewModel>()};
        //    StudioBLL.GetClassesForCalander(roomId.Value, weekNo.Value).ForEach(studioClassModel => model.Classes.Add(Mapper.Map<ClassViewModel>(studioClassModel)));
        //    model.Classes.ForEach(x => x.IsUserEnrolled = ClassBLL.IsUserEnrolled(x.Id.Value, User.Identity.GetUserId())); 
                    

        //    model.AvailableRooms = StudioBLL.GetStudioRooms(0);
        //    return View("Calander", model);
        //  }


        //public ActionResult GetCalanderTable(int? weekNo, int? roomId)
        //{
        //    var currentweekno = Utils.GetIso8601WeekOfYear(DateTime.Now);
        //    if (!weekNo.HasValue) weekNo = currentweekno;
        //    if (!roomId.HasValue) roomId = 7;
        //    var model = new GymCalanderViewModel { WeekNo = weekNo.Value, CurrentWeekNo = currentweekno, RoomId = roomId.Value, Date = Utils.FirstDateOfWeek(DateTime.Now.Year, weekNo.Value), Classes = new List<ClassViewModel>() };
        //    StudioBLL.GetClassesForCalander(roomId.Value, weekNo.Value).ForEach(studioClassModel => model.Classes.Add(Mapper.Map<ClassViewModel>(studioClassModel)));
        //    //model.Classes.ForEach(x => x.IsUserEnrolled = ClassBLL.IsUserEnrolled(x.Id.Value, User.Identity.GetUserId())); 
            
        //    //StudioBLL.GetClassesForCalander(roomId.Value, weekNo.Value)
        //    //    .ForEach(
        //    //        studioClassModel =>
        //    //            model.Classes.Add(new ClassViewModel
        //    //            {
        //    //                Id = studioClassModel.Id,
        //    //                Name = studioClassModel.Name,
        //    //                Description = studioClassModel.Description,
        //    //                Duration = studioClassModel.Duration,
        //    //                Participants = studioClassModel.Participants,
        //    //                Date = studioClassModel.Date,
        //    //                Time = studioClassModel.Date,
        //    //                ClassTypeId = studioClassModel.ClassTypeId.Id,
        //    //                StudioRoomId = studioClassModel.StudioRoomId.Id,
        //    //                ClassType = studioClassModel.ClassTypeId,
        //    //                IsUserEnrolled = ClassBLL.IsUserEnrolled(studioClassModel.Id, User.Identity.GetUserId()),
        //    //                StudioRoom = studioClassModel.StudioRoomId
        //    //            }));
        //    return PartialView("CalanderWeekly", model);
        //}

        public ActionResult GetCalander(DateTime? date, int? Roomid, bool reload = false)
        {
            ViewBag.Reloaded = reload;
            switch (CurrentCompany.CalanderMode)
            {
                case "ByDailyRoom":
                case "Daily":
                    return GetDailyCalander(date);
                case "ByRoom":
                    return GetCalanderByRoom(Roomid);
            }
            return GetDailyCalander(date);
        }


        public ActionResult GetDailyCalander(DateTime? date)
        {
            DateTime showdate = !date.HasValue ? 
                DateTime.UtcNow.ToLocal().Hour >= App.Configuration.CalanderChangeHour ? DateTime.UtcNow.ToLocal().AddDays(1).Date 
                : DateTime.UtcNow.ToLocal().Date : date.Value.Date;
            //var userid = User.Identity.GetUserId();
            //var user = UserManager.Users.FirstOrDefault(x => x.Id == userid);
            if (CurrentUser != null && !CurrentUser.SignedHealthTandC && CurrentCompany.HealthTandCEnabled) return RedirectToAction("SignHealthTandC");
            var model = new GymCalanderViewModel { Date = showdate, Classes = new List<ClassViewModel>() };
            //StudioBLL.GetDailyClassesForCalander(showdate, userid, user.Gender).ForEach(studioClassModel => model.Classes.Add(Mapper.Map<ClassViewModel>(studioClassModel)));
            model.Classes = Mapper.Map<List<ClassViewModel>>(StudioBLL.GetDailyClassesForCalander(showdate, CurrentUser.Id, CurrentUser.Gender, CurrentUser.AgeGroup, CurrentUser.StudioId, false));
            model.Classes.ForEach(c => c.CurrentCompany = CurrentCompany);
            model.AvailableRooms = StudioBLL.GetStudioRooms(0, CurrentUser.StudioId);
            if (!date.HasValue) return View("Calander", model);
            return PartialView(CurrentCompany.CalanderMode == "Daily" ? "CalanderDailyAllRooms" : "CalanderDaily", model);
        }



        public ActionResult GetCalanderByRoom(int? Roomid)
        {
            int roomid = Roomid ?? 0;
            //var userid = User.Identity.GetUserId();
            //var user = UserManager.Users.FirstOrDefault(x => x.Id == userid);
            var model = new GymCalanderViewModel {RoomId = roomid, Classes = new List<ClassViewModel>()};
            //StudioBLL.GetDailyClassesForCalander(showdate, userid, user.Gender).ForEach(studioClassModel => model.Classes.Add(Mapper.Map<ClassViewModel>(studioClassModel)));
            model.Classes =
                Mapper.Map<List<ClassViewModel>>(StudioBLL.GetClassesForCalanderByRoom(roomid, CurrentUser.Id, CurrentUser.Gender, CurrentUser.AgeGroup));
            model.AvailableRooms = StudioBLL.GetStudioRooms(0, CurrentUser.StudioId);
            if (!Roomid.HasValue) return View("CalanderByRoom", model);
            return PartialView("Calanderweekly", model);
        }

        //todo : pass the model instead of id
        public ActionResult GetCalanderDetails(int classid)
        {
            //var userid = User.Identity.GetUserId();
            //var user = UserManager.Users.FirstOrDefault(x => x.Id == userid);
            var studioClassModel = StudioBLL.GetClass(classid, CurrentUser.Id, CurrentUser.Gender, CurrentUser.StudioId, CurrentUser.AgeGroup);
            var model = Mapper.Map<ClassViewModel>(studioClassModel);
            model.CurrentCompany = CurrentCompany;
            if (model == null) return PartialView("CalanderDetailPartial", null);
            return PartialView(CurrentCompany.UsePlacements ? "CalanderDetailPartial-plc" : "CalanderDetailPartial", model);
        }


        //public ActionResult GetCalanderDetails2(int classid)
        //{
        //    //var userid = User.Identity.GetUserId();
        //    //var user = UserManager.Users.FirstOrDefault(x => x.Id == userid);
        //    var studioClassModel = StudioBLL.GetClasses(classid, true, CurrentUser.Id, CurrentUser.Gender, CurrentUser.StudioId, CurrentUser.AgeGroup).FirstOrDefault();
        //    var model = Mapper.Map<ClassViewModel>(studioClassModel);
        //    model.CurrentCompany = CurrentCompany;
        //    return PartialView("CalanderDetailPartial2", model);
        //}


        [HttpPost]
        public ActionResult EnrollToClass(int classId, int? classAvailablePlacementId)
        {
            var result = ClassBLL.EnrollUserToClass(classId, classAvailablePlacementId?? 0, CurrentUser.Id, false, CurrentCompany.NumAdvncedEnrollments, CurrentCompany.LateRegistration, CurrentCompany.LateCancelPenalty);
            return result.Result ? GetCalander(result.Date, result.RoomId) : Json(new { Response = "Error", Message = string.IsNullOrEmpty(result.Error) ? "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר.": result.Error });
            //return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) : Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה." });

        }

        [HttpPost]
        public ActionResult EnrollToClassFromWaitList(int classId, int? classAvailablePlacementId)
        {
            var result = ClassBLL.EnrollUserToClass(classId, classAvailablePlacementId?? 0, CurrentUser.Id, true, CurrentCompany.NumAdvncedEnrollments, CurrentCompany.LateRegistration, CurrentCompany.LateCancelPenalty);
            return result.Result ? GetCalander(result.Date, result.RoomId, true) : Json(new { Response = "Error", Message = string.IsNullOrEmpty(result.Error) ? "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." : result.Error });
            //return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) : Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה." });

        }


        [HttpPost]
        public ActionResult OutrollFromClass(int classId, bool isLate)
        {
            var result = ClassBLL.OutrolltUserToClass(classId, CurrentUser.Id, CurrentCompany.CancellationThresholdMins, CurrentCompany.LateCancelation, isLate);
            return result.Result ? GetCalander(result.Date, result.RoomId) : Json(new { Response = "Error", Message = string.IsNullOrEmpty(result.Error) ? "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." : result.Error });

            //return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) : Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה." });

        }

        [HttpPost]
        public ActionResult JoinWaitingList(int classId)
        {
            var result = ClassBLL.JoinWaitingList(classId, CurrentUser.Id);
            return result.Result ? GetCalander(result.Date, result.RoomId) : Json(new { Response = "Error", Message = string.IsNullOrEmpty(result.Error) ? "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." : result.Error });
            //return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) : Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה." });

        }


        [HttpPost]
        public ActionResult CancelWaitingLis(int classId)
        {
            var result = ClassBLL.CancelWaitingLis(classId, CurrentUser.Id);
            return result.Result ? GetCalander(result.Date, result.RoomId) : Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." });

            //return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) : Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה." });

        }


        [HttpPost]
        public ActionResult ReadMessages()
        {
            var result = SystemBLL.ReadMessages(CurrentUser.Id);
            return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) : Json(new { Response = "Success", Message = "אין הודעות חדשות." });
        }


        public ActionResult UploadProfileIMG(string ImageType)
        {
            var model = new UserImageUpload {UserId = CurrentUser.Id, ImageType = ImageType};
            return PartialView("_ImageUpload", model);
        }


        [HttpPost]
        public ActionResult UploadProfileIMG(UserImageUpload model)
        {
            if (model.ImageUpload == null || model.ImageUpload.ContentLength == 0)
            {
                ModelState.AddModelError("ImageUpload", "השדה נדרש");
            }

            else if (!Utilities.Utils.ValidImageTypes.Contains(model.ImageUpload.ContentType))
            {
                ModelState.AddModelError("ImageUpload", "יש להעלות רק קבצי תמונה בלבד.");
            }
            if (!ModelState.IsValid) return View("_ImageUpload", model);
            var progresspath = model.ImageType == "progress" ? "_" + DateTime.UtcNow.ToLocal().ToString("yyyy-M-dd-hh-mm"): string.Empty;
            var filename = CurrentUser.Id + progresspath + "." + model.ImageUpload.FileName.Substring(model.ImageUpload.FileName.Length-3, 3);
            var imagePath = Path.Combine(Server.MapPath(App.Configuration.ProfileUploadDir), filename);
            //var imageUrl = ProfileUploadDir.TrimStart('~') + "/" + filename; //Path.Combine(StudioRoomUploadDir, Model.ImageUpload.FileName);

            byte[] imageData = new byte[model.ImageUpload.ContentLength];
            model.ImageUpload.InputStream.Read(imageData, 0, model.ImageUpload.ContentLength);

            Utilities.Utils.SaveImage(imageData,imagePath);

            //update user
            bool result = false;
            result = model.ImageType == "progress" ? UserBll.UpdateProgressIMG(User.Identity.GetUserId(), filename) : UserBll.UpdateProfileIMG(User.Identity.GetUserId(), filename);
            if (result) return RedirectToAction("UserInfo"); 
            return Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." });
        }

        public ActionResult GetGraph(string type)
        {
            //var userid = User.Identity.GetUserId();
            var profiledata = UserBll.GetUserProfileforGraph(CurrentUser.Id);
            var graphdata = Mapper.Map<List<ProfileViewModel>>(profiledata);
            return PartialView("ProgressChart", graphdata);
        }


        public ActionResult NetxUserEnrollmentPopup()
        {
            //var userid = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(CurrentUser.Id)) return new EmptyResult();
            List<CalendarClassEnrollmentModel> enrollments = ClassBLL.GetEnrollmentsByUserDate(CurrentUser.Id, DateTime.UtcNow.ToLocal(), false);
            return PartialView("NextClassesPartialPopup", enrollments);
        }

        public ActionResult NetxUserEnrollment()
        {
            //var userid = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(CurrentUser.Id)) return new EmptyResult();
            List<CalendarClassEnrollmentModel> enrollments = ClassBLL.GetEnrollmentsByUserDate(CurrentUser.Id, DateTime.UtcNow.ToLocal(), true);
            return PartialView("NextClassesPartial", enrollments); //.OrderBy(x=>x.Class.Date).ToList());
        }

        public ActionResult LastClass()
        {
            //var userid = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(CurrentUser.Id)) return new EmptyResult();
            List<CalendarClassEnrollmentModel> enrollments = ClassBLL.GetEnrollmentsByUserDate(CurrentUser.Id, DateTime.UtcNow.ToLocal(), true);
            return PartialView("NextClassesPartial", enrollments); //.OrderBy(x => x.Class.Date).ToList());
        }

        public ActionResult GetLastClass()
        {
            if (!CurrentCompany.ClassRatingEnabled) return new EmptyResult();
            //var userid = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(CurrentUser.Id)) return new EmptyResult();
            var Lastenrollment = ClassBLL.GetLastEnrollmentsByUser(CurrentUser.Id);
            return Lastenrollment== null ? null : PartialView("RatingPartial", Lastenrollment);
        }

        [HttpPost]
        public ActionResult RateClass(int enrolmentid, int rating)
        {
            var result = ClassBLL.RateClass(enrolmentid, rating);
            return !result.Result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) : Json(new { Response = "Success", Message = "אין הודעות חדשות." });
        }

    }
}