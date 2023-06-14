using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InShapeModels
{
    public class UserProfileModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? Height { get; set; }

        public decimal? Weight { get; set; }

        public decimal WeightChange { get; set; }

        public decimal? BMI { get; set; }

        public decimal? Fat { get; set; }

        public int? Dim_Hands { get; set; }

        public int? Dim_Legs { get; set; }

        public int? Dim_Waist { get; set; }

        public int? Dim_Thighs { get; set; }

        //public int? D_Other { get; set; }

        public decimal? Fat_HandL { get; set; }

        public decimal? Fat_HandR { get; set; }

        public decimal? Fat_LegR { get; set; }

        public decimal? Fat_LegL { get; set; }

        public decimal? Fat_Belly { get; set; }

        public decimal? Mucsle { get; set; }

        public decimal? OrigWeight { get; set; }
        public bool Update { get; set; }

        public DateTime? Date { get; set; }

        public string Picture { get; set; }

    }
}
