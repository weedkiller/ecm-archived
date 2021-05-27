using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace IG.Engine.EmailService.BLL
{
    public class EmailLog
    {
        #region " Fields & Properties "

        public int Id;
        public int EmailQueId;
        public int Status;
        public int Resending;
        public DateTime InsertDate; 
        public string ReturnMessage;

        #endregion

        #region " Constructors "

        public EmailLog()
        { 
        }

        public EmailLog(DataRow dr)
        {
            Id = DataManager.ConvertToInteger(dr["Id"]);
            EmailQueId = DataManager.ConvertToInteger(dr["EmailQueId"]);
            Status = DataManager.ConvertToInteger(dr["Status"]);
            Resending = DataManager.ConvertToInteger(dr["Resending"]);
            ReturnMessage = DataManager.ConvertToString(dr["ReturnMessage"]);
        }

        #endregion

    }
}
