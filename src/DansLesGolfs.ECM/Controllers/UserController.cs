using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.ECM.Controllers;
using Kendo.Mvc.UI.Fluent;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.ECM.Controllers
{
    public class UserController : BaseECMCRUDController<UserViewModel>
    {
        #region Constructor
        public UserController()
        {
            ObjectName = "User";
            TitleName = Resources.Resources.User;
            PrimaryKey = "UserId";

            // Define Column Names.
            ColumnNames.Add("Email", Resources.Resources.Email);
            ColumnNames.Add("Firstname", Resources.Resources.FirstName);
            ColumnNames.Add("Lastname", Resources.Resources.LastName);
        }
        #endregion

        #region Overriden Methods
        protected override void DoSetGridColumns(GridColumnFactory<UserViewModel> columns)
        {
            columns.Bound(it => it.Email).Title(Resources.Resources.Email);
            columns.Bound(it => it.FirstName).Title(Resources.Resources.FirstName);
            columns.Bound(it => it.LastName).Title(Resources.Resources.LastName);
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
                return DataAccess.GetListECMUsers(Auth.User.SiteId, 1);
            }
            else
            {
                return DataAccess.GetListECMUsers(null, 1);
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

            InitUserTypeDropDown();
        }

        protected override object DoPrepareNew()
        {
            return new User();
        }

        protected override object DoPrepareEdit(long id)
        {
            User model = DataAccess.GetUser(id);
            return model;
        }

        protected override bool DoSave()
        {

            User model = null;
            int id = DataManager.ToInt(Request.Form["id"]);
            int result = -1;

            if (DataAccess.IsExistsEmailOfECMUser(DataManager.ToString(Request.Form["Email"]), id))
                throw new Exception(Resources.Resources.YourEmailIsAlreadyInUse);

            if (id > 0)
            {
                model = DataAccess.GetUser(id);
            }
            else
            {
                model = new User();
            }

            if (Auth.User.UserTypeId == UserType.Type.SuperAdmin || Auth.User.UserTypeId == UserType.Type.Admin)
            {
                model.UserTypeId = DataManager.ToInt(Request.Form["UserTypeId"]);

                // Validate User Type.
                if (model.UserTypeId != (int)UserType.Type.Admin && model.UserTypeId != (int)UserType.Type.SiteManager && model.UserTypeId != (int)UserType.Type.Staff)
                {
                    model.UserTypeId = (int)UserType.Type.Staff;
                }

                model.SiteId = DataManager.ToInt(Request.Form["SiteId"]);
            }
            else if (Auth.User.UserTypeId == UserType.Type.SiteManager)
            {
                model.UserTypeId = DataManager.ToInt(Request.Form["UserTypeId"]);

                // Validate User Type.
                if (model.UserTypeId != (int)UserType.Type.SiteManager && model.UserTypeId != (int)UserType.Type.Staff)
                {
                    model.UserTypeId = (int)UserType.Type.Staff;
                }

                model.SiteId = Auth.User.SiteId;
            }
            else
            {
                model.UserTypeId = UserType.Type.Staff;
                model.SiteId = Auth.User.SiteId;
            }

            model.TitleId = DataManager.ToInt(Request.Form["TitleId"]);
            model.Email = DataManager.ToString(Request.Form["Email"]);
            model.Password = DataManager.ToString(Request.Form["Password"]);
            model.PasswordEncrypted = DataProtection.Encrypt(model.Password);
            model.FirstName = DataManager.ToString(Request.Form["FirstName"]);
            model.MiddleName = DataManager.ToString(Request.Form["MiddleName"]);
            model.LastName = DataManager.ToString(Request.Form["LastName"]);
            model.Gender = DataManager.ToInt(Request.Form["Gender"]);
            model.Birthdate = DataManager.ToDateTime(Request.Form["Birthdate"], "dd/MM/yyyy", DateTime.Today);

            model.IsSubscriber = false;
            model.IsReceiveEmailInfo = false;

            //model.Address = DataManager.ToString(Request.Form["Address"]);
            //model.Street = DataManager.ToString(Request.Form["Street"]);
            //model.City = DataManager.ToString(Request.Form["City"]);
            //model.PostalCode = DataManager.ToString(Request.Form["PostalCode"]);
            //model.Phone = DataManager.ToString(Request.Form["Phone"]);
            //model.MobilePhone = DataManager.ToString(Request.Form["MobilePhone"]);
            //model.CountryId = DataManager.ToInt(Request.Form["CountryId"]);
            //model.ShippingAddress = DataManager.ToString(Request.Form["ShippingAddress"]);
            //model.ShippingStreet = DataManager.ToString(Request.Form["ShippingStreet"]);
            //model.ShippingCity = DataManager.ToString(Request.Form["ShippingCity"]);
            //model.ShippingPostalCode = DataManager.ToString(Request.Form["ShippingPostalCode"]);
            //model.ShippingPhone = DataManager.ToString(Request.Form["ShippingPhone"]);
            //model.ShippingCountryId = DataManager.ToInt(Request.Form["ShippingCountryId"]);
            //model.Remarks = DataManager.ToString(Request.Form["Remarks"]);
            //model.FBAccount = DataManager.ToString(Request.Form["FBAccount"]);
            //model.IsReceiveEmailInfo = DataManager.ToBoolean(Request.Form["IsReceiveEmailInfo"]);

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

        protected override bool DoDelete(long id)
        {
            return DataAccess.DeleteUser(id) > 0;
        }
        #endregion

        #region Private Methods
        private void InitUserTypeDropDown()
        {
            List<object> userTypes = new List<object>();
            {

            }
            if (Auth.User.UserTypeId == UserType.Type.SuperAdmin || Auth.User.UserTypeId == UserType.Type.Admin)
            {
                userTypes.Add(new
                {
                    UserTypeId = (int)UserType.Type.Admin,
                    UserTypeName = Resources.Resources.Administrator
                });
            }
            userTypes.Add(new
            {
                UserTypeId = (int)UserType.Type.SiteManager,
                UserTypeName = Resources.Resources.SiteManager
            });
            userTypes.Add(new
            {
                UserTypeId = (int)UserType.Type.Staff,
                UserTypeName = Resources.Resources.Staff
            });
            ViewBag.DropDownUserTypes = ListToDropDownList(userTypes, "UserTypeId", "UserTypeName");
        }
        #endregion
    }
}
