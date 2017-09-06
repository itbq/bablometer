using Nop.Admin.Validators.GoogleAnalitics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using System.Web.Mvc;

namespace Nop.Admin.Models.GoogleAnalitics
{
    [Validator(typeof(GoogleAnaliticsSetupModelValidator))]
    public class AnaliticsSetupModel
    {
        [NopResourceDisplayName("ETF.Admin.Analitics.ClientId")]
        [AllowHtml]
        public string ClientId { get; set; }

        [NopResourceDisplayName("ETF.Admin.Analitics.AccountId")]
        [AllowHtml]
        public string AccountId { get; set; }

        [NopResourceDisplayName("ETF.Admin.Analitics.PrivateKey")]
        [UIHint("Download")]
        public int PrivateKeyDownnloadId { get; set; }
    }
}