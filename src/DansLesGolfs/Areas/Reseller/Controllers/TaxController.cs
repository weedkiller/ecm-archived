using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Reseller.Controllers
{
    public class TaxController : BaseResellerCRUDController
    {
        #region Constructor
        public TaxController()
        {
            ObjectName = "Tax";
            TitleName = "Tax";
            PrimaryKey = "TaxId";

            // Define Column Names.
            ColumnNames.Add("TaxCode", "Tax Code");
            ColumnNames.Add("TaxName", "Tax Name");
            ColumnNames.Add("TaxPercent", "Tax Percent (%)");
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<Tax> models = DataAccess.GetAllTaxes(param);
            return models;
        }

        protected override object DoPrepareNew()
        {
            return new Tax();
        }

        protected override object DoPrepareEdit(long id)
        {
            Tax model = DataAccess.GetTax(id);
            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            Tax model = null;
            int id = DataManager.ToInt(Request.Form["id"]);
            if (id > 0)
            {
                model = DataAccess.GetTax(id);
                if (model == null)
                {
                    model = new Tax();
                }
            }
            else
            {
                model = new Tax();
            }
            model.TaxId = id;
            model.TaxCode = DataManager.ToString(Request.Form["TaxCode"]).Trim();
            model.TaxName = DataManager.ToString(Request.Form["TaxName"]).Trim();
            model.TaxPercent = DataManager.ToFloat(Request.Form["TaxPercent"]);

            if (id > 0)
            {
                result = DataAccess.UpdateTax(model);
            }
            else
            {
                result = DataAccess.AddTax(model);
                model.TaxId = result;
            }
            ViewBag.id = result > -1 ? model.TaxId : -1;
            return result > 0;
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteTax(id) > 0;
        }
        #endregion
    }
}
