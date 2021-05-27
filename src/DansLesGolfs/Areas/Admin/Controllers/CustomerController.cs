using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Admin.Controllers
{
    public class CustomerController : BaseAdminCRUDController
    {
        #region Constructor
        public CustomerController()
        {
            ObjectName = "Customer";
            TitleName = Resources.Resources.CustomerInformation;
            PrimaryKey = "UserId";

            // Define Column Names.
            ColumnNames.Add("Email", Resources.Resources.Email);
            ColumnNames.Add("Firstname", Resources.Resources.FirstName);
            ColumnNames.Add("Middlename", Resources.Resources.MiddleName);
            ColumnNames.Add("Lastname", Resources.Resources.LastName);

            // Add actions list.
            ActionList.Add("export", Resources.Resources.Export);

            // Add Javascrips
            JavaScripts.Add("~/Assets/Admin/scripts/page/list-customer.js");
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<User> models = DataAccess.GetAllCustomers(param);
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
                if (model == null)
                {
                    model = new User();
                }
            }
            else
            {
                model = new User();
            }
            model.UserTypeId = (int)UserType.Type.Customer;
            model.TitleId = DataManager.ToInt(Request.Form["TitleId"]);
            model.Email = DataManager.ToString(Request.Form["Email"]);
            model.Password = DataManager.ToString(Request.Form["Password"]);
            model.PasswordEncrypted = DataProtection.Encrypt(model.Password);
            model.FirstName = DataManager.ToString(Request.Form["Firstname"]);
            model.MiddleName = DataManager.ToString(Request.Form["Middlename"]);
            model.LastName = DataManager.ToString(Request.Form["Lastname"]);
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

            model.Address = DataManager.ToString(Request.Form["Address"]);
            model.Street = DataManager.ToString(Request.Form["Street"]);
            model.CityId = DataManager.ToInt(Request.Form["CityId"]);
            model.PostalCode = DataManager.ToString(Request.Form["PostalCode"]);
            model.Phone = DataManager.ToString(Request.Form["Phone"]);
            model.PhoneCountryCode = DataManager.ToString(Request.Form["PhoneCountryCode"]).Trim();
            model.MobilePhone = DataManager.ToString(Request.Form["MobilePhone"]).Trim();
            model.MobilePhoneCountryCode = DataManager.ToString(Request.Form["MobilePhoneCountryCode"]).Trim();
            model.CountryId = DataManager.ToInt(Request.Form["CountryId"]);
            model.ShippingAddress = DataManager.ToString(Request.Form["ShippingAddress"]);
            model.ShippingStreet = DataManager.ToString(Request.Form["ShippingStreet"]);
            model.ShippingCityId = DataManager.ToInt(Request.Form["ShippingCityId"]);
            model.ShippingPostalCode = DataManager.ToString(Request.Form["ShippingPostalCode"]).Trim();
            model.ShippingPhone = DataManager.ToString(Request.Form["ShippingPhone"]).Trim();
            model.ShippingPhoneCountryCode = DataManager.ToString(Request.Form["ShippingPhoneCountryCode"]).Trim();
            model.ShippingCountryId = DataManager.ToInt(Request.Form["ShippingCountryId"]);
            model.Remarks = DataManager.ToString(Request.Form["Remarks"]);
            model.FBAccount = DataManager.ToString(Request.Form["FBAccount"]);
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
        #endregion

        #region Action Methods
        public ActionResult ExportData()
        {
            List<User> customers = DataAccess.GetAllCustomers();
            string text = string.Empty;

            // Make CSV text.
            text = CreateCSVTextFile<User>(customers);

            var cd = new System.Net.Mime.ContentDisposition()
            {
                FileName = "customers-" + DateTime.Today.ToString("yyyy-MM-dd") + ".csv",
                Inline = false
            };
            Response.AppendHeader("Content-disposition", cd.ToString());
            return File(System.Text.Encoding.UTF8.GetBytes(text), "text/csv");
        }

        private static string CreateCSVTextFile<T>(List<T> data)
        {
            var properties = typeof(T).GetProperties();
            var result = new StringBuilder();


            var propNames = properties.Select(p => p.Name);
            result.AppendLine(String.Join(",", propNames));

            foreach (var row in data)
            {
                var values = properties.Select(p => p.GetValue(row, null))
                                       .Select(v => StringToCSVCell(Convert.ToString(v)));
                var line = string.Join(",", values);
                result.AppendLine(line);
            }

            return result.ToString();
        }

        private static string StringToCSVCell(string str)
        {
            bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n"));
            if (mustQuote)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"");
                foreach (char nextChar in str)
                {
                    sb.Append(nextChar);
                    if (nextChar == '"')
                        sb.Append("\"");
                }
                sb.Append("\"");
                return sb.ToString();
            }

            return str;
        }
        #endregion
    }
}
