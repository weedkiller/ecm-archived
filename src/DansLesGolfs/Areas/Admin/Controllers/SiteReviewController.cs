using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Admin.Controllers
{
    public class SiteReviewController : BaseAdminController
    {
        public ActionResult Index()
        {
            ViewBag.Title = Resources.Resources.SiteReview;
            ViewBag.TitleName = Resources.Resources.SiteReview;
            ViewBag.ClassName = "sitereview";
            Breadcrumbs.Add(Resources.Resources.SiteReview, "~/Admin/SiteReview");
            InitBreadcrumbs();

            return View();
        }

        public ActionResult AjaxLoadData(jQueryDataTableParamModel param)
        {
            try
            {
                param.search = Request.QueryString["search[value]"];
                List<SiteReview> siteReviews = DataAccess.GetAllSiteReviews(param, this.CultureId);
                IEnumerable<object[]> resultArray = GetDataArray(siteReviews);
                var data = new
                {
                    draw = param.draw,
                    recordsTotal = param.recordsTotal,
                    recordsFiltered = param.recordsTotal,
                    data = resultArray
                };

                return new CorrectJsonResult
                {
                    Data = data,
                    ContentType = "application/json",
                    ContentEncoding = Encoding.UTF8
                };
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private IEnumerable<object[]> GetDataArray(List<SiteReview> sites)
        {
            List<string[]> returnData = new List<string[]>();
            string[] htmlArray = null;
            string html = string.Empty;
            foreach (var it in sites)
            {
                htmlArray = new string[4];
                htmlArray[0] = "<input type=\"checkbox\" class=\"checkbox review-" + it.SiteReviewId + "\" value=\"" + it.SiteReviewId + "\" />";
                htmlArray[1] = "<h4>(" + it.Rating + "/5) " + (String.IsNullOrEmpty(it.Subject) || String.IsNullOrWhiteSpace(it.Subject) ? Resources.Resources.Untitled : it.Subject) + "</h4>\n";
                htmlArray[1] += "<p>" + it.Message.StripHtml() + "</p>\n";
                htmlArray[1] += "<p> " + Resources.Resources.On + " <a href=\"" + Url.Content("~/Site/" + it.SiteSlug) + "\" target=\"_blank\">" + it.SiteName + "</a><br />";
                htmlArray[1] += Resources.Resources.Status + " : <span class=\"review-status " + (it.IsApproved ? "approved" : "unapproved") + "\">" + (it.IsApproved ? Resources.Resources.Approved : Resources.Resources.Unapproved) + "</span>";
                htmlArray[1] += "</p>";
                htmlArray[2] += "<a href=\"javascript:void(0);\" class=\"approve-link\" data-id=\"" + it.SiteReviewId + "\" data-approved=\"" + (it.IsApproved ? 0 : 1) + "\">" + (it.IsApproved ? Resources.Resources.Unapproved : Resources.Resources.Approved) + "</a>";
                htmlArray[3] += "<a href=\"javascript:void(0);\" class=\"delete-link\" data-id=\"" + it.SiteReviewId + "\">" + Resources.Resources.Delete + "</a>";
                returnData.Add(htmlArray);
            }
            return returnData;
        }

        public ActionResult AjaxDelete(int[] ids)
        {
            try
            {
                if (ids == null)
                    throw new Exception("Please specific ids parameter.");

                DataAccess.DeleteSiteReview(ids);
                return Json(new
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        public ActionResult AjaxSetApproval(int id, int approved = 1)
        {
            try
            {
                bool isApproved = approved == 1;

                DataAccess.UpdateSiteReviewApproval(id, isApproved);

                string text = string.Empty;
                string className = string.Empty;
                if (isApproved)
                {
                    text = Resources.Resources.Approved;
                    className = "approved";
                }
                else
                {
                    text = Resources.Resources.Unapproved;
                    className = "unapproved";
                }
                return Json(new
                {
                    isSuccess = true,
                    text = text,
                    className = className
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
    }
}
