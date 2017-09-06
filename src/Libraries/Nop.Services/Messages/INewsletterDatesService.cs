using Nop.Core.Domain.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Messages
{
    public interface INewsletterDatesService
    {
        IList<NewsletterDates> GetAllNewsletterDates();
        void UpdateNewsletterDates(NewsletterDates entity);
    }
}
