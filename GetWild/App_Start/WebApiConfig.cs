using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Converters;

namespace GetWild
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            IsoDateTimeConverter converter = new IsoDateTimeConverter
            {
                DateTimeStyles = DateTimeStyles.AssumeLocal,
                DateTimeFormat = "r"
                //DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK"
            };

            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(converter);

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
