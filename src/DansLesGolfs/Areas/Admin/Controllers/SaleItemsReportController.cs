using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using DansLesGolfs.Reports;
using DevExpress.XtraReports.Parameters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Admin.Controllers
{
    public class SaleItemsReportController : BaseAdminController
    {
        public ActionResult Index()
        {
            ViewBag.Title = Resources.Resources.SaleDetail;
            ViewBag.TitleName = Resources.Resources.SaleDetail;
            ViewBag.ClassName = "saleitemsreport";
            Breadcrumbs.Add(Resources.Resources.SaleDetail, "~/Admin/SaleItemsReport");
            InitBreadcrumbs();

            ViewBag.ToDate = DateTime.Today;
            ViewBag.FromDate = DateTime.Today;

            return View();
        }

        public ActionResult ViewReport(string fromDate, string toDate)
        {
            rpSaleItems report = GetReportByParams(fromDate, toDate);
            return View(report);
        }
        public ActionResult DocumentViewerPartial(string fromDate, string toDate)
        {
            rpSaleItems report = GetReportByParams(fromDate, toDate);
            return PartialView("DocumentViewerPartial", report);
        }

        public ActionResult ExportDocumentViewer(string fromDate, string toDate)
        {
            rpSaleItems report = GetReportByParams(fromDate, toDate);
            return DevExpress.Web.Mvc.DocumentViewerExtension.ExportTo(report);
        }

        private rpSaleItems GetReportByParams(string fromDate, string toDate)
        {
            jQueryDataTableParamModel param = new jQueryDataTableParamModel();
            param.search = Request.QueryString["search[value]"];
            List<SaleItemReport> list = DataAccess.GetSaleItemsReport(param, fromDate, toDate, 0, this.CultureId);

            list.ForEach(it =>
            {
                it.PaymentType = TransactionHelper.GetPaymentTypeText(it.PaymentType);
                it.CategoryName = TransactionHelper.GetCategoryText(it.ItemTypeId);
            });

            rpSaleItems report = new rpSaleItems();
            report.Name = "SaleItemsReport_" + DateTime.Now.ToString("yyyy-MM-dd");
            report.DataSource = list;
            report.Parameters["ReportTitle"].Value = Resources.Resources.SaleDetail;
            report.Parameters["FromDate"].Value = Resources.Resources.From + " : " + fromDate;
            report.Parameters["ToDate"].Value = Resources.Resources.To + " : " + toDate;

            // Set report headers.
            report.Parameters["TableHeaderMonth"].Value = Resources.Resources.Month;
            report.Parameters["TableHeaderWeeks"].Value = Resources.Resources.NumberOfWeeks;
            report.Parameters["TableHeaderOrderDate"].Value = Resources.Resources.OrderDate;
            report.Parameters["TableHeaderTransactionId"].Value = Resources.Resources.Invoice;
            report.Parameters["TableHeaderOrderNumber"].Value = Resources.Resources.OrderNumber;
            report.Parameters["TableHeaderCategory"].Value = Resources.Resources.Category;
            report.Parameters["TableHeaderItemCode"].Value = Resources.Resources.ItemCode;
            report.Parameters["TableHeaderItemName"].Value = Resources.Resources.ItemName;
            report.Parameters["TableHeaderGolfName"].Value = Resources.Resources.SiteName;
            report.Parameters["TableHeaderFirstname"].Value = Resources.Resources.FirstName;
            report.Parameters["TableHeaderLastname"].Value = Resources.Resources.LastName;
            report.Parameters["TableHeaderPaymentMethod"].Value = Resources.Resources.PaymentType;
            report.Parameters["TableHeaderQuantity"].Value = Resources.Resources.Quantity;
            report.Parameters["TableHeaderTotalBasePrice"].Value = Resources.Resources.TotalBasePrice;
            report.Parameters["TableHeaderDiscount"].Value = Resources.Resources.Discount;
            report.Parameters["TableHeaderShippingCost"].Value = Resources.Resources.ShippingCost;
            report.Parameters["TableHeaderTotalTTC"].Value = "Total TTC";
            report.Parameters["TableHeaderTotalHT"].Value = "Total HT";
            report.Parameters["TableHeaderGolfBrand"].Value = Resources.Resources.GolfBrand;

            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            return report;
        }

        public ActionResult AjaxLoadData(jQueryDataTableParamModel param, string fromDate, string toDate)
        {
            try
            {
                if ((string.IsNullOrEmpty(fromDate) && string.IsNullOrWhiteSpace(fromDate))
                    || (string.IsNullOrEmpty(toDate) && string.IsNullOrWhiteSpace(toDate)))
                {
                    fromDate = DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy");
                    toDate = DateTime.Today.ToString("dd/MM/yyyy");
                }

                param.search = Request.QueryString["search[value]"];
                List<SaleItemReport> list = DataAccess.GetSaleItemsReport(param, fromDate, toDate, Auth.User.SiteId, this.CultureId);
                list.ForEach(it =>
                {
                    it.PaymentType = TransactionHelper.GetPaymentTypeText(it.PaymentType);
                    it.CategoryName = TransactionHelper.GetCategoryText(it.ItemTypeId);
                });
                IEnumerable<object[]> resultArray = GetDataArray(list);
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

        private IEnumerable<object[]> GetDataArray(List<SaleItemReport> list)
        {
            List<string[]> returnData = new List<string[]>();
            List<string> htmlArray = null;
            Type type = typeof(SaleItemReport);
            PropertyInfo[] props = type.GetProperties();
            int columnLength = props.Length;
            string value = string.Empty;
            long orderId = 0;
            string paymentStatus = string.Empty;

            foreach (SaleItemReport row in list)
            {
                orderId = 0;
                paymentStatus = string.Empty;
                // Find out order Id
                props.Where(it => it.Name == "OrderId").ToList().ForEach(it =>
                {
                    orderId = DataManager.ToLong(it.GetValue(row));
                });
                // Find out payment status
                props.Where(it => it.Name == "PaymentStatus").ToList().ForEach(it =>
                {
                    paymentStatus = DataManager.ToString(it.GetValue(row));
                });

                htmlArray = new List<string>();
                for (int i = 0; i < columnLength; i++)
                {
                    if (props[i].Name == "OrderId" || props[i].Name == "ItemTypeId" || props[i].Name == "PaymentStatus")
                    {
                        continue;
                    }
                    else if (props[i].Name == "OrderNumber" && orderId > 0)
                    {
                        value = DataManager.ToString(props[i].GetValue(row));
                        htmlArray.Add("<a href=\"" + Url.Content("~/Admin/Orders/ViewOrderInvoice/" + orderId) + "\" target=\"_blank\">" + value + "</a>");
                    }
                    else
                    {
                        htmlArray.Add(DataManager.ToString(props[i].GetValue(row)));
                    }
                }
                returnData.Add(htmlArray.ToArray());
            }

            return returnData;
        }
    }
}
