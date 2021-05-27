using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Controllers
{
    public class PageController : BaseFrontController
    {
        public PageController()
        {
        }

        public ActionResult Detail(string contentKey = "")
        {
            WebContent content = DataAccess.GetWebContentByKey(contentKey, this.CultureId);
            if(content.ContentId > 0)
            {
                LoadAdsList("Default");
                return View(content);
            }
            else
            {
                return View("PageNotFound");
            }
        }

    }
}
