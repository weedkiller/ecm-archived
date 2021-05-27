using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using DansLesGolfs.ECM.Models;
using DansLesGolfs.Base;
using System.Runtime.Caching;
using System.Globalization;
using System.Text.RegularExpressions;
using Limilabs.Mail;
using Limilabs.Mail.Tools;
using Limilabs.Client.IMAP;
using System.Threading.Tasks;
using Limilabs.FTP.Client;
using System.IO;
using System.Xml;

namespace DansLesGolfs.ECM.Controllers
{
    public class TrackController : BaseController
    {
        public ActionResult Open(string id)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(id))
                {
                    string decodedId = DataProtection.Decrypt(id);
                    if (decodedId.Contains('|'))
                    {
                        long campaignId = 0, emailQueId = 0;
                        string[] parts = decodedId.Split('|');
                        campaignId = DataManager.ToLong(parts[0]);
                        emailQueId = DataManager.ToLong(parts[1]);
                        DataAccess.SetOpenMail(campaignId, emailQueId);
                        AddEmailTracking(campaignId, emailQueId, "open");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return File(new byte[0], "image/png");
        }

        public ActionResult Click(string id, string link)
        {
            if (!String.IsNullOrWhiteSpace(id))
            {
                try
                {
                    string decodedId = DataProtection.Decrypt(id);
                    if (decodedId.Contains('|'))
                    {
                        long campaignId = 0, emailQueId = 0;
                        string[] parts = decodedId.Split('|');
                        campaignId = DataManager.ToLong(parts[0]);
                        emailQueId = DataManager.ToLong(parts[1]);
                        DataAccess.SetMailClickLink(campaignId, emailQueId);

                        // Prepare link.
                        if (!link.StartsWith("http://") && !link.StartsWith("https://"))
                        {
                            link = "http://" + link;
                        }

                        AddEmailTracking(campaignId, emailQueId, "click", link);

                        #region Unsubscribe Tracking
                        string unsubscribeLink = string.Empty;
                        if (MemoryCache.Default["UnsubscribeLink"] == null)
                        {
                            unsubscribeLink = System.Configuration.ConfigurationManager.AppSettings["UnsubscribeLink"];
                            MemoryCache.Default.Add("UnsubscribeLink", unsubscribeLink, DateTime.Now.AddMinutes(5));
                        }
                        else
                        {
                            unsubscribeLink = MemoryCache.Default["UnsubscribeLink"].ToString();
                        }
                        #endregion

                        return Redirect(link);
                    }
                    else if(!String.IsNullOrEmpty(link))
                    {

                        // Prepare link.
                        if (!link.StartsWith("http://") && !link.StartsWith("https://"))
                        {
                            link = "http://" + link;
                        }
                        return Redirect(link);
                    }
                    else
                    {
                        return Redirect(Url.Content("~/"));
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    return Redirect(Url.Content("~/"));
                }
            }
            else
            {
                return Redirect(Url.Content("~/"));
            }
        }

        public ActionResult Unsubscribe()
        {
            try
            {
                List<Task> tasks = new List<Task>();
                string server = string.Empty;
                string username = string.Empty;
                string password = string.Empty;
                int port = 25;
                bool enableSsl = false;

                var options = DataAccess.GetOptions("UnsubscribeServer", "UnsubscribeEmail", "UnsubscribePassword", "UnsubscribePort", "UnsubscribeUseSSL");
                if (options != null && options.Any())
                {
                    server = options["UnsubscribeServer"];
                    username = options["UnsubscribeEmail"];
                    password = options["UnsubscribePassword"];
                    port = DataManager.ToInt(options["UnsubscribePort"]);
                    enableSsl = DataManager.ToBoolean(options["UnsubscribeUseSSL"]);

                    tasks.Add(new Task(() => { CheckUnsubscribeEmails(server, username, password, port, enableSsl); }));
                }

                tasks.ForEach(task => task.Start());
                return Content("Updated.");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return Content("Failed.");
            }
        }

        private void CheckUnsubscribeEmails(string server, string username, string password, int port, bool enableSsl)
        {
            try
            {
                using (Imap imap = new Imap())
                {
                    imap.Connect(server);
                    imap.Login(username, password);

                    if (!imap.Connected)
                        throw new Exception("Can't login to Mail Server, please check your setting");

                    imap.SelectInbox();

                    foreach (long uid in imap.Search(Flag.Unseen))
                    {
                        var eml = imap.GetMessageByUID(uid);
                        IMail email = new MailBuilder().CreateFromEml(eml);
                        string parsedAddress = email.To[0].GetMailboxes().FirstOrDefault().Address;

                        string subject = email.Subject;
                        if (subject.IndexOf("Unsubscribe|") >= 0)
                        {
                            string id = DataProtection.Decrypt(subject.Replace("Unsubscribe|", "").Replace('~', '='));
                            string[] idParts = id.Split('|');
                            if (idParts.Length == 3)
                            {
                                long campaignId = DataManager.ToLong(idParts[0]);
                                long emailQueId = DataManager.ToLong(idParts[1]);
                                long siteId = DataManager.ToLong(idParts[2]);

                                if (campaignId > 0 && emailQueId > 0)
                                {
                                    Task task = new Task(() =>
                                    {
                                        var trackingId = DataAccess.AddEmailTracking(campaignId, emailQueId, "unsubscribe", "", "", "", "");
                                        EmailQue queue = DataAccess.GetEmailQue(emailQueId);
                                        DataAccess.UnSubscribeUserEmail(queue.Email, siteId, 0, string.Empty, trackingId);
                                        DataAccess.SetMailUnsubscribe(campaignId, emailQueId);
                                    });
                                    task.Start();
                                }
                            }
                        }

                        imap.DeleteMessageByUID(uid);
                    }
                    imap.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "There are error while check unsubscribe email." + Environment.NewLine + ex.Message);
            }
        }

        public ActionResult Bounce()
        {
            try
            {
                List<Task> tasks = new List<Task>();
                string server = string.Empty;
                string username = string.Empty;
                string password = string.Empty;
                int port = 25;
                bool enableSsl = false;
                string bouncedEmail = string.Empty;
                bool useVERP = true;

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

                    tasks.Add(new Task(() => { CheckBouncedEmails(server, username, password, port, enableSsl, bouncedEmail, useVERP); }));
                }

                List<long> siteIds = DataAccess.GetManuallySMTPSiteIds();
                foreach (long siteId in siteIds)
                {
                    if (DataAccess.GetSiteSMTPSettings(siteId, out server, out username, out password, out port, out enableSsl, out useVERP, out bouncedEmail))
                    {
                        tasks.Add(new Task(() => { CheckBouncedEmails(server, username, password, port, enableSsl, bouncedEmail, useVERP); }));
                    }
                }

                tasks.ForEach(task => task.Start());

                return Content("Updated.");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return Content("Failed.");
            }
        }

        private void CheckBouncedEmails(string server, string username, string password, int port, bool enableSsl, string bouncedEmail, bool useVERP)
        {
            try
            {
                using (Imap imap = new Imap())
                {
                    imap.Connect(server);
                    imap.Login(username, password);
                    //if (useVERP)
                    //{
                    //    imap.Connect(server);
                    //    imap.Login(username, password);
                    //}
                    //else
                    //{
                    //    imap.Connect(server);
                    //    imap.Login(bouncedEmail, password);
                    //}

                    if (!imap.Connected)
                        throw new Exception("Can't login to Mail Server, please check your setting");

                    imap.SelectInbox();

                    foreach (long uid in imap.Search(Flag.Unseen))
                    {
                        var eml = imap.GetMessageByUID(uid);
                        IMail email = new MailBuilder().CreateFromEml(eml);
                        string parsedAddress = email.To[0].GetMailboxes().FirstOrDefault().Address;

                        if (useVERP)
                        {
                            VERPAddress verp = VERPAddress.Parse(parsedAddress);
                            parsedAddress = verp.ToAddress;
                        }

                        Task task = new Task(() =>
                        {
                            DataAccess.SetEmailQueBouncedByEmail(parsedAddress);
                            DataAccess.AddUserToUnsubscriberListByEmail(parsedAddress);
                        });
                        task.Start();

                        imap.DeleteMessageByUID(uid);
                    }
                    imap.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "There are error while checking bounce email." + Environment.NewLine + ex.Message);
            }
        }

        public ActionResult DownloadNetmessageReports()
        {
            try
            {
                List<Task> tasks = new List<Task>();
                List<NetMessageCampaign> campaigns = DataAccess.GetPendingNetmessageCampaigns();
                var groups = from it in campaigns
                             group it by it.EmailId into g
                             select new
                             {
                                 EmailId = g.Key,
                                 Campaigns = g.ToList()
                             };

                string username = string.Empty, password = string.Empty, accountName = string.Empty, downloadPath = Server.MapPath("~/Uploads/Netmessage/Reports");
                if (!Directory.Exists(downloadPath))
                {
                    System.IO.Directory.CreateDirectory(downloadPath);
                }
                foreach (var g in groups)
                {
                    if (DataAccess.GetNetmessageSettingByEmailId(g.EmailId.Value, out username, out password, out accountName))
                    {
                        if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
                            continue;

                        tasks.Add(new Task(() =>
                        {
                            BeginDownloadNetmessageReports(username, password, accountName, downloadPath, g.Campaigns);
                        }));
                    }
                }

                tasks.ForEach(task => task.Start());
                return Content("Downloaded.");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return Content("Failed.");
            }
        }

        private void BeginDownloadNetmessageReports(string username, string password, string accountName, string downloadPath, List<NetMessageCampaign> campaigns)
        {
            if (campaigns == null || !campaigns.Any())
                return;

            using (Ftp ftp = new Ftp())
            {
                ftp.Connect("ftp.netmessage.com");
                ftp.Login(username, password);

                foreach (var c in campaigns)
                {
                    try
                    {
                        RemoteSearchOptions options = new RemoteSearchOptions(c.RefCode + ".*", false);
                        ftp.DownloadFiles("OUT", downloadPath, options);
                        //if (System.IO.File.Exists(Path.Combine(downloadPath, c.RefCode + ".zip")))
                        //{
                        //    ftp.DeleteFiles("OUT", options);
                        //}
                    }
                    catch (Exception)
                    {
                    }
                }

                ftp.Close();
            }
        }

        public ActionResult ReadNetmessageReports()
        {
            try
            {
                string downloadPath = Server.MapPath("~/Uploads/Netmessage/Reports");
                if (!Directory.Exists(downloadPath))
                {
                    System.IO.Directory.CreateDirectory(downloadPath);
                }
                List<Task> tasks = new List<Task>();
                List<NetMessageCampaign> campaigns = DataAccess.GetPendingNetmessageCampaigns();
                var groups = from it in campaigns
                             group it by it.EmailId into g
                             select new
                             {
                                 EmailId = g.Key,
                                 Campaigns = g.ToList()
                             };

                foreach (var g in groups)
                {
                    tasks.Add(new Task(() =>
                    {
                        try
                        {
                            string subPath = Path.Combine(downloadPath, g.Campaigns.First().RefCode + ".sub");
                            if (g.Campaigns.Any() && System.IO.File.Exists(subPath))
                            {
                                string line = string.Empty, status = string.Empty;
                                NetMessageCampaign campaign = g.Campaigns.First();
                                string zipPath = Path.Combine(downloadPath, g.Campaigns.First().RefCode + ".zip");
                                string listPath = Path.Combine(downloadPath, g.Campaigns.First().RefCode);

                                string subContent = System.IO.File.ReadAllText(subPath);
                                Regex regex = new Regex("\\<status\\>(.+)\\<\\/status\\>", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                Match match = regex.Match(subContent);
                                if (match.Success && match.Groups.Count == 2)
                                {
                                    status = match.Groups[1].Value.ToLower();
                                }

                                if (status == "success") // If campaign succeed.
                                {
                                    if (Directory.Exists(listPath))
                                        @Directory.Delete(listPath);

                                    if (!System.IO.File.Exists(zipPath))
                                        throw new Exception("Can't found \"" + zipPath + "\".");

                                    System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, listPath);

                                    if (!Directory.Exists(listPath))
                                        throw new Exception("Can't found \"" + listPath + "\".");

                                    string[] listFiles = Directory.GetFiles(listPath, "*.csv");

                                    // Read each file.
                                    string[] lines = null;
                                    List<string> allLines = new List<string>();
                                    Dictionary<string, int> columns = null;
                                    foreach (string file in listFiles)
                                    {
                                        lines = System.IO.File.ReadAllLines(file);
                                        if (lines.Length > 1)
                                        {
                                            if (columns == null)
                                                columns = GetNetmessageReportColumns(lines[0]);

                                            for (int i = 1, n = lines.Length; i < n; i++)
                                            {
                                                line = lines[i].Trim();
                                                if (String.IsNullOrEmpty(line))
                                                    continue;

                                                DataAccess.AddEmailQue(GetEmailQueFromNetmessageCsvLine(line, columns, campaign));
                                            }
                                        }
                                    }
                                }
                                else // If campaign failed.
                                {
                                    g.Campaigns.ForEach(c => c.Status = -1);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                        }
                    }));
                }

                tasks.ForEach(task => task.Start());
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return Content("Readed.");
        }

        private Dictionary<string, int> GetNetmessageReportColumns(string line)
        {
            string[] columns = line.Split(';').Select(it => it.Trim().Length > 2 ? it.Remove(it.Length - 1).Remove(0) : string.Empty).ToArray();
            Dictionary<string, int> keyValueColumns = new Dictionary<string, int>();
            for (int i = 0, n = columns.Length; i < n; i++)
            {
                keyValueColumns.Add(columns[i], i);
            }
            return keyValueColumns;
        }

        private string[] GetNetmessageReportLineValues(string line)
        {
            return line.Split(';').Select(it => it.Trim().Length > 2 ? it.Remove(it.Length - 1).Remove(0) : string.Empty).ToArray();
        }

        private EmailQue GetEmailQueFromNetmessageCsvLine(string line, Dictionary<string, int> columns, NetMessageCampaign campaign)
        {
            string[] values = GetNetmessageReportLineValues(line);
            EmailQue obj = new EmailQue()
            {
                Status = 1,
                Email = values[columns["EMAIL"]],
                SendDate = DataManager.ToDateTime(values[columns["DATE_SEND"]], "yyyy-MM-dd HH:mm:ss", DateTime.Now),
                NetMessageCampaignId = campaign.NetMessageCampaignId,
                EmailId = campaign.EmailId.HasValue ? campaign.EmailId.Value : 0
            };
            return obj;
        }

        private void AddEmailTracking(long campaignId, long emailQueId, string action, string value = null)
        {
            string ip = Request.UserHostAddress;
            var browser = Request.Browser;
            string platform = WebHelper.GetUserPlatform(Request);
            string browserName = string.Format("{0} {1} / {2}", browser.Browser, browser.Version, platform);
            DataAccess.AddEmailTracking(campaignId, emailQueId, action, value, ip, browserName, platform);
        }

        public ActionResult Generate(string cid, string eid, string sid)
        {
            string id = cid + "|" + eid;
            if (!String.IsNullOrWhiteSpace(sid))
            {
                id += "|" + sid;
            }
            return Content(DataProtection.Encrypt(id), "text/html");
        }
    }
}
