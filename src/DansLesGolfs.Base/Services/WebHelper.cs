using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace DansLesGolfs.Base
{
    public static class WebHelper
    {
        #region Slug Generation
        /// <summary>
        /// Generate slug from specific text.
        /// </summary>
        /// <param name="text">Text that you want to generate slug.</param>
        /// <returns>Slug that is generated from specified text.</returns>
        public static string GenerateSlug(string text)
        {
            string slug = RemoveAccent(text).Trim().ToLower();
            // Invalid character.
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "").Trim();
            // Convert multiple whitespace into one space.
            slug = Regex.Replace(slug, @"\s+", " ");
            // Replace space with hyphens.
            slug = Regex.Replace(slug, @"\s", "-");
            slug = Regex.Replace(slug, @"\-+", "-");
            return slug;
        }

        /// <summary>
        /// Remove accent from text.
        /// </summary>
        /// <param name="text">Text that you want to remove accent.</param>
        /// <returns>Text that is removed accent.</returns>
        private static string RemoveAccent(string text)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
        #endregion

        #region Communication
        public static dynamic Post(string url, dynamic data)
        {
            dynamic result = default(dynamic);
            return result;
        }
        #endregion

        #region SendRequest
        public static string SendRequest(string method = "GET", string url = "", Dictionary<string, string> parameters = null, string contentType = "text/html")
        {
            string contentStr = string.Empty;
            string queryString = method.Equals("GET", StringComparison.InvariantCultureIgnoreCase) ? GetQueryString(parameters) : String.Empty;
            url = url.Replace("?", "");
            var webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url + queryString);
            webRequest.KeepAlive = false;
            webRequest.Method = method;
            webRequest.ProtocolVersion = System.Net.HttpVersion.Version10;
            webRequest.ContentType = contentType;

            if (!method.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
            {
                byte[] postData = new byte[0];
                if (parameters != null && parameters.Any())
                {
                    postData = Encoding.ASCII.GetBytes(GetQueryString(parameters).Replace("?", ""));
                }
                System.IO.Stream requestStream = webRequest.GetRequestStream();
                requestStream.Write(postData, 0, postData.Length);
                requestStream.Close();
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
                    using (var reader = new System.IO.StreamReader(content, Encoding.UTF8))
                    {
                        contentStr = reader.ReadToEnd();
                    }
                }
            }

            return contentStr;
        }
        #endregion

        #region GetQueryString
        private static string GetQueryString(Dictionary<string, string> parameters)
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

        #region GetUserPlatform
        public static string GetUserPlatform(HttpRequestBase request)
        {
            var ua = request.UserAgent;

            if (ua.Contains("Android"))
                return string.Format("Android {0}", GetMobileVersion(ua, "Android"));

            if (ua.Contains("iPad"))
                return string.Format("iPad OS {0}", GetMobileVersion(ua, "OS"));

            if (ua.Contains("iPhone"))
                return string.Format("iPhone OS {0}", GetMobileVersion(ua, "OS"));

            if (ua.Contains("Linux") && ua.Contains("KFAPWI"))
                return "Kindle Fire";

            if (ua.Contains("RIM Tablet") || (ua.Contains("BB") && ua.Contains("Mobile")))
                return "Black Berry";

            if (ua.Contains("Windows Phone"))
                return string.Format("Windows Phone {0}", GetMobileVersion(ua, "Windows Phone"));

            if (ua.Contains("Mac OS"))
                return "Mac OS";

            if (ua.Contains("Windows NT 5.1") || ua.Contains("Windows NT 5.2"))
                return "Windows XP";

            if (ua.Contains("Windows NT 6.0"))
                return "Windows Vista";

            if (ua.Contains("Windows NT 6.1"))
                return "Windows 7";

            if (ua.Contains("Windows NT 6.2"))
                return "Windows 8";

            if (ua.Contains("Windows NT 6.3"))
                return "Windows 8.1";

            if (ua.Contains("Windows NT 10"))
                return "Windows 10";

            //fallback to basic platform:
            return request.Browser.Platform + (ua.Contains("Mobile") ? " Mobile " : "");
        }
        #endregion

        #region GetMobileVersion
        public static String GetMobileVersion(string userAgent, string device)
        {
            var temp = userAgent.Substring(userAgent.IndexOf(device) + device.Length).TrimStart();
            var version = string.Empty;

            foreach (var character in temp)
            {
                var validCharacter = false;
                int test = 0;

                if (Int32.TryParse(character.ToString(), out test))
                {
                    version += character;
                    validCharacter = true;
                }

                if (character == '.' || character == '_')
                {
                    version += '.';
                    validCharacter = true;
                }

                if (validCharacter == false)
                    break;
            }

            return version;
        }
        #endregion
    }
}
