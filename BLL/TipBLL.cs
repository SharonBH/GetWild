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
    public static class TipBLL
    {
        //readonly ClassRepo _classRepo = new ClassRepo();
        public static List<TipModel> GetTips(int id, int StudioId)
        {
            //var tips = TipRepo.GetTips(id, StudioId);
            //var Tipmodel = new List<TipModel>();
            //if (tips.Any()) tips.ForEach(x => Tipmodel.Add(new TipModel { Id = x.Id, Tip = x.Short }));
            return Mapper.Map<List<TipModel>>(TipRepo.GetTips(id, StudioId));
        }

        public static bool CreateTip(TipModel Tipmodel)
        {
            var tip = Mapper.Map<Tip>(Tipmodel);
            return TipRepo.CreateTip(tip);
        }

        public static bool UpdateTip(TipModel Tipmodel)
        {
            var tip = Mapper.Map<Tip>(Tipmodel);
            return TipRepo.UpdateTip(tip);
        }

        public static bool DeleteTip(int tipId)
        {
            return TipRepo.DeleteTip(tipId);
        }

    }
}
