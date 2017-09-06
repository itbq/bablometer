using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Messages
{
    public interface IMiniSiteEmailSender
    {
        /// <summary>
        /// Send notification to administrator about mini iste domain ghange request
        /// </summary>
        /// <param name="miniSiteId"></param>
        /// <param name="oldDomainName"></param>
        /// <param name="languageId"></param>
        void SendMiniSiteChangeDomainEmail(int miniSiteId, string oldDomainName, int languageId);

        /// <summary>
        /// Send notification to administrator about mini site activation request
        /// </summary>
        /// <param name="miniSiteId"></param>
        /// <param name="languageId"></param>
        void SendMiniSiteActivationEmail(int miniSiteId, int languageId);

        /// <summary>
        /// Send email to customer about succesfull mini site activation
        /// </summary>
        /// <param name="miniSiteId"></param>
        /// <param name="languageId"></param>
        void SendMiniSiteAcceptEmail(int miniSiteId, int languageId);

        /// <summary>
        /// Send email to customer about unsecessfull mini site activation
        /// </summary>
        /// <param name="miniSiteId"></param>
        /// <param name="languageId"></param>
        void SendMiniSiteRejectEmail(int miniSiteId, int languageId);
    }
}
