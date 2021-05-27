using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Data;
using Limilabs.Client.SMTP;
using Limilabs.Mail;
using Limilabs.Mail.Headers;
using Limilabs.Mail.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.ECM.Controllers
{
    public class TasksController : BaseController
    {
        #region Action Methods

        #region SyncCustomerTypes
        public ActionResult SyncCustomerTypes(long? id)
        {
            List<long> siteIds = new List<long>();
            if (!id.HasValue)
            {
                siteIds = DataAccess.GetAllSitesWithChronogolfClubId();
            }
            else
            {
                siteIds.Add(id.Value);
            }

            var task = new Task(() =>
            {
                foreach (long siteId in siteIds)
                {
                    try
                    {
                        DataAccess.SyncCustomerTypes(siteId);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
            });

            task.Start();

            return Json(new
            {
                success = true
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SyncCustomerGroups
        public ActionResult SyncCustomerGroups(long? id, int? all)
        {
            List<long> siteIds = new List<long>();
            if (!id.HasValue)
            {
                siteIds = DataAccess.GetAllSitesWithChronogolfClubId();
            }
            else
            {
                siteIds.Add(id.Value);
            }

            var task = new Task(() =>
            {
                foreach (long siteId in siteIds)
                {
                    try
                    {
                        DataAccess.SyncCustomerTypes(siteId);
                        DataAccess.SyncAllCustomerGroups(siteId, all.HasValue && all.Value == 1);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
            });

            task.Start();

            return Json(new
            {
                success = true
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region RunEmailService
        public ActionResult RunEmailService()
        {
            Task task = new Task(() =>
            {
                try
                {
                    DataSet ds = this.DataAccess.GetPendingEmail();
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        ProcessBatchSendEmails(ds);
                    }
                    else
                    {
                        // Resending email
                        DataSet dsFailed = this.DataAccess.GetFailedEmail();
                        ProcessBatchSendEmails(dsFailed);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "There are error while using Tasks/RunEmailService");
                }
            });
            task.Start();

            return Json(new
            {
                success = true
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region Private Methods

        #region ProcessBatchSendEmails
        /// <summary>
        /// Process Batch Sending Email by Specific DataSet.
        /// </summary>
        /// <param name="ds"></param>
        private void ProcessBatchSendEmails(DataSet ds)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                int status = 0;
                string message = String.Empty;
                long emailId = 0;
                int resent = 0;
                List<EmailingAttachment> attachments = new List<EmailingAttachment>();
                EmailArguments globalArgs = GetGlobalEmailArguments(ds.Tables[1].Rows);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    message = String.Empty;
                    emailId = DataManager.ToLong(dr["EmailId"]);
                    resent = DataManager.ToInt(dr["Resent"]);
                    attachments = DataAccess.GetEmailingAttachmentsByEmailId(emailId);
                    status = SendEmail(dr, globalArgs, attachments, ref message);

                    if (status == 1)
                    {
                        // Send emmail successfully
                        this.DataAccess.UpdateEmailQueStatus(DataManager.ToInt(dr["EmailQueId"]), status, DateTime.Now, false, message, resent);
                    }
                    else
                    {
                        // Send email Error 
                        resent++;
                        this.DataAccess.UpdateEmailQueStatus(DataManager.ToInt(dr["EmailQueId"]), status, DateTime.Now, true, message, resent);
                    }
                }
            }
        }
        #endregion

        #region GetGlobalEmailArguments
        /// <summary>
        /// Get Global Email Arguments.
        /// </summary>
        /// <param name="rows">Data Row</param>
        /// <returns>Global Email Arguments</returns>
        private EmailArguments GetGlobalEmailArguments(DataRowCollection rows)
        {
            EmailArguments globalArgs = EmailArguments.GetInstanceFromConfig();
            foreach (DataRow dr in rows)
            {
                switch (dr["OptionKey"])
                {
                    case "SMTPServer":
                        globalArgs.SMTPServer = DataManager.ToString(dr["OptionValue"]);
                        break;
                    case "SMTPPort":
                        globalArgs.Port = DataManager.ToInt(dr["OptionValue"], 587);
                        break;
                    case "SMTPUsername":
                        globalArgs.Username = DataManager.ToString(dr["OptionValue"]);
                        break;
                    case "SMTPPassword":
                        globalArgs.Password = DataManager.ToString(dr["OptionValue"]);
                        break;
                    case "SMTPUseSSL":
                        globalArgs.EnableSsl = DataManager.ToBoolean(dr["OptionValue"]);
                        break;
                    case "SMTPUseVERP":
                        globalArgs.IsVERP = DataManager.ToBoolean(dr["OptionValue"]);
                        break;
                    case "DefaultSenderName":
                        globalArgs.SenderName = DataManager.ToString(dr["OptionValue"]);
                        break;
                    case "DefaultSenderEmail":
                        globalArgs.SenderEmail = DataManager.ToString(dr["OptionValue"]);
                        break;
                    case "BouncedReturnEmail":
                        globalArgs.BouncedReturnEmail = DataManager.ToString(dr["OptionValue"]);
                        break;
                }
            }
            if (String.IsNullOrWhiteSpace(globalArgs.SenderName))
            {
                globalArgs.SenderName = globalArgs.SenderEmail;
            }
            return globalArgs;
        }
        #endregion

        #region SendEmail
        /// <summary>
        /// Send Email.
        /// </summary>
        /// <param name="dr">Data Row</param>
        /// <param name="global">Global Email Arguments</param>
        /// <param name="attachments">Email Attachments</param>
        /// <param name="sMessage">Returned Message</param>
        /// <returns></returns>
        private int SendEmail(DataRow dr, EmailArguments global, List<EmailingAttachment> attachments, ref string sMessage)
        {
            // Send email 
            try
            {
                // Override Site's SMTP Settings if exists.
                EmailArguments args = global.Clone();
                string toEmail = DataManager.ToString(dr["Email"]);
                string toName = DataManager.ToString(dr["CustomerName"]);
                byte[] contentBytes = null;
                string content = String.Empty;

                // Set From Name & Email.
                args.SenderEmail = DataManager.ToString(dr["FromEmail"]);
                args.SenderName = DataManager.ToString(dr["FromName"]);

                if (!DataManager.ToBoolean(dr["IsUseGlobalSMTPSettings"], false))
                {
                    args.SMTPServer = DataManager.ToString(dr["SMTPServer"], "");
                    args.Port = DataManager.ToInt(dr["SMTPPort"], 587);
                    args.Username = DataManager.ToString(dr["SMTPUsername"], "");
                    args.Password = DataManager.ToString(dr["SMTPPassword"], "");
                    args.EnableSsl = DataManager.ToBoolean(dr["SMTPUseSSL"], false);
                    args.IsVERP = DataManager.ToBoolean(dr["SMTPUseVERP"], false);
                    args.BouncedReturnEmail = DataManager.ToString(dr["BouncedReturnEmail"]);
                    if (String.IsNullOrWhiteSpace(args.SenderName))
                    {
                        args.SenderName = args.SenderEmail;
                    }
                }

                List<IMail> mailList = new List<IMail>();
                MailBuilder mBuilder = new MailBuilder();
                Bounce bounce = new Bounce();

                mBuilder.From.Add(new MailBox(args.SenderEmail, args.SenderName));
                mBuilder.ReplyTo.Add(new MailBox(args.SenderEmail, args.SenderName));
                mBuilder.Sender = new MailBox(args.Username, args.SenderName);

                string unsubscribeMailTo = DataManager.ToString(dr["UnsubscribeMailTo"]);
                string unsubscribeUrl = DataManager.ToString(dr["UnsubscribeUrl"]);

                mBuilder.To.Clear();
                if (String.IsNullOrWhiteSpace(toName))
                {
                    mBuilder.To.Add(new MailBox(toEmail));
                }
                else
                {
                    mBuilder.To.Add(new MailBox(toEmail, toName));
                }

                mBuilder.Subject = DataManager.ToString(dr["Subject"]);

                if (dr["HtmlDetail"] != null && dr["HtmlDetail"] != DBNull.Value)
                {
                    contentBytes = (byte[])dr["HtmlDetail"];
                    args.MailBody = Encoding.UTF8.GetString(contentBytes);
                }
                else
                {
                    throw new Exception("Empty Email Body.");
                }

                mBuilder.Html = args.MailBody;

                if (attachments != null && attachments.Any())
                {
                    foreach (EmailingAttachment att in attachments)
                    {
                        if (System.IO.File.Exists(att.FilePath))
                            mBuilder.AddAttachment(att.FilePath);
                    }
                }

                IMail mail = mBuilder.Create();
                if (!String.IsNullOrWhiteSpace(unsubscribeMailTo))
                {
                    mail.ListUnsubscribe.Add(unsubscribeMailTo);
                }
                if (!String.IsNullOrWhiteSpace(args.BouncedReturnEmail))
                {
                    mail.ReturnPath = args.BouncedReturnEmail;
                }

                Limilabs.Mail.Log.Enabled = true;
                using (Smtp smtp = new Smtp())
                {
                    smtp.Connect(args.SMTPServer, args.Port, args.EnableSsl);
                    smtp.UseBestLogin(args.Username, args.Password);

                    BounceResult bResult = bounce.Examine(mail);
                    if (bResult.IsDeliveryFailure)
                    {
                        throw new Exception("Bounced (" + mail.To.FirstOrDefault() + ") : " + bResult.Reason.ToString());
                    }
                    else
                    {
                        ISendMessageResult result = null;
                        if (args.IsVERP)
                        {
                            SmtpMail smtpMail = SmtpMail.CreateUsingVERP(mail);
                            result = smtp.SendMessage(smtpMail);
                        }
                        else
                        {
                            result = smtp.SendMessage(mail);
                        }
                        if (result.Status != SendMessageStatus.Success)
                        {
                            throw new Exception(result.ToString());
                        }
                    }

                    smtp.Close();
                }

                sMessage = "Sent completed";
                return 1;
            }
            catch (Exception ex)
            {
                sMessage = "Send Failed : " + ex.Message;
                return -1;
            }
        }
        #endregion

        #endregion

        #region Test
        public ActionResult Test()
        {
            string html = "<div id=\"c357\" style=\"box-sizing: border-box; font-family: \'MyriadPro-Regular, \'Myriad Pro Regular\', MyriadPro, \'Myriad Pro\', Helvetica, Arial, sans-serif\'; font-size: 15px; color: rgb(0, 0, 0); width: 100%; min-height: 100%; background-color: #f3f4f4; padding: 20px 0px 20px 0px; border-collapse: separate;\"><table width=\"100%\" bgcolor=\"#fff\" id=\"c361\" class=\"c1939\" style=\"box-sizing: border-box; background-color: #fff; max-width: 640px; width: 100%; margin: 0 auto 20px;\"><tr id=\"c370\" style=\"box-sizing: border-box;\"><td id=\"c2244\" valign=\"top\" class=\"c2244\" style=\"box-sizing: border-box; font-family: MyriadPro-Regular, \'Myriad Pro Regular\', MyriadPro, \'Myriad Pro\', Helvetica, Arial, sans-serif; font-size: 1em; vertical-align: top; color: #000; margin: 0; padding: 0;\"></td><td id=\"c2852\" valign=\"top\" class=\"c2852\" style=\"box-sizing: border-box; font-family: MyriadPro-Regular, \'Myriad Pro Regular\', MyriadPro, \'Myriad Pro\', Helvetica, Arial, sans-serif; font-size: 1em; vertical-align: top; color: #000; margin: 0; padding: 0;\"></td><td id=\"c1951\" width=\"100%\" height=\"50\" valign=\"top\" style=\"box-sizing: border-box; font-family: \'MyriadPro-Regular, \'Myriad Pro Regular\', MyriadPro, \'Myriad Pro\', Helvetica, Arial, sans-serif\'; font-size: 1em; color: rgb(0, 0, 0); width: 100%; height: 50px; vertical-align: top; max-width: 640px;\"><table width=\"100%\" height=\"150\" id=\"c381\" class=\"c1353\" style=\"box-sizing: border-box; height: auto; margin: 0 auto 10px auto; padding: 5px 5px 5px 5px; width: 100%;\"><tr id=\"c390\" style=\"box-sizing: border-box;\"><td id=\"c1365\" valign=\"top\" style=\"box-sizing: border-box; font-family: MyriadPro-Regular, \'Myriad Pro Regular\', MyriadPro, \'Myriad Pro\', Helvetica, Arial, sans-serif; font-size: 15px; vertical-align: top; color: #000; margin: 0; padding: 0;\"><img src=\"https://ecm.danslesgolfs.com/Uploads/EmailTemplates/85/1.jpg\" id=\"c398\" class=\"c1524\" style=\"display: inline-block; box-sizing: border-box; color: black; vertical-align: top;\"><img src=\"https://ecm.danslesgolfs.com/Uploads/EmailTemplates/85/2.jpg\" id=\"c399\" class=\"c1965\" style=\"display: inline-block; box-sizing: border-box; max-width: 100%; vertical-align: top;\"></td></tr></table><table width=\"100%\" height=\"150\" id=\"c413\" class=\"c1466\" style=\"box-sizing: border-box; height: 150px; margin: 0 auto 10px auto; padding: 5px 5px 5px 5px; width: 100%;\"><tr id=\"c422\" style=\"box-sizing: border-box;\"><td id=\"c1478\" valign=\"top\" style=\"box-sizing: border-box; font-family: MyriadPro-Regular, \'Myriad Pro Regular\', MyriadPro, \'Myriad Pro\', Helvetica, Arial, sans-serif; font-size: 1em; vertical-align: top; color: #000; margin: 0; padding: 0;\"><img src=\"https://ecm.danslesgolfs.com/Uploads/EmailTemplates/Global/offre1.jpg\" id=\"c430\" class=\"c1553\" style=\"box-sizing: border-box; vertical-align: top; display: inline-block; color: black\"><img src=\"https://ecm.danslesgolfs.com/Uploads/EmailTemplates/Global/offre2.jpg\" id=\"c434\" class=\"c1997\" style=\"box-sizing: border-box; max-width: 100%; vertical-align: top; display: inline-block; color: black;\"><img src=\"https://ecm.danslesgolfs.com/Uploads/EmailTemplates/Global/NEWS-destockage-janvier2018.jpg\" id=\"c438\" class=\"c2045\" style=\"box-sizing: border-box; max-width: 100%; vertical-align: top; display: inline-block; color: black;\"><img src=\"https://ecm.danslesgolfs.com/Uploads/EmailTemplates/Global/1200x627slider-FB-lacanau.jpg\" id=\"c442\" class=\"c2117\" style=\"box-sizing: border-box; max-width: 100%; vertical-align: top; display: inline-block; color: black;\"></td></tr></table><table width=\"100%\" id=\"c446\" height=\"1\" bgcolor=\"rgba(0, 0, 0, 0.1)\" class=\"c1846\" style=\"border: 0px solid #000000; .dividerbox-sizing: border-box; font-family: \'MyriadPro-Regular, \'Myriad Pro Regular\', MyriadPro, \'Myriad Pro\', Helvetica, Arial, sans-serif\'; font-size: 1em; background-color: rgba(0, 0, 0, 0.1); height: 1px; padding: 6px 1px 6px 1px; color: #000000; width: 100%; margin-top: 10px; margin-bottom: 10px; margin: 0px 0px 0px 0px; box-sizing: border-box;\"><tr id=\"c455\" style=\"box-sizing: border-box;\"></tr></table><table width=\"100%\" height=\"150\" id=\"c464\" class=\"c1730\" style=\"height: auto; margin: 0 auto 10px auto; padding: 5px 5px 5px 5px; width: 100%; box-sizing: border-box;\"><tr id=\"c473\" style=\"box-sizing: border-box;\"><td id=\"c1742\" width=\"33.3333%\" valign=\"top\" style=\"box-sizing: border-box; font-family: MyriadPro-Regular, \'Myriad Pro Regular\', MyriadPro, \'Myriad Pro\', Helvetica, Arial, sans-serif; font-size: 1em; vertical-align: top; color: #000; margin: 0; padding: 0; width: 33.3333%;\"><div id=\"c481\" class=\"c1687\" style=\"box-sizing: border-box; font-size: 1em; color: rgb(0, 0, 0); padding: 10px; font-family: Tahoma, Geneva, sans-serif; text-align: center;\">R\u00E9servation membres</div><a title=\"R\u00E9servation membres\" href=\"https://www.chronogolf.fr/\" target=\"_blank\" id=\"c486\" class=\"c1942\" style=\"box-sizing: border-box; display: inline-block; padding: 5px; min-height: auto; min-width: 50px;\"><img src=\"https://ecm.danslesgolfs.com/Uploads/EmailTemplates/Global/bouton reservation disneyland.png\" id=\"c492\" class=\"c1638\" style=\"box-sizing: border-box; max-width: auto; vertical-align: top; display: inline-block; color: black;\"></a></td><td id=\"c1746\" width=\"33.3333%\" valign=\"top\" align=\"center\" style=\"box-sizing: border-box; font-family: MyriadPro-Regular, \'Myriad Pro Regular\', MyriadPro, \'Myriad Pro\', Helvetica, Arial, sans-serif; font-size: 1em; vertical-align: top; color: #000; margin: 0; padding: 0; width: 33.3333%; text-align: center;\"><div id=\"c490\" class=\"c1788\" style=\"box-sizing: border-box; font-size: 1em; color: rgb(0, 0, 0); padding: 10px; font-family: Tahoma, Geneva, sans-serif;\">R\u00E9servation visiteurs</div><a id=\"c500\" class=\"c2974\" style=\"box-sizing: border-box; display: inline-block; padding: 5px; min-height: 50px; min-width: 50px;\"><img src=\"https://ecm.danslesgolfs.com/Uploads/EmailTemplates/Global/bouton reservation disneyland.png\" id=\"c506\" class=\"c2612\" style=\"box-sizing: border-box; vertical-align: top; display: inline-block; color: black; max-width: auto;\"></a></td><td id=\"c1750\" width=\"33.3333%\" valign=\"top\" align=\"center\" style=\"box-sizing: border-box; font-family: MyriadPro-Regular, \'Myriad Pro Regular\', MyriadPro, \'Myriad Pro\', Helvetica, Arial, sans-serif; font-size: 1em; vertical-align: top; color: #000; margin: 0; padding: 0; width: 33.3333%; text-align: center;\"><div id=\"c509\" class=\"c1948\" style=\"box-sizing: border-box; font-size: 1em; color: rgb(0, 0, 0); padding: 10px; font-family: Tahoma, Geneva, sans-serif; text-align: center;\">R\u00E9servation cours</div><a id=\"c519\" class=\"c2225\" style=\"box-sizing: border-box; display: inline-block; padding: 5px; min-height: 50px; min-width: 50px;\"><img src=\"https://ecm.danslesgolfs.com/Uploads/EmailTemplates/Global/bouton reservation disneyland.png\" id=\"c525\" class=\"c2169\" style=\"box-sizing: border-box; max-width: 100%; vertical-align: top; display: inline-block; color: black;\"></a></td></tr></table><div id=\"c423\" class=\"c1584\" style=\"max-width: 100%; height: auto; margin: 0 auto 10px auto; width: 100%; box-sizing: border-box; font-family: \'MyriadPro-Regular, \'Myriad Pro Regular\', MyriadPro, \'Myriad Pro\', Helvetica, Arial, sans-serif\'; color: rgb(0, 0, 0); text-align: center; padding: 5px 0; font-size: 12px;\"><span id=\"view\" style=\"box-sizing: border-box;\"><a href=\"{!web}\" id=\"c432\" style=\"box-sizing: border-box;\">Voir la version en ligne</a></span><div id=\"c548\" class=\"c1759\" style=\"box-sizing: border-box; font-family: \'MyriadPro-Regular, \'Myriad Pro Regular\', MyriadPro, \'Myriad Pro\', Helvetica, Arial, sans-serif\'; color: rgb(0, 0, 0); text-align: center; padding: 5px 0; font-size: 12px;\"><span id=\"unsub\" style=\"box-sizing: border-box;\"><a href=\"{!unsubscribe_url}\" id=\"c557\" style=\"box-sizing: border-box;\">Cliquez ici pour vous d\u00E9sabonner</a></span></div></div></td></tr></table><div id=\"c451\" class=\"c1224\" style=\"box-sizing: border-box; font-family: \'MyriadPro-Regular, \'Myriad Pro Regular\', MyriadPro, \'Myriad Pro\', Helvetica, Arial, sans-serif\'; font-size: 15px; color: rgb(0, 0, 0); background-color: #fff; width: 554px; max-width: 100%; margin: 0 auto 20px;\"></div></div>";
            html = ECMHelper.GetReadyToSendHTML(html);
            return Content(html);
        }
        #endregion

        #region GenerateNetmessageReports
        public ActionResult GenerateNetmessageReports()
        {
            Task task = new Task(() =>
            {
                string baseSavePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Uploads/Netmessage/"), DateTime.Now.ToString("yyyyMMddHHmmssffff"));
                var helper = new NetmessageHelper(baseSavePath);
                helper.GenerateReports();
            });
            task.Start();
            return Json(new
            {
                success = true
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}