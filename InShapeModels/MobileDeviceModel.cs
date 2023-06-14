using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InShapeModels
{
    public class MobileDeviceModel
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string DeviceId { get; set; }

        public string PushNotificationId { get; set; }

        public string DeviceType { get; set; }

        public string DeviceOS { get; set; }

        public DateTime DateAdded { get; set; }

        public string DeviceAdsId { get; set; }

        public bool IsDeleted { get; set; }

    }
}
