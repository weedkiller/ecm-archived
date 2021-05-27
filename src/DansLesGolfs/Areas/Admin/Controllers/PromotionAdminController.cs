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
    public class PromotionAdminController : BaseAdminCRUDController
    {
        #region Constructor
        public PromotionAdminController()
        {
            //ViewBag.ObjectName = Resources.Resources.Reports;
            //ViewBag.ClassName = "Promotion";

            ObjectName = "Promotion";
            TitleName = "Promotion";
            PrimaryKey = "PromotionId";

            // Define Column Names.
            ColumnNames.Add("PromotionId", "PromotionId");
            ColumnNames.Add("BrandName", "BrandName");
            
        }
        #endregion


       
        //public ActionResult Index()
        //{

        //    PromotionModel vm = new PromotionModel();
        //    return View("AddPromotion", vm);
        //}

        public ActionResult PromotionList()
        {
            ViewBag.ColumnNames = ColumnNames;
            return View("index");
        }

        public ActionResult AddPromotion()
        {
            PromotionModel vm = new PromotionModel();

            List<Brand> brands = DataAccess.GetBrandDropDownListData();
            vm.BrandsList = ListToDropDownList<Brand>(brands, "BrandId", "BrandName");

            return View("AddPromotion", vm);
        }

        public ActionResult SavePromotion(PromotionModel vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.PromotionId.HasValue)
                {
                    DataAccess.UpdatePromotion(vm.PromotionId, vm.PromotionImage, vm.PromotionBrandImage, vm.PromotionContent, vm.PromotionTimecontent, vm.BrandNameId);
                }
                else
                {
                    vm.PromotionId = DataAccess.SavePromotion(vm.PromotionContent, vm.PromotionTimecontent, vm.BrandNameId);
                    return RedirectToAction("EditPromotion", "PromotionAdmin", new { @promotionId = vm.PromotionId });
                }
                

            }

            return RedirectToAction("EditPromotion", "PromotionAdmin", new { @promotionId = vm.PromotionId });
        }

        [HttpGet]
        public ActionResult EditPromotion(int promotionId)
        {
            
            PromotionModel vm = new PromotionModel();
            vm = DataAccess.GetPromotion(promotionId);
            List<Brand> brands = DataAccess.GetBrandDropDownListData();
            vm.BrandsList = ListToDropDownList<Brand>(brands, "BrandId", "BrandName");
            return View("AddPromotion", vm);
        }


        [HttpPost]
        public ActionResult AddPromotionImages()
        {
            int id = DataManager.ToInt(Request.Form["id"]);
            string idStr = id > 0 ? id.ToString() : "Temp";
            string uploadDir = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
            string productDir = Path.Combine(Server.MapPath("~/" + uploadDir), "Promotion", idStr);
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

                newImage.Dispose();
                newImage = null;
                img.Dispose();
                img = null;

                return Content(fileName);
            }
            else
            {
                throw new Exception("Can't save file please try again.");
            }
        }

        [HttpPost]
        public ActionResult LoadPromotionImages(int? promotionId)
        {
            try
            {
                string uploadDir = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
                //List<DLGCardStyle> itemImages = DataAccess.GetCardByCardId(id);
                PromotionModel promotionImages = DataAccess.GetPromotion(promotionId);
                

                return Json(new
                {
                    isSuccess = true,
                    PromotionImages = promotionImages.PromotionImage
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
        public ActionResult LoadPromotionBrandImages(int promotionId)
        {
            try
            {
                string uploadDir = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
                //List<DLGCardStyle> itemImages = DataAccess.GetCardByCardId(id);
                PromotionModel promotionImages = DataAccess.GetPromotion(promotionId);


                return Json(new
                {
                    isSuccess = true,
                    PromotionImages = promotionImages.PromotionBrandImage
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

        [HttpGet]
        public JsonResult LoadDataPromotionJSON(jQueryDataTableParamModel param)
        {
            try
            {

                IEnumerable<object> result = DoLoadDataJSON(param);
                var list = result.Cast<PromotionModel>().ToArray();
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

        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<PromotionModel> promotion = DataAccess.GetAllPromotion(param.start, param.length, null, null, param.search);
            return promotion;
        }

        protected override bool DoDelete(int promotionId)
        {
            return DataAccess.DeletePromotionAdmin(promotionId) > 0;

        }


    }

}
