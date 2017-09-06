using Nop.Core.Domain;
using Nop.Services.MiniSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Web.Framework.Mvc
{ 	
    public class LocalhostConstraint : IRouteConstraint 
    {
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly IMiniSiteService _miniSiteService; 

        public LocalhostConstraint()
        {
            this._storeInformationSettings = DependencyResolver.Current.GetService <StoreInformationSettings>();
            this._miniSiteService = DependencyResolver.Current.GetService<IMiniSiteService>();
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection) 
        {
            var uri = new Uri(_storeInformationSettings.StoreUrl);
            return httpContext.Request.Url.Host.Replace("www.", "") == uri.Host;
        } 
    }

    public class InnerHostConstraint : IRouteConstraint
    {
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly IMiniSiteService _miniSiteService;

        public InnerHostConstraint()
        {
            this._storeInformationSettings = DependencyResolver.Current.GetService <StoreInformationSettings>();
            this._miniSiteService = DependencyResolver.Current.GetService<IMiniSiteService>();
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var miniSite = _miniSiteService.GetMiniSiteByDomain(httpContext.Request.Url.Host.Replace("www.",""));
            if (miniSite == null)
                return false;

            return miniSite.Active;
        } 
    }

    public class RobotsTxtConstraint : IRouteConstraint
    {
        private readonly StoreInformationSettings _storeInformationSettings;

        public RobotsTxtConstraint()
        {
            this._storeInformationSettings = DependencyResolver.Current.GetService <StoreInformationSettings>();
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return httpContext.Request.Url.ToString().IndexOf(_storeInformationSettings.StoreUrl + "robots.txt") >= 0;
        } 
    }
}
