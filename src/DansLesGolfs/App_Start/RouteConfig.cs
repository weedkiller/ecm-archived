using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DansLesGolfs
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.RouteExistingFiles = true;

            routes.MapRoute(
                name: "Thumbnail",
                url: "Thumb/{path}/{w}/{h}",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Thumb", action = "Index" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );
            routes.MapRoute(
                name: "Login",
                url: "Login",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Logon", action = "Login" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );
            routes.MapRoute(
                name: "Logout",
                url: "Logout",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Logon", action = "Logout" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );
            routes.MapRoute(
                name: "Register",
                url: "Register",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Logon", action = "Register" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );
            routes.MapRoute(
                name: "ForgotPassword",
                url: "ForgotPassword",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Logon", action = "ForgotPassword" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );
            routes.MapRoute(
                name: "LoginViaFacebook",
                url: "Logon/LoginViaFacebook",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Logon", action = "LoginViaFacebook" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );
            routes.MapRoute(
                name: "DoRetrievePassword",
                url: "DoRetrievePassword",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Logon", action = "DoRetrievePassword" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );
            routes.MapRoute(
                name: "ProductSearch",
                url: "Product/Search",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Product", action = "Search" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );
            routes.MapRoute(
                name: "Shop",
                url: "Shop",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Product", action = "Index" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );
            routes.MapRoute(
                name: "DLGShop",
                url: "DLGShop",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Product", action = "DLGShop" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );
            routes.MapRoute(
                name: "ProductSite",
                url: "Product/Site/{slug}",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Product", action = "Site" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );
            routes.MapRoute(
                name: "GreenFees",
                url: "GreenFee",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Product", action = "GreenFees" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );
            routes.MapRoute(
                name: "StayPackages",
                url: "StayPackage",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Product", action = "StayPackages" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );
            routes.MapRoute(
                name: "GolfLessons",
                url: "GolfLesson",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Product", action = "GolfLessons" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );
            routes.MapRoute(
                name: "DrivingRanges",
                url: "DrivingRange",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Product", action = "DrivingRanges" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );
            routes.MapRoute(
                name: "ItemAjax_Default",
                url: "ItemAjax/{action}/{id}",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "ItemAjax", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "ItemPrint",
                url: "Item/Print/{slug}",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Item", action = "Print" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );
            routes.MapRoute(
                name: "ItemDetail",
                url: "Item/{slug}",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Item", action = "Detail" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );
            routes.MapRoute(
                name: "TeeSheetTableDetail",
                url: "TeeSheet/Table/{slug}",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "TeeSheet", action = "Table" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );
            routes.MapRoute(
                name: "SiteDetail",
                url: "Site/{slug}",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Site", action = "Detail" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );
            routes.MapRoute(
                name: "Logon",
                url: "Logon/{action}/{id}",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Logon", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Item_Default",
                url: "Item/{action}/{id}",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Item", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "DLGCard_Default",
                url: "DLGCard/{action}/{id}",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "DLGCard", action = "Index", id = "" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }

            );
            routes.MapRoute(
                name: "Cart_Default",
                url: "Cart/{action}/{id}",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Cart", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Order_Default",
                url: "Order/{action}/{id}",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Order", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "FindCourse_Default",
                url: "FindCourse/{action}/{id}",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "FindCourse", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Conversation",
                url: "Message/Conversation/{name}",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Message", action = "Conversation", name = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "TeeSheet_Default",
                url: "TeeSheet/{action}/{id}",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "TeeSheet", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "TopNavIcon1",
                url: "TopNavIcon1",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Home", action = "TopNavIcon1" }
            );

            routes.MapRoute(
                name: "TopNavIcon2",
                url: "TopNavIcon2",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Home", action = "TopNavIcon2" }
            );

            routes.MapRoute(
                name: "PageDetail",
                url: "Page/{contentKey}",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Page", action = "Detail" },
                namespaces: new string[] { "DansLesGolfs.Controllers" }
            );

            routes.MapRoute(
                name: "Unsubscribe",
                url: "unsubscribe",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Home", action = "Unsubscribe" }
            );

            routes.MapRoute(
                name: "Home_Default",
                url: "Home/{action}/{id}",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SiteMapIndex",
                url: "sitemap_index.xml",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "SiteMap", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ProductSiteMap",
                url: "product-sitemap.xml",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "SiteMap", action = "Product", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "GolfSiteMap",
                url: "golf-sitemap.xml",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "SiteMap", action = "Golf", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SiteMapStylesheet",
                url: "main-sitemap.xsl",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "SiteMap", action = "Stylesheet", id = UrlParameter.Optional }
            );


            /*routes.MapRoute(
                name: "404-PageNotFound",
                url: "{*url}",
                defaults: new { controller = "Error", action = "PageNotFound" }
            );*/
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { namespaces = "DansLesGolfs.Controllers", controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}