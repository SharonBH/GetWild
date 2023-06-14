using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using InShapeModels;
using Utilities;

namespace DAL
{
    public static class UserRepo
    {
        private static Object thisLock = new Object();
        public static ProfileTracking GetUserProfile(string userid)
        {
            using (var context = new InShapeEntities())
            {
                return context.ProfileTrackings.Where(p => p.UserID == userid).OrderByDescending(p => p.Date).FirstOrDefault() ?? new ProfileTracking();
            }
        }

        public static ProfileTracking GetFirstUserProfile(string userid)
        {
            using (var context = new InShapeEntities())
            {
                return context.ProfileTrackings.Where(p => p.UserID == userid).OrderBy(p => p.Date).FirstOrDefault() ?? new ProfileTracking();
            }
        }

        public static List<ProfileTracking> GetUserProfileforGraph(string userid)
        {
            using (var context = new InShapeEntities())
            {
                return context.ProfileTrackings.Where(p => p.UserID == userid).OrderBy(p => p.Date).ToList();
            }
        }

        public static List<FrozenSubscriptionModel> GetFrozenReport(int StudioId)
        {
            //var studios = App.CurrentCompany.Studios;
            using (var context = new InShapeEntities())
            {
                return
                    context.FrozenSubscriptions.Where(p => !p.IsDeleted)
                    .Include("UserSubscription")
                    //.Include("Studio")
                    .Where(s=>s.UserSubscription.AspNetUser.StudioId == StudioId)
                    //.FilterByCompany()
                    //.Where(s=> studios.Any(s2=> s2.Id == s.UserSubscription.AspNetUser.StudioId))
                    .ProjectTo<FrozenSubscriptionModel>().ToList();
            }
        }

        public static bool UpdateUserProfile(ProfileTracking profile)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    context.ProfileTrackings.Attach(profile);
                    context.Entry(profile).State = profile.Id > 0 ? EntityState.Modified : EntityState.Added;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
            return true;
        }

        public static bool ConfirmSalary(string userId, double Adjustment, DateTime date, string note, string userConfirmed)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var instructorid = new SqlParameter("UserId", userId);
                    var dateparam = new SqlParameter("Date", date);
                    var noteparam = new SqlParameter("Note", note);
                    var adjustment = new SqlParameter("Adjustment", Adjustment);
                    var userConfirmedparam = new SqlParameter("UserConfirmed", userConfirmed);
                    var result = context.Database.SqlQuery<bool>("EXEC ConfirmSalary @UserId, @Adjustment, @Date, @Note, @UserConfirmed", instructorid, adjustment,dateparam,noteparam, userConfirmedparam).FirstOrDefault();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public static UserSubscriptionModel GetUserSubscriptions(string userid)
        {
            using (var context = new InShapeEntities())
            {
                return
                    context.UserSubscriptions.Where(p => p.UserId == userid && p.Active)
                        .ProjectTo<UserSubscriptionModel>()
                        .FirstOrDefault() ?? new UserSubscriptionModel();
            }
        }

        public static UserSubscriptionModelForNav GetUserSubscription(string userid)
        {
            using (var context = new InShapeEntities())
            {
                return
                    context.UserSubscriptions.Where(p => p.UserId == userid && p.Active)
                        .ProjectTo<UserSubscriptionModelForNav>()
                        .FirstOrDefault() ?? new UserSubscriptionModelForNav();
            }
        }

        public static bool ConfirmTandC(string userId)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var user = context.AspNetUsers.FirstOrDefault(x => x.Id == userId);
                    if (user != null)
                    {
                        user.AcceptedTandC = true;
                        context.AspNetUsers.Attach(user);
                        context.Entry(user).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
            return true;
        }

        //public static List<UserWithSubscription> GetUsersWithSubscription()
        //{
        //    using (var context = new InShapeEntities())
        //    {
        //        return (from u in context.AspNetUsers
        //                where
        //                u.AspNetUserRoles.Any(x => x.RoleId == "2")    
        //                from s in u.UserSubscriptions.DefaultIfEmpty()
        //            select new UserWithSubscription
        //            {
        //                FullName = u.FirstName + " " + u.LastName,
        //                PhoneNumber = u.PhoneNumber,
        //                Gender = (Gender)u.Gender,
        //                JoinDate = u.JoinDate.Value,
        //                ProfileIMG = u.ProfileIMG,
        //                UserId = u.Id,
        //                Active = s.Active,
        //                SubscriptionExpireDate = s.DateExpire
        //            }).ToList();
        //    }
        //}

        //public static List<UserWithSubscription> GetUsersWithSubscription(DateTime? startdate, int id = -1)
        //{
        //    List<UserWithSubscription> results;
        //    using (var context = new InShapeEntities())
        //    {
        //        results = !startdate.HasValue ? context.Database.SqlQuery<UserWithSubscription>("GetUsersList @DayNo", new SqlParameter("DayNo", id)).ToList() : 
        //            context.Database.SqlQuery<UserWithSubscription>("GetUsersListForWeek @DayNo, @StartDate", new SqlParameter("DayNo", id), new SqlParameter("StartDate", startdate)).ToList();

        //    }
        //    return results;
        //}

        public static List<UserWithSubscription> GetUsersWithSubscription(DateTime? startdate, int StudioId, bool? active, int id = -1, int pageno = 1, int userType = 0, bool frozen = true)
        {
            List<UserWithSubscription> results;
            using (var context = new InShapeEntities())
            {
                //var compid = context.Companies.FirstOrDefault(c => c.Studios.Any(s => s.Id == StudioId))
                results = context.Database.SqlQuery<UserWithSubscription>("GetUsersList @DayNo, @StartDate, @StudioId, @PageNo, @RemoveMarked, @UserType, @Frozen, @Active",
                        new SqlParameter("DayNo", id), new SqlParameter("StartDate", startdate ?? (object)DBNull.Value), 
                        new SqlParameter("StudioId", StudioId), new SqlParameter("PageNo", pageno),
                        new SqlParameter("RemoveMarked", App.Companies.FirstOrDefault(c => c.Studios.Any(s => s.Id == StudioId)).RemoveMarked),
                        new SqlParameter("UserType", userType), new SqlParameter("Frozen", frozen), 
                        new SqlParameter("Active", id == 0 ? true : active ?? (object)DBNull.Value)



                        ).AsQueryable().ToList();
            }
            return results;
        }

        public static List<UserWithSubscription> GetUsersWithSubscriptionForSearch(int StudioId, string Search)
        {
            List<UserWithSubscription> results;
            int usertype = 0;
            using (var context = new InShapeEntities())
            {
                results = context.Database.SqlQuery<UserWithSubscription>("GetUsersList @DayNo, @StartDate, @StudioId, @PageNo, @RemoveMarked, @UserType, @Frozen, @Active, @Search",
                        new SqlParameter("DayNo", -1), new SqlParameter("StartDate", (object)DBNull.Value),
                        new SqlParameter("StudioId", StudioId), new SqlParameter("PageNo", 1),
                        new SqlParameter("RemoveMarked", App.Companies.FirstOrDefault(c => c.Studios.Any(s => s.Id == StudioId)).RemoveMarked),
                        new SqlParameter("UserType", usertype), new SqlParameter("Frozen", false), new SqlParameter("Active", (object)DBNull.Value), new SqlParameter("Search", Search)
                        ).AsQueryable().ToList();

            }
            return results;
        }

        //public static List<UserWithSubscription> GetUsersWithSubscription(DateTime? startdate, int id = -1, int pageno = 1, int userType = 0)
        //{
        //    List<UserWithSubscription> results;
        //    using (var context = new InShapeEntities())
        //    {
        //        results = context.Database.SqlQuery<UserWithSubscription>("GetUsersList @DayNo, @StartDate, @CompanyId, @PageNo",
        //                new SqlParameter("DayNo", id), new SqlParameter("StartDate", startdate ?? (object)DBNull.Value), new SqlParameter("CompanyId", App.CurrentCompany.Id), new SqlParameter("PageNo", pageno)
        //                ).AsQueryable().ToList();

        //    }
        //    return results;
        //}

        public static List<UserWithSubscription> GetNewUsers(int StudioId, int days = -7)
        {
            List<UserWithSubscription> results;
            using (var context = new InShapeEntities())
            {
                results = context.Database.SqlQuery<UserWithSubscription>("GetNewUsersList @Days, @StudioId",
                        new SqlParameter("Days", days), new SqlParameter("StudioId", StudioId)).AsQueryable().ToList();

            }
            return results;
        }

        public static APIResult RegisterMobileDevice(UserMobileDevice mobileDevice)
        {
            var result = new APIResult();
            try
            {
                using (var context = new InShapeEntities())
                {
                    var device = context.UserMobileDevices.FirstOrDefault(x => x.UserId == mobileDevice.UserId);
                    if (device != null)
                    {
                        device.IsDeleted = true;
                        context.UserMobileDevices.Attach(device);
                        context.Entry(device).State = EntityState.Modified;
                        
                        //var user = context.AspNetUsers.FirstOrDefault(u => u.Id == subscription.UserId);
                        //user.Active = true;
                        //context.AspNetUsers.Attach(user);
                        //context.Entry(user).State = EntityState.Modified;
                        //context.SaveChanges();
                    }
                    context.UserMobileDevices.Attach(mobileDevice);
                    context.Entry(mobileDevice).State = EntityState.Added;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                result.Error = ex.Message;
                result.Result = false;
                return result;
            }
            result.Result = true;
            return result;
        }

        public static List<UserWithSubscription> GetUsersWithSubscriptionForRef(int? RefId, DateTime? RefDate, int StudioId)
        {
            List<UserWithSubscription> results;
            using (var context = new InShapeEntities())
            {
                results =
                    context.Database.SqlQuery<UserWithSubscription>("GetUsersListFor @RefId, @RefDate, @StudioId",
                        new SqlParameter("RefId", RefId ?? (object)DBNull.Value), 
                        new SqlParameter("RefDate", RefDate ?? (object)DBNull.Value),
                        new SqlParameter("StudioId", StudioId)).AsQueryable().ToList();

            }
            return results;
        }

        public static List<AspNetUser> GetUsers(string search, string type, int StudioId)
        {
            using (var context = new InShapeEntities())
            {
                //var roles = Enum.GetValues(typeof(ParticipantType));

                var roles = type == "user" ? Enum.GetValues(typeof(ParticipantType)).Cast<int>().Select(x => x.ToString()).ToList()
                    : Enum.GetValues(typeof(AdminType)).Cast<int>().Select(x => x.ToString()).ToList();

                var users = string.IsNullOrWhiteSpace(search) ? context.AspNetUsers.FilterByUser(StudioId).Where(u => u.AspNetUserRoles.Any(x => roles.Contains(x.RoleId))) : 
                            context.AspNetUsers.FilterByUser(StudioId).Where(c => c.AspNetUserRoles.Any(x => roles.Contains(x.RoleId)) && 
                            (c.FullName.ToLower().StartsWith(search.ToLower()) || c.UserName.ToLower().StartsWith(search.ToLower())));
                return users.ToList();
            }
        }

        public static bool UpdateUserSubscription(UserSubscription subscription)
        {
            try
            {
                lock (thisLock)
                {
                    using (var context = new InShapeEntities())
                    {

                        var exsub = context.UserSubscriptions.FirstOrDefault(x => x.UserId == subscription.UserId && x.Active);
                        if (exsub == null)
                        {
                            var old = context.UserSubscriptions.Any(o=> o.UserId == subscription.UserId && o.SubscriptionTypeId != 6);
                            subscription.IsFirst = !old;
                            if (subscription.Id == 0) subscription.DateExpireOriginal = subscription.DateExpire;
                            context.UserSubscriptions.Attach(subscription);
                            context.Entry(subscription).State = subscription.Id > 0
                                ? EntityState.Modified
                                : EntityState.Added;
                            var alerts = context.SysAlerts.Where(u => u.UserId == subscription.UserId);
                            foreach (var alert in alerts)
                            {
                                alert.IsRead = true;
                                context.SysAlerts.Attach(alert);
                                context.Entry(alert).State = EntityState.Modified;
                            }
                            var user = context.AspNetUsers.FirstOrDefault(u => u.Id == subscription.UserId);
                            user.Active = true;
                            context.AspNetUsers.Attach(user);
                            context.Entry(user).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
            return true;
        }

        public static List<UsersSummaryModel> GetUsersSummary(int StudioId)
        {
           List<UsersSummaryModel> results;
            using (var context = new InShapeEntities())
            {
                results = context.Database.SqlQuery<UsersSummaryModel>("GetUsersSummary @StudioId, @RemoveMarked", new SqlParameter("StudioId", StudioId),
                    new SqlParameter("RemoveMarked", App.Companies.FirstOrDefault(c => c.Studios.Any(s => s.Id == StudioId)).RemoveMarked)).AsQueryable().ToList();

            }
            return results;
        }

        public static bool UpdateSubscriptionExpire(int id, DateTime expireDate)
        {
            using (var context = new InShapeEntities())
            {
                var sub = context.UserSubscriptions.FirstOrDefault(x => x.Id == id);
                if (sub == null) return false;
                sub.DateExpire = expireDate;
                context.UserSubscriptions.Attach(sub);
                context.Entry(sub).State = EntityState.Modified;
                var alerts = context.SysAlerts.Where(u => u.UserId == sub.UserId);
                foreach (var alert in alerts)
                {
                    alert.IsRead = true;
                    context.SysAlerts.Attach(alert);
                    context.Entry(alert).State = EntityState.Modified;
                }
                context.SaveChanges();
            }
            return true;
        }


        public static bool ChangeSubscription(UserBalanceTracking userBalanceTracking, bool valueAsDays)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    // get current balance
                    var Subscription = context.UserSubscriptions.FirstOrDefault(x => x.Id == userBalanceTracking.SubscriptionId);
                    if (!valueAsDays)
                    {
                        userBalanceTracking.Balance = Subscription.CurrentBalance + userBalanceTracking.Value;
                        Subscription.CurrentBalance += userBalanceTracking.Value;
                        Subscription.Active = Subscription.CurrentBalance > 0;
                    }
                    else
                    {
                        userBalanceTracking.Balance = Subscription.CurrentBalance;
                        Subscription.DateExpire = Subscription.DateExpire.Value.AddDays(userBalanceTracking.Value);
                        Subscription.Active = Subscription.DateExpire > DateTime.UtcNow.ToLocal();
                    }
                    
                    
                    context.UserBalanceTrackings.Attach(userBalanceTracking);
                    context.Entry(userBalanceTracking).State = EntityState.Added;

                    
                    context.UserSubscriptions.Attach(Subscription);
                    context.Entry(Subscription).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
            return true;
        }

        public static InstructorDetailsModel GetInstructor(string userid)
        {
            var c = UserType.ClassInstructor.ToString("d");
            using (var context = new InShapeEntities())
            {
                return context.InstructorDetails
                    .Where(x => x.AspNetUser.AspNetUserRoles.Any(r => r.RoleId == c) && x.AspNetUser.Id == userid)
                    .Include("AspNetUser")
                    .ProjectTo<InstructorDetailsModel>().FirstOrDefault();
            }
        }

        public static List<InstructorDetailsModel> GetInstructorList(int StudioId)
        {
            var c = UserType.ClassInstructor.ToString("d");
            using (var context = new InShapeEntities())
            {
                return context.InstructorDetails
                    .Where(x => !x.IsDeleted && x.AspNetUser.AspNetUserRoles.Any(r => r.RoleId == c))
                    .Include("AspNetUser")
                    .FilterByUser(StudioId)
                    .ProjectTo<InstructorDetailsModel>().ToList();
            }
        }

        public static List<InstructorDetailsModel> GetClassInstructors(string ids)
        {
            var c = ids.Split(';');
            using (var context = new InShapeEntities())
            {
                return context.InstructorDetails
                    .Where(x => c.Any(i=> i.Trim() == x.AspNetUser.Id))
                    .Include("AspNetUser")
                    //.FilterByUser(StudioId)
                    .ProjectTo<InstructorDetailsModel>().ToList();
            }
        }

        public static List<InstructorDetailsModel> GetInstructorList(DateTime? startdate, int StudioId)
        {
            List<InstructorDetailsModel> results;
            using (var context = new InShapeEntities())
            {
                results = context.Database.SqlQuery<InstructorDetailsModel>("GetInstructorList @StartDate, @StudioId",
                        new SqlParameter("StartDate", startdate ?? (object)DBNull.Value), new SqlParameter("StudioId", StudioId))
                        .AsQueryable().ToList();

            }
            return results;
        }

        public static List<InstructorReminderModel> GetInstructorWithClasses(DateTime? startdate, int companyId)
        {
            List<InstructorReminderModel> results;
            using (var context = new InShapeEntities())
            {
                results = context.AspNetUsers.Where(i => i.AspNetUserRoles.Any(r => r.RoleId == ((int)AdminType.ClassInstructor).ToString())
                           && i.Class_Instructors.Any(c => DbFunctions.TruncateTime(c.Class.Date) == startdate)
                           && i.Studio.CompanyId == companyId).ProjectTo<InstructorReminderModel>().ToList();


                foreach (var item in results)
                {
                    item.Classes = Mapper.Map<List<StudioClassModel>>(
                    context.Classes.Include("ClassType")
                    .Where(x => !x.IsDeleted && DbFunctions.TruncateTime(x.Date) == startdate && x.Class_Instructors.Any(i => i.InstructorId == item.Instructor.UserId)).ToList());
                    //.FilterByCompany().OrderBy(x => x.Date).ToList());
                }
            }
            return results;
        }

        public static bool UpdateInstructorRate(InstructorDetail instructorDetails)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    context.InstructorDetails.Attach(instructorDetails);
                    context.Entry(instructorDetails).State = instructorDetails.Id > 0 ? EntityState.Modified : EntityState.Added;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.Message);
                return false;
            }
            return true;
        }

        public static bool UpdateSubscriptionUnFreeze(int subscriptionId, string userid)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    // get current balance

                    var Subscription = context.UserSubscriptions.FirstOrDefault(x => x.Id == subscriptionId);
                    if (Subscription == null) return false;
                    Subscription.Frozen = false;
                    var frozendetails = context.FrozenSubscriptions.FirstOrDefault(x => x.SubscriptionId == subscriptionId && !x.IsDeleted);
                    frozendetails.DateFinished = DateTime.UtcNow.ToLocal();
                    frozendetails.UserFinished = userid;
                    frozendetails.IsDeleted = true;
                    TimeSpan difference = frozendetails.DateFinished.Value - frozendetails.Date;
                    Subscription.DateExpire = Subscription.DateExpire.Value.AddDays(difference.Days);

                    var balanceTracking = new UserBalanceTracking
                    {
                        ChangeTypeId = 7,
                        Date = DateTime.UtcNow.ToLocal(),
                        Value = 0,
                        SubscriptionId = Subscription.Id,
                        UserUpdated = userid,
                        Balance = Subscription.NumClasses > 0 ? Subscription.CurrentBalance : 0,
                        Note = $"{DateTime.UtcNow.ToLocal()}: ביטול הקפאה, הוספו {difference.Days} ימים למנוי"
                    };

                    context.UserSubscriptions.Attach(Subscription);
                    context.Entry(Subscription).State = EntityState.Modified;
                    context.FrozenSubscriptions.Attach(frozendetails);
                    context.Entry(frozendetails).State = EntityState.Modified;
                    context.UserBalanceTrackings.Attach(balanceTracking);
                    context.Entry(balanceTracking).State = EntityState.Added;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
            return true;
        }

        public static FrozenSubscriptionModel GetFrozenSubscriptionDetails(int subscriptionId)
        {
            using (var context = new InShapeEntities())
            {
                var Subscription = context.UserSubscriptions.FirstOrDefault(x => x.Id == subscriptionId);
                var frozendetails = context.FrozenSubscriptions.Where(x => x.SubscriptionId == subscriptionId && !x.IsDeleted)
                    .ProjectTo<FrozenSubscriptionModel>()
                    .FirstOrDefault() ?? new FrozenSubscriptionModel();
                frozendetails.SubscriptionUser = Subscription.UserId;
                return frozendetails;
            }
        }

        public static List<FrozenSubscriptionModel> GetPastFrozenSubscriptionDetails(int subscriptionId)
        {
            using (var context = new InShapeEntities())
            {
                //var Subscription = context.UserSubscriptions.FirstOrDefault(x => x.Id == subscriptionId);
                var frozendetails = context.FrozenSubscriptions.Where(x => x.SubscriptionId == subscriptionId)
                    .ProjectTo<FrozenSubscriptionModel>().ToList();
                //frozendetails.SubscriptionUser = Subscription.UserId;
                return frozendetails;
            }
        }

        public static bool UpdateSubscriptionFreeze(int subscriptionId, string note, string toDate, string userId)
        {
            DateTime todate = DateTime.MinValue;
            DateTime.TryParse(toDate,out todate);
            DateTime? freezetodate = todate;
            if (freezetodate == DateTime.MinValue) freezetodate = null;
            using (var context = new InShapeEntities())
            {
                var Subscription = context.UserSubscriptions.FirstOrDefault(x => x.Id == subscriptionId && !x.Frozen);
                if (Subscription == null) return false;

                var frozen = context.FrozenSubscriptions.FirstOrDefault(x => x.SubscriptionId == subscriptionId && !x.IsDeleted);
                if (frozen != null) return false;

                Subscription.Frozen = true;
                FrozenSubscription f = new FrozenSubscription
                {
                    SubscriptionId = Subscription.Id,
                    UserCreated = userId,
                    Date = DateTime.UtcNow.ToLocal(),
                    Note = note,
                    FreezeToDate = freezetodate,
                    IsDeleted = false
                };

                var balanceTracking = new UserBalanceTracking
                {
                    ChangeTypeId = 7,
                    Date = DateTime.UtcNow.ToLocal(),
                    Value = 0,
                    SubscriptionId = Subscription.Id,
                    UserUpdated = userId,
                    Balance = Subscription.NumClasses > 0 ? Subscription.CurrentBalance : 0,
                    Note = $"{note} - עד לתאריך: {freezetodate}"
                };

                context.UserSubscriptions.Attach(Subscription);
                context.Entry(Subscription).State = EntityState.Modified;
                context.FrozenSubscriptions.Attach(f);
                context.Entry(f).State = EntityState.Added;
                context.SaveChanges();
                return true;
            }
        }

        public static bool UnFreezeSubscription(int subscriptionId, string userId)
        {
            using (var context = new InShapeEntities())
            {
                var us = context.UserSubscriptions.FirstOrDefault(s => s.Id == subscriptionId);
                if (us == null) return false;

                //check if subscription is frozen (unfreeze)
                if (!us.Frozen) return false;
                us.Frozen = false;

                var f = context.FrozenSubscriptions.FirstOrDefault(x => x.SubscriptionId == us.Id && !x.IsDeleted);
                if (f == null) return false;

                f.DateFinished = DateTime.UtcNow.ToLocal();
                f.UserFinished = userId;
                f.IsDeleted = true;
                context.FrozenSubscriptions.Attach(f);
                context.Entry(f).State = EntityState.Modified;
                TimeSpan difference = f.DateFinished.Value - f.Date;
                us.DateExpire = us.DateExpire.Value.AddDays(difference.Days);

                context.UserSubscriptions.Attach(us);
                context.Entry(us).State = EntityState.Modified;
                context.SaveChanges();
            }
            return true;
        }


        public static List<UserBalanceTracking> GetSubscriptionDetails(int subscriptionID)
        {
            using (var context = new InShapeEntities())
            {
                return context.UserBalanceTrackings.Include("AspNetUser").Include("BalanceChangeType").Where(x => x.SubscriptionId == subscriptionID).ToList();
            }
        }

        public static List<BalanceChangeType> GetChangeTypes(int typeId, bool showdeleted = false)
        {
            using (var context = new InShapeEntities())
            {
                return typeId > 0 ? context.BalanceChangeTypes.Where(r => r.Id == typeId && !r.IsDeleted).ToList() : showdeleted ? context.BalanceChangeTypes.ToList() : context.BalanceChangeTypes.Where(r => !r.IsDeleted).ToList();
            }
        }

        public static bool DeleteSubscription(int subscriptionId)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var subscription = context.UserSubscriptions.First(r => r.Id == subscriptionId);
                    //context.StudioRooms.Remove(room);
                    subscription.Active = false;
                    subscription.Frozen = false;
                    subscription.DateExpire = DateTime.UtcNow.ToLocal();
                    subscription.CurrentBalance = 0;
                    context.UserSubscriptions.Attach(subscription);
                    context.Entry(subscription).State = EntityState.Modified;

                    var frozen = context.FrozenSubscriptions.FirstOrDefault(r => r.SubscriptionId == subscriptionId);
                    if (frozen != null)
                    {
                        frozen.IsDeleted = true;
                        frozen.DateFinished = DateTime.UtcNow.ToLocal();
                        context.FrozenSubscriptions.Attach(frozen);
                        context.Entry(frozen).State = EntityState.Modified;
                    }

                    var user = context.AspNetUsers.FirstOrDefault(u => u.Id == subscription.UserId);
                    user.Active = false;
                    context.AspNetUsers.Attach(user);
                    context.Entry(user).State = EntityState.Modified;
                    //context.Entry(subscription).State = EntityState.Deleted;
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

        public static bool RemoveLateCacelationSubscription(int subscriptionId)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var subscription = context.UserSubscriptions.First(r => r.Id == subscriptionId);
                    //context.StudioRooms.Remove(room);
                    subscription.LateCacelation -= 1; // = false;
                    context.UserSubscriptions.Attach(subscription);
                    context.Entry(subscription).State = EntityState.Modified;
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

        public static bool AddContactUs(Subscriber subscriber)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    context.Subscribers.Attach(subscriber);
                    context.Entry(subscriber).State = EntityState.Added;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
            return true;
        }

        public static bool UpdateUserProfileIMG (string userId, string filename)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var profile = context.AspNetUsers.FirstOrDefault(x => x.Id == userId);
                    if (profile == null) return false;
                    profile.ProfileIMG = filename;
                    context.AspNetUsers.Attach(profile);
                    context.Entry(profile).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
            return true;
        }

        public static bool UpdateUserIMG(string userId, string filename)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var profile = context.AspNetUsers.FirstOrDefault(x => x.Id == userId);
                    if (profile == null) return false;
                    var profileTrackings = context.ProfileTrackings.OrderByDescending(x => x.Id).FirstOrDefault(x => x.UserID == userId);
                    if (profileTrackings == null)
                    {
                        profile.ProfileIMG = filename;
                        context.AspNetUsers.Attach(profile);
                        context.Entry(profile).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    else
                    {
                        profileTrackings.Picture = filename.Insert(filename.Length-4, "_" + DateTime.UtcNow.ToLocal().ToString("yyyy-M-dd-hh-mm"));
                        profileTrackings.Date = DateTime.UtcNow.ToLocal();
                        //profile.Id = 0;
                        context.ProfileTrackings.Attach(profileTrackings);
                        context.Entry(profile).State = EntityState.Added;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
            return true;
        }

        public static List<UserReportModel> GetUsersReport(int MinInactiveDays, int StudioId)
        {
            var UsersReport = new List<UserReportModel>();
            var date = DateTime.UtcNow.ToLocal().Date.AddDays(-MinInactiveDays);
            var now = DateTime.UtcNow.ToLocal();
            var date2 = Utilities.Utils.FirstDateOfWeek();
            //using (var context = new InShapeEntities())
            //{
            //    context.AspNetUsers.OrderBy(x => x.LastClass)
            //        .Where(
            //            x => x.AspNetUserRoles.Any(r => r.RoleId == "2") && (x.LastClass == null || x.LastClass < date))
            //        .Include("UserProcessings");
            //}
            //var roles = Enum.GetValues(typeof(ParticipantType)).Cast<int>().Select(x => x.ToString()).ToList();

            using (var context = new InShapeEntities())
            {
                return (from u in context.AspNetUsers.FilterByUser(StudioId)
                        where
                            u.AspNetUserRoles.Any(r => r.RoleId == "2") &&
                            (u.LastClass < date || (u.LastClass == null && u.JoinDate <= date))
                            && u.Active && !u.UserSubscriptions.Any(x=>x.Frozen)
                        orderby u.LastClass
                        select new UserReportModel
                        {
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            PhoneNumber = u.PhoneNumber,
                            Note = u.UserDailyTicks.OrderByDescending(x => x.Date).FirstOrDefault(d => d.Date >= date2).Note,
                            UserId = u.Id,
                            Active = u.Active,
                            LastClassDate = u.LastClass.HasValue ? u.LastClass : u.JoinDate,
                            ProcessDate = u.UserDailyTicks.OrderByDescending(x => x.Date).FirstOrDefault(d => d.Date >= date2).Date,
                            Processed = u.UserDailyTicks.Any(d => d.Date >= date2),
                            NextClassDate = u.UserSubscriptions.FirstOrDefault(c => c.Active).ClassEnrollments.OrderByDescending(x => x.Class.Date).FirstOrDefault(ce => !ce.IsDeleted && ce.Class.Date > now).Class.Date,
                            NextClassType = u.UserSubscriptions.FirstOrDefault(c => c.Active).ClassEnrollments.OrderByDescending(x => x.Class.Date).FirstOrDefault(ce => !ce.IsDeleted && ce.Class.Date > now).Class.ClassType.Name,

                        }).ToList();
            }
        }

        public static bool UpdateUserProgressIMG(string userId, string filename)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var profile = context.ProfileTrackings.OrderByDescending(x=>x.Id).FirstOrDefault(x => x.UserID == userId) ??
                                  new ProfileTracking{ UserID = userId};
                    profile.Picture = filename;
                    profile.Date = DateTime.UtcNow.ToLocal();
                    //profile.Id = 0;
                    context.ProfileTrackings.Attach(profile);
                    context.Entry(profile).State = EntityState.Added;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
            return true;
        }


        public static AspNetUser GetUser(string userId)
        {
            using (var context = new InShapeEntities())
            {
                return context.AspNetUsers.FirstOrDefault(x => x.Id == userId);
            }
        }

        //public static bool UpdateUserProgressIMG(string userId, string filename)
        //{
        //    SqlParameter UserId = new SqlParameter("UserId", userId);
        //    SqlParameter ProgressIMG = new SqlParameter("ProgressIMG", filename);
        //    try
        //    {
        //        using (var context = new InShapeEntities())
        //        {
        //            context.Database.ExecuteSqlCommand("UpdateProgressImage @UserId, @ProgressIMG", UserId, ProgressIMG);
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        return false;
        //    }
        //    return true;
        //}


        public static bool DeleteUser(string UserId)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var user = context.AspNetUsers.First(r => r.Id == UserId);
                    //var subscriptions = context.UserSubscriptions.Where(s => s.UserId == UserId);
                    context.FrozenSubscriptions.RemoveRange(context.FrozenSubscriptions.Where(s => s.UserSubscription.UserId == UserId));
                    context.LastClasses.RemoveRange(context.LastClasses.Where(s => s.UserId == UserId));
                    context.UserSubscriptions.RemoveRange(context.UserSubscriptions.Where(x=> x.UserId == UserId));
                    //context.StudioRooms.Remove(room);
                    context.AspNetUsers.Attach(user);
                    context.Entry(user).State = EntityState.Deleted;
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

        public static bool DeleteInstructor(string UserId)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var instructor = context.AspNetUsers.First(r => r.Id == UserId);
                    var details = context.InstructorDetails.First(s => s.InstructorId == UserId && !s.IsDeleted);
                    details.IsDeleted = true;
                    //context.StudioRooms.Remove(room);
                    //context.AspNetUsers.Attach(user);
                    context.Entry(details).State = EntityState.Modified;
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

        public static bool ProcessUser(string UserId, string Note)
        {
            var up = new UserProcessing{ UserId = UserId, Note = Note, Date = DateTime.UtcNow.ToLocal() };
            try
            {
                using (var context = new InShapeEntities())
                {
                    context.UserProcessings.Attach(up);
                    context.Entry(up).State = EntityState.Added;
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

        public static bool TickUser(string UserId)
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    var startdateParameter = new SqlParameter("userId", UserId);
                    context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction,"EXEC TickUser @userId", startdateParameter);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public static List<AspNetUser> GetUsersForClass(string search, int studioId, int classid)
        {
            using (var context = new InShapeEntities())
            {
                var cl = context.Classes.FirstOrDefault(x => x.Id == classid);
                if (cl.IsForFemale && cl.IsForMale)
                {
                    var users = string.IsNullOrWhiteSpace(search)
                        ? context.AspNetUsers.FilterByUser(studioId).Where(u =>  u.UserSubscriptions.Any(x => x.Active))
                        : context.AspNetUsers.FilterByUser(studioId).Where(
                            c => c.UserSubscriptions.Any(x => x.Active) &&
                                (c.FullName.ToLower().StartsWith(search.ToLower())));
                    return users.ToList();
                }
                else
                {
                    int g = cl.IsForMale ? (int)Gender.זכר : (int)Gender.נקבה;
                    var users = string.IsNullOrWhiteSpace(search)
                        ? context.AspNetUsers.FilterByUser(studioId).Where(u => u.UserSubscriptions.Any(x => x.Active) && u.Gender == g)
                        : context.AspNetUsers.FilterByUser(studioId).Where(
                            c => c.UserSubscriptions.Any(x => x.Active) && c.Gender == g &&
                                (c.FullName.ToLower().StartsWith(search.ToLower())));
                    return users.ToList();
                }
                
            }
        }

        public static List<UserWithSubscription> GetUsersWithNoEnrollments(int StudioId)
        {
            List<UserWithSubscription> results;
            using (var context = new InShapeEntities())
            {
                var companyIdParameter = new SqlParameter("@StudioId", StudioId);
                results = context.Database.SqlQuery<UserWithSubscription>("GetUsersNoEnrollmentList @StudioId", companyIdParameter).AsQueryable().ToList();

            }
            return results;
        }

        public static AboutToExpireMessageModel GetAboutToExpireSubscriptions(int companyId = 0, bool forSMS = false)
        {
            var result = new AboutToExpireMessageModel();
            if (companyId == 0)
            {
                result.AboutToExpireSubscriptionModels = new List<UserSubscriptionModel>();
            }
            else
            {
                if (forSMS)
                {
                    using (var context = new InShapeEntities())
                    {
                        var interval = context.MSGTypes.FirstOrDefault(x => x.MessageTypeId == (int)MessageType.Expire && x.CompanyId == companyId);
                        if (interval == null) return result;
                        result.Message = interval.Message;
                        //var start = DateTime.UtcNow.ToLocal().Date;
                        var end = DateTime.UtcNow.ToLocal().AddMinutes(interval.TimeBefore.Value).Date;
                        result.AboutToExpireSubscriptionModels =
                            context.UserSubscriptions.Where(x => x.DateExpire <= end
                                                                 && x.Active && !x.Frozen && x.AspNetUser.AspNetUserRoles.Any(r => r.RoleId == ((int)ParticipantType.User).ToString())
                                                                 && x.AspNetUser.Studio.CompanyId == companyId)
                                .ProjectTo<UserSubscriptionModel>()
                                .ToList();
                    }
                }
                else
                {
                    using (var context = new InShapeEntities())
                    {
                        var interval = context.MSGTypes.FirstOrDefault(x => x.MessageTypeId == (int)MessageType.Expire && x.CompanyId == companyId);
                        if (interval == null) return result;
                        result.Message = interval.Message;
                        //var start = DateTime.UtcNow.ToLocal().Date;
                        var end = DateTime.UtcNow.ToLocal().AddMinutes(interval.TimeBefore.Value).Date;
                        result.AboutToExpireSubscriptionModels =
                            context.UserSubscriptions.Where(x => (x.DateExpire <= end && x.NumClasses ==0) //|| (x.CurrentBalance <= 3 && x.NumClasses > 3))
                                                                 && x.Active && !x.Frozen && x.AspNetUser.AspNetUserRoles.Any(r => r.RoleId == ((int)ParticipantType.User).ToString())
                                                                 && x.AspNetUser.Studio.CompanyId == companyId)
                                .ProjectTo<UserSubscriptionModel>()
                                .OrderBy(x=> x.CurrentBalance).ThenBy(y=>y.DateExpire).ToList();
                    }
                }
            }
            return result;
        }

        public static AboutToExpireMessageModel GetInactiveSubscriptions(int companyId)
        {
            var result = new AboutToExpireMessageModel();
            using (var context = new InShapeEntities())
            {
                var interval = context.MSGTypes.FirstOrDefault(x => x.MessageTypeId == (int)MessageType.Inactive && x.CompanyId == companyId);
                if (interval == null) return result;
                result.Message = interval.Message;
                var lastactive = DateTime.UtcNow.ToLocal().AddMinutes(-interval.TimeBefore.Value).Date;
                result.AboutToExpireSubscriptionModels =
                    context.UserSubscriptions.Where(x => x.ClassEnrollments.Count(c => !c.IsDeleted && c.DateEnrolled > lastactive) == 0 && x.Active
                    && x.AspNetUser.Studio.CompanyId == companyId) //.FilterByCompany()
                        .ProjectTo<UserSubscriptionModel>()
                        .ToList();
            }
            return result;
        }

    }
}
