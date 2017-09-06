using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.SellerCatalog
{
    public class CategorySelectorModel
    {
        public string CatalogUrl { get; set; }
        public CategoryNavigationModel[] Categories { get; set; }
        public int SelectedCategoryId { get; set; }
        public string SelectedCategorySeName { get; set; }
        public int ItemType { get; set; }
        public bool HaveConversionImages { get; set; }
    }
}