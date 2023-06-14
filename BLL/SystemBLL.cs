using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InShapeModels;
using DAL;
using Utilities;

namespace BLL
{
    public static class SystemBLL
    {
        //readonly ClassRepo _classRepo = new ClassRepo();
        public static List<SysMessage> GetMessagesForUser(string userId)
        {
            var msgs = SystemRepo.GetMessagesForUser(userId);
            var messages = new List<SysMessage>();
            if (msgs.Any()) msgs.ForEach(x => messages.Add(new SysMessage { Id = x.Id, Date = x.Date, Message = x.Message, TypeId = x.TypeId.Value}));
            return messages;
        }


        public static bool ReadMessage(int messageId)
        {
            return SystemRepo.ReadMessage(messageId);
        }

        public static bool ReadMessages(string userId)
        {
            return SystemRepo.ReadMessages(userId);
        }

        public static bool ProcessMarked(int companyId)
        {
            return SystemRepo.ProcessMarked(companyId);
        }

        public static CompanyConfiguration GetCompany(string url)
        {
            //var c = new Utils.CompanyConfiguration();
            return SystemRepo.GetCompany(url);
        }

        public static List<CompanyConfiguration> GetCompanies()
        {
            //var c = new Utils.CompanyConfiguration();
            return SystemRepo.GetCompanies();
        }

        public static List<CompanyConfiguration> GetCompanies2()
        {
            //var c = new Utils.CompanyConfiguration();
            return SystemRepo.GetCompanies2();
        }

    }
}
