using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.MiniSite
{
    public class MiniSiteShortModel:BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.MiniSite.Username")]
        public string UserName { get; set; }

        [NopResourceDisplayName("Admin.MiniSite.DomainName")]
        public string DomainName { get; set; }

        [NopResourceDisplayName("Admin.MiniSite.Active")]
        public bool Active { get; set; }
    }
}