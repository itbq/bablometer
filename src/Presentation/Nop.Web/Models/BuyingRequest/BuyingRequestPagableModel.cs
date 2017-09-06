using Nop.Web.Framework.UI.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.BuyingRequest
{
    public partial class BuyingRequestPagableModel : BasePageableModel, IPageableModel
    {
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public int ProductItemTypeId { get; set; }
        public int SelectedCategoryId { get; set; }
        public Dictionary<string, int> Brands { get; set; }
        public int CustomerId { get; set; }
        public int ProductTagId { get; set; }
    }
}