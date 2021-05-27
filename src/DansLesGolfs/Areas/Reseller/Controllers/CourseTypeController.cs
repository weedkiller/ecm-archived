using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Reseller.Controllers
{
    public class CourseTypeController : BaseResellerCRUDController
    {
        #region Constructor
        public CourseTypeController()
        {
            ObjectName = "CourseType";
            TitleName = "Course Type";
            PrimaryKey = "CourseTypeId";

            // Define Column Names.
            ColumnNames.Add("CourseTypeName", "Course Type Name");
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<CourseType> models = DataAccess.GetAllCourseTypes(param);
            return models;
        }

        protected override object DoPrepareNew()
        {
            return new CourseType();
        }

        protected override object DoPrepareEdit(long id)
        {
            CourseType model = DataAccess.GetCourseType(id);
            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            CourseType model = null;
            int id = DataManager.ToInt(Request.Form["id"]);
            if (id > 0)
            {
                model = DataAccess.GetCourseType(id);
                if (model == null)
                {
                    model = new CourseType();
                }
            }
            else
            {
                model = new CourseType();
            }
            model.CourseTypeId = id;
            model.CourseTypeName = DataManager.ToString(Request.Form["CourseTypeName"]).Trim();

            if (id > -1)
            {
                result = DataAccess.UpdateCourseType(model);
            }
            else
            {
                result = DataAccess.AddCourseType(model);
                model.CourseTypeId = result;
            }
            ViewBag.id = result > -1 ? model.CourseTypeId : -1;
            return result > 0;
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteCourseType(id) > 0;
        }
        #endregion
    }
}
