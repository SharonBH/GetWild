using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using InShapeModels;
using DAL;

namespace BLL
{
    public static class ClassBLL
    {
        //readonly ClassRepo _classRepo = new ClassRepo();
        //private static readonly bool SendSMS = ConfigurationManager.AppSettings["SendSMS"].Equals("true");
        public static List<ClassTypeModel> GetClassTypes(int id, int StudioId)
        {
            var classTypesModel = Mapper.Map<List<ClassTypeModel>>(ClassRepo.GetClassTypes(id, StudioId));
            return classTypesModel;
        }

        public static List<ClassTypeDetailsModel> GetClassTypesDetails(int id, bool addEmpty = false)
        {
            var classTypesDetailsModel = ClassRepo.GetClassTypesDetails(id);
            if (addEmpty) classTypesDetailsModel.Add(new ClassTypeDetailsModel { Id = -1, Name = "ללא מערך" });
            return classTypesDetailsModel.OrderBy(c => c.Id).ToList(); ;
        }

        public static List<ClassTypeDetailsModel> GetClassTypesDetailsByType(int id, bool addEmpty = false)
        {
            var classTypesDetailsModel = Mapper.Map<List<ClassTypeDetailsModel>>(ClassRepo.GetClassTypesDetailsByType(id));
            if (addEmpty) classTypesDetailsModel.Add(new ClassTypeDetailsModel { Id = -1, Name = "ללא מערך" });
            return classTypesDetailsModel.OrderBy(c=>c.Id).ToList();
        }

        public static bool CreateClassTypeDetails(ClassTypeDetailsModel classTypeDetailsModel)
        {
            var details = Mapper.Map<ClassTypeDetail>(classTypeDetailsModel);
            return ClassRepo.CreateClassTypeDetails(details);
        }

        public static bool UpdateClassTypeDetails(ClassTypeDetailsModel classTypeDetailsModel)
        {
            var details = Mapper.Map<ClassTypeDetail>(classTypeDetailsModel);
            return ClassRepo.UpdateClassTypeDetails(details);
        }

        public static bool DeleteClassTypeDetails(int classId)
        {
            return ClassRepo.DeleteClassTypeDetails(classId);
        }

        public static List<DailySlotModel> GetClassTimeSlots(int id, int StudioId, bool addother = false)
        {
            var TimeSlots = ClassRepo.GetClassTimeSlots(id, StudioId);
            var dailySlotsModel = new List<DailySlotModel>();
            if (TimeSlots.Any())
                TimeSlots.ForEach(
                    x =>
                        dailySlotsModel.Add(new DailySlotModel
                        {
                            Id = x.Id,
                            Description = x.Description,
                            StartTime = x.StartTime,
                            EndTime = x.EndTime
                        }));
            //if (addother) dailySlotsModel.Add(new DailySlotModel {Id = 99999, Description = "אחר", StartTime = new TimeSpan(0), EndTime = new TimeSpan(0) });
            return dailySlotsModel;
        }

        public static bool CreateClassType(ClassTypeModel classTypeModel)
        {
            var classType = new ClassType
            {
                Description = classTypeModel.Description,
                Name = classTypeModel.Name,
                Picture = classTypeModel.Picture,
                BGColor = classTypeModel.BGColor,
                StudioId = classTypeModel.StudioId
            };
            return ClassRepo.CreateClassType(classType);
        }

        public static bool UpdateClassType(ClassTypeModel classTypeModel)
        {
            var classType = new ClassType
            {
                Id = classTypeModel.Id,
                Description = classTypeModel.Description,
                Name = classTypeModel.Name,
                BGColor = classTypeModel.BGColor,
                StudioId = classTypeModel.StudioId,
                Picture = classTypeModel.Picture
            };
            return ClassRepo.UpdateClassType(classType);
        }

        //internal static List<AvailablePlacementsModel> GetClassAvailablePlacements(StudioClassModel classmodel)
        //{
        //    return ClassRepo.GetClassAvailablePlacements(classmodel);
        //}

        public static bool DeleteClassType(int classId)
        {
            return ClassRepo.DeleteClassType(classId);
        }

        public static List<ClassEnrollmentModel> GetEnrollmentsByClass(int classId, bool withComments = false)
        {
            var result = ClassRepo.GetEnrollmentsByClass(classId);
            if (withComments)
            {
                var comments = ClassRepo.GetClassComments(classId);
                if (comments.Any())
                {
                    foreach (var item in result)
                    {
                        var comment = comments.Where(x => x.UserId == item.UserSubscription.UserId).FirstOrDefault();
                        if (comment != null)
                        {
                            item.Comment = comment.Comment;
                            item.CommentBy = comment.CommentBy;
                            item.CommentByAdmin = comment.IsAdmin;
                            item.CommentDate = comment.CreateDate.Value;
                        }
                    }
                }
            }
            return result;
        }

        public static List<EnrollmentCommentModel> GetClassComments(int classId)
        {
            return ClassRepo.GetClassComments(classId);
        }

        //public static ClassPlacementPrintModel GetPlacementsByClass(int classId)
        //{
        //    return ClassRepo.GetPlacementsByClass(classId);
        //}

        public static List<AvailablePlacementsModel> GetClassAvailablePlacements(int classId, int StudioId, bool includeEmptyEnrollment = false, bool withComments = false)
        {
            var result = ClassRepo.GetClassAvailablePlacements(classId, StudioId);
            if (withComments)
            {
                var comments = ClassRepo.GetClassComments(classId);
                if (comments.Any())
                {
                    foreach (var item in result.Where(p=>p.ClassEnrollment != null))
                    {
                        var comment = comments.Where(x => x.UserId == item.ClassEnrollment.UserSubscription.UserId).FirstOrDefault();
                        if (comment != null)
                        {
                            item.ClassEnrollment.Comment = comment.Comment;
                            item.ClassEnrollment.CommentBy = comment.CommentBy;
                            item.ClassEnrollment.CommentByAdmin = comment.IsAdmin;
                            item.ClassEnrollment.CommentDate = comment.CreateDate.Value;
                        }
                    }
                }
            }
            if (includeEmptyEnrollment) result.ForEach(x => x.SetEmptyClassEnrollmentModel());
            //if (classId > 0) result.ForEach(x => x.ToCreate = !x.IsDeleted);
            return result;
        }

        public static List<CalendarAvailablePlacementsModel> GetClassAvailablePlacementsForDate(DateTime date, int StudioId, int classid = 0)
        {
            if (classid > 0) return ClassRepo.GetClassAvailablePlacementsForClass(classid, StudioId);
            var result = ClassRepo.GetClassAvailablePlacementsForDate(date, StudioId);
            //if (includeEmptyEnrollment) result.ForEach(x => x.SetEmptyClassEnrollmentModel());
            //if (classId > 0) result.ForEach(x => x.ToCreate = !x.IsDeleted);
            return result;
        }

        public static List<ClassEnrollmentModel> GetWaitListEnrollmentsByClass(int classId)
        {
            return ClassRepo.GetWaitListEnrollmentsByClass(classId);
        }
        

        public static DailyClassEnrollmentModel GetEnrollmentsByDate(DateTime date, int StudioId, int? userrole = null, bool removeEmpty = false)
        {
            var classenrollments = new DailyClassEnrollmentModel();
            var classes = StudioBLL.GetDailyClassesForCalander(date, StudioId);
            var enrollments = ClassRepo.GetEnrollmentsByDate(date.Date, StudioId, userrole);
            //var comments = ClassRepo.GetClassComments
            foreach (var studioClassModel in classes)
            {
                studioClassModel.Enrollments = enrollments.Where(x => x.ClassId == studioClassModel.Id).ToList();                
            }
            classenrollments.Classes = removeEmpty ? classes.Where(x => x.Enrollments.Any()).ToList() : classes;
            classenrollments.Date = date;
            return classenrollments;
        }

        public static DailyClassEnrollmentModel GetCommentsByDate(DateTime date, int StudioId, int? userrole = null, bool removeEmpty = false)
        {
            var classenrollments = new DailyClassEnrollmentModel();
            var classes = StudioBLL.GetDailyClassesForCalander(date, StudioId);
            var enrollments = ClassRepo.GetCommentsByDate(date.Date, StudioId, userrole);
            //var comments = ClassRepo.GetClassComments
            foreach (var studioClassModel in classes)
            {
                studioClassModel.Enrollments = enrollments.Where(x => x.ClassId == studioClassModel.Id).ToList();
            }
            classenrollments.Classes = removeEmpty ? classes.Where(x => x.Enrollments.Any()).ToList() : classes;
            classenrollments.Date = date;
            return classenrollments;
        }

        public static DailyClassEnrollmentModel GetActivatedEnrollmentsByDate(DateTime date, int StudioId, int? userrole = null, bool removeEmpty = true)
        {
            var classenrollments = new DailyClassEnrollmentModel();
            var classes = StudioBLL.GetDailyClassesForCalander(date, StudioId);
            var enrollments = ClassRepo.GetActivatedEnrollmentsByDate(date.Date, StudioId, userrole);
            classenrollments.IsReactive = true;
            foreach (var studioClassModel in classes)
            {
                studioClassModel.Enrollments = enrollments.Where(x => x.ClassId == studioClassModel.Id).ToList();
            }
            classenrollments.Classes = removeEmpty ? classes.Where(x => x.Enrollments.Any()).ToList() : classes;
            classenrollments.Date = date;
            return classenrollments;
        }

        public static DailyClassEnrollmentModel GetLateCancelEnrollmentsByDate(DateTime date, int StudioId)
        {
            var classenrollments = new DailyClassEnrollmentModel();
            var classes = StudioBLL.GetDailyClassesForCalander(date, StudioId);
            var enrollments = ClassRepo.GetLateCancelEnrollmentsByDate(date.Date, StudioId);
            classenrollments.IsReactive = true;
            foreach (var studioClassModel in classes)
            {
                studioClassModel.Enrollments = enrollments.Where(x => x.ClassId == studioClassModel.Id).ToList();
            }
            classenrollments.Classes = classes.Where(x => x.Enrollments.Any()).ToList();
            classenrollments.Date = date;
            return classenrollments;
        }

        internal static WaitListEnrollment UserInWaitList(int classId, string userid)
        {
            return ClassRepo.UserInWaitList(classId, userid);
        }

        public static DailyClassEnrollmentModel GetEnrollmentsByWeek(int weekno, int StudioId, int? userrole = null, bool removeEmpty = false)
        {
            var classenrollments = new DailyClassEnrollmentModel();
            var classes = StudioBLL.GetWeeklyClassesForCalander(weekno, StudioId);
            var enrollments = ClassRepo.GetEnrollmentsByWeek(weekno, StudioId, userrole);
            foreach (var studioClassModel in classes)
            {
                studioClassModel.Enrollments = enrollments.Where(x => x.ClassId == studioClassModel.Id).ToList();
            }
            classenrollments.Classes = removeEmpty ? classes.Where(x => x.Enrollments.Any()).ToList() : classes;
            //classenrollments.Date = date;
            return classenrollments;
        }

        public static bool IsUserEnrolled(int classId, string userid)
        {
            return ClassRepo.IsUserEnrolled(classId, userid);
        }

        public static CalendarClassEnrollmentModel GetUserEnrollment(int classId, string userid)
        {
            return ClassRepo.GetUserEnrollment(classId, userid);
        }

        public static List<ClassEnrollmentModel> GetEnrollmentsBySubscription(int subscriptionId)
        {
            var classenrollments = new List<ClassEnrollmentModel>();
            var enrollments = ClassRepo.GetEnrollmentsBySubscription(subscriptionId);
            if (enrollments.Any())
                enrollments.ForEach(
                    x =>
                        classenrollments.Add(new ClassEnrollmentModel
                        {
                            Id = x.Id,
                            ClassId = x.ClassId,
                            SubscriptionId = x.SubscriptionId,
                            DateEnrolled = x.DateEnrolled,
                            IsDeleted = x.IsDeleted,
                            DateCanceled = x.DateCanceled
                        }));
            return classenrollments;
        }

        //public static List<ClassEnrollmentModel> GetEnrollmentsByUser(string userId)
        //{
        //    var classenrollments = new List<ClassEnrollmentModel>();
        //    var enrollments = ClassRepo.GetEnrollmentsByUser(userId);
        //    classenrollments = enrollments.Select(x => new ClassEnrollmentModel
        //    {
        //        Id = x.Id,
        //        ClassId = x.ClassId,
        //        SubscriptionId = x.SubscriptionId,
        //        DateEnrolled = x.DateEnrolled,
        //        IsDeleted = x.IsDeleted,
        //        DateCanceled = x.DateCanceled,
        //        StudioClass = new StudioClassModel { Name = x.Class.Name, Date = x.Class.Date },
        //        Subscription = new UserSubscriptionModel { Id = x.UserSubscription.Id }
        //    }).ToList();

        //    return classenrollments;
        //}

        public static List<ClassEnrollmentModel> GetEnrollmentsByUser(string userId, bool next)
        {
            var enrollments = next ? ClassRepo.GetNextEnrollmentsByUser(userId) : ClassRepo.GetEnrollmentsByUser(userId);
            return enrollments;
        }

        public static List<CalendarClassEnrollmentModel> GetEnrollmentsByUserDate(string userId, DateTime date, bool next = false)
        {
            var enrollments = ClassRepo.GetEnrollmentsByUserDate(userId, date, next);
            return enrollments;
        }

        public static List<WaitListEnrollment> GetWaitingListByUserDate(string userId, DateTime date)
        {
            var enrollments = ClassRepo.GetWaitingListByUserDate(userId, date);
            return enrollments;
        }

        public static ClassEnrollResult AdminMarkNoShow(int enrollmentId, bool value)
        {
            return ClassRepo.AdminMarkNoShow(enrollmentId, value);
        }

        public static List<ClassEnrollmentModel> GetOldEnrollmentsByUser(string userId)
        {
            var enrollments = ClassRepo.GetOldEnrollmentsByUser(userId);
            return enrollments;
        }

        public static ClassEnrollmentModel GetLastEnrollmentsByUser(string userId)
        {
            var enrollment = ClassRepo.GetLastEnrollmentByUser(userId);
            return enrollment;
        }

        public static ClassEnrollResult RateClass(int EnrolmentId, int rating)
        {
            return ClassRepo.RateClass(EnrolmentId, rating);
        }


        public static ClassEnrollResult EnrollUserToClass(int classId, int classAvailablePlacementId, string userId, bool isFromWaitList, int NumAdvncedEnrollments, int LateRegistration, string CancelationPenalty, bool isAdmin = false)
        {
            var result = ClassRepo.EnrollUser(classId, classAvailablePlacementId, userId, isFromWaitList, NumAdvncedEnrollments, LateRegistration, CancelationPenalty, isAdmin);
            //if (result.Result && SendSMS) SMSBLL.SendOnEnrollmentAlert(userId, classId);
            return result;
        }

        //public static ClassEnrollResult EnrollUserToClassFromWaitList(int classId, string placementkey, string userId, bool isAdmin = false)
        //{
        //    var result = ClassRepo.EnrollUserFromWaitList(classId, placementkey, userId, isAdmin);
        //    //if (result.Result && SendSMS) SMSBLL.SendOnEnrollmentAlert(userId, classId);
        //    return result;
        //}
        

        public static ClassEnrollResult OutrolltUserToClass(int classId, string userId, int CancellationThresholdMins, int LateCancellationThresholdMins, bool isLate = false)
        {

            var result = ClassRepo.CancelEnrollment(classId, userId, CancellationThresholdMins, LateCancellationThresholdMins, isLate);
            //if (result.StartWaitingListProcess) SMSBLL.SendWaitingListMSG(classId);
            //if (result.Result && SendSMS) SMSBLL.SendOnOutrollmentAlert(userId, classId);
            return result;
            }

        //public static bool OutrolltUserToClass(int subscriptionId)
        //{
        //    var result = ClassRepo.CancelEnrollment(subscriptionId);
        //    if (result && SendSMS) SMSBLL.SendOnOutrollmentAlert(subscriptionId);
        //    return result;
        //}

        public static ClassEnrollResult AdminCancelEnrollment(int classId, int subscriptionId, string userId)
        {
            var result = ClassRepo.AdminCancelEnrollment(classId, subscriptionId, userId);
            //if (result.StartWaitingListProcess) SMSBLL.SendWaitingListMSG(classId);
            return result;
        }

        public static ClassEnrollResult AdminRemoveFromWaitList(int classId, int subscriptionId, string userId )
        {
            return ClassRepo.CancelWaitingList(classId, subscriptionId, userId);
        }

        public static ClassEnrollResult JoinWaitingList(int classId, string userId, bool isAdmin = false)
        {
            return ClassRepo.JoinWaitingList(classId, userId, isAdmin);
        }

        public static ClassEnrollResult CancelWaitingLis(int classId, string userId, bool isAdmin = false)
        {
            return ClassRepo.CancelWaitingList(classId, userId, isAdmin);
        }

        public static bool SaveUserComment(EnrollmentCommentModel comment)
        {
            var commentdal = Mapper.Map<EnrollmentComment>(comment);
            return ClassRepo.SaveUserComment(commentdal);
        }

        public static EnrollmentCommentModel GetUserComment(string userId, int enrollmentId)
        {
            return ClassRepo.GetUserComment(userId, enrollmentId);
        }
    }
}
