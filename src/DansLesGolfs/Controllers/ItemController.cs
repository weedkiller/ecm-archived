using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DansLesGolfs.Controllers
{
    public class ItemController : BaseFrontController
    {
        int pageSize = 4;
        public ItemController()
        {
            pageSize = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["RelatedProductsPageSize"], 4);
        }

        public ActionResult Detail(string slug)
        {
            Item item = null;
            slug = slug.Trim();
            if (String.IsNullOrEmpty(slug))
            {
                long id = DataManager.ToLong(Request.QueryString["id"], 0);
                if (id > 0)
                {
                    item = DataAccess.GetItem(id, this.CultureId);
                }
                else
                {
                    return RedirectToAction("PageNotFound", "Error");
                }
            }
            else
            {
                slug = WebHelper.GenerateSlug(slug);
                item = DataAccess.GetItemBySlug(slug, this.CultureId);
                if (item == null)
                {
                    return RedirectToAction("PageNotFound", "Error");
                }
            }

            if (item.ItemTypeId == (int)ItemType.Type.Product)
            {
                LoadDLGItemData(item);
                LoadAdsList("DLG Shop Detail Page");
            }
            else if (item.ItemTypeId == (int)ItemType.Type.GreenFee)
            {
                LoadTeeSheetData(item);
                LoadAdsList("Default");
            }
            else
            {
                LoadAdsList("Offer Detail Page");
            }

            // Get important information.
            ViewBag.SiteName = System.Configuration.ConfigurationManager.AppSettings["SiteName"];

            // Finding your rating.
            int yourRating = 0;
            if (Auth.User != null && item.ItemReviews != null && item.ItemReviews.Any())
            {
                var result = item.ItemReviews.Where(it => it.UserId == Auth.User.UserId);
                if (result != null && result.Any())
                {
                    yourRating = result.First().Rating;
                }
            }
            ViewBag.YourRating = yourRating;

            // Get Tracking Code.
            ViewBag.ConversionTrackingCode = DataAccess.GetItemConversionTrackingCode(item.ItemId, "viewcontent");

            // Get Body Classes
            ViewBag.BodyClasses = ViewBag.ItemType = GetItemClasses(item);

            LoadRelateItems(item);

            // Difference Layouts.
            if (item.ItemTypeId == (int)ItemType.Type.Product)
            {
                AssignItemAttributeNames(item);
                return View("~/Views/Home/Temp.cshtml");
                //return View("DLGItemDetail", item);
            }
            else
            {
                return View(item);
            }
        }

        public ActionResult Print(string slug)
        {
            Item item = null;
            slug = slug.Trim();
            if (String.IsNullOrEmpty(slug))
            {
                long id = DataManager.ToLong(Request.QueryString["id"], 0);
                if (id > 0)
                {
                    item = DataAccess.GetItem(id, this.CultureId);
                }
                else
                {
                    return Content("", "text/html");
                }
            }
            else
            {
                slug = WebHelper.GenerateSlug(slug);
                item = DataAccess.GetItemBySlug(slug, this.CultureId);
                if (item == null)
                {
                    return Content("", "text/html");
                }
            }

            if (item.ItemTypeId == (int)ItemType.Type.Product)
            {
                //List<Modifier> modifiers = DataAccess.GetModifiersByItemTypeId(item.ItemTypeId);

                //modifiers.Where(it => it.ModifierName == "Shape").ToList().ForEach(it =>
                //{
                //    if (it.Choices != null && it.Choices.Any())
                //    {
                //        it.Choices.Where(itc => itc.ChoiceId == item.Shape).ToList().ForEach(itc => item.ShapeName = itc.ChoiceName);
                //    }
                //});

                //modifiers.Where(it => it.ModifierName == "Shaft").ToList().ForEach(it =>
                //{
                //    if (it.Choices != null && it.Choices.Any())
                //    {
                //        it.Choices.Where(itc => itc.ChoiceId == item.Shaft).ToList().ForEach(itc => item.ShaftName = itc.ChoiceName);
                //    }
                //});

                //modifiers.Where(it => it.ModifierName == "Genre").ToList().ForEach(it =>
                //{
                //    if (it.Choices != null && it.Choices.Any())
                //    {
                //        it.Choices.Where(itc => itc.ChoiceId == item.Genre).ToList().ForEach(itc => item.GenreName = itc.ChoiceName);
                //    }
                //});

                //modifiers.Where(it => it.ModifierName == "Dexterity").ToList().ForEach(it =>
                //{
                //    if (it.Choices != null && it.Choices.Any())
                //    {
                //        it.Choices.Where(itc => itc.ChoiceId == item.Dexterity).ToList().ForEach(itc => item.DexterityName = itc.ChoiceName);
                //    }
                //});
            }

            AssignItemAttributeNames(item);
            return View(item);
        }

        private string GetItemClasses(Item item)
        {
            string classes = "product ";
            switch (item.ItemTypeId)
            {
                case 1: classes += "reseller-product";
                    break;
                case 2: classes += "green-fee";
                    break;
                case 3: classes += "stay-package";
                    break;
                case 4: classes += "golf-lesson";
                    break;
                case 5: classes += "driving-range";
                    break;
            }
            return classes;
        }

        /// <summary>
        /// Generate Unit-In-Stock drop down list.
        /// </summary>
        /// <param name="item">Item object.</param>
        private void GenerateViewBagQuantity(Item item)
        {
            List<SelectListItem> quantity = new List<SelectListItem>();
            if (item.UnitInStock <= 0)
            {
                quantity.Add(new SelectListItem()
                {
                    Text = "0",
                    Value = "0",
                    Selected = true
                });
            }
            else
            {
                for (int i = 1; i <= item.UnitInStock; i++)
                {
                    quantity.Add(new SelectListItem()
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    });
                }
                quantity.First().Selected = true;
            }
            ViewBag.DropDownQuantity = quantity;
        }

        private ProductsListModel GetProductsByItemType(int page, ItemType.Type itemType, int? countryId = 0, int? regionId = 0, int? stateId = 0, long? siteId = 0, int? categoryId = 0, string searchText = "")
        {
            ProductsListModel model = new ProductsListModel();
            if (page < 1)
                page = 1;

            int totalPages = 0;
            model.Items = DataAccess.GetLatestItems(out totalPages, pageSize, page, countryId, regionId, stateId, siteId, searchText, null, "", (int)itemType, categoryId);
            model.Page = page;
            model.TotalPages = totalPages;
            return model;
        }

        private void AssignItemAttributeNames(Item item)
        {
            LoadItemAttributesData();
            List<SelectListItem> shapes = ViewBag.DropDownShapes as List<SelectListItem>;
            if (shapes != null)
            {
                shapes.Where(it => it.Value == item.Shape.ToString()).ToList().ForEach(it =>
                {
                    if (it.Value != "0" && !String.IsNullOrEmpty(it.Value))
                        item.ShapeName = it.Text;
                    else
                        item.ShapeName = "-";
                });
            }
            List<SelectListItem> genres = ViewBag.DropDownGenres as List<SelectListItem>;
            if (genres != null)
            {
                genres.Where(it => it.Value == item.Genre.ToString()).ToList().ForEach(it =>
                {
                    if (it.Value != "0" && !String.IsNullOrEmpty(it.Value))
                        item.GenreName = it.Text;
                    else
                        item.GenreName = "-";
                });
            }
            List<SelectListItem> dexterities = ViewBag.DropDownDexterities as List<SelectListItem>;
            if (dexterities != null)
            {
                dexterities.Where(it => it.Value == item.Dexterity.ToString()).ToList().ForEach(it =>
                {
                    if (it.Value != "0" && !String.IsNullOrEmpty(it.Value))
                        item.DexterityName = it.Text;
                    else
                        item.DexterityName = "-";
                });
            }
            List<SelectListItem> shafts = ViewBag.DropDownShafts as List<SelectListItem>;
            if (shafts != null)
            {
                shafts.Where(it => it.Value == item.Shaft.ToString()).ToList().ForEach(it =>
                {
                    if (it.Value != "0" && !String.IsNullOrEmpty(it.Value))
                        item.ShaftName = it.Text;
                    else
                        item.ShaftName = "-";
                });
            }
        }

        private void LoadRelateItems(Item item)
        {
            ProductsListModel relatedItemsModel = new ProductsListModel();
            if (item.ItemTypeId == (int)ItemType.Type.Product)
            {
                pageSize = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["RelatedDLGProductsPageSize"], 3);
            }
            else
            {
                pageSize = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["RelatedProductsPageSize"], 4);
            }
            int totalPages = 0;
            relatedItemsModel.Items = DataAccess.GetLatestItems(out totalPages, pageSize, 1, null, null, null, null, string.Empty, null, null, item.ItemTypeId, null, this.CultureId, item.ItemId.ToString());
            //relatedItemsModel.Items = DataAccess.GetItemsByItemTypeId(item.ItemTypeId, pageSize, item.ItemId, this.CultureId);
            relatedItemsModel.IsShowNoItemText = false;
            ViewBag.RelatedItems = relatedItemsModel;
        }

        private void LoadDLGItemData(Item item)
        {
            //List<Modifier> modifiers = DataAccess.GetModifiersByItemTypeId(item.ItemTypeId, this.CultureId);

            //modifiers.Where(it => it.ModifierName == "Shape").ToList().ForEach(it =>
            //{
            //    if (it.Choices != null && it.Choices.Any())
            //    {
            //        it.Choices.Where(itc => itc.ChoiceId == item.Shape).ToList().ForEach(itc => item.ShapeName = itc.ChoiceName);
            //    }
            //});

            //modifiers.Where(it => it.ModifierName == "Shaft").ToList().ForEach(it =>
            //{
            //    if (it.Choices != null && it.Choices.Any())
            //    {
            //        it.Choices.Where(itc => itc.ChoiceId == item.Shaft).ToList().ForEach(itc => item.ShaftName = itc.ChoiceName);
            //    }
            //});

            //modifiers.Where(it => it.ModifierName == "Genre").ToList().ForEach(it =>
            //{
            //    if (it.Choices != null && it.Choices.Any())
            //    {
            //        it.Choices.Where(itc => itc.ChoiceId == item.Genre).ToList().ForEach(itc => item.GenreName = itc.ChoiceName);
            //    }
            //});

            //modifiers.Where(it => it.ModifierName == "Dexterity").ToList().ForEach(it =>
            //{
            //    if (it.Choices != null && it.Choices.Any())
            //    {
            //        it.Choices.Where(itc => itc.ChoiceId == item.Dexterity).ToList().ForEach(itc => item.DexterityName = itc.ChoiceName);
            //    }
            //});

            GenerateViewBagQuantity(item);
        }

        private void LoadTeeSheetData(Item item)
        {
            ViewBag.TeeSheetDate = DataManager.ToDateTime(Request.QueryString["date"], "dd/MM/yyyy", DateTime.Today);

            var options = DataAccess.GetOptions("DisabledTeeSheetCategories");
            List<int> DisabledTeeSheetCategories = new List<int>();
            options["DisabledTeeSheetCategories"].Split(',').ToList().ForEach(it =>
            {
                DisabledTeeSheetCategories.Add(DataManager.ToInt(it.Trim()));
            });
            if(DisabledTeeSheetCategories.Contains(item.CategoryId))
            {
                item.HasTeeSheet = false;
                ViewBag.IsAlbatrosTeeSheet = false;
                return;
            }
            else
            {
                item.HasTeeSheet = true;
            }

            #region Old Code
            //List<int> albatrosSiteIds = DataAccess.GetAlbatrosSiteIds();

            //if (albatrosSiteIds.Contains(item.SiteId))
            //{
            //    ViewBag.IsAlbatrosTeeSheet = true;
            //}
            //else
            //{
            //    ViewBag.IsAlbatrosTeeSheet = false;
            //    List<TeeSheet> teeSheets = DataAccess.GetTeeSheetsByItemId(item.ItemId);
            //    List<TeeSheetModel> simpleTeeSheets = GetSimpleTeeSheet(teeSheets);
            //    ViewBag.TeeSheetData = new JavaScriptSerializer().Serialize(simpleTeeSheets);
            //} 
            #endregion

            ReservationAPIType reservationType = DataAccess.GetReservationAPIBySiteId(item.SiteId);
            ViewBag.ReservationAPI = reservationType;
            if (reservationType == ReservationAPIType.Albatros)
            {
                ViewBag.IsAlbatrosTeeSheet = true;
            }
            else
            {
                ViewBag.IsAlbatrosTeeSheet = false;
                List<TeeSheet> teeSheets = DataAccess.GetTeeSheetsByItemId(item.ItemId);
                List<TeeSheetModel> simpleTeeSheets = GetSimpleTeeSheet(teeSheets);
                ViewBag.TeeSheetData = new JavaScriptSerializer().Serialize(simpleTeeSheets);
            }

            // Get Cheapest Price of Teesheet.
            //item.TeeSheetCheapestPrice = DataAccess.GetTeeSheetCheapestPrice(item.ItemId);
        }
        private List<TeeSheetModel> GetSimpleTeeSheet(List<TeeSheet> teeSheets)
        {
            List<TeeSheetModel> list = new List<TeeSheetModel>();
            foreach (TeeSheet t in teeSheets)
            {
                list.Add(new TeeSheetModel()
                {
                    id = t.TeeSheetId,
                    day = t.TeeSheetDay,
                    date = t.TeeSheetDate.ToString("yyyy-M-d"),
                    start = t.FromTime.Hours,
                    end = t.ToTime.Hours,
                    price = t.Price,
                    prebooking = t.PreBookingDays,
                    discount = t.Discount,
                    //available = IsTeeTimeAvailable(t)
                });
            }
            return list;
        }

        private bool IsTeeTimeAvailable(TeeSheet t)
        {
            bool isAvailable = false;
            DateTime now = DateTime.Now;
            DateTime today = DateTime.Today;
            //DateTime teeSheetDate
            return isAvailable;
        }

    }
}
