using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GetWild.Models
{
    public class SubscriberViewModel
    {
        [Required]
        [Display(Name = "שם מלא")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "טלפון נייד")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "חובה למלא כתובת אימייל")]
        [Display(Name = "אימייל")]
        [EmailAddress]
        public string Email { get; set; }
    }
}