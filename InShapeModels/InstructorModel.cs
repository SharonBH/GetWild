using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InShapeModels
{
    //public class InstructorModel
    //{
    //    public string UserId { get; set; }

    //    public string FirstName { get; set; }

    //    public string LastName { get; set; }

    //    public string FullName { get { return FirstName + " " + LastName; } }

    //    public Gender Gender { get; set; }

    //    public DateTime? JoinDate { get; set; }

    //    public string ProfileIMG { get; set; }

    //    public string ProfileIMGPath { get { return string.IsNullOrEmpty(ProfileIMG) ? @"/images/Members/no-profile.jpg" : ProfileIMG; } }

    //    public string PhoneNumber { get; set; }

    //    public int StudioId { get; set; }

    //    public int CompanyId { get; set; }

    //    public string Email { get; set; }

    //    public DateTime? DOB { get; set; }

    //    public string Address { get; set; }

    //    public bool ReceiveSMS { get; set; }

    //    //public InstructorDetailsModel DetailsModel { get; set; }
    //}

    public class InstructorDetailsModel: InShapeUser
    {
        public int Id { get; set; }
        public string InstructorId { get; set; }
        public double Rate { get; set; }
        public double DailyRate { get; set; }
        public string ColorCode { get; set; }

        public bool IsDeleted { get; set; }

        public string ProfileIMGPath { get { return string.IsNullOrEmpty(ProfileIMG) ? @"/images/Members/no-profile.jpg" : ProfileIMG; } }

        public DateTime DateUpdated { get; set; }

        public int CurrentNumClasses { get; set; }

        public double CurrentEarnings { get; set; }

        public DateTime? LastClassDate { get; set; }

        public DateTime? NextClassDate { get; set; }

        public string LastClassType { get; set; }

        public string NextClassType { get; set; }

        public int CurrentDays { get; set; }

        public double Expenses { get { return CurrentDays * DailyRate; } }

        //public InstructorModel InstructorModel { get; set; }
    }

    public class InstructorReminderModel
    {
        public InShapeUser Instructor { get; set; }
        public List<StudioClassModel> Classes { get; set; }

        public string ClassesText { get {
                StringBuilder text = new StringBuilder();
                foreach (var cls in Classes)
                {
                    text.AppendLine($"אימון {cls.ClassTypeName} בשעה {cls.Date.ToShortTimeString()}.");
                }
                return text.ToString();
            } } 

    }



}
