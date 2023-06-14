using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using AutoMapper;
using BLL;
using GetWild.Models;
using InShapeModels;
using InShapeModels.APIModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Utilities;


namespace GetWild.Controllers.API
{
    
    public class UserController : BaseAPIController
    {
        private ApplicationUserManager _userManager;
        //private const string ProfileUploadDir = "~/images/Members";

        public UserController()
        {
        }

        public UserController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }


        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        //public IEnumerable<string> Get()
        //{
        //    var user = HttpContext.Current.User.Identity.GetUserId();
        //    return new string[] { "userid", user };
        //}


        /// <summary>
        /// get the current logged in user (from token) info to show
        /// </summary>
        /// <returns>user info object including personal details, subscription and profile infos.</returns>
        public GymUserViewModel GetUserInfo()
        {
            var userid = HttpContext.Current.User.Identity.GetUserId();
            var userInfo = new GymUserViewModel {User = UserManager.Users.FirstOrDefault(u => u.Id == userid)};
            var subscription = UserBll.GetSubscription(userid);
            userInfo.UserSubscription = Mapper.Map<SubscriptionViewModel>(subscription);
            var profile = UserBll.GetUserProfile(userid);
            userInfo.UserProfile = Mapper.Map<ProfileViewModel>(profile);

            var firstprofile = UserBll.GetFirstUserProfile(userid);
            userInfo.UserFirstProfile = Mapper.Map<ProfileViewModel>(firstprofile);
            return userInfo;
        }

        /// <summary>
        /// Save user confirmation of T and C
        /// </summary>
        /// <returns>result object, if result param is false there was an error, should be in the error param</returns>
        [System.Web.Http.HttpGet]
        public APIResult ConfirmTandC()
        {
            var result = new APIResult {Result = UserBll.ConfirmTandC(HttpContext.Current.User.Identity.GetUserId())};
            if (!result.Result) result.Error = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר.";
            return result;
        }


        /// <summary>
        /// Upload Image Profile picture
        /// </summary>
        /// <param name="picture">posted image, use ImageType to indicate if "first" image or "progress"</param>
        /// <returns>result object, if result param is false there was an error, should be in the error param</returns>
        public APIResult UploadProfileIMG(ProfileImage picture)
        {
            var result = new APIResult();
            //var progresspath = "_" + DateTime.UtcNow.ToLocal().ToString("yyyy-M-dd-hh-mm");
            var filename = User.Identity.GetUserId() + "." + picture.FileExtention;
            var imagePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(App.Configuration.ProfileUploadDir), filename);
            //var imageUrl = ProfileUploadDir.TrimStart('~') + "/" + filename; //Path.Combine(StudioRoomUploadDir, Model.ImageUpload.FileName);

            byte[] imageData = Convert.FromBase64String(picture.ImageData);
            result.Result = Utils.SaveImage(imageData, imagePath);
            if (!result.Result)
            {
                result.Error = "לא ניתן לשמור את התמונה, אנא נסה מאוחר יותר.";
                return result;
            }
            //update user

            result.Result = UserBll.UpdateUserIMG(User.Identity.GetUserId(), filename);

            if (!result.Result) result.Error = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר.";
            return result;
        }

        //public APIResult UploadProfileIMG(UserImageUpload picture)
        //{
        //    var progresspath = picture.ImageType == "progress" ? "_" + DateTime.UtcNow.ToLocal().ToString("yyyy-M-dd-hh-mm") : string.Empty;
        //    var filename = User.Identity.GetUserId() + progresspath + "." + picture.ImageUpload.FileName.Substring(picture.ImageUpload.FileName.Length - 3, 3);
        //    var imagePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(App.Configuration.ProfileUploadDir), filename);
        //    //var imageUrl = ProfileUploadDir.TrimStart('~') + "/" + filename; //Path.Combine(StudioRoomUploadDir, Model.ImageUpload.FileName);

        //    byte[] imageData = new byte[picture.ImageUpload.ContentLength];
        //    picture.ImageUpload.InputStream.Read(imageData, 0, picture.ImageUpload.ContentLength);
        //    Utils.SaveImage(imageData, imagePath);

        //    //update user
        //    var result = new APIResult
        //    {
        //        Result = picture.ImageType == "progress"
        //            ? UserBll.UpdateProgressIMG(User.Identity.GetUserId(), filename)
        //            : UserBll.UpdateProfileIMG(User.Identity.GetUserId(), filename)
        //    };
        //    if (!result.Result) result.Error = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר.";
        //    return result;
        //}

        /// <summary>
        /// Post Health T and C form - signed
        /// </summary>
        /// <param name="form">Health T and C form</param>
        /// <returns>result object, if result param is false there was an error, should be in the error param</returns>
        public APIResult SignHealthTandC(APIHealthTandCModel form)
        {
            var result = new APIResult();
            try
            {
                var userid = User.Identity.GetUserId();

                var filename = userid + "_signature.png";
                var imagePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(App.Configuration.ProfileUploadDir), filename);
                var user = UserManager.Users.FirstOrDefault(u => u.Id == userid && !u.SignedHealthTandC);
                if (user != null)
                {
                    user.FirstName = form.FirstName;
                    user.LastName = form.LastName;
                    user.PhoneNumber = form.PhoneNumber;
                    user.Address = form.Address;
                    user.DOB = DateTime.ParseExact(form.DOB, "dd/MM/yyyy", null);
                    user.CitizenId = form.CitizenId;
                    user.Occupation = form.Occupation;
                    user.SignedHealthTandC = true;
                    user.SignedDate = DateTime.UtcNow.ToLocal();

                    //var saved = Utilities.Utils.SaveImage(form.Signature, imagePath);
                    byte[] imageData = Convert.FromBase64String(form.SignatureData);
                    var saved = Utils.SaveImage(imageData, imagePath);

                    if (saved)
                    {
                        var result2 = UserManager.Update(user);
                        result.Result = result2.Succeeded;
                        if (!result2.Succeeded) result.Error = "error updating user data";
                        //Logger.WriteInfo($"Success - userid: {model.Userid}");

                    }
                    else
                    {
                        //Logger.WriteError($"error saving image: {imagePath}");
                        result.Result = false;
                        result.Error = "error saving image";
                    }
                }
                else
                {
                    Logger.WriteError($"error User not found: {userid}");
                    result.Result = false;
                    result.Error = $"error User not found: {userid}";
                }
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Error = ex.Message;
            }
            return result;
        }


        /// <summary>
        /// Register the User Device for Push - can be done on first login only
        /// </summary>
        /// <param name="model">send device id, ads id, type and OS</param>
        /// <returns>result object, if result param is false there was an error, should be in the error param</returns>
        public APIResult RegisterMobileDevice(APIMobileDevice model)
        {

            var mobiledevice = Mapper.Map<MobileDeviceModel>(model);

            mobiledevice.UserId = User.Identity.GetUserId();
            mobiledevice.DateAdded = DateTime.UtcNow.ToLocal();

            var result = UserBll.RegisterMobileDevice(mobiledevice);
            return result;
        }


    }
}
