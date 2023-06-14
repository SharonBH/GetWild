using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public class CompanyConfiguration
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ManagerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime StartDate { get; set; }
        public bool UseSMS { get; set; }
        public bool UseHosting { get; set; }
        public bool Active { get; set; }
        public bool IsDeleted { get; set; }
        public string CompanyCode { get; set; }

        public List<Studio> Studios { get; set; }

        public static void Initialize()
        {
            var c = SystemBLL.GetCompany(ctx.Request.Uri.Host);
        }

        public class Studio
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string ManagerName { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            public DateTime StartDate { get; set; }
            public bool Active { get; set; }
            public int CompanyId { get; set; }
            public bool IsDeleted { get; set; }
        }

    }
}
