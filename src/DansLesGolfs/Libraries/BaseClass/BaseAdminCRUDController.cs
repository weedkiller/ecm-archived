using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DansLesGolfs.Controllers
{
    public class BaseAdminCRUDController : BaseAdminController
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
        #endregion

        #region Constructors
        public BaseAdminCRUDController()
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
            ViewBag.Title = !String.IsNullOrEmpty(TitleName) ? TitleName : "Admin Dashboard";
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
            Breadcrumbs.Add("Add New " + TitleName, "~/" + AreaName + "/" + ObjectName + "/Form");

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
                Breadcrumbs.Add("Edit " + TitleName, "~/" + AreaName + "/" + ObjectName + "/Form/" + id);
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
                    row.Add("<input type=\"checkbox\" value=\"" + pkid + "\" />");
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
                                    row.Add(RowCellDisplayText(column.Key, "<a href=\"" + Url.Content("~/" + AreaName + "/" + ObjectName + "/Form/" + pkid) + "\" class=\"edit-link\">" + value + "</a>", it));
                                }
                            }
                            else
                            {
                                row.Add(RowCellDisplayText(column.Key, value, it));
                            }
                        }
                        i++;
                    }
                    if (ObjectName == "DlgCard")
                    {
                        row.Add("<a href=\"" + Url.Content("~/" + AreaName + "/DlgCard/" + "EditDlgCardAdmin/?itemId=" + pkid) + "\" class=\"edit-link\">Edit</a>");
                        row.Add("<a href=\"javascript:void(0);\" data-id=\"" + pkid + "\" class=\"delete-link\">Delete</a>");
                    }
                    else if (ObjectName == "Promotion")
                    {
                        row.Add("<a href=\"" + Url.Content("~/" + AreaName + "/PromotionAdmin/" + "EditPromotion/?promotionId=" + pkid) + "\" class=\"edit-link\">Edit</a>");
                        row.Add("<a href=\"javascript:void(0);\" data-id=\"" + pkid + "\" class=\"delete-link\">Delete</a>");
                    }
                    else
                    {
                        if (IsClonable)
                        {
                            row.Add("<a href=\"" + Url.Content("~/" + AreaName + "/" + ObjectName + "/Clone/" + pkid) + "\" class=\"duplicate-link\" target=\"_blank\">" + Resources.Resources.Duplicate + "</a>");
                        }
                        row.Add("<a href=\"" + Url.Content("~/" + AreaName + "/" + ObjectName + "/Form/" + pkid) + "\" class=\"edit-link\">Edit</a>");
                        row.Add("<a href=\"javascript:void(0);\" data-id=\"" + pkid + "\" class=\"delete-link\">Delete</a>");
                    }


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
            param.search = Request.QueryString["search[value]"];
            if (Request.QueryString["order[0][dir]"] != null && Request.QueryString["order[0][column]"] != null)
            {
                param.iSortingCols = DataManager.ToInt(Request.QueryString["order[0][column]"]);
                if (param.iSortingCols > 0)
                {
                    param.order = ColumnNames.ElementAt(param.iSortingCols - 1).Key + " " + Request.QueryString["order[0][dir]"].ToUpper();
                }
            }
        }
        #endregion
        #endregion

        #region Action Methods
        [HttpGet]
        public ActionResult Index()
        {
            // Initialize Important Variables.
            Init();

            // Set Breadcrumbs Navigation.
            Breadcrumbs.Clear();
            Breadcrumbs.Add(TitleName, "~/" + AreaName + "/" + ObjectName);
            ViewBag.Breadcrumbs = Breadcrumbs;

            if (ObjectName == "DlgCard")
            {
                return RedirectToAction("DisplayDlgCardList", "DlgCard");
            }
            else if (ObjectName == "Promotion")
            {
                return RedirectToAction("PromotionList", "PromotionAdmin");
            }
            else
            {
                return View("~/Areas/" + AreaName + "/Views/CRUD/List.cshtml");
            }
        }

        [HttpGet]
        public JsonResult LoadDataJSON(jQueryDataTableParamModel param)
        {
            try
            {
                PrepareListParams(param);
                IEnumerable<object> result = DoLoadDataJSON(param);

                if (result != null)
                {
                    object[] resultArray = ConvertIEnumerableToArray(result);
                    int totalRecords = resultArray.Count();

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

        [HttpGet]
        public ActionResult Form(int? id)
        {
            InitializeForm(id);
            return View();
        }

        private void InitializeForm(int? id)
        {
            // Initialize Important Variables.
            Init();

            // Set Breadcrumbs Navigation.
            Breadcrumbs.Add(TitleName, "~/" + AreaName + "/" + ObjectName);

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
        public JsonResult Delete(int[] ids)
        {
            bool result = false;
            int failedID = -1;

            try
            {
                foreach (int id in ids)
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

            InitializeForm(id);
            ViewData["id"] = 0;
            ViewData[PrimaryKey] = 0;
            DoClone(id.Value);
            TempData["TempViewData"] = ViewData;

            return RedirectToAction("Form");
        }
        #endregion

        #region Override Methods
        protected virtual IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            return null;
        }

        protected virtual bool DoDelete(int id)
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