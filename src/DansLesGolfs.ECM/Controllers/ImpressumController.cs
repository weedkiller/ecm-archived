using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.ECM.Controllers
{
    public class ImpressumController : BaseECMCRUDController<Impressum>
    {
        #region Constructor
        public ImpressumController()
        {
            ObjectName = "Impressum";
            TitleName = Resources.Resources.PrivacyPolicy;
            PrimaryKey = "ImpressumId";

            // Define Column Names.
            ColumnNames.Add("Name", Resources.Resources.PrivacyPolicyName);

            AddAllowedUserType(UserType.Type.Admin);
            AddAllowedUserType(UserType.Type.Staff);
        }
        #endregion

        #region Overriden Methods
        protected override IQueryable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<Impressum> models = null;
            if (Auth.User.UserTypeId == UserType.Type.SiteManager || Auth.User.UserTypeId == UserType.Type.Staff)
            {
                models = DataAccess.GetAllImpressums(param, Auth.User.SiteId);
            }
            else
            {
                models = DataAccess.GetAllImpressums(param);
            }
            return models;
        }

        protected override object DoPrepareNew()
        {
            return new
            {
                ImpressumName = string.Empty,
                Active = false,
                ContentType = 0,
                SiteId = 0
            };
        }

        protected override object DoPrepareEdit(long id)
        {
            Impressum model = DataAccess.GetImpressumById(id);
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
            int id = DataManager.ToInt(Request.Form["id"], -1);
            Impressum model = null;
            if (id > 0)
            {
                model = DataAccess.GetImpressumById(id);
                if(model == null)
                {
                    model = new Impressum();
                }
            }
            else
            {
                model = new Impressum();
            }
            model.ImpressumId = DataManager.ToInt(Request.Form["id"], -1);
            model.Name = DataManager.ToString(Request.Form["Name"]);
            model.Detail = DataManager.ToString(Request.Form["Detail"]);

            if (Auth.User.UserTypeId == UserType.Type.SuperAdmin || Auth.User.UserTypeId == UserType.Type.Admin)
            {
                model.SiteId = DataManager.ToLong(Request.Form["SiteId"]);
            }
            else
            {
                model.SiteId = Auth.User.SiteId;
            }

            if (id > 0)
            {
                result = DataAccess.UpdateImpressum(model);
            }
            else
            {
                result = DataAccess.AddImpressum(model);
                model.ImpressumId = result;
            }
            ViewBag.id = result > -1 ? model.ImpressumId : -1;
            return result > 0;
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteImpressum(id) > 0;
        }
        #endregion
    }
}
