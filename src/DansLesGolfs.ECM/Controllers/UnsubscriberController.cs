using DansLesGolfs.Base;
using DansLesGolfs.Base.Services;
using DansLesGolfs.BLL;
using DansLesGolfs.ECM.Controllers;
using Kendo.Mvc.UI.Fluent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.ECM.Controllers
{
    public class UnsubscriberController : BaseECMCRUDController<UserViewModel>
    {
        #region Constructor
        public UnsubscriberController()
        {
            ObjectName = "Unsubscriber";
            TitleName = Resources.Resources.Unsubscribers;
            PrimaryKey = "UserId";

            // Define Column Names.
            ColumnNames.Add("Email", Resources.Resources.Email);
            ColumnNames.Add("Firstname", Resources.Resources.FirstName);
            ColumnNames.Add("Lastname", Resources.Resources.LastName);
            if (Auth.User.UserTypeId == UserType.Type.SuperAdmin || Auth.User.UserTypeId == UserType.Type.Admin)
            {
                ColumnNames.Add("SiteName", Resources.Resources.Site);
            }
        }
        #endregion

        #region Overriden Methods
        protected override void DoSetGridColumns(GridColumnFactory<UserViewModel> columns)
        {
            columns.Bound(it => it.Email).Title(Resources.Resources.Email);
            columns.Bound(it => it.FirstName).Title(Resources.Resources.FirstName);
            columns.Bound(it => it.LastName).Title(Resources.Resources.LastName);
            columns.Bound(it => it.InsertDate).Title(Resources.Resources.Created).Format("{0:dd/MM/yyyy}");
            if (Auth.User.UserTypeId == UserType.Type.SuperAdmin || Auth.User.UserTypeId == UserType.Type.Admin)
            {
                columns.Bound(it => it.SiteName).Title(Resources.Resources.Site);
            }
        }

        protected override void DoSetDataSorting(DataSourceSortDescriptorFactory<UserViewModel> sorting)
        {
            sorting.Add(it => it.Email);
            sorting.Add(it => it.FirstName);
            sorting.Add(it => it.LastName);
        }
        protected override IQueryable<UserViewModel> DoLoadDataJSON()
        {
            if (Auth.User.UserTypeId == UserType.Type.SiteManager || Auth.User.UserTypeId == UserType.Type.Staff)
            {
                return DataAccess.GetListECMUnsubscribers(Auth.User.SiteId, 1);
            }
            else
            {
                return DataAccess.GetListECMUnsubscribers(null, 1);
            }
        }

        protected override void DoPrepareForm(int? id)
        {
            List<Site> sites = DataAccess.GetSitesDropDownListData();
            List<SelectListItem> siteList = ListToDropDownList<Site>(sites, "SiteId", "SiteName");
            siteList.Insert(0, new SelectListItem()
            {
                Text = Resources.Resources.SelectSite,
                Value = "0",
                Selected = true
            });
            ViewBag.Sites = siteList;

            List<Country> countries = DataAccess.GetAllCountries();
            ViewBag.Countries = ListToDropDownList<Country>(countries, "CountryId", "CountryName");

            List<Title> titles = DataAccess.GetItemTitlesDropDownList(1);
            ViewBag.Titles = ListToDropDownList<Title>(titles, "TitleId", "TitleName");

            #region Load Lists
            List<CustomerGroup> customerGroups = null;
            List<CustomerType> customerTypes = null;
            List<CustomerType> subCustomerTypes = null;
            if (Auth.User.UserTypeId == UserType.Type.SiteManager || Auth.User.UserTypeId == UserType.Type.Staff)
            {
                customerGroups = DataAccess.GetAllCustomerGroups(Auth.User.SiteId);
                customerTypes = DataAccess.GetAllCustomerTypes(1, Auth.User.SiteId);
                subCustomerTypes = DataAccess.GetAllCustomerTypes(1, Auth.User.SiteId);
            }
            else
            {
                customerGroups = DataAccess.GetAllCustomerGroups();
                customerTypes = DataAccess.GetAllCustomerTypes(1);
                subCustomerTypes = DataAccess.GetAllCustomerTypes(1);
            }
            #endregion

            #region Prepare Drop Down List
            ViewBag.CustomerGroupsList = customerGroups;

            List<SelectListItem> customerTypeList = ListToDropDownList<CustomerType>(customerTypes, "CustomerTypeId", "CustomerTypeName");
            customerTypeList.Add(new SelectListItem()
            {
                Selected = true,
                Text = Resources.Resources.CustomerType,
                Value = "0"
            });
            ViewBag.CustomerTypes = customerTypeList;

            List<SelectListItem> subCustomerTypeList = ListToDropDownList<CustomerType>(subCustomerTypes, "CustomerTypeId", "CustomerTypeName");
            subCustomerTypeList.Add(new SelectListItem()
            {
                Selected = true,
                Text = Resources.Resources.SubCustomerType,
                Value = "0"
            });
            ViewBag.SubCustomerTypes = subCustomerTypeList;
            #endregion
        }
        protected override object DoPrepareNew()
        {
            return new User();
        }

        protected override object DoPrepareEdit(long id)
        {
            User model = DataAccess.GetUser(id);
            ViewBag.SelectedCustomerGroups = DataAccess.GetCustomerGroupsByCustomerId(id);
            return model;
        }

        protected override bool DoSave()
        {
            User model = null;
            int id = DataManager.ToInt(Request.Form["id"]);
            int result = -1;
            if (id > 0)
            {
                model = DataAccess.GetUser(id);
            }
            else
            {
                model = new User();
            }
            model.UserTypeId = (int)UserType.Type.Customer;
            model.TitleId = DataManager.ToInt(Request.Form["TitleId"]);
            model.Email = DataManager.ToString(Request.Form["Email"]);
            model.FirstName = DataManager.ToString(Request.Form["Firstname"]);
            model.MiddleName = DataManager.ToString(Request.Form["Middlename"]);
            model.LastName = DataManager.ToString(Request.Form["Lastname"]);
            model.Gender = DataManager.ToInt(Request.Form["Gender"]);
            model.Birthdate = DataManager.ToDateTime(Request.Form["Birthdate"], "dd/MM/yyyy", DateTime.Today);
            model.Remarks = DataManager.ToString(Request.Form["Remarks"]);
            model.Career = DataManager.ToString(Request.Form["Career"]);
            model.LicenseNumber = DataManager.ToString(Request.Form["LicenseNumber"]);
            model.Index = DataManager.ToFloat(Request.Form["Index"]);
            model.CustomField1 = DataManager.ToString(Request.Form["CustomField1"]);
            model.CustomField2 = DataManager.ToString(Request.Form["CustomField2"]);
            model.CustomField3 = DataManager.ToString(Request.Form["CustomField3"]);

            model.CustomerTypeId = DataManager.ToLong(Request.Form["CustomerTypeId"]);
            model.SubCustomerTypeId = DataManager.ToLong(Request.Form["CustomerTypeId"]);

            model.Address = DataManager.ToString(Request.Form["Address"]);
            model.Street = DataManager.ToString(Request.Form["Street"]);
            model.City = DataManager.ToString(Request.Form["City"]);
            model.PostalCode = DataManager.ToString(Request.Form["PostalCode"]);
            model.Phone = DataManager.ToString(Request.Form["Phone"]);
            model.PhoneCountryCode = DataManager.ToString(Request.Form["PhoneCountryCode"]).Trim();
            model.MobilePhone = DataManager.ToString(Request.Form["MobilePhone"]).Trim();
            model.MobilePhoneCountryCode = DataManager.ToString(Request.Form["MobilePhoneCountryCode"]).Trim();
            model.CountryId = DataManager.ToInt(Request.Form["CountryId"]);

            model.IsSubscriber = DataManager.ToBoolean(Request.Form["IsSubscriber"]);
            model.IsReceiveEmailInfo = DataManager.ToBoolean(Request.Form["IsSubscriber"]);

            if (Auth.User.UserTypeId == UserType.Type.SuperAdmin || Auth.User.UserTypeId == UserType.Type.Admin)
            {
                model.SiteId = DataManager.ToInt(Request.Form["SiteId"]);
            }
            else
            {
                model.SiteId = Auth.User.SiteId;
            }

            if (Auth.User != null)
                model.ModifyUserId = Auth.User.UserId;

            model.UpdateDate = DateTime.Now;

            if (id > 0)
            {
                result = DataAccess.UpdateUser(model);
            }
            else
            {
                model.InsertDate = model.UpdateDate;
                model.RegisteredDate = model.UpdateDate;
                model.LastLoggedOn = model.UpdateDate;
                model.ExpiredDate = model.UpdateDate.Value.AddYears(100);
                model.Active = true;
                result = DataAccess.AddUser(model);
                model.UserId = result;
            }
            ViewBag.id = result > -1 ? model.UserId : -1;
            return result > 0;
        }

        protected override void DoSaveSuccess(int id)
        {
            string[] customerGroups = Request.Form.GetValues("CustomerGroups");
            DataAccess.DeleteCustomerGroupCustomerByCustomerId(id);
            if (customerGroups != null && customerGroups.Any())
            {
                foreach (var cg in customerGroups)
                {
                    DataAccess.AddCustomerGroupCustomer(DataManager.ToInt(cg), id);
                }
            }
        }

        protected override bool DoDelete(long id)
        {
            return DataAccess.DeleteUser(id) > 0;
        }
        #endregion
    }
}