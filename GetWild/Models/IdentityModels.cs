using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using InShapeModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using Utilities;

namespace GetWild.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DOB { get; set; }

        //public AgeGroup AgeGroup
        //{
        //    get
        //    {
        //        if (DOB == null) return AgeGroup.מבוגרים;
        //        var today = DateTime.UtcNow.ToLocal();
        //        int age = today.Year - DOB.Value.Year;

        //        if (today.Month > DOB.Value.Month || (today.Month == DOB.Value.Month && today.Day > DOB.Value.Day))
        //            age++;

        //        return age > App.CurrentCompany.AdultAge ? AgeGroup.מבוגרים : age > App.CurrentCompany.TeenAge ? AgeGroup.נוער : AgeGroup.ילדים;
        //    }
        //}

        public AgeGroup AgeGroup { get; set; }

        public string Address { get; set; }

        public DateTime JoinDate { get; set; }

        public DateTime? ProfileUpdateDate { get; set; }

        public string ProfileIMG { get; set; }

        public bool ReceiveSMS { get; set; }

        public bool Marked { get; set; }

        //public Profile UserProfile { get; set; }
        public string FullMame { get { return FirstName + " " + LastName; } }

        public string ProfileIMGPath { get { return string.IsNullOrEmpty(ProfileIMG) ? @"/images/Members/no-profile.jpg" : @"/images/Members/" + ProfileIMG ; } }

        public Gender Gender { get; set; }

        public bool AcceptedTandC { get; set; }

        public bool SignedHealthTandC { get; set; }

        public DateTime? SignedDate { get; set; }

        public string SignatureIMGPath
        {
            get { return SignedHealthTandC ? $"/images/Members/{Id}_signature.png" + ProfileIMG : string.Empty; }
        }


        public long? CitizenId { get; set; }

        public string Occupation { get; set; }

        public int StudioId { get; set; }

        public bool Active { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<GetWild.Models.ClassViewModel> ClassViewModels { get; set; }

        public System.Data.Entity.DbSet<InShapeModels.ClassTypeModel> ClassTypeModels { get; set; }

        public System.Data.Entity.DbSet<InShapeModels.StudioRoomModel> StudioRoomModels { get; set; }

    }

    public class UserWithCompany
    {
        public ApplicationUser CurrentUser { get; set; }

        public CompanyConfiguration CurrentCompany { get; set; }
    }
}