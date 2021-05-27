using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace DansLesGolfs.Base.Revervation
{
    public class SoapHelper
    {
        /// <summary>
        /// Execute a Soap WebService call
        /// </summary>
        public static dynamic Execute(string url, string soapXml)
        {
            string soapResult = string.Empty;
            HttpWebRequest request = CreateWebRequest(url);
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(soapXml);

            using (Stream stream = request.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                    var xDoc = XDocument.Load(rd);
                    return ExpandoObjectHelper.Parse(default(dynamic), xDoc.Elements().First());
                }
            }


            return default(dynamic);
        }

        /// <summary>
        /// Create a soap webrequest to [Url]
        /// </summary>
        /// <returns></returns>
        private static HttpWebRequest CreateWebRequest(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
    }
}