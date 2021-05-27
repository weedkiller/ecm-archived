using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using System.Globalization;
using DansLesGolfs.Base;
using DansLesGolfs.Data;

namespace DansLesGolfs.Controllers
{
    public class CartController : BaseFrontController
    {
        ShoppingCart cart;
        public CartController()
        {
            cart = ShoppingCart.Instance;
        }

        public ActionResult Index()
        {
            return View(cart);
        }
        [HttpPost]
        public ActionResult AddItem(long itemId, int quantity = 1, string reserveDate = "")
        {
            try
            {
                Item item = GetItem(itemId);

                DateTime reserveDateObject = DataManager.ToDateTime(reserveDate, "d/M/yyyy", DateTime.Today);

                decimal price = item.Price, periodPrice = 0, specialPrice = 0, cheapestPrice = 0, cheapestPeriodPrice = 0, teeSheetCheapestPrice = 0;
                DataAccess.GetItemPricesByDate(item.ItemId, reserveDateObject, out price, out periodPrice, out specialPrice, out cheapestPrice, out cheapestPeriodPrice, out teeSheetCheapestPrice);
                item.Price = price;
                item.PeriodPrice = periodPrice;
                item.SpecialPrice = specialPrice;
                item.CheapestPeriodPrice = cheapestPeriodPrice;
                item.TeeSheetCheapestPrice = teeSheetCheapestPrice;

                if (item != null)
                {
                    var items = cart.Items.Where(it => it.ItemId == itemId && it.Description.Contains(reserveDateObject.ToString("dd/MM/yyyy")));

                    if (items.Any())
                    {
                        foreach (var it in items)
                        {
                            it.Quantity += quantity;
                        }
                    }
                    else
                    {
                        CartItem cartItem = cart.AddItem(item, quantity);
                        cartItem.ReserveDate = reserveDateObject;
                        if (!item.IsUserCanSelectDate && item.ItemMaxDate.HasValue)
                        {
                            if (item.ItemMaxDate.Value == DateTime.Today)
                            {
                                cartItem.CustomDescription = string.Format(" - {0} {1}", Resources.Resources.DateOfPurchase, DateTime.Today.ToString("dd/MM/yyyy"));
                            }
                            else
                            {
                                cartItem.CustomDescription = string.Format(" - {0} {1} | {2} {3}", Resources.Resources.DateOfPurchase, DateTime.Today.ToString("dd/MM/yyyy"), Resources.Resources.ExpiredAt, item.ItemMaxDate.Value.ToString("dd/MM/yyyy"));
                            }
                        }
                        else
                        {
                            cartItem.CustomDescription = string.Format(" - {0} {1} | {2} {3}", Resources.Resources.ReserveDate, reserveDateObject.ToString("dd/MM/yyyy"), Resources.Resources.DateOfPurchase, DateTime.Today.ToString("dd/MM/yyyy"));
                        }

                        if (!String.IsNullOrEmpty(reserveDate) && !String.IsNullOrWhiteSpace(reserveDate))
                        {
                            cartItem.ReserveDate = String.IsNullOrWhiteSpace(reserveDate) ? null : (DateTime?)reserveDateObject;
                        }
                        ValidateCoupon(cartItem);
                    }

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
                        message = Resources.Resources.ItemNotFound
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

        [HttpPost]
        public ActionResult AddItemTeeSheet(long? itemId, int? numberOfPlayers = 1, string teeSheetDate = "", int? teeSheetStart = 1, int? teeSheetEnd = 1, decimal? price = 0, decimal? discount = 0)
        {
            try
            {
                if (!itemId.HasValue)
                    throw new Exception("Please specific Item ID");

                if (!teeSheetStart.HasValue)
                    throw new Exception("Please specific Start Time");

                if (!teeSheetEnd.HasValue)
                    throw new Exception("Please specific End Time");

                if (!price.HasValue)
                    throw new Exception("Please specific Price");

                if (!discount.HasValue)
                    throw new Exception("Please specific Discount");

                numberOfPlayers = !numberOfPlayers.HasValue || numberOfPlayers.Value < 1 ? 1 : numberOfPlayers.Value;

                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime teeSheetDateTime = String.IsNullOrEmpty(teeSheetDate) ? DateTime.Now : DateTime.ParseExact(teeSheetDate, "yyyy-M-d", provider);

                Item item = GetItem(itemId.Value);

                string customDescription = string.Format(" - {0} {1} {2} - {3} | {4} {5}",
                    Resources.Resources.ReserveDate,
                    teeSheetDateTime.ToString("dd/MM/yyyy"),
                    teeSheetStart.Value.ToString("00") + ":00",
                    teeSheetEnd.Value.ToString("00") + ":00",
                    Resources.Resources.DateOfPurchase,
                    DateTime.Today.ToString("dd/MM/yyyy"));

                if (item != null)
                {
                    var items = cart.Items.Where(it => it.ItemId == itemId
                        && it.Description.Contains(teeSheetDateTime.ToString("dd/MM/yyyy"))
                        && it.Description.Contains(teeSheetStart.Value.ToString("00") + ":00" + " - " + teeSheetEnd.Value.ToString("00") + ":00"));

                    if (items.Any())
                    {
                        foreach (var i in items)
                        {
                            i.Quantity = numberOfPlayers.Value;
                            i.UnitPrice = discount.Value > 0 ? discount.Value : price.Value;
                        }
                    }
                    else
                    {
                        CartItem cartItem = new CartItem(item);
                        cartItem.Quantity = numberOfPlayers.Value;
                        cartItem.UnitPrice = discount.Value > 0 ? discount.Value : price.Value;
                        cartItem.CustomDescription = customDescription;
                        cartItem.MaxQuantity = 8;

                        if (!String.IsNullOrEmpty(teeSheetDate) && !String.IsNullOrWhiteSpace(teeSheetDate))
                        {
                            cartItem.ReserveDate = teeSheetDateTime;
                        }

                        cart.Items.Add(cartItem);
                        ValidateCoupon(cartItem);
                    }
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
                        message = Resources.Resources.ItemNotFound
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

        [HttpPost]
        public ActionResult AddItemAlbatrosTeeSheet(long? itemId, int? teeTimeId, string teeDate, string teeTime, int numberOfPlayers, decimal price, int? gameType, int? available)
        {
            try
            {
                if (!itemId.HasValue)
                    throw new Exception("Please specific Item ID.");

                if (!teeTimeId.HasValue)
                    throw new Exception("Please specific Tee Time ID.");

                if (!gameType.HasValue)
                    throw new Exception("Please specific Game Type (9 or 18).");

                if (!available.HasValue)
                    throw new Exception("Please specific available players.");

                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime teeSheetDateTime = DataManager.ToDateTime(teeDate + " " + teeTime, "yyyy-M-d HH:mm", DateTime.Today);

                Item item = GetItem(itemId.Value);

                string description = string.Format(" - {0} {1} | {2} {3}",
                    Resources.Resources.ReserveDate,
                    teeSheetDateTime.ToString("dd/MM/yyyy HH:mm"),
                    Resources.Resources.DateOfPurchase,
                    DateTime.Today.ToString("dd/MM/yyyy"));
                string refCode = string.Empty;

                if (item != null)
                {
                    if (!AddAlbatrosReservation(item, teeTimeId.Value, gameType.Value, teeSheetDateTime, numberOfPlayers, out refCode))
                        throw new Exception("Can't add reservation on this golf course.");

                    CartItem cartItem = new CartItem(item);
                    cartItem.Quantity = numberOfPlayers;
                    cartItem.MaxQuantity = available.Value;
                    cartItem.UnitPrice = price;
                    cartItem.CustomDescription = description;
                    cartItem.RefCode = refCode;
                    cartItem.SpecialData.Add("AlbatrosTeeTimeId", teeTimeId.Value);
                    cartItem.SpecialData.Add("AlbatrosTeeStartTime", teeSheetDateTime);
                    cartItem.SpecialData.Add("AlbatrosGameType", gameType.Value);
                    cartItem.ReserveDate = teeSheetDateTime;
                    cart.Items.Add(cartItem);
                    ValidateCoupon(cartItem);

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
                        message = Resources.Resources.ItemNotFound
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

        /// <summary>
        /// Get Item by ItemID
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns>Item object</returns>
        private Item GetItem(long itemId)
        {
            Item item = DataAccess.GetItem(itemId, this.CultureId);
            if (item == null || item.ItemId <= 0)
                throw new Exception(Resources.Resources.ItemNotFound);

            CheckPaymentGateway(item);
            return item;
        }

        /// <summary>
        /// Checking confliction of Payment Gateway
        /// </summary>
        /// <param name="item"></param>
        private void CheckPaymentGateway(Item item)
        {
            if (!cart.Items.Any())
                return;

            CartItem cartItem = cart.Items.First();
            string itemVendorToken = "";
            string itemPrivateKey = "";
            string cartItemVendorToken = "";
            string cartItemPrivateKey = "";
            DataAccess.GetPaymentGatewayByItemId(item.ItemId, out itemVendorToken, out itemPrivateKey);
            DataAccess.GetPaymentGatewayByItemId(cartItem.ItemId, out cartItemVendorToken, out cartItemPrivateKey);
            if (itemVendorToken != cartItemVendorToken)
                throw new Exception(Resources.Resources.CannotAddItemFromDifferentVendorInCart);
        }

        private bool AddAlbatrosReservation(Item item, int teeTimeId, int gameType, DateTime startTime, int numberOfPlayers, out string refCode)
        {
            refCode = string.Empty;
            Site site = DataAccess.GetSiteAlbatrosSettingBySiteId(item.SiteId);
            if (site == null)
                return false;

            Albatros.SetConnection(site.AlbatrosUrl, site.AlbatrosUsername, site.AlbatrosPassword);
            Albatros.Login();

            if (!Albatros.IsLoggedIn)
                throw new Exception("Can't login to Albatros.");

            var teeTimeDetail = Albatros.GetTeeTimeDetails(teeTimeId, startTime);

            if (teeTimeDetail.code == "0")
            {
                gameType = teeTimeDetail.holes9 == "true" ? 9 : 18;

                var addReservation = Albatros.AddReservation(startTime, teeTimeId, gameType, string.Empty, numberOfPlayers, Auth.User);
                if (addReservation.code == "0")
                {
                    refCode = addReservation.bookNr;
                    return true;
                }
                else
                {
                    throw new Exception(addReservation.message);
                }
            }
            else
            {
                throw new Exception(teeTimeDetail.message);
            }
        }

        [HttpPost]
        public ActionResult AddItemPrimaTeeSheet(long? itemId, string courseId, string teeDate, string teeTime, string teeTime9In, string teeTime9Out, int numberOfPlayers, decimal price, string gameType)
        {
            try
            {
                if (!itemId.HasValue)
                    throw new Exception("Please specific Item ID.");

                if (String.IsNullOrWhiteSpace(courseId))
                    throw new Exception("Please specific Place No.");

                if (String.IsNullOrWhiteSpace(gameType))
                    throw new Exception("Please specific Game Type (9 or 18 holes).");

                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime teeSheetDateTime = DataManager.ToDateTime(teeDate + " " + teeTime, "yyyyMMdd HHmm", DateTime.Today);
                if (!String.IsNullOrWhiteSpace(teeTime9In))
                {
                    DateTime teeSheetDateTime9In = DataManager.ToDateTime(teeDate + " " + teeTime9In, "yyyyMMdd HHmm", DateTime.Today);
                }

                Item item = GetItem(itemId.Value);
                Site site = DataAccess.GetSitePrimaSettingBySiteId(item.SiteId);
                if (site == null)
                    throw new Exception("Site not found.");

                PrimaDataAccess prima = new PrimaDataAccess(site.PrimaAPIKey, site.PrimaClubKey);
                string error = string.Empty;
                string lockCode = prima.Lock(courseId, teeSheetDateTime, numberOfPlayers, out error);

                if (!String.IsNullOrWhiteSpace(error))
                    throw new Exception(error);

                string description = string.Format(" - {0} {1} | {2} {3}",
                    Resources.Resources.ReserveDate,
                    teeSheetDateTime.ToString("dd/MM/yyyy HH:mm"),
                    Resources.Resources.DateOfPurchase,
                    DateTime.Today.ToString("dd/MM/yyyy"));

                if (item != null)
                {
                    CartItem cartItem = new CartItem(item);
                    cartItem.Quantity = numberOfPlayers;
                    cartItem.MaxQuantity = 4;
                    cartItem.UnitPrice = price;
                    cartItem.CustomDescription = description;
                    cartItem.RefCode = string.Empty;
                    cartItem.SpecialData.Add("PrimaCourseId", courseId);
                    cartItem.SpecialData.Add("PrimaTeeStartTime", teeSheetDateTime);
                    cartItem.SpecialData.Add("PrimaTeeStartTime9In", teeSheetDateTime);
                    cartItem.SpecialData.Add("PrimaGameType", gameType);
                    cartItem.SpecialData.Add("PrimaLockCode", lockCode);
                    cartItem.ReserveDate = teeSheetDateTime;
                    cart.Items.Add(cartItem);
                    ValidateCoupon(cartItem);

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
                        message = Resources.Resources.ItemNotFound
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                if(ex.Message == "The remote server returned an error: (401) Unauthorized.")
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = Resources.Resources.YouAlreadyLockSomeOfTeeTimePleaseTry15Minutes
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = ex.Message
                    });
                }
            }
        }

        [HttpPost]
        public ActionResult SetItemQuantity(long itemId, int quantity = 1)
        {
            try
            {
                Item item = DataAccess.GetItem(itemId);
                if (item != null)
                {
                    if (!cart.SetItemQuantity(itemId, quantity))
                    {
                        cart.AddItem(item, quantity);
                    }
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
                        message = Resources.Resources.CantRemoveItemFromCart
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
        [HttpPost]
        public ActionResult GetItemQuantity(long itemId)
        {
            try
            {
                if (itemId > 0)
                {
                    cart.GetItemQuantity(itemId);
                    return Json(new
                    {
                        isSuccess = true,
                        quantity = cart.GetItemQuantity(itemId)
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = Resources.Resources.CantRemoveItemFromCart
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
        [HttpPost]
        public ActionResult RemoveItem(long itemId)
        {
            try
            {
                Item item = DataAccess.GetItem(itemId, this.CultureId);
                if (item != null)
                {
                    cart.RemoveItem(itemId);
                    return Json(new
                    {
                        isSuccess = true,
                        itemCount = cart.Quantity
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = Resources.Resources.CantRemoveItemFromCart
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
        [HttpPost]
        public ActionResult ApplyCouponCode(string code)
        {
            try
            {
                Coupon coupon = DataAccess.GetCouponByCouponCode(code);
                if (coupon == null)
                    throw new Exception(Resources.Resources.CouponCodeIsInvalid);

                // Validate coupon by date.
                bool validateResult = DataAccess.CheckCouponUsagePeriod(coupon);
                if (!validateResult)
                {
                    if (coupon.CouponGroup.UsagePeriodType == (int)CouponUsagePeriodType.Total)
                    {
                        throw new Exception(Resources.Resources.CouponHasAlreadyBeenUsed);
                    }
                    else if (coupon.CouponGroup.UsagePeriodType == (int)CouponUsagePeriodType.ByDay)
                    {
                        throw new Exception(Resources.Resources.CouponReachToUsageLimitPerDay);
                    }
                    else if (coupon.CouponGroup.UsagePeriodType == (int)CouponUsagePeriodType.ByWeek)
                    {
                        throw new Exception(Resources.Resources.CouponReachToUsageLimitPerWeek);
                    }
                }

                cart.Coupon = coupon;
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

        private void ValidateCoupon(CartItem cartItem)
        {
            if (cartItem != null && cart.HasCoupon)
            {
                if (cart.Coupon.CouponGroup.CouponUsageType == (int)CouponUsageType.PerOrder)
                {
                    cartItem.Coupon = cart.Coupon;
                }
                else if (cart.Coupon.CouponGroup.CouponUsageType == (int)CouponUsageType.PerItemType)
                {
                    cartItem.Coupon = cart.Coupon.CouponGroup.ItemTypeIds.Contains(cartItem.Item.ItemTypeId) ? cart.Coupon : null;
                }
                else if (cart.Coupon.CouponGroup.CouponUsageType == (int)CouponUsageType.PerCategories)
                {
                    cartItem.Coupon = cart.Coupon.CouponGroup.ItemCategoryIds.Contains(cartItem.Item.CategoryId) ? cart.Coupon : null;
                }
                else if (cart.Coupon.CouponGroup.CouponUsageType == (int)CouponUsageType.PerItems)
                {
                    cartItem.Coupon = cart.Coupon.CouponGroup.Items.Select(it => it.ItemId).Contains(cartItem.Item.ItemId) ? cart.Coupon : null;
                }
                else
                {
                    cartItem.Coupon = null;
                }
            }
        }
    }
}