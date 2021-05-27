using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Controllers
{
    public class HomeController : BaseFrontController
    {
        [HttpGet]
        public ActionResult Index(int? page = 1)
        {
            AddSystemBodyClass("home");

            return View();
        }

        public ActionResult TopNavIcon1()
        {
            int catId = DataManager.ToInt(DataAccess.GetOption("TopNavIconGreenFeeCatID1"), 0);
            return Redirect("~/GreenFees" + (catId > 0 ? "?cat=" + catId : ""));
        }

        public ActionResult TopNavIcon2()
        {
            int catId = DataManager.ToInt(DataAccess.GetOption("TopNavIconGreenFeeCatID2"), 0);
            return Redirect("~/GreenFees" + (catId > 0 ? "?cat=" + catId : ""));
        }

        public ActionResult GiftCard()
        {
            return Redirect("~/DLGCard");
        }

        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture;  // set culture
            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);
            return RedirectToAction("Index");
        }

        public ActionResult Unsubscribe(string email, string id)
        {
            try
            {
                ViewBag.UnsubscribeReasons = DataAccess.GetUnsubscribeReasons();

                if (string.IsNullOrWhiteSpace(email))
                {
                    if (Auth.User != null)
                    {
                        email = Auth.User.Email;
                    }
                    else
                    {
                        return Redirect("~/");
                    }
                }

                string campaignId = "", emailId = "", siteId = "";
                if (!String.IsNullOrWhiteSpace(id))
                {
                    string[] parts = DataProtection.Decrypt(id).Split('|');
                    if (parts.Length == 3)
                    {
                        campaignId = parts[0];
                        emailId = parts[1];
                        siteId = parts[2];
                    }
                }

                ViewBag.Email = email.Trim();
                ViewBag.CampaignId = campaignId;
                ViewBag.EmailId = emailId;
                ViewBag.SiteId = siteId;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
            return View();
        }

        [HttpPost]
        public ActionResult ProcessUnsubscribe(string email, string campaignId, string emailId, string siteId)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Redirect("~/");

            try
            {
                string unsubscribe_reason_id = Request.Form["unsubscribe_reason_id"];
                string other_detail = Request.Form["other_detail"];

                DataAccess.ChangeConnectionString("ECMConnectionString");

                long trackingId = 0;
                long cid = DataManager.ToLong(campaignId);
                long eid = DataManager.ToLong(emailId);
                long sid = DataManager.ToLong(siteId);
                if (cid > 0 && eid > 0)
                {
                    trackingId = DataAccess.AddEmailTracking(cid, eid, "unsubscribe", "", "", "", "");
                }

                DataAccess.UnSubscribeUserEmail(email, sid, DataManager.ToLong(unsubscribe_reason_id), other_detail, trackingId);
                DataAccess.SetMailUnsubscribe(cid, eid);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }

            return RedirectToAction("UnsubscribeSuccess");
        }
        public ActionResult UnsubscribeSuccess()
        {
            return View();
        }

        private ProductsListModel GetAllProducts(int page, int pageSize = 10)
        {
            ProductsListModel model = new ProductsListModel();
            if (page < 1)
                page = 1;

            int totalPages = 0;
            model.Items = DataAccess.GetLatestItems(out totalPages, pageSize, 0, null, null, null, null, string.Empty, null, string.Empty, 0, null, this.CultureId, string.Empty);
            //model.Page = page;
            //model.TotalPages = totalPages;
            model.TotalPages = 0;

            model.Items.Where(it => string.IsNullOrEmpty(it.ItemName) || string.IsNullOrWhiteSpace(it.ItemName)).ToList().ForEach(it =>
            {
                it.ItemName = Resources.Resources.Untitled;
            });

            return model;
        }
    }
}
