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
    public class BrandController : BaseAdminCRUDController
    {
        #region Constructor
        public BrandController()
        {
            ObjectName = "Brand";
            TitleName = Resources.Resources.Brand;
            PrimaryKey = "BrandId";

            // Define Column Names.
            ColumnNames.Add("BrandName", Resources.Resources.BrandName);
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<Brand> models = DataAccess.GetAllBrands(param);
            return models;
        }

        protected override object DoPrepareNew()
        {
            return new Brand()
            {
                Active = true
            };
        }

        protected override object DoPrepareEdit(long id)
        {
            Brand model = DataAccess.GetBrand(id);
            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            int id = DataManager.ToInt(Request.Form["id"]);
            Brand model = null;
            if (id > 0)
            {
                model = DataAccess.GetBrand(id);
                if (model == null)
                {
                    model = new Brand();
                }
            }
            else
            {
                model = new Brand();
            }
            model.BrandId = id;
            model.BrandName = DataManager.ToString(Request.Form["BrandName"]).Trim();
            model.BrandDesc = DataManager.ToString(Request.Form["BrandDesc"]).Trim();

            if (id > 0)
            {
                result = DataAccess.UpdateBrand(model);
            }
            else
            {
                model.Active = true;
                result = DataAccess.AddBrand(model);
                model.BrandId = result;
            }
            ViewBag.id = result > -1 ? model.BrandId : -1;
            return result > 0;
        }

        protected override void DoSaveSuccess(int id)
        {
            if(Request.Files["BrandImage"] != null 
                && !String.IsNullOrEmpty(Request.Files["BrandImage"].FileName) 
                && !String.IsNullOrEmpty(Path.GetExtension(Request.Files["BrandImage"].FileName)))
            {
                var file = Request.Files["BrandImage"];
                string fileDir = "~/Uploads/Brands";
                string imageName = id + Path.GetExtension(file.FileName);
                string filePath = Server.MapPath(Path.Combine(fileDir, id + Path.GetExtension(file.FileName)));
                file.SaveAs(filePath);
                if (System.IO.File.Exists(filePath))
                DataAccess.SaveBrandImage(id, imageName);
            }
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteBrand(id) > 0;
        }
        #endregion
    }
}
