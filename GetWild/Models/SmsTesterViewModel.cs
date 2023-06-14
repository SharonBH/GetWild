using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetWild.Models
{
    public class SmsTesterViewModel
    {
        [Required]
        [Display(Name = "שם השולח")]
        public string  Sender { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "תוכן ההודעה:")]
        public string Message { get; set; }

        [Required]
        [Display(Name = "טלפון לשליחה")]
        public string Recipient { get; set; }
    }
}
