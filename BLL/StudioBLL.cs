using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using InShapeModels;
using DAL;
using Utilities;

namespace BLL
{
    public static class StudioBLL
    {
        //readonly StudioRepo _studioRepo = new StudioRepo();
        public static List<StudioRoomModel> GetStudioRooms(int id, int studioId)
        {
            return StudioRepo.GetStudioRooms(id, studioId);
        }

        public static List<StudioPlacementModel> GetStudioPlacements(int id, int studioId)
        {
            return StudioRepo.GetStudioPlacements(id, studioId);
        }

        public static void CreateStudioRoom(StudioRoomModel room)
        {
            StudioRepo.CreateStudioRoom(Mapper.Map<StudioRoom>(room));
        }

        public static bool UpdateStudioRoom(StudioRoomModel room)
        {
            return StudioRepo.UpdateStudioRoom(Mapper.Map<StudioRoom>(room));
        }

        public static bool DeleteStudioRoom(int roomid)
        {
            return StudioRepo.DeleteStudioRoom(roomid);
        }


        //public static List<StudioClassModel> GetClasses(int id, bool showPast)
        //{
        //    var studioClasses = StudioRepo.GetClasses(id, showPast);
        //    var classes = new List<StudioClassModel>();
            
        //    if (studioClasses.Any()) studioClasses.ForEach(x => classes.Add(ConvertToModel(x)));
        //    return classes.OrderBy(x => x.Date).ThenBy(x => x.DailySlotId).ToList();
        //}

        public static List<StudioClassModel> GetClasses(int id, bool showPast, int StudioId, bool filterPlacements = false)
        {
            var studioClasses = StudioRepo.GetClasses(id, showPast, StudioId);
            //var classes = new List<StudioClassModel>();

            if (filterPlacements && studioClasses.Any()) studioClasses.ForEach(x => x.ClassAvailablePlacements = x.ClassAvailablePlacements.Where(p=> !p.IsDeleted && !p.IsInUse).OrderBy(y=> y.StudioPlacementId).ThenBy(y=> y.ClassPlacementNumber).ToList());
            studioClasses.Where(c => c.Id > 0).ToList().ForEach(x => x.ClassAvailablePlacements.ForEach(y => y.ToCreate = !y.IsDeleted));
            return studioClasses.OrderBy(x => x.Date).ThenBy(x => x.DailySlotId).ToList();
        }

        //public static List<StudioClassModel> GetClasses(int id, bool showPast, string userId, Gender gender)
        //{
        //    var studioClasses = StudioRepo.GetClasses(id, showPast);
        //    var classes = new List<StudioClassModel>();

        //    if (studioClasses.Any()) studioClasses.ForEach(x => classes.Add(ConvertToModel(x, userId, gender)));
        //    return classes;
        //}

        public static List<StudioClassModel> GetClasses(int id, bool showPast, string userId, Gender gender, int StudioId, AgeGroup age = AgeGroup.מבוגרים)
        {
            var studioClasses = StudioRepo.GetClasses(id, showPast, StudioId);
            //var classes = new List<StudioClassModel>();
            var enrollments = ClassBLL.GetEnrollmentsByUserDate(userId, DateTime.MinValue.Date);
            var waitingList = ClassBLL.GetWaitingListByUserDate(userId, DateTime.MinValue.Date);
            var placements = ClassBLL.GetClassAvailablePlacementsForDate(studioClasses.FirstOrDefault().Date.Date, StudioId);
            var instructors = UserBll.GetInstructorList(StudioId);
            if (studioClasses.Any())
                studioClasses.ForEach(x => SetExtraProperties(Mapper.Map<CalendarStudioClassModel>(x) , userId, gender, age, enrollments, waitingList, placements, instructors));
            return studioClasses.OrderBy(x => x.Date).ThenBy(x => x.DailySlotId).ToList();
        }

        public static CalendarStudioClassModel GetClass(int id, string userId, Gender gender, int StudioId, AgeGroup age = AgeGroup.מבוגרים)
        {
            var classmodel = StudioRepo.GetDailyClassesForCalander(DateTime.Now, StudioId, id).FirstOrDefault();
            //var classes = new List<StudioClassModel>();
            //var enrollments = ClassBLL.GetEnrollmentsByUserDate(userId, studioClasses.FirstOrDefault().Date.Date);
            //var waitingList = ClassBLL.GetWaitingListByUserDate(userId, studioClasses.FirstOrDefault().Date.Date);
            var placements = ClassBLL.GetClassAvailablePlacementsForDate(classmodel.Date.Date, StudioId, classmodel.Id);
            classmodel.Instructors = UserBll.GetClassInstructors(classmodel.InstructorIds);
            //if (studioClasses.Any()) studioClasses
            //       .ForEach(x => SetExtraProperties(Mapper.Map<CalendarStudioClassModel>(x), userId, gender, age, enrollments, waitingList, placements, instructors));
            classmodel.IsUserGender = classmodel.Gender == Gender.מעורב || classmodel.Gender == gender;
            classmodel.IsUserAge = (int)age >= (int)classmodel.AgeGroup;
            var enrollment = ClassBLL.GetUserEnrollment(classmodel.Id, userId);
            if (enrollment != null)
            {
                classmodel.SetUserEnrolled(true, enrollment.StudioPlacementId > 0 ? enrollment.PlacementDisplyName : string.Empty);
            }
            else classmodel.SetUserEnrolled(false, string.Empty);
            classmodel.UserWaitingList = ClassBLL.UserInWaitList(classmodel.Id, userId);
            classmodel.Time = classmodel.Date;
            if (placements.Any()) classmodel.ClassAvailablePlacements = placements.Where(p => !p.IsDeleted && !p.IsInUse).ToList();
            return classmodel;
        }


        private static void SetExtraProperties(CalendarStudioClassModel classmodel, string userId, Gender gender, AgeGroup age, List<CalendarClassEnrollmentModel> enrollments, List<WaitListEnrollment> waitingList, List<CalendarAvailablePlacementsModel> placements, List<InstructorDetailsModel> instructors)
        {
            classmodel.IsUserGender = classmodel.Gender == Gender.מעורב || classmodel.Gender == gender;
            classmodel.IsUserAge = (int)age >= (int)classmodel.AgeGroup;
            var instructorids = classmodel.InstructorIds.Split(';');
            var enrollment = enrollments.FirstOrDefault(e => e.ClassId == classmodel.Id);
            
            if (enrollment != null)
            {
                //if (classmodel.UsePlacements) var selectedplacement = placements.FirstOrDefault(p=> p.Id == enrollment.)
                //classmodel.SetUserEnrolled(true, enrollment.SelectedPlacement != null ? enrollment.SelectedPlacement.DisplyName : string.Empty);
                classmodel.SetUserEnrolled(true, enrollment.StudioPlacementId > 0 ? enrollment.PlacementDisplyName : string.Empty);
            }
            else classmodel.SetUserEnrolled(false, string.Empty);
            classmodel.UserWaitingList = waitingList.FirstOrDefault(e => e.ClassId == classmodel.Id);
            //classmodel.IsUserInWaitingList = userId != null && waitinglistenrollment != null;
            classmodel.Time = classmodel.Date;
            if (placements.Any(x=> x.ClassId == classmodel.Id))
                classmodel.ClassAvailablePlacements = placements.Where(p => !p.IsDeleted && !p.IsInUse && p.ClassId == classmodel.Id).ToList();
            if (instructors.Any())
                classmodel.Instructors = instructors.Where(p => instructorids.Any(x => x.Trim() == p.InstructorId)).ToList();
        }

        //private static void SetExtraProperties(StudioClassModel classmodel, string userId, Gender gender, AgeGroup age)
        //{
        //    classmodel.IsUserGender = classmodel.Gender == Gender.מעורב || classmodel.Gender == gender;
        //    classmodel.IsUserAge = (int)age >= (int)classmodel.AgeGroup;
        //    var enrollment = ClassBLL.GetUserEnrollment(classmodel.Id, userId);

        //    if (enrollment != null)
        //    {
        //        classmodel.SetUserEnrolled(true, enrollment.SelectedPlacement != null ? enrollment.SelectedPlacement.DisplyName : string.Empty);
        //    }
        //    else classmodel.SetUserEnrolled(false, string.Empty);
        //    classmodel.UserWaitingList = ClassBLL.UserInWaitList(classmodel.Id, userId);
        //    //classmodel.IsUserInWaitingList = userId != null && waitinglistenrollment != null;
        //    classmodel.Time = classmodel.Date;
        //    if (classmodel.ClassAvailablePlacements.Any()) classmodel.ClassAvailablePlacements = classmodel.ClassAvailablePlacements.Where(p => !p.IsDeleted && !p.IsInUse).ToList();
        //}

        public static List<StudioClassModel> GetSClassesForReporting(int roomid)
        {
            var studioClasses = StudioRepo.GetSClassesForReporting(roomid);
            //var classes = new List<StudioClassModel>();

            //if (studioClasses.Any()) studioClasses.ForEach(x => classes.Add(ConvertToModel(x)));
            //return classes;
            //studioClasses.ForEach(x => x.SetUserEnrolled(ClassBLL.IsUserEnrolled(x.Id, userId)));
            return studioClasses;
        }

        

        public static void CreateClass(StudioClassModel classmodel,string userid, int StudioId, bool AutoPublish, bool UseClassNamefromType)
        {
            classmodel.Published = AutoPublish? AutoPublish: classmodel.Published; //|| Utils.GetIso8601WeekOfYear(classmodel.Date.ToLocal().Date) == Utils.GetIso8601WeekOfYear(DateTime.UtcNow.ToLocal().Date);
            StudioRepo.CreateClass(ConvertToEntity(classmodel, userid), StudioId, UseClassNamefromType);
        }

        public static bool UpdateClass(StudioClassModel classmodel, string userid, bool UseClassNamefromType)
        {
            var result = StudioRepo.UpdateClass(ConvertToEntity(classmodel, userid), UseClassNamefromType);
            return result;
            //if (classmodel.MaxParticipants - classmodel.Participants > 0 && classmodel.IsFull) SMSBLL.SendWaitingListMSG(classmodel.Id);
        }

        public static bool DeleteClass(int classid)
        {
            return StudioRepo.DeleteClass(classid);
        }

        public static List<StudioClassModel> GetClassesForCalander(int roomId, int WeekNo)
        {
            var startdate = Utilities.Utils.FirstDateOfWeek(DateTime.Now.Year, WeekNo);
            var studioClasses = StudioRepo.GetClassesForCalander(roomId, startdate);
            //var classes = new List<StudioClassModel>();

            //if (studioClasses.Any()) studioClasses.ForEach(x => classes.Add(ConvertToModel(x)));
            //return classes;
            //studioClasses.ForEach(x => x.SetUserEnrolled(ClassBLL.IsUserEnrolled(x.Id, userId)));
            return studioClasses;
        }

        public static List<StudioClassModel> GetClassesByWeek(int WeekNo, int StudioId, bool inclAVG = false)
        {
            var startdate = Utils.FirstDateOfWeek(DateTime.Now.Year, WeekNo);
            var studioClasses = StudioRepo.GetClasses(startdate, inclAVG, StudioId);
            
            return studioClasses;
        }

        public static List<StudioClassModel> GetNextClasses(DateTime start, int StdudioId, int days)
        {
            var studioClasses = StudioRepo.GetClasses(start,false, StdudioId, days);

            return studioClasses;
        }

        public static List<StudioClassModel> GetDailyClassesForCalander(DateTime date, int StudioId)
        {
            var studioClasses = StudioRepo.GetDailyClassesForCalander(date, StudioId);
            var classes = Mapper.Map<List<StudioClassModel>>(studioClasses);
            AddEmptySlots(classes, date, StudioId);
            //var classes = new List<StudioClassModel>();

            //if (studioClasses.Any()) studioClasses.ForEach(x => classes.Add(ConvertToModel(x)));
            //return classes;
            //studioClasses.ForEach(x => x.SetUserEnrolled(ClassBLL.IsUserEnrolled(x.Id, userId)));
            return classes;
        }

        public static List<StudioClassModel> GetClassByInstructor(string InstructorId, DateTime date)
        {
            var studioClasses = StudioRepo.GetClassByInstructor(InstructorId,date);
            //AddEmptySlots(studioClasses, date);
            //var classes = new List<StudioClassModel>();

            //if (studioClasses.Any()) studioClasses.ForEach(x => classes.Add(ConvertToModel(x)));
            //return classes;
            //studioClasses.ForEach(x => x.SetUserEnrolled(ClassBLL.IsUserEnrolled(x.Id, userId)));
            return studioClasses;
        }


        public static List<StudioClassModel> GetClassByTypesDetails(int TypeId, int month)
        {
            var studioClasses = StudioRepo.GetClassByTypesDetails(TypeId, month);
            //AddEmptySlots(studioClasses, date);
            //var classes = new List<StudioClassModel>();

            //if (studioClasses.Any()) studioClasses.ForEach(x => classes.Add(ConvertToModel(x)));
            //return classes;
            //studioClasses.ForEach(x => x.SetUserEnrolled(ClassBLL.IsUserEnrolled(x.Id, userId)));
            return studioClasses;
        }



        internal static List<StudioClassModel> GetWeeklyClassesForCalander(int weekno, int StudioId)
        {
            var studioClasses = StudioRepo.GetWeeklyClassesForCalander(weekno, StudioId);
            //AddEmptySlots(studioClasses, date);
            //var classes = new List<StudioClassModel>();

            //if (studioClasses.Any()) studioClasses.ForEach(x => classes.Add(ConvertToModel(x)));
            //return classes;
            //studioClasses.ForEach(x => x.SetUserEnrolled(ClassBLL.IsUserEnrolled(x.Id, userId)));
            return studioClasses;
        }

        public static List<CalendarStudioClassModel> GetDailyClassesForCalander(DateTime date, string userId, Gender gender, AgeGroup ageGroup, int StudioId, bool AddEmpty = true)
        {
            var studioClasses = StudioRepo.GetDailyClassesForCalander(date, StudioId);
            //var classes = Mapper.Map<List<StudioClassModel>>(studioClasses);
            //if (studioClasses.Any()) studioClasses.ForEach(x => classes.Add(ConvertToModel(x,userId,gender)));
            //return classes;
            //studioClasses.ForEach(x => x.SetUserEnrolled();
            //studioClasses.ForEach(x => x.SetUserinWaitList();
            var enrollments = ClassBLL.GetEnrollmentsByUserDate(userId, date);
            var waitingList = ClassBLL.GetWaitingListByUserDate(userId, date);
            var placements = ClassBLL.GetClassAvailablePlacementsForDate(date, StudioId);
            var instructors = UserBll.GetInstructorList(StudioId);
            studioClasses.ForEach(x => SetExtraProperties(x, userId, gender, ageGroup, enrollments, waitingList, placements, instructors));
            if (AddEmpty) studioClasses.AddRange(AddEmptyClasses(studioClasses)); //AddEmptySlots(studioClasses, date.Date);
            return studioClasses;
        }

        private static List<CalendarStudioClassModel> AddEmptyClasses(List<CalendarStudioClassModel> studioClasses)
        {
            var emptyClasses = new List<CalendarStudioClassModel>();
            var rooms = studioClasses.Select(r => r.StudioRoomId).Distinct().ToList();
            foreach (var studioClass in studioClasses.Where(c=> !c.IsMultiRoom))
            {
                emptyClasses.AddRange(from room in rooms.Where(r => r != studioClass.StudioRoomId)
                                      where !studioClasses.Exists(c => c.StudioRoomId == room && ((c.DailySlotId == studioClass.DailySlotId && studioClass.DailySlotId != -1) || c.Time == studioClass.Time))
                                      select CreateEmptyClass(studioClass.Date, studioClass.Time, room, studioClass.DailySlotId ?? -1, studioClass.Duration));
            }
            var distinctEmpty = emptyClasses
  .GroupBy(p => new { p.StudioRoomId, p.Time })
  .Select(g => g.First())
  .ToList();
            //foreach (var item in emptyClasses)      
            //{
            //    if (studioClasses.Any(c => c.StudioRoomId == item.StudioRoomId && ((c.DailySlotId == item.DailySlotId && item.DailySlotId != -1) || c.Time == item.Time)))
            //        emptyClasses.Remove(item);
            //}
            return distinctEmpty;
        }

        private static void AddEmptySlots(List<StudioClassModel> studioClasses, DateTime date, int StudioId)
        {
            var rooms = GetStudioRooms(0, StudioId);
            var DailySlots = ClassRepo.GetClassTimeSlots(0, StudioId);
            var slots = studioClasses.Select(x => x.DailySlotId).Distinct().ToList();

            foreach (var slot in slots.Where(x=> x >= 0))
            {
                foreach (var room in rooms.Where(room => !studioClasses.Any(c => c.DailySlotId == slot && (c.StudioRoomId == room.Id || c.IsMultiRoom))))
                {
                    studioClasses.Add(CreateEmptySlot(DailySlots.FirstOrDefault(x => x.Id == slot), room, date));
                }
            }
        }

        private static StudioClassModel CreateEmptySlot(ClassDailySlot slot, StudioRoomModel room , DateTime date)
        {
            var empty = new StudioClassModel
            {
                Id = -1,
                Date = date.Add(slot.StartTime),
                DailySlotId = slot.Id,
                Duration = slot.Duration,
                StudioRoomId = room.Id,
                Name = "אין אימון",
                ClassTypeId = 999,
                Time = date.Add(slot.StartTime)
            };
            return empty;
        }

        private static CalendarStudioClassModel CreateEmptyClass(DateTime date, DateTime time, int roomId,  int dailySlotId, int duration)
        {
            var empty = new CalendarStudioClassModel
            {
                Id = -1,
                Date = date,
                DailySlotId = dailySlotId,
                Duration = duration,
                StudioRoomId = roomId,
                Name = "אין אימון",
                ClassTypeId = 999,
                Time = time
            };
            return empty;
        }

        //private static StudioClassModel ConvertToModel(StudioClassModel studioclass,string userId = null, Gender gender = Gender.מעורב)
        //{
        //    var classmodel = Mapper.Map<StudioClassModel>(studioclass);

        //    //if (studioclass.IsForFemale && studioclass.IsForMale) classmodel.Gender = Gender.מעורב;
        //    //else if (studioclass.IsForMale) classmodel.Gender = Gender.זכר;
        //    //else classmodel.Gender = Gender.נקבה;

        //    classmodel.Time = studioclass.Date;
        //    classmodel.IsUserGender = classmodel.Gender == Gender.מעורב || classmodel.Gender == gender;
        //    //classmodel.Picture = studioclass.ClassType == null ? string.Empty : studioclass.ClassType.Picture;
        //    classmodel.IsUserEnrolled = userId != null && ClassBLL.IsUserEnrolled(classmodel.Id, userId);

        //    return classmodel;

        //}

        public static object GetClassesForCalanderByRoom(int roomid, string userId, Gender gender, AgeGroup ageGroup)
        {
            var studioClasses = StudioRepo.GetClassesForCalanderByRoom(roomid);
            var enrollments = ClassBLL.GetEnrollmentsByUserDate(userId, DateTime.MinValue.Date);
            var waitingList = ClassBLL.GetWaitingListByUserDate(userId, DateTime.MinValue.Date);
            var placements = ClassBLL.GetClassAvailablePlacementsForDate(DateTime.MinValue.Date, 0);
            var instructors = UserBll.GetInstructorList(0);
            //var classes = new List<StudioClassModel>();

            //if (studioClasses.Any()) studioClasses.ForEach(x => classes.Add(ConvertToModel(x, userId, gender)));
            //return classes;
            studioClasses.ForEach(x => SetExtraProperties(x, userId, gender, ageGroup, enrollments, waitingList, placements, instructors));
            return studioClasses;
        }

        private static Class ConvertToEntity(StudioClassModel classModel, string userid)
        {
            classModel.ClassAvailablePlacements.ForEach(x => x.IsDeleted = !x.ToCreate);
            classModel.MaxExtraParticipants = classModel.ClassAvailablePlacements.Count(x => (x.StudioPlacementId == 999 || x.StudioPlacementId == 1003 || x.StudioPlacementId == 1007) && !x.IsDeleted);
            var newClass = Mapper.Map<Class>(classModel);
            newClass.IsForMale = classModel.Gender != Gender.נקבה;
            newClass.IsForFemale = classModel.Gender != Gender.זכר;
            //if (classModel.Id == 0 && classModel.UsePlacements) newClass.MaxParticipants = classModel.MaxParticipants - classModel.MaxExtraParticipants;
            //newClass.Participants = classModel.Participants;
            if (classModel.InstructorIds.Length == 0)
                newClass.Class_Instructors.Add(new Class_Instructors {ClassId = newClass.Id, InstructorId = userid});
            else
            {
                foreach (var instructor in classModel.InstructorIds)
                {
                    newClass.Class_Instructors.Add(new Class_Instructors
                    {
                        ClassId = newClass.Id,
                        InstructorId = instructor
                    }); 
                }
            }

            //if (classModel.ClassAvailablePlacements.Any())
            //{
            //    foreach (var plc in classModel.ClassAvailablePlacements)
            //    {
            //        newClass.ClassAvailablePlacements.Add(new ClassAvailablePlacement { Id });
            //    }
            //}
            
            if (classModel.ClassTypeDetailsId == 0) newClass.ClassTypeDetailsId = null;
            //else 
            return newClass;

        }

        public static bool CopyWeeklyCalander(int currentweekno, int StudioId, bool AutoPublish)
        {
            var startdate = Utilities.Utils.FirstDateOfWeek(DateTime.Now.Year, currentweekno);
            return StudioRepo.CopyWeeklyCalander(startdate, StudioId, AutoPublish);
        }

        public static bool PublishWeeklyCalander(int currentweekno, int StudioId)
        {
            var startdate = Utilities.Utils.FirstDateOfWeek(DateTime.Now.Year, currentweekno);
            return StudioRepo.PublishWeeklyCalander(startdate, StudioId);
        }

        public static List<object> GetClasses(DateTime date, int studioId)
        {
            var result = new List<object>();
            var classes = StudioRepo.GetDailyClassesForCalander(date, studioId).OrderBy(x=>x.StudioRoomId).ThenBy(x=>x.Date);
            //var classmodels = Mapper.Map<List<StudioClassModel>>(classes);
            foreach (var cl in classes)
            {
                result.Add(new { value = cl.StudioRoomName + ": " + cl.Name + " @ " + cl.Date.ToShortTimeString(), id = cl.Id });
            }

            return result;
        }

        //public static int GetDefaultStudioByComapny(int CompanyId)
        //{
        //    //var studio = App.CurrentCompany.Studios.Count == 1 ? App.CurrentCompany.Studios.First() : ;
        //    var studio = App.Companies.FirstOrDefault(c=>c.Id == CompanyId).Studios.OrderBy(x=>x.Id).FirstOrDefault();
        //    return studio?.Id ?? 0;
        //    //return StudioRepo.GetDefaultStudioByComapny(companyId);
        //}
    }
}
 