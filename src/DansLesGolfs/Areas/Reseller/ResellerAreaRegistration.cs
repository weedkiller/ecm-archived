using System.Web.Mvc;

namespace DansLesGolfs.Areas.Reseller
{
    public class ResellerAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Reseller";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Reseller_Login",
                "Reseller/Login",
                new { controller = "Logon", action = "Login" },
                new[] { "DansLesGolfs.Areas.Reseller.Controllers" }
            );
            context.MapRoute(
                "Reseller_Logout",
                "Reseller/Logout",
                new { controller = "Logon", action = "Logout" },
                new[] { "DansLesGolfs.Areas.Reseller.Controllers" }
            );
            context.MapRoute(
                "Reseller_index",
                "Reseller",
                new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Reseller_default",
                "Reseller/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Reseller_404_NotFound",
                "Reseller/{*url}",
                new { controller = "Dashboard", action = "PageNotFound", id = UrlParameter.Optional }
            );
        }
    }
}
