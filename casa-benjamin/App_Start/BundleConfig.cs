using System.Collections.Generic;
using System.Web.Optimization;

namespace casa_benjamin
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            #if DEBUG
            BundleTable.EnableOptimizations = false;
#else
                            BundleTable.EnableOptimizations = true;
#endif

            var jsBundle = new Bundle("~/bundles/prebodyjs").Include(
                 "~/plugins/jquery/jquery.min.js",
                  "~/js/angular.min.js",
                 "~/plugins/angualr-material/angular-animate.min.js",
                  "~/plugins/angualr-material/angular-aria.min.js",
                  "~/plugins/angualr-material/angular-messages.min.js",
                 "~/plugins/angualr-material/angular-material.min.js",
                 "~/js/mustache.min.js",
                 "~/js/underscore-min.js",
                 "~/js/eventBus.js",
                 "~/plugins/bootstrap/js/bootstrap.js",
                 "~/js/select2.full.min.js",
                 "~/js/highcharts.js",
                "~/js/chartHelper.js",
                   "~/plugins/momentjs/moment.js",
                 "~/js/app.js",
                 "~/js/app.date.js",
                 "~/i18n/i18n.js",
                 "~/js/table.js",
                 "~/plugins/bootstrap/js/bootstrap.js",
              "~/js/select2.full.min.js",
              "~/plugins/jquery-slimscroll/jquery.slimscroll.js",
              "~/js/tippy.min.js",
              "~/js/dropdowns-enhancement.js",
              //"~/js/admin.js",
              "~/js/jquery-ui.min.js",
              "~/plugins/jquery-datatable/jquery.dataTables.js",
              "~/plugins/jquery-datatable/skin/bootstrap/js/dataTables.bootstrap.js",
              "~/plugins/jquery-datatable/extensions/export/dataTables.buttons.min.js",
              "~/plugins/jquery-datatable/extensions/export/buttons.flash.min.js",
              "~/plugins/jquery-datatable/extensions/export/jszip.min.js",
              "~/plugins/jquery-datatable/extensions/export/pdfmake.min.js",
              "~/plugins/jquery-datatable/extensions/export/vfs_fonts.js",
              "~/plugins/jquery-datatable/extensions/export/buttons.html5.min.js",
              "~/plugins/jquery-datatable/extensions/export/buttons.print.min.js",

              "~/plugins/bootstrap-material-datetimepicker/js/bootstrap-material-datetimepicker.js",
              "~/plugins/bootstrap-daterangepicker-master/daterangepicker.js",
              "~/js/lightense.min.js",
              "~/js/plugins.js",
              "~/js/pages/nav-bar.js",
              "~/plugins/jquery-inputmask/jquery.inputmask.bundle.js",
              "~/js/numeral.js"            
              );
            jsBundle.Orderer = new NonOrderingBundleOrderer();
            bundles.Add(jsBundle);


            var cssBundle = new Bundle("~/bundles/css").Include(
                "~/fonts/beds/flaticon.css",
                 "~/plugins/bootstrap/css/bootstrap.css",
                 "~/plugins/jquery-datatable/skin/bootstrap/css/dataTables.bootstrap.css",
                 "~/plugins/node-waves/waves.css",
                 "~/plugins/animate-css/animate.css",
                 "~/plugins/morrisjs/morris.css",
                 "~/css/style.css",
                 "~/css/themes/theme-deep-purple.min.css",
                 "~/css/select2.css",
                 "~/css/app.css",
                 "~/css/alertify.css",
                 "~/css/tippy.css",
                 "~/css/dropdowns-enhancement.css",
                 "~/css/jquery-ui.css",
                 "~/css/override.css",
                 "~/plugins/bootstrap-material-datetimepicker/css/bootstrap-material-datetimepicker.css",
                 "~/plugins/bootstrap-daterangepicker-master/daterangepicker.css",
                 "~/plugins/angualr-material/angular-material.min.css"
                 );
            cssBundle.Orderer = new NonOrderingBundleOrderer();

            bundles.Add(cssBundle);
        }

        class NonOrderingBundleOrderer : IBundleOrderer
        {
            public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
            {
                return files;
            }
        }
    }
}
