using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.ECM.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.ECM.Controllers
{
    public class MailingListController : BaseECMCRUDController<MailingList>
    {
        #region Constructor
        public MailingListController()
        {
            ObjectName = "MailingList";
            TitleName = Resources.Resources.MailingList;
            PrimaryKey = "MailingListId";

            // Define Column Names.
            ColumnNames.Add("MailingListName", Resources.Resources.MailingListName);

            AddAllowedUserType(UserType.Type.Admin);
            AddAllowedUserType(UserType.Type.Staff);
        }
        #endregion

        #region Overriden Methods
        protected override IQueryable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<MailingList> models = null;
            if (Auth.User.UserTypeId == UserType.Type.SiteManager || Auth.User.UserTypeId == UserType.Type.Staff)
            {
                models = DataAccess.GetAllMailingLists(param, Auth.User.SiteId);
            }
            else
            {
                models = DataAccess.GetAllMailingLists(param);
            }
            return models;
        }

        public JsonResult LoadContactListJSON(jQueryDataTableParamModel param, long? siteId)
        {
            try
            {
                List<User> customers = null;
                ColumnNames.Clear();
                ColumnNames.Add("FirstName", "FirstName");
                ColumnNames.Add("LastName", "LastName");
                ColumnNames.Add("Email", "Email");
                ColumnNames.Add("CustomerTypeName", "CustomerTypeName");
                ColumnNames.Add("SubCustomerTypeName", "SubCustomerTypeName");
                PrepareListParams(param);
                if (Auth.User.UserTypeId == UserType.Type.SiteManager || Auth.User.UserTypeId == UserType.Type.Staff)
                {
                    customers = DataAccess.GetAllCustomers(param, Auth.User.SiteId, 1);
                }
                else
                {
                    customers = DataAccess.GetAllCustomers(param, siteId, 1);
                }

                if (customers != null)
                {
                    object[] resultArray = CustomerListToArray(customers);
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
                else
                {
                    return Json(new
                    {
                        draw = param.draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = new object[0]
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    draw = param.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new object[0]
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private object[] CustomerListToArray(List<User> customers)
        {
            List<object> list = new List<object>();
            List<object> row = null;
            foreach (User c in customers)
            {
                row = new List<object>();
                row.Add("<input type=\"checkbox\" value=\"" + c.UserId + "\" />");
                row.Add(c.FirstName);
                row.Add(c.LastName);
                row.Add(c.Email);
                row.Add(c.CustomerTypeName);
                row.Add(c.SubCustomerTypeName);
                row.Add("<a href=\"javascript:void(0);\" data-id=\"" + c.UserId + "\" data-firstname=\"" + c.FirstName + "\" data-lastname=\"" + c.LastName + "\" data-fullname=\"" + c.FullName + "\" data-email=\"" + c.Email + "\" data-customertype=\"" + c.CustomerTypeName + "\" data-subcustomertype=\"" + c.SubCustomerTypeName + "\" class=\"select-link\">Select</a>");
                list.Add(row.ToArray());
            }
            return list.ToArray();
        }

        protected override object DoPrepareNew()
        {
            return new
            {
                MailingListName = string.Empty,
                Active = false,
                ContentType = 0,
                SiteId = 0
            };
        }

        protected override void DoPrepareForm(int? id = null)
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
        }

        protected override object DoPrepareEdit(long id)
        {
            MailingList model = DataAccess.GetMailingListByID(id);
            DataSet ds = DataAccess.GetCustomerMailingList(id);
            List<User> customers = new List<User>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                customers.Add(new User()
                {
                    UserId = DataManager.ToInt(row["UserId"]),
                    FirstName = DataManager.ToString(row["Firstname"]),
                    MiddleName = DataManager.ToString(row["Middlename"]),
                    LastName = DataManager.ToString(row["Lastname"]),
                    Email = DataManager.ToString(row["Email"]),
                    CustomerTypeName = DataManager.ToString(row["CustomerTypeName"]),
                    SubCustomerTypeName = DataManager.ToString(row["SubCustomerTypeName"])
                });
            }
            ViewBag.Customers = customers;
            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            MailingList model = new MailingList();
            model.MailingListId = DataManager.ToInt(Request.Form["id"], -1);
            model.MailingListName = DataManager.ToString(Request.Form["MailingListName"]);

            if (Auth.User.UserTypeId == UserType.Type.SuperAdmin || Auth.User.UserTypeId == UserType.Type.Admin)
            {
                model.SiteId = DataManager.ToInt(Request.Form["SiteId"]);
            }
            else
            {
                model.SiteId = Auth.User.SiteId;
            }

            MailingList obj = DataAccess.GetMailingListByID(model.MailingListId);
            if (obj != null)
            {
                DataAccess.UpdateMailingList(model);
                result = model.MailingListId;
            }
            else
            {
                model.Active = true;
                result = DataAccess.AddMailingList(model);
                model.MailingListId = result;
            }
            return result > 0;
        }

        protected override void DoSaveSuccess(int id)
        {
            string[] customerIds = Request.Form.GetValues("CustomerIds");
            if(customerIds != null && customerIds.Any())
            {
                DataAccess.DeleteCustomerMailingList(id);
                foreach (string customerId in customerIds)
                {
                    DataAccess.AddCustomerMailingList(id, DataManager.ToInt(customerId));
                }
            }
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteMailingList(id) > 0;
        }
        #endregion
    }
}
