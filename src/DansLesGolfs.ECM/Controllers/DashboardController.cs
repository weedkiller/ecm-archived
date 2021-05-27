using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.ECM.Controllers
{
    public class DashboardController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.ClassName = "dashboard";
            return View();
        }
    }
}
