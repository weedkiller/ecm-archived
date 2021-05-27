using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace IG.Engine.EmailService.BLL
{
    public class Emailing
    {
        #region Fields 

        public int EmailId;
        public string EmailName;
        public string Subject;
        public string FromName;
        public string FromEmail;
        public int EmailFormatId;
        public int TemplateId;
        public DateTime InsertDate;
        public DateTime UpdateDate;
        public bool Active;
        public int UserId;
        public int StatusId;
        public string HtmlDetail;
        public string TextDetail;

        public string TemplateName;

        #endregion

        #region Constructors 

        public Emailing()
        { }

        public Emailing(DataRow dr)
        {
            EmailId = DataManager.ConvertToInteger(dr["EmailId"]);
            EmailName = DataManager.ConvertToString(dr["EmailName"]);
            Subject = DataManager.ConvertToString(dr["Subject"]);
            FromName = DataManager.ConvertToString(dr["FromName"]);
            FromEmail = DataManager.ConvertToString(dr["FromEmail"]);
            EmailFormatId = DataManager.ConvertToInteger(dr["EmailFormatId"]);
            TemplateId = DataManager.ConvertToInteger(dr["TemplateId"]);
            Active = DataManager.ConvertToBoolean(dr["Active"], true);
            UserId = DataManager.ConvertToInteger(dr["UserId"]);
            StatusId = DataManager.ConvertToInteger(dr["StatusId"]);

            HtmlDetail = DataManager.ConvertToString(dr["HtmlDetail"]);
            TextDetail = DataManager.ConvertToString(dr["TextDetail"]);

            if (dr.Table.Columns.Contains("TemplateName"))
            {
                TemplateName = DataManager.ConvertToString(dr["TemplateName"]);
            }
        }

        #endregion


    }
}
