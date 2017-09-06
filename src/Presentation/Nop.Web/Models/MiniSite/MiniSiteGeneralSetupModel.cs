using Nop.Web.Validators.MiniSite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using System.Web.Mvc;

namespace Nop.Web.Models.MiniSite
{
    [Validator(typeof(MiniSiteGeneralSetupModelValidator))]
    public class MiniSiteGeneralSetupModel
    {
        /// <summary>
        /// Mini site pages root title
        /// </summary>
        [AllowHtml]
        public string Title { get; set; }

        /// <summary>
        /// Mini site page root title for russian language
        /// </summary>
        [AllowHtml]
        public string TitleRus { get; set; }

        /// <summary>
        /// Mini site layout id
        /// </summary>
        public int LayoutId { get; set; }

        /// <summary>
        /// Css template name used for this mini site
        /// </summary>
        [AllowHtml]
        public string CssTemplateName { get; set; }

        /// <summary>
        /// Aviable css templates for mini site
        /// </summary>
        public List<string> AviableCssTemplates { get; set; }

        /// <summary>
        /// Mini Site domain name
        /// </summary>
        [AllowHtml]
        public string DomainName { get; set; }

        [NopResourceDisplayName("Doamin name")]
        [AllowHtml]
        public string OwnDomain { get; set; }

        [NopResourceDisplayName("Use second lavel domain")]
        public bool UseSecondLavelDomain { get; set; }

        /// <summary>
        /// Email where to send messages from contact form. Also displayed in footer
        /// </summary>
        [AllowHtml]
        public string ContactEmail { get; set; }

        [AllowHtml]
        public string GoogleAnalytics { get; set; }

        /// <summary>
        /// Id of logo picture
        /// </summary>
        [UIHint("Picture")]
        public int LogoId { get; set; }
    }
}