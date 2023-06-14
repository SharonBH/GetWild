using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using InShapeModels;

namespace GetWild.Models
{
    public class ClassTypeDetailsViewModel
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "שם")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "תיאור")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "סוג אימון")]
        public int ClassTypeId { get; set; }

        public List<ClassTypeModel> ClassTypes { get; set; }

        public int StudioId { get; set; }

        public string Picture { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "תמונה")]
        public HttpPostedFileBase ImageUpload { get; set; }

        public List<StudioModel> Studios { get; set; }
    }
}