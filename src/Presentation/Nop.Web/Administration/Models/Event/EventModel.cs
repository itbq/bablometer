using Nop.Admin.Validators.Event;
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

namespace Nop.Admin.Models.Event
{
    [Validator(typeof(EventValidator))]
    public partial class EventModel : BaseNopEntityModel, ILocalizedModel<EventLocalizedModel>
    {
        [NopResourceDisplayName("event.title")]
        [AllowHtml]
        public string Title { get; set; }

        [NopResourceDisplayName("event.shortdescription")]
        [AllowHtml]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("event.fulldescription")]
        [AllowHtml]
        public string FullDescription { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("ETF.Profile.News.Image.HomePage")]
        [AllowHtml]
        public int? PictureId { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("ETF.Profile.News.Image.CatalogPage")]
        [AllowHtml]
        public int? CatalogPictureId { get; set; }

        [NopResourceDisplayName("event.startdate")]
        public DateTime StartDate { get; set; }

        [NopResourceDisplayName("event.enddate")]
        public DateTime? EndDate { get; set; }

        [NopResourceDisplayName("event.seoname")]
        [AllowHtml]
        public string SeName { get; set; }

        [NopResourceDisplayName("event.dayslast")]
        [AllowHtml]
        public bool EndDateEnabled { get; set; }

        public IList<EventLocalizedModel> Locales { get; set; }
    }

    public partial class EventLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("event.title")]
        [AllowHtml]
        public string Title { get; set; }

        [NopResourceDisplayName("event.shortdescription")]
        [AllowHtml]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("event.fulldescription")]
        [AllowHtml]
        public string FullDescription { get; set; }

        [NopResourceDisplayName("event.seoname")]
        [AllowHtml]
        public string SeName { get; set; }
    }
}