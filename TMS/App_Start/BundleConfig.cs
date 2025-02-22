﻿using System.Web;
using System.Web.Optimization;

namespace TMS
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/Others").Include(
                      "~/Scripts/Chart.bundle.js",
                      "~/Scripts/home-data.js",
                      "~/Scripts/jquery.blockui.min.js",
                      "~/Scripts/jquery.counterup.min.js",
                      "~/Scripts/jquery.min.js",
                      "~/Scripts/jquery.slimscroll.js",
                      "~/Scripts/jquery.waypoints.min.js",
                      "~/Scripts/layout.js",
                      "~/Scripts/login.js",
                      "~/Scripts/utils.js",
                      "~/Scripts/app.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/login.css",
                      "~/Content/plugins.min.css",
                      "~/Content/responsive.css",
                      "~/Content/style.css",
                      "~/Content/theme_style.css"));
        }
    }
}
