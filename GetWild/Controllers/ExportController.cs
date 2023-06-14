using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using BLL;
using Utilities;

namespace GetWild.Controllers
{
    [Authorize(Roles = "Instructor, ClassInstructor, admin")]
    public class ExportController : InShapeMVCController
    {
        // GET: Export
        public FileStreamResult ExportUserWithSubscription()
        {
            var result = ExportCSV.ExportUserWithSubscription(CurrentUser.StudioId);
            var memoryStream = new MemoryStream(result);
            return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = $"Users_{DateTime.UtcNow.Date.ToLocal().ToShortDateString()}.csv" };
        }

        public FileStreamResult ExportWeeklyUserWithSubscription(int id, int? weekno, int ut = 0, bool includeForzen = false)
        {
            var result = ExportCSV.ExportWeeklyUserWithSubscription(CurrentUser.StudioId, id, weekno, ut, includeForzen);
            var memoryStream = new MemoryStream(result);
            return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = $"Users_{DateTime.UtcNow.Date.ToLocal().ToShortDateString()}.csv" };
        }

        public FileStreamResult ExportUserWithTicketSubscription(bool includefrozen, int ut)
        {
            var result = ExportCSV.ExportUserWithTicketSubscription(CurrentUser.StudioId, includefrozen, ut);
            var memoryStream = new MemoryStream(result);
            return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = $"Users_{DateTime.UtcNow.Date.ToLocal().ToShortDateString()}.csv" };
        }

        public FileStreamResult ExportUserWithNoEnrollments()
        {
            var result = ExportCSV.ExportUserWithNoEnrollments(CurrentUser.StudioId);
            var memoryStream = new MemoryStream(result);
            return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = $"Inactive_users_{DateTime.UtcNow.Date.ToLocal().ToShortDateString()}.csv" };
        }

        public FileStreamResult ExportFrozenUsers()
        {
            var result = ExportCSV.ExportFrozenUsers(CurrentUser.StudioId);
            var memoryStream = new MemoryStream(result);
            return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = $"Frozen_users_{DateTime.UtcNow.Date.ToLocal().ToShortDateString()}.csv" };
        }

        


        public FileStreamResult ExportEnrollmentBydate(string date, int? userRole, bool activated = false, bool isLateCancel = false)
        {
            DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var result = ExportCSV.ExportEnrollmentBydate(CurrentUser.StudioId, dt, userRole, activated, isLateCancel);
            var memoryStream = new MemoryStream(result);
            return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = $"DailyUsers_{dt.ToShortDateString()}.csv" };
        }
    }
}