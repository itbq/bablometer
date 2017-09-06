using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
    public partial class ProductPriceService : IProductPriceService
    {
        #region Fields

        private readonly IRepository<ProductPrice> _productPriceRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion


        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="productTagRepository">Product price repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ProductPriceService(IRepository<ProductPrice> productPriceRepository,
            IEventPublisher eventPublisher)
        {
            _productPriceRepository = productPriceRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public virtual void DeleteProductPrice(ProductPrice productPrice)
        {
            if (productPrice == null)
                throw new ArgumentNullException("productPrice");

            _productPriceRepository.Delete(productPrice);

            //event notification
            _eventPublisher.EntityDeleted(productPrice);
        }
        /// <summary>
        /// Delete a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public virtual void DeleteProductPriceById(int productPriceId)
        {
            if (productPriceId > 0)
            {
                var productPrice = _productPriceRepository.GetById(productPriceId);
                if (productPrice != null)
                {
                    _productPriceRepository.Delete(productPrice);
                    //event notification
                    _eventPublisher.EntityDeleted(productPrice);
                }
            }
        }
        /// <summary>
        /// Gets all product prices
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product tags</returns>
        public virtual IList<ProductPrice> GetAllProductPrices(int ProductId)
        {
            var query = _productPriceRepository.Table;
            query = query.Where(pt => pt.ProductId == ProductId).OrderByDescending(pt => pt.Currency.DisplayOrder);
            return query.ToList();
        }

        /// <summary>
        /// Gets product tag
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>Product tag</returns>
        public virtual ProductPrice GetProductPriceById(int productPriceId)
        {
            if (productPriceId == 0)
                return null;

            var productPrice = _productPriceRepository.GetById(productPriceId);
            return productPrice;
        }

        /// <summary>
        /// Gets product tag
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>Product tag</returns>
        public virtual ProductPrice GetProductPriceByIdAndCurrencyId(int productId, int currencyId)
        {
            if (productId == 0 || currencyId == 0)
                return null;
            var query = _productPriceRepository.Table;
            query = query.Where(pt => pt.ProductId == productId && pt.CurrencyId == currencyId).OrderByDescending(pt => pt.Currency.DisplayOrder);
            var productPrice = query.FirstOrDefault();
            return productPrice;
        }

        /// <summary>
        /// Inserts a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public virtual void InsertProductPrice(ProductPrice productPrice)
        {
            if (productPrice == null)
                throw new ArgumentNullException("ProductPrice");

            _productPriceRepository.Insert(productPrice);

            //event notification
            _eventPublisher.EntityInserted(productPrice);
        }

        /// <summary>
        /// Updates the product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public virtual void UpdateProductPrice(ProductPrice productPrice)
        {
            if (productPrice == null)
                throw new ArgumentNullException("productPrice");

            _productPriceRepository.Update(productPrice);

            //event notification
            _eventPublisher.EntityUpdated(productPrice);
        }



        #endregion
    }
}
