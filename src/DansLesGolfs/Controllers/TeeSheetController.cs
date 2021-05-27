using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using DansLesGolfs.Base;
using DansLesGolfs.Models;
using System.Web.Script.Serialization;

namespace DansLesGolfs.Controllers
{
    public class TeeSheetController : BaseFrontController
    {
        int pageSize = 4;
        public TeeSheetController()
        {
            pageSize = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["RelatedGreenFeesPageSize"], 2);
        }

        public ActionResult Table(string slug)
        {
            Item item = null;
            slug = slug.Trim();
            if (String.IsNullOrEmpty(slug))
            {
                long id = DataManager.ToLong(Request.QueryString["id"], 0);
                if (id > 0)
                {
                    item = DataAccess.GetItem(id);
                }
                else
                {
                    return View("NotFound");
                }
            }
            else
            {
                slug = WebHelper.GenerateSlug(slug);
                item = DataAccess.GetItemBySlug(slug);
                if (item == null)
                {
                    return View("NotFound");
                }
            }

            LoadRelateItems(item);

            ViewBag.TeeSheetDate = DataManager.ToDateTime(Request.QueryString["date"], "dd/MM/yyyy", DateTime.Today);

            if (item.AlbatrosCourseId > 0)
            {
                return View("AlbatrosTeeSheet", item);
            }
            else
            {
                List<TeeSheet> teeSheets = DataAccess.GetTeeSheetsByItemId(item.ItemId);
                List<TeeSheetModel> simpleTeeSheets = GetSimpleTeeSheet(teeSheets);
                ViewBag.TeeSheetData = new JavaScriptSerializer().Serialize(simpleTeeSheets);

                return View(item);
            }

        }

        #region Private Methods
        private List<TeeSheetModel> GetSimpleTeeSheet(List<TeeSheet> teeSheets)
        {
            List<TeeSheetModel> list = new List<TeeSheetModel>();
            foreach (TeeSheet t in teeSheets)
            {
                list.Add(new TeeSheetModel()
                {
                    id = t.TeeSheetId,
                    day = t.TeeSheetDay ?? 0,
                    date = t.TeeSheetDate.HasValue ? t.TeeSheetDate.Value.ToString("yyyy-M-d") : DateTime.Today.ToString("yyyy-M-d"),
                    start = t.ToTime.HasValue ? t.ToTime.Value.Hours : 0,
                    end = t.ToTime.HasValue ? t.ToTime.Value.Hours : 0,
                    price = t.Price ?? 0,
                    discount = t.Discount ?? 0
                });
            }
            return list;
        }

        private void LoadRelateItems(Item item)
        {
            ProductsListModel relatedItemsModel = new ProductsListModel();
            pageSize = DataManager.ToInt(System.Configuration.ConfigurationManager.AppSettings["RelatedGreenFeesPageSize"], 2);
            relatedItemsModel.Items = DataAccess.GetItemsByItemTypeId(item.ItemTypeId, pageSize, item.ItemId, this.CultureId);
            relatedItemsModel.IsShowNoItemText = false;
            ViewBag.RelatedItems = relatedItemsModel;
        }
        #endregion
    }
}
