using Nop.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Banner
{
    public partial class BannerModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public BannerTypeEnum BannerType { get; set; }
        public string AltText { get; set; }
        public string NetBanner { get; set; }
        public string TitleText { get; set; }
        public string Url { get; set; }
        public bool DisplayOnHomePage { get; set; }
    }
}