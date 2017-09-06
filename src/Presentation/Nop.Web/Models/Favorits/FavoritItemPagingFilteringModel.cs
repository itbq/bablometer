using Nop.Web.Framework.UI.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Favorits
{
    public partial class FavoritItemPagingFilteringModel : BasePageableModel
    {
        public Dictionary<string, int> Brands { get; set; }
        public int ItemType { get; set; }
        public int BrandId { get; set; }
        public int SelectedCategoryId {get; set;}
    }
}