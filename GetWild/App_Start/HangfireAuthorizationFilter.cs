using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire.Dashboard;
using Microsoft.Owin;

namespace GetWild.App_Start
{
    public class HangfireAuthorizationFilter : IAuthorizationFilter
    {
        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            var boolAuthorizeCurrentUserToAccessHangFireDashboard = false;
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (HttpContext.Current.User.IsInRole("admin"))
                    boolAuthorizeCurrentUserToAccessHangFireDashboard = true;
            }

            return boolAuthorizeCurrentUserToAccessHangFireDashboard;
        }
    }
}