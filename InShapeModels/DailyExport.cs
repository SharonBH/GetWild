using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace InShapeModels
{
    public class DailyExportModel
    {
        public int ClassEnrollmentId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DOB { get; set; }
        public string Address { get; set; }
        public int Gender { get; set; }
        public bool ReceiveSMS { get; set; }
        public int StudioId { get; set; }
        public string UserId { get; set; }
        public bool Active { get; set; }
        public bool Frozen { get; set; }
        public string SubscriptionName { get; set; }
        public DateTime Date { get; set; }
        public int SubscriptionId { get; set; }
        public int? RoleId { get; set; }
        public DateTime? LastClassDate { get; set; }
        public string LastClass { get; set; }
        public DateTime? NextClassDate { get; set; }
        public string NextClass { get; set; }
        public int? DaysSinceLastClass { get; set; }
        public bool IsLateCancel { get; set; }

        public string UserTypeName
        {
            get
            {
                var type = (ParticipantType)RoleId;
                return type.GetDisplayName();
            }
        }

    }
}
