using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace DansLesGolfs.ECM
{
    public class ECMHelper
    {
        private const string ATTR_MAX_WIDTH = "640";

        /// <summary>
        /// Get fixed HTML string that ready to save.
        /// </summary>
        /// <param name="html">Original HTML</param>
        /// <returns>Fixed HTML String.</returns>
        public static string GetFixedHTML(string html)
        {
            var htmlDoc = GetHtmlDocument(html);
            FixingImageMaxWidth(htmlDoc);
            RemoveStyleFromVitalTags(htmlDoc);
            return GetStringFromHtmlDocument(htmlDoc);
        }

        private static string GetStringFromHtmlDocument(HtmlDocument htmlDoc)
        {
            string result = String.Empty;
            using (StringWriter sw = new StringWriter())
            {
                htmlDoc.Save(sw);
                result = sw.ToString();
            }
            return result;
        }

        /// <summary>
        /// Get HTML that ready to send to multiple email clients.
        /// </summary>
        /// <param name="html">Original HTML string.</param>
        /// <returns>Fixed and Ready-to-Send HTML String.</returns>
        public static string GetReadyToSendHTML(string html)
        {
            var htmlDoc = GetHtmlDocument(html);
            FixingImageMaxWidth(htmlDoc);
            RemoveStyleFromVitalTags(htmlDoc);
            var msoHtmlDoc = GetMicrosoftOutlookHTMLDocument(htmlDoc);
            html = CombineHtmlDocuments(htmlDoc, msoHtmlDoc);
            return html;
        }

        private static void RemoveStyleFromVitalTags(HtmlDocument htmlDoc)
        {
            var view = htmlDoc.GetElementbyId("view");
            if (view != null)
            {
                if (view.Attributes.Contains("style"))
                {
                    view.Attributes["style"].Remove();
                }

                if(view.ChildNodes != null)
                {
                    foreach(var child in view.ChildNodes)
                    {
                        if (child.Attributes.Contains("style"))
                        {
                            child.Attributes["style"].Remove();
                        }
                        if (child.Attributes.Contains("id"))
                        {
                            child.Attributes["id"].Remove();
                        }
                    }
                }
            }

            var unsub = htmlDoc.GetElementbyId("unsub");
            if (unsub != null)
            {
                if (unsub.Attributes.Contains("style"))
                {
                    unsub.Attributes["style"].Remove();
                }

                if (unsub.ChildNodes != null)
                {
                    foreach (var child in unsub.ChildNodes)
                    {
                        if (child.Attributes.Contains("style"))
                        {
                            child.Attributes["style"].Remove();
                        }
                        if (child.Attributes.Contains("id"))
                        {
                            child.Attributes["id"].Remove();
                        }
                    }
                }
            }
        }

        private static string CombineHtmlDocuments(HtmlDocument htmlDoc, HtmlDocument msoHtmlDoc)
        {
            // Fixing Pixel Resize in Microsoft Outlook 2007-2013
            string html = @"<!--[if gte mso 7]><xml>
              <o:OfficeDocumentSettings>
              <o:AllowPNG/>
              <o:PixelsPerInch>96</o:PixelsPerInch>
              </o:OfficeDocumentSettings>
            </xml><![endif]-->" + Environment.NewLine;

            // Add browse condition for Normal Browser.
            html += "<!--[if !mso]><!-- -->" + Environment.NewLine;
            html += GetStringFromHtmlDocument(htmlDoc) + Environment.NewLine;
            html += "<![endif]>" + Environment.NewLine;

            // Add browse condition for Microsoft Outlook.
            html += "<!--[if mso]>" + Environment.NewLine;
            html += GetStringFromHtmlDocument(msoHtmlDoc) + Environment.NewLine;
            html += "<![endif]-->" + Environment.NewLine;

            return html;
        }

        private static HtmlDocument GetMicrosoftOutlookHTMLDocument(HtmlDocument htmlDoc)
        {
            // Clone HTML Document.
            var msoHtmlDoc = new HtmlDocument();
            msoHtmlDoc.LoadHtml(htmlDoc.DocumentNode.InnerHtml);

            FixingMicrosoftOutlookTableWidth(msoHtmlDoc);

            return msoHtmlDoc;
        }

        private static void FixingMicrosoftOutlookTableWidth(HtmlDocument msoHtmlDoc)
        {
            FixingTableWidthForMicrosoftOutlook(msoHtmlDoc);
            FixingImageWidthForMicrosoftOutlook(msoHtmlDoc);
        }

        private static void FixingTableWidthForMicrosoftOutlook(HtmlDocument msoHtmlDoc)
        {
            var tables = msoHtmlDoc.DocumentNode.SelectNodes("//table");

            foreach (var table in tables)
            {
                var style = table.GetAttributeValue("style", "");
                var attributes = GetStyleAttributes(style);
                if (attributes.ContainsKey("max-width"))
                {
                    string maxWidth = attributes["max-width"];
                    table.SetAttributeValue("width", maxWidth.Replace("px", ""));

                    attributes.Remove("max-width");
                    if (attributes.ContainsKey("width"))
                    {
                        attributes.Remove("width");
                    }
                }
                else if (attributes.ContainsKey("width"))
                {
                    string width = attributes["width"];
                    table.SetAttributeValue("width", width.Replace("px", ""));
                    attributes.Remove("width");
                }

                table.SetAttributeValue("style", GetStyleValueFromAttributes(attributes));
            }
        }
        private static void FixingImageWidthForMicrosoftOutlook(HtmlDocument msoHtmlDoc)
        {
            var imgs = msoHtmlDoc.DocumentNode.SelectNodes("//img");

            foreach (var img in imgs)
            {
                var style = img.GetAttributeValue("style", "");
                var attributes = GetStyleAttributes(style);

                if (attributes.ContainsKey("max-width"))
                {
                    string maxWidth = attributes["max-width"];
                    attributes.Remove("max-width");
                    img.SetAttributeValue("width", maxWidth.Replace("px", ""));
                }

                if (attributes.ContainsKey("width"))
                {
                    string width = attributes["width"];
                    attributes.Remove("width");
                    img.SetAttributeValue("width", width.Replace("px", ""));
                }

                if (img.GetAttributeValue("width", "") == "")
                {
                    img.SetAttributeValue("width", ATTR_MAX_WIDTH);
                }
                else
                {
                    string width = img.GetAttributeValue("width", "");
                    if (width.IndexOf("%") > -1)
                    {
                        img.SetAttributeValue("width", ATTR_MAX_WIDTH);
                    }
                }

                if (attributes.ContainsKey("height"))
                {
                    attributes.Remove("height");
                }

                if (img.Attributes.Contains("height"))
                {
                    img.Attributes["height"].Remove();
                }

                img.SetAttributeValue("style", GetStyleValueFromAttributes(attributes));
            }
        }

        private static HtmlDocument FixingImageMaxWidth(HtmlDocument htmlDoc)
        {
            var imgTags = GetImgTags(htmlDoc);
            AddMaxWidthToImgTags(imgTags);
            return htmlDoc;
        }

        private static string ReplaceImagesInHTML(string html, ImgTagReplacement[] imgTags)
        {
            foreach (ImgTagReplacement img in imgTags)
            {
                html = html.Replace(img.OldTag, img.NewTag);
            }
            return html;
        }

        private static HtmlNode[] AddMaxWidthToImgTags(HtmlNode[] imgTags)
        {
            foreach (var img in imgTags)
            {
                var style = img.GetAttributeValue("style", string.Empty).Trim();

                var attributes = GetStyleAttributes(style);
                if (attributes.ContainsKey("max-width"))
                {
                    attributes["max-width"] = "100%";
                }
                else
                {
                    attributes.Add("max-width", "100%");
                }
                img.SetAttributeValue("style", GetStyleValueFromAttributes(attributes));

            }
            return imgTags;
        }

        private static string GetStyleValueFromAttributes(Dictionary<string, string> attributes)
        {
            return String.Join("; ", attributes.Select(a => String.Concat(a.Key, ": ", a.Value)).ToArray());
        }

        private static Dictionary<string, string> GetStyleAttributes(string style)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();

            if (!String.IsNullOrWhiteSpace(style))
            {
                foreach (var attribute in style.Split(';').Select(a => a.Trim()))
                {
                    var attributeParts = attribute.Split(':').Select(a => a.Trim());
                    if (attributeParts.Count() == 2)
                    {
                        attributes.Add(attributeParts.ElementAt(0), attributeParts.ElementAt(1));
                    }
                }
            }

            return attributes;
        }

        private static HtmlDocument GetHtmlDocument(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }

        private static HtmlNode[] GetImgTags(HtmlDocument htmlDoc)
        {
            return htmlDoc.DocumentNode.SelectNodes("//img").ToArray();
        }

        internal class ImgTagReplacement
        {
            public string OldTag { get; set; }
            public string NewTag { get; set; }

            public ImgTagReplacement(string tag)
            {
                OldTag = tag;
            }
        }
    }
}