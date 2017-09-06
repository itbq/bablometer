using Nop.Web.Models.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.SearchProduct
{
    public class SearchProductCategoriesModel
    {
        public IList<SearchProductCategoryModel> Categories { get; set; }
        public SearchProductCategoryModel CustomerAttributes { get; set; }
    }
}