using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using AutoMapper;
using BLL;
using GetWild.Models;
using InShapeModels;
using Microsoft.AspNet.Identity;
using Utilities;

namespace GetWild.Controllers.API
{
    public class APIGymController : InShapeMVCController
    {
        /// <summary>
        /// Get Daily Calander to show to user
        /// </summary>
        /// <param name="date">date dormat yyyy-MM-dd (empty date will retrieve today)</param>
        /// <returns>full calander for specific date in order to build calander</returns>
        public GymCalanderViewModel GetDailyCalanderForDate(string date)
        {
            DateTime showdate = string.IsNullOrEmpty(date) ?
                //DateTime.UtcNow.ToLocal().Hour >= App.Configuration.CalanderChangeHour ? DateTime.UtcNow.ToLocal().AddDays(1).Date
                //: 
                DateTime.UtcNow.ToLocal().Date : DateTime.ParseExact(date, "yyyy-MM-dd",null);
            var userid = User.Identity.GetUserId();
            var user = Mapper.Map<ApplicationUser>(UserBll.GetUser(userid));
            //if (user == null) return new APIResult { Result = false, Error = "User Not Found"};
            var model = new GymCalanderViewModel { Date = showdate, Classes = new List<ClassViewModel>() };
            if (user == null) return model;
            //StudioBLL.GetDailyClassesForCalander(showdate, userid, user.Gender).ForEach(studioClassModel => model.Classes.Add(Mapper.Map<ClassViewModel>(studioClassModel)));
            model.Classes = Mapper.Map<List<ClassViewModel>>(StudioBLL.GetDailyClassesForCalander(showdate, userid, user.Gender, user.AgeGroup, user.StudioId, false));
            model.AvailableRooms = StudioBLL.GetStudioRooms(0, CurrentUser.StudioId);
            return model;
        }

        /// <summary>
        /// Get Class Details to show in Popup and allow interaction
        /// </summary>
        /// <param name="classid">the id of the class to get details</param>
        /// <returns>details of a specific class to show when calander class clicked</returns>
        public ClassViewModel GetClassDetails(int classid)
        {
            var userid = User.Identity.GetUserId();
            var user = Mapper.Map<ApplicationUser>(UserBll.GetUser(userid));
            var studioClassModel = StudioBLL.GetClasses(classid, true, userid, user.Gender, user.StudioId).FirstOrDefault();
            var model = Mapper.Map<ClassViewModel>(studioClassModel);
            return model;
        }

        /// <summary>
        /// Get the list of next classes the user is enrolled to
        /// </summary>
        /// <returns>a list of enrollments</returns>
        public List<CalendarClassEnrollmentModel> GetNetxUserEnrollment()
        {
            var userid = User.Identity.GetUserId();
            List<CalendarClassEnrollmentModel> enrollments = ClassBLL.GetEnrollmentsByUserDate(userid, DateTime.UtcNow.ToLocal(), true);
            return enrollments; //.OrderBy(x => x.Class.Date).ToList();
        }



        /// <summary>
        /// Enroll user to a class
        /// </summary>
        /// <param name="classId">the id of the class user want to enroll to</param>
        /// <returns>result object, if result param is false there was an error, should be in the error param</returns>
        [System.Web.Http.HttpGet]
        public ClassEnrollResult EnrollToClass(int classId, int classAvailablePlacementId)
        {
            var result = ClassBLL.EnrollUserToClass(classId, classAvailablePlacementId, CurrentUser.Id, false, CurrentCompany.NumAdvncedEnrollments, CurrentCompany.LateRegistration, CurrentCompany.LateCancelPenalty);
            return result;
        }

        /// <summary>
        /// outroll (unregister) user from a class
        /// </summary>
        /// <param name="classId">the id of the class user want to outroll to</param>
        /// <returns>result object, if result param is false there was an error, should be in the error param</returns>
        [System.Web.Http.HttpGet]
        public ClassEnrollResult OutrollFromClass(int classId)
        {
            var result = ClassBLL.OutrolltUserToClass(classId, CurrentUser.Id, CurrentCompany.CancellationThresholdMins, CurrentCompany.LateCancelation);
            return result;
        }

        /// <summary>
        /// Enroll user to a waiting list for a class
        /// </summary>
        /// <param name="classId">the id of the class user want to register for waiting list</param>
        /// <returns>result object, if result param is false there was an error, should be in the error param</returns>
        [System.Web.Http.HttpGet]
        public ClassEnrollResult EnrollToClassFromWaitList(int classId, int classAvailablePlacementId)
        {
            var result = ClassBLL.EnrollUserToClass(classId, classAvailablePlacementId, CurrentUser.Id, true, CurrentCompany.NumAdvncedEnrollments, CurrentCompany.LateRegistration, CurrentCompany.LateCancelPenalty);
            return result;
        }

        /// <summary>
        /// cancel registration of user to a class waiting list
        /// </summary>
        /// <param name="classId">the id of the class user want to cancel waiting list registration</param>
        /// <returns>result object, if result param is false there was an error, should be in the error param</returns>
        [System.Web.Http.HttpGet]
        public ClassEnrollResult CancelWaitingList(int classId)
        {
            var result = ClassBLL.CancelWaitingLis(classId, User.Identity.GetUserId());
            return result;
        }

        /// <summary>
        /// register user to a class waiting list
        /// </summary>
        /// <param name="classId">the id of the class user want to register for waiting list</param>
        /// <returns>result object, if result param is false there was an error, should be in the error param</returns>
        [System.Web.Http.HttpGet]
        public ClassEnrollResult JoinWaitingList(int classId)
        {
            var result = ClassBLL.JoinWaitingList(classId, User.Identity.GetUserId());
            return result;
        }




        //public GymCalanderViewModel GetDailyCalander()
        //{
        //    DateTime showdate = DateTime.UtcNow.ToLocal().Hour >= App.Configuration.CalanderChangeHour ? 
        //        DateTime.UtcNow.ToLocal().AddDays(1).Date
        //        : DateTime.UtcNow.ToLocal().Date;
        //    var userid = User.Identity.GetUserId();
        //    var user = Mapper.Map<ApplicationUser>(UserBll.GetUser(userid));
        //    //if (user != null && !user.SignedHealthTandC && App.CurrentCompany.HealthTandCEnabled) return RedirectToAction("SignHealthTandC");
        //    var model = new GymCalanderViewModel { Date = showdate, Classes = new List<ClassViewModel>() };
        //    //StudioBLL.GetDailyClassesForCalander(showdate, userid, user.Gender).ForEach(studioClassModel => model.Classes.Add(Mapper.Map<ClassViewModel>(studioClassModel)));
        //    model.Classes = Mapper.Map<List<ClassViewModel>>(StudioBLL.GetDailyClassesForCalander(showdate, userid, user.Gender, App.CurrentCompany.CalanderMode == "ByDailyRoom"));
        //    model.AvailableRooms = StudioBLL.GetStudioRooms(0);
        //    return model;
        //}

    }
}
