using System.Web.Mvc;
using Nop.Web.Framework.Security;
using Nop.Web.Models.Customer;
using Nop.Services.BannerService;
using Nop.Core;
using Nop.Core.Domain;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Web.Models.Banner;
using Nop.Services.Media;

namespace Nop.Web.Controllers
{
    public partial class HomeController : BaseNopController
    {
        private readonly IBannerService _bannerService;
        private readonly IWorkContext _workContext;
        private readonly IPictureService _pictureService;
        public HomeController(IBannerService bannerService,
            IWorkContext workContext,
            IPictureService pictureService)
        {
            this._bannerService = bannerService;
            this._workContext = workContext;
            this._pictureService = pictureService;
        }

        [NopHttpsRequirement(SslRequirement.No)]
        public ActionResult Index()
        {
            IList<Banner> banners;
            if (_workContext.CurrentCustomer.IsRegistered())
            {
                banners = _bannerService.GetAllBanners()
                    .Where(x => x.DisplayOnMain).ToList();
            }
            else
            {
                banners = _bannerService.GetAllBanners()
                    .Where(x => x.DisplayOnMain).ToList();
            }
            var model = banners.Select(x => new BannerModel()
            {
                AltText = x.Alt,
                BannerType = x.BannerType,
                TitleText = x.Title,
                Url = x.Url,
                ImageUrl = _pictureService.GetPictureUrl(x.PictureId,showDefaultPicture: false)
            }).ToList();

            return View(model);
        }
    }
}
