using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace IG.Engine.EmailService.BLL
{
    public class EmailQue
    {
        private byte[] htmlDetail;
        private byte[] textDetail;

        public int EmailQueId;
        public int EmailId;
        public int CustomerId;
        public string Email;
        public int Status;
        public DateTime InsertDate;
        public DateTime SendDate;
        public bool IsError;
        public bool IsResent;

        public string HtmlDetail
        {
            get
            {
                if (htmlDetail == null)
                {
                    htmlDetail = new byte[0];
                    return string.Empty;
                }
                else
                {
                    return Encoding.UTF8.GetString(htmlDetail);
                }
            }
            set
            {
                htmlDetail = Encoding.UTF8.GetBytes(value);
            }
        }

        public string TextDetail
        {
            get
            {
                if (textDetail == null)
                {
                    textDetail = new byte[0];
                    return string.Empty;
                }
                else
                {
                    return Encoding.UTF8.GetString(textDetail);
                }
            }
            set
            {
                textDetail = Encoding.UTF8.GetBytes(value);
            }
        }

        public bool IsOpenMail;
        public bool IsClickLink;
        public byte[] HtmlDetailBytes
        {
            get { return htmlDetail; }
            set { htmlDetail = value; }
        }
        public byte[] TextDetailBytes
        {
            get { return textDetail; }
            set { textDetail = value; }
        }

        public EmailQue()
        { }

        public EmailQue(DataRow dr)
        {
            EmailQueId = DataManager.ConvertToInteger(dr["EmailQueid"]);
            EmailId = DataManager.ConvertToInteger(dr["Emailid"]);
            CustomerId = DataManager.ConvertToInteger(dr["CustomerId"]);
            Email = DataManager.ConvertToString(dr["Email"]);
            Status = DataManager.ConvertToInteger(dr["Status"]);
            InsertDate = DataManager.ConvertToDateTime(dr["InsertDate"]);
            SendDate = DataManager.ConvertToDateTime(dr["SendDate"]);
            IsError = DataManager.ConvertToBoolean(dr["IsError"]);
            IsResent = DataManager.ConvertToBoolean(dr["IsResent"]);
            if (dr["HtmlDetail"] != null && dr["HtmlDetail"] != DBNull.Value)
            {
                HtmlDetailBytes = dr["HtmlDetail"] as byte[];
            }
            if (dr["TextDetail"] != null && dr["TextDetail"] != DBNull.Value)
            {
                TextDetailBytes = dr["TextDetail"] as byte[];
            }
            IsOpenMail = DataManager.ConvertToBoolean(dr["IsOpenMail"]);
            IsClickLink = DataManager.ConvertToBoolean(dr["IsClickLink"]);
        }

    }
}
