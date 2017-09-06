using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Validators.MiniSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.Models.MiniSite
{
    [Validator(typeof(MiniSiteActivationModelValidator))]
    public class MiniSiteActivationModel
    {
        [NopResourceDisplayName("Doamin name")]
        [AllowHtml]
        public string DomainName { get; set; }

        [NopResourceDisplayName("Doamin name")]
        [AllowHtml]
        public string OwnDomain { get; set; }

        [NopResourceDisplayName("Layout type")]
        [AllowHtml]
        public int LayoutTemplateId { get; set; }

        [NopResourceDisplayName("Css template name")]
        [AllowHtml]
        public string CssTemplateName { get; set; }

        [NopResourceDisplayName("Use second lavel domain")]
        public bool UseSecondLavelDomain { get; set; }

        public IList<string> AviableCssTemplate { get; set; }

        public bool Success { get; set; }
    }
}