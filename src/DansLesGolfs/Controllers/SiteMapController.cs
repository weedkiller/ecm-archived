using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using System.Text;
using DansLesGolfs.Models;

namespace DansLesGolfs.Controllers
{
    public class SiteMapController : BaseFrontController
    {
        public ActionResult Index()
        {
            string xml = GetHTMLFromView("~/Views/SiteMap/Index.cshtml", null);
            return Content(xml, "text/xml", Encoding.UTF8);
        }

        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Product()
        {
            List<Item> items = DataAccess.GetProductSiteMap(this.CultureId);
            string xml = GetHTMLFromView("~/Views/SiteMap/Product.cshtml", items);
            return Content(xml, "text/xml", Encoding.UTF8);
        }

        [OutputCache(CacheProfile = "Medium")]
        public ActionResult Golf()
        {
            List<Site> sites = DataAccess.GetGolfSiteMap(this.CultureId);
            string xml = GetHTMLFromView("~/Views/SiteMap/Golf.cshtml", sites);
            return Content(xml, "text/xml", Encoding.UTF8);
        }

        public ActionResult Stylesheet()
        {
            return View();
        }
    }
}
