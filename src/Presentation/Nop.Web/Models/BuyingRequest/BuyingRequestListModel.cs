using Nop.Core;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.BuyingRequest
{
    public class BuyingRequestListModel:BaseNopModel
    {
        public IPagedList<BuyingRequestOverviewModel> BuyingRequestList { get; set; }
        public BuyingRequestPagableModel PagingContext { get; set; }
    }

    public class BuyingRequestCatalogListModel : BaseNopModel
    {
        public int SelectedCategoryId { get; set; }
        public int ItemType { get; set; }
        public IPagedList<BuyingRequestCatalogModel> BuyingRequestList { get; set; }
        public BuyingRequestPagableModel PagingContext { get; set; }
        public CategoryNavigationModel[] Categories { get; set; }
        public string SelectedCategorySeName { get; set; }
    }
}