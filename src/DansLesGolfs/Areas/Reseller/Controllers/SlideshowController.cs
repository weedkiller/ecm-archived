using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Reseller.Controllers
{
    public class SlideshowController : BaseResellerController
    {
        public ActionResult Index()
        {
            ViewBag.Title = Resources.Resources.Slideshow;
            ViewBag.TitleName = Resources.Resources.Slideshow;
            Breadcrumbs.Add("Slideshow", "~/Reseller/Slideshow");
            InitBreadcrumbs();

            return View("Form");
        }

        public ActionResult LoadSlideImages()
        {
            try
            {
                string uploadDir = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
                List<SlideImage> images = DataAccess.GetSlideImages();
                foreach (SlideImage image in images)
                {
                    image.ImageUrl = Url.Content("~/" + uploadDir + "/Slideshow/" + image.ImageName);
                }
                return Json(new
                {
                    isSuccess = true,
                    images = images
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
        public ActionResult AddImages()
        {
            string uploadDir = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
            string slideshowDir = Path.Combine(Server.MapPath("~/" + uploadDir), "Slideshow");
            string imagePath = string.Empty;
            string imageUrl = string.Empty;

            var file = Request.Files["Filedata"];
            imagePath = Path.Combine(slideshowDir, file.FileName);
            imageUrl = Url.Content("~/" + uploadDir + "/Slideshow/" + file.FileName);
            if (System.IO.File.Exists(imagePath))
            {
                @System.IO.File.Delete(imagePath);
            }
            file.SaveAs(imagePath);

            if (System.IO.File.Exists(imagePath))
            {
                return Content(file.FileName + "," + Url.Content(imageUrl));
            }
            else
            {
                throw new Exception("Can't save file please try again.");
            }
        }

        public ActionResult AjaxSaveAllSlideImages(string[] imageNames, string[] descriptions, string[] linkUrls)
        {
            try
            {
                DataAccess.DeleteAllSlideImages();
                if (DataAccess.AddSlideImages(imageNames, descriptions, linkUrls))
                {
                    return Json(new
                    {
                        isSuccess = true,
                        message = Resources.Resources.SaveSlideImagesSuccessful
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = Resources.Resources.SaveSlideImagesFailed
                    });
                }
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
    }
}
