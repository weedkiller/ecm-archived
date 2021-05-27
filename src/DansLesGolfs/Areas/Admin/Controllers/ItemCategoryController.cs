using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Admin.Controllers
{
    public class ItemCategoryController : BaseAdminCRUDController
    {
        #region Constructor
        public ItemCategoryController()
        {
            ObjectName = "ItemCategory";
            TitleName = "Product Category";
            PrimaryKey = "CategoryId";

            // Define Column Names.
            ColumnNames.Add("CategoryName", "Category Name");
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<ItemCategory> models = DataAccess.GetAllItemCategories(param,null);
            return models;
        }

        protected override void DoPrepareForm(int? id = null)
        {
            List<Site> sites = DataAccess.GetSitesDropDownListData();
            ViewBag.Sites = ListToDropDownList<Site>(sites, "SiteId", "SiteName");
        }

        protected override object DoPrepareNew()
        {
            return new ItemCategory();
        }

        protected override object DoPrepareEdit(long id)
        {
            ItemCategory model = DataAccess.GetItemCategory(id);
            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            ItemCategory model = null;
            int id = DataManager.ToInt(Request.Form["id"]);
            if (id > 0)
            {
                model = DataAccess.GetItemCategory(id);
                if (model == null)
                {
                    model = new ItemCategory();
                }
            }
            else
            {
                model = new ItemCategory();
            }
            model.CategoryId = id;
            model.CategoryName = DataManager.ToString(Request.Form["CategoryName"]).Trim();
            model.Prefix = DataManager.ToString(Request.Form["Prefix"]).Trim();
            model.ItemTypeId = (int)ItemType.Type.Product;
            //model.SiteId = DataManager.ToInt(Request.Form["SiteId"]);
            model.UpdateDate = DateTime.Now;

            if (Auth.User != null)
                model.UserId = Auth.User.UserId;

            if (id > 0)
            {
                model.InsertDate = model.InsertDate == DateTime.MinValue ? model.UpdateDate : model.InsertDate;
                result = DataAccess.UpdateItemCategory(model);
            }
            else
            {
                model.InsertDate = model.UpdateDate;
                model.Active = true;

                result = DataAccess.AddItemCategory(model);
                model.CategoryId = result;
            }
            ViewBag.id = result > -1 ? model.CategoryId : -1;
            return result > 0;
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteItemCategory(id) > 0;
        }
        #endregion
    }
}
