using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using BLL;
using GetWild.Models;
using InShapeModels;
using Microsoft.AspNet.Identity;
using Utilities;

namespace GetWild.Controllers
{
    [Authorize(Roles = "Instructor, admin")]
    [CompanyAuthorization("UseExpenses")]
    public class ExpensesController: InShapeMVCController
    {

        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return
                    View();
            }
            var model = ExpensesBLL.GetById(id.Value);
            if (model == null)
            {
                return HttpNotFound();
            }
            var viewmodel = Mapper.Map<ExpensesViewModel>(model);
            return View(viewmodel);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ExpensesViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);
            var model = Mapper.Map<ExpensesModel>(viewModel);
            model.StudioId = CurrentUser.StudioId;
            var userid = User.Identity.GetUserId();
            if (model.Id == 0) ExpensesBLL.Create(model);
            else ExpensesBLL.Update(model);
            ViewBag.Date = model.Date;
            return RedirectToAction("ExpensesMonthlyReport", "Report", new { month = model.Date.ToString("MM/yyyy")});
        }


        public ActionResult DeleteConfirmed(int id)
        {
            ExpensesBLL.Delete(id);
            return RedirectToAction("ExpensesMonthlyReport", "Report");
        }

    }
}
