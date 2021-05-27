using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Admin.Controllers
{
    public class DlgCardController : BaseAdminCRUDController
    {
        //#region Fields
        //private bool isNew = false;
        //#endregion

        #region Constructor
        public DlgCardController()
        {
            ObjectName = "DlgCard";
            TitleName = "DlgCard";
            PrimaryKey = "ItemId";

            // Define Column Names.
            ColumnNames.Add("ItemId", "ItemId");
            ColumnNames.Add("ItemCode", "ItemCode");

        }
        #endregion

        
        public ActionResult DisplayDlgCardList()
        {
            //ItemDlgCardCore vm = new ItemDlgCardCore();
            //vm.ItemDlgCardList = GetItemDldgCardAdmin();


            Breadcrumbs.Clear();
            Breadcrumbs.Add(TitleName, "DisplayDlgCardList");
            ViewBag.Breadcrumbs = Breadcrumbs;

            ViewBag.ColumnNames = ColumnNames;
            //ViewBag.ClassName = "dlgcard";
            ViewBag.ClassName = "displaydlgcardlist";
            return View("DisplayDlgList");
        }

        public ActionResult AddDlgCard()
        {
            Breadcrumbs.Clear();
            Breadcrumbs.Add(TitleName, "DisplayDlgCardList");
            ViewBag.Breadcrumbs = Breadcrumbs;

            ViewBag.ClassName = "displaydlgcardlist";

            AddDlgCard vm = new AddDlgCard();
            return View("AddDlgCardAdmin",vm);
        }

        [HttpPost]
        public ActionResult SaveDlgCardAdmin(AddDlgCard vm)
        {
            vm.UserId = 1;

            //save item
            var cardItemId = DataAccess.SaveDlgCardItemAdmin(vm);

            //save price
            string[] itemprice = {};
            if (!string.IsNullOrEmpty(vm.Amount))
            {
                itemprice = vm.Amount.Split(';');
            }
            else
            {
                DataAccess.DeletePriceDlgCardItemAdmin(cardItemId);
            }

            if (itemprice.Length > 0)
            {
                DataAccess.DeletePriceDlgCardItemAdmin(cardItemId);
                float number;
                foreach (var price in itemprice)
                {
                    if (float.TryParse(price, out number))
                        DataAccess.SavePriceDlgCardItemAdmin(cardItemId, float.Parse(price), vm.UserId);
                }

            }
            //save primary image
            //var filecount = 0;
            if (vm.ItemId.HasValue && Request.Form["ImageNames"] != null)
            {
                string[] ImageNames = Request.Form["ImageNames"].Split(',');
                string[] BaseNames = Request.Form["BaseNames"].Split(',');
                string[] FileExtensions = Request.Form["FileExtensions"].Split(',');

                DataAccess.DeleteImageDlgCardAdmin(Convert.ToInt32(vm.ItemId));
                for (int i = 0; i < ImageNames.Length;i++ )
                {
                    if (i == 0)
                    {
                        DataAccess.SavePrimaryImageDlgCardAdmin(Convert.ToInt32(vm.ItemId), ImageNames[i], BaseNames[i], FileExtensions[i]);
                        DataAccess.SaveStyleImageDlgCardAdmin(cardItemId, ImageNames[i], (i + 1));
                    }
                    else
                    {
                        DataAccess.SaveStyleImageDlgCardAdmin(cardItemId, ImageNames[i], (i + 1));
                    }

                }
            }

            vm.ItemId = cardItemId;
            return RedirectToAction("EditDlgCardAdmin", "DLGCard", new { @itemId = cardItemId });
        }

        private List<ItemDlgCard> GetItemDldgCardAdmin()
        {
            //get item dlg card
            List<ItemDlgCard> itemDlgCard = DataAccess.GetItemDlgCardByItemTypeId(6,0,null,null,null,null,null);

            return itemDlgCard;
        }


        public ActionResult EditDlgCardAdmin(int itemId)
        {
            AddDlgCard vm = new AddDlgCard();
            List<ItemDlgCard> itemDlgCard = DataAccess.GetItemDlgCardByItemTypeId(6, 0, null, null, null, null, itemId);
            List<ItemPriceDlgCard> priceitemList = DataAccess.GetItemPriceDlgCard(itemId);

            if (itemDlgCard != null)
            {
                vm.ItemId = itemDlgCard[0].ItemId;
                vm.CardName = itemDlgCard[0].ItemCode;
            }

            foreach (var dataprice in priceitemList)
            {
                vm.Amount += dataprice.Price;
                vm.Amount += ";";
            }

            return View("AddDlgCardAdmin", vm);
        }






        //#region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<ItemDlgCard> itemDlgCard = DataAccess.GetItemDlgCardByItemTypeId(6, param.start, param.length, null, null, param.search,null);
            return itemDlgCard;
        }

        [HttpGet]
        public JsonResult LoadDataDlgCardJSON(jQueryDataTableParamModel param)
        {
            try
            {

                IEnumerable<object> result = DoLoadDataJSON(param);
                var list = result.Cast<ItemDlgCard>().ToArray();
                if (result != null)
                {
                    object[] resultArray = ConvertIEnumerableToArray(result);
                    return Json(new
                    {
                        sEcho = param.draw,
                        iTotalRecords = list[0].ToltalItemCount,
                        iTotalDisplayRecords = list[0].ToltalItemCount,
                        aaData = resultArray
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        sEcho = param.draw,
                        iTotalRecords = 0,
                        iTotalDisplayRecords = 0,
                        aaData = new object[0]
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    sEcho = param.draw,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new object[0]
                }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteDlgCardAdmin(id) > 0;
            //return DataAccess.DeleteUser(id) > 0;
        }

        //protected override void DoPrepareForm(int? id)
        //{
        //}

        //protected override object DoPrepareNew()
        //{
        //    return new Site();
        //}

        //protected override object DoPrepareEdit(long id)
        //{
        //    Site model = DataAccess.GetSiteById(id);
        //    return model;
        //}

        //protected override bool DoSave()
        //{
        //    int result = -1;
        //    int id = DataManager.ToInt(Request.Form["id"]);


        //    Item modelItem = null;

        //    if(id > 0) // Edit
        //    {
        //        modelItem = DataAccess.GetItem(id);
        //        if (modelItem == null)
        //        {
        //            modelItem = new Item();
        //        }
        //    }
        //    else // Add new
        //    {
        //        modelItem = new Item();
        //        isNew = true;
        //    }

        //    // Add data
        //    modelItem.ItemId = id;
        //    modelItem.ItemSlug = DataManager.ToString(Request.Form["CardName"]).Trim();
        //    modelItem.ItemSlug = DataManager.ToString(Request.Form["Description"]).Trim();
        //    modelItem.UpdateDate = DateTime.Now;

        //    if (Auth.User != null)
        //        modelItem.UserId = Auth.User.UserId;

        //    if (id > 0) // Update to Database
        //    {
        //        // Update Item
        //        modelItem.InsertDate = modelItem.InsertDate == DateTime.MinValue ? modelItem.UpdateDate : modelItem.InsertDate;
        //        result = DataAccess.UpdateItem(modelItem);

        //        // Update Item Data

        //        // Update Card

        //        // Update Card Style
        //    }
        //    else // Insert in to Database
        //    {
        //        modelItem.InsertDate = modelItem.UpdateDate;
        //        modelItem.Active = true;
        //        result = DataAccess.AddItem(modelItem);
        //        modelItem.ItemId = result;

        //        if (result > 0)
        //        {
        //            // Add DLG Card
        //            DLGCard modelDlgCard = new DLGCard();
        //            modelDlgCard.CardId = result;
        //            modelDlgCard.Amount = DataManager.ToDouble(Request.Form["Amount"].Trim());
        //            modelDlgCard.Qualtity = DataManager.ToInt(Request.Form["Qualtity"].Trim());
        //            modelDlgCard.CreatedDate = DateTime.Now;
        //            modelDlgCard.Active = true;
        //            if (Auth.User != null)
        //                modelDlgCard.UserId = Auth.User.UserId;

        //            var dlgCardResult = DataAccess.AddDLgCard(modelDlgCard);

        //            // Add DLG Card Style
        //            DLGCardStyle modelDlgCardStyle = new DLGCardStyle();                 

        //        }
        //    }

        //    ViewBag.id = result > -1 ? modelItem.ItemId : -1;

        //    return result > 0;
        //}

        //protected override void DoSaveSuccess(int id)
        //{
        //    // Add new Item lange
        //    ItemLang modelItemLang = new ItemLang();
        //    modelItemLang.ItemId = id;
        //    modelItemLang.LangId = 1;
        //    modelItemLang.UpdateDate = DateTime.Now;
        //    if (Auth.User != null)
        //        modelItemLang.UserId = Auth.User.UserId;
        //    DataAccess.SaveItemLang(modelItemLang);


        //    // Save Card Images
        //    SaveCardImages(id);
        //}

        //protected override bool DoDelete(int id)
        //{
        //    return DataAccess.DeleteSite(id) > 0;
        //}

        //#endregion

        //#region AJAX Methods

        [HttpPost]
        public ActionResult AddCardImages()
        {
            int id = DataManager.ToInt(Request.Form["id"]);
            string idStr = id > 0 ? id.ToString() : "Temp";
            string uploadDir = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
            string productDir = Path.Combine(Server.MapPath("~/" + uploadDir), "Cards", idStr);
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
                string fullUrl = Url.Content("~/" + uploadDir + "/Cards/" + idStr + "/" + fileName);
                string thumbUrl = Url.Content("~/" + uploadDir + "/Cards/" + idStr + "/" + baseName + "_t" + extension);
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

                newImage.Dispose();
                newImage = null;
                img.Dispose();
                img = null;

                return Content("0," + fullUrl + "," + thumbUrl + "," + fileName + "," + thumbUrl + "," + baseName + "," + extension);
            }
            else
            {
                throw new Exception("Can't save file please try again.");
            }
        }

        [HttpPost]
        public ActionResult LoadCardImages(int id)
        {
            try
            {
                string uploadDir = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
                //List<DLGCardStyle> itemImages = DataAccess.GetCardByCardId(id);
                List<DLGCardStyle> itemImages = DataAccess.GetDLGCardStyle(id);
                List<object> list = new List<object>();
                foreach (var item in itemImages)
                {
                    list.Add(new
                    {
                        CardStyleId = item.CardStyleId,
                        CardId = item.CardId,
                        ImageName = item.ImageName,
                        ListNo = item.ListNo,
                        BaseName = item.BaseName,
                        FileExtension = item.FileExtension,
                        ImageUrl = Url.Content("~/" + uploadDir + "/Cards/" + id + "/" + item.ImageName),
                        ThumbnailUrl = Url.Content("~/" + uploadDir + "/Cards/" + id + "/" + item.ImageName)
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
        public ActionResult DeleteCardImage(long id, int itemImageId)
        {
            try
            {
                // Delete Real Image before
                DLGCardStyle img = DataAccess.GetCardImage(itemImageId);
                if (img != null && DataAccess.DeleteCardImage(itemImageId) > 0)
                {
                    string uploadDir = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
                    string imageDir = Path.Combine(Server.MapPath("~/" + uploadDir), "Cards", id.ToString());
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

        public ActionResult DlgCardHistory()
        {
            ViewBag.ClassName = "dlgcardhistory";
            return View("DlgCardHistoryAdmin");
        }

        public ActionResult AjaxHandler(jQueryDataTableParamModel param)
        {
            ViewBag.ClassName = "dlgcardhistory";
            var result = DataAccess.GetDlgCardHistory();

            var adata = new List<string[]>();

            foreach (var data in result)
            {
                adata.Add(new string[] {data.DlgCardId.ToString(), data.ItemId.ToString(), data.SaleId.ToString(), data.FirstName, data.LastName, data.Email, data.CardNumber, data.Message, data.BeginBalance.ToString(), data.InsertDate.ToString(), data.UpdateDate.ToString(), data.UserId.ToString(), data.SelectedCardStyleId.ToString() });
            }


            return Json(new
            {
                sEcho = param.draw,
                iTotalRecords = adata.Count,
                iTotalDisplayRecords = param.length,
                aaData = adata

            },
            JsonRequestBehavior.AllowGet);
        }

        public ActionResult DLGCardBalanceHistory(int DlgCardId)
        {
            ViewBag.ClassName = "dlgcardhistory";
            return View("DLGCardBalanceHistoryAdmin", DlgCardId);
        }


        public ActionResult AjaxHandlerDLGCardBalance(jQueryDataTableParamModel param, int DlgCardId)
        {
            ViewBag.ClassName = "dlgcardhistory";

            var result = DataAccess.DLGCardBalanceById(DlgCardId);

            var adata = new List<string[]>();

            foreach (var data in result)
            {
                adata.Add(new string[] { data.Id.ToString(), data.DLGCardId.ToString(), data.UserId.ToString(), data.ActionType.ToString(), data.Description, data.Debit.ToString(), data.Credit.ToString(), data.Balance.ToString(), data.InsertDate.ToString(), data.SaleId.ToString() });
            }


            return Json(new
            {
                sEcho = param.draw,
                iTotalRecords = adata.Count,
                iTotalDisplayRecords = param.length,
                aaData = adata

            },
            JsonRequestBehavior.AllowGet);
        }


        



    }
}
