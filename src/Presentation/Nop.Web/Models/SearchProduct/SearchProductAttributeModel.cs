using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.SearchProduct
{
    public class SearchProductAttributeModel
    {
        public string AttributeTitle { get; set; }
        public string PictureThumbnailUrl { get; set; }
        public string Description { get; set; }
        public int AttributeControlTypeId { get; set; }
        public int Id { get; set; }
        public int SelectedIntValue { get; set; }
        public int SelectedMaxIntValue { get; set; }
        public string AttributeValue { get; set; }
        public string AttributeValueMax { get; set; }
        public int SelectedAttributeId { get; set; }
        public string ValidationMessage { get; set; }
        public int CurrencyId { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public IList<SearchProductAttributeValueModel> Values { get; set; }
        public IList<SearchProductAttributeValueModel> PopularValues { get; set; }
    }
}