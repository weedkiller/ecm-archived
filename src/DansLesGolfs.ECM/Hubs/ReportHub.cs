using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using DansLesGolfs.Data;
using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using System.IO;
using System.Net;
using System.Threading;
using Limilabs.FTP.Client;

namespace DansLesGolfs.ECM.Hubs
{
    public class ReportHub : Hub
    {
        public void GetNetmessageReport(long id)
        {
            #region Initialize important data
            string dateRef = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            List<string> dispatcherFilesList = new List<string>();
            string tempDispatcherPath = Path.Combine(Path.GetTempPath(), id + "_" + dateRef);
            #endregion

            try
            {
                #region Get Data Access Instance & basic objects
                SqlDataAccess DataAccess = DataFactory.GetInstance();
                Emailing email = DataAccess.GetEmailing(id);
                Site site = DataAccess.GetSite(email.SiteId);
                List<NetMessageCampaign> netMessageCampaigns = DataAccess.GetNetmessageCampaignsByEmailId(id);

                if (netMessageCampaigns == null || !netMessageCampaigns.Any())
                    throw new Exception("There was no any Netmessage Campaign be linked to this Email Campaign.");
                #endregion

                #region Get Credential
                string server = "", username = "", password = "", accountName;
                int port = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["NetmessageFTPPort"], 21);
                server = System.Configuration.ConfigurationManager.AppSettings["NetmessageFTPServer"];
                if (String.IsNullOrWhiteSpace(server))
                    throw new Exception("Please check app's setting name NetmessageFTPServer.");

                var systemEmail = DataAccess.GetOption("SMTPUsername");
                if (site.IsUseGlobalNetmessageSettings.HasValue && site.IsUseGlobalNetmessageSettings.Value)
                {
                    var options = DataAccess.GetOptions("NetmessageFTPUsername", "NetmessageFTPPassword", "NetmessageAccountName");
                    username = options["NetmessageFTPUsername"];
                    password = options["NetmessageFTPPassword"];
                    accountName = options["NetmessageAccountName"];
                }
                else
                {
                    username = site.NetmessageFTPUsername;
                    password = site.NetmessageFTPPassword;
                    accountName = site.NetmessageAccountName;
                }

                if (String.IsNullOrWhiteSpace(username))
                    throw new Exception("Netmessage FTP Username should not be empty.");

                if (String.IsNullOrWhiteSpace(password))
                    throw new Exception("Netmessage FTP Password should not be empty.");
                #endregion

                #region Create Refresh Dispatcher File
                Clients.Client(Context.ConnectionId).GetInfoMessage("Preparing dispatcher files for updating report.");

                string dispatcherFilePath = string.Empty;
                string dispatcherContent = "<account>" + accountName + "_EMAIL</account>" + Environment.NewLine;
                dispatcherContent += "<media>EMAIL</media>" + Environment.NewLine;
                dispatcherContent += "<function>refresh</function>" + Environment.NewLine;

                foreach (var campaign in netMessageCampaigns)
                {
                    dispatcherFilePath = Path.GetTempFileName();
                    System.IO.File.WriteAllText(dispatcherFilePath, dispatcherContent + "<operation>" + campaign.JobNumber + "</operation>");
                    dispatcherFilesList.Add(dispatcherFilePath);
                }

                Clients.Client(Context.ConnectionId).GetInfoMessage("Uploading dispatcher files to Netmessage FTP Server.");
                using (WebClient client = new WebClient())
                {
                    string ftpUploadPath = "ftp://" + server + "/IN/";
                    client.Credentials = new NetworkCredential(username, password);
                    for (int i = 0, n = netMessageCampaigns.Count; i < n; i++)
                    {
                        client.UploadFile(ftpUploadPath + netMessageCampaigns[i].RefCode + "_Refresh_" + dateRef + ".xnm", "STOR", dispatcherFilesList[i]);
                    }
                }

                Clients.Client(Context.ConnectionId).GetInfoMessage("Waiting responding from Netmessage FTP Server...");

                if (!Directory.Exists(tempDispatcherPath))
                    Directory.CreateDirectory(tempDispatcherPath);

                Thread.Sleep(5); // Waiting for 5 seconds.
                using (Ftp ftp = new Ftp())
                {
                    ftp.Connect("ftp.netmessage.com");
                    ftp.Login(username, password);
                    for (int i = 0, n = dispatcherFilesList.Count; i < n; i++)
                    {
                        ftp.DownloadFiles("OUT", tempDispatcherPath, new RemoteSearchOptions(netMessageCampaigns[i].RefCode + "_Refresh_" + dateRef + ".*", false));
                    }

                    ftp.Close();
                }
                #endregion
            }
            catch (Exception ex)
            {
                Clients.Client(Context.ConnectionId).GetErrorMessage(ex.Message);
            }
            finally
            {
                #region Clear Temp Files
                if (Directory.Exists(tempDispatcherPath))
                {
                    Directory.Delete(tempDispatcherPath, true);
                }

                foreach (string tempFile in dispatcherFilesList)
                {
                    if (File.Exists(tempFile))
                    {
                        @File.Delete(tempFile);
                    }
                }
                #endregion
            }
        }
    }
}