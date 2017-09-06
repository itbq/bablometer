using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Web.Models.Common;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Models.SearchProduct
{
    public class SearchModelFull
    {
        public SearchModelFull()
        {
            Products = new List<OffersProductModel>();
            BestProducts = new List<OffersProductModel>();
            Categories = new List<SearchCategoryModel>();
            ProductTags = new List<ProductTagModel>();
        }
        public SearchProductCategoryModel DetailedSelectedCategoryAttributes { get; set; }
        public SearchProductCategoryModel SelectedAdditionalCategoryAttributes { get; set; }
        public SearchProductCategoryModel SelectedCategoryAttributes { get; set; }
        public SearchProductCategoryModel CustomerAttributes { get; set; }
        public IList<SearchCategoryModel> Categories { get; set; }
        public IList<OffersProductModel> Products { get; set; }
        public IList<OffersProductModel> BestProducts { get; set; }
        public IList<ProductTagModel> ProductTags { get; set; }
        public bool LowerButtonClick { get; set; }
    }
}