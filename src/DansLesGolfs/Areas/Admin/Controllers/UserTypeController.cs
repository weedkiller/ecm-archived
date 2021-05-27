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
    public class UserTypeController : BaseAdminCRUDController
    {
        #region Constructor
        public UserTypeController()
        {
            ObjectName = "UserType";
            TitleName = "User Type";
            PrimaryKey = "UserTypeId";

            // Define Column Names.
            ColumnNames.Add("UserTypeName", "User Type Name");
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<UserType> models = DataAccess.GetAllUserTypes(param);
            return models;
        }

        protected override object DoPrepareNew()
        {
            return new
            {
                UserTypeName = string.Empty
            };
        }

        protected override object DoPrepareEdit(long id)
        {
            UserType model = DataAccess.GetUserType(id);
            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            int id = DataManager.ToInt(Request.Form["id"], -1);
            UserType model = null;
            if(id > 0)
            {
                model = DataAccess.GetUserType(id);
                if(model == null)
                {
                    model = new UserType();
                }
            }
            else
            {
                model = new UserType();
            }
            model.UserTypeId = id;
            model.UserTypeName = DataManager.ToString(Request.Form["UserTypeName"]).Trim();

            if (id > 0)
            {
                result = DataAccess.UpdateUserType(model);
            }
            else
            {
                result = DataAccess.AddUserType(model);
                model.UserTypeId = result;
            }
            ViewBag.id = result > -1 ? model.UserTypeId : -1;
            return result > 0;
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteUserType(id) > 0;
        }
        #endregion
    }
}
