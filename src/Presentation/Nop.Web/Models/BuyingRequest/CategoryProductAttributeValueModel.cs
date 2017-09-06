using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.BuyingRequest
{
    public class CategoryProductAttributeValueModel : BaseNopEntityModel
    {
        public string Name { get; set; }
        public string NameMax { get; set; }
        public bool IsPreSelected { get; set; }
        public int DisplayOrder { get; set; }
        public string ColorSquaresRgb { get; set; }
        public int CategoryProductAttributeId { get; set; }
    }
}