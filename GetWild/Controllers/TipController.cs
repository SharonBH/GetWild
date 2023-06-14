using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using BLL;
using GetWild.Models;
using InShapeModels;
using Utilities;

namespace GetWild.Controllers
{
    [Authorize(Roles = "Instructor, admin")]
    public class TipController : InShapeMVCController
    {

        // GET: Studio
        public ActionResult Index()
        {
            var t = TipBLL.GetTips(0, CurrentUser.StudioId);
            return View(TipBLL.GetTips(0, CurrentUser.StudioId));
        }

        [AllowAnonymous]
        public ActionResult GetTips()
        {
            return PartialView("TipsPartial", TipBLL.GetTips(0, CurrentUser.StudioId));
        }

        // GET: Studio/Create
        public ActionResult Create(int? id)
        {
            //var SingleStudio = App.CurrentCompany.Studios.Count == 1 ? App.CurrentCompany.Studios.First().Id : 0;
            if (id == null)
            {
                return View(new TipViewModel
                {
                    Studios = Mapper.Map<List<StudioModel>>(CurrentCompany.Studios),
                    StudioId = CurrentUser.StudioId
                });
            }
            var tipviewModel = Mapper.Map<TipViewModel>(TipBLL.GetTips(id.Value, CurrentUser.StudioId).First());
            if (tipviewModel == null || tipviewModel.Id == 0)
            {
                return HttpNotFound();
            }
            tipviewModel.Studios = Mapper.Map<List<StudioModel>>(CurrentCompany.Studios);
            tipviewModel.StudioId = CurrentUser.StudioId;
            return View(tipviewModel);

        }

        // POST: Studio/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TipViewModel Model)
        {

            if (!ModelState.IsValid) return View(Model);

            var tip = Mapper.Map<TipModel>(Model);
            if (Model.Id == null) TipBLL.CreateTip(tip);
            else
            {
                //tip.Id = Model.Id.Value;
                TipBLL.UpdateTip(tip);
            }
            return RedirectToAction("Index");
        }


        public ActionResult DeleteConfirmed(int id)
        {
            TipBLL.DeleteTip(id);
            return RedirectToAction("Index");
        }

        

    }
}
