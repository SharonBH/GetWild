using InShapeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Utilities;
using GetWild.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace GetWild.Controllers
{
    public class InShapeMVCController : Controller
    {
        //public CompanyModel Company
        //{
        //    get
        //    {
        //        object company;
        //        if (!Request.GetOwinContext().Environment.TryGetValue("Company", out company))
        //        { throw new ApplicationException("Could not find Company");}
        //        return (CompanyModel) company;
        //    }
        //}

        public ApplicationUser CurrentUser
        {
            get
            {
                return System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            }
        }

        public CompanyConfiguration CurrentCompany
        {
            get
            {
                return CurrentUser == null ?  null : App.Companies.FirstOrDefault(c => c.Studios.Any(s => s.Id == CurrentUser.StudioId));
            }
        }

    }
}
