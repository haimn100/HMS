using System.Web.Mvc;
using System.Web.Routing;

namespace casa_benjamin
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
              name: "Admin",
              url: "{controller}/{action}/{id}",
              defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
          );

            routes.MapRoute(
             name: "Reservation",
             url: "{controller}/{action}/{id}",
             defaults: new { controller = "Reservation", action = "Index", id = UrlParameter.Optional }
         );

            routes.MapRoute(
             name: "Room",
             url: "{controller}/{action}/{id}",
             defaults: new { controller = "Room", action = "Index", id = UrlParameter.Optional }
         );

            routes.MapRoute(
             name: "Guest",
             url: "{controller}/{action}/{id}",
             defaults: new { controller = "Guest", action = "Index", id = UrlParameter.Optional }
         );

            routes.MapRoute(
           name: "Kitchen",
           url: "{controller}/{action}/{id}",
           defaults: new { controller = "Kitchen", action = "Index", id = UrlParameter.Optional }
       );

            routes.MapRoute(
          name: "HouseKeeping",
          url: "{controller}/{action}/{id}",
          defaults: new { controller = "HouseKeeping", action = "Index", id = UrlParameter.Optional }
      );

            routes.MapMvcAttributeRoutes();
        }
    }
}
