using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public static class ServiceRepo
    {
        public static void RunExpireSubscriptions()
        {
            try
            {
                using (var context = new InShapeEntities())
                {
                    context.ExpireSubscriptions();
                    context.SetLastClass();
                    context.DailySystemAlerts();
                    context.ResetLateCancel();
                    //context.Database.ExecuteSqlCommand("Exec ExpireSubscriptions");
                    //context.Database.ExecuteSqlCommand("Exec SetLastClass");
                    //context.Database.ExecuteSqlCommand("Exec DailySystemAlerts");
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
    }
}
