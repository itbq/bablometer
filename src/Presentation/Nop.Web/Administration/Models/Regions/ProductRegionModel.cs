using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.Regions
{
    public class ProductRegionModel : BaseNopEntityModel
    {
        public string Title { get; set; }
        public bool Selected { get; set; }
    }
}