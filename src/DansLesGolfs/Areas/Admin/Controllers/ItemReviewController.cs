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
    public class ItemReviewController : BaseAdminController
    {
        public ActionResult Index()
        {
            ViewBag.Title = Resources.Resources.ItemReview;
            ViewBag.TitleName = Resources.Resources.ItemReview;
            ViewBag.ClassName = "itemreview";
            Breadcrumbs.Add(Resources.Resources.SiteReview, "~/Admin/ItemReview");
            InitBreadcrumbs();

            return View();
        }

        public ActionResult AjaxLoadData(jQueryDataTableParamModel param)
        {
            try
            {
                param.search = Request.QueryString["search[value]"];
                List<ItemReview> itemReviews = DataAccess.GetAllItemReviews(param, this.CultureId);
                IEnumerable<object[]> resultArray = GetDataArray(itemReviews);
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

        private IEnumerable<object[]> GetDataArray(List<ItemReview> items)
        {
            List<string[]> returnData = new List<string[]>();
            string[] htmlArray = null;
            foreach (var it in items)
            {
                htmlArray = new string[4];
                htmlArray[0] = "<input type=\"checkbox\" class=\"checkbox review-" + it.ItemReviewId + "\" value=\"" + it.ItemReviewId + "\" />";
                htmlArray[1] = "<h4>(" + it.Rating + "/5) " + (String.IsNullOrEmpty(it.Subject) || String.IsNullOrWhiteSpace(it.Subject) ? Resources.Resources.Untitled : it.Subject) + "</h4>\n";
                htmlArray[1] += "<p>" + it.Message.StripHtml() + "</p>\n";
                htmlArray[1] += "<p> " + Resources.Resources.On + " <a href=\"" + Url.Content("~/Item/" + it.ItemSlug) + "\" target=\"_blank\">" + it.ItemName + "</a><br />";
                htmlArray[1] += Resources.Resources.Status + " : <span class=\"review-status " + (it.IsApproved ? "approved" : "unapproved") + "\">" + (it.IsApproved ? Resources.Resources.Approved : Resources.Resources.Unapproved) + "</span>";
                htmlArray[1] += "</p>";
                htmlArray[2] += "<a href=\"javascript:void(0);\" class=\"approve-link\" data-id=\"" + it.ItemReviewId + "\" data-approved=\"" + (it.IsApproved ? 0 : 1) + "\">" + (it.IsApproved ? Resources.Resources.Unapproved : Resources.Resources.Approved) + "</a>";
                htmlArray[3] += "<a href=\"javascript:void(0);\" class=\"delete-link\" data-id=\"" + it.ItemReviewId + "\">" + Resources.Resources.Delete + "</a>";
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

                DataAccess.DeleteItemReview(ids);
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

                DataAccess.UpdateItemReviewApproval(id, isApproved);

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
