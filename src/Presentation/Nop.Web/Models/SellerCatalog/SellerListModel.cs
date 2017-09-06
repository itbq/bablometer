using Nop.Core;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.SellerCatalog
{
    public class SellerListModel:BaseNopModel
    {
        public IPagedList<ProfileModel> Sellers { get; set; }
        public SellerListPagableModel PagingContext { get; set; }
        public int SelectedCategoryId { get; set; }
        public int ItemType { get; set; }
        public string SelectedCategorySeName { get; set; }
        public IList<CategoryModel> CategoryBreadCrumb { get; set; }
        public CategoryNavigationModel[] Categories { get; set; }
    }
}