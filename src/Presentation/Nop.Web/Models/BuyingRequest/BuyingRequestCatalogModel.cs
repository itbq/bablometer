using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.BuyingRequest
{
    public partial class BuyingRequestCatalogModel:BaseNopEntityModel
    {
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string CompanyTitle { get; set; }
        public string CompanySeName { get; set; }
        public string ProductSeName { get; set; }
        public int ProductId { get; set; }
        public string Brand { get; set; }
        public int BrandId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public PictureModel Picture { get; set; }
    }
}