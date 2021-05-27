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
using System.Globalization;
using Newtonsoft.Json;

namespace DansLesGolfs.Data
{
    public partial class PrimaDataAccess
    {
        #region Members
        private string apiKey = "";
        private string clubKey = "";
        private string url = "http://api.primagolf.fr/v1/";
        #endregion

        #region Properties
        public string APIKey { get { return apiKey; } private set { apiKey = value; } }
        public string ClubKey { get { return clubKey; } private set { clubKey = value; } }
        #endregion

        #region Constructor
        public PrimaDataAccess(string apiKey, string clubKey)
        {
            this.apiKey = apiKey;
            this.clubKey = clubKey;
        }
        #endregion

        #region Private Methods
        private string SendRequest(string method = "GET", string path = "", Dictionary<string, object> parameters = null)
        {
            string contentStr = string.Empty;
            string queryString = method.Equals("GET", StringComparison.InvariantCultureIgnoreCase) ? GetQueryString(parameters) : String.Empty;
            url = url.Replace("?", "");
            var webRequest = (HttpWebRequest)WebRequest.Create(url + path + queryString);
            webRequest.KeepAlive = false;
            webRequest.Method = method;
            webRequest.ProtocolVersion = HttpVersion.Version10;
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add("API-Key", this.apiKey);
            webRequest.Headers.Add("Club-Key", this.clubKey);

            if (method.Equals("POST", StringComparison.InvariantCultureIgnoreCase) && parameters != null && parameters.Any())
            {
                string postData = JsonConvert.SerializeObject(parameters);
                webRequest.ContentLength = postData.Length;

                byte[] postDataBytes = Encoding.ASCII.GetBytes(postData);
                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    requestStream.Write(postDataBytes, 0, postDataBytes.Length);
                }
            }

            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            using (var response = webRequest.EndGetResponse(asyncResult))
            {
                using (var content = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(content, Encoding.GetEncoding("ISO-8859-1")))
                    {
                        contentStr = reader.ReadToEnd();
                    }
                }
            }

            return contentStr;
        }

        private string GetQueryString(Dictionary<string, object> parameters)
        {
            string queryStr = string.Empty;
            List<string> paramList = new List<string>();

            if (parameters != null && parameters.Any())
            {
                foreach (var entry in parameters)
                {
                    paramList.Add(entry.Key + "=" + entry.Value);
                }
            }

            queryStr = "?" + String.Join("&", paramList);

            return queryStr;
        }
        #endregion

        #region GetTeeTimes
        public List<PrimaTeeTime> GetTeeTimes(DateTime date, int? gameTypeFilter, out string msg)
        {
            List<PrimaTeeTime> list = new List<PrimaTeeTime>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string dateStr = date.ToString("yyyyMMdd");

            parameters.Add("date", dateStr); // Reservation Date.

            string content = SendRequest("GET", "courses/teetimes", parameters);
            dynamic obj = JsonConvert.DeserializeObject(content);

            if (obj.code == "200" && obj.data != null && obj.data.courses != null)
            {
                msg = string.Empty;
                foreach (var it in (IEnumerable<dynamic>)obj.data.courses)
                {
                    string id = DataManager.ToString(it.id);
                    string name = DataManager.ToString(it.name);
                    PrimaGameType gameType = DataManager.ToString(it.type) == "1" ? PrimaGameType.Holes9 : PrimaGameType.Holes18;
                    string tempTime, tempTime9In, tempTime9Out;
                    if (it.teetimes != null)
                    {
                        foreach (var teeTime in (IEnumerable<dynamic>)it.teetimes)
                        {
                            tempTime = DataManager.ToString(teeTime.time);
                            tempTime9In = DataManager.ToString(teeTime.time_9in);
                            tempTime9Out = DataManager.ToString(teeTime.time_9out);

                            list.Add(new PrimaTeeTime()
                            {
                                ID = id,
                                Name = name,
                                GameType = gameType,
                                RateBase = DataManager.ToDecimal(teeTime.init_price) / 100m,
                                Discount = DataManager.ToInt(teeTime.discount),
                                Price = DataManager.ToDecimal(teeTime.price) / 100m,
                                Availability = DataManager.ToInt(teeTime.availability),
                                MaxSlot = 4,
                                ReservationDateTime = DataManager.ToDateTime(dateStr + tempTime, "yyyyMMddHHmm"),
                                ReservationDateTime9In = String.IsNullOrEmpty(tempTime9In) ? null : (DateTime?)DataManager.ToDateTime(dateStr + tempTime9In, "yyyyMMddHHmm"),
                                ReservationDateTime9Out = String.IsNullOrEmpty(tempTime9Out) ? null : (DateTime?)DataManager.ToDateTime(dateStr + tempTime9Out, "yyyyMMddHHmm")
                            });
                        }
                    }
                }
            }
            else if (obj.message != null)
            {
                msg = DataManager.ToString(obj.message);
            }
            else
            {
                msg = Resources.Resources.CannotGetTeeTimesFromPrimaGolfRightNow;
            }

            if (gameTypeFilter.HasValue)
            {
                PrimaGameType gtFilter = gameTypeFilter.Value == 1 ? PrimaGameType.Holes9 : PrimaGameType.Holes18;
                list = list.Where(it => it.GameType == gtFilter).ToList();
            }

            return list.OrderBy(it => it.ReservationDateTime).ThenBy(it => it.Name).ThenBy(it => it.GameType).ToList();
        }
        #endregion

        #region Lock
        /// <summary>
        /// Lock TeeTime for 15 minutes
        /// </summary>
        /// <param name="courseId">Course ID</param>
        /// <param name="date">Reservation DateTime</param>
        /// <param name="numberOfPlayers">Nubmer of Players</param>
        /// <returns>TeeTime's Lock Code</returns>
        public string Lock(string courseId, DateTime date, int numberOfPlayers, out string msg)
        {
            string lockCode = string.Empty;

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("date", date.ToString("yyyyMMdd")); // Reservation Date.
            parameters.Add("time", date.ToString("HHmm")); // Reservation Date.
            parameters.Add("course_id", courseId); // Reservation Date.
            parameters.Add("n_players", numberOfPlayers.ToString()); // Reservation Date.

            string content = SendRequest("POST", "bookings/lock", parameters);
            dynamic obj = JsonConvert.DeserializeObject(content);

            if (obj.code == "200" && obj.data != null)
            {
                lockCode = DataManager.ToString(obj.data.lock_code);
                msg = string.Empty;
            }
            else if (obj.message != null)
            {
                msg = DataManager.ToString(obj.message);
            }
            else
            {
                msg = Resources.Resources.TeeTimeAlreadyLocked;
            }

            return lockCode;
        }
        #endregion

        public bool Confirm(string courseId, DateTime reserveDate, int numberOfPlayers, string lockCode, User user, DateTime? reserveDateTime9In, out string[] bookingIds, out string msg)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            List<string> ids = new List<string>();

            parameters.Add("course_id", courseId); // Course ID.
            parameters.Add("name", user.FullName); // Name of person who takes the reservation.
            parameters.Add("date", reserveDate.ToString("yyyyMMdd")); // Reserve date.
            parameters.Add("time", reserveDate.ToString("HHmm")); // Reserve time.
            parameters.Add("n_players", numberOfPlayers.ToString()); // Reserve time.
            parameters.Add("lock_code", lockCode); // Reserve time.
            if (reserveDateTime9In.HasValue)
            {
                parameters.Add("time_9in", reserveDateTime9In.Value.ToString("HHmm")); // Reserve time 9 In (for 18 holes).
            }

            List<Dictionary<string, object>> playersList = new List<Dictionary<string, object>>();
            for (int i = 0; i < numberOfPlayers; i++)
            {
                Dictionary<string, object> player = new Dictionary<string, object>();
                player.Add("first_name", user.FirstName);
                player.Add("last_name", user.LastName);
                playersList.Add(player);
            }
            parameters.Add("players", playersList);

            string content = SendRequest("POST", "bookings/confirm", parameters);

            dynamic obj = JsonConvert.DeserializeObject(content);

            if ((obj.code == "200" || obj.code == "201" || obj.status == "success") && obj.data != null && obj.data.booking_ids != null)
            {
                foreach (string id in obj.data.booking_ids)
                {
                    ids.Add(id);
                }
                msg = string.Empty;
            }
            else if (obj.message != null)
            {
                msg = DataManager.ToString(obj.message);
            }
            else
            {
                msg = Resources.Resources.TeeTimeAlreadyLocked;
            }

            bookingIds = ids.ToArray();

            return bookingIds != null && bookingIds.Any();
        }
    }

    public class PrimaTeeTime
    {
        public DateTime ReservationDateTime { get; set; }
        public DateTime? ReservationDateTime9In { get; set; }
        public DateTime? ReservationDateTime9Out { get; set; }
        public PrimaGameType GameType { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public decimal RateBase { get; set; }
        public int Discount { get; set; }
        public decimal Price { get; set; }
        public int Availability { get; set; }
        public bool IsAvailable
        {
            get { return Availability > 0; }
        }
        public int MaxSlot { get; set; }
    }

    public enum PrimaGameType
    {
        Holes9 = 1,
        Holes18 = 2
    }
}
