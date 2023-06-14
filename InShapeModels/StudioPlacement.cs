using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InShapeModels
{
    public class StudioPlacementModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StudioId { get; set; }
        public bool IsDeleted { get; set; }
        public int Places { get; set; }
        public int TypeId { get; set; }

    }

    //public class ClassPlacementModel
    //{
    //    public int Id { get; set; }
    //    public int ClassId { get; set; }
    //    public int StudioPlacementId { get; set; }

    //    public string StudioPlacementName { get; set; }
    //    public byte ClassPlacementNumber { get; set; }

    //}

    public class ClassPlacementPrintModel
    {
        public int ClassId { get; set; }
        //public List<ClassPlacementModel> Placements { get; set; }
        public List<ClassEnrollmentModel> Enrollments { get; set; }
    }

    public class AvailablePlacementsModel
    {
        //public AvailablePlacementsModel()
        //{
        //    if (!IsDeleted) ToCreate = true;
        //}
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int StudioPlacementId { get; set; }
        public string StudioPlacementName { get; set; }
        public byte ClassPlacementNumber { get; set; }
        public bool IsInUse { get; set; }
        public bool IsDeleted { get; set; }
        public int TypeId { get; set; }

        public bool ToCreate { get; set; }

        public ClassEnrollmentModel ClassEnrollment { get; set; }

         
        public string DisplyName { get { return StudioPlacementId != 999 && StudioPlacementId != 1003 && StudioPlacementId != 1007 ?  $"{StudioPlacementName}: {ClassPlacementNumber}": $"{StudioPlacementName}: {ClassPlacementNumber+11}"; } } //{ClassPlacementNumber + 11}"; } }

        public void SetEmptyClassEnrollmentModel()
        {
            if (ClassEnrollment == null)
            {
                ClassEnrollment = new ClassEnrollmentModel();
                ClassEnrollment.UserSubscription = new UserSubscriptionModel();
            }
        }

        public string BGColor
        {
            get
            {
                switch (StudioPlacementId)
                {
                    case 1:
                        return "#ffd196";
                    case 2:
                        return "#fcffc4";
                    case 999:
                        return "#dbd1c9";
                    case 1003:
                        return "#dbd1c9";
                    case 1007:
                        return "#dbd1c9";
                    default:
                        return "";
                }
            }
        }

    }

    public class CalendarAvailablePlacementsModel
    {
        //public AvailablePlacementsModel()
        //{
        //    if (!IsDeleted) ToCreate = true;
        //}
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int StudioPlacementId { get; set; }
        public string StudioPlacementName { get; set; }
        public byte ClassPlacementNumber { get; set; }
        public bool IsInUse { get; set; }
        public bool IsDeleted { get; set; }
        public int TypeId { get; set; }

        public bool ToCreate { get; set; }

        //public ClassEnrollmentModel ClassEnrollment { get; set; }


        public string DisplyName { get { return StudioPlacementId != 999 && StudioPlacementId != 1003 && StudioPlacementId != 1007 ? $"{StudioPlacementName}: {ClassPlacementNumber}" : $"{StudioPlacementName}: {ClassPlacementNumber + 11}"; } } //{ClassPlacementNumber + 11}"; } }

        

        public string BGColor
        {
            get
            {
                switch (StudioPlacementId)
                {
                    case 1:
                        return "#ffd196";
                    case 2:
                        return "#fcffc4";
                    case 999:
                        return "#dbd1c9";
                    case 1003:
                        return "#dbd1c9";
                    case 1007:
                        return "#dbd1c9";
                    default:
                        return "";
                }
            }
        }

    }

}
