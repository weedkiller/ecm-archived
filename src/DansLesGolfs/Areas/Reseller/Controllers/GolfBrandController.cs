using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Reseller.Controllers
{
    public class GolfBrandController : BaseResellerCRUDController
    {
        #region Constructor
        public GolfBrandController()
        {
            ObjectName = "GolfBrand";
            TitleName = Resources.Resources.GolfBrand;
            PrimaryKey = "GolfBrandId";

            // Define Column Names.
            ColumnNames.Add("GolfBrandName", Resources.Resources.GolfBrandName);
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<GolfBrand> models = DataAccess.GetAllGolfBrands(param);
            return models;
        }

        protected override object DoPrepareNew()
        {
            return new GolfBrand()
            {
                Active = true
            };
        }

        protected override object DoPrepareEdit(long id)
        {
            GolfBrand model = DataAccess.GetGolfBrand(id);
            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            int id = DataManager.ToInt(Request.Form["id"]);
            GolfBrand model = null;
            if (id > 0)
            {
                model = DataAccess.GetGolfBrand(id);
                if (model == null)
                {
                    model = new GolfBrand();
                }
            }
            else
            {
                model = new GolfBrand();
            }
            model.GolfBrandId = id;
            model.GolfBrandName = DataManager.ToString(Request.Form["GolfBrandName"]).Trim();
            model.GolfBrandDesc = DataManager.ToString(Request.Form["GolfBrandDesc"]).Trim();

            if (id > 0)
            {
                result = DataAccess.UpdateGolfBrand(model);
            }
            else
            {
                model.Active = true;
                result = DataAccess.AddGolfBrand(model);
                model.GolfBrandId = result;
            }
            ViewBag.id = result > -1 ? model.GolfBrandId : -1;
            return result > 0;
        }

        protected override void DoSaveSuccess(int id)
        {
            if(Request.Files["GolfBrandImage"] != null 
                && !String.IsNullOrEmpty(Request.Files["GolfBrandImage"].FileName) 
                && !String.IsNullOrEmpty(Path.GetExtension(Request.Files["GolfBrandImage"].FileName)))
            {
                var file = Request.Files["GolfBrandImage"];
                string fileDir = "~/Uploads/GolfBrands";
                string imageName = id + Path.GetExtension(file.FileName);
                string filePath = Server.MapPath(Path.Combine(fileDir, id + Path.GetExtension(file.FileName)));
                file.SaveAs(filePath);
                if (System.IO.File.Exists(filePath))
                DataAccess.SaveGolfBrandImage(id, imageName);
            }
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteGolfBrand(id) > 0;
        }
        #endregion
    }
}
