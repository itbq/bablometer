using Nop.Core;
using Nop.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services.BannerService
{
    public interface IBannerService
    {
        Banner GetBannerById(int Id);
        IList<Banner> GetBannersByType(BannerTypeEnum type, bool onHomePage);
        IList<Banner> GetBanners(bool onhomePage);
        void DeleteBannner(Banner banner);
        void UpdateBanner(Banner banner);
        void InsertBanner(Banner banner);
        IList<Banner> GetAllBanners();
    }
}
