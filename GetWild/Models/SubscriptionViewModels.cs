using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InShapeModels;

namespace GetWild.Models
{
    public class SubscriptionTypeViewModel
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "סוג מנוי")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "תיאור")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "תקופה בחודשים")]
        [Range(0, int.MaxValue, ErrorMessage = "תקופת מנוי בחודשים לא יכול להיות שלילי")]
        public int PeriodMonths { get; set; }

        [Required]
        [Display(Name = "מספר אימונים")]
        [Range(0, int.MaxValue, ErrorMessage = "מספר אימונים לא יכול להיות שלילי")]
        public int NumClasses { get; set; }

        //[Required]
        [Display(Name = "מחיר")]
        [Range(0, int.MaxValue, ErrorMessage = "מחיר לא יכול להיות שלילי")]
        public double Price { get; set; }

        public int StudioId { get; set; }

        public List<StudioModel> Studios { get; set; }

        //public bool IsDeleted { get; set; }

        //public bool IsUnlimited
        //{
        //    get { return NumClasses == 0; }
        //}

        //public bool IsNeverExpire
        //{
        //    get { return PeriodMonths == 0; }
        //}
    }
}
