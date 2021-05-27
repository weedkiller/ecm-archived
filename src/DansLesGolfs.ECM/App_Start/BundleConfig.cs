using BundleTransformer.Core.Builders;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Transformers;
using System.Web;
using System.Web.Optimization;

namespace DansLesGolfs.ECM
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            var nullBuilder = new NullBuilder();
            var scriptTransformer = new ScriptTransformer();
            var styleTransformer = new StyleTransformer();
            var nullOrderer = new NullOrderer();

            BundleResolver.Current = new BundleResolver();

            var scriptsBundle = new Bundle("~/Bundles/js")
                                    .Include("~/Assets/Libraries/jquery-1.12.2.min.js",
                                    "~/Assets/Libraries/jquery-ui/jquery-ui-1.10.3.custom.min.js",
                                    "~/Assets/Libraries/jquery-validation/dist/jquery.validate.min.js",
                                    "~/Assets/Libraries/kendo/2016.1.112/js/kendo.all.min.js",
                                    "~/Assets/Libraries/kendo/2016.1.112/js/kendo.aspnetmvc.min.js",
                                    "~/Assets/Libraries/bootstrap/js/bootstrap.min.js",
                                    "~/Assets/Libraries/bootstrap-hover-dropdown/twitter-bootstrap-hover-dropdown.min.js",
                                    "~/Assets/Libraries/jquery-slimscroll/jquery.slimscroll.min.js",
                                    "~/Assets/Libraries/ckeditor/ckeditor.js",
                                    "~/Assets/Libraries/ckeditor/config.js",
                                    "~/Assets/Libraries/grapesjs/grapes.min.js",
                                    "~/Assets/Libraries/grapesjs/plugins/ckeditor/grapesjs-plugin-ckeditor.min.js",
                                    "~/Assets/Libraries/grapesjs/plugins/newsletter/grapesjs-preset-newsletter.min.js",
                                    "~/Assets/Libraries/grapesjs/plugins/mjml/grapesjs-mjml.min.js",
                                    "~/Assets/Libraries/grapesjs/plugins/aviary/grapesjs-aviary.min.js",
                                    "~/Assets/Libraries/jquery.blockui.min.js",
                                    "~/Assets/Libraries/jquery.cokie.min.js",
                                    "~/Assets/Libraries/uniform/jquery.uniform.min.js",
                                    "~/Assets/Libraries/flot/jquery.flot.js",
                                    "~/Assets/Libraries/flot/jquery.flot.resize.js",
                                    "~/Assets/Libraries/jquery.pulsate.min.js",
                                    "~/Assets/Libraries/bootstrap-daterangepicker/moment.min.js",
                                    "~/Assets/Libraries/bootstrap-daterangepicker/daterangepicker.js",
                                    "~/Assets/Libraries/gritter/js/jquery.gritter.js",
                                    "~/Assets/Libraries/fullcalendar/fullcalendar/fullcalendar.min.js",
                                    "~/Assets/Libraries/jquery-easy-pie-chart/jquery.easy-pie-chart.js",
                                    "~/Assets/Libraries/jquery.sparkline.min.js",
                                    "~/Assets/Libraries/fancybox/source/jquery.fancybox.js",
                                    "~/Assets/Libraries/jquery.teesheet/i18n/fr-FR.js",
                                    "~/Assets/Libraries/jquery-ui/i18n/datepicker-fr.js",
                                    "~/Assets/Admin/scripts/app.js",
                                    "~/Assets/Admin/scripts/tasks.js",
                                    "~/Assets/Libraries/data-tables/js/jquery.dataTables.min.js",
                                    "~/Assets/Libraries/data-tables/DT_bootstrap.js",
                                    "~/Assets/Libraries/uploadifive/jquery.uploadifive.min.js",
                                    "~/Assets/Front/plugins/uniform/jquery.uniform.min.js",
                                    "~/Assets/Libraries/signalr/jquery.signalR-2.2.1.min.js");
            scriptsBundle.Builder = nullBuilder;
            scriptsBundle.Transforms.Add(scriptTransformer);
            scriptsBundle.Orderer = nullOrderer;
            bundles.Add(scriptsBundle);

            var stylesBundle = new Bundle("~/Bundles/css")
                                    .Include("~/Assets/Libraries/font-awesome/css/font-awesome.min.css",
                                    "~/Assets/Libraries/bootstrap/css/bootstrap.min.css",
                                    "~/Assets/Libraries/uniform/css/uniform.default.css",
                                    "~/Assets/Libraries/grapesjs/css/grapes.min.css",
                                    "~/Assets/Libraries/grapesjs/css/grapesjs-mjml.css",
                                    "~/Assets/Libraries/grapesjs/presets/newsletter/grapesjs-preset-newsletter.css",
                                    "~/Assets/Libraries/kendo/2016.1.112/css/kendo.common.min.css",
                                    "~/Assets/Libraries/kendo/2016.1.112/css/kendo.default.min.css",
                                    "~/Assets/Libraries/kendo/2016.1.112/css/kendo.default.mobile.min.css",
                                    "~/Assets/Libraries/ckeditor/content.css",
                                    "~/Assets/Libraries/ckeditor/skins/moono-lisa/editor.css",
                                    "~/Assets/Libraries/ckeditor/skins/moono-lisa/dialog.css",
                                    "~/Assets/Libraries/gritter/css/jquery.gritter.css",
                                    "~/Assets/Libraries/bootstrap-daterangepicker/daterangepicker-bs3.css",
                                    "~/Assets/Libraries/fullcalendar/fullcalendar/fullcalendar.css",
                                    "~/Assets/Libraries/jqvmap/jqvmap/jqvmap.css",
                                    "~/Assets/Libraries/jquery-easy-pie-chart/jquery.easy-pie-chart.css",
                                    "~/Assets/Libraries/data-tables/DT_bootstrap.css",
                                    "~/Assets/Admin/css/style-metronic.css",
                                    "~/Assets/Admin/css/style.css",
                                    "~/Assets/Admin/css/style-responsive.css",
                                    "~/Assets/Admin/css/plugins.css",
                                    "~/Assets/Admin/css/pages/tasks.css",
                                    "~/Assets/Libraries/jquery-ui/themes/custom/jquery-ui.min.css",
                                    "~/Assets/Libraries/jquery-ui/themes/custom/jquery-ui.theme.min.css",
                                    "~/Assets/Libraries/bootstrap/css/bootstrap.min.css",
                                    "~/Assets/Libraries/bootstrap/css/bootstrap-theme.min.css",
                                    "~/Assets/Libraries/fancybox/source/jquery.fancybox.css",
                                    "~/Assets/Libraries/uploadifive/uploadifive.css",
                                    "~/Assets/Libraries/uniform/css/uniform.default.min.css",
                                    "~/Assets/Libraries/uniform/themes/danslesgolfs/css/uniform.danslesgolfs.css",
                                    "~/Assets/Front/css/style.css",
                                    "~/Assets/Front/css/style_hockie.css",
                                    "~/Assets/Admin/css/custom.css");
            stylesBundle.Builder = nullBuilder;
            stylesBundle.Transforms.Add(styleTransformer);
            stylesBundle.Orderer = nullOrderer;
            bundles.Add(stylesBundle);

            BundleTable.EnableOptimizations = false;
        }
    }
}
