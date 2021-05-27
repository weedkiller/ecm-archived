using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using DansLesGolfs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Admin.Controllers
{
    public class CouponController : BaseAdminCRUDController
    {
        #region Constructor
        public CouponController()
        {
            ObjectName = "Coupon";
            TitleName = Resources.Resources.Coupon;
            PrimaryKey = "CouponGroupId";

            // Define Column Names.
            ColumnNames.Add("CouponGroupName", Resources.Resources.CouponName);
            ColumnNames.Add("CouponsCount", Resources.Resources.Used + " / " + Resources.Resources.Created);
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<CouponGroup> models = DataAccess.GetAllCouponGroups(param);
            return models;
        }

        protected override void DoPrepareForm(int? id)
        {
            InitCategoriesList();
            InitCouponDropDownList();
            InitCouponUsageDropDownList();
            InitUsagePeriodTypeDropDownList();
        }

        protected override object DoPrepareNew()
        {
            return new CouponGroup()
            {
                Reduction = 0,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1),
                Coupons = new List<Coupon>(),
                Items = new List<Item>(),
            };
        }

        protected override object DoPrepareEdit(long id)
        {
            CouponGroup model = DataAccess.GetCouponGroup(id, this.CultureId);
            ViewBag.DiscountType = model.CouponUsageType;
            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            int id = DataManager.ToInt(Request.Form["id"]);
            CouponGroup model = null;
            if (id > 0)
            {
                model = DataAccess.GetCouponGroup(id);
                if (model == null)
                {
                    model = new CouponGroup();
                }
            }
            else
            {
                model = new CouponGroup();
            }
            model.CouponGroupId = id;
            model.CouponGroupName = DataManager.ToString(Request.Form["CouponGroupName"]).Trim();
            model.CouponGroupDesc = DataManager.ToString(Request.Form["CouponGroupDesc"]).Trim();
            model.StartDate = DataManager.ToDateTime(Request.Form["StartDate"], "dd/MM/yyyy");
            model.EndDate = DataManager.ToDateTime(Request.Form["EndDate"], "dd/MM/yyyy");
            model.CouponType = DataManager.ToInt(Request.Form["CouponType"]);
            model.CouponUsageType = DataManager.ToInt(Request.Form["CouponUsageType"]);
            model.UsagePeriodType = DataManager.ToInt(Request.Form["UsagePeriodType"]);
            model.Reduction = DataManager.ToDecimal(Request.Form["Reduction"]);
            model.MinimumAmount = DataManager.ToDecimal(Request.Form["MinimumAmount"]);

            model.TimesToUse = DataManager.ToInt(Request.Form["TimesToUse"], 1);
            if (model.TimesToUse < 1)
                model.TimesToUse = 1;

            model.UpdatedDate = DateTime.Now;

            if (id > 0)
            {
                model.InsertedDate = model.InsertedDate == DateTime.MinValue ? model.UpdatedDate : model.InsertedDate;
                result = DataAccess.UpdateCouponGroup(model);
            }
            else
            {
                model.Active = true;
                model.InsertedDate = model.UpdatedDate;
                result = DataAccess.AddCouponGroup(model);
                model.CouponGroupId = result;
            }
            ViewBag.id = result > -1 ? model.CouponGroupId : -1;
            return result > 0;
        }

        protected override void DoSaveSuccess(int id)
        {
            string[] itemIds = Request.Form.GetValues("ItemIds");
            DataAccess.DeleteCouponGroupItemsByCouponGroupId(id);
            if (itemIds != null && itemIds.Any())
            {
                foreach (string itemId in itemIds)
                {
                    DataAccess.AddCouponGroupItem((long)id, DataManager.ToLong(itemId));
                }
            }

            DataAccess.DeleteCouponGroupItemsCategoriesByCouponId(id);
            string[] itemCategoryIds = Request.Form.GetValues("ItemCategories");
            if (itemCategoryIds != null && itemCategoryIds.Any())
            {
                foreach (string itemCategoryId in itemCategoryIds)
                {
                    DataAccess.AddCouponGroupItemCategory((long)id, DataManager.ToLong(itemCategoryId));
                }
            }

            DataAccess.DeleteCouponGroupItemsTypesByCouponId(id);
            string[] itemTypeIds = Request.Form.GetValues("ItemType");
            if (itemTypeIds != null && itemTypeIds.Any())
            {
                foreach (string itemTypeId in itemTypeIds)
                {
                    DataAccess.AddCouponGroupItemType((long)id, DataManager.ToLong(itemTypeId));
                }
            }
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteCoupon(id) > 0;
        }
        #endregion

        #region Action Methods
        public ActionResult ItemsPicker(string exclude = "")
        {
            PickerModel model = new PickerModel();
            model.Columns.Add("ItemId", Resources.Resources.ItemName);
            model.Columns.Add("ItemName", Resources.Resources.ItemName);
            model.Columns.Add("ItemTypeName", Resources.Resources.ItemType);
            model.Columns.Add("SiteName", Resources.Resources.SiteName);

            List<Item> items = DataAccess.GetItemPickerData(exclude);
            foreach (Item item in items)
            {
                model.Data.Add(new
                {
                    ItemId = item.ItemId,
                    ItemCode = item.ItemCode,
                    ItemName = item.ItemName,
                    ItemTypeName = item.ItemTypeName,
                    SiteName = item.SiteName
                });
            }

            return View("~/Areas/Admin/Views/Picker/List.cshtml", model);
        }

        public ActionResult Export(long? id)
        {
            if (!id.HasValue)
                return Content("", "text/plain");

            string content = string.Empty;
            List<string> coupons = DataAccess.GetCouponCodesByCouponGroupId(id.Value);
            foreach (string coupon in coupons)
            {
                content += coupon + Environment.NewLine;
            }
            if (content.Length > 0)
            {
                content = content.Remove(content.Length - 1);
            }
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            return File(bytes, "text/plain", "CouponList" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt");
        }
        #endregion

        #region Private Methods
        private void InitCouponDropDownList()
        {
            List<object> list = new List<object>();
            list.Add(new
            {
                ID = (int)CouponType.Amount,
                Value = Resources.Resources.Amount
            });
            list.Add(new
            {
                ID = (int)CouponType.Percent,
                Value = Resources.Resources.Percent
            });
            ViewBag.CouponTypes = ListToDropDownList(list, "ID", "Value");
        }

        private void InitCouponUsageDropDownList()
        {
            List<object> list = new List<object>();
            list.Add(new
            {
                ID = (int)CouponUsageType.PerOrder,
                Value = Resources.Resources.PerOrder
            });
            list.Add(new
            {
                ID = (int)CouponUsageType.PerItemType,
                Value = Resources.Resources.PerItemType
            });
            list.Add(new
            {
                ID = (int)CouponUsageType.PerCategories,
                Value = Resources.Resources.PerCategorie
            });
            list.Add(new
            {
                ID = (int)CouponUsageType.PerItems,
                Value = Resources.Resources.PerItem
            });
            ViewBag.CouponUsageTypes = ListToDropDownList(list, "ID", "Value");
        }

        private void InitUsagePeriodTypeDropDownList()
        {
            List<object> list = new List<object>();
            list.Add(new
            {
                ID = (int)CouponUsagePeriodType.Total,
                Value = Resources.Resources.Total
            });
            list.Add(new
            {
                ID = (int)CouponUsagePeriodType.ByDay,
                Value = Resources.Resources.ByDay
            });
            list.Add(new
            {
                ID = (int)CouponUsagePeriodType.ByWeek,
                Value = Resources.Resources.ByWeek
            });
            ViewBag.UsagePeriodTypes = ListToDropDownList(list, "ID", "Value");
        }

        private void InitCategoriesList()
        {
            Dictionary<int, string> itemTypes = new Dictionary<int, string>();
            itemTypes.Add((int)ItemType.Type.GreenFee, "Green-fee");
            itemTypes.Add((int)ItemType.Type.StayPackage, "Séjour");
            itemTypes.Add((int)ItemType.Type.GolfLesson, "Stage");
            itemTypes.Add((int)ItemType.Type.DrivingRange, "Carte de pratice");
            itemTypes.Add((int)ItemType.Type.Product, "DLG Shop");
            ViewBag.ItemType = itemTypes;
            ViewBag.ItemCategories = DataAccess.GetAllItemCategories();
        }
        #endregion

        #region AJAX Methods
        #region AjaxGenerateCoupons
        public ActionResult AjaxGenerateCoupons(long? couponGroupId, string prefix = "", int? qty = 0)
        {
            if (!couponGroupId.HasValue)
                throw new Exception("There is no couponGroupId.");

            if (!qty.HasValue || qty.Value < 0)
                qty = 0;

            if (String.IsNullOrEmpty(prefix.Trim()))
                prefix = DateTime.Now.ToString("yyyyMMdd");

            try
            {
                List<string> couponCodes = new List<string>();
                string code = string.Empty;
                DateTime now = DateTime.Now;
                for (int i = 0; i < qty; i++)
                {
                    do
                    {
                        code = prefix + StringHelper.RandomString(6);
                    } while (DataAccess.IsExistsCouponCode(code));
                    DataAccess.AddCoupon(new Coupon()
                    {
                        CouponCode = code,
                        CouponGroupId = couponGroupId.Value,
                        InsertedDate = now,
                        UpdatedDate = now,
                        Active = true
                    });
                }
                List<Coupon> coupons = DataAccess.GetCouponsByCouponGroupId(couponGroupId.Value);
                string html = this.RenderViewToString("~/Views/_Shared/UC/Admin/Coupon/UCCouponRows.cshtml", coupons);
                return Json(new
                {
                    isSuccess = true,
                    html = html
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
        #endregion

        #region AjaxDeleteCoupons
        public ActionResult AjaxDeleteCoupons(long[] couponIds)
        {
            try
            {
                DataAccess.DeleteCouponByIds(couponIds);

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
        #endregion

        #region AjaxImportCoupons
        public JsonResult AjaxImportCoupons(int? id, string[] coupons)
        {
            try
            {
                if (!id.HasValue)
                    throw new Exception("Please specific id parameter.");

                if (coupons == null)
                    throw new Exception("Please specific id parameter.");

                coupons = coupons.Where(it => !String.IsNullOrWhiteSpace(it)).ToArray();

                DataAccess.ImportCoupons(id.Value, coupons);

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
        #endregion

        #region AjaxCheckCouponCodeAvailable
        public JsonResult AjaxCheckCouponCodeAvailable(string[] coupons)
        {
            try
            {
                if (coupons == null)
                    throw new Exception("Please specific id parameter.");
                List<Coupon> list = new List<Coupon>();
                List<string> availableCoupons = new List<string>();
                foreach (string coupon in coupons)
                {
                    if (String.IsNullOrWhiteSpace(coupon))
                        continue;

                    if (DataAccess.IsExistsCouponCode(coupon))
                    {
                        list.Add(new Coupon()
                        {
                            CouponCode = coupon,
                            IsAvailable = false
                        });
                    }
                    else
                    {
                        list.Add(new Coupon()
                        {
                            CouponCode = coupon,
                            IsAvailable = true
                        });
                        availableCoupons.Add(coupon);
                    }
                }

                int pass = list.Count(it => it.IsAvailable);
                int notPass = list.Count(it => !it.IsAvailable);
                ViewBag.Pass = pass;
                ViewBag.NotPass = notPass;
                string html = RenderViewToString("~/Views/_Shared/UC/Admin/Coupon/UCConfirmImportTable.cshtml", list);

                return Json(new
                {
                    isSuccess = true,
                    availableCoupons = availableCoupons,
                    pass = pass,
                    notPass = list.Count(it => !it.IsAvailable),
                    html = html
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
        #endregion

        #region AjaxLoadCoupon
        public ActionResult AjaxLoadCoupon(long? couponGroupId, int? page = 1, int? pageSize = 100)
        {
            if (!couponGroupId.HasValue)
                throw new Exception("There is no couponGroupId.");

            if (!page.HasValue)
                throw new Exception("There is no page.");

            if (!pageSize.HasValue)
                throw new Exception("There is no pageSize.");

            try
            {
                List<string> couponCodes = new List<string>();
                int totalPages = 1;
                List<Coupon> coupons = DataAccess.GetCouponsPagingByCouponGroupId(couponGroupId.Value, page.Value - 1, pageSize.Value, out totalPages);
                var couponsArray = coupons.Select(it => new
                {
                    couponId = it.CouponId,
                    couponCode = it.CouponCode,
                    availableText = it.IsAvailable ? Resources.Resources.Yes : Resources.Resources.No,
                    isAvailable = it.IsAvailable
                });
                var data = new
                {
                    isSuccess = true,
                    coupons = couponsArray,
                    totalPages = totalPages,
                    page = page
                };
                return new CorrectJsonResult
                {
                    Data = data,
                    ContentType = "application/json",
                    ContentEncoding = Encoding.UTF8,
                    MaxJsonLength = int.MaxValue
                };
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
        #endregion
        #endregion
    }
}
