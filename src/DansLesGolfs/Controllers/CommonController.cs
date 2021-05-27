using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using DansLesGolfs.Models;
using DansLesGolfs.Base;
using System.Runtime.Caching;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DansLesGolfs.Controllers
{
    public class CommonController : BaseFrontController
    {
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
                logger.Error(ex);
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
                logger.Error(ex);
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
                logger.Error(ex);
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
                logger.Error(ex);
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
                List<State> states = DataAccess.GetStatesListByCountryId(countryId);
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
                logger.Error(ex);
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
                List<Region> regions = DataAccess.GetRegionsListByCountryId(countryId);
                regions.Insert(0, new Region
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
                logger.Error(ex);
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
                logger.Error(ex);
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
                logger.Error(ex);
                return Json(new
                {
                    isResult = false,
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
                logger.Error(ex);
                return Json(new
                {
                    isResult = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult AjaxGetCartItems()
        {
            try
            {
                List<object> cartItems = new List<object>();
                foreach (CartItem ci in ShoppingCart.Instance.Items)
                {
                    string imageUrl = DataAccess.GetImageThumbnailByItemId(ci.Item.ItemId);


                    if (ci.DlgCardStyleId > 0)
                    {
                        imageUrl = this.Url.Content("~/Uploads/Cards/" + ci.Item.ItemId + "/" + ci.Item.ItemImages[0].ImageName);
                    }
                    else
                    {
                        imageUrl = this.Url.Content("~/Uploads/Products/" + ci.Item.ItemId + "/" + imageUrl);
                    }

                    cartItems.Add(new
                    {
                        itemId = ci.ItemId,
                        itemName = ci.Description,
                        itemImage = imageUrl,
                        itemSlug = ci.Item.ItemSlug,
                        quantity = ci.Quantity,
                        unitPrice = ci.UnitPrice,
                        totalPrice = ci.TotalPrice
                    });
                }
                return Json(new
                {
                    isSuccess = true,
                    cartItems = cartItems,
                    cartTotalPrice = ShoppingCart.Instance.GetSubTotal(),
                    currency = "&euro;"
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new
                {
                    isSuccess = false,
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
                logger.Error(ex);
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
                logger.Error(ex);
                return Json(new
                {
                    isResult = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [OutputCache(CacheProfile = "Medium")]
        public JsonResult AjaxGetFooterTabInfomations()
        {
            try
            {
                List<WebContent> contents = null;
                contents = DataAccess.GetAllWebContent("FOOTER", this.CultureId);
                //if(MemoryCache.Default["FooterContents"] == null)
                //{
                //    contents = DataAccess.GetAllWebContent("FOOTER", this.CultureId);
                //    MemoryCache.Default.Add("FooterContents", contents, DateTime.Now.AddHours(1));
                //}
                //else
                //{
                //    contents = MemoryCache.Default["FooterContents"] as List<WebContent>;
                //}
                return Json(new
                {
                    isSuccess = true,
                    list = contents
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new
                {
                    isResult = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [OutputCache(CacheProfile = "Medium")]
        public JsonResult AjaxGetFooterTabCategory()
        {
            try
            {
                List<ItemTypeCategory> sites = DataAccess.GetWebContentCategory(this.CultureId);
                List<Tuple<string, ItemTypeCategory>> tp_sites = new List<Tuple<string, ItemTypeCategory>>();

                foreach (var item in sites)
                {
                    tp_sites.Add(new Tuple<string, ItemTypeCategory>(item.ItemTypeName, item));

                }


                var data = sites.ToLookup(d => d.ItemTypeName, d => d).ToDictionary(g => g.Key, g => g.ToList());

                return Json(new
                {
                    isSuccess = true,
                    list = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new
                {
                    isResult = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #region AjaxNewsletterSubscribe
        [HttpPost]
        public JsonResult AjaxNewsletterSubscribe(string email)
        {
            try
            {
                var pattern = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";
                if (Regex.IsMatch(email, pattern))
                {
                    var subscribe = DataAccess.SubScribeCustomer(email);
                    string message = string.Empty;
                    if (subscribe > 0)
                    {
                        string siteUrl = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"];
                        message = String.Format(Resources.Resources.ThankYouForSubscription, siteUrl);
                    }
                    else if (subscribe == -2)
                    {
                        message = Resources.Resources.YourEmailIsAlreadyInUse;
                    }
                    else
                    {
                        message = Resources.Resources.YourEmailIsInvalid;
                    }
                    return Json(new
                    {
                        isSuccess = true,
                        data = subscribe,
                        message = message
                    });
                }

                return Json(new
                {
                    isSuccess = false,
                    data = "Error"
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        private void SubscriptionPostProcess(string email)
        {
            SendNewsletterWelcomeMsg(email);
            AddEmailToECM(email);
        }

        [HttpPost]
        public JsonResult AjaxNewsletterSubscribePostProcess(string email)
        {
            try
            {
                SendNewsletterWelcomeMsg(email);
                AddEmailToECM(email);

                return Json(new
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        private void AddEmailToECM(string email)
        {
            DataAccess.ChangeConnectionString("ECMConnectionString");
            DataAccess.AddSubscriber(email);
        }

        private void SendNewsletterWelcomeMsg(string email)
        {
            string senderName = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
            string senderEmail = System.Configuration.ConfigurationManager.AppSettings["SiteEmail"].ToString();
            ViewBag.ReceiverEmail = email;
            EmailTemplate template = DataAccess.GetEmailTemplateByKey("newsletter-welcome", this.CultureId);
            template.HtmlDetail = PersonalizeText(template.HtmlDetail);
            string textMsg = PersonalizeText(template.TextDetail);
            EmailArguments emailArgs = EmailArguments.GetInstanceFromConfig();
            emailArgs.SenderEmail = senderEmail;
            emailArgs.SenderName = senderName;
            emailArgs.Subject = template.Subject;
            emailArgs.To.Add(email, email);
            //emailArgs.BCC.Add(senderEmail, senderEmail);
            emailArgs.MailBody = GetHTMLFromView("~/Views/_Shared/UC/EmailTemplates/WelcomeEmail.cshtml", template);
            emailArgs.MailBodyText = textMsg;
            EmailHelper.SendEmailWithAttachments(emailArgs);
        }
        #endregion

        [HttpPost]
        public JsonResult AjaxRegisterFacebookUser(string id, string username, string email, string password, string first_name, string last_name, string gender, string birthday)
        {
            try
            {
                if (Auth.User != null)
                {
                    if (email.Equals(Auth.User.Email))
                    {
                        DataAccess.UpdateConnectWithFacebook(Auth.User.UserId, id);
                    }
                    return Json(new
                    {
                        isSuccess = true
                    });
                }
                else
                {
                    if (String.IsNullOrEmpty(email) || String.IsNullOrWhiteSpace(email))
                    {
                        throw new Exception(Resources.Resources.PasswordCantBeEmpty);
                    }
                    if (String.IsNullOrEmpty(password) || String.IsNullOrWhiteSpace(password))
                    {
                        throw new Exception(Resources.Resources.PasswordCantBeEmpty);
                    }
                    DansLesGolfs.BLL.User user = new User();
                    user.Email = email;
                    user.Password = password;
                    user.PasswordEncrypted = DataProtection.Encrypt(password);
                    user.FirstName = first_name;
                    user.LastName = last_name;
                    user.Gender = gender == "female" ? 1 : 0;
                    user.UpdateDate = user.InsertDate = user.RegisteredDate = user.LastLoggedOn = DateTime.Now;
                    user.ExpiredDate = user.RegisteredDate.Value.AddYears(100);
                    user.Active = true;
                    user.IsReceiveEmailInfo = false;
                    user.IsSubscriber = false;
                    user.UserTypeId = (int)UserType.Type.Customer;
                    user.FBAccount = id;

                    // Birthdate.
                    DateTime birthDate = DateTime.Today;
                    DateTime.TryParseExact(birthday, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate);
                    user.Birthdate = birthDate;

                    user.UserId = DataAccess.AddUser(user);

                    if (user.UserId > 0)
                    {
                        Auth.SetSessionByUserObject(user);
                        SendNewRegisteredUserEmail(user);
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
                            message = Resources.Resources.RegisterFailedMessage
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
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
            emailArgs.To.Add(user.Email, user.Fullname);
            emailArgs.BCC.Add(siteEmail, siteEmail);
            emailArgs.MailBody = GetHTMLFromView("~/Views/_Shared/UC/EmailTemplates/Register.cshtml", user);
            EmailHelper.SendEmailWithAttachments(emailArgs);
        }

        public JsonResult AjaxLoadFooterTabImprint()
        {
            try
            {
                var data = DataAccess.GetWebContentByKey("MENTIONSLEGALES", this.CultureId, "");
                return Json(new
                {
                    isSuccess = true,
                    list = data.ContentText
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new
                {
                    isResult = false,
                    message = ex.Message
                });
            }
        }

        [OutputCache(CacheProfile = "Medium")]
        public JsonResult AjaxLoadFooterTabContract()
        {
            try
            {
                var data = DataAccess.GetWebContentByKey("Contract", this.CultureId, "");
                return Json(new
                {
                    isSuccess = true,
                    list = data.ContentText
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new
                {
                    isResult = false,
                    message = ex.Message
                });
            }
        }

        public JsonResult SentEmailContract(string name, string email, string subject, string message)
        {

            try
            {

                ContractFooter contract = new ContractFooter();
                contract.Name = name;
                contract.EmailAddress = email;
                contract.Subject = subject;
                contract.Message = message;

                string siteUrl = System.Configuration.ConfigurationManager.AppSettings["SiteEmail"].ToString();
                string siteEmail = System.Configuration.ConfigurationManager.AppSettings["SiteEmail"].ToString();

                ViewBag.ReceiverEmail = siteEmail;
                EmailArguments emailArgs = EmailArguments.GetInstanceFromConfig();
                emailArgs.SenderName = name;
                emailArgs.SenderEmail = email;
                emailArgs.Subject = subject;
                emailArgs.To.Add(siteEmail, siteUrl);
                emailArgs.MailBody = GetHTMLFromView("~/Views/_Shared/UC/EmailTemplates/ContractFooter.cshtml", contract);
                EmailHelper.SendEmailWithAttachments(emailArgs);

                return Json(new
                {
                    isSuccess = true,
                    message = Resources.Resources.ThankYouYourMessageHasBeenSent
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new
                {
                    isResult = false,
                    message = ex.Message
                });
            }
        }

        public string kidtest()
        {
            return "kid";
        }

        [HttpPost]
        public ActionResult AjaxGenerateItemCode(int? itemTypeId, int? itemCategoryId = null)
        {
            try
            {
                if (!itemTypeId.HasValue)
                    throw new Exception("Please specific itemTypeId.");

                string itemCode = DataAccess.GetItemRunningNumber(itemTypeId.Value, 6, itemCategoryId);
                return Json(new
                {
                    isSuccess = true,
                    itemCode = itemCode
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
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

                List<Region> regions = null;
                if (MemoryCache.Default["Countries"] == null)
                {
                    regions = DataAccess.GetAllRegions();
                    MemoryCache.Default.Add("Regions", regions, DateTime.Now.AddDays(1));
                }
                else
                {
                    regions = (List<Region>)MemoryCache.Default["Regions"];
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
                    cityId = cityId,
                    countryId = countryId,
                    cities = filteredCities
                });
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public ActionResult AjaxGetHomepageSlider()
        {
            try
            {
                List<SlideImage> slideImages = DataAccess.GetSlideImages();
                foreach (SlideImage image in slideImages)
                {
                    if(!String.IsNullOrWhiteSpace(image.ImageName))
                        image.ImageUrl = Url.Content("~/Uploads/Slideshow/" + image.ImageName);
                }
                string html = GetHTMLFromView("~/Views/_Shared/UC/Front/SlideImages.cshtml", slideImages);
                return Json(new
                {
                    isSuccess = true,
                    html = html
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
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
            if(String.IsNullOrWhiteSpace(returnUrl))
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
        [OutputCache(CacheProfile="Long")]
        public ActionResult FBChannel()
        {
            return Content("<script src=\"//connect.facebook.net/en_US/all.js\"></script>", "text/html");
        }
        #endregion
    }
}
