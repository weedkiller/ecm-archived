using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Admin.Controllers
{
    public class ItemController : BaseAdminCRUDController
    {
        #region Constructor
        public ItemController()
        {
            ObjectName = "Item";
            TitleName = Resources.Resources.Product;
            PrimaryKey = "ItemId";
            IsClonable = true;

            // Define Column Names.
            ColumnNames.Add("ItemCode", Resources.Resources.ProductCode);
            ColumnNames.Add("ItemName", Resources.Resources.ProductName);
            ColumnNames.Add("SiteName", Resources.Resources.SiteName);
            ColumnNames.Add("CategoryName", Resources.Resources.Category);
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<Item> models = DataAccess.GetAllItems(param, ItemType.Type.Product, null, this.CultureId);
            return models;
        }

        protected override void DoPrepareForm(int? id = null)
        {
            List<ItemCategory> itemCategories = DataAccess.GetItemcategoryByItemTypeId((int)ItemType.Type.Product);
            //List<ItemCategory> itemCategories = DataAccess.GetItemCategoriesDropDownList((int)ItemType.Type.Product);
            itemCategories.Insert(0, new ItemCategory() { CategoryId = 0, CategoryName = Resources.Resources.Category });
            ViewBag.ItemCategories = GetCategoryDropDownList(itemCategories);

            //List<ItemCategory> itemSubCategories = new List<ItemCategory>();
            //itemSubCategories.Insert(0, new ItemCategory() { CategoryId = 0, CategoryName = Resources.Resources.SubCategory });
            //ViewBag.ItemSubCategories = ListToDropDownList(itemSubCategories, "CategoryId", "CategoryName");

            List<Supplier> suppliers = DataAccess.GetAllSuppliers();
            ViewBag.Suppliers = ListToDropDownList<Supplier>(suppliers, "SupplierId", "SupplierName");

            List<Site> sites = DataAccess.GetSitesDropDownListData();
            List<SelectListItem> siteList = ListToDropDownList<Site>(sites, "SiteId", "SiteName");
            siteList.Insert(0, new SelectListItem()
            {
                Text = Resources.Resources.SelectSite,
                Value = "0",
                Selected = true
            });
            ViewBag.Sites = siteList;

            List<Course> courses = null;
            if (sites.Any())
            {
                courses = DataAccess.GetCoursesListBySiteId(sites.First().SiteId);
            }
            else
            {
                courses = new List<Course>();
            }
            List<SelectListItem> courseList = ListToDropDownList<Course>(courses, "CourseId", "CourseName");
            courseList.Add(new SelectListItem() { Text = Resources.Resources.SelectCourse, Value = "0" });
            ViewBag.Courses = courseList;

            List<Tax> taxes = DataAccess.GetAllTaxes();
            ViewBag.Taxes = ListToDropDownList<Tax>(taxes, "TaxId", "TaxName");

            List<ShippingType> shippingTypes = DataAccess.GetShippingTypeDropDownListData(this.CultureId);
            ViewBag.ShippingTypes = ListToDropDownList<ShippingType>(shippingTypes, "ShippingTypeId", "ShippingTypeName");
            
            List<Brand> brands = DataAccess.GetBrandDropDownListData();
            ViewBag.Brands = ListToDropDownList<Brand>(brands, "BrandId", "BrandName");

            //List<Modifier> modifiers = DataAccess.GetAllModifiers();
            //ViewBag.Modifiers = ListToDropDownList<Modifier>(modifiers, "ModifierId", "ModifierName");

            ViewBag.DefaultLatitude = DataManager.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["DefaultLatitude"], 47);
            ViewBag.DefaultLongitude = DataManager.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["DefaultLongitude"], 2);

            ViewBag.Modifiers = DataAccess.GetModifiersByItemTypeId((int)ItemType.Type.Product, this.CultureId);

        }

        protected override void DoClone(long id)
        {
            ViewBag.ItemCode = DataAccess.GetItemRunningNumber((int)ItemType.Type.Product, 6, DataManager.ToInt(ViewBag.CategoryId));
            ViewBag.ItemSlug = GetItemSlug(ViewBag.ItemSlug, null);
        }

        protected override object DoPrepareNew()
        {
            return new Item()
            {
                ItemCode = DataAccess.GetItemRunningNumber((int)ItemType.Type.Product, 6),
                ItemTypeId = (int)ItemType.Type.Product,
                IsPublish = true,
                IsAllowReview = false,
                IsShowOnHomepage = false
            };
        }

        protected override object DoPrepareEdit(long id)
        {
            Item model = DataAccess.GetItem(id);
            if (model.ItemLangs != null && model.ItemLangs.Any())
            {
                model.ItemName = model.ItemLangs[0].ItemName;
                ViewBag.ItemDesc = model.ItemLangs[0].ItemDesc;
                ViewBag.ItemShortDesc = model.ItemLangs[0].ItemShortDesc;
            }

            ItemConversionTracking tracking = DataAccess.GetItemConversionTrackingByItemId(model.ItemId);
            ViewBag.TrackingCodeViewContent = tracking.TrackingCodeViewContent;
            ViewBag.TrackingCodeAddToCart = tracking.TrackingCodeAddToCart;
            ViewBag.TrackingCodeInitialCheckout = tracking.TrackingCodeInitialCheckout;
            ViewBag.TrackingCodePurchase = tracking.TrackingCodePurchase;

            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            Item model = null;
            int id = DataManager.ToInt(Request.Form["id"]);
            if (id > 0)
            {
                model = DataAccess.GetItem(id);
                if (model == null)
                {
                    model = new Item();
                }
            }
            else
            {
                model = new Item();
            }
            model.PromotionStatusId = DataManager.ToInt(Request.Form["PromotionStatusId"]);
            model.ItemId = id;
            model.ItemTypeId = (int)ItemType.Type.Product;
            model.CategoryId = DataManager.ToInt(Request.Form["CategoryId"]);
            model.ItemCode = DataManager.ToString(Request.Form["ItemCode"]).Trim();
            model.ItemSlug = GetItemSlug(DataManager.ToString(Request.Form["ItemSlug"]).Trim(), id);
            model.SupplierId = DataManager.ToInt(Request.Form["SupplierId"]);
            model.SiteId = DataManager.ToInt(Request.Form["SiteId"]);
            model.TaxId = DataManager.ToInt(Request.Form["TaxId"]);
            model.UnitInStock = DataManager.ToInt(Request.Form["UnitInStock"]);
            model.Price = DataManager.ToDecimal(Request.Form["Price"]);
            model.OldPrice = DataManager.ToDecimal(Request.Form["OldPrice"]);
            model.ProductCost = DataManager.ToDecimal(Request.Form["ProductCost"]);
            model.SpecialPrice = DataManager.ToDecimal(Request.Form["SpecialPrice"]);
            model.IsAllowReview = DataManager.ToBoolean(Request.Form["IsAllowReview"]);
            model.IsPublish = DataManager.ToBoolean(Request.Form["IsPublish"]);
            model.IsShowOnHomepage = DataManager.ToBoolean(Request.Form["IsShowOnHomepage"]);
            model.Shape = DataManager.ToInt(Request.Form["Shape"]);
            model.Genre = DataManager.ToInt(Request.Form["Genre"]);
            model.Shaft = DataManager.ToInt(Request.Form["Shaft"]);
            model.Dexterity = DataManager.ToInt(Request.Form["Dexterity"]);
            model.Size = DataManager.ToString(Request.Form["Size"]);
            model.ShippingCost = DataManager.ToDecimal(Request.Form["ShippingCost"]);
            model.ShippingTimeMin = DataManager.ToInt(Request.Form["ShippingTimeMin"]);
            model.ShippingTimeMax = DataManager.ToInt(Request.Form["ShippingTimeMax"]);
            model.CanReturn = DataManager.ToBoolean(Request.Form["CanReturn"]);
            model.RefundWithIn = DataManager.ToInt(Request.Form["RefundWithIn"]);
            model.ShippingTypeId = DataManager.ToInt(Request.Form["ShippingTypeId"]);
            model.BrandId = DataManager.ToInt(Request.Form["BrandId"]);
            model.SpecialPriceStartDate = null;
            model.SpecialPriceEndDate = null;
            model.AvailableStartDate = null;
            model.AvailableEndDate = null;
            model.IncludeAccommodation = false;

            #region PublishDate
            DateTime tempDate = DateTime.Today;
            if (!String.IsNullOrEmpty(Request.Form["PublishStartDate"]))
            {
                DateTime.TryParseExact(Request.Form["PublishStartDate"], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate);
                model.PublishStartDate = tempDate;
                if (model.PublishStartDate == DateTime.MinValue)
                    model.PublishStartDate = null;
            }
            else
            {
                model.PublishStartDate = null;
            }

            tempDate = DateTime.Today;
            if (!String.IsNullOrEmpty(Request.Form["PublishEndDate"]))
            {
                DateTime.TryParseExact(Request.Form["PublishEndDate"], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate);
                model.PublishEndDate = tempDate;
                if (model.PublishEndDate == DateTime.MinValue)
                    model.PublishEndDate = null;
            }
            else
            {
                model.PublishEndDate = null;
            }

            if (model.PublishEndDate < model.PublishStartDate)
            {
                tempDate = model.PublishStartDate.Value;
                model.PublishStartDate = model.PublishEndDate;
                model.PublishEndDate = tempDate;
            }
            #endregion

            model.UpdateDate = DateTime.Now;

            // Item Slug.
            if (String.IsNullOrEmpty(model.ItemSlug))
            {
                model.ItemSlug = GetItemSlug(DataManager.ToString(Request.Form["ItemName"]), model.ItemId);
            }

            if (Auth.User != null)
                model.UserId = Auth.User.UserId;

            if (id > 0)
            {
                model.InsertDate = model.InsertDate == DateTime.MinValue ? model.UpdateDate : model.InsertDate;
                result = DataAccess.UpdateItem(model);
            }
            else
            {
                model.InsertDate = model.UpdateDate;
                model.Active = true;

                result = DataAccess.AddItem(model);
                model.ItemId = result;
            }
            ViewBag.id = result > -1 ? model.ItemId : -1;
            return result > 0;
        }

        protected override void DoSaveSuccess(int id)
        {
            long itemId = DataManager.ToLong(Request.Form["id"]);
            string[] itemNames = Request.Form.GetValues("ItemName");
            string[] invoiceNames = Request.Form.GetValues("InvoiceName");
            string[] itemDescs = Request.Form.GetValues("itemDesc");
            string[] ItemShortDescs = Request.Form.GetValues("ItemShortDesc");
            string[] SpecialPrices = Request.Form.GetValues("SpecialPrices");
            string[] priceStartDates = Request.Form.GetValues("PriceStartDates");
            string[] priceEndDates = Request.Form.GetValues("PriceEndDates");
            string[] priceTypes = Request.Form.GetValues("PriceTypes");

            ItemLang itemLang = null;

            for (int i = 0, j = itemNames.Length; i < j; i++)
            {
                itemLang = new ItemLang();
                itemLang.ItemId = itemId;
                itemLang.LangId = i + 1;
                itemLang.ItemName = itemNames[i].Trim();
                itemLang.ItemDesc = itemDescs[i].Trim();
                itemLang.InvoiceName = invoiceNames[i].Trim();
                itemLang.ItemShortDesc = ItemShortDescs[i].Trim();
                itemLang.UpdateDate = DateTime.Now;
                if (Auth.User != null)
                    itemLang.UserId = Auth.User.UserId;

                DataAccess.SaveItemLang(itemLang);
            }

            DataAccess.DeleteItemPriceByItemId(itemId);
            DateTime tempDate = DateTime.Today;
            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today;
            if (SpecialPrices != null && priceStartDates != null && priceEndDates != null)
            {
                ItemPrice itemPrice = null;
                for (int i = 0, j = SpecialPrices.Length; i < j; i++)
                {
                    tempDate = DateTime.Today;
                    if (!String.IsNullOrEmpty(priceStartDates[i]))
                    {
                        DateTime.TryParseExact(priceStartDates[i], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate);
                        startDate = tempDate;
                    }
                    else
                    {
                        startDate = DateTime.Today;
                    }

                    tempDate = DateTime.Today;
                    if (!String.IsNullOrEmpty(priceEndDates[i]))
                    {
                        DateTime.TryParseExact(priceEndDates[i], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate);
                        endDate = tempDate;
                    }
                    else
                    {
                        endDate = DateTime.Today;
                    }
                    itemPrice = new ItemPrice()
                    {
                        ItemId = itemId,
                        Price = DataManager.ToDecimal(SpecialPrices[i]),
                        PriceType = DataManager.ToInt(priceTypes[i]),
                        StartDate = startDate,
                        EndDate = endDate,
                        Active = true,
                        InsertDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    };

                    DataAccess.SaveItemPrice(itemPrice);
                }
            }

            ItemConversionTracking itemConversionTracking = new ItemConversionTracking();
            itemConversionTracking.ItemId = itemId;
            itemConversionTracking.TrackingCodeViewContent = DataManager.ToString(Request.Form["TrackingCodeViewContent"]);
            itemConversionTracking.TrackingCodeAddToCart = DataManager.ToString(Request.Form["TrackingCodeAddToCart"]);
            itemConversionTracking.TrackingCodeInitialCheckout = DataManager.ToString(Request.Form["TrackingCodeInitialCheckout"]);
            itemConversionTracking.TrackingCodePurchase = DataManager.ToString(Request.Form["TrackingCodePurchase"]);
            DataAccess.SaveItemConversionTrackingCode(itemConversionTracking);
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteItem(id) > 0;
        }
        #endregion

        #region Private Methods
        #region GetCategoryDropDownList
        private dynamic GetCategoryDropDownList(List<ItemCategory> itemCategories)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            itemCategories.Where(parent => parent.ParentCategoryId == 0).OrderBy(parent => parent.CategoryId).ToList().ForEach(parent =>
            {
                list.Add(new SelectListItem()
                {
                    Text = parent.CategoryName,
                    Value = parent.CategoryId.ToString()
                });
                itemCategories.Where(child => child.ParentCategoryId == parent.CategoryId && child.ParentCategoryId != 0).OrderBy(child => child.CategoryName).ToList().ForEach(child =>
                {
                    list.Add(new SelectListItem()
                    {
                        Text = parent.CategoryName + " / " + child.CategoryName,
                        Value = child.CategoryId.ToString()
                    });
                });
            });
            return list;
        } 
        #endregion
        #endregion

        #region AJAX Methods
        [HttpPost]
        public ActionResult AddItemImages()
        {
            int id = DataManager.ToInt(Request.Form["id"]);
            string uploadDir = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
            string productDir = Path.Combine(Server.MapPath("~/" + uploadDir), "Products", id.ToString());
            string imagePath = string.Empty;
            string fileName = string.Empty;
            string baseName = string.Empty;
            string extension = string.Empty;

            if (!Directory.Exists(productDir))
            {
                Directory.CreateDirectory(productDir);
                if (!Directory.Exists(productDir))
                {
                    return Content("Can't create directory. Please check directory permession.");
                }
            }

            var file = Request.Files["Filedata"];
            extension = Path.GetExtension(file.FileName).ToLower();
            do
            {
                baseName = Guid.NewGuid().ToString();
                fileName = baseName + extension;
                imagePath = Path.Combine(productDir, fileName);
            } while (System.IO.File.Exists(imagePath));
            file.SaveAs(imagePath);

            if (System.IO.File.Exists(imagePath))
            {
                string fullUrl = Url.Content("~/" + uploadDir + "/Products/" + id.ToString() + "/" + fileName);
                string thumbUrl = Url.Content("~/" + uploadDir + "/Products/" + id.ToString() + "/" + baseName + "_t" + extension);
                string thumbFilePath = Path.Combine(productDir, baseName + "_t" + extension);
                string smallFilePath = Path.Combine(productDir, baseName + "_s" + extension);
                string mediumFilePath = Path.Combine(productDir, baseName + "_m" + extension);
                string largeFilePath = Path.Combine(productDir, baseName + "_l" + extension);

                // Create Thumbnail.
                Bitmap img = new Bitmap(imagePath);
                int newWidth = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["ThumbnailWidth"]);
                int newHeight = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["ThumbnailHeight"]);
                Image newImage = ImageHelper.GetResizedImage(img, newWidth, newHeight);
                newImage.Save(thumbFilePath);

                // Create small image.
                newWidth = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["SmaillImageWidth"]);
                newHeight = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["SmaillImageHeight"]);
                newImage = ImageHelper.GetResizedImage(img, newWidth, newHeight);
                newImage.Save(smallFilePath);

                // Create medium image.
                newWidth = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["MediumImageWidth"]);
                newHeight = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["MediumImageHeight"]);
                newImage = ImageHelper.GetResizedImage(img, newWidth, newHeight);
                newImage.Save(mediumFilePath);

                // Create large image.
                newWidth = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["LargeImageWidth"]);
                newHeight = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["LargeImageHeight"]);
                newImage = ImageHelper.GetResizedImage(img, newWidth, newHeight);
                newImage.Save(largeFilePath);

                ItemImage obj = new ItemImage();
                obj.ItemId = id;
                obj.ImageName = fileName;
                obj.BaseName = baseName;
                obj.FileExtension = extension;
                obj.ItemImageId = DataAccess.AddItemImage(obj);

                newImage.Dispose();
                newImage = null;
                img.Dispose();
                img = null;

                return Content(obj.ItemImageId + "," + fullUrl + "," + thumbUrl + "," + fileName + "," + thumbUrl + "," + baseName + "," + extension);
            }
            else
            {
                throw new Exception("Can't save file please try again.");
            }
        }

        [HttpPost]
        public ActionResult SaveItemImages(int id, string[] imageIds, string[] fileNames, string[] baseNames, string[] fileExtensions)
        {
            ItemImage obj = new ItemImage();
            try
            {
                obj.ItemId = id;
                for (int i = 0; i < fileNames.Length; i++)
                {
                    obj.ItemImageId = DataManager.ToLong(imageIds[i], -1);
                    obj.ImageName = fileNames[i];
                    obj.BaseName = baseNames[i];
                    obj.FileExtension = fileExtensions[i];
                    obj.ListNo = i;
                    DataAccess.SaveItemImage(obj);
                }
                return Json(new
                {
                    isSuccess = true,
                    message = "Save Product Images successful."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public ActionResult LoadItemImages(int id)
        {
            try
            {
                string uploadDir = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
                List<ItemImage> itemImages = DataAccess.GetItemImagesByItemId(id);
                List<object> list = new List<object>();
                foreach (var item in itemImages)
                {
                    list.Add(new
                    {
                        ItemImageId = item.ItemImageId,
                        ItemId = item.ItemId,
                        ImageName = item.ImageName,
                        ListNo = item.ListNo,
                        BaseName = item.BaseName,
                        FileExtension = item.FileExtension,
                        ImageUrl = Url.Content("~/" + uploadDir + "/Products/" + id + "/" + item.ImageName),
                        ThumbnailUrl = Url.Content("~/" + uploadDir + "/Products/" + id + "/" + item.BaseName + "_t" + item.FileExtension)
                    });
                }
                return Json(new
                {
                    isSuccess = true,
                    itemImages = list
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public ActionResult DeleteItemImage(long id, int itemImageId)
        {
            try
            {
                // Delete Real Image before
                ItemImage img = DataAccess.GetItemImage(itemImageId);
                if (img != null && DataAccess.DeleteItemImage(itemImageId) > 0)
                {
                    string uploadDir = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
                    string imageDir = Path.Combine(Server.MapPath("~/" + uploadDir), "Products", id.ToString());
                    string imageFile = Path.Combine(imageDir, img.BaseName + img.FileExtension);
                    if (System.IO.File.Exists(imageFile))
                    {
                        @System.IO.File.Delete(imageFile);
                    }
                    imageFile = Path.Combine(imageDir, img.BaseName + "_t" + img.FileExtension);
                    if (System.IO.File.Exists(imageFile))
                    {
                        @System.IO.File.Delete(imageFile);
                    }
                    imageFile = Path.Combine(imageDir, img.BaseName + "_s" + img.FileExtension);
                    if (System.IO.File.Exists(imageFile))
                    {
                        @System.IO.File.Delete(imageFile);
                    }
                    imageFile = Path.Combine(imageDir, img.BaseName + "_m" + img.FileExtension);
                    if (System.IO.File.Exists(imageFile))
                    {
                        @System.IO.File.Delete(imageFile);
                    }
                    imageFile = Path.Combine(imageDir, img.BaseName + "_l" + img.FileExtension);
                    if (System.IO.File.Exists(imageFile))
                    {
                        @System.IO.File.Delete(imageFile);
                    }
                }
                return Json(new
                {
                    isSuccess = true,
                    message = "Delete Image Successful."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public ActionResult GenerateItemSlug(string text, long? skipId)
        {
            try
            {
                string slug = GetItemSlug(text, skipId);
                return Json(new
                {
                    isSuccess = true,
                    slug = slug
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        private string GetItemSlug(string text, long? itemId = null)
        {
            int num = 1;
            string slug = WebHelper.GenerateSlug(text);

            while (DataAccess.IsExistsItemSlug(slug, itemId))
            {
                slug = WebHelper.GenerateSlug(text);
                slug = slug + (num++).ToString();
            }
            return slug;
        }

        [HttpPost]
        public JsonResult GetChoicesJSON(int modifierId = 0)
        {
            List<ModifierChoice> modifierChoices = DataAccess.GetModifierChoices(modifierId);
            string[] choicesArray = modifierChoices.Select(it => it.ChoiceName).ToArray();
            return Json(choicesArray);
        }

        [HttpPost]
        public JsonResult ReorderItemModifier(long id, int[] modifierIds)
        {
            try
            {
                long itemId = DataManager.ToLong(id, 0);
                for (int i = 0; i < modifierIds.Length; i++)
                {
                    DataAccess.UpdateItemModifierOrder(itemId, DataManager.ToInt(modifierIds[i]), i);
                }
                return Json(new
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult AjaxGetItemSubCategoriesByItemCategoryId(int categoryId = 0)
        {
            try
            {
                List<ItemCategory> itemSubCategories = DataAccess.GetItemCategoriesDropDownList((int)ItemType.Type.Product, categoryId);
                itemSubCategories.Insert(0, new ItemCategory
                {
                    CategoryId = 0,
                    CategoryName = Resources.Resources.SubCategories
                });
                return Json(new
                {
                    isResult = true,
                    list = itemSubCategories
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isResult = false,
                    message = ex.Message
                });
            }
        }
        #endregion
    }
}
