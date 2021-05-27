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
    public class PostController : BaseResellerCRUDController
    {
        //#region Constructor
        //public PostController()
        //{
        //    ObjectName = "Post";
        //    TitleName = Resources.Resources.Post;
        //    PrimaryKey = "PostId";

        //    // Define Column Names.
        //    ColumnNames.Add("PostTitle", Resources.Resources.PostTitle);
        //}
        //#endregion

        //#region Overriden Methods
        //protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        //{
        //    List<Post> models = DataAccess.GetAllPosts(param);
        //    return models;
        //}

        //protected override void DoPrepareForm(int? id)
        //{
        //    List<PostType> PostTypes = DataAccess.GetAllPostTypes();
        //    ViewBag.PostTypes = ListToDropDownList<PostType>(PostTypes, "PostTypeId", "PostTypeName");

        //    List<Site> sites = DataAccess.GetSitesDropDownListData();
        //    ViewBag.Sites = ListToDropDownList<Site>(sites, "SiteId", "SiteName");
        //}

        //protected override object DoPrepareNew()
        //{
        //    return new Post()
        //    {
        //        Hole = 9,
        //        Duration = 2,
        //        StartTime = new TimeSpan(8, 0, 0),
        //        EndTime = new TimeSpan(18, 0, 0)
        //    };
        //}

        //protected override object DoPrepareEdit(long id)
        //{
        //    Post model = DataAccess.GetPost(id);
        //    return model;
        //}

        //protected override bool DoSave()
        //{
        //    int result = -1;
        //    int id = DataManager.ToInt(Request.Form["id"]);
        //    Post model = null;
        //    if (id > 0)
        //    {
        //        model = DataAccess.GetPost(id);
        //        if (model == null)
        //        {
        //            model = new Post();
        //        }
        //    }
        //    else
        //    {
        //        model = new Post();
        //    }
        //    model.PostId = id;
        //    model.PostTypeId = DataManager.ToInt(Request.Form["PostTypeId"]);
        //    model.SiteId = DataManager.ToInt(Request.Form["SiteId"]);
        //    model.PostName = DataManager.ToString(Request.Form["PostName"]);
        //    model.PostDesc = DataManager.ToString(Request.Form["PostDesc"]);
        //    model.Hole = DataManager.ToInt(Request.Form["Hole"]);
        //    model.DefaultPrice = DataManager.ToDecimal(Request.Form["DefaultPrice"]);
        //    model.Duration = DataManager.ToShort(Request.Form["Duration"]);
        //    model.StartTime = DataManager.ToTimeSpan(Request.Form["StartTime"]);
        //    model.EndTime = DataManager.ToTimeSpan(Request.Form["EndTime"]);
        //    model.UpdateDate = DateTime.Now;
        //    if(Auth.User != null)
        //    model.UserId = Auth.User.UserId;

        //    if (id > 0)
        //    {
        //        result = DataAccess.UpdatePost(model);
        //    }
        //    else
        //    {
        //        model.Active = true;
        //        model.InsertDate = model.UpdateDate;
        //        result = DataAccess.AddPost(model);
        //        model.PostId = result;
        //    }
        //    ViewBag.id = result > -1 ? model.PostId : -1;
        //    return result > 0;
        //}

        //protected override bool DoDelete(int id)
        //{
        //    return DataAccess.DeletePost(id) > 0;
        //}
        //#endregion
    }
}
