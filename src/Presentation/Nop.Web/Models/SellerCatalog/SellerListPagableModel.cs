using Nop.Web.Framework.UI.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.SellerCatalog
{
    public class SellerListPagableModel : BasePageableModel, IPageableModel
    {
        public int CategoryId { get; set; }
    }
}