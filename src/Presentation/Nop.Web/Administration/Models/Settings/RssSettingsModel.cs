using FluentValidation.Attributes;
using Nop.Admin.Validators.Settings;
using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.Settings
{
    [Validator(typeof(RssSettingsValidator))]
    public partial class RssSettingsModel
    {
        [NopResourceDisplayName("Rss.Name.Product")]
        public int ProductFeedCount { get; set; }

        [NopResourceDisplayName("Rss.Name.Seller")]
        public int SellerFeedCount { get; set; }

        [NopResourceDisplayName("Rss.Name.News")]
        public int NewsFeedCount { get; set; }

        [NopResourceDisplayName("Rss.Name.Events")]
        public int EventFeedCount { get; set; }
    }
}