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
    public static class SmsRepo
    {
        public static SMSSetting GetSettings(int CompanyId)
        {
            using (var context = new InShapeEntities())
            {
                return context.SMSSettings.FirstOrDefault(x=>x.CompanyId == CompanyId);
            }
        }

        public static MSGType GetMessageByType(MessageType type, int CompanyId)
        {
            using (var context = new InShapeEntities())
            {
                return context.MSGTypes.FirstOrDefault(x => x.MessageTypeId == (int)type && x.CompanyId == CompanyId);
            }
        }

        public static bool UpdateMessage(MSGType msg)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    context.MSGTypes.Attach(msg);
                    context.Entry(msg).State = EntityState.Modified;
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


        public static void UpdateBalance(int balance)
        {
            using (var context = new InShapeEntities())
            {
                var settings = context.SMSSettings.FirstOrDefault();
                if (settings == null) return;
                settings.Balance = balance;
                context.SMSSettings.Attach(settings);
                context.Entry(settings).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static bool UpdateSenderNumber(string SenderNumber, int CompanyId)
        {
            using (var context = new InShapeEntities())
            {
                var settings = context.SMSSettings.FirstOrDefault(x=>x.CompanyId == CompanyId);
                if (settings == null) return false;
                settings.DisplyName = SenderNumber;
                context.SMSSettings.Attach(settings);
                context.Entry(settings).State = EntityState.Modified;
                context.SaveChanges();
            }
            return true;
        }

        public static bool UpdateMassSMS(SmsSenderModel model, int StudioId)
        {
            if (model.ListType == SmsListType.ByClass)
            {
                var id = int.Parse(model.RefId);
                var c = StudioRepo.GetClasses(id, true, StudioId).FirstOrDefault();
                model.SmsMessage = model.SmsMessage.Replace("[[אימון]]", c.Name)
                    .Replace("[[זמן]]", c.Date.ToString("HH:mm"));
            }
            if (model.IsBulk)
            {
                SmsSender bulk = new SmsSender
                {
                    Date = DateTime.UtcNow.ToLocal(),
                    RefId = model.RefId,
                    SendDate = model.SendNow ? DateTime.UtcNow.ToLocal() : model.SendDate.Value,
                    Sender = model.Sender,
                    Sent = false,
                    UserId = model.UserId,
                    Recipients = model.Recipients,
                    Source = model.Source,
                    Message = model.SmsMessage
                };
                using (var context = new InShapeEntities())
                {
                    context.SmsSenders.Attach(bulk);
                    context.Entry(bulk).State = EntityState.Added;
                    context.SaveChanges();
                    return true;
                }
            }
            using (var context = new InShapeEntities())
            {
                foreach (var user in model.Users.Where(x=>x.Selected))
                {
                    SmsSender personal = new SmsSender
                    {
                        Date = DateTime.UtcNow.ToLocal(),
                        RefId = model.RefId,
                        SendDate = model.SendNow ? DateTime.UtcNow.ToLocal() : model.SendDate.Value,
                        Sender = model.Sender,
                        Sent = false,
                        UserId = model.UserId,
                        Recipients = user.PhoneNumber,
                        Source = model.Source,
                        Message = ConfigMessage(model.SmsMessage, user)
                    };

                    context.SmsSenders.Attach(personal);
                    context.Entry(personal).State = EntityState.Added;
                }
                context.SaveChanges();
            }
            return true;
        }

        //public static bool UpdateSmsSent(SmsSenderModel model)
        //{
        //    using (var context = new InShapeEntities())
        //    {
        //        var sms = context.SmsSenders.FirstOrDefault(x=>x.)
        //    }
        //}

        public static List<SmsSender> GetMassSms(int interval, int CompanyId)
        {
            var from = DateTime.UtcNow.ToLocal().AddMinutes(-interval);
            var to = DateTime.UtcNow.ToLocal();
            using (var context = new InShapeEntities())
            {
                return context.SmsSenders.Where(x => !x.Sent && x.SendDate >= from && x.SendDate <= to && x.AspNetUser.Studio.CompanyId == CompanyId).ToList();
            }
        }

        private static string ConfigMessage(string smsMessage, UserSMSSubscriptionModel Subscription)
        {
            return smsMessage.Replace("[[שם]]", Subscription.FirstName);
        }

        public static bool UpdateSmsSent(SmsSender msg)
        {
            try
            {
                msg.Sent = true;
                using (var context = new InShapeEntities())
                {
                    context.SmsSenders.Attach(msg);
                    context.Entry(msg).State = EntityState.Modified;
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

        public static bool LogSMSs(List<AspNetUser> users, string sms)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    foreach (var user in users)
                    {
                        UserSMSs smslog = new UserSMSs { UserId = user.Id, Date = DateTime.UtcNow.ToLocal(), SMS = sms };
                        context.UserSMSses.Attach(smslog);
                        context.Entry(smslog).State = EntityState.Added;
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

        public static bool LogSMS(string userid, string sms, string result, MessageType type)
        {
            try
            {
                using (var context = new InShapeEntities())
                {

                    var smslog = new UserSMSs { UserId = userid, Date = DateTime.UtcNow.ToLocal(), SMS = sms, Response = result, TypeId = (int)type };
                    context.UserSMSses.Attach(smslog);
                    context.Entry(smslog).State = EntityState.Added;
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


        public static List<SmsLog> GetSmsLog(DateTime date, int StudioId)
        {
            var todate = date.AddDays(1);
            using (var context = new InShapeEntities())
            {
                return
                    context.UserSMSses.Where(s => s.Date >= date && s.Date < todate && s.AspNetUser.StudioId == StudioId)
                    .ProjectTo<SmsLog>().OrderByDescending(x=>x.Date).ToList();
            }
        }


    }
}
