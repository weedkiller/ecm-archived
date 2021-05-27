using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using DansLesGolfs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Reseller.Controllers
{
    public class CouponController : BaseResellerCRUDController
    {
        #region Constructor
        public CouponController()
        {
            ObjectName = "Coupon";
            TitleName = Resources.Resources.Coupon;
            PrimaryKey = "CouponGroupId";

            // Define Column Names.
            ColumnNames.Add("CouponGroupName", Resources.Resources.CouponName);
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
            model.Reduction = DataManager.ToDecimal(Request.Form["Reduction"]);
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
            if(itemIds != null && itemIds.Any())
            {
                foreach (string itemId in itemIds)
                {
                    DataAccess.AddCouponGroupItem((long)id, DataManager.ToLong(itemId));
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
            foreach(Item item in items)
            {
                model.Data.Add(new {
                    ItemId = item.ItemId,
                    ItemCode = item.ItemCode,
                    ItemName = item.ItemName,
                    ItemTypeName = item.ItemTypeName,
                    SiteName = item.SiteName
                });
            }

            return View("~/Areas/Reseller/Views/Picker/List.cshtml", model);
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
                ID = (int)CouponUsageType.PerCategories,
                Value = Resources.Resources.PerCategorie
            });
            list.Add(new
            {
                ID = (int)CouponUsageType.PerItems,
                Value = Resources.Resources.PerItems
            });
            ViewBag.CouponUsageTypes = ListToDropDownList(list, "ID", "Value");
        }
        private void InitCategoriesList()
        {
            Dictionary<int, string> itemTypes = new Dictionary<int, string>();
            itemTypes.Add((int)ItemType.Type.Product, ItemType.Type.Product.ToString());
            itemTypes.Add((int)ItemType.Type.GreenFee, ItemType.Type.GreenFee.ToString());
            itemTypes.Add((int)ItemType.Type.StayPackage, ItemType.Type.StayPackage.ToString());
            itemTypes.Add((int)ItemType.Type.GolfLesson, ItemType.Type.GolfLesson.ToString());
            itemTypes.Add((int)ItemType.Type.DrivingRange, ItemType.Type.DrivingRange.ToString());
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
                string html = this.RenderViewToString("~/Views/_Shared/UC/Reseller/Coupon/UCCouponRows.cshtml", coupons);
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
        #endregion
    }
}
