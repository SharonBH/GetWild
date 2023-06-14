using System;
using BLL;
using GetWild.App_Start;
using Hangfire;
using Microsoft.Owin;
using Owin;
using Utilities;

[assembly: OwinStartupAttribute(typeof(GetWild.Startup))]
namespace GetWild
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            //app.Use(async (ctx, next) =>
            //{
            //    App.Companies = SystemBLL.GetCompanies(); 
            //    //ctx.Environment.Add("Company", company);
            //    await next();
            //});
            //if (App.Configuration.MultiTenant)
            //{
            App.Companies = SystemBLL.GetCompanies();
            App.SetDefaultCompany();
            app.Use(async (ctx, next) =>
                {
                    //if (App.Configuration.MultiTenant) App.SetCurrentCompany(ctx.Request.Uri.Host);

                    //else App.SetDefaultCompany();
                    //Logger.WriteInfo($"{App.Companies.Count} Companies loaded, current Company: {App.CurrentCompany.SiteName}");
                    //ctx.Environment.Add("Company", company);
                    await next();
                });
            //}

            if (!App.Configuration.HangFireServiceEnabled) return;
            GlobalConfiguration.Configuration
                .UseSqlServerStorage("DefaultConnection");

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                AuthorizationFilters = new[] { new HangfireAuthorizationFilter() }
            });
            app.UseHangfireServer();

            //            RecurringJob.AddOrUpdate("RunExpireSubscriptions", () => ServiceBLL.RunExpireSubscriptions(),Cron.Daily, TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time"));

            //#if !DEBUG
            //if (App.Configuration.SendSMS)
            //{
            RecurringJob.AddOrUpdate("SendMassSms", () => SMSBLL.SendMassSms(16), "*/15 * * * *");
            RecurringJob.AddOrUpdate("SMSBeforeClassStart", () => SMSBLL.SendSMSBeforeStart(), "*/30 * * * *");
            RecurringJob.AddOrUpdate("SMSBeforeSubscriptionExpire", () => SMSBLL.SendBeforeExpireAlert(),
                "0 12 * * *");
            RecurringJob.AddOrUpdate("SendInstructorReminder", () => SMSBLL.SendInstructorReminder(),
                "0 20 * * *", TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time"));
            RecurringJob.AddOrUpdate("SendInactiveAlert", () => SMSBLL.SendInactiveAlert(),
                "0 13 * * *");
            RecurringJob.AddOrUpdate("SendWaitingListSMS", () => SMSBLL.SendWaitingListSMS(false), "*/5 * * * *");
            //}

            RecurringJob.AddOrUpdate("RunExpireSubscriptions", () => ServiceBLL.RunExpireSubscriptions(), Cron.Daily);
            //TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time"));
            RecurringJob.AddOrUpdate("UpdateClassesURLs", () => ServiceBLL.UpdateAllClassesURLs(), Cron.Daily);
                //TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time"));
            //#endif

        }
    }
}
