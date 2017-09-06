using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Services.Events;
using Nop.Core.Domain.BrandDomain; 
namespace Nop.Services.Catalog
{
    /// <summary>
    /// Brand Service
    /// </summary>
    public partial class BrandService : IBrandService
    {
        private const string BRAND_ALL_KEY = "Nop.productbrand.all";
        private const string BRAND_BY_ID_KEY = "Nop.productbrand.id-{0}";
        private const string BRAND_PATTERN_KEY = "Nop.productbrand.";

         #region Fields

      
        private readonly IRepository<Brand> _brandRepository;

        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;


        #endregion

        #region ctor

        public BrandService(ICacheManager cacheManager,
            IRepository<Brand> brandRepository,  
            IEventPublisher eventPublisher
           )
        {
            _cacheManager = cacheManager;
            _brandRepository = brandRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region product category group
        public IList<Brand> GetAllBrands()
        {

            string key = BRAND_ALL_KEY;
            return _cacheManager.Get(key, () =>
            {
                var query = from pa in _brandRepository.Table
                            orderby pa.Name
                            select pa;
                var productAttributes = query.ToList();
                return productAttributes;
            });

        }
        public void DeleteBrand(Brand Brand)
        {
            if (Brand == null)
                throw new ArgumentNullException("BrandService_Delete");

            _brandRepository.Delete(Brand);

            //cache
            _cacheManager.RemoveByPattern(BRAND_PATTERN_KEY);
          

            //event notification
            _eventPublisher.EntityDeleted(Brand);
        }

        public Brand GetBrandById(int BrandId)
        {
            if (BrandId == 0)
                return null;

            string key = string.Format(BRAND_BY_ID_KEY, BrandId);
            return _cacheManager.Get(key, () =>
            {
                var pa = _brandRepository.GetById(BrandId);
                return pa;
            });
        }

        public void InsertBrand(Brand Brand)
        {
            if (Brand == null)
                throw new ArgumentNullException("BrandService_Insert");

            _brandRepository.Insert(Brand);

            _cacheManager.RemoveByPattern(BRAND_PATTERN_KEY);
           

            //event notification
            _eventPublisher.EntityInserted(Brand);
        }

        public void UpdateBrand(Brand Brand)
        {
            if (Brand == null)
                throw new ArgumentNullException("BrandService_Update");

            _brandRepository.Update(Brand);

            _cacheManager.RemoveByPattern(BRAND_PATTERN_KEY);
           

            //event notification
            _eventPublisher.EntityUpdated(Brand);
        }

        public Brand GetBrandByName(string name)
        {
            if (name == null || name == "")
                throw new ArgumentNullException("brand");
            return _brandRepository.Table.Where(x => x.Name == name).FirstOrDefault();
        }
        #endregion

    }
}
