using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace InShapeModels
{
    public class FrozenSubscriptionModel
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public string SubscriptionUser { get; set; }
        public string UserNameCreated { get; set; }
        public DateTime Date { get; set; }
        public DateTime? FreezeToDate { get; set; }
        public string Note { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DateFinished { get; set; }
        public string UserNameFinished { get; set; }

        public UserSubscriptionModel UserSubscription { get; set; }

        public string FrozenDetails
        {
            get
            {
                var s = $"המנוי הוקפא בתאריך: {Date.ToShortDateString()}, ע'י {UserNameCreated}";
                s = FreezeToDate.HasValue ? s + ", עד לתאריך " + FreezeToDate.Value.ToShortDateString() : s;
                return s;
            }
        }

        public int DaysFrozen { get
            { return ((FreezeToDate?.Date ?? DateFinished?.Date ?? DateTime.UtcNow.ToLocal()) - Date).Days; } }
    }
}
