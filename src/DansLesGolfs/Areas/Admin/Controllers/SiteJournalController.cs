using DansLesGolfs.Albatros;
using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Admin.Controllers
{
    public class SiteJournalController : BaseAdminController
    {
        #region Fields
        private bool isNew = false;
        #endregion

        #region Constructor
        public SiteJournalController()
        {
            ViewBag.ClassName = "site";
        }
        #endregion

        #region Action Methods
        public ActionResult Yearly(long? id, int? fiscalYear)
        {
            // Generate Payment Type DropDownList Items
            InitPaymentTypeDropDownList();

            // Init Site's Drop Down List
            InitSiteDropDownList();

            // Init Restaurant Supplier's Drop Down List
            InitRestaurantSupplierDropDownList();

            // Init Restaurant Product Categor's Drop Down List
            InitRestaurantProductCategoryDropDownList();

            if (!id.HasValue || id.Value <= 0)
            {
                List<SelectListItem> sites = (List<SelectListItem>)ViewBag.Sites;
                if(sites.Any())
                {
                    id = DataManager.ToLong(sites.FirstOrDefault().Value);
                }
                else
                {
                    return RedirectToAction("Index", "Site");
                }
            }

            DateTime today = DateTime.Today;

            if(!fiscalYear.HasValue)
                fiscalYear = today.Year;

            ViewBag.id = id.Value;
            ViewBag.SiteId = id.Value;
            ViewBag.Site = DataAccess.GetSite(id.Value);
            ViewBag.FiscalYear = fiscalYear;
            ViewBag.SiteEvent = DataAccess.GetSiteEvent(id.Value, fiscalYear.Value);
            ViewBag.SiteFinancial = DataAccess.GetSiteFinancial(id.Value, fiscalYear.Value);
            ViewBag.SiteCommunication = DataAccess.GetSiteCommunication(id.Value, fiscalYear.Value);
            ViewBag.SiteCentralLineSEO = DataAccess.GetSiteCentralLineSEO(id.Value, fiscalYear.Value);
            ViewBag.SiteRestaurantSupplierIds = DataAccess.GetSiteRestaurantSupplierIds(id.Value, fiscalYear.Value);
            ViewBag.SiteRestaurantProductCategoryIds = DataAccess.GetSiteRestaurantProductCategoryIds(id.Value, fiscalYear.Value);

            // Make breadcrumbs.
            Breadcrumbs.Add(Resources.Resources.Site, "~/Admin/Site/Form/" + id.Value);
            Breadcrumbs.Add(Resources.Resources.YearlyJournal, "~/Admin/SiteJournal/Yearly/" + id.Value);
            InitBreadcrumbs();

            ViewBag.ClassName = "sitejournalyearly";

            return View();
        }

        public ActionResult Daily(long? id, string visitDate)
        {
            // Init Site's Drop Down List
            InitSiteDropDownList();

            if (!id.HasValue || id.Value <= 0)
            {
                List<SelectListItem> sites = (List<SelectListItem>)ViewBag.Sites;
                if (sites.Any())
                {
                    id = DataManager.ToLong(sites.FirstOrDefault().Value);
                }
                else
                {
                    return RedirectToAction("Index", "Site");
                }
            }

            DateTime today = DateTime.Today;
            DateTime vDate = today;

            if (!String.IsNullOrWhiteSpace(visitDate))
            {
                vDate = DataManager.ToDateTime(visitDate, "dd/MM/yyyy");
            }

            ViewBag.SiteId = id.Value;
            ViewBag.Site = DataAccess.GetSite(id.Value);
            ViewBag.VisitDate = vDate;
            ViewBag.SiteCommercialFollowUp = DataAccess.GetSiteCommercialFollowUp(id.Value, vDate);

            // Make breadcrumbs.
            Breadcrumbs.Add(Resources.Resources.Site, "~/Admin/Site/Form/" + id.Value);
            Breadcrumbs.Add(Resources.Resources.DailyJournal, "~/Admin/SiteJournal/Daily/" + id.Value);
            InitBreadcrumbs();

            ViewBag.ClassName = "sitejournaldaily";

            return View();
        }

        private void InitSiteDropDownList()
        {
            List<Site> sites = DataAccess.GetSitesDropDownListData();
            ViewBag.Sites = ListToDropDownList<Site>(sites, "SiteId", "SiteName");
        }

        private void InitRestaurantSupplierDropDownList()
        {
            ViewBag.RestaurantSuppliers = DataAccess.GetRestaurantSupplierDropDownListData();
        }

        private void InitRestaurantProductCategoryDropDownList()
        {
            ViewBag.RestaurantProductCategories = DataAccess.GetRestaurantProductCategoryDropDownListData();
        }

        private void InitPaymentTypeDropDownList()
        {
            List<SelectListItem> paymentTypes = new List<SelectListItem>();
            paymentTypes.Add(new SelectListItem()
            {
                Text = Resources.Resources.Sample,
                Value = "0"
            });
            paymentTypes.Add(new SelectListItem()
            {
                Text = Resources.Resources.Check,
                Value = "1"
            });
            paymentTypes.Add(new SelectListItem()
            {
                Text = Resources.Resources.Transfer,
                Value = "2"
            });
            paymentTypes.Add(new SelectListItem()
            {
                Text = Resources.Resources.Other,
                Value = "3"
            });
            ViewBag.PaymentTypesDropDownList = paymentTypes;
        }
        #endregion

        #region Ajax Action Methods
        [HttpPost]

        public JsonResult SaveYearlyData(long? id, int? FiscalYear, string[] SiteRestaurantSuppliers, string[] SiteRestaurantProductCategories)
        {
            try
            {
                if (!FiscalYear.HasValue)
                {
                    FiscalYear = DateTime.Today.Year;
                }

                DateTime now = DateTime.Now;

                SiteFinancial financial = DataAccess.GetSiteFinancial(id.Value, FiscalYear.Value);
                SiteEvent ev = DataAccess.GetSiteEvent(id.Value, FiscalYear.Value);
                SiteCommunication communication = DataAccess.GetSiteCommunication(id.Value, FiscalYear.Value);
                SiteCentralLineSEO central = DataAccess.GetSiteCentralLineSEO(id.Value, FiscalYear.Value);

                var form = Request.Form;

                financial.SiteId = id.Value;
                financial.FiscalYear = FiscalYear.Value;
                financial.MemberAmount = DataManager.ToString(form["MemberAmount"]);
                financial.PaymentType = DataManager.ToInt(form["PaymentType"]);
                financial.PaymentDate = DataManager.ToDateTime(form["PaymentDate"], "dd/MM/yyyy");
                financial.ClassicCard = DataManager.ToString(form["ClassicCard"]);
                financial.ClassicCardTurnover = DataManager.ToString(form["ClassicCardTurnover"]);
                financial.GolfCardTurnover = DataManager.ToString(form["GolfCardTurnover"]);
                financial.ClassicCorpoTurnover = DataManager.ToString(form["ClassicCorpoTurnover"]);
                financial.RFAPractice = DataManager.ToString(form["RFAPractice"]);
                financial.RFAProshop = DataManager.ToString(form["RFAProshop"]);
                financial.RFAField = DataManager.ToString(form["RFAField"]);
                financial.RFAServices = DataManager.ToString(form["RFAServices"]);
                financial.RFACarts = DataManager.ToString(form["RFACarts"]);
                financial.RFARestauration = DataManager.ToString(form["RFARestauration"]);
                financial.RFATotal = DataManager.ToString(form["RFATotal"]);
                financial.AdsPackVisibility = DataManager.ToString(form["AdsPackVisibility"]);
                financial.AdsAdditionalServices = DataManager.ToString(form["AdsAdditionalServices"]);
                financial.AdsShow = DataManager.ToString(form["AdsShow"]);
                financial.CommisionGFOnline = DataManager.ToString(form["CommisionGFOnline"]);
                financial.CreatedBy = Auth.User.UserId;
                financial.CreatedDate = now;
                financial.UpdatedBy = Auth.User.UserId;
                financial.UpdatedDate = now;
                financial.Active = true;

                ev.SiteId = id.Value;
                ev.FiscalYear = FiscalYear.Value;
                ev.HasChallenge = DataManager.ToBoolean(form["HasChallenge"]);
                ev.ChallengeDate = DataManager.ToDateTime(form["ChallengeDate"], "dd/MM/yyyy");
                ev.HasPingClassicTour = DataManager.ToBoolean(form["HasPingClassicTour"]);
                ev.PingClassicTourDate = DataManager.ToDateTime(form["PingClassicTourDate"], "dd/MM/yyyy");
                ev.HasCongressOwner = DataManager.ToBoolean(form["HasCongressOwner"]);
                ev.HasCongressManager = DataManager.ToBoolean(form["HasCongressManager"]);
                ev.HasCongressGreenKeeper = DataManager.ToBoolean(form["HasCongressGreenKeeper"]);
                ev.HasCongressRestaurateur = DataManager.ToBoolean(form["HasCongressRestaurateur"]);
                ev.HasCongressRespProshop = DataManager.ToBoolean(form["HasCongressRespProshop"]);
                ev.HasShowroomManager = DataManager.ToBoolean(form["HasShowroomManager"]);
                ev.HasShowroomRespProshop = DataManager.ToBoolean(form["HasShowroomRespProshop"]);
                ev.CreatedBy = Auth.User.UserId;
                ev.CreatedDate = now;
                ev.UpdatedBy = Auth.User.UserId;
                ev.UpdatedDate = now;
                ev.Active = true;

                communication.SiteId = id.Value;
                communication.FiscalYear = FiscalYear.Value;
                communication.NewslettersOrder = DataManager.ToString(form["NewslettersOrder"]);
                communication.NewslettersSendDate = DataManager.ToDateTime(form["NewslettersSendDate"], "dd/MM/yyyy");
                communication.NewslettersSendDetail = DataManager.ToString(form["NewslettersSendDetail"]);
                communication.UsePageGuide = DataManager.ToBoolean(form["UsePageGuide"]);
                communication.UsePageSite = DataManager.ToBoolean(form["UsePageSite"]);
                communication.UseStaysOffers = DataManager.ToBoolean(form["UseStaysOffers"]);
                communication.UseTerminal = DataManager.ToBoolean(form["UseTerminal"]);
                communication.UseShowBooklet = DataManager.ToBoolean(form["UseShowBooklet"]);
                communication.UseLogoOnSite = DataManager.ToBoolean(form["UseLogoOnSite"]);
                communication.UseTotem = DataManager.ToBoolean(form["UseTotem"]);
                communication.UseGuideFlyers = DataManager.ToBoolean(form["UseGuideFlyers"]);
                communication.UseSheet = DataManager.ToBoolean(form["UseSheet"]);
                communication.CreatedBy = Auth.User.UserId;
                communication.CreatedDate = now;
                communication.UpdatedBy = Auth.User.UserId;
                communication.UpdatedDate = now;
                communication.Active = true;

                central.SiteId = id.Value;
                central.FiscalYear = FiscalYear.Value;
                central.InvoiceEntryField = DataManager.ToBoolean(form["InvoiceEntryField"]);
                central.InvoiceEntryRestaurant = DataManager.ToBoolean(form["InvoiceEntryRestaurant"]);
                central.InvoiceEntryOther = DataManager.ToBoolean(form["InvoiceEntryOther"]);
                central.InformationRequestDate = DataManager.ToDateTime(form["InformationRequestDate"], "dd/MM/yyyy");
                central.BeingCalledByProductResp = DataManager.ToString(form["BeingCalledByProductResp"]);
                central.InformationRequestOther = DataManager.ToString(form["InformationRequestOther"]);
                central.CreatedBy = Auth.User.UserId;
                central.CreatedDate = now;
                central.UpdatedBy = Auth.User.UserId;
                central.UpdatedDate = now;
                central.Active = true;

                DataAccess.SaveSiteFinancial(financial);
                DataAccess.SaveSiteEvent(ev);
                DataAccess.SaveSiteCommunication(communication);
                DataAccess.SaveSiteCentralLineSEO(central);

                DataAccess.DeleteAllSiteRestaurantSuppliers(id.Value, FiscalYear.Value);
                if (SiteRestaurantSuppliers != null && SiteRestaurantSuppliers.Any())
                {
                    foreach (var it in SiteRestaurantSuppliers)
                    {
                        DataAccess.SaveSiteRestaurantSupplier(id.Value, DataManager.ToLong(it), FiscalYear.Value);
                    }
                }

                DataAccess.DeleteAllSiteRestaurantProductCategories(id.Value, FiscalYear.Value);
                if (SiteRestaurantProductCategories != null && SiteRestaurantProductCategories.Any())
                {
                    foreach (var it in SiteRestaurantProductCategories)
                    {
                        DataAccess.SaveSiteRestaurantProductCategory(id.Value, DataManager.ToLong(it), FiscalYear.Value);
                    }
                }

                return Json(new
                {
                    isSuccess = true,
                    message = Resources.Resources.DataHasBeenSaved
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
        [HttpPost]

        public JsonResult SaveDailyData(long? SiteId, string visitDate)
        {
            try
            {
                DateTime vDate = DateTime.Today;
                if (!String.IsNullOrWhiteSpace(visitDate))
                {
                    vDate = DataManager.ToDateTime(visitDate, "dd/MM/yyyy");
                }

                DateTime now = DateTime.Now;

                SiteCommercialFollowUp commercial = DataAccess.GetSiteCommercialFollowUp(SiteId.Value, vDate);

                var form = Request.Form;

                commercial.SiteId = SiteId.Value;
                commercial.VisitDate = vDate;
                commercial.ContractualTerms = DataManager.ToString(form["ContractualTerms"]);
                commercial.NeedsInCommunicationTools = DataManager.ToString(form["NeedsInCommunicationTools"]);
                commercial.CardSeller = DataManager.ToString(form["CardSeller"]);
                commercial.HotelPartnerSeeking = DataManager.ToString(form["HotelPartnerSeeking"]);
                commercial.AdsSpotSelling = DataManager.ToString(form["AdsSpotSelling"]);
                commercial.SellGFOnline = DataManager.ToString(form["SellGFOnline"]);
                commercial.CentralPurchasingPoint = DataManager.ToString(form["CentralPurchasingPoint"]);
                commercial.FormationNeeds = DataManager.ToString(form["FormationNeeds"]);
                commercial.ConsultingNeeds = DataManager.ToString(form["ConsultingNeeds"]);
                commercial.FidelityShop = DataManager.ToString(form["FidelityShop"]);
                commercial.RegionalProspectingPoint = DataManager.ToString(form["RegionalProspectingPoint"]);
                commercial.CreatedBy = Auth.User.UserId;
                commercial.CreatedDate = now;
                commercial.UpdatedBy = Auth.User.UserId;
                commercial.UpdatedDate = now;
                commercial.Active = true;
                
                DataAccess.SaveSiteCommercialFollowUp(commercial);

                return Json(new
                {
                    isSuccess = true,
                    message = Resources.Resources.DataHasBeenSaved
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
        #endregion
    }
}
