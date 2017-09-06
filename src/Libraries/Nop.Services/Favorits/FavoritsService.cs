using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Favorit;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Favorits
{
    public partial class FavoritsService:IFavoritsService
    {
        private const string FAVORIT_BY_ID_KEY = "Nop.favorits.id-{0}";
        private const string FAVORIT_CUSTOMER_KEY = "Nop.favorits.customer-{0}";
        private const string FAVORIT_PATTERN_KEY = "Nop.favorits.";

        private IRepository<FavoritItem> _favoritRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        public FavoritsService(IRepository<FavoritItem> favoritRepository,
            IEventPublisher eventPublisher,
            ICacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
            this._favoritRepository = favoritRepository;
            this._eventPublisher = eventPublisher;
        }

        public FavoritItem GetFavoritById(int id)
        {
            if (id == 0)
                return null;
            string key = string.Format(FAVORIT_BY_ID_KEY, id);
            return _cacheManager.Get(key, () =>
            {
                var n = _favoritRepository.GetById(id);
                return n;
            });
        }

        public void Insert(FavoritItem entity)
        {
            if (entity == null)
                throw new ArgumentNullException("favorit");
            _favoritRepository.Insert(entity);

            _cacheManager.RemoveByPattern(FAVORIT_PATTERN_KEY);
            //event notification
            _eventPublisher.EntityInserted(entity);
        }

        public void DeleteFavorit(FavoritItem item)
        {
            if (item == null)
                throw new ArgumentNullException("favorit");

            this._favoritRepository.Delete(item);

            _cacheManager.RemoveByPattern(FAVORIT_PATTERN_KEY);

            _eventPublisher.EntityDeleted(item);
        }

        public IList<FavoritItem> GetCustomerFavorits(int customerId)
        {
            if (customerId == 0)
                return null;
            string key = string.Format(FAVORIT_CUSTOMER_KEY, customerId);
            return _cacheManager.Get(key, () =>
            {
                var n = _favoritRepository.Table.Where(x => x.CustomerId == customerId && !x.Product.Deleted).ToList();
                return n;
            });
        }

        public bool IsItemFavorit(int customerId, int productId)
        {
            var favorit = _favoritRepository.Table.Where(x => x.ProductId == productId && x.CustomerId == customerId).FirstOrDefault();
            return favorit != null;
        }
    }
}
