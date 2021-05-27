using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NLog;
using System.Runtime.Caching;

namespace DansLesGolfs.Controllers
{
    public class BaseController : Controller
    {
        #region Fields
        protected int CultureId;
        private string systemBodyClasses;
        protected Logger logger = null;
        #endregion

        #region Properties
        protected SqlDataAccess DataAccess { get; set; }
        protected AlbatrosDataAccess Albatros { get; set; }
        protected AuthHelper Auth { get; set; }
        protected Dictionary<string, string> Options { get; set; }
        #endregion

        #region Constructor
        public BaseController()
            : base()
        {
            Initialize();
            logger = LogManager.GetCurrentClassLogger();
        }
        #endregion

        #region Private Methods
        #region Initialize
        private void Initialize()
        {
            DataAccess = DataFactory.GetInstance();
            Albatros = DataFactory.GetAlbatrosInstance();
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
                if(Auth.User.UserTypeId == UserTypes.Type.SuperAdmin || Auth.User.UserTypeId == UserTypes.Type.Admin)
                {
                    AddSystemBodyClass("admin-bar");
                }
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
            ViewBag.CurrentCulture = currentCulture != null ? currentCulture.Value : CultureHelper.Cultures.Length > 0 ? CultureHelper.Cultures.First() : "fr-FR";
        }
        #endregion

        private Dictionary<string, string> GetPersonalizeData()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            if (Auth.User != null)
            {
                data.Add("{!name}", Auth.User.FullName);
                data.Add("{!firstname}", Auth.User.FirstName);
                data.Add("{!lastname}", Auth.User.LastName);
                data.Add("{!email}", Auth.User.Email);
                data.Add("{!phone}", Auth.User.Phone);
                data.Add("{!mobile}", Auth.User.MobilePhone);
            }
            else
            {
                data.Add("{!name}", string.Empty);
                data.Add("{!firstname}", string.Empty);
                data.Add("{!lastname}", string.Empty);
                data.Add("{!email}", string.Empty);
                data.Add("{!phone}", string.Empty);
                data.Add("{!mobile}", string.Empty);
            }
            data.Add("{!order_number}", string.Empty);
            data.Add("{!order_subtotal}", string.Empty);
            data.Add("{!order_date}", string.Empty);
            data.Add("{!order_table}", string.Empty);
            return data;
        }


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

        protected List<SelectListItem> ListToDropDownList<T>(List<T> objectList, string key, string value, long? selectedValue = null, int defaultValue = 0)
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
        protected void AddSystemBodyClass(string className)
        {
            systemBodyClasses += " " + className;
            systemBodyClasses = systemBodyClasses.Trim();
            ViewBag.SystemBodyClasses = systemBodyClasses;
        }

        protected string PersonalizeText(string text, Dictionary<string, string> additionData = null)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrWhiteSpace(text))
                return string.Empty;

            string result = text;
            Dictionary<string, string> data = GetPersonalizeData();
            if(additionData != null)
            {
                foreach(var entry in additionData)
                {
                    if(data.ContainsKey(entry.Key))
                    {
                        data[entry.Key] = entry.Value;
                    }
                    else
                    {
                        data.Add(entry.Key, entry.Value);
                    }
                }
            }
            foreach(var entry in data)
            {
                result = result.Replace(entry.Key, entry.Value);
            }
            return result;
        }
        #endregion
    }
}
