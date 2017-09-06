
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
    public class MiniSiteTextPageService : IMiniSiteTextPageService
    {
        private const string MINISITETEXT_BY_ID_KEY = "Nop.minisitetext.id-{0}";
        private const string MINISITETEXT_NAME_SITE_ID_KEY = "Nop.minisitetext.name.siteid-{0}-{1}";
        private const string MINISITETEXT_PATTERN_KEY = "Nop.minisitetext.";


        private readonly IRepository<MiniSiteTextPage> _repository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        public MiniSiteTextPageService(IRepository<MiniSiteTextPage> repository,
            IEventPublisher eventPublisher,
            ICacheManager cacheManager)
        {
            this._repository = repository;
            this._eventPublisher = eventPublisher;
            this._cacheManager = cacheManager;
        }

        public MiniSiteTextPage GetById(int id)
        {
            if (id == 0)
                return null;

            string key = string.Format(MINISITETEXT_BY_ID_KEY, id);
            return _cacheManager.Get(key, () =>
            {
                var n = _repository.GetById(id);
                return n;
            });
        }

        public MiniSiteTextPage GetByMiniSiteAndName(string name, int miniSiteId)
        {
            if (String.IsNullOrEmpty(name) || miniSiteId == 0)
                return null;


            string key = string.Format(MINISITETEXT_NAME_SITE_ID_KEY, name,miniSiteId);
            return _cacheManager.Get(key, () =>
            {
                var n = _repository.Table.Where(x => x.UserMiniSiteId == miniSiteId && x.Title == name).FirstOrDefault();
                return n;
            });
        }

        public void Insert(MiniSiteTextPage page)
        {
            _repository.Insert(page);
            _eventPublisher.EntityInserted(page);

            _cacheManager.RemoveByPattern(MINISITETEXT_PATTERN_KEY);
        }

        public void Delete(MiniSiteTextPage page)
        {
            _repository.Delete(page);
            _eventPublisher.EntityDeleted(page);

            _cacheManager.RemoveByPattern(MINISITETEXT_PATTERN_KEY);
        }

        public void Update(MiniSiteTextPage page)
        {
            _repository.Update(page);
            _eventPublisher.EntityUpdated(page);

            _cacheManager.RemoveByPattern(MINISITETEXT_PATTERN_KEY);
        }

    }
}
