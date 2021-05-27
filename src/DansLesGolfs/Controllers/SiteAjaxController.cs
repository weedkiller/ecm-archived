using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using DansLesGolfs.Data;
using DansLesGolfs.Base;

namespace DansLesGolfs.Controllers
{
    public class SiteAjaxController : BaseController
    {
        [HttpPost]
        public JsonResult UserGiveRating(long siteId = 0, byte rating = 1, string subject = "", string message = "")
        {
            try
            {
                if (Auth.User == null)
                    throw new Exception("Please login before give a rating.");

                int averageRating = 0, reviewNumber = 0;
                DataAccess.SaveSiteReview(siteId, Auth.User.UserId, rating, subject, message, out averageRating, out reviewNumber);
                SiteReview review = new SiteReview()
                {
                    Rating = rating,
                    Message = message
                };
                List<SiteReview> reviews = DataAccess.GetSiteReviewsBySiteId(siteId);
                string html = GetHTMLFromView("~/Views/_Shared/UC/Front/Site/UCSiteReviews.cshtml", reviews);
                return Json(new
                {
                    isSuccess = true,
                    html = html,
                    averageRating = averageRating,
                    reviewNumber = reviewNumber
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isResult = false,
                    message = ex.Message
                });
            }
        }
    }
}
