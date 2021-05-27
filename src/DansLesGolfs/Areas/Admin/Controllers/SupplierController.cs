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
    public class SupplierController : BaseAdminCRUDController
    {
        #region Constructor
        public SupplierController()
        {
            ObjectName = "Supplier";
            TitleName = "Supplier";
            PrimaryKey = "SupplierId";

            // Define Column Names.
            ColumnNames.Add("SupplierName", "Supplier Name");
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<Supplier> models = DataAccess.GetAllSuppliers(param);
            return models;
        }

        protected override object DoPrepareNew()
        {
            return new Supplier();
        }

        protected override object DoPrepareEdit(long id)
        {
            Supplier model = DataAccess.GetSupplier(id);
            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            Supplier model = null;
            int id = DataManager.ToInt(Request.Form["id"]);
            if (id > 0)
            {
                model = DataAccess.GetSupplier(id);
                if (model == null)
                {
                    model = new Supplier();
                }
            }
            else
            {
                model = new Supplier();
            }
            model.SupplierId = id;
            model.SupplierName = DataManager.ToString(Request.Form["SupplierName"]).Trim();
            model.UpdateDate = DateTime.Now;
            if (Auth.User != null)
                model.UserId = Auth.User.UserId;

            if (id > -1)
            {
                model.InsertDate = model.InsertDate == DateTime.MinValue ? model.UpdateDate : model.InsertDate;
                result = DataAccess.UpdateSupplier(model);
            }
            else
            {
                model.InsertDate = model.UpdateDate;
                model.Active = true;

                result = DataAccess.AddSupplier(model);
                model.SupplierId = result;
            }
            ViewBag.id = result > -1 ? model.SupplierId : -1;
            return result > 0;
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteSupplier(id) > 0;
        }
        #endregion
    }
}
