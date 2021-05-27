using DansLesGolfs.Base;
using DansLesGolfs.Base.Services;
using DansLesGolfs.BLL;
using DansLesGolfs.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.ECM.Controllers
{
    public class BaseController : Controller
    {
        #region Fields
        protected int CultureId;
        protected Logger logger = LogManager.GetCurrentClassLogger();
        protected ACLHelper acl = new ACLHelper();
        #endregion

        #region Properties
        protected SqlDataAccess DataAccess { get; set; }
        protected AuthHelper Auth { get; set; }
        #endregion

        #region Constructor
        public BaseController()
            : base()
        {
            Initialize();
        }
        #endregion

        #region Private Methods
        #region Initialize
        private void Initialize()
        {
            DataAccess = DataFactory.GetInstance();
            Auth = new AuthHelper();
            InitCulture();
            InitLogonViewBags();
        }

        private void InitLogonViewBags()
        {
            if (Auth.Check())
            {
                ViewBag.LogonUserID = Auth.User.UserId;
                ViewBag.LogonUserFirstName = Auth.User.FirstName;
                ViewBag.LogonUserMiddleName = Auth.User.MiddleName;
                ViewBag.LogonUserLastName = Auth.User.LastName;
                ViewBag.LogonUserType = Auth.User.UserTypeId;
                ViewBag.LogonSiteId = Auth.User.SiteId;
            }
            else
            {
                ViewBag.LogonUserID = 0;
                ViewBag.LogonUserFirstName = string.Empty;
                ViewBag.LogonUserMiddleName = string.Empty;
                ViewBag.LogonUserLastName = string.Empty;
                ViewBag.LogonUserType = -1;
                ViewBag.LogonSiteId = 0;
            }

            if (MemoryCache.Default["WebsiteName"] == null)
            {
                string websiteName = System.Configuration.ConfigurationManager.AppSettings["SiteName"].ToString();
                ViewBag.WebsiteName = websiteName;
                MemoryCache.Default.Add("WebsiteName", websiteName, DateTime.Now.AddDays(1));
            }
            else
            {
                ViewBag.WebsiteName = MemoryCache.Default["WebsiteName"].ToString();
            }

            if (MemoryCache.Default["DefaultLatitude"] == null)
            {
                string defaultLatitude = System.Configuration.ConfigurationManager.AppSettings["DefaultLatitude"].ToString();
                ViewBag.DefaultLatitude = defaultLatitude;
                MemoryCache.Default.Add("DefaultLatitude", defaultLatitude, DateTime.Now.AddDays(1));
            }
            else
            {
                ViewBag.DefaultLatitude = MemoryCache.Default["DefaultLatitude"].ToString();
            }

            if (MemoryCache.Default["DefaultLongitude"] == null)
            {
                string defaultLongitude = System.Configuration.ConfigurationManager.AppSettings["DefaultLongitude"].ToString();
                ViewBag.DefaultLongitude = defaultLongitude;
                MemoryCache.Default.Add("DefaultLongitude", defaultLongitude, DateTime.Now.AddDays(1));
            }
            else
            {
                ViewBag.DefaultLongitude = MemoryCache.Default["DefaultLongitude"].ToString();
            }

            if (MemoryCache.Default["GoogleAPIKey"] == null)
            {
                string googleAPIKey = System.Configuration.ConfigurationManager.AppSettings["GoogleAPIKey"].ToString();
                ViewBag.GoogleAPIKey = googleAPIKey;
                MemoryCache.Default.Add("GoogleAPIKey", googleAPIKey, DateTime.Now.AddDays(1));
            }
            else
            {
                ViewBag.GoogleAPIKey = MemoryCache.Default["GoogleAPIKey"].ToString();
            }
        }
        #endregion

        #region InitCulture
        private void InitCulture()
        {
            string cultureName = null;

            HttpRequest request = System.Web.HttpContext.Current.Request;

            HttpCookie currentCulture = request.Cookies["_culture"];
            HttpCookie currentCultureId = request.Cookies["_cultureId"];
            if (currentCulture != null)
            {
                cultureName = currentCulture.Value;
            }
            else
            {
                cultureName = request.UserLanguages != null && request.UserLanguages.Any() ? request.UserLanguages[0] : null;
            }
            cultureName = CultureHelper.GetImplementedCulture(cultureName);
            CultureHelper.ChangeCulture(cultureName);
            ViewBag.CurrentCultureId = this.CultureId = (currentCultureId != null ? DataManager.ToInt(currentCultureId.Value, 1) : 1);
            ViewBag.CurrentCulture = currentCulture != null ? currentCulture.Value : CultureHelper.Cultures.Length > 0 ? CultureHelper.Cultures.First() : "en-US";
        }
        #endregion
        #endregion

        #region Protected Methods
        /// <summary>
        /// Translate given text into locale text.
        /// </summary>
        /// <param name="text">Text that you want to translate.</param>
        /// <returns>Translated text in current locale.</returns>
        protected string _T(string text)
        {
            return text;
        }

        protected List<SelectListItem> EnumToDropDownList<T>(int? selectedValue = null, int defaultValue = 0)
        {
            selectedValue = selectedValue.HasValue ? selectedValue : defaultValue;
            Dictionary<int, string> data = DataManager.EnumToKeyValuePairs<T>();
            SelectListItem item = null;
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var entry in data)
            {
                item = new SelectListItem();
                item.Text = entry.Value;
                item.Value = entry.Key.ToString();
                item.Selected = selectedValue.Value == entry.Key ? true : false;
                list.Add(item);
            }
            return list;
        }

        protected List<SelectListItem> ListToDropDownList<T>(List<T> objectList, string key, string value, int? selectedValue = null, int defaultValue = 0)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            if (objectList != null && objectList.Any())
            {
                SelectListItem item = null;
                object val = null;
                Type type = objectList.First().GetType();
                PropertyInfo pikey = type.GetProperty(key);
                PropertyInfo pivalue = type.GetProperty(value);
                selectedValue = selectedValue.HasValue ? selectedValue : defaultValue;

                foreach (var it in objectList)
                {
                    item = new SelectListItem();

                    val = pivalue.GetValue(it, null);
                    item.Text = val != null ? val.ToString() : string.Empty;

                    val = pikey.GetValue(it, null);
                    if (val != null)
                    {
                        item.Value = val.ToString();
                        item.Selected = selectedValue.Value.ToString() == val.ToString() ? true : false;
                    }
                    list.Add(item);
                }
                var result = list.Where(it => it.Value == val.ToString());
                if (result != null && result.Any())
                {
                    result.First().Selected = true;
                }
                else
                {
                    list.First().Selected = true;
                }
            }
            return list;
        }

        /// <summary>
        /// Get Rendered HTML from specific view.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewpath">Path to view file.</param>
        /// <param name="model">model object that you want to put</param>
        /// <returns></returns>
        protected string GetHTMLFromView(string viewpath, object model)
        {
            ViewData.Model = model;
            string html = string.Empty;
            using (var writer = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, viewpath);
                var viewContext = new ViewContext(this.ControllerContext, viewResult.View, ViewData, TempData, writer);
                viewContext.View.Render(viewContext, writer);
                viewResult.ViewEngine.ReleaseView(this.ControllerContext, viewResult.View);
                html = writer.ToString();
            }
            return html;
        }

        private List<SelectListItem> LoadDropDownListCache(string cacheName, Func<List<SelectListItem>> loadFunction, object value = null)
        {
            if (loadFunction == null)
                return new List<SelectListItem>();

            if (MemoryCache.Default[cacheName] != null)
            {
                List<SelectListItem> list = MemoryCache.Default[cacheName] as List<SelectListItem>;
                if (list != null && value != null)
                {
                    list.Where(it => it.Value == value.ToString()).ToList().ForEach(it => it.Selected = true);
                }
                return list;
            }
            else
            {
                List<SelectListItem> list = loadFunction();
                MemoryCache.Default.Add(cacheName, list, DateTime.Now.AddMinutes(20));
                return list;
            }
        }

        private List<SelectListItem> GetDepartureMonths(string selectedMonth)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem()
            {
                Text = Resources.Resources.SelectMonth,
                Value = "0",
                Selected = (string.IsNullOrEmpty(selectedMonth) || string.IsNullOrWhiteSpace(selectedMonth))
            });
            for (DateTime i = DateTime.Today, j = DateTime.Today.AddYears(2); i < j; i = i.AddMonths(1))
            {
                items.Add(new SelectListItem()
                {
                    Text = i.ToString("MMMM yyyy"),
                    Value = i.ToString("yyyyMM"),
                    Selected = (i.ToString("yyyyMM") == selectedMonth)
                });
            }
            return items;
        }

        #region GetTimeSlotsDropDownList
        private List<SelectListItem> GetTimeSlotsDropDownList(int? timeSlot = null)
        {
            List<SelectListItem> timeSlots = new List<SelectListItem>();
            timeSlots.Add(new SelectListItem()
            {
                Text = Resources.Resources.TimeSlot,
                Value = "0",
                Selected = timeSlot == null
            });
            for (int i = 8; i < 20; i++)
            {
                timeSlots.Add(new SelectListItem()
                {
                    Text = i.ToString("00") + "h00-" + (i + 1).ToString("00") + "h00",
                    Value = i.ToString(),
                    Selected = timeSlot == i
                });
            }
            return timeSlots;
        }
        #endregion

        #region AddAllowedUserType
        protected void AddAllowedUserType(int userType)
        {
            acl.Add(userType);
        }
        #endregion

        #region CheckAllowedUserType
        /// <summary>
        /// Check allowed user types.
        /// </summary>
        /// <returns></returns>
        protected bool CheckAllowedUserType()
        {
            if (Auth.User == null || !acl.Check(Auth.User.UserTypeId))
                return false;

            return true;
        }
        #endregion
        #endregion

        #region Overriden Methods
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            //if (!CheckAllowedUserType())
            //    Response.Redirect("~/Error/NotFound");
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            return base.BeginExecuteCore(callback, state);
        }
        #endregion
    }
}
