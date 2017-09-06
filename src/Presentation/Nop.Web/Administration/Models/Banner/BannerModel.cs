using Nop.Admin.Models.Media;
using Nop.Admin.Validators;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework.Localization;
using Nop.Core.Domain;

namespace Nop.Admin.Models.Banners
{
    [Validator(typeof(BannerValidator))]
    public partial class BannerModel: BaseNopEntityModel
    {
        [NopResourceDisplayName("banner.title")]
        [AllowHtml]
        public string Title { get; set; }

        [NopResourceDisplayName("banner.url")]
        [AllowHtml]
        public string Url { get; set; }

        [NopResourceDisplayName("banner.Netbanner")]
        [AllowHtml]
        public string NetBanner { get; set; }

        [NopResourceDisplayName("banner.size")]
        [AllowHtml]
        public int Size { get; set; }

        [NopResourceDisplayName("banner.alt")]
        [AllowHtml]
        public string Alt { get; set; }

        [UIHint("MyPicture")]
        [NopResourceDisplayName("banner.picture")]
        [AllowHtml]
        public int PictureId { get; set; }

        public int Height { get; set; }

        public PictureModel PictureModel { get; set; }

        public string BannerTypeString { get; set; }

        public bool DisplayOnMain { get; set; }

        [NopResourceDisplayName("ITBSFA.Admin.Banner.Category")]
        public int CategoryId { get; set; }

        public IList<SelectListItem> AviableCategories { get; set; }

        [NopResourceDisplayName("ITBSFA.Admin.Banner.Category")]
        public string CategoryName { get; set; }

        [NopResourceDisplayName("ITBSFA.Admin.Banner.Position")]
        public int BannerTypeId { get; set; }

        public BannerTypeEnum BannerType { get; set; }

        public List<SelectListItem> BannerTypeDropDown { get; set; }
    }
}