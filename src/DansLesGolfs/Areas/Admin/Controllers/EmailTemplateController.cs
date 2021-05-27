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
    public class EmailTemplateController : BaseAdminCRUDController
    {
        #region Constructor
        public EmailTemplateController()
        {
            ObjectName = "EmailTemplate";
            TitleName = Resources.Resources.EmailTemplates;
            PrimaryKey = "TemplateId";

            // Define Column Names.
            ColumnNames.Add("Name", Resources.Resources.Name);

            ViewBag.HideAddButton = true;
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<EmailTemplate> models = DataAccess.GetAllEmailTemplates(param, this.CultureId);
            return models;
        }

        protected override object DoPrepareNew()
        {
            return new EmailTemplate();
        }

        protected override object DoPrepareEdit(long id)
        {
            EmailTemplate model = DataAccess.GetEmailTemplate(id);
            return model;
        }

        protected override void DoPrepareForm(int? id = 0)
        {
            ViewBag.EmailTemplateVariables = DataAccess.GetAllEmailTemplateVariables();
        }

        protected override bool DoSave()
        {
            int result = -1;
            EmailTemplate model = null;
            int id = DataManager.ToInt(Request.Form["id"]);
            if (id > 0)
            {
                model = DataAccess.GetEmailTemplate(id);
                if (model == null)
                {
                    model = new EmailTemplate();
                }
            }
            else
            {
                model = new EmailTemplate();
            }
            model.TemplateId = id;
            model.CategoryId = 0;

            if (Auth.User != null)
                model.UserId = Auth.User.UserId;

            if (id > 0)
            {
                result = DataAccess.UpdateEmailTemplate(model);
            }
            else
            {
                model.Active = true;
                result = DataAccess.AddEmailTemplate(model);
                model.TemplateId = result;
            }
            ViewBag.id = result > -1 ? model.TemplateId : -1;
            return result > 0;
        }

        protected override void DoSaveSuccess(int id)
        {
            string[] names = Request.Form.GetValues("Name");
            string[] descriptions = Request.Form.GetValues("Description");
            string[] subjects = Request.Form.GetValues("Subject");
            string[] htmlDetails = Request.Form.GetValues("HtmlDetail");
            string[] textDetails = Request.Form.GetValues("TextDetail");

            EmailTemplateLang emailTemplateLang = null;

            for (int i = 0, j = names.Length; i < j; i++)
            {
                emailTemplateLang = new EmailTemplateLang();
                emailTemplateLang.TemplateId = id;
                emailTemplateLang.LangId = i + 1;
                emailTemplateLang.TemplateName = names[i].Trim();
                emailTemplateLang.Description = descriptions[i].Trim();
                emailTemplateLang.Subject = subjects[i].Trim();
                emailTemplateLang.HtmlDetailString = htmlDetails[i].Trim();
                emailTemplateLang.TextDetail = textDetails[i].Trim();

                DataAccess.SaveEmailTemplateLang(emailTemplateLang);
            }
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteEmailTemplate(id) > 0;
        }
        #endregion

        #region Action Methods
        public ActionResult Preview(string previewId)
        {
            EmailTemplate template = Session["EmailTemplate_Preview_" + previewId] as EmailTemplate;
            Dictionary<string, string> personalizeData = new Dictionary<string, string>();
            personalizeData.Add("{!order_subtotal}", "999");
            personalizeData.Add("{!payment_type}", "Credit Card");
            personalizeData.Add("{!order_table}", GetSampleOrderTableHTML());
            ViewBag.HTMLBody = PersonalizeText(template.HtmlDetail, personalizeData);
            return View(template);
        }

        private string GetSampleOrderTableHTML()
        {
            string html = "";
            return html;
        }
        #endregion

        #region Ajax Methods
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PerformPreview(string Name, string Description, string Subject, string HtmlDetail, string TextDetail)
        {
            try
            {
                EmailTemplate template = new EmailTemplate();
                template.EmailTemplateLangs.Add(new EmailTemplateLang()
                    {
                        TemplateName = Name,
                        Description = Description,
                        Subject = Subject,
                        HtmlDetailString = HtmlDetail,
                        TextDetail = TextDetail
                    });
                string k = string.Empty;
                foreach (string key in Session.Keys)
                {
                    if (key.Contains("EmailTemplate_Preview_"))
                    {
                        k = key;
                        break;
                    }
                }
                string previewId = string.Empty;
                if (!String.IsNullOrEmpty(k) && !String.IsNullOrWhiteSpace(k))
                {
                    previewId = k.Replace("EmailTemplate_Preview_", "");
                    Session[k] = template;
                }
                else
                {
                    previewId = StringHelper.RandomString();
                    Session["EmailTemplate_Preview_" + previewId] = template;
                }
                return Json(new
                {
                    isSuccess = true,
                    previewId = previewId
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
        #endregion
    }
}
