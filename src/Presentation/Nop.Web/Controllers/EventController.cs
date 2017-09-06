using Nop.Core.Domain.Event;
using Nop.Services.EventService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Web.Extensions;
using Nop.Services.Media;
using Nop.Web.Models.Media;
using Nop.Core.Domain.Media;
using Nop.Web.Models.Event;
using Nop.Core;
using Nop.Web.Framework.UI.Paging;
using Nop.Core.Domain.Catalog;
using Nop.Services.Configuration;
using Nop.Services.BannerService;
using Nop.Web.Models.Banner;
using Nop.Core.Domain;
using System.ServiceModel.Syndication;
using Nop.Web.Framework;
using Nop.Services.Localization;

namespace Nop.Web.Controllers
{
    public partial class EventController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingsService;
        private readonly IBannerService _bannerService;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly ILocalizationService _localizationService;

        public EventController(IEventService eventService,
            IPictureService pictureService,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            IWebHelper webHelper,
            IWorkContext workContext,
            ISettingService settingsService,
            IBannerService bannerService,
            StoreInformationSettings storeInformationSettings,
            ILocalizationService localizationService)
        {
            this._eventService = eventService;
            this._pictureService = pictureService;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._webHelper = webHelper;
            this._workContext = workContext;
            this._settingsService = settingsService;
            this._bannerService = bannerService;
            this._storeInformationSettings = storeInformationSettings;
            this._localizationService = localizationService;
        }

        public ActionResult List(EventPagingModel pageModel)
        {
            pageModel.PageSize = int.Parse(_settingsService.GetSettingByKey("event.eventsnumber").Value); 
            var events = _eventService.GetAllEvents()
                .Where(x => (x.EndDate.HasValue && x.EndDate.Value.Date >= DateTime.UtcNow.Date) || (!x.EndDate.HasValue && x.StartDate.Date >= DateTime.UtcNow.Date))
                .OrderBy(x => x.StartDate);
            int pictureSize = _mediaSettings.EventThumbNailImageSize;
            var model = events.Select(x => x.ToModel(_workContext.WorkingLanguage.Id))
                .Where(x=>x.Title!=null && x.ShortDescription != null && x.FullDescription != null)
                .Select(x =>
                {
                    x.PictureModel = new PictureModel()
                        {
                            FullSizeImageUrl = _pictureService.GetPictureUrl(x.PictureId),
                            ImageUrl = _pictureService.GetPictureUrl(x.PictureId, pictureSize,showDefaultPicture:false)
                        };
                    x.CatalogPictureModel = new PictureModel()
                    {
                        FullSizeImageUrl = _pictureService.GetPictureUrl(x.CatalogPictureid),
                        ImageUrl = _pictureService.GetPictureUrl(x.CatalogPictureid, pictureSize, showDefaultPicture: false)
                    };
                    return x;
                });
            IPagedList<EventInfoModel> modeList = new PagedList<EventInfoModel>(model.ToList(), pageModel.PageIndex, pageModel.PageSize);
            pageModel.LoadPagedList(modeList);

            var resultModel = new EventNavigationModel()
            {
                PagingContext = pageModel,
                EventList = modeList
            };

            resultModel.Banners = _bannerService.GetAllBanners().Where(x => !x.DisplayOnMain)
                .Select(x => new BannerModel()
                {
                    BannerType = x.BannerType,
                    TitleText = x.Title,
                    AltText = x.Alt,
                    Url = x.Url,
                    ImageUrl = _pictureService.GetPictureUrl(x.PictureId)
                }).ToList();
            return View(resultModel);
        }

        public ActionResult EventInfo(int eventid)
        {
            var evnt = _eventService.GetEventById(eventid);
            if (evnt.EndDate.HasValue)
            {
                if(evnt.EndDate.Value.Date < DateTime.UtcNow.Date)
                    return RedirectToAction("List", "Event");
            }else
            {
                if(evnt.StartDate.Date < DateTime.UtcNow.Date)
                    return RedirectToAction("List", "Event");
            }
            var model = evnt.ToModel(_workContext.WorkingLanguage.Id);
            if (model.Title == null || model.FullDescription == null || model.ShortDescription == null)
            {
                return RedirectToAction("List", "Event");
            }
            int pictureSize = _mediaSettings.EventDetailsImageSize;
            model.PictureModel = new PictureModel()
                {
                    FullSizeImageUrl = _pictureService.GetPictureUrl(evnt.PictureId),
                    ImageUrl = _pictureService.GetPictureUrl(evnt.PictureId,showDefaultPicture:false)
                };
            model.CatalogPictureModel = new PictureModel()
            {
                FullSizeImageUrl = _pictureService.GetPictureUrl(evnt.CatalogPictureId),
                ImageUrl = _pictureService.GetPictureUrl(evnt.CatalogPictureId, showDefaultPicture: false)
            };
            return View(model);
        }

        public PartialViewResult LatestEvents()
        {
            var events = _eventService.GetAllEvents()
                .Where(x=>(x.EndDate.HasValue && x.EndDate.Value.Date >= DateTime.UtcNow.Date) || (!x.EndDate.HasValue && x.StartDate.Date >= DateTime.UtcNow.Date))
                .OrderBy(x => x.StartDate);
            int pictureSize = _mediaSettings.EventThumbNailImageSize;
            var model = events.Select(x => x.ToModel(_workContext.WorkingLanguage.Id))
                .Where(x=>x.Title!=null && x.ShortDescription != null && x.FullDescription != null)
                .Take(3)
                .Select(x =>
                {
                    x.PictureModel = new PictureModel()
                        {
                            FullSizeImageUrl = _pictureService.GetPictureUrl(x.PictureId),
                            ImageUrl = _pictureService.GetPictureUrl(x.PictureId, pictureSize)
                        };
                    return x;
                });
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ShareButton(bool product)
        {
            string shareCode = _catalogSettings.PageShareCode;
            if (product)
            {
                shareCode = _catalogSettings.PageShareProduct;
            }
            else
            {
                shareCode = _catalogSettings.PageShareCode;
            }
            if (_webHelper.IsCurrentConnectionSecured())
            {
                //need to change the addthis link to be https linked when the page is, so that the page doesnt ask about mixed mode when viewed in https...
                shareCode = shareCode.Replace("http://", "https://");
            }
            return PartialView("~/Views/Catalog/ShareButton.cshtml", shareCode);
        }
        /// <summary>
        /// Show recent event rss feed
        /// </summary>
        /// <returns></returns>
        public ActionResult RecentlyAddedEventsFeed()
        {
            var feed = new SyndicationFeed(
                                    string.Format("{0}: {1}", _storeInformationSettings.StoreName, _localizationService.GetResource("Events.LatestEvents")),
                                    _localizationService.GetResource("Events.LatestEvents"),
                                    new Uri(_webHelper.GetStoreLocation(false)),
                                    "EventsRSS",
                                    DateTime.UtcNow);

            var items = new List<SyndicationItem>();
            var events = _eventService.GetAllEvents()
                 .Where(x => (x.EndDate.HasValue && x.EndDate.Value.Date >= DateTime.UtcNow.Date) || (!x.EndDate.HasValue && x.StartDate.Date >= DateTime.UtcNow.Date))
                 .OrderBy(x => x.StartDate);
            int pictureSize = _mediaSettings.EventThumbNailImageSize;
            var model = events.Select(x => x.ToModel(_workContext.WorkingLanguage.Id))
                .Where(x => x.Title != null && x.ShortDescription != null && x.FullDescription != null)
                .Take(_settingsService.GetSettingByKey<int>("Rss.Events.Count"))
                .Select(x =>
                {
                    x.PictureModel = new PictureModel()
                    {
                        FullSizeImageUrl = _pictureService.GetPictureUrl(x.PictureId),
                        ImageUrl = _pictureService.GetPictureUrl(x.PictureId, pictureSize,showDefaultPicture:false)
                    };
                    return x;
                });
            foreach (var eventItem in model)
            {
                string eventUrl = Url.RouteUrl("Event", new { SeName = eventItem.SeName }, "http");
                string content = "";
                if (eventItem.PictureModel.ImageUrl != null && eventItem.PictureModel.ImageUrl != "")
                    content += "<img src=\"" + eventItem.PictureModel.ImageUrl + "\" /><br>";
                content += eventItem.ShortDescription;
                items.Add(new SyndicationItem(eventItem.Title, content, new Uri(eventUrl), String.Format("RecentlyAddedProduct:{0}", eventItem.Id), eventItem.StartDate));
            }
            feed.Items = items;
            return new RssActionResult() { Feed = feed };
        }
    }
}
