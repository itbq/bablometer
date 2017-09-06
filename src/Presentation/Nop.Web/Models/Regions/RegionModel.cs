using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Regions
{
    public class RegionModel : BaseNopEntityModel
    {
        public string Title { get; set; }
    }

    public class CityModel : BaseNopEntityModel
    {
        public string Title { get; set; }
    }
}