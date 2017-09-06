using Nop.Core.Domain.MiniSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.MiniSite
{
    public interface IMiniSiteTextPageService
    {
        MiniSiteTextPage GetById(int id);
        MiniSiteTextPage GetByMiniSiteAndName(string name, int miniSiteId);
        void Insert(MiniSiteTextPage page);
        void Delete(MiniSiteTextPage page);
        void Update(MiniSiteTextPage page);
    }
}
