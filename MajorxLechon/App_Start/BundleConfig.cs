using System.Web;
using System.Web.Optimization;

namespace MajorxLechon
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Landing page | Home- css
            bundles.Add(new StyleBundle("~/Content/Home-css").Include(
                        "~/Content/bootstrap.min.css",
                        "~/Content/toastr.css",
                        "~/Content/font-awesome.min.css",
                        "~/Content/nprogress.css",
                        "~/Content/home.css",
                        "~/Content/Site.css",
                        "~/Scripts/loader.css"));

            // Landing page | Home - js
            bundles.Add(new ScriptBundle("~/Scripts/Home-js").Include(
                        "~/Scripts/jquery-3.7.1.min.js",
                        "~/Scripts/jquery-1.10.2.min.js",
                        "~/Scripts/bootstrap.min.js",
                        "~/Scripts/custom-tooltip.js",
                        "~/Scripts/toastr.js",
                        "~/Scripts/menu.js",
                        "~/Scripts/nprogress.js",
                        "~/Scripts/loader.js"));

            // Software - css
            bundles.Add(new StyleBundle("~/Content/Software-css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/toastr.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/wijmo.min.css",
                      "~/Content/nprogress.css",
                      "~/Content/loader.css",
                      "~/Content/software.css",
                      "~/Content/Site.css"));

            // System - css
            bundles.Add(new StyleBundle("~/Content/System-css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/toastr.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/nprogress.css",
                      "~/Content/loader.css",
                      "~/Content/system.css"));

            // Software -js
            bundles.Add(new ScriptBundle("~/Scripts/Software-js").Include(
                    "~/Scripts/jquery-3.7.1.min.js",
                    "~/Scripts/jquery-1.10.2.min.js",
                    "~/Scripts/bootstrap.min.js",
                    "~/Scripts/toastr.js",
                    "~/Scripts/nprogress.js",
                    "~/Scripts/loader.js",
                    "~/Scripts/menu.js",
                    "~/Scripts/software.js",
                    "~/Scripts/jquery.slimscroll.min.js",
                    "~/Scripts/app.min.js"));

            // JQueries
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // JQuery Validation
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));
        }
    }
}
