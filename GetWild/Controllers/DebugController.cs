using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utilities;

namespace GetWild.Controllers
{
    public class DebugController : InShapeMVCController
    {
        // GET: Debug
        public PartialViewResult DebugInfo()
        {
            return App.Configuration.IsDebug ? PartialView("_DebugInfo", CurrentCompany?? App.DefaultCompany) : null;
        }
    }
}