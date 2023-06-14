using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InShapeModels.APIModels
{
    public class APIHealthTandCModel
    {
     

        
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string DOB { get; set; }

        public string Address { get; set; }

        [JsonIgnore]
        public byte[] Signature { get; set; }

        public string Occupation { get; set; }

        public long CitizenId { get; set; }

        public string SignatureData { get; set; }
    }
}
