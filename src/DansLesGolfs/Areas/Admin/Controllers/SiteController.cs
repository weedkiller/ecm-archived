using DansLesGolfs.Albatros;
using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Admin.Controllers
{
    public class SiteController : BaseAdminCRUDController
    {
        #region Fields
        private bool isNew = false;
        #endregion

        #region Constructor
        public SiteController()
        {
            ObjectName = "Site";
            TitleName = "Site";
            PrimaryKey = "SiteId";

            // Define Column Names.
            ColumnNames.Add("SiteName", Resources.Resources.SiteName);
            ColumnNames.Add("YearlyJournal", Resources.Resources.YearlyJournal);
            ColumnNames.Add("DailyJournal", Resources.Resources.DailyJournal);
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<Site> models = DataAccess.GetAllSites(param, this.CultureId);
            return models;
        }

        protected override string RowCellDisplayText(string columnName, object value, object dataItem)
        {
            if (columnName == "YearlyJournal")
            {
                Site site = dataItem as Site;
                return "<a href=\"" + Url.Content("~/Admin/SiteJournal/Yearly/" + site.SiteId) + "\" target=\"_blank\" />" + Resources.Resources.YearlyJournal + "</a>";
            }
            else if (columnName == "DailyJournal")
            {
                Site site = dataItem as Site;
                return "<a href=\"" + Url.Content("~/Admin/SiteJournal/Daily/" + site.SiteId) + "\" target=\"_blank\" />" + Resources.Resources.DailyJournal + "</a>";
            }
            else
            {
                return base.RowCellDisplayText(columnName, value, dataItem);
            }
        }

        protected override void DoPrepareForm(int? id)
        {
            List<GolfBrand> golfBrands = DataAccess.GetGolfBrandDropDownListData();
            ViewBag.DropDownGolfBrands = ListToDropDownList<GolfBrand>(golfBrands, "GolfBrandId", "GolfBrandName");

            List<ReservationAPI> apiList = DataAccess.GetAllReservationAPIs();
            ViewBag.DropDownGolfAPIs = ListToDropDownList<ReservationAPI>(apiList, "APIId", "APIName");

            List<SelectListItem> listVisible = new List<SelectListItem>();
            listVisible.Add(new SelectListItem()
            {
                Text = Resources.Resources.Show,
                Value = "True"
            });
            listVisible.Add(new SelectListItem()
            {
                Text = Resources.Resources.Hidden,
                Value = "False"
            });
            ViewBag.VisibleDropDownList = listVisible;

            List<SelectListItem> listManagement = new List<SelectListItem>();
            listManagement.Add(new SelectListItem()
            {
                Text = Resources.Resources.Direct,
                Value = "D"
            });
            listManagement.Add(new SelectListItem()
            {
                Text = Resources.Resources.Subcontracting,
                Value = "S"
            });
            ViewBag.ManagementFlagDropDownList = listManagement;

            ViewBag.DefaultLatitude = DataManager.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["DefaultLatitude"], 47);
            ViewBag.DefaultLongitude = DataManager.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["DefaultLongitude"], 2);


        }

        protected override object DoPrepareNew()
        {
            isNew = true;
            Site site = new Site()
            {
                Visible = true,
                Latitude = DataManager.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["DefaultLatitude"], 47),
                Longitude = DataManager.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["DefaultLongitude"], 2)
            };
            LoadDropDownLists(site);
            ViewBag.DropDownAlbatrosCourses = new List<SelectListItem>();

            return site;
        }

        protected override object DoPrepareEdit(long id)
        {
            isNew = false;
            Site site = DataAccess.GetSiteById(id);
            LoadDropDownLists(site);

            //LoadAlbatrosSites(site.AlbatrosUrl, site.AlbatrosUsername, site.AlbatrosPassword, site.AlbatrosCourseId);
            return site;
        }

        protected override bool DoSave()
        {
            int result = -1;
            int id = DataManager.ToInt(Request.Form["id"]);
            Site model = null;
            if (id > 0)
            {
                model = DataAccess.GetSite(id);
                if (model == null)
                {
                    model = new Site();
                }
            }
            else
            {
                model = new Site();
                isNew = true;
            }
            model.SiteId = id;
            model.SiteSlug = DataManager.ToString(Request.Form["SiteSlug"]).Trim();
            model.GolfBrandId = DataManager.ToInt(Request.Form["GolfBrandId"]);
            model.Address = DataManager.ToString(Request.Form["Address"]).Trim();
            //model.Street = DataManager.ToString(Request.Form["Street"]).Trim();
            model.City = DataManager.ToString(Request.Form["City"]).Trim();
            model.PostalCode = DataManager.ToString(Request.Form["PostalCode"]).Trim();
            model.CountryId = DataManager.ToInt(Request.Form["CountryId"]);
            model.StateId = DataManager.ToInt(Request.Form["StateId"]);
            model.RegionId = DataManager.ToInt(Request.Form["RegionId"]);
            model.Phone = DataManager.ToString(Request.Form["Phone"]).Trim();
            model.Fax = DataManager.ToString(Request.Form["Fax"]).Trim();
            model.Website = DataManager.ToString(Request.Form["Website"]).Trim();
            model.Email = DataManager.ToString(Request.Form["Email"]).Trim();
            model.FB = DataManager.ToString(Request.Form["FB"]).Trim();
            model.Latitude = DataManager.ToDecimal(Request.Form["Latitude"]);
            model.Longitude = DataManager.ToDecimal(Request.Form["Longitude"]);
            model.ReservationAPI = DataManager.ToInt(Request.Form["ReservationAPI"]);
            model.PrimaAPIKey = DataManager.ToString(Request.Form["PrimaAPIKey"]);
            model.PrimaClubKey = DataManager.ToString(Request.Form["PrimaClubKey"]);
            model.AlbatrosCourseId = DataManager.ToInt(Request.Form["AlbatrosCourseId"]);
            model.AlbatrosUrl = DataManager.ToString(Request.Form["AlbatrosUrl"]).Trim();
            model.AlbatrosUsername = DataManager.ToString(Request.Form["AlbatrosUsername"]).Trim();
            model.AlbatrosPassword = DataManager.ToString(Request.Form["AlbatrosPassword"]).Trim();
            model.LydiaVendorToken = DataManager.ToString(Request.Form["LydiaVendorToken"]).Trim();
            model.LydiaVendorId = DataManager.ToString(Request.Form["LydiaVendorId"]).Trim();
            model.IsCommercial = DataManager.ToBoolean(Request.Form["IsCommercial"]);
            model.IsAssociative = DataManager.ToBoolean(Request.Form["IsAssociative"]);
            model.IsRegie = DataManager.ToBoolean(Request.Form["IsRegie"]);
            model.Visible = DataManager.ToBoolean(Request.Form["Visible"], true);
            model.UpdateDate = DateTime.Now;

            // Management.
            model.ManageRestaurantFlag = DataManager.ToString(Request.Form["ManageRestaurantFlag"]).Trim();
            model.ManageProshopFlag = DataManager.ToString(Request.Form["ManageProshopFlag"]).Trim();
            model.ManageFieldFlag = DataManager.ToString(Request.Form["ManageFieldFlag"]).Trim();
            model.ManageGolfFlag = DataManager.ToString(Request.Form["ManageGolfFlag"]).Trim();

            // Phone & Email Data.
            model.ManagerPhone = DataManager.ToString(Request.Form["ManagerPhone"]).Trim();
            model.RespReceptionPhone = DataManager.ToString(Request.Form["RespReceptionPhone"]).Trim();
            model.GreenKeeperPhone = DataManager.ToString(Request.Form["GreenKeeperPhone"]).Trim();
            model.RespProshopPhone = DataManager.ToString(Request.Form["RespProshopPhone"]).Trim();
            model.RestaurateurPhone = DataManager.ToString(Request.Form["RestaurateurPhone"]).Trim();
            model.AssociationPresidentPhone = DataManager.ToString(Request.Form["AssociationPresidentPhone"]).Trim();
            model.ManagerEmail = DataManager.ToString(Request.Form["ManagerEmail"]).Trim();
            model.RespReceptionEmail = DataManager.ToString(Request.Form["RespReceptionEmail"]).Trim();
            model.GreenKeeperEmail = DataManager.ToString(Request.Form["GreenKeeperEmail"]).Trim();
            model.RespProshopEmail = DataManager.ToString(Request.Form["RespProshopEmail"]).Trim();
            model.RestaurateurEmail = DataManager.ToString(Request.Form["RestaurateurEmail"]).Trim();
            model.AssociationPresidentEmail = DataManager.ToString(Request.Form["AssociationPresidentEmail"]).Trim();

            // Commercial Information.
            model.CommercialNBSubscriber = DataManager.ToString(Request.Form["CommercialNBSubscriber"]).Trim();
            model.CommercialNBGF = DataManager.ToString(Request.Form["CommercialNBGF"]).Trim();
            model.CommercialTurnOver = DataManager.ToString(Request.Form["CommercialTurnOver"]).Trim();
            model.CommercialReservationSystem = DataManager.ToString(Request.Form["CommercialReservationSystem"]).Trim();
            
            // Hotel Information.
            model.HotelOnSite = DataManager.ToBoolean(Request.Form["HotelOnSite"], false);
            model.HotelPartner = DataManager.ToBoolean(Request.Form["HotelPartner"], false);

            if (Auth.User != null)
                model.UserId = Auth.User.UserId;

            if (id > 0)
            {
                model.InsertDate = model.InsertDate == DateTime.MinValue ? model.UpdateDate : model.InsertDate;
                result = DataAccess.UpdateSite(model);
            }
            else
            {
                model.InsertDate = model.UpdateDate;
                model.Active = true;
                result = DataAccess.AddSite(model);
                model.SiteId = result;
            }
            ViewBag.id = result > -1 ? model.SiteId : -1;
            return result > 0;
        }

        protected override void DoSaveSuccess(int id)
        {
            string[] siteNames = Request.Form.GetValues("SiteName");
            string[] descriptions = Request.Form.GetValues("Description");
            string[] practicalInfos = Request.Form.GetValues("PracticalInfo");
            string[] accommodations = Request.Form.GetValues("Accommodation");
            string[] restaurants = Request.Form.GetValues("Restaurant");
            SiteLang siteLang = null;

            for (int i = 0, j = siteNames.Length; i < j; i++)
            {
                siteLang = new SiteLang();
                siteLang.SiteId = id;
                siteLang.LangId = i + 1;
                siteLang.SiteName = siteNames[i];
                siteLang.Description = descriptions[i];
                siteLang.PracticalInfo = practicalInfos[i];
                siteLang.Accommodation = accommodations[i];
                siteLang.Restaurant = restaurants[i];

                DataAccess.SaveSiteLang(siteLang);
            }

            // Save Site Images
            SaveSiteImages(id);
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteSite(id) > 0;
        }
        #endregion

        #region AJAX Methods
        [HttpPost]
        public ActionResult AddSiteImages()
        {
            int id = DataManager.ToInt(Request.Form["id"]);
            string idStr = id > 0 ? id.ToString() : "Temp";
            string uploadDir = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
            string productDir = Path.Combine(Server.MapPath("~/" + uploadDir), "Sites", idStr);
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
                string fullUrl = Url.Content("~/" + uploadDir + "/Sites/" + idStr + "/" + fileName);
                string thumbUrl = Url.Content("~/" + uploadDir + "/Sites/" + idStr + "/" + baseName + "_t" + extension);
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
        public ActionResult SaveSiteImages(int id, string[] imageIds, string[] fileNames, string[] baseNames, string[] fileExtensions)
        {
            SiteImage obj = new SiteImage();
            try
            {
                obj.SiteId = id;
                for (int i = 0; i < fileNames.Length; i++)
                {
                    obj.SiteImageId = DataManager.ToLong(imageIds[i], -1);
                    obj.ImageName = fileNames[i];
                    obj.BaseName = baseNames[i];
                    obj.FileExtension = fileExtensions[i];
                    obj.ListNo = i;
                    DataAccess.SaveSiteImage(obj);
                }
                return Json(new
                {
                    isSuccess = true,
                    message = "Save Site Images successful."
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
        public ActionResult LoadSiteImages(int? id)
        {
            try
            {
                id = id.HasValue ? id.Value : 0;
                string uploadDir = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
                List<SiteImage> itemImages = DataAccess.GetSiteImagesBySiteId(id.Value);
                List<object> list = new List<object>();
                foreach (var item in itemImages)
                {
                    list.Add(new
                    {
                        SiteImageId = item.SiteImageId,
                        SiteId = item.SiteId,
                        ImageName = item.ImageName,
                        ListNo = item.ListNo,
                        BaseName = item.BaseName,
                        FileExtension = item.FileExtension,
                        ImageUrl = Url.Content("~/" + uploadDir + "/Sites/" + id + "/" + item.ImageName),
                        ThumbnailUrl = Url.Content("~/" + uploadDir + "/Sites/" + id + "/" + item.BaseName + "_t" + item.FileExtension)
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
        public ActionResult DeleteSiteImage(long? siteId = 0, long? siteImageId = 0)
        {
            try
            {
                if (!siteId.HasValue)
                    throw new Exception("Please specific siteId value.");

                if (!siteImageId.HasValue)
                    throw new Exception("Please specific siteImageId value.");

                // Delete Real Image before
                SiteImage img = DataAccess.GetSiteImage(siteImageId.Value);
                if (img != null && DataAccess.DeleteSiteImage(siteImageId.Value) > 0)
                {
                    string uploadDir = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
                    string imageDir = Path.Combine(Server.MapPath("~/" + uploadDir), "Sites", siteId.ToString());
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
        public ActionResult GenerateSiteSlug(string text, long? skipId)
        {
            try
            {
                string slug = GetSiteSlug(text, skipId);
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

        private string GetSiteSlug(string text, long? itemId)
        {
            int num = 1;
            string slug = WebHelper.GenerateSlug(text);

            while (DataAccess.IsExistsSiteSlug(slug, itemId))
            {
                slug = WebHelper.GenerateSlug(text);
                slug = slug + (num++).ToString();
            }
            return slug;
        }

        public ActionResult ConnectAlbatros(long? siteId, string url, string username, string password)
        {
            string html = string.Empty;
            string message = string.Empty;
            bool isSuccess = false;

            Dictionary<string, string> courseList = new Dictionary<string, string>();
            courseList.Add("0", Resources.Resources.SelectAlbatrosSite);
            try
            {
                if (String.IsNullOrEmpty(url) || String.IsNullOrWhiteSpace(url)
                    || String.IsNullOrEmpty(username) || String.IsNullOrWhiteSpace(username)
                    || String.IsNullOrEmpty(password) || String.IsNullOrWhiteSpace(password))
                    throw new Exception("Please specific Albatros Web Service URL, Username and Password");

                Albatros.SetConnection(url, username, password);
                Albatros.Login();
                if (!Albatros.IsLoggedIn)
                {
                    throw new Exception("Couldn't login with your specified data.");
                }
                else
                {
                    var coursesResponse = Albatros.GetReservationCourses(DateTime.Today);

                    if (coursesResponse.code == "0")
                    {
                        if (coursesResponse.groups.group.courses.course is List<object>)
                        {
                            foreach (var course in coursesResponse.groups.group.courses.course)
                            {
                                courseList.Add(course.id, course.name);
                            }
                        }
                        else
                        {
                            courseList.Add(coursesResponse.groups.group.courses.course.id, coursesResponse.groups.group.courses.course.name);
                        }
                    }

                    // Change status
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message = ex.Message;
            }
            finally
            {
                Albatros.Logout();
                foreach (var course in courseList)
                {
                    html += "<option value=\"" + course.Key + "\">" + course.Value + "</option>";
                }
            }

            return Json(new
            {
                isSuccess = isSuccess,
                message = message,
                html = html
            });
        }
        #endregion

        #region Private Methods

        #region LoadDropDownLists
        private void LoadDropDownLists(Site site)
        {
            List<Country> countries = DataAccess.GetAllCountries();
            List<SelectListItem> listCountries = ListToDropDownList<Country>(countries, "CountryId", "CountryName", site.CountryId);
            ViewBag.Countries = listCountries;

            List<DansLesGolfs.BLL.Region> regions = DataAccess.GetAllRegionsByCountryId(site.CountryId > 0 ? site.CountryId : (countries != null && countries.Any() ? countries.First().CountryId : 0));
            List<SelectListItem> listRegions = ListToDropDownList(regions, "RegionId", "RegionName", site.RegionId);
            listRegions.Insert(0, new SelectListItem()
            {
                Value = "",
                Text = Resources.Resources.SelectRegion
            });
            ViewBag.Regions = listRegions;

            List<State> states = DataAccess.GetStatesListByRegionId(site.RegionId);
            List<SelectListItem> listStates = ListToDropDownList(states, "StateId", "StateName", site.StateId);
            listStates.Insert(0, new SelectListItem()
            {
                Value = "",
                Text = Resources.Resources.SelectState
            });
            ViewBag.States = listStates;
        }
        #endregion

        #region LoadAlbatrosSites
        private void LoadAlbatrosSites(string albatrosUrl, string albatrosUsername, string albatrosPassword, int selectedCourseId = 0)
        {
            ViewBag.DropDownAlbatrosCourses = new List<SelectListItem>();

            if (String.IsNullOrEmpty(albatrosUrl) || String.IsNullOrWhiteSpace(albatrosUrl))
            {
                List<SelectListItem> list = new List<SelectListItem>(); ;
                list.Insert(0, new SelectListItem()
                {
                    Text = Resources.Resources.SelectAlbatrosSite,
                    Value = "0"
                });
                ViewBag.DropDownAlbatrosCourses = list;
                return;
            }

            List<object> courseList = new List<object>();
            try
            {
                Albatros.SetConnection(albatrosUrl, albatrosUsername, albatrosPassword);
                Albatros.Login();
                if (Albatros.IsLoggedIn)
                {
                    var coursesResponse = Albatros.GetReservationCourses(DateTime.Today);

                    if (coursesResponse.code == "0")
                    {
                        if (coursesResponse.groups.group.courses.course is List<object>)
                        {
                            foreach (var course in coursesResponse.groups.group.courses.course)
                            {
                                courseList.Add(new
                                {
                                    ID = course.id,
                                    Name = course.name
                                });
                            }
                        }
                        else
                        {
                            courseList.Add(new
                            {
                                ID = coursesResponse.groups.group.courses.course.id,
                                Name = coursesResponse.groups.group.courses.course.name
                            });
                        }
                    }
                    List<SelectListItem> list = ListToDropDownList(courseList, "ID", "Name");
                    list.Insert(0, new SelectListItem()
                    {
                        Text = Resources.Resources.SelectAlbatrosSite,
                        Value = "0"
                    });
                    ViewBag.DropDownAlbatrosCourses = list;
                }
                else
                {
                    ViewBag.DropDownAlbatrosCourses = new List<SelectListItem>();
                    throw new Exception("Can't login with your specific credential, please try again.");
                }
            }
            catch (Exception ex)
            {
                ViewBag.DropDownAlbatrosCourses = new List<SelectListItem>();
                ViewBag.ErrorMessages = ex.Message;
            }
        }
        #endregion

        #region SaveSiteImages
        private void SaveSiteImages(int id)
        {
            if (Request.Form["SiteImageIds"] != null)
            {
                SiteImage siteImage = null;
                string[] SiteImageIds = Request.Form["SiteImageIds"].Split(',');
                string[] ImageNames = Request.Form["ImageNames"].Split(',');
                string[] BaseNames = Request.Form["BaseNames"].Split(',');
                string[] FileExtensions = Request.Form["FileExtensions"].Split(',');
                string[] IsDeletes = Request.Form["IsDelete"].Split(',');

                if (id > 0) // For new data, we have to move images from temp to destination directory.
                {
                    string uploadDir = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
                    string tempDir = Path.Combine(Server.MapPath("~/" + uploadDir), "Sites", "Temp");
                    string destDir = Path.Combine(Server.MapPath("~/" + uploadDir), "Sites", id.ToString());
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

                    for (int i = 0, imagesCount = SiteImageIds.Length; i < imagesCount; i++)
                    {
                        siteImage = new SiteImage()
                        {
                            SiteId = id,
                            SiteImageId = DataManager.ToLong(SiteImageIds[i]),
                            ImageName = ImageNames[i],
                            BaseName = BaseNames[i],
                            FileExtension = FileExtensions[i],
                            ListNo = i
                        };
                        if (siteImage.SiteImageId == 0) // For new site image
                        {
                            tempFilePath = Path.Combine(tempDir, siteImage.ImageName);
                            tempthumbFilePath = Path.Combine(tempDir, siteImage.BaseName + "_t" + siteImage.FileExtension);
                            tempsmallFilePath = Path.Combine(tempDir, siteImage.BaseName + "_s" + siteImage.FileExtension);
                            tempmediumFilePath = Path.Combine(tempDir, siteImage.BaseName + "_m" + siteImage.FileExtension);
                            templargeFilePath = Path.Combine(tempDir, siteImage.BaseName + "_l" + siteImage.FileExtension);
                            destFilePath = Path.Combine(destDir, siteImage.ImageName);
                            thumbFilePath = Path.Combine(destDir, siteImage.BaseName + "_t" + siteImage.FileExtension);
                            smallFilePath = Path.Combine(destDir, siteImage.BaseName + "_s" + siteImage.FileExtension);
                            mediumFilePath = Path.Combine(destDir, siteImage.BaseName + "_m" + siteImage.FileExtension);
                            largeFilePath = Path.Combine(destDir, siteImage.BaseName + "_l" + siteImage.FileExtension);
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
                                DataAccess.SaveSiteImage(siteImage);
                            }
                            catch
                            {

                            }
                        }
                        else // for exists site image
                        {
                            if (IsDeletes[i] == "1")
                            {
                                DeleteSiteImage(id, siteImage.SiteImageId);
                            }
                            else
                            {
                                DataAccess.SaveSiteImage(siteImage);
                            }
                        }
                    }
                }
                else // For update
                {
                    for (int i = 0, imagesCount = SiteImageIds.Length; i < imagesCount; i++)
                    {
                        siteImage = new SiteImage()
                        {
                            SiteId = id,
                            SiteImageId = DataManager.ToLong(SiteImageIds[i]),
                            ImageName = ImageNames[i],
                            BaseName = BaseNames[i],
                            FileExtension = FileExtensions[i],
                            ListNo = i
                        };
                        DataAccess.SaveSiteImage(siteImage);
                    }
                }
            }
        }
        #endregion

        #endregion
    }
}
