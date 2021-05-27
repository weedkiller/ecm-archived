using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using System.Data;
using DansLesGolfs.Models;
using DansLesGolfs.Base;

namespace DansLesGolfs.Controllers
{
    public class FindCourseController : BaseFrontController
    {
        public FindCourseController()
            : base()
        {
            LoadAdsList("Default");
        }

        public ActionResult Index()
        {
            DataSet ds = DataAccess.GetSitePinData(this.CultureId);
            List<PinModel> pinsList = new List<PinModel>();
            Item item = new Item();
            HtmlHelper html = new HtmlHelper(new ViewContext(), new ViewPage());
            PinModel pinModel = null;
            DataTable table = ds.Tables[1];
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                pinModel = new PinModel()
                {
                    SiteId = DataManager.ToInt(row["SiteId"]),
                    SiteName = DataManager.ToString(row["SiteName"]),
                    SiteDescription = DataManager.ToString(row["Description"]).Replace(Environment.NewLine, "<br />").Replace("\"", "\\\"").StripHtml(),
                    SiteUrl = Url.Content("~/Site/" + DataManager.ToString(row["SiteSlug"])),
                    Latitude = DataManager.ToDecimal(row["Latitude"]),
                    Longitude = DataManager.ToDecimal(row["Longitude"])
                };
                pinModel.HasGreenFees = table.Select("SiteId = " + pinModel.SiteId + " AND ItemTypeId = " + (int)ItemType.Type.GreenFee).Length > 0;
                pinModel.HasStayPackages = table.Select("SiteId = " + pinModel.SiteId + " AND ItemTypeId = " + (int)ItemType.Type.StayPackage).Length > 0;
                pinModel.HasGolfLessons = table.Select("SiteId = " + pinModel.SiteId + " AND ItemTypeId = " + (int)ItemType.Type.GolfLesson).Length > 0;
                pinModel.HasDrivingRanges = table.Select("SiteId = " + pinModel.SiteId + " AND ItemTypeId = " + (int)ItemType.Type.DrivingRange).Length > 0;
                pinModel.HasResellerProducts = table.Select("SiteId = " + pinModel.SiteId + " AND ItemTypeId = " + (int)ItemType.Type.Product).Length > 0;
                pinModel.PinIcon = GetPinIcon(pinModel);
                pinsList.Add(pinModel);
            }

            int pageSize = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["RelatedProductsPageSize"], 4);
            int totalPages = 1;
            ViewBag.RelatedItems = DataAccess.GetLatestItems(out totalPages, pageSize, 1, null, null, null, null, "", null, "", (int)ItemType.Type.All, null, this.CultureId);
            return View(pinsList);
        }

        [HttpPost]
        public ActionResult Index(int? countryId, int? regionId, int? stateId, long? siteId, int? page = 1)
        {
            page = !page.HasValue || page.Value < 1 ? 1 : page.Value;
            DataSet ds = DataAccess.GetSitePinData(CultureId, countryId, regionId, stateId, siteId);
            List<PinModel> pinsList = new List<PinModel>();
            Item item = new Item();
            HtmlHelper html = new HtmlHelper(new ViewContext(), new ViewPage());
            PinModel pinModel = null;
            DataTable table = ds.Tables[1];
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                pinModel = new PinModel()
                {
                    SiteId = DataManager.ToInt(row["SiteId"]),
                    SiteName = DataManager.ToString(row["SiteName"]),
                    SiteDescription = DataManager.ToString(row["Description"]).Replace(Environment.NewLine, "<br />").Replace("\"", "\\\"").StripHtml(),
                    SiteUrl = Url.Content("~/Site/" + DataManager.ToString(row["SiteSlug"])),
                    Latitude = DataManager.ToDecimal(row["Latitude"]),
                    Longitude = DataManager.ToDecimal(row["Longitude"])
                };
                pinModel.HasGreenFees = table.Select("SiteId = " + pinModel.SiteId + " AND ItemTypeId = " + (int)ItemType.Type.GreenFee).Length > 0;
                pinModel.HasStayPackages = table.Select("SiteId = " + pinModel.SiteId + " AND ItemTypeId = " + (int)ItemType.Type.StayPackage).Length > 0;
                pinModel.HasGolfLessons = table.Select("SiteId = " + pinModel.SiteId + " AND ItemTypeId = " + (int)ItemType.Type.GolfLesson).Length > 0;
                pinModel.HasDrivingRanges = table.Select("SiteId = " + pinModel.SiteId + " AND ItemTypeId = " + (int)ItemType.Type.DrivingRange).Length > 0;
                pinModel.HasResellerProducts = table.Select("SiteId = " + pinModel.SiteId + " AND ItemTypeId = " + (int)ItemType.Type.Product).Length > 0;
                pinModel.PinIcon = GetPinIcon(pinModel);
                pinsList.Add(pinModel);
            }

            int pageSize = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["RelatedProductsPageSize"], 4);
            int totalPages = 1;
            ViewBag.RelatedItems = DataAccess.GetLatestItems(out totalPages, pageSize, 1, countryId, regionId, stateId, siteId, "", null, "", (int)ItemType.Type.All, null, this.CultureId);
            return View(pinsList);
        }

        private string GetPinIcon(PinModel pinModel)
        {
            string pinIcon = "pin-";
            if (pinModel.HasGreenFees && pinModel.HasStayPackages && pinModel.HasGolfLessons && pinModel.HasDrivingRanges && pinModel.HasResellerProducts)
            {
                pinIcon += "all";
            }
            else if (!pinModel.HasGreenFees && !pinModel.HasStayPackages && !pinModel.HasGolfLessons && !pinModel.HasDrivingRanges && !pinModel.HasResellerProducts)
            {
                pinIcon += "none";
            }
            else
            {
                if (pinModel.HasGreenFees)
                {
                    pinIcon += "g";
                }
                if (pinModel.HasStayPackages)
                {
                    pinIcon += "s";
                }
                if (pinModel.HasGolfLessons)
                {
                    pinIcon += "l";
                }
                if (pinModel.HasDrivingRanges)
                {
                    pinIcon += "d";
                }
                if (pinModel.HasResellerProducts)
                {
                    pinIcon += "i";
                }
            }
            pinIcon += ".png";
            return pinIcon;
        }
    }
}
