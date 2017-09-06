using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Admin.Models.Catalog;

namespace Nop.Web.Models.Common
{
    public partial class OffersModel : BaseNopModel
    {
        public OffersModel()
        {
            Products = new List<OffersProductModel>();
            Categories = new List<OffersCategoryModel>();

        }
        public IList<OffersProductModel> Products { get; set; }
        public IList<OffersCategoryModel> Categories { get; set; }

    }
}