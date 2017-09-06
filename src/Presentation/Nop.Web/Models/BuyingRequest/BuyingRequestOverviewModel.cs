using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.BuyingRequest
{
    public partial class BuyingRequestOverviewModel
    {
        public string ProductTitle { get; set; }
        public string ProductSeName { get; set; }
        public int ProductId { get; set; }
        public string ProductDescription { get; set; }
        public string BrandName { get; set; }
        public string ProductItemType { get; set; }
        public string CategoryString { get; set; }
        public PictureModel Picture { get; set; }
        public List<BuyingRequestLanguageModel> Languages { get; set; }
    }
}