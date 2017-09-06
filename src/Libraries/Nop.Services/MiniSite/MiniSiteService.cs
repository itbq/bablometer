using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.MiniSite;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.MiniSite
{
    public class MiniSiteService : IMiniSiteService
    {
        private const string MINISITE_BY_ID_KEY = "Nop.minisites.id-{0}";
        private const string MINISITE_DOMAIN_KEY = "Nop.minisites.domain-{0}";
        private const string MINISITE_PATTERN_KEY = "Nop.minisites.";

        private readonly IRepository<UserMiniSite> _repository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        public MiniSiteService(IRepository<UserMiniSite> repository,
            IEventPublisher eventPublisher,
            ICacheManager cacheManager)
        {
            this._repository = repository;
            this._eventPublisher = eventPublisher;
            this._cacheManager = cacheManager;
        }

        public UserMiniSite GetMiniSiteByUserId(int customerId)
        {
            if (customerId == 0)
                return null;

            return _repository.Table.Where(x => x.Customer.Id == customerId).FirstOrDefault();
        }

        public UserMiniSite GetMiniSiteById(int miniSiteId)
        {
            if (miniSiteId == 0)
                return null;

            string key = string.Format(MINISITE_BY_ID_KEY, miniSiteId);
            return _cacheManager.Get(key, () =>
            {
                var n = _repository.GetById(miniSiteId);
                return n;
            });
        }

        public UserMiniSite GetMiniSiteByDomain(string Domain)
        {
            if (String.IsNullOrEmpty(Domain))
                return null;
            string key = string.Format(MINISITE_DOMAIN_KEY, Domain);
            return _cacheManager.Get(key, () =>
            {
                var n = _repository.Table.Where(x=>x.DomainName == Domain).FirstOrDefault();
                return n;
            });
        }

        public void InsertMiniSite(UserMiniSite miniSite)
        {
            _repository.Insert(miniSite);
            _eventPublisher.EntityInserted(miniSite);

            _cacheManager.RemoveByPattern(MINISITE_PATTERN_KEY);
        }

        public void DeeteMiniSite(UserMiniSite miniSite)
        {
            _repository.Delete(miniSite);
            _eventPublisher.EntityDeleted(miniSite);

            _cacheManager.RemoveByPattern(MINISITE_PATTERN_KEY);
        }

        public void UpdateMiniSite(UserMiniSite miniSite)
        {
            _repository.Update(miniSite);
            _eventPublisher.EntityUpdated(miniSite);

            _cacheManager.RemoveByPattern(MINISITE_PATTERN_KEY);
        }

        public IList<UserMiniSite> GetAllMiniSites()
        {
            return _repository.Table.ToList();
        }
    }
}
