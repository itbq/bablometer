using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Banner;
using System.Collections.Generic;

namespace Nop.Web.Models.Topics
{
    public partial class TopicModel : BaseNopEntityModel
    {
        public string SystemName { get; set; }

        public bool IncludeInSitemap { get; set; }

        public bool IsPasswordProtected { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public string MetaTitle { get; set; }

        public IList<BannerModel> Banners { get; set; }
    }
}