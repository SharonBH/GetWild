using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Utilities;

namespace GetWild
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CompanyAuthorizationAttribute : AuthorizeAttribute
    {
        public string[] Permission { get; set; }
        public CompanyAuthorizationAttribute(params string[] permission)
        {
            Permission = permission;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return Permission.Any(p=> (bool)App.DefaultCompany.GetType().GetProperty(p).GetValue(App.DefaultCompany,null));
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class SwitchableAuthorizationAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool disableAuthentication = false;

#if DEBUG
            disableAuthentication = true;
#endif

            if (disableAuthentication)
                return true;

            return base.AuthorizeCore(httpContext);
        }
    }
}