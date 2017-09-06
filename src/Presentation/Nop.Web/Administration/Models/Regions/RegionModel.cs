using FluentValidation.Attributes;
using Nop.Admin.Validators.Regions;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.Regions
{
    [Validator(typeof(RegionModelValidator))]
    public class RegionModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("ITB.Admin.Region.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("ITB.Admin.Region.Code")]
        public int Code { get; set; }
    }
}