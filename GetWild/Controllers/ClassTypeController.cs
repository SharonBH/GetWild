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
    public class ClassTypeController : InShapeMVCController
    {
            //private readonly ClassBLL _classBll = new ClassBLL();
            //private const string ClassTypeUploadDir = "~/images/ClassTypes";

        // GET: Studio
        public ActionResult Index()
        {
            return View(ClassBLL.GetClassTypes(0, CurrentUser.StudioId));
        }


        // GET: Studio/Create
        public ActionResult Create(int? id)
        {
            //var SingleStudio = App.CurrentCompany.Studios.Count == 1 ? App.CurrentCompany.Studios.First().Id : 0;
            if (id == null)
            {
                return View(new ClassTypeViewModel
                {
                    Studios = Mapper.Map<List<StudioModel>>(CurrentCompany.Studios),
                    StudioId = CurrentUser.StudioId
                });
            }
            var classType = Mapper.Map<ClassTypeViewModel>( ClassBLL.GetClassTypes(id.Value, CurrentUser.StudioId).First());
            classType.Studios = Mapper.Map<List<StudioModel>>(CurrentCompany.Studios);
            classType.StudioId = CurrentUser.StudioId;
            if (classType.Id == null || classType.Id == 0)
            {
                return HttpNotFound();
            }
            return
                View(classType);

        }

        // POST: Studio/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClassTypeViewModel Model)
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
            if (!ModelState.IsValid) return View(Model);
            var imageUrl = Model.Picture;
            if (Model.ImageUpload != null)
            {
                var imagePath = Path.Combine(Server.MapPath(App.Configuration.ClassTypeUploadDir), Model.ImageUpload.FileName);
                imageUrl = App.Configuration.ClassTypeUploadDir.TrimStart('~') + "/" + Model.ImageUpload.FileName;
                    //Path.Combine(StudioRoomUploadDir, Model.ImageUpload.FileName);
                Model.ImageUpload.SaveAs(imagePath);
            }
            var classType = Mapper.Map<ClassTypeModel>(Model);
            classType.Picture = imageUrl;
            if (Model.Id == null) ClassBLL.CreateClassType(classType);
            else
            {
                classType.Id = Model.Id.Value;
                ClassBLL.UpdateClassType(classType);
            }
            return RedirectToAction("Index");
        }

        
        
        // POST: Studio/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClassBLL.DeleteClassType(id);
            return RedirectToAction("Index");
        }

        
    }
}
