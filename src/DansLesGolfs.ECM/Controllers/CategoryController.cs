using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI.Fluent;

namespace DansLesGolfs.ECM.Controllers
{
    public class CategoryController : BaseECMCRUDController<Category>
    {
        #region Constructor
        public CategoryController()
        {
            ObjectName = "Category";
            TitleName = Resources.Resources.Category;
            PrimaryKey = "CategoryId";
        }
        #endregion

        #region Overriden Methods
        protected override void DoSetGridColumns(GridColumnFactory<Category> columns)
        {
            columns.Bound(c => c.CategoryName).Title(Resources.Resources.CategoryName);
        }
        protected override IQueryable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            using (var db = DataAccess.GetModels())
            {
                if (Auth.User.UserTypeId == UserType.Type.SiteManager || Auth.User.UserTypeId == UserType.Type.Staff)
                {
                    return db.Categories.Where(it => it.SiteId == Auth.User.SiteId);
                }
                else
                {
                    return db.Categories;
                }
            }
        }

        protected override object DoPrepareNew()
        {
            return new
            {
                CategoryName = string.Empty,
                Active = false,
                ContentType = 0
            };
        }

        protected override object DoPrepareEdit(long id)
        {
            Category model = DataAccess.GetCategory(id);
            return model;
        }

        protected override void DoPrepareForm(int? id = null)
        {
            List<Site> sites = DataAccess.GetSitesDropDownListData();
            List<SelectListItem> siteList = ListToDropDownList<Site>(sites, "SiteId", "SiteName");
            siteList.Insert(0, new SelectListItem()
            {
                Text = Resources.Resources.SelectSite,
                Value = "0",
                Selected = true
            });
            ViewBag.Sites = siteList;
        }

        protected override bool DoSave()
        {
            int result = -1;
            Category model = new Category();
            model.CategoryId = DataManager.ToInt(Request.Form["id"], -1);
            model.CategoryName = DataManager.ToString(Request.Form["CategoryName"]);

            if (Auth.User.UserTypeId == UserType.Type.SuperAdmin || Auth.User.UserTypeId == UserType.Type.Admin)
            {
                model.SiteId = DataManager.ToInt(Request.Form["SiteId"]);
            }

            model.ContentType = 0;

            Category obj = DataAccess.GetCategory(model.CategoryId);
            if (obj != null)
            {
                result = DataAccess.UpdateCategory(model);
            }
            else
            {
                model.Active = true;
                result = DataAccess.AddCategory(model);
                model.CategoryId = result;
            }
            ViewBag.id = result > -1 ? model.CategoryId : -1;
            return result > 0;
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteCategory(id) > 0;
        }
        #endregion
    }
}
