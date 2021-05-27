using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DansLesGolfs.ECM.Controllers
{
    public class BaseAdminController : BaseController
    {
        #region Properties
        public virtual Dictionary<string, string> Breadcrumbs { get; set; } 
        #endregion

        public BaseAdminController()
            : base()
        {
            Breadcrumbs = new Dictionary<string, string>();
            CheckLogin();
        }

        private void CheckLogin()
        {
            if(!Auth.Check())
            {
                System.Web.HttpContext.Current.Response.Redirect("~/Login");
            }
            else if(System.Web.HttpContext.Current.Session["LogonUserType"] != null)
            {
                int userType = DataManager.ToInt(System.Web.HttpContext.Current.Session["LogonUserType"]);
                if (userType != UserType.Type.SuperAdmin && userType != UserType.Type.Admin && userType != UserType.Type.Staff && userType != UserType.Type.SiteManager)
                {
                    System.Web.HttpContext.Current.Response.Redirect("~/Login");
                }
            }
        }

        #region InitBreadcrumbs
        protected void InitBreadcrumbs()
        {
            ViewBag.Breadcrumbs = Breadcrumbs;
        }
        #endregion

        private Dictionary<string, string> GetPersonalizeData(User givenUser = null, long? siteId = null)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            User user = null;
            if (givenUser != null)
            {
                user = givenUser;
            }
            else if (Auth.User != null)
            {
                user = Auth.User;
            }
            else
            {
                user = new User();
            }

            if (user != null)
            {
                data.Add("{!name}", user.FullName);
                data.Add("{!firstname}", user.FirstName);
                data.Add("{!lastname}", user.LastName);
                data.Add("{!email}", user.Email);
                data.Add("{!phone}", user.Phone);
                data.Add("{!mobile}", user.MobilePhone);
                data.Add("{!description}", user.Remarks);
                data.Add("{!profession}", user.Career);
                data.Add("{!index}", user.Index.ToString());
                data.Add("{!field1}", user.CustomField1);
                data.Add("{!field2}", user.CustomField2);
                data.Add("{!field3}", user.CustomField3);
                data.Add("{!gender}", user.Gender == 0 ? Resources.Resources.Male : Resources.Resources.Female);
            }
            else
            {
                data.Add("{!name}", string.Empty);
                data.Add("{!firstname}", string.Empty);
                data.Add("{!lastname}", string.Empty);
                data.Add("{!email}", string.Empty);
                data.Add("{!description}", string.Empty);
                data.Add("{!profession}", string.Empty);
                data.Add("{!index}", string.Empty);
                data.Add("{!field1}", string.Empty);
                data.Add("{!field2}", string.Empty);
                data.Add("{!field3}", string.Empty);
                data.Add("{!phone}", string.Empty);
                data.Add("{!mobile}", string.Empty);
                data.Add("{!gender}", string.Empty);
            }

            return data;
        }

        protected string PersonalizeText(string text, Dictionary<string, string> additionData = null, User givenUser = null, long? siteId = null)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrWhiteSpace(text))
                return string.Empty;

            string result = text;
            Dictionary<string, string> data = GetPersonalizeData(givenUser, siteId);
            if (additionData != null)
            {
                foreach (var entry in additionData)
                {
                    if (data.ContainsKey(entry.Key))
                    {
                        data[entry.Key] = entry.Value;
                    }
                    else
                    {
                        data.Add(entry.Key, entry.Value);
                    }
                }
            }
            foreach (var entry in data)
            {
                result = result.Replace(entry.Key, entry.Value);
            }
            return result;
        }

        protected string PersonalizeTextNM(string text)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrWhiteSpace(text))
                return string.Empty;

            string result = text;

            if (result.IndexOf("{!unsubscribe}") >= 0 || result.IndexOf("{!unsubscribe_url}") >= 0)
            {
                result = result.Replace("{!unsubscribe}", "<a href=\"$$http://unsub.html$$\">" + Resources.Resources.Unsubscribe + "</a>");
                result = result.Replace("{!unsubscribe_url}", "$$http://unsub.html$$");
            }
            else
            {
                string unsubscribeLink = "<p style=\"font-size: 0.8em; text-align: center;\">Cet email a été envoyé à <a href=\"mailto:$$[record]EMAIL$$\">$$[record]EMAIL$$</a>, <a href=\"$$http://unsub.html$$\">Cliquez ici pour vous désabonner</a></p>";
                int beforeEndPosition = result.LastIndexOf("</body>");
                result = result.Insert(beforeEndPosition, unsubscribeLink);
            }
            result = result.Replace("{!web}", "$$http://clicview.html$$");
            result = Regex.Replace(result, "\\{\\!(\\w+)\\}", m => "$$[record]" + (m.Groups.Count > 1 && m.Captures.Count > 0 ? m.Groups[1].Value.ToUpper() : m.ToString().ToUpper()) + "$$");
            return result;
        }
        protected string InsertTrackingLinks(string html, long campaignId, long emailQueId, long siteId, long userId, string email, ref string unsubscribeMailTo, ref string unsubscribeUrl)
        {
            string id = DataProtection.Encrypt(campaignId + "|" + emailQueId);

            // Adding Tracking Links.
            Regex regex = new Regex("<a[^>]* href=\"([^\"]*)\"|<a[^>]* href='([^']*)'", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var matches = regex.Matches(html);
            string oldLink = string.Empty;
            string newLink = string.Empty;
            string replacement = string.Empty;
            foreach (Match m in matches)
            {
                if (m.Groups.Count == 3 && m.Captures.Count > 0 && m.Groups[1].Value != "#")
                {
                    oldLink = m.Groups[1].Value;
                    if (String.IsNullOrWhiteSpace(oldLink) || oldLink.StartsWith("{!"))
                        continue;

                    newLink = Url.ServerUrl("~/Track/Click?id=" + id + "&link=") + Server.UrlEncode(oldLink);
                    replacement = m.Value.Replace(oldLink, newLink);
                    html = html.Replace(m.Value, replacement);
                }
            }

            // On-web version.
            string onWebLink = System.Configuration.ConfigurationManager.AppSettings["OnWebLink"];
            onWebLink += "?cid=" + campaignId + "&uid=" + userId + "&email=" + email;
            html = html.Replace("{!web}", onWebLink);

            string unsubscribeLink = System.Configuration.ConfigurationManager.AppSettings["UnsubscribeLink"];
            string unsubscribeId = DataProtection.Encrypt(campaignId + "|" + emailQueId + "|" + siteId).Replace('=', '~');
            unsubscribeLink += "?id=" + unsubscribeId;
            if (!String.IsNullOrWhiteSpace(email))
            {
                unsubscribeLink += "&email=" + email;
            }
            unsubscribeMailTo = !String.IsNullOrWhiteSpace(unsubscribeMailTo) ? unsubscribeMailTo + "?subject=Unsubscribe|" + unsubscribeId : "";
            unsubscribeUrl = unsubscribeLink;
            html = html.Replace("{!unsubscribe_url}", unsubscribeUrl);
            html = html.Replace("{!unsubscribe}", String.Format("<a href=\"{0}\">désabonnés</a>", unsubscribeUrl));

            string newCode = string.Empty;

            // Force add unsubscribe link below of email.
            //newCode += "<div style=\"margin-top: 40px; text-align: center;font-size: 0.9em\">";
            //newCode += Resources.Resources.ThisEmailWasSentTo + " " + email + "<br />" + Environment.NewLine;
            //newCode += Resources.Resources.DontWantToReceiveThisTypeOfEmail + " <a href=\"" + unsubscribeLink + "\">désabonnés</a>" + Environment.NewLine;
            //newCode += "</div>";

            // Adding Tracking Open Email.
            var options = DataAccess.GetOptions("GoogleAnalyticsID");
            newCode += "<img src=\"" + Url.ServerUrl("~/Track/Open/" + id) + "\" id=\"tracking-open\" width=\"1\" height=\"1\" style=\"width:1px;height:1px;\" />";
            if (options != null && options.ContainsKey("GoogleAnalyticsID"))
            {
                string googleAnalyticsID = options["GoogleAnalyticsID"];
                if (!String.IsNullOrWhiteSpace(googleAnalyticsID))
                {
                    string analyticsTrackingCode = "http://www.google-analytics.com/collect?v=1&tid=" + googleAnalyticsID + "&cid=" + emailQueId + "&t=event&ec=email&ea=open&el=recipient_id&cs=newsletter&cm=email&cn=ECM";
                    newCode += "<img src=\"\" id=\"tracking-open\" width=\"1\" height=\"1\" style=\"width:1px;height:1px;\" />";
                }
            }

            // Find position before closing </body>
            int insertPosition = html.LastIndexOf("</body>");
            if(insertPosition < 0)
            {
                insertPosition = html.Length;
            }
            html = html.Insert(insertPosition, newCode);

            return html;
        }
    }
}
