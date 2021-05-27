using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace IG.Engine.EmailService.BLL
{
    public class MailingList
    {
        #region Fields
        public int MailingListId;
        public string MailingListName;
        public bool Active;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public MailingList()
        {
        }

        public MailingList(DataRow dr)
        {
            MailingListId = DataManager.ConvertToInteger(dr["MailingListId"]);
            MailingListName = DataManager.ConvertToString(dr["MailingListName"]);
            Active = DataManager.ConvertToBoolean(dr["Active"]); 
        }
        #endregion

    }
}
