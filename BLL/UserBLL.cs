using System.Linq.Expressions;
using AutoMapper;
using DAL;
using InShapeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace BLL
{
    public static class UserBll
    {

        //public static ApplicationUser GetUser(string userid)
        //{

        //}

        //readonly UserRepo _userRepo = new UserRepo();
        public static UserProfileModel GetUserProfile(string userId)
        {
            var p = UserRepo.GetUserProfile(userId);
            var profile = Mapper.Map<UserProfileModel>(p);
            profile.OrigWeight = profile.Weight;
            return profile;
        }

        public static UserProfileModel GetFirstUserProfile(string userId)
        {
            var p = UserRepo.GetFirstUserProfile(userId);
            var profile = Mapper.Map<UserProfileModel>(p);
            profile.OrigWeight = profile.Weight;
            return profile;
        }

        public static List<FrozenSubscriptionModel> GetFrozenReport(int StudioId)
        {
            return UserRepo.GetFrozenReport(StudioId);
        }

        public static List<UserProfileModel> GetUserProfileforGraph(string userId)
        {

            return Mapper.Map<List<UserProfileModel>>(UserRepo.GetUserProfileforGraph(userId));
            //var profile = UserRepo.GetUserProfileforGraph(userId);
            //return profile.Select(p => new UserProfileModel {Height = p.Height, Weight = p.Weight, BMI = p.BMI, FatPer = p.Fat, Dim_Hands = p.Dim_Hands, Dim_Thighs = p.Dim_Thighs, Dim_Waist = p.Dim_Waist, Dim_Legs = p.Dim_Legs, LastUpdated = p.Date, UserId = p.UserID, Id = p.Id, Update = false, WeightChange = p.WeightChange}).ToList();
        }


        public static bool UpdateProfile(UserProfileModel profile)
        {
            if (!profile.Date.HasValue || profile.Date.Value.Date != DateTime.UtcNow.ToLocal().Date)
            {
                profile.Id = 0;
                profile.Date = DateTime.UtcNow.ToLocal();
            }
            profile.WeightChange += (profile.OrigWeight - profile.Weight) ?? 0;

            bool result = UserRepo.UpdateUserProfile(Mapper.Map<ProfileTracking>(profile));
            return result;
        }

        public static UserSubscriptionModel GetSubscription(string userId)
        {
            return UserRepo.GetUserSubscriptions(userId);
        }

        public static UserSubscriptionModelForNav GetSubscriptionForUser(string userId)
        {
            return UserRepo.GetUserSubscription(userId);
        }

        public static List<UserWithSubscription> GetUsersWithSubscription(int StudioId)
        {
            return UserRepo.GetUsersWithSubscription(null, StudioId, null, -1, 1, 0, false);
        }

        public static UsersList GetUsersWithSubscription2(int StudioId, bool includefrozen, int pageno, int userType = 0)
        {
            var userlist = UserRepo.GetUsersWithSubscription(null, StudioId, null, -1, pageno, userType, false);
            if (!includefrozen) userlist.RemoveAll(x => x.Frozen);
            return new UsersList { UserWithSubscriptions = userlist, IncludeFrozen = includefrozen, PageNo = pageno };
        }

        public static UsersList GetUsersWithSubscription4Export(int StudioId, bool includefrozen, int pageno, int userType = 0)
        {
            var userlist = UserRepo.GetUsersWithSubscription(null, StudioId, null, 999, pageno, userType, false);
            if (!includefrozen) userlist.RemoveAll(x => x.Frozen);
            return new UsersList { UserWithSubscriptions = userlist, IncludeFrozen = includefrozen, PageNo = pageno };
        }

        public static UsersList GetUsersWithSubscription2(int StudioId, bool frozen, bool? active, int pageno = 1, int userType = 0)
        {
            var userlist = UserRepo.GetUsersWithSubscription(null, StudioId, active, -1, pageno, userType, frozen);
            //userlist.RemoveAll(x => x.Frozen != frozen);
            //else userlist.RemoveAll(x => x.Active != active);
            return new UsersList { UserWithSubscriptions = userlist, IncludeFrozen = frozen, PageNo = pageno };
        }

        public static UsersList GetUsersWithTicketSubscription(int StudioId, bool includefrozen, int pageno, int userType = 0)
        {
            var userlist = UserRepo.GetUsersWithSubscription(null, StudioId, true, -1, -1, userType, false);
            userlist.RemoveAll(x => x.NumClasses < 2 || x.CurrentBalance <= 0);
            if (!includefrozen) userlist.RemoveAll(x => x.Frozen);
            return new UsersList { UserWithSubscriptions = userlist.OrderBy(s=> s.CurrentBalance).ThenBy(s=>s.SubscriptionExpireDate).ToList(), IncludeFrozen = includefrozen, PageNo = pageno };
        }
        

        public static UsersList GetUsersWithSubscriptionforSearch(int StudioId, string search)
        {
            var userlist = UserRepo.GetUsersWithSubscriptionForSearch(StudioId, search);
            //if (frozen) userlist.RemoveAll(x => !x.Frozen);
            //else userlist.RemoveAll(x => x.Active != active);
            return new UsersList { UserWithSubscriptions = userlist, IncludeFrozen = false, PageNo = 1 };
        }


        public static List<UserWithSubscription> GetUsersWithSubscriptionForRef(int? RefId, DateTime? RefDate, int StudioId)
        {
            return UserRepo.GetUsersWithSubscriptionForRef(RefId, RefDate, StudioId);
        }

        public static List<UserWithSubscription> GetNewUsers(int StudioId)
        {
            return UserRepo.GetNewUsers(StudioId);
        }

        public static bool UpdateSubscription(UserSubscriptionModel subscriptionModel)
        {
            return UserRepo.UpdateUserSubscription(Mapper.Map<UserSubscription>(subscriptionModel));
        }

        public static bool ChangeSubscription(SubscriptionDetailModel subscriptionDetailModel)
        {
            var changetype = GetChangeTypes(subscriptionDetailModel.ChangeType.Id).First();
            var result = UserRepo.ChangeSubscription(new UserBalanceTracking()
            {
                Id = subscriptionDetailModel.Id,
                SubscriptionId = subscriptionDetailModel.SubscriptionId,
                Date = subscriptionDetailModel.Date,
                UserUpdated = subscriptionDetailModel.User,
                Note = subscriptionDetailModel.Note,
                Value = subscriptionDetailModel.Value * changetype.Multiplier,
                ChangeTypeId = subscriptionDetailModel.ChangeType.Id
            }, changetype.ValueAsDays);
            return result;
        }

        public static bool ConfirmTandC(string userId)
        {
            return UserRepo.ConfirmTandC(userId);
        }

        public static List<SubscriptionDetailModel> GetSubscriptionDetails(int subscriptionId )
        {
            var list = UserRepo.GetSubscriptionDetails(subscriptionId);
            return list.Select(userBalanceTracking => new SubscriptionDetailModel
            {
                Id = userBalanceTracking.Id, SubscriptionId = userBalanceTracking.SubscriptionId, Date = userBalanceTracking.Date, ChangeType = new BalanceChangeModel
                {
                    Id = userBalanceTracking.BalanceChangeType.Id, Name = userBalanceTracking.BalanceChangeType.Name
                },
                Value = userBalanceTracking.Value, Note = userBalanceTracking.Note, User = userBalanceTracking.AspNetUser.FirstName + " " + userBalanceTracking.AspNetUser.LastName
            }).ToList();
        }

        //public static bool SignHealthTandC()
        //{
        //    throw new NotImplementedException();
        //}

        public static List<BalanceChangeModel> GetChangeTypes(int id, bool showdeleted = false)
        {
            var changeTypes = UserRepo.GetChangeTypes(id, showdeleted);
            var changeTypesModel = new List<BalanceChangeModel>();
            if (changeTypes.Any()) changeTypes.ForEach(x => changeTypesModel.Add(new BalanceChangeModel { Id = x.Id, Name = x.Name, Multiplier = x.Multiplier, ValueAsDays = x.ValueAsDays}));
            return changeTypesModel;
        }


        public static bool DeleteSubacription(int subscriptionId)
        {
            return UserRepo.DeleteSubscription(subscriptionId);
        }

        public static bool RemoveLateCacelationSubscription(int subscriptionId)
        {
            return UserRepo.RemoveLateCacelationSubscription(subscriptionId);
        }


        public static bool AddContactUs(SubscriberModel subscriber)
        {
            return UserRepo.AddContactUs(new Subscriber{ Name = subscriber.Name, Email = subscriber.Email, PhoneNumber = subscriber.PhoneNumber, CompanyId = subscriber.CompanyId, Date = DateTime.UtcNow.ToLocal() });
        }

        public static bool UpdateProfileIMG(string userId, string filename)
        {
            return UserRepo.UpdateUserProfileIMG(userId, filename);
        }

        public static bool UpdateUserIMG(string userId, string filename)
        {
            return UserRepo.UpdateUserIMG(userId, filename);
        }

        public static bool UpdateSubscriptionExpire(int id, DateTime expireDate)
        {
            return UserRepo.UpdateSubscriptionExpire(id, expireDate);
        }

        public static bool UpdateProgressIMG(string userId, string filename)
        {
            return UserRepo.UpdateUserProgressIMG(userId, filename);
        }

        public static APIResult RegisterMobileDevice(MobileDeviceModel model)
        {
            return UserRepo.RegisterMobileDevice(Mapper.Map<UserMobileDevice>(model));
        }

        public static List<object> GetUsers(string search, int StudioId, string type = "user")
        {
            var result = new List<object>();
            var users = UserRepo.GetUsers(search, type, StudioId);
            foreach (var user in users)
            {
                result.Add(new { value = user.FirstName + " " + user.LastName, id = user.Id });
            }

            return result;
        }


        public static List<UserReportModel> GetUsersReport(int MinInactiveDays, int StudioId)
        {
            return UserRepo.GetUsersReport(MinInactiveDays, StudioId).OrderBy(x=>x.LastClassDate).ToList();
        }


        public static bool DeleteUser(string UserId)
        {
            return UserRepo.DeleteUser(UserId);
        }

        public static bool DeleteInstructor(string UserId)
        {
            return UserRepo.DeleteInstructor(UserId);
        }

        public static bool TickUser(string UserId)
        {
            return UserRepo.TickUser(UserId);
        }

        public static List<object> GetUsersForClass(string search, int stusioId, int classid)
        {
            var result = new List<object>();
            var users = UserRepo.GetUsersForClass(search, stusioId, classid);
            foreach (var user in users)
            {
                result.Add(new { value = user.FullName, id = user.Id });
            }

            return result;
        }

        public static List<UserWithSubscription> GetUsersWithSubscription(int id, int StudioId)
        {
            return UserRepo.GetUsersWithSubscription(null, StudioId, null, id);
        }

        public static UsersList GetUsersWithSubscription(int id, int weekno, int StudioId, int ut = 0)
        {
            DateTime? startdate = weekno == -1 ? DateTime.MinValue : Utils.FirstDateOfWeek(DateTime.Now.Year, weekno);
            if (startdate == DateTime.MinValue) startdate = null;
            return new UsersList { UserWithSubscriptions = UserRepo.GetUsersWithSubscription(startdate, StudioId, null, id, 1, ut, false)};
        }

        public static UsersList GetUsersWithNoEnrollments(int StudioId)
        {
            return new UsersList { UserWithSubscriptions = UserRepo.GetUsersWithNoEnrollments(StudioId).ToList(), PageNo = -1 }; //.Where(x=>!x.Frozen).ToList()};
        }

        public static AspNetUser GetUser(string userId)
        {
            return UserRepo.GetUser(userId);
        }

        public static AboutToExpireMessageModel GetAboutToExpireSubscriptions(int companyId, bool forSMS = false)
        {
            return UserRepo.GetAboutToExpireSubscriptions(companyId, forSMS);
        }

        public static AboutToExpireMessageModel GetInactiveSubscriptions(int companyId)
        {
            return UserRepo.GetInactiveSubscriptions(companyId);
        }

        public static bool UpdateSubscriptionFreeze(int subscriptionId, string note, string toDate, string userId)
        {
            return UserRepo.UpdateSubscriptionFreeze(subscriptionId, note, toDate, userId);
        }

        public static FrozenSubscriptionModel GetFrozenSubscriptionDetails(int subscriptionId)
        {
            return UserRepo.GetFrozenSubscriptionDetails(subscriptionId);
        }

        public static List<FrozenSubscriptionModel> GetPastFrozenSubscriptionDetails(int subscriptionId)
        {
            return UserRepo.GetPastFrozenSubscriptionDetails(subscriptionId);
        }
        
        public static bool UpdateSubscriptionUnFreeze(int subscriptionId, string userId)
        {
            return UserRepo.UpdateSubscriptionUnFreeze(subscriptionId, userId);
        }

        public static bool UpdateInstructorRate(InstructorDetailsModel instructorDetailsModel)
        {
            return UserRepo.UpdateInstructorRate(Mapper.Map<InstructorDetail>(instructorDetailsModel));
        }

        public static List<InstructorDetailsModel> GetInstructorList(int StudioId)
        {
            return UserRepo.GetInstructorList(StudioId);
        }

        public static List<InstructorDetailsModel> GetClassInstructors(string ids)
        {
            return UserRepo.GetClassInstructors(ids);
        }

        public static List<InstructorDetailsModel> GetInstructorList(DateTime? startdate, int StudioId)
        {
            return UserRepo.GetInstructorList(startdate, StudioId);
        }

        public static List<InstructorReminderModel> GetInstructorWithClasses(DateTime? startdate, int companyId)
        {
            return UserRepo.GetInstructorWithClasses(startdate, companyId);
        }
        

        public static InstructorDetailsModel GetInstructor(string userid)
        {
            return UserRepo.GetInstructor(userid);
        }

        public static List<UsersSummaryModel> GetUsersSummary(int StudioId)
        {
            return UserRepo.GetUsersSummary(StudioId);
        }
    }
}
