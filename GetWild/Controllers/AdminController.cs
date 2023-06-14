using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using GetWild.Models;

namespace GetWild.Controllers
{
    [Authorize(Roles = "Instructor, ClassInstructor, admin")]
    public class AdminController : InShapeMVCController
    {
        // GET: Admin
        public ActionResult Index()
        {
            //ServiceBLL.RunExpireSubscriptions();
            return View();
        }

        public PartialViewResult GetAdminTop()
        {
            if(User.IsInRole("admin") || User.IsInRole("Instructor") || User.IsInRole("ClassInstructor"))
            {
                var model = new UserWithCompany { CurrentUser = CurrentUser, CurrentCompany = CurrentCompany };
                return PartialView("_AdminTop", model);
            }
            return default(PartialViewResult);
        }

        [HttpPost]
        public ActionResult ProcessMarked()
        {
            var result = SystemBLL.ProcessMarked(CurrentCompany.Id);
            return !result ? Json(new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." }) 
                           : Json(new { Response = "Success", Message = "השינוי נשמר בהצלחה." });
        }
    }
}