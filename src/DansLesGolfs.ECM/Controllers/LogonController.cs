using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.ECM.Controllers
{
    public class LogonController : BaseController
    {
        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.Username = string.Empty;
            ViewBag.Password = string.Empty;
            ViewBag.Remember = 0;
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password, int remember = 0)
        {
            if (Auth.Attempt(username, password, remember > 0))
            {
                if (Auth.User.UserTypeId == (int)UserType.Type.SiteManager || Auth.User.UserTypeId == (int)UserType.Type.Staff || Auth.User.UserTypeId == (int)UserType.Type.Admin || Auth.User.UserTypeId == (int)UserType.Type.SuperAdmin)
                {
                    return RedirectToAction("Index", "Emailing");
                }
                else
                {
                    ViewBag.ErrorMessage = "You have not permission to access.";
                }
            }
            else
            {
                ViewBag.ErrorMessage = Resources.Resources.InvalidEmailOrPassword;
            }
            ViewBag.Username = username;
            ViewBag.Password = password;
            ViewBag.Remember = remember;
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Auth.Logout();
            return RedirectToAction("Login");
        }

    }
}
