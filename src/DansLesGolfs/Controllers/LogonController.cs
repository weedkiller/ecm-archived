using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Controllers
{
    public class LogonController : BaseFrontController
    {
        public ActionResult Login()
        {
            User user = new BLL.User();
            user.Birthdate = DateTime.Today;

            if (Auth.User != null)
            {
                if(Request.QueryString["returnUrl"] != null)
                {
                    return Redirect(Request.QueryString["returnUrl"]);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            List<Title> titles = DataAccess.GetItemTitlesDropDownList(this.CultureId);
            ViewBag.DropDownTitles = ListToDropDownList<Title>(titles, "TitleId", "TitleName");
            if (ViewBag.TitleId == null && titles.Any())
            {
                ViewBag.TitleId = titles.First().TitleId;
            }

            List<Country> countries = DataAccess.GetAllCountries();
            ViewBag.Countries = ListToDropDownList<Country>(countries, "CountryId", "CountryName", 1);
            if (ViewBag.CountryId == null && countries.Any())
            {
                countries.Where(it => it.CountryName == "France").ToList().ForEach(it => ViewBag.CountryId = it.CountryId);
            }

            return View(user);
        }

        #region RegisterForm
        [HttpPost]
        public ActionResult RegisterForm(string Email, string Password, int? Civility, string Firstname, string Lastname, string Birthdate, string Address, string Complement, string PostalCode, string City, int? CountryId, string Phone, string Mobile, string Remarks, bool? IsReceiveNewsletters, bool? IsReceiveSpecialOffers)
        {
            try
            {
                User user = new User()
                {
                    Email = Email,
                    Password = Password,
                    PasswordEncrypted = DataProtection.Encrypt(Password),
                    FirstName = Firstname,
                    LastName = Lastname,
                    Birthdate = DataManager.ToDateTime(Birthdate),
                    Address = Address,
                    ShippingAddress = Address,
                    PostalCode = PostalCode,
                    City = City,
                    ShippingCity = City,
                    CountryId = CountryId.HasValue ? CountryId.Value : 0,
                    Phone = Phone,
                    ShippingPhone = Phone,
                    MobilePhone = Mobile,
                    Remarks = Remarks,
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
                return Json(new
                {
                    isResult = false,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region LoginForm
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
                return Json(new
                {
                    isResult = false,
                    message = ex.Message
                });
            }
        }
        #endregion

        //public ActionResult Login()
        //{
        //    if (Auth.Check())
        //    {
        //        return RedirectLoggedOnUser();
        //    }
        //    ViewBag.Username = string.Empty;
        //    ViewBag.Password = string.Empty;
        //    ViewBag.Remember = 1;
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Login(string username, string password, int remember = 0)
        //{
        //    if (Auth.Attempt(username, password, remember > 0))
        //    {
        //        return RedirectLoggedOnUser();
        //    }
        //    else
        //    {
        //        ViewBag.ErrorMessage = "Invalid email or password.";
        //        ViewBag.Username = username;
        //        ViewBag.Password = password;
        //        ViewBag.Remember = remember;
        //        return View();
        //    }
        //}

        [HttpPost]
        public JsonResult LoginViaFacebook(string id, string first_name, string last_name, string gender, string email)
        {
            try
            {
                User user = DataAccess.GetUserByFacebookId(id);
                if (user != null)
                {
                    Auth.SetSessionByUserObject(user);
                }
                else
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
                            throw new Exception(Resources.Resources.EmailCantBeEmpty);
                        }
                        string password = StringHelper.RandomString(8);

                        user = new User();
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
                        user.Birthdate = DateTime.Today;

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

                return Json(new
                {
                    isSuccess = true
                });

                //bool isRequireUsername = false;

                //User user = DataAccess.GetUserByFacebookId(id);
                //if (user != null)
                //{
                //    Auth.SetSessionByUserObject(user);
                //}
                //else
                //{
                //    isRequireUsername = true;
                //}

                //return Json(new
                //{
                //    isSuccess = true,
                //    isRequireUsername = isRequireUsername
                //});
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
        public ActionResult Logout()
        {
            Auth.Logout();
            return Redirect("~/");
        }

        public ActionResult ForgotPassword()
        {
            if (Request.Form.AllKeys.Contains("Submit"))
            {
                try
                {
                    string email = DataManager.ToString(Request.Form["Email"]);
                    ViewBag.Email = email;

                    User user = DataAccess.GetUserByEmail(email);
                    if (user == null)
                        throw new Exception(string.Format(Resources.Resources.EmailNotExists, email));

                    ViewBag.ReceiverEmail = user.Email;
                    EmailArguments emailArgs = EmailArguments.GetInstanceFromConfig();
                    emailArgs.Subject = Resources.Resources.RecoverPassword;
                    emailArgs.To.Add(user.Email, user.Fullname);
                    emailArgs.MailBody = GetHTMLFromView("~/Views/_Shared/UC/EmailTemplates/ForgotPassword.cshtml", user);
                    EmailHelper.SendEmailWithAttachments(emailArgs);

                    ViewBag.SuccessMessage = Resources.Resources.PasswordHasBeenSent;
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessages = ex.Message;
                }
            }
            return View();
        }

        public ActionResult Register()
        {
            if(Auth.Check())
            {
                return Redirect("~/Account");
            }
            else
            {
                RegisterModel model = new RegisterModel();
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    User user = new User();
                    user.UserTypeId = (int)UserType.Type.Customer;
                    user.TitleId = 0;
                    user.Email = model.Email;
                    user.Password = model.Password;
                    user.PasswordEncrypted = DataProtection.Encrypt(user.Password);
                    user.FirstName = model.FirstName;
                    user.MiddleName = string.Empty;
                    user.LastName = model.LastName;
                    user.Gender = 0;
                    //user.Birthdate = null;
                    //user.Address = string.Empty;
                    //user.Street = DataManager.ToString(Request.Form["Street"]);
                    //user.City = DataManager.ToString(Request.Form["City"]);
                    //user.PostalCode = DataManager.ToString(Request.Form["PostalCode"]);
                    //user.Phone = DataManager.ToString(Request.Form["Phone"]);
                    //user.MobilePhone = DataManager.ToString(Request.Form["MobilePhone"]);
                    //user.CountryId = DataManager.ToInt(Request.Form["CountryId"]);
                    //user.ShippingAddress = DataManager.ToString(Request.Form["ShippingAddress"]);
                    //user.ShippingStreet = DataManager.ToString(Request.Form["ShippingStreet"]);
                    //user.ShippingCity = DataManager.ToString(Request.Form["ShippingCity"]);
                    //user.ShippingPostalCode = DataManager.ToString(Request.Form["ShippingPostalCode"]);
                    //user.ShippingPhone = DataManager.ToString(Request.Form["ShippingPhone"]);
                    //user.ShippingCountryId = DataManager.ToInt(Request.Form["ShippingCountryId"]);
                    //user.Remarks = DataManager.ToString(Request.Form["Remarks"]);
                    //user.FBAccount = DataManager.ToString(Request.Form["FBAccount"]);
                    user.IsReceiveEmailInfo = model.IsReceiveEmailInfo;

                    //if (Auth.User != null)
                    //    user.ModifyUserId = Auth.User.UserId;

                    user.UpdateDate = DateTime.Now;
                    user.UserId = DataAccess.AddUser(user);
                    if (user.UserId > 0)
                    {
                        SendNewRegisteredUserEmail(user);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.ErrorMessages = Resources.Resources.RegisterFailed;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessages = ex.Message;
            }
            return View(model);
        }

        private ActionResult RedirectLoggedOnUser()
        {
            if (String.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
            {
                if (Auth.User.UserTypeId == UserTypes.Type.Admin || Auth.User.UserTypeId == UserTypes.Type.SuperAdmin)
                {
                    return Redirect("~/Admin");
                }
                else if (Auth.User.UserTypeId == UserTypes.Type.Reseller)
                {
                    return Redirect("~/Reseller");
                }
                else
                {
                    return Redirect("~/");
                }
            }
            else
            {
                return Redirect(Request.QueryString["ReturnUrl"]);
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
            emailArgs.BCC.Add(siteEmail, siteUrl);
            emailArgs.MailBody = GetHTMLFromView("~/Views/_Shared/UC/EmailTemplates/Register.cshtml", user);
            EmailHelper.SendEmailWithAttachments(emailArgs);
        }

    }
}
