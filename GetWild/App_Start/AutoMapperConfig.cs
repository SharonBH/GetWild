using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using GetWild.Models;
using InShapeModels;
using InShapeModels.APIModels;
using Utilities;

namespace GetWild
{
    public class AutoMapperConfig: Profile
    {
        protected override void Configure()
        {
            CreateMap<ProfileTracking, UserProfileModel>().ReverseMap();
            CreateMap<ClassWaitList, WaitListEnrollment>().ReverseMap();
            CreateMap<ApplicationUser, AspNetUser>().ReverseMap();
            //CreateMap<InShapeUser, AspNetUser>().ReverseMap();

            CreateMap<AspNetUser, InShapeUser>() //.ReverseMap();
                .ForMember(d => d.Gender, o => o.MapFrom(y => (Gender)y.Gender))
                .ForMember(d => d.UserId, o => o.MapFrom(y => y.Id))
            .ForMember(d => d.JoinDate, o => o.MapFrom(y => y.JoinDate ?? DateTime.UtcNow)).ReverseMap();
            CreateMap<AspNetUser, InstructorDetailsModel>()
                 .ForMember(d => d.Gender, o => o.MapFrom(y => (Gender)y.Gender)).ReverseMap();

            CreateMap<ClassAvailablePlacement, AvailablePlacementsModel>()
                 .ForMember(d => d.TypeId, o => o.MapFrom(y => y.StudioPlacement.TypeId)).ReverseMap();

            CreateMap<AspNetUser, InstructorReminderModel>()
                .ForMember(d => d.Instructor, o => o.MapFrom(y => y)).ReverseMap();
            CreateMap<InstructorDetail, InstructorDetailsModel>()
                //.ForMember(x => x.InstructorModel, o => o.MapFrom(u => u.AspNetUser))
                .ForMember(x => x.UserId, o => o.MapFrom(u => u.AspNetUser.Id))
                .ForMember(x => x.Gender, o => o.MapFrom(u => (Gender)u.AspNetUser.Gender))
                .ForMember(x => x.StudioId, o => o.MapFrom(u => u.AspNetUser.StudioId))
                .ForMember(x => x.DOB, o => o.MapFrom(u => u.AspNetUser.DOB))
                .ForMember(x => x.Email, o => o.MapFrom(u => u.AspNetUser.Email))
                .ForMember(x => x.FirstName, o => o.MapFrom(u => u.AspNetUser.FirstName))
                .ForMember(x => x.LastName, o => o.MapFrom(u => u.AspNetUser.LastName))
                .ForMember(x => x.ProfileIMG, o => o.MapFrom(u => u.AspNetUser.ProfileIMG))
                .ForMember(x => x.PhoneNumber, o => o.MapFrom(u => u.AspNetUser.PhoneNumber))
                .ForMember(x => x.Address, o => o.MapFrom(u => u.AspNetUser.Address))
                .ForMember(x => x.ReceiveSMS, o => o.MapFrom(u => u.AspNetUser.ReceiveSMS))
                .ReverseMap();
            CreateMap<ProfileViewModel, UserProfileModel>().ReverseMap();
            CreateMap<Class, StudioClassReportModel>()
                .ForMember(c=>c.Time, opt => opt.MapFrom(x=>x.Date))
                .ForMember(c => c.Instructors, opt => opt.MapFrom(x => x.Class_Instructors))
                .ForMember(c => c.InstructorIds, y => y.MapFrom(i => i.Class_Instructors.Select(x => x.InstructorId).ToArray()))
                .ReverseMap();
            CreateMap<Class, StudioClassModel>()
                .ForMember(x=>x.Gender, opt=> opt.MapFrom(c=>c.IsForFemale && c.IsForMale ? Gender.מעורב : c.IsForFemale ? Gender.נקבה : Gender.זכר))
                .ForMember(x=>x.Picture, y=>y.MapFrom(c=>c.ClassType.Picture))
                .ForMember(x => x.Time, y => y.MapFrom(c => c.Date))
                .ForMember(x => x.BGColor, y => y.MapFrom(c => c.ClassType.BGColor))
                .ForMember(x => x.AgeGroup, y => y.MapFrom(c => (AgeGroup)c.AgeGroup))
                .ForMember(c=>c.InstructorIds, y => y.MapFrom(i=>i.Class_Instructors.Select(x=> x.InstructorId).ToArray()))
                //.ForMember(c => c.PlacementIds, y => y.MapFrom(i => i.ClassPlacements.Select(x => x.StudioPlacementId).ToArray()))
                .ForMember(c=>c.Instructors, opt=> opt.MapFrom(x=>x.Class_Instructors))
                .ForMember(c=>c.ClassTypeDetailsName, o=> o.MapFrom(d=>d.ClassTypeDetail.Name))
                .ForMember(c=>c.ClassTypeDescription, y=>y.MapFrom(i=>i.ClassType.Description))
                .ForMember(c => c.Enrollments, y => y.MapFrom(i => i.ClassEnrollments))
                .ReverseMap();

            CreateMap<Class, CalendarStudioClassModel>()
                .ForMember(x => x.Gender, opt => opt.MapFrom(c => c.IsForFemale && c.IsForMale ? Gender.מעורב : c.IsForFemale ? Gender.נקבה : Gender.זכר))
                .ForMember(x => x.Picture, y => y.MapFrom(c => c.ClassType.Picture))
                .ForMember(x => x.Time, y => y.MapFrom(c => c.Date))
                .ForMember(x => x.BGColor, y => y.MapFrom(c => c.ClassType.BGColor))
                .ForMember(x => x.AgeGroup, y => y.MapFrom(c => (AgeGroup)c.AgeGroup))
                //.ForMember(c => c.InstructorIds, y => y.MapFrom(i => i.Class_Instructors.Select(x => x.InstructorId).ToArray()))
                //.ForMember(c => c.PlacementIds, y => y.MapFrom(i => i.ClassPlacements.Select(x => x.StudioPlacementId).ToArray()))
                //.ForMember(c => c.Instructors, opt => opt.MapFrom(x => x.Class_Instructors))
                .ForMember(c => c.ClassTypeDetailsName, o => o.MapFrom(d => d.ClassTypeDetail.Name))
                .ForMember(c => c.ClassTypeDescription, y => y.MapFrom(i => i.ClassType.Description))
                //.ForMember(c => c.Enrollments, y => y.MapFrom(i => i.ClassEnrollments))
                .ReverseMap();

            CreateMap<Class_Instructors, InstructorDetailsModel>()
                .ForMember(x => x.UserId, o => o.MapFrom(u => u.AspNetUser.Id))
                .ForMember(x => x.Gender, o => o.MapFrom(u => (Gender)u.AspNetUser.Gender))
                .ForMember(x => x.StudioId, o => o.MapFrom(u => u.AspNetUser.StudioId))
                .ForMember(x => x.DOB, o => o.MapFrom(u => u.AspNetUser.DOB))
                .ForMember(x => x.Email, o => o.MapFrom(u => u.AspNetUser.Email))
                .ForMember(x => x.FirstName, o => o.MapFrom(u => u.AspNetUser.FirstName))
                .ForMember(x => x.LastName, o => o.MapFrom(u => u.AspNetUser.LastName))
                .ForMember(x => x.ProfileIMG, o => o.MapFrom(u => u.AspNetUser.ProfileIMG))
                .ForMember(x => x.PhoneNumber, o => o.MapFrom(u => u.AspNetUser.PhoneNumber))
                .ForMember(x => x.Address, o => o.MapFrom(u => u.AspNetUser.Address))
                .ForMember(x => x.ReceiveSMS, o => o.MapFrom(u => u.AspNetUser.ReceiveSMS))
                //.ForMember(c=>c.InstructorModel, opt=>opt.MapFrom(x=>x.AspNetUser))
                .ReverseMap();
            CreateMap<StudioClassModel, ClassViewModel>()
                //.ForMember(c=>c.Instructors, opt=>opt.MapFrom(x=>x.Instructors))
                .ReverseMap();
            CreateMap<CalendarStudioClassModel, ClassViewModel>()
                //.ForMember(c=>c.Instructors, opt=>opt.MapFrom(x=>x.Instructors))
                .ReverseMap();
            CreateMap<StudioClassModel, CalendarStudioClassModel>().ReverseMap();
            CreateMap<AvailablePlacementsModel, CalendarAvailablePlacementsModel>().ReverseMap(); 

            CreateMap<ClassTypeModel, ClassType>().ReverseMap();
            CreateMap<ClassTypeModel, ClassTypeViewModel>().ReverseMap();
            CreateMap<ClassTypeDetailsModel, ClassTypeDetail>().ReverseMap();
            CreateMap<ClassTypeDetailsModel, ClassTypeDetailsViewModel>().ReverseMap();
            CreateMap<ClassDailySlot, DailySlotModel>().ReverseMap();
            CreateMap<StudioRoomModel, StudioRoom>().ReverseMap();
            CreateMap<StudioModel, DAL.Studio>().ReverseMap();
            CreateMap<UserSubscription, UserSubscriptionModel>()
                .ForMember(x=>x.FirstName, opt=> opt.MapFrom(u=>u.AspNetUser.FirstName))
                .ForMember(x => x.LastName, opt => opt.MapFrom(u => u.AspNetUser.LastName))
                .ForMember(x => x.PhoneNumber, opt => opt.MapFrom(u => u.AspNetUser.PhoneNumber))
                .ForMember(x => x.Gender, opt => opt.MapFrom(u => (Gender)u.AspNetUser.Gender))
                .ForMember(x=> x.Roles, opt => opt.MapFrom(u=> u.AspNetUser.AspNetUserRoles))
                .ReverseMap();
            CreateMap<UserSubscription, UserSubscriptionModelForNav>().ReverseMap();
            CreateMap<SubscriptionViewModel, UserSubscriptionModelForNav>().ReverseMap(); 
            CreateMap<AspNetUserRole, UserRole>()
                .ReverseMap();
            CreateMap<ClassEnrollment, ClassEnrollmentModel>().
                ForMember(x=>x.UserTypestr, opt=> opt.MapFrom(m=>m.UserSubscription.AspNetUser.AspNetUserRoles.FirstOrDefault().RoleId))
                .ForMember(x=> x.LastClass, opt=> opt.MapFrom(m=>m.UserSubscription.AspNetUser.LastClass))
                .ForMember(x => x.SelectedPlacement, opt => opt.MapFrom(m => m.ClassAvailablePlacement))
                //.ForMember(x => x.Comment, opt => opt.MapFrom(m => m.EnrollmentComments.FirstOrDefault().Comment))
                //.ForMember(x => x.CommentBy, opt => opt.MapFrom(m => m.EnrollmentComments.FirstOrDefault().AspNetUser.FullName))
                .ReverseMap();

            CreateMap<ClassEnrollment, CalendarClassEnrollmentModel>()
                //.ForMember(x => x.SelectedPlacement, opt => opt.MapFrom(m => m.ClassAvailablePlacement))
                .ReverseMap();
            CreateMap<ClassWaitList, ClassEnrollmentModel>().
                ForMember(x => x.UserTypestr, opt => opt.MapFrom(m => m.UserSubscription.AspNetUser.AspNetUserRoles.FirstOrDefault().RoleId))
                .ForMember(x => x.LastClass, opt => opt.MapFrom(m => m.UserSubscription.AspNetUser.LastClass))
                .ReverseMap();
            CreateMap<UserWithSubscription, UserSMSSubscriptionModel>().ReverseMap();
            CreateMap<UserSMSSubscriptionModel, UserReportModel>().ReverseMap();
            CreateMap<AdvancedSmsViewModel, SmsSenderModel>().ReverseMap();
            CreateMap<SubscriptionType, SubscriptionTypeModel>().ReverseMap();
            CreateMap<SubscriptionTypeModel, SubscriptionTypeViewModel>().ReverseMap();
            CreateMap<UserSubscriptionModel, SubscriptionViewModel>().ReverseMap();
            CreateMap<FrozenSubscription, FrozenSubscriptionModel>().ForMember(x => x.UserNameCreated, opt => opt.MapFrom(u => u.AspNetUser.FullName))
                .ForMember(x => x.UserNameFinished, opt => opt.MapFrom(u => u.AspNetUser1.FullName))
                .ForMember(x => x.SubscriptionUser, opt => opt.MapFrom(u => u.UserSubscription.UserId))
                .ReverseMap();
            CreateMap<UserSMSs, SmsLog>().ForMember(x => x.UserName, opt => opt.MapFrom(u => u.AspNetUser.UserName))
                .ForMember(x => x.FullName, opt => opt.MapFrom(u => u.AspNetUser.FullName))
                .ForMember(x => x.MessageType, opt => opt.MapFrom(u => (MessageType)u.TypeId))                
                .ReverseMap();
            CreateMap<Company, CompanyConfiguration>().ReverseMap();
            CreateMap<DAL.Studio, Utilities.Studio>().ReverseMap();
            CreateMap<StudioModel, Utilities.Studio>().ReverseMap();
            CreateMap<StudioRoomModel, StudioRoomViewModel>().ReverseMap();
            CreateMap<TipModel, TipViewModel>().ReverseMap();
            CreateMap<TipModel, Tip>().ForMember(x => x.Short, opt => opt.MapFrom(u => u.Tip)).ReverseMap();
            CreateMap<Tip, TipModel>().ForMember(x => x.Tip, opt => opt.MapFrom(u => u.Short)).ReverseMap();

            CreateMap<MobileDeviceModel,UserMobileDevice>().ReverseMap();
            CreateMap<APIMobileDevice, MobileDeviceModel>().ReverseMap();

            CreateMap<ExpensesModel, StudioExpens>().ReverseMap();
            CreateMap<ExpensesViewModel, ExpensesModel>().ReverseMap();

            //CreateMap<ClassPlacement, ClassPlacementModel>().ForMember(x => x.ClassPlacementNumber, opt => opt.MapFrom(u => u.Number)).ReverseMap();
            //CreateMap<ClassPlacementModel, ClassPlacement>().ForMember(x => x.Number, opt => opt.MapFrom(u => u.ClassPlacementNumber)).ReverseMap();
            CreateMap<StudioPlacement, StudioPlacementModel>().ReverseMap();

            CreateMap<ClassAvailablePlacement, AvailablePlacementsModel>().ForMember(x => x.ClassEnrollment, opt => opt.MapFrom(u => u.ClassEnrollments.FirstOrDefault(e=>!e.IsDeleted))).ReverseMap();
            CreateMap<ClassAvailablePlacement, CalendarAvailablePlacementsModel>()
                //.ForMember(x => x.ClassEnrollment, opt => opt.MapFrom(u => u.ClassEnrollments.FirstOrDefault(e => !e.IsDeleted)))
                .ReverseMap();

            //CreateMap<ClassAvailablePlacement, ClassPlacementModel>().ReverseMap();

            CreateMap<EnrollmentComment, EnrollmentCommentModel>()
                .ForMember(x => x.CommentBy, opt => opt.MapFrom(u => u.AspNetUser.FullName))
                .ReverseMap();

            CreateMap<DailyExport, DailyExportModel>()
                .ForMember(x => x.Gender, opt => opt.MapFrom(u => (Gender)u.Gender))
                .ReverseMap();
        }
    }
}
