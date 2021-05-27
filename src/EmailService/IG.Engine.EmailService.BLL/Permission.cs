using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace IG.Engine.EmailService.BLL
{

    public class Permission
    {
        #region Fields

        public int PermissionId;
        public int RoleId;
        public int ModuleId;
        public bool AllowedView;
        public bool AllowedAdd;
        public bool AllowedEdit;
        public bool AllowedDelete;
        public bool AllowedPrint;
        public bool FullControl;
        public DateTime InsertDate;
        public DateTime UpdateDate;
        public bool Active;
        public bool IsAdmin;
        public int UserTypeId;
        public int BranchId;

        #endregion

        #region Constructors
        public Permission()
        {
        }

        public Permission(DataRow dr)
        {
            PermissionId = DataManager.ConvertToInteger(dr["PermissionId"]);
            RoleId = DataManager.ConvertToInteger(dr["RoleId"]);
            ModuleId = DataManager.ConvertToInteger(dr["ModuleId"]);
            AllowedView = DataManager.ConvertToBoolean(dr["AllowedView"]);
            AllowedAdd = DataManager.ConvertToBoolean(dr["AllowedAdd"]);
            AllowedEdit = DataManager.ConvertToBoolean(dr["AllowedEdit"]);
            AllowedDelete = DataManager.ConvertToBoolean(dr["AllowedDelete"]);
            AllowedPrint = DataManager.ConvertToBoolean(dr["AllowedPrint"]);
            FullControl = DataManager.ConvertToBoolean(dr["FullControl"]);
            InsertDate = DataManager.ConvertToDateTime(dr["InsertDate"]);
            UpdateDate = DataManager.ConvertToDateTime(dr["UpdateDate"]);
            Active = DataManager.ConvertToBoolean(dr["Active"]);
            IsAdmin = DataManager.ConvertToBoolean(dr["IsAdminRole"], false);
            UserTypeId = DataManager.ConvertToInteger(dr["UserTypeId"], 0);
            BranchId = DataManager.ConvertToInteger(dr["BranchId"], -1);
        }
        #endregion

    } 

}
