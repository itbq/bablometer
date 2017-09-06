using Nop.Core.Domain.MiniSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.MiniSite
{
    public interface IMiniSiteService
    {
        UserMiniSite GetMiniSiteByUserId(int customerId);
        UserMiniSite GetMiniSiteById(int miniSiteId);
        UserMiniSite GetMiniSiteByDomain(string Domain);
        void InsertMiniSite(UserMiniSite miniSite);
        void DeeteMiniSite(UserMiniSite miniSite);
        void UpdateMiniSite(UserMiniSite miniSite);
        IList<UserMiniSite> GetAllMiniSites();
    }
}
