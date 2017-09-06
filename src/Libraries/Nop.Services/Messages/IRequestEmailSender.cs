using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Messages
{
    public interface IRequestEmailSender
    {
        /// <summary>
        /// Send email about new request of product or service or buying request
        /// </summary>
        /// <param name="requestId">request id</param>
        /// <param name="languageId">primary language on wich to look for localised resources</param>
        void SendNewRequestEmail(int requestId, int languageId);

        /// <summary>
        /// Send email about new responce to specified request
        /// </summary>
        /// <param name="requestId">request id</param>
        /// <param name="languageId">primary language on wich to look for localised resources</param>
        void SendRequestResponceEmail(int requestId, int languageId);
    }
}
