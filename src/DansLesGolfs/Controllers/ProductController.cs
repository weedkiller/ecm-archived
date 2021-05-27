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
    public class ProductController : BaseFrontController
    {
        private int pageSize = 0;
        public ProductController()
        {
            pageSize = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["ProductListPageSize"]);

            string allowTeeSheet = DataAccess.GetOption("AllowTeeSheetCategories");
            List<int> AllowTeeSheetCategories = new List<int>();
            allowTeeSheet.Split(',').ToList().ForEach(it =>
            {
                AllowTeeSheetCategories.Add(DataManager.ToInt(it.Trim()));
            });
            ViewBag.AllowTeeSheetCategories = AllowTeeSheetCategories;
            ViewBag.SearchTerm = string.Empty;
        }

        public ActionResult Index(int page = 1)
        {
            ViewBag.Title = Resources.Resources.Products;
            ViewBag.ItemType = "product";
            ViewBag.ItemTypeId = 0;
            ProductsListModel model = GetAllProducts(page);
            return View("ProductsList", model);
        }
        //public ActionResult ItemCategory(int? id)
        //{

        //}

        public ActionResult GreenFees(int page = 1, int? countryId = null, int? regionId = null, int? stateId = null, long? siteId = null, int? timeSlot = null, int? cat = null)
        {
            ViewBag.ItemCategoryId = cat.HasValue ? cat.Value : 0;
            DateTime? fromDate = Request.Form["FromDate"] != null ? (DateTime?)DataManager.ToDateTime(Request.Form["FromDate"], "dd/MM/yyyy", DateTime.Today) : null;
            DateTime? toDate = Request.Form["ToDate"] != null ? (DateTime?)DataManager.ToDateTime(Request.Form["ToDate"], "dd/MM/yyyy", DateTime.Today) : null;
            bool? includePractice = ViewBag.IncludePractice = Request.Form["IncludePractice"] != null ? (bool?)DataManager.ToBoolean(Request.Form["IncludePractice"]) : null;
            ViewBag.Title = Resources.Resources.GreenFees;
            ViewBag.BodyClasses = ViewBag.ItemType = "product green-fee";
            ViewBag.MenuCategoryId = cat.HasValue ? cat.Value : 0;
            ViewBag.ItemTypeId = (int)ItemType.Type.GreenFee;
            //ProductsListModel model = GetProductsByItemType(page, ItemType.Type.GreenFee, countryId, null, stateId, siteId, cat);
            ProductsListModel model = GetGreenFeeProducts(page, countryId, regionId, stateId, siteId, timeSlot, fromDate, toDate, cat, includePractice, "");
            //model.IsShowStar = false;
            return View("ProductsList", model);
        }

        public ActionResult StayPackages(int page = 1, int? countryId = null, int? regionId = null, int? stateId = null, long? siteId = null, int? cat = null, string departureMonth = null)
        {
            ViewBag.Title = Resources.Resources.StayPackages;
            ViewBag.BodyClasses = ViewBag.ItemType = "product stay-package";
            ViewBag.ItemTypeId = (int)ItemType.Type.StayPackage;
            ProductsListModel model = GetProductsByItemType(page, ItemType.Type.StayPackage, countryId, regionId, stateId, siteId, cat, "", departureMonth);
            return View("ProductsList", model);
        }

        public ActionResult GolfLessons(int page = 1, int? countryId = null, int? regionId = null, int? stateId = null, long? siteId = null)
        {
            bool? includeAccommodation = ViewBag.IncludeAccommodation = Request.Form["IncludeAccommodation"] != null ? (bool?)DataManager.ToBoolean(Request.Form["IncludeAccommodation"]) : null;
            int? golfLessonCategoryId = Request.Form["GolfLessonCategoryId"] != null ? (int?)DataManager.ToInt(Request.Form["GolfLessonCategoryId"]) : null;
            ViewBag.Title = Resources.Resources.GolfLessons;
            ViewBag.BodyClasses = ViewBag.ItemType = "product golf-lesson";
            ViewBag.ItemTypeId = (int)ItemType.Type.GolfLesson;
            ProductsListModel model = GetProductsByItemType(page, ItemType.Type.GolfLesson, countryId, regionId, stateId, siteId, golfLessonCategoryId, "", "", includeAccommodation);
            return View("ProductsList", model);
        }

        public ActionResult DrivingRanges(int page = 1, int? countryId = null, int? regionId = null, int? stateId = null, long? siteId = null, int? cat = null, string departureMonth = null)
        {
            ViewBag.Title = Resources.Resources.DrivingRanges;
            ViewBag.BodyClasses = ViewBag.ItemType = "product driving-range";
            ViewBag.ItemTypeId = (int)ItemType.Type.DrivingRange;
            ProductsListModel model = GetProductsByItemType(page, ItemType.Type.DrivingRange, countryId, regionId, stateId, siteId, cat, "", departureMonth);
            return View("ProductsList", model);
        }

        public ActionResult DLGShop(int page = 1, int? countryId = null, int? regionId = null, int? stateId = null, long? siteId = null, int? cat = null, string s = "", int brandId = 0)
        {
            ViewBag.ItemTypeId = (int)ItemType.Type.Product;
            return View("~/Views/Home/Temp.cshtml");
            //LoadAdsList("DLG Shop List Page");
            //string shapeIds = Request.Form["SearchShapeIds"] != null ? Request.Form["SearchShapeIds"] : null;
            //string genreIds = Request.Form["SearchGenreIds"] != null ? Request.Form["SearchGenreIds"] : null;
            //int? dexterityId = Request.Form["SearchDexterityId"] != null ? (int?)DataManager.ToInt(Request.Form["SearchDexterityId"]) : null;
            //int? levelId = Request.Form["SearchLevelId"] != null ? (int?)DataManager.ToInt(Request.Form["SearchLevelId"]) : null;
            //int? shaftId = Request.Form["SearchShaftId"] != null ? (int?)DataManager.ToInt(Request.Form["SearchShaftId"]) : null;
            //int? itemCategoryId = Request.Form["ItemCategoryId"] != null ? (int?)DataManager.ToInt(Request.Form["ItemCategoryId"]) : null;
            //int? itemSubCategoryId = Request.Form["ItemSubCategoryId"] != null ? (int?)DataManager.ToInt(Request.Form["ItemSubCategoryId"]) : null;
            //ViewBag.Title = "DLGShop";
            //ViewBag.BodyClasses = ViewBag.ItemType = "product reseller-product";
            //ProductsListModel model = GetDLGItemsList(page, shapeIds, genreIds, levelId, shaftId, dexterityId, itemCategoryId, itemSubCategoryId, brandId);
            //return View("DLGShopProductsList", model);
        }

        public ActionResult Site(string slug, int? page = 1, int? countryId = null, int? regionId = null, int? stateId = null, int? cat = null, string s = "", int brandId = 0)
        {
            long siteId = DataAccess.GetSiteIdBySlug(slug.Trim());
            page = page.HasValue && page.Value > 0 ? page.Value : 1;
            string shapeIds = Request.Form["SearchShapeIds"] != null ? Request.Form["SearchShapeIds"] : null;
            string genreIds = Request.Form["SearchGenreIds"] != null ? Request.Form["SearchGenreIds"] : null;
            int? dexterityId = Request.Form["SearchDexterityId"] != null ? (int?)DataManager.ToInt(Request.Form["SearchDexterityId"]) : null;
            int? levelId = Request.Form["SearchLevelId"] != null ? (int?)DataManager.ToInt(Request.Form["SearchLevelId"]) : null;
            int? shaftId = Request.Form["SearchShaftId"] != null ? (int?)DataManager.ToInt(Request.Form["SearchShaftId"]) : null;
            int? itemCategoryId = Request.Form["ItemCategoryId"] != null ? (int?)DataManager.ToInt(Request.Form["ItemCategoryId"]) : null;
            int? itemSubCategoryId = Request.Form["ItemSubCategoryId"] != null ? (int?)DataManager.ToInt(Request.Form["ItemSubCategoryId"]) : null;
            ViewBag.Title = Resources.Resources.ProductsInThisSite;
            ViewBag.BodyClasses = ViewBag.ItemType = "product reseller-product";
            ProductsListModel model = GetDLGItemsList(page.Value, shapeIds, genreIds, levelId, shaftId, dexterityId, itemCategoryId, itemSubCategoryId, brandId, siteId);
            return View("DLGShopProductsList", model);
        }

        public ActionResult Search(int page = 1)
        {
            string q = Request.Form["q"];
            ViewBag.SearchTerm = q;
            ViewBag.Title = Resources.Resources.ResultForSearchTerm + " : " + q;
            ProductsListModel model = GetProductsByItemType(page, ItemType.Type.All, null, null, null, null, null, q);
            return View("ProductsList", model);
        }
        private ProductsListModel GetAllProducts(int page)
        {
            ProductsListModel model = new ProductsListModel();
            if (page < 1)
                page = 1;

            int totalPages = 0;
            model.Items = DataAccess.GetLatestItems(out totalPages, pageSize, page, null, null, null, null, "", null, "", 0, 0, this.CultureId, string.Empty);
            model.Page = page;
            model.TotalPages = totalPages;
            return model;
        }
        private ProductsListModel GetProductsByItemType(int page, ItemType.Type itemType, int? countryId = 0, int? regionId = 0, int? stateId = 0, long? siteId = 0, int? categoryId = 0, string searchText = "", string departureMonth = "", bool? includeAccommodation = null)
        {
            ProductsListModel model = new ProductsListModel();
            if (page < 1)
                page = 1;

            int totalPages = 0;
            model.Items = DataAccess.GetLatestItems(out totalPages, pageSize, page, countryId, regionId, stateId, siteId, searchText, includeAccommodation, departureMonth, (int)itemType, categoryId, this.CultureId, string.Empty);
            model.Page = page;
            model.TotalPages = totalPages;

            model.Items.Where(it => string.IsNullOrEmpty(it.ItemName) || string.IsNullOrWhiteSpace(it.ItemName)).ToList().ForEach(it =>
            {
                it.ItemName = Resources.Resources.Untitled;
            });

            return model;
        }
        private ProductsListModel GetGreenFeeProducts(int page, int? countryId = 0, int? regionId = 0, int? stateId = 0, long? siteId = 0, int? timeSlot = null, DateTime? fromDate = null, DateTime? toDate = null, int? cat = null, bool? includePractice = null, string searchText = "")
        {
            ProductsListModel model = new ProductsListModel();
            if (page < 1)
                page = 1;

            int totalPages = 0;
            model.Items = DataAccess.SearchGreenFee(out totalPages, pageSize, page, regionId, stateId, siteId, timeSlot, fromDate, toDate, cat, includePractice, searchText, this.CultureId);
            model.Page = page;
            model.TotalPages = totalPages;

            model.Items.Where(it => string.IsNullOrEmpty(it.ItemName) || string.IsNullOrWhiteSpace(it.ItemName)).ToList().ForEach(it =>
            {
                it.ItemName = Resources.Resources.Untitled;
            });

            return model;
        }

        private ProductsListModel GetDLGItemsList(int page, string shape = null, string genre = null, int? level = 0, int? shaft = 0, int? dexterity = 0, int? itemCategoryId = 0, int? itemSubCategoryId = 0, int? brandId = 0, long? siteId = 0)
        {
            ProductsListModel model = new ProductsListModel();
            if (page < 1)
                page = 1;

            int totalPages = 0;
            model.Items = DataAccess.GetLatestDLGItems(
                totalPages: out totalPages,
                pageSize: pageSize,
                page: page,
                shape: shape,
                genre: genre,
                level: level,
                shaft: shaft,
                dexterity: dexterity,
                searchText: "",
                itemCategory: itemCategoryId,
                itemSubCategory: itemSubCategoryId,
                langId: this.CultureId, brandId: brandId);
            model.Page = page;
            model.TotalPages = totalPages;

            model.Items.Where(it => string.IsNullOrEmpty(it.ItemName) || string.IsNullOrWhiteSpace(it.ItemName)).ToList().ForEach(it =>
            {
                it.ItemName = Resources.Resources.Untitled;
            });

            return model;
        }
    }
}
