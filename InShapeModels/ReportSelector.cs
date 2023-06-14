using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace InShapeModels
{
    public class ReportByClassModel
    {
        public List<StudioClassModel> RoomClasses { get; set; }

        public int SelectedClass { get; set; }

        public List<StudioRoomModel> StudioRooms { get; set; }
        
        public int SelectedRoom { get; set; }
        
    }

    public class ClassReportUsers
    {
        public string Name { get; set; }

        public int? Participate { get; set; }
    }

    public class UserReportModel
    {
        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get { return FirstName + " " + LastName; } }

        public string PhoneNumber { get; set; }

        public bool Active { get; set; }

        public DateTime? LastClassDate { get; set; }

        public DateTime? NextClassDate { get; set; }

        public string NextClassType { get; set; }

        public int DaysInactive
        {
            get { return LastClassDate.HasValue ? (DateTime.UtcNow.ToLocal().Date - LastClassDate.Value).Days : 0; }
        }

        public bool Processed { get; set; }

        public DateTime? ProcessDate { get; set; }

        public string Note { get; set; }
    }

    public class WeeklyReportModel
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int WeekNo { get; set; }
        public List<DailySlotModel> DailySlots { get; set; }

        public List<StudioRoomModel> StudioRooms { get; set; }

        public List<DailyStatsModel> WeeklyDates { get; set; }

        public List<ClassTypeModel> ClassTypes { get; set; }
        
        public List<StudioClassReportModel> WeeklyReportDetails { get; set; }

        public List<InstructorDetailsModel> Instructors { get; set; }

        public string GetBGClass(DayOfWeek day)
        {
            if (day == DayOfWeek.Sunday || day == DayOfWeek.Tuesday ||
                day == DayOfWeek.Thursday || day == DayOfWeek.Saturday)
            {
                return "col-odd";
            }
            return "col-even";
        }

        public string GetBGInstructorClass(string[] InstructorIds)
        {
            if (!CurrentCompany.UseInstructors) return "";
            var color = Instructors.Where(x => InstructorIds.Contains(x.InstructorId)).Select(i=> "#"+i.ColorCode.Trim());
            return color.Count() > 1 ? "linear-gradient(" + string.Join(",", color)+")": color.FirstOrDefault();
        }

        public CompanyConfiguration CurrentCompany { get; set; }
    }

    public class InstructorReport
    {
        public DateTime ReportDate { get; set; }

        public string ReportMonth { get; set; }

        public List<InstructorReportModel> InstructorReportList { get; set; }
    }

    public class InstructorReportModel
    {
        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get { return FirstName + " " + LastName; } }

        public DateTime JoinDate { get; set; }

        public int StudioId { get; set; }

        public int CompanyId { get; set; }

        public double Rate { get; set; }
        public double DailyRate { get; set; }
        public string ColorCode { get; set; }

        public string ProfileIMG { get; set; }

        public string ProfileIMGPath { get { return string.IsNullOrEmpty(ProfileIMG) ? @"/images/Members/no-profile.jpg" : ProfileIMG; } }

        public DateTime DateUpdated { get; set; }

        public int Classes { get; set; }

        public int CurrentNumClasses { get; set; }

        public int CurrentDays { get; set; }

        public double Expenses { get { return CurrentDays * DailyRate; } }

        public double CurrentEarnings { get { return Rate * CurrentNumClasses + Expenses; } }

        public DateTime CurrentMonthDate { get; set; }

        public string CurrentMonth
        {
            get { return CurrentMonthDate.ToShortDateString(); }
        }

        public double RateSalary { get; set; }

        public double AmountSalary { get; set; }

        public DateTime? DateSalary { get; set; }

        public DateTime? LastClassDate { get; set; }

        public DateTime? NextClassDate { get; set; }

        public string LastClassType { get; set; }

        public string NextClassType { get; set; }

        public bool Confirmed { get; set; }

        public DateTime? DateConfirmed { get; set; }

        public string UserConfirmed { get; set; }

        public string Note { get; set; }
        



    }


    public class ExpensesReport
    {
        public DateTime ReportDate { get; set; }

        public string ReportMonth { get; set; }

        public List<ExpensesModel> ExpensesReportList { get; set; }
    }

    public class ExpensesModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public int StudioId { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class UsersGraphModel
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public List<DateUsers> UsersPerDay { get; set; }

        public int InActiveUsers { get; set; }

        public int ActiveUsers { get; set; }

        public int TotalUsers { get { return ActiveUsers + InActiveUsers; } }

    }

    public class DateUsers
    {
        public DateTime Date { get; set; }

        public int ActiveUsers { get; set; }
    }


}
