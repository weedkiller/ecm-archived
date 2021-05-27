using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.ECM.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI.Fluent;

namespace DansLesGolfs.ECM.Areas.Admin.Controllers
{
    public class SiteController : BaseECMCRUDController<SiteViewModel>
    {
        #region Fields
        private bool isNew = false;
        #endregion

        #region Constructor
        public SiteController()
        {
            ObjectName = "Site";
            TitleName = "Site";
            PrimaryKey = "SiteId";

            // Define Column Names.
            ColumnNames.Add("SiteName", "Site Name");
        }
        #endregion

        #region Overriden Methods
        protected override void DoSetGridColumns(GridColumnFactory<SiteViewModel> columns)
        {
            columns.Bound(c => c.SiteName).Title(Resources.Resources.SiteName);
        }

        protected override void DoSetDataSorting(DataSourceSortDescriptorFactory<SiteViewModel> sorting)
        {
            sorting.Add(it => it.SiteName);
        }
        protected override IQueryable<SiteViewModel> DoLoadDataJSON()
        {
            return DataAccess.GetListSites(1);
        }

        protected override void DoPrepareForm(int? id)
        {
            ViewBag.DefaultLatitude = DataManager.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["DefaultLatitude"], 47);
            ViewBag.DefaultLongitude = DataManager.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["DefaultLongitude"], 2);

            #region Brands
            List<Brand> brands = DataAccess.GetBrandDropDownList();
            List<SelectListItem> brandList = new List<SelectListItem>();
            brandList.Add(new SelectListItem()
            {
                Text = Resources.Resources.SelectBrand,
                Value = "0"
            });
            foreach (Brand brand in brands)
            {
                brandList.Add(new SelectListItem()
                {
                    Text = brand.BrandName,
                    Value = brand.BrandId.ToString()
                });
            }
            ViewBag.Brands = brandList;
            #endregion

            #region Email Templates
            ViewBag.EmailTemplates = DataAccess.GetSelectableEmailTemplates(id, 1);
            #endregion

            List<SelectListItem> sendMailUsingOptions = new List<SelectListItem>();
            sendMailUsingOptions.Add(new SelectListItem()
            {
                Text = Resources.Resources.None,
                Value = ((int)SendMailUsing.None).ToString()
            });
            sendMailUsingOptions.Add(new SelectListItem()
            {
                Text = "SMTP Server",
                Value = ((int)SendMailUsing.SMTP).ToString()
            });
            sendMailUsingOptions.Add(new SelectListItem()
            {
                Text = "Netmessage",
                Value = ((int)SendMailUsing.Netmessage).ToString()
            });
            ViewBag.SendMailUsingOptions = sendMailUsingOptions;
        }

        protected override object DoPrepareNew()
        {
            isNew = true;
            Site site = new Site()
            {
                SMTPPort = 25,
                IsUseGlobalSMTPSettings = true,
                IsUseGlobalNetmessageSettings = true
            };
            LoadDropDownLists(site);
            return site;
        }

        protected override object DoPrepareEdit(long id)
        {
            isNew = false;
            Site site = DataAccess.GetSiteById(id, 1);
            LoadDropDownLists(site);
            return site;
        }

        protected override bool DoSave()
        {
            int result = -1;
            int id = DataManager.ToInt(Request.Form["id"]);
            Site model = null;
            if (id > 0)
            {
                model = DataAccess.GetSite(id);
                if (model == null)
                {
                    model = new Site();
                }
            }
            else
            {
                model = new Site();
                isNew = true;
            }
            model.SiteId = id;
            model.SiteSlug = DataManager.ToString(Request.Form["SiteSlug"]).Trim();
            model.GolfBrandId = DataManager.ToInt(Request.Form["GolfBrandId"]);
            //model.Address = DataManager.ToString(Request.Form["Address"]).Trim();
            //model.Street = DataManager.ToString(Request.Form["Street"]).Trim();
            model.City = DataManager.ToString(Request.Form["City"]).Trim();
            model.PostalCode = DataManager.ToString(Request.Form["PostalCode"]).Trim();
            model.CountryId = DataManager.ToInt(Request.Form["CountryId"]);
            model.StateId = DataManager.ToInt(Request.Form["StateId"]);
            model.RegionId = DataManager.ToInt(Request.Form["RegionId"]);
            model.Phone = DataManager.ToString(Request.Form["Phone"]).Trim();
            model.Fax = DataManager.ToString(Request.Form["Fax"]).Trim();
            model.Website = DataManager.ToString(Request.Form["Website"]).Trim();
            model.Email = DataManager.ToString(Request.Form["Email"]).Trim();
            model.FB = DataManager.ToString(Request.Form["FB"]).Trim();
            model.Latitude = DataManager.ToDecimal(Request.Form["Latitude"]);
            model.Longitude = DataManager.ToDecimal(Request.Form["Longitude"]);

            // Email Settings.
            model.IsUseGlobalSMTPSettings = DataManager.ToBoolean(Request.Form["IsUseGlobalSMTPSettings"]);
            model.SMTPServer = DataManager.ToString(Request.Form["SMTPServer"]).Trim();
            model.SMTPUsername = DataManager.ToString(Request.Form["SMTPUsername"]).Trim();
            model.SMTPPassword = DataManager.ToString(Request.Form["SMTPPassword"]).Trim();
            model.SMTPPort = DataManager.ToInt(Request.Form["SMTPPort"]);
            model.SMTPUseSSL = DataManager.ToBoolean(Request.Form["SMTPUseSSL"]);
            model.SMTPUseVERP = DataManager.ToBoolean(Request.Form["SMTPUseVERP"]);
            model.BouncedReturnEmail = DataManager.ToString(Request.Form["BouncedReturnEmail"]).Trim();
            model.DefaultSenderName = DataManager.ToString(Request.Form["DefaultSenderName"]).Trim();
            model.DefaultSenderEmail = DataManager.ToString(Request.Form["DefaultSenderEmail"]).Trim();
            model.IsTrackingOpenMail = DataManager.ToBoolean(Request.Form["IsTrackingOpenMail"]);
            model.IsTrackingLinkClick = DataManager.ToBoolean(Request.Form["IsTrackingLinkClick"]);

            // Netmessage Settings.
            model.IsUseGlobalNetmessageSettings = DataManager.ToBoolean(Request.Form["IsUseGlobalNetmessageSettings"]);
            model.NetmessageFTPUsername = DataManager.ToString(Request.Form["NetmessageFTPUsername"]).Trim();
            model.NetmessageFTPPassword = DataManager.ToString(Request.Form["NetmessageFTPPassword"]).Trim();
            model.NetmessageAccountName = DataManager.ToString(Request.Form["NetmessageAccountName"]).Trim();

            //model.AlbatrosCourseId = DataManager.ToInt(Request.Form["AlbatrosCourseId"]);

            // Chronogolf API Settings.
            model.ChronogolfClubId = DataManager.ToInt(Request.Form["ChronogolfClubId"]);

            model.UpdateDate = DateTime.Now;
            if (Auth.User != null)
                model.UserId = Auth.User.UserId;

            if (id > 0)
            {
                model.InsertDate = model.InsertDate == DateTime.MinValue ? model.UpdateDate : model.InsertDate;
                result = DataAccess.UpdateSite(model);
            }
            else
            {
                model.InsertDate = model.UpdateDate;
                model.Active = true;
                result = DataAccess.AddSite(model);
                model.SiteId = result;
            }
            ViewBag.id = result > -1 ? model.SiteId : -1;
            return result > 0;
        }

        protected override void DoSaveSuccess(int id)
        {
            string[] siteNames = Request.Form.GetValues("SiteName");
            string[] descriptions = Request.Form.GetValues("Description");
            string[] siteEmailTemplateIds = Request.Form.GetValues("SiteEmailTemplateIds");
            SiteLang siteLang = null;

            if (siteNames != null && siteNames.Any())
            {
                siteLang = new SiteLang();
                siteLang.SiteId = id;
                siteLang.LangId = 1;
                siteLang.SiteName = siteNames[0];
                siteLang.Description = descriptions[0];

                DataAccess.SaveSiteLang(siteLang);
            }

            DataAccess.DeleteAllSiteEmailTemplatesBySiteId(id);
            if(siteEmailTemplateIds != null && siteEmailTemplateIds.Any())
            {
                foreach (string s in siteEmailTemplateIds)
                {
                    DataAccess.SaveSiteEmailTemplate(id, DataManager.ToLong(s)); 
                }
            }
        }

        protected override bool DoDelete(long id)
        {
            return DataAccess.DeleteSite(id) > 0;
        }
        #endregion

        #region AJAX Methods
        [HttpPost]
        public ActionResult GenerateSiteSlug(string text, long? skipId)
        {
            try
            {
                string slug = GetSiteSlug(text, skipId);
                return Json(new
                {
                    isSuccess = true,
                    slug = slug
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

        private string GetSiteSlug(string text, long? itemId)
        {
            int num = 1;
            string slug = WebHelper.GenerateSlug(text);

            while (DataAccess.IsExistsSiteSlug(slug, itemId))
            {
                slug = WebHelper.GenerateSlug(text);
                slug = slug + (num++).ToString();
            }
            return slug;
        }
        #endregion

        #region Private Methods

        #region LoadDropDownLists
        private void LoadDropDownLists(Site site)
        {
            if (site == null)
                return;

            List<Country> countries = DataAccess.GetAllCountries();
            List<SelectListItem> listCountries = ListToDropDownList<Country>(countries, "CountryId", "CountryName", site.CountryId);
            ViewBag.Countries = listCountries;

            List<DansLesGolfs.BLL.Region> regions = DataAccess.GetAllRegionsByCountryId(site.CountryId > 0 ? site.CountryId : (countries != null && countries.Any() ? countries.First().CountryId : 0));
            List<SelectListItem> listRegions = ListToDropDownList(regions, "RegionId", "RegionName", site.RegionId);
            listRegions.Insert(0, new SelectListItem()
            {
                Value = "",
                Text = Resources.Resources.SelectRegion
            });
            ViewBag.Regions = listRegions;

            List<State> states = DataAccess.GetStatesListByRegionId(site.RegionId);
            List<SelectListItem> listStates = ListToDropDownList(states, "StateId", "StateName", site.StateId);
            listStates.Insert(0, new SelectListItem()
            {
                Value = "",
                Text = Resources.Resources.SelectState
            });
            ViewBag.States = listStates;
        }
        #endregion

        #endregion
    }
}
