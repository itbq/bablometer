using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.BuyingRequest
{
    public class CategoryAttributeModel : BaseNopEntityModel
    {
        public bool Error { get; set; }
        public List<CategoryProductAttributeValueModel> Values { get; set; }
        public string Name { get; set; }
        public AttributeControlType ControlType { get; set; }
        public CategoryProductAttributeValueModel SelectedValue { get; set; }
        public int DisplayOrder { get; set; }
        public string CurrencyCode { get; set; }
    }
}