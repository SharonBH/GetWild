using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InShapeModels.APIModels
{
    public class ProfileImage
    {
        [JsonIgnore]
        public string UserId { get; set; }

        public string ImageName { get; set; }

        [JsonIgnore]
        public string FileName { get { return ImageName.Substring(0,ImageName.Length-4); } }

        [JsonIgnore]
        public string FileExtention { get { return ImageName.Substring(ImageName.Length - 3); } }

        public string ImageData { get; set; }
    }
}
