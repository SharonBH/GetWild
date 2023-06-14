using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using GetWild.Models;
using Utilities;

namespace GetWild.App_Start
{
    public class ForceLogoutFilter : ActionFilterAttribute
    {
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var UserManager = filterContext.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                //var user = UserManager.FindByEmailAsync(HttpContext.Current.User.Identity.Name).Result; 
                var user = UserManager.FindByNameAsync(HttpContext.Current.User.Identity.Name).Result;

                if (user != null && user.UserName == "yaronharel9@gmail.com")
                {
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    filterContext.HttpContext.Session.Clear();
                    filterContext.HttpContext.Session.Abandon();
                    filterContext.Result = new RedirectToRouteResult(
                                new RouteValueDictionary    {{ "Controller", "Home" },
                                                            { "Action", "Index" } });
                }
            }


            base.OnActionExecuting(filterContext);
        }
    }

    public class SetCompanyActionFilter : ActionFilterAttribute
    {
        //private IAuthenticationManager AuthenticationManager
        //{
        //    get
        //    {
        //        return HttpContext.Current.GetOwinContext().Authentication;
        //    }
        //}


        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{

        //    if (filterContext.HttpContext.User.Identity.IsAuthenticated)
        //    {
        //        var UserManager = filterContext.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //        //var user = UserManager.FindByEmailAsync(HttpContext.Current.User.Identity.Name).Result; 
        //        var user = UserManager.FindByNameAsync(HttpContext.Current.User.Identity.Name).Result;
        //        string actionName = filterContext.ActionDescriptor.ActionName;
        //        string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

        //        if (user != null) App.SetCurrentCompanybyStudioID(user.StudioId);
        //        Logger.WriteDebug($"OnActionExecuting ({controllerName}:{actionName}) StudioId: {user.StudioId}");
        //    }
        //    base.OnActionExecuting(filterContext);
        //}
    }
}