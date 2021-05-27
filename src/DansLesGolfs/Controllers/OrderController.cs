using ApiPayment;
using ApiPayment.Common;
using ApiPayment.Web;
using DansLesGolfs.Base;
using DansLesGolfs.Base.Revervation;
using DansLesGolfs.BLL;
using DansLesGolfs.Data;
using DansLesGolfs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Controllers
{
    public class OrderController : BaseFrontController
    {
        #region Fields
        private ShoppingCart cart;
        private string orderShippingSessionName = "OrderShippingData";
        #endregion

        #region Constructors
        public OrderController()
        {
            cart = ShoppingCart.Instance;
        }
        #endregion

        #region Action Methods

        #region AjaxSendConfirmationEmail
        public ActionResult AjaxSendConfirmationEmail(long? orderId)
        {
            Order order = DataAccess.GetOrderByOrderId(orderId.Value);
            SendConfirmationEmail(order);
            return Content("1");
        }
        #endregion

        #region Order : Identification
        #region Identification
        public ActionResult Identification()
        {
            User user = new BLL.User();
            user.Birthdate = DateTime.Today;

            if (Auth.User != null)
                return RedirectToAction("Shipping");

            List<Title> titles = DataAccess.GetItemTitlesDropDownList(this.CultureId);
            ViewBag.DropDownTitles = ListToDropDownList<Title>(titles, "TitleId", "TitleName");
            if (titles.Any())
            {
                ViewBag.TitleId = titles.First().TitleId;
            }

            List<Country> countries = DataAccess.GetAllCountries();
            ViewBag.Countries = ListToDropDownList<Country>(countries, "CountryId", "CountryName");

            return View(user);
        }
        #endregion

        #region Register
        [HttpPost]
        public ActionResult RegisterForm(string Email, string Password, int? Civility, string Firstname, string Lastname, string Birthdate, string Address, string Complement, string PostalCode, int? CityId, int? CountryId, string Phone, string PhoneCountryCode, string MobilePhone, string MobilePhoneCountryCode, string Remarks, bool? IsReceiveNewsletters, bool? IsReceiveSpecialOffers)
        {
            try
            {
                if (DataAccess.IsExistsEmail(Email))
                    throw new Exception(Resources.Resources.YourEmailIsAlreadyInUse);

                User user = new User()
                {
                    Email = Email.Trim(),
                    Password = Password.Trim(),
                    PasswordEncrypted = DataProtection.Encrypt(Password.Trim()),
                    FirstName = Firstname.Trim(),
                    LastName = Lastname.Trim(),
                    Birthdate = DataManager.ToDateTime(Birthdate),
                    Address = Address.Trim(),
                    ShippingAddress = Address.Trim(),
                    PostalCode = PostalCode.Trim(),
                    CityId = CityId.HasValue ? CityId.Value : 0,
                    ShippingCityId = CityId.HasValue ? CityId.Value : 0,
                    CountryId = CountryId.HasValue ? CountryId.Value : 0,
                    Phone = Phone.Replace("_", "").Replace("-", ""),
                    ShippingPhone = MobilePhone.Replace("_", "").Replace("-", ""),
                    MobilePhone = MobilePhone.Replace("_", "").Replace("-", ""),
                    PhoneCountryCode = PhoneCountryCode.Replace("_", "").Replace("-", ""),
                    ShippingPhoneCountryCode = PhoneCountryCode.Replace("_", "").Replace("-", ""),
                    MobilePhoneCountryCode = MobilePhoneCountryCode.Replace("_", "").Replace("-", ""),
                    Remarks = Remarks.Trim(),
                    ExpiredDate = DateTime.Today.AddYears(999),
                    LastLoggedOn = DateTime.Today,
                    Active = true,
                    UpdateDate = DateTime.Now,
                    InsertDate = DateTime.Now,
                    RegisteredDate = DateTime.Now,
                    IsSubscriber = IsReceiveNewsletters.HasValue ? IsReceiveNewsletters.Value : false,
                    IsReceiveEmailInfo = IsReceiveSpecialOffers.HasValue ? IsReceiveSpecialOffers.Value : false,
                    UserTypeId = (int)UserTypes.Customer
                };
                user.UserId = DataAccess.AddUser(user);
                if (user.UserId > 0)
                {
                    Auth.SetSessionByUserObject(user);
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
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return Json(new
                {
                    isResult = false,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region Login
        [HttpPost]
        public JsonResult LoginForm(string Email = "", string Password = "")
        {
            try
            {
                if (Auth.Attempt(Email, Password, true))
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
                        message = Resources.Resources.InvalidEmailOrPassword
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return Json(new
                {
                    isResult = false,
                    message = ex.Message
                });
            }
        }
        #endregion
        #endregion

        #region Order : Payment
        #region Payment
        public ActionResult Payment()
        {
            /*if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }*/

            // Prepare order session

            OrderShippingModel model = GetShippingModel();
            //CreateCreditCardRequestViewBag(model);
            Order o = model.GetOrder();
            CreatePaymentViewBags();

            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }

            return View(model);
            //if (model.Cart.Items.Any())
            //{
            //    CreatePaymentViewBags();
            //    return View(model);
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Cart");
            //}
        }
        #endregion

        #region PaymentSuccess
        public ActionResult PaymentSuccess()
        {
            OrderShippingModel order = GetShippingModel();
            return View(order);
        }
        #endregion

        #region PaymentFailed
        public ActionResult PaymentFailed()
        {
            OrderShippingModel order = GetShippingModel();
            return View(order);
        }
        #endregion

        #region PaymentCheckAlbatros()
        public ActionResult PaymentCheckAlbatros()
        {
            try
            {
                DateTime startTime = DateTime.Now;
                int teeId = 0;
                int gameType = 18;
                Site site = null;
                OrderShippingModel model = GetShippingModel();
                var greenFees = cart.Items.Where(it => it.SpecialData.ContainsKey("AlbatrosTeeTimeId"));
                foreach (CartItem item in greenFees)
                {
                    site = DataAccess.GetSiteAlbatrosSettingBySiteId(item.Item.SiteId);
                    if (site == null)
                        continue;

                    Albatros.SetConnection(site.AlbatrosUrl, site.AlbatrosUsername, site.AlbatrosPassword);
                    Albatros.Login();

                    if (!Albatros.IsLoggedIn)
                        throw new Exception("Can't login to Albatros.");

                    if (item.SpecialData.ContainsKey("AlbatrosTeeTimeId") && item.SpecialData.ContainsKey("AlbatrosTeeStartTime"))
                    {
                        teeId = DataManager.ToInt(item.SpecialData["AlbatrosTeeTimeId"]);
                        startTime = DataManager.ToDateTime(item.SpecialData["AlbatrosTeeStartTime"]);

                        var teeTimeDetail = Albatros.GetTeeTimeDetails(teeId, startTime);

                        if (teeTimeDetail.code == "0")
                        {
                            gameType = teeTimeDetail.holes9 == true ? 9 : 18;

                            var addReservation = Albatros.AddReservation(startTime, teeId, gameType, string.Empty, item.Quantity, Auth.User);
                            if (addReservation.code == "0")
                            {
                                item.RefCode = addReservation.bookNr;
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
                }
                SaveShippingModel(model);

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
            finally
            {
                Albatros.Logout();
            }
        }
        #endregion

        #region Payment Gateway : Free
        public ActionResult PaymentFree()
        {
            try
            {
                OrderShippingModel model = GetShippingModel();
                if (model.Cart.GetTotalPrice() <= 0)
                {
                    // Save Order
                    Order order = model.GetOrder();
                    order.PaymentStatus = "success";
                    order.PaymentType = "free";
                    order.Active = true;
                    SaveOrder(order);
                    return RedirectToAction("Confirmation", new { id = order.OrderId });
                }
                else
                {
                    throw new Exception(Resources.Resources.ThisOrderIsNotFree);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Payment");
            }
        }
        #endregion

        #region Payment Geteway : Lydia App
        #region PaymentLydia
        public ActionResult PaymentLydia(string type = "card")
        {
            OrderShippingModel model = GetShippingModel();
            model.GenerateTransactionId();
            Order order = model.GetOrder();
            order.PaymentStatus = "pending";
            if (type == "mobile")
            {
                order.PaymentType = "lydia";
                ViewBag.PaymentMethod = "lydia";
            }
            else
            {
                order.PaymentType = "creditcard";
                ViewBag.PaymentMethod = "card";
            }
            order.Active = false;
            try
            {
                SaveOrder(order);
                SaveShippingModel(model);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }


            PrepareLydiaParameters(model);
            WebContent content = DataAccess.GetWebContentByKey("payment-lydia-waitingmessage", this.CultureId, "PAYMENT");
            ViewBag.WaitingMessage = content.ContentText;
            return View(model);
        }
        #endregion

        #region PaymentLydiaConfirm
        public ActionResult PaymentLydiaConfirm()
        {
            OrderShippingModel model = GetShippingModel();
            PrepareLydiaParameters(model);
            WebContent content = DataAccess.GetWebContentByKey("payment-lydia-waitingmessage", this.CultureId, "PAYMENT");
            ViewBag.WaitingMessage = content.ContentText;

            try
            {
                Order order = DataAccess.GetOrderByTransactionId(model.TransactionId, model.OrderNumber);
                order.RequestId = model.LydiaRequestId;
                order.PaymentStatus = "pending";
                order.Active = true;
                order.ModifiedDate = order.OrderDate = DateTime.Now;
                SaveOrder(order, true, true);

                ViewBag.OrderId = order.OrderId;
                ViewBag.ConfirmationUrl = Url.Content("~/Order/Confirmation/" + order.OrderId);

                if (order.OrderId > 0)
                {
                    model.Clear(Auth.User.UserId);
                    SaveShippingModel(model);
                    return View(model);
                }
                else
                {
                    return RedirectToAction("PaymentLydiaCancel");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Payment");
            }
        }
        #endregion

        #region AjaxCheckLydiaPaymentStatus
        public JsonResult AjaxCheckLydiaPaymentStatus(long? id, string requestId)
        {
            try
            {
                //using (var client = new System.Net.Http.HttpClient())
                //{
                //    string lydiaUrl = System.Configuration.ConfigurationManager.AppSettings["LydiaUrl"];
                //    var values = new Dictionary<string, string>
                //    {
                //        {"requestId",requestId}
                //    };
                //    var content = new System.Net.Http.FormUrlEncodedContent(values);
                //    var response = client.PostAsync(lydiaUrl + "api/request/state.json", content);
                //    var responseString = response.Content.ReadAsStringAsync();
                //    DataAccess.SetLydiaPaymentStatus(requestId, "success");
                //}

                DataAccess.SetLydiaPaymentStatus(id.Value, "success");
                return Json(new
                {
                    isSuccess = true
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

        #region PaymentLydiaConfirm (Old)
        //public ActionResult PaymentLydiaConfirm()
        //{
        //    OrderShippingModel model = GetShippingModel();
        //    PrepareLydiaParameters(model);
        //    WebContent content = DataAccess.GetWebContentByKey("payment-lydia-waitingmessage", this.CultureId, "PAYMENT");
        //    ViewBag.WaitingMessage = content.ContentText;
        //    return View(model);
        //}
        #endregion

        #region PaymentLydiaCheck (Old)
        //[HttpPost]
        //public ActionResult PaymentLydiaCheck(string requestId, string state, string message)
        //{
        //    try
        //    {
        //        OrderShippingModel model = GetShippingModel();
        //        string transactionId = model.TransactionId;
        //        string orderNumber = model.OrderNumber;

        //        if (model.LydiaRequestId != requestId)
        //            throw new Exception("Request ID not match!");

        //        if (state == "-1")
        //            throw new Exception("There are error while pay with Lydia App.");

        //        if (state == "5")
        //            throw new Exception("Request refused.");

        //        if (state == "6")
        //            throw new Exception("Request cancelled.");

        //        if ((String.IsNullOrEmpty(state) || String.IsNullOrWhiteSpace(state)) && (!String.IsNullOrEmpty(message) && !String.IsNullOrWhiteSpace(message)))
        //            throw new Exception(message);

        //        Order order = DataAccess.GetOrderByTransactionId(transactionId, orderNumber);
        //        if (order == null || order.OrderId <= 0)
        //            order = model.GetOrder();

        //        order.PaymentType = "lydia";
        //        order.PaymentStatus = "success";
        //        order.RequestId = requestId;
        //        order.Active = true;
        //        order.ModifiedDate = order.OrderDate = DateTime.Now;
        //        SaveOrder(order, true, true);
        //        if (order.OrderId > 0)
        //        {
        //            model.Clear(Auth.User.UserId);
        //            SaveShippingModel(model);
        //            return Json(new
        //            {
        //                isSuccess = true,
        //                orderId = order.OrderId,
        //                url = Url.Content("~/Order/Confirmation/" + order.OrderId),
        //                postProcessUrl = Url.Content("~/Order/AjaxSendConfirmationEmail"),
        //                orderNumber = orderNumber,
        //                transactionId = transactionId
        //            });
        //        }
        //        else
        //        {
        //            throw new Exception(Resources.Resources.ErrorSaveFailed);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex.Message);
        //        TempData["ErrorMessage"] = ex.Message;
        //        return Json(new
        //        {
        //            isSuccess = false,
        //            message = ex.Message,
        //            url = Url.Action("Payment")
        //        });
        //    }
        //}
        #endregion

        #region PaymentLydiaCancel
        public ActionResult PaymentLydiaCancel()
        {
            OrderShippingModel model = GetShippingModel();
            model.LydiaRequestId = string.Empty;
            SaveShippingModel(model);
            TempData["ErrorMessage"] = Resources.Resources.PaymentWasCancelled;
            return RedirectToAction("Payment");
        }
        #endregion

        #region PaymentLydiaExpired
        public ActionResult PaymentLydiaExpired()
        {
            OrderShippingModel model = GetShippingModel();
            model.LydiaRequestId = string.Empty;
            SaveShippingModel(model);
            TempData["ErrorMessage"] = Resources.Resources.RequestTimeout;
            return RedirectToAction("Payment");
        }
        #endregion

        [HttpPost]
        public ActionResult PaymentLydiaSetRequestID(string requestId)
        {
            OrderShippingModel model = GetShippingModel();
            PrepareLydiaParameters(model);
            model.LydiaRequestId = requestId;
            SaveShippingModel(model);

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult PaymentLydiaShowError(string message)
        {
            TempData["ErrorMessage"] = message;
            return Json(new
            {
                isSuccess = true,
                url = Url.Action("Payment")
            });
        }

        private void PrepareLydiaParameters(OrderShippingModel model)
        {
            string vendorToken = string.Empty;
            string vendorId = string.Empty;
            string privateToken = string.Empty;

            if (model.Cart.Items.Any())
            {
                DataAccess.GetPaymentGatewayByItemId(model.Cart.Items.First().ItemId, out vendorToken, out vendorId);
                if (String.IsNullOrWhiteSpace(vendorToken))
                {
                    ViewBag.LydiaVendorToken = vendorToken = System.Configuration.ConfigurationManager.AppSettings["LydiaVendorToken"];
                }
                else
                {
                    ViewBag.LydiaVendorToken = vendorToken;
                }
            }
            else
            {
                ViewBag.LydiaVendorToken = vendorToken = System.Configuration.ConfigurationManager.AppSettings["LydiaVendorToken"];
            }

            string lydiaBaseUrl = System.Configuration.ConfigurationManager.AppSettings["LydiaUrl"];
            if (!lydiaBaseUrl.EndsWith("/"))
            {
                lydiaBaseUrl += "/";
            }
            ViewBag.LydiaPrivateToken = privateToken = System.Configuration.ConfigurationManager.AppSettings["LydiaPrivateToken"];
            ViewBag.LydiaBaseUrl = lydiaBaseUrl;
            ViewBag.IsLydiaTest = ((string)ViewBag.LydiaBaseUrl).Contains("homologation.lydia-app.com");

            // Create signature.
            string signatureParams = "transaction_identifier=" + model.TransactionId + "&amount=" + model.Cart.GetSubTotal() + "&" + privateToken;
            ViewBag.LydiaSignature = signatureParams;
        }
        #endregion

        #region Payment Gateway : Paypal
        #region PaymentPaypal
        public ActionResult PaymentPaypal()
        {
            OrderShippingModel model = GetShippingModel();
            CreatePaymentViewBags();
            return View(model);
        }
        #endregion

        #region PaypalProgress
        public ActionResult PaypalProgress(string tx = "", string st = "")
        {
            if (!string.IsNullOrEmpty(tx) && !string.IsNullOrEmpty(st) && st == "Completed")
            {
                OrderShippingModel model = GetShippingModel();
                Order order = DataAccess.GetOrderByTransactionId(tx);
                if (order == null)
                {
                    model.TransactionId = tx;
                    model.PaypalSuccess = true;
                    order = model.GetOrder();
                }
                order.PaymentType = "paypal";
                order.PaymentStatus = "success";
                order.Active = true;
                SaveOrder(order);
                if (order.OrderId > 0)
                {
                    model.Clear(Auth.User.UserId);
                    SaveShippingModel(model);
                    return RedirectToAction("Confirmation", new { id = order.OrderId });
                }
                else
                {
                    ViewBag.CheckCount = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["PaypalCheckCount"], 10);
                    ViewBag.Interval = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["PaypalCheckInterval"], 1000);
                    return View(model);
                }
            }
            else
            {
                return RedirectToAction("PaymentFailed");
            }
        }
        #endregion

        #region PaypalIPN
        public ActionResult PaypalIPN()
        {
            // if you want to use the PayPal sandbox change this from false to true
            try
            {
                string response = GetPayPalResponse();

                OrderShippingModel model = GetShippingModel();

                if (response == "VERIFIED")
                {

                    string transactionId = Request["txn_id"];
                    decimal amountPaid = DataManager.ToDecimal(Request["mc_gross"]);
                    int cartNumItems = DataManager.ToInt(Request["num_cart_items"]);
                    var custom = HttpUtility.ParseQueryString(Request["custom"]);

                    Order order = new Order();
                    order.TransactionId = transactionId;
                    order.AddressId = DataManager.ToLong(custom["addressId"], 0);
                    order.CustomerId = DataManager.ToInt(custom["customerId"], 0);
                    order.OrderItems = new List<OrderItem>();

                    for (int i = 1; i <= cartNumItems; i++)
                    {
                        order.OrderItems.Add(new OrderItem()
                        {
                            ItemId = DataManager.ToLong(Request["item_number" + i]),
                            ItemName = DataManager.ToString(Request["item_name" + i]),
                            Quantity = DataManager.ToInt(Request["quantity" + i]),
                            UnitPrice = DataManager.ToDecimal(Request["mc_gross_" + i]),
                        });
                    }

                    order.PaymentType = "paypal";
                    order.PaymentStatus = "success";
                    order.Active = true;
                    SaveOrder(order);

                    if (order.OrderId > 0)
                    {
                        return Content("1");
                    }
                    else
                    {
                        model.PaypalSuccess = false;
                        return Content("0");
                    }
                }
                model.PaypalSuccess = false;
                //["ErrorMessages"] = Resources.Resources.PaymentFailedDescription;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData["ErrorMessages"] = ex.Message;
            }
            return Content("0");
        }
        #endregion

        #region PaypalProgressCheck
        public JsonResult PaypalProgressCheck(string guid)
        {
            try
            {
                Order order = DataAccess.GetOrderByTransactionId(guid);
                if (order != null)
                {
                    OrderShippingModel model = GetShippingModel();
                    model.TransactionId = order.TransactionId;
                    model.PaypalSuccess = true;

                    Order modelOrder = model.GetOrder();
                    order.OrderItems = modelOrder.OrderItems;
                    order.PaymentType = "paypal";
                    order.PaymentStatus = "success";
                    order.Active = true;
                    SaveOrder(order);
                    return Json(new
                    {
                        isSuccess = true,
                        orderId = order.OrderId
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
        #endregion
        #endregion

        #region PaymentCheck
        public ActionResult PaymentCheck()
        {
            OrderShippingModel model = GetShippingModel();
            WebContent content = DataAccess.GetWebContentByKey("payment-by-check", this.CultureId, "PAYMENT");
            content.ContentText = content.ContentText.Replace("[ORDERNUMBER]", model.OrderNumber).Replace("[ordernumber]", model.OrderNumber);
            content.ContentText = content.ContentText.Replace("[ORDERAMOUNT]", model.Cart.GetSubTotal().ToString("#,##0.00")).Replace("[orderamount]", model.Cart.GetSubTotal().ToString("#,##0.00"));
            return View(content);
        }
        #endregion

        #region AjaxConfirmPaymentByCheck
        [HttpPost]
        public JsonResult AjaxConfirmPaymentByCheck()
        {
            // if you want to use the PayPal sandbox change this from false to true
            try
            {
                OrderShippingModel model = GetShippingModel();

                Order order = model.GetOrder();
                order.PaymentType = "check";
                order.PaymentStatus = "pending";
                order.Active = true;

                SaveOrder(order);

                if (order.OrderId > 0)
                {
                    model.Clear(Auth.User.UserId);
                    SaveShippingModel(model);
                    return Json(new
                    {
                        isSuccess = true,
                        orderId = order.OrderId
                    });

                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region AjaxCancelPaymentByCheck
        [HttpPost]
        public JsonResult AjaxCancelPaymentByCheck()
        {
            // if you want to use the PayPal sandbox change this from false to true
            try
            {
                OrderShippingModel model = GetShippingModel();

                Order order = model.GetOrder();
                order.PaymentType = "check";
                order.PaymentStatus = "cancel";
                order.Active = true;

                SaveOrder(order);

                if (order.OrderId > 0)
                {
                    model.Clear(Auth.User.UserId);
                    SaveShippingModel(model);
                    return Json(new
                    {
                        isSuccess = true,
                        orderId = order.OrderId
                    });

                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region AjaxPaymentCheckCartStatus
        public ActionResult AjaxPaymentCheckCartStatus()
        {
            try
            {
                OrderShippingModel model = GetShippingModel();

                if (model.Cart.Items == null || !model.Cart.Items.Any())
                    throw new Exception(Resources.Resources.TheBasketIsEmpty);

                return Json(new
                {
                    isSuccess = true,
                    cartStatus = new
                    {
                        isPassed = true,
                        message = "Passed!"
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return Json(new
                {
                    isSuccess = true,
                    cartStatus = new
                    {
                        isPassed = false,
                        message = ex.Message
                    }
                });
            }
        }
        #endregion
        #endregion

        #region Order : Shipping
        #region Shipping
        public ActionResult Shipping()
        {
            if (!Auth.Check())
                return RedirectToAction("Identification");

            if (cart == null || !cart.Items.Any())
            {
                //return RedirectToAction("Index", "Cart");
            }

            List<Address> addresses = DataAccess.GetAddressByUserID(Auth.User.UserId);

            ViewBag.HasNoAddress = addresses == null || !addresses.Any();

            List<Title> titles = DataAccess.GetItemTitlesDropDownList(this.CultureId);
            ViewBag.DropDownTitles = ListToDropDownList<Title>(titles, "TitleId", "TitleName");
            if (titles.Any())
            {
                ViewBag.TitleId = titles.First().TitleId;
            }

            List<Country> countries = DataAccess.GetAllCountries();
            ViewBag.Countries = ListToDropDownList<Country>(countries, "CountryId", "CountryName");

            return View(addresses);
        }

        [HttpPost]
        public ActionResult Shipping(OrderShippingModel model)
        {
            if (ModelState.IsValid)
            {
                Session[orderShippingSessionName] = model;
                return RedirectToAction("Payment");
            }
            else
            {
                List<Address> addresses = DataAccess.GetAddressByUserID(Auth.User.UserId);

                ViewBag.HasNoAddress = addresses == null || addresses.Any();

                List<Title> titles = DataAccess.GetItemTitlesDropDownList(this.CultureId);
                ViewBag.DropDownTitles = ListToDropDownList<Title>(titles, "TitleId", "TitleName");
                if (titles.Any())
                {
                    ViewBag.TitleId = titles.First().TitleId;
                }

                List<Country> countries = DataAccess.GetAllCountries();
                ViewBag.Countries = ListToDropDownList<Country>(countries, "CountryId", "CountryName");

                //List<City> cities = DataAccess.GetCityByCountryId(countries.Any() ? countries.First().CountryId : 0);
                List<SelectListItem> citiesList = new List<SelectListItem>();
                ViewBag.DropDownCities = citiesList; // For Debug
                //if (MemoryCache.Default["DropDownCities"] == null)
                //{
                //    cities = DataAccess.GetCityByCountryId(countries.Any() ? countries.First().CountryId : 0);
                //    foreach (City city in cities)
                //    {
                //        citiesList.Add(new SelectListItem()
                //        {
                //            Value = city.CityId.ToString(),
                //            Text = city.CityName
                //        });
                //    }
                //    ViewBag.DropDownCities = citiesList;
                //    MemoryCache.Default.Add("DropDownCities", citiesList, DateTime.Now.AddDays(1));
                //}
                //else
                //{
                //    ViewBag.DropDownCities = citiesList = (List<SelectListItem>)MemoryCache.Default["DropDownCities"];
                //}
                //if (cities.Any())
                //{
                //    ViewBag.CityId = citiesList.First().Value;
                //}

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult CreateShippingAddressByProfileAddress()
        {
            try
            {
                if (Auth.User != null)
                {
                    Address address = new Address()
                    {
                        AddressId = -1,
                        AddressName = Resources.Resources.DefaultShippingAddress,
                        Address1 = !String.IsNullOrWhiteSpace(Auth.User.Address) ? Auth.User.Address : Auth.User.ShippingAddress,
                        CityId = Auth.User.CityId > 0 ? Auth.User.CityId : DataAccess.GetCityIdByCityName(Auth.User.City, Auth.User.CountryId),
                        CityName = Auth.User.City,
                        CountryId = Auth.User.CountryId,
                        Country = DataAccess.GetCountryNameByCountryId(Auth.User.ShippingCountryId),
                        Firstname = Auth.User.FirstName,
                        Lastname = Auth.User.LastName,
                        Phone = !String.IsNullOrWhiteSpace(Auth.User.Phone) ? Auth.User.Phone : Auth.User.ShippingPhone,
                        PhoneCountryCode = !String.IsNullOrWhiteSpace(Auth.User.PhoneCountryCode) ? Auth.User.PhoneCountryCode : Auth.User.ShippingPhoneCountryCode,
                        MobilePhone = !String.IsNullOrWhiteSpace(Auth.User.Phone) ? Auth.User.Phone : Auth.User.ShippingPhone,
                        MobilePhoneCountryCode = !String.IsNullOrWhiteSpace(Auth.User.PhoneCountryCode) ? Auth.User.PhoneCountryCode : Auth.User.ShippingPhoneCountryCode,
                        Street = !String.IsNullOrWhiteSpace(Auth.User.Street) ? Auth.User.Street : Auth.User.ShippingStreet,
                        PostalCode = !String.IsNullOrWhiteSpace(Auth.User.PostalCode) ? Auth.User.PostalCode : Auth.User.ShippingPostalCode,
                        UserId = Auth.User.UserId
                    };
                    DataAccess.SaveAddress(address);
                    return Json(new
                    {
                        isSuccess = true
                    });
                }
                else
                {
                    throw new Exception(Resources.Resources.UserHasNotLoggedIn);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public ActionResult PrepareShippingCacheData()
        {
            try
            {
                if (MemoryCache.Default["Cities"] == null)
                {
                    List<City> cities = DataAccess.GetAllCities();
                    MemoryCache.Default.Add("Cities", cities, DateTime.Now.AddDays(1));
                }

                if (MemoryCache.Default["Regions"] == null)
                {
                    List<Region> regions = DataAccess.GetAllRegions();
                    MemoryCache.Default.Add("Regions", regions, DateTime.Now.AddDays(1));
                }

                if (MemoryCache.Default["Countries"] == null)
                {
                    List<Country> countries = DataAccess.GetAllCountries();
                    MemoryCache.Default.Add("Countries", countries, DateTime.Now.AddDays(1));
                }

                return Json(new
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
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
                string country = string.Empty;
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
                    country = result.First().country.CountryName;
                    foreach (var entry in result.ToList())
                    {
                        filteredCities.Add(entry.city);
                    }
                }

                var jsonResult = Json(new
                {
                    isSuccess = true,
                    cityId = cityId,
                    country = country,
                    cities = filteredCities
                });
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public ActionResult GetAllCities()
        {
            try
            {
                List<City> cities = DataAccess.GetAllCities();
                return Json(new
                {
                    isSuccess = true,
                    cities = cities
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region SaveAddress
        [HttpPost]
        public ActionResult SaveAddress(long? AddressId = 0, int? TitleId = null, string Firstname = "", string Lastname = "", string Address = "", string Complement = "", string PostalCode = "", int? CityId = null, string Country = "", string MobilePhone = "", string MobilePhoneCountryCode = "", string Digicode = "", string Floor = "", string AddressName = "")
        {
            try
            {
                if (Auth.User == null)
                    throw new Exception(Resources.Resources.UserHasNotLoggedIn);

                long userId = Auth.User.UserId;

                AddressId = AddressId.HasValue ? AddressId.Value : 0;
                DansLesGolfs.BLL.Address address = null;
                if (AddressId.HasValue)
                {
                    address = DataAccess.GetAddressByAddressId(AddressId.Value);
                    if (address == null)
                        address = new Address();
                }
                else
                {
                    address = new Address();
                }
                address.AddressName = AddressName;
                address.TitleId = TitleId.HasValue ? TitleId.Value : 0;
                address.Firstname = Firstname;
                address.Lastname = Lastname;
                address.Complement = Complement;
                address.Address1 = Address;
                address.PostalCode = PostalCode;
                address.CityId = CityId.HasValue ? CityId.Value : 0;
                address.Country = Country;
                address.Phone = MobilePhone;
                address.PhoneCountryCode = MobilePhoneCountryCode;
                address.MobilePhone = MobilePhone;
                address.MobilePhoneCountryCode = MobilePhoneCountryCode;
                address.Digicode = Digicode;
                address.Floor = Floor;
                address.UserId = userId;
                DataAccess.SaveAddress(address);
                SaveShippingData(address);

                if (Request.IsAjaxRequest())
                {
                    List<Address> adressesList = DataAccess.GetAddressByUserID(userId);
                    string html = GetHTMLFromView("~/Views/_Shared/UC/Front/Order/UCAddressesList.cshtml", adressesList);
                    return Json(new
                    {
                        isSuccess = true,
                        html = html
                    });
                }
                else
                {
                    return RedirectToAction("Shipping");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                if (Request.IsAjaxRequest())
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = ex.Message
                    });
                }
                else
                {
                    TempData["ErrorMessages"] = ex.Message;
                    return RedirectToAction("Shipping");
                }
            }
        }
        #endregion

        #region DeleteAddress
        [HttpPost]
        public ActionResult DeleteAddress(long? addressId = 0)
        {
            try
            {
                if (Auth.User == null)
                    throw new Exception(Resources.Resources.UserHasNotLoggedIn);

                if (!addressId.HasValue)
                    throw new Exception("Invalid Address ID");

                DataAccess.DeleteAddress(addressId.Value);

                List<Address> adressesList = DataAccess.GetAddressByUserID(Auth.User.UserId);
                string html = GetHTMLFromView("~/Views/_Shared/UC/Front/Order/UCAddressesList.cshtml", adressesList);
                return Json(new
                {
                    isSuccess = true,
                    html = html
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region SaveShippingData
        [HttpPost]
        public ActionResult SaveShippingData(int? addressId = 0)
        {
            try
            {
                if (Auth.User == null)
                    throw new Exception(Resources.Resources.UserHasNotLoggedIn);

                if (!addressId.HasValue)
                    throw new Exception("Invalid Address ID");

                Address address = DataAccess.GetAddress(addressId.Value);
                SaveShippingData(address);

                return Json(new
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        public void SaveShippingData(Address address)
        {
            OrderShippingModel model = GetShippingModel();
            model.AddressId = address.AddressId;
            model.Address = address.Address1;
            model.Street = address.Street;
            model.CityName = address.CityName;
            model.CountryId = address.CountryId;
            model.PostalCode = address.PostalCode;
            model.Phone = address.Phone;
            model.PhoneCountryCode = address.PhoneCountryCode;
            model.MobilePhone = address.MobilePhone;
            model.MobilePhoneCountryCode = address.MobilePhoneCountryCode;
            model.ShippingAddress = address.Address1;
            model.ShippingStreet = address.Street;
            model.ShippingCity = address.CityName;
            model.ShippingPostalCode = address.PostalCode;
            model.ShippingCountryId = address.CountryId;
            model.ShippingPhone = address.Phone;
            model.ShippingPhoneCountryCode = address.PhoneCountryCode;
            SaveShippingModel(model);
        }
        #endregion
        #endregion

        #region Order : Confirmation
        public ActionResult Confirmation(long? id = 0)
        {
            long orderId = id.HasValue ? id.Value : 0;
            Order order = DataAccess.GetOrderByOrderId(orderId);

            WebContent content = DataAccess.GetWebContentByKey("order-confirmation", this.CultureId, "ORDER");
            ViewBag.ConfirmationContent = content.ContentText;

            return View(order);
        }

        public ActionResult ConfirmationEmail(long? id = 0)
        {
            Order order = DataAccess.GetOrderByOrderId(id.Value);
            InitConfirmationEmail(order);
            return View(order);
        }

        public ActionResult ConfirmationEmailSite(long? id = 0)
        {
            Order order = DataAccess.GetOrderByOrderId(id.Value);
            InitConfirmationEmail(order);
            var result = from item in order.OrderItems
                         group item by item.SiteId into siteItems
                         select new
                         {
                             SiteId = siteItems.Key,
                             Items = siteItems.ToList()
                         };

            foreach (var it in result)
            {
                if (!it.Items.Any())
                    continue;

                order.OrderItems = it.Items;
                ViewBag.SiteId = it.SiteId;
                break;
            }


            ViewBag.SiteUrl = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"];
            ViewBag.BuyerName = Auth.User.Fullname;
            ViewBag.BuyerEmail = Auth.User.Email;
            ViewBag.BuyerPhone = "+01 1230 2432";
            ViewBag.OrderItems = order.OrderItems;
            return View(order);
        }

        private void InitConfirmationEmail(Order order)
        {
            ViewBag.SiteName = System.Configuration.ConfigurationManager.AppSettings["SiteName"].ToString();

            ViewBag.PaymentType = TransactionHelper.GetPaymentTypeText(order.PaymentType);

            ViewBag.ReceiverEmail = "sample@email.com";
            ViewBag.Title = Resources.Resources.ConfirmPaymentTitle + " " + ViewBag.PaymentType;
        }

        private void SendConfirmationEmail(Order order)
        {
            OrderShippingModel model = GetShippingModel();
            InitConfirmationEmail(order);
            if (order == null)
                return;

            string siteUrl = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
            string siteEmail = System.Configuration.ConfigurationManager.AppSettings["SiteEmail"].ToString();
            ViewBag.ReceiverEmail = Auth.User.Email;
            EmailArguments emailArgs = EmailArguments.GetInstanceFromConfig();
            emailArgs.SenderEmail = siteEmail;
            emailArgs.SenderName = siteUrl;
            emailArgs.Subject = ViewBag.Title;
            emailArgs.To.Add(Auth.User.Email, Auth.User.FullName);
            emailArgs.BCC.Add("weerayut@infogrammer.com", "Weerayut");
            emailArgs.BCC.Add(siteEmail, siteUrl);
            emailArgs.MailBody = GetHTMLFromView("~/Views/Order/ConfirmationEmail.cshtml", order);
            EmailHelper.SendEmailWithAttachments(emailArgs);

            ViewBag.SiteUrl = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"];
            ViewBag.BuyerName = Auth.User.FullName;
            ViewBag.BuyerEmail = Auth.User.Email;
            ViewBag.BuyerPhone = model.ShippingPhoneComplete;

            // Send E-mail for each site.
            var result = from item in order.OrderItems
                         group item by item.SiteId into siteItems
                         select new
                         {
                             SiteId = siteItems.Key,
                             Items = siteItems.ToList()
                         };

            OrderItem orderItem = null;
            foreach (var it in result)
            {
                if (!it.Items.Any())
                    continue;

                orderItem = it.Items.First();
                ViewBag.OrderItems = it.Items;
                ViewBag.SiteId = it.SiteId;
                if (String.IsNullOrEmpty(orderItem.SiteEmail) || String.IsNullOrWhiteSpace(orderItem.SiteEmail))
                    continue;

                ViewBag.ReceiverEmail = orderItem.SiteEmail;
                emailArgs = EmailArguments.GetInstanceFromConfig();
                emailArgs.SenderEmail = siteEmail;
                emailArgs.SenderName = siteUrl;
                emailArgs.Subject = ViewBag.Title;
                emailArgs.To.Add(orderItem.SiteEmail, orderItem.SiteEmail);
                emailArgs.BCC.Add("weerayut@infogrammer.com", "Weerayut");
                emailArgs.MailBody = GetHTMLFromView("~/Views/Order/ConfirmationEmailSite.cshtml", order);
                EmailHelper.SendEmailWithAttachments(emailArgs);
            }
        }
        #endregion

        #endregion

        #region Private Methods

        #region CreatePaymentViewBags
        private void CreatePaymentViewBags()
        {
            bool usePaypalSandbox = ViewBag.UsePaypalSandbox = DataManager.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UsePaypalSandbox"]);
            ViewBag.PaypalUrl = GetPaypalUrl(usePaypalSandbox);
            ViewBag.PaypalBusinessEmail = System.Configuration.ConfigurationManager.AppSettings["PaypalBusinessEmail"];
        }
        #endregion

        #region GetPayPalResponse
        string GetPayPalResponse()
        {
            // Parse the variables
            var formVals = Request.Form;
            formVals.Add("cmd", "_notify-validate");

            // Choose whether to use sandbox or live environment
            bool useSandbox = DataManager.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UsePaypalSandbox"]);
            string paypalUrl = GetPaypalUrl(useSandbox);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(paypalUrl);

            // Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            byte[] param = Request.BinaryRead(Request.ContentLength);
            string strRequest = Encoding.ASCII.GetString(param);

            StringBuilder sb = new StringBuilder();
            sb.Append(strRequest);

            foreach (string key in formVals.Keys)
            {
                sb.AppendFormat("&{0}={1}", key, formVals[key]);
            }
            strRequest += sb.ToString();
            req.ContentLength = strRequest.Length;

            //for proxy
            //WebProxy proxy = new WebProxy(new Uri("http://urlort#");
            //req.Proxy = proxy;
            //Send the request to PayPal and get the response
            string response = "";
            using (StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII))
            {

                streamOut.Write(strRequest);
                streamOut.Close();
                using (StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    response = streamIn.ReadToEnd();
                }
            }

            return response;
        }
        #endregion

        #region GetPaypalUrl
        private string GetPaypalUrl(bool useSandbox = false)
        {
            return useSandbox ? @"https://www.sandbox.paypal.com/cgi-bin/webscr" : @"https://www.paypal.com/cgi-bin/webscr";
        }
        #endregion

        #region GetShippingModel
        private OrderShippingModel GetShippingModel()
        {
            OrderShippingModel model = new OrderShippingModel();

            if (Session[orderShippingSessionName] != null && Session[orderShippingSessionName] is OrderShippingModel)
            {
                OrderShippingModel orderShipping = Session[orderShippingSessionName] as OrderShippingModel;
                model.AddressId = orderShipping.AddressId;
                model.Email = orderShipping.Email;
                model.Address = orderShipping.Address;
                model.Street = orderShipping.Street;
                model.CityName = orderShipping.CityName;
                model.CountryId = orderShipping.CountryId;
                model.PostalCode = orderShipping.PostalCode;
                model.Phone = orderShipping.Phone;
                model.PhoneCountryCode = orderShipping.PhoneCountryCode;
                model.MobilePhone = orderShipping.MobilePhone;
                model.MobilePhoneCountryCode = orderShipping.MobilePhoneCountryCode;
                model.ShippingAddress = orderShipping.ShippingAddress;
                model.ShippingStreet = orderShipping.ShippingStreet;
                model.ShippingCity = orderShipping.ShippingCity;
                model.ShippingPostalCode = orderShipping.ShippingPostalCode;
                model.ShippingCountryId = orderShipping.ShippingCountryId;
                model.ShippingPhone = orderShipping.ShippingPhone;
                model.ShippingPhoneCountryCode = orderShipping.ShippingPhoneCountryCode;
                model.LydiaRequestId = orderShipping.LydiaRequestId;
                model.TransactionId = orderShipping.TransactionId;
                model.OrderNumber = orderShipping.OrderNumber;

                if (Auth.User != null)
                    model.CustomerId = Auth.User.UserId;
            }
            else if (Auth.User != null)
            {
                Auth.RefreshUserInfo();
                model.GenerateTransactionId();
                model.CustomerId = Auth.User.UserId;
                model.Email = Auth.User.Email;
                model.Address = Auth.User.Address;
                model.Street = Auth.User.Street;
                model.CityName = Auth.User.City;
                model.CountryId = Auth.User.CountryId;
                model.PostalCode = Auth.User.PostalCode;
                model.Phone = Auth.User.Phone;
                model.PhoneCountryCode = Auth.User.PhoneCountryCode;
                model.MobilePhone = Auth.User.MobilePhone;
                model.MobilePhoneCountryCode = Auth.User.MobilePhoneCountryCode;
                model.ShippingAddress = Auth.User.ShippingAddress;
                model.ShippingStreet = Auth.User.ShippingStreet;
                model.ShippingCity = Auth.User.ShippingCity;
                model.ShippingPostalCode = Auth.User.ShippingPostalCode;
                model.ShippingCountryId = Auth.User.ShippingCountryId;
                model.ShippingPhone = Auth.User.ShippingPhone;
                model.ShippingPhoneCountryCode = Auth.User.ShippingPhoneCountryCode;
                SaveShippingModel(model);
            }

            model.Cart = cart;
            return model;
        }
        private void SaveShippingModel(OrderShippingModel model)
        {
            Session[orderShippingSessionName] = model;
        }
        #endregion

        #region SaveOrder
        private void SaveOrder(Order order, bool forceNew = false, bool skipSendConfirmationEmail = false)
        {
            bool isNew = false;
            if (ShoppingCart.Instance.Coupon != null)
            {
                order.CouponId = ShoppingCart.Instance.Coupon.CouponId;
            }
            order.CreatedDate = order.ModifiedDate = order.OrderDate = DateTime.Now;

            if (forceNew ||
                (order.OrderId <= 0
                && order.Active
                && ((order.PaymentType == "check" && order.PaymentStatus == "pending")
                || (order.PaymentType != "check" && order.PaymentStatus == "success"))))
                isNew = true;

            try
            {
                DataAccess.BeginTransaction();
                order.OrderId = DataAccess.SaveOrder(order);

                if (order.OrderId > 0)
                {
                    if (isNew) // For new order, just send confirmation email & confirm albatros webservice.
                    {
                        CheckAlbatrosAPI(order, ShoppingCart.Instance);
                        CheckPrimaAPI(order, ShoppingCart.Instance);
                        if (!skipSendConfirmationEmail)
                        {
                            SendConfirmationEmail(order);
                        }
                    }
                    DataAccess.CommitTransaction();
                    DataAccess.BeginTransaction();
                    DataAccess.SaveOrder(order);
                }
                DataAccess.CommitTransaction();

                foreach (CartItem ci in ShoppingCart.Instance.Items)
                {
                    if (ci.DlgCardStyleId > 0)
                    {
                        for (int i = 0; i < ci.Quantity; i++)
                        {
                            var randigit = DataAccess.RandomDigit();
                            DataAccess.SaveDlgCard(Convert.ToInt32(ci.ItemId), ci.FirstName, ci.LastName, ci.Email, ci.PersonalMessage, ci.itemPriceDlgCardId, Auth.User.UserId, ci.DlgCardStyleId, randigit.ToString(), Convert.ToInt32(order.OrderId));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                DataAccess.RollbackTransaction();
                throw ex;
            }
        }

        private void CheckAlbatrosAPI(Order order, ShoppingCart cart)
        {
            DateTime startTime = DateTime.Now;
            int teeId = 0;
            string placeNo = string.Empty, gameType = string.Empty;
            Site site = null;

            #region Albatros
            var albatrosGreenFees = cart.Items.Where(it => it.SpecialData.ContainsKey("AlbatrosTeeTimeId"));
            foreach (CartItem item in albatrosGreenFees)
            {
                site = DataAccess.GetSiteAlbatrosSettingBySiteId(item.Item.SiteId);
                if (site == null)
                    continue;

                Albatros.SetConnection(site.AlbatrosUrl, site.AlbatrosUsername, site.AlbatrosPassword);
                Albatros.Login();

                if (!Albatros.IsLoggedIn)
                    throw new Exception("Can't login to Albatros.");

                if (item.SpecialData.ContainsKey("AlbatrosTeeTimeId") && item.SpecialData.ContainsKey("AlbatrosTeeStartTime"))
                {
                    teeId = DataManager.ToInt(item.SpecialData["AlbatrosTeeTimeId"]);
                    startTime = DataManager.ToDateTime(item.SpecialData["AlbatrosTeeStartTime"]);
                    var confirmResponse = Albatros.ConfirmReservations(item.RefCode, Auth.User, DateTime.Today, item.Quantity, item.TotalPrice, site.Email, order.TransactionId);
                    if (confirmResponse.code != "0")
                    {
                        throw new Exception("Method: ConfirmReservations, Error Code: " + confirmResponse.code + ", Message: " + confirmResponse.message);
                    }
                    else
                    {
                        if (confirmResponse.confirmResults.confirmResult.code != "0")
                        {
                            throw new Exception("Method: ConfirmReservations (Result), Error Code: " + confirmResponse.confirmResults.confirmResult.code + ", Message: " + confirmResponse.confirmResults.confirmResult.message);
                        }
                    }
                }

                Albatros.Logout();
            }
            #endregion
        }

        private void CheckPrimaAPI(Order order, ShoppingCart cart)
        {
            DateTime bookingDateTime = DateTime.Now;
            DateTime? bookingDateTime9In = DateTime.Now;
            string courseId = string.Empty, gameType = string.Empty, lockCode = string.Empty, error = string.Empty;;
            string[] bookingIds = null;
            Site site = null;

            #region Prima
            var primaGreenFees = cart.Items.Where(it => it.SpecialData.ContainsKey("PrimaCourseId"));
            PrimaDataAccess prima = null;
            foreach (CartItem item in primaGreenFees)
            {
                site = DataAccess.GetSitePrimaSettingBySiteId(item.Item.SiteId);
                if (site == null)
                    continue;

                error = string.Empty;

                courseId = DataManager.ToString(item.SpecialData["PrimaCourseId"]);
                bookingDateTime = DataManager.ToDateTime(item.SpecialData["PrimaTeeStartTime"]);
                bookingDateTime9In = (DateTime?)item.SpecialData["PrimaTeeStartTime9In"];
                gameType = DataManager.ToString(item.SpecialData["PrimaGameType"]);
                lockCode = DataManager.ToString(item.SpecialData["PrimaLockCode"]);

                prima = new PrimaDataAccess(site.PrimaAPIKey, site.PrimaClubKey);
                if (prima.Confirm(courseId, bookingDateTime, item.Quantity, lockCode, Auth.User, bookingDateTime9In, out bookingIds, out error))
                {
                    item.RefCode = String.Join(", ", bookingIds);
                    var orderItems = order.OrderItems.Where(it => it.ItemId == item.ItemId);
                    if(orderItems != null &&orderItems.Any())
                    {
                        foreach(var o in orderItems)
                        {
                            o.ItemName = item.Description;
                        }
                    }
                }
                else
                {
                    throw new Exception("Can't add a reservation at this time.");
                }
            }
            #endregion
        }
        #endregion

        #endregion
    }
}
