using System;
using System.Collections.Generic;
using System.Text;
using Limilabs.Mail;
using Limilabs.Mail.Headers;
using Limilabs.Mail.Tools;
using Limilabs.Client.SMTP;
using IG.Engine.EmailService.BLL;
using System.Linq;
using System.IO;

namespace IG.Engine.EmailService
{
    public class Email
    {
        //public static bool SendEmail(EmailItem pEmail, ref string sMessage, ref bool isBounced)
        //{
        //    SmtpClient client = new SmtpClient();

        //    MailMessage email = new MailMessage();
        //    email.From = new MailAddress(pEmail.From, pEmail.FromDisplayName, Encoding.UTF8);
        //    email.Sender = new MailAddress(pEmail.UserName, pEmail.FromDisplayName, Encoding.UTF8);
        //    email.ReplyToList.Add(new MailAddress(pEmail.From, pEmail.FromDisplayName, Encoding.UTF8));

        //    string[] tos = pEmail.Tos.Split(';', ',');

        //    // Email TO
        //    for (int i = 0; i < tos.Length; i++)
        //    {
        //        email.To.Add(new MailAddress(tos[i], tos[i], Encoding.UTF8));
        //    }

        //    // Email CC
        //    if (pEmail.CC != "")
        //    {
        //        string[] cc = pEmail.CC.Split(';', ',');
        //        for (int i = 0; i < cc.Length; i++)
        //        {
        //            email.CC.Add(new MailAddress(cc[i], cc[i], Encoding.UTF8));
        //        }
        //    }

        //    // Email BCC
        //    if (pEmail.BCC != "")
        //    {
        //        string[] bcc = pEmail.BCC.Split(';', ',');
        //        for (int i = 0; i < bcc.Length; i++)
        //        {
        //            email.Bcc.Add(new MailAddress(bcc[i], bcc[i], Encoding.UTF8));
        //        }
        //    }

        //    // Attach Files
        //    if (pEmail.AttachFiles.Count > 0)
        //    {
        //        for (int i = 0; i < pEmail.AttachFiles.Count; i++)
        //        {
        //            email.Attachments.Add(new Attachment(pEmail.AttachFiles[i]));
        //        }
        //    }

        //    email.Subject = pEmail.Subject;
        //    email.Body = pEmail.Body;
        //    email.IsBodyHtml = true;

        //    System.Net.NetworkCredential basicAuthenticationInfo = new System.Net.NetworkCredential();
        //    basicAuthenticationInfo.UserName = pEmail.UserName;
        //    basicAuthenticationInfo.Password = pEmail.Password;

        //    client.UseDefaultCredentials = false;
        //    client.Credentials = basicAuthenticationInfo;
        //    client.EnableSsl = pEmail.IsEnableSSL;

        //    try
        //    {
        //        client.Send(email);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        sMessage = ex.ToString();
        //        if (sMessage.Length > 1000)
        //            sMessage = sMessage.Substring(0, 900);
        //        return false;
        //    }
        //}
        public static bool SendEmail(EmailItem pEmail, List<EmailingAttachment> attachments, ref string sMessage, ref bool isBounced)
        {
            MailBuilder mBuilder = new MailBuilder();

            mBuilder.From.Add(new MailBox(pEmail.From, pEmail.FromDisplayName));
            mBuilder.ReplyTo.Add(new MailBox(pEmail.From, pEmail.FromDisplayName));
            mBuilder.Sender = new MailBox(pEmail.UserName, pEmail.FromDisplayName);

            string[] tos = pEmail.Tos.Split(';', ',');

            // Email TO
            for (int i = 0; i < tos.Length; i++)
            {
                mBuilder.To.Add(new MailBox(tos[i]));
            }

            // Email CC
            if (pEmail.CC != "")
            {
                string[] cc = pEmail.CC.Split(';', ',');
                for (int i = 0; i < cc.Length; i++)
                {
                    mBuilder.Cc.Add(new MailBox(cc[i]));
                }
            }

            // Email BCC
            if (pEmail.BCC != "")
            {
                string[] bcc = pEmail.BCC.Split(';', ',');
                for (int i = 0; i < bcc.Length; i++)
                {
                    mBuilder.Bcc.Add(new MailBox(bcc[i]));
                }
            }

            // Attach Files
            if (pEmail.AttachFiles.Count > 0)
            {
                for (int i = 0; i < pEmail.AttachFiles.Count; i++)
                {
                    mBuilder.AddAttachment(pEmail.AttachFiles[i]);
                }
            }

            mBuilder.Subject = pEmail.Subject;
            mBuilder.Html = pEmail.Body;

            if(attachments != null && attachments.Any())
            {
                foreach(EmailingAttachment att in attachments)
                {
                    if(File.Exists(att.FilePath))
                        mBuilder.AddAttachment(att.FilePath);
                }
            }

            IMail mail = mBuilder.Create();

            // Add List-Unsubscribe Header
            if (!String.IsNullOrWhiteSpace(pEmail.UnsubscribeMailTo))
            {
                mail.ListUnsubscribe.Add(pEmail.UnsubscribeMailTo);
            }

            try
            {
                // Check bounce address.
                Bounce bounce = new Bounce();
                BounceResult bResult = bounce.Examine(mail);
                if (bResult.IsDeliveryFailure)
                {
                    isBounced = true;
                    throw new Exception(bResult.Status.ToString());
                }

                ISendMessageResult result = null;
                using (Smtp smtp = new Smtp())
                {
                    smtp.Connect(pEmail.Host, pEmail.Port, pEmail.IsEnableSSL);
                    smtp.UseBestLogin(pEmail.UserName, pEmail.Password);

                    if (pEmail.IsUseVERP)
                    {
                        SmtpMail smtpMail = SmtpMail.CreateUsingVERP(mail);
                        result = smtp.SendMessage(smtpMail);
                    }
                    else
                    {
                        //mail.ReturnPath = pEmail.ReturnPath;
                        result = smtp.SendMessage(mail);
                    }

                    smtp.Close();
                }

                if (result.Status != SendMessageStatus.Success)
                {
                    throw new Exception(result.ToString());
                }
                return true;
            }
            catch (Exception ex)
            {
                sMessage = ex.ToString();
                if (sMessage.Length > 1000)
                    sMessage = sMessage.Substring(0, 900);
                return false;
            }
        }
    }


}
