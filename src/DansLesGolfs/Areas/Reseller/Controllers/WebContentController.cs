using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Reseller.Controllers
{
    public class WebContentController : BaseResellerCRUDController
    {
        #region Constructor

        public WebContentController()
        {
            ObjectName = "WebContent";
            TitleName = "Web Contents";
            PrimaryKey = "ContentId";

            // Define Column Names.
            ColumnNames.Add("ContentCategory", "Category");
            ColumnNames.Add("ContentKey", "Content Name");

            ViewBag.HideAddButton = true;
        }
        #endregion

        #region Overriden Methods

        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<WebContent> models = DataAccess.GetAllWebContent(param, this.CultureId);
            return models;
        }

        protected override void DoPrepareForm(int? id)
        {

        }

        protected override object DoPrepareNew()
        {
            return null;
        }

        protected override object DoPrepareEdit(long id)
        {
            WebContent model = DataAccess.GetWebContentById(id);
            if (model.WebContentLangs != null && model.WebContentLangs.Any())
            {
                ViewBag.TopicName = model.WebContentLangs[0].TopicName;
                ViewBag.ContentText = model.WebContentLangs[0].ContentText;
            }
            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            int id = DataManager.ToInt(Request.Form["id"]);

            WebContent model = null;

            if (id > 0) // Edit
            {
                model = DataAccess.GetWebContentById(id);
                if (model == null)
                {
                    model = new WebContent();
                }
            }
            else // Add
            {
                model = new WebContent();
            }

            model.ContentId = id;
            model.ContentKey = DataManager.ToString(Request.Form["ContentKey"]).Trim();
            model.ContentCategory = DataManager.ToString(Request.Form["ContentCategory"]).Trim();
            if (Auth.User != null)
                model.UserId = Auth.User.UserId;

            if (id > 0)
            {
                DataAccess.UpdateWebContent(model);
                result = 1;
            }
            else
            {
                model.Active = true;
                model.InsertDate = DateTime.Now;
                result = DataAccess.AddWebContent(model);
                model.ContentId = result;
            }

            ViewBag.id = result > -1 ? model.ContentId : -1;

            return result > 0;
        }

        protected override void DoSaveSuccess(int id)
        {
            string[] topicNames = Request.Form.GetValues("TopicName");
            string[] contentTexts = Request.Form.GetValues("ContentText");

            WebContentLang contentLang = null;

            DataAccess.DeleteWebContentLangByContentId(id);
            for (int i = 0, j = contentTexts.Length; i < j; i++)
            {
                contentLang = new WebContentLang();
                contentLang.ContentId = id;
                contentLang.LangId = i + 1;
                contentLang.ContentText = contentTexts[i].Trim();
                contentLang.TopicName = topicNames[i].Trim();
                contentLang.UpdateDate = DateTime.Now;
                if (Auth.User != null)
                    contentLang.UserId = Auth.User.UserId;

                DataAccess.AddWebContentLang(contentLang);
            }
        }

        protected override bool DoDelete(int id)
        {
            return true;
        }

        #endregion
    }
}
