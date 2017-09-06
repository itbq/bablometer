using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.ExternalAuth.Vkontakte
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.ExternalAuth.Vkontakte.Login",
                 "Plugins/ExternalAuthVkontakte/Login",
                 new { controller = "ExternalAuthVkontakte", action = "Login" },
                 new[] { "Nop.Plugin.ExternalAuth.Vkontakte.Controllers" }
            );

            routes.MapRoute("Plugin.ExternalAuth.Vkontakte.LoginCallback",
                 "Plugins/ExternalAuthVkontakte/LoginCallback",
                 new { controller = "ExternalAuthVkontakte", action = "LoginCallback" },
                 new[] { "Nop.Plugin.ExternalAuth.Vkontakte.Controllers" }
            );
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
