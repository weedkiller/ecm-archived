﻿using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DansLesGolfs.Controllers
{
    public class BaseResellerController : BaseController
    {
        #region Properties
        public virtual Dictionary<string, string> Breadcrumbs { get; set; }
        #endregion

        public BaseResellerController()
            : base()
        {
            Breadcrumbs = new Dictionary<string, string>();
            CheckLogin();
        }

        private void CheckLogin()
        {
            if (!Auth.Check())
            {
                System.Web.HttpContext.Current.Response.Redirect("~/Login");
            }
            else if (System.Web.HttpContext.Current.Session["LogonUserType"] != null)
            {
                int userType = DataManager.ToInt(System.Web.HttpContext.Current.Session["LogonUserType"]);
                if (userType != UserTypes.Type.Reseller && userType != UserTypes.Type.Admin && userType != UserTypes.Type.SuperAdmin)
                {
                    System.Web.HttpContext.Current.Response.Redirect("~/Login");
                }
            }
        }

        #region InitBreadcrumbs
        protected void InitBreadcrumbs()
        {
            ViewBag.Breadcrumbs = Breadcrumbs;
        } 
        #endregion

        #region RenderViewToString
        /// <summary>
        /// Render specific view into string.
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <param name="model">Model to use while rendering view.</param>
        /// <returns>String content that come from rendering specific view.</returns>
        protected string RenderViewToString(string viewName, object model = null)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContent = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContent, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewContent.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        #endregion
    }
}
