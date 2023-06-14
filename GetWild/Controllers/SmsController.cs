using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AutoMapper;
using BLL;
using DAL;
using GetWild.Models;
using InShapeModels;

namespace GetWild.Controllers
{
    [Authorize(Roles = "Instructor, admin")]
    public class SmsController : InShapeMVCController
    {
        // GET: Smsing
        public ActionResult Index()
        {
            ViewBag.SMSCount = SMSBLL.GetSMSBalance(CurrentCompany.Id);
            ViewBag.WaitingListEnabled = CurrentCompany.WaitingListEnabled;
            return View();
        }

        public ActionResult EditMessage(int tid)
        {
            var model = SMSBLL.GetMessageByType((MessageType)tid, CurrentCompany.Id);
            return model != null ? PartialView("MessageEdit", model) : null;
        }

        [HttpPost]
        public ActionResult EditMessage(MSGType model)
        {
            var result = SMSBLL.UpdateMessage(model);
            return Json(result ? new { Response = "Success", Message = "עודכן בהצלחה!." } : new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." });
        }

        [HttpPost]
        public ActionResult UpdateSenderNumber(string SenderNumber)
        {
            var result = SMSBLL.UpdateSenderNumber(SenderNumber, CurrentCompany.Id);
            return Json(result ? new { Response = "Success", Message = "עודכן בהצלחה!." } : new { Response = "Error", Message = "לא ניתן לשמור שינויים, אנא נסה מאוחר יותר." });
        }

        public ActionResult SendTestMSG()
        {
            return PartialView("SmsTester");
        }

        public ActionResult SmsSender(string smsType ,int? refid, string date, int usertype = 0, int? weekno = null)
        {
            var SmsType = (SmsListType)Enum.Parse(typeof(SmsListType), smsType);
            DateTime? dt1 = null;
            if (!string.IsNullOrEmpty(date)) dt1 = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var model = new AdvancedSmsViewModel();
            model.ListType = SmsType;
            switch (SmsType)
            {
                case SmsListType.All:
                    model.Users = Mapper.Map<List<UserSMSSubscriptionModel>>(UserBll.GetUsersWithSubscription(CurrentUser.StudioId));
                    break;
                case SmsListType.Active:
                    model.Users =
                        Mapper.Map<List<UserSMSSubscriptionModel>>(
                            UserBll.GetUsersWithSubscription(CurrentUser.StudioId).Where(x => x.Active.HasValue && x.Active.Value));
                    break;
                case SmsListType.Inactive:
                    model.Users =
                        Mapper.Map<List<UserSMSSubscriptionModel>>(
                            UserBll.GetUsersWithSubscription(CurrentUser.StudioId).Where(x => !x.Active.HasValue || !x.Active.Value));
                    break;
                case SmsListType.Frozen:
                    model.Users =
                        Mapper.Map<List<UserSMSSubscriptionModel>>(
                            UserBll.GetUsersWithSubscription(CurrentUser.StudioId).Where(x => x.Frozen));
                    break;
                case SmsListType.ByClass:
                    model.Users =
                        Mapper.Map<List<UserSMSSubscriptionModel>>(UserBll.GetUsersWithSubscriptionForRef(refid, null, CurrentUser.StudioId));
                    break;
                case SmsListType.ByDay:
                    model.Users =
                        Mapper.Map<List<UserSMSSubscriptionModel>>(UserBll.GetUsersWithSubscriptionForRef(null, dt1, CurrentUser.StudioId));
                    break;
                case SmsListType.UserReport:
                    model.Users = Mapper.Map<List<UserSMSSubscriptionModel>>(UserBll.GetUsersReport(3, CurrentUser.StudioId));
                    break;
                case SmsListType.WeeklyReport:
                    model.Users =
                        Mapper.Map<List<UserSMSSubscriptionModel>>(
                            UserBll.GetUsersWithSubscription(refid ?? 0, CurrentUser.StudioId, weekno ?? -1).UserWithSubscriptions.Where(x => x.Active.HasValue && x.Active.Value));
                    break;
                case SmsListType.NoEnrollmentReport:
                    model.Users = Mapper.Map<List<UserSMSSubscriptionModel>>(UserBll.GetUsersWithNoEnrollments(CurrentUser.StudioId).UserWithSubscriptions);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (usertype <= 0) return View(model);
            var selectedtype = (UserType)usertype;
            model.Users.RemoveAll(x => x.UserType != selectedtype);
            return View(model);
        }

        [HttpPost]
        public ActionResult SmsSender(AdvancedSmsViewModel model)
        {
            var Sender = Mapper.Map<SmsSenderModel>(model);
            Sender.UserId = CurrentUser.Id;
            var result = SMSBLL.UpdateMassSMS(Sender, CurrentUser.StudioId);
            ViewBag.Message = result ? "ההודעות נשלחו בהצלחה!" : "לא ניתן לשלוח את ההודעות!";
            return View();
        }


        [HttpPost]
        public ActionResult SendTestMSG(SmsTesterViewModel model)
        {
            var result = SMSBLL.SentTestSMS(model.Sender, model.Message, model.Recipient, CurrentCompany.Id);
            return Json(new {Response = "Success", Message = result});
        }
    }
}