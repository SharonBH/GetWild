using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace InShapeModels.APIModels
{
    public class ClassModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

       public DateTime Time { get; set; }


        //public int DailySlotId { get; set; }

        public int ClassTypeId { get; set; }

        public int StudioRoomId { get; set; }

        public int Participants { get; set; }

        public int MaxParticipants { get; set; }


        public int Duration { get; set; }

        public bool IsFull { get; set; }

        public List<InstructorDetailsModel> Instructors { get; set; }

        public string ClassInstructors
        {
            get { return Instructors.Count == 0 ? "" : string.Join(",", Instructors.Select(x => x.FirstName)); }
        }

        //public string[] InstructorIds { get; set; }

        public bool IsMultiRoom { get; set; }

        public bool IsUserEnrolled { get; set; }

        public bool IsUserGender { get; set; }

        public string Picture { get; set; }

        //public bool CanCancel
        //{
        //    get
        //    {
        //        return Date.AddMinutes(App.CurrentCompany.CancellationThresholdMins * -1) >
        //               DateTime.UtcNow.ToLocal();
        //    }
        //}

        public int WaitingList { get; set; }

        public bool CanEnroll
        {
            get { return !IsFull && Date > DateTime.UtcNow.ToLocal() && IsUserGender && SpaceLeft > 0; }
        }

        public WaitListEnrollment UserWaitingList { get; set; }

        //public bool CanEnrollFromWaitList
        //{
        //    get
        //    {
        //        return App.CurrentCompany.WaitingListEnabled && Date > DateTime.UtcNow.ToLocal() && IsUserGender && SpaceLeft > 0
        //          && (UserWaitingList != null && UserWaitingList.CanEnroll);
        //    }
        //}

        public bool IsUserInWaitingList { get; set; }

        //public bool CanJoinWaitingList
        //{
        //    get
        //    {
        //        return App.CurrentCompany.WaitingListEnabled && IsFull
        //          && Date > DateTime.UtcNow.ToLocal().AddMinutes(App.CurrentCompany.CancellationThresholdMins)
        //          && !IsUserInWaitingList;
        //    }
        //    //return false; }
        //    //IsFull && Date > DateTime.UtcNow.ToLocal().AddHours(2) && !IsUserInWaitingList; } //todo: move to config
        //}

        public int SpaceLeft
        {
            get { return MaxParticipants - Participants; }
        }

        //public string SpaceLeftMSG
        //{
        //    get
        //    {
        //        if (App.CurrentCompany.SpacesLeftToShow > 0)
        //        {
        //            if (App.CurrentCompany.SpacesLeftToShow >= SpaceLeft)
        //            {
        //                return $"נותרו עוד {SpaceLeft} מקומות.";
        //            }
        //        }
        //        else if (App.CurrentCompany.SpacesLeftToShow == 0)
        //        {
        //            return $"נותרו עוד {SpaceLeft} מקומות.";
        //        }
        //        return string.Empty;
        //    }
        //}

        public Gender Gender { get; set; }

        public string BGColor { get; set; }

        public string BGColor2 => IsUserEnrolled ? "#008000" : "#" + BGColor;

        public string ShortURL { get; set; }

        public string StudioRoomName { get; set; }

        public string ClassTypeName { get; set; }
    }
}
