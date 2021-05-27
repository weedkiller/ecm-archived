using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DansLesGolfs.BLL;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using DansLesGolfs.Base;
using System.Net;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Dynamic;

namespace DansLesGolfs.Data
{
    public partial class AlbatrosDataAccess
    {
        #region Members
        private string url;
        private string login;
        private string password;
        private string locale = "fr";
        private string sessionId;
        private bool isloggedIn;
        private SecurityProtocolType protocol = SecurityProtocolType.Tls;
        #endregion

        #region Properties
        public bool IsLoggedIn { get { return isloggedIn; } private set { isloggedIn = value; } }
        private string SessionInfo { get { return "<sessionInfo><sessionID>" + sessionId + "</sessionID><locale>" + locale + "</locale></sessionInfo>"; } }
        #endregion

        #region Constructor
        public AlbatrosDataAccess(AlbatrosSettings settings)
        {
            this.url = settings.Url;
            this.login = settings.Login;
            this.password = settings.Password;
            switch(settings.Protocol.ToLower())
            {
                case "tls":
                    protocol = SecurityProtocolType.Tls;
                    break;
                case "tls11":
                    protocol = SecurityProtocolType.Tls11;
                    break;
                case "tls12":
                    protocol = SecurityProtocolType.Tls12;
                    break;
                case "ssl3":
                    protocol = SecurityProtocolType.Ssl3;
                    break;
            }
        }
        #endregion

        #region Private Methods
        public dynamic InvokeSoapMethod(string sActionName, XmlDocument soapEnvelopeXml)
        {
            //ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = protocol;

            // allows for validation of SSL conversations
            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });

            HttpWebRequest webRequest = CreateWebRequest(sActionName);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            // get the response from the completed web request.  
            XmlDocument pResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    string sResult = rd.ReadToEnd();
                    pResult = new XmlDocument();
                    pResult.LoadXml(sResult);
                }
            }

            return ConvertXmlToDynamicObject(pResult);
        }

        private HttpWebRequest CreateWebRequest(string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(this.url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            webRequest.KeepAlive = false;
            webRequest.ProtocolVersion = HttpVersion.Version10;
            return webRequest;
        }

        private void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        private dynamic ConvertXmlToDynamicObject(XmlDocument xml)
        {
            dynamic obj = new ExpandoObject();
            XDocument xdoc = XDocument.Load(new XmlNodeReader(xml));
            var returnNodes = xdoc.Descendants(XName.Get("return"));
            if (returnNodes.Count() > 0)
            {
                var returnObj = returnNodes.First();
                returnObj.Name = "data";
                ExpandoObjectHelper.Parse(obj, returnObj);
                return obj.data;
            }
            else
            {
                obj.data = new ExpandoObject();
                return obj.data;
            }
        }

        private string MakeEnvelop(string envelop)
        {
            return "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:alb=\"http://albatros.services/\"><soapenv:Header/><soapenv:Body>" + envelop + "</soapenv:Body></soapenv:Envelope>";
        }
        #endregion

        #region Login
        public bool Login()
        {
            XmlDocument soapEnvelop = new XmlDocument();
            string sRequest = MakeEnvelop("<alb:doLogin><request><anonymous>false</anonymous><login>" + this.login + "</login><password>" + this.password + "</password></request></alb:doLogin>");
            soapEnvelop.LoadXml(sRequest);
            var response = InvokeSoapMethod("doLogin", soapEnvelop);
            if (response.code == "0" || response.code == "1")
            {
                sessionId = response.sessionInfo.sessionID;
                isloggedIn = true;
            }
            else
            {
                isloggedIn = false;
            }
            return isloggedIn;
        }
        #endregion

        #region Logout
        public void Logout()
        {
            if (String.IsNullOrEmpty(SessionInfo) || String.IsNullOrWhiteSpace(SessionInfo))
                return;

            XmlDocument soapEnvelop = new XmlDocument();
            string sRequest = MakeEnvelop("<alb:doLogout><request>" + SessionInfo + "</request></alb:doLogout>");
            soapEnvelop.LoadXml(sRequest);
            InvokeSoapMethod("doLogout", soapEnvelop);
            isloggedIn = false;
            sessionId = string.Empty;
        }
        #endregion

        #region SetConnection
        public void SetConnection(string url, string username, string password)
        {
            this.url = url.Trim();
            this.login = username.Trim();
            this.password = password.Trim();
        }
        #endregion

        #region GetReservationCourses
        public dynamic GetReservationCourses(DateTime reserveDate)
        {
            XmlDocument soapEnvelop = new XmlDocument();
            string sRequest = MakeEnvelop("<alb:getReservationCourses><request>" + SessionInfo + "<date>" + reserveDate.ToString("dd.MM.yyyy") + "</date><types>courses</types></request></alb:getReservationCourses>");
            soapEnvelop.LoadXml(sRequest);
            return InvokeSoapMethod("getReservationCourses", soapEnvelop);
        }
        #endregion

        #region GetTeeTimes
        public dynamic GetTeeTimes(int roId, DateTime dateTimeFrom, DateTime dateTimeTo, TeeTimesSearchType type, TeeTimesSortBy sortBy, bool sortAscending = true)
        {
            XmlDocument soapEnvelop = new XmlDocument();
            string sRequest = MakeEnvelop("<alb:getTeeTimes><request>" + SessionInfo + "<roId>" + roId + "</roId><dateTimeFrom>" + dateTimeFrom.ToString("dd.MM.yyyy HH:mm") + "</dateTimeFrom><dateTimeTo>" + dateTimeTo.ToString("dd.MM.yyyy HH:mm") + "</dateTimeTo><type>" + (int)type + "</type><sortBy>" + (int)sortBy + "</sortBy><sortAscending>" + sortAscending.ToString() + "</sortAscending></request></alb:getTeeTimes>");
            soapEnvelop.LoadXml(sRequest);
            return InvokeSoapMethod("getTeeTimes", soapEnvelop);
        }
        #endregion

        #region GetTeeTimeDetails
        public dynamic GetTeeTimeDetails(int roId, DateTime dateTime)
        {
            XmlDocument soapEnvelop = new XmlDocument();
            string sRequest = MakeEnvelop("<alb:getTeeTimeDetails><request>" + SessionInfo + "<roId>" + roId + "</roId><dateTime>" + dateTime.ToString("dd.MM.yyyy HH:mm") + "</dateTime></request></alb:getTeeTimeDetails>");
            soapEnvelop.LoadXml(sRequest);
            return InvokeSoapMethod("getTeeTimeDetails", soapEnvelop);
        }
        #endregion

        #region AddReservation
        public dynamic AddReservation(DateTime startTime, int roId, int gameType, string remark, int numberOfPlayers, User user)
        {
            if (numberOfPlayers < 1)
                numberOfPlayers = 1;

            string playerList = string.Empty;
            playerList += "<player>";
            // playerList += "<id>" + Guid.NewGuid().ToString().ToUpper().Replace("-", "") + "</id>";
            playerList += "<details>";
            playerList += "<externalId>" + user.UserId + "</externalId>";
            playerList += "<email>" + user.Email + "</email>";
            playerList += "<firstName>" + user.FirstName + "</firstName>";
            playerList += "<lastName>" + user.LastName + "</lastName>";
            playerList += "<sex>" + (user.Gender + 1).ToString() + "</sex>";
            playerList += "<mobileNumber>" + user.MobilePhone + "</mobileNumber>";
            playerList += "<notify>1</notify>";
            playerList += "</details>";
            playerList += "<playerType>5</playerType>";
            playerList += "</player>";
            for (int i = 1; i < numberOfPlayers; i++)
            {
                playerList += "<player>";
                playerList += "<playerType>2</playerType>";
                playerList += "</player>";
            }

            XmlDocument soapEnvelop = new XmlDocument();
            string sRequest = MakeEnvelop(@"<alb:addReservation><request>" + SessionInfo
                                            + "<info>"
                                                + "<roId>" + roId + "</roId>"
                                                + "<remark>" + remark + "</remark>"
                                                + "<startTime>" + startTime.ToString("dd.MM.yyyy HH:mm") + "</startTime>"
                                                + "<gameType>" + gameType + "</gameType>"
                                                + "<players>"
                                                    + playerList
                                                + "</players>"
                                            + "</info>"
                                            + "</request></alb:addReservation>");
            soapEnvelop.LoadXml(sRequest);
            return InvokeSoapMethod("addReservation", soapEnvelop);
        }
        #endregion

        #region ConfirmReservations
        public dynamic ConfirmReservations(string bookingNumber, User user, DateTime paymentRequestDate, int numberOfPlayers, decimal amount, string recieverEmail, string transactionId)
        {
            if (numberOfPlayers < 1)
                numberOfPlayers = 1;

            string amountStr = amount.ToString("###0.00");
            amountStr = String.Join("", amountStr.Where(it => Char.IsDigit(it)));

            XmlDocument soapEnvelop = new XmlDocument();
            string sRequest = MakeEnvelop(@"<alb:confirmReservations><request>" + SessionInfo
                                            + "<confirmations>"
                                                   + "<confirmation>"
                                                        + "<bookNr>" + bookingNumber + "</bookNr>"
                                                        + "<players>"
                                                            + "<player>"
                                                                + "<externalId>" + user.UserId + "</externalId>"
                                                                + "<amount>" + amountStr + "</amount>"
                                                                + "<quantity>" + numberOfPlayers + "</quantity>"
                                                            + "</player>"
                                                        + "</players>"
                                                   + "</confirmation>"
                                            + "</confirmations>"
                                            + "<prePayment>"
                                                + "<amount>" + amountStr + "</amount>"
                                                + "<paymentPlace>1</paymentPlace>" // Collect money by club
                                                + "<paymentRequestDate>" + paymentRequestDate.ToString("dd.MM.yyyy") + "</paymentRequestDate>"
                                                + "<reciever>" + recieverEmail + "</reciever>"
                                                + "<sender>" + user.Email + "</sender>"
                                                + "<senderTransactionId>" + transactionId + "</senderTransactionId>"
                                                + "<transactionId>" + transactionId + "</transactionId>"
                                            + "</prePayment>"
                                        + "</request></alb:confirmReservations>");
            soapEnvelop.LoadXml(sRequest);
            return InvokeSoapMethod("confirmReservations", soapEnvelop);
        }
        #endregion

        public dynamic CancelReservation(IEnumerable<string> bookNrs)
        {
            string bookNrsList = string.Empty;
            foreach(string bookNr in bookNrs)
            {
                bookNrsList += "<bookNr>" + bookNr + "</bookNr>";
            }
            XmlDocument soapEnvelop = new XmlDocument();
            string sRequest = MakeEnvelop(@"<alb:cancelReservations>"
                                            + "<request>" + SessionInfo
                                                + "<bookNrs>"
                                                    + bookNrsList
                                                + "</bookNrs>"
                                             + "</request>"
                                          + "</alb:cancelReservations>");
            soapEnvelop.LoadXml(sRequest);
            return InvokeSoapMethod("cancelReservations", soapEnvelop);
        }
    }

    public enum TeeTimeType
    {
        WithoutPrice = 0,
        WithNormalPrice = 1,
        WithReductionPrice = 2
    }

    public enum TeeTimesSearchType
    {
        All = 0,
        WithReductionPrice = 1
    }

    public enum TeeTimesSortBy
    {
        No = 0,
        DateTime = 1,
        ReductionFee = 2,
        NormalFee = 3,
        ReductionPercent = 4
    }
}
