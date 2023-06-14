using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetWild.Models
{
    public class InShapeUserModels
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Profile UserProfile { get; set; }
    }

    public class Profile
    {
        public int Height { get; set; }

        public int Weight { get; set; }

        public int BMI { get; set; }

        public int FatPer { get; set; }

        public int D_Hands { get; set; }

        public int D_legs { get; set; }

        public int D_Waist { get; set; }

        public int D_Thighs { get; set; }

        public int D_Other { get; set; }

        public bool Update { get; set; }
    }


}