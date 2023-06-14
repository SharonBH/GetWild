using System.Web;
using System.Web.Optimization;
using InShapeModels;
using Utilities;

namespace GetWild
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/_extensions.js"));


            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
            "~/Scripts/jquery-ui-{version}.js",
            "~/Scripts/jquery-ui-timepicker-addon.js",
            "~/scripts/jquery.popconfirm.js"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/moment-with-locales.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));

            var cdnPath = "https://fonts.googleapis.com/earlyaccess/opensanshebrew.css";
            bundles.Add(new StyleBundle("~/Content/fonts", cdnPath));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-rtl.css",
                "~/Content/jquery-ui-timepicker-addon.css", //));
                "~/Content/sitebase.css"));
                //"~/Content/"+ App.CurrentCompany.CSSFileName));


            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
            "~/Content/themes/base/accordion.css",
            "~/Content/themes/base/autocomplete.css",
            "~/Content/themes/base/button.css",
            "~/Content/themes/base/core.css",
            "~/Content/themes/base/datepicker.css",
            "~/Content/themes/base/dialog.css",
            "~/Content/themes/base/draggable.css",
            "~/Content/themes/base/menu.css",
            "~/Content/themes/base/progressbar.css",
            "~/Content/themes/base/resizable.css",
            "~/Content/themes/base/selectable.css",
            "~/Content/themes/base/selectmenu.css",
            "~/Content/themes/base/slider.css",
            "~/Content/themes/base/sortable.css",
            "~/Content/themes/base/spinner.css",
            "~/Content/themes/base/tabs.css",
            "~/Content/themes/base/theme.css",
            "~/Content/themes/base/tooltip.css"));

            //BundleTable.EnableOptimizations = true;
        }
    }
}
