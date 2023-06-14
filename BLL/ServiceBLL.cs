using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Utilities;

namespace BLL
{
    public static class ServiceBLL
    {
        public static void RunExpireSubscriptions()
        {
            ServiceRepo.RunExpireSubscriptions();
        }

        public static void UpdateClassesURLs(int StudioId)
        {
            StudioRepo.UpdateClassLinks(StudioId);
        }

        public static void UpdateAllClassesURLs()
        {
            foreach (var company in App.Companies)
            {
                foreach (var studio in company.Studios)
                {
                    StudioRepo.UpdateClassLinks(studio.Id);
                }
            }
        }
    }
}
