using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace IG.Engine.EmailService.BLL
{ 
    // Role

    public class Role
    {
        #region Fields
        public int RoleId;
        public string RoleName;
        public string Description;
        public bool Active;
        public bool IsAdminRole;
        #endregion

        #region Constructors
        public Role()
        {
        }

        public Role(DataRow dr)
        {
            RoleId = DataManager.ConvertToInteger(dr["RoleId"]);
            RoleName = DataManager.ConvertToString(dr["RoleName"]);
            Description = DataManager.ConvertToString(dr["Description"]);
            Active = DataManager.ConvertToBoolean(dr["Active"]);
            IsAdminRole = DataManager.ConvertToBoolean(dr["IsAdminRole"],false );
        }
        #endregion

    }

}
