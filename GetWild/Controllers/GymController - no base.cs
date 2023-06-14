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
    public class GyymController : Controller
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
            var userid = User.Identity.GetUserId();
            var user = UserManager.Users.FirstOrDefault(x => x.Id == userid);
            if (user == null) RedirectToAction("Index", "Home");
            var userInfo = new GymUserViewModel { User = user};
            ViewBag.ShowTandC = !userInfo.User.AcceptedTandC; //RedirectToAction("TandC");
            //ViewBag.ShowHealthTandC = !userInfo.User.SignedHealthTandC; //RedirectToAction("TandC");
            if (!userInfo.User.SignedHealthTandC && App.CurrentCompany.HealthTandCEnabled) return RedirectToAction("SignHealthTandC");
            var expiresoon = SystemBLL.GetMessagesForUser(userid).FirstOrDefault(x=>x.TypeId == 1);
            ViewBag.ProfileIMG = userInfo.User.ProfileIMGPath;
            if (expiresoon == null) return View();
            ViewBag.PopupMSG = expiresoon.Message;
            ViewBag.ShowPopup = true;
            return View();
        }

        //public ActionResult TandC()
        //{
        //    return View();
        //}

        public ActionResult GetRightNav()
        {
            var userid = User.Identity.GetUserId();
            UserSubscriptionModel subscription = UserBll.GetSubscription(userid);
            var model = Mapper.Map<SubscriptionViewModel>(subscription);
            model.Messages = SystemBLL.GetMessagesForUser(userid);
            return PartialView("_RightNav", model);
        }

        public ActionResult GetHealthTandCPopup()
        {
            var userid = User.Identity.GetUserId();
            var user = UserManager.Users.FirstOrDefault(x => x.Id == userid);
            var model = new HealthTandCViewModel { FirstName = user.FirstName, LastName = user.LastName, DOB = user.DOB, Email = user.Email, PhoneNumber = user.PhoneNumber, Address = user.Address, Userid = user.Id, CitizenId = user.CitizenId};
            return PartialView("HealthTandCPopup", model);
        }

        public ActionResult SignHealthTandC()
        {
            var userid = User.Identity.GetUserId();
            var user = UserManager.Users.FirstOrDefault(x => x.Id == userid);
            var model = new HealthTandCViewModel { FirstName = user.FirstName, Gender = user.Gender, LastName = user.LastName, DOB = user.DOB, Email = user.Email, PhoneNumber = user.PhoneNumber, Address = user.Address, Userid = user.Id, CitizenId = user.CitizenId };
            return View("HealthTandCFull", model);
        }

        [HttpPost]
        public ActionResult SignHealthTandC (HealthTandCViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("HealthTandCFull");
            }
            var userid = User.Identity.GetUserId();
            if (userid != model.Userid)
                return View("HealthTandCFull");
            var filename = userid + "_signature.png";
            var imagePath = Path.Combine(Server.MapPath(App.Configuration.ProfileUploadDir), filename);
            Logger.WriteInfo($"Begore Getuser - userid: {model.Userid}");
            var user = UserManager.Users.FirstOrDefault(x => x.Id == userid);

            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                user.Address = model.Address;
                user.DOB = model.DOB;
                user.CitizenId = model.CitizenId;
                user.Gender = model.Gender;
                user.SignedHealthTandC = true;
                user.SignedDate = DateTime.UtcNow.ToLocal();
            }
            var result = Utilities.Utils.SaveImage(model.Signature, imagePath);
            if (result)
            {
                var result2 = UserManager.Update(user);
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
            var userid = User.Identity.GetUserId();
            var userInfo = new GymUserViewModel {User = UserManager.Users.FirstOrDefault(x => x.Id == userid)};
            var subscription = UserBll.GetSubscription(userid);
            userInfo.UserSubscription = Mapper.Map<SubscriptionViewModel>(subscription);
            var profile = UserBll.GetUserProfile(userid);
            userInfo.UserProfile = Mapper.Map<ProfileViewModel>(profile);

            var firstprofile = UserBll.GetFirstUserProfile(userid);
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

        public ActionResult GetCalander(DateTime? date, int? Roomid)
        {
            switch (App.CurrentCompany.CalanderMode)
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
                : DateTime.UtcNow.ToLocal().Date : date.Value;
            var userid = User.Identity.GetUserId();
            var user = UserManager.Users.FirstOrDefault(x => x.Id == userid);
            if (user != null && !user.SignedHealthTandC && App.CurrentCompany.HealthTandCEnabled) return RedirectToAction("SignHealthTandC");
            var model = new GymCalanderViewModel { Date = showdate, Classes = new List<ClassViewModel>() };
            //StudioBLL.GetDailyClassesForCalander(showdate, userid, user.Gender).ForEach(studioClassModel => model.Classes.Add(Mapper.Map<ClassViewModel>(studioClassModel)));
            model.Classes = Mapper.Map<List<ClassViewModel>>(StudioBLL.GetDailyClassesForCalander(showdate, userid, user.Gender, user.AgeGroup, user.StudioId, App.CurrentCompany.CalanderMode == "ByDailyRoom"));
            model.AvailableRooms = StudioBLL.GetStudioRooms(0);
            if (!date.HasValue) return View("Calander", model);
            return PartialView(App.CurrentCompany.CalanderMode == "Daily" ? "CalanderDailyAllRooms" : "CalanderDaily", model);
        }



        public ActionResult GetCalanderByRoom(int? Roomid)
        {
            int roomid = Roomid ?? 0;
            var userid = User.Identity.GetUserId();
            var user = UserManager.Users.FirstOrDefault(x => x.Id == userid);
            var model = new GymCalanderViewModel {RoomId = roomid, Classes = new List<ClassViewModel>()};
            //StudioBLL.GetDailyClassesForCalander(showdate, userid, user.Gender).ForEach(studioClassModel => model.Classes.Add(Mapper.Map<ClassViewModel>(studioClassModel)));
            model.Classes =
                Mapper.Map<List<ClassViewModel>>(StudioBLL.GetClassesForCalanderByRoom(roomid, userid, user.Gender, user.AgeGroup));
            model.AvailableRooms = StudioBLL.GetStudioRooms(0);
            if (!Roomid.HasValue) return View("CalanderByRoom", model);
            return PartialView("Calanderweekly", model);
        }

        //todo : pass the model instead of id
        public ActionResult GetCalanderDetails(int classid)
        {
            var userid = User.Identity.GetUserId();
            var user = UserManager.Users.FirstOrDefault(x => x.Id == userid);
            var studioClassModel = StudioBLL.GetClasses(classid, true, userid,user.Gender, user.AgeGroup).FirstOrDefault();
            var model = Mapper.Map<ClassViewModel>(studioClassModel);
            if (model == null) return PartialView("CalanderDetailPartial", null);
            return PartialView(App.CurrentCompany.UsePlacements ? "CalanderDetailPartial-plc" : "CalanderDetailPartial", model);
        }


        public ActionResult GetCalanderDetails2(int classid)
        {
            var userid = User.Identity.GetUserId();
            var user = UserManager.Users.FirstOrDefault(x => x.Id == userid);
            var studioClassModel = StudioBLL.GetClasses(classid, true, userid, user.Gender, user.AgeGroup).FirstOrDefault();
            var model = Mapper.Map<ClassViewModel>(studioClassModel);
            return PartialView("CalanderDetailPartial2", model);
        }


        [HttpPost]
        public ActionResult EnrollToClass(int classId, int? classAvailablePlacementId)
        {
            var result = ClassBLL.EnrollUserToClass(classId, classAvailablePlacementId?? 0, User.Identity.GetUserId(),false);
            return result.Result ? GetCalander(result.Date, result.RoomId) : Json(new { Response = "Error", Message = string.IsNullOrEmpty(result.Error) ? "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר.": result.Error });
            //return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) : Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה." });

        }

        [HttpPost]
        public ActionResult EnrollToClassFromWaitList(int classId, int? classAvailablePlacementId)
        {
            var result = ClassBLL.EnrollUserToClass(classId, classAvailablePlacementId?? 0, User.Identity.GetUserId(), true);
            return result.Result ? GetCalander(result.Date, result.RoomId) : Json(new { Response = "Error", Message = string.IsNullOrEmpty(result.Error) ? "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." : result.Error });
            //return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) : Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה." });

        }


        [HttpPost]
        public ActionResult OutrollFromClass(int classId)
        {
            var result = ClassBLL.OutrolltUserToClass(classId, User.Identity.GetUserId());
            return result.Result ? GetCalander(result.Date, result.RoomId) : Json(new { Response = "Error", Message = string.IsNullOrEmpty(result.Error) ? "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." : result.Error });

            //return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) : Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה." });

        }

        [HttpPost]
        public ActionResult JoinWaitingList(int classId)
        {
            var result = ClassBLL.JoinWaitingList(classId, User.Identity.GetUserId());
            return result.Result ? GetCalander(result.Date, result.RoomId) : Json(new { Response = "Error", Message = string.IsNullOrEmpty(result.Error) ? "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." : result.Error });
            //return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) : Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה." });

        }


        [HttpPost]
        public ActionResult CancelWaitingLis(int classId)
        {
            var result = ClassBLL.CancelWaitingLis(classId, User.Identity.GetUserId());
            return result.Result ? GetCalander(result.Date, result.RoomId) : Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." });

            //return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) : Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה." });

        }


        [HttpPost]
        public ActionResult ReadMessages()
        {
            var result = SystemBLL.ReadMessages(User.Identity.GetUserId());
            return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) : Json(new { Response = "Success", Message = "אין הודעות חדשות." });
        }


        public ActionResult UploadProfileIMG(string ImageType)
        {
            var model = new UserImageUpload {UserId = User.Identity.GetUserId(), ImageType = ImageType};
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
            var filename = User.Identity.GetUserId() + progresspath + "." + model.ImageUpload.FileName.Substring(model.ImageUpload.FileName.Length-3, 3);
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
            var userid = User.Identity.GetUserId();
            var profiledata = UserBll.GetUserProfileforGraph(userid);
            var graphdata = Mapper.Map<List<ProfileViewModel>>(profiledata);
            return PartialView("ProgressChart", graphdata);
        }


        public ActionResult NetxUserEnrollmentPopup()
        {
            var userid = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userid)) return new EmptyResult();
            List<ClassEnrollmentModel> enrollments = ClassBLL.GetEnrollmentsByUser(userid, false);
            return PartialView("NextClassesPartialPopup", enrollments);
        }

        public ActionResult NetxUserEnrollment()
        {
            var userid = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userid)) return new EmptyResult();
            List<ClassEnrollmentModel> enrollments = ClassBLL.GetEnrollmentsByUser(userid, true);
            return PartialView("NextClassesPartial", enrollments.OrderBy(x=>x.Class.Date).ToList());
        }

        public ActionResult LastClass()
        {
            var userid = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userid)) return new EmptyResult();
            List<ClassEnrollmentModel> enrollments = ClassBLL.GetEnrollmentsByUser(userid, true);
            return PartialView("NextClassesPartial", enrollments.OrderBy(x => x.Class.Date).ToList());
        }

        public ActionResult GetLastClass()
        {
            if (!App.CurrentCompany.ClassRatingEnabled) return new EmptyResult();
            var userid = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userid)) return new EmptyResult();
            var Lastenrollment = ClassBLL.GetLastEnrollmentsByUser(userid);
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