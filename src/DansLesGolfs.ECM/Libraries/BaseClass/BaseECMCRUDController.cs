using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;
using Kendo.Mvc.UI.Html;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.ECM.Controllers
{
    public class BaseECMCRUDController<T> : BaseAdminController where T : class
    {
        #region Fields
        private string _objectName = string.Empty;
        private string _titleName = string.Empty;
        private string _className = string.Empty;
        private string _primaryKey = string.Empty;
        private bool _isClonable = false;
        private Dictionary<string, string> _columnNames = new Dictionary<string, string>();
        private Dictionary<string, string> _actionLists = new Dictionary<string, string>();
        private List<string> _javascripts = new List<string>();
        #endregion

        #region Properties
        public bool IsClonable { get { return _isClonable; } set { _isClonable = value; } }
        public virtual string ObjectName { get; set; }
        public virtual string TitleName { get; set; }
        public virtual string ClassName { get; set; }
        public virtual string PrimaryKey { get; set; }
        public virtual string Description { get; set; }
        public virtual string AreaName
        {
            get { return "Admin"; }
        }
        public virtual Dictionary<string, string> ColumnNames
        {
            get { return _columnNames; }
            set { _columnNames = value; }
        }
        /// <summary>
        /// Add key-value pairs for actions list (List page)
        /// </summary>
        public virtual Dictionary<string, string> ActionList
        {
            get { return _actionLists; }
            set { _actionLists = value; }
        }
        public virtual List<string> JavaScripts
        {
            get { return _javascripts; }
            set { _javascripts = value; }
        }
        protected List<int> AllowUserTypes = new List<int>();
        #endregion

        #region Constructors
        public BaseECMCRUDController()
            : base()
        {
        }
        #endregion

        #region Private Methods
        protected void Init()
        {
            if (!string.IsNullOrEmpty(ObjectName))
            {
                ViewBag.ObjectName = ObjectName;
                ViewBag.ClassName = ObjectName.ToLower();
            }
            else
            {
                ViewBag.ObjectName = "Unknown";
                ViewBag.ClassName = "unknown";
            }
            ViewBag.Title = !String.IsNullOrEmpty(TitleName) ? TitleName : "ECM Dashboard";
            ViewBag.TitleName = ViewBag.Title;
            ViewBag.Description = !String.IsNullOrEmpty(Description) ? Description : string.Empty;
            ViewBag.PrimaryKey = !String.IsNullOrEmpty(PrimaryKey) ? PrimaryKey : "Id";
            ViewBag.ColumnNames = ColumnNames;
            ViewBag.ActionList = ActionList;
            ViewBag.JavaScripts = JavaScripts;
            ViewBag.IsClonable = IsClonable;

            if (TempData.Keys.Contains("SuccessMessage"))
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            if (TempData.Keys.Contains("ErrorMessages"))
            {
                ViewBag.ErrorMessages = TempData["ErrorMessages"].ToString();
            }

            InitBreadcrumbs();
        }

        private dynamic DoNew()
        {
            dynamic model;
            model = DoPrepareNew();
            ViewBag.Title = "Create " + ViewBag.Title;
            ViewBag.IsNew = true;
            ViewBag.IsEdit = false;
            //ViewBag.id = -1;
            Breadcrumbs.Add("Add New " + TitleName, "~/" + ObjectName + "/Form");

            return model;
        }

        private dynamic DoEdit(long id)
        {
            dynamic model;
            model = DoPrepareEdit(id);
            if (model != null)
            {
                ViewBag.Title = "Edit " + TitleName;
                ViewBag.IsNew = false;
                ViewBag.IsEdit = true;
                ViewBag.id = id;
                Breadcrumbs.Add("Edit " + TitleName, "~/" + ObjectName + "/Form/" + id);
            }
            else
            {
                model = DoNew();
            }
            return model;
        }

        protected void CreateViewBagFromObject(object model)
        {
            if (model != null)
            {
                Type type = model.GetType();
                foreach (PropertyInfo pi in type.GetProperties())
                {
                    ViewData[pi.Name] = pi.GetValue(model, null);
                }
            }
        }

        private void RecoveryViewBag()
        {
            foreach (string keyName in System.Web.HttpContext.Current.Request.Form.AllKeys)
            {
                ViewData[keyName] = System.Web.HttpContext.Current.Request.Form[keyName];
            }
        }
        private void SetGridColumns(GridColumnFactory<T> columns)
        {
            string checkboxHeaderTemplate = "<input type=\"checkbox\" class=\"checkbox\" class=\"checkAll\" />";
            string checkboxClientTemplate = "<input type=\"checkbox\" class=\"checkbox\" class=\"checkbox-select\" value=\"#:" + PrimaryKey + "#\" />";
            columns.Bound(PrimaryKey)
                .Title(Resources.Resources.Select)
                .HeaderTemplate(checkboxHeaderTemplate)
                .ClientTemplate(checkboxClientTemplate)
                .Sortable(false)
                .Filterable(false)
                .Groupable(false)
                .Width(30);
            DoSetGridColumns(columns);
            columns.Bound(PrimaryKey)
                .Title(Resources.Resources.Edit)
                .ClientTemplate("<a href=\"" + Url.Content("~/" + ObjectName + "/Form/") + "#:" + PrimaryKey + "#\">" + Resources.Resources.Edit + "</a>")
                .Sortable(false)
                .Filterable(false)
                .Groupable(false)
                .Width(80);
            columns.Bound(PrimaryKey)
                .Title(Resources.Resources.Delete)
                .ClientTemplate("<a class=\"delete-link\" data-id=\"#:" + PrimaryKey + "#\" href=\"javascript:void(0)\">" + Resources.Resources.Delete + "</a>")
                .Sortable(false)
                .Filterable(false)
                .Groupable(false)
                .Width(80);
        }

        private void SetGridSorting(GridSortSettingsBuilder<T> sorting)
        {
            DoSetGridSorting(sorting);
        }

        private void SetDataSorting(DataSourceSortDescriptorFactory<T> sorting)
        {
            DoSetDataSorting(sorting);
        }
        #endregion

        #region Protected Methods
        protected object[] ConvertIEnumerableToArray(IEnumerable<object> result)
        {
            if (result != null)
            {
                List<object> list = new List<object>();
                Type type = null;
                List<object> row = null;
                PropertyInfo pi = null;
                object value = null;
                int i = 0;
                foreach (var it in result)
                {
                    i = 0;
                    row = new List<object>();
                    type = it.GetType();
                    pi = type.GetProperty(PrimaryKey);
                    if (pi == null)
                        throw new Exception("Primary Key name \"" + PrimaryKey + "\" not belong in this object type.");

                    int pkid = DataManager.ToInt(pi.GetValue(it, null));
                    row.Add("<input type=\"checkbox\" class=\"checkAll\" value=\"" + pkid + "\" />");
                    foreach (var column in ColumnNames)
                    {
                        pi = type.GetProperty(column.Key);
                        if (pi == null)
                        {
                            row.Add(RowCellDisplayText(column.Key, string.Empty, it));
                        }
                        else
                        {
                            value = pi.GetValue(it, null);
                            if (i == 0)
                            {
                                if (ObjectName == "Promotion")
                                {
                                    row.Add(RowCellDisplayText(column.Key, value, it));

                                }
                                else
                                {
                                    row.Add(RowCellDisplayText(column.Key, "<a href=\"" + Url.Content("~/" + ObjectName + "/Form/" + pkid) + "\" class=\"edit-link\">" + value + "</a>", it));
                                }
                            }
                            else
                            {
                                row.Add(RowCellDisplayText(column.Key, value, it));
                            }
                        }
                        i++;
                    }

                    if (IsClonable)
                    {
                        row.Add("<a href=\"" + Url.Content("~/" + ObjectName + "/Clone/" + pkid) + "\" class=\"duplicate-link\" target=\"_blank\">" + Resources.Resources.Duplicate + "</a>");
                    }
                    row.Add("<a href=\"" + Url.Content("~/" + ObjectName + "/Form/" + pkid) + "\" class=\"edit-link\">Edit</a>");
                    row.Add("<a href=\"javascript:void(0);\" data-id=\"" + pkid + "\" class=\"delete-link\">Delete</a>");

                    list.Add(row.ToArray());
                }
                return list.ToArray();
            }
            else
            {
                return new object[0];
            }
        }

        #region PrepareListParams
        protected void PrepareListParams(jQueryDataTableParamModel param)
        {
            param.search = Request.Form["search[value]"];
            if (Request.Form["order[0][dir]"] != null && Request.Form["order[0][column]"] != null)
            {
                param.iSortingCols = DataManager.ToInt(Request.Form["order[0][column]"]);
                if (param.iSortingCols > 0)
                {
                    param.order = ColumnNames.ElementAt(param.iSortingCols - 1).Key + " " + Request.Form["order[0][dir]"].ToUpper();
                }
            }
        }
        #endregion

        #endregion

        #region Action Methods
        [HttpGet]
        public ActionResult Index()
        {
            if (Auth.User != null && AllowUserTypes != null && AllowUserTypes.Any())
            {
                if(!AllowUserTypes.Contains(Auth.User.UserTypeId))
                {
                    return Redirect("~/");
                }
            }

            HtmlHelper htmlHelper = new HtmlHelper(new ViewContext(ControllerContext, new WebFormView(ControllerContext, "CRUD"), new ViewDataDictionary(), new TempDataDictionary(), new StringWriter()), new ViewPage());
            var kendo = htmlHelper.Kendo();
            var grid = kendo.Grid<T>()
                        .Name("gridCRUD")
                        .Columns(SetGridColumns)
                        .Pageable()
                        .Sortable(SetGridSorting)
                        .Scrollable()
                        .Filterable()
                        .Selectable(s =>
                        {
                            s.Mode(GridSelectionMode.Multiple);
                            s.Type(GridSelectionType.Row);
                        })
                        .Events(events => events.DataBound("onGridDataBound"))
                        .DataSource(dataSource => dataSource
                            .Ajax()
                            .PageSize(100)
                            .Read(read => read.Action("LoadDataJSON", this.ObjectName, new { id = ViewBag.id }).Data("onGridRequestData"))
                            .Events(it =>
                            {
                                it.RequestEnd("onGridRequestEnd");
                            })
                            .Sort(SetDataSorting)
                         );

            ViewBag.CRUDGrid = grid;

            List<T> Model = new List<T>();

            // Initialize Important Variables.
            Init();

            // Set Breadcrumbs Navigation.
            Breadcrumbs.Clear();
            Breadcrumbs.Add(TitleName, "~/" + ObjectName);
            ViewBag.Breadcrumbs = Breadcrumbs;

            return View("~/Views/CRUD/List.cshtml", Model);
        }

        [HttpPost]
        public JsonResult LoadDataJSON([DataSourceRequest]DataSourceRequest request, bool? isDeleteMode = false)
        {
            if (Auth.User != null && AllowUserTypes != null && AllowUserTypes.Any())
            {
                if (!AllowUserTypes.Contains(Auth.User.UserTypeId))
                {
                    return new CorrectJsonResult()
                    {
                        Data = null,
                        MaxJsonLength = int.MaxValue
                    };
                }
            }

            DataSourceResult result = null;

            if (isDeleteMode.HasValue && isDeleteMode.Value)
            {
                Task deletionTask = new Task(() =>
                {
                    IQueryable<T> deletionData = DoLoadDataJSON();
                    Type type = typeof(T);
                    PropertyInfo active = type.GetProperty("Active");
                    PropertyInfo pk = type.GetProperty(PrimaryKey);
                    DataSourceRequest deleteRequest = new DataSourceRequest()
                    {
                        Aggregates = request.Aggregates,
                        Filters = request.Filters,
                        Groups = request.Groups,
                        Page = request.Page,
                        PageSize = 0,
                        Sorts = request.Sorts
                    };
                    DataSourceResult deletionResult = deletionData.ToDataSourceResult(deleteRequest, it => (long)pk.GetValue(it));
                    var deletionIds = deletionResult.Data as IEnumerable<long>;
                    if (deletionIds != null)
                    {
                        Delete(deletionIds.ToArray());
                    }
                });
                deletionTask.Start();
            }

            IQueryable<T> data = DoLoadDataJSON();
            result = data.ToDataSourceResult(request);

            return new CorrectJsonResult()
            {
                Data = result,
                MaxJsonLength = int.MaxValue
            };
        }

        [HttpGet]
        public ActionResult Form(int? id)
        {
            if (Auth.User != null && AllowUserTypes != null && AllowUserTypes.Any())
            {
                if (!AllowUserTypes.Contains(Auth.User.UserTypeId))
                {
                    return Redirect("~/");
                }
            }

            InitializeForm(id);
            return View();
        }

        private void InitializeForm(int? id)
        {
            // Initialize Important Variables.
            Init();

            // Set Breadcrumbs Navigation.
            Breadcrumbs.Add(TitleName, "~/" + ObjectName);

            dynamic model;
            if (id.HasValue && id.Value > 0)
            {
                model = DoEdit(id.Value);
            }
            else
            {
                model = DoNew();
            }
            CreateViewBagFromObject(model);
            DoPrepareForm(id);
            if (TempData.ContainsKey("TempViewData") && TempData["TempViewData"] != null)
            {
                this.ViewData = (ViewDataDictionary)TempData["TempViewData"];
                TempData.Remove("TempViewData");
            }
            ViewBag.Breadcrumbs = Breadcrumbs;
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Form()
        {
            if (Auth.User != null && AllowUserTypes != null && AllowUserTypes.Any())
            {
                if (!AllowUserTypes.Contains(Auth.User.UserTypeId))
                {
                    return Redirect("~/");
                }
            }

            InitializeForm(DataManager.ToInt(Request.Form["id"]));

            try
            {
                ExceptionHelper.ClearExceptions();
                DoValidateSave();

                if (ExceptionHelper.Count == 0)
                {
                    if (DoSave())
                    {
                        DoSaveSuccess((int)ViewBag.id);
                        TempData["SuccessMessage"] = String.Format(Resources.Resources.ObjectHasBeenSaved, TitleName);
                        return RedirectToAction("Form", new { id = (int)ViewBag.id });
                    }
                    else
                    {
                        RecoveryViewBag();
                        DoSaveFailed();
                        ViewBag.ErrorMessages = "Save " + TitleName + " Failed.";
                    }
                }
                else
                {
                    RecoveryViewBag();
                    DoSaveFailed();
                    string errors = string.Empty;
                    foreach (Exception ex in ExceptionHelper.Exceptions)
                    {
                        errors += ex.Message + "<br />";
                    }
                    ViewBag.ErrorMessages = errors;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                RecoveryViewBag();
                DoSaveFailed();
                ViewBag.ErrorMessages = "Save " + TitleName + " Failed. Please take a look at error message below.<br />" + ex.Message;
            }

            ViewBag.Breadcrumbs = Breadcrumbs;
            return View();
        }

        protected virtual void DoPrepareForm(int? id = null)
        {
        }

        protected virtual void DoValidateSave()
        {
        }

        protected virtual void DoSaveSuccess(int id)
        {
        }

        [ValidateInput(false)]
        protected virtual void DoSaveFailed()
        {
        }

        [HttpPost]
        public JsonResult Delete(long[] ids)
        {
            if (Auth.User != null && AllowUserTypes != null && AllowUserTypes.Any())
            {
                if (!AllowUserTypes.Contains(Auth.User.UserTypeId))
                {
                    throw new Exception("You have no permission for this action.");
                }
            }

            bool result = false;
            long failedID = -1;

            try
            {
                foreach (long id in ids)
                {
                    result = DoDelete(id);
                    if (!result)
                    {
                        failedID = id;
                    }
                }
                if (result)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Delete successful."
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Delete failed on ID " + failedID + "."
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public ActionResult Clone(int? id)
        {
            if (!id.HasValue || id.Value <= 0)
                return RedirectToAction("Form");

            if (Auth.User != null && AllowUserTypes != null && AllowUserTypes.Any())
            {
                if (!AllowUserTypes.Contains(Auth.User.UserTypeId))
                {
                    return Redirect("~/");
                }
            }


            InitializeForm(id);
            ViewData["id"] = 0;
            ViewData[PrimaryKey] = 0;
            DoClone(id.Value);
            TempData["TempViewData"] = ViewData;

            return RedirectToAction("Form");
        }
        #endregion

        #region Override Methods
        protected virtual void DoSetGridColumns(GridColumnFactory<T> columns)
        {
        }

        protected virtual void DoSetGridSorting(GridSortSettingsBuilder<T> sorting)
        {
        }

        protected virtual void DoSetDataSorting(DataSourceSortDescriptorFactory<T> sorting)
        {
        }

        protected virtual IQueryable<T> DoLoadDataJSON()
        {
            return null;
        }

        protected virtual bool DoDelete(long id)
        {
            return false;
        }

        protected virtual object DoPrepareEdit(long id)
        {
            return null;
        }

        protected virtual object DoPrepareNew()
        {
            return null;
        }

        protected virtual bool DoSave()
        {
            return false;
        }

        protected virtual void DoClone(long id)
        {

        }

        protected virtual string RowCellDisplayText(string columnName, object value, object dataItem)
        {
            return value == null ? string.Empty : value.ToString();
        }
        #endregion
    }
}