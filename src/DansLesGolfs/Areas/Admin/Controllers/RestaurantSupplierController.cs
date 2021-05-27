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
    public class RestaurantSupplierController : BaseAdminCRUDController
    {
        #region Constructor
        public RestaurantSupplierController()
        {
            ObjectName = "RestaurantSupplier";
            TitleName = Resources.Resources.RestaurantSupplier;
            PrimaryKey = "RestaurantSupplierId";

            // Define Column Names.
            ColumnNames.Add("SupplierName", Resources.Resources.SupplierName);
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<RestaurantSupplier> models = DataAccess.GetAllRestaurantSuppliers(param);
            return models;
        }

        protected override object DoPrepareNew()
        {
            return new RestaurantSupplier()
            {
                Active = true
            };
        }

        protected override object DoPrepareEdit(long id)
        {
            RestaurantSupplier model = DataAccess.GetRestaurantSupplier(id);
            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            int id = DataManager.ToInt(Request.Form["id"]);
            RestaurantSupplier model = null;
            if (id > 0)
            {
                model = DataAccess.GetRestaurantSupplier(id);
                if (model == null)
                {
                    model = new RestaurantSupplier();
                }
            }
            else
            {
                model = new RestaurantSupplier();
            }
            model.RestaurantSupplierId = id;
            model.SupplierName = DataManager.ToString(Request.Form["SupplierName"]).Trim();

            if (id > 0)
            {
                model.UpdatedBy = Auth.User.UserId;
                model.UpdatedDate = DateTime.Now;
                result = DataAccess.UpdateRestaurantSupplier(model);
            }
            else
            {
                model.CreatedBy = model.UpdatedBy = Auth.User.UserId;
                model.CreatedDate = model.UpdatedDate = DateTime.Now;
                model.Active = true;
                result = DataAccess.AddRestaurantSupplier(model);
                model.RestaurantSupplierId = result;
            }
            ViewBag.id = result > -1 ? model.RestaurantSupplierId : -1;
            return result > 0;
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteRestaurantSupplier(id) > 0;
        }
        #endregion
    }
}
