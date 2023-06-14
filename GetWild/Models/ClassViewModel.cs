using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using InShapeModels;
using Newtonsoft.Json;
using Utilities;

namespace GetWild.Models
{
    public class ClassViewModel
    {
        public int? Id { get; set; }

        //[Required]
        [Display(Name = "שם האימון")]
        public string Name { get; set; }

        [Display(Name = "תיאור")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "תאריך")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Date { get; set; }

        [JsonIgnore]
        //[Required]
        [Display(Name = "שעה")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Time { get; set; }

        [JsonIgnore]
        [Required]
        [Display(Name = "שעה")]
        public int? DailySlotId { get; set; }

        [Display(Name = "סוג אימון")]
        public int ClassTypeId { get; set; }

        [Display(Name = "מערך אימון")]
        public int ClassTypeDetailsId { get; set; }

        [JsonIgnore]
        [Display(Name = "חדר סטודיו")]
        public int StudioRoomId { get; set; }

        public int Participants { get; set; }

        public int ExtraParticipants { get; set; }
        

        [Required]
        [Display(Name = "מקסימום משתתפים")]
        [Range(1, int.MaxValue, ErrorMessage = "מספר משתתפים צריך להיות גדול מ 0")]
        public int MaxParticipants { get; set; }


        public int MaxExtraParticipants { get; set; }

        //[Required]
        [Display(Name = "זמן האימון")]
        [DataType(DataType.Duration)]
        public int? Duration { get; set; }

        public bool IsFull { get; set; }
        [JsonIgnore]
        public List<ClassTypeModel> ClassTypes;
        [JsonIgnore]
        public List<StudioRoomModel> StudioRooms;
        [JsonIgnore]
        public List<DailySlotModel> TimeSlots;
        [JsonIgnore]
        public List<InstructorDetailsModel> Instructors { get; set; }

        public string ClassInstructors
        {
            get { return Instructors.Count == 0 ? "" : string.Join(",",Instructors.Select(x=>x.FirstName)); }
        }
        [JsonIgnore]
        [Display(Name = "מאמן")]
        public string[] InstructorIds { get; set; }

        public List<StudioPlacementModel> Placements { get; set; }

        //[UIHint("ClassPlacements")]
        //public List<ClassPlacementModel> ClassPlacements { get; set; }


        [JsonIgnore]
        [Display(Name = "סוגי מקום")]
        public int[] PlacementIds { get; set; }


        [Display(Name = "אימון משולב")]
        public bool IsMultiRoom { get; set; }

        [UIHint("ClassPlacements")]
        public List<AvailablePlacementsModel> ClassAvailablePlacements { get; set; }

        [UIHint("ClassPlacements")]
        public List<AvailablePlacementsModel> KangooClassAvailablePlacements { get; set; }

        [UIHint("ClassPlacements")]
        public List<AvailablePlacementsModel> NumbersClassAvailablePlacements { get; set; }

        public bool AutoShowPlacements { get { return ClassAvailablePlacements == null ? false : ClassAvailablePlacements.Any(x=>x.Id > 0); } }

        public bool AutoShowKangooPlacements { get { return KangooClassAvailablePlacements == null ? false : KangooClassAvailablePlacements.Any(x => x.Id > 0); } }

        public bool AutoShowNumbersPlacements { get { return NumbersClassAvailablePlacements == null ? false : NumbersClassAvailablePlacements.Any(x => x.Id > 0); } }

        public string SelectedPlacement { get; set; }

        public int SelectedPlacementId { get; set; }

        [Display(Name = "אימון עם מיקום?")] 
        public bool UsePlacements { get; set; }

        [Display(Name = "קנגו?")]
        public bool UseKangoo { get; set; }

        [Display(Name = "מספרים?")]
        public bool UseNumbers { get; set; }

        //public ClassTypeModel ClassType { get; set; }

        //public StudioRoomModel StudioRoom { get; set; }
        //public string TimeofDay
        //{
        //    get
        //    {
        //        if (!Time.HasValue) return "NA";
        //        if (Time.Value.Hour <= 12) return "Morning";
        //        if (Time.Value.Hour < 18) return "MidDay";
        //        return Time.Value.Hour <= 23 ? "Evening" : "NA";
        //    }
        //}
        [JsonIgnore]
        public int DayNo
        {
            get
            {
                if (!Date.HasValue) return 0;
               return (int)Date.Value.DayOfWeek;
            }
        }

        public bool IsUserEnrolled { get; set; }

        public bool IsUserGender { get; set; }

        public bool IsUserAge { get; set; }

        public string Picture { get; set; }

        bool GetCanCancel(int CancellationThresholdMins)
        {
            return Date.HasValue && Date.Value.AddMinutes(CancellationThresholdMins * -1) > DateTime.UtcNow.ToLocal();
        }

        public bool CanCancel { get { return CurrentCompany != null ? GetCanCancel(CurrentCompany.CancellationThresholdMins): false; } }

        public bool CanLateCancel
        {
            get
            {
                if (CurrentCompany != null && CurrentCompany.LateCancelation >= 0)
                { return Date.HasValue && Date.Value.AddMinutes(CurrentCompany.LateCancelation * -1) > DateTime.UtcNow.ToLocal(); }
                else { return false; }
            }
        }


        [JsonIgnore]
        public int WaitingList { get; set; }

        public bool CanEnroll
        {
            get { return !IsFull && Date > DateTime.UtcNow.ToLocal() && IsUserGender && IsUserAge; }
        }

        public string CannotEnrollMSG { get { return !IsUserAge ? "אינך בקבוצת גיל מתאימה" : !IsUserGender ? "אימון מתאים למין אחר" : "";  } }

        [JsonIgnore]
        public WaitListEnrollment UserWaitingList { get; set; }

        bool GetCanEnrollFromWaitList (bool WaitingListEnabled, int WaitingListSpaces)
        {
            return WaitingListEnabled && (WaitingListSpaces == 0 || WaitingListSpaces < WaitingList)
                    && Date > DateTime.UtcNow.ToLocal() && IsUserGender && SpaceLeft > 0 
                    && (UserWaitingList != null && UserWaitingList.CanEnroll);
        }

        public bool CanEnrollFromWaitList { get { return CurrentCompany != null ? GetCanEnrollFromWaitList(CurrentCompany.WaitingListEnabled, CurrentCompany.WaitingListSpaces): false; } }

        public bool IsUserInWaitingList { get; set; }

        bool GetCanJoinWaitingList(bool WaitingListEnabled)
        {

            return WaitingListEnabled && IsFull
                   && Date > DateTime.UtcNow.ToLocal() //.AddMinutes(App.CurrentCompany.CancellationThresholdMins)
                   && !IsUserInWaitingList && !IsUserEnrolled && IsUserGender && IsUserAge;

            //return false; }
            //IsFull && Date > DateTime.UtcNow.ToLocal().AddHours(2) && !IsUserInWaitingList; } //todo: move to config
        }

        public bool CanJoinWaitingList { get { return CurrentCompany != null ? GetCanJoinWaitingList(CurrentCompany.WaitingListEnabled): false; } }

        [JsonIgnore]
        public int SpaceLeft
        {
            get { return MaxParticipants - Participants; } //not adding extras to user side
        }

        public string SpaceLeftHTML { get
            {
                return CurrentCompany != null ? GetSpaceLeftHTML(CurrentCompany.WaitingListEnabled, CurrentCompany.SpacesLeftToShow ?? 0) :  string.Empty;
            }

        }

        string GetSpaceLeftHTML(bool WaitingListEnabled, int SpacesLeftToShow)
        {

            if (Id <= 0 || Date < DateTime.UtcNow.ToLocal()) return "";
            string text = string.Empty;
            string icon = string.Empty;
            string label = "primary";
            var html = $"<span class=\"label label-[[label]]\">[[text]] <i class=\"glyphicon glyphicon-[[icon]]\"></i>[[extra]]</span>";
            if (IsFull)
            {
                text = WaitingListEnabled ? "רשימת המתנה ("+WaitingList+")" : "האימון מלא";
                icon = WaitingListEnabled ? "time" : "time";
                label = "warning";
            }
            else if ((SpacesLeftToShow > 0 && SpacesLeftToShow >= SpaceLeft) || SpacesLeftToShow == 0)
            {
                text = $"{(SpaceLeft - WaitingList).ToString()} מקומות";
                icon = "plus";
            }
            string extra = string.Empty;
            if (ClassAvailablePlacements.Count(x => !x.IsDeleted && !x.IsInUse && (x.StudioPlacementId == 999 || x.StudioPlacementId == 1003 || x.StudioPlacementId == 1007)) > 0)
            {
                var extraleft = ClassAvailablePlacements
                    .Count(x => !x.IsDeleted && !x.IsInUse && (x.StudioPlacementId == 999 || x.StudioPlacementId == 1003 || x.StudioPlacementId == 1007)); //- ExtraParticipants;
                if (extraleft > 0) extra = $"<BR/>({extraleft} בדאבל)";
            }
            return html.Replace("[[text]]", text).Replace("[[icon]]", icon).Replace("[[label]]", label).Replace("[[extra]]", extra);

        }

        public CompanyConfiguration CurrentCompany { get; set; }

        public string SpaceLeftMSG { get
            {
                return CurrentCompany != null ? GetSpaceLeftMSG(CurrentCompany.WaitingListEnabled, CurrentCompany.SpacesLeftToShow??0): string.Empty;
            }
        }

        string GetSpaceLeftMSG(bool WaitingListEnabled, int SpacesLeftToShow)
        {
            if (Id <= 0) return "";
            if (IsFull)
            {
                return WaitingListEnabled ? "רשימת המתנה" : "האימון מלא";
            }
            string extra = string.Empty;
            if (ClassAvailablePlacements.Count(x => !x.IsDeleted && !x.IsInUse && (x.StudioPlacementId == 999 || x.StudioPlacementId == 1003 || x.StudioPlacementId == 1007)) > 0)
            {
                var extraleft = ClassAvailablePlacements.Count(x => !x.IsDeleted && !x.IsInUse && (x.StudioPlacementId == 999 || x.StudioPlacementId == 1003 || x.StudioPlacementId == 1007)) - ExtraParticipants;
                if (extraleft > 0) extra = $"(ועוד {extraleft} בדאבל ריצפה)";
            }

            if (SpacesLeftToShow > 0)
            {
                if (SpacesLeftToShow >= SpaceLeft)
                {
                    return $"נותרו עוד {SpaceLeft - WaitingList} מקומות. {extra}";
                }
            }
            else if (SpacesLeftToShow == 0)
            {
                return $"נותרו עוד {SpaceLeft - WaitingList} מקומות. {extra}";
            }
            return string.Empty;

        }


        [Display(Name = "מין")]
        public Gender Gender { get; set; }
        [JsonIgnore]
        public string CssClass { get { return "Class" + ClassTypeId; } }
        [JsonIgnore]
        public string BGColor { get; set; }

        public string BGColor2 => IsUserEnrolled || IsUserInWaitingList ? "#008000" : string.IsNullOrEmpty(BGColor) ? "#393939" : "#" + BGColor;
        [JsonIgnore]
        public string SourcePage { get; set; }

        public string ShortURL { get; set; }

        public string StudioRoomName { get; set; }

        public string ClassTypeName { get; set; }

        [JsonIgnore]
        public List<ClassTypeDetailsModel> ClassTypesDetails;

        public string ClassTypeDetailsName { get; set; }

        public bool Published { get; set; }

        public string ClassTypeDescription { get; set; }

        //public string ClassTypeDescriptionBreaks { get { return ClassTypeDescription == null ? "" : ClassTypeDescription.Replace("%%", "<br>"); } }

        //public string DescriptionBreaks { get { return Description == null ? "" : Description.Replace("%%", "<br>"); } }

        public string GetGenderImageURL { get { return Gender == Gender.מעורב ? string.Empty : $"/images/Gender_{Gender.ToString()}.png"; } }

        public string GetAgeGroupImageURL { get { return AgeGroup == AgeGroup.מבוגרים ? string.Empty : $"/images/age_{AgeGroup.ToString()}.png"; } }

        //public int MinAge { get; set; }

        [Display(Name = "קבוצת גיל")]
        public AgeGroup AgeGroup { get; set; }
    }
}