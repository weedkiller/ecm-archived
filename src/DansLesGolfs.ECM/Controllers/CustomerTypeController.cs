using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI.Fluent;

namespace DansLesGolfs.ECM.Controllers
{
    public class CustomerTypeController : BaseECMCRUDController<CustomerTypeViewModel>
    {
        #region Constructor
        public CustomerTypeController()
        {
            ObjectName = "CustomerType";
            TitleName = Resources.Resources.CustomerType;
            PrimaryKey = "CustomerTypeId";

            // Define Column Names.
            ColumnNames.Add("CustomerTypeName", Resources.Resources.Name);
            ColumnNames.Add("ParentCustomerTypeName", Resources.Resources.Parent);

            AddAllowedUserType(UserType.Type.Admin);
            AddAllowedUserType(UserType.Type.Staff);
        }
        #endregion

        #region Overriden Methods
        protected override void DoSetGridColumns(GridColumnFactory<CustomerTypeViewModel> columns)
        {
            columns.Bound(c => c.CustomerTypeName).Title(Resources.Resources.Name);
            columns.Bound(c => c.ParentCustomerTypeName).Title(Resources.Resources.Parent);
        }

        protected override IQueryable<CustomerTypeViewModel> DoLoadDataJSON()
        {
            if (Auth.User.UserTypeId == UserType.Type.SiteManager || Auth.User.UserTypeId == UserType.Type.Staff)
            {
                return DataAccess.GetListCustomerTypes(1, Auth.User.SiteId);
            }
            else
            {
                return DataAccess.GetListCustomerTypes(1);
            }
        }

        protected override object DoPrepareNew()
        {
            return new CustomerType
            {
                CustomerTypeId = 0,
                Active = false,
                CustomerTypeName = string.Empty,
                ParentId = 0
            };
        }

        protected override object DoPrepareEdit(long id)
        {
            CustomerType model = DataAccess.GetCustomerTypeById(id);
            return model;
        }

        protected override void DoPrepareForm(int? id = null)
        {
            long? siteId = null;
            if (Auth.User.UserTypeId == UserType.Type.SiteManager || Auth.User.UserTypeId == UserType.Type.Staff)
            {
                siteId = Auth.User.SiteId;
            }
            else if (ViewBag.SiteId != null)
            {
                siteId = DataManager.ToLong(ViewBag.SiteId);
            }

            List<Site> sites = DataAccess.GetSitesDropDownListData();
            List<SelectListItem> siteList = ListToDropDownList<Site>(sites, "SiteId", "SiteName");
            siteList.Insert(0, new SelectListItem()
            {
                Text = Resources.Resources.SelectSite,
                Value = "0",
                Selected = true
            });
            ViewBag.Sites = siteList;

            List<CustomerType> nestedCustomerTypes = DataAccess.GetAllNestedCustomerTypes(1, siteId, id);
            List<SelectListItem> customerTypesList = ListToDropDownList<CustomerType>(nestedCustomerTypes, "CustomerTypeId", "CustomerTypeName");
            customerTypesList.Insert(0, new SelectListItem()
            {
                Text = Resources.Resources.None,
                Value = "0",
                Selected = true
            });
            ViewBag.DropDownListCustomerTypes = customerTypesList;
        }

        protected override bool DoSave()
        {
            int result = -1;
            long id = DataManager.ToLong(Request.Form["id"], -1);
            CustomerType model = null;
            if (id > 0)
            {
                model = DataAccess.GetCustomerTypeById(id);
                if (model == null)
                {
                    model = new CustomerType();
                }
            }
            else
            {
                model = new CustomerType();
            }
            model.CustomerTypeId = id;
            model.AffiliationTypeId = DataManager.ToInt(Request.Form["AffiliationTypeId"]);
            model.ParentId = DataManager.ToLong(Request.Form["ParentId"]);

            if (Auth.User.UserTypeId == UserType.Type.SuperAdmin || Auth.User.UserTypeId == UserType.Type.Admin)
            {
                model.SiteId = DataManager.ToLong(Request.Form["SiteId"]);
            }
            else
            {
                model.SiteId = Auth.User.SiteId;
            }

            if (id > 0)
            {
                model.UpdatedDate = DateTime.Now;
                result = DataAccess.UpdateCustomerType(model);
            }
            else
            {
                model.Active = true;
                model.UpdatedDate = model.CreatedDate = DateTime.Now;
                result = DataAccess.AddCustomerType(model);
                model.CustomerTypeId = result;
            }
            ViewBag.id = result > -1 ? model.CustomerTypeId : -1;
            return result > 0;
        }

        protected override void DoSaveSuccess(int id)
        {
            string[] customerTypeNames = Request.Form.GetValues("CustomerTypeName");

            CustomerTypeLang customerTypeLang = null;

            if (customerTypeNames != null && customerTypeNames.Any())
            {
                customerTypeLang = new CustomerTypeLang();
                customerTypeLang.CustomerTypeId = id;
                customerTypeLang.LangId = 1;
                customerTypeLang.CustomerTypeName = customerTypeNames[0].Trim();

                DataAccess.SaveCustomerTypeLang(customerTypeLang);
            }
        }

        protected override bool DoDelete(long id)
        {
            return DataAccess.DeleteCustomerType(id) > 0;
        }
        #endregion

        #region AJAX Actions

        public ActionResult AjaxGetAffiliationTypes(long id, long siteId)
        {
            if (id <= 0)
                return Json(new
                {
                    success = false,
                    message = "Please specify customer type id."
                }, JsonRequestBehavior.AllowGet);

            if (siteId <= 0)
                return Json(new
                {
                    success = false,
                    message = "Please specify siteId."
                }, JsonRequestBehavior.AllowGet);

            int clubId = DataAccess.GetSiteChronogolfClubId(siteId);
            if (clubId <= 0)
                return Json(new
                {
                    success = true,
                    data = new String[0]
                }, JsonRequestBehavior.AllowGet);

            var affiliationTypeId = DataAccess.GetAffiliationTypeIdByCustomerTypeId(id);

            var chronogolf = Data.DataFactory.GetChronogolfInstance(clubId);
            var affiliationTypes = chronogolf.GetAffiliateTypes();

            var data = new List<Object>();
            foreach (var a in affiliationTypes)
            {
                var obj = new
                {
                    id = a.id,
                    name = a.name,
                    selected = affiliationTypeId == a.id
                };
                data.Add(obj);
            }

            return Json(new
            {
                success = true,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetParentCustomerTypes(long? siteId, long? skipId)
        {
            try
            {
                var list = DataAccess.GetCustomerTypesDropDownList(1, null, siteId, skipId);
                list.Insert(0, new CustomerType() { CustomerTypeId = 0, CustomerTypeName = Resources.Resources.None });
                string html = string.Empty;
                foreach (var it in list)
                {
                    html += "<option value=\"" + it.CustomerTypeId + "\">" + it.CustomerTypeName + "</option>";
                }
                return Json(new
                {
                    isSuccess = true,
                    html = html
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
