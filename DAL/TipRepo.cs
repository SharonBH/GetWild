using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace DAL
{
    public static class TipRepo
    {
        public static List<Tip> GetTips(int tipId, int StudioId)
        {
            using (var context = new InShapeEntities())
            {
                return tipId > 0 ? context.Tips.Where(r => r.Id == tipId && !r.IsDeleted).ToList() : context.Tips.Where(r => !r.IsDeleted).FilterByUser(StudioId).ToList();
            }
        }

        public static bool CreateTip(Tip tip)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    context.Tips.Add(tip);
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


        public static bool UpdateTip(Tip tip)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    context.Tips.Attach(tip);
                    context.Entry(tip).State = EntityState.Modified;
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

        public static bool DeleteTip(int tipId)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var tip = context.Tips.First(r => r.Id == tipId && !r.IsDeleted);
                    //context.StudioRooms.Remove(room);
                    tip.IsDeleted = true;
                    context.Tips.Attach(tip);
                    context.Entry(tip).State = EntityState.Modified;
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
