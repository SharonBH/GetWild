using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InShapeModels;

namespace GetWild.Models
{
    public class AdvancedSmsViewModel
    {
        [Required]
        [Display(Name = "שם השולח")]
        public string Sender { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "תוכן ההודעה:")]
        public string SmsMessage { get; set; }


        [Display(Name = "מתי?")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? SendDate { get; set; }


        [Display(Name = "שעה?")]
        [DataType(DataType.Time)]
        public DateTime? Time { get; set; }
        

        public List<UserSMSSubscriptionModel> Users { get; set; }

        public SmsListType ListType { get; set; }

        public string RefId { get; set; }

        public AdvancedSmsViewModel()
        {
            Users = new List<UserSMSSubscriptionModel>();
        }
    }

    

}
