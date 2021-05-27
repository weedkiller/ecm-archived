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
    public class SiteController : BaseFrontController
    {
        int pageSize = 4;
        public ActionResult Detail(string slug)
        {
            Site site = null;
            slug = slug.Trim();
            if(String.IsNullOrEmpty(slug))
            {
                int id = DataManager.ToInt(Request.QueryString["id"], 0);
                if(id > 0)
                {
                    site = DataAccess.GetSiteById(id, this.CultureId);
                }
                else
                {
                    return View("NotFound");
                }
            }
            else
            {
                slug = WebHelper.GenerateSlug(slug);
                site = DataAccess.GetSiteBySlug(slug, this.CultureId);
                if(site == null)
                {
                    return View("NotFound");
                }
            }

            // Finding your rating.
            int yourRating = 0;
            if (Auth.User != null && site.SiteReviews != null && site.SiteReviews.Any())
            {
                var result = site.SiteReviews.Where(it => it.UserId == Auth.User.UserId);
                if (result != null && result.Any())
                {
                    yourRating = result.First().Rating;
                }
            }
            ViewBag.YourRating = yourRating;

            LoadRelateItems(site);

            LoadAdsList("Default");

            return View(site);
        }

        private void LoadRelateItems(Site site)
        {
            ProductsListModel relatedItemsModel = new ProductsListModel();
                pageSize = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["RelatedProductsPageSize"], 4);
            int totalPages = 0;
            relatedItemsModel.Items = DataAccess.GetLatestItems(totalPages: out totalPages, 
                pageSize: pageSize, pageIndex: 1, 
                siteId: site.SiteId, langId: this.CultureId);
            relatedItemsModel.IsShowNoItemText = false;
            ViewBag.RelatedItems = relatedItemsModel;
        }

    }
}
