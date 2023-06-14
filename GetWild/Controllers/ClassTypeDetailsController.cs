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
using System.Globalization;

namespace GetWild.Controllers
{
    [Authorize(Roles = "Instructor, admin")]
    [CompanyAuthorization("UseClassTypeDetails")]
    public class ClassTypeDetailsController : InShapeMVCController
    {
        //private readonly ClassBLL _classBll = new ClassBLL();
        //private const string ClassTypeUploadDir = "~/images/ClassTypes";

        // GET: Studio
        public ActionResult Index()
        {
            return View(ClassBLL.GetClassTypesDetails(0));
        }


        // GET: Studio/Create
        public ActionResult Create(int? id)
        {
            //var SingleStudio = App.CurrentCompany.Studios.Count == 1 ? App.CurrentCompany.Studios.First().Id : 0;
            if (id == null)
            {
                return View(new ClassTypeDetailsViewModel()
                {
                    Studios = Mapper.Map<List<StudioModel>>(CurrentCompany.Studios),
                    StudioId = CurrentUser.StudioId,
                    ClassTypes = ClassBLL.GetClassTypes(0, CurrentUser.StudioId)
                });
            }
            var classTypeDetails = Mapper.Map<ClassTypeDetailsViewModel>(ClassBLL.GetClassTypesDetails(id.Value).First());
            classTypeDetails.Studios = Mapper.Map<List<StudioModel>>(CurrentCompany.Studios);
            classTypeDetails.StudioId = CurrentUser.StudioId;
            classTypeDetails.ClassTypes = ClassBLL.GetClassTypes(0, CurrentUser.StudioId);
            if (classTypeDetails.Id == null || classTypeDetails.Id == 0)
            {
                return HttpNotFound();
            }
            return
                View(classTypeDetails);

        }

        // POST: Studio/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClassTypeDetailsViewModel Model)
        {
            if (!Model.Id.HasValue)
            {
                if (Model.ImageUpload == null || Model.ImageUpload.ContentLength == 0)
                {
                    ModelState.AddModelError("ImageUpload", "This field is required");
                }
                else if (!Utilities.Utils.ValidImageTypes.Contains(Model.ImageUpload.ContentType))
                {
                    ModelState.AddModelError("ImageUpload", "Please choose either a GIF, JPG or PNG image.");
                }
            }
            else
            {
                if (Model.ImageUpload != null && Model.ImageUpload.ContentLength > 0)
                {
                    if (!Utilities.Utils.ValidImageTypes.Contains(Model.ImageUpload.ContentType))
                    {
                        ModelState.AddModelError("ImageUpload", "Please choose either a GIF, JPG or PNG image.");
                    }
                }
            }
            var imageUrl = Model.Picture;
            if (Model.ImageUpload != null)
            {
                var imagePath = Path.Combine(Server.MapPath(App.Configuration.ClassTypeUploadDir),
                    Model.ImageUpload.FileName);
                imageUrl = App.Configuration.ClassTypeUploadDir.TrimStart('~') + "/" + Model.ImageUpload.FileName;
                //Path.Combine(StudioRoomUploadDir, Model.ImageUpload.FileName);
                Model.ImageUpload.SaveAs(imagePath);
            }
            if (!ModelState.IsValid) return View(Model);
            var classTypeDetails = Mapper.Map<ClassTypeDetailsModel>(Model);
            classTypeDetails.Picture = imageUrl;
            if (Model.Id == null) ClassBLL.CreateClassTypeDetails(classTypeDetails);
            else
            {
                classTypeDetails.Id = Model.Id.Value;
                ClassBLL.UpdateClassTypeDetails(classTypeDetails);
            }
            return RedirectToAction("Index");
        }



        // POST: Studio/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClassBLL.DeleteClassTypeDetails(id);
            return RedirectToAction("Index");
        }


        public ActionResult GetClassTypesDetailsByType(int typeid)
        {
            var typesdetails = ClassBLL.GetClassTypesDetailsByType(typeid, true);
            return Json(typesdetails, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GetClassByTypesDetails(int typeid, int month)
        {
            var classes = StudioBLL.GetClassByTypesDetails(typeid, month);
            return PartialView("_LinkedClasses", classes);
        }

    }
}
