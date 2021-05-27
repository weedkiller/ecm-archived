using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using DansLesGolfs.ECM.Models;
using DansLesGolfs.Base;
using System.Runtime.Caching;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;

namespace DansLesGolfs.ECM.Controllers
{
    public class CommonController : BaseFrontController
    {

        public ActionResult Test()
        {
            var chronogolf = Data.DataFactory.GetChronogolfInstance(127);
            var data = chronogolf.GetAffiliateTypes();
            return Json(new
            {
                success = true,
                data
            }, JsonRequestBehavior.AllowGet);
        }

        #region AJax Method

        [HttpPost]
        public JsonResult CheckLogin()
        {
            try
            {
                return Json(new
                {
                    isSuccess = true,
                    isLoggedIn = Auth.Check()
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

        [HttpPost]
        public JsonResult AjaxLogin(string username, string password, bool remember = false)
        {
            try
            {
                if (Auth.Attempt(username, password, remember))
                {
                    return Json(new
                    {
                        isSuccess = true
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = Resources.Resources.InvalidUsernameOrPassword
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
        public JsonResult AjaxGetSitesByCountryId(int countryId = 0)
        {
            try
            {
                List<Site> sites = DataAccess.GetSitesListByCountryId(countryId);
                sites.Insert(0, new Site
                {
                    SiteId = 0,
                    SiteName = Resources.Resources.SelectDepartment
                });
                return Json(new
                {
                    isResult = true,
                    list = sites
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
        public JsonResult AjaxGetSitesByStateId(int stateId = 0, int regionId = 0, int countryId = 0)
        {
            try
            {
                List<Site> sites = DataAccess.GetSitesListByStateId(stateId, regionId, countryId, this.CultureId);
                sites.Insert(0, new Site
                {
                    SiteId = 0,
                    SiteName = Resources.Resources.SelectSite
                });
                return Json(new
                {
                    isResult = true,
                    list = sites
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
        public JsonResult AjaxGetStatesByCountryId(int countryId = 0)
        {
            try
            {
                List<State> list = DataAccess.GetStatesListByCountryId(countryId);
                list.Insert(0, new State
                {
                    StateId = 0,
                    StateName = Resources.Resources.SelectState
                });
                return Json(new
                {
                    isResult = true,
                    list
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
        public JsonResult AjaxGetRegionsByCountryId(int countryId = 0)
        {
            try
            {
                List<DansLesGolfs.BLL.Region> regions = DataAccess.GetRegionsListByCountryId(countryId);
                regions.Insert(0, new DansLesGolfs.BLL.Region
                {
                    RegionId = 0,
                    RegionName = Resources.Resources.SelectRegion
                });
                return Json(new
                {
                    isResult = true,
                    list = regions
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
        public JsonResult AjaxGetStatesByRegionId(int? regionId = 0, int? countryId = 1)
        {
            try
            {
                List<State> states = DataAccess.GetStatesListByRegionId(regionId.Value, countryId.Value);
                states.Insert(0, new State
                {
                    StateId = 0,
                    StateName = Resources.Resources.SelectState
                });
                return Json(new
                {
                    isResult = true,
                    list = states
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
        public JsonResult AjaxGetCoursesBySiteId(long siteId = 0)
        {
            try
            {
                List<Course> courses = DataAccess.GetCoursesListBySiteId(siteId);
                courses.Insert(0, new Course
                {
                    CourseId = 0,
                    CourseName = Resources.Resources.SelectCourse
                });
                return Json(new
                {
                    isResult = true,
                    list = courses
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

        public JsonResult AjaxGetAllFooter()
        {
            try
            {
                ////List<Site> sites = DataAccess.GetSitesListByCountryId(countryId);
                ////sites.Insert(0, new Site
                ////{
                ////    SiteId = 0,
                ////    SiteName = Resources.Resources.SelectDepartment
                ////});
                return Json(new
                {
                    isResult = true,
                    //list = sites
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

        [OutputCache(CacheProfile = "Medium")]
        public JsonResult AjaxGetSiteFooter()
        {
            try
            {
                List<Site> sites = DataAccess.GetSiteForFooter(this.CultureId);
                return Json(new
                {
                    isSuccess = true,
                    list = sites
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isResult = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private void SendNewRegisteredUserEmail(User user)
        {
            string siteUrl = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
            string siteEmail = System.Configuration.ConfigurationManager.AppSettings["SiteEmail"].ToString();
            ViewBag.ReceiverEmail = user.Email;
            EmailArguments emailArgs = EmailArguments.GetInstanceFromConfig();
            emailArgs.SenderEmail = siteEmail;
            emailArgs.SenderName = siteUrl;
            emailArgs.Subject = Resources.Resources.RecoverPassword;
            emailArgs.To.Add(user.Email, user.FullName);
            emailArgs.BCC.Add(siteEmail, siteEmail);
            emailArgs.MailBody = GetHTMLFromView("~/Views/_Shared/UC/EmailTemplates/Register.cshtml", user);
            EmailHelper.SendEmailWithAttachments(emailArgs);
        }

        [HttpPost]
        public ActionResult GetDataByPostalCode(string postalCode)
        {
            try
            {
                int cityId = 0;
                int countryId = 0;
                List<City> cities = null;
                if (MemoryCache.Default["Cities"] == null)
                {
                    cities = DataAccess.GetAllCities();
                    MemoryCache.Default.Add("Cities", cities, DateTime.Now.AddDays(1));
                }
                else
                {
                    cities = (List<City>)MemoryCache.Default["Cities"];
                }

                List<DansLesGolfs.BLL.Region> regions = null;
                if (MemoryCache.Default["Countries"] == null)
                {
                    regions = DataAccess.GetAllRegions();
                    MemoryCache.Default.Add("Regions", regions, DateTime.Now.AddDays(1));
                }
                else
                {
                    regions = (List<DansLesGolfs.BLL.Region>)MemoryCache.Default["Regions"];
                }

                List<Country> countries = null;
                if (MemoryCache.Default["Countries"] == null)
                {
                    countries = DataAccess.GetAllCountries();
                    MemoryCache.Default.Add("Countries", countries, DateTime.Now.AddDays(1));
                }
                else
                {
                    countries = (List<Country>)MemoryCache.Default["Countries"];
                }

                var result = from it in cities
                             join r in regions on it.RegionId equals r.RegionId into rjoin
                             from r in rjoin.DefaultIfEmpty()
                             join c in countries on r.CountryId equals c.CountryId into cjoin
                             from c in cjoin.DefaultIfEmpty()
                             where it.PostalCode == postalCode
                             select new
                             {
                                 city = it,
                                 region = r,
                                 country = c
                             };

                List<City> filteredCities = new List<City>();
                if (result != null && result.Any())
                {
                    cityId = result.First().city.CityId;
                    countryId = result.First().country.CountryId;
                    foreach (var entry in result.ToList())
                    {
                        filteredCities.Add(entry.city);
                    }
                }
                else
                {
                    filteredCities.Insert(0, new City()
                    {
                        CityId = 0,
                        CityName = Resources.Resources.PleaseEnterPostalCode
                    });
                    cityId = 0;
                }

                var jsonResult = Json(new
                {
                    isSuccess = true,
                    cityId,
                    countryId,
                    cities = filteredCities
                });
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
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
        public JsonResult GetEmailTemplateImages()
        {
            List<dynamic> data = new List<dynamic>();
            string path = "~/Uploads/EmailTemplates/Global";
            string uploadDir = Server.MapPath(path);
            if (Directory.Exists(uploadDir))
            {
                foreach (string file in Directory.GetFiles(uploadDir))
                {
                    if (System.IO.File.Exists(file))
                    {
                        try
                        {
                            using (Bitmap bitmap = new Bitmap(file))
                            {
                                data.Add(new
                                {
                                    type = "image",
                                    src = Url.ServerUrl(path + "/" + Path.GetFileName(file)),
                                    width = bitmap.Width,
                                    height = bitmap.Height
                                });
                            }
                        }
                        catch (Exception)
                        {
                            // Just skip.
                        }
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(uploadDir);
            }
            return Json(new
            {
                isSuccess = true,
                data
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteEmailTemplateImage(string file = "")
        {
            List<string> data = new List<string>();
            string path = Server.MapPath("~/Uploads/EmailTemplates/Global");
            string fileName = Path.GetFileName(file);
            string filePath = Path.Combine(path, fileName);
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    @System.IO.File.Delete(filePath);
                }
                catch (Exception)
                {
                    // Do nothing.
                }
            }

            return Json(new
            {
                isSuccess = true,
            });
        }

        public JsonResult UploadEmailTemplateImages()
        {
            string path = "~/Uploads/EmailTemplates/Global";
            string uploadDir = Server.MapPath(path);
            string tempFileName = string.Empty;
            string savePath = string.Empty;
            string extension = string.Empty;
            List<dynamic> data = new List<dynamic>();

            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            foreach (String fileKey in Request.Files.AllKeys)
            {
                savePath = Path.Combine(uploadDir, Request.Files[fileKey].FileName);
                Request.Files[fileKey].SaveAs(savePath);

                if (System.IO.File.Exists(savePath))
                {
                    using (Bitmap bitmap = new Bitmap(savePath))
                    {
                        data.Add(new
                        {
                            type = "image",
                            src = Url.ServerUrl(path + "/" + Request.Files[fileKey].FileName),
                            width = bitmap.Width,
                            height = bitmap.Height
                        });
                    }
                }
            }

            return Json(new
            {
                isSuccess = true,
                data
            });
        }

        public JsonResult GetEmailingImages(long? id = null)
        {
            List<dynamic> data = new List<dynamic>();
            string path = "~/Uploads/Emailing" + (id.HasValue ? "/" + id.ToString() : "");
            string uploadDir = Server.MapPath(path);
            if (Directory.Exists(uploadDir))
            {
                foreach (string file in Directory.GetFiles(uploadDir))
                {
                    if (System.IO.File.Exists(file))
                    {
                        using (Bitmap bitmap = new Bitmap(file))
                        {
                            data.Add(new
                            {
                                type = "image",
                                src = Url.ServerUrl(path + "/" + Path.GetFileName(file)),
                                width = bitmap.Width,
                                height = bitmap.Height
                            });
                        }
                    }
                }
            }
            return Json(new
            {
                isSuccess = true,
                data
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UploadEmailingImages(long? id = null)
        {
            string path = "~/Uploads/Emailing" + (id.HasValue ? "/" + id.ToString() : "");
            string uploadDir = Server.MapPath(path);
            string tempFileName = string.Empty;
            string savePath = string.Empty;
            string extension = string.Empty;
            List<dynamic> data = new List<dynamic>();

            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            foreach (String fileKey in Request.Files.AllKeys)
            {
                savePath = Path.Combine(uploadDir, Request.Files[fileKey].FileName);
                Request.Files[fileKey].SaveAs(savePath);

                if (System.IO.File.Exists(savePath))
                {
                    using (Bitmap bitmap = new Bitmap(savePath))
                    {
                        data.Add(new
                        {
                            type = "image",
                            src = Url.ServerUrl(path + "/" + Request.Files[fileKey].FileName),
                            width = bitmap.Width,
                            height = bitmap.Height
                        });
                    }
                }
            }

            return Json(new
            {
                isSuccess = true,
                data
            });
        }

        #endregion

        #region UpdateUserPassword
        public ActionResult UpdateUserPassword()
        {
            DataAccess.UpdateUserPassword();
            return Content("1");
        }
        #endregion

        #region ClearCache
        public ActionResult ClearCache(string returnUrl)
        {
            System.Runtime.Caching.MemoryCache.Default.Dispose();

            InMemoryCache cache = new InMemoryCache("WebSiteCache");
            cache.Clear();

            OutputCacheAttribute.ChildActionCache = new MemoryCache("SiteMemoryCache");
            if (String.IsNullOrWhiteSpace(returnUrl))
            {
                return Content("1");
            }
            else
            {
                return Redirect(returnUrl);
            }
        }
        #endregion

        #region Facebook Custom Channel URL
        [OutputCache(CacheProfile = "Long")]
        public ActionResult FBChannel()
        {
            return Content("<script src=\"//connect.facebook.net/en_US/all.js\"></script>", "text/html");
        }
        #endregion
    }
}
