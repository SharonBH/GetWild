using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Internal;
using BLL.SMSService;
using DAL;
using Elmah;
using InShapeModels;
using Utilities;

namespace BLL
{
    public static class SMSBLL
    {
        //private const string Username = "test35";
        //private const string Password = "789789";
        //private const int AccountId = 353;
        //private const string AccountName = "itnewslettrSMS";

        private static bool SendSMS(string numbers, string msg, string userid, string from, MessageType type, int companyId, bool UseSMS, bool log = true, bool isTest = false)
        {
            var settings = SmsRepo.GetSettings(companyId);
            //var numbers = new StringBuilder();
            //foreach (var user in users)
            //{
            //    numbers.Append(user.PhoneNumber + ",");
            //}
            //numbers.Remove(numbers.Length - 1, 1);
            // DateTime.Now.ToString()
            //YYYY/MM/DD HH:MM:SS
            var smsSoapClient = new WebServiceSMSSoapClient();
            int sent;
            if (!UseSMS || isTest) return true;
            if (!string.IsNullOrEmpty(from)) settings.DisplyName = from;
            try
            {
                var result = smsSoapClient.sendSMSrecipients(settings.UserName, settings.Password, settings.AccountId, settings.AccountName,string.Empty,
                    settings.DisplyName, numbers.TrimEnd(','), msg, string.Empty);
                if (log) SmsRepo.LogSMS(userid, msg, result, type);
                Int32.TryParse(result, out sent);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                Logger.WriteError($"SendSMS Error, user: {userid}, msg: {msg}", ex);
                return false;
            }
            

            return sent > 0;
        }

        public static string SentTestSMS(string sender, string msg, string recipient, int CompanyId)
        {
            var settings = SmsRepo.GetSettings(CompanyId);
            var smsSoapClient = new WebServiceSMSSoapClient();
            if (!string.IsNullOrEmpty(sender)) settings.DisplyName = sender;
            return smsSoapClient.sendSMSrecipients(settings.UserName, settings.Password, settings.AccountId, settings.AccountName, string.Empty,
                    settings.DisplyName, recipient, msg, string.Empty);
        }

        public static int GetSMSBalance(int CompanyId)
        {
            var settings = SmsRepo.GetSettings(CompanyId);
            var smsSoapClient = new WebServiceSMSSoapClient();
            int balance = 0;
            try
            {
                var result = smsSoapClient.getSmsCount(settings.UserName, settings.Password, settings.AccountId, settings.AccountName,string.Empty);
                if (int.TryParse(result, out balance)) SmsRepo.UpdateBalance(balance);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                Logger.WriteError($"GetSMSBalance Error", ex);
                balance = 0;
            }
            return balance;
        }

        public static MSGType GetMessageByType(MessageType type, int CompanyId)
        {
            return SmsRepo.GetMessageByType(type, CompanyId);
        }

        public static bool UpdateMessage(MSGType msg)
        {
            return SmsRepo.UpdateMessage(msg);
        }

        public static Task<string> SendSMSAsync(string numbers, string msg, string userid, int CompanyId)
        {
            var settings = SmsRepo.GetSettings(CompanyId);
            var smsSoapClient = new WebServiceSMSSoapClient();
            var result = smsSoapClient.sendSMSrecipientsAsync(settings.UserName, settings.Password, settings.AccountId,
                settings.AccountName,
                string.Empty,
                settings.DisplyName, numbers.TrimEnd(','), msg, string.Empty);
            
            return result;
        }


        //public static bool SendOnEnrollmentAlert(string userid, int classid)
        //{
        //    var user = UserBll.GetUser(userid);
        //    var cls = StudioBLL.GetClasses(classid, false).FirstOrDefault();
        //    var numbers = user.PhoneNumber;
        //    var msg = string.Format("{0}, הנך רשום לאימון {2}, ב {1}", user.FullName, cls.Date.ToString("g"), cls.Name);
        //    return SendSMS(numbers, msg, userid) == 1;
        //}

        //public static bool SendOnOutrollmentAlert(string userid, int classid)
        //{
        //    var user = UserBll.GetUser(userid);
        //    var cls = StudioBLL.GetClasses(classid, false).FirstOrDefault();
        //    var numbers = user.PhoneNumber;
        //    var msg = string.Format("{0}, רישום לאימון: {2}, ב {1} - בוטל", user.FullName, cls.Date.ToString("g"), cls.Name);
        //    return SendSMS(numbers, msg, userid) == 1;
        //}


        //public static bool SendBeforeStartAlert(StudioClassModel cls)
        //{
        //    var classenrollments = ClassRepo.GetEnrollmentsByClass(cls.Id);
        //    var i = 0;
        //    foreach (var user in classenrollments)
        //    {
        //        var msg = string.Format("{0}, הנך רשום לאימון {2}, ב {1}", user.UserSubscription.FullName, cls.Date.ToString("g"), cls.Name);
        //        i = i + SendSMS(user.UserSubscription.PhoneNumber, msg, user.UserSubscription.UserId);
        //    }
        //    var result = i == classenrollments.Count;
        //    return result;
        //}

        public static void SendSMSBeforeStart()
        {
            //var localdate = Utils.ConvertNowToLocal();
            foreach (var company in App.Companies)
            {
                var data = ClassRepo.GetEnrollmentsToSendAlert(company.Id);
                Logger.WriteInfo($"running SendSMSBeforeStart ({company.Id}): {data.EnrollmentModels.Count}");
                foreach (var user in data.EnrollmentModels)
                {
                    var msg = PersonalizeNSG(data.Message, user, company.WebSiteURL);
                    if (SendSMS(user.UserSubscription.PhoneNumber, msg, user.UserSubscription.UserId, string.Empty, MessageType.BeforeStart, company.Id, company.UseSMS))
                        ClassRepo.UpdateSmsSent(user.Id);
                }
            }
            
        }

        public static bool SendSMSWelcome(UserSMSSubscriptionModel user, int CompanyId, string WebSiteURL, bool UseSMS)
        {
            //var localdate = Utils.ConvertNowToLocal();
            Logger.WriteInfo($"running SendSMSWelcome: {user.PhoneNumber}");
            var msg = SystemRepo.GetMessageForTypeCompany(MessageType.Welcome, CompanyId);
            msg.Message = msg.Message.Replace("[[שם]]", user.FirstName)
                .Replace("[[משתמש]]", user.Username)
                .Replace("[[סיסמא]]", user.Password)
                .Replace("[[לינק-אתר]]", WebSiteURL);
            if (!SendSMS(user.PhoneNumber, msg.Message, user.UserId, string.Empty, MessageType.Welcome, CompanyId, UseSMS)) return false;
            ClassRepo.UpdateSmsSent(user.Id);
            return true;
        }

        private static object thisLock = new object();
        public static void SendMassSms(int interval)
        {
            lock (thisLock)
            {
                foreach (var company in App.Companies)
                {
                    var msgs = SmsRepo.GetMassSms(interval, company.Id);
                    if (msgs.Count == 0) return;
                    Logger.WriteInfo($"running SendMassSms: {msgs.Count}");
                    foreach (var msg in msgs)
                    {
                        var numbers = msg.Recipients;
                        if (SendSMS(numbers, msg.Message, msg.UserId, msg.Sender, MessageType.MassSMS, company.Id, company.UseSMS))
                            SmsRepo.UpdateSmsSent(msg);
                    }
                }
            }
        }

        //internal static void SendWaitingListMSG(int classId)
        //{
        //    var data = ClassRepo.GetWaitingListToSendAlert(classId);
        //    Logger.WriteInfo($"running SendWaitingListMSG: {data.EnrollmentModels.Count}, Class: {classId}");
        //    foreach (var user in data.EnrollmentModels)
        //    {
        //        var msg = PersonalizeNSG(data.Message, user);
        //        if (SendSMS(user.UserSubscription.PhoneNumber, msg, user.UserSubscription.UserId, string.Empty,
        //            MessageType.WaitList))
        //        {
        //            ClassRepo.UpdateWaitingListSmsSent(user.Id);
        //            Logger.WriteInfo($"running SendWaitingListMSGFor Class:SubscriptionId: {user.SubscriptionId}, Class: {user.ClassId}");
        //        }
        //    }

        //}

        public static void SendWaitingListSMS(bool isTest = false)
        {
            //var msg = SystemRepo.GetMessageForType(MessageType.WaitList);
            //if (!msg.Active) return;
            // Delete from waiting list after time threshold or broadcast
            Logger.WriteInfo($"running SendWaitingListSMS (compaines): {App.Companies.Count}");
            foreach (var company in App.Companies)
            {
                ClassRepo.RemoveFromWatingListBeforeSMS(company.Id);
                var data = ClassRepo.GetWaitingListToSendAlert(false, company.Id, company.PriorityWaitListDays);
                Logger.WriteInfo($"running SendWaitingListSMS (company:{company.Id}): {data.EnrollmentModels.Count}");

                foreach (var user in data.EnrollmentModels.Where(x => !x.IsSmsSent))
                {
                    var sms = PersonalizeNSG(data.Message, user, company.WebSiteURL);
                    if (SendSMS(user.UserSubscription.PhoneNumber, sms, user.UserSubscription.UserId, string.Empty,
                        MessageType.WaitList, company.Id, company.UseSMS, !isTest, isTest))
                    {
                        ClassRepo.UpdateWaitingListSmsSent(user.Id);
                        Logger.WriteInfo(
                            $"running SendWaitingListSMS:SubscriptionId: {user.SubscriptionId}, Class: {user.ClassId}");
                    }
                }
                var dataBroadcast = ClassRepo.GetWaitingListToSendAlert(true, company.Id, company.PriorityWaitListDays);
                Logger.WriteInfo($"running SendWaitingListSMS (Broadcast): {dataBroadcast.EnrollmentModels.Count}");

                foreach (var user in dataBroadcast.EnrollmentModels)
                {
                    var sms = PersonalizeNSG(dataBroadcast.Message, user, company.WebSiteURL);
                    if (SendSMS(user.UserSubscription.PhoneNumber, sms, user.UserSubscription.UserId, string.Empty,
                        MessageType.BroadcastWaitList, company.Id, company.UseSMS, !isTest, isTest))
                    {
                        ClassRepo.UpdateWaitingListSmsSent(user.Id, true);
                        Logger.WriteInfo(
                            $"running SendWaitingListSMS (Broadcast):SubscriptionId: {user.SubscriptionId}, Class: {user.ClassId}");
                    }
                }
            }
            
        }

        public static void SendBeforeExpireAlert()
        {
            foreach (var company in App.Companies)
            {
                var msg = SystemRepo.GetMessageForTypeCompany(MessageType.Expire, company.Id);
                if (msg.Active)
                {
                    var data = UserBll.GetAboutToExpireSubscriptions(company.Id, true);
                    Logger.WriteInfo($"running SendBeforeExpireAlert: {data.AboutToExpireSubscriptionModels.Count}");
                    foreach (var Subscription in data.AboutToExpireSubscriptionModels)
                    {
                        var numbers = Subscription.PhoneNumber;
                        var sms = msg.Message.Replace("[[שם]]", Subscription.FirstName);
                        SendSMS(numbers, sms, Subscription.UserId, string.Empty, MessageType.Expire, company.Id, company.UseSMS);
                    }
                }
            }
        }

        public static void SendInstructorReminder()
        {
            foreach (var company in App.Companies)
            {
                var msg = SystemRepo.GetMessageForTypeCompany(MessageType.InstructorReminder, company.Id);
                if (msg.Active)
                {
                    var data = UserBll.GetInstructorWithClasses(DateTime.UtcNow.ToLocal().Date.AddDays(1), company.Id); 
                    Logger.WriteInfo($"running SendInstructorReminder: {data.Count}");
                    foreach (var instructor in data)
                    {
                        var numbers = instructor.Instructor.PhoneNumber;
                        var sms = msg.Message.Replace("[[שם]]", instructor.Instructor.FirstName).Replace("[[אימונים]]", instructor.ClassesText);
                        SendSMS(numbers, sms, instructor.Instructor.UserId, string.Empty, MessageType.InstructorReminder, company.Id, company.UseSMS, true);
                    }
                }
            }
        }

        public static void SendInactiveAlert()
        {
            foreach (var company in App.Companies)
            {
                var msg = SystemRepo.GetMessageForTypeCompany(MessageType.Inactive, company.Id);
                if (msg.Active)
                {
                    var data = UserBll.GetInactiveSubscriptions(company.Id);
                    Logger.WriteInfo($"running SendInactiveAlert: {data.AboutToExpireSubscriptionModels.Count}");
                    foreach (var Subscription in data.AboutToExpireSubscriptionModels)
                    {
                        var numbers = Subscription.PhoneNumber;
                        var sms = msg.Message.Replace("[[שם]]", Subscription.FirstName);
                        SendSMS(numbers, sms, Subscription.UserId, string.Empty, MessageType.Expire, company.Id, company.UseSMS);
                    }
                }
            }
        }

        public static List<SmsLog> GetSmsLog(DateTime date, int StudioId)
        {
            return SmsRepo.GetSmsLog(date, StudioId);
        }



        public static bool UpdateSenderNumber(string SenderNumber, int CompanyId)
        {
            return SmsRepo.UpdateSenderNumber(SenderNumber, CompanyId);
        }

        public static bool UpdateMassSMS(SmsSenderModel model, int StudioId)
        {
            //model.SmsMessage = ConfigMessage(model.SmsMessage);
            try
            {
                model.Source = configSource(model.RefId, model.ListType);
                if (!model.SendNow && model.SendDate.HasValue)
                    model.SendDate = model.SendDate.Value.ToUniversalTime(); //Utils.Utils.ConvertLocalDateTimeToUTC(model.SendDate.Value);
                if (SmsRepo.UpdateMassSMS(model,StudioId) && model.SendNow) SendMassSms(5);
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
                //if (result && model.SendNow)
                //{
                //    ErrorLog.GetDefault(null).Log(new Error(new Exception("running MassSMS")));
                //    if (model.IsBulk)
                //    {
                //        var msg = model.SmsMessage;
                //        if (model.ListType == SmsListType.ByClass)
                //        {
                //            var id = int.Parse(model.RefId);
                //            var c = StudioBLL.GetClasses(id, true).FirstOrDefault();
                //            msg = msg.Replace("[[אימון]]", c.Name).Replace("[[זמן]]", c.Date.ToString("HH:mm"));
                //        }
                //        SendSMS(model.Recipients, msg, model.UserId, model.Sender);
                //    }
                //    else
                //    {
                //        foreach (var user in model.Users)
                //        {
                //            var numbers = user.PhoneNumber;
                //            var sms = model.SmsMessage.Replace("[[שם]]", user.FullName);
                //            SendSMS(numbers, sms, model.UserId, model.Sender);
                //        }
                //    }
                //}
        }

        public static string PersonalizeNSG(string Message, ClassEnrollmentModel user, string WebSiteURL)
        {
            CultureInfo c = new CultureInfo("he-IL");
            var msg = Message.Replace("[[שם]]", user.UserSubscription.FirstName)
                .Replace("[[לינק]]", user.Class.ShortURL) // add today / tomorrow
                .Replace("[[אימון]]", user.Class.Name)
                .Replace("[[תאריך]]", user.Class.Date.ToString("M"))
                .Replace("[[יום]]", c.DateTimeFormat.DayNames[(int)user.Class.Date.DayOfWeek])
                .Replace("[[זמן]]", user.Class.Date.ToString("HH:mm"))
                //    .Replace("[[משתמש]]", user.Username)
                //.Replace("[[סיסמא]]", user.Password)
                .Replace("[[לינק-אתר]]", WebSiteURL); //"http://www.getwild.co.il");;
            if (user.SelectedPlacement != null) msg = !string.IsNullOrEmpty(user.SelectedPlacement.StudioPlacementName)? msg.Replace("[[מיקום]]", user.SelectedPlacement.DisplyName) : msg.Replace("[[מיקום]]", "");
            else msg = msg.Replace("[[מיקום]]", "");
            if (user.Class.Date.Date == DateTime.UtcNow.ToLocal().Date.AddDays(1)) msg = msg.Replace("היום", "מחר");
            return msg;
        }

        private static string configSource(string refId, SmsListType listType)
        {
            switch (listType)
            {
                case SmsListType.All:
                    return "כל השתמשים";
                case SmsListType.Active:
                    return "כל השתמשים הפעילים";
                case SmsListType.Inactive:
                    return "כל השתמשים הלא פעילים";
                case SmsListType.ByClass:
                    return "רשומים לאימון";
                case SmsListType.ByDay:
                    return "רשומים ליום";
                case SmsListType.UserReport:
                    return "דוח משתמשים";
                case SmsListType.WeeklyReport:
                    return "דוח שבועי";
                case SmsListType.NoEnrollmentReport:
                    return "דוח עצלנים";
                default:
                    throw new ArgumentOutOfRangeException(nameof(listType), listType, null);
            }
        }

        
    }
}
