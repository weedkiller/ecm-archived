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
using System.Data;
using System.Net.Mail;
using System.Configuration;
using DansLesGolfs.Models;
using Resources;
using System.Runtime.Caching;

namespace DansLesGolfs.Controllers
{
    public class AccountController : BaseFrontSecurityController
    {
        #region Constructor
        public AccountController()
            : base()
        {
            LoadAdsList("Member Area");
        }
        #endregion

        #region "Dashboard"

        [HttpGet]
        public ActionResult Index()
        {
            SqlDataAccess DataAccess = DataFactory.GetInstance();
            if (Auth.User != null)
            {
                int totalOrders = 0, totalMessages = 0, totalReferals = 0;
                DataAccess.GetUserNotifications(Auth.User.UserId, out totalOrders, out totalMessages, out totalReferals);
                ViewBag.TotalUserOrders = totalOrders;
                ViewBag.TotalUserMessage = totalMessages;
                ViewBag.TotalSponsorEmails = totalReferals;
            }
            return View();
        }

        #endregion

        #region "Detail"

        public ActionResult UpdateConnectWithFacebook(int userId)
        {
            LoadAdsList("Member Area");

            DataAccess.UpdateConnectWithFacebook(userId, "");

            return RedirectToAction("Details", "Account");
        }

        [HttpGet]
        public ActionResult Details()
        {
            int userId = ViewBag.LogonUserID;

            User user = DataAccess.GetUser(userId);
            ViewBag.User = user;
            ViewBag.Countries = DataAccess.GetAllCountries();

            ViewBag.Civilite = user.TitleId;

            List<Title> titles = DataAccess.GetItemTitlesDropDownList(this.CultureId);
            ViewBag.DropDownTitles = ListToDropDownList<Title>(titles, "TitleId", "TitleName");

            if (user.Birthdate.HasValue)
            {
                ViewBag.getDays = Getdays(user.Birthdate.Value.Day);
                ViewBag.getmonth = Getmonth(user.Birthdate.Value.Month);
                ViewBag.getyear = Getyears(user.Birthdate.Value.Year);
            }
            else
            {
                ViewBag.getDays = Getdays();
                ViewBag.getmonth = Getmonth();
                ViewBag.getyear = Getyears();
            }
            ViewBag.IsFacebookId = user.FBAccount;

            return View();
        }

        [HttpPost]
        public ActionResult Details(FormCollection Form)
        {
            int userId = ViewBag.LogonUserID;
            SqlDataAccess DataAccess = DataFactory.GetInstance();
            try
            {
                User user = DataAccess.GetUser(userId);
                //user.Password = Form["password"];
                //user.Civilite = DataManager.ToInt(Form["city"]);
                user.FirstName = Form["firstname"];
                user.LastName = Form["lastname"];
                user.Birthdate = DataManager.ToDateTime(Form["birthdate"]);
                user.TitleId = DataManager.ToInt(Form["Civilite"]);

                user.Address = Form["address"];
                user.CountryId = DataManager.ToInt(Form["country"]);
                user.CityId = DataManager.ToInt(Form["city"]);
                user.PostalCode = Form["postalcode"];
                user.Phone = Form["phone"];
                user.PhoneCountryCode = Form["phone_country_code"];
                user.MobilePhone = Form["phone"];
                user.MobilePhoneCountryCode = Form["phone_country_code"];

                user.ShippingAddress = Form["address"];
                user.ShippingCountryId = DataManager.ToInt(Form["country"]);
                user.ShippingCityId = DataManager.ToInt(Form["city"]);
                user.ShippingPostalCode = Form["postalcode"];
                user.ShippingPhone = Form["phone"];
                user.ShippingPhoneCountryCode = Form["phone_country_code"];

                //user.ShippingCity = 
                //user.XXXid = Form["pays"];
                //user.Complement = Form["complement"];
                //user.portable = Form["portable"];
                //user.profession = Form["profession"];
                user.IsReceiveEmailInfo = Form["receive"] == "True";

                if (string.IsNullOrEmpty(user.Email))
                {
                    TempData["ErrorMessage"] = Resources.Resources.YouMustEnterRequireField;
                }
                else
                {
                    int result = DataAccess.UpdateUser(user);
                    TempData["SuccessMessage"] = Resources.Resources.SaveUserProfileSuccess;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Resources.YouMustEnterRequireField;
                //TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Details");
        }

        public JsonResult ValidateEmail(string email)
        {
            bool result = false;

            result = DataAccess.IsExistsEmail(email);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public SelectList GetCivilityList(int? valueseleted = null)
        {



            var result = DataAccess.GetItemTitlesDropDownList(this.CultureId);
            SelectList items = new SelectList(result, "TitleId", "TitleName", valueseleted);
            //items.Add(new SelectListItem { Text = "-", Value = "", Selected = valueseleted.Equals("") });

            //items.Add(new SelectListItem { Text = "Monsieur", Value = "Monsieur" ,Selected = valueseleted.Equals("Monsieur") });

            //items.Add(new SelectListItem { Text = "Madame", Value = "Madame", Selected = valueseleted.Equals("Madame") });

            //items.Add(new SelectListItem { Text = "Mademoiselle", Value = "Mademoiselle", Selected = valueseleted.Equals("Mademoiselle") });

            return items;
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


        #endregion

        #region "Orders"

        public ActionResult Orders()
        {
            List<AccountOrder> vm = new List<AccountOrder>();
            vm = DataAccess.GetAccountOrderList(Auth.User.UserId);
            DataAccess.UpdateOrderNoticeStatusByUserId(true, Auth.User.UserId);
            return View(vm);
        }

        public ActionResult OrdersDetail(int orderId)
        {
            Order order = DataAccess.GetOrderByOrderId(orderId);

            return View(order);
        }

        public ActionResult SendOrderHelpEmail(long? id, string subject, string message)
        {
            try
            {
                if (!id.HasValue)
                    throw new Exception("Please specific Order ID.");

                Order order = DataAccess.GetOrderByOrderId(id.Value);
                if (order == null)
                    throw new Exception("Not found order that has Order ID = " + id.Value + ".");

                ViewBag.CustomerFullname = Auth.User.FullName;
                ViewBag.CustomerEmail = Auth.User.Email;
                ViewBag.CustomerPhone = !String.IsNullOrWhiteSpace(Auth.User.Phone) ? Auth.User.PhoneCountryCode + Auth.User.Phone : (!String.IsNullOrWhiteSpace(Auth.User.MobilePhone) ? Auth.User.MobilePhoneCountryCode + Auth.User.MobilePhone : "-");
                ViewBag.Subject = subject.Trim();
                ViewBag.Message = HttpUtility.HtmlEncode(message.Trim()).Replace(" ", "&nbsp;").Replace("\n", "<br />\n");

                string siteUrl = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
                string siteEmail = System.Configuration.ConfigurationManager.AppSettings["SiteEmail"].ToString();

                EmailArguments emailArgs = EmailArguments.GetInstanceFromConfig();
                ViewBag.ReceiverEmail = siteEmail;
                emailArgs.Subject = Resources.Resources.OrderSupport;
                emailArgs.SenderName = Auth.User.FullName;
                emailArgs.SenderEmail = Auth.User.Email;
                emailArgs.To.Add(siteEmail, siteUrl);
                emailArgs.BCC.Add("kenessar@gmail.com", "kenessar@gmail.com");
                emailArgs.MailBody = GetHTMLFromView("~/Views/Account/EmailOrderHelp.cshtml", order);
                EmailHelper.SendEmailWithAttachments(emailArgs);

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

        public ActionResult ViewOrderInvoice(long? id)
        {
            DansLesGolfs.Reports.rpOrderInvoice report = GetOrderInvoiceReport(id);
            return View(report);
        }
        public ActionResult OrderInvoiceDocViewPartial(long? id)
        {
            DansLesGolfs.Reports.rpOrderInvoice report = GetOrderInvoiceReport(id);
            return PartialView("OrderInvoiceDocViewer", report);
        }

        public ActionResult OrderInvoiceExportDocumentViewer(long? id)
        {
            DansLesGolfs.Reports.rpOrderInvoice report = GetOrderInvoiceReport(id);
            return DevExpress.Web.Mvc.DocumentViewerExtension.ExportTo(report);
        }

        private Reports.rpOrderInvoice GetOrderInvoiceReport(long? id)
        {
            DansLesGolfs.Reports.rpOrderInvoice report = new Reports.rpOrderInvoice();
            report.DisplayName = "OrderInvoice_" + DateTime.Now.ToString("yyyy-MM-dd");

            Order order = DataAccess.GetOrderByOrderId(id.Value);
            if (order == null)
            {
                ViewBag.OrderId = 0;
                ViewBag.OrderNumber = "";
                ViewBag.ErrorMessage = "Not found order that has Order ID = " + id.Value + ".";
            }
            else
            {
                ViewBag.OrderId = order.OrderId;
                ViewBag.OrderNumber = order.OrderNumber;

                Address addr = DataAccess.GetAddress(order.AddressId);

                WebContent content = DataAccess.GetWebContentByKey("legal-information", this.CultureId);

                string siteUrl = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"];
                string siteEmail = System.Configuration.ConfigurationManager.AppSettings["SiteEmail"];

                report.DataMember = "";
                report.DataSource = order.OrderItems;

                // Assign parameters.
                report.Parameters["InvoiceTitle"].Value = Resources.Resources.Invoice.ToUpper();
                report.Parameters["TransactionId"].Value = Resources.Resources.InvoiceNumber + order.TransactionId;
                report.Parameters["OrderNumber"].Value = Resources.Resources.OrderNumberInvoice + order.OrderNumber;
                report.Parameters["OrderDate"].Value = "Date : " + order.OrderDate.ToString("dd/MM/yyyy");
                report.Parameters["PaymentType"].Value = Resources.Resources.MethodOfSettlement + " : " + TransactionHelper.GetPaymentTypeText(order.PaymentType);
                DevExpress.XtraReports.UI.XRRichText xrrichInvoiceMemo = report.FindControl("xrrichInvoiceMemo", true) as DevExpress.XtraReports.UI.XRRichText;
                if (xrrichInvoiceMemo != null)
                {
                    string invoiceMemo = String.Format(Resources.Resources.InvoiceMemo, siteUrl) + " : " + siteEmail;
                    System.Windows.Forms.RichTextBox textbox = new System.Windows.Forms.RichTextBox();
                    textbox.Text = invoiceMemo;
                    textbox.Select(invoiceMemo.LastIndexOf(siteEmail) - 1, siteEmail.Length);
                    textbox.SelectionFont = new System.Drawing.Font(xrrichInvoiceMemo.Font, System.Drawing.FontStyle.Bold);
                    xrrichInvoiceMemo.Rtf = textbox.Rtf;
                }

                if (addr != null)
                {
                    report.Parameters["BillingAddress"].Value = addr.ToString();
                }
                else
                {
                    report.Parameters["BillingAddress"].Value = string.Empty;
                }

                if (content != null)
                {
                    report.Parameters["LegalInformation"].Value = HttpUtility.HtmlDecode(content.ContentText.StripHtml());
                }
                else
                {
                    report.Parameters["LegalInformation"].Value = string.Empty;
                }

                // Table Header
                report.Parameters["TableHeaderDescription"].Value = Resources.Resources.Description;
                report.Parameters["TableHeaderUnitPrice"].Value = Resources.Resources.UnitPrice;
                report.Parameters["TableHeaderQuantity"].Value = Resources.Resources.Quantity;
                report.Parameters["TableHeaderTotalPrice"].Value = Resources.Resources.TotalTTC;

                // Summary Values
                decimal baseTotal = order.GetBaseTotal();
                decimal totalShippingCost = order.GetTotalShippingCost();
                decimal totalDiscount = order.GetDiscount();
                decimal totalPrice = order.GetTotalPrice();
                decimal totalVat = order.GetTotalVAT();
                decimal totalWithoutVat = totalPrice - totalVat;
                report.Parameters["BaseTTC"].Value = baseTotal;
                report.Parameters["PortTTC"].Value = totalShippingCost;
                report.Parameters["TotalDiscount"].Value = totalDiscount;
                report.Parameters["TotalWithoutVAT"].Value = totalWithoutVat;
                report.Parameters["TotalVAT"].Value = totalVat;
                report.Parameters["TotalWithVAT"].Value = totalPrice;
            }
            return report;
        }

        #endregion

        #region "Messages"

        public ActionResult Messages(int pageIndex = 1)
        {

            UserMessageViewModel vm = new UserMessageViewModel();
            vm.PageIndex = pageIndex;
            var Setpage = pageIndex == 0 ? 0 : (pageIndex - 1) * 10;
            vm.UserMessageModelList = DataAccess.GetUserMessageList(Setpage, 10, null, null, null, Auth.User.UserId, null);
            return View("Messages", vm);
        }

        public ActionResult MessagesSearch(UserMessageViewModel vm)
        {

            //UserMessageViewModel vm = new UserMessageViewModel();
            vm.PageIndex = vm.PageIndex;
            var Setpage = vm.PageIndex == 0 ? 0 : (vm.PageIndex - 1) * 10;
            vm.UserMessageModelList = DataAccess.GetUserMessageList(Setpage, 10, null, null, vm.SearchText, Auth.User.UserId, null);
            return View("Messages", vm);
        }

        [HttpPost]
        public ActionResult MessagesSearch(int pageIndex = 1, string searchText = null)
        {

            UserMessageViewModel vm = new UserMessageViewModel();
            vm.PageIndex = vm.PageIndex;
            vm.SearchText = searchText;
            var Setpage = vm.PageIndex == 0 ? 0 : (vm.PageIndex - 1) * 10;
            vm.UserMessageModelList = DataAccess.GetUserMessageList(Setpage, 10, null, null, searchText, Auth.User.UserId, null);
            return View("Messages", vm);
        }



        public ActionResult GetUserMessageByUserId(int pageIndex)
        {
            UserMessageViewModel vm = new UserMessageViewModel();
            vm.UserMessageModelList = DataAccess.GetUserMessageList(pageIndex * 10, 10, null, null, null, Auth.User.UserId, null);
            return View("Messages", vm);
        }

        public ActionResult MessageDetail(int messageId)
        {
            UserMessageViewModel vm = new UserMessageViewModel();
            vm.UserMessageModelList = DataAccess.GetUserMessageList(0, 10, null, null, null, Auth.User.UserId, messageId);
            return View("MessageDetail", vm);
        }

        //public ActionResult MessageDetail(int messageId)
        //{
        //    int userId = ViewBag.LogonUserID;

        //    DataSet ds = DataAccess.GetUserMessages(userId);

        //    UserMessage userMessageDetail = new UserMessage();
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        userMessageDetail.MessageId = DataManager.ToInt(row["MessageId"]);
        //        userMessageDetail.Subject = DataManager.ToString(row["Subject"]);
        //        userMessageDetail.Body = DataManager.ToString(row["Body"]);
        //        userMessageDetail.SentDate = DataManager.ToDateTime(row["SentDate"]);
        //        userMessageDetail.ReadDate = DataManager.ToDateTime(row["ReadDate"]);
        //        userMessageDetail.FromUser = DataManager.ToString(row["FromUserName"]);
        //        userMessageDetail.ToUser = DataManager.ToString(row["ToUserName"]);
        //        userMessageDetail.IsFlag = DataManager.ToBoolean(row["IsFlag"]);
        //        userMessageDetail.HasAttachedFile = DataManager.ToBoolean(row["HasAttachedFile"]);
        //    }

        //    ViewBag.UserMessageDetail = userMessageDetail;

        //    return View();
        //}

        public JsonResult UpdateReadMessage(int messageId)
        {
            var result = DataAccess.UpdateReadMessage(messageId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateFlagMessage(int messageId)
        {
            var result = DataAccess.UpdateFlagMessage(messageId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<SelectListItem> Getmonth(int? selectmonth = null)
        {
            List<SelectListItem> monthlist = new List<SelectListItem>
                    {
                        new SelectListItem{Text = "Month", Value="" },
                        new SelectListItem{Text = Resources.Resources.January, Value="1", Selected = selectmonth.ToString().Equals("1")},
                        new SelectListItem{Text = Resources.Resources.February, Value="2", Selected = selectmonth.ToString().Equals("2")},
                        new SelectListItem{Text = Resources.Resources.March, Value="3", Selected = selectmonth.ToString().Equals("3")},
                        new SelectListItem{Text = Resources.Resources.April, Value="4", Selected = selectmonth.ToString().Equals("4")},
                        new SelectListItem{Text = Resources.Resources.May, Value="5" , Selected = selectmonth.ToString().Equals("5")},
                        new SelectListItem{Text = Resources.Resources.June, Value="6", Selected = selectmonth.ToString().Equals("6")},
                        new SelectListItem{Text = Resources.Resources.July, Value="7", Selected = selectmonth.ToString().Equals("7")},
                        new SelectListItem{Text = Resources.Resources.August, Value="8", Selected = selectmonth.ToString().Equals("8")},
                        new SelectListItem{Text = Resources.Resources.September, Value="9" , Selected = selectmonth.ToString().Equals("9")},
                        new SelectListItem{Text = Resources.Resources.October, Value="10", Selected = selectmonth.ToString().Equals("10")},
                        new SelectListItem{Text = Resources.Resources.November, Value="11", Selected = selectmonth.ToString().Equals("11")},
                        new SelectListItem{Text = Resources.Resources.December, Value="12", Selected = selectmonth.ToString().Equals("12")},
                    };
            return monthlist;
        }


        private List<SelectListItem> Getdays(int? selectday = null)
        {
            List<SelectListItem> dayslist = new List<SelectListItem>();
            dayslist.Add(new SelectListItem { Text = "Days", Value = "" });
            for (int i = 1; i < 32; i++)
            {
                dayslist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = i.ToString().Equals(selectday.ToString()) });
            }

            return dayslist;
        }


        private List<SelectListItem> Getyears(int? selectyear = null)
        {
            List<SelectListItem> yearslist = new List<SelectListItem>();
            yearslist.Add(new SelectListItem { Text = "Years", Value = "" });
            for (int i = DateTime.Now.Year; i > DateTime.Now.Year - 120; i--)
            {
                yearslist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = i.ToString().Equals(selectyear.ToString()) });

            }

            return yearslist;
        }

        #endregion

        #region "Address"

        public ActionResult Addresses()
        {
            ViewBag.ListAddress = DataAccess.GetAddressByUserID((int)ViewBag.LogonUserID);
            return View("Addresses");
        }

        public ActionResult AddressesDetail(int? id)
        {
            if (id == null || id == 0)
            {
                ViewBag.Address = string.Empty;
                ViewBag.Street = string.Empty;
                ViewBag.CityId = 0;
                ViewBag.PostalCode = string.Empty;
                ViewBag.Countries = string.Empty;
                ViewBag.Phone = string.Empty;
                ViewBag.PhoneCountryCode = string.Empty;
                ViewBag.MobilePhone = string.Empty;
                ViewBag.MobilePhoneCountryCode = string.Empty;
                ViewBag.Firstname = string.Empty;
                ViewBag.Lastname = string.Empty;
                ViewBag.Floor = string.Empty;
                ViewBag.AddressName = string.Empty;
                ViewBag.id = 0;
            }
            else
            {
                Address addr = DataAccess.GetAddress(id ?? 0);
                ViewBag.Address = addr.Address1;
                ViewBag.Street = addr.Street;
                ViewBag.CityId = addr.CityId;
                ViewBag.PostalCode = addr.PostalCode;
                ViewBag.Countries = addr.CountryId;
                ViewBag.Phone = addr.Phone;
                ViewBag.PhoneCountryCode = addr.PhoneCountryCode;
                ViewBag.MobilePhone = addr.MobilePhone;
                ViewBag.MobilePhoneCountryCode = addr.MobilePhoneCountryCode;
                ViewBag.Firstname = addr.Firstname;
                ViewBag.Lastname = addr.Lastname;
                ViewBag.Floor = addr.Floor;
                ViewBag.AddressName = addr.AddressName;
                ViewBag.id = id;
            }
            List<Country> countries = DataAccess.GetAllCountries();
            ViewBag.Countries = ListToDropDownList<Country>(countries, "CountryId", "CountryName");
            return View("AddressesDetail");
        }

        #region [HttpPost] AddressesDetail
        [HttpPost]
        public ActionResult AddressesDetail(FormCollection Form)
        {
            int userId = ViewBag.LogonUserID;
            SqlDataAccess DataAccess = DataFactory.GetInstance();
            Address addr = new Address();

            addr.Address1 = Form["Address"];
            addr.CountryId = DataManager.ToInt(Form["CountryId"]);
            addr.PostalCode = Form["postalcode"];
            addr.Phone = addr.MobilePhone = Form["Phone"];
            addr.PhoneCountryCode = addr.MobilePhoneCountryCode = Form["PhoneCountryCode"];
            addr.Street = Form["Address"];
            addr.CityId = DataManager.ToInt(Form["CityId"]);
            addr.AddressName = Form["AddressName"];
            addr.Firstname = Form["Firstname"];
            addr.Lastname = Form["Lastname"];
            addr.Floor = Form["Floor"];
            addr.UserId = userId;
            addr.AddressId = DataManager.ToInt(Form["id"]);
            int result = 0;
            if (addr.AddressId == 0)
            {
                result = DataAccess.AddAddress(addr);
            }
            else
            {
                result = DataAccess.UpdateAddress(addr);
            }
            if (result <= 0)
            {
                TempData["ErrorMessage"] = Resources.Resources.ErrorSaveFailed;
            }

            //return Json(new
            //{
            //    isSuccess = isSuccess,
            //    message = message
            //});
            return RedirectToAction("Addresses");
        }
        #endregion

        public ActionResult DeleteAddress(int id)
        {
            int result = DataAccess.DeleteAddress(id);
            return RedirectToAction("Addresses");
        }
        #endregion

        #region "Change Password"

        public ActionResult ChangePassword()
        {
            return View();
        }

        #region [HttpPost] Index
        [HttpPost]
        public ActionResult ChangePassword(RegisterModel model)
        {

            if (DataAccess.CheckPassword(DataProtection.Encrypt(model.OldPassword)))
            {
                if (model.Password.Trim() == model.ConfirmPassword.Trim())
                {
                    int userId = ViewBag.LogonUserID;
                    if (DataAccess.UpdatePassword(DataProtection.Encrypt(model.Password), userId) > 0)
                    {
                        ViewBag.SuccessMessage = Resources.Resources.PasswordHasBeenChanged;
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = Resources.Resources.PasswordNotMatch;
                }
            }
            else
            {
                ViewBag.ErrorMessage = Resources.Resources.InvalidPassword;
            }
            return View(model);
        }
        #endregion
        #endregion

        #region "Sponsorship"

        [HttpGet]
        public ActionResult Sponsorship(string Email = "")
        {

            AccountSponsorship vm = new AccountSponsorship();
            vm.SponsorEmailFrom = Auth.User.Email;
            vm.SponsorFullName = Auth.User.FullName;

            var ListGodchildren = new List<GodChildren>();

            var data = DataAccess.GetWebContentByKey("Sponsorship", 1, "");
            //var SponsorContent = "Bonjour,\n"
            //+ "Rejoins-moi sur DansLesGolfs.com, le premier site des vente en ligne\n"
            //+ "des golfs français.\n"
            //+ "Gràce à DansLesGolfs.com, j'ai accés au meilleur du golf en ligne : \n"
            //+ "green-fees, séjours, matériel,stages, cartes de practice...!\n"
            //+ "A tout de suite sur DansLesGolfs.com!\n";

            EmailTemplate template = DataAccess.GetEmailTemplateByKey("sponsorship", this.CultureId);
            vm.SubjectEmail = PersonalizeText(template.Subject);
            vm.SponsorContent = PersonalizeText(template.TextDetailString);
            //vm.SponsorContent =  Resources.Resources.MsgDetailSponsor; 
            vm.EmailTo = Email; // กรณีที่กดมาจาก Footer จะส่งอีเมล์มาด้วยครับ By OAT

            var AllSponsor = DataAccess.GetAllSponsorEmail(Auth.User.UserId);
            foreach (var _sponsor in AllSponsor)
            {
                var model = new GodChildren();
                model.SponsorEmail = _sponsor.ToEmail;
                model.SponsorDate = _sponsor.InsertDate.ToString("MMMM d, yyyy");
                ListGodchildren.Add(model);
            }

            vm.GodChildren = GetGodChildrenByIndexSize(ListGodchildren, 1, 10);
            ViewBag.AuthUser = Auth.User.Firstname;
            ViewBag.PageSize = 10;
            ViewBag.PageIndex = 1;
            ViewBag.TotalSize = ListGodchildren.Count();
            //var user = DataAccess.GetUser((int)ViewBag.LogonUserID);

            //ViewBag.SponsorEmailFrom = user.Email;
            //ViewBag.SponsorEmail = DataAccess.GetSponsorEmail();

            return View(vm);
        }



        [HttpPost]
        public ActionResult Sponsorship(AccountSponsorship vm)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    EmailArguments emailArgs = EmailArguments.GetInstanceFromConfig();
                    emailArgs.Subject = vm.SubjectEmail;
                    var list = vm.EmailTo.Split(',');
                    foreach (var email in list)
                    {
                        ViewBag.ReceiverEmail = email;
                        emailArgs.To.Add(email, email);
                    }

                    //var data = DataAccess.GetWebContentByKey("Sponsorship", 1, "");
                    //vm.SponsorContent = data.ContentText;
                    emailArgs.MailBody = GetHTMLFromView("~/Views/_Shared/UC/EmailTemplates/Sponsership.cshtml", vm);
                    var resultsendemail = EmailHelper.SendEmail(emailArgs);
                    if (resultsendemail)
                    {
                        foreach (var email in list)
                        {
                            var model = new SponsorEmail();
                            model.Body = emailArgs.MailBody;
                            model.Subject = vm.SubjectEmail;
                            model.FromUserId = Auth.User.UserId;
                            model.ToEmail = email;
                            DataAccess.AddSponsorEmail(model);
                        }
                        TempData["Error"] = "success";
                    }


                    return RedirectToAction("Sponsorship", "Account");
                }
                else
                {
                    TempData["Error"] = "error";
                    return View();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData["Error"] = "error";
                return View();
            }
        }


        [HttpPost]
        public ActionResult GetGodChildren(int? PageIndex, int? PageSize)
        {
            var ListGodchildren = new List<GodChildren>();
            var ListData = new List<GodChildren>();
            var IsResult = false;
            try
            {
                var AllSponsor = DataAccess.GetAllSponsorEmail(Auth.User.UserId);
                foreach (var _sponsor in AllSponsor)
                {
                    var model = new GodChildren();
                    model.SponsorEmail = _sponsor.ToEmail;
                    model.SponsorDate = _sponsor.InsertDate.ToString("MMMM d, yyyy");
                    ListGodchildren.Add(model);
                }

                ListData = GetGodChildrenByIndexSize(ListGodchildren, PageIndex, PageSize);
                IsResult = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return Json(new { IsResult = IsResult, ListData = ListData }, JsonRequestBehavior.AllowGet);
        }

        private List<GodChildren> GetGodChildrenByIndexSize(List<GodChildren> ListGodchildren, int? PageIndex, int? PageSize)
        {
            var ListData = new List<GodChildren>();
            try
            {

                var start = (PageIndex - 1) * PageSize;
                var end = PageIndex * PageSize;

                for (int i = (int)start; i < end; i++)
                {
                    if (i < ListGodchildren.Count())
                    {
                        ListData.Add(ListGodchildren[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return ListData;
        }
        #endregion

        #region "CouponsAndDiscounts

        public ActionResult CouponsAndDiscounts()
        {
            return View();
        }

        #endregion

        #region "Credit Card

        [HttpGet]
        public ActionResult CreditCard()
        {
            int userId = ViewBag.LogonUserID;
            DataSet ds = DataAccess.GetCustomerCreditCards(userId);

            List<CustomerCreditCard> customerCreditCard = new List<CustomerCreditCard>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var expireDate = DataManager.ToString(row["CardExpireX"]).Split('|');

                customerCreditCard.Add(new CustomerCreditCard()
                {
                    CreditCardId = DataManager.ToInt(row["CreditCardId"]),
                    UserId = DataManager.ToInt(row["UserId"]),
                    CardTypeId = DataManager.ToInt(row["CardTypeId"]),
                    CardHolderName = DataManager.ToString(row["CardHolderName"]),
                    CardNumberX = DataManager.ToString(row["CardNumberX"]),
                    CardExpireX = expireDate[0] + expireDate[1],
                });
                ViewBag.CardassociationsTypeList = GetCreditcardassociationsList(DataManager.ToInt(row["CardTypeId"]));
            }

            ViewBag.CustomerCreditCard = customerCreditCard;
            ViewBag.CreditCardContent = DataAccess.GetWebContentByKey("MyCreditCard", 1, "").ContentText;

            return View();
        }

        [HttpGet]
        public ActionResult FormCreditCard(int? creditCardId)
        {
            if (creditCardId == null || creditCardId == 0)
            {
                ViewBag.CardNumberX = string.Empty;
                ViewBag.CardName = string.Empty;
                ViewBag.CardExpireMonth = string.Empty;
                ViewBag.CardExpireYear = string.Empty;
                ViewBag.CreditCardId = 0;
                ViewBag.CardassociationsTypeList = GetCreditcardassociationsList(null);
            }
            else
            {
                CustomerCreditCard cusCreditCard = DataAccess.GetCustomerCreditCard(creditCardId ?? 0);
                var splitExpire = cusCreditCard.CardExpireX.Split('|');

                ViewBag.CardNumberX = cusCreditCard.CardNumberX;
                ViewBag.CardName = cusCreditCard.CardHolderName;
                ViewBag.CardExpireMonth = splitExpire[0];
                ViewBag.CardExpireYear = splitExpire[1];
                ViewBag.CreditCardId = cusCreditCard.CreditCardId;
                ViewBag.CardassociationsTypeList = GetCreditcardassociationsList(cusCreditCard.CardTypeId);
            }

            ViewBag.CreditCardContent = DataAccess.GetWebContentByKey("MyCreditCard", 1, "");


            return View();
        }

        [HttpPost]
        public ActionResult FormCreditCard(FormCollection Form)
        {
            ViewBag.CreditCardContent = DataAccess.GetWebContentByKey("MyCreditCard", 1, "");

            int userId = ViewBag.LogonUserID;
            CustomerCreditCard pObj = new CustomerCreditCard();

            pObj.CreditCardId = DataManager.ToInt(Form["creditCardId"]);
            pObj.Active = true;
            pObj.UserId = userId;
            pObj.CardTypeId = 1;
            pObj.CardHolderName = Form["cardname"].ToString();

            pObj.CardTypeId = Convert.ToInt32(Form["CardTypeId"]);

            string cardNumber = Form["cardnumber"].ToString();
            int indexOfPhoneNumber = cardNumber.Length - 3;
            var prefix_cardnumber = string.Join("", Enumerable.Range(0, indexOfPhoneNumber).Select(i => "*"));
            var maskedCardNumber = prefix_cardnumber + cardNumber.Substring(indexOfPhoneNumber);

            string cardExpire = Form["month"].ToString() + "|" + Form["year"].ToString();
            int indexOfCardExpire = cardExpire.Length - 3;
            var prefix_expire = string.Join("", Enumerable.Range(0, indexOfCardExpire).Select(i => "*"));
            var maskedCardExpire = prefix_expire + cardExpire.Substring(indexOfCardExpire);

            pObj.CardNumberE = DataProtection.Encrypt(cardNumber);
            pObj.CardNumberX = maskedCardNumber;

            pObj.CardExpireX = maskedCardExpire;

            ViewBag.CardassociationsTypeList = GetCreditcardassociationsList(pObj.CardTypeId);

            int result = 0;
            if (pObj.CreditCardId == 0)
            {
                result = DataAccess.AddCustomerCreditCard(pObj);
            }
            else
            {
                result = DataAccess.UpdateCustomerCreditCard(pObj);
            }
            if (result > 0) // success
            {
                return RedirectToAction("CreditCard");
            }
            else // Error
            {
                return RedirectToAction("CreditCard");
            }

        }

        public ActionResult DeleteCreditCard(int creditCardId)
        {
            int userId = ViewBag.LogonUserID;
            int result = DataAccess.DeleteCustomerCredit(creditCardId, userId);
            if (result > 1)
            {
                return RedirectToAction("CreditCard");
            }
            else
            {
                return RedirectToAction("CreditCard");
            }

        }

        private SelectList GetCreditcardassociationsList(int? valueseleted)
        {

            var result = DataAccess.GetCreditcardassociations();
            SelectList items = new SelectList(result, "CardassociationsId", "CardassociationsType", valueseleted);

            return items;
        }

        #endregion

        #region "Newsletters"

        [HttpGet]
        public ActionResult Newsletters()
        {
            bool result = DataAccess.GetUser(Auth.User.UserId).IsReceiveEmailInfo;
            var listinterest = DataAccess.GetInterestList(this.CultureId);
            var interestbyid = DataAccess.GetUserInterestedByUserId(Auth.User.UserId);


            ViewBag.resultcheckbox = result;
            ViewBag.listinterest = listinterest;
            ViewBag.interestbyid = interestbyid;

            return View();
        }

        [HttpPost]
        public ActionResult Newsletters(FormCollection Form)
        {
            bool intesting = Convert.ToBoolean(Form["newletter"]);

            if (!string.IsNullOrEmpty(Form["selectinterestlist"].ToString()))
            {
                var listinterest = Form["selectinterestlist"].ToString().Trim().Split(',').ToArray();

                if (listinterest.Count() > 0)
                {
                    DataAccess.DeleteUserInterestedByUserId(Auth.User.UserId);

                    foreach (var data in listinterest)
                    {
                        DataAccess.AddUserInterested(Convert.ToInt32(data), Auth.User.UserId);
                    }
                }
            }
            else
            {
                DataAccess.DeleteUserInterestedByUserId(Auth.User.UserId);
            }

            DataAccess.UpdateNewletter(intesting, Auth.User.UserId);

            return RedirectToAction("Newsletters");
        }

        #endregion

    }
}
