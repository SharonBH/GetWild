using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;


namespace InShapeModels
{
    public class StudioClassModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public DateTime Time { get; set; }

        public int ClassTypeId { get; set; }

        public int StudioRoomId { get; set; }

        public string StudioRoomName { get; set; }

        public string ClassTypeName { get; set; }

        public int Participants { get; set; }

        public int AllParticipants { get { return Participants + ExtraParticipants; } }

        public double AvgParticipants1M { get; set; }

        public double AvgParticipants3M { get; set; }

        public int MaxParticipants { get; set; }

        public int Duration { get; set; }

        public bool IsFull { get; set; }

        public Gender Gender { get; set; }

        public bool IsUserEnrolled { get; set; }

        public bool IsUserGender { get; set; }

        public bool IsUserAge { get; set; }

        public bool IsStarted { get { return DateTime.UtcNow.AddMinutes(15).ToLocal() > Date; } }

        public string Picture { get; set; }

        public int? DailySlotId { get; set; }

        public int SpacesLeft {
            get { return MaxParticipants + MaxExtraParticipants - Participants - ExtraParticipants; }  //+ MaxExtraParticipants
        }

        public string BGColor { get; set; }

        public List<ClassEnrollmentModel> Enrollments { get; set; }

        //public List<WaitListEnrollment> WaitListEnrollments { get; set; }

        public WaitListEnrollment UserWaitingList { get; set; }
        public bool IsUserInWaitingList { get { return UserWaitingList != null; } }

        public int WaitingList { get; set; }

        public string ShortURL { get; set; }

        public double Rating { get; set; }

        public List<InstructorDetailsModel> Instructors { get; set; }

        public string ClassInstructors
        {
            get { return Instructors.Count == 0 ? "" : "(" + string.Join(",", Instructors.Select(x => x.FullName)) + ")"; }
        }

        public string[] InstructorIds { get; set; }

        //public List<StudioPlacementModel> Placements { get; set; }


        //public int[] PlacementIds { get; set; }

        public List<AvailablePlacementsModel> ClassAvailablePlacements { get; set; }


        //public void SetUserGender(Gender userGender)
        //{
        //    IsUserGender = Gender == Gender.מעורב || Gender == userGender;
        //}

        public void SetUserEnrolled(bool enrolled, string placementName)
        {
            IsUserEnrolled = enrolled;
            SelectedPlacement = placementName;
        }

        public void SetUserinWaitList(WaitListEnrollment inWaitList)
        {
            UserWaitingList = inWaitList;
        }

        public bool IsMultiRoom { get; set; }

        public int? ClassTypeDetailsId { get; set; }

        public string ClassTypeDetailsName { get; set; }
        public bool Published { get; set; }

        public string ClassTypeDescription { get; set; }

        //public List<ClassPlacementModel> ClassPlacements { get; set; }

        public string SelectedPlacement { get; set; }

        public int ExtraParticipants { get; set; }

        public int MaxExtraParticipants { get; set; }

        public bool UsePlacements { get; set; }

        public int MinAge { get; set; }

        public AgeGroup AgeGroup { get; set; }

    }

    public class CalendarStudioClassModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public DateTime Time { get; set; }

        public int ClassTypeId { get; set; }

        public int StudioRoomId { get; set; }

        public string StudioRoomName { get; set; }

        public string ClassTypeName { get; set; }

        public int Participants { get; set; }

        public int AllParticipants { get { return Participants + ExtraParticipants; } }

        public double AvgParticipants1M { get; set; }

        public double AvgParticipants3M { get; set; }

        public int MaxParticipants { get; set; }

        public int Duration { get; set; }

        public bool IsFull { get; set; }

        public bool IsForFemale { get; set; }

        public bool IsForMale { get; set; }

        public Gender Gender { get
            {
                return IsForFemale && IsForMale ? Gender.מעורב : IsForFemale ? Gender.נקבה : Gender.זכר;
            }
        }

        public bool IsUserEnrolled { get; set; }

        public bool IsUserGender { get; set; }

        public bool IsUserAge { get; set; }

        public bool IsStarted { get { return DateTime.UtcNow.AddMinutes(15).ToLocal() > Date; } }

        public string Picture { get; set; }

        public int? DailySlotId { get; set; }

        public int SpacesLeft
        {
            get { return MaxParticipants + MaxExtraParticipants - Participants - ExtraParticipants; }  //+ MaxExtraParticipants
        }

        public string BGColor { get; set; }

        //public List<ClassEnrollmentModel> Enrollments { get; set; }

        //public List<WaitListEnrollment> WaitListEnrollments { get; set; }

        public WaitListEnrollment UserWaitingList { get; set; }
        public bool IsUserInWaitingList { get { return UserWaitingList != null; } }

        public int WaitingList { get; set; }

        public string ShortURL { get; set; }

        public double Rating { get; set; }

        public List<InstructorDetailsModel> Instructors { get; set; }

        public string ClassInstructors
        {
            get { return Instructors.Count == 0 ? "" : "(" + string.Join(",", Instructors.Select(x => x.FullName)) + ")"; }
        }

        public string InstructorIds { get; set; }

        //public List<StudioPlacementModel> Placements { get; set; }


        //public int[] PlacementIds { get; set; }

        public List<CalendarAvailablePlacementsModel> ClassAvailablePlacements { get; set; }


        //public void SetUserGender(Gender userGender)
        //{
        //    IsUserGender = Gender == Gender.מעורב || Gender == userGender;
        //}

        public void SetUserEnrolled(bool enrolled, string placementName)
        {
            IsUserEnrolled = enrolled;
            SelectedPlacement = placementName;
        }

        public void SetUserinWaitList(WaitListEnrollment inWaitList)
        {
            UserWaitingList = inWaitList;
        }

        public bool IsMultiRoom { get; set; }

        public int? ClassTypeDetailsId { get; set; }

        public string ClassTypeDetailsName { get; set; }
        public bool Published { get; set; }

        public string ClassTypeDescription { get; set; }

        //public List<ClassPlacementModel> ClassPlacements { get; set; }

        public string SelectedPlacement { get; set; }

        public int ExtraParticipants { get; set; }

        public int MaxExtraParticipants { get; set; }

        public bool UsePlacements { get; set; }

        public int MinAge { get; set; }

        public AgeGroup AgeGroup { get; set; }

    }

    public class StudioClassReportModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        //public string Description { get; set; }

        public DateTime Date { get; set; }

        public DateTime Time { get; set; }

        public int ClassTypeId { get; set; }

        public int StudioRoomId { get; set; }

        //public string StudioRoomName { get; set; }

        //public string ClassTypeName { get; set; }

        public int Participants { get; set; }

        public int AllParticipants { get { return Participants + ExtraParticipants; } }

        public double AvgParticipants1M { get; set; }

        public double AvgParticipants3M { get; set; }

        public int MaxParticipants { get; set; }

        public int Duration { get; set; }

        public bool IsFull { get; set; }

        
        public bool IsStarted { get { return DateTime.UtcNow.AddMinutes(15).ToLocal() > Date; } }

        
        public int? DailySlotId { get; set; }

        public int SpacesLeft
        {
            get { return MaxParticipants + MaxExtraParticipants - Participants - ExtraParticipants; }  //+ MaxExtraParticipants
        }

        public string BGColor { get; set; }

        public int WaitingList { get; set; }

        public string ShortURL { get; set; }

        public double Rating { get; set; }

        public List<InstructorDetailsModel> Instructors { get; set; }

        public string ClassInstructors
        {
            get { return Instructors.Count == 0 ? "" : "(" + string.Join(",", Instructors.Select(x => x.FullName)) + ")"; }
        }

        public string[] InstructorIds { get; set; }

        //public List<StudioPlacementModel> Placements { get; set; }


        //public int[] PlacementIds { get; set; }


        //public void SetUserGender(Gender userGender)
        //{
        //    IsUserGender = Gender == Gender.מעורב || Gender == userGender;
        //}

       

        public bool IsMultiRoom { get; set; }

        public int? ClassTypeDetailsId { get; set; }

        //public string ClassTypeDetailsName { get; set; }
        public bool Published { get; set; }

        //public string ClassTypeDescription { get; set; }

        //public List<ClassPlacementModel> ClassPlacements { get; set; }

        public string SelectedPlacement { get; set; }

        public int ExtraParticipants { get; set; }

        public int MaxExtraParticipants { get; set; }

        public bool UsePlacements { get; set; }

        
    }

    public class ClassesList
    {
        public IEnumerable<StudioClassModel> Classes { get; set; }

        public CompanyConfiguration CurrentCompany { get; set; }
    }

    public class ClassTypeModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Picture { get; set; }

        public string BGColor { get; set; }

        public int StudioId { get; set; }
        
    }

    public class ClassTypeDetailsModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int ClassTypeId { get; set; }

        public string ClassTypeName { get; set; }

        public List<ClassTypeModel> ClassTypes { get; set; }

        public int StudioId { get; set; }

        public DateTime? LastClass { get; set; }

        public DateTime? NextClass { get; set; }

        public string Picture { get; set; }

        public int Usage1Month { get; set; }

        public int Usage3Month { get; set; }

    }

    public class ClassEnrollResult
    {
        public bool Result { get; set; }

        public string Error { get; set; }

        public int RoomId { get; set; }

        public DateTime Date { get; set; }

        public string Message { get; set; }

        public string SubMessage { get; set; }

        public bool StartWaitingListProcess { get; set; }
    }

    public class APIResult
    {
        public bool Result { get; set; }

        public string Error { get; set; }

        public DateTime Date { get { return DateTime.UtcNow.ToLocal(); } }

    }

    public class DailySlotModel
    {
        public int Id { get; set; }
        public System.TimeSpan StartTime { get; set; }
        public System.TimeSpan EndTime { get; set; }
        public string Description { get; set; }

        public int Duration { get { return (int)(EndTime - StartTime).TotalMinutes; } }

        public string Name { get { return EndTime != StartTime ? EndTime.ToString(@"hh\:mm") + " - " + StartTime.ToString(@"hh\:mm") : "זמן אחר"; } }

        public int Participants { get; set; }

        public int DailyClasses { get; set; }

        public double AVGParticipants { get; set; }


    }


    public class DailyStatsModel
    {
        public DateTime Date { get; set; }
        public int TotalParticipants { get; set; }

        public int TotalClasses { get; set; }

        public int MissedParticipants { get; set; }

        public int Activated { get; set; }
        
        public int TrailParticipants { get; set; }

        public int Comments { get; set; }

        public int LateCancel { get; set; }

    }

}
