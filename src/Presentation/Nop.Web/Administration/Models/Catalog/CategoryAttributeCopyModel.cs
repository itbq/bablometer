using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public class CategoryAttributeCopyModel
    {
        public int CategoryProductAttributeId { get; set; }

        [NopResourceDisplayName("Admin.CategoryProductAttributeValues.Copy.AttributeGroup")]
        public int CategoryProductAttributeGroupId { get; set; }
        public IList<SelectListItem> AviableGroups { get; set; }
    }
}