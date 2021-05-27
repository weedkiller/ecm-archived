using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Limilabs.Client.SMTP;
using Limilabs.Mail;
using Limilabs.Mail.Fluent;
using Limilabs.Mail.Headers;
using Limilabs.Mail.Tools;
using Limilabs.FTP.Client;
using System.Xml.Linq;
using System.IO;
using System.IO.Compression;
using System.Security.AccessControl;
using System.Net;
using System.Security;
using System.Threading;

namespace DansLesGolfs.ECM.Controllers
{
    public class EmailingController : BaseECMCRUDController<EmailingViewModel>
    {
        #region Members
        private string maleText = Resources.Resources.Male;
        private string femaleText = Resources.Resources.Female;
        #endregion

        #region Constructor
        public EmailingController()
        {
            ObjectName = "Emailing";
            TitleName = Resources.Resources.SendingEmail;
            PrimaryKey = "EmailId";
        }
        #endregion

        #region Overriden Methods
        protected override void DoSetGridColumns(GridColumnFactory<EmailingViewModel> columns)
        {
            columns.Bound(it => it.EmailName).Title(Resources.Resources.EmailName);
            columns.Bound(it => it.Subject).Title(Resources.Resources.Subject);
            columns.Bound(it => it.InsertDate).Title(Resources.Resources.Created).Format("{0:dd/MM/yyyy}");
            columns.Bound(it => it.EmailId).Title(Resources.Resources.Report)
                .ClientTemplate("<a href=\"" + Url.Content("~/Emailing/SendingMailReport/") + "#:EmailId#\">" + Resources.Resources.Report + "</a>")
                .Sortable(false)
                .Filterable(false)
                .Groupable(false)
                .Width(100);
            columns.Bound(it => it.EmailId).Title(Resources.Resources.Cancel)
                .ClientTemplate("<a href=\"" + Url.Content("~/Emailing/CancelEmail/") + "#:EmailId#\">" + Resources.Resources.Cancel + "</a>")
                .Sortable(false)
                .Filterable(false)
                .Groupable(false)
                .Width(100);
        }

        protected override void DoSetDataSorting(DataSourceSortDescriptorFactory<EmailingViewModel> sorting)
        {
            sorting.Add(it => it.InsertDate).Order(System.ComponentModel.ListSortDirection.Descending);
        }

        protected override IQueryable<EmailingViewModel> DoLoadDataJSON()
        {
            if (Auth.User.UserTypeId == UserType.Type.SiteManager || Auth.User.UserTypeId == UserType.Type.Staff)
            {
                return DataAccess.GetListEmailings(Auth.User.SiteId);
            }
            else
            {
                return DataAccess.GetListEmailings();
            }
        }

        protected override object DoPrepareNew()
        {
            Session["Emailing"] = null;
            Response.Redirect("~/Emailing/Step1");
            string defaultSenderName = string.Empty;
            string defaultSenderEmail = string.Empty;
            if (Auth.User.UserTypeId == UserType.Type.SiteManager || Auth.User.UserTypeId == UserType.Type.Staff)
            {
                DataAccess.GetSiteDefaultSender(Auth.User.SiteId, out defaultSenderName, out defaultSenderEmail);
            }
            else
            {
                defaultSenderName = System.Configuration.ConfigurationManager.AppSettings["DefaultSenderName"];
                defaultSenderEmail = System.Configuration.ConfigurationManager.AppSettings["DefaultSenderEmail"];
            }
            return new Emailing()
            {
                FromName = defaultSenderName,
                FromEmail = defaultSenderEmail
            };
        }

        protected override object DoPrepareEdit(long id)
        {
            Emailing emailing = DataAccess.GetEmailing(id);
            Session["Emailing"] = emailing;
            Response.Redirect("~/Emailing/Step1");
            return emailing;
        }

        protected override bool DoSave()
        {
            int result = -1;
            Category model = new Category();
            model.CategoryId = DataManager.ToInt(Request.Form["id"], -1);
            model.CategoryName = DataManager.ToString(Request.Form["CategoryName"]);
            model.ContentType = 0;

            Category obj = DataAccess.GetCategory(model.CategoryId);
            if (obj != null)
            {
                result = DataAccess.UpdateCategory(model);
            }
            else
            {
                model.Active = true;
                result = DataAccess.AddCategory(model);
                model.CategoryId = result;
            }
            ViewBag.id = result > -1 ? model.CategoryId : -1;
            return result > 0;
        }

        protected override bool DoDelete(long id)
        {
            return DataAccess.DeleteEmailing(id) > 0;
        }

        protected override string RowCellDisplayText(string columnName, object value, object dataItem)
        {
            if (columnName != "Report")
                return base.RowCellDisplayText(columnName, value, dataItem);

            Emailing emailing = (Emailing)dataItem;
            return "<a target=\"_blank\" href=\"" + Url.Action("SendingMailReport", new { id = emailing.EmailId }) + "\">" + Resources.Resources.Report + "</a>";
        }
        #endregion

        #region Action Methods

        public ActionResult Step1()
        {
            // Define Breadcrumbs.
            Breadcrumbs.Add(Resources.Resources.SendEmail, "~/Emailing");
            Breadcrumbs.Add(Resources.Resources.EmailInformation, "~/Emailing/Step1");
            Init();
            Emailing model = Preparing();
            InitSiteDropDownList();
            return View(model);
        }

        public ActionResult Step2()
        {
            // Define Breadcrumbs.
            Breadcrumbs.Add(Resources.Resources.SendEmail, "~/Emailing");
            Breadcrumbs.Add(Resources.Resources.EmailInformation, "~/Emailing/Step1");
            Breadcrumbs.Add(Resources.Resources.EmailBody, "~/Emailing/Step2");
            Init();
            InitEmailTemplateDropDownList();
            ViewBag.EmailTemplateVariables = DataAccess.GetAllEmailTemplateVariables();
            Emailing model = Preparing();
            ViewBag.SelectTemplateId = model.TemplateId;
            ViewBag.Attachments = DataAccess.GetEmailingAttachmentsByEmailId(model.EmailId);
            ViewBag.EditorLimitAccess = Auth.User.UserTypeId != UserType.Type.SuperAdmin && Auth.User.UserTypeId != UserType.Type.Admin;

            // Validate step1.
            if (String.IsNullOrEmpty(model.EmailName) || String.IsNullOrEmpty(model.Subject) || String.IsNullOrEmpty(model.FromName) || String.IsNullOrEmpty(model.FromEmail))
            {
                return RedirectToAction("Step1");
            }
            return View(model);
        }

        public ActionResult Step3()
        {
            // Define Breadcrumbs.
            Breadcrumbs.Add(Resources.Resources.SendEmail, "~/Emailing");
            Breadcrumbs.Add(Resources.Resources.EmailInformation, "~/Emailing/Step1");
            Breadcrumbs.Add(Resources.Resources.EmailBody, "~/Emailing/Step2");
            Breadcrumbs.Add(Resources.Resources.SendingEmail, "~/Emailing/Step3");
            Init();
            InitCustomerGroupDropDownList();
            Emailing model = Preparing();

            List<SelectListItem> sendMailUsingOptions = new List<SelectListItem>();
            sendMailUsingOptions.Add(new SelectListItem()
            {
                Text = "SMTP Server",
                Value = ((int)SendMailUsing.SMTP).ToString()
            });
            sendMailUsingOptions.Add(new SelectListItem()
            {
                Text = "Netmessage",
                Value = ((int)SendMailUsing.Netmessage).ToString()
            });
            ViewBag.IsSendToAllCustomer = ViewBag.IsSendToAllCustomer;
            ViewBag.SendMailUsingOptions = sendMailUsingOptions;

            // Validate step1.
            if (String.IsNullOrEmpty(model.EmailName) || String.IsNullOrEmpty(model.Subject) || String.IsNullOrEmpty(model.FromName) || String.IsNullOrEmpty(model.FromEmail))
            {
                return RedirectToAction("Step1");
            }

            return View(model);
        }

        public ActionResult CancelEmail(long? id = 0)
        {
            try
            {
                id = id.HasValue ? id.Value : 0;
                DataAccess.DeleteEmailQue(id.Value);

                TempData["SuccessMessage"] = Resources.Resources.YourCampaignHasBeenCancelled;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData["ErrorMessages"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        public ActionResult SendingMailReport(int? id = 0)
        {
            if (!id.HasValue)
                return RedirectToAction("Index");

            Breadcrumbs.Add(Resources.Resources.EmailCampaigns, "~/Emailing");
            Breadcrumbs.Add(Resources.Resources.EmailReport, "~/Emailing/SendingMailReport/" + id.Value);
            TitleName = Resources.Resources.EmailReport;
            Init();

            Emailing email = DataAccess.GetEmailing(id.Value, this.CultureId);

            #region Set Default Data
            ViewBag.id = id;
            ViewBag.EmailCampaignName = email.EmailName;
            ViewBag.TotalOpens = 0;
            ViewBag.TotalClicks = 0;
            ViewBag.TotalUnsubscribers = 0;
            ViewBag.OpenPercent = 0;
            ViewBag.ClickPercent = 0;
            ViewBag.UnsubscribePercent = 0;
            ViewBag.BounceRate = 0;
            #endregion

            #region Get Summary Data
            DataSet ds = null;
            string viewName = String.Empty;
            string cacheKey = "EmailingSummaryReport_" + id.Value;

            if (DataAccess.HasNetmessageCampaigns(id.Value))
            {
                if (MemoryCache.Default.Contains(cacheKey))
                {
                    ds = (DataSet)MemoryCache.Default.Get(cacheKey);
                }
                else
                {
                    ds = DataAccess.GetNetmessageSummaryReport(id.Value);
                    MemoryCache.Default.Add(cacheKey, ds, DateTime.Now.AddMinutes(5));
                }
                viewName = "NetmessageReport";
            }
            else
            {
                if (MemoryCache.Default.Contains(cacheKey))
                {
                    ds = (DataSet)MemoryCache.Default.Get(cacheKey);
                }
                else
                {
                    ds = DataAccess.GetEmailingStatus(id.Value);
                    MemoryCache.Default.Add(cacheKey, ds, DateTime.Now.AddMinutes(5));
                }
                viewName = "SendingMailReport";
            }
            #endregion

            #region Prepare Additional Data
            if (ds.Tables.Count == 5)
            {
                long totalSentEmails = DataManager.ToLong(ds.Tables[0].Rows[0]["TotalSentEmails"]);
                long totalOpens = DataManager.ToLong(ds.Tables[1].Rows[0]["TotalOpens"]);
                long totalClicks = DataManager.ToLong(ds.Tables[2].Rows[0]["TotalClicks"]);
                long totalUnsubscribers = DataManager.ToLong(ds.Tables[3].Rows[0]["TotalUnsubscribers"]);
                long totalBounces = DataManager.ToLong(ds.Tables[4].Rows[0]["TotalBounces"]);

                if (totalSentEmails > 0)
                {
                    ViewBag.TotalSentEmails = totalSentEmails;
                    ViewBag.TotalOpens = totalOpens;
                    ViewBag.OpenPercent = (decimal)totalOpens * 100 / totalSentEmails;
                    ViewBag.TotalClicks = totalClicks;
                    ViewBag.ClickPercent = (decimal)totalClicks * 100 / totalSentEmails;
                    ViewBag.TotalUnsubscribers = totalUnsubscribers;
                    ViewBag.UnsubscribePercent = (decimal)totalUnsubscribers * 100 / totalSentEmails;
                    ViewBag.TotalBounces = totalBounces;
                    ViewBag.BouncePercent = (decimal)totalBounces * 100 / totalSentEmails;
                }
                else
                {
                    ViewBag.TotalSentEmails = 0;
                    ViewBag.TotalOpens = 0;
                    ViewBag.OpenPercent = 0;
                    ViewBag.TotalClicks = 0;
                    ViewBag.ClickPercent = 0;
                    ViewBag.TotalUnsubscribers = 0;
                    ViewBag.UnsubscribePercent = 0;
                    ViewBag.TotalBounces = 0;
                    ViewBag.BouncePercent = 0;
                }
            }
            else
            {
                ViewBag.TotalSentEmails = 0;
                ViewBag.EmailingStatus = null;
                ViewBag.TotalOpens = 0;
                ViewBag.OpenPercent = 0;
                ViewBag.TotalClicks = 0;
                ViewBag.ClickPercent = 0;
                ViewBag.TotalUnsubscribers = 0;
                ViewBag.UnsubscribePercent = 0;
                ViewBag.TotalBounces = 0;
                ViewBag.BouncePercent = 0;
            }
            #endregion

            return View(viewName, email);
        }

        public ActionResult AjaxGetSendingEmailList([DataSourceRequest]DataSourceRequest request, long? id)
        {
            if (!id.HasValue)
                id = 0;

            IQueryable data = null;
            string cacheKey = "EmailSendingRecords_" + id.Value;
            if (MemoryCache.Default.Contains(cacheKey))
            {
                data = (IQueryable)MemoryCache.Default.Get(cacheKey);
            }
            else
            {
                data = DataAccess.GetEmailQueReportByEmailId(id.Value);
                MemoryCache.Default.Add(cacheKey, data, DateTime.Now.AddMinutes(5));
            }

            return Json(data.ToDataSourceResult(request));
        }

        public ActionResult AjaxGetNetmessageReportRecordList([DataSourceRequest]DataSourceRequest request, long? id)
        {
            if (!id.HasValue)
                id = 0;

            IQueryable data = null;
            string cacheKey = "EmailSendingRecords_" + id.Value;
            if (MemoryCache.Default.Contains(cacheKey))
            {
                data = (IQueryable)MemoryCache.Default.Get(cacheKey);
            }
            else
            {
                data = DataAccess.GetNetmessageReportRecordsByEmailId(id.Value);
                MemoryCache.Default.Add(cacheKey, data, DateTime.Now.AddMinutes(5));
            }

            return Json(data.ToDataSourceResult(request));
        }
        #endregion

        #region Action Methods (POST)
        [HttpPost]
        public ActionResult SaveStep1()
        {
            Emailing model = Preparing();
            model.EmailName = DataManager.ToString(Request.Form["EmailName"]);
            model.Subject = DataManager.ToString(Request.Form["Subject"]);
            model.FromName = DataManager.ToString(Request.Form["FromName"]);
            model.FromEmail = DataManager.ToString(Request.Form["FromEmail"]);
            model.EmailFormatId = DataManager.ToInt(Request.Form["EmailFormatId"]);
            string scheduleDate = DataManager.ToString(Request.Form["ScheduleDate"]);
            string scheduleTime = DataManager.ToString(Request.Form["ScheduleTime"]);
            if (!String.IsNullOrEmpty(scheduleDate))
            {
                if (String.IsNullOrEmpty(scheduleTime))
                {
                    model.ScheduleDateTime = DataManager.ToDateTime(scheduleDate, "dd/MM/yyyy");
                }
                else
                {
                    model.ScheduleDateTime = DataManager.ToDateTime(scheduleDate + " " + scheduleTime, "dd/MM/yyyy hh:mm");
                }
            }
            else
            {
                model.ScheduleDateTime = null;
            }

            //model.TemplateId = 0;

            if (Auth.User.UserTypeId == UserType.Type.SuperAdmin || Auth.User.UserTypeId == UserType.Type.Admin)
            {
                model.SiteId = DataManager.ToLong(Request.Form["SiteId"]);
            }
            else
            {
                model.SiteId = Auth.User.SiteId;
            }

            SaveEmailing(model);
            return RedirectToAction("Step2");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveStep2()
        {
            Emailing model = Preparing();
            model.MjmlDetailString = DataManager.ToString(Request.Form["MjmlDetailString"]);
            model.HtmlDetailString = DataManager.ToString(Request.Form["HtmlDetailString"]);
            model.TemplateId = DataManager.ToInt(Request.Form["TemplateId"]);
            //model.TextDetailString = DataManager.ToString(Request.Form["TextDetailString"]);

            SaveEmailing(model);

            #region Emailing Attachments
            string[] attachmentIds = Request.Form.GetValues("AttachmentIds");
            string[] fileNames = Request.Form.GetValues("FileNames");
            string[] baseNames = Request.Form.GetValues("BaseNames");
            string[] fileExtensions = Request.Form.GetValues("FileExtensions");
            string[] filePaths = Request.Form.GetValues("FilePaths");
            string[] isCopies = Request.Form.GetValues("IsCopy");
            string[] deletedAttachmentIds = Request.Form.AllKeys.Contains("deletedAttachmentIds") ? Request.Form["deletedAttachmentIds"].Split(',') : new String[0];

            EmailingAttachment attachment = null;
            string uploadPath = Server.MapPath("~/Uploads/Emailing/" + model.EmailId);
            if (!Directory.Exists(uploadPath) && attachmentIds != null)
            {
                Directory.CreateDirectory(uploadPath);
            }
            string destPath = string.Empty;
            string destFileName = string.Empty;
            int fileIndex = 1;
            if (attachmentIds != null && attachmentIds.Any())
            {
                for (int i = 0, n = attachmentIds.Length; i < n; i++)
                {
                    if (!System.IO.File.Exists(filePaths[i]))
                        continue;

                    fileIndex = 0;
                    destPath = Path.Combine(uploadPath, fileNames[i]);
                    while (System.IO.File.Exists(destPath))
                    {
                        baseNames[i] = baseNames[i] + " (" + (fileIndex++) + ")";
                        fileNames[i] = baseNames[i] + fileExtensions[i];
                        destPath = Path.Combine(uploadPath, fileNames[i]);
                    }
                    if (DataManager.ToBoolean(isCopies[i]))
                    {
                        System.IO.File.Copy(filePaths[i], destPath);
                    }
                    else
                    {
                        System.IO.File.Move(filePaths[i], destPath);
                    }

                    attachment = new EmailingAttachment();
                    attachment.EmailId = model.EmailId;
                    attachment.FileName = fileNames[i];
                    attachment.BaseName = baseNames[i];
                    attachment.FileExtension = fileExtensions[i];
                    attachment.FilePath = destPath;
                    DataAccess.AddEmailingAttachment(attachment);
                }
            }

            long attachmentId = 0;
            if (deletedAttachmentIds != null && deletedAttachmentIds.Any())
            {
                foreach (var it in deletedAttachmentIds)
                {
                    if (String.IsNullOrWhiteSpace(it))
                        continue;

                    attachmentId = DataManager.ToLong(it);
                    attachment = DataAccess.GetEmailingAttachment(attachmentId);
                    if (attachment == null)
                        continue;

                    if (System.IO.File.Exists(attachment.FilePath))
                    {
                        @System.IO.File.Decrypt(attachment.FilePath);
                    }
                    DataAccess.DeleteEmailingAttachment(attachmentId);
                }
            }
            #endregion

            if (Request.Form.AllKeys.Contains("Previous"))
            {
                return RedirectToAction("Step1");
            }
            else
            {
                return RedirectToAction("Step3");
            }
        }

        [HttpPost]
        public ActionResult SaveStep3()
        {
            if (Request.Form.AllKeys.Contains("Previous"))
            {
                return RedirectToAction("Step2");
            }
            else
            {
                Emailing model = Preparing();

                // General Email Settings.
                model.SendMailUsing = DataManager.ToInt(Request.Form["SendMailUsing"]);

                model.IsSendToAllCustomer = DataManager.ToBoolean(Request.Form["SendToAllCustomer"]);
                if (Request.Form.AllKeys.Contains("CustomerGroupIds") && !String.IsNullOrEmpty(Request.Form["CustomerGroupIds"]))
                {
                    model.CustomerGroupIds = DataManager.ToString(Request.Form["CustomerGroupIds"]);
                }

                if (Request.Form.AllKeys.Contains("OtherEmails"))
                {
                    model.OtherEmails = new List<string>();
                    var otherEmails = DataManager.ToString(Request.Form["OtherEmails"]).Trim().Split(';').Select(it => it.Trim());
                    foreach (string email in otherEmails)
                    {
                        if (string.IsNullOrEmpty((email)))
                            continue;

                        model.OtherEmails.Add(email);
                    }
                }

                SaveEmailing(model);
                TempData["SuccessMessage"] = Resources.Resources.EmailIsInSchedule;
                TempData["IsSave"] = true;
                if (Request.Form.AllKeys.Contains("Schedule"))
                {
                    TempData["IsSchedule"] = true;
                }

                return RedirectToAction("Step3");
            }
        }

        public ActionResult AjaxAddEmailIntoQueue()
        {
            try
            {
                Emailing model = Preparing();
                bool isSchedule = TempData.ContainsKey("IsSchedule") && model.ScheduleDateTime.HasValue;
                DateTime now = DateTime.Now;
                DateTime sendDate = isSchedule ? model.ScheduleDateTime.Value : now.AddMinutes(5);

                // Determine the method for sending 
                SendMailUsing sendMailUsing = model.SendMailUsing.HasValue ? (SendMailUsing)model.SendMailUsing : SendMailUsing.SMTP;
                List<EmailingAttachment> attachments = DataAccess.GetEmailingAttachmentsByEmailId(model.EmailId);

                List<User> users = null;
                Site site = DataAccess.GetSite(model.SiteId, this.CultureId);

                if (site == null)
                    throw new Exception("Golf site not found.");

                // Prepare Email content.
                string htmlBody = GetReadyToSendHTML(model.HtmlDetailString);

                if (model.IsSendToAllCustomer)
                {
                    users = DataAccess.GetAllECMSubscribers(site.SiteId).Distinct().ToList();
                }
                else
                {
                    if (!String.IsNullOrEmpty(model.CustomerGroupIds))
                    {
                        users = DataAccess.GetAllCustomerInCustomerGroups(model.CustomerGroupIds, 1);
                    }
                }

                if (users == null || !users.Any())
                    throw new Exception("Can't send with empty email list.");

                if (sendMailUsing == SendMailUsing.SMTP)
                {
                    #region SMTP

                    EmailQue emailQue = null;
                    string unsubscribeMailTo = string.Empty;
                    string unsubscribeUrl = string.Empty;

                    string unsubscribeEmail = DataAccess.GetOption("UnsubscribeEmail");

                    if (users != null)
                    {
                        foreach (User u in users)
                        {
                            // Reset data.
                            unsubscribeMailTo = unsubscribeEmail;
                            unsubscribeUrl = string.Empty;

                            emailQue = CreateEmailQueObject(model.EmailId, now);
                            emailQue.CustomerId = u.UserId;
                            emailQue.Email = u.Email;
                            emailQue.HtmlDetailString = PersonalizeText(htmlBody, null, u, model.SiteId);
                            emailQue.Status = -2;
                            emailQue.ScheduleDateTime = isSchedule ? model.ScheduleDateTime : null;
                            emailQue.EmailQueId = DataAccess.AddEmailQue(emailQue);

                            emailQue.Status = 0;
                            emailQue.HtmlDetailString = InsertTrackingLinks(emailQue.HtmlDetailString, model.EmailId, emailQue.EmailQueId, model.SiteId, u.UserId, u.Email, ref unsubscribeMailTo, ref unsubscribeUrl);
                            emailQue.UnsubscribeMailTo = unsubscribeMailTo;
                            emailQue.UnsubscribeUrl = unsubscribeUrl;

                            DataAccess.UpdateEmailQueHtml(emailQue);
                        }

                    }
                    User tempUser = new User();
                    if (model.OtherEmails != null && model.OtherEmails.Any())
                    {
                        foreach (string email in model.OtherEmails)
                        {
                            if (string.IsNullOrEmpty((email)))
                                continue;

                            // Reset data.
                            unsubscribeMailTo = unsubscribeEmail;
                            unsubscribeUrl = string.Empty;

                            tempUser.Email = email;
                            emailQue = CreateEmailQueObject(model.EmailId, now);
                            emailQue.CustomerId = 0;
                            emailQue.Email = email;
                            emailQue.HtmlDetailString = PersonalizeText(htmlBody, null, tempUser, model.SiteId);
                            //emailQue.TextDetailString = PersonalizeText(model.TextDetailString, null, tempUser, model.SiteId);
                            emailQue.Status = -2;
                            emailQue.ScheduleDateTime = isSchedule ? model.ScheduleDateTime : null;
                            emailQue.EmailQueId = DataAccess.AddEmailQue(emailQue);

                            emailQue.Status = 0;
                            emailQue.HtmlDetailString = InsertTrackingLinks(emailQue.HtmlDetailString, model.EmailId, emailQue.EmailQueId, model.SiteId, 0, email, ref unsubscribeMailTo, ref unsubscribeUrl);
                            emailQue.UnsubscribeMailTo = unsubscribeMailTo;
                            emailQue.UnsubscribeUrl = unsubscribeUrl;

                            DataAccess.UpdateEmailQueHtml(emailQue);
                        }
                    }

                    model.StatusId = 1;
                    SaveEmailing(model);

                    return Json(new
                    {
                        isSuccess = true
                    });
                    #endregion
                }
                else
                {
                    #region Netmessage

                    #region Get Credential
                    string server = "", username = "", password = "", accountName;
                    server = System.Configuration.ConfigurationManager.AppSettings["NetmessageFTPServer"];
                    //int port = 21;

                    var systemEmail = DataAccess.GetOption("SMTPUsername");
                    if (site.IsUseGlobalNetmessageSettings.HasValue && site.IsUseGlobalNetmessageSettings.Value)
                    {
                        var options = DataAccess.GetOptions("NetmessageFTPServer", "NetmessageFTPPort", "NetmessageFTPUsername", "NetmessageFTPPassword", "NetmessageAccountName");
                        //server = options["NetmessageFTPServer"];
                        //port = DataManager.ToInt(options["NetmessageFTPPort"], 21);
                        username = options["NetmessageFTPUsername"];
                        password = options["NetmessageFTPPassword"];
                        accountName = options["NetmessageAccountName"];
                    }
                    else
                    {
                        //server = site.NetmessageFTPServer;
                        //port = site.NetmessageFTPPort.HasValue ? site.NetmessageFTPPort.Value : 21;
                        server = "ftp.netmessage.com";
                        //port = 21;
                        username = site.NetmessageFTPUsername;
                        password = site.NetmessageFTPPassword;
                        accountName = site.NetmessageAccountName;
                    }

                    if (String.IsNullOrWhiteSpace(username))
                        throw new Exception("Netmessage FTP Username should not be empty.");

                    if (String.IsNullOrWhiteSpace(password))
                        throw new Exception("Netmessage FTP Password should not be empty.");
                    #endregion

                    #region Create XNM File
                    string refName = "ECMC-" + model.EmailId + "-" + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                    string xnmContent = string.Empty;
                    xnmContent += GetXNMNode("account", accountName + "_EMAIL");
                    xnmContent += GetXNMNode("from", model.FromEmail);
                    xnmContent += GetXNMNode("media", "EMAIL");
                    xnmContent += GetXNMNode("bc", refName);
                    xnmContent += GetXNMNode("ref", refName);
                    xnmContent += GetXNMNode("name", GetSecureAttributeValue(model.EmailName));
                    xnmContent += GetXNMNode("object", GetSecureAttributeValue(model.Subject));
                    xnmContent += GetXNMNode("email_open", 1);
                    xnmContent += GetXNMNode("email_image", 1);
                    xnmContent += GetXNMNode("email_view", 1);
                    xnmContent += GetXNMNode("email_unsub", 1);
                    xnmContent += GetXNMNode("email_url", 1);
                    xnmContent += GetXNMNode("datetime", sendDate.ToString("yyyy-MM-dd hh:mm:dd"));
                    xnmContent += GetXNMNode("doubloon", "NO");
                    xnmContent += GetXNMNode("reply_to", model.FromEmail);
                    xnmContent += GetXNMNode("error_to", String.IsNullOrWhiteSpace(systemEmail) ? model.FromEmail : systemEmail);

                    string uploadPath = Server.MapPath("~/Uploads/Netmessage");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    string reportPath = Path.Combine(uploadPath, "Reports");
                    string ackFileName = refName + ".ack";
                    string ackFilePath = Path.Combine(reportPath, ackFileName);

                    if (!Directory.Exists(reportPath))
                    {
                        Directory.CreateDirectory(reportPath);
                    }

                    string destPath = Path.Combine(uploadPath, refName);
                    if (!Directory.Exists(destPath))
                    {
                        Directory.CreateDirectory(destPath);
                    }
                    string fileshostPath = Path.Combine(destPath, "fileshost");
                    if (!Directory.Exists(fileshostPath))
                    {
                        Directory.CreateDirectory(fileshostPath);
                    }
                    string databasesPath = Path.Combine(destPath, "databases");
                    if (!Directory.Exists(databasesPath))
                    {
                        Directory.CreateDirectory(databasesPath);
                    }
                    string xnmFileName = refName + ".xnm";
                    string xnmFilePath = Path.Combine(uploadPath, xnmFileName);
                    System.IO.File.WriteAllText(xnmFilePath, xnmContent);
                    #endregion

                    #region Create HTML File
                    string html = htmlBody;
                    html = ExtractImagesFromHTML(html, fileshostPath);
                    System.IO.File.WriteAllText(Path.Combine(destPath, refName + ".html"), PersonalizeTextNM(html));
                    #endregion

                    #region Create CSV File
                    string csvContent = string.Empty;
                    csvContent += GetNetmessageCSVHeaders();
                    var uniqueUsers = users.Distinct().ToList();

                    if (model.OtherEmails != null && model.OtherEmails.Any())
                    {
                        foreach (var email in model.OtherEmails)
                        {
                            uniqueUsers.Add(new BLL.User
                            {
                                Email = email
                            });
                        }
                    }

                    List<string> csvContents = new List<string>();
                    int csvFilesCount = (uniqueUsers.Count / 200) + 1;

                    for (int i = 0; i < csvFilesCount; i++)
                    {
                        csvContent = GetNMCSVContent(i, uniqueUsers);
                        System.IO.File.WriteAllText(Path.Combine(databasesPath, refName + "_" + i + ".csv"), csvContent);
                    }
                    #endregion

                    #region Attachment Files
                    if (attachments != null && attachments.Any())
                    {
                        string attachmentPath = Path.Combine(destPath, "attached");
                        Directory.CreateDirectory(attachmentPath);
                        foreach (EmailingAttachment att in attachments)
                        {
                            if (!System.IO.File.Exists(att.FilePath))
                                continue;

                            try
                            {
                                @System.IO.File.Copy(att.FilePath, Path.Combine(attachmentPath, att.FileName));
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                            }
                        }
                    }
                    #endregion

                    #region Zipping File
                    string zipFileName = refName + ".zip";
                    string zipFilePath = Path.Combine(uploadPath, zipFileName);
                    if (System.IO.File.Exists(zipFilePath))
                    {
                        @System.IO.File.Delete(zipFilePath);
                    }
                    ZipFile.CreateFromDirectory(destPath, zipFilePath);
                    #endregion

                    #region Upload File via FTP
                    try
                    {
                        using (WebClient client = new WebClient())
                        {
                            string ftpUploadPath = "ftp://" + server + "/IN/";
                            client.Credentials = new NetworkCredential(username, password);
                            client.UploadFile(ftpUploadPath + zipFileName, "STOR", zipFilePath);
                            client.UploadFile(ftpUploadPath + xnmFileName, "STOR", xnmFilePath);
                        }

                        NetMessageCampaign nmCampaign = new NetMessageCampaign();
                        nmCampaign.CreatedDate = nmCampaign.UpdatedDate = DateTime.Now;
                        nmCampaign.EmailId = model.EmailId;
                        nmCampaign.Status = 0;
                        nmCampaign.RefCode = refName;
                        nmCampaign.NetMessageCampaignId = DataAccess.AddNetmessageCampaign(nmCampaign);

                        // Checking Acknowledge File.
                        bool foundAckFile = false;
                        RemoteSearchOptions ackSearchOption = new RemoteSearchOptions(ackFileName, false);
                        for (int i = 0; i < 5; i++)
                        {
                            Thread.Sleep(10); // Waiting for 10 seconds.
                            using (Ftp ftp = new Ftp())
                            {
                                ftp.Connect(server);
                                ftp.Login(username, password);
                                ftp.DownloadFiles("OUT", reportPath, ackSearchOption);
                                ftp.Close();
                            }
                            if (System.IO.File.Exists(ackFilePath))
                            {
                                foundAckFile = true;
                                break;
                            }
                        }

                        if (foundAckFile)
                        {
                            string ackContent = System.IO.File.ReadAllText(ackFilePath);
                            Regex regex = new Regex("\\<job_number\\>(.+)\\<\\/job_number\\>", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                            Match match = regex.Match(ackContent);
                            string jobNumber = string.Empty;
                            if (match.Success && match.Groups.Count == 2)
                            {
                                jobNumber = match.Groups[1].Value.ToLower();
                                DataAccess.UpdateNetmessageCampaignJobNumber(nmCampaign.NetMessageCampaignId, jobNumber);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        @System.IO.File.Delete(xnmFilePath);
                        @System.IO.File.Delete(zipFilePath);
                        @Directory.Delete(destPath, true);
                    }
                    #endregion

                    return Json(new
                    {
                        isSuccess = true
                    });
                    #endregion
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new
                {
                    isSuccess = false
                });
            }
        }

        private string[] FTPDownloadFiles(string path, IEnumerable<string> files, string username, string password, string baseSavePath)
        {
            List<string> destinationFiles = new List<string>();
            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(username, password);
                foreach (string file in files)
                {
                    string savePath = Path.Combine(baseSavePath, file);
                    client.DownloadFile(path + file, savePath);
                    destinationFiles.Add(savePath);
                }
            }
            return destinationFiles.ToArray();
        }

        /// <summary>
        /// Extract images from HTML.
        /// </summary>
        /// <param name="html">HtmlContent</param>
        /// <param name="savePath">Path for save images</param>
        private string ExtractImagesFromHTML(string html, string savePath)
        {
            Regex regex = new Regex("<img.*src=\"(.+?)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var matches = regex.Matches(html);
            if (matches.Count <= 0)
                return html;

            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            string destPath = string.Empty, fileName = string.Empty, fileExt = string.Empty;
            List<string> imageUrls = new List<string>();
            Uri fileUrl = null;
            foreach (Match m in matches)
            {
                imageUrls.Add(m.Groups[1].Value);
            }
            imageUrls = imageUrls.Distinct().ToList();
            foreach (var originalPath in imageUrls)
            {
                if (originalPath.StartsWith("http")) // Absolute Path.
                {
                    fileUrl = new Uri(originalPath);
                    fileExt = Path.GetExtension(fileUrl.LocalPath);
                    do
                    {
                        fileName = StringHelper.RandomString() + fileExt;
                        destPath = Path.Combine(savePath, fileName);
                    } while (System.IO.File.Exists(destPath));
                    try
                    {
                        using (WebClient client = new WebClient())
                        {
                            client.DownloadFile(originalPath, destPath);
                            if (System.IO.File.Exists(destPath))
                            {
                                html = html.Replace("src=\"" + originalPath + "\"", "src=\"fileshost/" + fileName + "\"");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
            }

            return html;
        }

        private string GetNMCSVContent(int fileIndex, List<User> uniqueUsers)
        {
            int start = (fileIndex * 200);
            int estimatedLimit = start + 200;
            int limit = estimatedLimit < uniqueUsers.Count ? estimatedLimit : uniqueUsers.Count;
            string csvContent = GetNetmessageCSVHeaders();
            for (int i = start; i < limit; i++)
            {
                csvContent += GetNetmessageCSVRow(uniqueUsers[i]);
            }
            return csvContent;
        }

        private string GetXNMNode(string name, object content)
        {
            return String.Format("<{0}>{1}</{0}>", name, content) + Environment.NewLine;
        }

        private string GetSecureAttributeValue(string value)
        {
            return SecurityElement.Escape(value).Replace("\r", "&#xD;").Replace("\n", "&#xA;");
        }

        private string GetNetmessageCSVHeaders()
        {
            return "EMAIL;FIRSTNAME;LASTNAME;NAME;GENDER;PHONE;MOBILE;DESCRIPTION;PROFESSION;INDEX;FIELD1;FIELD2;FIELD3";
        }

        private string GetNetmessageCSVRow(User user)
        {
            string data = string.Empty;
            data += "\"" + user.Email + "\";";
            data += "\"" + user.FirstName + "\";";
            data += "\"" + user.LastName + "\";";
            data += "\"" + user.FullName + "\";";
            data += "\"" + (!user.Gender.HasValue || user.Gender.Value == 0 ? maleText : femaleText) + "\";";
            data += "\"" + user.Phone + "\";";
            data += "\"" + user.MobilePhone + "\";";
            data += "\"" + user.Remarks + "\";";
            data += "\"" + user.Career + "\";";
            data += "\"" + user.Index + "\";";
            data += "\"" + user.CustomField1 + "\";";
            data += "\"" + user.CustomField2 + "\";";
            data += "\"" + user.CustomField3 + "\"";
            return Environment.NewLine + data;
        }

        private static EmailQue CreateEmailQueObject(long emailId, DateTime now)
        {
            EmailQue emailQue = new EmailQue();
            emailQue.EmailId = emailId;
            emailQue.InsertDate = now;
            emailQue.IsError = false;
            emailQue.Resent = 0;
            emailQue.Status = 0;
            return emailQue;
        }

        public ActionResult Edit(int? id)
        {
            if (id.HasValue)
            {
                Emailing emailing = DataAccess.GetEmailing(id.Value);
                Session["Emailing"] = emailing;
            }
            return RedirectToAction("Step1");
        }

        #region SendTestEmail
        [HttpPost]
        public ActionResult SendTestEmail(string emails, int[] customerGroupIds = null)
        {
            try
            {
                if (customerGroupIds == null || !customerGroupIds.Any())
                    throw new Exception(Resources.Resources.PleaseSelectAtLeastOneCustomerGroup);

                string[] emailList = emails.Replace("\n", "").Split(';').Where(it => it.Trim().Length > 0).ToArray();

                if (emailList == null || !emailList.Any())
                    throw new Exception(Resources.Resources.PleaseInputOnlyValidEmails);

                Emailing model = Preparing();

                // Prepare Email content.
                string htmlBody = GetReadyToSendHTML(model.HtmlDetailString);
                List<EmailingAttachment> attachments = DataAccess.GetEmailingAttachmentsByEmailId(model.EmailId);

                User sampleCustomer = new User();
                if (MemoryCache.Default["SampleCustomer"] != null)
                {
                    sampleCustomer = MemoryCache.Default["SampleCustomer"] as User;

                    if (sampleCustomer == null)
                    {
                        sampleCustomer = Auth.User;
                        MemoryCache.Default.Add("SampleCustomer", sampleCustomer, DateTime.Now.AddDays(1));
                    }
                }
                else
                {
                    Random r = new Random();
                    int index = r.Next(0, customerGroupIds.Length);
                    sampleCustomer = DataAccess.GetSampleCustomerInMailingList(customerGroupIds[index]);

                    if (sampleCustomer == null)
                    {
                        sampleCustomer = Auth.User;
                    }

                    MemoryCache.Default.Add("SampleCustomer", sampleCustomer, DateTime.Now.AddDays(1));
                }

                // Override Site's SMTP Settings if exists.
                EmailArguments args = EmailArguments.GetInstanceFromConfig();
                string unsubscribeMailTo = string.Empty;
                string unsubscribeUrl = string.Empty;
                string server = args.SMTPServer;
                string username = args.Username;
                string password = args.Password;
                int port = args.Port;
                bool enableSsl = false;
                string html = string.Empty;
                string text = string.Empty;
                bool useVERP = false;
                string bouncedEmail = string.Empty;


                if (model.SiteId > 0)
                {
                    if (!DataAccess.GetSiteSMTPSettings(model.SiteId, out server, out username, out password, out port, out enableSsl, out useVERP, out bouncedEmail))
                    {
                        var options = DataAccess.GetOptions("SMTPServer", "SMTPUsername", "SMTPPassword", "SMTPPort", "SMTPUseSSL", "SMTPUseVERP", "BouncedReturnEmail");
                        if (options != null && options.Any())
                        {
                            server = options["SMTPServer"];
                            username = options["SMTPUsername"];
                            password = options["SMTPPassword"];
                            port = DataManager.ToInt(options["SMTPPort"]);
                            enableSsl = DataManager.ToBoolean(options["SMTPUseSSL"]);
                            bouncedEmail = options["BouncedReturnEmail"];
                            useVERP = DataManager.ToBoolean(options["SMTPUseVERP"]);
                        }
                    }
                }
                else
                {
                    var options = DataAccess.GetOptions("SMTPServer", "SMTPUsername", "SMTPPassword", "SMTPPort", "SMTPUseSSL", "SMTPUseVERP", "BouncedReturnEmail");
                    if (options != null && options.Any())
                    {
                        server = options["SMTPServer"];
                        username = options["SMTPUsername"];
                        password = options["SMTPPassword"];
                        port = DataManager.ToInt(options["SMTPPort"]);
                        enableSsl = DataManager.ToBoolean(options["SMTPUseSSL"]);
                        bouncedEmail = options["BouncedReturnEmail"];
                        useVERP = DataManager.ToBoolean(options["SMTPUseVERP"]);
                    }
                }

                string unsubscribeEmail = DataAccess.GetOption("UnsubscribeEmail");

                List<IMail> mailList = new List<IMail>();
                MailBuilder mBuilder = new MailBuilder();
                Bounce bounce = new Bounce();

                mBuilder.From.Add(new MailBox(model.FromEmail, model.FromName));
                mBuilder.ReplyTo.Add(new MailBox(model.FromEmail, model.FromName));
                mBuilder.Sender = new MailBox(username, model.FromName);

                foreach (string email in emailList)
                {
                    unsubscribeMailTo = unsubscribeEmail;
                    unsubscribeUrl = string.Empty;

                    if (!EmailHelper.IsValidEmail(email))
                        continue;

                    mBuilder.To.Clear();
                    mBuilder.To.Add(new MailBox(email));
                    mBuilder.Subject = PersonalizeText(model.Subject, null, sampleCustomer, model.SiteId);
                    html = PersonalizeText(htmlBody, null, sampleCustomer, model.SiteId);
                    //text = PersonalizeText(model.TextDetailString, null, sampleCustomer, model.SiteId);
                    html = InsertTrackingLinks(html, model.EmailId, 0, 0, sampleCustomer.UserId, emailList.First(), ref unsubscribeMailTo, ref unsubscribeUrl);
                    mBuilder.Html = html;

                    if (attachments != null && attachments.Any())
                    {
                        foreach (EmailingAttachment att in attachments)
                        {
                            if (System.IO.File.Exists(att.FilePath))
                                mBuilder.AddAttachment(att.FilePath);
                        }
                    }

                    IMail mail = mBuilder.Create();
                    if (!string.IsNullOrWhiteSpace(unsubscribeMailTo))
                    {
                        mail.ListUnsubscribe.Add(unsubscribeMailTo);
                    }
                    mailList.Add(mail);
                }

                Limilabs.Mail.Log.Enabled = true;
                using (Smtp smtp = new Smtp())
                {
                    smtp.Connect(server, port, enableSsl);
                    smtp.UseBestLogin(username, password);

                    foreach (IMail mail in mailList)
                    {
                        BounceResult bResult = bounce.Examine(mail);
                        if (bResult.IsDeliveryFailure)
                        {
                            throw new Exception("Bounced (" + mail.To.FirstOrDefault() + ") : " + bResult.Reason.ToString());
                        }
                        else
                        {
                            ISendMessageResult result = null;
                            if (useVERP)
                            {
                                SmtpMail smtpMail = SmtpMail.CreateUsingVERP(mail);
                                result = smtp.SendMessage(smtpMail);
                            }
                            else
                            {
                                //mail.ReturnPath = bouncedEmail;
                                result = smtp.SendMessage(mail);
                            }
                            if (result.Status != SendMessageStatus.Success)
                            {
                                throw new Exception(result.ToString());
                            }
                        }
                    }
                    smtp.Close();
                }

                return Json(new
                {
                    isSuccess = true,
                    message = Resources.Resources.TestEmailWasSent
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
        #endregion

        [HttpPost]
        public ActionResult GetDefaultSender(long? siteId)
        {
            try
            {
                if (!siteId.HasValue)
                    throw new Exception(Resources.Resources.PleaseSelectSite);

                string name = "", email = "";
                DataAccess.GetSiteDefaultSender(siteId.Value, out name, out email);

                return Json(new
                {
                    isSuccess = true,
                    name = name,
                    email = email
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
        #endregion

        #region AJax Action Methods

        [HttpPost]
        public ActionResult UploadAttachments()
        {
            int id = DataManager.ToInt(Request.Form["id"]);
            string uploadDir = Path.GetTempPath();
            string tempFileName = string.Empty;
            string imagePath = string.Empty;
            string fileName = string.Empty;
            string baseName = string.Empty;
            string extension = string.Empty;

            var file = Request.Files["Filedata"];
            baseName = Path.GetFileNameWithoutExtension(file.FileName);
            extension = Path.GetExtension(file.FileName).ToLower();
            fileName = file.FileName;
            do
            {
                tempFileName = Path.GetTempFileName() + extension;
                imagePath = Path.Combine(uploadDir, tempFileName);
            } while (System.IO.File.Exists(imagePath));
            file.SaveAs(imagePath);

            if (System.IO.File.Exists(imagePath))
            {
                return Content(fileName + "|" + baseName + "|" + extension + "|" + imagePath);
            }
            else
            {
                throw new Exception("Can't save file please try again.");
            }
        }

        public ActionResult GetTemplateContent(long? templateId = null)
        {
            try
            {
                if (!templateId.HasValue)
                    throw new ArgumentNullException("templateId");

                EmailTemplate template = DataAccess.GetEmailTemplate(templateId.Value, 1);
                if (template == null)
                    throw new Exception("Template not found.");

                List<EmailTemplateAttachment> attachments = DataAccess.GetEmailTemplateAttachmentsByTemplateId(templateId.Value);

                return Json(new
                {
                    isSuccess = true,
                    html = template.HtmlDetailString,
                    attachments = attachments
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
        #endregion

        #region Private Methods
        private void SaveEmailing(Emailing model)
        {
            model.UpdateDate = DateTime.Now;
            if (Auth.User != null)
                model.UserId = Auth.User.UserId;

            if (model.EmailId <= 0)
            {
                model.MjmlDetailString = string.Empty;
                model.HtmlDetailString = string.Empty;
                model.TextDetailString = string.Empty;
                model.StatusId = 0;
                model.InsertDate = model.UpdateDate;
                model.Active = true;
                model.EmailId = DataAccess.AddEmailing(model);
            }
            else
            {
                DataAccess.UpdateEmailing(model);
            }
            Session["Emailing"] = model;
        }

        private Emailing Preparing()
        {
            Emailing emailing = null;
            if (Session["Emailing"] == null)
            {
                string defaultSenderName = string.Empty;
                string defaultSenderEmail = string.Empty;
                if (Auth.User.UserTypeId == UserType.Type.SiteManager || Auth.User.UserTypeId == UserType.Type.Staff)
                {
                    DataAccess.GetSiteDefaultSender(Auth.User.SiteId, out defaultSenderName, out defaultSenderEmail);
                }
                else
                {
                    defaultSenderName = System.Configuration.ConfigurationManager.AppSettings["DefaultSenderName"];
                    defaultSenderEmail = System.Configuration.ConfigurationManager.AppSettings["DefaultSenderEmail"];
                }
                emailing = new Emailing()
                {
                    FromName = defaultSenderName,
                    FromEmail = defaultSenderEmail,
                    HtmlDetail = null,
                    TextDetail = null
                };
                Session["Emailing"] = emailing;
            }
            else
            {
                emailing = (Emailing)Session["Emailing"];
            }
            return emailing;
        }

        private void InitEmailTemplateDropDownList()
        {
            List<EmailTemplateViewModel> list = null;
            if (Auth.User.UserTypeId == UserType.Type.SuperAdmin || Auth.User.UserTypeId == UserType.Type.Admin)
            {
                list = DataAccess.GetSelectableEmailTemplates(1);
            }
            else
            {
                list = DataAccess.GetAvailableEmailTemplates(1, Auth.User.SiteId);
            }
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            foreach (EmailTemplateViewModel it in list)
            {
                selectListItems.Add(new SelectListItem()
                {
                    Text = it.TemplateName,
                    Value = it.TemplateId.ToString()
                });
            }
            ViewBag.EmailTemplates = selectListItems;
        }

        private void InitSiteDropDownList()
        {
            List<Site> sites = DataAccess.GetSitesDropDownListData();
            List<SelectListItem> siteList = ListToDropDownList<Site>(sites, "SiteId", "SiteName");
            siteList.Insert(0, new SelectListItem()
            {
                Text = Resources.Resources.SelectSite,
                Value = "",
                Selected = true
            });
            ViewBag.Sites = siteList;
        }

        private void InitCustomerGroupDropDownList()
        {
            List<CustomerGroup> list = null;
            if (Auth.User.UserTypeId == UserType.Type.SiteManager || Auth.User.UserTypeId == UserType.Type.Staff)
            {
                list = DataAccess.GetAllCustomerGroups(Auth.User.SiteId);
            }
            else
            {
                list = DataAccess.GetAllCustomerGroups();
            }
            ViewBag.CustomerGroups = list;
        }

        private void ValidateEmailing(Emailing model)
        {
            if (String.IsNullOrEmpty(model.EmailName) || String.IsNullOrEmpty(model.Subject) || String.IsNullOrEmpty(model.FromName) || String.IsNullOrEmpty(model.FromEmail))
            {
                Response.Redirect("~/Emailing/Step1");
                return;
            }
        }

        private string GetReadyToSendHTML(string html)
        {
            if (html.IndexOf("{!web}") < 0)
            {
                string content = "<div style=\"color: #000; font-size: 0.8em; text-align: center; margin: 5px auto;\">";
                content += "<span id=\"view\"><a href=\"{!web}\">Voir la version en ligne</a></span>";
                content += "</div>";
                html = InsertContentToHtml(html, content);
            }
            if (html.IndexOf("{!unsubscribe_url}") < 0)
            {
                string content = "<div style=\"color: #000; font-size: 0.8em; text-align: center; margin: 5px auto;\">";
                content += "<span id=\"unsub\"><a href=\"{!unsubscribe_url}\">Cliquez ici pour vous désabonner</a></span>";
                content += "</div>";
                html = InsertContentToHtml(html, content);
            }
            return html;
        }

        private string InsertContentToHtml(string html, string content)
        {
            Regex regex = new Regex(@"<\/body>");
            Match match = regex.Match(html);
            if (match != null)
            {
                html = html.Insert(match.Index, content);
            }
            else
            {
                html += content;
            }
            return html;
        }


        #endregion
    }
}
