using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using InShapeModels;
using Utilities;


namespace DAL
{
    public class StudioRepo
    {
        private static Object thisLock = new Object();
        public static List<StudioRoomModel> GetStudioRooms(int roomId, int studioId = 0)
        {
            using (var context = new InShapeEntities())
            {
                return roomId > 0 ?
                    context.StudioRooms.Where(r => r.Id == roomId && !r.IsDeleted)
                        .ProjectTo<StudioRoomModel>()
                        .ToList() :
                        context.StudioRooms.Where(r => !r.IsDeleted && r.StudioId == studioId).ProjectTo<StudioRoomModel>().ToList();
            }
        }

        public static List<StudioPlacementModel> GetStudioPlacements(int placementId, int studioId)
        {
            using (var context = new InShapeEntities())
            {
                return placementId > 0
                    ? context.StudioPlacements.Where(r => r.Id == placementId && !r.IsDeleted)
                        .FilterByUser(studioId)
                        .ProjectTo<StudioPlacementModel>()
                        .ToList()
                    : context.StudioPlacements.Where(r => !r.IsDeleted).FilterByUser(studioId).ProjectTo<StudioPlacementModel>().ToList();
            }
        }


        public static bool CreateStudioRoom(StudioRoom room)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    context.StudioRooms.Add(room);
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


        public static bool UpdateStudioRoom(StudioRoom room)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    context.StudioRooms.Attach(room);
                    context.Entry(room).State = EntityState.Modified;
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

        public static bool DeleteStudioRoom(int roomId)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var room = context.StudioRooms.First(r => r.Id == roomId && !r.IsDeleted);
                    //context.StudioRooms.Remove(room);
                    room.IsDeleted = true;
                    context.StudioRooms.Attach(room);
                    context.Entry(room).State = EntityState.Modified;
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


        //public static List<Class> GetClasses(int classId, bool showPast)
        //{
        //    using (var context = new InShapeEntities())
        //    {
        //        if (showPast)
        //        {
        //            return classId > 0
        //                ? context.Classes.Include("ClassType").Include("StudioRoom").Where(r => r.Id == classId && !r.IsDeleted).ToList()
        //                : context.Classes.Include("ClassType").Include("StudioRoom").Where(r => !r.IsDeleted).ToList();
        //        }
        //        var now = DateTime.Now.Date;
        //            return classId > 0
        //                ? context.Classes.Include("ClassType").Include("StudioRoom").Where(r => r.Id == classId && !r.IsDeleted && r.Date >= now).ToList()
        //                : context.Classes.Include("ClassType").Include("StudioRoom").Where(r => !r.IsDeleted && r.Date >= now).ToList();
        //    }
        //}

        public static Class GetClass(int classId)
        {
            var todate = DateTime.UtcNow.ToLocal().Date;
            var from1m = todate.AddMonths(-1);
            var from3m = todate.AddMonths(-3);
            using (var context = new InShapeEntities())
            {
                return
                    context.Classes
                        .Include("ClassType")
                        .Include("StudioRoom")
                        .Include("Class_Instructors").FirstOrDefault(r => r.Id == classId && !r.IsDeleted);
                //.ProjectTo<StudioClassModel>()
                //.ToList();
            }
        }

        public static List<StudioClassModel> GetClasses(int classId, bool showPast, int StudioId)
        {
            var todate = DateTime.UtcNow.ToLocal().Date;
            var from1m = todate.AddMonths(-1);
            var from3m = todate.AddMonths(-3);
            using (var context = new InShapeEntities())
            {
                if (classId > 0)
                {
                    var classes =
                        context.Classes.Include("ClassType")
                            .Include("StudioRoom")
                            .Include("Class_Instructors")
                            .Where(r => r.Id == classId && !r.IsDeleted)
                            //.ProjectTo<StudioClassModel>()
                            .ToList();
                    var result = Mapper.Map<List<StudioClassModel>>(classes);
                    
                    return result;
                }
                //get avarages
                var qry1m = from cls in context.Classes
                          where !cls.IsDeleted && cls.Date >= from1m && cls.Date < todate
                            group cls by cls.DailySlotId
                  into grp1
                          select new
                          {
                              SlotId = grp1.Key,
                              Count = grp1.Select(x => x.Date).Distinct().Count(),
                              Sum = grp1.Sum(x=>x.Participants),
                              count = grp1.Select(x => x.Date).Distinct().Count(),
                              avg = !grp1.Select(x => x.Date).Distinct().Any() ? 0.0 : (double)grp1.Sum(x => x.Participants) / grp1.Select(x => x.Date).Distinct().Count(),

                          };
                var qry3m = from cls in context.Classes
                            where !cls.IsDeleted && cls.Date >= from3m && cls.Date < todate
                            group cls by cls.DailySlotId
                  into grp3
                            select new
                            {
                                SlotId = grp3.Key,
                                Count = grp3.Select(x => x.Date).Distinct().Count(),
                                Sum = grp3.Sum(x => x.Participants),
                                count = grp3.Select(x => x.Date).Distinct().Count(),
                                avg = !grp3.Select(x => x.Date).Distinct().Any() ? 0.0 : (double)grp3.Sum(x => x.Participants) / grp3.Select(x => x.Date).Distinct().Count(),
                            };
                if (showPast)
                {
                    return context.Classes.Include("ClassType").Include("StudioRoom").Where(r => !r.IsDeleted).FilterByUser(StudioId).Select(
                        x => new StudioClassModel
                        {
                            Id = x.Id,
                            StudioRoomId = x.StudioRoomId,
                            DailySlotId = x.DailySlotId,
                            ClassTypeId = x.ClassTypeId,
                            Date = x.Date,
                            Description = x.Description,
                            Duration = x.Duration,
                            IsFull = x.IsFull.Value,
                            MaxParticipants = x.MaxParticipants,
                            MaxExtraParticipants = x.MaxExtraParticipants,
                            Name = x.Name,
                            Participants = x.Participants,
                            ExtraParticipants = x.ExtraParticipants,
                            Gender =
                                x.IsForFemale && x.IsForMale ? Gender.מעורב : x.IsForFemale ? Gender.נקבה : Gender.זכר,
                            Time = x.Date,
                            Picture = x.ClassType.Picture,
                            Published = x.Published,
                            AgeGroup = (AgeGroup)x.AgeGroup,
                            AvgParticipants1M = qry1m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId) == null ? 0 : qry1m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId).avg,
                            AvgParticipants3M = qry3m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId) == null ? 0 : qry3m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId).avg
                            

                            //        AvgParticipants1M = (context.Classes
                            //.Where(a => a.DailySlotId == x.DailySlotId && x.Date >= from1m && x.Date < todate)
                            //.GroupBy(z => z.DailySlotId).Select(g => new { avg = g.Average(c => c.Participants) })).FirstOrDefault().avg.Value,
                            //AvgParticipants3M = (context.Classes.Where(
                            //        a => a.DailySlotId == x.DailySlotId && x.Date >= from1m && x.Date < todate)
                            //        .Average(z => z.Participants)) ?? 0

                            //(context.Classes.Where(
                            //        a => a.DailySlotId == x.DailySlotId && x.Date >= from1m && x.Date < todate)
                            //        .Average(z => z.Participants)) ?? 0,
                            //AvgParticipants3M =
                            //    (context.Classes.Where(
                            //        a => a.DailySlotId == x.DailySlotId && x.Date >= from3m && x.Date < todate)
                            //        .Average(z => z.Participants)) ?? 0,
                        }).ToList();
                }

                var now = DateTime.UtcNow.ToLocal().Date;
                return
                    context.Classes.Include("ClassType")
                        .Include("StudioRoom")
                        .Where(r => !r.IsDeleted && r.Date >= now).FilterByUser(StudioId)
                        .Select(
                            x => new StudioClassModel
                            {
                                Id = x.Id,
                                StudioRoomId = x.StudioRoomId,
                                DailySlotId = x.DailySlotId,
                                ClassTypeId = x.ClassTypeId,
                                Date = x.Date,
                                Description = x.Description,
                                Duration = x.Duration,
                                IsFull = x.IsFull.Value,
                                MaxParticipants = x.MaxParticipants,
                                MaxExtraParticipants = x.MaxExtraParticipants,
                                Name = x.Name,
                                Published = x.Published,
                                Participants = x.Participants,
                                ExtraParticipants = x.ExtraParticipants,
                                WaitingList = x.WaitingList,
                                Gender =
                                    x.IsForFemale && x.IsForMale
                                        ? Gender.מעורב
                                        : x.IsForFemale ? Gender.נקבה : Gender.זכר,
                                Time = x.Date,
                                Picture = x.ClassType.Picture,
                                AvgParticipants1M = qry1m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId) == null ? 0 : qry1m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId).avg,
                                AvgParticipants3M = qry3m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId) == null ?  0 : qry3m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId).avg
                            }).ToList();
            }
        }

        public static StudioClassModel GetClass(int classId, int StudioId)
        {
            var todate = DateTime.UtcNow.ToLocal().Date;
            var from1m = todate.AddMonths(-1);
            var from3m = todate.AddMonths(-3);
            using (var context = new InShapeEntities())
            {

                var classes =
                    context.Classes.Include("ClassType")
                        .Include("StudioRoom")
                        .Include("Class_Instructors")
                        .Where(r => r.Id == classId && !r.IsDeleted)
                        //.ProjectTo<StudioClassModel>()
                        .ToList();
                var result = Mapper.Map<List<StudioClassModel>>(classes);

                return result.FirstOrDefault();
            }

        }


        public static int GetDefaultStudioByComapny(int companyId)
        {
            using (var context = new InShapeEntities())
            {
                var studio = context.Studios.FirstOrDefault(s => s.CompanyId == companyId);
                return studio?.Id ?? 0;
            }
        }

        public static bool CreateClass(Class studioclass, int StudioId, bool UseClassNamefromType)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    if (UseClassNamefromType)
                    {
                        var classtype = context.ClassTypes.FirstOrDefault(t => t.Id == studioclass.ClassTypeId);
                        studioclass.Description = classtype.Description;
                    }
                    context.Classes.Add(studioclass);
                    context.SaveChanges();
                    studioclass.ShortURL = URLShortener.Shorten(studioclass.Id, StudioId);
                    context.Entry(studioclass).State = EntityState.Modified;
                    //context.Class_Instructors.AddRange(studioclass.Class_Instructors);
                    context.SaveChanges();
                    //foreach (var item in studioclass.ClassPlacements)
                    //{
                    //    item.ClassId = studioclass.Id;
                    //}
                    return true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
            //catch (DbEntityValidationException ex)
            //{
            //    var errorMessages = (from validationResult in ex.EntityValidationErrors 
            //                         let entityName = validationResult.Entry.Entity.GetType().Name 
            //                         from error in validationResult.ValidationErrors 
            //                         select entityName + "." + error.PropertyName + ": " + error.ErrorMessage).ToList();
            //    return false;
            //}
        }


        public static bool UpdateClass(Class studioclass, bool UseClassNamefromType)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    if (UseClassNamefromType)
                    {
                        var classtype = context.ClassTypes.FirstOrDefault(t => t.Id == studioclass.ClassTypeId);
                        studioclass.Description = classtype.Description;
                    }
                    //var cl = context.Classes.Include("ClassType")
                    //        .Include("StudioRoom")
                    //        .Include("Class_Instructors").FirstOrDefault(x => x.Id == studioclass.Id);
                    //if (cl != null) studioclass.Participants = cl.Participants;
                    //studioclass.IsFull = studioclass.MaxParticipants <= (studioclass.Participants+studioclass.WaitingList);
                    //studioclass.IsFull = studioclass.IsFull || studioclass.MaxParticipants <= (studioclass.Participants);
                    
                    //context.Entry(studioclass.Class_Instructors).State = EntityState.Added;
                    //context.Entry(studioclass).State = EntityState.Modified;
                    
                    //remove old instructors and add new ones
                    context.Class_Instructors.RemoveRange(
                        context.Class_Instructors.Where(x => x.ClassId == studioclass.Id));
                    context.Class_Instructors.AddRange(studioclass.Class_Instructors);
                    context.Classes.Attach(studioclass);
                    context.Entry(studioclass).State = EntityState.Modified;
                    foreach (var item in studioclass.ClassAvailablePlacements)
                    {
                        context.ClassAvailablePlacements.Attach(item);
                        context.Entry(item).State = EntityState.Modified;
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

        //public static bool UpdateClass(Class studioclass)
        //{
        //    try
        //    {
        //        using (var context = new InShapeEntities())
        //        {
        //            //var cl = context.Classes.FirstOrDefault(x => x.Id == studioclass.Id);
        //            //if (cl != null) studioclass.Participants = cl.Participants;
        //            studioclass.IsFull = studioclass.IsFull || studioclass.MaxParticipants <= (studioclass.Participants);

        //            context.Entry(studioclass.Class_Instructors).State = EntityState.Added;
        //            context.Entry(studioclass).State = EntityState.Modified;
        //            context.Classes.Attach(studioclass);
        //            //remove old instructors and add new ones
        //            context.Class_Instructors.RemoveRange(
        //                context.Class_Instructors.Where(x => x.ClassId == studioclass.Id));
        //            context.Class_Instructors.AddRange(studioclass.Class_Instructors);
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

        public static bool DeleteClass(int classId)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var studioclass = context.Classes.First(r => r.Id == classId && !r.IsDeleted);
                    //context.StudioRooms.Remove(room);
                    studioclass.IsDeleted = true;
                    context.Classes.Attach(studioclass);
                    context.Entry(studioclass).State = EntityState.Modified;
                    //delete class enrolments and update subscriptions
                    var enrollments = context.ClassEnrollments.Where(e => e.ClassId == studioclass.Id && !e.IsDeleted);
                    foreach (var enrollment in enrollments)
                    {
                        var subscription =
                            context.UserSubscriptions.FirstOrDefault(s => s.Id == enrollment.SubscriptionId && s.Active);
                        enrollment.IsDeleted = true;
                        enrollment.DateCanceled = DateTime.UtcNow.ToLocal();
                        if (subscription != null && subscription.NumClasses > 0)
                        {
                            subscription.CurrentBalance ++;
                            context.UserSubscriptions.Attach(subscription);
                            context.Entry(subscription).State = EntityState.Modified;
                        }
                        context.ClassEnrollments.Attach(enrollment);
                        context.Entry(enrollment).State = EntityState.Modified;
                    }
                    Logger.WriteInfo($"Class: {studioclass.Id}, at: {studioclass.Date.ToLongDateString()} deleted at: {DateTime.UtcNow.ToLocal()} with {enrollments.Count()} enrollments");
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


        public static List<StudioClassModel> GetClassesForCalander(int roomId, DateTime StartDate)
        {
            var enddate = StartDate.AddDays(7);
            using (var context = new InShapeEntities())
            {
                return Mapper.Map<List<StudioClassModel>>(
                    context.Classes.Include("ClassType").Include("StudioRoom").Where(
                        x => !x.IsDeleted && x.StudioRoomId == roomId && x.Date >= StartDate && x.Date < enddate)
                        .ToList());
            }
        }

        public static List<CalendarStudioClassModel> GetDailyClassesForCalander(DateTime date, int StudioId, int classid = 0)
        {
            var startdate = date.Date;
            var enddate = date.Date.AddDays(1); // was 1
            using (var context = new InShapeEntities())
            {
                //return Mapper.Map<List<CalendarStudioClassModel>>(
                //    context.Classes
                //    //.Include("ClassType").Include("ClassTypeDetail").Include("StudioRoom").Include("Class_Instructors").Include("Class_Instructors.AspNetUser")
                //    .Where(
                //        x => !x.IsDeleted && x.Date >= startdate && x.Date < enddate && x.Published).FilterByUser(StudioId).OrderBy(x => x.Date)
                //        .ToList());
                var dateParameter = new SqlParameter("Date", date);
                var StudioIdParameter = new SqlParameter("StudioId", StudioId);
                var ClassIdParameter = new SqlParameter("ClassId", classid);
                return context.Database.SqlQuery<CalendarStudioClassModel>("EXEC GetClassesForDailyCalendar @Date, @StudioId, @ClassId", dateParameter, StudioIdParameter, ClassIdParameter).ToList();
            }
        }

        public static List<StudioClassModel> GetClassByInstructor(string instructorId,DateTime date)
        {
            var startdate = date.Date;
            var enddate = startdate.AddMonths(1);
            using (var context = new InShapeEntities())
            {
                return Mapper.Map<List<StudioClassModel>>(
                    context.Classes.Include("ClassType").Include("StudioRoom").Include("Class_Instructors")
                    .Where(x => !x.IsDeleted && x.Date >= startdate && x.Date < enddate 
                           && x.Class_Instructors.Any(i=>i.InstructorId == instructorId))
                    .OrderBy(x => x.Date).ToList());
            }
        }

        public static List<StudioClassModel> GetClassByTypesDetails(int TypeId, int month)
        {
            var now = DateTime.UtcNow.ToLocal();
            var from = DateTime.UtcNow.ToLocal().AddMonths(-month);
            using (var context = new InShapeEntities())
            {
                return Mapper.Map<List<StudioClassModel>>(
                    context.Classes.Include("ClassType").Include("StudioRoom")
                    .Where(x => !x.IsDeleted && x.Date >= from && x.Date < now && x.ClassTypeDetailsId == TypeId)
                    .OrderBy(x => x.Date).ToList());
            }
        }


        public static List<StudioClassModel> GetWeeklyClassesForCalander(int weekno, int StudioId)
        {
            var startdate = Utils.FirstDateOfWeek(DateTime.UtcNow.Year, weekno).Date;
            var enddate = startdate.Date.AddDays(7);
            using (var context = new InShapeEntities())
            {
                return Mapper.Map<List<StudioClassModel>>(
                    context.Classes.Include("ClassType").Include("StudioRoom").Where(
                        x => !x.IsDeleted && x.Date >= startdate && x.Date < enddate).FilterByUser(StudioId).OrderBy(x => x.Date)
                        .ToList());
            }
        }

        public static List<CalendarStudioClassModel> GetClassesForCalanderByRoom(int roomid)
        {
            var today = DateTime.Now.Date;
            using (var context = new InShapeEntities())
            {
                return Mapper.Map<List<CalendarStudioClassModel>>(
                    context.Classes.Include("ClassType").Include("StudioRoom").Where(x => !x.IsDeleted && x.StudioRoomId == roomid && x.Date >= today && x.Published).ToList());
            }
        }

        public static List<StudioClassModel> GetSClassesForReporting(int roomId)
        {
            var fromdate = new DateTime(DateTime.UtcNow.ToLocal().Year, DateTime.UtcNow.ToLocal().Month-1, 1);
            var todate = DateTime.UtcNow.ToLocal();
            using (var context = new InShapeEntities())
            {
                return Mapper.Map<List<StudioClassModel>>(
                    context.Classes.Include("ClassType").Include("StudioRoom").Where(
                        x => x.StudioRoomId == roomId && x.Date >= fromdate && x.Date < todate)
                        .ToList());
            }
        }

        public static List<StudioClassModel> GetClassesByWeek(int WeekNo, int StudioId)
        {
            var startdate = Utils.FirstDateOfWeek(DateTime.UtcNow.ToLocal().Year, WeekNo);
            var studioClasses = GetClasses(startdate, false, StudioId);

            return studioClasses;
        }

        //public static List<StudioClassModel> GetClassesByWeek(DateTime StartDate)
        //{
        //    var todate = StartDate.AddDays(7);
        //    //var enddate = StartDate.AddDays(7);
        //    var from1m = todate.AddMonths(-1);
        //    var from3m = todate.AddMonths(-3);
        //    using (var context = new InShapeEntities())
        //    {
        //        //get avarages
        //        var qry1m = from cls in context.Classes
        //                    where !cls.IsDeleted && cls.Date >= from1m && cls.Date < todate
        //                    group cls by cls.DailySlotId
        //          into grp1
        //                    select new
        //                    {
        //                        SlotId = grp1.Key,
        //                        Count = grp1.Select(x => x.Date).Distinct().Count(),
        //                        Sum = grp1.Sum(x => x.Participants),
        //                        count = grp1.Select(x => x.Date).Distinct().Count(),
        //                        avg = !grp1.Select(x => x.Date).Distinct().Any() ? 0.0 : (double)grp1.Sum(x => x.Participants) / grp1.Select(x => x.Date).Distinct().Count(),

        //                    };
        //        var qry3m = from cls in context.Classes
        //                    where !cls.IsDeleted && cls.Date >= from3m && cls.Date < todate
        //                    group cls by cls.DailySlotId
        //          into grp3
        //                    select new
        //                    {
        //                        SlotId = grp3.Key,
        //                        Count = grp3.Select(x => x.Date).Distinct().Count(),
        //                        Sum = grp3.Sum(x => x.Participants),
        //                        count = grp3.Select(x => x.Date).Distinct().Count(),
        //                        avg = !grp3.Select(x => x.Date).Distinct().Any() ? 0.0 : (double)grp3.Sum(x => x.Participants) / grp3.Select(x => x.Date).Distinct().Count(),
        //                    };

        //        return
        //            context.Classes.Include("ClassType")
        //                .Include("StudioRoom")
        //                .Where(r => !r.IsDeleted && r.Date >= StartDate && r.Date < todate).FilterByCompany()
        //                .Select(
        //                    x => new StudioClassModel
        //                    {
        //                        Id = x.Id,
        //                        StudioRoomId = x.StudioRoomId,
        //                        StudioRoomName = x.StudioRoom.Name,
        //                        DailySlotId = x.DailySlotId,
        //                        ClassTypeId = x.ClassTypeId,
        //                        Date = x.Date,
        //                        Description = x.Description,
        //                        Duration = x.Duration,
        //                        IsFull = x.IsFull,
        //                        MaxParticipants = x.MaxParticipants,
        //                        Name = x.Name,
        //                        Participants = x.Participants,
        //                        Gender =
        //                            x.IsForFemale && x.IsForMale
        //                                ? Gender.מעורב
        //                                : x.IsForFemale ? Gender.נקבה : Gender.זכר,
        //                        Time = x.Date,
        //                        Instructors = Mapper.Map<List<InstructorDetailsModel>>(x.Class_Instructors),
        //                        Picture = x.ClassType.Picture,
        //                        AvgParticipants1M = qry1m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId) == null ? 0 : qry1m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId).avg,
        //                        AvgParticipants3M = qry3m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId) == null ? 0 : qry3m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId).avg
        //                    }).ToList();
        //    }
        //}

        //public static List<StudioClassModel> GetClassesByWeek(DateTime StartDate)
        //{
        //    var todate = StartDate.AddDays(7);
        //    //var enddate = StartDate.AddDays(7);
        //    var from1m = todate.AddMonths(-1);
        //    var from3m = todate.AddMonths(-3);
        //    //var usePlacements = false;
        //    using (var context = new InShapeEntities())
        //    {
        //        //get avarages
        //        var qry1m = from cls in context.Classes
        //                    where !cls.IsDeleted && cls.Date >= from1m && cls.Date < todate
        //                    group cls by cls.DailySlotId
        //          into grp1
        //                    select new
        //                    {
        //                        SlotId = grp1.Key,
        //                        Count = grp1.Select(x => x.Date).Distinct().Count(),
        //                        Sum = grp1.Sum(x => x.Participants),
        //                        count = grp1.Select(x => x.Date).Distinct().Count(),
        //                        avg = !grp1.Select(x => x.Date).Distinct().Any() ? 0.0 : (double)grp1.Sum(x => x.Participants) / grp1.Select(x => x.Date).Distinct().Count(),

        //                    };
        //        var qry3m = from cls in context.Classes
        //                    where !cls.IsDeleted && cls.Date >= from3m && cls.Date < todate
        //                    group cls by cls.DailySlotId
        //          into grp3
        //                    select new
        //                    {
        //                        SlotId = grp3.Key,
        //                        Count = grp3.Select(x => x.Date).Distinct().Count(),
        //                        Sum = grp3.Sum(x => x.Participants),
        //                        count = grp3.Select(x => x.Date).Distinct().Count(),
        //                        avg = !grp3.Select(x => x.Date).Distinct().Any() ? 0.0 : (double)grp3.Sum(x => x.Participants) / grp3.Select(x => x.Date).Distinct().Count(),
        //                    };

        //        var classes = Mapper.Map<List<StudioClassModel>>(
        //            context.Classes.Include("ClassType").Include("ClassTypeDetail")
        //                .Include("StudioRoom")
        //                .Where(r => !r.IsDeleted && r.Date >= StartDate && r.Date < todate).FilterByCompany()
        //                //.Select(
        //                //    x => new StudioClassModel
        //                //    {
        //                //        Id = x.Id,
        //                //        StudioRoomId = x.StudioRoomId,
        //                //        StudioRoomName = x.StudioRoom.Name,
        //                //        DailySlotId = x.DailySlotId,
        //                //        ClassTypeId = x.ClassTypeId,
        //                //        Date = x.Date,
        //                //        Description = x.Description,
        //                //        Duration = x.Duration,
        //                //        IsFull = x.IsFull,
        //                //        MaxParticipants = x.MaxParticipants,
        //                //        Name = x.Name,
        //                //        Participants = x.Participants,
        //                //        Gender =
        //                //            x.IsForFemale && x.IsForMale
        //                //                ? Gender.מעורב
        //                //                : x.IsForFemale ? Gender.נקבה : Gender.זכר,
        //                //        Time = x.Date,
        //                //        Instructors = Mapper.Map<List<InstructorDetailsModel>>(x.Class_Instructors),
        //                //        Picture = x.ClassType.Picture,
        //                //        AvgParticipants1M = qry1m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId) == null ? 0 : qry1m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId).avg,
        //                //        AvgParticipants3M = qry3m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId) == null ? 0 : qry3m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId).avg
        //                //    })
        //                .ToList());
        //        classes.ForEach(x =>
        //        {
        //            x.AvgParticipants1M = qry1m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId) == null
        //                ? 0
        //                : qry1m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId).avg;
        //            x.AvgParticipants3M = qry3m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId) == null
        //                ? 0
        //                : qry3m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId).avg;
        //            x.UsePlacements = context.ClassAvailablePlacements.Any(c => c.ClassId == x.Id && !c.IsDeleted);
        //        });
        //        return classes;
        //    }
            
        //}


        public static List<StudioClassModel> GetClasses(DateTime StartDate, int StudioId, int days = 7)
        {
            var todate = StartDate.AddDays(days);
            //var enddate = StartDate.AddDays(7);
            var from1m = todate.AddMonths(-1);
            var from3m = todate.AddMonths(-3);
            //var usePlacements = false;
            using (var context = new InShapeEntities())
            {
                //get avarages
                var qry1m = from cls in context.Classes
                            where !cls.IsDeleted && cls.Date >= from1m && cls.Date < todate
                            group cls by cls.DailySlotId
                  into grp1
                            select new
                            {
                                SlotId = grp1.Key,
                                Count = grp1.Select(x => x.Date).Distinct().Count(),
                                Sum = grp1.Sum(x => x.Participants),
                                count = grp1.Select(x => x.Date).Distinct().Count(),
                                avg = !grp1.Select(x => x.Date).Distinct().Any() ? 0.0 : (double)grp1.Sum(x => x.Participants) / grp1.Select(x => x.Date).Distinct().Count(),

                            };
                var qry3m = from cls in context.Classes
                            where !cls.IsDeleted && cls.Date >= from3m && cls.Date < todate
                            group cls by cls.DailySlotId
                  into grp3
                            select new
                            {
                                SlotId = grp3.Key,
                                Count = grp3.Select(x => x.Date).Distinct().Count(),
                                Sum = grp3.Sum(x => x.Participants),
                                count = grp3.Select(x => x.Date).Distinct().Count(),
                                avg = !grp3.Select(x => x.Date).Distinct().Any() ? 0.0 : (double)grp3.Sum(x => x.Participants) / grp3.Select(x => x.Date).Distinct().Count(),
                            };

                var classes = Mapper.Map<List<StudioClassModel>>(
                    context.Classes.Include("ClassType").Include("ClassTypeDetail")
                        .Include("StudioRoom")
                        .Where(r => !r.IsDeleted && r.Date >= StartDate && r.Date < todate).FilterByUser(StudioId)
                        //.Select(
                        //    x => new StudioClassModel
                        //    {
                        //        Id = x.Id,
                        //        StudioRoomId = x.StudioRoomId,
                        //        StudioRoomName = x.StudioRoom.Name,
                        //        DailySlotId = x.DailySlotId,
                        //        ClassTypeId = x.ClassTypeId,
                        //        Date = x.Date,
                        //        Description = x.Description,
                        //        Duration = x.Duration,
                        //        IsFull = x.IsFull,
                        //        MaxParticipants = x.MaxParticipants,
                        //        Name = x.Name,
                        //        Participants = x.Participants,
                        //        Gender =
                        //            x.IsForFemale && x.IsForMale
                        //                ? Gender.מעורב
                        //                : x.IsForFemale ? Gender.נקבה : Gender.זכר,
                        //        Time = x.Date,
                        //        Instructors = Mapper.Map<List<InstructorDetailsModel>>(x.Class_Instructors),
                        //        Picture = x.ClassType.Picture,
                        //        AvgParticipants1M = qry1m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId) == null ? 0 : qry1m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId).avg,
                        //        AvgParticipants3M = qry3m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId) == null ? 0 : qry3m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId).avg
                        //    })
                        .ToList());
                classes.ForEach(x =>
                {
                    x.AvgParticipants1M = qry1m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId) == null
                        ? 0
                        : qry1m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId).avg;
                    x.AvgParticipants3M = qry3m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId) == null
                        ? 0
                        : qry3m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId).avg;
                    x.UsePlacements = context.ClassAvailablePlacements.Any(c => c.ClassId == x.Id && !c.IsDeleted);
                });
                return classes;
            }

        }


        public static List<StudioClassModel> GetClasses(DateTime StartDate, bool inclAVG, int StudioId, int days = 7)
        {
            var todate = StartDate.AddDays(days);
            //var enddate = StartDate.AddDays(7);
            var from1m = todate.AddMonths(-1);
            var from3m = todate.AddMonths(-3);
            //var usePlacements = false;
            using (var context = new InShapeEntities())
            {
                //get avarages
                

                var classes = Mapper.Map<List<StudioClassModel>>(
                    context.Classes.Include("ClassType").Include("ClassTypeDetail")
                        .Include("StudioRoom")
                        .Where(r => !r.IsDeleted && r.Date >= StartDate && r.Date < todate).FilterByUser(StudioId)
                        //.Select(
                        //    x => new StudioClassModel
                        //    {
                        //        Id = x.Id,
                        //        StudioRoomId = x.StudioRoomId,
                        //        StudioRoomName = x.StudioRoom.Name,
                        //        DailySlotId = x.DailySlotId,
                        //        ClassTypeId = x.ClassTypeId,
                        //        Date = x.Date,
                        //        Description = x.Description,
                        //        Duration = x.Duration,
                        //        IsFull = x.IsFull,
                        //        MaxParticipants = x.MaxParticipants,
                        //        Name = x.Name,
                        //        Participants = x.Participants,
                        //        Gender =
                        //            x.IsForFemale && x.IsForMale
                        //                ? Gender.מעורב
                        //                : x.IsForFemale ? Gender.נקבה : Gender.זכר,
                        //        Time = x.Date,
                        //        Instructors = Mapper.Map<List<InstructorDetailsModel>>(x.Class_Instructors),
                        //        Picture = x.ClassType.Picture,
                        //        AvgParticipants1M = qry1m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId) == null ? 0 : qry1m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId).avg,
                        //        AvgParticipants3M = qry3m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId) == null ? 0 : qry3m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId).avg
                        //    })
                        .ToList());
                classes.ForEach(x =>
                {
                    if (inclAVG)
                    {
                        var qry1m = from cls in context.Classes
                                    where !cls.IsDeleted && cls.Date >= from1m && cls.Date < todate
                                    group cls by cls.DailySlotId
                  into grp1
                                    select new
                                    {
                                        SlotId = grp1.Key,
                                        Count = grp1.Select(c => c.Date).Distinct().Count(),
                                        Sum = grp1.Sum(c => c.Participants),
                                        count = grp1.Select(c => c.Date).Distinct().Count(),
                                        avg = !grp1.Select(c => c.Date).Distinct().Any() ? 0.0 : (double)grp1.Sum(c => x.Participants) / grp1.Select(c => c.Date).Distinct().Count(),

                                    };
                        var qry3m = from cls in context.Classes
                                    where !cls.IsDeleted && cls.Date >= from3m && cls.Date < todate
                                    group cls by cls.DailySlotId
                          into grp3
                                    select new
                                    {
                                        SlotId = grp3.Key,
                                        Count = grp3.Select(c => x.Date).Distinct().Count(),
                                        Sum = grp3.Sum(c => c.Participants),
                                        count = grp3.Select(c => c.Date).Distinct().Count(),
                                        avg = !grp3.Select(c => c.Date).Distinct().Any() ? 0.0 : (double)grp3.Sum(c => c.Participants) / grp3.Select(c => c.Date).Distinct().Count(),
                                    };
                        x.AvgParticipants1M = qry1m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId) == null
                            ? 0
                            : qry1m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId).avg;
                        x.AvgParticipants3M = qry3m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId) == null
                            ? 0
                            : qry3m.FirstOrDefault(grp => grp.SlotId == x.DailySlotId).avg;
                    }
                    x.UsePlacements = context.ClassAvailablePlacements.Any(c => c.ClassId == x.Id && !c.IsDeleted);
                });
                return classes;
            }

        }

        public static bool CopyWeeklyCalander(DateTime startdate, int StudioId, bool AutoPublish)
        {
            bool result = false;
            try
            {
                lock (thisLock)
                {
                    using (var context = new InShapeEntities())
                    {
                        //string sqlQuery = "SELECT [dbo].[CountMeals] ({0})";
                        //Object[] parameters = { startdate };
                        //int activityCount = context.Database.SqlQuery<int>(sqlQuery, parameters).FirstOrDefault();
                        var startdateParameter = new SqlParameter("startDate", startdate);
                        var AutoPublishParameter = new SqlParameter("AutoPublish", AutoPublish);
                        var StudioIdParameter = new SqlParameter("StudioId", StudioId);
                        result = context.Database.SqlQuery<bool>("EXEC CopyWeeklyClanderForCompany @startDate, @AutoPublish, @StudioId", startdateParameter, AutoPublishParameter, StudioIdParameter).SingleOrDefault();         
                    }
                }
            }
            catch
            {
                result = false;
            }
            UpdateClassLinks(StudioId);
            return result;
        }

        public static bool PublishWeeklyCalander(DateTime startdate, int StudioId)
        {
            using (var context = new InShapeEntities())
            {
                //string sqlQuery = "SELECT [dbo].[CountMeals] ({0})";
                //Object[] parameters = { startdate };
                //int activityCount = context.Database.SqlQuery<int>(sqlQuery, parameters).FirstOrDefault();
                var startdateParameter = new SqlParameter("startDate", startdate);
                var StudioIdParameter = new SqlParameter("StudioId", StudioId);
                var result = context.Database.SqlQuery<bool>("EXEC PublishWeeklyClanderForCompany @startDate, @StudioId", startdateParameter, StudioIdParameter).SingleOrDefault();
                UpdateClassLinks(StudioId);
                return result;
            }
        }

        public static void UpdateClassLinks(int StudioId)
        {
            var date = DateTime.UtcNow.ToLocal();
            using (var context = new InShapeEntities())
            {
                var classes = context.Classes.Where(c => !c.IsDeleted && c.Date > date && c.ShortURL == null );
                foreach (var c in classes)
                {
                    c.ShortURL = URLShortener.ShortenFB(c.Id, StudioId);
                    context.Classes.Attach(c);
                    context.Entry(c).State = EntityState.Modified;
                }
                context.SaveChanges();
            }
        }

    }
}
