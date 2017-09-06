using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Catalog;


namespace Nop.Web.Models.Common
{
    public class OffersCategoryModel : BaseNopModel
    {
        public string CateogyTitle { get; set; }
        public int CategoryId { get; set; }
    }
}