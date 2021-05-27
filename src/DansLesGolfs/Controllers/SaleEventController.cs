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
    public class SaleEventController : BaseFrontController
    {
        public SaleEventController()
        {
        }

        public ActionResult Index()
        {
            List<Brand> brands = DataAccess.GetBrandsWithSaleItems();
            return View(brands);
        }
    }
}
