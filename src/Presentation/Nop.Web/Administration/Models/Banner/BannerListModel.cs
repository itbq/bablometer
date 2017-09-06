using Nop.Admin.Models.Banners;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Web.Mvc;

namespace Nop.Admin.Models.Banner
{
    public partial class BannerListModel: BaseNopModel
    {
        public IList<BannerModel> HomePageBanners { get; set; }
        public IList<BannerModel> InnerPageBanners { get; set; }
    }
}