using Nop.Core;
using Nop.Core.Domain.News;
using Nop.Services.News;
using Nop.Web.Models.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Services.Seo;
using Nop.Services.Helpers;
using Nop.Web.Models.Media;
using Nop.Services.Media;
using Nop.Services.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Core.Domain;
using Nop.Web.Controllers;

namespace Nop.Web.Areas.MiniSite.Controllers
{
    public class NewsController : BaseNopController
    {
        private readonly INewsService _newsService;
        private readonly IWorkContext _workContext;
        private readonly NewsSettings _newsSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPictureService _pictureService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly StoreInformationSettings _storeInformationSettings;

        public NewsController(INewsService newsService,
            IWorkContext workContext,
            NewsSettings newsSettings,
            IDateTimeHelper dateTimeHelper,
            IPictureService pictureService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            StoreInformationSettings storeInformationSettings)
        {
            this._newsService = newsService;
            this._workContext = workContext;
            this._newsSettings = newsSettings;
            this._dateTimeHelper = dateTimeHelper;
            this._pictureService = pictureService;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._storeInformationSettings = storeInformationSettings;
        }

        [NonAction]
        protected void PrepareNewsItemModel(NewsItemModel model, NewsItem newsItem, bool prepareComments)
        {
            if (newsItem == null)
                throw new ArgumentNullException("newsItem");

            if (model == null)
                throw new ArgumentNullException("model");

            switch (newsItem.ExtendedProfileOnly)
            {
                case (int)NewsDisplayEnum.Both:
                    {
                        model.MiniSite = _localizationService.GetResource("News.Manage.MiniSite.Location.Both");
                        break;
                    }
                case (int)NewsDisplayEnum.Main:
                    {
                        model.MiniSite = _storeInformationSettings.StoreName;
                        break;
                    }
                case (int)NewsDisplayEnum.MiniSite:
                    {
                        model.MiniSite = _localizationService.GetResource("News.Manage.MiniSite.Location.MiniSite"); ;
                        break;
                    }
            }
            model.ExtendedProfileDisplay = newsItem.ExtendedProfileOnly;
            model.Language = newsItem.LanguageId;
            model.Featured = newsItem.Featured;
            model.Id = newsItem.Id;
            model.SeName = newsItem.GetSeName(newsItem.LanguageId, ensureTwoPublishedLanguages: false);
            model.Title = newsItem.Title;
            model.Short = newsItem.Short;
            model.Full = newsItem.Full;
            model.AllowComments = newsItem.AllowComments;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(newsItem.CreatedOnUtc, DateTimeKind.Utc);
            model.NumberOfComments = newsItem.ApprovedCommentCount;
            model.CustomerId = newsItem.CustomerId.GetValueOrDefault();
            model.PictureId = newsItem.GetAttribute<int>(SystemCustomerAttributeNames.PictureId);
            model.CatalogPictureId = newsItem.GetAttribute<int>(SystemCustomerAttributeNames.PictureCatalogId);
            if (model.PictureId != 0)
            {
                model.Picture = new PictureModel() { ImageUrl = _pictureService.GetPictureUrl(model.PictureId, showDefaultPicture: false) };
            }

            if (model.CatalogPictureId != 0)
            {
                model.CatalogPicture = new PictureModel() { ImageUrl = _pictureService.GetPictureUrl(model.CatalogPictureId, showDefaultPicture: false) };
            }

            model.Published = newsItem.Published;
            model.PublishingDate = newsItem.PublishingDate;
        }

        public ActionResult LatestNews()
        {
            var customer = _workContext.CurrentMiniSite.Customer;
            var companyNews = _newsService.GetAllCompanyNews(_workContext.WorkingLanguage.Id, customer.Id, 0, 2,miniSite: (int)(NewsDisplayEnum.MiniSite | NewsDisplayEnum.Both));
            var model = companyNews
               .Select(x =>
               {
                   var newsModel = new NewsItemModel();
                   PrepareNewsItemModel(newsModel, x, false);
                   if (newsModel.PictureId != 0)
                   {
                       newsModel.Picture = new PictureModel()
                       {
                           ImageUrl = _pictureService.GetPictureUrl(newsModel.PictureId,showDefaultPicture:false,targetSize:150)
                       };
                   }

                   //newsModel.Short = newsModel.Short.Length > 81 ? newsModel.Short.Substring(0, 80) : newsModel.Short;
                   newsModel.Short = newsModel.Short.Replace("<p>", "");
                   newsModel.Short = newsModel.Short.Replace("</p>", "");
                   return newsModel;
               }).ToList();
            return View(model);
        }

        public ActionResult List(NewsPagingFilteringModel command, string seName)
        {
            int sellerId = _workContext.CurrentMiniSite.Customer.Id;
            var model = new NewsItemListModel();
            model.WorkingLanguageId = _workContext.WorkingLanguage.Id;

            if (command.PageSize <= 0) command.PageSize = _newsSettings.NewsArchivePageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;

            var newsItems = _newsService.GetAllCompanyNews(_workContext.WorkingLanguage.Id, sellerId, command.PageNumber - 1, command.PageSize, miniSite: (int)(NewsDisplayEnum.Both | NewsDisplayEnum.MiniSite));
            model.PagingFilteringContext.LoadPagedList(newsItems);

            if (newsItems.Count == 0 && command.PageNumber > 1)
                return RedirectToAction("List");

            model.NewsItems = newsItems
                .Select(x =>
                {
                    var newsModel = new NewsItemModel();
                    PrepareNewsItemModel(newsModel, x, false);
                    if (newsModel.PictureId != 0)
                    {
                        newsModel.Picture = new PictureModel()
                        {
                            ImageUrl = _pictureService.GetPictureUrl(newsModel.PictureId)
                        };
                    }
                    return newsModel;
                })
                .ToList();

            return View(model);
        }

        public ActionResult NewsItem(int newsItemId)
        {
            if (!_newsSettings.Enabled)
                return RedirectToRoute("MiniSiteHomePage");

            var newsItem = _newsService.GetNewsById(newsItemId);
            if (newsItem.LanguageId != _workContext.WorkingLanguage.Id)
            {
                return RedirectToAction("List");
            }
            if (newsItem == null ||
                !newsItem.Published ||
                (newsItem.StartDateUtc.HasValue && newsItem.StartDateUtc.Value >= DateTime.UtcNow) ||
                (newsItem.EndDateUtc.HasValue && newsItem.EndDateUtc.Value <= DateTime.UtcNow))
                return RedirectToRoute("MiniSiteHomePage");

            if (newsItem.Customer == null || newsItem.Customer.Id != _workContext.CurrentMiniSite.Customer.Id)
            {
                throw new HttpException(404, "Not found");
            }
            var model = new NewsItemModel();
            PrepareNewsItemModel(model, newsItem, true);

            return View(model);
        }
    }
}
