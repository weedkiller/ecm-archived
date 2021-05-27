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

namespace DansLesGolfs.Areas.Reseller.Controllers
{
    public class DlgCardController : BaseResellerCRUDController
    {
        #region Fields
        private bool isNew = false;
        #endregion

        #region Constructor
        public DlgCardController()
        {
            ObjectName = "DLGCard";
            TitleName = "DLG Card";
            PrimaryKey = "CardId";

            // Define Column Names.
            ColumnNames.Add("CardName", "DLGCard");
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<Site> models = DataAccess.GetAllSites(param);
            return models;
        }

        protected override void DoPrepareForm(int? id)
        {
        }

        protected override object DoPrepareNew()
        {
            return new Site();
        }

        protected override object DoPrepareEdit(int id)
        {
            Site model = DataAccess.GetSiteById(id);
            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            int id = DataManager.ToInt(Request.Form["id"]);


            Item modelItem = null;

            if(id > 0) // Edit
            {
                modelItem = DataAccess.GetItem(id);
                if (modelItem == null)
                {
                    modelItem = new Item();
                }
            }
            else // Add new
            {
                modelItem = new Item();
                isNew = true;
            }

            // Add data
            modelItem.ItemId = id;
            modelItem.ItemSlug = DataManager.ToString(Request.Form["CardName"]).Trim();
            modelItem.ItemSlug = DataManager.ToString(Request.Form["Description"]).Trim();
            modelItem.UpdateDate = DateTime.Now;

            if (Auth.User != null)
                modelItem.UserId = Auth.User.UserId;

            if (id > 0) // Update to Database
            {
                // Update Item
                modelItem.InsertDate = modelItem.InsertDate == DateTime.MinValue ? modelItem.UpdateDate : modelItem.InsertDate;
                result = DataAccess.UpdateItem(modelItem);

                // Update Item Data

                // Update Card

                // Update Card Style
            }
            else // Insert in to Database
            {
                modelItem.InsertDate = modelItem.UpdateDate;
                modelItem.Active = true;
                result = DataAccess.AddItem(modelItem);
                modelItem.ItemId = result;

                if (result > 0)
                {
                    // Add DLG Card
                    DLGCard modelDlgCard = new DLGCard();
                    modelDlgCard.CardId = result;
                    modelDlgCard.Amount = DataManager.ToDouble(Request.Form["Amount"].Trim());
                    modelDlgCard.Qualtity = DataManager.ToInt(Request.Form["Qualtity"].Trim());
                    modelDlgCard.CreatedDate = DateTime.Now;
                    modelDlgCard.Active = true;
                    if (Auth.User != null)
                        modelDlgCard.UserId = Auth.User.UserId;

                    var dlgCardResult = DataAccess.AddDLgCard(modelDlgCard);

                    // Add DLG Card Style
                    DLGCardStyle modelDlgCardStyle = new DLGCardStyle();                 

                }
            }

            ViewBag.id = result > -1 ? modelItem.ItemId : -1;

            return result > 0;
        }

        protected override void DoSaveSuccess(int id)
        {
            // Add new Item lange
            ItemLang modelItemLang = new ItemLang();
            modelItemLang.ItemId = id;
            modelItemLang.LangId = 1;
            modelItemLang.UpdateDate = DateTime.Now;
            if (Auth.User != null)
                modelItemLang.UserId = Auth.User.UserId;
            DataAccess.SaveItemLang(modelItemLang);


            // Save Card Images
            SaveCardImages(id);
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteSite(id) > 0;
        }

        #endregion

        #region AJAX Methods

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
        public ActionResult SaveCardImages(int id, string[] imageIds, string[] fileNames, string[] fileExtensions)
        {
            DLGCardStyle obj = new DLGCardStyle();
            try
            {
                obj.CardId = id;
                for (int i = 0; i < fileNames.Length; i++)
                {
                    obj.CardStyleId = DataManager.ToLong(imageIds[i], -1);
                    obj.ImageName = fileNames[i];
                    obj.FileExtension = fileExtensions[i];
                    obj.ListNo = i;

                    DataAccess.SaveCardImage(obj);
                }
                return Json(new
                {
                    isSuccess = true,
                    message = "Save Card Images successful."
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
        public ActionResult LoadCardImages(int id)
        {
            try
            {
                string uploadDir = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
                List<DLGCardStyle> itemImages = DataAccess.GetCardByCardId(id);
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
                        ThumbnailUrl = Url.Content("~/" + uploadDir + "/Cards/" + id + "/" + item.BaseName + "_t" + item.FileExtension)
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

        #endregion

        #region Private Methods

        #region Save Card Images

        private void SaveCardImages(int id)
        {
            if (Request.Form["CardImageIds"] != null)
            {
                DLGCardStyle cardImage = null;
                string[] CardImageIds = Request.Form["CardImageIds"].Split(',');
                string[] ImageNames = Request.Form["ImageNames"].Split(',');
                string[] BaseNames = Request.Form["BaseNames"].Split(',');
                string[] FileExtensions = Request.Form["FileExtensions"].Split(',');

                if (isNew) // For new data, we have to move images from temp to destination directory.
                {
                    string uploadDir = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
                    string tempDir = Path.Combine(Server.MapPath("~/" + uploadDir), "Cards", "Temp");
                    string destDir = Path.Combine(Server.MapPath("~/" + uploadDir), "Cards", id.ToString());
                    if (!Directory.Exists(destDir))
                    {
                        Directory.CreateDirectory(destDir);
                    }
                    string tempFilePath = string.Empty;
                    string tempthumbFilePath = string.Empty;
                    string tempsmallFilePath = string.Empty;
                    string tempmediumFilePath = string.Empty;
                    string templargeFilePath = string.Empty;
                    string destFilePath = string.Empty;
                    string thumbFilePath = string.Empty;
                    string smallFilePath = string.Empty;
                    string mediumFilePath = string.Empty;
                    string largeFilePath = string.Empty;

                    for (int i = 0, imagesCount = CardImageIds.Length; i < imagesCount; i++)
                    {
                        cardImage = new DLGCardStyle()
                        {
                            CardId = id,
                            CardStyleId = DataManager.ToLong(CardImageIds[i]),
                            ImageName = ImageNames[i],
                            BaseName = BaseNames[i],
                            FileExtension = FileExtensions[i],
                            ListNo = i
                        };
                        if (cardImage.CardStyleId > 0) // For new card image
                        {
                            tempFilePath = Path.Combine(tempDir, cardImage.ImageName);
                            tempthumbFilePath = Path.Combine(tempDir, cardImage.BaseName + "_t" + cardImage.FileExtension);
                            tempsmallFilePath = Path.Combine(tempDir, cardImage.BaseName + "_s" + cardImage.FileExtension);
                            tempmediumFilePath = Path.Combine(tempDir, cardImage.BaseName + "_m" + cardImage.FileExtension);
                            templargeFilePath = Path.Combine(tempDir, cardImage.BaseName + "_l" + cardImage.FileExtension);
                            destFilePath = Path.Combine(destDir, cardImage.ImageName);
                            thumbFilePath = Path.Combine(destDir, cardImage.BaseName + "_t" + cardImage.FileExtension);
                            smallFilePath = Path.Combine(destDir, cardImage.BaseName + "_s" + cardImage.FileExtension);
                            mediumFilePath = Path.Combine(destDir, cardImage.BaseName + "_m" + cardImage.FileExtension);
                            largeFilePath = Path.Combine(destDir, cardImage.BaseName + "_l" + cardImage.FileExtension);
                            try
                            {
                                if (System.IO.File.Exists(tempFilePath))
                                {
                                    System.IO.File.Move(tempFilePath, destFilePath);
                                }
                                if (System.IO.File.Exists(tempthumbFilePath))
                                {
                                    System.IO.File.Move(tempthumbFilePath, thumbFilePath);
                                }
                                if (System.IO.File.Exists(tempsmallFilePath))
                                {
                                    System.IO.File.Move(tempsmallFilePath, smallFilePath);
                                }
                                if (System.IO.File.Exists(tempmediumFilePath))
                                {
                                    System.IO.File.Move(tempmediumFilePath, mediumFilePath);
                                }
                                if (System.IO.File.Exists(templargeFilePath))
                                {
                                    System.IO.File.Move(templargeFilePath, largeFilePath);
                                }
                                DataAccess.SaveCardImage(cardImage);
                            }
                            catch
                            {

                            }
                        }
                        else // for exists card image
                        {
                            DataAccess.SaveCardImage(cardImage);
                        }
                    }
                }
                else // For update
                {
                    for (int i = 0, imagesCount = CardImageIds.Length; i < imagesCount; i++)
                    {
                        cardImage = new DLGCardStyle()
                        {
                            CardId = id,
                            CardStyleId = DataManager.ToLong(CardImageIds[i]),
                            ImageName = ImageNames[i],
                            BaseName = BaseNames[i],
                            FileExtension = FileExtensions[i],
                            ListNo = i
                        };
                        DataAccess.SaveCardImage(cardImage);
                    }
                }
            }
        }
        
        #endregion

        #endregion
    }
}
