using casa_benjamin.ActionFilters;
using System.Web.Mvc;

namespace casa_benjamin
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthenticateActionFilter());
            filters.Add(new AlertActionFilter());

        }
    }
}
