using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using InShapeModels;
using DAL;

namespace BLL
{
    public static class SubscriptionTypeBLL
    {
        //readonly ClassRepo _classRepo = new ClassRepo();
        public static List<SubscriptionTypeModel> GetTypes(int id, int StudioId, bool includeEmpty = false)
        {
            var types = SubscriptionTypeRepo.GetTypes(id, StudioId);
            if (includeEmpty) types.Add(new SubscriptionTypeModel { Id = -1, Name = "ללא מנוי" });
            return types;
        }

        public static bool CreateType(SubscriptionTypeModel Typemodel)
        {
            return SubscriptionTypeRepo.CreateType(Mapper.Map<SubscriptionType>(Typemodel));
        }

        public static bool UpdateType(SubscriptionTypeModel Typemodel)
        {
            return SubscriptionTypeRepo.UpdateType(Mapper.Map<SubscriptionType>(Typemodel));
        }

        public static bool DeleteType(int tipId)
        {
            return SubscriptionTypeRepo.DeleteType(tipId);
        }

    }
}
