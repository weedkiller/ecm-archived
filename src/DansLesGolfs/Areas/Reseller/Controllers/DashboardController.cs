using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Reseller.Controllers
{
    public class DashboardController : BaseResellerController
    {
        public ActionResult Index()
        {
            List<object> ordersJSON = new List<object>();
            DateTime now = DateTime.Now;
            DateTime endTime = now.AddHours(-24);
            for (DateTime tempTime = endTime; tempTime < now; tempTime = tempTime.AddHours(1))
            {
                ordersJSON.Add(new int[] { tempTime.Hour, 0 });
            }
            ViewBag.OrdersJSON = ordersJSON;
            return View();
        }
    }
}
