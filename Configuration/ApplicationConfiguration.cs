using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Westwind.Utilities.Configuration;

namespace Configuration
{
    public class App
    {
        public static ApplicationConfiguration Configuration { get; set; }

        public static CompanyConfiguration Company { get; set; }

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
            Configuration = new ApplicationConfiguration();
            Company = new CompanyConfiguration();
            Configuration.Initialize();
            //Configuration.Write();
        }
    }
    public class ApplicationConfiguration : Westwind.Utilities.Configuration.AppConfiguration
    {

        public ApplicationConfiguration()
        {
            //ApplicationTitle = "Sample Application";
            //DebugMode = DebugModes.ApplicationErrorMessage;
            //MaxPageItems = 20;
        }

        // Create properties for values to read or persist to/from the config store
        public string ApplicationTitle { get; set; }
        public string ConnectionString { get; set; }
        //public DebugModes DebugMode { get; set; }  // enum
        public int MaxPageItems { get; set; }   // number

        public string ProfileUploadDir { get; set; }

        public string ClassTypeUploadDir { get; set; }

        public string StudioRoomUploadDir { get; set; }
        public int CancellationThresholdMins { get; set; }

        public int WaitingListThersholdMins { get; set; }

        public bool SendSMS { get; set; }

        public bool ClassRatingEnabled { get; set; }

        public bool HealthTandCEnabled { get; set; }
        

        //public bool ShowAllSpacesLeft { get; set; }

        public int SpacesLeftToShow { get; set; }

        public string GoogleAPIKey { get; set; }

        public bool HangFireServiceEnabled { get; set; }

        protected override IConfigurationProvider OnCreateDefaultProvider(string sectionName, object configData)
        {
            var provider = new ConfigurationFileConfigurationProvider<ApplicationConfiguration>()
            {
                ConfigurationFile = Utilities.stringGetFilePath(),
                ConfigurationSection = "SiteConfiguration",
                EncryptionKey = "XDkGvrVcZU43tjE9",
                PropertiesToEncrypt = ""
            };

            return provider;
        }
    }

    public class DatabaseConfiguration : Westwind.Utilities.Configuration.AppConfiguration
    {
        public string ApplicationName { get; set; }
        public int MaxDisplayListItems { get; set; }
        public bool SendAdminEmailConfirmations { get; set; }
        public string Password { get; set; }
        public string AppConnectionString { get; set; }

        // Must implement public default constructor
        public DatabaseConfiguration()
        {
            //ApplicationName = "Configuration Tests";
            //MaxDisplayListItems = 15;
            //SendAdminEmailConfirmations = false;
            //Password = "seekrit";
            //AppConnectionString = "server=.;database=hosers;uid=bozo;pwd=seekrit;";
        }

        ///// 

        ///// Override this method to create the custom default provider - in this case a database
        ///// provider with a few options.
        ///// 

        protected override IConfigurationProvider OnCreateDefaultProvider(string sectionName, object configData)
        {
            string connectionString = "DefaultConnection";
            string tableName = "AppConfiguration";

            var provider = new SqlServerConfigurationProvider<DatabaseConfiguration>()
            {
                ConnectionString = connectionString,
                Tablename = tableName,
                EncryptionKey = "ultra-seekrit", // use a generated value here
                PropertiesToEncrypt = "Password,AppConnectionString"
            };

            return provider;
        }
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

        public List<Studio> Studios { get; set; }


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
