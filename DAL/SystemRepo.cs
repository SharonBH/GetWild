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
    public static class SystemRepo
    {
        public static List<SysAlert> GetMessagesForUser(string userId)
        {
            using (var context = new InShapeEntities())
            {
                return context.SysAlerts.Where(r => r.UserId == userId && !r.IsRead).ToList();
            }
        }


        public static bool ReadMessage(int messageId)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var alert = context.SysAlerts.First(r => r.Id == messageId && !r.IsRead);
                    alert.IsRead = true;
                    context.SysAlerts.Attach(alert);
                    context.Entry(alert).State = EntityState.Modified;
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

        public static bool ReadMessages(string userid)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var alerts = context.SysAlerts.Where(r => r.UserId == userid);
                    //context.StudioRooms.Remove(room);
                    foreach (var alert in alerts)
                    {
                        alert.IsRead = true;
                        context.SysAlerts.Attach(alert);
                        context.Entry(alert).State = EntityState.Modified;
                    }
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

        public static bool ProcessMarked(int companyId)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var comp = context.Companies.FirstOrDefault(r => r.Id == companyId);
                    //context.StudioRooms.Remove(room);
                    if(comp != null)
                    {
                        comp.RemoveMarked = true;
                        context.Entry(comp).State = EntityState.Modified;
                        context.SaveChanges();
                        App.Companies = GetCompanies();
                        App.SetDefaultCompany();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }


        //public static MSGType GetMessageForType(MessageType type, int StudioId)
        //{
        //    using (var context = new InShapeEntities())
        //    {
        //        return context.MSGTypes.FilterByUser(StudioId).FirstOrDefault(x => x.MessageTypeId == (int)type);

        //    }
        //}

        public static MSGType GetMessageForTypeCompany(MessageType type, int companyId)
        {
            using (var context = new InShapeEntities())
            {
                return context.MSGTypes.FirstOrDefault(x => x.MessageTypeId == (int)type && x.CompanyId == companyId);

            }
        }

        public static CompanyConfiguration GetCompany(string url)
        {
            using (var context = new InShapeEntities())
            {
                var company = context.Companies.Where(x => x.WebSiteURL.ToLower().Equals(url.ToLower()))
                    .ProjectTo<CompanyConfiguration>().FirstOrDefault();
                return company;
            }
        }

        public static List<CompanyConfiguration> GetCompanies()
        {
            using (var context = new InShapeEntities())
            {
                var companies = context.Companies.Include("Studio").Where(x => !x.IsDeleted && x.Active)
                    .ProjectTo<CompanyConfiguration>().ToList();
                Logger.WriteInfo("Getting Companies:" + companies.Count());
                return companies;
            }
        }


        public static List<CompanyConfiguration> GetCompanies2()
        {
            using (var context = new InShapeEntities())
            {
                var companies = context.Database.SqlQuery<CompanyConfiguration>("GetCompanies").AsQueryable().ToList();

                Logger.WriteInfo("Getting Companies");
                return companies;
            }
        }
        
    }
}

