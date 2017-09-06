using FluentValidation.Attributes;
using Nop.Admin.Validators.Footer;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Admin.Models.Footer
{
    [Validator(typeof(FooterValidator))]
    public partial class FooterContentModel: BaseNopModel
    {
        [NopResourceDisplayName("Admin.ContentManagement.Footer.Copyright")]
        [AllowHtml]
        public string CopyrightText { get; set; }
        
        [NopResourceDisplayName("Admin.ContentManagement.Footer.CopyrightLink")]
        [AllowHtml]
        public string CopyRightLink { get; set; }
        
        [NopResourceDisplayName("Admin.ContentManagement.Footer.Contact")]
        [AllowHtml]
        public string ContactText { get; set; }
    }
}