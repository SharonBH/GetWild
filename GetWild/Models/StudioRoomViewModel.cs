using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using InShapeModels;

namespace GetWild.Models
{
    public class StudioRoomViewModel
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "שם הסטודיו")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "מקסימום משתתפים")]
        public int MaxParticipants { get; set; }

        public string Picture { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "תמונה")]
        public HttpPostedFileBase ImageUpload { get; set; }

        public int StudioId { get; set; }

        public List<StudioModel> Studios { get; set; }


    }
}