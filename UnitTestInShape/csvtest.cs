using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL;
using CsvHelper;
using CsvHelper.Configuration;
using DAL;
using GetWild;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;

namespace UnitTestInShape
{
    [TestClass]
    public class csvtest
    {
        public csvtest()
        {
            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperConfig>());
            App.Companies = SystemBLL.GetCompanies();
            //App.SetCurrentCompanybyID(7);
        }

        [TestMethod]
        public void savecsv1()
        {
            var users = UserBll.GetUsersWithSubscription2(5,true, 1);
            using (TextWriter writer = new StreamWriter($@"C:\users_{DateTime.UtcNow.Date.ToLocal().ToString("yyyyMMdd")}.csv", false, Encoding.UTF8))
            {
                var csv = new CsvWriter(writer);
                csv.Configuration.RegisterClassMap<MyClassMap>();
                csv.WriteRecords(users.UserWithSubscriptions); // where values implements IEnumerable
            }
        }

    }

    public sealed class MyClassMap : ClassMap<InShapeModels.UserWithSubscription>
    {
        public MyClassMap()
        {

            Map(m => m.FullName).Name("שם");
            Map(m => m.UserType).ConvertUsing(m=> { var type = m.UserType.GetDisplayName(); return type; }).Name("סוג");
            Map(m => m.PhoneNumber).Name("טלפון");
            Map(m => m.SubscriptionExpireDate).TypeConverterOption.Format("dd/MM/yyyy").Name("תפוגת מנוי");
            Map(m => m.PayEndDate).TypeConverterOption.Format("dd/MM/yyyy").Name("סיום תשלום");
            Map(m => m.SubscriptionType).Default("-").Name("מנוי");
            Map(m => m.NumClasses).Default("-").Name("אימונים");
            Map(m => m.ClassesDone).Default("-").Name("נוצל");
            Map(m => m.JoinDate).TypeConverterOption.Format("dd/MM/yyyy").Name("רישום"); //.Default("אין");
            //Map(m => m.SubscriptionStartDate).TypeConverterOption.Format("dd/MM/yyyy");
            Map(m => m.LastClassDate).Default("-").Name("אימון אחרון");
            Map(m => m.LastClassType).Default("-").Name("אימון אחרון");
            Map(m => m.NextClassDate).Default("-").Name("אימון הבא");
            Map(m => m.NextClassType).Default("-").Name("אימון הבא");
        }
        
    }
}
