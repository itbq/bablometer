using Nop.Core.Domain.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Messages
{
    public interface INewPublicationEmailSender
    {
        void SendNewsPublicationNotification(NewsItem newItem);
    }
}
