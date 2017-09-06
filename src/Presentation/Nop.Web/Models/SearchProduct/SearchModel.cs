using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.SearchProduct
{
    public class SearchModel
    {
        public SearchProductCategoryModel DetailedSelectedCategoryAttributes { get; set; }
        public SearchProductCategoryModel SelectedAdditionalCategoryAttributes { get; set; }
        public SearchProductCategoryModel SelectedCategoryAttributes { get; set; }
        public SearchProductCategoryModel CustomerAttributes { get; set; }
        public bool LowerButtonClick { get; set; }
    }
}