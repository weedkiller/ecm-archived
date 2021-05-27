using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Reseller.Controllers
{
    public class OrdersController : BaseResellerCRUDController
    {
        #region Constructor
        public OrdersController()
        {
            ObjectName = "Orders";
            TitleName = Resources.Resources.Invoice;
            PrimaryKey = "OrderId";
            ViewBag.HideAddButton = true;

            // Define Column Names.
            ColumnNames.Add("TransactionId", Resources.Resources.Invoice);
            ColumnNames.Add("OrderNumber", Resources.Resources.OrderNumber);
            ColumnNames.Add("CustomerName", Resources.Resources.CustomerName);
            ColumnNames.Add("TotalTTC", Resources.Resources.TotalTTC);
            ColumnNames.Add("PaymentType", Resources.Resources.PaymentType);
            ColumnNames.Add("PaymentStatus", Resources.Resources.Status);
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<Order> models = DataAccess.GetAllOrders(param, this.CultureId);
            return models;
        }

        protected override void DoPrepareForm(int? id = null)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Text = "Pending",
                Value = "pending"
            });
            list.Add(new SelectListItem()
            {
                Text = "Success",
                Value = "success"
            });
            list.Add(new SelectListItem()
            {
                Text = "On Hold",
                Value = "onhold"
            });
            list.Add(new SelectListItem()
            {
                Text = "Cancelled",
                Value = "cancelled"
            });
            ViewBag.PaymentStatusDropDownList = list;
        }

        protected override object DoPrepareNew()
        {
            Order model = new Order();
            ViewBag.Order = model;
            ViewBag.IsEditable = true;
            return model;
        }

        protected override object DoPrepareEdit(long id)
        {
            Order model = DataAccess.GetOrderByOrderId(id);
            ViewBag.Order = model;
            ViewBag.Address = DataAccess.GetAddressByAddressId(model.AddressId);
            ViewBag.IsEditable = model.PaymentStatus != "success";
            return model;
        }

        protected override bool DoSave()
        {
            long id = DataManager.ToLong(Request.Form["id"]);
            string paymentStatus = DataManager.ToString(Request.Form["PaymentStatus"]);
            DataAccess.UpdateOrder(id, paymentStatus);
            return true;
        }

        protected override void DoSaveSuccess(int id)
        {
            var orderItemIds = Request.Form.GetValues("OrderItemId").Select(it => DataManager.ToLong(it)).ToArray();
            var itemNames = Request.Form.GetValues("ItemName");
            var unitPrices = Request.Form.GetValues("UnitPrice").Select(it => DataManager.ToDecimal(it)).ToArray();
            var quantities = Request.Form.GetValues("Qty").Select(it => DataManager.ToInt(it)).ToArray();
            for (int i = 0, n = orderItemIds.Length; i < n; i++)
            {
                DataAccess.UpdateOrderItem(orderItemIds[i], itemNames[i], unitPrices[i], quantities[i]);
            }
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteOrder(id) > 0;
        }
        #endregion

        #region Action Methods

        public ActionResult ViewOrderInvoice(long? id)
        {
            DansLesGolfs.Reports.rpOrderInvoice report = GetOrderInvoiceReport(id);
            return View(report);
        }
        public ActionResult OrderInvoiceDocViewPartial(long? id)
        {
            DansLesGolfs.Reports.rpOrderInvoice report = GetOrderInvoiceReport(id);
            return PartialView("OrderInvoiceDocViewer", report);
        }

        public ActionResult OrderInvoiceExportDocumentViewer(long? id)
        {
            DansLesGolfs.Reports.rpOrderInvoice report = GetOrderInvoiceReport(id);
            return DevExpress.Web.Mvc.DocumentViewerExtension.ExportTo(report);
        }

        private Reports.rpOrderInvoice GetOrderInvoiceReport(long? id)
        {
            DansLesGolfs.Reports.rpOrderInvoice report = new Reports.rpOrderInvoice();
            report.DisplayName = "OrderInvoice_" + DateTime.Now.ToString("yyyy-MM-dd");

            Order order = DataAccess.GetOrderByOrderId(id.Value);
            if (order == null)
            {
                ViewBag.OrderId = 0;
                ViewBag.OrderNumber = "";
                ViewBag.ErrorMessage = "Not found order that has Order ID = " + id.Value + ".";
            }
            else
            {
                ViewBag.OrderId = order.OrderId;
                ViewBag.OrderNumber = order.OrderNumber;

                Address addr = DataAccess.GetAddress(order.AddressId);

                WebContent content = DataAccess.GetWebContentByKey("legal-information", this.CultureId);

                string siteUrl = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"];
                string siteEmail = System.Configuration.ConfigurationManager.AppSettings["SiteEmail"];

                report.DataMember = "";
                report.DataSource = order.OrderItems;

                // Assign parameters.
                report.Parameters["InvoiceTitle"].Value = Resources.Resources.Invoice.ToUpper();
                report.Parameters["TransactionId"].Value = Resources.Resources.InvoiceNumber + order.TransactionId;
                report.Parameters["OrderNumber"].Value = Resources.Resources.OrderNumberInvoice + order.OrderNumber;
                report.Parameters["OrderDate"].Value = "Date : " + order.OrderDate.ToString("dd/MM/yyyy");
                report.Parameters["PaymentType"].Value = Resources.Resources.MethodOfSettlement + " : " + TransactionHelper.GetPaymentTypeText(order.PaymentType);
                DevExpress.XtraReports.UI.XRRichText xrrichInvoiceMemo = report.FindControl("xrrichInvoiceMemo", true) as DevExpress.XtraReports.UI.XRRichText;
                if (xrrichInvoiceMemo != null)
                {
                    string invoiceMemo = String.Format(Resources.Resources.InvoiceMemo, siteUrl) + " : " + siteEmail;
                    System.Windows.Forms.RichTextBox textbox = new System.Windows.Forms.RichTextBox();
                    textbox.Text = invoiceMemo;
                    textbox.Select(invoiceMemo.LastIndexOf(siteEmail) - 1, siteEmail.Length);
                    textbox.SelectionFont = new System.Drawing.Font(xrrichInvoiceMemo.Font, System.Drawing.FontStyle.Bold);
                    xrrichInvoiceMemo.Rtf = textbox.Rtf;
                }

                if (addr != null)
                {
                    report.Parameters["BillingAddress"].Value = addr.ToString();
                }
                else
                {
                    report.Parameters["BillingAddress"].Value = string.Empty;
                }

                if (content != null)
                {
                    report.Parameters["LegalInformation"].Value = HttpUtility.HtmlDecode(content.ContentText.StripHtml());
                }
                else
                {
                    report.Parameters["LegalInformation"].Value = string.Empty;
                }

                // Table Header
                report.Parameters["TableHeaderDescription"].Value = Resources.Resources.Description;
                report.Parameters["TableHeaderUnitPrice"].Value = Resources.Resources.UnitPrice;
                report.Parameters["TableHeaderQuantity"].Value = Resources.Resources.Quantity;
                report.Parameters["TableHeaderTotalPrice"].Value = Resources.Resources.TotalTTC;

                // Summary Values
                decimal baseTotal = order.GetBaseTotal();
                decimal totalShippingCost = order.GetTotalShippingCost();
                decimal totalDiscount = order.GetDiscount();
                decimal totalPrice = order.GetTotalPrice();
                decimal totalVat = order.GetTotalVAT();
                decimal totalWithoutVat = totalPrice - totalVat;
                report.Parameters["BaseTTC"].Value = baseTotal;
                report.Parameters["PortTTC"].Value = totalShippingCost;
                report.Parameters["TotalDiscount"].Value = totalDiscount;
                report.Parameters["TotalWithoutVAT"].Value = totalWithoutVat;
                report.Parameters["TotalVAT"].Value = totalVat;
                report.Parameters["TotalWithVAT"].Value = totalPrice;
            }
            return report;
        }
        #endregion

        #region Private Methods
        #region GetPaymentStatusDropDownList
        private dynamic GetPaymentStatusDropDownList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            return list;
        }
        #endregion
        #endregion

        #region AJAX Methods
        #endregion
    }
}
