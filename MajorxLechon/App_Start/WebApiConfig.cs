using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MajorxLechon
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { Key = RouteParameter.Optional, id = RouteParameter.Optional, Action = "Get" }

            );
        }
    }
}
