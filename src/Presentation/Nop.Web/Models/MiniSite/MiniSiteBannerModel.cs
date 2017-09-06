using Nop.Web.Framework.Mvc;
using Nop.Web.Models.BuyingRequest;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FluentValidation.Attributes;
using Nop.Web.Validators.MiniSite;
using Nop.Web.Framework;
using System.Web.Mvc;

namespace Nop.Web.Models.MiniSite
{
    [Validator(typeof(MiniSiteBannerModelValidator))]
    public class MiniSiteBannerModel:BaseNopEntityModel
    {
        [UIHint("Picture")]
        public int PictureId { get; set; }

        public PictureModel Picture { get; set; }

        public List<BuyingRequestLanguageModel> Languages { get; set; }

        [NopResourceDisplayName("MiniSite.Banner.TitleText")]
        [AllowHtml]
        public string TitleTag { get; set; }

        [NopResourceDisplayName("MiniSite.Banner.AltText")]
        [AllowHtml]
        public string AltTag { get; set; }

        [NopResourceDisplayName("ETF.MiniSite.Banner.Url")]
        [AllowHtml]
        public string Url { get; set; }

        public int WorkingLanguage { get; set; }
    }
}