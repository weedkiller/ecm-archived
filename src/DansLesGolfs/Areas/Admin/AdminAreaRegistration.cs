using System.Web.Mvc;

namespace DansLesGolfs.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_Login",
                "Admin/Login",
                new { controller = "Logon", action = "Login" },
                new [] { "DansLesGolfs.Areas.Admin.Controllers" }
            );
            context.MapRoute(
                "Admin_Logout",
                "Admin/Logout",
                new { controller = "Logon", action = "Logout" },
                new[] { "DansLesGolfs.Areas.Admin.Controllers" }
            );
            context.MapRoute(
                "Admin_index",
                "Admin",
                new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional }
            ); 
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Admin_404_NotFound",
                "Admin/{*url}",
                new { controller = "Dashboard", action = "PageNotFound", id = UrlParameter.Optional }
            );
        }
    }
}
