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
    public class SubscriptionTypeController : InShapeMVCController
    {

        // GET: Studio
        public ActionResult Index()
        {
            return View(SubscriptionTypeBLL.GetTypes(0, CurrentUser.StudioId));
        }

        //[AllowAnonymous]
        //public ActionResult GetTips()
        //{
        //    return PartialView("TipsPartial", TipBLL.GetTips(0));
        //}

        // GET: Studio/Create
        public ActionResult Create(int? id)
        {
            //var SingleStudio = App.CurrentCompany.Studios.Count == 1 ? App.CurrentCompany.Studios.First().Id : 0;
            if (id == null)
            {
                return View(new SubscriptionTypeViewModel
                {
                    Studios = Mapper.Map<List<StudioModel>>(CurrentCompany.Studios),
                    StudioId = CurrentUser.StudioId
                });
            }
            var typeModel = SubscriptionTypeBLL.GetTypes(id.Value, CurrentUser.StudioId).First();
            if (typeModel == null)
            {
                return HttpNotFound();
            }
            var model = Mapper.Map<SubscriptionTypeViewModel>(typeModel);
            model.Studios = Mapper.Map<List<StudioModel>>(CurrentCompany.Studios);
            model.StudioId = CurrentUser.StudioId;
            return View(model);
            
        }

        // POST: Studio/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SubscriptionTypeViewModel ViewModel)
        {
            
            if (!ModelState.IsValid) return View(ViewModel);
            var model = Mapper.Map<SubscriptionTypeModel>(ViewModel);
            var result = ViewModel.Id > 0 ? SubscriptionTypeBLL.UpdateType(model) : SubscriptionTypeBLL.CreateType(model);
            return RedirectToAction("Index");
        }

        
        public ActionResult DeleteConfirmed(int id)
        {
            SubscriptionTypeBLL.DeleteType(id);
            return RedirectToAction("Index");
        }

        
    }
}
