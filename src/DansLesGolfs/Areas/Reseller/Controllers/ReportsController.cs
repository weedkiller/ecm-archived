using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace DansLesGolfs.Areas.Reseller.Controllers
{
    public partial class ReportsController : BaseResellerCRUDController
    {
        #region Constructor
        public ReportsController()
        {
            ViewBag.ObjectName = Resources.Resources.Reports;
            ViewBag.ClassName = "reports";
        }
        #endregion


        #region Action Methods
        public ActionResult Index()
        {
            ViewBag.Title = Resources.Resources.TeeSheet;
            ViewBag.TitleName = Resources.Resources.TeeSheet;
            Breadcrumbs.Add(Resources.Resources.Reports, "~/Reseller/Report");
            InitBreadcrumbs();

            // Init Drop Down List.
            List<Course> courses = DataAccess.GetAllCourses();
            ViewBag.Courses = ListToDropDownList<Course>(courses, "CourseId", "CourseName");

            return View();
        }

        public ActionResult SalesReport()
        {

            Breadcrumbs.Add(Resources.Resources.Reports, "~/Reseller/Reports");
            InitBreadcrumbs();

            // Init Drop Down List.
            List<Course> courses = DataAccess.GetAllCourses();
            ViewBag.Courses = ListToDropDownList<Course>(courses, "CourseId", "CourseName");

            ViewBag.StartDate = DateTime.Today;
            ViewBag.EndDate = DateTime.Today;

            ViewBag.ClassName = "salesreport";
            var dn = DateTime.Now;
            var datetime = String.Format("{0:yyyy-MM-dd}", dn);
            var model = new SalesReportModel(); // GetsalesReportfunction(datetime, datetime);

            return View("SalesReport", model);
        }

        #region ViewReport
        public ActionResult ViewReport(string startdate, string enddate)
        {
            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;

            var model = DataAccess.GetSalesReport(startdate, enddate, Auth.User.SiteId, null);
            var report = new DansLesGolfs.Areas.Reseller.Views.Reports.SalesReport();
            ViewBag.SalesReport = report;

            return View();
        }

        public ActionResult DocViewPartial(FormCollection form)
        {
            var report = new DansLesGolfs.Areas.Reseller.Views.Reports.SalesReport();
            var startdate = Request.Params["startdate"];
            var enddate = Request.Params["enddate"];

            List<SalesReport> salesReportlist = DataAccess.GetSalesReport(startdate, enddate, null, null);

            report.DataMember = "";
            report.DataSource = salesReportlist;
            ViewBag.SalesReport = report;

            return PartialView("DocViewPartial");
        }


        public ActionResult ExportDocumentViewer()
        {
            var report = new DansLesGolfs.Areas.Reseller.Views.Reports.SalesReport();
            var startdate = Request.Params["startdate"];
            var enddate = Request.Params["enddate"];

            List<SalesReport> salesReportlist = DataAccess.GetSalesReport(startdate, enddate, null, null);

            report.DataMember = "";
            report.DataSource = salesReportlist;

            return DevExpress.Web.Mvc.DocumentViewerExtension.ExportTo(report);
        }

        #endregion

        #region DoLoadDataJSON

        #region SetSortColumn
        public string SetSortColumn(string sortCol, string sortOrder)
        {
            var orderby = "";

            switch (sortCol)
            {
                case "0": orderby = " OrderNumber "; break;
                case "1": orderby = " OrderDate "; break;
                case "2": orderby = " ItemName "; break;
                //case "3": orderby = " CustomerName "; break;
                //case "4": orderby = " UnitPrice "; break;
                //case "5": orderby = " Quantity "; break;
                case "3": orderby = " ExtendedPrice "; break;
                case "4": orderby = " PaymentStatus "; break;
            }

            switch (sortOrder)
            {
                case "asc": orderby += " ASC "; break;
                case "desc": orderby += " DESC "; break;
            }

            return orderby;
        }
        #endregion

        public JsonResult DoLoadDataJSON(
             jQueryDataTableParamModel param, string startDate, string endDate, string status)
        {

            var sales = new List<SalesReport>();

            var echo = DataManager.ToInt(param.draw);
            var search = Request.Params["sSearch"].ToString();
            var displayLength = DataManager.ToInt(Request.Params["iDisplayLength"].ToString());
            var displayStart = DataManager.ToInt(Request.Params["iDisplayStart"].ToString());
            var sortOrder = param.iSortingCols;

            var _sortCol = Request.Params["iSortCol_0"].ToString(CultureInfo.CurrentCulture);
            var _sortOrder = Request.Params["sSortDir_0"].ToString(CultureInfo.CurrentCulture);
            var sqlOrderBy = SetSortColumn(_sortCol, _sortOrder);
            sales = DataAccess.GetSalesReport(startDate, endDate, null, sqlOrderBy);
            var result = sales.Skip(
               displayStart).Take(displayLength);


            #region Filter

            switch (status)
            {
                case "unsuccess":
                    sales = sales.Where(m => m.PaymentStatus == "").ToList();
                    break;
                case "success":
                    sales = sales.Where(m => m.PaymentStatus == "success").ToList();
                    break;
                default:
                    break;
            }
            #endregion

            #region search
            if (!string.IsNullOrEmpty(search) && search.Length > 2)
            {
                sales = sales.Where(m => m.OrderNumber.Contains(search) || m.CustomerName.Contains(search)).ToList();
            }
            #endregion


            var _result = (from o in sales.Skip(
                  displayStart).Take(displayLength)
                           select new[] { 
                                o.OrderNumber ,
                                o.OrderDate.ToString("yyyy/MM/dd HH:mm"),
                                o.CustomerName, 
                                //String.Format("{0:0,0.0}",o.UnitPrice), 
                                //String.Format("{0:0,0.0}",o.Quantity),
                                String.Format("{0:0,0.0}",o.ExtendedPrice) ,
                                o.Active ? "Active" : "Inactive",
                                o.PaymentStatus, o.OrderId.ToString() }).ToList();
            return Json(new
            {
                sEcho = param.draw,
                iTotalRecords = sales.Count(),
                iTotalDisplayRecords = sales.Count(),
                aaData = _result
            },
                JsonRequestBehavior.AllowGet
            );
        }

        #endregion

        #region DoUpdateOrderPaymentStatus
        [HttpPost]
        public ActionResult DoUpdateOrderPaymentStatus(string OrderId, bool IsSuccess)
        {
            var msgError = "";
            var result = false;
            try
            {
                var rs = DataAccess.UpdateOrderPaymentStatus(OrderId, IsSuccess);
                if (rs > 0)
                    result = true;
                else
                    result = false;
            }
            catch (Exception ex)
            {
                result = false;
                msgError = ex.Message;
            }
            return Json(new { OrderId = OrderId, isSuccess = result, msgError = msgError }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region GeneratePDF

        public ActionResult GeneratePDF()
        {
            var model = TempData["SAlesReportTemp"] as SalesReportModel;
            return new Rotativa.PartialViewAsPdf("_SalesTableData", model)
            {
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageSize = Rotativa.Options.Size.A4,

            };
        }
        #endregion

        #endregion

        #region Private Method

        #region GetsalesReportfunction
        private SalesReportModel GetsalesReportfunction(string startdate, string enddate)
        {
            SalesReportModel model = new SalesReportModel();

            List<SalesReport> salesReportlist = DataAccess.GetSalesReport(startdate, enddate, null, null);
            model.SalesReportList = salesReportlist;

            List<int> listOrderId = salesReportlist
                            .Select(m => m.OrderId)
                            .Distinct()
                            .ToList();

            model.ListOrderId = listOrderId;


            TempData["SAlesReportTemp"] = model;


            return model;
        }

        #endregion

        #endregion
    }
}
