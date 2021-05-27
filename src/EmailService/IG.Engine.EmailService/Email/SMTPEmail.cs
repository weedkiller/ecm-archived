using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace IG.Engine.EmailService
{
    class SMTPEmail
    {
        public static bool SendEmail(EmailItem pEmail, ref string sMessage)
        {
            MailMessage mail = new MailMessage();
            
            mail.From = new MailAddress(pEmail.From,pEmail.DisplayName);
           
            string[] tos = pEmail.Tos.Split(';', ',');
            foreach (string sTo in tos)
            {
                if (sTo.Trim().Length > 0 && sTo.IndexOf("@") > 0)
                {
                    mail.To.Add(sTo);
                }
            }

            mail.Subject = pEmail.Subject ;
            mail.IsBodyHtml = true;
            mail.Body = pEmail.Body ;

            
            //if (cc != string.Empty)
            //{
            //    mail.CC.Add(cc);
            //}
            //if (BCC != string.Empty)
            //{
            //    mail.Bcc.Add(BCC);
            //}

            SmtpClient smtp = new SmtpClient(pEmail.Host , pEmail.Port);
            smtp.EnableSsl = false ;
            smtp.Credentials = new System.Net.NetworkCredential(pEmail.UserName, pEmail.Password);

            try
            {
                smtp.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                sMessage = ex.ToString();
                return false;
            }
            
        }
        //SendEmail
    }

}
