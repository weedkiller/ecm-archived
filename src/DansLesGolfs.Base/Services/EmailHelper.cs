using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace DansLesGolfs.Base
{
    #region Class : NationalMailAddressCollection
    public class NationalMailAddressCollection : IList<MailAddress>, ICollection<MailAddress>, IEnumerable<MailAddress>
    {
        MailAddressCollection mails = new MailAddressCollection();

        public MailAddress this[int index]
        {
            get
            {
                return mails[index];
            }
            set
            {
                mails[index] = value;
            }
        }

        public void Add(string emailAddress, string displayName)
        {
            mails.Add(new MailAddress(emailAddress, displayName, Encoding.UTF8));
        }

        public void Add(MailAddress item)
        {
            mails.Add(new MailAddress(item.Address, item.DisplayName, Encoding.UTF8));
        }

        public int IndexOf(MailAddress item)
        {
            return mails.IndexOf(item);
        }

        public void Insert(int index, MailAddress item)
        {
            mails.Insert(index, new MailAddress(item.Address, item.DisplayName, Encoding.UTF8));
        }

        public void RemoveAt(int index)
        {
            mails.RemoveAt(index);
        }

        public void Clear()
        {
            mails.Clear();
        }

        public bool Contains(MailAddress item)
        {
            return mails.Contains(item);
        }

        public void CopyTo(MailAddress[] array, int arrayIndex)
        {
            if (arrayIndex >= mails.Count)
                return;

            array[arrayIndex] = new MailAddress(mails[arrayIndex].Address, mails[arrayIndex].DisplayName, Encoding.UTF8);
        }

        public int Count
        {
            get { return mails.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(MailAddress item)
        {
            return mails.Remove(item);
        }

        public IEnumerator<MailAddress> GetEnumerator()
        {
            return mails.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    }
    #endregion

    #region Class : EmailArguments
    /// <summary>
    /// Represent the email argument. use for config email sending process.
    /// </summary>
    public class EmailArguments
    {
        #region Fields
        private string smtpServer;
        private int port = 25;
        private bool hasPassword;
        private string username;
        private string password;
        private string subject;
        private string mailBody;
        private string mailBodyText;
        private string senderEmail;
        private string senderName;
        private string bouncedReturnEmail;
        private NationalMailAddressCollection to = new NationalMailAddressCollection();
        private NationalMailAddressCollection cc = new NationalMailAddressCollection();
        private NationalMailAddressCollection bcc = new NationalMailAddressCollection();
        private NationalMailAddressCollection replyTo = new NationalMailAddressCollection();
        private List<Attachment> attachments = new List<Attachment>();
        private MailPriority priority = MailPriority.Normal;
        private bool isDeleteFilesAfterSent = false;
        private Encoding encoding = Encoding.UTF8;
        private bool isAsync = true;
        private bool isVERP = false;
        private bool enableSsl = false;
        private List<string> listUnsubscribes = new List<string>();
        private Dictionary<string, string> headers = new Dictionary<string, string>();
        #endregion

        #region Properties
        /// <summary>
        /// SMTP Server.
        /// </summary>
        public string SMTPServer
        {
            get { return smtpServer; }
            set { smtpServer = value; }
        }

        /// <summary>
        /// SMTP PORT
        /// </summary>
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        /// <summary>
        /// There is a password or not?
        /// </summary>
        public bool HasPassword
        {
            get { return hasPassword; }
            set { hasPassword = value; }
        }

        /// <summary>
        /// SMTP Username.
        /// </summary>
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        /// <summary>
        /// SMTP Password.
        /// </summary>
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        /// <summary>
        /// SMTP Password.
        /// </summary>
        public string BouncedReturnEmail
        {
            get { return bouncedReturnEmail; }
            set { bouncedReturnEmail = value; }
        }

        /// <summary>
        /// Email subject.
        /// </summary>
        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        /// <summary>
        /// Email's message body.
        /// </summary>
        public string MailBody
        {
            get { return mailBody; }
            set { mailBody = value; }
        }

        /// <summary>
        /// Email's message body (text format).
        /// </summary>
        public string MailBodyText
        {
            get { return mailBodyText; }
            set { mailBodyText = value; }
        }

        /// <summary>
        /// Sender name.
        /// </summary>
        public string SenderName
        {
            get { return senderName; }
            set { senderName = value; }
        }

        /// <summary>
        /// Sender email.
        /// </summary>
        public string SenderEmail
        {
            get { return senderEmail; }
            set { senderEmail = value; }
        }

        /// <summary>
        /// Email Address of Recipients.
        /// </summary>
        public NationalMailAddressCollection To
        {
            get { return to; }
            set { to = value; }
        }

        /// <summary>
        /// CC Emails
        /// </summary>
        public NationalMailAddressCollection CC
        {
            get { return cc; }
            set { cc = value; }
        }

        /// <summary>
        /// BCC Emails
        /// </summary>
        public NationalMailAddressCollection BCC
        {
            get { return bcc; }
            set { bcc = value; }
        }

        public EmailArguments Clone()
        {
            var args = GetInstanceFromConfig();
            var type = typeof(EmailArguments);
            var properties = type.GetProperties();
            foreach(var property in properties)
            {
                if(property.CanRead && property.CanWrite)
                {
                    property.SetValue(args, property.GetValue(this));
                }
            }
            return args;
        }

        /// <summary>
        /// Reply to Emails
        /// </summary>
        public NationalMailAddressCollection ReplyTo
        {
            get { return replyTo; }
            set { replyTo = value; }
        }

        /// <summary>
        /// Email priority (default is normal).
        /// </summary>
        public MailPriority Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        /// <summary>
        /// List of Attachment Objects.
        /// </summary>
        public List<Attachment> Attachments
        {
            get { return attachments; }
            private set { attachments = value; }
        }

        /// <summary>
        /// Determind this email argument have attachments or not
        /// </summary>
        public bool HaveAttachments
        {
            get
            {
                return attachments != null && attachments.Any();
            }
        }

        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        public bool IsAsync
        {
            get { return isAsync; }
            set { isAsync = value; }
        }

        public bool IsVERP
        {
            get { return isVERP; }
            set { isVERP = value; }
        }

        public bool EnableSsl
        {
            get { return enableSsl; }
            set { enableSsl = value; }
        }

        public List<string> ListUnsubscribes
        {
            get { return listUnsubscribes; }
            set { listUnsubscribes = value; }
        }

        public Dictionary<string, string> Headers
        {
            get { return headers; }
            set { headers = value; }
        }
        #endregion

        public EmailArguments()
            : this("", "", "", 25, "", "")
        {
        }

        public EmailArguments(string smtpserver, string username, string password, int port, string senderName, string senderEmail)
        {
            this.smtpServer = smtpserver;
            this.username = username;
            this.password = password;
            this.port = port;
            this.senderName = senderName;
            this.senderEmail = senderEmail;
        }

        #region Delegate
        /// <summary>
        /// Callback that will be called after SendCompleted Event was fired.
        /// </summary>
        public Action SendCompletedCallback;
        /// <summary>
        /// Callback that will be called when sending email failed.
        /// </summary>
        public Action SendFailedCallback;
        #endregion

        #region Methods

        #region From methods
        /// <summary>
        /// Create and get new MailAddress object of sender.
        /// </summary>
        /// <returns></returns>
        public MailAddress GetFromMailAddress()
        {
            return new MailAddress(this.senderEmail, this.senderName, Encoding.UTF8);
        }
        #endregion

        #region Sender methods
        /// <summary>
        /// Create and get new MailAddress object of sender.
        /// </summary>
        /// <returns></returns>
        public MailAddress GetSenderMailAddress()
        {
            return new MailAddress(this.username, this.senderName, Encoding.UTF8);
        }
        #endregion

        #region Attachment methods
        /// <summary>
        /// Assign file path to be attachment file.
        /// </summary>
        /// <param name="path">File path.</param>
        public void AddAttachment(string path)
        {
            Attachment attachment = new Attachment(path);
            attachment.NameEncoding = Encoding.UTF8;
            this.Attachments.Add(attachment);
        }

        /// <summary>
        /// Assign file path to be attachment file.
        /// </summary>
        /// <param name="path">File paths.</param>
        public void AddAttachment(IEnumerable<string> paths)
        {
            Attachment attachment = null;
            foreach (string path in paths)
            {
                attachment = new Attachment(path);
                attachment.NameEncoding = Encoding.UTF8;
                this.Attachments.Add(attachment);
            }
        }

        /// <summary>
        /// Assign File name and Stream object to be attachment file.
        /// </summary>
        /// <param name="filename">File name.</param>
        /// <param name="stream">Stream Object.</param>
        public void AddAttachment(string filename, Stream stream)
        {
            Attachment attachment = new Attachment(stream, filename);
            attachment.NameEncoding = Encoding.UTF8;
            this.Attachments.Add(attachment);
        }

        /// <summary>
        /// Add Attachment Directly
        /// </summary>
        /// <param name="attachment">Attachment object that you want to add to attachment list</param>
        public void AddAttachment(Attachment attachment)
        {
            attachment.NameEncoding = Encoding.UTF8;
            this.Attachments.Add(attachment);
        }

        /// <summary>
        /// Remove specific attachment object
        /// </summary>
        /// <param name="attachment">Attachment object that you want to remove from attachment list.</param>
        public void RemoveAttachment(Attachment attachment)
        {
            this.Attachments.Remove(attachment);
        }

        public void RemoveAttachmentAt(int index)
        {
            if (this.Attachments.Count > index)
            {
                this.Attachments.RemoveAt(index);
            }
        }

        /// <summary>
        /// Clear all attachments
        /// </summary>
        public void ClearAttachments()
        {
            this.attachments.Clear();
        }
        #endregion

        #region GetInstance
        /// <summary>
        /// Get Email Argument based on Application config.
        /// </summary>
        /// <returns></returns>
        public static EmailArguments GetInstanceFromConfig()
        {
            EmailConfig config = EmailConfig.GetInstanceFromConfig();
            EmailArguments args = new EmailArguments();
            args.SMTPServer = config.SMTPServer;
            args.Username = config.Username;
            args.password = config.Password;
            args.port = config.Port;
            args.SenderName = config.DefaultSenderName;
            args.SenderEmail = config.DefaultSenderEmail;
            return args;
        }
        #endregion

        private string GetVERPEmail(string from, string to)
        {
            string verpEmail = string.Empty;
            string[] fromParts = from.Split('@');
            string[] toParts = to.Split('@');

            if (fromParts.Length < 2 || toParts.Length < 2)
            {
                verpEmail = from;
            }
            else
            {
                verpEmail = fromParts[0] + "+" + to.Replace('@', '=');
            }
            return verpEmail;
        }

        #endregion
    }
    #endregion

    #region Class : EmailConfig
    public class EmailConfig
    {
        public string Pop3Server { get; set; }
        public string SMTPServer { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string DefaultSenderName { get; set; }
        public string DefaultSenderEmail { get; set; }

        public static EmailConfig GetInstanceFromConfig()
        {
            EmailConfig config = new EmailConfig();
            config.SMTPServer = System.Configuration.ConfigurationManager.AppSettings["SMTPHost"];
            config.Username = System.Configuration.ConfigurationManager.AppSettings["FromEmailAddress"];
            config.Password = System.Configuration.ConfigurationManager.AppSettings["FromEmailPassword"];
            config.DefaultSenderName = System.Configuration.ConfigurationManager.AppSettings["DefaultSenderName"];
            config.DefaultSenderEmail = System.Configuration.ConfigurationManager.AppSettings["DefaultSenderEmail"];

            int port = 25;
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["SMTPPort"], out port);
            config.Port = port;

            return config;
        }
    }
    #endregion

    #region Class : EmailHelper
    /// <summary>
    /// ทำหน้าที่ในการส่ง E-mail
    /// </summary>
    public class EmailHelper
    {
        #region SendEmail
        /// <summary>
        /// ส่ง E-mail ถ้าส่งไม่สำเร็จ จะเก็บเอาไว้ที่ StartUpPath\\MailBox ของเครื่อง Client
        /// </summary>
        /// <param name="smtpServer">ชื่อของ Mail Server หรือ Smtp Mail</param>
        /// <param name="subject">ชื่อ Subject หรือชื่อหัวข้อ</param>
        /// <param name="text">ข้อความที่ต้องการส่ง สามารถส่งเป็นแบบ ภาษา Html ได้</param>
        /// <param name="from">ชื่อผู้ส่ง</param>
        /// <param name="to">E-mail ของคนรับ</param>
        /// <param name="displayTo">ชื่อของผู้รับที่ต้องการแสดงให้เห็น</param>
        /// <returns></returns>
        public static bool SendEmail(string smtpServer, string subject, string text, string from, string to, string displayTo)
        {
            EmailArguments emailArgs = new EmailArguments()
            {
                SMTPServer = smtpServer,
                Port = 25,
                HasPassword = false,
                Username = string.Empty,
                Password = string.Empty,
                Subject = subject,
                MailBody = text,
                SenderEmail = from,
                SenderName = null
            };
            emailArgs.To.Add(to, displayTo);
            return SendEmail(emailArgs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="smtpServer">ชื่อของ Mail Server หรือ Smtp Mail</param>
        /// <param name="port">Port ที่ต้องการใช้สำหรับการส่ง EMail</param>
        /// <param name="subject">ชื่อ Subject หรือชื่อหัวข้อ</param>
        /// <param name="text">ข้อความที่ต้องการส่ง สามารถส่งเป็นแบบ ภาษา Html ได้</param>
        /// <param name="from">ชื่อผู้ส่ง</param>
        /// <param name="to">E-mail ของคนรับ</param>
        /// <param name="displayTo">ชื่อของผู้รับที่ต้องการแสดงให้เห็น</param>
        /// <returns></returns>
        public static bool SendEmail(string smtpServer, int port, string subject, string text, string from, string to, string displayTo)
        {
            EmailArguments emailArgs = new EmailArguments()
            {
                SMTPServer = smtpServer,
                Port = port,
                HasPassword = false,
                Username = string.Empty,
                Password = string.Empty,
                Subject = subject,
                MailBody = text,
                SenderEmail = from,
                SenderName = null
            };
            emailArgs.To.Add(to, displayTo);
            return SendEmail(emailArgs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="smtpServer">ชื่อของ Mail Server หรือ Smtp Mail</param>
        /// <param name="port">Port ที่ต้องการใช้สำหรับการส่ง EMail</param>
        /// <param name="subject">ชื่อ Subject หรือชื่อหัวข้อ</param>
        /// <param name="text">ข้อความที่ต้องการส่ง สามารถส่งเป็นแบบ ภาษา Html ได้</param>
        /// <param name="from">ชื่อผู้ส่ง</param>
        /// <param name="to">E-mail ของคนรับ</param>
        /// <param name="displayTo">ชื่อของผู้รับที่ต้องการแสดงให้เห็น</param>
        /// <param name="attchFiles">Path ของ File ที่ต้องการแนบไปพร้อมกับ E-mail</param>
        /// <returns></returns>
        public static bool SendEmail(string smtpServer, int port, string subject, string text, string from, string to, string displayTo, params string[] attchFiles)
        {
            EmailArguments emailArgs = new EmailArguments()
            {
                SMTPServer = smtpServer,
                Port = port,
                HasPassword = false,
                Username = string.Empty,
                Password = string.Empty,
                Subject = subject,
                MailBody = text,
                SenderEmail = from,
                SenderName = null,
            };
            emailArgs.To.Add(to, displayTo);
            if (attchFiles != null && attchFiles.Any())
            {
                emailArgs.AddAttachment(attchFiles);
            }
            return SendEmail(emailArgs);
        }

        /// <summary>
        /// ส่ง E-mail ถ้าส่งไม่สำเร็จ จะเก็บเอาไว้ที่ StartUpPath\\MailBox ของเครื่อง Client
        /// </summary>
        /// <param name="smtpServer">ชื่อของ Mail Server หรือ Smtp Mail</param>
        /// <param name="subject">ชื่อ Subject หรือชื่อหัวข้อ</param>
        /// <param name="text">ข้อความที่ต้องการส่ง สามารถส่งเป็นแบบ ภาษา Html ได้</param>
        /// <param name="from">ชื่อผู้ส่ง</param>
        /// <param name="to">E-mail ของคนรับ</param>
        /// <param name="displayTo">ชื่อที่จะแสดงของคนรับ</param>
        /// <param name="attchFiles">Path ของ File ที่ต้องการแนบไปพร้อมกับ E-mail</param>
        /// <returns></returns>
        public static bool SendEmail(string smtpServer, string subject, string text, string from, string to, string displayTo, params string[] attchFiles)
        {
            EmailArguments emailArgs = new EmailArguments()
            {
                SMTPServer = smtpServer,
                Port = 25,
                HasPassword = false,
                Username = string.Empty,
                Password = string.Empty,
                Subject = subject,
                MailBody = text,
                SenderEmail = from,
                SenderName = null
            };
            if (attchFiles != null && attchFiles.Any())
            {
                emailArgs.AddAttachment(attchFiles);
            }
            emailArgs.To.Add(to, displayTo);
            return SendEmail(emailArgs);
        }

        /// <summary>
        /// ส่ง E-mail ถ้าส่งไม่สำเร็จ จะเก็บเอาไว้ที่ StartUpPath\\MailBox ของเครื่อง Client
        /// </summary>
        /// <param name="emailArgs">Email Arguments</param>
        /// <returns>True if success, otherwise return False.</returns>
        public static bool SendEmail(EmailArguments emailArgs)
        {
            if (emailArgs.To == null)
            {
                return false;
            }
            if (emailArgs.SenderName == null || string.IsNullOrEmpty(emailArgs.SenderName))
            {
                emailArgs.SenderName = emailArgs.SenderEmail;
            }
            try
            {
                System.Net.Mail.MailMessage msg = CreateMailMessage(emailArgs.MailBody, emailArgs.MailBodyText);

                msg.Priority = emailArgs.Priority;
                msg.HeadersEncoding = Encoding.UTF8;
                msg.SubjectEncoding = Encoding.UTF8;
                msg.BodyEncoding = Encoding.UTF8;
                msg.IsBodyHtml = true;

                msg.From = emailArgs.GetFromMailAddress();
                msg.Subject = emailArgs.Subject;
                msg.Body = emailArgs.MailBody;

                // Add To
                if (emailArgs.To == null || emailArgs.To.Count == 0)
                {
                    throw new ArgumentNullException("To Property cannot be null.");
                }
                else
                {
                    foreach (MailAddress to in emailArgs.To)
                    {
                        msg.To.Add(to);
                    }
                }

                // Add Cc
                if (emailArgs.CC != null && emailArgs.CC.Count > 0)
                {
                    foreach (MailAddress cc in emailArgs.CC)
                    {
                        msg.CC.Add(cc);
                    }
                }

                // Add Bcc
                if (emailArgs.BCC != null && emailArgs.BCC.Count > 0)
                {
                    foreach (MailAddress bcc in emailArgs.BCC)
                    {
                        msg.Bcc.Add(bcc);
                    }
                }

                // Add Reply List
                if (emailArgs.ReplyTo != null && emailArgs.ReplyTo.Count > 0)
                {
                    foreach (MailAddress replyTo in emailArgs.ReplyTo)
                    {
                        msg.ReplyToList.Add(replyTo);
                    }
                }

                // Add Attachment Files.
                if (emailArgs.HaveAttachments)
                {
                    foreach (Attachment att in emailArgs.Attachments)
                    {
                        msg.Attachments.Add(att);
                    }
                }

                SmtpClient smtp = new SmtpClient();
                //sm.DeliveryMethod = SmtpDeliveryMethod.Network;                
                if (!string.IsNullOrEmpty(emailArgs.Username) || !string.IsNullOrEmpty(emailArgs.Password))
                {
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential(emailArgs.Username, emailArgs.Password);
                }

                // Send Complete callback.
                smtp.SendCompleted += (sender, e) =>
                {
                    msg.Dispose();
                    if (null != emailArgs.SendCompletedCallback)
                        emailArgs.SendCompletedCallback();
                };

                smtp.Host = emailArgs.SMTPServer;// "mail.prosoft.co.th";
                smtp.Port = emailArgs.Port;
                if (emailArgs.IsAsync)
                {
                    // Create sending GUID for make it unique.
                    System.Guid sendGuid = System.Guid.NewGuid();
                    smtp.SendAsync(msg, sendGuid);
                }
                else
                {
                    smtp.Send(msg);
                }
                return true;
            }
            catch (Exception er)// เมื่อทำการส่ง ไม่สำเร็จจะ write เป็น Text File เก็บเอาไว้ในเครื่อง Client
            {
                if (emailArgs.SendFailedCallback != null)
                    emailArgs.SendFailedCallback();

                //SendEmailToOutBox(emailArgs);
                throw er;
            }
        }


        /// <summary>
        /// ส่ง E-mail ถ้าส่งไม่สำเร็จ จะเก็บเอาไว้ที่ StartUpPath\\MailBox ของเครื่อง Client
        /// </summary>
        /// <param name="smtpServer">ชื่อของ Mail Server หรือ Smtp Mail</param>
        /// <param name="port">Port ที่ต้องการใช้สำหรับการส่ง EMail</param>
        /// <param name="subject">ชื่อ Subject หรือชื่อหัวข้อ</param>
        /// <param name="text">ข้อความที่ต้องการส่ง สามารถส่งเป็นแบบ ภาษา Html ได้</param>
        /// <param name="from">ชื่อผู้ส่ง</param>
        /// <param name="displayFrom"></param>
        /// <param name="to">E-mail ของคนรับ</param>
        /// <param name="displayTo">ชื่อของผู้รับที่ต้องการแสดงให้เห็น</param>
        /// <param name="cc">E-mail ของคนที่ต้องการ cc ไปถึง</param>
        /// <param name="displayCc">ชื่อของผู้ CC ที่ต้องการแสดงให้เห็น</param>
        /// <param name="fileNames">ชื่อไฟล์ที่ต้องการจะแสดงเป็นชื่อไฟล์ Attachments</param>
        /// <param name="contentStreams">Stream ของไฟล์ที่จะใช้เป็น Attachments</param>
        /// <returns></returns>       
        public static bool SendEmailWithAttachments(EmailArguments emailArgs)
        {
            if (emailArgs.To == null)
            {
                return false;
            }
            if (emailArgs.SenderName == null || string.IsNullOrEmpty(emailArgs.SenderName))
            {
                emailArgs.SenderName = emailArgs.SenderEmail;
            }
            try
            {
                System.Guid sendGuid = System.Guid.NewGuid();

                System.Net.Mail.MailMessage msg = CreateMailMessage(emailArgs.MailBody, emailArgs.MailBodyText);

                msg.Priority = emailArgs.Priority;
                msg.HeadersEncoding = Encoding.UTF8;
                msg.SubjectEncoding = Encoding.UTF8;
                msg.BodyEncoding = Encoding.UTF8;
                msg.IsBodyHtml = true;

                msg.From = emailArgs.GetFromMailAddress();
                msg.Sender = emailArgs.GetSenderMailAddress();
                msg.Subject = emailArgs.Subject;
                msg.Body = emailArgs.MailBody;

                msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess | DeliveryNotificationOptions.OnFailure;

                // Add To
                for (int index = 0; index < emailArgs.To.Count; index++)
                {
                    string toMail = string.Empty;
                    toMail = emailArgs.To[index].Address;
                    string displayToMail = string.Empty;
                    if (!string.IsNullOrEmpty(toMail))
                    {
                        if (string.IsNullOrEmpty(emailArgs.To[index].DisplayName))
                        {
                            displayToMail = toMail;
                        }
                        else
                        {
                            displayToMail = emailArgs.To[index].DisplayName;
                        }
                        msg.To.Add(new MailAddress(toMail, displayToMail));
                    }
                }
                // Add Cc
                if (emailArgs.CC != null)
                {
                    for (int index = 0; index < emailArgs.CC.Count; index++)
                    {
                        string ccMail = emailArgs.CC[index].Address;
                        string displayCcMail = string.Empty;
                        if (!string.IsNullOrEmpty(ccMail))
                        {
                            if (string.IsNullOrEmpty(emailArgs.CC[index].DisplayName))
                            {
                                displayCcMail = ccMail;
                            }
                            else
                            {
                                displayCcMail = emailArgs.CC[index].DisplayName;
                            }
                            msg.CC.Add(new MailAddress(ccMail, displayCcMail));
                        }
                    }
                }
                // Add Bcc
                if (emailArgs.BCC != null)
                {
                    for (int index = 0; index < emailArgs.BCC.Count; index++)
                    {
                        string bccMail = emailArgs.BCC[index].Address;
                        string displayBccMail = string.Empty;
                        if (!string.IsNullOrEmpty(bccMail))
                        {
                            if (string.IsNullOrEmpty(emailArgs.BCC[index].DisplayName))
                            {
                                displayBccMail = bccMail;
                            }
                            else
                            {
                                displayBccMail = emailArgs.BCC[index].DisplayName;
                            }
                            msg.Bcc.Add(new MailAddress(bccMail, displayBccMail));
                        }
                    }
                }
                if (emailArgs.ReplyTo != null)
                {
                    for (int index = 0; index < emailArgs.ReplyTo.Count; index++)
                    {
                        string replyToMail = emailArgs.ReplyTo[index].Address;
                        string displayReplyToMail = string.Empty;
                        if (!string.IsNullOrEmpty(replyToMail))
                        {
                            if (string.IsNullOrEmpty(emailArgs.ReplyTo[index].DisplayName))
                            {
                                displayReplyToMail = replyToMail;
                            }
                            else
                            {
                                displayReplyToMail = emailArgs.ReplyTo[index].DisplayName;
                            }
                            msg.ReplyToList.Add(new MailAddress(replyToMail, displayReplyToMail));
                        }
                    }
                }

                // List-Unsubscribe
                //if (emailArgs.ListUnsubscribes != null && emailArgs.ListUnsubscribes.Any())
                //{
                //    msg.Headers.Add("List-Unsubscribe", String.Join(", ", emailArgs.ListUnsubscribes.Select(it => "<" + it + ">")));
                //}

                // Include all attachments.
                foreach (Attachment attachment in emailArgs.Attachments)
                {
                    msg.Attachments.Add(attachment);
                }

                foreach (var entry in emailArgs.Headers)
                {
                    if (msg.Headers.AllKeys.Contains(entry.Key))
                    {
                        msg.Headers[entry.Key] = entry.Value;
                    }
                    else
                    {
                        msg.Headers.Add(entry.Key, entry.Value);
                    }
                }

                SmtpClient smtp = new SmtpClient(emailArgs.SMTPServer, emailArgs.Port);

                //smtp.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;

                if (!string.IsNullOrEmpty(emailArgs.Username) || !string.IsNullOrEmpty(emailArgs.Password))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential(emailArgs.Username, emailArgs.Password);
                }
                smtp.EnableSsl = emailArgs.EnableSsl;

                // Send Complete callback.
                smtp.SendCompleted += (sender, e) =>
                {
                    msg.Dispose();
                    if (null != emailArgs.SendCompletedCallback)
                        emailArgs.SendCompletedCallback();
                };

                smtp.Timeout = 60000;
                if (emailArgs.IsAsync)
                {
                    smtp.SendAsync(msg, sendGuid);
                }
                else
                {
                    smtp.Send(msg);
                }
                return true;
            }
            catch (Exception ex)// เมื่อทำการส่ง ไม่สำเร็จจะ write เป็น Text File เก็บเอาไว้ในเครื่อง Client
            {
                if (emailArgs.SendFailedCallback != null)
                    emailArgs.SendFailedCallback();

                SendEmailToOutBoxWithAttachments(emailArgs);
                throw ex;
            }
        }
        #endregion

        #region AddAttachmentFiles
        private static void AddAttachmentFiles(string[] attchFiles, System.Net.Mail.MailMessage msg)
        {
            if (attchFiles != null && attchFiles.Length > 0)
            {
                foreach (string path in attchFiles)
                {
                    if (!string.IsNullOrEmpty(path))
                    {
                        Attachment att = new Attachment(path);
                        att.NameEncoding = Encoding.UTF8;
                        msg.Attachments.Add(att);
                    }
                }
            }
        }
        #endregion

        #region AddAttachmentStream
        private static void AddAttachmentStream(Stream[] streams, MailMessage msg)
        {
            if (streams != null && streams.Any())
            {
                foreach (Stream stream in streams)
                {
                    if (stream != null)
                    {
                        Attachment att = new Attachment(stream, "");
                        att.NameEncoding = Encoding.UTF8;
                        msg.Attachments.Add(att);
                    }
                }
            }
        }
        #endregion

        #region SendEmailToOutBox
        /// <summary>
        /// ส่ง Mail ที่มีปัญหาส่งออกไปไม่ได้มาพักเอาไว้ที่ Application.StartupPath + "\\Mail_OutBox" เพื่อรอการส่งต่อไปครั้งหน้า
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="text"></param>
        /// <param name="from"></param>
        /// <param name="displayFrom"></param>
        /// <param name="to"></param>
        /// <param name="displayTo"></param>
        public static void SendEmailToOutBoxWithAttachments(EmailArguments emailArgs)
        {
            //SendEmailToOutBoxWithAttachments(emailArgs);
        }

        /// <summary>
        /// ส่ง Mail ที่มีปัญหาส่งออกไปไม่ได้มาพักเอาไว้ที่ Application.StartupPath + "\\Mail_OutBox" เพื่อรอการส่งต่อไปครั้งหน้า
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="text"></param>
        /// <param name="from"></param>
        /// <param name="displayFrom"></param>
        /// <param name="to"></param>
        /// <param name="displayTo"></param>
        /// <param name="cc"></param>
        /// <param name="displayCc"></param>
        /// <param name="attchFiles"></param>
        public static void SendEmailToOutBox(EmailArguments emailArgs)
        {
            //try
            //{
            //    DirectoryInfo directory = null;
            //    try
            //    {
            //        directory = new DirectoryInfo(Application.StartupPath + "\\MailBox\\OutBox");
            //        if (!directory.Exists)
            //        {
            //            directory = Directory.CreateDirectory(Application.StartupPath + "\\MailBox\\OutBox");
            //        }
            //    }
            //    catch
            //    {
            //        directory = Directory.CreateDirectory(Application.StartupPath + "\\MailBox\\OutBox");
            //    }
            //    string time = DateTimeHelper.Now.ToString("dd/mm/yyyy hh:mm:ss");// DateTimeHelper.Now.Year.ToString() + DateTimeHelper.Now.Month.ToString() + DateTimeHelper.Now.Day.ToString() + "_" + DateTimeHelper.Now.Hour.ToString() + DateTimeHelper.Now.Minute.ToString() + DateTimeHelper.Now.Second.ToString();
            //    string attchStr = string.Empty;
            //    string toMail = string.Empty;
            //    string toDisplay = string.Empty;
            //    string ccMail = string.Empty;
            //    string ccDisplay = string.Empty;
            //    if (to != null)
            //    {
            //        for (int index = 0; index < to.Length; index++)
            //        {
            //            toMail += to[index] + ",";
            //            if (displayTo == null || string.IsNullOrEmpty(displayTo[index]))
            //            {
            //                toDisplay += to[index] + ",";
            //            }
            //            else
            //            {
            //                toDisplay += displayTo[index] + ",";
            //            }
            //        }
            //    }
            //    if (toMail.Length > 0 && toMail[toMail.Length - 1] == ',')
            //    {
            //        toMail = toMail.Remove(toMail.Length - 1, 1);
            //    }
            //    if (toDisplay.Length > 0 && toDisplay[toDisplay.Length - 1] == ',')
            //    {
            //        toDisplay = toDisplay.Remove(toDisplay.Length - 1, 1);
            //    }
            //    //
            //    if (cc != null)
            //    {
            //        for (int index = 0; index < cc.Length; index++)
            //        {
            //            if (!string.IsNullOrEmpty(cc[index]))
            //            {
            //                ccMail += to[index] + ",";
            //                if (displayCc == null || string.IsNullOrEmpty(displayCc[index]))
            //                {
            //                    ccDisplay += cc[index] + ",";
            //                }
            //                else
            //                {
            //                    ccDisplay += displayCc[index] + ",";
            //                }
            //            }
            //        }
            //    }
            //    if (ccMail.Length > 0 && ccMail[ccMail.Length - 1] == ',')
            //    {
            //        ccMail = ccMail.Remove(ccMail.Length - 1, 1);
            //    }
            //    if (ccDisplay.Length > 0 && ccDisplay[ccDisplay.Length - 1] == ',')
            //    {
            //        ccDisplay = ccDisplay.Remove(ccDisplay.Length - 1, 1);
            //    }
            //    //
            //    if (attchFiles != null)
            //    {
            //        foreach (string attch in attchFiles)
            //        {
            //            attchStr += attch + ",";
            //        }
            //    }
            //    if (attchStr.Length > 0 && attchStr[attchStr.Length - 1] == ',')
            //    {
            //        attchStr = attchStr.Remove(attchStr.Length - 1, 1);
            //    }
            //    string fileName = subject.Replace(":", "");
            //    fileName = fileName.Replace("/", "");
            //    fileName = fileName.Replace("\\", "");
            //    fileName = fileName.Replace("*", "");
            //    fileName = fileName.Replace("?", "");
            //    fileName = fileName.Replace("|", "");
            //    fileName = fileName.Replace("<", "");
            //    fileName = fileName.Replace(">", "");
            //    fileName = fileName.Replace("\"", "");
            //    fileName = fileName.Replace(" ", "");
            //    StreamWriter sw = new StreamWriter(directory.FullName + "\\" + fileName + ".txt");
            //    sw.WriteLine(from + "|" + displayFrom + "|" + toMail + "|" + toDisplay + "|" + ccMail + "|" + ccDisplay + "|" + subject + "|" + attchStr + "|" + time);
            //    sw.Write(text);
            //    sw.Flush();
            //    sw.Close();
            //}
            //catch
            //{

            //}
        }


        public static void SendEmailToOutBoxWithAttachments(string subject, string text, string from, string displayFrom, string[] to, string[] displayTo, string[] cc, string[] displayCc, Attachment[] attachments)
        {
            //try
            //{
            //    DirectoryInfo directory = null;
            //    try
            //    {
            //        directory = new DirectoryInfo(Application.StartupPath + "\\MailBox\\OutBox");
            //        if (!directory.Exists)
            //        {
            //            directory = Directory.CreateDirectory(Application.StartupPath + "\\MailBox\\OutBox");
            //        }
            //    }
            //    catch
            //    {
            //        directory = Directory.CreateDirectory(Application.StartupPath + "\\MailBox\\OutBox");
            //    }
            //    string time = DateTimeHelper.Now.ToString("dd/mm/yyyy hh:mm:ss");// DateTimeHelper.Now.Year.ToString() + DateTimeHelper.Now.Month.ToString() + DateTimeHelper.Now.Day.ToString() + "_" + DateTimeHelper.Now.Hour.ToString() + DateTimeHelper.Now.Minute.ToString() + DateTimeHelper.Now.Second.ToString();
            //    string attchStr = string.Empty;
            //    string toMail = string.Empty;
            //    string toDisplay = string.Empty;
            //    string ccMail = string.Empty;
            //    string ccDisplay = string.Empty;
            //    if (to != null)
            //    {
            //        for (int index = 0; index < to.Length; index++)
            //        {
            //            toMail += to[index] + ",";
            //            if (displayTo == null || string.IsNullOrEmpty(displayTo[index]))
            //            {
            //                toDisplay += to[index] + ",";
            //            }
            //            else
            //            {
            //                toDisplay += displayTo[index] + ",";
            //            }
            //        }
            //    }
            //    if (toMail.Length > 0 && toMail[toMail.Length - 1] == ',')
            //    {
            //        toMail = toMail.Remove(toMail.Length - 1, 1);
            //    }
            //    if (toDisplay.Length > 0 && toDisplay[toDisplay.Length - 1] == ',')
            //    {
            //        toDisplay = toDisplay.Remove(toDisplay.Length - 1, 1);
            //    }
            //    //
            //    if (cc != null)
            //    {
            //        for (int index = 0; index < cc.Length; index++)
            //        {
            //            if (!string.IsNullOrEmpty(cc[index]))
            //            {
            //                ccMail += to[index] + ",";
            //                if (displayCc == null || string.IsNullOrEmpty(displayCc[index]))
            //                {
            //                    ccDisplay += cc[index] + ",";
            //                }
            //                else
            //                {
            //                    ccDisplay += displayCc[index] + ",";
            //                }
            //            }
            //        }
            //    }
            //    if (ccMail.Length > 0 && ccMail[ccMail.Length - 1] == ',')
            //    {
            //        ccMail = ccMail.Remove(ccMail.Length - 1, 1);
            //    }
            //    if (ccDisplay.Length > 0 && ccDisplay[ccDisplay.Length - 1] == ',')
            //    {
            //        ccDisplay = ccDisplay.Remove(ccDisplay.Length - 1, 1);
            //    }
            //    //
            //    if (attchFiles != null)
            //    {
            //        foreach (string attch in attchFiles)
            //        {
            //            attchStr += attch + ",";
            //        }
            //    }
            //    if (attchStr.Length > 0 && attchStr[attchStr.Length - 1] == ',')
            //    {
            //        attchStr = attchStr.Remove(attchStr.Length - 1, 1);
            //    }
            //    string fileName = subject.Replace(":", "");
            //    fileName = fileName.Replace("/", "");
            //    fileName = fileName.Replace("\\", "");
            //    fileName = fileName.Replace("*", "");
            //    fileName = fileName.Replace("?", "");
            //    fileName = fileName.Replace("|", "");
            //    fileName = fileName.Replace("<", "");
            //    fileName = fileName.Replace(">", "");
            //    fileName = fileName.Replace("\"", "");
            //    fileName = fileName.Replace(" ", "");
            //    StreamWriter sw = new StreamWriter(directory.FullName + "\\" + fileName + ".txt");
            //    sw.WriteLine(from + "|" + displayFrom + "|" + toMail + "|" + toDisplay + "|" + ccMail + "|" + ccDisplay + "|" + subject + "|" + attchStr + "|" + time);
            //    sw.Write(text);
            //    sw.Flush();
            //    sw.Close();
            //}
            //catch
            //{

            //}
        }
        #endregion

        #region SendAllEmailFromOutBox
        /// <summary>
        /// ส่ง Mail ที่ยังค้างอยู่ใน Folder MailBox ของ StartUp Path
        /// </summary>
        /// <param name="smtpServer">Smtp Server</param>
        /// <param name="port">Port ที่ต้องการใช้ในการส่ง</param>
        public static void SendAllEmailFromOutBox(string smtpServer, int port)
        {
            //SendEmail(smtpServer, port, false, "", "", "", "", GetAllEmailFormOutBox());
        }

        /// <summary>
        /// ส่ง Mail ที่ยังค้างอยู่ใน Folder MailBox ของ StartUp Path
        /// </summary>
        /// <param name="smtpServer">Smtp Server</param>
        /// <param name="port">Port ที่ต้องการใช้ในการส่ง</param>
        /// <param name="defaultForm">Email Address default ใช้ในกรณีที่ส่วนของ from ไม่มีชื่อจะเอา email นี้กำหนดลงไปแทน</param>
        /// <param name="defaultDisplay">คำที่ใช้สำหรับแสดงจะทำมีผลก้อต่อเมื่อ defaultForm ถูกใช้งาน</param>
        public static void SendAllEmailFromOutBox(string smtpServer, int port, string defaultForm, string defaultDisplay)
        {
            //SendEmail(smtpServer, port, false, "", "", defaultForm, defaultDisplay, GetAllEmailFormOutBox());
        }
        #endregion

        #region GetAllEmailFormOutBox
        /// <summary>
        /// หา Mail ที่ยังค้างส่ง return ออกมาเป็น Type Dataset
        /// </summary>
        /// <returns></returns>
        //public static EMailDS GetAllEmailFormOutBox()
        //{
        //    EMailDS tdsEmail = new EMailDS();
        //    DirectoryInfo directory = new DirectoryInfo(Application.StartupPath + "\\MailBox\\OutBox");
        //    if (directory.Exists)
        //    {
        //        FileInfo[] files = directory.GetFiles();
        //        if (files != null)
        //        {
        //            foreach (FileInfo file in files)
        //            {
        //                if (file.Extension.ToLower() == ".txt")
        //                {
        //                    StreamReader sr = new StreamReader(file.FullName);
        //                    string text = sr.ReadLine();
        //                    try
        //                    {
        //                        if (!string.IsNullOrEmpty(text))
        //                        {
        //                            string[] header = text.Split('|');
        //                            string from = header[0];
        //                            string displayFrom = header[1];
        //                            string[] tomail = header[2].Split(',');
        //                            string[] displayTo = header[3].Split(',');
        //                            string[] ccMail = header[4].Split(',');
        //                            string[] displayCc = header[5].Split(',');
        //                            string subject = header[6];
        //                            string time = header[7];
        //                            string[] attchFile = header[8].Split(',');
        //                            text = sr.ReadToEnd();
        //                            EMailDS.EmailListRow drEmail = tdsEmail.EmailList.NewEmailListRow();
        //                            tdsEmail.EmailList.Rows.Add(drEmail);
        //                            drEmail.MailID = Guid.NewGuid().ToString();
        //                            drEmail.From = from;
        //                            drEmail.DisplayFrom = displayFrom;
        //                            //drEmail.ToMail = header[2];
        //                            //drEmail.DisplayTo = header[3];
        //                            //drEmail.CcMail = header[4];
        //                            //drEmail.DisplayCc = header[5];
        //                            drEmail.Subject = subject;
        //                            drEmail.Text = text;
        //                            drEmail.Path = file.FullName;
        //                            try
        //                            {
        //                                drEmail.CreatedDate = DateTime.Parse(time);
        //                            }
        //                            catch { }
        //                            for (int index = 0; index < tomail.Length; index++)
        //                            {
        //                                string mail = tomail[index];
        //                                if (!string.IsNullOrEmpty(mail))
        //                                {
        //                                    EMailDS.ToMailRow drToMail = tdsEmail.ToMail.NewToMailRow();
        //                                    tdsEmail.ToMail.Rows.Add(drToMail);
        //                                    drToMail.Mail = mail;
        //                                    drToMail.MailID = drEmail.MailID;
        //                                    if (displayTo.Length > index) drToMail.Display = displayTo[index];
        //                                }
        //                            }
        //                            for (int index = 0; index < ccMail.Length; index++)
        //                            {
        //                                string mail = ccMail[index];
        //                                if (!string.IsNullOrEmpty(mail))
        //                                {
        //                                    EMailDS.CcMailRow drCcMail = tdsEmail.CcMail.NewCcMailRow();
        //                                    tdsEmail.CcMail.Rows.Add(drCcMail);
        //                                    drCcMail.Mail = mail;
        //                                    drCcMail.MailID = drEmail.MailID;
        //                                    if (displayCc.Length > index) drCcMail.Display = displayCc[index];
        //                                }
        //                            }
        //                        }
        //                    }
        //                    catch { }
        //                    finally
        //                    {
        //                        sr.Close();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return tdsEmail;
        //}
        #endregion

        #region CreateMailMessage
        /// <summary>
        /// Create and prepare mail message object before sending mail.
        /// </summary>
        /// <param name="htmlContent">HTML Content</param>
        /// <param name="textContent">Text Content</param>
        /// <returns>Prepared Mail Message</returns>
        private static MailMessage CreateMailMessage(string htmlContent, string textContent)
        {
            MailMessage msg = new MailMessage();
            // If not assign Text Content, use html content and stript tags.
            AlternateView textView = String.IsNullOrEmpty(textContent) ? GetPlainTextView(htmlContent) : AlternateView.CreateAlternateViewFromString(textContent, null, "text/plain");

            // Create HTML View and parse Data URI Scheme link into linked resources.
            AlternateView htmlView = GetHTMLView(htmlContent, msg);

            // Add both text and html view.
            msg.AlternateViews.Add(textView);
            msg.AlternateViews.Add(htmlView);

            // return it.
            return msg;
        }
        #endregion

        #region GetPlainTextView
        /// <summary>
        /// Get Plain Text View.
        /// </summary>
        /// <param name="content">Email Content</param>
        /// <returns>Alternate View that contains plain text.</returns>
        private static AlternateView GetPlainTextView(string content)
        {
            string text = Regex.Replace(content, "<.*?>", string.Empty);
            text = System.Web.HttpUtility.HtmlDecode(text);
            return AlternateView.CreateAlternateViewFromString(text, null, "text/plain");
        }
        #endregion

        #region GetHTMLView
        /// <summary>
        /// Get HTML Alternate View.
        /// </summary>
        /// <param name="content">Email Content in HTML Format.</param>
        /// <param name="msg">MailMessage Object for assign default html content.</param>
        /// <returns>Alternate View in HTML format.</returns>
        private static AlternateView GetHTMLView(string content, MailMessage msg)
        {
            Dictionary<string, LinkedResource> resources = GetLinkedResourcesInContent(content);

            List<LinkedResource> existsLinkedResources = new List<LinkedResource>();
            if (resources != null)
            {
                foreach (var resource in resources.Where(it => it.Value != null))
                {
                    content = content.Replace(resource.Key, "cid:" + resource.Value.ContentId);
                    existsLinkedResources.Add(resource.Value);
                }
            }
            msg.Body = content;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(content, null, "text/html");
            foreach (LinkedResource res in existsLinkedResources)
            {
                htmlView.LinkedResources.Add(res);
            }
            return htmlView;
        }
        #endregion

        #region GetMatchesDataURIScheme
        /// <summary>
        /// Parse inline image (Data URI Scheme) in mail message and convert them to attachment, then replace its with linked resource images.
        /// </summary>
        /// <param name="content">Mail message that you want to convert Data URI Scheme into linked resource images</param>
        /// <param name="isDistinct">Do you want to keep only unique results?</param>
        /// <returns>Data URI Scheme Links.</returns>
        public static string[] GetMatchesDataURIScheme(string content, bool isDistinct = false)
        {
            Regex reg = new Regex("(data:image(.*?)(?=')|data:image(.*?)(?=\"))");
            MatchCollection matchDataURISchemes = reg.Matches(content);
            string[] matches = null;
            if (matchDataURISchemes.Count > 0)
            {
                matches = new string[matchDataURISchemes.Count];
                for (int i = 0; i < matchDataURISchemes.Count; i++)
                {
                    matches[i] = matchDataURISchemes[i].Value;
                }
                if (isDistinct)
                {
                    matches = matches.Distinct().ToArray();
                }
            }
            else
            {
                matches = new string[0];
            }
            return matches;
        }
        #endregion

        #region GetLinkedResourcesInContent
        /// <summary>
        /// Get Linked Resources Dictionary in HTML Content.
        /// </summary>
        /// <param name="content">Email Content in HTML Format</param>
        /// <returns>Dictionary that have Data URI Scheme as Key, and have LinkedResource object as value.</returns>
        public static Dictionary<string, LinkedResource> GetLinkedResourcesInContent(string content)
        {
            string[] links = GetMatchesDataURIScheme(content, true);
            Dictionary<string, LinkedResource> dic = new Dictionary<string, LinkedResource>();
            foreach (string link in links)
            {
                dic.Add(link, GetLinkedResourceFromDataURIScheme(link));
            }
            return dic;
        }
        #endregion

        #region GetLinkedResourceFromDataURIScheme
        /// <summary>
        /// Get LinkedResource by Data URI Scheme Link.
        /// </summary>
        /// <param name="link">Data URI Scheme Link.</param>
        /// <returns>LinkedResource that decoded from base64 string in specific Data URI Scheme.</returns>
        private static LinkedResource GetLinkedResourceFromDataURIScheme(string link)
        {
            LinkedResource res = null;
            string contentType = string.Empty;
            Regex rex = new Regex("(?<=(data:))(.*?)(?=;)");
            Match match = rex.Match(link);
            if (match.Success)
            {
                // Get content type.
                contentType = match.Value;

                Regex rexContent = new Regex("(?<=[,]).*");
                Match matchContent = rexContent.Match(link);
                if (matchContent.Success)
                {
                    byte[] contentBytes = Convert.FromBase64String(matchContent.Value);
                    MemoryStream ms = new MemoryStream(contentBytes);
                    res = new LinkedResource(ms, contentType);
                    res.TransferEncoding = TransferEncoding.Base64;
                }
            }

            return res;
        }
        #endregion

        #region IsValidEmail
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            else
            {
                //Regex emailPattern = new Regex("([_A-Za-z]+[_A-Za-z0-9\\.]*@[A-Za-z0-9]+[A-Za-z0-9-\\.]*[.]([A-Za-z]{3}|[A-Za-z]{2}[.][A-Za-z]{2}))");
                //return Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z");
                return Regex.IsMatch(email, @"([_A-Za-z]+[_A-Za-z0-9\\.]*@[A-Za-z0-9]+[A-Za-z0-9-\\.]*[.]([A-Za-z]{2,3}|[A-Za-z]{2}[.][A-Za-z]{2}))");
            }
        }
        #endregion

        #region ParseContactList
        /// <summary>
        /// Convert contact list text into MailAddress Objects.
        /// </summary>
        /// <param name="contactListStr">Contact list in string format.</param>
        /// <returns>MailAddress objects which parsed ffrom contact list string.</returns>
        public static Dictionary<string, string> ParseContactList(string contactListStr)
        {
            Dictionary<string, string> contactCollection = new Dictionary<string, string>();

            // Split contact list.
            string[] contactListStrArray = contactListStr.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
            string name = string.Empty;
            string email = string.Empty;
            Regex emailPattern = new Regex("([_A-Za-z]+[_A-Za-z0-9\\.]*@[A-Za-z0-9]+[A-Za-z0-9-\\.]*[.]([A-Za-z]{3}|[A-Za-z]{2}[.][A-Za-z]{2}))");
            //Regex emailPattern = new Regex(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z");
            Match emailMatch = null;
            Regex namePattern = new Regex("[A-Za-z0-9\\s]*(?=<)");
            Match nameMatch = null;
            foreach (string contactElement in contactListStrArray)
            {
                string element = contactElement.Trim();
                if (string.IsNullOrEmpty(element))
                    continue;

                emailMatch = emailPattern.Match(element);
                if (emailMatch.Success)
                {
                    email = emailMatch.Value.Trim();
                }
                else
                {
                    continue;
                }

                nameMatch = namePattern.Match(element);
                if (nameMatch.Success)
                {
                    name = nameMatch.Value.Trim();
                }
                else
                {
                    name = email;
                }
                contactCollection.Add(email, name);
            }

            return contactCollection;
        }

        public static void ParseContactList(string contactListStr, NationalMailAddressCollection list)
        {
            Dictionary<string, string> contactList = ParseContactList(contactListStr);
            foreach (var c in contactList)
            {
                list.Add(c.Key, c.Value);
            }
        }
        #endregion
    }
    #endregion
}