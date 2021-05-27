using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using DansLesGolfs.Models;

namespace DansLesGolfs.Controllers
{
    public class ErrorController : BaseFrontController
    {
        #region Action Methods
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PageNotFound()
        {
            string url = RouteData.Values["url"] != null ? RouteData.Values["url"].ToString() : string.Empty;
            if(String.IsNullOrEmpty(url))
            {
                return View();
            }
            else
            {
                return Redirect(url);
            }
        }

        public ActionResult NotLogIn()
        {
            return View();
        }

        public ActionResult NoPermission()
        {
            return View();
        }
        #endregion

        #region Private Methods
        private ProductsListModel GetAllProducts(int page)
        {
            ProductsListModel model = new ProductsListModel();
            if (page < 1)
                page = 1;

            int totalPages = 0;
            model.Items = DataAccess.GetLatestItems(out totalPages, 8, page);
            model.Page = page;
            model.TotalPages = totalPages;
            return model;
        }
        #endregion
    }
}
