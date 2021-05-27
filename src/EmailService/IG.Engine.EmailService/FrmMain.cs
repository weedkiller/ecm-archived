using IG.Engine.EmailService.BLL;
using IG.Engine.EmailService.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace IG.Engine.EmailService
{
    public partial class FrmMain : Form
    {
        #region Fields & Property

        private SqlDataAccess m_DataAccess;
        private bool m_bIsRunning = true;
        private const int MaxTime = 300;
        private DataTable m_DTEmailKeys;
        private Thread m_EmailThread;
        private bool m_bIsExit = false;
        private string m_sTitle1 = "";
        private string m_sTitle2 = "";
        private string m_sTitle3 = "";
        private string m_sTitle4 = "";

        private string PrefixTitle1
        {
            get
            {
                if (m_sTitle1 == "")
                    m_sTitle1 = DataAccess.GetPrefixText(1);

                return m_sTitle1;
            }
        }
        private string PrefixTitle2
        {
            get
            {
                if (m_sTitle2 == "")
                    m_sTitle2 = DataAccess.GetPrefixText(2);

                return m_sTitle2;
            }
        }
        private string PrefixTitle3
        {
            get
            {
                if (m_sTitle3 == "")
                    m_sTitle3 = DataAccess.GetPrefixText(3);

                return m_sTitle3;
            }
        }
        private string PrefixTitle4
        {
            get
            {
                if (m_sTitle4 == "")
                    m_sTitle4 = DataAccess.GetPrefixText(4);

                return m_sTitle4;
            }
        }
        private DataTable DTEmailKeys
        {
            get
            {
                if (m_DTEmailKeys == null)
                {
                    DataSet ds = this.DataAccess.GetEmailKey();
                    if (ds != null && ds.Tables.Count > 0)
                        m_DTEmailKeys = ds.Tables[0];
                }
                return m_DTEmailKeys;
            }
        }

        private SqlDataAccess DataAccess
        {
            get
            {
                if (m_DataAccess == null)
                    m_DataAccess = new SqlDataAccess(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);

                return m_DataAccess;
            }
        }
        #endregion

        #region Constructor
        public FrmMain()
        {
            InitializeComponent();
        }
        #endregion

        #region EventHandler : Form

        private void FrmMain_Load(object sender, EventArgs e)
        {
            sysMonTray.ShowBalloonTip(5, "Email Service", "Running Email Service", ToolTipIcon.Info);
            LoadEmailConfiguration();

#if DEBUG
            txtURL.Text = "http://localhost:2460/IG.Engine.EmailService";
#endif

        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!m_bIsExit)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void FrmMain_VisibleChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.Hide();
        }

        #endregion

        #region Support Method

        private void RunEmailService()
        {
            try
            {
                int nStatus = 0;
                string sMessage = "";

                while (m_bIsRunning)
                {
                    DataSet ds = this.DataAccess.GetPendingEmail();

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            if (DataManager.ConvertToString(dr["Email"]) != "")
                            {
                                nStatus = SendEmail(dr, ref sMessage);

                                if (nStatus == 1)
                                {
                                    // Send emmail successfully
                                    this.DataAccess.UpdateEmailQueStatus(DataManager.ConvertToInteger(dr["EmailQueId"]), nStatus, DateTime.Now, false, sMessage, false);
                                }
                                else
                                {
                                    // Send email Error 
                                    this.DataAccess.UpdateEmailQueStatus(DataManager.ConvertToInteger(dr["EmailQueId"]), nStatus, DateTime.Now, true, sMessage, false);
                                }

                                Thread.Sleep(100);
                            }
                        }

                        System.Threading.Thread.Sleep(500);
                    }
                    else
                    {

                        // Resending email
                        DataSet dsFailed = this.DataAccess.GetFailedEmail();

                        if (dsFailed != null && dsFailed.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in dsFailed.Tables[0].Rows)
                            {
                                nStatus = SendEmail(dr, ref sMessage);

                                if (nStatus == 1)
                                {
                                    // Send emmail successfully
                                    this.DataAccess.UpdateEmailQueStatus(DataManager.ConvertToInteger(dr["EmailQueId"]), nStatus, DateTime.Now, false, sMessage, true);
                                }
                                else
                                {
                                    // Send email Error 
                                    this.DataAccess.UpdateEmailQueStatus(DataManager.ConvertToInteger(dr["EmailQueId"]), nStatus, DateTime.Now, true, sMessage, true);
                                }

                                Thread.Sleep(100);
                            }
                        }

                        // Wait for next loop
                        System.Threading.Thread.Sleep(30000);
                    }
                }
            }
            catch (Exception ex)
            {
                writeLog(ex.ToString());
                RunEmailService();
            }
            finally
            {

            }

        }

        private void writeLog(string message)
        {
            try
            {
                string logFile = String.Empty;
                StreamWriter logWriter;
                logFile = Application.StartupPath + "\\Error.txt";
                if (File.Exists(logFile))
                {
                    logWriter = File.AppendText(logFile);
                }
                else
                {
                    logWriter = File.CreateText(logFile);
                }
                logWriter.WriteLine(message);
                logWriter.Close();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Error in writing log :" + e.Message);
            }
        }

        private string GetDownloadString(DataRow dr, bool bIsHtmlFormat)
        {
            string sResult = "";

            if (DataManager.ConvertToString(dr["FileName1"]) + DataManager.ConvertToString(dr["FileName2"]) + DataManager.ConvertToString(dr["FileName3"]) + DataManager.ConvertToString(dr["FileName4"]) + DataManager.ConvertToString(dr["FileName5"]) == "")
                return sResult;

            if (bIsHtmlFormat)
            {
                sResult = String.Format("<Font Color='#6F6F6F'><b>{0}</b></Font> <br/>", "Herunterladen");
                if (DataManager.ConvertToString(dr["FileName1"]).Trim().Length > 0)
                {
                    sResult += string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a target='_blank' href='{0}'>{1}</a><br/>", dr["FileUrl1"], dr["FileName1"]);
                    sResult += string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;({0})<br/><br/>", dr["FileName1"]);
                }

                if (DataManager.ConvertToString(dr["FileName2"]).Trim().Length > 0)
                {
                    sResult += string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a target='_blank' href='{0}'>{1}</a><br/>", dr["FileUrl2"], dr["FileName2"]);
                    sResult += string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;({0})<br/><br/>", dr["FileName2"]);
                }

                if (DataManager.ConvertToString(dr["FileName3"]).Trim().Length > 0)
                {
                    sResult += string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a target='_blank' href='{0}'>{1}</a><br/>", dr["FileUrl3"], dr["FileName3"]);
                    sResult += string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;({0})<br/><br/>", dr["FileName3"]);
                }

                if (DataManager.ConvertToString(dr["FileName4"]).Trim().Length > 0)
                {
                    sResult += string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a target='_blank' href='{0}'>{1}</a><br/>", dr["FileUrl4"], dr["FileName4"]);
                    sResult += string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;({0})<br/><br/>", dr["FileName4"]);
                }

                if (DataManager.ConvertToString(dr["FileName5"]).Trim().Length > 0)
                {
                    sResult += string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a target='_blank' href='{0}'>{1}</a><br/>", dr["FileUrl5"], dr["FileName5"]);
                    sResult += string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;({0})<br/><br/>", dr["FileName5"]);
                }
            }
            else
            {
                sResult = "Download" + Environment.NewLine;
                if (DataManager.ConvertToString(dr["FileName1"]).Trim().Length > 0)
                    sResult += dr["FileUrl1"].ToString() + Environment.NewLine;
                if (DataManager.ConvertToString(dr["FileName2"]).Trim().Length > 0)
                    sResult += dr["FileUrl2"].ToString() + Environment.NewLine;
                if (DataManager.ConvertToString(dr["FileName3"]).Trim().Length > 0)
                    sResult += dr["FileUrl3"].ToString() + Environment.NewLine;
                if (DataManager.ConvertToString(dr["FileName4"]).Trim().Length > 0)
                    sResult += dr["FileUrl4"].ToString() + Environment.NewLine;
                if (DataManager.ConvertToString(dr["FileName5"]).Trim().Length > 0)
                    sResult += dr["FileUrl5"].ToString() + Environment.NewLine;

            }
            return sResult;
        }

        private int SendEmail(DataRow dr, ref string sMessage)
        {
            int nResult = 0;
            bool isBounced = false;
            //string sTemp = File.ReadAllText("template.txt");

            EmailItem pItem = new EmailItem();
            List<EmailingAttachment> attachments = DataAccess.GetEmailingAttachmentsByEmailId(DataManager.ConvertToLong(dr["EmailId"], 0));

            if (!DataManager.ConvertToBoolean(dr["IsUseGlobalSMTPSettings"], false))
            {
                string sHost = DataManager.ConvertToString(dr["SMTPServer"], "");
                int nPort = DataManager.ConvertToInteger(dr["SMTPPort"], 0);
                string sUsername = DataManager.ConvertToString(dr["SMTPUsername"], "");
                string sPassword = DataManager.ConvertToString(dr["SMTPPassword"], "");
                bool useSSL = DataManager.ConvertToBoolean(dr["SMTPUseSSL"], false);
                bool useVERP = DataManager.ConvertToBoolean(dr["SMTPUseVERP"], false);
                string bouncedReturnEmail = DataManager.ConvertToString(dr["BouncedReturnEmail"]);

                if (sHost.Length > 0)
                    pItem.Host = sHost;
                if (nPort > 0)
                    pItem.Port = nPort;
                if (sUsername.Length > 0)
                    pItem.UserName = sUsername;
                if (sPassword.Length > 0)
                    pItem.Password = sPassword;
            }
            else
            {
                var options = DataAccess.GetOptions("SMTPServer", "SMTPUsername", "SMTPPassword", "SMTPPort", "SMTPUseSSL", "SMTPUseVERP", "BouncedReturnEmail");
                if (options != null && options.Any())
                {
                    string sHost = DataManager.ConvertToString(options["SMTPServer"], "");
                    int nPort = DataManager.ConvertToInteger(options["SMTPPort"], 0);
                    string sUsername = DataManager.ConvertToString(options["SMTPUsername"], "");
                    string sPassword = DataManager.ConvertToString(options["SMTPPassword"], "");
                    bool useSSL = DataManager.ConvertToBoolean(options["SMTPUseSSL"], false);
                    bool useVERP = DataManager.ConvertToBoolean(options["SMTPUseVERP"], false);
                    string bouncedReturnEmail = DataManager.ConvertToString(options["BouncedReturnEmail"]);

                    if (sHost.Length > 0)
                        pItem.Host = sHost;
                    if (nPort > 0)
                        pItem.Port = nPort;
                    if (sUsername.Length > 0)
                        pItem.UserName = sUsername;
                    if (sPassword.Length > 0)
                        pItem.Password = sPassword;

                    pItem.IsEnableSSL = useSSL;
                    pItem.IsUseVERP = useVERP;
                    pItem.ReturnPath = bouncedReturnEmail;

                }
            }

            pItem.Subject = DataManager.ConvertToString(dr["Subject"]);
            pItem.Tos = DataManager.ConvertToString(dr["Email"]);
            pItem.DisplayName = DataManager.ConvertToString(dr["CustomerName"]);
            pItem.FromDisplayName = DataManager.ConvertToString(dr["FromName"]);
            pItem.From = DataManager.ConvertToString(dr["FromEmail"]);
            pItem.UnsubscribeMailTo = DataManager.ConvertToString(dr["UnsubscribeMailTo"]);
            byte[] contentBytes = null;
            string content = string.Empty;
            if (DataManager.ConvertToInteger(dr["NewsletterType"], 1) == 1)
            {
                contentBytes = dr["HtmlDetail"] != null && dr["HtmlDetail"] != DBNull.Value ? (byte[])dr["HtmlDetail"] : new byte[0];
                content = Encoding.UTF8.GetString(contentBytes);
                // Html Format
                pItem.Body = content + "<br/><br/>";

                // Fixed image url problem
                pItem.Body = pItem.Body.Replace(@"/newsletter/FilesEmail", txtURL.Text.Trim() + "/FilesEmail/");
                pItem.IsBodyHtml = true;
            }
            else
            {
                contentBytes = dr["TextDetail"] != null && dr["TextDetail"] != DBNull.Value ? (byte[])dr["TextDetail"] : new byte[0];
                content = Encoding.UTF8.GetString(contentBytes);
                // Text Format
                pItem.Body = content + System.Environment.NewLine;
                pItem.IsBodyHtml = false;
            }

            // Prefix Text
            if (dr["TitleId"].ToString() == "1")
                pItem.Body = pItem.Body.Replace("[CustomerName]", string.Format("{0} {1},", PrefixTitle1, dr["CustomerName"].ToString()));
            else if (dr["TitleId"].ToString() == "2")
                pItem.Body = pItem.Body.Replace("[CustomerName]", string.Format("{0} {1},", PrefixTitle2, dr["CustomerName"].ToString()));
            else if (dr["TitleId"].ToString() == "3")
                pItem.Body = pItem.Body.Replace("[CustomerName]", string.Format("{0},", PrefixTitle3));
            else
                pItem.Body = pItem.Body.Replace("[CustomerName]", dr["CustomerName"].ToString());

            // Link
            if (PrefixTitle4 != "")
            {
                //[UNSUBSCRIBE]
                m_sTitle4 = PrefixTitle4.Replace("[UNSUBSCRIBE]", "<a href='[URL]?" +
                    string.Format("link=contents/Unsubscribe.aspx?id={0}&user={1}", DataProtection.Encrypt(dr["CustomerId"].ToString())
                       , DataProtection.Encrypt(dr["Email"].ToString())) + "'>Link</a>");

                // [PROFILE]
                m_sTitle4 = PrefixTitle4.Replace("[PROFILE]", "<a href='[URL]?" +
                    string.Format("link=Customer/CustomerProfile.aspx?id={0}&user={1}", DataProtection.Encrypt(dr["CustomerId"].ToString())
                       , DataProtection.Encrypt(dr["Email"].ToString())) + "'>Link</a>");

                //[TEXTUNSUBSCRIBE]
                m_sTitle4 = PrefixTitle4.Replace("[TEXTUNSUBSCRIBE]", "[URL]?" +
                    string.Format("link=contents/Unsubscribe.aspx?id={0}&user={1}", DataProtection.Encrypt(dr["CustomerId"].ToString())
                       , DataProtection.Encrypt(dr["Email"].ToString())));

                //[TEXTPROFILE]
                m_sTitle4 = PrefixTitle4.Replace("[TEXTPROFILE]", "[URL]?" +
                   string.Format("link=Customer/CustomerProfile.aspx?id={0}&user={1}", DataProtection.Encrypt(dr["CustomerId"].ToString())
                      , DataProtection.Encrypt(dr["Email"].ToString())));

                if (pItem.IsBodyHtml)
                {
                    pItem.Body = pItem.Body.Replace("[Link]", PrefixTitle4).Replace("[URL]", txtURL.Text + "/Home.aspx");

                }
                else
                {
                    string sDetail = Regex.Replace(m_sTitle4, @"<(.|\n)*?>", string.Empty);
                    pItem.Body = pItem.Body.Replace("[Link]", sDetail).Replace("[URL]", txtURL.Text + "/Home.aspx").Replace("&nbsp;", "");
                }
            }

            // Download
            pItem.Body = pItem.Body.Replace("[Download]", GetDownloadString(dr, pItem.IsBodyHtml));


            // Send email 
            try
            {

                if (Email.SendEmail(pItem, attachments, ref sMessage, ref isBounced))
                {
                    sMessage = "Sent completed";
                    nResult = 1;
                }
                else
                {
                    if (isBounced)
                    {
                        DataAccess.SetBouncedEmailQue(DataManager.ConvertToLong(dr["EmailQueId"]));
                        DataAccess.AddUserToUnsubscriberList(DataManager.ConvertToLong(dr["CustomerId"]));
                    }
                    sMessage = "Sending Failed : " + sMessage;
                    nResult = -1;
                }

            }
            catch (Exception ex)
            {
                sMessage = "Send Fail : " + ex.Message;
                nResult = -1;
            }

            // Clear Text 
            m_sTitle4 = "";

            return nResult;
        }

        private void LoadEmailConfiguration()
        {
            try
            {
                MailSettingsSectionGroup mailSettings = System.Configuration.ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath + ".config").GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;

                if (mailSettings != null)
                {
                    GlobalData.Host = System.Configuration.ConfigurationSettings.AppSettings["Host"];
                    GlobalData.UserName = System.Configuration.ConfigurationSettings.AppSettings["UserName"];
                    GlobalData.Password = System.Configuration.ConfigurationSettings.AppSettings["Password"];
                    GlobalData.Port = DataManager.ConvertToInteger(System.Configuration.ConfigurationSettings.AppSettings["Port"]);

                    // Show Configuration
                    this.txtHost.Text = GlobalData.Host;
                    this.txtPort.Text = GlobalData.Port.ToString();
                    this.txtUsername.Text = GlobalData.UserName;
                    this.txtPassword.Text = GlobalData.Password;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Load Configration Failed", "Email Service Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

        }

        private string EncriptURL(string sString)
        {
            Regex pReg = new Regex("(?<=<code>).*(?=</code>)");
            string sURL = "";
            string sTemp = "";
            string sReturn = "";

            foreach (Match m in pReg.Matches(sString))
            {
                sTemp += m.Value;
            }

            sURL = DataProtection.Encrypt(sTemp);

            sReturn = sString.Replace(sTemp, sURL);
            //Dim rx As New Regex("(?<=<p>).*(?=</p>)")
            //#
            //Dim returnString As String = String.Empty
            //#
            //For Each m As Match In rx.Matches(Textbox1.text)
            //#
            //     returnString &= m.Value & " "
            //#
            //Next 

            return sReturn;
        }
        #endregion

        #region " EventHandler : Buutton EventHandler "

        private void btnStart_Click(object sender, EventArgs e)
        {
            m_bIsRunning = true;
            // Init Email Thread
            m_EmailThread = new Thread(new ThreadStart(RunEmailService));
            m_EmailThread.IsBackground = true;
            m_EmailThread.Start();
            writeLog("Start");

            this.btnStop.Enabled = true;
            this.btnStart.Enabled = false;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (this.m_EmailThread.IsAlive)
                this.m_EmailThread.Abort();

            m_bIsRunning = false;
            this.btnStop.Enabled = false;
            this.btnStart.Enabled = true;

            m_sTitle1 = "";
            m_sTitle2 = "";
            m_sTitle3 = "";
        }

        #endregion

        #region " EventHandler : Context Menu "

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_bIsExit = true;
            try
            {
                if (m_EmailThread != null && m_EmailThread.ThreadState == ThreadState.Running)
                    m_EmailThread.Abort();

                sysMonTray.Visible = false;
                sysMonTray.Dispose();
            }
            catch
            {
            }

            Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Visible = true;
            this.Show();
        }

        private void minimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        #endregion

        #region " EventHandler : Ok & Cancel Button "

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        #endregion


    } // end class

}// end namespace