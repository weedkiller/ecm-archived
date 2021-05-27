using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text;
using System.Threading;

namespace DansLesGolfs.Data
{
    public partial class ChronogolfDataAccess
    {
        #region Members
        private string tokenUrl = "http://chronogolf.danslesgolfs.com/token";
        private string tokenSecret = "";
        private string clientId = "";
        private string clientSecret = "";
        private string refreshToken = "";
        private string accessToken = "";
        private int clubId = 0;
        private string endpoint = "https://www.chronogolf.com/partner_api/v1/";
        private SecurityProtocolType protocol = SecurityProtocolType.Tls;
        private string defaultAffiliationType = "";
        #endregion

        #region Properties
        public string ClientId { get { return clientId; } private set { clientId = value; } }
        public string ClientSecret { get { return clientSecret; } private set { clientSecret = value; } }
        public string RefreshToken { get { return refreshToken; } private set { refreshToken = value; } }
        public string AccessToken { get { return accessToken; } set { accessToken = value; } }
        public int ClubId { get { return clubId; } private set { clubId = value; } }
        public string Endpoint { get { return endpoint; } private set { endpoint = value; } }
        public SecurityProtocolType Protocol { get { return protocol; } set { protocol = value; } }
        public string DefaultAffiliationType { get { return defaultAffiliationType; } set { defaultAffiliationType = value; } }
        #endregion

        #region Constructor
        public ChronogolfDataAccess(string tokenUrl, string tokenSecret, int clubId = 1)
        {
            this.tokenUrl = tokenUrl;
            this.tokenSecret = tokenSecret;
            this.clubId = clubId;
            this.Refresh();
        }
        #endregion

        #region Refresh
        public void Refresh()
        {
            string contentStr = String.Empty;

            var webRequest = (HttpWebRequest)WebRequest.Create(tokenUrl);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";
            webRequest.ContentLength = 0;
            webRequest.Headers.Add("Authorization", "Bearer " + tokenSecret);

            try
            {
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
                asyncResult.AsyncWaitHandle.WaitOne();

                using (var response = webRequest.EndGetResponse(asyncResult))
                {
                    using (var content = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(content, Encoding.GetEncoding("UTF-8")))
                        {
                            contentStr = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException we)
            {
                throw we;
            }

            var json = JsonConvert.DeserializeObject<ChronogolfAccessToken>(contentStr);

            this.clientId = json.ClientId;
            this.accessToken = json.AccessToken;
            this.endpoint = json.Endpoint;
            SetProtocol(json.Protocol);
            this.defaultAffiliationType = json.DefaultAffiliationType;
        }
        #endregion

        #region SetProtocol
        private void SetProtocol(string protocolName)
        {
            switch (protocolName.ToLower())
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
        private ChronogolfResponse SendRequest(string method = "GET", string url = "", Dictionary<string, object> parameters = null)
        {
            ChronogolfResponse chronogolfResponse = null;
            string contentStr = string.Empty;
            string queryString = method.Equals("GET", StringComparison.InvariantCultureIgnoreCase) ? GetQueryString(parameters) : String.Empty;
            url = url.Replace("?", "");

            //ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = protocol;

            // allows for validation of SSL conversations
            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });

            var webRequest = (HttpWebRequest)WebRequest.Create(url + queryString);
            webRequest.KeepAlive = false;
            webRequest.Method = method;
            webRequest.ProtocolVersion = HttpVersion.Version10;
            webRequest.ContentType = "application/json";

            if (!String.IsNullOrWhiteSpace(accessToken))
            {
                webRequest.Headers.Add("Authorization", "Bearer " + accessToken);
            }

            if (method.Equals("POST", StringComparison.InvariantCultureIgnoreCase) && parameters != null && parameters.Any())
            {
                string postData = JsonConvert.SerializeObject(parameters);

                using (StreamWriter sw = new StreamWriter(webRequest.GetRequestStream()))
                {
                    sw.Write(postData);
                    sw.Flush();
                }
            }

            try
            {
                // begin async call to web request.
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                // suspend this thread until call is complete. You might want to
                // do something usefull here like update your UI.
                asyncResult.AsyncWaitHandle.WaitOne();

                using (var response = (HttpWebResponse)webRequest.EndGetResponse(asyncResult))
                {
                    chronogolfResponse = new ChronogolfResponse(response);
                }
            }
            catch (WebException we)
            {
                var response = we.Response as HttpWebResponse;
                if (response.StatusCode.ToString() == "422")
                {
                    throw new Exception("Your player type can only booking in 7 days advanced.");
                }

                throw we;
            }

            return chronogolfResponse;
        }

        private string GetQueryString(Dictionary<string, object> parameters)
        {
            string queryStr = string.Empty;
            List<string> paramList = new List<string>();

            if (parameters != null && parameters.Any())
            {
                foreach (var entry in parameters)
                {
                    if (entry.Value is IEnumerable<string>)
                    {
                        foreach (var v in (IEnumerable<string>)entry.Value)
                        {
                            if (v == null || (v is string && String.IsNullOrWhiteSpace(v.ToString())))
                                continue;

                            paramList.Add(entry.Key + "[]=" + v.ToString());
                        }
                    }
                    else
                    {
                        paramList.Add(entry.Key + "=" + entry.Value);
                    }
                }
            }

            if (paramList.Any())
            {
                queryStr = "?" + String.Join("&", paramList);
            }

            return queryStr;
        }
        #endregion

        #region GetAffiliateTypes

        public List<ChronogolfAffiliateType> GetAllAffiliationTypes()
        {
            int page = 0;
            int totalPages = 0;
            int itemsPerPage = 100;

            ChronogolfResponse response = null;
            List<ChronogolfAffiliateType> list = null;
            List<ChronogolfAffiliateType> affiliationTypes = new List<ChronogolfAffiliateType>();
            do
            {
                try
                {
                    response = DoGetAffiliateTypes(++page, itemsPerPage);
                    totalPages = (int)Math.Ceiling((double)response.Total / itemsPerPage);
                    list = ConvertResponseToAffiliationTypes(response);
                }
                catch
                {
                    list = new List<ChronogolfAffiliateType>();
                }

                if (list.Count > 0)
                {
                    affiliationTypes.AddRange(list);
                }
                if (page >= totalPages)
                {
                    break;
                }
                Thread.Sleep(5000);
                this.Refresh();
            } while (true);
            return affiliationTypes;
        }

        public List<ChronogolfAffiliateType> GetAffiliateTypes(int page = 1)
        {
            ChronogolfResponse obj = DoGetAffiliateTypes(page);

            return ConvertResponseToAffiliationTypes(obj);
        }

        private ChronogolfResponse DoGetAffiliateTypes(int page, int itemsPerPage = 100)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("page", page);
            parameters.Add("per_page", 100);

            var obj = SendRequest("GET", endpoint + "clubs/" + clubId + "/affiliation_types", parameters);
            return obj;
        }

        private static List<ChronogolfAffiliateType> ConvertResponseToAffiliationTypes(ChronogolfResponse obj)
        {
            List<ChronogolfAffiliateType> list = new List<ChronogolfAffiliateType>();
            if (obj.Body != null && obj.Body is IEnumerable<dynamic> && ((IEnumerable<dynamic>)obj.Body).Any())
            {
                foreach (var affiliateType in (IEnumerable<dynamic>)obj.Body)
                {
                    list.Add(new ChronogolfAffiliateType()
                    {
                        id = DataManager.ToInt(affiliateType.id),
                        name = DataManager.ToString(affiliateType.name),
                        default_role = DataManager.ToString(affiliateType.default_role),
                        club_id = affiliateType.clubId,
                        organization_id = affiliateType.organization_id
                    });
                }
            }

            return list.OrderBy(it => it.id).ToList();
        }

        #endregion

        #region GetCustomers

        public ChronogolfGetAllCustomersResponse GetAllCustomers(ChronogolfCustomer filters = null)
        {
            int page = 0;
            int totalPages = 0;
            int itemsPerPage = 100;

            ChronogolfGetAllCustomersResponse returns = new ChronogolfGetAllCustomersResponse();

            ChronogolfResponse response = null;
            List<ChronogolfCustomer> customers = null;

            List<string> errors = new List<string>();

            do
            {
                // Try to fetch customers.
                try
                {
                    response = DoGetCustomers(filters, ++page, itemsPerPage);
                    totalPages = (int)Math.Ceiling((double)response.Total / itemsPerPage);
                    customers = ConvertResposneToCustomers(response);
                }
                catch (Exception ex)
                {
                    customers = new List<ChronogolfCustomer>();
                    errors.Add(String.Format("Error while getting customer in page {0}, Club ID: {1}.\nMessage:{2}", page, ClubId, ex.Message));
                }

                if (customers.Count > 0)
                {
                    returns.Customers.AddRange(customers);
                }
                if (page >= totalPages)
                {
                    break;
                }
                Thread.Sleep(5000);
                this.Refresh();
            } while (true);

            // Create easy-to-read error message if exists.
            if(errors.Any())
            {
                returns.Errors = new Exception(String.Join(Environment.NewLine, errors));
            }

            return returns;
        }

        public List<ChronogolfCustomer> GetCustomers(ChronogolfCustomer filters = null, int page = 1, int perPage = 100)
        {
            ChronogolfResponse response = DoGetCustomers(filters, page, perPage);
            List<ChronogolfCustomer> customers = ConvertResposneToCustomers(response);

            return customers;
        }

        private ChronogolfResponse DoGetCustomers(ChronogolfCustomer filters, int page, int perPage)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("per_page", perPage);
            parameters.Add("page", page);

            if (filters != null)
            {
                if (!String.IsNullOrWhiteSpace(filters.Email))
                {
                    parameters.Add("email", filters.Email);
                }
                if (!String.IsNullOrWhiteSpace(filters.FirstName))
                {
                    parameters.Add("first_name", filters.FirstName);
                }
                if (!String.IsNullOrWhiteSpace(filters.LastName))
                {
                    parameters.Add("last_name", filters.LastName);
                }
                if (!String.IsNullOrWhiteSpace(filters.Phone))
                {
                    parameters.Add("phone", filters.Phone);
                }
                if (filters.AffiliationTypeId > 0)
                {
                    parameters.Add("affiliation_type_id", filters.AffiliationTypeId);
                }
            }

            var obj = SendRequest("GET", endpoint + "clubs/" + clubId + "/customers", parameters);
            return obj;
        }

        private static List<ChronogolfCustomer> ConvertResposneToCustomers(ChronogolfResponse response)
        {
            List<ChronogolfCustomer> customers = new List<Data.ChronogolfCustomer>();
            if (response.Body != null && response.Body is IEnumerable<dynamic> && ((IEnumerable<dynamic>)response.Body).Any())
            {
                foreach (var customer in (IEnumerable<dynamic>)response.Body)
                {
                    customers.Add(new Data.ChronogolfCustomer()
                    {
                        CustomerId = DataManager.ToInt(customer.id),
                        Email = customer.email,
                        FirstName = customer.first_name,
                        LastName = customer.last_name,
                        Phone = customer.phone,
                        AffiliationTypeId = DataManager.ToInt(customer.affiliation_type_id)
                    });
                }
            }

            return customers;
        }
        #endregion

        #region GetPublicAffiliationTypeId
        public int GetPublicAffiliationTypeId(List<ChronogolfAffiliateType> types)
        {
            if (types != null && types.Any())
            {
                if (!String.IsNullOrWhiteSpace(defaultAffiliationType))
                {
                    var dTypes = types.Where(it => !String.IsNullOrWhiteSpace(it.name) && it.name.Trim().Equals(defaultAffiliationType, StringComparison.InvariantCultureIgnoreCase)).ToList();
                    if (dTypes != null && dTypes.Any())
                    {
                        return dTypes.First().id;
                    }
                }

                var aTypes = types.Where(it => !String.IsNullOrWhiteSpace(it.name) && it.default_role.Trim().Equals("public", StringComparison.InvariantCultureIgnoreCase)).ToList();
                if (aTypes != null && aTypes.Any())
                {
                    return aTypes.First().id;
                }
                else
                {
                    return types.First().id;
                }
            }
            return 0;
        }
        #endregion
    }

    public class ChronogolfAccessToken
    {
        public string ClientId;
        public string AccessToken;
        public string ExpiresIn;
        public string Protocol;
        public string Endpoint;
        public string DefaultAffiliationType;
    }

    public class ChronogolfCourse
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int Holes { get; set; }
        public int Par { get; set; }
        public int Distance { get; set; }
        public bool OnlineBookingEnabled { get; set; }
    }

    public class ChronogolfCustomer
    {
        public int CustomerId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public int AffiliationTypeId { get; set; }
    }

    public class ChronogolfAffiliateType
    {
        public int id { get; set; }
        public string name { get; set; }
        public string default_role { get; set; }
        public int? club_id { get; set; }
        public int? organization_id { get; set; }
    }

    public class ChronogolfResponse
    {
        public int PerPage { get; set; }
        public int Total { get; set; }
        public dynamic Body { get; set; }

        public ChronogolfResponse(HttpWebResponse response)
        {
            ParseWebResponse(response);
        }

        private void ParseWebResponse(HttpWebResponse response)
        {
            string contentStr = String.Empty;
            using (var content = response.GetResponseStream())
            {
                using (var reader = new StreamReader(content, Encoding.GetEncoding("UTF-8")))
                {
                    contentStr = reader.ReadToEnd();
                }
            }

            if (response.Headers.AllKeys.Contains("Per-Page"))
            {
                PerPage = DataManager.ToInt(response.Headers["Per-Page"]);
            }
            if (response.Headers.AllKeys.Contains("Total"))
            {
                Total = DataManager.ToInt(response.Headers["Total"]);
            }

            if (String.IsNullOrWhiteSpace(contentStr))
            {
                Body = null;
            }
            else
            {
                Body = JsonConvert.DeserializeObject<dynamic>(contentStr);
            }
        }
    }

    public class ChronogolfGetAllCustomersResponse
    {
        public List<ChronogolfCustomer> Customers { get; internal set; }
        public Exception Errors { get; internal set; }

        public ChronogolfGetAllCustomersResponse()
        {
            Customers = new List<ChronogolfCustomer>();
        }
    }
}
