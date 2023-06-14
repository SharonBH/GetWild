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
    public class StudioController : InShapeMVCController
    {
            //private readonly StudioBLL _studioBll = new StudioBLL();
            //private const string StudioRoomUploadDir = "~/images/StudioRoom";
            // GET: Studio
        public ActionResult Index()
        {
            return View(StudioBLL.GetStudioRooms(0, CurrentUser.StudioId));
        }


        // GET: Studio/Create
        public ActionResult Create(int? id)
        {
            //var SingleStudio = App.CurrentCompany.Studios.Count == 1 ? App.CurrentCompany.Studios.First().Id : 0;
            if (id == null)
            {
                return View(new StudioRoomViewModel
                {
                    Studios = Mapper.Map<List<StudioModel>>(CurrentCompany.Studios),
                    StudioId = CurrentUser.StudioId
                });
            }
            StudioRoomModel studioRoomModel = StudioBLL.GetStudioRooms(id.Value, CurrentUser.StudioId).First();
            if (studioRoomModel == null)
            {
                return HttpNotFound();
            }
            
            return
                View(new StudioRoomViewModel
                {
                    Id = studioRoomModel.Id,
                    Name = studioRoomModel.Name,
                    MaxParticipants = studioRoomModel.MaxParticipants,
                    Picture = studioRoomModel.Picture,
                    Studios = Mapper.Map<List<StudioModel>>(CurrentCompany.Studios),
                    StudioId = CurrentUser.StudioId
                });

        }

        // POST: Studio/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Create(StudioRoomViewModel Model)
            {
                
                //if (Model.ImageUpload == null || Model.ImageUpload.ContentLength == 0)
                //{
                //    ModelState.AddModelError("ImageUpload", "This field is required");
                //}

                //else if (!Utils.ValidImageTypes.Contains(Model.ImageUpload.ContentType))
                //{
                //    ModelState.AddModelError("ImageUpload", "Please choose either a GIF, JPG or PNG image.");
                //}
                if (!ModelState.IsValid) return View(Model);
                //StudioRoomModel room = new StudioRoomModel
                //{
                //    Name = Model.Name,
                //    MaxParticipants = Model.MaxParticipants,
                //    StudioId = Model.StudioId
                //};


                //var imagePath = Path.Combine(Server.MapPath(StudioRoomUploadDir), Model.ImageUpload.FileName);
                //var imageUrl = StudioRoomUploadDir.TrimStart('~') + "/" + Model.ImageUpload.FileName; //Path.Combine(StudioRoomUploadDir, Model.ImageUpload.FileName);
                //Model.ImageUpload.SaveAs(imagePath);
                //room.Picture = imageUrl;
                var room = Mapper.Map<StudioRoomModel>(Model);
                if (Model.Id == null) StudioBLL.CreateStudioRoom(room);
                else
                {
                    //room.Id = Model.Id.Value;
                    StudioBLL.UpdateStudioRoom(room);
                }
                return RedirectToAction("Index");
            }



            // POST: Studio/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StudioBLL.DeleteStudioRoom(id);
            return RedirectToAction("Index");
        }

        
    }
}
