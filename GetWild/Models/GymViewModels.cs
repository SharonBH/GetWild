using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using InShapeModels;
using Newtonsoft.Json;

namespace GetWild.Models
{
    public class GymUserViewModel
    {
        public ApplicationUser User { get; set; }

        public SubscriptionViewModel UserSubscription { get; set; }

        public ProfileViewModel UserProfile { get; set; }

        public ProfileViewModel UserFirstProfile { get; set; }
    }

    public class GymCalanderViewModel
    {
        public DateTime Date { get; set; }
        [JsonIgnore]
        public int WeekNo { get; set; }
        [JsonIgnore]
        public int CurrentWeekNo { get; set; }
        [JsonIgnore]
        public int RoomId { get; set; }

        public List<ClassViewModel> Classes { get; set; }
        [JsonIgnore]
        public List<StudioRoomModel> AvailableRooms { get; set; }

    }

    public class UserImageUpload
    {
        public string UserId { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "תמונה")]
        [Required]
        public HttpPostedFileBase ImageUpload { get; set; }

        public string ImageType { get; set; }
    }


    public class TipViewModel
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "טיפ")]
        [DataType(DataType.MultilineText)]
        public string Tip { get; set; }

        public int StudioId { get; set; }

        public List<StudioModel> Studios { get; set; }

    }

}