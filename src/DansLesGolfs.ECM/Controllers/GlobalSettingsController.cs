using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.ECM.Controllers
{
    public class GlobalSettingsController : BaseAdminController
    {
        #region Constructor
        public GlobalSettingsController()
        {
            ViewBag.TitleName = Resources.Resources.GlobalSettings;
            ViewBag.ClassName = "globalsettings";
        }
        #endregion

        #region Action Methods
        public ActionResult Index()
        {
            Breadcrumbs.Add(Resources.Resources.GlobalSettings, "~/GlobalSettings");
            InitBreadcrumbs();

            var options = DataAccess.GetOptions("GoogleAnalyticsID",
                "SMTPServer", "SMTPUsername", "SMTPPassword", "SMTPPort", "DefaultSenderName", "DefaultSenderEmail", "SMTPUseSSL", "SMTPUseVERP", "BouncedReturnEmail"
                , "UnsubscribeServer", "UnsubscribeEmail", "UnsubscribePassword", "UnsubscribePort", "UnsubscribeUseSSL"
                , "NetmessageFTPServer", "NetmessageFTPPort", "NetmessageFTPUsername", "NetmessageFTPPassword", "NetmessageAccountName");

            ViewBag.GoogleAnalyticsID = options["GoogleAnalyticsID"];
            ViewBag.SMTPServer = options["SMTPServer"];
            ViewBag.SMTPUsername = options["SMTPUsername"];
            ViewBag.SMTPPassword = options["SMTPPassword"];
            ViewBag.SMTPPort = options["SMTPPort"];
            ViewBag.SMTPUseSSL = options["SMTPUseSSL"];
            ViewBag.DefaultSenderName = options["DefaultSenderName"];
            ViewBag.DefaultSenderEmail = options["DefaultSenderEmail"];
            ViewBag.SMTPUseVERP = options["SMTPUseVERP"];
            ViewBag.BouncedReturnEmail = options["BouncedReturnEmail"];

            ViewBag.UnsubscribeServer = options["UnsubscribeServer"];
            ViewBag.UnsubscribeEmail = options["UnsubscribeEmail"];
            ViewBag.UnsubscribePassword = options["UnsubscribePassword"];
            ViewBag.UnsubscribePort = options["UnsubscribePort"];
            ViewBag.UnsubscribeUseSSL = options["UnsubscribeUseSSL"];

            //ViewBag.NetmessageFTPServer = options["NetmessageFTPServer"];
            //ViewBag.NetmessageFTPPort = options["NetmessageFTPPort"];
            ViewBag.NetmessageFTPUsername = options["NetmessageFTPUsername"];
            ViewBag.NetmessageFTPPassword = options["NetmessageFTPPassword"];
            ViewBag.NetmessageAccountName = options["NetmessageAccountName"];

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            if (TempData["ErrorMessages"] != null)
            {
                ViewBag.ErrorMessages = TempData["ErrorMessages"].ToString();
            }

            return View();
        }

        [HttpPost]
        public ActionResult SaveSettings()
        {
            Dictionary<string, string> options = new Dictionary<string, string>();
            try
            {
                options["GoogleAnalyticsID"] = DataManager.ToString(Request.Form["GoogleAnalyticsID"]).Trim();
                options["SMTPServer"] = DataManager.ToString(Request.Form["SMTPServer"]).Trim();
                options["SMTPUsername"] = DataManager.ToString(Request.Form["SMTPUsername"]).Trim();
                options["SMTPPassword"] = DataManager.ToString(Request.Form["SMTPPassword"]).Trim();
                options["SMTPPort"] = DataManager.ToString(Request.Form["SMTPPort"]).Trim();
                options["SMTPUseSSL"] = DataManager.ToBoolean(Request.Form["SMTPUseSSL"]).ToString();
                options["DefaultSenderName"] = DataManager.ToString(Request.Form["DefaultSenderName"]).Trim();
                options["DefaultSenderEmail"] = DataManager.ToString(Request.Form["DefaultSenderEmail"]).Trim();
                options["SMTPUseVERP"] = DataManager.ToBoolean(Request.Form["SMTPUseVERP"]).ToString();
                options["BouncedReturnEmail"] = DataManager.ToString(Request.Form["BouncedReturnEmail"]).Trim();

                options["UnsubscribeServer"] = DataManager.ToString(Request.Form["UnsubscribeServer"]).Trim();
                options["UnsubscribeEmail"] = DataManager.ToString(Request.Form["UnsubscribeEmail"]).Trim();
                options["UnsubscribePassword"] = DataManager.ToString(Request.Form["UnsubscribePassword"]).Trim();
                options["UnsubscribePort"] = DataManager.ToString(Request.Form["UnsubscribePort"]).Trim();
                options["UnsubscribeUseSSL"] = DataManager.ToBoolean(Request.Form["UnsubscribeUseSSL"]).ToString();

                //options["NetmessageFTPServer"] = DataManager.ToString(Request.Form["NetmessageFTPServer"]).Trim();
                //options["NetmessageFTPPort"] = DataManager.ToString(Request.Form["NetmessageFTPPort"]).Trim();
                options["NetmessageFTPUsername"] = DataManager.ToString(Request.Form["NetmessageFTPUsername"]).Trim();
                options["NetmessageFTPPassword"] = DataManager.ToString(Request.Form["NetmessageFTPPassword"]).Trim();
                options["NetmessageAccountName"] = DataManager.ToString(Request.Form["NetmessageAccountName"]).Trim();

                if (DataAccess.SaveOptions(options))
                {
                    TempData["SuccessMessage"] = Resources.Resources.SavedSuccessfully;
                }
                else
                {
                    TempData["ErrorMessages"] = Resources.Resources.ErrorSaveFailed;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData["ErrorMessages"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
        #endregion
    }
}
