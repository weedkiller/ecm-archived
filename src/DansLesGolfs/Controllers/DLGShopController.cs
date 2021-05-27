using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using System.Runtime.Caching;
using DansLesGolfs.Models;
using DansLesGolfs.Base;

namespace DansLesGolfs.Controllers
{
    public class DLGShopController : BaseFrontController
    {
        public ActionResult Register()
        {
            List<Title> titles = DataAccess.GetItemTitlesDropDownList(this.CultureId);
            List<SelectListItem> list = ListToDropDownList<Title>(titles, "TitleId", "TitleName");
            if (list.Any())
            {
                list.First().Selected = true;
                ViewBag.TitleName = list.First().Value;
            }
            ViewBag.DropDownTitles = list;
            ViewBag.BodyClasses = ViewBag.ItemType = "product reseller-product";
            DLGShopRegisterModel model = new DLGShopRegisterModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Register(string TitleName, string FirstName, string LastName, string SiteName, string Email, string Telephone)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    if (DataAccess.IsExistsEmail(Email))
                        throw new Exception(Resources.Resources.YourEmailIsAlreadyInUse);

                    SendNewRegisteredUserEmail(TitleName, FirstName, LastName, SiteName, Email, Telephone);
                    return RedirectToAction("RegisterSuccess", "DLGShop");
                }
                else
                {
                    string errors = "<ul>";
                    foreach (ModelState modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            errors += "<li>" + error.ErrorMessage + "</li>";
                        }
                    }
                    errors += "</ul>";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            List<Title> titles = DataAccess.GetItemTitlesDropDownList(this.CultureId);
            List<SelectListItem> list = ListToDropDownList<Title>(titles, "TitleId", "TitleName");
            if(list.Any())
            {
                list.First().Selected = true;
                ViewBag.TitleName = list.First().Value;
            }
            ViewBag.DropDownTitles = list;
            ViewBag.BodyClasses = ViewBag.ItemType = "product reseller-product";

            return View();
        }

        public ActionResult RegisterSuccess()
        {
            ViewBag.SuccessMessage = Resources.Resources.DLGShopRegisterSuccessText;
            return View();
        }

        public ActionResult ActivateAccount(string email, string code)
        {
            string contentKey = string.Empty;
            if (DataAccess.CheckAccountActivation(email, code))
            {
                contentKey = "dlgshop-activation-success";
            }
            else
            {
                contentKey = "dlgshop-activation-failed";
            }
            WebContent content = DataAccess.GetWebContentByKey(contentKey, this.CultureId);
            ViewBag.BodyClasses = ViewBag.ItemType = "product reseller-product";
            return View(content);
        }

        private void SendNewRegisteredUserEmail(string titleName, string firstName, string lastName, string siteName, string email, string telephone)
        {
            string siteUrl = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
            string siteEmail = System.Configuration.ConfigurationManager.AppSettings["SiteEmail"].ToString();

            ViewBag.TitleName = titleName;
            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;
            ViewBag.SiteName = siteName;
            ViewBag.Email = email;
            ViewBag.Telephone = telephone;
            ViewBag.

            ViewBag.ReceiverName = siteUrl;
            ViewBag.ReceiverEmail = siteEmail;
            EmailArguments emailArgs = EmailArguments.GetInstanceFromConfig();
            emailArgs.Subject = "New DLG Shop Registeration";
            emailArgs.To.Add(siteEmail, siteUrl);
            emailArgs.MailBody = GetHTMLFromView("~/Views/_Shared/UC/EmailTemplates/NotifyDLGShopRegister.cshtml", null);
            EmailHelper.SendEmailWithAttachments(emailArgs);
        }
    }
}
