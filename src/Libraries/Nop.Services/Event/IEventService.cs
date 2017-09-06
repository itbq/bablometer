using Nop.Core;
using Nop.Core.Domain.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.EventService
{
    public interface IEventService
    {
        IPagedList<Event> GetAllEvents(int pageNum = 0, int pageSize = int.MaxValue);
        IList<Event> GetAllEvents();
        Event GetEventById(int id);
        void DeleteEvent(Event entity);
        void UpdateEvent(Event entity);
        void AddEvent(Event entity);
    }
}
