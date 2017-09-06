using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Event;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Seo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.EventService
{
    public partial class EventService:IEventService
    {
        private const string EVENT_BY_ID_KEY = "Nop.events.id-{0}";
        private const string EVENT_ALL_KEY = "Nop.events.all";
        private const string EVENT_PATTERN_KEY = "Nop.events.";


        private readonly IRepository<Event> _eventRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        public EventService(IRepository<Event> _repository,
            IEventPublisher eventPublisher,
            ICacheManager cacheManager)
        {
            this._eventRepository = _repository;
            this._eventPublisher = eventPublisher;
            this._cacheManager = cacheManager;
        }

        /// <summary>
        /// Get all events
        /// </summary>
        /// <returns></returns>
        public IList<Event> GetAllEvents()
        {
            string key = string.Format(EVENT_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                var n = _eventRepository.Table.ToList();
                return n;
            });
        }

        /// <summary>
        /// Get event by id
        /// </summary>
        /// <param name="id">event id</param>
        /// <returns></returns>
        public Event GetEventById(int id)
        {
            if (id == 0)
                return null;

            string key = string.Format(EVENT_BY_ID_KEY, id);
            return _cacheManager.Get(key, () =>
            {
                var n = _eventRepository.GetById(id);
                return n;
            });
        }

        /// <summary>
        /// Delete event 
        /// </summary>
        /// <param name="entity">event to delete</param>
        public void DeleteEvent(Event entity)
        {
            if (entity == null)
                throw (new ArgumentNullException("event"));

            _eventRepository.Delete(entity);

            _cacheManager.RemoveByPattern(EVENT_PATTERN_KEY);
            //event notification
            _eventPublisher.EntityDeleted(entity);
        }

        /// <summary>
        /// Update event
        /// </summary>
        /// <param name="entity">event to update</param>
        public void UpdateEvent(Event entity)
        {
            if (entity == null)
                throw (new ArgumentNullException("event"));

            _eventRepository.Update(entity);

            _cacheManager.RemoveByPattern(EVENT_PATTERN_KEY);
            //event notification
            _eventPublisher.EntityUpdated(entity);
        }

        /// <summary>
        /// Add event
        /// </summary>
        /// <param name="entity">event to add</param>
        public void AddEvent(Event entity)
        {
            if (entity == null)
                throw (new ArgumentNullException("event"));

            _eventRepository.Insert(entity);

            _cacheManager.RemoveByPattern(EVENT_PATTERN_KEY);
            //event notification
            _eventPublisher.EntityInserted(entity);
        }

        /// <summary>
        /// Get all events paged
        /// </summary>
        /// <param name="pageNum">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns></returns>
        public IPagedList<Event> GetAllEvents(int pageNum = 0, int pageSize = int.MaxValue)
        {
            var events = _eventRepository.Table
                .OrderBy(x => x.StartDate);
            var list = new PagedList<Event>(events, pageNum, pageSize);
            return list;
        }
    }
}
