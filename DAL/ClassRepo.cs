using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using InShapeModels;
using Utilities;

namespace DAL
{
    public static class ClassRepo
    {
        private static Object thisLock = new Object();
        public static List<ClassType> GetClassTypes(int typeId, int StudioId)
        {
            using (var context = new InShapeEntities())
            {
                return typeId > 0
                    ? context.ClassTypes.Where(r => r.Id == typeId && !r.IsDeleted).ToList()
                    : context.ClassTypes.Where(r => !r.IsDeleted).FilterByUser(StudioId).ToList();
            }
        }

        public static List<ClassTypeDetailsModel> GetClassTypesDetails(int typeId)
        {
            var ClassTypesDetails = new List<ClassTypeDetailsModel>();
            var now = DateTime.UtcNow.ToLocal();
            var from1m = DateTime.UtcNow.ToLocal().AddMonths(-1);
            var from3m = DateTime.UtcNow.ToLocal().AddMonths(-3);
            using (var context = new InShapeEntities())
            {
                if (typeId > 0)
                {
                    ClassTypesDetails =
                        context.ClassTypeDetails.Include("ClassType")
                            .Where(r => !r.IsDeleted && r.Id == typeId)
                            .ProjectTo<ClassTypeDetailsModel>()
                            .ToList();
                }
                else
                {
                    ClassTypesDetails =
                        context.ClassTypeDetails.Include("ClassType")
                            .Where(r => !r.IsDeleted)
                            .ProjectTo<ClassTypeDetailsModel>()
                            .ToList();
                    ClassTypesDetails.ForEach( c=>
                    {
                        var lastclass =
                            context.Classes.OrderByDescending(d => d.Date)
                                .FirstOrDefault(x => !x.IsDeleted && x.ClassTypeDetailsId == c.Id && x.Date < now);
                        if (lastclass != null) c.LastClass = lastclass.Date;
                        var nextclass =
                            context.Classes.OrderBy(d => d.Date)
                                .FirstOrDefault(x => !x.IsDeleted && x.ClassTypeDetailsId == c.Id && x.Date > now);
                        if (nextclass != null) c.NextClass = nextclass.Date;
                        c.Usage1Month =
                            context.Classes.Count(
                                x =>
                                    !x.IsDeleted && x.ClassTypeDetailsId == c.Id && x.Date > from1m &&
                                    x.Date < now);
                        c.Usage3Month =
                            context.Classes.Count(
                                x =>
                                    !x.IsDeleted && x.ClassTypeDetailsId == c.Id && x.Date > from3m &&
                                    x.Date < now);
                    });
                }
            }
            return ClassTypesDetails;
        }

        public static List<ClassTypeDetail> GetClassTypesDetailsByType(int typeId)
        {
            using (var context = new InShapeEntities())
            {
                return context.ClassTypeDetails.Include("ClassType").Where(r => !r.IsDeleted && r.ClassTypeId==typeId).ToList();
            }
        }

        public static bool CreateClassTypeDetails(ClassTypeDetail classTypedetails)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    context.ClassTypeDetails.Add(classTypedetails);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        //public static List<AvailablePlacementsModel> GetClassAvailablePlacements(StudioClassModel classmodel)
        //{
        //    List<AvailablePlacementsModel> classPlacements = new List<AvailablePlacementsModel>();
        //    foreach (var placement in classmodel.ClassPlacements)
        //    {
        //        for (int i = 1; i <= placement.Number; i++)
        //        {
        //            classPlacements.Add(new AvailablePlacementsModel {Key = placement.StudioPlacementId+"_"+ i, StudioPlacementId = placement.StudioPlacementId, StudioPlacementName = placement.StudioPlacementName, PlacementNumber = i });
        //        }
        //    } 
        //        using (var context = new InShapeEntities())
        //    {

        //        var p = context.ClassEnrollments.Where(pl => pl.ClassId == classmodel.Id && !pl.IsDeleted);
        //        //    var classparam = new SqlParameter("classId", classId);
        //        //    context.Database.SqlQuery<List<AvailablePlacementsModel>>("EXEC GetAvailablePlacements @classId", classparam);
        //        classPlacements.RemoveAll(x => p.Any(y => y.StudioPlacementId == x.StudioPlacementId && y.ClassPlacementNumber == x.PlacementNumber));
        //    }
        //    return classPlacements;

        //}

        public static bool UpdateClassTypeDetails(ClassTypeDetail classTypedetails)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    context.ClassTypeDetails.Attach(classTypedetails);
                    context.Entry(classTypedetails).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public static bool DeleteClassTypeDetails(int typeId)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var now = DateTime.UtcNow.ToLocal();
                    var classTypedetails = context.ClassTypeDetails.First(r => r.Id == typeId && !r.IsDeleted);
                    //context.StudioRooms.Remove(room);
                    classTypedetails.IsDeleted = true;
                    var classes =
                        context.Classes.Where(c => !c.IsDeleted && c.Date >= now && c.ClassTypeDetailsId == typeId);
                    classes.ToList().ForEach(c=>c.ClassTypeDetailsId = null);
                    //context.Classes(classes);
                    context.ClassTypeDetails.Attach(classTypedetails);
                    context.Entry(classTypedetails).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public static List<ClassDailySlot> GetClassTimeSlots(int slotId, int StudioId)
        {
            using (var context = new InShapeEntities())
            {
                return slotId > 0 ? context.ClassDailySlots.Where(r => r.Id == slotId && !r.IsDeleted).ToList() : context.ClassDailySlots.Where(r => !r.IsDeleted).FilterByUser(StudioId).OrderBy(x=>x.StartTime).ToList();
            }
        }

        public static bool CreateClassType(ClassType classType)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    context.ClassTypes.Add(classType);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }


        public static bool UpdateClassType(ClassType classType)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    context.ClassTypes.Attach(classType);
                    context.Entry(classType).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public static bool DeleteClassType(int typeId)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var classType = context.ClassTypes.First(r => r.Id == typeId && !r.IsDeleted);
                    //context.StudioRooms.Remove(room);
                    classType.IsDeleted = true;
                    context.ClassTypes.Attach(classType);
                    context.Entry(classType).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }


        public static ClassEnrollment GetEnrollment(int enrolmentId)
        {
            using (var context = new InShapeEntities())
            {
                return context.ClassEnrollments.FirstOrDefault(r => r.Id == enrolmentId && !r.IsDeleted);
            }
        }

        public static bool IsUserEnrolled(int classid, string userid)
        {
            using (var context = new InShapeEntities())
            {
                return context.ClassEnrollments.Any(r => r.ClassId == classid && r.UserSubscription.UserId == userid && r.IsDeleted == false);
            }
        }

        public static WaitListEnrollment UserInWaitList(int classid, string userid)
        {
            using (var context = new InShapeEntities())
            {
                return context.ClassWaitLists.Where(r => r.ClassId == classid && r.UserSubscription.UserId == userid && r.IsDeleted == false)
                    .ProjectTo<WaitListEnrollment>()
                    .FirstOrDefault();
            }
        }

        public static CalendarClassEnrollmentModel GetUserEnrollment(int classid, string userid)
        {
            using (var context = new InShapeEntities())
            {
                return Mapper.Map<CalendarClassEnrollmentModel>(context.ClassEnrollments //.Include("ClassAvailablePlacement")
                    .FirstOrDefault(r => r.ClassId == classid && r.UserSubscription.UserId == userid && r.IsDeleted == false));
                    //.ProjectTo<ClassEnrollmentModel>()
                    //.FirstOrDefault();
            }
        }

        public static List<ClassEnrollmentModel> GetEnrollmentsByClass(int classId)
        {
            using (var context = new InShapeEntities())
            {
                return Mapper.Map<List<ClassEnrollmentModel>>(context.ClassEnrollments
                    .Include("UserSubscription").Include("Class").Include("ClassAvailablePlacement")
                    .Include("UserSubscription.AspNetUser")
                    .Where(r => r.ClassId == classId && !r.IsDeleted)
                    //.ProjectTo<ClassEnrollmentModel>()
                    .OrderBy(y => y.ClassAvailablePlacement.StudioPlacementId).ThenBy(y => y.ClassAvailablePlacement.ClassPlacementNumber)
                    .ToList());
            }
        }

        //public static ClassPlacementPrintModel GetPlacementsByClass(int classId)
        //{
        //    var Placements = new ClassPlacementPrintModel { ClassId = classId };
        //    using (var context = new InShapeEntities())
        //    {
        //        Placements.Enrollments = Mapper.Map<List<ClassEnrollmentModel>>(context.ClassEnrollments
        //            .Include("UserSubscription").Include("Class").Include("ClassAvailablePlacement")
        //            .Include("UserSubscription.AspNetUser")
        //            .Where(r => r.ClassId == classId && !r.IsDeleted)
        //            //.ProjectTo<ClassEnrollmentModel>()
        //            .ToList());

        //        Placements.Placements = Mapper.Map<List<ClassPlacementModel>>(context.ClassPlacements.Where(p => p.ClassId == classId));
        //    }
        //    return Placements;
        //}

        public static List<AvailablePlacementsModel> GetClassAvailablePlacements(int classId, int StudioId)
        {
            using (var context = new InShapeEntities())
            {
                if (classId > 0) return Mapper.Map<List<AvailablePlacementsModel>>(context.ClassAvailablePlacements.Where(r => r.ClassId == classId)
                    .OrderBy(x=>x.StudioPlacementId).ThenBy(x=>x.ClassPlacementNumber)
                    //.ProjectTo<ClassEnrollmentModel>()
                    .ToList());
                return GenerateEmptyClassAvailablePlacements(StudioId);
            }
        }

        //public static List<AvailablePlacementsModel> GetClassAvailablePlacements(int classId, int StudioId, bool withComments)
        //{
        //    var result = new List<AvailablePlacementsModel>();
        //    using (var context = new InShapeEntities())
        //    {
        //        if (classId > 0) result = Mapper.Map<List<AvailablePlacementsModel>>(context.ClassAvailablePlacements.Where(r => r.ClassId == classId)
        //            .OrderBy(x => x.StudioPlacementId).ThenBy(x => x.ClassPlacementNumber)
        //            //.ProjectTo<ClassEnrollmentModel>()
        //            .ToList());
        //        result = GenerateEmptyClassAvailablePlacements(StudioId);

        //        if (withComments)
        //        {
        //            var comments = GetClassComments(classId);
        //            foreach (var item in result)
        //            {
        //                var comment = comments.Where(x => x.UserId == item.ClassEnrollment.UserSubscription.UserId).FirstOrDefault();
        //                item.ClassEnrollment.Comment = comment.Comment;
        //                item.ClassEnrollment.CommentBy = comment.CommentBy;
        //                item.ClassEnrollment.CommentDate = comment.CreateDate;
        //            }
        //        }
        //    }

        //    return result;
        //}

        public static List<EnrollmentCommentModel> GetClassComments(int classId)
        {
            using (var context = new InShapeEntities())
            {
                //var StudioIdParameter = new SqlParameter("StudioId", StudioId);
                var ClassIdParameter = new SqlParameter("ClassId", classId);
                //return context.Database.SqlQuery<EnrollmentCommentModel>("EXEC GetCommentsForClass @StudioId, @ClassId", StudioIdParameter, ClassIdParameter).ToList();
                return context.Database.SqlQuery<EnrollmentCommentModel>("EXEC GetCommentsForClass @ClassId", ClassIdParameter).ToList();
            }
        }

        public static List<CalendarAvailablePlacementsModel> GetClassAvailablePlacementsForDate(DateTime date, int StudioId)
        {
            using (var context = new InShapeEntities())
            {
                return Mapper.Map<List<CalendarAvailablePlacementsModel>>(context.ClassAvailablePlacements
                    .Where(r => DbFunctions.TruncateTime(r.Class.Date) == date && r.Class.StudioRoom.StudioId == StudioId)
                    .OrderBy(x => x.StudioPlacementId).ThenBy(x => x.ClassPlacementNumber)
                    //.ProjectTo<ClassEnrollmentModel>()
                    .ToList());
            }
        }

        public static List<CalendarAvailablePlacementsModel> GetClassAvailablePlacementsForClass(int classId, int StudioId)
        {
            using (var context = new InShapeEntities())
            {
                return Mapper.Map<List<CalendarAvailablePlacementsModel>>(context.ClassAvailablePlacements
                    .Where(r => r.Class.Id == classId && r.Class.StudioRoom.StudioId == StudioId)
                    .OrderBy(x => x.StudioPlacementId).ThenBy(x => x.ClassPlacementNumber)
                    //.ProjectTo<ClassEnrollmentModel>()
                    .ToList());
            }
        }

        public static EnrollmentCommentModel GetUserComment(string userId, int enrollmentId)
        {
            using (var context = new InShapeEntities())
            {
                return Mapper.Map<EnrollmentCommentModel>(context.EnrollmentComments.FirstOrDefault(r => r.EnrollmentId == enrollmentId && r.UserId == userId && !r.IsDeleted));
            }
        }

        public static bool SaveUserComment(EnrollmentComment comment)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    comment.CreateDate = DateTime.UtcNow.ToLocal();
                    if (comment.Id > 0)
                    {
                        context.EnrollmentComments.Attach(comment);
                        context.Entry(comment).State = EntityState.Modified;
                    }
                    else
                    {
                        if (context.EnrollmentComments.Any(c => c.ClassId == comment.ClassId && c.EnrollmentId == comment.EnrollmentId)) return false;
                        context.EnrollmentComments.Add(comment);
                    }
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        private static List<AvailablePlacementsModel> GenerateEmptyClassAvailablePlacements(int StudioId)
        {
            List<AvailablePlacementsModel> result = new List<AvailablePlacementsModel>();
            var studioPlacments = StudioRepo.GetStudioPlacements(0, StudioId);
            foreach (var item in studioPlacments)
            {
                //var lenth = item.Id == 999 ? 2 : 11;
                for (int i = 1; i <= item.Places; i++)
                {
                    result.Add(new AvailablePlacementsModel { ClassPlacementNumber = Convert.ToByte(i), IsDeleted = false, IsInUse = false, StudioPlacementId = item.Id, StudioPlacementName = item.Name, ToCreate = true, TypeId = item.TypeId });
                }
            }
            return result;
            }

        public static List<ClassEnrollmentModel> GetWaitListEnrollmentsByClass(int classId)
        {
            using (var context = new InShapeEntities())
            {
                var enrollments = Mapper.Map<List<ClassEnrollmentModel>>(
                    context.ClassWaitLists.Include("UserSubscription").Include("Class").Include("UserSubscription.AspNetUser")
                    .Where(r => r.ClassId == classId && !r.IsDeleted)
                    //.ProjectTo<ClassEnrollmentModel>()
                    .ToList());
                
                foreach (ClassEnrollmentModel enrollment in enrollments)
                {
                    //enrollment.HaveClassToday =
                    //    context.ClassEnrollments.Count(e => e.SubscriptionId == enrollment.SubscriptionId &
                    //    DbFunctions.TruncateTime(e.Class.Date) == DbFunctions.TruncateTime(enrollment.Class.Date) && e.IsDeleted == false) > 0;
                    enrollment.Class = Mapper.Map<StudioClassModel>(
                        context.Classes.FirstOrDefault(
                            e => e.ClassEnrollments.Any(
                                    x => x.SubscriptionId == enrollment.SubscriptionId && x.ClassId != enrollment.ClassId &&
                                        !x.IsDeleted) && 
                                DbFunctions.TruncateTime(e.Date) == DbFunctions.TruncateTime(enrollment.Class.Date) &&
                                e.IsDeleted == false));
                }
                //var q = from d in context.ClassWaitLists.Where(r => r.ClassId == classId && !r.IsDeleted)
                //        .Include("UserSubscription").Include("Class").Include("UserSubscription.AspNetUser")
                //        select new ClassEnrollmentModel
                //        {
                //            Id = d.Id,
                //            MessageCount = d.Messages.Count(),
                //        };
                return enrollments;
            }
        }

        public static List<ClassEnrollmentModel> GetEnrollmentsByDate(DateTime date, int StudioId, int? userrole = null)
        {
            DateTime enddate = date.AddDays(1);
            var result = new List<ClassEnrollmentModel>();

            using (var context = new InShapeEntities())
            {
                if (userrole.HasValue)
                {
                    result = Mapper.Map<List<ClassEnrollmentModel>>(
                        context.ClassEnrollments.Include("UserSubscription")
                            .Include("UserSubscription.AspNetUser")
                            .Include("Class")
                            .Where(r => r.Class.Date >= date && r.Class.Date <= enddate && !r.IsDeleted
                                        && r.UserSubscription.AspNetUser.AspNetUserRoles.Any(ur => ur.RoleId == userrole.Value.ToString()))
                            .FilterByUser(StudioId)
                            //.ProjectTo<ClassEnrollmentModel>()
                            .ToList());
                }
                else
                {
                    result = Mapper.Map<List<ClassEnrollmentModel>>(
                        context.ClassEnrollments.Include("UserSubscription")
                            .Include("UserSubscription.AspNetUser")
                            .Include("Class")
                            .Where(r => r.Class.Date >= date && r.Class.Date <= enddate && !r.IsDeleted)
                            .FilterByUser(StudioId)
                            //.ProjectTo<ClassEnrollmentModel>()
                            .ToList());
                }
                
            }
            return result;
        }

        public static List<DailyExportModel> GetDailyExport (DateTime date, int StudioId, int? userrole = null)
        {
            DateTime enddate = date.AddDays(1);
            var result = new List<DailyExportModel>();

            using (var context = new InShapeEntities())
            {
                result = Mapper.Map<List<DailyExportModel>>(context.DailyExports.Where(e => e.StudioId == StudioId && e.Date >= date && e.Date < enddate));
            }
            if (userrole.HasValue) result.RemoveAll(r => r.RoleId != userrole);
            return result.OrderBy(x=>x.Date).ToList();
        }

        public static List<ClassEnrollmentModel> GetCommentsByDate(DateTime date, int StudioId, int? userrole = null)
        {
            DateTime enddate = date.AddDays(1);
            var result = new List<ClassEnrollmentModel>();

            using (var context = new InShapeEntities())
            {
                if (userrole.HasValue)
                {
                    result = Mapper.Map<List<ClassEnrollmentModel>>(
                        context.ClassEnrollments.Include("UserSubscription")
                            .Include("UserSubscription.AspNetUser")
                            .Include("Class")
                            .Where(r => r.Class.Date >= date && r.Class.Date <= enddate && !r.IsDeleted
                                        && r.UserSubscription.AspNetUser.AspNetUserRoles.Any(ur => ur.RoleId == userrole.Value.ToString()))
                            .FilterByUser(StudioId)
                            //.ProjectTo<ClassEnrollmentModel>()
                            .ToList());
                }
                else
                {
                    result = Mapper.Map<List<ClassEnrollmentModel>>(
                        context.ClassEnrollments.Include("UserSubscription")
                            .Include("UserSubscription.AspNetUser")
                            .Include("Class")
                            .Where(r => r.Class.Date >= date && r.Class.Date <= enddate && !r.IsDeleted)
                            .FilterByUser(StudioId)
                            //.ProjectTo<ClassEnrollmentModel>()
                            .ToList());
                }
                var ids = result.Select(x => x.Id);
                var comments = context.EnrollmentComments.Where(ec => ids.Any(p => ec.EnrollmentId == p)).ToList();
                result.RemoveAll(x => !comments.Any(c => c.EnrollmentId == x.Id));
                foreach (var i in result)
                {
                    var comment = comments.FirstOrDefault(c => c.EnrollmentId == i.Id);
                    if (comment != null)
                    {
                        i.Comment = comment.Comment;
                        i.CommentBy = comment.AspNetUser.FullName;
                        i.CommentByAdmin = comment.AspNetUser.AspNetUserRoles.Any(x => x.RoleId == ((int)AdminType.Instructor).ToString());
                        i.CommentDate = comment.CreateDate;
                    }
                }
            }
            return result;
        }


        public static List<ClassEnrollmentModel> GetActivatedEnrollmentsByDate(DateTime date, int StudioId, int? userrole = null)
        {
            DateTime enddate = date.AddDays(1);
            DateTime dateDay = date.Date;
            if (userrole.HasValue)
            {
                using (var context = new InShapeEntities())
                {
                    return Mapper.Map<List<ClassEnrollmentModel>>(
                        context.ClassEnrollments.Include("UserSubscription")
                            .Include("UserSubscription.AspNetUser")
                            .Include("Class")
                            .Include("LastClasses")
                            .Where(r => r.Class.Date >= date && r.Class.Date <= enddate && !r.IsDeleted
                            && r.UserSubscription.AspNetUser.LastClasses.Any(x => x.RowDate == dateDay && DbFunctions.DiffDays(x.Date, x.RowDate) >= 14)
                                        && r.UserSubscription.AspNetUser.AspNetUserRoles.Any(ur => ur.RoleId == userrole.Value.ToString()))
                            .FilterByUser(StudioId)
                            //.ProjectTo<ClassEnrollmentModel>()
                            .ToList());
                }

            }
            using (var context = new InShapeEntities())
            {
                return Mapper.Map<List<ClassEnrollmentModel>>(
                    context.ClassEnrollments.Include("UserSubscription")
                        .Include("UserSubscription.AspNetUser")
                        .Include("Class")
                        .Where(r => r.Class.Date >= date && r.Class.Date <= enddate && !r.IsDeleted
                        && r.UserSubscription.AspNetUser.LastClasses.Any(x=> x.RowDate == dateDay && DbFunctions.DiffDays(x.Date, x.RowDate) >= 14))
                        //&& DbFunctions.DiffDays(r.UserSubscription.AspNetUser.LastClasses.FirstOrDefault().Date, date) >= 14)
                        .FilterByUser(StudioId)
                        //.ProjectTo<ClassEnrollmentModel>()
                        .ToList());
            }
        }

        public static List<ClassEnrollmentModel> GetLateCancelEnrollmentsByDate(DateTime date, int StudioId)
        {
            DateTime enddate = date.AddDays(1);
            DateTime dateDay = date.Date;
            using (var context = new InShapeEntities())
            {
                return Mapper.Map<List<ClassEnrollmentModel>>(
                        context.ClassEnrollments.Include("UserSubscription")
                            .Include("UserSubscription.AspNetUser")
                            .Include("Class")
                            .Where(r => r.Class.Date >= date && r.Class.Date <= enddate && r.IsDeleted
                                        && r.IsLateCancel)
                            .FilterByUser(StudioId)
                            //.ProjectTo<ClassEnrollmentModel>()
                            .ToList());
            }
        }
        

        public static List<ClassEnrollmentModel> GetEnrollmentsByWeek(int weekno, int StudioId, int? userrole = null)
        {
            var startdate = Utilities.Utils.FirstDateOfWeek(DateTime.UtcNow.Year, weekno).Date;
            var enddate = startdate.Date.AddDays(7);
            if (userrole.HasValue)
            {
                using (var context = new InShapeEntities())
                {
                    return Mapper.Map<List<ClassEnrollmentModel>>(
                        context.ClassEnrollments.Include("UserSubscription")
                            .Include("UserSubscription.AspNetUser")
                            .Include("Class")
                            .Where(r => r.Class.Date >= startdate && r.Class.Date <= enddate && !r.IsDeleted 
                                        && r.UserSubscription.AspNetUser.AspNetUserRoles.Any(ur => ur.RoleId == userrole.Value.ToString()))
                            .FilterByUser(StudioId)
                            //.ProjectTo<ClassEnrollmentModel>()
                            .ToList());
                }
                
            }
            using (var context = new InShapeEntities())
            {
                return Mapper.Map<List<ClassEnrollmentModel>>(
                    context.ClassEnrollments.Include("UserSubscription")
                        .Include("UserSubscription.AspNetUser")
                        .Include("Class")
                        .Where(r => r.Class.Date >= startdate && r.Class.Date <= enddate && !r.IsDeleted)
                        .FilterByUser(StudioId)
                        //.ProjectTo<ClassEnrollmentModel>()
                        .ToList());
            }
        }

        public static List<ClassEnrollment> GetEnrollmentsBySubscription(int subscriptionId)
        {
            using (var context = new InShapeEntities())
            {
                return context.ClassEnrollments.Where(r => r.SubscriptionId == subscriptionId && !r.IsDeleted).ToList();
            }
        }

        //public static IQueryable<ClassEnrollment> GetEnrollmentsByUser(string userId)
        //{
        //    using (var context = new InShapeEntities())
        //    {
        //        var query = from ce in context.ClassEnrollments.Include("").Include("")
        //        where ce.UserSubscription.AspNetUser.Id == userId && ce.UserSubscription.Active
        //        select ce;

        //        return query;
        //    }
        //}

        public static List<ClassEnrollmentModel> GetEnrollmentsByUser(string userId)
        {
            using (var context = new InShapeEntities())
            {
                var query = from ce in context.ClassEnrollments.Include("Class").Include("UserSubscription").Include("ClassAvailablePlacement")
                            where ce.UserSubscription.AspNetUser.Id == userId && ce.UserSubscription.Active && !ce.IsDeleted
                            select ce;

                return Mapper.Map<List<ClassEnrollmentModel>>(query.OrderByDescending(x=> x.Class.Date).ToList());
            }
        }

        //public static List<ClassEnrollmentModel> GetCalendarEnrollmentsByUserDate(string userId, DateTime date)
        //{
        //    //List<ClassEnrollmentModel> enrollments = new List<ClassEnrollmentModel>();
        //    List<ClassEnrollment> enrollments = new List<ClassEnrollment>();
        //    using (var context = new InShapeEntities())
        //    {
        //        enrollments = context.ClassEnrollments.Where(x => !x.IsDeleted && x.UserSubscription.UserId == userId && DbFunctions.TruncateTime(x.Class.Date) == date).ToList();

        //        //.OrderByDescending(x => x.Class.Date).ToList());
        //    }
        //    return Mapper.Map<List<ClassEnrollmentModel>>(enrollments);
        //}

        public static List<CalendarClassEnrollmentModel> GetEnrollmentsByUserDate(string userId, DateTime date, bool next)
        {
            using (var context = new InShapeEntities())
            {
                SqlParameter dateParameter = new SqlParameter();
                if (date == DateTime.MinValue) dateParameter = new SqlParameter("Date", DBNull.Value);
                else dateParameter = new SqlParameter("Date", date);
                //var dateParameter = new SqlParameter("Date", next ? DateTime.UtcNow.ToLocal() : date);
                var userIdParameter = new SqlParameter("UserId", userId);
                var nextParameter = new SqlParameter("Next", next);
                return context.Database.SqlQuery<CalendarClassEnrollmentModel>("EXEC GetUserEnrollForDailyCalendar @Date, @UserId, @Next", dateParameter, userIdParameter, nextParameter).ToList();
            }
        }

        public static List<WaitListEnrollment> GetWaitingListByUserDate(string userId, DateTime date)
        {
            using (var context = new InShapeEntities())
            {
                var query = from ce in context.ClassWaitLists.Include("Class").Include("UserSubscription")
                            where ce.UserSubscription.AspNetUser.Id == userId && ce.UserSubscription.Active && !ce.IsDeleted
                            && DbFunctions.TruncateTime(ce.Class.Date) == date
                            select ce;

                return Mapper.Map<List<WaitListEnrollment>>(query.OrderByDescending(x => x.Class.Date).ToList());
            }
        }

        

        public static ClassEnrollResult CancelWaitingList(int classId, string userId, bool isAdmin)
        {
            var result = new ClassEnrollResult { Result = false };
            using (var context = new InShapeEntities())
            {
                var wl = context.ClassWaitLists.FirstOrDefault(
                        w => w.ClassId == classId && w.UserSubscription.UserId == userId && !w.IsDeleted);
                if (wl == null) { result.Error = "אינך ברשימת ההמתנה לאימון זה "; return result; };
                wl.IsDeleted = true;
                wl.DateCanceled = DateTime.UtcNow.ToLocal();
                context.ClassWaitLists.Attach(wl);
                context.Entry(wl).State = EntityState.Modified;
                context.SaveChanges();
                var Class = context.Classes.FirstOrDefault(c => c.Id == classId && !c.IsDeleted);
                //if (Class != null)
                //{
                //    Class.WaitingList -= 1;
                //    Class.IsFull = Class.WaitingList > 0 || (Class.MaxParticipants - Class.Participants) == 0;
                //    context.Classes.Attach(Class);
                //    context.Entry(Class).State = EntityState.Modified;
                //}
                
                result.Result = true;
                result.RoomId = Class.StudioRoomId;
                result.Date = Class.Date;
                Logger.WriteDebug($"running CancelWaitingList, Class:{classId}, User: {userId}, WaitingList for {Class.Date}: {Class.WaitingList}");
            }
            Logger.WriteDebug($"running CancelWaitingList, Class:{classId}, User: {userId}, admin: {isAdmin}");
            return result;
        }

        
        public static ClassEnrollResult CancelWaitingList(int classId, int subscriptionId, string userId)
        {
            var result = new ClassEnrollResult { Result = false };
            using (var context = new InShapeEntities())
            {
                var wl = context.ClassWaitLists.FirstOrDefault(
                        w => w.ClassId == classId && w.SubscriptionId == subscriptionId && !w.IsDeleted);
                if (wl == null) { result.Error = "אינך ברשימת ההמתנה לאימון זה "; return result; };
                wl.IsDeleted = true;
                wl.DateCanceled = DateTime.UtcNow.ToLocal();
                context.ClassWaitLists.Attach(wl);
                context.Entry(wl).State = EntityState.Modified;
                context.SaveChanges();
                var Class = context.Classes.FirstOrDefault(c => c.Id == classId && !c.IsDeleted);
                //if (Class != null)
                //{
                //    Class.WaitingList = Class.WaitingList - 1;
                //    Class.IsFull = Class.WaitingList > 0 || (Class.MaxParticipants - Class.Participants) == 0;
                //    context.Classes.Attach(Class);
                //    context.Entry(Class).State = EntityState.Modified;
                //}
                
                result.Result = true;
                result.RoomId = Class.StudioRoomId;
                result.Date = Class.Date;
                Logger.WriteDebug($"running CancelWaitingList, Class:{classId}, User: {userId}, subscriptionId: {subscriptionId}, WaitingList for {Class.Date}: {Class.WaitingList}");
            }
            
            return result;
        }

        public static ClassEnrollResult JoinWaitingList(int classId, string userId, bool isAdmin)
        {
            var result = new ClassEnrollResult {Result = false};
            using (var context = new InShapeEntities())
            {
                var Class = context.Classes.FirstOrDefault(c => c.Id == classId && c.IsFull.Value && !c.IsDeleted);
                if (Class == null)
                {
                    result.Error = "האימון שבחרת לא מלא";
                    return result;
                }
                var us =
                    context.UserSubscriptions.FirstOrDefault(
                        s => s.UserId == userId && s.Active && (s.DateExpire >= Class.Date.Date || s.DateExpire == null));
                if (us == null)
                {
                    result.Error = "המנוי שלך לא תקף לאימון שבחרת ";
                    return result;
                }
                var wl = context.ClassWaitLists.FirstOrDefault(
                    w => w.ClassId == classId && w.UserSubscription.UserId == userId && !w.IsDeleted);
                if (wl != null)
                {
                    result.Error = "הנך כבר ברשימת ההמתנה לאימון זה ";
                    return result;
                }
                ;
                wl = new ClassWaitList
                {
                    ClassId = classId,
                    DateEnrolled = DateTime.UtcNow.ToLocal(),
                    SubscriptionId = us.Id
                };
                //Class.WaitingList += 1;
                //context.Classes.Attach(Class);
                context.Entry(Class).State = EntityState.Unchanged;
                context.ClassWaitLists.Attach(wl);
                context.Entry(wl).State = EntityState.Added;
                context.SaveChanges();
                result.Result = true;
                result.RoomId = Class.StudioRoomId;
                result.Date = Class.Date;
            }

            return result;
        }

        public static ClassEnrollResult AdminMarkNoShow(int enrollmentId, bool value)
        {
            var result = new ClassEnrollResult { Result = false };
            using (var context = new InShapeEntities())
            {
                var ce = context.ClassEnrollments.FirstOrDefault(x => x.Id == enrollmentId);
                if (ce == null) return result;
                ce.IsVerified = value;
                context.ClassEnrollments.Attach(ce);
                context.Entry(ce).State = EntityState.Modified;
                context.SaveChanges();
                result.Result = true;
            }

            return result;
        }

        //public static List<ClassEnrollmentModel> GetNextEnrollmentsByUser(string userId)
        //{
        //    var date = DateTime.UtcNow.ToLocal();
        //    using (var context = new InShapeEntities())
        //    {
        //        return context.ClassEnrollments.Where(e => e.UserSubscription.UserId == userId && !e.IsDeleted && !e.Class.IsDeleted && e.Class.Date >= date)
        //            .OrderBy(x => x.Class.Date).ProjectTo<ClassEnrollmentModel>().ToList();
        //    }
        //}

        public static List<ClassEnrollmentModel> GetNextEnrollmentsByUser(string userId)
        {
            var date = DateTime.UtcNow.ToLocal();
            List<ClassEnrollmentModel> result = new List<ClassEnrollmentModel>();
            using (var context = new InShapeEntities())
            {
                var enrollments = context.ClassEnrollments.Include("Class").Include("UserSubscription").Include("ClassAvailablePlacement")
                    .Where(e => e.UserSubscription.UserId == userId && !e.IsDeleted && !e.Class.IsDeleted && e.Class.Date >= date)
                    .OrderBy(x => x.Class.Date);

                foreach (var item in enrollments)
                {
                    result.Add(new ClassEnrollmentModel
                    {
                        Id = item.Id,
                        ClassId = item.ClassId,
                        //ClassPlacementNumber = s.ClassAvailablePlacement.StudioPlacementId == 999 ? (Byte)(s.ClassAvailablePlacement.ClassPlacementNumber+11) : s.ClassAvailablePlacement.ClassPlacementNumber,
                        DateEnrolled = item.DateEnrolled,
                        //StudioPlacementName = s.ClassAvailablePlacement.StudioPlacementName,
                        SubscriptionId = item.SubscriptionId,
                        SelectedPlacement = Mapper.Map<AvailablePlacementsModel>(item.ClassAvailablePlacement),
                        Class = new StudioClassModel { Id = item.Class.Id, Name = item.Class.Name, Date = item.Class.Date, Time = item.Class.Date }
                    });
                }
            }
            return result;
        }

        //public static List<ClassEnrollmentModel> GetNextEnrollmentsByUser(string userId)
        //{
        //    var date = DateTime.UtcNow.ToLocal();
        //    using (var context = new InShapeEntities())
        //    {
        //        return context.ClassEnrollments.Include("Class").Include("UserSubscription").Include("ClassAvailablePlacement")
        //            .Where(e => e.UserSubscription.UserId == userId && !e.IsDeleted && !e.Class.IsDeleted && e.Class.Date >= date)
        //            .OrderBy(x => x.Class.Date).Select(s => new ClassEnrollmentModel
        //            {
        //                Id = s.Id,
        //                ClassId = s.ClassId,
        //                ClassPlacementNumber = s.StudioPlacement.Id == 999 ? (Byte)(s.ClassPlacementNumber + 11) : s.ClassPlacementNumber,
        //                DateEnrolled = s.DateEnrolled,
        //                StudioPlacementName = s.StudioPlacement.Name,
        //                SubscriptionId = s.SubscriptionId,
        //                Class = new StudioClassModel { Id = s.Class.Id, Name = s.Class.Name, Date = s.Class.Date, Time = s.Class.Date }
        //            }).ToList();
        //    }
        //}

        //public static List<ClassEnrollmentModel> GetNextEnrollmentsByUser(string userId)
        //{
        //    var date = DateTime.UtcNow.ToLocal();
        //    using (var context = new InShapeEntities())
        //    {
        //        var query = from ce in context.ClassEnrollments.Include("Class").Include("UserSubscription").Include("ClassAvailablePlacement")
        //                    where ce.UserSubscription.AspNetUser.Id == userId && !ce.IsDeleted && !ce.Class.IsDeleted && ce.Class.Date >= date
        //                    select ce;

        //        var result = query.OrderBy(x => x.Class.Date)//.ProjectTo<ClassEnrollmentModel>().OrderByDescending(x=>x.Class.Date)
        //                    .ToList();
        //        return Mapper.Map<List<ClassEnrollmentModel>>(result);
        //    }
        //}

        public static List<ClassEnrollmentModel> GetWeeklyEnrollment(string userId, DateTime classDate)
        {
            var date = classDate.Date.StartOfWeek();
            var endofweek = date.AddDays(7);
            using (var context = new InShapeEntities())
            {
                var query = from ce in context.ClassEnrollments.Include("Class").Include("UserSubscription")
                            where ce.UserSubscription.AspNetUser.Id == userId && !ce.IsDeleted && ce.Class.Date >= date && ce.Class.Date < endofweek
                            select ce;

                return Mapper.Map<List<ClassEnrollmentModel>>( query.OrderBy(x => x.Class.Date).ToList());
            }
        }

        public static List<ClassEnrollmentModel> GetOldEnrollmentsByUser(string userId)
        {
            //var date = DateTime.Now;
            using (var context = new InShapeEntities())
            {
                var query = from ce in context.ClassEnrollments.Include("Class").Include("UserSubscription")
                            where ce.UserSubscription.AspNetUser.Id == userId && !ce.IsDeleted && !ce.UserSubscription.Active
                            select ce;

                return Mapper.Map<List<ClassEnrollmentModel>>(query.OrderByDescending(x => x.Class.Date).ToList());
            }
        }

        public static ClassEnrollmentModel GetLastEnrollmentByUser(string userId)
        {
            var date = DateTime.UtcNow.ToLocal();
            var weekback = date.AddDays(-6);
            using (var context = new InShapeEntities())
            {
                var lastclass = context.ClassEnrollments.Include("Class")
                    .Include("UserSubscription").Where(x => x.UserSubscription.UserId == userId
                                                                     && x.UserSubscription.Active && x.IsVerified &&
                                                                     x.Class.Date < date && x.Class.Date > weekback && !x.Rating.HasValue &&
                                                                     !x.IsDeleted).OrderByDescending(c=>c.Class.Date).FirstOrDefault();
                return Mapper.Map<ClassEnrollmentModel>(lastclass);
            }
        }

        public static ClassEnrollResult RateClass(int EnrolmentId, int rating)
        {
            var result = new ClassEnrollResult {Result = false};
            using (var context = new InShapeEntities())
            {
                var ce = context.ClassEnrollments.FirstOrDefault(x => x.Id == EnrolmentId);
                if (ce == null) return result;
                ce.Rating = rating;
                context.ClassEnrollments.Attach(ce);
                context.Entry(ce).State = EntityState.Modified;
                context.SaveChanges();
                result.Result = true;

                context.CalcClassRating(ce.ClassId);
            }

            //recalc avg rating in class table

            return result;

        }

        public static ClassEnrollResult EnrollUser(int classId, int classAvailablePlacementId, string userId, bool isFromWaitList, int NumAdvncedEnrollments, int LateRegistration, string CancelationPenalty, bool isAdmin = false)
        {
            Logger.WriteDebug($"running EnrollUser, Class:{classId}, User: {userId}, isAdmin: {isAdmin}");
            var result = new ClassEnrollResult {Result = false};
            try
            {
                lock (thisLock)
                {
                    using (var context = new InShapeEntities())
                    {
                        // check if there is room in the class (not full)
                        var Class = context.Classes.FirstOrDefault(c => c.Id == classId && !c.IsDeleted); //&& (!c.IsFull.Value || isAdmin));
                        if (Class == null)
                        {
                            result.Error = "האימון שבחרת לא נמצא";
                            return result;
                        }

                        if (Class.IsFull.Value && !Class.ClassAvailablePlacements.Any(x => x.StudioPlacementId == 999 || x.StudioPlacementId == 1003 || x.StudioPlacementId == 1007)
                                              && !((isFromWaitList || isAdmin) && Class.MaxParticipants > Class.Participants))
                        {
                            result.Error = "האימון שבחרת כבר מלא";
                            return result;
                        }

                       

                        //check the user subscription is active at the time of class
                        var us =
                            context.UserSubscriptions.FirstOrDefault(s => s.UserId == userId && s.Active && s.DateExpire >= Class.Date.Date &&
                            (s.NumClasses == 0 || s.CurrentBalance > 0));
                        if (us == null)
                        {
                            result.Error = "המנוי שלך לא תקף לאימון שבחרת ";
                            Logger.WriteDebug(
                                $"EnrollUser, Class:{classId}, User: {userId}, isAdmin: {isAdmin}, msg: {result.Error}");
                            return result;
                        }

                        if (us.AspNetUser.Studio.Id != Class.StudioRoom.StudioId)
                        {
                            result.Error = "המנוי שלך אינו תכף לסטודיו שבחרת";
                            Logger.WriteDebug(
                                $"EnrollUser, Class:{classId}, User: {userId}, isAdmin: {isAdmin}, msg: {result.Error}");
                            return result;
                        }
                        
                        var error = "";
                        var CanEnroll = (us.NumClasses > 0 || us.DateExpire >= Class.Date.Date) && ClassRepo.CanEnroll(userId, Class.Date, NumAdvncedEnrollments, LateRegistration, isAdmin, out error);
                        if (!CanEnroll)
                        {
                            result.Error = error;
                            Logger.WriteDebug(
                                $"EnrollUser, Class:{classId}, User: {userId}, isAdmin: {isAdmin}, msg: {result.Error}");
                            return result;
                        }
                        string msg;
                        //check late cancelation penalty
                        if (!CanEnrollWithLateCancel(us.LateCacelation, us.IsLate, Class.Date, CancelationPenalty, isAdmin, out msg))
                        {
                            //TimeSpan ts = TimeSpan.FromMinutes(CancelationPenalty);
                            result.Error = msg; //$"בשל ביטול מאוחר לאימון הבא ניתן להירשם {ts.Hours}:{ts.Minutes} שעות לפני";
                            return result;
                        }
                        //if (us.LateCacelation && DateTime.UtcNow.ToLocal().AddMinutes(CancelationPenalty) < Class.Date && !isAdmin)
                        //{
                        //    TimeSpan ts = TimeSpan.FromMinutes(CancelationPenalty);
                        //    result.Error = $"בשל ביטול מאוחר לאימון הבא ניתן להירשם {ts.Hours}:{ts.Minutes} שעות לפני";
                        //    return result;
                        //}
                        //enroll user update subscription and update class space
                        var enroll = context.ClassEnrollments.FirstOrDefault(s => s.SubscriptionId == us.Id && s.ClassId == classId && !s.IsDeleted);
                        if (enroll == null)
                        {
                            if (classAvailablePlacementId > 0)
                            {
                                var enrollPlacement = context.ClassAvailablePlacements.FirstOrDefault(a => a.Id == classAvailablePlacementId && !a.IsDeleted && !a.IsInUse);
                                var enrollment = context.ClassEnrollments.FirstOrDefault(e => e.ClassAvailablePlacementId == classAvailablePlacementId && !e.IsDeleted);
                                if (enrollPlacement == null && enrollment != null)
                                {
                                    result.Error = "המקום שכבר בחרת תפוס, נסה מקום אחר";
                                    return result;
                                }
                                enroll = new ClassEnrollment
                                {
                                    ClassId = Class.Id,
                                    SubscriptionId = us.Id,
                                    DateEnrolled = DateTime.UtcNow.ToLocal(),
                                    IsVerified = true,
                                    ClassAvailablePlacementId = enrollPlacement.Id,
                                };
                                enrollPlacement.IsInUse = true;

                            }
                            else
                            {
                                enroll = new ClassEnrollment
                                {
                                    ClassId = Class.Id,
                                    SubscriptionId = us.Id,
                                    DateEnrolled = DateTime.UtcNow.ToLocal(),
                                    IsVerified = true
                                };
                            }
                            context.ClassEnrollments.Attach(enroll);
                            context.Entry(enroll).State = EntityState.Added;
                        }
                        else
                        {
                            result.Error = "הנך כבר רשום לאימון הנבחר";
                            return result;
                        }


                        if (us.NumClasses > 0) us.CurrentBalance -= 1;
                        //us.Active = us.CurrentBalance > 0; // should be changed in db on the day of last class.
                        if (!isAdmin) us.IsLate = false;
                        context.UserSubscriptions.Attach(us);
                        context.Entry(us).State = EntityState.Modified;

                        //remove from waiting list
                        var waitlist = context.ClassWaitLists.FirstOrDefault(s => s.SubscriptionId == us.Id && s.ClassId == classId && !s.IsDeleted);
                        if (waitlist != null)
                        {
                            waitlist.IsDeleted = true;
                            waitlist.DateCanceled = DateTime.UtcNow.ToLocal();
                            context.ClassWaitLists.Attach(waitlist);
                            context.Entry(waitlist).State = EntityState.Modified;

                            //reset waiting list if broadcast and class is full
                            if (waitlist.IsBroadcastSmsSent && Class.MaxParticipants - Class.Participants - 1 == 0)
                            {
                                var wlist = context.ClassWaitLists.Where(l => l.ClassId == Class.Id && l.IsBroadcastSmsSent && !l.IsDeleted);
                                foreach (var item in wlist)
                                {
                                    item.IsBroadcastSmsSent = false;
                                    item.DateBroadcastSmsSent = null;
                                    context.Entry(item).State = EntityState.Modified;
                                }
                            }
                        }
                        
                        var balanceTracking = new UserBalanceTracking
                        {
                            ChangeTypeId = 5,
                            Date = DateTime.UtcNow.ToLocal(),
                            Value = -1,
                            SubscriptionId = us.Id,
                            UserUpdated = userId,
                            Balance = us.NumClasses > 0 ? us.CurrentBalance : 0,
                            Note = "רישום לאימון דרך המערכת: " + Class.Name + "(" + Class.Id + ")"
                        };

                        context.UserBalanceTrackings.Attach(balanceTracking);
                        context.Entry(balanceTracking).State = EntityState.Added;

                        //Class.Participants += 1;

                        //var max = context.StudioRooms.First(r => r.Id == Class.StudioRoomId).MaxParticipants;
                        //Class.IsFull = Class.MaxParticipants <= Class.Participants;
                        //context.Classes.Attach(Class);
                        //context.Entry(Class).State = EntityState.Modified;
                        context.Entry(Class).State = EntityState.Unchanged;
                        context.SaveChanges();
                        //check if subscription is frozen (unfreeze)
                        if (us.Frozen)
                        {
                            UserRepo.UpdateSubscriptionUnFreeze(us.Id, us.UserId);
                            //us.Frozen = false;

                            //var f = context.FrozenSubscriptions.FirstOrDefault(x => x.SubscriptionId == us.Id && !x.IsDeleted);
                            //if (f != null)
                            //{
                            //    f.DateFinished = DateTime.UtcNow.ToLocal();
                            //    f.UserFinished = userId;
                            //    f.IsDeleted = true;
                            //    context.FrozenSubscriptions.Attach(f);
                            //    context.Entry(f).State = EntityState.Modified;
                            //    TimeSpan difference = f.DateFinished.Value - f.Date;
                            //    us.DateExpire = us.DateExpire.Value.AddDays(difference.Days);
                            //}

                        }
                        result.Result = true;
                        result.RoomId = Class.StudioRoomId;
                        result.Date = Class.Date;
                        result.Error = $"{us.AspNetUser.FullName}:{us.Id}";
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                Logger.WriteError($"EnrollUser Error, user:{userId}, class: {classId}, admin: {isAdmin}", ex);
                return result;
            }
        }

        private static bool CanEnrollWithLateCancel(int lateCacelation, bool isLate, DateTime ClassDate, string cancelationPenalty, bool isAdmin, out string message)
        {
            message = "";
            if (isAdmin || !isLate) return true;
            if (lateCacelation > 0)
            {
                var penalties = cancelationPenalty.Split(';').Select(Int32.Parse).ToList();
                int penalty;
                if (penalties.Count > lateCacelation)
                    penalty = penalties[lateCacelation - 1];
                else penalty = penalties.Last();
                if (DateTime.UtcNow.ToLocal().AddHours(penalty) < ClassDate)
                {
                    if (penalty > 0)
                        //TimeSpan ts = TimeSpan.FromMinutes(penalty);
                        message = $"בעקבות הביטול המאוחר בשיעור שעבר תוכל להירשם לאימון זה החל מ- {penalty} שעות לפני תחילת השיעור.";
                    else
                        message = $"בעקבות ביטולים מאוחרים לא ניתן להירשם לאימונים החודש.";
                    return false;
                }
            }
            return true;
        }

        private static bool CanEnroll(string userId, DateTime classDate,int NumAdvncedEnrollments, int LateRegistration, bool isAdmin, out string error)
        {
            error = "";
            if (isAdmin) return true;
            //check user can enroll to class this week (in case of unlimited subscription)
            var localnow = DateTime.UtcNow.ToLocal();
            var WeeklyClasses = GetWeeklyEnrollment(userId, classDate);
            //var classes = WeeklyClasses.Count;
            var MissinedClasses = WeeklyClasses.Count(x => !x.IsVerified);
            var sameDayClass = WeeklyClasses.Any(c => c.Class.Date.Date == classDate.Date);
            //var DoneClasses = WeeklyClasses.Count(x => x.Class.Date <= localnow && x.IsVerified);
            var BookedClasses = WeeklyClasses.Count(x => x.Class.Date > localnow);
            //var CanEnroll = (us.NumClasses > 0 || GetWeeklyEnrollment(userId).Count(x=> !x.IsVerified || x.Class.Date > localnow) < 2);

            var canEnroll = DateTime.UtcNow.ToLocal() < classDate && BookedClasses + MissinedClasses < NumAdvncedEnrollments
                || (DateTime.UtcNow.ToLocal() < classDate && BookedClasses + MissinedClasses == NumAdvncedEnrollments 
                && DateTime.UtcNow.ToLocal().AddHours(LateRegistration) >= classDate);
            //((BookedClasses + MissinedClasses + DoneClasses) < App.CurrentCompany.NumAdvncedEnrollments);
            //(MissinedClasses == 1 && (BookedClasses + DoneClasses) < 1);
            //if (canEnroll) return true;
            if (BookedClasses == NumAdvncedEnrollments)
                error = "הינך רשום כבר ל -2 שיעורים השבוע , עליך להיות נוכח באימון אחד בכדי להירשם לאימון נוסף השבוע";
            else if (MissinedClasses > 0)
                error = "לא הגעת לאימון שאליו נרשמת ולכן אינך יכול להירשם לאימון נוסף השבוע";
            else if (sameDayClass)
            {
                canEnroll = false;
                error = "הרשמה לשני אימונים באותו היום על בסיס מקום פנוי בלבד, לבירור מקום יש לשלוח הודעה למנהל";
            }
            //else error = "לא ניתן להירשם לאימון כרגע, נסה מאוחר יותר";
            return canEnroll;
        }

        //public static ClassEnrollResult EnrollUserFromWaitList(int classId, string placementkey, string userId, bool isAdmin = false)
        //{
        //    var result = new ClassEnrollResult { Result = false };
        //    try
        //    {
        //        using (var context = new InShapeEntities())
        //        {
        //            // check if there is room in the class (not full)
        //            var Class = context.Classes.FirstOrDefault(c => c.Id == classId && c.MaxParticipants > c.Participants);
        //            if (Class == null)
        //            {
        //                result.Error = "האימון שבחרת כבר מלא";
        //                Logger.WriteDebug($"EnrollUserFromWaitList, Class:{classId}, User: {userId}, isAdmin: {isAdmin}, msg: {result.Error}");
        //                return result;
        //            }
        //            //check the user subscription is active at the time of class
        //            var us = context.UserSubscriptions.FirstOrDefault(s => s.UserId == userId && s.Active && (s.DateExpire >= Class.Date.Date || s.DateExpire == null));
        //            if (us == null)
        //            {
        //                result.Error = "המנוי שלך לא תקף לאימון שבחרת ";
        //                Logger.WriteDebug($"EnrollUserFromWaitList, Class:{classId}, User: {userId}, isAdmin: {isAdmin}, msg: {result.Error}");
        //                return result;
        //            }

        //            //check if subscription is frozen (unfreeze)
                    

        //            var error = "";
        //            var CanEnroll = (us.NumClasses > 0 || us.DateExpire >= Class.Date.Date) && ClassRepo.CanEnroll(userId, Class.Date, out error); //us.NumClasses > 0 &&

        //            if (!CanEnroll && !isAdmin)
        //            {
        //                result.Error = error;
        //                Logger.WriteDebug($"EnrollUserFromWaitList, Class:{classId}, User: {userId}, isAdmin: {isAdmin}, msg: {result.Error}");
        //                return result;
        //            }
        //            //enroll user update subscription and update class space
        //            var enroll = context.ClassEnrollments.FirstOrDefault(s => s.SubscriptionId == us.Id && s.ClassId == classId);
        //            if (enroll == null)
        //            {
        //                if (!string.IsNullOrEmpty(placementkey))
        //                {
        //                    var placement = placementkey.Split('_');
        //                    enroll = new ClassEnrollment
        //                    {
        //                        ClassId = Class.Id,
        //                        SubscriptionId = us.Id,
        //                        DateEnrolled = DateTime.UtcNow.ToLocal(),
        //                        IsVerified = true,
        //                        StudioPlacementId = int.Parse(placement[0]),
        //                        ClassPlacementNumber = Byte.Parse(placement[1])
        //                    };
        //                }
        //                else
        //                {
        //                    enroll = new ClassEnrollment
        //                    {
        //                        ClassId = Class.Id,
        //                        SubscriptionId = us.Id,
        //                        DateEnrolled = DateTime.UtcNow.ToLocal(),
        //                        IsVerified = true
        //                    };
        //                }
        //                context.ClassEnrollments.Attach(enroll);
        //                context.Entry(enroll).State = EntityState.Added;
        //            }
        //            else if (enroll.IsDeleted)
        //            {
        //                enroll.DateEnrolled = DateTime.UtcNow.ToLocal();
        //                enroll.DateCanceled = null;
        //                enroll.IsDeleted = false;
        //                enroll.IsVerified = true;
        //                if (!string.IsNullOrEmpty(placementkey))
        //                {
        //                    var placement = placementkey.Split('_');
        //                    enroll.StudioPlacementId = int.Parse(placement[0]);
        //                    enroll.ClassPlacementNumber = Byte.Parse(placement[1]);
        //                }
        //                context.ClassEnrollments.Attach(enroll);
        //                context.Entry(enroll).State = EntityState.Modified;
        //            }
        //            else { return result; }

        //            //remove from waiting list
        //            var waitlist = context.ClassWaitLists.FirstOrDefault(s => s.SubscriptionId == us.Id && s.ClassId == classId && !s.IsDeleted);
        //            if (waitlist != null)
        //            {
        //                waitlist.IsDeleted = true;
        //                waitlist.DateCanceled = DateTime.UtcNow.ToLocal();
        //                context.ClassWaitLists.Attach(waitlist);
        //                context.Entry(waitlist).State = EntityState.Modified;
        //            }

        //            //reset waiting list if broadcast and class is full
        //            if (waitlist.IsBroadcastSmsSent && Class.MaxParticipants-Class.Participants-1 == 0)
        //            {
        //                var wlist = context.ClassWaitLists.Where(l => l.ClassId == Class.Id && l.IsBroadcastSmsSent && !l.IsDeleted);
        //                foreach (var item in wlist)
        //                {
        //                    item.IsBroadcastSmsSent = false;
        //                    item.DateBroadcastSmsSent = null;
        //                    context.Entry(item).State = EntityState.Modified;
        //                }
        //            }

        //            if (us.NumClasses > 0) us.CurrentBalance -= 1;
        //            //us.Active = us.CurrentBalance > 0; // should be changed in db on the day of last class.
        //            context.UserSubscriptions.Attach(us);
        //            context.Entry(us).State = EntityState.Modified;

        //            var balanceTracking = new UserBalanceTracking
        //            {
        //                ChangeTypeId = 5,
        //                Date = DateTime.UtcNow.ToLocal(),
        //                Value = -1,
        //                SubscriptionId = us.Id,
        //                UserUpdated = userId,
        //                Balance = us.NumClasses > 0 ? us.CurrentBalance : 0,
        //                Note = "רישום לאימון דרך המערכת: " + Class.Name + "(" + Class.Id + ")"
        //            };

        //            context.UserBalanceTrackings.Attach(balanceTracking);
        //            context.Entry(balanceTracking).State = EntityState.Added;

        //            //Class.Participants += 1;
        //            //Class.WaitingList = Class.WaitingList - 1;
        //            //var max = context.StudioRooms.First(r => r.Id == Class.StudioRoomId).MaxParticipants;
        //            //Class.IsFull = Class.MaxParticipants <= Class.Participants;
        //            //context.Classes.Attach(Class);
        //            context.Entry(Class).State = EntityState.Unchanged;

        //            context.SaveChanges();
        //            if (us.Frozen)
        //            {
        //                UserRepo.UpdateSubscriptionUnFreeze(us.Id, us.UserId);
        //            }
        //            result.Result = true;
        //            result.RoomId = Class.StudioRoomId;
        //            result.Date = Class.Date;
        //            Logger.WriteDebug($"EnrollUserFromWaitList, Class:{classId}, User: {userId},  WaitingList for {Class.Date}: {Class.WaitingList}");
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //        Logger.WriteError($"EnrollUserFromWaitingList Error, user:{userId}, class: {classId}, admin: {isAdmin}", ex);
        //        return result;
        //    }
        //}

        
        public static ClassEnrollResult CancelEnrollment(int classId, string userId, int CancellationThresholdMins, int LateCancellationThresholdMins, bool isLate)
        {
            Logger.WriteDebug($"CancelEnrollment, Class:{classId}, User: {userId}");
            var result = new ClassEnrollResult { Result = false };
            try
            {
                using (var context = new InShapeEntities())
                {
                    // check if class is not starting (2 hours)
                    DateTime Threshold = DateTime.UtcNow.ToLocal().AddMinutes(CancellationThresholdMins);
                    DateTime ThresholdLate = DateTime.UtcNow.ToLocal().AddMinutes(LateCancellationThresholdMins);
                    var Class = context.Classes.FirstOrDefault(c => c.Id == classId && !c.IsDeleted);
                    if (Class == null) return result;
                    if (!isLate)
                    {
                        if (Class.Date < Threshold)
                        {
                            result.Error = $"ניתן לבטל אימון עד {CancellationThresholdMins} דקות לפני תחילת האימון";
                            return result;
                        }
                    }
                    else
                    {
                        if (Class.Date < ThresholdLate)
                        {
                            result.Error = $"ניתן לבטל מאוחר אימון עד {LateCancellationThresholdMins} דקות לפני תחילת האימון";
                            return result;
                        }
                    }
                    result.Error = string.Empty;
                    //check the user subscription 
                    var us = context.UserSubscriptions.FirstOrDefault(s => s.UserId == userId && s.ClassEnrollments.Any(e => e.ClassId == classId && !e.IsDeleted)); //&& s.Active);
                    if (us == null) return result;
                    //outroll user update subscription and update class space
                    var outroll = context.ClassEnrollments.FirstOrDefault(s => s.SubscriptionId == us.Id && s.ClassId == classId && !s.IsDeleted);
                    if (outroll == null) return result;
                    outroll.IsDeleted = true;
                    outroll.DateCanceled = DateTime.UtcNow.ToLocal();
                    outroll.IsLateCancel = isLate;
                    if (outroll.ClassAvailablePlacementId > 0) outroll.ClassAvailablePlacement.IsInUse = false;
                    context.ClassEnrollments.Attach(outroll);
                    context.Entry(outroll).State = EntityState.Modified;

                    if (us.NumClasses > 0) us.CurrentBalance += 1;
                    us.LateCacelation = us.LateCacelation + (isLate? 1: 0);
                    us.IsLate = isLate;
                    //us.Active = us.CurrentBalance > 0;
                    context.UserSubscriptions.Attach(us);
                    context.Entry(us).State = EntityState.Modified;


                    var balanceTracking = new UserBalanceTracking
                    {
                        ChangeTypeId = 6,
                        Date = DateTime.UtcNow.ToLocal(),
                        Value = 1,
                        SubscriptionId = us.Id,
                        UserUpdated = userId,
                        Balance = us.NumClasses > 0 ? us.CurrentBalance : 0,
                        Note = "ביטול רישום לאימון דרך המערכת: " + Class.Name + "(" + Class.Id + ")"
                    };

                    context.UserBalanceTrackings.Attach(balanceTracking);
                    context.Entry(balanceTracking).State = EntityState.Added;

                    //Class.Participants -= 1;
                    //var max = context.StudioRooms.First(r => r.Id == Class.StudioRoomId).MaxParticipants;
                    //Class.IsFull = Class.WaitingList > 0 && Class.MaxParticipants > Class.Participants+Class.WaitingList; 
                    //context.Classes.Attach(Class);
                    //context.Entry(Class).State = EntityState.Modified;
                    context.Entry(Class).State = EntityState.Unchanged;
                    context.SaveChanges();

                    //if (Class.WaitingList > 0 && DateTime.UtcNow.ToLocal() < Class.Date.AddHours(2))  //todo: move to configuration
                        //result.StartWaitingListProcess = true;
                    result.Result = true;
                    result.RoomId = Class.StudioRoomId;
                    result.Date = Class.Date;
                    return result;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                Logger.WriteError($"CancelEnrollment Error, user:{userId}, class: {classId}", ex);
                return result;
            }
        }


        //public static bool CancelEnrollment(int enrollmentId)
        //{
        //    try
        //    {
        //        using (var context = new InShapeEntities())
        //        {
        //            var enrollment = context.ClassEnrollments.FirstOrDefault(e => e.Id == enrollmentId && !e.IsDeleted);
        //            if (enrollment == null) return false;
        //            // check if class is not starting (2 hours)
        //            if (enrollment.Class.Date <= DateTime.Now.AddHours(2)) return false;
        //            //outroll user update subscription and update class space

        //            enrollment.IsDeleted = true;
        //            enrollment.DateCanceled = DateTime.Now;
        //            context.ClassEnrollments.Attach(enrollment);
        //            context.Entry(enrollment).State = EntityState.Modified;

        //            enrollment.UserSubscription.CurrentBalance += 1;
        //            //enrollment.UserSubscription.Active = enrollment.UserSubscription.CurrentBalance > 0;
        //            context.UserSubscriptions.Attach(enrollment.UserSubscription);
        //            context.Entry(enrollment.UserSubscription).State = EntityState.Modified;

        //            var balanceTracking = new UserBalanceTracking
        //            {
        //                ChangeTypeId = 6,
        //                Date = DateTime.Now,
        //                Value = 1,
        //                SubscriptionId = enrollment.SubscriptionId,
        //                UserUpdated = enrollment.UserSubscription.UserId,
        //                Balance = enrollment.UserSubscription.CurrentBalance,
        //                Note = "ביטול רישום לאימון דרך המערכת " + enrollment.Class.Name + "(" + enrollment.Class.Id + ")"
        //            };

        //            context.UserBalanceTrackings.Attach(balanceTracking);
        //            context.Entry(balanceTracking).State = EntityState.Added;


        //            enrollment.Class.Participants -= 1;
        //            //var max = context.StudioRooms.First(r => r.Id == enrollment.Class.StudioRoomId).MaxParticipants;
        //            enrollment.Class.IsFull = enrollment.Class.MaxParticipants <= enrollment.Class.Participants;
        //            context.Classes.Attach(enrollment.Class);
        //            context.Entry(enrollment.Class).State = EntityState.Modified;

        //            context.SaveChanges();
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //        return false;
        //    }
        //}


        public static ClassEnrollResult AdminCancelEnrollment(int classId, int subscriptionId, string userId)
        {
            Logger.WriteDebug($"AdminCancelEnrollment, Class:{classId}, Subscription: {subscriptionId}, User: {userId}");
            var result = new ClassEnrollResult { Result = false };
            try
            {
                using (var context = new InShapeEntities())
                {
                    var Class = context.Classes.FirstOrDefault(c => c.Id == classId);
                    if (Class == null) return result;
                    //check the user subscription 
                    var us = context.UserSubscriptions.FirstOrDefault(s => s.Id == subscriptionId);
                    if (us == null) return result;
                    //outroll user update subscription and update class space
                    var outroll = context.ClassEnrollments.FirstOrDefault(s => s.SubscriptionId == us.Id && s.ClassId == classId); //&& !s.IsDeleted);
                    if (outroll == null) return result;
                    outroll.IsDeleted = true;
                    outroll.DateCanceled = DateTime.UtcNow.ToLocal();
                    if (outroll.ClassAvailablePlacementId > 0) outroll.ClassAvailablePlacement.IsInUse = false;
                    context.ClassEnrollments.Attach(outroll);
                    context.Entry(outroll).State = EntityState.Modified;

                    if (us.NumClasses > 0) us.CurrentBalance += 1;
                    //us.Active = us.CurrentBalance > 0;
                    context.UserSubscriptions.Attach(us);
                    context.Entry(us).State = EntityState.Modified;


                    var balanceTracking = new UserBalanceTracking
                    {
                        ChangeTypeId = 6,
                        Date = DateTime.UtcNow.ToLocal(),
                        Value = 1,
                        SubscriptionId = us.Id,
                        UserUpdated = userId,
                        Balance = us.NumClasses > 0 ? us.CurrentBalance : 0,
                        Note = "ביטול רישום לאימון דרך מנהל: " + Class.Name + "(" + Class.Id + ")"
                    };

                    context.UserBalanceTrackings.Attach(balanceTracking);
                    context.Entry(balanceTracking).State = EntityState.Added;

                    //Class.Participants -= 1;
                    //var max = context.StudioRooms.First(r => r.Id == Class.StudioRoomId).MaxParticipants;
                    //Class.IsFull = Class.WaitingList > 0 && Class.MaxParticipants > Class.Participants+Class.WaitingList;  //Class.MaxParticipants <= Class.Participants 
                    //context.Classes.Attach(Class);
                    //context.Entry(Class).State = EntityState.Modified;
                    context.Entry(Class).State = EntityState.Unchanged;
                    context.SaveChanges();
                    result.Result = true;
                    //if (Class.WaitingList > 0 && DateTime.UtcNow.ToLocal() < Class.Date.AddHours(2))  //todo: move to configuration
                        //result.StartWaitingListProcess = true;
                    result.RoomId = Class.StudioRoomId;
                    result.Date = Class.Date;
                    return result;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                Logger.WriteError($"CancelEnrollmentAdmin Error, user:{userId}, class: {classId} admin", ex);
                return result;
            }
        }

        public static MessageEnrollmentsModel GetEnrollmentsToSendAlert(int companyid)
        {
            var localDateTime = DateTime.UtcNow.ToLocal(); //DateTime.UtcNow.ToLocal();
            Logger.WriteDebug($"running GetEnrollmentsToSendAlert (localtime): {localDateTime.ToString("g")}");
            var result = new MessageEnrollmentsModel();
            try
            {
                using (var context = new InShapeEntities())
                {
                    var interval = context.MSGTypes.FirstOrDefault(x => x.MessageTypeId == (int) MessageType.BeforeStart && x.CompanyId == companyid);
                    if (interval == null || !interval.Active) return result;
                    var start = localDateTime; //.Date.AddHours(localDateTime.Hour);
                    var end = start.AddMinutes(interval.TimeBefore.Value);
                    result.Message = interval.Message;
                    result.EnrollmentModels = Mapper.Map<List<ClassEnrollmentModel>>(
                        context.ClassEnrollments.Include("UserSubscription").Include("ClassAvailablePlacement")
                            .Include("UserSubscription.AspNetUser")
                            .Include("Class")
                            .Where(r =>
                                    r.Class.Date >= start && r.Class.Date <= end && !r.IsDeleted && !r.IsSmsSent &&
                                    r.UserSubscription.AspNetUser.ReceiveSMS
                                    && r.UserSubscription.AspNetUser.Studio.CompanyId == companyid)
                            //.FilterByCompany()
                            //.ProjectTo<ClassEnrollmentModel>()
                            .ToList());
                    Logger.WriteDebug($"running GetEnrollmentsToSendAlert (count): {result.EnrollmentModels.Count}");
                    //add early morning classes if local time is >= 22:00 should be @21 send to all upto 9:30 
                    if (localDateTime.Hour < 21) return result;
                    {
                        var start2 = localDateTime.Date.AddDays(1); // 00:00
                        var end2 = start2.AddHours(9).AddMinutes(31); // 09:30
                        result.EnrollmentModels.AddRange(Mapper.Map<List<ClassEnrollmentModel>>(
                            context.ClassEnrollments.Include("UserSubscription").Include("ClassAvailablePlacement")
                                .Include("UserSubscription.AspNetUser")
                                .Include("Class")
                                .Where(r =>
                                        r.Class.Date >= start2 && r.Class.Date < end2 && !r.IsDeleted && !r.IsSmsSent &&
                                        r.UserSubscription.AspNetUser.ReceiveSMS
                                        && r.UserSubscription.AspNetUser.Studio.CompanyId == companyid)
                                //.FilterByCompany()
                                //.ProjectTo<ClassEnrollmentModel>()
                                .ToList()));
                        Logger.WriteDebug($"running GetEnrollmentsToSendAlert (next mornning): {end2.ToString("g")}, (count): {result.EnrollmentModels.Count}");
                    }
                    
                    return result;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                Logger.WriteError("GetEnrollmentsToSendAlert Error", ex);
                return result;
            }
        }

        //public static MessageEnrollmentsModel GetWaitingListToSendAlert(int classid)
        //{
        //    var localDateTime = DateTime.UtcNow.ToLocal();
        //    Logger.WriteDebug($"running GetWaitingListToSendAlert class: {classid}");
        //    var result = new MessageEnrollmentsModel();
        //    try
        //    {
        //        using (var context = new InShapeEntities())
        //        {
        //            var interval = context.MSGTypes.FirstOrDefault(x => x.Id == (int)MessageType.WaitList);
        //            if (interval == null || !interval.Active) return result;
        //            var start = localDateTime; //.Date.AddHours(localDateTime.Hour);
        //            var end = start.AddMinutes(interval.TimeBefore.Value);
        //            result.Message = interval.Message;
        //            var cls = context.Classes.FirstOrDefault(c => c.Id == classid && c.Date >= start && c.WaitingList > 0 && !c.IsDeleted
        //                                                          && (c.MaxParticipants - c.Participants) > 0);
        //            if (cls == null) return result;
                    
        //                result.EnrollmentModels = Mapper.Map<List<ClassEnrollmentModel>>(
        //                    context.ClassWaitLists.Include("UserSubscription")
        //                        .Include("UserSubscription.AspNetUser")
        //                        .Include("Class")
        //                        .Where(
        //                            w => !w.IsDeleted &&
        //                            w.Class.Id == cls.Id &&
        //                                 (w.DateSmsSent == null ||
        //                                  w.DateSmsSent < DbFunctions.AddMinutes(start, -interval.TimeBefore.Value)))
        //                        .OrderBy(o => o.DateEnrolled)
        //                        .ToList().Take(cls.MaxParticipants - cls.Participants).Where(x => !x.IsSmsSent)
        //                        .Where(x => !x.IsSmsSent));
        //            Logger.WriteDebug($"running GetWaitingListToSendAlert (count): {result.EnrollmentModels.Count}");
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //        Logger.WriteError($"GetWaitingListToSendAlert Error, Class: {classid}", ex);
        //        return result;
        //    }
        //}

        //public static MessageEnrollmentsModel GetWaitingListToSendAlert()
        //{
        //    var localDateTime = DateTime.UtcNow.ToLocal();
        //    var result = new MessageEnrollmentsModel();
        //    try
        //    {
        //        using (var context = new InShapeEntities())
        //        {
        //            var interval = context.MSGTypes.FirstOrDefault(x => x.Id == (int)MessageType.WaitList);
        //            if (interval == null || !interval.Active) return result;
        //            var start = localDateTime; //.Date.AddHours(localDateTime.Hour);
        //            //var end = start.AddMinutes(interval.TimeBefore.Value);
        //            result.Message = interval.Message;
        //                result.EnrollmentModels =
        //                    context.ClassWaitLists.Include("UserSubscription")
        //                        .Include("UserSubscription.AspNetUser")
        //                        .Include("Class")
        //                        .Where(
        //                            r =>
        //                                r.Class.Date >= start && !r.IsDeleted &&
        //                                r.UserSubscription.AspNetUser.ReceiveSMS)
        //                        .ProjectTo<ClassEnrollmentModel>()
        //                        .ToList();
                    


        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //        return result;
        //    }
        //}

        public static MessageEnrollmentsModel  GetWaitingListToSendAlert(bool Broadcast, int companyId, int PriorityWaitListDays)
        {
            var now = DateTime.UtcNow.ToLocal();
            Logger.WriteDebug($"running GetWaitingListToSendAlert localtime: {now.ToString("g")}, Broadcast: {Broadcast}");
            var result = new MessageEnrollmentsModel();
            try
            {
                using (var context = new InShapeEntities())
                {
                    //var smsinterval = DateTime.UtcNow.ToLocal().AddMinutes(-10);
                    //var end = start.AddMinutes(interval.TimeBefore.Value);

                    if (Broadcast)
                    {
                        var interval = context.MSGTypes.FirstOrDefault(x => x.MessageTypeId == (int)MessageType.BroadcastWaitList && x.CompanyId == companyId);
                        if (interval == null || !interval.Active) return result;
                        result.Message = interval.Message;
                        result.EnrollmentModels =
                            Mapper.Map<List<ClassEnrollmentModel>>(context.ClassWaitLists.Include("UserSubscription")
                                .Include("UserSubscription.AspNetUser")
                                .Include("Class")
                                .Where(w => !w.IsDeleted && w.Class.Date >= now && !w.Class.IsDeleted &&
                                            w.Class.WaitingList > 0 &&
                                            !w.IsBroadcastSmsSent &&
                                            ((w.Class.MaxParticipants - w.Class.Participants) > 0) &&
                                            //w.Class.Date <= DbFunctions.AddMinutes(now, interval.TimeBefore.Value))
                                            //w.Class.Date <= DbFunctions.AddMinutes(now, App.CurrentCompany.CancellationThresholdMins > interval.TimeBefore.Value ? interval.TimeBefore.Value : App.CurrentCompany.CancellationThresholdMins))
                                            //w.Class.Date <= DbFunctions.AddMinutes(now, App.CurrentCompany.CancellationThresholdMins > interval.TimeBefore.Value ? App.CurrentCompany.CancellationThresholdMins : interval.TimeBefore.Value))
                                            w.Class.Date <= DbFunctions.AddMinutes(now, interval.TimeBefore.Value)
                                            && w.UserSubscription.AspNetUser.Studio.CompanyId == companyId)
                                //.ProjectTo<ClassEnrollmentModel>()
                                .ToList());
                        //DATEADD(MINUTE, @BroadcastThresholdMins, getdate())
                        Logger.WriteDebug(
                            $"running GetWaitingListToSendAlert (No Class: Broadcast) (count): {result.EnrollmentModels.Count}");
                    }
                    else
                    {
                        var fromdate = DateTime.UtcNow.ToLocal().Date.AddDays(-PriorityWaitListDays);
                        var interval = context.MSGTypes.FirstOrDefault(x => x.MessageTypeId == (int)MessageType.WaitList && x.CompanyId == companyId);
                        var broadcastinterval = context.MSGTypes.FirstOrDefault(x => x.MessageTypeId == (int)MessageType.BroadcastWaitList && x.CompanyId == companyId);
                        if (interval == null || !interval.Active) return result;
                        var classes = context.Classes.Where(c => c.Date >= now && c.MaxParticipants - c.Participants > 0 && c.WaitingList > 0 && !c.IsDeleted
                            && c.Date > DbFunctions.AddMinutes(now, broadcastinterval.TimeBefore.Value)); //not send if broadcast thershold reached - we send to all above
                        result.Message = interval.Message;
                        result.EnrollmentModels = new List<ClassEnrollmentModel>();
                        //DateTime fromdate = DateTime.UtcNow.ToLocal().AddDays(-0);
                        foreach (var cls in classes)
                        {
                            result.EnrollmentModels.AddRange(
                                Mapper.Map<List<ClassEnrollmentModel>>(context.ClassWaitLists.Include("UserSubscription")
                                .Include("UserSubscription.AspNetUser")
                                .Include("Class")
                                .Where(
                                    w => !w.IsDeleted && w.Class.Id == cls.Id &&
                                         (w.DateSmsSent == null ||
                                          w.DateSmsSent > DbFunctions.AddMinutes(now, -interval.TimeBefore.Value-1))
                                          && w.UserSubscription.AspNetUser.Studio.CompanyId == companyId)
                                //.GroupBy(w => w.ClassId)
                                //.Select(g => g.OrderBy(p => p.DateEnrolled).FirstOrDefault())
                                //.ProjectTo<ClassEnrollmentModel>()
                                //.Where(x => !x.IsSmsSent) //todo: tocheck
                                //.OrderBy(o=>o.DateSmsSent)
                                //.OrderBy(o => DbFunctions.DiffDays(o.UserSubscription.AspNetUser.LastClass?? DateTime.MinValue, fromdate)??0)
                                //.OrderBy(o => DbFunctions.DiffDays((o.UserSubscription.AspNetUser.LastClass == null ? fromdate : 
                                //                                    o.UserSubscription.AspNetUser.LastClass < fromdate ? fromdate : 
                                //                                    o.UserSubscription.AspNetUser.LastClass), fromdate))
                                //.OrderBy(o => o.DateEnrolled)
                                //.OrderBy(o => o.DateEnrolled)
                                .ToList())
                                .OrderByDescending(o=> Math.Min(PriorityWaitListDays, o.DaysSinceLastClass)).ThenBy(o => o.DateEnrolled)
                                //.OrderBy(o => o.DateEnrolled)
                                .Take(cls.MaxParticipants - cls.Participants)
                                .Where(x => !x.IsSmsSent));

                            Logger.WriteDebug($"running GetWaitingListToSendAlert (Class Id: {cls.Id}) (count): {result.EnrollmentModels.Count}");
                        }


                    }
                    //var classes = context.Classes.Where(c => c.IsFull && c.WaitingList > 0 && c.Date >= start
                    //                                         && c.ClassWaitLists.Any(
                    //                                             w =>
                    //                                                 !w.IsDeleted && !w.IsSmsSent &&
                    //                                                 (w.DateSmsSent == null ||
                    //                                                  w.DateSmsSent < smsinterval))).Select(i=>i.Id);
                    //result.EnrollmentModels =
                    //    context.ClassWaitLists.Include("UserSubscription")
                    //        .Include("UserSubscription.AspNetUser")
                    //        .Include("Class")

                    //        .Where(r => classes.Contains(r.Id) &&
                    //               !r.IsDeleted)
                    //        .OrderBy(r=>r.DateEnrolled)
                    //        .GroupBy(g => g.ClassId)
                    //        .SelectMany(w=> w.Take(1))
                    //        //.FirstOrDefault()
                    //        .ProjectTo<ClassEnrollmentModel>()
                    //        .ToList();


                    
                    return result;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                Logger.WriteError($"GetWaitingListToSendAlert Error, Broadcast: {Broadcast}", ex);
                return result;
            }
        }

        //public static MessageEnrollmentsModel GetWaitingListWithProirityToSendAlert(bool Broadcast)
        //{
        //    var now = DateTime.UtcNow.ToLocal();
        //    Logger.WriteDebug($"running GetWaitingListWithProirityToSendAlert localtime: {now.ToString("g")}, Broadcast: {Broadcast}");
        //    var result = new MessageEnrollmentsModel();
        //    try
        //    {
        //        using (var context = new InShapeEntities())
        //        {
        //            //var smsinterval = DateTime.UtcNow.ToLocal().AddMinutes(-10);
        //            //var end = start.AddMinutes(interval.TimeBefore.Value);

        //            if (Broadcast)
        //            {
        //                var interval = context.MSGTypes.FirstOrDefault(x => x.MessageTypeId == (int)MessageType.BroadcastWaitList);
        //                if (interval == null || !interval.Active) return result;
        //                result.Message = interval.Message;
        //                result.EnrollmentModels =
        //                    Mapper.Map<List<ClassEnrollmentModel>>(context.ClassWaitLists.Include("UserSubscription")
        //                        .Include("UserSubscription.AspNetUser")
        //                        .Include("Class")
        //                        .Where(w => !w.IsDeleted && w.Class.Date >= now && !w.Class.IsDeleted &&
        //                                    w.Class.WaitingList > 0 &&
        //                                    ((w.Class.MaxParticipants - w.Class.Participants) > 0) &&
        //                                    w.Class.Date <= DbFunctions.AddMinutes(now, interval.TimeBefore.Value))
        //                        //.ProjectTo<ClassEnrollmentModel>()
        //                        .ToList());
        //                //DATEADD(MINUTE, @BroadcastThresholdMins, getdate())
        //                Logger.WriteDebug(
        //                    $"running GetWaitingListWithProirityToSendAlert (No Class: Broadcast) (count): {result.EnrollmentModels.Count}");
        //            }
        //            else
        //            {
        //                var interval = context.MSGTypes.FirstOrDefault(x => x.MessageTypeId == (int)MessageType.WaitList);
        //                var broadcastinterval = context.MSGTypes.FirstOrDefault(x => x.MessageTypeId == (int)MessageType.BroadcastWaitList);
        //                if (interval == null || !interval.Active) return result;
        //                var classes = context.Classes.Where(c => c.Date >= now && c.MaxParticipants - c.Participants > 0 && c.WaitingList > 0 && !c.IsDeleted
        //                    && c.Date > DbFunctions.AddMinutes(now, broadcastinterval.TimeBefore.Value)); //not send if broadcast thershold reached - we send to all above
        //                result.Message = interval.Message;
        //                result.EnrollmentModels = new List<ClassEnrollmentModel>();
        //                //DateTime fromdate = DateTime.UtcNow.ToLocal().AddDays(-0);
        //                foreach (var cls in classes)
        //                {
        //                    result.EnrollmentModels.AddRange(
        //                        Mapper.Map<List<ClassEnrollmentModel>>(context.ClassWaitLists.Include("UserSubscription")
        //                        .Include("UserSubscription.AspNetUser")
        //                        .Include("Class")
        //                        .Where(
        //                            w => !w.IsDeleted && w.Class.Id == cls.Id &&
        //                                 (w.DateSmsSent == null ||
        //                                  w.DateSmsSent > DbFunctions.AddMinutes(now, -interval.TimeBefore.Value - 1)))
        //                        //.GroupBy(w => w.ClassId)
        //                        //.Select(g => g.OrderBy(p => p.DateEnrolled).FirstOrDefault())
        //                        //.ProjectTo<ClassEnrollmentModel>()
        //                        //.Where(x => !x.IsSmsSent) //todo: tocheck
        //                        //.OrderBy(o=>o.DateSmsSent)
        //                        //.OrderBy(o => DbFunctions.DiffDays(o.UserSubscription.AspNetUser.LastClass, fromdate))
        //                        .OrderBy(o => o.DateEnrolled)
        //                        .ToList().Take(cls.MaxParticipants - cls.Participants))
        //                        .Where(x => !x.IsSmsSent));

        //                    Logger.WriteDebug($"running GetWaitingListWithProirityToSendAlert (Class Id: {cls.Id}) (count): {result.EnrollmentModels.Count}");
        //                }


        //            }
        //            //var classes = context.Classes.Where(c => c.IsFull && c.WaitingList > 0 && c.Date >= start
        //            //                                         && c.ClassWaitLists.Any(
        //            //                                             w =>
        //            //                                                 !w.IsDeleted && !w.IsSmsSent &&
        //            //                                                 (w.DateSmsSent == null ||
        //            //                                                  w.DateSmsSent < smsinterval))).Select(i=>i.Id);
        //            //result.EnrollmentModels =
        //            //    context.ClassWaitLists.Include("UserSubscription")
        //            //        .Include("UserSubscription.AspNetUser")
        //            //        .Include("Class")

        //            //        .Where(r => classes.Contains(r.Id) &&
        //            //               !r.IsDeleted)
        //            //        .OrderBy(r=>r.DateEnrolled)
        //            //        .GroupBy(g => g.ClassId)
        //            //        .SelectMany(w=> w.Take(1))
        //            //        //.FirstOrDefault()
        //            //        .ProjectTo<ClassEnrollmentModel>()
        //            //        .ToList();



        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //        Logger.WriteError($"GetWaitingListToSendAlert Error, Broadcast: {Broadcast}", ex);
        //        return result;
        //    }
        //}


        public static bool UpdateSmsSent(int enrolmentId)
        {
            using (var context = new InShapeEntities())
            {
                var ue = context.ClassEnrollments.FirstOrDefault(x => x.Id == enrolmentId);
                if (ue == null) return false;
                ue.IsSmsSent = true;
                context.ClassEnrollments.Attach(ue);
                context.Entry(ue).State = EntityState.Modified;
                context.SaveChanges();
                return true;
            }
        }

        public static bool UpdateWaitingListSmsSent(int enrolmentId, bool isBroadcast = false)
        {
            using (var context = new InShapeEntities())
            {
                var ue = context.ClassWaitLists.FirstOrDefault(x => x.Id == enrolmentId);
                if (ue == null) return false;
                if (isBroadcast)
                {
                    ue.IsBroadcastSmsSent = true;
                    ue.DateBroadcastSmsSent = DateTime.UtcNow.ToLocal();
                }
                else
                {
                    ue.IsSmsSent = true;
                    ue.DateSmsSent = DateTime.UtcNow.ToLocal();
                }
                
                context.ClassWaitLists.Attach(ue);
                context.Entry(ue).State = EntityState.Modified;
                context.SaveChanges();
                return true;
            }
        }

        public static void RemoveFromWatingListBeforeSMS(int companyId)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var interval = context.MSGTypes.Where(x => x.MessageTypeId == (int)MessageType.WaitList || x.MessageTypeId == (int)MessageType.BroadcastWaitList && x.CompanyId == companyId);
                    if (!interval.Any()) return;
                    var now = DateTime.UtcNow.ToLocal();
                    var WaitListInterval = interval.FirstOrDefault(x => x.MessageTypeId == (int)MessageType.WaitList);
                    var wl = context.ClassWaitLists.Where(
                        x => !x.IsDeleted && ((x.IsSmsSent  && x.DateSmsSent <= DbFunctions.AddMinutes(now, -WaitListInterval.TimeBefore.Value))
                        || (x.IsBroadcastSmsSent && x.DateBroadcastSmsSent <= DbFunctions.AddMinutes(now, -WaitListInterval.TimeBefore.Value))) 
                            //&& x.Class.MaxParticipants > x.Class.Participants))
                            && x.UserSubscription.AspNetUser.Studio.CompanyId == companyId).ToList();

                    //var BroadcastWaitListInterval = interval.FirstOrDefault(x => x.Id == (int)MessageType.BroadcastWaitList);
                    //if(BroadcastWaitListInterval != null)
                    //wl.AddRange(context.ClassWaitLists.Where(
                    //    x => !x.IsDeleted && x.Class.MaxParticipants > x.Class.Participants
                    //    && x.Class.Date <= DbFunctions.AddMinutes(now, BroadcastWaitListInterval.TimeBefore.Value - WaitListInterval.TimeBefore.Value)));
                    //    //&& x.Class.Date <= DbFunctions.AddMinutes(now, BroadcastWaitListInterval.TimeBefore.Value)));

                    foreach (var classWaitList in wl)
                    {
                        classWaitList.IsDeleted = true;
                        classWaitList.DateCanceled = now;
                        context.ClassWaitLists.Attach(classWaitList);
                        context.Entry(classWaitList).State = EntityState.Modified;
                        Logger.WriteDebug($"RemoveFromWatingListBeforeSMS: Class: {classWaitList.Class.Name}: {classWaitList.Class.Id} - waiting List: {classWaitList.Class.WaitingList}, UserSubscription: {classWaitList.UserSubscription.Id}");
                    }

                    //var temp = wl.GroupBy(x => x.ClassId).Select(g => new {g.Key, Count = g.Count()});

                    //foreach (var disclass in wl.Select(x=>x.ClassId).Distinct())
                    //foreach (var disclass in wl.GroupBy(x=>x.ClassId).Select(g => new { g.Key, Count = g.Count() }))
                    //{
                    //    var cls = context.Classes.FirstOrDefault(x => x.Id == disclass.Key);
                    //    if (cls == null) continue;
                    //    cls.WaitingList -= disclass.Count;
                    //    cls.IsFull = cls.WaitingList > 0 || (cls.MaxParticipants - cls.Participants) == 0;
                    //    context.Classes.Attach(cls);
                    //    context.Entry(cls).State = EntityState.Modified;
                    //    Logger.WriteDebug($"Class: {cls.Name}: {cls.Id} - waiting List: {cls.WaitingList}");
                    //}

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError("RemoveFromWatingListAfterSMS Error", ex);
            }

        }

    }
}
