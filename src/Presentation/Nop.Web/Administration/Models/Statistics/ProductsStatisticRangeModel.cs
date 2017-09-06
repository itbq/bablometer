using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.Statistics
{
    public class ProductsStatisticRangeModel
    {
        public ProductsStatisticRangeModel(ILocalizationService localizationService)
        {
            this.Product = new ProductsStatisticModel();
            this.Product.Name = localizationService.GetResource("Admin.Statistics.Product");
            this.Service = new ProductsStatisticModel();
            this.Service.Name = localizationService.GetResource("Admin.Statistics.Service");
            this.ProductBuyingRequest = new ProductsStatisticModel();
            this.ProductBuyingRequest.Name = localizationService.GetResource("Admin.Statistics.ProductRequest");
            this.ServiceBuyingRequest = new ProductsStatisticModel();
            this.ServiceBuyingRequest.Name = localizationService.GetResource("Admin.Statistics.ServiceRequest");
        }

        public ProductsStatisticModel Product { get; set; }
        public ProductsStatisticModel Service { get; set; }
        public ProductsStatisticModel ProductBuyingRequest { get; set; }
        public ProductsStatisticModel ServiceBuyingRequest { get; set; }
    }
}