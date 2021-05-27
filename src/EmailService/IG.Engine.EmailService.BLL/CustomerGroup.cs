using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace IG.Engine.EmailService.BLL
{
    public class CustomerGroup
    {
        #region Fields
        public int CustomerGroupId;
        public string CustomerGroupName;
        public bool Active;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public CustomerGroup()
        {
        }

        public CustomerGroup(DataRow dr)
        {
            CustomerGroupId = DataManager.ConvertToInteger(dr["CustomerGroupId"]);
            CustomerGroupName = DataManager.ConvertToString(dr["CustomerGroupName"]);
            Active = DataManager.ConvertToBoolean(dr["Active"]); 
        }
        #endregion

    }
}
