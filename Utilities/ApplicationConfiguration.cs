using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Utilities
{
    public class App
    {
        public static CompanyConfiguration DefaultCompany { get; set; }
        public static ServerConfiguration Configuration { get; set; }

        public static List<CompanyConfiguration> Companies { get; set; }

        static App()
        {

            // Create a customized provider to set provider options
            //var provider = new ConfigurationFileConfigurationProvider<ApplicationConfiguration>()
            //{
            //    ConfigurationFile = "Configuration.config",
            //    ConfigurationSection = "SiteConfiguration",
            //    EncryptionKey = "XDkGvrVcZU43tjE9",
            //    PropertiesToEncrypt = ""
            //};
            //Counter = 0;
            Configuration = new ServerConfiguration();
            Companies = new List<CompanyConfiguration>();
            //Configuration.Initialize();
            //Configuration.Write();
        }

        //public static void SetCurrentCompany(string host)
        //{
        //    host = host.Replace("www.", "").Replace("dev","");
        //    Logger.WriteInfo(host);
        //    if (Companies.Any(c => c.WebSiteURL == host))
        //        CurrentCompany = Companies.FirstOrDefault(c => c.WebSiteURL == host);
        //    else CurrentCompany = new CompanyConfiguration{ Id = -1, CSSFileName = string.Empty, Name = string.Empty, SiteName = string.Empty };
        //    Logger.WriteInfo($"SetCurrentCompany: {CurrentCompany.Id}");
        //}

        public static void SetDefaultCompany()
        {
            //var a = HttpContext.Current.Request.IsAuthenticated;
            DefaultCompany = Companies.FirstOrDefault();
            Logger.WriteDebug($"SetDefaultCompany: {DefaultCompany.Id}");
        }

        //public static void SetCurrentCompanybyID(int id)
        //{
        //    CurrentCompany = Companies.FirstOrDefault(c => c.Id == id);
        //}

        //public static void SetCurrentCompanybyStudioID(int sid)
        //{
        //    if (Configuration.MultiTenant) return;
        //    CurrentCompany = Companies.FirstOrDefault(c => c.Studios.Any(s=> s.Id == sid));
        //    Logger.WriteDebug($"SetCurrentCompanybyStudioID: {CurrentCompany.Id}");
        //}
    }
    public class ServerConfiguration
    {

        public ServerConfiguration()
        {
            HangFireServiceEnabled = ConfigurationManager.AppSettings["HangFireServiceEnabled"].Equals("true");
            ProfileUploadDir = ConfigurationManager.AppSettings["ProfileUploadDir"];
            ClassTypeUploadDir = ConfigurationManager.AppSettings["ClassTypeUploadDir"];
            ClassTypeDetailUploadDir = ConfigurationManager.AppSettings["ClassTypeDetailUploadDir"];
            InstructorUploadDir = ConfigurationManager.AppSettings["InstructorUploadDir"];
            StudioRoomUploadDir = ConfigurationManager.AppSettings["StudioRoomUploadDir"];
            MultiTenant = ConfigurationManager.AppSettings["MultiTenant"].Equals("true");
            IsDebug = ConfigurationManager.AppSettings["IsDebug"].Equals("true");
            CalanderChangeHour = int.Parse(ConfigurationManager.AppSettings["CalanderChangeHour"]);
            DaysSinceLastClassMarker = int.Parse(ConfigurationManager.AppSettings["DaysSinceLastClassMarker"]);
        }

        // Create properties for values to read or persist to/from the config store
        //public string ApplicationTitle { get; set; }
        //public string ConnectionString { get; set; }
        //public DebugModes DebugMode { get; set; }  // enum
        //public int MaxPageItems { get; set; }   // number

        public string ProfileUploadDir { get; set; }

        public string ClassTypeUploadDir { get; set; }

        public string ClassTypeDetailUploadDir { get; set; }
        

        public string InstructorUploadDir { get; set; }

        public string StudioRoomUploadDir { get; set; }

        public bool MultiTenant { get; set; }

        public bool IsDebug { get; set; }

        public int CalanderChangeHour { get; set; }

        //public int CancellationThresholdMins { get; set; }

        //public int WaitingListThersholdMins { get; set; }

        //public bool SendSMS { get; set; }

        //public bool ClassRatingEnabled { get; set; }

        //public bool HealthTandCEnabled { get; set; }


        //public bool ShowAllSpacesLeft { get; set; }

        //public int SpacesLeftToShow { get; set; }

        //public string GoogleAPIKey { get; set; }

        public bool HangFireServiceEnabled { get; set; }

        public string ServerName { get; set; }

        public int DaysSinceLastClassMarker { get; set; }
    }


    public class CompanyConfiguration
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ManagerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime StartDate { get; set; }
        public bool UseSMS { get; set; }
        public bool UseHosting { get; set; }
        public bool Active { get; set; }
        public bool IsDeleted { get; set; }
        public string CompanyCode { get; set; }
        public string GoogleAPIKey { get; set; }
        public string WebSiteURL { get; set; }
        public bool HealthTandCEnabled { get; set; }
        public bool ClassRatingEnabled { get; set; }
        public int CancellationThresholdMins { get; set; }
        public bool WaitingListEnabled { get; set; }

        public int WaitingListSpaces { get; set; }
        //public int WaitingListThersholdMins { get; set; }
        public int? SpacesLeftToShow { get; set; }
        public List<Studio> Studios { get; set; }
        public string SiteName { get; set; }
        public string LogoURL { get; set; }
        public string CSSFileName { get; set; }

        public string StudioRoomName { get; set; }
        public string TipsName { get; set; }
        public string CalanderMode { get; set; }

        public bool UseClassNamefromType { get; set; }

        public bool UseInstructors { get; set; }

        public bool UseExpenses { get; set; }

        public bool UseClassTypeDetails { get; set; }

        public int NumAdvncedEnrollments { get; set; }

        public bool ManageAfterRegister { get; set; }

        public bool AutoPublish { get; set; }

        public int LateRegistration { get; set; }

        public bool UsePlacements { get; set; }

        //public bool UseAgeforGender { get; set; }

        public int PriorityWaitListDays { get; set; }

        public int AdultAge { get; set; }

        public int TeenAge { get; set; }

        public int LateCancelation { get; set; }

        public string LateCancelPenalty { get; set; }

        public bool RemoveMarked { get; set; }

        //public int DefaultStudio
        //{
        //    get
        //    {
        //        //var studio = App.CurrentCompany.Studios.Count == 1 ? App.CurrentCompany.Studios.First() : ;
        //        //var studio = Studios.OrderBy(x => x.Id).FirstOrDefault();
        //        return Studios.OrderBy(x => x.Id).FirstOrDefault().Id;
        //        //return StudioRepo.GetDefaultStudioByComapny(companyId);

        //    }
        //}

    }

    public class Studio
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ManagerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime StartDate { get; set; }
        public bool Active { get; set; }
        public int CompanyId { get; set; }
        public bool IsDeleted { get; set; }
    }

}
