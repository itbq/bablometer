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
    public class SliderItemService : ISliderItemService
    {
        private readonly IRepository<SliderItem> _repository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        public SliderItemService(IRepository<SliderItem> repository,
            ICacheManager cacheManager,
            IEventPublisher eventPublisher)
        {
            this._repository = repository;
            this._cacheManager = cacheManager;
            this._eventPublisher = eventPublisher;
        }
        public IList<SliderItem> GetMailPageSlides()
        {
            return _repository.Table.Where(x => !x.UserMiniSiteId.HasValue).ToList();
        }

        public void Insert(SliderItem item)
        {
            if (item == null)
                throw new ArgumentNullException("SliderItem");
            _repository.Insert(item);
        }

        public void Delete(SliderItem item)
        {
            if (item == null)
                throw new ArgumentNullException("SliderItem");
            _repository.Delete(item);
        }

        public void Update(SliderItem item)
        {
            if (item == null)
                throw new ArgumentNullException("SliderItem");
            _repository.Update(item);
        }
    }
}
