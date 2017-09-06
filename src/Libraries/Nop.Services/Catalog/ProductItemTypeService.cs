using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
    public partial class ProductItemTypeService : IProductItemTypeService
    {
        #region Constants
        private const string PRODUCTITEMTYPES_BY_ID_KEY = "Nop.productitemtype.id-{0}";
        private const string PRODUCTITEMTYPES_ALL_KEY = "Nop.productitemtype.all";
        private const string PRODUCTITEMTYPES_PATTERN_KEY = "Nop.productitemtype.";

        #endregion

        #region Fields

        private readonly IRepository<ProductItemType> _productItemTypeRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion
          #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="productTemplateRepository">Product item type repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ProductItemTypeService(ICacheManager cacheManager,
            IRepository<ProductItemType> productTemplateRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _productItemTypeRepository = productTemplateRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion




        public virtual void DeleteProductItemType(ProductItemType productItemType)
        {
            if (productItemType == null)
                throw new ArgumentNullException("productItemType");

            _productItemTypeRepository.Delete(productItemType);

            _cacheManager.RemoveByPattern(PRODUCTITEMTYPES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productItemType);
        }

        public virtual IList<ProductItemType> GetAllProductItemTypes()
        {
            string key = PRODUCTITEMTYPES_ALL_KEY;
            return _cacheManager.Get(key, () =>
            {
                var query = from pt in _productItemTypeRepository.Table
                            orderby pt.DisplayOrder
                            select pt;

                var itemtypes = query.ToList();
                return itemtypes;
            });
        }

        public virtual ProductItemType GetProductItemTypeById(int productItemTypeId)
        {
            if (productItemTypeId == 0)
                return null;

            string key = string.Format(PRODUCTITEMTYPES_BY_ID_KEY, productItemTypeId);
            return _cacheManager.Get(key, () =>
            {
                var template = _productItemTypeRepository.GetById(productItemTypeId);
                return template;
            });
        }

        public virtual void InsertProductItemType(ProductItemType productItemType)
        {
            if (productItemType == null)
                throw new ArgumentNullException("productItemType");

            _productItemTypeRepository.Insert(productItemType);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTITEMTYPES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productItemType);
        }

        public virtual void UpdateProductItemType(ProductItemType productItemType)
        {
            if (productItemType == null)
                throw new ArgumentNullException("productItemType");

            _productItemTypeRepository.Update(productItemType);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTITEMTYPES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productItemType);
        }
    }
}
