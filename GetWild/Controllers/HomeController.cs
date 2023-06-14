using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using GetWild.Models;
using InShapeModels;
using Utilities;

namespace GetWild.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //var c = App.Company;
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult TheTeam()
        {

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddContactUs(SubscriberViewModel model)
        {
            if (!ModelState.IsValid) return Json(new { Response = "Error", Message = "נתונים שגויים." });
            SubscriberModel contact = new SubscriberModel
            {
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                CompanyId = 7 //todo get default company
            };

            return Json(UserBll.AddContactUs(contact) ? new { Response = "Success", Message = "הנתונים נשמרו בהצלחה." } : new { Response = "Error", Message = "לא ניתן לשמור, אנא נסה מאוחר יותר." });

        }

    }
}