using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Reseller.Controllers
{
    public class AdvertiseController : BaseResellerController
    {
        public ActionResult Manage()
        {
            ViewBag.Title = Resources.Resources.AdsManager;
            ViewBag.TitleName = Resources.Resources.AdsManager;
            ViewBag.ClassName = "ads-manager";
            Breadcrumbs.Add(Resources.Resources.AdsManager, "~/Reseller/Advertise/Manage");
            InitBreadcrumbs();

            ViewBag.Adsets = DataAccess.GetAllAdsets();

            return View();
        }

        [HttpPost]
        public ActionResult SaveAdset(string adset)
        {
            try
            {
                if (String.IsNullOrEmpty(adset) && String.IsNullOrWhiteSpace(adset))
                    throw new Exception(Resources.Resources.PleaseInputAdsetName);

                if (DataAccess.IsExistsAdsetName(adset))
                    throw new Exception(Resources.Resources.AdsetNameAlreadyInUse);

                int adsetId = DataAccess.AddAdset(adset);
                return Json(new
                {
                    isSuccess = true,
                    adsetId = adsetId
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
        public ActionResult DeleteAdset(int? adsetId = 0)
        {
            try
            {
                adsetId = adsetId.HasValue ? adsetId.Value : 0;
                DataAccess.DeleteAdset(adsetId.Value);
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
        public ActionResult GetAdsInAdset(int? adsetId = null)
        {
            try
            {
                adsetId = adsetId.HasValue ? adsetId.Value : 0;
                List<Ad> adsList = DataAccess.GetAdsByAdsetId(adsetId.Value);
                string html = RenderViewToString("~/Views/_Shared/UC/Reseller/Ads/UCAdsList.cshtml", adsList);
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
        public ActionResult GetPreviewAds(int? adsetId, string[] imageUrls)
        {
            try
            {
                adsetId = adsetId.HasValue ? adsetId.Value : 0;
                if ((adsetId.HasValue && adsetId.Value > 0) && (imageUrls != null && imageUrls.Length > 0))
                {
                    List<Ad> adsList = new List<Ad>();
                    foreach (string imageUrl in imageUrls)
                    {
                        adsList.Add(new Ad()
                        {
                            AdsId = 0,
                            AdsName = string.Empty,
                            LinkUrl = string.Empty,
                            ImageUrl = imageUrl,
                            AdsetId = adsetId.Value,
                            Active = true,
                            FromDate = DateTime.Today,
                            ToDate = DateTime.Today
                        });
                    }
                    string html = RenderViewToString("~/Views/_Shared/UC/Reseller/Ads/UCAdsList.cshtml", adsList);
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

        [HttpPost]
        public ActionResult SaveAds(int? adsetId, int[] adsIds, string[] adsNames, string[] linkUrls, string[] imageUrls, int[] deletedAds, string[] fromDates, string[] toDates)
        {
            try
            {
                if (!adsetId.HasValue)
                    throw new Exception("Please input adset id.");

                // Delete ads
                if (deletedAds != null && deletedAds.Length > 0)
                {
                    for (int i = 0, n = deletedAds.Length; i < n; i++)
                    {
                        DataAccess.DeleteAds(deletedAds[i]);
                    }
                }

                if (adsIds != null && adsIds.Length > 0)
                {
                    DateTime fromDate = DateTime.Today;
                    DateTime toDate = DateTime.Today;
                    for (int i = 0, n = adsIds.Length; i < n; i++)
                    {
                        fromDate = DataManager.ToDateTime(fromDates[i], "dd/MM/yyyy", DateTime.Today);
                        toDate = DataManager.ToDateTime(toDates[i], "dd/MM/yyyy", DateTime.Today);
                        DataAccess.SaveAds(adsIds[i], adsetId.Value, adsNames[i], linkUrls[i], imageUrls[i], i, fromDate, toDate);
                    }
                }

                RefreshAdsCache();

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

        private void RefreshAdsCache()
        {
            List<Ad> ads = DataAccess.GetAllAds();
            System.Runtime.Caching.MemoryCache.Default.Remove("DLGAds");
            System.Runtime.Caching.MemoryCache.Default.Add("DLGAds", ads, DateTime.Now.AddDays(1));
        }

        [HttpPost]
        public ActionResult ReorderAds(int[] adsIds)
        {
            try
            {
                for (int i = 0; i < adsIds.Length; i++)
                {
                    DataAccess.SaveAdsListNo(adsIds[i], i);
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

        [HttpPost]
        public ActionResult DeleteAds(int? adsId)
        {
            try
            {
                if (adsId.HasValue)
                {
                    DataAccess.DeleteAds(adsId.Value);
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
