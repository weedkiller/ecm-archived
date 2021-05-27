using DansLesGolfs.Base;
using DansLesGolfs.Base.Services;
using DansLesGolfs.BLL;
using DansLesGolfs.ECM.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Kendo.Mvc.UI.Fluent;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Threading.Tasks;
using System.Threading;

namespace DansLesGolfs.ECM.Controllers
{
    public class CustomerController : BaseECMCRUDController<UserViewModel>
    {
        #region Constructor
        public CustomerController()
        {
            ObjectName = "Customer";
            TitleName = Resources.Resources.Subscriber;
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
            if (Auth.User.UserTypeId == UserType.Type.SuperAdmin || Auth.User.UserTypeId == UserType.Type.Admin)
            {
                return DataAccess.GetListECMSubscribers(null, 1);
            }
            else
            {
                return new List<UserViewModel>().AsQueryable();
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
                subCustomerTypes = DataAccess.GetAllCustomerTypes(1, Auth.User.SiteId, DataManager.ToLong(ViewBag.CustomerTypeId));
            }
            else
            {
                customerGroups = DataAccess.GetAllCustomerGroups();
                customerTypes = DataAccess.GetAllCustomerTypes(1, DataManager.ToLong(ViewBag.SiteId));
                subCustomerTypes = DataAccess.GetAllCustomerTypes(1, DataManager.ToLong(ViewBag.SiteId), DataManager.ToLong(ViewBag.CustomerTypeId));
            }
            #endregion

            #region Prepare Drop Down List
            ViewBag.CustomerGroupsList = customerGroups;

            List<SelectListItem> customerTypeList = ListToDropDownList<CustomerType>(customerTypes, "CustomerTypeId", "CustomerTypeName");
            customerTypeList.Insert(0, new SelectListItem()
            {
                Selected = true,
                Text = Resources.Resources.CustomerType,
                Value = "0"
            });
            ViewBag.CustomerTypes = customerTypeList;

            List<SelectListItem> subCustomerTypeList = ListToDropDownList<CustomerType>(subCustomerTypes, "CustomerTypeId", "CustomerTypeName");
            subCustomerTypeList.Insert(0, new SelectListItem()
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
            return new User() { IsSubscriber = true, IsReceiveEmailInfo = true };
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
            model.SubCustomerTypeId = DataManager.ToLong(Request.Form["SubCustomerTypeId"]);

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

        #region Action Methods
        #region Import
        public ActionResult Import()
        {
            if (Auth.User.UserTypeId == UserType.Type.SuperAdmin || Auth.User.UserTypeId == UserType.Type.Admin)
            {
                ViewBag.CustomerGroups = DataAccess.GetAllCustomerGroups();
                List<Site> sites = DataAccess.GetSitesDropDownListData();
                List<SelectListItem> siteList = ListToDropDownList<Site>(sites, "SiteId", "SiteName");
                siteList.Insert(0, new SelectListItem()
                {
                    Text = Resources.Resources.SelectSite,
                    Value = "0",
                    Selected = true
                });
                ViewBag.Sites = siteList;
                ViewBag.SiteId = 0;
            }
            else
            {
                ViewBag.CustomerGroups = DataAccess.GetAllCustomerGroups(Auth.User.SiteId);
            }

            ObjectName = "ImportCustomer";
            Breadcrumbs.Add(Resources.Resources.Customer, "~/Customer");
            Breadcrumbs.Add(Resources.Resources.ImportCustomers, "~/Customer/Import");
            Init();
            return View();
        }
        #endregion

        #region BulkEdit
        public ActionResult BulkEdit(string mode)
        {
            string[] columnNames = new string[] {
                Resources.Resources.Email,
                Resources.Resources.FirstName,
                Resources.Resources.LastName,
                Resources.Resources.Gender,
                Resources.Resources.BirthDate,
                Resources.Resources.LicenseNumber,
                Resources.Resources.Career,
                Resources.Resources.Index,
                Resources.Resources.Description,
                Resources.Resources.Address,
                Resources.Resources.City,
                Resources.Resources.Country,
                Resources.Resources.Telephone,
                Resources.Resources.MobilePhone,
                "Custom Field 1",
                "Custom Field 2",
                "Custom Field 3"
            };
            string[] columnWidths = new string[]
            {
                "*",
                "100",
                "100",
                "100",
                "120",
                "100",
                "100",
                "100",
                "100",
                "100",
                "100",
                "100",
                "120",
                "120",
                "100",
                "100",
                "100"
            };
            string[] columnAligns = new string[]
            {
                "left",
                "left",
                "left",
                "center",
                "center",
                "left",
                "left",
                "right",
                "left",
                "left",
                "left",
                "left",
                "center",
                "center",
                "left",
                "left",
                "left"
            };
            string[] columnSortings = new string[]
            {
                "str",
                "str",
                "str",
                "str",
                "str",
                "str",
                "str",
                "int",
                "str",
                "str",
                "str",
                "str",
                "str",
                "str",
                "str",
                "str",
                "str"
            };
            string[] columnFilters = new string[]
            {
                "#text_filter",
                "#text_filter",
                "#text_filter",
                "#select_filter",
                "#text_filter",
                "#text_filter",
                "#text_filter",
                "#numeric_filter",
                "#text_filter",
                "#text_filter",
                "#text_filter",
                "#text_filter",
                "#text_filter",
                "#text_filter",
                "#text_filter",
                "#text_filter",
                "#text_filter"
            };
            ViewBag.GridViewColumnNames = String.Join(",", columnNames);
            ViewBag.GridViewColumnWidths = String.Join(",", columnWidths);
            ViewBag.GridViewColumnAligns = String.Join(",", columnAligns);
            ViewBag.GridViewColumnSortings = String.Join(",", columnSortings);
            ViewBag.GridViewColumnFilters = String.Join(",", columnFilters);
            if (!String.IsNullOrWhiteSpace(mode) && mode.Equals("import", StringComparison.InvariantCultureIgnoreCase))
            {

            }

            TitleName = Resources.Resources.BulkEdit;
            ObjectName = "BulkEditCustomer";
            Breadcrumbs.Add(Resources.Resources.Customer, "~/Customer");
            Breadcrumbs.Add(Resources.Resources.BulkEdit, "~/Customer/BulkEdit");
            Init();
            return View();
        }

        public ActionResult AjaxSearchBulkCustomer(string siteId, string email, string firstname, string lastname, string gender, string birthdateFrom, string birthdateTo, string description, string career, string index, string licenseNumber, string address, string city, string country, string telephone, string mobilephone, string customField1, string customField2, string customField3,
            string siteIdOp, string emailOp, string firstnameOp, string lastnameOp, string genderOp, string birthdateOp, string descriptionOp, string careerOp, string indexOp, string licenseNumberOp, string addressOp, string cityOp, string countryOp, string telephoneOp, string mobilephoneOp, string customField1Op, string customField2Op, string customField3Op,
            string customerType, string customerTypeOp, string subCustomerType, string subCustomerTypeOp, string page, string pageSize)
        {
            try
            {
                string where = "LEN(ISNULL([Users].[Email], '')) > 0";

                #region SiteId
                if (Auth.User.UserTypeId == UserType.Type.SiteManager || Auth.User.UserTypeId == UserType.Type.Staff)
                {
                    where += " AND [Users].[SiteId] = " + Auth.User.SiteId;
                }
                else if (!String.IsNullOrEmpty(siteId))
                {
                    where += " AND [Users].[SiteId] ";
                    if (siteIdOp == "eq")
                    {
                        where += " = '" + siteId + "'";
                    }
                    else
                    {
                        where += " <> '" + siteId + "'";
                    }
                }
                #endregion

                #region Email
                where += GetSQLFilter(email, emailOp, "[Users].[Email]");
                #endregion

                #region FirstName
                where += GetSQLFilter(firstname, firstnameOp, "[Users].[FirstName]");
                #endregion

                #region LastName
                where += GetSQLFilter(lastname, lastnameOp, "[Users].[LastName]");
                #endregion

                #region Gender
                if (!String.IsNullOrWhiteSpace(gender))
                {
                    where += " AND [Users].[Gender] = '" + gender + "'";
                }
                #endregion

                #region BirthDate
                if (!String.IsNullOrWhiteSpace(birthdateFrom) && !String.IsNullOrWhiteSpace(birthdateTo))
                {
                    DateTime d1 = DataManager.ToDateTime(birthdateFrom, "d/M/yyyy");
                    DateTime d2 = DataManager.ToDateTime(birthdateTo, "d/M/yyyy");
                    where += " AND [Users].[BirthDate] BETWEEN '" + d1.ToString("yyyyMMdd") + "' AND '" + d2.ToString("yyyyMMdd") + "'";
                }
                else if (!String.IsNullOrWhiteSpace(birthdateFrom))
                {
                    DateTime d1 = DataManager.ToDateTime(birthdateFrom, "d/M/yyyy");
                    where += " AND [Users].[BirthDate] >= '" + d1.ToString("yyyyMMdd") + "'";
                }
                else if (!String.IsNullOrWhiteSpace(birthdateTo))
                {
                    DateTime d1 = DataManager.ToDateTime(birthdateTo, "d/M/yyyy");
                    where += " AND [Users].[BirthDate] <= '" + d1.ToString("yyyyMMdd") + "'";
                }
                #endregion

                #region LicenseNumber
                where += GetSQLFilter(licenseNumber, licenseNumberOp, "[Users].[LicenseNumber]");
                #endregion

                #region Career
                where += GetSQLFilter(career, careerOp, "[Users].[Career]");
                #endregion

                #region CustomerType
                where += GetSQLFilter(customerType, customerTypeOp, "[CustomerTypeLang].[CustomerTypeName]");
                #endregion

                #region CustomerType
                where += GetSQLFilter(subCustomerType, subCustomerTypeOp, "[SubCustomerTypeLang].[CustomerTypeName]");
                #endregion

                //int totalPages = 0, start = 0, end = 0, totalItems = 0, nPage = DataManager.ToInt(page, 1), nPageSize = DataManager.ToInt(pageSize, 500);
                //List<User> customers = DataAccess.GetAllUsers(where, nPage, nPageSize, out start, out end, out totalItems, out totalPages, 1);
                List<User> customers = new List<BLL.User>();
                CustomerBulkEditModel data = new CustomerBulkEditModel();
                foreach (User customer in customers)
                {
                    data.Add(customer);
                }

                var jsonContent = new
                {
                    isSuccess = true,
                    data = data,
                    //start = start,
                    //end = end,
                    //totalItems = totalItems,
                    //totalPages = totalPages,
                    //page = nPage,
                    //pageSize = nPageSize
                };
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = int.MaxValue;
                return new ContentResult()
                {
                    Content = serializer.Serialize(jsonContent),
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json"
                };
                //return new CorrectJsonResult
                //{
                //    Data = jsonContent,
                //    MaxJsonLength = int.MaxValue,
                //    ContentType = "application/json",
                //    ContentEncoding = Encoding.UTF8
                //};
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
        public ActionResult AjaxSaveBulkCustomer(FormCollection form)
        {
            try
            {
                string ids = DataManager.ToString(form["ids"]);
                string action_type = DataManager.ToString(form[ids + "_!nativeeditor_status"]);
                long source_id = DataManager.ToLong(form[ids + "_gr_id"]);
                long target_id = DataManager.ToLong(form[ids + "_gr_id"]);
                User user = null;
                int i = 0;
                if (DataManager.ToString(form[ids + "_c0"]) == "new customer")
                {
                    user = new BLL.User();
                    i = 1;
                }
                else
                {
                    user = DataAccess.GetUser(DataManager.ToLong(ids));
                    if (user == null)
                    {
                        user = new BLL.User();
                        i = 1;
                    }
                }

                user.Email = GetBulkCustomerField(form, ids, ref i);
                user.FirstName = GetBulkCustomerField(form, ids, ref i);
                user.LastName = GetBulkCustomerField(form, ids, ref i);
                user.Gender = DataManager.ToString(form[ids + "_c" + (i++)]) == Resources.Resources.Male ? 0 : 1;
                user.Birthdate = DataManager.ToDateTime(form[ids + "_c" + (i++)], "d/M/yyyy");
                user.LicenseNumber = GetBulkCustomerField(form, ids, ref i);
                user.Career = GetBulkCustomerField(form, ids, ref i);
                user.Index = DataManager.ToFloat(form[ids + "_c" + (i++)]);
                user.Remarks = GetBulkCustomerField(form, ids, ref i);
                user.Address = user.ShippingAddress = GetBulkCustomerField(form, ids, ref i);
                user.City = user.ShippingCity = GetBulkCustomerField(form, ids, ref i);
                string countryName = GetBulkCustomerField(form, ids, ref i);
                if (!String.IsNullOrWhiteSpace(countryName))
                {
                    int countryId = DataAccess.GetCountryIdByCountryName(countryName);
                    user.CountryId = user.ShippingCountryId = countryId;
                }
                user.Phone = user.ShippingPhone = GetBulkCustomerField(form, ids, ref i);
                user.MobilePhone = GetBulkCustomerField(form, ids, ref i);
                user.CustomField1 = GetBulkCustomerField(form, ids, ref i);
                user.CustomField2 = GetBulkCustomerField(form, ids, ref i);
                user.CustomField3 = GetBulkCustomerField(form, ids, ref i);

                try
                {
                    switch (action_type)
                    {
                        case "inserted":
                            target_id = user.UserId = DataAccess.AddUser(user);
                            break;
                        case "deleted":
                            DataAccess.DeleteUser(user.UserId);
                            break;
                        default: // "updated"     
                            DataAccess.UpdateUser(user);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    action_type = "error";
                }

                return Json(new
                {
                    data = new
                    {
                        action = action_type,
                        sid = source_id,
                        tid = target_id
                    }
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

        private string GetBulkCustomerField(FormCollection form, string id, ref int i)
        {
            return DataManager.ToString(form[id + "_c" + (i++)]);
        }

        private string GetSQLFilter(string value, string op, string field)
        {
            string where = "";
            if (!String.IsNullOrEmpty(value))
            {
                where += " AND " + field + " ";
                if (op == "eq")
                {
                    where += " = '" + value + "'";
                }
                else if (op == "con")
                {
                    where += " LIKE '%" + value + "%'";
                }
                else if (op == "start")
                {
                    where += " LIKE '" + value + "%'";
                }
                else if (op == "end")
                {
                    where += " LIKE '%" + value + "'";
                }
                else
                {
                    where += " <> '" + value + "'";
                }
            }

            return where;
        }
        #endregion

        #region UnsubscribedCustomers
        public ActionResult UnsubscribersList()
        {
            Init();
            return View("~/Views/CRUD/List.cshtml");
        }
        #endregion
        #endregion

        #region Ajax Action

        #region GetCustomerTypes
        [HttpPost]
        public JsonResult GetCustomerTypes(long? siteId)
        {
            try
            {
                string html = string.Empty;
                List<CustomerType> customerTypes = DataAccess.GetCustomerTypesDropDownList(1, null, siteId.Value);
                html += "<option value=\"\">" + Resources.Resources.CustomerType + "</option>";
                foreach (CustomerType it in customerTypes)
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
                logger.Error(ex);
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region GetSubCustomerTypes
        [HttpPost]
        public JsonResult GetSubCustomerTypes(int? customerTypeId, long? siteId)
        {
            try
            {
                string html = string.Empty;
                List<CustomerType> customerTypes = DataAccess.GetCustomerTypesDropDownList(1, customerTypeId, siteId);
                html += "<option value=\"\">" + Resources.Resources.SubCustomerType + "</option>";
                foreach (CustomerType it in customerTypes)
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
                logger.Error(ex);
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region Importing
        #region AjaxImportFile
        [HttpPost]
        public ActionResult AjaxImportFile()
        {
            string uploadDir = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
            string destinationDir = Path.Combine(Server.MapPath("~/" + uploadDir), "Customers", "Import");
            string filePath = string.Empty;
            string fileName = string.Empty;
            string baseName = string.Empty;
            string extension = string.Empty;

            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
                if (!Directory.Exists(destinationDir))
                {
                    return Content("Can't create directory. Please check directory permession.");
                }
            }

            var file = Request.Files["Filedata"];
            extension = Path.GetExtension(file.FileName).ToLower();
            do
            {
                baseName = Guid.NewGuid().ToString();
                fileName = baseName + extension;
                filePath = Path.Combine(destinationDir, fileName);
            } while (System.IO.File.Exists(filePath));
            file.SaveAs(filePath);

            if (System.IO.File.Exists(filePath))
            {
                DataTable dt = new DataTable();
                if (extension == ".csv")
                {
                    dt = DataImportHelper.ImportCsv(filePath);
                }
                else if (extension == ".xls" || extension == ".xlsx")
                {
                    DataSet ds = DataImportHelper.ImportExcel(filePath);
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                    }
                }

                CustomerImporter importer = new CustomerImporter();
                importer.Table = dt;
                SaveCustomerImporter(importer);

                List<DataColumn> columns = new List<DataColumn>();
                foreach (DataColumn c in dt.Columns)
                {
                    columns.Add(c);
                }
                string columnsStr = String.Join(",", columns.Select(it => it.ColumnName));

                @System.IO.File.Delete(filePath);

                return Content(columnsStr);
            }
            else
            {
                throw new Exception("Can't save file please try again.");
            }
        }
        #endregion

        #region AjaxStartImport
        public JsonResult AjaxStartImport(long? siteId, long[] customerGroupIds, string email, string firstname, string lastname, string birthdate, string gender, string phone, string mobile, string address, string city, string country, string postalcode, string description, string career, string licensenumber, string index, string customertype, string subcustomertype, string customfield1, string customfield2, string customfield3)
        {
            List<string> warningMessages = new List<string>();
            List<string> messageTypes = new List<string>();

            try
            {
                if (String.IsNullOrWhiteSpace(email) || email == "none")
                    throw new Exception(Resources.Resources.EmailCantBeEmpty);

                if (String.IsNullOrWhiteSpace(firstname) || firstname == "none")
                    throw new Exception(String.Format(Resources.Resources.ValueCannotBeEmpty, Resources.Resources.FirstName));

                if (String.IsNullOrWhiteSpace(lastname) || lastname == "none")
                    throw new Exception(String.Format(Resources.Resources.ValueCannotBeEmpty, Resources.Resources.LastName));

                if ((Auth.User.UserTypeId == (int)UserType.Type.SuperAdmin || Auth.User.UserTypeId == (int)UserType.Type.Admin) && (!siteId.HasValue || siteId.Value <= 0))
                    throw new Exception(String.Format(Resources.Resources.ValueCannotBeEmpty, Resources.Resources.Golf));


                if (String.IsNullOrWhiteSpace(birthdate) || birthdate == "none")
                {
                    messageTypes.Add("warning");
                    warningMessages.Add(String.Format(Resources.Resources.YouMayNeedToEditLater, Resources.Resources.BirthDate));
                }

                if (String.IsNullOrWhiteSpace(gender) || gender == "none")
                {
                    messageTypes.Add("warning");
                    warningMessages.Add(String.Format(Resources.Resources.YouMayNeedToEditLater, Resources.Resources.Gender));
                }

                if (String.IsNullOrWhiteSpace(phone) || phone == "none")
                {
                    messageTypes.Add("warning");
                    warningMessages.Add(String.Format(Resources.Resources.YouMayNeedToEditLater, Resources.Resources.Telephone));
                }

                if (String.IsNullOrWhiteSpace(mobile) || mobile == "none")
                {
                    messageTypes.Add("warning");
                    warningMessages.Add(String.Format(Resources.Resources.YouMayNeedToEditLater, Resources.Resources.MobilePhone));
                }

                if (String.IsNullOrWhiteSpace(address) || address == "none")
                {
                    messageTypes.Add("warning");
                    warningMessages.Add(String.Format(Resources.Resources.YouMayNeedToEditLater, Resources.Resources.Address));
                }

                if (String.IsNullOrWhiteSpace(city) || city == "none")
                {
                    messageTypes.Add("warning");
                    warningMessages.Add(String.Format(Resources.Resources.YouMayNeedToEditLater, Resources.Resources.City));
                }

                if (String.IsNullOrWhiteSpace(country) || country == "none")
                {
                    messageTypes.Add("warning");
                    warningMessages.Add(String.Format(Resources.Resources.YouMayNeedToEditLater, Resources.Resources.Country));
                }

                if (String.IsNullOrWhiteSpace(career) || career == "none")
                {
                    messageTypes.Add("warning");
                    warningMessages.Add(String.Format(Resources.Resources.YouMayNeedToEditLater, Resources.Resources.Career));
                }

                if (String.IsNullOrWhiteSpace(licensenumber) || licensenumber == "none")
                {
                    messageTypes.Add("warning");
                    warningMessages.Add(String.Format(Resources.Resources.YouMayNeedToEditLater, Resources.Resources.LicenseNumber));
                }

                if (String.IsNullOrWhiteSpace(index) || index == "none")
                {
                    messageTypes.Add("warning");
                    warningMessages.Add(String.Format(Resources.Resources.YouMayNeedToEditLater, Resources.Resources.Index));
                }

                if (String.IsNullOrWhiteSpace(customertype) || customertype == "none")
                {
                    messageTypes.Add("warning");
                    warningMessages.Add(String.Format(Resources.Resources.YouMayNeedToEditLater, Resources.Resources.CustomerType));
                }

                if (String.IsNullOrWhiteSpace(subcustomertype) || subcustomertype == "none")
                {
                    messageTypes.Add("warning");
                    warningMessages.Add(String.Format(Resources.Resources.YouMayNeedToEditLater, Resources.Resources.SubCustomerType));
                }

                CustomerImporter importer = GetCustomerImporter();
                importer.SiteId = (Auth.User.UserTypeId == (int)UserType.Type.SuperAdmin || Auth.User.UserTypeId == (int)UserType.Type.Admin) ? siteId.Value : Auth.User.SiteId;
                importer.CustomerGroupIds = customerGroupIds;
                importer.Email = email;
                importer.Firstname = firstname;
                importer.Lastname = lastname;
                importer.Birthdate = birthdate;
                importer.Gender = gender;
                importer.Phone = phone;
                importer.Mobile = mobile;
                importer.Address = address;
                importer.City = city;
                importer.Country = country;
                importer.PostalCode = postalcode;
                importer.Description = description;
                importer.Career = career;
                importer.LicenseNumber = licensenumber;
                importer.Index = index;
                importer.CustomerType = customertype;
                importer.SubCustomerType = subcustomertype;
                importer.CustomField1 = customfield1;
                importer.CustomField2 = customfield2;
                importer.CustomField3 = customfield3;
                importer.Messages.AddRange(warningMessages);
                SaveCustomerImporter(importer);

                //System.ComponentModel.BackgroundWorker bg = new System.ComponentModel.BackgroundWorker();
                //bg.WorkerSupportsCancellation = false;
                //bg.WorkerReportsProgress = true;
                //bg.DoWork += CustomerImporterBG_DoWork;
                //bg.RunWorkerAsync(importer);
                //Session["CustomerImportWorker"] = bg;

                Task task = new Task(() =>
                {
                    DoCustomerImport(importer);
                });
                task.Start();

                return new CorrectJsonResult()
                {
                    MaxJsonLength = int.MaxValue,
                    ContentType = "application/json",
                    Data = new
                    {
                        isSuccess = true,
                        message = "Import service has started."
                    }
                };
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

        private void DoCustomerImport(CustomerImporter importer)
        {
            if (importer == null)
                return;

            if (importer.Email == "none")
                return;

            importer.Percent = 0;
            importer.IsCompleted = false;
            importer.ErrorMessage = string.Empty;
            DataRow row = null;
            User user = null;
            CustomerType newCustomerType = null;
            CustomerTypeLang newCustomerTypeLang = null;
            int newCustomerTypeId = 0;

            DateTime now = DateTime.Now;
            DateTime today = DateTime.Today;
            Dictionary<string, int> countryIdLookupTable = DataAccess.GetCountryIdLookupTable();
            Dictionary<string, int> customerTypeIdookupTable = DataAccess.GetCustomerTypeIdLookupTable(1, importer.SiteId);
            string countryString = string.Empty;
            string customerTypeString = string.Empty;
            try
            {
                for (int i = 0, n = importer.Table.Rows.Count; i < n; i++)
                {
                    row = importer.Table.Rows[i];

                    // Prepare User object and save.
                    user = new User();
                    user.SiteId = importer.SiteId;
                    user.UserTypeId = (int)UserType.Type.Customer;
                    user.Email = importer.Table.Columns.Contains(importer.Email) ? DataManager.ToString(row[importer.Email]) : string.Empty;
                    user.FirstName = importer.Table.Columns.Contains(importer.Firstname) ? DataManager.ToString(row[importer.Firstname]) : string.Empty;
                    user.LastName = importer.Table.Columns.Contains(importer.Lastname) ? DataManager.ToString(row[importer.Lastname]) : string.Empty;

                    // Checking existence by email, first name, last name.
                    //if (DataAccess.IsExistsUsers(user.Email, user.FirstName, user.LastName, user.SiteId))
                    //{
                    //    importer.Messages.Add(String.Format(Resources.Resources.UserIsDuplicateSoSkipped, user.FirstName, user.LastName));
                    //    UpdateImportingPercent(importer, i, n);
                    //    continue;
                    //}

                    newCustomerTypeId = 0;
                    countryString = string.Empty;
                    customerTypeString = string.Empty;

                    if (importer.Table.Columns.Contains(importer.Gender))
                    {
                        user.Gender = GetGenderId(DataManager.ToString(row[importer.Gender]));
                    }

                    user.Birthdate = importer.Table.Columns.Contains(importer.Birthdate) ? (row[importer.Birthdate] is DateTime? ? (DateTime?)row[importer.Birthdate] : DataManager.ToDateTime(row[importer.Birthdate], "d/M/yyyy")) : null;

                    user.Remarks = importer.Table.Columns.Contains(importer.Description) ? DataManager.ToString(row[importer.Description]) : string.Empty;
                    user.Career = importer.Table.Columns.Contains(importer.Career) ? DataManager.ToString(row[importer.Career]) : string.Empty;
                    user.LicenseNumber = importer.Table.Columns.Contains(importer.LicenseNumber) ? DataManager.ToString(row[importer.LicenseNumber]) : string.Empty;
                    user.Index = importer.Table.Columns.Contains(importer.Index) ? DataManager.ToFloat(row[importer.Index]) : 0f;
                    user.CustomField1 = importer.Table.Columns.Contains(importer.CustomField1) ? DataManager.ToString(row[importer.CustomField1]) : string.Empty;
                    user.CustomField2 = importer.Table.Columns.Contains(importer.CustomField2) ? DataManager.ToString(row[importer.CustomField2]) : string.Empty;
                    user.CustomField3 = importer.Table.Columns.Contains(importer.CustomField3) ? DataManager.ToString(row[importer.CustomField3]) : string.Empty;

                    #region CustomerType & SubCustomerType
                    if (importer.CustomerType != "none" && importer.Table.Columns.Contains(importer.CustomerType))
                    {
                        customerTypeString = DataManager.ToString(row[importer.CustomerType]);
                        if (!customerTypeIdookupTable.Keys.Contains(customerTypeString))
                        {
                            newCustomerType = new CustomerType()
                            {
                                SiteId = importer.SiteId,
                                CreatedDate = now,
                                UpdatedDate = now,
                                Active = true
                            };
                            newCustomerTypeId = DataAccess.AddCustomerType(newCustomerType);
                            if (newCustomerTypeId > 0)
                            {
                                newCustomerTypeLang = new CustomerTypeLang()
                                {
                                    CustomerTypeId = newCustomerTypeId,
                                    CustomerTypeName = customerTypeString,
                                    LangId = 1
                                };
                                DataAccess.SaveCustomerTypeLang(newCustomerTypeLang);
                                customerTypeIdookupTable.Add(customerTypeString, newCustomerTypeId);
                                user.CustomerTypeId = customerTypeIdookupTable[customerTypeString];
                            }
                        }
                        else
                        {
                            user.CustomerTypeId = newCustomerTypeId = customerTypeIdookupTable[customerTypeString];
                        }

                        // Insert Sub Customer Type
                        if (importer.SubCustomerType != "none" && importer.Table.Columns.Contains(importer.SubCustomerType))
                        {
                            customerTypeString = DataManager.ToString(row[importer.SubCustomerType]);

                            if (!customerTypeIdookupTable.Keys.Contains(customerTypeString))
                            {
                                newCustomerType = new CustomerType()
                                {
                                    ParentId = newCustomerTypeId,
                                    SiteId = importer.SiteId,
                                    CreatedDate = now,
                                    UpdatedDate = now,
                                    Active = true
                                };
                                newCustomerTypeId = DataAccess.AddCustomerType(newCustomerType);
                                if (newCustomerTypeId > 0)
                                {
                                    newCustomerTypeLang = new CustomerTypeLang()
                                    {
                                        CustomerTypeId = newCustomerTypeId,
                                        CustomerTypeName = customerTypeString,
                                        LangId = 1
                                    };
                                    DataAccess.SaveCustomerTypeLang(newCustomerTypeLang);
                                    customerTypeIdookupTable.Add(customerTypeString, newCustomerTypeId);
                                    user.SubCustomerTypeId = customerTypeIdookupTable[customerTypeString];
                                }
                            }
                            else
                            {
                                user.SubCustomerTypeId = customerTypeIdookupTable[customerTypeString];
                            }
                        }
                    }
                    #endregion

                    user.Address = user.ShippingAddress = importer.Table.Columns.Contains(importer.Address) ? DataManager.ToString(row[importer.Address]) : string.Empty;
                    //user.Street = user.ShippingStreet = DataManager.ToString(Request.Form["Street"]);
                    user.PostalCode = user.ShippingPostalCode = importer.Table.Columns.Contains(importer.PostalCode) ? DataManager.ToString(row[importer.PostalCode]) : string.Empty;
                    user.City = user.ShippingCity = importer.Table.Columns.Contains(importer.City) ? DataManager.ToString(row[importer.City]) : string.Empty;
                    user.Phone = user.ShippingPhone = importer.Table.Columns.Contains(importer.Phone) ? DataManager.ToString(row[importer.Phone]) : string.Empty;
                    //user.PhoneCountryCode = DataManager.ToString(Request.Form["PhoneCountryCode"]).Trim();
                    user.MobilePhone = importer.Table.Columns.Contains(importer.Mobile) ? DataManager.ToString(row[importer.Mobile]) : string.Empty;
                    //user.MobilePhoneCountryCode = DataManager.ToString(Request.Form["MobilePhoneCountryCode"]).Trim();
                    if (importer.Table.Columns.Contains(importer.Country))
                    {
                        countryString = DataManager.ToString(row[importer.Country]);
                        user.CountryId = user.ShippingCountryId = countryIdLookupTable.Keys.Contains(countryString) ? countryIdLookupTable[countryString] : 1;
                    }

                    user.IsSubscriber = true;
                    user.IsReceiveEmailInfo = true;
                    user.UpdateDate = now;
                    user.InsertDate = user.UpdateDate;
                    user.RegisteredDate = user.UpdateDate;
                    user.LastLoggedOn = user.UpdateDate;
                    user.ExpiredDate = user.UpdateDate.Value.AddYears(100);
                    user.Active = true;

                    user.UserId = DataAccess.AddUser(user);

                    #region CustomerGroup
                    if (importer.CustomerGroupIds != null && importer.CustomerGroupIds.Any())
                    {
                        foreach (long customerGroupId in importer.CustomerGroupIds)
                        {
                            DataAccess.AddCustomerGroupCustomer(customerGroupId, user.UserId);
                        }
                    }
                    #endregion

                    // Update progress percent.
                    UpdateImportingPercent(importer, i, n);
                }

                importer.IsCompleted = true;
            }
            catch (Exception ex)
            {
                importer.IsCompleted = false;
                importer.IsError = true;
                importer.ErrorMessage = ex.Message;
            }
            finally
            {
            }
        }
        #endregion

        #region AjaxGetImportProgress
        public JsonResult AjaxGetImportProgress()
        {
            int percent = 0;
            try
            {
                CustomerImporter importer = GetCustomerImporter();
                percent = importer.Percent;
                if (importer.Percent >= 100)
                {
                    ClearCustomerImporter();
                    return new CorrectJsonResult()
                    {
                        Data = new
                        {
                            isSuccess = true,
                            percent = percent,
                            isCompleted = true,
                            messages = importer.Messages.ToArray(),
                            messageTypes = importer.Messages.ToArray(),
                            errorText = "Error",
                            warningText = "Warning",
                            infoText = "Info"
                        },
                        MaxJsonLength = int.MaxValue
                    };
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = true,
                        percent = percent,
                        isCompleted = false
                    });
                }
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
        #endregion

        #region Bulk Edit
        [HttpPost]
        public ActionResult AjaxGetBulkEditCustomers([DataSourceRequest]DataSourceRequest request, long? siteId)
        {
            var customers = DataAccess.GetListECMSubscribers(siteId, this.CultureId);
            DataSourceResult result = customers.ToDataSourceResult(request);
            return Json(result);
        }
        #endregion


        #endregion

        #region Private Methods
        private CustomerImporter GetCustomerImporter()
        {
            if (Session["CustomerImporter"] == null || !(Session["CustomerImporter"] is CustomerImporter))
            {
                CustomerImporter importer = new CustomerImporter();
                Session["CustomerImporter"] = importer;
                return importer;
            }
            else
            {
                return Session["CustomerImporter"] as CustomerImporter;
            }
        }

        private void SaveCustomerImporter(CustomerImporter importer)
        {
            if (importer == null)
            {
                importer = new CustomerImporter();
            }
            Session["CustomerImporter"] = importer;
        }

        private void ClearCustomerImporter()
        {
            if (Session["CustomerImporter"] != null && (Session["CustomerImporter"] is CustomerImporter))
            {
                CustomerImporter importer = Session["CustomerImporter"] as CustomerImporter;
                importer.Dispose();
                Session["CustomerImporter"] = null;
            }
        }

        #region MyRegion
        //private void CustomerImporterBG_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        //{
        //    System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();
        //    CustomerImporter importer = e.Argument as CustomerImporter;
        //    if (importer == null)
        //        e.Cancel = true;

        //    if (importer.Email == "none")
        //        e.Cancel = true;

        //    if (worker.CancellationPending)
        //    {
        //        e.Cancel = true;
        //    }
        //    else
        //    {
        //        importer.Percent = 0;
        //        importer.IsCompleted = false;
        //        importer.ErrorMessage = string.Empty;
        //        DataRow row = null;
        //        User user = null;
        //        CustomerType newCustomerType = null;
        //        CustomerTypeLang newCustomerTypeLang = null;
        //        int newCustomerTypeId = 0;

        //        DateTime now = DateTime.Now;
        //        DateTime today = DateTime.Today;
        //        Dictionary<string, int> countryIdLookupTable = DataAccess.GetCountryIdLookupTable();
        //        Dictionary<string, int> customerTypeIdookupTable = DataAccess.GetCustomerTypeIdLookupTable(1, importer.SiteId);
        //        string countryString = string.Empty;
        //        string customerTypeString = string.Empty;
        //        try
        //        {
        //            for (int i = 0, n = importer.Table.Rows.Count; i < n; i++)
        //            {
        //                row = importer.Table.Rows[i];

        //                // Prepare User object and save.
        //                user = new User();
        //                user.SiteId = importer.SiteId;
        //                user.UserTypeId = (int)UserType.Type.Customer;
        //                user.Email = importer.Table.Columns.Contains(importer.Email) ? DataManager.ToString(row[importer.Email]) : string.Empty;
        //                user.FirstName = importer.Table.Columns.Contains(importer.Firstname) ? DataManager.ToString(row[importer.Firstname]) : string.Empty;
        //                user.LastName = importer.Table.Columns.Contains(importer.Lastname) ? DataManager.ToString(row[importer.Lastname]) : string.Empty;

        //                // Checking existence by email, first name, last name.
        //                if (DataAccess.IsExistsUsers(user.Email, user.FirstName, user.LastName, user.SiteId))
        //                {
        //                    importer.Messages.Add(String.Format(Resources.Resources.UserIsDuplicateSoSkipped, user.FirstName, user.LastName));
        //                    UpdateImportingPercent(importer, i, n);
        //                    continue;
        //                }

        //                newCustomerTypeId = 0;
        //                countryString = string.Empty;
        //                customerTypeString = string.Empty;

        //                if (importer.Table.Columns.Contains(importer.Gender))
        //                {
        //                    user.Gender = GetGenderId(DataManager.ToString(row[importer.Gender]));
        //                }

        //                user.Birthdate = importer.Table.Columns.Contains(importer.Birthdate) ? (row[importer.Birthdate] is DateTime? ? (DateTime?)row[importer.Birthdate] : DataManager.ToDateTime(row[importer.Birthdate], "d/M/yyyy")) : null;

        //                user.Remarks = importer.Table.Columns.Contains(importer.Description) ? DataManager.ToString(row[importer.Description]) : string.Empty;
        //                user.Career = importer.Table.Columns.Contains(importer.Career) ? DataManager.ToString(row[importer.Career]) : string.Empty;
        //                user.LicenseNumber = importer.Table.Columns.Contains(importer.LicenseNumber) ? DataManager.ToString(row[importer.LicenseNumber]) : string.Empty;
        //                user.Index = importer.Table.Columns.Contains(importer.Index) ? DataManager.ToFloat(row[importer.Index]) : 0f;
        //                user.CustomField1 = importer.Table.Columns.Contains(importer.CustomField1) ? DataManager.ToString(row[importer.CustomField1]) : string.Empty;
        //                user.CustomField2 = importer.Table.Columns.Contains(importer.CustomField2) ? DataManager.ToString(row[importer.CustomField2]) : string.Empty;
        //                user.CustomField3 = importer.Table.Columns.Contains(importer.CustomField3) ? DataManager.ToString(row[importer.CustomField3]) : string.Empty;

        //                #region CustomerType & SubCustomerType
        //                if (importer.CustomerType != "none" && importer.Table.Columns.Contains(importer.CustomerType))
        //                {
        //                    customerTypeString = DataManager.ToString(row[importer.CustomerType]);
        //                    if (!customerTypeIdookupTable.Keys.Contains(customerTypeString))
        //                    {
        //                        newCustomerType = new CustomerType()
        //                        {
        //                            SiteId = importer.SiteId,
        //                            CreatedDate = now,
        //                            UpdatedDate = now,
        //                            Active = true
        //                        };
        //                        newCustomerTypeId = DataAccess.AddCustomerType(newCustomerType);
        //                        if (newCustomerTypeId > 0)
        //                        {
        //                            newCustomerTypeLang = new CustomerTypeLang()
        //                            {
        //                                CustomerTypeId = newCustomerTypeId,
        //                                CustomerTypeName = customerTypeString,
        //                                LangId = 1
        //                            };
        //                            DataAccess.SaveCustomerTypeLang(newCustomerTypeLang);
        //                            customerTypeIdookupTable.Add(customerTypeString, newCustomerTypeId);
        //                            user.CustomerTypeId = customerTypeIdookupTable[customerTypeString];
        //                        }
        //                    }
        //                    else
        //                    {
        //                        user.CustomerTypeId = newCustomerTypeId = customerTypeIdookupTable[customerTypeString];
        //                    }

        //                    // Insert Sub Customer Type
        //                    if (importer.SubCustomerType != "none" && importer.Table.Columns.Contains(importer.SubCustomerType))
        //                    {
        //                        customerTypeString = DataManager.ToString(row[importer.SubCustomerType]);

        //                        if (!customerTypeIdookupTable.Keys.Contains(customerTypeString))
        //                        {
        //                            newCustomerType = new CustomerType()
        //                            {
        //                                ParentId = newCustomerTypeId,
        //                                SiteId = importer.SiteId,
        //                                CreatedDate = now,
        //                                UpdatedDate = now,
        //                                Active = true
        //                            };
        //                            newCustomerTypeId = DataAccess.AddCustomerType(newCustomerType);
        //                            if (newCustomerTypeId > 0)
        //                            {
        //                                newCustomerTypeLang = new CustomerTypeLang()
        //                                {
        //                                    CustomerTypeId = newCustomerTypeId,
        //                                    CustomerTypeName = customerTypeString,
        //                                    LangId = 1
        //                                };
        //                                DataAccess.SaveCustomerTypeLang(newCustomerTypeLang);
        //                                customerTypeIdookupTable.Add(customerTypeString, newCustomerTypeId);
        //                                user.SubCustomerTypeId = customerTypeIdookupTable[customerTypeString];
        //                            }
        //                        }
        //                        else
        //                        {
        //                            user.SubCustomerTypeId = customerTypeIdookupTable[customerTypeString];
        //                        }
        //                    }
        //                }
        //                #endregion

        //                user.Address = user.ShippingAddress = importer.Table.Columns.Contains(importer.Address) ? DataManager.ToString(row[importer.Address]) : string.Empty;
        //                //user.Street = user.ShippingStreet = DataManager.ToString(Request.Form["Street"]);
        //                user.PostalCode = user.ShippingPostalCode = importer.Table.Columns.Contains(importer.PostalCode) ? DataManager.ToString(row[importer.PostalCode]) : string.Empty;
        //                user.City = user.ShippingCity = importer.Table.Columns.Contains(importer.City) ? DataManager.ToString(row[importer.City]) : string.Empty;
        //                user.Phone = user.ShippingPhone = importer.Table.Columns.Contains(importer.Phone) ? DataManager.ToString(row[importer.Phone]) : string.Empty;
        //                //user.PhoneCountryCode = DataManager.ToString(Request.Form["PhoneCountryCode"]).Trim();
        //                user.MobilePhone = importer.Table.Columns.Contains(importer.Mobile) ? DataManager.ToString(row[importer.Mobile]) : string.Empty;
        //                //user.MobilePhoneCountryCode = DataManager.ToString(Request.Form["MobilePhoneCountryCode"]).Trim();
        //                if (importer.Table.Columns.Contains(importer.Country))
        //                {
        //                    countryString = DataManager.ToString(row[importer.Country]);
        //                    user.CountryId = user.ShippingCountryId = countryIdLookupTable.Keys.Contains(countryString) ? countryIdLookupTable[countryString] : 1;
        //                }

        //                user.IsSubscriber = true;
        //                user.IsReceiveEmailInfo = true;
        //                user.UpdateDate = now;
        //                user.InsertDate = user.UpdateDate;
        //                user.RegisteredDate = user.UpdateDate;
        //                user.LastLoggedOn = user.UpdateDate;
        //                user.ExpiredDate = user.UpdateDate.Value.AddYears(100);
        //                user.Active = true;

        //                user.UserId = DataAccess.AddUser(user);

        //                #region CustomerGroup
        //                if (importer.CustomerGroupIds != null && importer.CustomerGroupIds.Any())
        //                {
        //                    foreach (long customerGroupId in importer.CustomerGroupIds)
        //                    {
        //                        DataAccess.AddCustomerGroupCustomer(customerGroupId, user.UserId);
        //                    }
        //                }
        //                #endregion

        //                // Update progress percent.
        //                UpdateImportingPercent(importer, i, n);
        //                SaveCustomerImporter(importer);
        //            }

        //            importer.IsCompleted = true;
        //            SaveCustomerImporter(importer);
        //        }
        //        catch (Exception ex)
        //        {
        //            importer.IsCompleted = false;
        //            importer.IsError = true;
        //            importer.ErrorMessage = ex.Message;
        //            e.Cancel = true;
        //        }
        //        finally
        //        {
        //            SaveCustomerImporter(importer);
        //        }
        //    }
        //} 
        #endregion

        #region UpdateImportingPercent
        private static void UpdateImportingPercent(CustomerImporter importer, int i, int n)
        {
            importer.Percent = (i + 1) * 100 / n;
        }
        #endregion

        private int GetGenderId(string genderText)
        {
            string[] maleTaxonomies = new string[] { "man", "male", "masculin" };
            return maleTaxonomies.Contains(genderText.ToLower()) ? 0 : 1;
        }
        #endregion

        #region Class : CustomerImporter
        private class CustomerImporter
        {
            public DataTable Table;
            public bool IsCompleted = false;
            public bool IsError = false;
            public string ErrorMessage;
            public int Percent = 0;

            public long SiteId;
            public long[] CustomerGroupIds;
            public string Email;
            public string Firstname;
            public string Lastname;
            public string Birthdate;
            public string Gender;
            public string Phone;
            public string Mobile;
            public string Address;
            public string City;
            public string Country;
            public string PostalCode;
            public string Description;
            public string Career;
            public string LicenseNumber;
            public string Index;
            public string CustomerType;
            public string SubCustomerType;
            public string CustomField1;
            public string CustomField2;
            public string CustomField3;

            public List<string> Messages = new List<string>();
            public List<string> MessageTypes = new List<string>();

            public void Dispose()
            {
                Table.Dispose();
                Table = null;
                SiteId = 0;
                Email = string.Empty;
                Firstname = string.Empty;
                Lastname = string.Empty;
                Birthdate = string.Empty;
                Gender = string.Empty;
                Phone = string.Empty;
                Mobile = string.Empty;
                Address = string.Empty;
                City = string.Empty;
                Country = string.Empty;
                PostalCode = string.Empty;
                Description = string.Empty;
                Career = string.Empty;
                LicenseNumber = string.Empty;
                Index = string.Empty;
                CustomerType = string.Empty;
                SubCustomerType = string.Empty;
                CustomField1 = string.Empty;
                CustomField2 = string.Empty;
                CustomField3 = string.Empty;
            }
        }
        #endregion
    }
}