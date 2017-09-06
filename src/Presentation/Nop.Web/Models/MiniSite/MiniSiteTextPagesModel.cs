using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Common;
using Nop.Web.Validators.MiniSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.Models.MiniSite
{
    [Validator(typeof(MiniSiteTextPagesModelValidator))]
    public class MiniSiteTextPagesModel:BaseNopEntityModel
    {
        /// <summary>
        /// Html content of about us page
        /// </summary>
        [NopResourceDisplayName("ETF.MiniSite.TextPage.Html")]
        [AllowHtml]
        public string PageHtml { get; set; }

        /// <summary>
        /// Title of about us page
        /// </summary>
        [NopResourceDisplayName("ETF.MiniSite.TextPage.Header")]
        [AllowHtml]
        public string PageTitle { get; set; }

        /// <summary>
        /// Title tag of about us page
        /// </summary>
        [NopResourceDisplayName("ETF.MiniSite.TextPage.PageTitle")]
        [AllowHtml]
        public string PageTitleTag { get; set; }

        [NopResourceDisplayName("ETF.MiniSite.TextPage.ManuTitle")]
        [AllowHtml]
        public string PageMenuTitle { get; set; }

        public IList<LanguageModel> AviableLanguages { get; set; }

        public LanguageModel CurrentLanguage { get; set; }

        public int SelectedLanguageId { get; set; }
    }
}