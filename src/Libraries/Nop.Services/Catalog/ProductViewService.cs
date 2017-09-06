using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Localization;

namespace Nop.Services.Catalog
{
    public partial class ProductViewService:IProductViewService
    {
        private readonly IRepository<ProductView> _repository;
        private readonly IProductService _productService;
        private readonly CatalogSettings _catalogSettings;

        public ProductViewService(IRepository<ProductView> repository,
            CatalogSettings catalogSettings,
            IProductService productService)
        {
            this._repository = repository;
            this._catalogSettings = catalogSettings;
            this._productService = productService;
        }

        /// <summary>
        /// Check if specified product has specified language locale
        /// </summary>
        /// <param name="productId">product id</param>
        /// <param name="languageId">language id</param>
        /// <returns></returns>
        protected bool CheckProduct(int productId, int languageId)
        {
            var product = _productService.GetProductById(productId);
            if (product.GetLocalized(x => x.Name, languageId, false) == null )
            {
                return false;
            }

            if (product.Deleted)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get recently viewed by customer product
        /// </summary>
        /// <param name="customerId">customer id</param>
        /// <returns></returns>
        public IList<ProductView> GetCustomerProductViews(int customerId, int languageId)
        {
            var productViews = _repository.Table.Where(x => x.CustomerId == customerId).OrderByDescending(x=>x.LastViewOnUtc).ToList();
            return productViews.Where(x=>CheckProduct(x.ProductId,languageId)).Take(_catalogSettings.RecentlyViewedProductsNumber).ToList();
        }

        /// <summary>
        /// Get most populer cuctomer products
        /// </summary>
        /// <param name="customerId">customer id</param>
        /// <returns>list of product ids</returns>
        public IList<int> GetPopularCustomerProducts(int customerId, int languageId)
        {
            var productViews = _repository.Table.Where(x => x.Product.CustomerId == customerId && !x.Product.Deleted)
                .OrderByDescending(x=>x.LastViewOnUtc)
                .ToList();

            productViews = productViews.Where(x=>CheckProduct(x.ProductId,languageId)).ToList();
            var groupedViews = productViews.GroupBy(x => x.ProductId);

            return groupedViews.OrderBy(x => x.Count())
                .Select(x => x.Key)
                .Take(_catalogSettings.RecentlyViewedProductsNumber
                ).ToList();
        }

        /// <summary>
        /// Add product to recently viewed by customer product
        /// </summary>
        /// <param name="productId">product id</param>
        /// <param name="customerId">customer id</param>
        public void AddProductToProductViews(int productId, int customerId)
        {
            if(productId == 0 || customerId == 0)
                throw new ArgumentNullException("product view");
            var productViews= _repository.Table.Where(x=>x.CustomerId == customerId).OrderBy(x=>x.LastViewOnUtc);
            var productView = productViews.Where(x=>x.ProductId == productId).FirstOrDefault();
            if (productView == null)
            {
                if (productViews.Count() > _catalogSettings.ProductViewStoreCount)
                    _repository.Delete(productViews.ToList().Last());

                _repository.Insert(new ProductView() { CustomerId = customerId, ProductId = productId,LastViewOnUtc = DateTime.UtcNow });
            }
            else
            {
                productView.LastViewOnUtc = DateTime.UtcNow;
                _repository.Update(productView);
            }
        }

    }
}
