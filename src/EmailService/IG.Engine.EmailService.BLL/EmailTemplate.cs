using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace IG.Engine.EmailService.BLL
{
    public class EmailTemplate
    {
        #region Fields
        public int TemplateId;
        public int CategoryId;
        public string Name;
        public string Description;
        public string HtmlDetail;
        public string TextDetail;
        public int UserId;
        public bool Active;

        public string FileName1;
        public string FileName2;
        public string FileName3;
        public string FileName4;
        public string FileName5;

        public string FileDescription1;
        public string FileDescription2;
        public string FileDescription3;
        public string FileDescription4;
        public string FileDescription5;

        public string FileUrl1;
        public string FileUrl2;
        public string FileUrl3;
        public string FileUrl4;
        public string FileUrl5;

        #endregion

        #region Properties
        #endregion

        #region Constructors
        public EmailTemplate()
        {
        }

        public EmailTemplate(DataRow dr)
        {
            TemplateId = DataManager.ConvertToInteger(dr["TemplateId"]);
            CategoryId = DataManager.ConvertToInteger(dr["CategoryId"]);
            Name = DataManager.ConvertToString(dr["Name"]);
            Description = DataManager.ConvertToString(dr["Description"]);
            HtmlDetail = DataManager.ConvertToString(dr["HtmlDetail"]);
            TextDetail = DataManager.ConvertToString(dr["TextDetail"]);
            UserId = DataManager.ConvertToInteger(dr["UserId"]);
            Active = DataManager.ConvertToBoolean(dr["Active"]);

            FileName1 = DataManager.ConvertToString(dr["FileName1"]);
            FileName2 = DataManager.ConvertToString(dr["FileName2"]);
            FileName3 = DataManager.ConvertToString(dr["FileName3"]);
            FileName4 = DataManager.ConvertToString(dr["FileName4"]);
            FileName5 = DataManager.ConvertToString(dr["FileName5"]);

            FileDescription1 = DataManager.ConvertToString(dr["FileDescription1"]);
            FileDescription2 = DataManager.ConvertToString(dr["FileDescription2"]);
            FileDescription3 = DataManager.ConvertToString(dr["FileDescription3"]);
            FileDescription4 = DataManager.ConvertToString(dr["FileDescription4"]);
            FileDescription5 = DataManager.ConvertToString(dr["FileDescription5"]);

            FileUrl1 = DataManager.ConvertToString(dr["FileUrl1"]);
            FileUrl2 = DataManager.ConvertToString(dr["FileUrl2"]);
            FileUrl3 = DataManager.ConvertToString(dr["FileUrl3"]);
            FileUrl4 = DataManager.ConvertToString(dr["FileUrl4"]);
            FileUrl5 = DataManager.ConvertToString(dr["FileUrl5"]);

        }
        #endregion
    }
}
