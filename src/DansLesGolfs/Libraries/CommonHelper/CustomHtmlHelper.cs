using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace System.Web.Mvc
{
    public static partial class CustomHtmlHelper
    {
        private static UrlHelper Url
        {
            get
            {
                return new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
            }
        }
        #region Drop Down List
        public static MvcHtmlString DropDownList(this HtmlHelper html, string name, SelectList values, bool canEdit)
        {
            if (canEdit)
            {
                return html.DropDownList(name, values);
            }
            return html.DropDownList(name, values, new { disabled = "disabled" });
        }
        public static MvcHtmlString DropDownList(this HtmlHelper html, string name, IEnumerable<SelectListItem> values, bool canEdit)
        {
            if (canEdit)
            {
                return html.DropDownList(name, values);
            }
            return html.DropDownList(name, values, new { disabled = "disabled" });
        } 
        #endregion

        public static string GetBodyClasses(this HtmlHelper html)
        {
            string systemBodyClasses = html.ViewBag.SystemBodyClasses;
            string bodyClasses = html.ViewBag.BodyClasses;
            string output = !String.IsNullOrEmpty(systemBodyClasses) && !String.IsNullOrWhiteSpace(systemBodyClasses) ? systemBodyClasses : string.Empty;
            if (!String.IsNullOrEmpty(bodyClasses) && !String.IsNullOrWhiteSpace(bodyClasses))
            {
                output = !String.IsNullOrEmpty(output) ? output + " " + bodyClasses : bodyClasses;
            }
            return output.Trim();
        }

        public static int GetLoggedOnUserType(this HtmlHelper html)
        {
            return DataManager.ToInt(System.Web.HttpContext.Current.Session["LogonUserType"]);
        }

        public static string ResolveServerUrl(string serverUrl, bool forceHttps = false)
        {
            if (serverUrl.IndexOf("://") > -1)
                return serverUrl;

            string newUrl = serverUrl;
            Uri originalUri = System.Web.HttpContext.Current.Request.Url;
            newUrl = (forceHttps ? "https" : originalUri.Scheme) +
                "://" + originalUri.Authority + newUrl;
            return newUrl;
        }

        public static string T(this HtmlHelper html, string text)
        {
            return text;
        }

        public static string GetResellerItemImageUrl(this HtmlHelper html, int userId, ItemImage img, string size = "original")
        {
            string imageUrl = string.Empty;
            if (String.IsNullOrEmpty(img.BaseName) || String.IsNullOrWhiteSpace(img.BaseName))
            {
                imageUrl = "~/Assets/Front/img/no-image-300x400.png";
            }
            else
            {
                imageUrl = "~/Uploads/Resellers/" + userId + "/Items/" + img.BaseName;
                switch (size)
                {
                    case "thumbnail":
                        imageUrl += "_t";
                        break;
                    case "small":
                        imageUrl += "_s";
                        break;
                    case "medium":
                        imageUrl += "_m";
                        break;
                    case "large":
                        imageUrl += "_l";
                        break;
                }
                imageUrl += img.FileExtension;
            }
            return Url.Content(imageUrl);
        }

        public static string GetFullResellerItemImageUrl(this HtmlHelper html, int userId, ItemImage img, string size = "original")
        {
            return ResolveServerUrl(GetResellerItemImageUrl(html, userId, img, size));
        }

        public static string GetItemImageUrl(this HtmlHelper html, long itemId, ItemImage img, string size = "original", string productType = "", int? customWidth = null, int? customHeight = null)
        {
            string imageUrl = string.Empty;
            if (String.IsNullOrEmpty(img.BaseName) || String.IsNullOrWhiteSpace(img.BaseName))
            {
                imageUrl = "~/Assets/Front/img/no-image-300x400.png";
            }
            else
            {
                if (!string.IsNullOrEmpty(productType) && DataManager.ToInt(productType) > 0)
                {
                    imageUrl = "~/Uploads/Cards/" + itemId + "/" + img.ImageName;
                }
                else
                {
                    imageUrl = "~/Uploads/Products/" + itemId + "/" + img.BaseName;
                    switch (size.ToLower())
                    {
                        case "thumbnail":
                            imageUrl += "_t";
                            break;
                        case "small":
                            imageUrl += "_s";
                            break;
                        case "medium":
                            imageUrl += "_m";
                            break;
                        case "large":
                            imageUrl += "_l";
                            break;
                    }
                    imageUrl += img.FileExtension;
                }
            }
            if (customWidth.HasValue && customWidth.HasValue)
            {
                return Url.RouteUrl("Thumbnail", new
                {
                    path = imageUrl.Replace("~/", "").Replace("/", "||"),
                    w = customWidth.Value,
                    h = customHeight.Value
                });
            }
            else
            {
                return Url.Content(imageUrl);
            }

        }

        public static string GetFullItemImageUrl(this HtmlHelper html, long itemId, ItemImage img, string size = "original", string productType = "")
        {
            return ResolveServerUrl(GetItemImageUrl(html, itemId, img, size, productType));
        }

        public static string GetSiteImageUrl(this HtmlHelper html, long siteId, SiteImage img, string size = "original", int? customWidth = null, int? customHeight = null)
        {
            string imageUrl = "~/Uploads/Sites/" + siteId + "/" + img.BaseName;
            switch (size)
            {
                case "thumbnail":
                    imageUrl += "_t";
                    break;
                case "small":
                    imageUrl += "_s";
                    break;
                case "medium":
                    imageUrl += "_m";
                    break;
                case "large":
                    imageUrl += "_l";
                    break;
            }
            imageUrl += img.FileExtension;
            if (customWidth.HasValue && customWidth.HasValue)
            {
                return Url.RouteUrl("Thumbnail", new
                {
                    path = imageUrl.Replace("~/", "").Replace("/", "||"),
                    w = customWidth.Value,
                    h = customHeight.Value
                });
            }
            else
            {
                return Url.Content(imageUrl);
            }
        }

        public static string GetFullSiteImageUrl(this HtmlHelper html, long siteId, SiteImage img, string size = "original")
        {
            return ResolveServerUrl(GetSiteImageUrl(html, siteId, img, size));
        }

        public static string GetBrandImageUrl(this HtmlHelper html, Brand brand)
        {
            string imageUrl = string.Empty;
            if (String.IsNullOrEmpty(brand.BrandImage) || String.IsNullOrWhiteSpace(brand.BrandImage))
            {
                imageUrl = "~/Assets/Front/img/no-image-300x400.png";
            }
            else
            {
                imageUrl = "~/Uploads/Brands/" + brand.BrandImage;
            }
            return Url.Content(imageUrl);
        }

        public static string GetFullBrandImageUrl(this HtmlHelper html, Brand brand)
        {
            return ResolveServerUrl(GetBrandImageUrl(html, brand));
        }

        public static string GetPlaceHolderImageUrl(this HtmlHelper html, int width, int height, string text = "", string bgcolor = "000", string fgcolor = "fff")
        {
            return @"http://placehold.it/" + width + "x" + height + ".png/" + bgcolor + "/" + fgcolor + (!String.IsNullOrEmpty(text.Trim()) ? "&text=" + text.Trim() : "");
        }

        public static string MetaTitle(this HtmlHelper html, string title)
        {
            string siteName = System.Configuration.ConfigurationManager.AppSettings["SiteName"];
            if (String.IsNullOrEmpty(title.Trim()))
            {
                string siteDesc = System.Configuration.ConfigurationManager.AppSettings["SiteDescription"];
                if (String.IsNullOrEmpty(siteDesc))
                {
                    return siteName.Trim();
                }
                else
                {
                    return siteName.Trim() + " - " + siteDesc.Trim();
                }
            }
            else
            {
                return title.Trim() + " | " + siteName.Trim();
            }
        }

        public static string ItemUrl(this HtmlHelper html, Item item)
        {
            return String.IsNullOrEmpty(item.ItemSlug.Trim()) ? Url.Content("~/Item?id=" + item.ItemId) : Url.Content("~/Item/" + item.ItemSlug.Trim());
        }

        public static string ItemName(this HtmlHelper html, Item item)
        {
            return (item.ItemLangs != null && item.ItemLangs.Any()) ? item.ItemLangs[0].ItemName : item.ItemCode;
        }

        public static MvcHtmlString ItemPrice(this HtmlHelper html, Item item, string notAvailableText = "Not available", int qty = 1)
        {
            string priceHtml = string.Empty;

            if (qty < 1)
                qty = 1;

            decimal p = item.Price * qty;
            decimal sp = item.SpecialPrice * qty;
            decimal pp = item.PeriodPrice * qty;

            if (sp > 0)
            {
                if (pp > 0)
                {
                    if (sp < pp)
                    {
                        priceHtml += "<span class=\"regular-pricing\"><span class=\"regular-price-number\">" + pp.ToString("###0.00") + "</span> &euro;</span>";
                        priceHtml += "<span class=\"item-pricing\"><span class=\"item-price-number\">" + sp.ToString("###0.00") + "</span> &euro;</span>";
                    }
                    else
                    {
                        priceHtml += "<span class=\"regular-pricing\"><span class=\"regular-price-number\">" + sp.ToString("###0.00") + "</span> &euro;</span>";
                        priceHtml += "<span class=\"item-pricing\"><span class=\"item-price-number\">" + pp.ToString("###0.00") + "</span> &euro;</span>";
                    }
                }
                else
                {
                    if (sp < p)
                    {
                        priceHtml += "<span class=\"regular-pricing\"><span class=\"regular-price-number\">" + p.ToString("###0.00") + "</span> &euro;</span>";
                        priceHtml += "<span class=\"item-pricing\"><span class=\"item-price-number\">" + sp.ToString("###0.00") + "</span> &euro;</span>";
                    }
                    else
                    {
                        priceHtml += "<span class=\"regular-pricing\"><span class=\"regular-price-number\">" + sp.ToString("###0.00") + "</span> &euro;</span>";
                        priceHtml += "<span class=\"item-pricing\"><span class=\"item-price-number\">" + p.ToString("###0.00") + "</span> &euro;</span>";
                    }
                }
            }
            else if (pp > 0)
            {
                if (p > 0)
                {
                    if (pp < p)
                    {
                        priceHtml += "<span class=\"regular-pricing\"><span class=\"regular-price-number\">" + p.ToString("###0.00") + "</span> &euro;</span>";
                        priceHtml += "<span class=\"item-pricing\"><span class=\"item-price-number\">" + pp.ToString("###0.00") + "</span> &euro;</span>";
                    }
                    else
                    {
                        priceHtml += "<span class=\"regular-pricing\"><span class=\"regular-price-number\">" + pp.ToString("###0.00") + "</span> &euro;</span>";
                        priceHtml += "<span class=\"item-pricing\"><span class=\"item-price-number\">" + p.ToString("###0.00") + "</span> &euro;</span>";
                    }
                }
                else
                {
                    if (pp > 0)
                    {
                        priceHtml += "<span class=\"item-pricing\"><span class=\"item-price-number\">" + pp.ToString("###0.00") + "</span> &euro;</span>";
                    }
                    else
                    {
                        priceHtml += "<span class=\"item-pricing\"><span class=\"item-price-number\">" + notAvailableText + "</span></span>";
                    }
                }
            }
            else
            {
                if (p > 0)
                {
                    priceHtml += "<span class=\"item-pricing\"><span class=\"item-price-number\">" + p.ToString("###0.00") + "</span> &euro;</span>";
                }
                else
                {
                    priceHtml += "<span class=\"item-pricing\"><span class=\"item-price-number\">" + notAvailableText + "</span></span>";
                }
            }

            return MvcHtmlString.Create(priceHtml);
        }

        public static MvcHtmlString ItemFinalPrice(this HtmlHelper html, Item item)
        {
            string priceHtml = string.Empty;

            if (item.ItemTypeId != (int)ItemType.Type.StayPackage && item.HasSpecialPrice)
            {
                priceHtml = item.SpecialPrice.ToString("#,##0") + "&euro;";
            }
            else
            {
                priceHtml = item.Price.ToString("#,##0") + "&euro;";
            }
            return MvcHtmlString.Create(priceHtml);
        }

        public static string ItemClasses(this HtmlHelper html, Item item)
        {
            string classes = "product ";
            switch (item.ItemTypeId)
            {
                case 1: classes += "reseller-product";
                    break;
                case 2: classes += "green-fee";
                    break;
                case 3: classes += "stay-package";
                    break;
                case 4: classes += "golf-lesson";
                    break;
                case 5: classes += "driving-range";
                    break;
            }
            return classes;
        }

        public static MvcHtmlString Pagination(this HtmlHelper html, int page, int totalPages, string baseUrl = "")
        {
            if (totalPages < 2)
                return MvcHtmlString.Create("");

            string absoluteUrl = String.Empty;
            if (String.IsNullOrEmpty(baseUrl))
            {
                absoluteUrl = HttpContext.Current.Request.Url.AbsoluteUri;
                absoluteUrl = Regex.Replace(absoluteUrl, "page=\\d", "page={page}");
                if (!absoluteUrl.Contains("page={page}"))
                {
                    if (absoluteUrl.Contains('?'))
                    {
                        absoluteUrl += "&page={page}";
                    }
                    else
                    {
                        absoluteUrl += "?page={page}";
                    }
                }
            }
            else
            {
                absoluteUrl = Url.Content(baseUrl);
            }
            string pager = "<ul class=\"pagination\">";
            for (int i = 1; i <= totalPages; i++)
            {
                pager += "<li>";
                pager += "<a href=\"" + Regex.Replace(absoluteUrl, "(\\{page\\})", i.ToString(), RegexOptions.IgnoreCase) + "\"";
                if (page == i)
                {
                    pager += " class=\"active\"";
                }
                pager += ">" + i.ToString() + "</a>";
                pager += "</li>";
            }
            pager += "</ul>";
            return MvcHtmlString.Create(pager);
        }

        public static string LanguageUrl(this HtmlHelper html, string culture)
        {
            string absoluteUri = HttpContext.Current.Request.Url.AbsoluteUri;
            string returnUrl = HttpContext.Current.Server.UrlEncode(absoluteUri);
            return Url.Content("~/Culture/SetCulture?culture=" + culture + "&returnUrl=" + returnUrl);
        }

        public static MvcHtmlString Json(this HtmlHelper html, object obj)
        {
            var js = new System.Web.Script.Serialization.JavaScriptSerializer();
            return MvcHtmlString.Create(js.Serialize(obj));
        }

        public static MvcHtmlString TextBoxLang<T>(this HtmlHelper html, string name, IEnumerable<T> objects, string propertyName, object htmlAttributes)
        {
            List<object> values = new List<object>();
            if (objects != null)
            {
                Type t = typeof(T);
                PropertyInfo pi = t.GetProperty(propertyName);
                if (pi != null)
                {
                    foreach (T o in objects)
                    {
                        values.Add(pi.GetValue(o));
                    }
                }
            }
            return TextBoxLang(html, name, values, htmlAttributes);
        }

        public static MvcHtmlString TextBoxLang(this HtmlHelper html, string name, List<object> values, object htmlAttributes)
        {
            string htmlStr = "<div class=\"textbox-lang\">";
            Type t = null;
            PropertyInfo[] props = null;
            bool hasClassAttribute = false;
            string culture = "";
            for (int i = 0, j = CultureHelper.Cultures.Length; i < j; i++)
            {
                culture = CultureHelper.Cultures[i];
                htmlStr += "<span class=\"textbox-lang-wrapper " + CultureHelper.GetNeutralCulture(culture) + "\"><input type=\"text\" id=\"" + name + "_" + CultureHelper.GetNeutralCulture(culture) + "\" name=\"" + name + "\"";
                if (i < values.Count && values[i] != null && !String.IsNullOrEmpty(values[i].ToString()))
                {
                    htmlStr += " value=\"" + values[i].ToString() + "\"";
                }
                if (htmlAttributes != null)
                {
                    t = htmlAttributes.GetType();
                    props = t.GetProperties();
                    foreach (PropertyInfo pi in props)
                    {
                        if (pi.Name.ToLower() == "class")
                        {
                            htmlStr += " class=\"textbox-lang " + CultureHelper.GetNeutralCulture(culture) + " " + pi.GetValue(htmlAttributes) + "\"";
                        }
                        else
                        {
                            htmlStr += " " + pi.Name + "=\"" + pi.GetValue(htmlAttributes) + "\"";
                        }
                    }
                }
                if (!hasClassAttribute)
                {
                    htmlStr += " class=\"textbox-lang " + CultureHelper.GetNeutralCulture(culture) + "\"";
                }
                htmlStr += " /></span>";
            }
            htmlStr += "</div>";
            return MvcHtmlString.Create(htmlStr);
        }

        public static MvcHtmlString TextAreaLang<T>(this HtmlHelper html, string name, IEnumerable<T> objects, string propertyName, object htmlAttributes)
        {
            List<object> values = new List<object>();
            if (objects != null)
            {
                Type t = typeof(T);
                PropertyInfo pi = t.GetProperty(propertyName);
                if (pi != null)
                {
                    foreach (T o in objects)
                    {
                        values.Add(pi.GetValue(o));
                    }
                }
            }
            return TextAreaLang(html, name, values, htmlAttributes);
        }

        public static MvcHtmlString TextAreaLang(this HtmlHelper html, string name, List<object> values, object htmlAttributes)
        {
            string htmlStr = "";
            Type t = null;
            PropertyInfo[] props = null;
            bool hasClassAttribute = false;
            string culture = "";
            for (int i = 0, j = CultureHelper.Cultures.Length; i < j; i++)
            {
                culture = CultureHelper.Cultures[i];
                htmlStr += "<span class=\"textarea-lang-wrapper " + CultureHelper.GetNeutralCulture(culture) + "\"><textarea type=\"text\" id=\"" + name + "_" + CultureHelper.GetNeutralCulture(culture) + "\" name=\"" + name + "\"";
                if (htmlAttributes != null)
                {
                    t = htmlAttributes.GetType();
                    props = t.GetProperties();
                    foreach (PropertyInfo pi in props)
                    {
                        if (pi.Name.ToLower() == "class")
                        {
                            htmlStr += " class=\"textarea-lang " + CultureHelper.GetNeutralCulture(culture) + " " + pi.GetValue(htmlAttributes) + "\"";
                        }
                        else
                        {
                            htmlStr += " " + pi.Name + "=\"" + pi.GetValue(htmlAttributes) + "\"";
                        }
                    }
                }
                if (!hasClassAttribute)
                {
                    htmlStr += " class=\"textarea-lang " + CultureHelper.GetNeutralCulture(culture) + "\"";
                }
                htmlStr += ">";
                if (i < values.Count && values[i] != null && !String.IsNullOrEmpty(values[i].ToString()))
                {
                    htmlStr += values[i].ToString();
                }
                htmlStr += "</textarea></span>";
            }
            return MvcHtmlString.Create(htmlStr);
        }

        public static MvcHtmlString GetAds(this HtmlHelper html, string adsetName)
        {
            string htmlStr = string.Empty;
            if (MemoryCache.Default["DLGAds"] != null)
            {
                List<Ad> ads = MemoryCache.Default["DLGAds"] as List<Ad>;
                if (ads != null)
                {
                    htmlStr = "<ul class=\"widget-ads-list\">";
                    var result = from it in ads
                                 where it.AdsetName.ToLower() == adsetName.ToLower()
                                 select it;

                    foreach (Ad ad in ads)
                    {
                        htmlStr += "<li class=\"ads ads-" + ad.AdsId + "\">";
                        htmlStr += "<a href=\"" + ad.LinkUrl + "\">";
                        htmlStr += "<img src=\"" + ad.ImageUrl + "\" />";
                        htmlStr += "</a></li>";
                    }

                    htmlStr += "</ul>";
                }
            }
            return MvcHtmlString.Create(htmlStr);
        }

        public static string ServerUrl(this UrlHelper url, string urlPath)
        {
            return new Uri(System.Web.HttpContext.Current.Request.Url, url.Content(urlPath)).ToString();
        }

        public static string ThumbUrl(this UrlHelper url, string path, int w = 1, int h = 1)
        {
            return url.Action("Index", "Thumb", new
            {
                path = path,
                w = w,
                h = h
            });
        }

    }
}