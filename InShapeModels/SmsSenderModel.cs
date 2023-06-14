using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InShapeModels
{
    public class SmsSenderModel
    {
        public string UserId { get; set; }

        public string Sender { get; set; }

        public string SmsMessage { get; set; }

        public DateTime? SendDate { get; set; }

        public DateTime? Time { get; set; }

        public List<UserSMSSubscriptionModel> Users { get; set; }

        public SmsListType ListType { get; set; }

        public string RefId { get; set; }

        public bool IsBulk {
            get { return !SmsMessage.Contains("[[שם]]"); }
        }

        public bool SendNow {
            get { return !SendDate.HasValue; }
        }

        public string Source { get; set; }

        public string Recipients
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var user in Users)
                {
                    if(user.Selected) sb.Append(user.PhoneNumber + ",");
                }
                return sb.Remove(sb.Length - 1, 1).ToString();
            }
        }

    }
}
