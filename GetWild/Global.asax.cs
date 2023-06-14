using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AutoMapper;
using Utilities;

namespace GetWild
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperConfig>());
            Logger.WriteInfo($"Website {App.Configuration.ServerName} started, Multi Tanent Mode: {App.Configuration.MultiTenant}.");
            //if (!App.Configuration.MultiTenant) return;
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new MultTenantViewEngine());
        }
    }
}
