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
    [Validator(typeof(CityModelValidator))]
    public class CityModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("ITB.Admin.Region.Title")]
        public string Title { get; set; }
        public int RegionId { get; set; }
    }
}