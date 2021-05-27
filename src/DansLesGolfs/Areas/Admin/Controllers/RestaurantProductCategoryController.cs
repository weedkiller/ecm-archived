using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Admin.Controllers
{
    public class RestaurantProductCategoryController : BaseAdminCRUDController
    {
        #region Constructor
        public RestaurantProductCategoryController()
        {
            ObjectName = "RestaurantProductCategory";
            TitleName = Resources.Resources.RestaurantProductCategory;
            PrimaryKey = "RestaurantProductCategoryId";

            // Define Column Names.
            ColumnNames.Add("CategoryName", Resources.Resources.CategoryName);
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<RestaurantProductCategory> models = DataAccess.GetAllRestaurantProductCategories(param);
            return models;
        }

        protected override object DoPrepareNew()
        {
            return new RestaurantProductCategory()
            {
                Active = true
            };
        }

        protected override object DoPrepareEdit(long id)
        {
            RestaurantProductCategory model = DataAccess.GetRestaurantProductCategory(id);
            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            int id = DataManager.ToInt(Request.Form["id"]);
            RestaurantProductCategory model = null;
            if (id > 0)
            {
                model = DataAccess.GetRestaurantProductCategory(id);
                if (model == null)
                {
                    model = new RestaurantProductCategory();
                }
            }
            else
            {
                model = new RestaurantProductCategory();
            }
            model.RestaurantProductCategoryId = id;
            model.CategoryName = DataManager.ToString(Request.Form["CategoryName"]).Trim();

            if (id > 0)
            {
                model.UpdatedBy = Auth.User.UserId;
                model.UpdatedDate = DateTime.Now;
                result = DataAccess.UpdateRestaurantProductCategory(model);
            }
            else
            {
                model.CreatedBy = model.UpdatedBy = Auth.User.UserId;
                model.CreatedDate =  model.UpdatedDate = DateTime.Now;
                model.Active = true;
                result = DataAccess.AddRestaurantProductCategory(model);
                model.RestaurantProductCategoryId = result;
            }
            ViewBag.id = result > -1 ? model.RestaurantProductCategoryId : -1;
            return result > 0;
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteRestaurantProductCategory(id) > 0;
        }
        #endregion
    }
}
