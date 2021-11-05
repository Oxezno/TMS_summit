using System.Web.Mvc;
using System.Web.Routing;

namespace TMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Login", id = UrlParameter.Optional }
            );

            // Catch All InValid (NotFound) Routes - 404
            routes.MapRoute(
                "AccessDenied",
                "{*url}",
                new { controller = "Home", action = "AccessDenied" }
            );

            
        }
    }
}
