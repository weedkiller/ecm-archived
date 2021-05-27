using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using DansLesGolfs.Data;
using DansLesGolfs.Base;
using DansLesGolfs.Models;
using System.Collections;

namespace DansLesGolfs.Controllers
{
    public class ItemAjaxController : BaseController
    {
        [HttpPost]
        public JsonResult GetAnotherItems(string[] exclude, int? pageSize = 10, int? itemTypeId = 0, string searchTerm = "", int? countryId = 0, int? regionId = 0, int? stateId = 0, long? siteId = 0, int? itemCategoryId = 0, int? itemSubCategoryId = 0, int? golfLessonCategoryId = 0, int? timeSlot = 0, bool? includePractice = null, bool? includeAccommodation = null, string departureMonth = "", string fromDate = "", string toDate = "")
        {
            try
            {
                pageSize = pageSize.HasValue ? pageSize.Value : 10;
                itemTypeId = itemTypeId.HasValue && itemTypeId.Value > 0 ? itemTypeId.Value : 0;
                string excludeStr = exclude == null || exclude.Length <= 0 ? String.Empty : String.Join(",", exclude);
                ProductsListModel model = new ProductsListModel();

                int totalPages = 0;
                if (itemTypeId.Value == (int)ItemType.Type.GreenFee)
                {
                    DateTime? fromDateObj = !String.IsNullOrEmpty(fromDate) && !String.IsNullOrWhiteSpace(fromDate) ? (DateTime?)DataManager.ToDateTime(fromDate, "dd/MM/yyyy") : null;
                    DateTime? toDateObj = !String.IsNullOrEmpty(toDate) && !String.IsNullOrWhiteSpace(toDate) ? (DateTime?)DataManager.ToDateTime(toDate, "dd/MM/yyyy") : null;
                    includePractice = includePractice.HasValue && includePractice.Value ? (bool?)true : null;
                    model.Items = DataAccess.SearchGreenFee(out totalPages, pageSize.Value, 0, regionId, stateId, siteId, timeSlot, fromDateObj, toDateObj, itemCategoryId, includePractice, string.Empty, this.CultureId, excludeStr);
                }
                else
                {
                    model.Items = DataAccess.GetLatestItems(out totalPages, pageSize.Value, 0, countryId, regionId, stateId, siteId, searchTerm, includeAccommodation, departureMonth, itemTypeId.Value, itemCategoryId, this.CultureId, excludeStr);
                }
                model.TotalPages = 0;
                model.IsShowNoItemText = false;
                model.IsShowWrapper = false;

                model.Items.Where(it => string.IsNullOrEmpty(it.ItemName) || string.IsNullOrWhiteSpace(it.ItemName)).ToList().ForEach(it =>
                {
                    it.ItemName = Resources.Resources.Untitled;
                });
                string html = GetHTMLFromView("~/Views/_Shared/UC/Front/Item/UCProductList.cshtml", model);
                string[] itemIds = model.Items.Select(it => it.ItemId.ToString()).ToArray();
                int fetchCount = itemIds.Length + (exclude != null ? exclude.Length : 0);
                return Json(new
                {
                    isSuccess = true,
                    html = html,
                    itemIds = itemIds,
                    isFinal = (model.Items.Count < pageSize.Value) || (Math.Ceiling((decimal)fetchCount / pageSize.Value) >= totalPages)
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
        public JsonResult UserGiveRating(long itemId = 0, byte rating = 1, string subject = "", string message = "")
        {
            try
            {
                if (Auth.User == null)
                    throw new Exception("Please login before give a rating.");

                int averageRating = 0, reviewNumber = 0;
                DataAccess.SaveItemReview(itemId, Auth.User.UserId, rating, subject, message, out averageRating, out reviewNumber);
                ItemReview review = new ItemReview()
                {
                    Rating = rating,
                    Message = message
                };
                List<ItemReview> reviews = DataAccess.GetItemReviewsByItemId(itemId);
                string html = GetHTMLFromView("~/Views/_Shared/UC/Front/Item/UCItemReviews.cshtml", reviews);
                return Json(new
                {
                    isSuccess = true,
                    html = html,
                    averageRating = averageRating,
                    reviewNumber = reviewNumber
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

        public JsonResult GetDLGItemDetail(long? itemId = null)
        {
            itemId = itemId.HasValue ? itemId.Value : 0;
            try
            {
                if (Auth.User == null)
                    throw new Exception("Please login before give a rating.");

                Item item = DataAccess.GetItem(itemId.Value);
                if (item != null)
                {
                    string html = GetHTMLFromView("~/Views/_Shared/UC/Front/Item/UCDLGItemPopup.cshtml", item);
                    return Json(new
                    {
                        isSuccess = true,
                        html = html
                    });
                }
                else
                {
                    return Json(new
                    {
                        isResult = false,
                        message = Resources.Resources.ItemNotFound
                    });
                }
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

        public JsonResult GetPriceByDate(long? itemId, int? qty, string date = "")
        {
            try
            {
                if (!itemId.HasValue)
                    throw new Exception("Item ID can't be null.");

                if (!qty.HasValue || qty.Value < 1)
                    qty = 1;

                decimal price = 0, specialPrice = 0, periodPrice = 0, cheapestPrice = 0, cheapestPeriodPrice = 0, teeSheetCheapestPrice = 0;
                DateTime d = DataManager.ToDateTime(date, "d/M/yyyy", DateTime.Today);
                Item item = null;
                string priceTagHTML = string.Empty;
                MvcHtmlString checkOutPriceHTML = null;
                if (DataAccess.GetItemPricesByDate(itemId.Value, d, out price, out periodPrice, out specialPrice, out cheapestPrice, out cheapestPeriodPrice, out teeSheetCheapestPrice))
                {
                    item = new Item()
                    {
                        Price = price,
                        PeriodPrice = periodPrice,
                        SpecialPrice = specialPrice,
                        CheapestPeriodPrice = cheapestPeriodPrice,
                        TeeSheetCheapestPrice = teeSheetCheapestPrice
                    };
                    var htmlHelper = new HtmlHelper(new ViewContext(), new ViewPage());
                    priceTagHTML = GetHTMLFromView("~/Views/_Shared/UC/Front/Item/UCItemPriceTag.cshtml", item);
                    checkOutPriceHTML = htmlHelper.ItemPrice(item, Resources.Resources.NotAvailable, qty.Value);
                }
                else
                {
                    item = new Item()
                    {
                        Price = 0,
                        PeriodPrice = 0,
                        SpecialPrice = 0,
                        CheapestPeriodPrice = 0,
                        TeeSheetCheapestPrice = 0
                    };
                    var htmlHelper = new HtmlHelper(new ViewContext(), new ViewPage());
                    priceTagHTML = GetHTMLFromView("~/Views/_Shared/UC/Front/Item/UCItemPriceTag.cshtml", item);
                    checkOutPriceHTML = htmlHelper.ItemPrice(item, Resources.Resources.NotAvailable, qty.Value);
                }


                return Json(new
                {
                    isSuccess = true,
                    priceTagHTML = priceTagHTML,
                    checkOutPriceHTML = checkOutPriceHTML.ToHtmlString(),
                    price = price,
                    periodPrice = periodPrice,
                    specialPrice = specialPrice,
                    cheapestPrice = cheapestPrice,
                    cheapestPeriodPrice = cheapestPeriodPrice,
                    notAvailableText = Resources.Resources.NotAvailable
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

        public JsonResult SendEmailItemInfo(long? itemId = null, string email = "", string name = "")
        {
            try
            {
                if (String.IsNullOrEmpty(email) || String.IsNullOrWhiteSpace(email))
                    throw new Exception("Please provide receipt's email.");

                if (String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
                    name = email;

                Item item = DataAccess.GetItem(itemId.Value);
                if (item != null)
                {
                    ViewBag.Email = email;
                    ViewBag.Name = name;
                    ViewBag.ReceiverEmail = email;
                    EmailArguments emailArgs = EmailArguments.GetInstanceFromConfig();
                    emailArgs.Subject = Resources.Resources.RecommendedProduct;
                    emailArgs.To.Add(email, name);
                    emailArgs.MailBody = GetHTMLFromView("~/Views/_Shared/UC/EmailTemplates/RecommendOffer.cshtml", item);
                    EmailHelper.SendEmailWithAttachments(emailArgs);

                    return Json(new
                    {
                        isSuccess = true
                    });
                }
                else
                {
                    return Json(new
                    {
                        isResult = false,
                        message = Resources.Resources.CannotSentEmail
                    });
                }
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
        public ActionResult GetAlbatrosTeeSheet(int? courseId = null, long? siteId = null, string date = "")
        {
            try
            {
                if (!siteId.HasValue)
                    throw new Exception("Please specific Site Id.");

                Site site = DataAccess.GetSiteAlbatrosSettingBySiteId(siteId.Value);

                if (site == null)
                    throw new Exception("Site not found.");

                DateTime dateObj = DataManager.ToDateTime(date, "dd/MM/yyyy", DateTime.Today);
                Albatros.SetConnection(site.AlbatrosUrl, site.AlbatrosUsername, site.AlbatrosPassword);
                Albatros.Login();

                if (!Albatros.IsLoggedIn)
                    throw new Exception("Can't login to Albatros.");

                List<AlbatrosTeeSheetModel> models = new List<AlbatrosTeeSheetModel>();
                List<TeeTimeModel> tees = new List<TeeTimeModel>();
                DateTime from = new DateTime(dateObj.Year, dateObj.Month, dateObj.Day);
                DateTime to = new DateTime(dateObj.Year, dateObj.Month, dateObj.Day, 23, 59, 59);
                string siteName = string.Empty;

                var coursesResponse = Albatros.GetReservationCourses(DateTime.Today);

                if (coursesResponse.code == "0")
                {
                    #region Old Code
                    //if (coursesResponse.groups.group.courses.course is List<object>)
                    //{
                    //    foreach (var course in coursesResponse.groups.group.courses.course)
                    //    {
                    //        if (DataManager.ToInt(course.id) == courseId)
                    //        {
                    //            siteName = DataManager.ToString(course.name);
                    //            if (course.tees.tee is List<object>)
                    //            {
                    //                foreach (var tee in course.tees.tee)
                    //                {
                    //                    tees.Add(DataManager.ToInt(tee.id), tee.name);
                    //                }
                    //            }
                    //            else
                    //            {
                    //                tees.Add(DataManager.ToInt(course.tees.tee.id), course.tees.tee.name);
                    //            }
                    //            break;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    if (DataManager.ToInt(coursesResponse.groups.group.courses.course.id) == courseId)
                    //    {
                    //        siteName = DataManager.ToString(coursesResponse.groups.group.courses.course.name);
                    //        if (coursesResponse.groups.group.courses.course.tees.tee is List<object>)
                    //        {
                    //            foreach (var tee in coursesResponse.groups.group.courses.course.tees.tee)
                    //            {
                    //                tees.Add(DataManager.ToInt(tee.id), tee.name);
                    //            }
                    //        }
                    //        else
                    //        {
                    //            tees.Add(DataManager.ToInt(coursesResponse.groups.group.courses.course.tees.tee.id), coursesResponse.groups.group.courses.course.tees.tee.name);
                    //        }
                    //    }
                    //}
                    #endregion

                    #region New Code
                    if (coursesResponse.groups.group.courses.course is List<object>)
                    {
                        foreach (var course in coursesResponse.groups.group.courses.course)
                        {
                            siteName = DataManager.ToString(course.name);
                            if (course.tees.tee is List<object>)
                            {
                                foreach (var tee in course.tees.tee)
                                {
                                    tees.Add(new TeeTimeModel(DataManager.ToInt(tee.id), tee.name, siteName));
                                }
                            }
                            else
                            {
                                tees.Add(new TeeTimeModel(DataManager.ToInt(course.tees.tee.id), course.tees.tee.name, siteName));
                            }
                        }
                    }
                    else
                    {
                        siteName = DataManager.ToString(coursesResponse.groups.group.courses.course.name);
                        if (coursesResponse.groups.group.courses.course.tees.tee is List<object>)
                        {
                            foreach (var tee in coursesResponse.groups.group.courses.course.tees.tee)
                            {
                                tees.Add(new TeeTimeModel(DataManager.ToInt(tee.id), tee.name, siteName));
                            }
                        }
                        else
                        {
                            tees.Add(new TeeTimeModel(DataManager.ToInt(coursesResponse.groups.group.courses.course.tees.tee.id), coursesResponse.groups.group.courses.course.tees.tee.name, siteName));
                        }
                    }
                    #endregion
                }

                foreach (var tee in tees)
                {
                    var teeTimes = Albatros.GetTeeTimes(tee.TeeId, from, to, TeeTimesSearchType.All, TeeTimesSortBy.DateTime, true);
                    int teeId = 0;
                    DateTime teeDateTime = DateTime.Now;
                    bool isHole9 = false;
                    bool isHole18 = false;
                    if (teeTimes.code == "0")
                    {
                        if (!(teeTimes.teetimes is string) && teeTimes.teetimes.teetime is List<object>)
                        {
                            foreach (var teeTime in teeTimes.teetimes.teetime)
                            {
                                teeId = DataManager.ToInt(tee.TeeId);
                                teeDateTime = DataManager.ToDateTime(teeTime.dateTime, "dd.MM.yyyy HH:mm");
                                isHole9 = false;
                                isHole18 = false;

                                models.Add(new AlbatrosTeeSheetModel()
                                {
                                    TeeTimeId = teeId,
                                    TeeName = tee.TeeName,
                                    SiteName = tee.SiteName,
                                    Date = teeDateTime,
                                    Max = DataManager.ToInt(teeTime.max),
                                    Free = DataManager.ToInt(teeTime.free),
                                    Booked = DataManager.ToInt(teeTime.booked),
                                    ConstraintCode = DataManager.ToInt(teeTime.constr),
                                    Detail = DataManager.ToString(teeTime.msg),
                                    Type = (TeeTimeType)DataManager.ToInt(teeTime.type),
                                    NormalFee = DataManager.ToDecimal(teeTime.feeN) / 100,
                                    ReductionFee = DataManager.ToDecimal(teeTime.feeR) / 100,
                                    IsHoles9 = isHole9,
                                    IsHoles18 = isHole18
                                    //IsAllowed = isAllowed
                                });
                            }
                        }
                    }
                }

                models = models.OrderBy(it => it.Date).ToList();

                string html = GetHTMLFromView("~/Views/_Shared/UC/Front/TeeSheet/UCAlbatrosTeeSheetRows.cshtml", models);

                return Json(new
                {
                    isSuccess = true,
                    html = html
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
            finally
            {
                Albatros.Logout();
            }
        }

        [HttpPost]
        public ActionResult GetPrimaTeeSheet(long? siteId = null, string date = "", int? gameType = 1)
        {
            try
            {
                if (!siteId.HasValue)
                    throw new Exception("Please specific Site Id.");

                Site site = DataAccess.GetSitePrimaSettingBySiteId(siteId.Value);

                if (site == null)
                    throw new Exception("Site not found.");

                DateTime dateObj = DataManager.ToDateTime(date, "dd/MM/yyyy", DateTime.Today);

                string error = string.Empty;
                PrimaDataAccess prima = new PrimaDataAccess(site.PrimaAPIKey, site.PrimaClubKey);
                List<PrimaTeeTime> teeTimes = prima.GetTeeTimes(dateObj, gameType, out error);

                if (!String.IsNullOrWhiteSpace(error))
                    throw new Exception(error);

                string html = GetHTMLFromView("~/Views/_Shared/UC/Front/TeeSheet/UCPrimaTeeSheetRows.cshtml", teeTimes);

                return Json(new
                {
                    isSuccess = true,
                    html = html
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
            finally
            {
                Albatros.Logout();
            }
        }

        [HttpGet]
        public ActionResult AjaxGetImageSlider(long? id)
        {
            try
            {
                if (!id.HasValue)
                    throw new Exception("Please specific Item ID.");

                List<ItemImage> itemImages = DataAccess.GetItemImagesByItemId(id.Value);
                string html = GetHTMLFromView("~/Views/_Shared/UC/Front/Item/UCItemImages.cshtml", itemImages);
                return Json(new
                {
                    isSuccess = true,
                    html = html
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AjaxTestTrackingCode(string code)
        {
            try
            {
                Session["TempConversionTrackingCode"] = code;
                return Json(new
                {
                    isSuccess = true,
                    message = Resources.Resources.Success
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
        public ActionResult AjaxShowTempConversionTrackingCode()
        {
            string content = string.Empty;
            if (Session["TempConversionTrackingCode"] != null)
            {
                content = (string)Session["TempConversionTrackingCode"];
            }
            return Content(content, "text/html");
        }

        [HttpPost]
        public ActionResult AjaxTriggerConversionTracking(long? id, string type = "viewcontent")
        {
            string content = string.Empty;
            if(id.HasValue)
            {
                content = DataAccess.GetItemConversionTrackingCode(id.Value, type);
            }
            return Content(content, "text/html");
        }
    }
}
