using Nop.Core.Domain.MiniSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.MiniSite
{
    public interface IMiniSiteBannerService
    {
        BannerMiniSite GetById(int id);
        void Insert(BannerMiniSite banner);
        void Delete(BannerMiniSite banner);
        void Update(BannerMiniSite banner);
    }
}
