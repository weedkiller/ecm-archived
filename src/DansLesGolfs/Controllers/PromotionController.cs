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
    public class PromotionController : BaseFrontController
    {
        public PromotionController()
        {

        }

        public ActionResult Index()
        {
            PromotionViewModel vm = new PromotionViewModel();
            vm.PromotionList = DataAccess.GetAllPromotion(0, null, null, null, null);
            return View(vm);
        }

        public ActionResult DlGShopFillter(int brandId)
        {
            return RedirectToAction("DLGShop", "Product", new { brandId = brandId });
        }


    }
}
