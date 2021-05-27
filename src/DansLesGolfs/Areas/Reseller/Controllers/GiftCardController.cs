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
    public class GiftCardController : BaseResellerCRUDController
    {
        #region Constructor
        public GiftCardController()
        {
            ObjectName = "GiftCard";
            TitleName = Resources.Resources.GiftCard;
            PrimaryKey = "GiftCardId";

            // Define Column Names.
            ColumnNames.Add("GiftCardName", Resources.Resources.GiftCardName);
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<Course> models = DataAccess.GetAllCourses(param);
            return models;
        }

        protected override void DoPrepareForm(int? id)
        {
            List<CourseType> courseTypes = DataAccess.GetAllCourseTypes();
            ViewBag.CourseTypes = ListToDropDownList<CourseType>(courseTypes, "CourseTypeId", "CourseTypeName");

            List<Site> sites = DataAccess.GetAllSites();
            ViewBag.Sites = ListToDropDownList<Site>(sites, "SiteId", "SiteName");
        }

        protected override object DoPrepareNew()
        {
            return new Course()
            {
                Hole = 1
            };
        }

        protected override object DoPrepareEdit(long id)
        {
            Course model = DataAccess.GetCourse(id);
            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            int id = DataManager.ToInt(Request.Form["id"]);
            Course model = null;
            if (id > 0)
            {
                model = DataAccess.GetCourse(id);
                if (model == null)
                {
                    model = new Course();
                }
            }
            else
            {
                model = new Course();
            }
            model.CourseId = id;
            model.CourseTypeId = DataManager.ToInt(Request.Form["CourseTypeId"]);
            model.SiteId = DataManager.ToInt(Request.Form["SiteId"]);
            model.CourseName = DataManager.ToString(Request.Form["CourseName"]);
            model.CourseDesc = DataManager.ToString(Request.Form["CourseDesc"]);
            model.Hole = DataManager.ToInt(Request.Form["Hole"]);
            model.UpdateDate = DateTime.Now;
            if(Auth.User != null)
            model.UserId = Auth.User.UserId;

            if (id > 0)
            {
                model.InsertDate = model.InsertDate == DateTime.MinValue ? model.UpdateDate : model.InsertDate;
                result = DataAccess.UpdateCourse(model);
            }
            else
            {
                model.Active = true;
                model.InsertDate = model.UpdateDate;
                result = DataAccess.AddCourse(model);
                model.CourseId = result;
            }
            ViewBag.id = result > -1 ? model.CourseId : -1;
            return result > 0;
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteCourse(id) > 0;
        }
        #endregion
    }
}
