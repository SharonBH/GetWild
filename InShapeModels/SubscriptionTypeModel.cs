using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InShapeModels
{
    public class SubscriptionTypeModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int PeriodMonths { get; set; }

        public int NumClasses { get; set; }

        public double Price { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsUnlimited
        {
            get { return NumClasses == 0; }
        }

        public bool IsNeverExpire
        {
            get { return PeriodMonths == 0; }
        }

        public int StudioId { get; set; }
    }
}
