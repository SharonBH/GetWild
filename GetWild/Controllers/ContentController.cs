using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace GetWild.Controllers
{
    public class ContentController : Controller
    {

        public PartialViewResult TandC()
        {
            return PartialView("TandCContent");
        }

        public PartialViewResult HealthTandC()
        {
            return PartialView("HealthTandCContent");
        }

    }
}
