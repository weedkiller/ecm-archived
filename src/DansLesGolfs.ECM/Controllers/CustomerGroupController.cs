using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DansLesGolfs.ECM.Controllers
{
    public class CustomerGroupController : BaseECMCRUDController<CustomerGroup>
    {
        #region Constructor
        public CustomerGroupController()
        {
            ObjectName = "CustomerGroup";
            TitleName = Resources.Resources.CustomerGroup;
            PrimaryKey = "CustomerGroupId";

            // Define Column Names.
            ColumnNames.Add("CustomerGroupName", Resources.Resources.CustomerGroupName);

            AddAllowedUserType(UserType.Type.Admin);
            AddAllowedUserType(UserType.Type.Staff);
        }
        #endregion

        #region Overriden Methods
        protected override void DoSetGridColumns(GridColumnFactory<CustomerGroup> columns)
        {
            columns.Bound(it => it.CustomerGroupName).Title(Resources.Resources.CustomerGroupName);
            columns.Bound(it => it.CustomerGroupId).Title("Sync")
                .ClientTemplate("<a class=\"sync-customer-group-button\" href=\"javascript:void(0);\" data-id=\"#:CustomerGroupId#\" data-site-id=\"#:SiteId#\">Sync</a>")
                .Sortable(false)
                .Filterable(false)
                .Groupable(false)
                .Width(100);
        }
        protected override IQueryable<CustomerGroup> DoLoadDataJSON()
        {
            if (Auth.User.UserTypeId == UserType.Type.SiteManager || Auth.User.UserTypeId == UserType.Type.Staff)
            {
                return DataAccess.GetListCustomerGroups(Auth.User.SiteId);
            }
            else
            {
                return DataAccess.GetListCustomerGroups();
            }
        }

        public JsonResult LoadContactListJSON([DataSourceRequest]DataSourceRequest request, long? siteId)
        {
            try
            {
                IQueryable<UserViewModel> customers = null;

                if (Auth.User.UserTypeId == UserType.Type.SiteManager || Auth.User.UserTypeId == UserType.Type.Staff)
                {
                    customers = DataAccess.GetListECMSubscribers(Auth.User.SiteId, 1);
                }
                else
                {
                    customers = DataAccess.GetListECMSubscribers(siteId, 1);
                }

                return Json(customers.ToDataSourceResult(request));
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
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
                row.Add(c.Gender);
                row.Add(c.Birthdate.HasValue ? c.Birthdate.Value.ToString("d/M/yyyy") : "-");
                row.Add(c.CustomerTypeName);
                row.Add(c.SubCustomerTypeName);
                row.Add(c.LicenseNumber);
                row.Add(c.Index);
                row.Add("<a href=\"javascript:void(0);\" data-id=\"" + c.UserId + "\" data-firstname=\"" + c.FirstName + "\" data-lastname=\"" + c.LastName + "\" data-fullname=\"" + c.FullName + "\" data-email=\"" + c.Email + "\" data-customertype=\"" + c.CustomerTypeName + "\" data-subcustomertype=\"" + c.SubCustomerTypeName + "\" class=\"select-link\">Select</a>");
                list.Add(row.ToArray());
            }
            return list.ToArray();
        }

        protected override object DoPrepareNew()
        {
            return new
            {
                CustomerGroupName = string.Empty,
                Active = false,
                ContentType = 0,
                SiteId = 0,
                AutoSync = true
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
            CustomerGroup model = DataAccess.GetCustomerGroup(id);
            //List<User> customers = DataAccess.GetCustomerInCustomerGroups(id.ToString());
            //ViewBag.CustomerLists = customers;

            var selectedCustomerTypeIds = DataAccess.GetCustomerTypeIdsBySiteId(id);
            ViewBag.SelectedCustomerTypeIds = selectedCustomerTypeIds.Count > 0 ? String.Join(",", selectedCustomerTypeIds) : String.Empty;

            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            long id = DataManager.ToLong(Request.Form["id"], -1);
            CustomerGroup model = null;
            if (id > 0)
            {
                model = DataAccess.GetCustomerGroup(id);
                if (model == null)
                {
                    model = new CustomerGroup();
                }
            }
            else
            {
                model = new CustomerGroup();
            }
            model.CustomerGroupName = DataManager.ToString(Request.Form["CustomerGroupName"]);
            model.AutoSync = DataManager.ToBoolean(Request.Form["AutoSync"], true);

            if (Auth.User.UserTypeId == UserType.Type.SuperAdmin || Auth.User.UserTypeId == UserType.Type.Admin)
            {
                model.SiteId = DataManager.ToInt(Request.Form["SiteId"]);
            }
            else
            {
                model.SiteId = Auth.User.SiteId;
            }

            CustomerGroup obj = DataAccess.GetCustomerGroup(model.CustomerGroupId);
            if (id > 0)
            {
                DataAccess.UpdateCustomerGroup(model);
                result = model.CustomerGroupId;
            }
            else
            {
                model.Active = true;
                result = DataAccess.AddCustomerGroup(model);
                model.CustomerGroupId = result;
            }
            ViewBag.id = result > -1 ? model.CustomerGroupId : -1;
            return result > 0;
        }

        protected override void DoSaveSuccess(int id)
        {
            base.DoSaveSuccess(id);

            string[] CustomerTypes = Request.Form.GetValues("CustomerTypes");

            DataAccess.DeleteAllCustomerGroupsCustomerTypes(id);
            if(CustomerTypes != null)
            {
                foreach (var c in CustomerTypes)
                {
                    DataAccess.SaveCustomerGroupCustomerType(id, DataManager.ToLong(c));
                }
            }
        }

        protected override bool DoDelete(long id)
        {
            return DataAccess.DeleteCustomerGroup(id) > 0;
        }
        #endregion

        [HttpPost]
        public ActionResult AjaxGetCustomersInCustomerGroup([DataSourceRequest]DataSourceRequest request, string id)
        {
            var customers = DataAccess.GetListCustomerInCustomerGroups(id, 1);
            DataSourceResult result = customers.ToDataSourceResult(request);
            return Json(result);
        }

        [HttpPost]
        public ActionResult AjaxGetCustomersNotInGroup([DataSourceRequest]DataSourceRequest request, string id, long? siteId, string mode)
        {
            var customers = DataAccess.GetListCustomerInCustomerGroups(id, 1, true);
            if(customers == null)
            {
                return new EmptyResult();
            }

            if (siteId.HasValue)
            {
                customers = customers.Where(it => it.SiteId == siteId.Value);
            }
            DataSourceResult result = customers.ToDataSourceResult(request);

            if (mode == "save")
            {
                #region Saving Delegate
                Action savingBgJob = () =>
                       {
                           request.PageSize = 0;
                           DataSourceResult resultSaving = customers.ToDataSourceResult(request, it => new UserViewModel
                           {
                               UserId = it.UserId
                           });
                           List<UserViewModel> data = resultSaving.Data as List<UserViewModel>;
                           IEnumerable<long> userIds = data.Select(it => it.UserId);
                           long customerGroupId = DataManager.ToLong(id);
                           foreach (var userId in userIds)
                           {
                               DataAccess.AddCustomerGroupCustomer(customerGroupId, userId);
                           }
                       };
                Task task = new Task(savingBgJob);
                task.Start();
                #endregion

                //System.ComponentModel.BackgroundWorker bg = new System.ComponentModel.BackgroundWorker();
                //bg.DoWork += new DoWorkEventHandler(savingBgJob);
                //string cacheKeyName = "AddCustomerIntoGroupBGJob_" + "_" + id + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss");
                //MemoryCache.Default.Add(cacheKeyName, bg, DateTime.Now.AddHours(1));
                //bg.RunWorkerAsync();
            }

            return Json(result);
        }

        [HttpPost]
        public ActionResult AjaxAddCustomersIntoCustomerGroup(long? id, long[] userIds)
        {
            try
            {
                foreach (var userId in userIds)
                {
                    DataAccess.AddCustomerGroupCustomer(id.Value, userId);
                }
                return Json(new
                {
                    isSuccess = true
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

        public ActionResult AjaxGetCustomerTypes(long id = 0, bool refresh = false)
        {
            if (id <= 0)
                return Json(new
                {
                    success = false,
                    message = "Please specify customer site id."
                }, JsonRequestBehavior.AllowGet);

            try
            {
                DataAccess.SyncCustomerTypes(id);
            }
            catch
            {
                // Do nothing.
            }

            var types = DataAccess.GetCustomerTypesBySiteId(id);
            if(types.Any())
            {
                types = types.OrderBy(it => it.CustomerTypeName).ToList();
            }
            List<Object> data = new List<object>();
            foreach (var type in types)
            {
                data.Add(new
                {
                    id = type.CustomerTypeId,
                    name = type.CustomerTypeName
                });
            }

            return Json(new
            {
                success = true,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxSyncCustomerGroups(long id = 0, long siteId = 0)
        {
            if (id <= 0)
                return Json(new
                {
                    success = false,
                    message = "Please specify customer group id."
                }, JsonRequestBehavior.AllowGet);

            if (siteId <= 0)
                return Json(new
                {
                    success = false,
                    message = "Please specify site id."
                }, JsonRequestBehavior.AllowGet);

            Task task = new Task(() => {
                try
                {
                    //DataAccess.SyncCustomerTypes(siteId);
                    DataAccess.SyncCustomerGroup(id);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            });
            task.Start();

            return Json(new
            {
                success = true
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
