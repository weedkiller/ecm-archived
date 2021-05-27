using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Admin.Controllers
{
    public class SponsorEmailController : BaseAdminCRUDController
    {
        #region Fields
        private bool isNew = false;
        #endregion

        #region Constructor

        public SponsorEmailController()
        {
            ObjectName = "SponsorEmail";
            TitleName = "Sponsor Email";
            PrimaryKey = "SponsorEmailId";

            // Define Column Names.
            ColumnNames.Add("Subject", "Subject");
        }
        #endregion

        #region Overriden Methods

        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            //List<Site> models = DataAccess.GetAllSites(param);
            //return models;
            List<SponsorEmail> models = DataAccess.GetAllSponsorEmail(null);
            return models;
        }

        protected override void DoPrepareForm(int? id)
        {
        }

        protected override object DoPrepareNew()
        {
            return new Site();
        }

        protected override object DoPrepareEdit(long id)
        {
            Site model = DataAccess.GetSiteById(id);
            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            int id = DataManager.ToInt(Request.Form["id"]);
            SponsorEmail model = null;
            if (id > 0)
            {
                //model = DataAccess.GetSite(id);
                //if (model == null)
                //{
                //    model = new Site();
                //}
            }
            else
            {
                model = new SponsorEmail();
                isNew = true;
            }

            model.SponsorEmailId = id;
            model.Subject = DataManager.ToString(Request.Form["Subject"]).Trim();
            model.Body = DataManager.ToString(Request.Form["Body"]).Trim();
            //model.UpdateDate = DateTime.Now;

            //if (Auth.User != null)
            //    model.UserId = Auth.User.UserId;

            if (id > 0)
            {
                result = DataAccess.UpdateSponsorEmail(model);
            }
            else
            {
                //model.InsertDate = model.UpdateDate;
                model.Active = true;
                result = DataAccess.AddSponsorEmail(model);
                model.SponsorEmailId = result;
            }
            ViewBag.id = result > -1 ? model.SponsorEmailId : -1;

            return result > 0;
        }

        protected override void DoSaveSuccess(int id)
        {
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteSponsorEmail(id) > 0;
        }

        #endregion

    }
}
