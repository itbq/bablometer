using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.BannerService
{
    public partial class BannerService : IBannerService
    {

        private const string BANNER_BY_ID_KEY = "Nop.banners.id-{0}";
        private const string BANNER_BY_TYPE_KEY = "Nop.banners.type-{0}-{1}";
        private const string BANNER_ALL_KEY = "Nop.banners.all";
        private const string BANNER_HOMEPAGE_KEY = "Nop.banners.homepage-{0}";
        private const string BANNER_PATTERN_KEY = "Nop.banners.";
        
        
        private readonly IRepository<Banner> _bannerRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;


        public BannerService(IRepository<Banner> bannerRepository,
            IEventPublisher eventPublisher,
            ICacheManager cacheManager)
        {
            this._bannerRepository = bannerRepository;
            this._eventPublisher = eventPublisher;
            this._cacheManager = cacheManager;
        }

        /// <summary>
        /// Get banner by id
        /// </summary>
        /// <param name="Id">banner id</param>
        /// <returns></returns>
        public Banner GetBannerById(int Id)
        {
            if (Id == 0)
                return null;

            string key = string.Format(BANNER_BY_ID_KEY, Id);
            return _cacheManager.Get(key, () =>
            {
                var n = _bannerRepository.GetById(Id);
                return n;
            });
        }

        /// <summary>
        /// Get banner by type and by should it be displayed on homepage
        /// </summary>
        /// <param name="type">banner type</param>
        /// <param name="onHomePage">displayed on homepage</param>
        /// <returns>banner</returns>
        public IList<Banner> GetBannersByType(BannerTypeEnum type, bool onHomePage)
        {
            string key = string.Format(BANNER_BY_TYPE_KEY, (int)type, false);
            return _cacheManager.Get(key, () =>
            {
                var n = _bannerRepository.Table.Where(x => x.BannerTypeId == (int)type).ToList();
                return n;
            });
        }

        /// <summary>
        /// Gat all baners fo homepage or for inner page
        /// </summary>
        /// <param name="onhomePage">indicates what kind of banners to get</param>
        /// <returns></returns>
        public IList<Banner> GetBanners(bool onhomePage)
        {
            string key = string.Format(BANNER_HOMEPAGE_KEY, onhomePage);
            return _cacheManager.Get(key, () =>
            {
                var n = _bannerRepository.Table.Where(x => x.DisplayOnMain == onhomePage).ToList();
                return n;
            });
        }

        /// <summary>
        /// Delete banner
        /// </summary>
        /// <param name="banner">banner to delete</param>
        public void DeleteBannner(Banner banner)
        {
            if (banner == null)
                throw new ArgumentNullException("banner");
            _bannerRepository.Delete(banner);

            _cacheManager.RemoveByPattern(BANNER_PATTERN_KEY);

            _eventPublisher.EntityDeleted(banner);
        }

        /// <summary>
        /// Update banner
        /// </summary>
        /// <param name="banner">banner to update</param>
        public void UpdateBanner(Banner banner)
        {
            if (banner == null)
                throw new ArgumentNullException("banner");

            _bannerRepository.Update(banner);

            _cacheManager.RemoveByPattern(BANNER_PATTERN_KEY);
            _eventPublisher.EntityUpdated(banner);
        }

        /// <summary>
        /// Insert new banner
        /// </summary>
        /// <param name="banner">banner to insert</param>
        public void InsertBanner(Banner banner)
        {
            if (banner == null)
                throw new ArgumentNullException("banner");

            _bannerRepository.Insert(banner);

            _cacheManager.RemoveByPattern(BANNER_PATTERN_KEY);
            _eventPublisher.EntityInserted(banner);
        }

        /// <summary>
        /// Get all banners
        /// </summary>
        /// <returns></returns>
        public IList<Banner> GetAllBanners()
        {
            string key = string.Format(BANNER_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                var n = _bannerRepository.Table.ToList();
                return n;
            });
        }
    }
}
