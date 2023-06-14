using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using InShapeModels;

namespace GetWild.Models
{
    public class ClassTypeViewModel
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "סוג אימון")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "תיאור")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "צבע רקע")]
        public string BGColor { get; set; }

        public string Picture { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "תמונה")]
        public HttpPostedFileBase ImageUpload { get; set; }


        public int StudioId { get; set; }

        public List<StudioModel> Studios { get; set; }
    }
}