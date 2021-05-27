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
    public class BrandController : BaseECMCRUDController<BrandViewModel>
    {
        #region Constructor
        public BrandController()
        {
            ObjectName = "Brand";
            TitleName = Resources.Resources.Brand;
            PrimaryKey = "BrandId";

            // Define Column Names.
            ColumnNames.Add("BrandName", Resources.Resources.Name);

            AddAllowedUserType(UserType.Type.Admin);
            AddAllowedUserType(UserType.Type.Staff);
        }
        #endregion

        #region Overriden Methods
        protected override void DoSetGridColumns(GridColumnFactory<BrandViewModel> columns)
        {
            columns.Bound(c => c.BrandName).Title(Resources.Resources.Name);
        }

        protected override IQueryable<BrandViewModel> DoLoadDataJSON()
        {
            return DataAccess.GetListBrands();
        }

        protected override object DoPrepareNew()
        {
            return new Brand
            {
                BrandId = 0,
                Active = false,
                BrandName = string.Empty
            };
        }

        protected override object DoPrepareEdit(long id)
        {
            Brand model = DataAccess.GetBrandById(id);
            return model;
        }

        protected override void DoPrepareForm(int? id = null)
        {
            long? siteId = null;
            if (Auth.User.UserTypeId == UserType.Type.SiteManager || Auth.User.UserTypeId == UserType.Type.Staff)
            {
                siteId = Auth.User.SiteId;
            }
            else if (ViewBag.SiteId != null)
            {
                siteId = DataManager.ToLong(ViewBag.SiteId);
            }
        }

        protected override bool DoSave()
        {
            int result = -1;
            int id = DataManager.ToInt(Request.Form["id"], -1);
            Brand model = null;
            if (id > 0)
            {
                model = DataAccess.GetBrandById(id);
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
            model.BrandName = DataManager.ToString(Request.Form["BrandName"]);

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

        protected override bool DoDelete(long id)
        {
            return DataAccess.DeleteBrand(id) > 0;
        }
        #endregion
    }
}
