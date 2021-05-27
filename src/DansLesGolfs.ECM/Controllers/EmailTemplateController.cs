using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.ECM.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI.Fluent;
using System.IO;
using System.Drawing;
using System.Text;

namespace DansLesGolfs.ECM.Controllers
{
    public class EmailTemplateController : BaseECMCRUDController<EmailTemplateViewModel>
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

            AllowUserTypes.Add(UserType.Type.SuperAdmin);
            AllowUserTypes.Add(UserType.Type.Admin);
        }
        #endregion

        #region Overriden Methods
        protected override void DoSetGridColumns(GridColumnFactory<EmailTemplateViewModel> columns)
        {
            columns.Bound(c => c.TemplateName).Title(Resources.Resources.Name);
        }
        protected override IQueryable<EmailTemplateViewModel> DoLoadDataJSON()
        {
            return DataAccess.GetAllEmailTemplates(1).OrderByDescending(it => it.TemplateId);
        }

        protected override object DoPrepareNew()
        {
            return new EmailTemplate();
        }

        protected override object DoPrepareEdit(long id)
        {
            EmailTemplate model = DataAccess.GetEmailTemplate(id, 1);
            ViewBag.Attachments = DataAccess.GetEmailTemplateAttachmentsByTemplateId(id);
            return model;
        }

        protected override void DoPrepareForm(int? id = 0)
        {
            ViewBag.EmailTemplateVariables = DataAccess.GetAllEmailTemplateVariables();
            ViewBag.EditorLimitAccess = Auth.User.UserTypeId != UserType.Type.SuperAdmin && Auth.User.UserTypeId != UserType.Type.Admin;
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
            string[] names = Request.Form.GetValues("TemplateName");
            string[] descriptions = Request.Form.GetValues("Description");
            string[] subjects = Request.Form.GetValues("Subject");
            string[] htmlDetails = Request.Form.GetValues("HtmlDetailString");

            string[] attachmentIds = Request.Form.GetValues("AttachmentIds");
            string[] fileNames = Request.Form.GetValues("FileNames");
            string[] baseNames = Request.Form.GetValues("BaseNames");
            string[] fileExtensions = Request.Form.GetValues("FileExtensions");
            string[] filePaths = Request.Form.GetValues("FilePaths");
            string[] deletedAttachmentIds = Request.Form["deletedAttachmentIds"].Split(',');

            EmailTemplateLang emailTemplateLang = null;

            if (names != null && names.Any())
            {
                emailTemplateLang = new EmailTemplateLang();
                emailTemplateLang.TemplateId = id;
                emailTemplateLang.LangId = 1;
                emailTemplateLang.TemplateName = names[0].Trim();
                emailTemplateLang.Description = descriptions[0].Trim();
                emailTemplateLang.Subject = subjects[0].Trim();
                emailTemplateLang.HtmlDetailString = htmlDetails[0].Trim();
                DataAccess.SaveEmailTemplateLang(emailTemplateLang);
            }

            EmailTemplateAttachment attachment = null;
            string uploadPath = Server.MapPath("~/Uploads/Emailing/" + id);
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            string destPath = string.Empty;
            string destFileName = string.Empty;
            int fileIndex = 1;
            if (attachmentIds != null && attachmentIds.Any())
            {
                for (int i = 0, n = attachmentIds.Length; i < n; i++)
                {
                    if (!System.IO.File.Exists(filePaths[i]))
                        continue;

                    fileIndex = 0;
                    destPath = Path.Combine(uploadPath, fileNames[i]);
                    while (System.IO.File.Exists(destPath))
                    {
                        baseNames[i] = baseNames[i] + " (" + (fileIndex++) + ")";
                        fileNames[i] = baseNames[i] + fileExtensions[i];
                        destPath = Path.Combine(uploadPath, fileNames[i]);
                    }
                    System.IO.File.Move(filePaths[i], destPath);

                    attachment = new EmailTemplateAttachment();
                    attachment.TemplateId = id;
                    attachment.FileName = fileNames[i];
                    attachment.BaseName = baseNames[i];
                    attachment.FileExtension = fileExtensions[i];
                    attachment.FilePath = destPath;
                    DataAccess.AddEmailTemplateAttachment(attachment);
                }
            }

            long attachmentId = 0;
            if (deletedAttachmentIds != null && deletedAttachmentIds.Any())
            {
                foreach (var it in deletedAttachmentIds)
                {
                    if (String.IsNullOrWhiteSpace(it))
                        continue;

                    attachmentId = DataManager.ToLong(it);
                    attachment = DataAccess.GetEmailTemplateAttachment(attachmentId);
                    if (attachment == null)
                        continue;

                    if (System.IO.File.Exists(attachment.FilePath))
                    {
                        @System.IO.File.Decrypt(attachment.FilePath);
                    }
                    DataAccess.DeleteEmailTemplateAttachment(attachmentId);
                }
            }
        }

        protected override bool DoDelete(long id)
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
            ViewBag.HTMLBody = PersonalizeText(template.HtmlDetailString, personalizeData);
            return View(template);
        }
        public ActionResult PreviewRaw(long? id)
        {
            EmailTemplate template = DataAccess.GetEmailTemplate(id.HasValue ? id.Value : 0, 1);
            if (template != null)
            {
                return Content(template.HtmlDetailString, "text/html", Encoding.UTF8);
            }
            else
            {
                return Content("Template not found.", "text/html", Encoding.UTF8);
            }
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
        public ActionResult PerformPreview(string Name, string Description, string Subject, string HtmlDetailString, string TextDetailString)
        {
            try
            {
                EmailTemplate template = new EmailTemplate();
                template.EmailTemplateLangs.Add(new EmailTemplateLang()
                {
                    TemplateName = Name,
                    Description = Description,
                    Subject = Subject,
                    HtmlDetailString = HtmlDetailString,
                    TextDetail = TextDetailString
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

                string unsubscribeMailTo = "", unsubscribeUrl = "";
                template.HtmlDetailString = InsertTrackingLinks(template.HtmlDetailString, 0, 0, 0, 0, "sample@gmail.com", ref unsubscribeMailTo, ref unsubscribeUrl);

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

        [HttpPost]
        public ActionResult UploadAttachments()
        {
            int id = DataManager.ToInt(Request.Form["id"]);
            string uploadDir = Path.GetTempPath();
            string tempFileName = string.Empty;
            string imagePath = string.Empty;
            string fileName = string.Empty;
            string baseName = string.Empty;
            string extension = string.Empty;

            var file = Request.Files["Filedata"];
            baseName = Path.GetFileNameWithoutExtension(file.FileName);
            extension = Path.GetExtension(file.FileName).ToLower();
            fileName = file.FileName;
            do
            {
                tempFileName = Path.GetTempFileName() + extension;
                imagePath = Path.Combine(uploadDir, tempFileName);
            } while (System.IO.File.Exists(imagePath));
            file.SaveAs(imagePath);

            if (System.IO.File.Exists(imagePath))
            {
                return Content(fileName + "|" + baseName + "|" + extension + "|" + imagePath);
            }
            else
            {
                throw new Exception("Can't save file please try again.");
            }
        }
    }
}
