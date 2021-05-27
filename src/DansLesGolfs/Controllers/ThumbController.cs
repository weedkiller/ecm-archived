using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using System.Text;
using DansLesGolfs.Models;
using System.Drawing;
using System.Web.UI;
using System.IO;

namespace DansLesGolfs.Controllers
{
    public class ThumbController : Controller
    {
        [OutputCache(CacheProfile="ThumbnailCache",Location=OutputCacheLocation.Any)]
        public ActionResult Index(string path, int? w = 1, int? h = 1)
        {
            path = path.Replace("||", "/");
            if(path.IndexOf("~/") != 0)
            {
                path = "~/" + path;
            }
            path = Server.MapPath(path);
            Image i = null;
            try
            {
                i = Image.FromFile(path);

                FileInfo fileInfo = new FileInfo(path);

                HttpCachePolicy cachePolicy = System.Web.HttpContext.Current.Response.Cache;
                cachePolicy.SetCacheability(HttpCacheability.Public);
                cachePolicy.VaryByParams["p"] = true;
                cachePolicy.SetOmitVaryStar(true);
                cachePolicy.SetExpires(DateTime.Now + TimeSpan.FromDays(365));
                cachePolicy.SetValidUntilExpires(true);
                cachePolicy.SetLastModified(fileInfo.CreationTime);

                return new ImageResult(i, w.Value, h.Value);
            }
            catch (Exception ex)
            {
                i = new Bitmap(1, 1);
                return new ImageResult(i, w.HasValue ? w.Value : 1, h.HasValue ? h.Value : 1);
            }
            finally
            {
                if (i != null) i.Dispose();
            }
        }
    }
}
