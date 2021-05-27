using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DansLesGolfs.ECM
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Ignore route statements.
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Login",
                url: "Login",
                defaults: new { namespaces = "DansLesGolfs.ECM.Controllers", controller = "Logon", action = "Login" },
                namespaces: new string[] { "DansLesGolfs.ECM.Controllers" }
            );

            routes.MapRoute(
                name: "Logout",
                url: "Logout",
                defaults: new { namespaces = "DansLesGolfs.ECM.Controllers", controller = "Logon", action = "Logout" },
                namespaces: new string[] { "DansLesGolfs.ECM.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { namespaces = "DansLesGolfs.ECM.Controllers", controller = "Emailing", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
