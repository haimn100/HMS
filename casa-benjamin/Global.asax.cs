using casa_benjamin.Internalization;
using NLog;
using System;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using casa_benjamin.ModelBinder;
using casa_benjamin.Modules.Shared.Services;

namespace casa_benjamin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            UnityConfig.RegisterComponents();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            CacheManager.Instance.Refresh();
            SystemTaskManager.Instance.StartTasks();

            I18n.LoadLanguages();

            System.Web.Mvc.ModelBinders.Binders[typeof(DateTime?)] =
                new NullableDateAndTimeModelBinder() { };

            System.Web.Mvc.ModelBinders.Binders[typeof(DateTime)] =
                new DateAndTimeModelBinder() { };

        }

        protected void Application_BeginRequest()
        {
            CultureInfo info = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            info.NumberFormat.NumberGroupSeparator = ",";
            info.NumberFormat.NumberDecimalSeparator = ".";
            info.NumberFormat.CurrencyGroupSeparator = ",";
            info.NumberFormat.CurrencyDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = info;
        }

        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            //log the error!
            logger.Error(ex);
        }

    }
}
