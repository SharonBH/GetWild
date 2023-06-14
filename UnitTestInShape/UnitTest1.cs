using System;
using System.Linq;
using AutoMapper;
using BLL;
using DAL;
using GetWild;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;

namespace UnitTestInShape
{
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperConfig>());
            App.Companies = SystemBLL.GetCompanies();
            App.SetDefaultCompany();
        }

        [TestMethod]
        public void SendWaitingListSMS()
        {
            SMSBLL.SendWaitingListSMS();
        }

        [TestMethod]
        public void SendInstructorMSG()
        {
            SMSBLL.SendInstructorReminder();
        }

        [TestMethod]
        public void OutrollUserAndSendSMS()
        {
            var result = ClassBLL.OutrolltUserToClass(7285, "257076f3-6f46-40cc-b42c-c44ca494feec", App.DefaultCompany.CancellationThresholdMins, App.DefaultCompany.LateCancelation);
        }

        [TestMethod]
        public void GetInstructorWithClasses()
        {
            var result = UserRepo.GetInstructorWithClasses(DateTime.Now.AddDays(1).Date, 7);
        }
        

        [TestMethod]
        public void SendSMSBeforeStart()
        {
            SMSBLL.SendSMSBeforeStart();
        }

        [TestMethod]
        public void SendBeforeExpireAlert()
        {
            SMSBLL.SendBeforeExpireAlert();
        }

        [TestMethod]
        public void GetCompanies()
        {
            var compnies = SystemBLL.GetCompanies2();
        }

        [TestMethod]
        public void UpdateClassesURLs()
        {
            ServiceBLL.UpdateAllClassesURLs();
        }

        [TestMethod]
        public void CopyWeeklyCalendar()
        {
            StudioBLL.CopyWeeklyCalander(Utils.GetIso8601WeekOfYear(DateTime.UtcNow.ToLocal()), App.DefaultCompany.Studios.FirstOrDefault().Id, App.DefaultCompany.AutoPublish);
        }

        [TestMethod]
        public void Convertinstructors()
        {
            var selectedclass = StudioRepo.GetClass(7245);
            var a = selectedclass.Class_Instructors.SelectMany(x => x.InstructorId);
            string[] instructorids = selectedclass.Class_Instructors.Select(x => x.InstructorId).ToArray();
        }

        [TestMethod]
        public void GetUsersSummary()
        {
            var a = UserBll.GetUsersSummary(App.DefaultCompany.Studios.FirstOrDefault().Id);
        }


        [TestMethod]
        public void getweeklyClasses()
        {
            var now = DateTime.Now;
            var startdate = Utils.FirstDateOfWeek();
            var weekno = Utilities.Utils.GetIso8601WeekOfYear(DateTime.UtcNow.ToLocal());
            //var studioClasses = StudioRepo.GetClasses(startdate, App.DefaultCompany.Studios.FirstOrDefault().Id);
            var studioClasses = ReportBLL.GetWeeklySummaryReport(weekno, App.DefaultCompany.Studios.FirstOrDefault().Id);
            var elps = DateTime.Now-now;
        }


        [TestMethod]
        public void getweeklyClassesnoAVG()
        {
            var startdate = Utils.FirstDateOfWeek();
            var studioClasses = StudioRepo.GetClasses(startdate, false, App.DefaultCompany.Studios.FirstOrDefault().Id);
        }

    }
}
