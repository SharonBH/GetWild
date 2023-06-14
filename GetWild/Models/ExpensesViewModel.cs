using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GetWild.Models
{
    public class ExpensesViewModel
    {

        public int? Id { get; set; }

        [Required]
        [Display(Name = "שם")]
        public string Name { get; set; }

        [Display(Name = "תאור")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "סכום")]
        public double Amount { get; set; }

        [Required]
        [Display(Name = "תאריך")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Date { get; set; }

        public int? StudioId { get; set; }

    }
}