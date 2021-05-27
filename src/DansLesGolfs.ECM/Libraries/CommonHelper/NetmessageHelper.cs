using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace DansLesGolfs.ECM
{
    public class NetmessageHelper
    {
        private string server = "";
        //private int port = 21;
        private string username = "";
        private string password = "";
        private string accountName = "";
        private string baseSavePath = "";

        public NetmessageHelper(string baseSavePath)
        {
            this.baseSavePath = baseSavePath;
            GetNetmessageSettings();
        }

        #region Public Methods
        public Task GenerateReports()
        {
            return Task.Run(() =>
            {
                string ftpBasePath = "ftp://" + server + "/OUT/";

                string[] files = null;
                string[] zippedFiles = null;

                try
                {
                    files = FTPListDirectory(ftpBasePath, username, password);
                    zippedFiles = GetZippedFiles(files);
                    PrepareSaveDirectory(baseSavePath);
                    var downloadedFiles = FTPDownloadFiles(ftpBasePath, zippedFiles, username, password, baseSavePath);
                    var ds = ProcessDownloadedFiles(baseSavePath, downloadedFiles);
                    SaveNetmessageReport(ds);

                }
                finally
                {
                    DeleteSaveDirectory(baseSavePath);
                }
            });
        }
        #endregion

        #region Private Methods
        private void GetNetmessageSettings()
        {
            var DataAccess = DataFactory.GetInstance();
            var options = DataAccess.GetOptions("NetmessageFTPServer", "NetmessageFTPPort", "NetmessageFTPUsername", "NetmessageFTPPassword", "NetmessageAccountName");
            //server = options["NetmessageFTPServer"];
            server = System.Configuration.ConfigurationManager.AppSettings["NetmessageFTPServer"];
            //port = DataManager.ToInt(options["NetmessageFTPPort"], 21);
            username = options["NetmessageFTPUsername"];
            password = options["NetmessageFTPPassword"];
            accountName = options["NetmessageAccountName"];
        }

        private string[] GetZippedFiles(string[] files)
        {
            string[] selectedFiles;
            var fileRegexString = @"^ECMC-.+\.zip$";
            var fileRegex = new System.Text.RegularExpressions.Regex(fileRegexString, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);
            selectedFiles = files.Where(file => fileRegex.IsMatch(file)).ToArray();
            return selectedFiles;
        }

        private void PrepareSaveDirectory(string baseSavePath)
        {
            if (!Directory.Exists(baseSavePath))
            {
                Directory.CreateDirectory(baseSavePath);
            }
        }

        private void DeleteSaveDirectory(string baseSavePath)
        {
            if (!Directory.Exists(baseSavePath))
            {
                Directory.Delete(baseSavePath, true);
            }
        }

        private DataSet ProcessDownloadedFiles(string baseSavePath, string[] downloadedFiles)
        {
            DataSet ds = new DataSet("NetmessageReport");
            foreach (var downloadedFile in downloadedFiles)
            {
                ProcessDownloadFile(baseSavePath, ds, downloadedFile);
            }

            return ds;
        }

        private void ProcessDownloadFile(string baseSavePath, DataSet ds, string downloadedFile)
        {
            var extractDir = ExtractFile(downloadedFile, baseSavePath);
            var csvFiles = GetCSVFiles(extractDir);
            var tableName = Path.GetFileNameWithoutExtension(downloadedFile);
            var table = GetDataTableFromCSVFiles(csvFiles, tableName);
            ds.Tables.Add(table);
        }

        private string[] FTPListDirectory(string path, string username, string password)
        {
            List<string> files = new List<string>();
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
            request.Credentials = new NetworkCredential(username, password);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            using (var response = (FtpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        string content = sr.ReadToEnd();
                        files = content.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    }
                }
            }
            return files.ToArray();
        }

        private void FTPDeleteFile(string path, string[] files, string username, string password)
        {
            FtpWebRequest request = null;
            foreach (string file in files)
            {
                request = (FtpWebRequest)WebRequest.Create(path + file);
                request.Credentials = new NetworkCredential(username, password);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            string content = sr.ReadToEnd();
                        }
                    }
                }
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

        private string ExtractFile(string downloadedFile, string extractDir)
        {
            string extractPath = Path.Combine(extractDir, Path.GetFileNameWithoutExtension(downloadedFile));
            if (Directory.Exists(extractPath))
            {
                Directory.Delete(extractPath, true);
            }
            System.IO.Compression.ZipFile.ExtractToDirectory(downloadedFile, extractPath);
            return extractPath;
        }

        private string[] GetCSVFiles(string dir)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\.csv$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return Directory.GetFiles(dir).Where(it => regex.IsMatch(it)).ToArray();
        }

        private DataTable GetDataTableFromCSVFiles(string[] csvFiles, string tableName)
        {
            Regex regexEventFile = new Regex(@"ECMC-.*\.evt.*\.csv$", RegexOptions.IgnoreCase);
            DataTable mainTable = null;
            DataTable eventTable = null;
            foreach (string csvFile in csvFiles)
            {
                var table = GetDataTableFromCSVFile(csvFile, tableName);

                // Check if it's event table.
                if (regexEventFile.IsMatch(csvFile))
                {
                    if (eventTable == null)
                    {
                        eventTable = table.Copy();
                    }
                    else
                    {
                        eventTable.Merge(table);
                    }
                }
                else
                {
                    if (mainTable == null)
                    {
                        mainTable = table.Copy();
                    }
                    else
                    {
                        mainTable.Merge(table);
                    }
                }
            }

            MergeEventTable(mainTable, eventTable);

            return mainTable;
        }

        private DataTable GetDataTableFromCSVFile(string csvFile, string tableName)
        {
            var csvLines = System.IO.File.ReadAllLines(csvFile);
            if (csvFile.Any())
            {
                var table = GetDataTableFromCSVLine(csvLines[0], ';', tableName);

                for (int i = 1, n = csvLines.Length; i < n; i++)
                {
                    var row = GetDataRowFromCSVLine(table, csvLines[i], ';');
                    table.Rows.Add(row);
                }
                return table;
            }
            else
            {
                return new DataTable();
            }
        }

        private DataTable GetDataTableFromCSVLine(string line, char delimiter = ',', string tableName = null)
        {
            var table = String.IsNullOrWhiteSpace(tableName) ? new DataTable() : new DataTable(tableName);

            var columns = line
                .Split(new char[] { delimiter }, StringSplitOptions.RemoveEmptyEntries)
                .Select(it => new DataColumn(it.Replace("\"", ""))).ToArray();
            table.Columns.AddRange(columns);

            return table;
        }

        private DataRow GetDataRowFromCSVLine(DataTable table, string line, char delimiter = ',')
        {
            DataRow row = table.NewRow();

            var values = line.Split(delimiter);

            for (int i = 0, n = table.Columns.Count; i < n; i++)
            {
                row[i] = values[i].Replace("\"", "");
            }

            return row;
        }

        private void MergeEventTable(DataTable mainTable, DataTable eventTable)
        {
            AddEventColumnsToTable(mainTable);

            if (eventTable == null)
                return;

            foreach (DataRow row in eventTable.Rows)
            {
                var email = DataManager.ToString(row["EMAIL"]);
                if (String.IsNullOrWhiteSpace(email))
                    continue;

                var mainRow = GetDataRowByEmail(mainTable, email);
                if (mainRow == null)
                    continue;

                mainRow["OPEN"] = DataManager.ToString(row["OPEN"]);
                mainRow["UNSUB"] = DataManager.ToString(row["UNSUB"]);
                mainRow["SUB"] = DataManager.ToString(row["SUB"]);
                mainRow["RADIATE"] = DataManager.ToString(row["RADIATE"]);
                mainRow["VIEW"] = DataManager.ToString(row["VIEW"]);
                mainRow["CLICK"] = DataManager.ToString(row[6]);
            }
        }

        private void AddEventColumnsToTable(DataTable mainTable)
        {
            if (!mainTable.Columns.Contains("OPEN"))
                mainTable.Columns.Add("OPEN");

            if (!mainTable.Columns.Contains("UNSUB"))
                mainTable.Columns.Add("UNSUB");

            if (!mainTable.Columns.Contains("SUB"))
                mainTable.Columns.Add("SUB");

            if (!mainTable.Columns.Contains("RADIATE"))
                mainTable.Columns.Add("RADIATE");

            if (!mainTable.Columns.Contains("VIEW"))
                mainTable.Columns.Add("VIEW");

            if (!mainTable.Columns.Contains("CLICK"))
                mainTable.Columns.Add("CLICK");
        }

        private DataRow GetDataRowByEmail(DataTable mainTable, string email)
        {
            var rows = mainTable.Select("EMAIL = '" + email + "'");
            return rows.Length > 0 ? rows[0] : null;
        }

        private void SaveNetmessageReport(DataSet ds)
        {
            var DataAccess = DataFactory.GetInstance();
            foreach (DataTable table in ds.Tables)
            {
                var tableName = table.TableName;
                var nameParts = tableName.Split('-');
                if (nameParts.Length < 3)
                    continue;

                var emailId = DataManager.ToLong(nameParts[1]);
                var reportDate = GetReportDateFromString(tableName);
                var report = new DansLesGolfs.BLL.NetmessageReport()
                {
                    EmailId = emailId,
                    ReportDate = reportDate,
                    Active = true
                };

                foreach (DataRow row in table.Rows)
                {
                    var record = new NetmessageReportRecord()
                    {
                        Email = DataManager.ToString(row["EMAIL"]).Trim(),
                        FirstName = DataManager.ToString(row["FIRSTNAME"]).Trim(),
                        LastName = DataManager.ToString(row["LASTNAME"]).Trim(),
                        Name = DataManager.ToString(row["NAME"]).Trim(),
                        Gender = DataManager.ToString(row["GENDER"]).Trim(),
                        Phone = DataManager.ToString(row["PHONE"]).Trim(),
                        Mobile = DataManager.ToString(row["MOBILE"]).Trim(),
                        Description = DataManager.ToString(row["DESCRIPTION"]).Trim(),
                        Profession = DataManager.ToString(row["PROFESSION"]).Trim(),
                        Index = DataManager.ToString(row["INDEX"]).Trim(),
                        Field1 = DataManager.ToString(row["FIELD1"]).Trim(),
                        Field2 = DataManager.ToString(row["FIELD2"]).Trim(),
                        Field3 = DataManager.ToString(row["FIELD3"]).Trim(),
                        Status = DataManager.ToString(row["ERROR_NAME"]).Trim()
                    };

                    if (!String.IsNullOrWhiteSpace(DataManager.ToString(row["DATE_SEND"])))
                    {
                        record.IsSent = true;
                        record.SentTime = DateTime.ParseExact(DataManager.ToString(row["DATE_SEND"]), "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        record.IsSent = false;
                        record.SentTime = null;
                    }

                    // Update event data.
                    UpdateNetmessageRecordEventColumns(row, record);

                    report.NetmessageReportRecords.Add(record);
                }

                DataAccess.SaveNetmessageReport(report);

                // Handle Unsubscribe
                var unsubscribers = report.NetmessageReportRecords.Where(it => it.IsUnsub.HasValue && it.IsUnsub.Value);
                if (unsubscribers.Any())
                {
                    foreach (var unsubscriber in unsubscribers)
                    {
                        DataAccess.UnSubscribeUserEmail(unsubscriber.Email, 0);
                    }
                }
            }
        }

        private void UpdateNetmessageRecordEventColumns(DataRow row, NetmessageReportRecord record)
        {
            if (!String.IsNullOrWhiteSpace(DataManager.ToString(row["OPEN"])))
            {
                record.IsOpen = true;
                record.OpenTime = DateTime.ParseExact(DataManager.ToString(row["OPEN"]), "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                record.IsOpen = false;
                record.OpenTime = null;
            }

            if (!String.IsNullOrWhiteSpace(DataManager.ToString(row["UNSUB"])))
            {
                record.IsUnsub = true;
                record.UnsubTime = DateTime.ParseExact(DataManager.ToString(row["UNSUB"]), "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                record.IsUnsub = false;
                record.UnsubTime = null;
            }

            if (!String.IsNullOrWhiteSpace(DataManager.ToString(row["SUB"])))
            {
                record.IsSub = true;
                record.SubTime = DateTime.ParseExact(DataManager.ToString(row["SUB"]), "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                record.IsSub = false;
                record.SubTime = null;
            }

            if (!String.IsNullOrWhiteSpace(DataManager.ToString(row["RADIATE"])))
            {
                record.IsRadiate = true;
                record.RadiateTime = DateTime.ParseExact(DataManager.ToString(row["RADIATE"]), "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                record.IsRadiate = false;
                record.RadiateTime = null;
            }

            if (!String.IsNullOrWhiteSpace(DataManager.ToString(row["VIEW"])))
            {
                record.IsView = true;
                record.ViewTime = DateTime.ParseExact(DataManager.ToString(row["VIEW"]), "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                record.IsView = false;
                record.ViewTime = null;
            }

            if (!String.IsNullOrWhiteSpace(DataManager.ToString(row["CLICK"])))
            {
                record.IsClick = true;
                record.ClickTime = DateTime.ParseExact(DataManager.ToString(row["CLICK"]), "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                record.IsClick = false;
                record.ClickTime = null;
            }
        }

        private DateTime GetReportDateFromString(string dateString)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"^\w+-[0-9]+-([0-9]+).*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var match = regex.Match(dateString);
            if (match.Groups.Count > 1)
            {
                return DateTime.ParseExact(match.Groups[1].Value, "yyyyMMddHHmmssffff", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                throw new InvalidDataException("GetReportDateFromString Error, date string: " + dateString);
            }
        }
        #endregion
    }
}