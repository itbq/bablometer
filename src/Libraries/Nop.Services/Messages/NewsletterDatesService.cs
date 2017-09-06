using Nop.Core.Data;
using Nop.Core.Domain.Messages;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Messages
{
    public partial class NewsletterDatesService: INewsletterDatesService
    {
        private readonly IRepository<NewsletterDates> _repository;
        private readonly IEventPublisher _eventPublisher;

        public NewsletterDatesService(IRepository<NewsletterDates> repository,
            IEventPublisher eventPublisher)
        {
            this._repository = repository;
            this._eventPublisher = eventPublisher;
        }

        public IList<NewsletterDates> GetAllNewsletterDates()
        {
            return _repository.Table.ToList();
        }

        public void UpdateNewsletterDates(NewsletterDates entity)
        {
            _repository.Update(entity);

            _eventPublisher.EntityUpdated(entity);
        }
    }
}
