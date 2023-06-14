using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using InShapeModels;
using Utilities;

namespace DAL
{
    public static class SubscriptionTypeRepo
    {
        public static List<SubscriptionTypeModel> GetTypes(int typeId, int StudioId)
        {
            using (var context = new InShapeEntities())
            {
                return typeId > 0
                    ? context.SubscriptionTypes.Where(r => r.Id == typeId && !r.IsDeleted).ProjectTo<SubscriptionTypeModel>().ToList()
                    : context.SubscriptionTypes.Where(r => !r.IsDeleted).FilterByUser(StudioId).ProjectTo<SubscriptionTypeModel>().ToList();
            }
        }

        public static bool CreateType(SubscriptionType type)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    context.SubscriptionTypes.Add(type);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }


        public static bool UpdateType(SubscriptionType type)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    context.SubscriptionTypes.Attach(type);
                    context.Entry(type).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public static bool DeleteType(int typeId)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var type = context.SubscriptionTypes.First(r => r.Id == typeId && !r.IsDeleted);
                    //context.StudioRooms.Remove(room);
                    type.IsDeleted = true;
                    context.SubscriptionTypes.Attach(type);
                    context.Entry(type).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }


    }
}
