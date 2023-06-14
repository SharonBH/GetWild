using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using GetWild.Models;
using BLL;
using InShapeModels;
using Utilities;
using System.IO;

namespace GetWild.Controllers
{
    [Authorize(Roles = "Instructor, ClassInstructor, admin")]
    public class AccountController : InShapeMVCController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        //private const string ProfileUploadDir = "~/images/Members";

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.isLogin = true;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    var user = await UserManager.FindAsync(model.Email, model.Password);
                    //App.SetCurrentCompanybyStudioID(user.StudioId);
                    await SignInManager.SignInAsync(user, true, model.RememberMe);
                    if (UserManager.IsInRole(user.Id, "Instructor") || UserManager.IsInRole(user.Id, "admin"))
                    {
                        return RedirectToManager();
                    }
                    Logger.WriteInfo($"user {user.UserName} logged in");
                    return UserManager.IsInRole(user.Id, "ClassInstructor") ? RedirectToInstructor() : RedirectToUser();
                //return User.IsInRole("Instructor") || User.IsInRole("admin") ? RedirectToManager() : RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new {ReturnUrl = returnUrl, RememberMe = model.RememberMe});
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "אימייל ו/או סיסמא לא נכונים.");
                    ViewBag.isLogin = true;
                    return View(model);
            }
        }


        [AllowAnonymous]
        public JsonResult ConfirmTandC()
        {
            var result = UserBll.ConfirmTandC(User.Identity.GetUserId());
            return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) 
                           : Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה." });
        }

        //[AllowAnonymous]
        //public JsonResult SignHealthTandC(HealthTandCViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var message = string.Join(" | ", ModelState.Values
        //            .SelectMany(v => v.Errors)
        //            .Select(e => e.ErrorMessage));
        //        Logger.WriteError($"SignHealthTandC - {model.Userid}, ModelErrors: {message}");
        //        var Exceptions = string.Join(" | ", ModelState.Values
        //            .SelectMany(v => v.Errors)
        //            .Select(e => e.Exception));
        //        Logger.WriteError($"SignHealthTandC - {model.Userid}, ModelErrors: {Exceptions}");
        //        return Json(new { Response = "Error", Message = message });
        //    }
        //    var userid = User.Identity.GetUserId();
        //    if (userid != model.Userid)
        //        return Json(new {Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר."});
        //    var filename = userid + "_signature.png";
        //    var imagePath = Path.Combine(Server.MapPath(ProfileUploadDir), filename);
        //    Logger.WriteInfo($"Begore Getuser - userid: {model.Userid}");
        //    var user = UserManager.Users.FirstOrDefault(x => x.Id == userid);
            
        //    if (user != null)
        //    {
        //        user.FirstName = model.FirstName;
        //        user.LastName = model.LastName;
        //        user.PhoneNumber = model.PhoneNumber;
        //        user.Address = model.Address;
        //        user.DOB = model.DOB;
        //        user.CitizenId = model.CitizenId;
        //        user.Occupation = model.Occupation;
        //        user.SignedHealthTandC = true;
        //        user.SignedDate = DateTime.UtcNow.ToLocal();
        //    }
        //    var result = Utils.Utils.SaveImage(model.Signature, imagePath);
        //    if (result)
        //    {
        //        var result2 = UserManager.Update(user);
        //        result = result2.Succeeded;
        //        Logger.WriteInfo($"Success - userid: {model.Userid}");
        //    }
        //    else
        //    {
        //        Logger.WriteError($"error saving image: {imagePath}");
        //    }
        //    return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." })
        //                   : Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה." });
        //}

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [Authorize(Roles = "Instructor, admin")]
        public ActionResult Register()
        {
            //var SingleStudio = App.CurrentCompany.Studios.Count == 1 ? App.CurrentCompany.Studios.First().Id : 0;
            return View(new RegisterViewModel { Userid = string.Empty, UserType = ParticipantType.User});
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [Authorize(Roles = "Instructor, admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.Fname,
                LastName = model.Lname,
                Address = model.Address,
                DOB = model.DOB,
                PhoneNumber = model.mobile,
                ProfileIMG = model.ProfileIMG,
                JoinDate = DateTime.UtcNow.ToLocal().Date,
                Gender = model.Gender,
                AgeGroup = model.AgeGroup,
                ReceiveSMS = model.ReceiveSMS,
                Marked = model.Marked,
                StudioId = CurrentUser.StudioId
            };
            var result = await UserManager.CreateAsync(user, model.Password);

                

            if (result.Succeeded)
            {
                //UserManager.AddToRole(user.Id, "User");
                UserManager.AddToRole(user.Id, model.UserType.ToString());
                return RedirectToManager();
            }
            AddErrors(result);

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [Authorize(Roles = "Instructor, admin")]
        public ActionResult RegisterPlus()
        {
            var subtypes = Mapper.Map<List<SubscriptionTypeViewModel>>(SubscriptionTypeBLL.GetTypes(0, CurrentUser.StudioId, true));
            //subtypes.Add(new SubscriptionTypeViewModel { Id = -1, Name = "ללא מנוי"});
            return
                View(new RegisterPlusViewModel
                {
                    SendWelcomeSMS = CurrentCompany.UseSMS,
                    RegistrationDetails = new RegisterViewModel {Userid = string.Empty, UserType = ParticipantType.User, ReceiveSMS = CurrentCompany.UseSMS, Password = "123456", ConfirmPassword = "123456" },
                    SubscriptionDetails = new SubscriptionViewModel { SubscriptionTypesList = subtypes.OrderBy(x=>x.Id).ToList()
                    }
                });
        }

        [Authorize(Roles = "Instructor, admin")]
        [CompanyAuthorization("UseInstructors")]
        public ActionResult RegisterInstructor()
        {
            //var subtypes = Mapper.Map<List<SubscriptionTypeViewModel>>(SubscriptionTypeBLL.GetTypes(0));
            //subtypes.Add(new SubscriptionTypeViewModel { Id = -1, Name = "ללא מנוי" });
            return
                View(new RegisterInstructorViewModel()
                {
                     Userid = string.Empty, UserType = ParticipantType.User   
                });
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [Authorize(Roles = "Instructor, admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterPlus(RegisterPlusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var subtypes = Mapper.Map<List<SubscriptionTypeViewModel>>(SubscriptionTypeBLL.GetTypes(0, CurrentUser.StudioId, true));
                if (model.SubscriptionDetails == null) model.SubscriptionDetails = new SubscriptionViewModel();
                model.SubscriptionDetails.SubscriptionTypesList = subtypes.OrderBy(x => x.Id).ToList();
                return View(model);
            }
            var user = new ApplicationUser
            {
                UserName = model.RegistrationDetails.Email,
                Email = model.RegistrationDetails.Email,
                FirstName = model.RegistrationDetails.Fname,
                LastName = model.RegistrationDetails.Lname,
                Address = model.RegistrationDetails.Address,
                DOB = model.RegistrationDetails.DOB,
                PhoneNumber = model.RegistrationDetails.mobile,
                ProfileIMG = model.RegistrationDetails.ProfileIMG,
                JoinDate = DateTime.UtcNow.ToLocal().Date,
                Gender = model.RegistrationDetails.Gender,
                AgeGroup = model.RegistrationDetails.AgeGroup,
                ReceiveSMS = model.RegistrationDetails.ReceiveSMS,
                Marked = model.RegistrationDetails.Marked,
                StudioId = CurrentUser.StudioId

            };
            var result = await UserManager.CreateAsync(user, model.RegistrationDetails.Password);



            if (result.Succeeded)
            {
                //UserManager.AddToRole(user.Id, "User");
                UserManager.AddToRole(user.Id, model.RegistrationDetails.UserType.ToString());

                //Send Welcome SMS
                if (model.SendWelcomeSMS)
                {
                    var u = Mapper.Map<AspNetUser>(user);
                    SendWelcomeSMS(u, model.RegistrationDetails.Password);
                }

                // add subscription
                if (model.SubscriptionDetails.SubscriptionTypeId > 0)
                {
                    var subscription = Mapper.Map<UserSubscriptionModel>(model.SubscriptionDetails);
                    subscription.UserId = user.Id;
                    subscription.DateSubscribed = DateTime.UtcNow.ToLocal().Date;
                    subscription.UserCreated = User.Identity.GetUserId();
                    subscription.CurrentBalance = subscription.NumClasses;
                    subscription.Active = true;
                    
                    var SubResult = UserBll.UpdateSubscription(subscription);

                }
                return CurrentCompany.ManageAfterRegister ? Manage(user.Id) : RedirectToManager();
            }
            AddErrors(result);

            // If we got this far, something failed, redisplay form
            var subtypes2 = Mapper.Map<List<SubscriptionTypeViewModel>>(SubscriptionTypeBLL.GetTypes(0, CurrentUser.StudioId, true));
            if (model.SubscriptionDetails == null) model.SubscriptionDetails = new SubscriptionViewModel();
            model.SubscriptionDetails.SubscriptionTypesList = subtypes2.OrderBy(x => x.Id).ToList();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Instructor, admin")]
        [CompanyAuthorization("UseInstructors")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterInstructor(RegisterInstructorViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (model.ImageUpload != null && model.ImageUpload.ContentLength > 0)
            {
                if (!Utilities.Utils.ValidImageTypes.Contains(model.ImageUpload.ContentType))
                {
                    ModelState.AddModelError("ImageUpload", "Please choose either a GIF, JPG or PNG image.");
                }
            }
            var imageUrl = model.ProfileIMG;
            if (model.ImageUpload != null)
            {
                var imagePath = Path.Combine(Server.MapPath(App.Configuration.ClassTypeUploadDir), model.ImageUpload.FileName);
                imageUrl = App.Configuration.ClassTypeUploadDir.TrimStart('~') + "/" + model.ImageUpload.FileName;
                //Path.Combine(StudioRoomUploadDir, Model.ImageUpload.FileName);
                model.ImageUpload.SaveAs(imagePath);
            }
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.Fname,
                LastName = model.Lname,
                Address = model.Address,
                DOB = model.DOB,
                PhoneNumber = model.mobile,
                ProfileIMG = imageUrl,
                JoinDate = DateTime.UtcNow.ToLocal().Date,
                Gender = model.Gender,
                AgeGroup = AgeGroup.מבוגרים,
                ReceiveSMS = model.ReceiveSMS,
                StudioId = CurrentUser.StudioId

            };
            var result = await UserManager.CreateAsync(user, model.Password);



            if (result.Succeeded)
            {
                //UserManager.AddToRole(user.Id, "User");
                UserManager.AddToRole(user.Id, UserType.ClassInstructor.ToString());

                //Send Welcome SMS
                //if (model.SendWelcomeSMS)
                //{
                //    var u = Mapper.Map<AspNetUser>(user);
                //    SendWelcomeSMS(u, model.RegistrationDetails.Password);
                //}

                //Add / Update Rate
                UserBll.UpdateInstructorRate(new InstructorDetailsModel
                {
                    InstructorId =  user.Id,
                    Rate = model.Rate,
                    DailyRate = model.DailyRate,
                    ColorCode = model.ColorCode,
                    DateUpdated = DateTime.UtcNow.ToLocal()
                });
                return RedirectToAction("GetInstructors");
            }
            AddErrors(result);

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [Authorize(Roles = "Instructor, admin")]
        public ActionResult Manage(string userid)
        {
            if (userid == null || string.IsNullOrEmpty(userid)) return new EmptyResult();
            //var user = UserManager.Users.FirstOrDefault(u => u.Id == userid);
            //var model = new RegisterViewModel { Fname = user.FirstName, Lname = user.LastName, DOB = user.DOB.Value.Date, Email = user.Email, mobile = user.PhoneNumber, Address = user.Address, userid = user.Id };
            return View("UserMngmnt", new RegisterViewModel { Userid = userid });
        }

        public PartialViewResult ManageUser(string userid)
        {
            if (userid == null || string.IsNullOrEmpty(userid)) return new PartialViewResult();
            var user = UserManager.Users.FirstOrDefault(u => u.Id == userid);
            if (user == null) return new PartialViewResult();
            var role = UserManager.GetRoles(user.Id).FirstOrDefault(x=>x.Contains("User"));
            var type = role == null ? ParticipantType.DemoUser : (ParticipantType)Enum.Parse(typeof(ParticipantType), role);
            var model = new RegisterViewModel
            {
                Fname = user.FirstName,
                Lname = user.LastName,
                DOB = user.DOB,
                Email = user.Email,
                mobile = user.PhoneNumber,
                Address = user.Address,
                Userid = user.Id,
                Password = user.PasswordHash,
                ConfirmPassword = user.PasswordHash,
                ReceiveSMS = user.ReceiveSMS,
                UserType = type,
                Gender = user.Gender,
                AgeGroup = user.AgeGroup,
                CurrentCompany = CurrentCompany,
                SignedHealthTandC = user.SignedHealthTandC,
                Marked = user.Marked
            };
            return PartialView("UserUpdatePartial", model);
        }

        [CompanyAuthorization("UseInstructors")]
        public ViewResult UpdateInstructor(string userid)
        {
            if (userid == null || string.IsNullOrEmpty(userid)) return new ViewResult();
            //var user = UserManager.Users.FirstOrDefault(u => u.Id == userid);
            var instructor = UserBll.GetInstructor(userid);
            if (instructor == null) return new ViewResult();
            
            var model = new RegisterInstructorViewModel
            {
                Fname = instructor.FirstName,
                Lname = instructor.LastName,
                DOB = instructor.DOB,
                Email = instructor.Email,
                mobile = instructor.PhoneNumber,
                Address = instructor.Address,
                Userid = instructor.UserId,
                Password = "password",
                ConfirmPassword = "password",
                Gender = instructor.Gender,
                ProfileIMG = instructor.ProfileIMG,
                ReceiveSMS = instructor.ReceiveSMS,
                Id = instructor.Id,
                ColorCode = instructor.ColorCode,
                Rate = instructor.Rate,
                DailyRate = instructor.DailyRate
                
            };
            return View("UpdateInstructor", model);
        }

        public PartialViewResult ManageProfile(string userid)
        {
            if (userid == null || string.IsNullOrEmpty(userid)) return new PartialViewResult();
            var user = UserManager.Users.FirstOrDefault(u => u.Id == userid);
            if (user == null) return new PartialViewResult();
            UserProfileModel profile = UserBll.GetUserProfile(userid);
            var model = Mapper.Map<ProfileViewModel>(profile);
            return PartialView("UserProfilePartial", model);
        }

        public PartialViewResult ManageSubscriptions(string userid)
        {
            if (userid == null || string.IsNullOrEmpty(userid)) return new PartialViewResult();
            var user = UserManager.Users.FirstOrDefault(u => u.Id == userid);
            if (user == null) return new PartialViewResult();
            UserSubscriptionModel subscription = UserBll.GetSubscription(userid);
            var model = Mapper.Map<SubscriptionViewModel>(subscription);
            if (subscription.Id == 0) model.SubscriptionTypesList = Mapper.Map<List<SubscriptionTypeViewModel>>(SubscriptionTypeBLL.GetTypes(0, CurrentUser.StudioId));
            return PartialView("UserSubscriptionsPartial", model);
        }

        public PartialViewResult GetFrozenSubscriptionDetails(int subscriptionId, bool frozen, string userid)
        {
            if (subscriptionId > 0 && frozen)
            {
                var model = UserBll.GetFrozenSubscriptionDetails(subscriptionId);
                return PartialView("SubscriptionFreeze", model);
            }
            var emptymodel = new FrozenSubscriptionModel { Id = 0, SubscriptionId = subscriptionId, SubscriptionUser = userid};
            return PartialView("SubscriptionFreeze", emptymodel);
        }

        public ActionResult ResetPass(string userid)
        {
            if (userid == null || string.IsNullOrEmpty(userid)) return new PartialViewResult();
            var user = UserManager.Users.FirstOrDefault(u => u.Id == userid);
            if (user == null) return new PartialViewResult();
            var model = new AdminSetPasswordViewModel()
            {
                Userid = userid,
                
            };
            return PartialView("_ResetPass", model);
        }

        public ActionResult HealthTandCPrint(string userid)
        {
            if (userid == null || string.IsNullOrEmpty(userid)) return new PartialViewResult();
            var user = UserManager.Users.FirstOrDefault(u => u.Id == userid);
            if (user == null) return new PartialViewResult();
            var model = new HealthTandCViewModel()
            {
                Userid = userid,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                CitizenId = user.CitizenId,
                DOB = user.DOB,
                Email = user.Email,
                Address = user.Address,
                SignedDate = user.SignedDate,
                SignatureIMGPath = user.SignatureIMGPath

            };
            return PartialView("HealthTandCPrint", model);
        }

        // POST: /Account/UpdateUser
        [HttpPost]
        [Authorize(Roles = "Instructor, admin")]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateUser(RegisterViewModel model)
        {
            return UpdateMembershipUser(model);
        }

        [HttpPost]
        [Authorize(Roles = "Instructor, admin")]
        [CompanyAuthorization("UseInstructors")]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateInstructor(RegisterInstructorViewModel model)
        {
            if (model.ImageUpload != null && model.ImageUpload.ContentLength > 0)
            {
                if (!Utilities.Utils.ValidImageTypes.Contains(model.ImageUpload.ContentType))
                {
                    ModelState.AddModelError("ImageUpload", "Please choose either a GIF, JPG or PNG image.");
                }
            }
            var imageUrl = model.ProfileIMG;
            if (model.ImageUpload != null)
            {
                var imagePath = Path.Combine(Server.MapPath(App.Configuration.ClassTypeUploadDir), model.ImageUpload.FileName);
                imageUrl = App.Configuration.ClassTypeUploadDir.TrimStart('~') + "/" + model.ImageUpload.FileName;
                //Path.Combine(StudioRoomUploadDir, Model.ImageUpload.FileName);
                model.ImageUpload.SaveAs(imagePath);
            }
            model.ProfileIMG = imageUrl;
            var result = UpdateMembershipUser(model, false);
            UserBll.UpdateProfileIMG(model.Userid, imageUrl);
            UserBll.UpdateInstructorRate(new InstructorDetailsModel
            {
                Id = model.Id,
                InstructorId = model.Userid,
                Rate = model.Rate,
                DailyRate = model.DailyRate,
                ColorCode = model.ColorCode,
                DateUpdated = DateTime.UtcNow.ToLocal()
            });
            return RedirectToAction("GetInstructors");
        }

        JsonResult UpdateMembershipUser(RegisterViewModel model, bool updateRole = true)
        {
            if (!ModelState.IsValid) return Json(new { Response = "Error", Message = "נתונים שגויים." });
            var user = UserManager.Users.FirstOrDefault(x => x.Id == model.Userid);
            var emailvalid = UserManager.Users.FirstOrDefault(x => x.Email == model.Email && x.Id != user.Id);
            if (emailvalid != null) return Json(new { Response = "Error", Message = "אימייל כבר בשימוש." });
            if (user != null)
            {
                //update user password
                //var resetResult = ResetPassword(user, model.Password);
                //if (!resetResult.Result.Succeeded) return Json(new { Response = "Error", Message = "לא ניתן להחליף סיסמא." });
                user.FirstName = model.Fname;
                user.LastName = model.Lname;
                user.PhoneNumber = model.mobile;
                user.Address = model.Address;
                user.DOB = model.DOB;
                user.Email = model.Email;
                user.UserName = model.Email;
                user.Gender = model.Gender;
                user.AgeGroup = model.AgeGroup;
                user.ReceiveSMS = model.ReceiveSMS;
                user.Marked = model.Marked;
            }
            var result = UserManager.Update(user);


            //update email / username
            //update role (user type)
            if (updateRole && !UserManager.IsInRole(user.Id, model.UserType.ToString()))
            {
                //remove all roles
                UserManager.RemoveFromRoles(user.Id, UserManager.GetRoles(user.Id).ToArray());
                //add to role
                UserManager.AddToRole(user.Id, model.UserType.ToString());
            }

            if (result.Succeeded)
            {
                return Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה." });
            }
            AddErrors(result);

            // If we got this far, something failed, redisplay form
            return Json(new { Response = "Error", Message = "לא ניתן לשמור, אנא נסה מאוחר יותר." });
        }


        // POST: /Account/UpdateUser
        [HttpPost]
        [Authorize(Roles = "Instructor, admin")]
        [ValidateAntiForgeryToken]
        public JsonResult ResetPasswordUser(AdminSetPasswordViewModel model)
        {
            //JsonResponse res = new JsonResponse();
            if (!ModelState.IsValid) return Json(new { Response = "Error", Message = "נתונים שגויים." });
            var user = UserManager.Users.FirstOrDefault(x => x.Id == model.Userid);
            if (user != null)
            {
                //update user password
                string resetToken = UserManager.GeneratePasswordResetToken(user.Id);
                IdentityResult resetResult = UserManager.ResetPassword(user.Id, resetToken, model.NewPassword);
                if (!resetResult.Succeeded)
                    return Json(new {Response = "Error", Message = "לא ניתן להחליף סיסמא."});



                //update email / username


                if (resetResult.Succeeded)
                {
                    return Json(new {Response = "Success", Message = "השינויים נשמרו בהצלחה."});
                }
                AddErrors(resetResult);
            }
            // If we got this far, something failed, redisplay form
            return Json(new { Response = "Error", Message = "לא ניתן לשמור, אנא נסה מאוחר יותר." });
        }


        // POST: /Account/UpdateProfile
        [HttpPost]
        [Authorize(Roles = "Instructor, admin")]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateProfile(ProfileViewModel model)
        {
            //JsonResponse res = new JsonResponse();
            if (!ModelState.IsValid) return Json(new { Response = "Error", Message = "נתונים שגויים." });
            //var user = UserManager.Users.FirstOrDefault(x => x.Id == model.UserId);
            //if (user == null) return Json(new { Response = "Error", Message = "אין אפשרות לשמור נתונים - משתמש לא נמצא." });

            var profile = Mapper.Map<UserProfileModel>(model);
            
            var result = UserBll.UpdateProfile(profile);

            return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) : Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה." });

            // If we got this far, something failed, redisplay form
        }


        // POST: /Account/SendWelcomeSMS
        [HttpPost]
        [Authorize(Roles = "Instructor, admin")]
        public JsonResult SendWelcomeSMS(string userId)
        {
            //JsonResponse res = new JsonResponse();
            if (string.IsNullOrEmpty(userId)) return Json(new { Response = "Error", Message = "נתונים שגויים." });

            var user = UserBll.GetUser(userId);
            var result = SendWelcomeSMS(user, "123456");

            return !result ? Json(new { Response = "Error", Message = "לא ניתן לשלוח הודעה, אנא נסה מאוחר יותר." }) : Json(new { Response = "Success", Message = "ההודעה נשלחה בהצלחה." });

        }


        // POST: /Account/UpdateSubscription
        [HttpPost]
        [Authorize(Roles = "Instructor, admin")]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSubscription(SubscriptionViewModel model)
        {
            if (!ModelState.IsValid) return Json(new { Response = "Error", Message = "נתונים שגויים." });
            //var user = UserManager.Users.FirstOrDefault(x => x.Id == model.UserId);
            //if (user == null) return Json(new { Response = "Error", Message = "אין אפשרות לשמור נתונים - משתמש לא נמצא." });
            //var subscription = new UserSubscriptionModel
            //{
            //    Id = model.Id,
            //    UserId = model.UserId,
            //    DateSubscribed = DateTime.Now.Date, 
            //    NumClasses = model.NumofClasses, 
            //    AmountPaid = model.AmountPaid,
            //    UserCreated = User.Identity.GetUserId(),
            //    CurrentBalance = model.NumofClasses, 
            //    Active = true,
            //    DateExpire = model.DateExpire
            //};
            var subscription = Mapper.Map<UserSubscriptionModel>(model);
            if (subscription.Id == 0)
            {
                subscription.DateSubscribed = DateTime.UtcNow.ToLocal().Date;
                subscription.UserCreated = User.Identity.GetUserId();
                subscription.CurrentBalance = subscription.NumClasses;
                subscription.Active = true;
            }
            var result = UserBll.UpdateSubscription(subscription);

            //return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) : Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה." });
            if (result) return ManageSubscriptions(model.UserId);
            return Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." });
        }

        [HttpPost]
        [Authorize(Roles = "Instructor, admin")]
        public ActionResult UpdateSubscriptionExpire(int subscriptionId, DateTime newExpireDate)
        {
            var result = UserBll.UpdateSubscriptionExpire(subscriptionId, newExpireDate);

            //return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) : Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה." });
            return Json(result ? new { Response = "Success", Message = "עודכן בהצלחה!." } : new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." });
        }

        [HttpPost]
        [Authorize(Roles = "Instructor, admin")]
        public ActionResult UpdateUnfreeze(int subscriptionId, string userId)
        {
            var result = UserBll.UpdateSubscriptionUnFreeze(subscriptionId, User.Identity.GetUserId());
            if (result) return ManageSubscriptions(userId);
            return Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." });
        }

        [HttpPost]
        [Authorize(Roles = "Instructor, admin")]
        public ActionResult SubscriptionFreeze(int subscriptionId, string userId, string toDate, string note)
        {
            //var todate = string.IsNullOrEmpty(toDate) ? DateTime.MaxValue : DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
            var result = UserBll.UpdateSubscriptionFreeze(subscriptionId, note ,toDate, User.Identity.GetUserId());
            
            if (result) return ManageSubscriptions(userId);
            return Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." });
        }
        

        [HttpPost]
        [Authorize(Roles = "Instructor, admin")]
        public ActionResult TickUser(string UserId)
        {
            var result = UserBll.TickUser(UserId);

            //return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) : Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה." });
            return Json(result ? new { Response = "Success", Message = "עודכן בהצלחה!." } : new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." });
        }


        [HttpPost]
        [Authorize(Roles = "Instructor, admin")]
        public ActionResult GetClassesDDLByDate(DateTime date)
        {
            var userid = User.Identity.GetUserId();
            var user = UserManager.Users.FirstOrDefault(x => x.Id == userid);
            return Json(StudioBLL.GetClasses(date, user.StudioId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "Instructor, admin")]
        public ActionResult GetPlacementsByClassesDDL(int classid)
        {
            return Json(ClassBLL.GetClassAvailablePlacements(classid, CurrentUser.StudioId, false).Where(x=>!x.IsInUse && !x.IsDeleted), JsonRequestBehavior.AllowGet);
        }
        


        public PartialViewResult GetSubsicriptionEdit(int subscriptionId, string userId)
        {
            var changetypes = UserBll.GetChangeTypes(0);
            return PartialView("UserSubscriptionEdit", new SubscriptionDetailViewModel { SubscriptionId = subscriptionId, ChangeTypes = changetypes, UserId = userId});
        }


        public PartialViewResult GetSubsicriptionDetails(int subscriptionId)
        {
            var details = UserBll.GetSubscriptionDetails(subscriptionId);
            var changetypes = UserBll.GetChangeTypes(0,true);
            List<SubscriptionDetailViewModel> detailsforView = new List<SubscriptionDetailViewModel>();
            foreach (var model in details)
            {
                detailsforView.Add(new SubscriptionDetailViewModel
                {
                    Id = model.Id,
                    Date = model.Date,
                    Note = model.Note,
                    SubscriptionId = model.SubscriptionId,
                    ChangeTypeId = model.ChangeType.Id,
                    User = model.User,
                    Value = model.Value,
                    ChangeTypes = changetypes
                });
            }
            //Thread.Sleep(500000);
            return PartialView("UserSubscriptionDetails", detailsforView);
        }

        public PartialViewResult GetFrozenSubsicriptionDetails(int subscriptionId)
        {
            var details = UserBll.GetPastFrozenSubscriptionDetails(subscriptionId);
            
            return PartialView("UserFrozenSubscriptionDetails", details);
        }

        // POST: /Account/UpdateSubscription
        [HttpPost]
        [Authorize(Roles = "Instructor, admin")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeSubscription(SubscriptionDetailViewModel model)
        {
            if (!ModelState.IsValid) return Json(new { Response = "Error", Message = "נתונים שגויים." });
            //var user = UserManager.Users.FirstOrDefault(x => x.Id == model.UserId);
            //if (user == null) return Json(new { Response = "Error", Message = "אין אפשרות לשמור נתונים - משתמש לא נמצא." });
            var subscription = new SubscriptionDetailModel
            {
                Id = model.Id,
                Date = DateTime.UtcNow.ToLocal(),
                SubscriptionId = model.SubscriptionId,
                Value = model.Value,
                Note = model.Note,
                User = User.Identity.GetUserId(),
                ChangeType = new BalanceChangeModel { Id = model.ChangeTypeId}
            };

            var result = UserBll.ChangeSubscription(subscription);

            //return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) : Json(new { Response = "Success", Message = "השינויים נשמרו בהצלחה." });
            if (result) return ManageSubscriptions(model.UserId);
            return Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." });
        }

        [Authorize(Roles = "Instructor, admin")]

        public ActionResult SubscriptionDeleteConfirmed(int id, string userId)
        {
            UserBll.DeleteSubacription(id);
            return ManageSubscriptions(userId);
        }

        public ActionResult RemoveLateCacelationSubscriptionConfirmed(int id, string userId)
        {
            UserBll.RemoveLateCacelationSubscription(id);
            return ManageSubscriptions(userId);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }


        //public ActionResult GetUsersByRole(string roleId)
        //{
        //    //return View("UserList", UserManager.Users.Where(x => x.Roles.Any(r => r.RoleId == roleId))); //role user = 2
        //    return View("UserList", UserBll.GetUsersWithSubscription()); //role user = 2
        //}
        //[OutputCache(Duration = 300, VaryByParam = "App.CurrentCompany.Id,frozen")]
        [Authorize(Roles = "Instructor, admin")]
        public ActionResult GetUsersTickets(int pageno = 1,bool frozen = true)
        {
            var users = UserBll.GetUsersWithTicketSubscription(CurrentUser.StudioId, frozen, pageno);
            users.CurrentCompany = CurrentCompany;
            //return View("UserList", UserManager.Users.Where(x => x.Roles.Any(r => r.RoleId == roleId))); //role user = 2
            return View("UserListTickets", users); //role user = 2
        }

        [Authorize(Roles = "Instructor, admin")]
        public ActionResult GetUsers(int pageno = 1, bool frozen = true)
        {
            var users = UserBll.GetUsersWithSubscription2(CurrentUser.StudioId, frozen, pageno);
            users.CurrentCompany = CurrentCompany;
            users.UserType = ParticipantType.User;
            //return View("UserList", UserManager.Users.Where(x => x.Roles.Any(r => r.RoleId == roleId))); //role user = 2
            return View("UserList", users); //role user = 2
        }


        //[OutputCache(Duration = 300)]
        [Authorize(Roles = "Instructor, admin")]
        public ActionResult GetNewUsers()
        {
            return PartialView("NewUserList", UserBll.GetNewUsers(CurrentUser.StudioId));
        }

        [Authorize(Roles = "Instructor, admin")]
        public ActionResult GetUsersSummary()
        {
            return PartialView("UsersSummaryPartial", UserBll.GetUsersSummary(CurrentUser.StudioId));
        }

        //[OutputCache(Duration = 300)]
        [Authorize(Roles = "Instructor, admin")]
        public ActionResult GetAlmostExpiredSubscriptions()
        {
            return PartialView("AboutToExpireSubscriptionsList", UserBll.GetAboutToExpireSubscriptions(CurrentCompany.Id));
        }

        [CompanyAuthorization("UseInstructors")]
        public ActionResult GetInstructors(DateTime? startdate)
        {
            var instructors = UserBll.GetInstructorList(startdate, CurrentUser.StudioId);
            return View("InstructorList", instructors);
        }

        //[HttpPost]
        //[Authorize(Roles = "Instructor, admin")]
        //public ActionResult GetUsersByType(int ut, bool frozen, int pageno, bool active, bool loadmore = false)
        //{

        //    var model = UserBll.GetUsersWithSubscription2(CurrentUser.StudioId,frozen, active, pageno, ut);
        //    //if (ut <= 0) return PartialView("UserListTable", model);
        //    //var selectedtype = (UserType) ut;
        //    //model.UserType = (ParticipantType) ut;
        //    //model.UserWithSubscriptions.RemoveAll(x => x.UserType != selectedtype);
        //    if (!loadmore) return PartialView("UserListTable", model);
        //    if (loadmore && model.UserWithSubscriptions.Any()) return PartialView("UserListOnlyTable", model);
        //    else return null;
        //    //return PartialView(loadmore ? model.UserWithSubscriptions.Any() ? "UserListOnlyTable" : null : "UserListTable", model);

        //}

        [HttpPost]
        [Authorize(Roles = "Instructor, admin")]
        public ActionResult GetUsersByTypeActiveFrozen(int ut, bool? active, bool frozen= false, int pageno = 1, bool loadmore = false)
        {

            var model = UserBll.GetUsersWithSubscription2(CurrentUser.StudioId,frozen, active, pageno, ut);
            //if (ut <= 0) return PartialView("UserListTable", model);
            //var selectedtype = (UserType) ut;
            //model.UserType = (ParticipantType) ut;
            //model.UserWithSubscriptions.RemoveAll(x => x.UserType != selectedtype);
            //return PartialView("UserListTable", model);
            //return PartialView(loadmore ? model.UserWithSubscriptions.Any() ? "UserListOnlyTable" : null : "UserListTable", model);
            if (!loadmore) return PartialView("UserListTable", model);
            if (loadmore && model.UserWithSubscriptions.Any()) return PartialView("UserListOnlyTable", model);
            else return null;
        }

        [HttpPost]
        [Authorize(Roles = "Instructor, admin")]
        public ActionResult GetTicketsUsersByType(int ut)
        {

            var model = UserBll.GetUsersWithTicketSubscription(CurrentUser.StudioId, true,-1 , ut);
            //if (ut <= 0) return PartialView("UserListTable", model);
            //var selectedtype = (UserType) ut;
            //model.UserType = (ParticipantType) ut;
            //model.UserWithSubscriptions.RemoveAll(x => x.UserType != selectedtype);
            return PartialView("UserListTicketsTable", model);
            //return PartialView(loadmore ? model.UserWithSubscriptions.Any() ? "UserListOnlyTable" : null : "UserListTable", model);

        }

        [HttpPost]
        [Authorize(Roles = "Instructor, admin")]
        public ActionResult GetUsersBySearch(string search)
        {
            var model = UserBll.GetUsersWithSubscriptionforSearch(CurrentUser.StudioId, search);
            //if (ut <= 0) return PartialView("UserListTable", model);
            //var selectedtype = (UserType) ut;
            //model.UserType = (ParticipantType) ut;
            //model.UserWithSubscriptions.RemoveAll(x => x.UserType != selectedtype);
            return PartialView("UserListTable", model);
            //return PartialView(loadmore ? model.UserWithSubscriptions.Any() ? "UserListOnlyTable" : null : "UserListTable", model);

        }

        //[HttpPost]
        public ActionResult WeeklyReportList(int id, int? weekno, int ut = 0, bool includeForzen = false)
        {
            ViewBag.Id = id;
            ViewBag.showHeader = 0;
            ViewBag.WeekNo = weekno ?? Utilities.Utils.GetIso8601WeekOfYear(DateTime.UtcNow.ToLocal());
            //show only active users.
            var model = UserBll.GetUsersWithSubscription(id, weekno ?? -1, CurrentUser.StudioId, ut);
            //model.UserWithSubscriptions.RemoveAll(x => !x.Active.HasValue || !x.Active.Value);
            if (!includeForzen) model.UserWithSubscriptions.RemoveAll(x => x.Frozen);
            //if (ut > 0)
            //{
            //    var selectedtype = (UserType) ut;
            //    model.UserType = (ParticipantType) ut;
            //    model.UserWithSubscriptions.RemoveAll(x => x.UserType != selectedtype);
            //}
            return PartialView("UserList", model);
        }

        public ActionResult NoEnrollmentReport()
        {
            return View("NoEnrollmentReport");
        }
        public ActionResult UsersWithNoEnrollmentsList()
        {
            ViewBag.showHeader = 0;
            return PartialView("UserList", UserBll.GetUsersWithNoEnrollments(CurrentUser.StudioId));
        }

        public ActionResult WeeklyReport(int? weekno)
        {
            ViewBag.Id = 0;
            ViewBag.showHeader = 0;
            ViewBag.WeekNo = weekno ?? Utils.GetIso8601WeekOfYear(DateTime.UtcNow.ToLocal());
            ViewBag.Header = Utils.GetWeekHeader(ViewBag.WeekNo);
            return View("UserWeeklyReport");
        }

        [Authorize(Roles = "Instructor, admin")]
        public ActionResult DeleteConfirmed(string UserId)
        {
            UserBll.DeleteUser(UserId);
            return RedirectToAction("GetUsers");
        }

        public ActionResult DeleteInstructor(string UserId)
        {
            UserBll.DeleteInstructor(UserId);
            return RedirectToAction("GetInstructors");
        }

        public JsonResult GetUsersSearch(string term, string type = "user")
        {
            return Json(UserBll.GetUsers(term, CurrentUser.StudioId, type), JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult GetUsersForClassSearch(string term, int classid)
        {
            return Json(UserBll.GetUsersForClass(term, CurrentUser.StudioId, classid), JsonRequestBehavior.AllowGet);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool SendWelcomeSMS(AspNetUser user, string password)
        {
            var userSms = new UserSMSSubscriptionModel
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                Password = password,
                PhoneNumber = user.PhoneNumber
            };
            return SMSBLL.SendSMSWelcome(userSms, CurrentCompany.Id, CurrentCompany.WebSiteURL, CurrentCompany.UseSMS);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private ActionResult RedirectToUser()
        {
            return RedirectToAction("GetCalander", "Gym");
        }

        private ActionResult RedirectToManager()
        {
            //return View("UserList", UserBll.GetUsersWithSubscription2(false));
            return RedirectToAction("Index", "Admin");//, new { roleId = "2" }
        }


        private ActionResult RedirectToInstructor()
        {

            //            return RedirectToAction("Index", "StudioClass");
            return RedirectToAction("WeeklySummaryReport", "Report");
        }
        

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}