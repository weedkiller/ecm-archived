using BundleTransformer.Core.Builders;
using BundleTransformer.Core.Bundles;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Transformers;
using System.Web.Optimization;

namespace DansLesGolfs
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            var nullBuilder = new NullBuilder();
            var scriptTransformer = new ScriptTransformer();
            var styleTransformer = new StyleTransformer();
            var nullOrderer = new NullOrderer();

            BundleResolver.Current = new BundleResolver();

            var scriptsBundle = new Bundle("~/Bundles/js")
                                    .Include(
                                    "~/Assets/Libraries/jquery-1.10.2.min.js",
                                            "~/Assets/Front/plugins/jquery-ui/js/jquery-ui-1.10.4.custom.min.js",
                                            "~/Assets/Libraries/uniform/jquery.uniform.min.js",
                                            "~/Assets/Libraries/dlg-cart-popup/dlg.cart-popup.js",
                                            "~/Assets/Front/plugins/bootstrap/js/bootstrap.min.js",
                                            "~/Assets/Front/plugins/jquery.validate.min.js",
                                            "~/Assets/Front/plugins/jquery.validate.unobtrusive.min.js",
                                            "~/Assets/Libraries/fancybox/source/jquery.fancybox.js",
                                            "~/Assets/Libraries/jquery-unveil/jquery-unveil.js",
                                            "~/Assets/Libraries/bootstrap-starrr/bootstrap.starrr.js",
                                            "~/Assets/Front/plugins/royalslider/jquery.royalslider.min.js",
                                            "~/Assets/Libraries/mCustomScrollbar/jquery.mCustomScrollbar.js",
                                            "~/Assets/Front/scripts/product-search.js",
                                            "~/Assets/Front/scripts/script.js");
            scriptsBundle.Builder = nullBuilder;
            scriptsBundle.Transforms.Add(scriptTransformer);
            scriptsBundle.Orderer = nullOrderer;
            bundles.Add(scriptsBundle);

            var stylesBundle = new Bundle("~/Bundles/css")
                    .Include("~/Assets/Libraries/jquery-ui/themes/custom/jquery-ui.css",
                    "~/Assets/Libraries/jquery-ui/themes/custom/jquery-ui.theme.css",
                    "~/Assets/Libraries/bootstrap/css/bootstrap.css",
                    "~/Assets/Libraries/bootstrap/css/bootstrap-theme.css",
                    "~/Assets/Libraries/uniform/css/uniform.default.min.css",
                    "~/Assets/Libraries/uniform/themes/danslesgolfs/css/uniform.danslesgolfs.css",
                    "~/Assets/Libraries/fancybox/source/jquery.fancybox.css",
                    "~/Assets/Front/plugins/royalslider/royalslider.css",
                    "~/Assets/Front/plugins/royalslider/skins/default/rs-default.css",
                    "~/Assets/Libraries/mCustomScrollbar/jquery.mCustomScrollbar.css",
                    "~/Assets/Libraries/dlg-cart-popup/dlg.cart-popup.css",
                    "~/Assets/Front/css/style.css",
                    "~/Assets/Front/css/style_hockie.css");
            stylesBundle.Builder = nullBuilder;
            stylesBundle.Transforms.Add(styleTransformer);
            stylesBundle.Orderer = nullOrderer;
            bundles.Add(stylesBundle);

            BundleTable.EnableOptimizations = true;
        }
    }
}