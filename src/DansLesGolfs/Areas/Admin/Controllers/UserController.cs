using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Admin.Controllers
{
    public class UserController : BaseAdminCRUDController
    {
        #region Constructor
        public UserController()
        {
            ObjectName = "User";
            TitleName = "User";
            PrimaryKey = "UserId";

            // Define Column Names.
            ColumnNames.Add("Email", "Email");
            ColumnNames.Add("Firstname", "Firstname");
            ColumnNames.Add("Middlename", "Middlename");
            ColumnNames.Add("Lastname", "Lastname");
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<User> models = DataAccess.GetAllUsers(param);
            return models;
        }

        protected override void DoPrepareForm(int? id)
        {
            List<Country> countries = DataAccess.GetAllCountries();
            ViewBag.Countries = ListToDropDownList<Country>(countries, "CountryId", "CountryName");
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
            int id = DataManager.ToInt(Request.Form["id"]);
            int result = -1;
            User model = GetUserByFormData(id);

            if (id > 0)
            {
                if (model.InsertDate == DateTime.MinValue)
                    model.InsertDate = model.UpdateDate = DateTime.Now;

                if (model.RegisteredDate == DateTime.MinValue)
                    model.RegisteredDate = DateTime.Now;

                if (model.LastLoggedOn == DateTime.MinValue)
                    model.LastLoggedOn = DateTime.Now;

                if (model.ExpiredDate == DateTime.MinValue)
                    model.ExpiredDate = model.UpdateDate.Value.AddYears(100);

                result = DataAccess.UpdateUser(model);
            }
            else
            {
                model.InsertDate = model.UpdateDate;
                model.RegisteredDate = model.UpdateDate;
                model.LastLoggedOn = model.UpdateDate;
                model.ExpiredDate = model.UpdateDate.Value.AddYears(100);
                model.SiteId = 0;
                model.Active = true;
                result = DataAccess.AddUser(model);
                model.UserId = result;
            }
            ViewBag.id = result > -1 ? model.UserId : -1;
            return result > 0;
        }

        private BLL.User GetUserByFormData(int id)
        {
            User model = null;
            if (id > 0)
            {
                model = DataAccess.GetUser(id);
                if(model == null)
                {
                    model = new User();
                }
            }
            else
            {
                model = new User();
            }
            model.UserTypeId = (int)UserType.Type.Admin;
            model.TitleId = DataManager.ToInt(Request.Form["TitleId"]);
            model.Email = DataManager.ToString(Request.Form["Email"]).Trim();
            model.Password = DataManager.ToString(Request.Form["Password"]).Trim();
            model.PasswordEncrypted = DataProtection.Encrypt(model.Password).Trim();
            model.FirstName = DataManager.ToString(Request.Form["Firstname"]).Trim();
            model.MiddleName = DataManager.ToString(Request.Form["Middlename"]).Trim();
            model.LastName = DataManager.ToString(Request.Form["Lastname"]).Trim();
            model.Gender = DataManager.ToInt(Request.Form["Gender"]);

            string birthDate = Request.Form["Birthdate"].ToString();
            if (!String.IsNullOrEmpty(birthDate) && !String.IsNullOrWhiteSpace(birthDate))
            {
                model.Birthdate = DataManager.ToDateTime(birthDate, "dd/MM/yyyy", DateTime.MinValue);
                model.Birthdate = model.Birthdate.Value == DateTime.MinValue ? null : (DateTime?)model.Birthdate.Value;
            }
            else
            {
                model.Birthdate = null;
            }

            model.Address = DataManager.ToString(Request.Form["Address"]).Trim();
            model.Street = DataManager.ToString(Request.Form["Street"]).Trim();
            model.CityId = DataManager.ToInt(Request.Form["CityId"]);
            model.PostalCode = DataManager.ToString(Request.Form["PostalCode"]).Trim();
            model.Phone = DataManager.ToString(Request.Form["Phone"]).Trim();
            model.PhoneCountryCode = DataManager.ToString(Request.Form["PhoneCountryCode"]).Trim();
            model.MobilePhone = DataManager.ToString(Request.Form["MobilePhone"]).Trim();
            model.MobilePhoneCountryCode = DataManager.ToString(Request.Form["MobilePhoneCountryCode"]).Trim();
            model.CountryId = DataManager.ToInt(Request.Form["CountryId"]);
            model.ShippingAddress = DataManager.ToString(Request.Form["ShippingAddress"]).Trim();
            model.ShippingStreet = DataManager.ToString(Request.Form["ShippingStreet"]).Trim();
            model.ShippingCityId = DataManager.ToInt(Request.Form["ShippingCityId"]);
            model.ShippingPostalCode = DataManager.ToString(Request.Form["ShippingPostalCode"]).Trim();
            model.ShippingPhone = DataManager.ToString(Request.Form["ShippingPhone"]).Trim();
            model.ShippingPhoneCountryCode = DataManager.ToString(Request.Form["ShippingPhoneCountryCode"]).Trim();
            model.ShippingCountryId = DataManager.ToInt(Request.Form["ShippingCountryId"]);
            model.Remarks = DataManager.ToString(Request.Form["Remarks"]).Trim();
            model.FBAccount = DataManager.ToString(Request.Form["FBAccount"]).Trim();
            model.IsReceiveEmailInfo = model.IsSubscriber = DataManager.ToBoolean(Request.Form["IsReceiveEmailInfo"]);

            if (Auth.User != null)
                model.ModifyUserId = Auth.User.UserId;

            model.UpdateDate = DateTime.Now;
            return model;
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteUser(id) > 0;
        }

        public ActionResult EditProfile()
        {

            User user = DataAccess.GetUser(Auth.User.UserId);
            if (user == null)
                return RedirectToAction("Index", "Dashboard");

            ObjectName = "EditProfile";
            TitleName = Resources.Resources.EditProfile;
            Breadcrumbs.Add(Resources.Resources.EditProfile, "~/Admin/User/EditProfile");
            Init();

            DoPrepareForm(user.UserId);

            CreateViewBagFromObject(user);
            return View();
        }

        [HttpPost]
        public ActionResult SaveProfile()
        {
            int result = -1;
            User model = GetUserByFormData(Auth.User.UserId);
            result = DataAccess.UpdateUser(model);
            if (result > -1)
            {
                TempData["SuccessMessage"] = Resources.Resources.SaveUserProfileSuccess;
            }
            else
            {
                TempData["ErrorMessages"] = Resources.Resources.SaveUserProfileFailed;
            }
            CreateViewBagFromObject(model);
            return View("EditProfile");
        }
        #endregion
    }
}
