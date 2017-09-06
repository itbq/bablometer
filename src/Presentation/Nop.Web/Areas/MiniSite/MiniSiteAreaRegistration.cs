using Nop.Web.Framework.Mvc;
using System.Web.Mvc;
using Nop.Web.Framework.Mvc.Routes;
using Nop.Web.Framework.Localization;

namespace Nop.Web.Areas.MiniSite
{
    public class MiniSiteAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MiniSite";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MiniSite_default",
                "MiniSite/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            context.MapLocalizedRoute(
                "MiniSiteHomePage",
                "",
                new { action = "Index", controller = "Home" },
                new { isLocal = new InnerHostConstraint() },
                new[] { "Nop.Web.Areas.MiniSite.Controllers" });

            context.MapLocalizedRoute(
                "MiniSiteNews",
                "news",
                new { action = "List", controller = "News" },
                new { isLocal = new InnerHostConstraint() },
                new[] { "Nop.Web.Areas.MiniSite.Controllers" });

            context.MapLocalizedRoute("CategorySelector",
                "Catalogue/GetSubCategories",
                 new { controller = "Catalogue", action = "GetSubCategories", area = "MiniSite" },
                new { isLocal = new InnerHostConstraint() },
                new[] { "Nop.Web.Areas.MiniSite.Controllers" });

            context.MapLocalizedRoute("MiniCategoryItem",
                "Catalogue/{itemtype}/{SeName}",
                new { controller = "Catalogue", action = "Category", SeName = "all", area = "MiniSite" },
                new { itemtype = @"\S+", SeName = @"\S+", isLocal = new InnerHostConstraint() },
                new[] { "Nop.Web.Areas.MiniSite.Controllers" });
            
            context.MapLocalizedRoute(
               "MiniContacts",
               "Contacts",
               new { action = "Contacts", controller = "Common" },
               new { isLocal = new InnerHostConstraint() },
               new[] { "Nop.Web.Areas.MiniSite.Controllers" });

            context.MapLocalizedRoute("MiniTextPage",
                "t/{SeName}",
                new { controller = "Common", action = "TextPage", area = "MiniSite" },
                new {isLocal = new InnerHostConstraint() },
                new[] { "Nop.Web.Areas.MiniSite.Controllers" });
            context.Routes.IgnoreRoute("news/addnews", new { isLocal = new InnerHostConstraint() });
            context.Routes.IgnoreRoute("news/managenews", new { isLocal = new InnerHostConstraint() });
        }
    }
}
