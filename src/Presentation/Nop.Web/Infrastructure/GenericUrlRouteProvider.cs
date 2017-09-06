using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routes;
using Nop.Web.Framework.Seo;
using Nop.Core.Infrastructure;
using Nop.Services.Seo;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Infrastructure
{
    public partial class GenericUrlRouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //generic URLs
            routes.MapGenericPathRoute("GenericUrl",
                                       "{generic_se_name}",
                                       new {controller = "Common", action = "GenericUrl"},
                                       new[] {"Nop.Web.Controllers"});

            //define this routes to use in UI views (in case if you want to customize some of them later)
            routes.MapLocalizedRoute("Category",
                            "{SeName}",
                            new { controller = "Catalogue", action = "Category"},
                            new { isLocal = new LocalhostConstraint() },
                            new[] { "Nop.Web.Controllers" });

            routes.MapLocalizedRoute("MiniCategory",
                            "{SeName}",
                            new { controller = "Catalogue", action = "Category", area="MiniSite" },
                            new { isLocal = new InnerHostConstraint() },
                            new[] { "Nop.Web.Areas.MiniSite.Controllers" });


            routes.MapRoute("AddRequest",
                    "Catalogue/AddRequest",
                    new { controller = "Catalogue", action = "AddRequest" },
                    new { isLocal = new LocalhostConstraint() },
                    new[] { "Nop.Web.Controllers" });

            routes.MapLocalizedRoute("CategorySellers",
                    "Sellers/{itemtype}/{SeName}",
                    new { controller = "Catalogue", action = "SellerCatalogue", SeName = "all" },
                    new { itemtype = @"\S+", SeName = @"\S+", isLocal = new LocalhostConstraint() },
                    new[] { "Nop.Web.Controllers" });

            routes.MapLocalizedRoute("SellerNews",
                "News/{SeName}",
                new { controller = "News", action = "SellerNews" },
                new { isLocal = new LocalhostConstraint() },
                new[] { "Nop.Web.Controllers" });

            routes.MapLocalizedRoute("Product",
                                    "{SeName}",
                                    new { controller = "Catalogue", action = "Details" },
                                    new { isLocal = new LocalhostConstraint() },
                                    new[] { "Nop.Web.Controllers" });
            routes.MapLocalizedRoute("MiniProduct",
                                    "{SeName}",
                                    new { controller = "Catalogue", action = "Details", area = "MiniSite" },
                                    new { isLocal = new InnerHostConstraint() },
                                    new[] { "Nop.Web.Areas.MiniSite.Controllers" });
            //routes.MapLocalizedRoute("Manufacturer",
            //                "{SeName}",
            //                new { controller = "Catalog", action = "Manufacturer" },
            //                new[] { "Nop.Web.Controllers" });

            routes.MapLocalizedRoute("NewsItem",
                            "{SeName}",
                            new { controller = "News", action = "NewsItem" },
                            new { isLocal = new LocalhostConstraint() },
                            new[] { "Nop.Web.Controllers" });
           
            routes.MapLocalizedRoute("MiniNewsItem",
                            "{SeName}",
                            new { controller = "News", action = "NewsItem", area = "MiniSite" },
                            new { isLocal = new InnerHostConstraint() },
                            new[] { "Nop.Web.Areas.MiniSite.Controllers" });

            routes.MapLocalizedRoute("BlogPost",
                            "{SeName}",
                            new { controller = "Blog", action = "BlogPost" },
                            new[] { "Nop.Web.Controllers" });
            
            routes.MapLocalizedRoute("Event",
                            "{SeName}",
                            new {controller = "Event", action="EventInfo", pagenum = "{pagenum}"},
                            new { isLocal = new LocalhostConstraint() },
                            new[] { "Nop.Web.Controllers" });
            
            routes.MapLocalizedRoute("CompanyInformation",
                            "{SeName}",
                            new { controller = "Company", action = "CompanyInfo" },
                            new { isLocal = new LocalhostConstraint() },
                            new[] { "Nop.Web.Controllers" });
        }

        public int Priority
        {
            get
            {
                //it should be the last route
                return -int.MaxValue;
            }
        }
    }
}
