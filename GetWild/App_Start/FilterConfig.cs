using GetWild.App_Start;
using System.Web;
using System.Web.Mvc;

namespace GetWild
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new ForceLogoutFilter());
            //filters.Add(new SetCompanyActionFilter());
        }
    }
}
