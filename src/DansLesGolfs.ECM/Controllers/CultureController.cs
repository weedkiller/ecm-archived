using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using System.Runtime.Caching;

namespace DansLesGolfs.ECM.Controllers
{
    public class CultureController : BaseFrontController
    {
        public ActionResult SetCulture(string culture, string returnUrl)
        {
            string[] cultureInfo = culture.Split(';');
            HttpCookie cookie = new HttpCookie("_culture");
            cookie.Value = cultureInfo[0];
            cookie.Expires = DateTime.Now.AddYears(100);
            Response.Cookies.Add(cookie); 
            cookie = new HttpCookie("_cultureId");
            cookie.Value = cultureInfo[1];
            cookie.Expires = DateTime.Now.AddYears(100);
            Response.Cookies.Add(cookie);
            string redirectUrl = String.IsNullOrEmpty(returnUrl.Trim()) ? "~/" : Server.UrlDecode(returnUrl);

            InMemoryCache cache = new InMemoryCache("WebSiteCache");
            cache.Clear();

            return Redirect(redirectUrl);
        }
    }
}
