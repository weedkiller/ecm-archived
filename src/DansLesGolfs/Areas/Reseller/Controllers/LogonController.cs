using DansLesGolfs.Base;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Reseller.Controllers
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
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
                ViewBag.Username = username;
                ViewBag.Password = password;
                ViewBag.Remember = remember;
                return View();
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Auth.Logout();
            return RedirectToAction("Login");
        }

    }
}
