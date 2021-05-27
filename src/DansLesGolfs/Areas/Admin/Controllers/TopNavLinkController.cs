using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Admin.Controllers
{
    public class TopNavLinkController : BaseAdminController
    {
        public ActionResult Index()
        {
            ViewBag.Title = Resources.Resources.TopNavigationLink;
            ViewBag.TitleName = Resources.Resources.TopNavigationLink;
            ViewBag.ClassName = "top-nav-link";
            Breadcrumbs.Add(Resources.Resources.TopNavigationLink, "~/Admin/TopNavLink");
            InitBreadcrumbs();

            ViewBag.TopNavLinks = DataAccess.GetAllTopNavLinks();

            return View();
        }

        [HttpPost]
        public ActionResult SaveTopNavLinks(int[] ids, string[] linkUrls, string[] imageUrls, int[] deletedTopNavLinks)
        {
            try
            {
                if (deletedTopNavLinks != null && deletedTopNavLinks.Length > 0)
                {
                    for (int i = 0, n = deletedTopNavLinks.Length; i < n; i++)
                    {
                        DataAccess.DeleteTopNavLink(deletedTopNavLinks[i]);
                    }
                }

                if (ids != null && ids.Length > 0)
                {
                    DateTime fromDate = DateTime.Today;
                    DateTime toDate = DateTime.Today;
                    for (int i = 0, n = ids.Length; i < n; i++)
                    {
                        DataAccess.SaveTopNavLink(ids[i], linkUrls[i], imageUrls[i], i);
                    }
                }

                RefreshTopNavLinksCache();

                return Json(new
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }


        [HttpPost]
        public ActionResult DeleteTopNavLink(int? id = 0)
        {
            try
            {
                id = id.HasValue ? id.Value : 0;
                DataAccess.DeleteTopNavLink(id.Value);
                return Json(new
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public ActionResult GetAllTopNavLinks()
        {
            try
            {
                List<TopNavLink> list = DataAccess.GetAllTopNavLinks();
                string html = RenderViewToString("~/Views/_Shared/UC/Admin/TopNavLink/UCTopNavLinksList.cshtml", list);
                return Json(new
                {
                    isSuccess = true,
                    content = html
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public ActionResult GetPreviewLinks(string[] imageUrls)
        {
            try
            {
                if ((imageUrls != null && imageUrls.Length > 0))
                {
                    List<TopNavLink> list = new List<TopNavLink>();
                    foreach (string imageUrl in imageUrls)
                    {
                        list.Add(new TopNavLink()
                        {
                            TopNavLinkId = 0,
                            LinkUrl = string.Empty,
                            ImageUrl = imageUrl,
                            ListNo = 0
                        });
                    }
                    string html = RenderViewToString("~/Views/_Shared/UC/Admin/TopNavLink/UCTopNavLinksList.cshtml", list);
                    return Json(new
                    {
                        isSuccess = true,
                        content = html
                    });
                }
                else
                {
                    throw new Exception("Parameter can't be null.");
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        
        private void RefreshTopNavLinksCache()
        {
            List<TopNavLink> topNavLinks = DataAccess.GetAllTopNavLinks();
            System.Runtime.Caching.MemoryCache.Default.Remove("DLGTopNavLinks");
            System.Runtime.Caching.MemoryCache.Default.Add("DLGTopNavLinks", topNavLinks, DateTime.Now.AddYears(1));
        }

        [HttpPost]
        public ActionResult Reorder(int[] ids)
        {
            try
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    DataAccess.SaveTopNavLinkListNo(ids[i], i);
                }
                return Json(new
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

    }
}
