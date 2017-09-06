using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Models.Brand
{
    public partial class BrandListModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Brand.List.SearchCategoryName")]
        [AllowHtml]
        public string SearchBrandName { get; set; }

        public GridModel<BrandModel> Brands { get; set; }
    }
}