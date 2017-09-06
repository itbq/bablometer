using Nop.Web.Models.Common;
using Nop.Web.Models.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.SearchProduct
{
    public class SearchProductCategoryModel
    {
        public string CateogyTitle { get; set; }
        public int CategoryId { get; set; }
        public IList<SearchProductAttributeModel> Attributes { get; set; }
        public IList<CurrencyModel> Currencies { get; set; }
        public IList<CityModel> Cities { get; set; }
        public int CityId { get; set; }
        public string SeoName { get; set; }
    }
}