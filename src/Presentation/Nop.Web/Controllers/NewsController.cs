using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Seo;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.UI.Captcha;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.News;
using Nop.Web.Models.Common;
using Nop.Web.Extensions;
using Nop.Web.Models.Media;
using Telerik.Web.Mvc;
using Nop.Services.Configuration;
using System.Web;

namespace Nop.Web.Controllers
{
    [NopHttpsRequirement(SslRequirement.No)]
    public partial class NewsController : BaseNopController
    {
		#region Fields

        private readonly INewsService _newsService;
        private readonly IWorkContext _workContext;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerContentService _customerContentService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IWebHelper _webHelper;
        private readonly ICacheManager _cacheManager;
        private readonly ICustomerActivityService _customerActivityService;

        private readonly MediaSettings _mediaSettings;
        private readonly NewsSettings _newsSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly ILanguageService _languageService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICompanyInformationService _companyInformationService;
        private readonly ISettingService _settingService;

        #endregion

		#region Constructors

        public NewsController(INewsService newsService, 
            IWorkContext workContext, IPictureService pictureService, ILocalizationService localizationService,
            ICustomerContentService customerContentService, IDateTimeHelper dateTimeHelper,
            IWorkflowMessageService workflowMessageService, IWebHelper webHelper,
            ICacheManager cacheManager, ICustomerActivityService customerActivityService,
            MediaSettings mediaSettings, NewsSettings newsSettings,
            LocalizationSettings localizationSettings, CustomerSettings customerSettings,
            StoreInformationSettings storeInformationSettings, CaptchaSettings captchaSettings,
            ILanguageService languageService,
            IUrlRecordService urlRecordService,
            IGenericAttributeService genericAttributeService,
            ICompanyInformationService companyInformationService,
            ISettingService settingService)
        {
            this._newsService = newsService;
            this._workContext = workContext;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._customerContentService = customerContentService;
            this._dateTimeHelper = dateTimeHelper;
            this._workflowMessageService = workflowMessageService;
            this._webHelper = webHelper;
            this._cacheManager = cacheManager;
            this._customerActivityService = customerActivityService;

            this._mediaSettings = mediaSettings;
            this._newsSettings = newsSettings;
            this._localizationSettings = localizationSettings;
            this._customerSettings = customerSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._captchaSettings = captchaSettings;
            this._languageService = languageService;
            this._urlRecordService = urlRecordService;
            this._genericAttributeService = genericAttributeService;
            this._companyInformationService = companyInformationService;
            this._settingService = settingService;
        }

        #endregion

        #region Utilities

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
            model.AddNewComment.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnNewsCommentPage;
            model.CustomerId = newsItem.CustomerId.GetValueOrDefault();
            model.PictureId = newsItem.GetAttribute<int>(SystemCustomerAttributeNames.PictureId);
            model.CatalogPictureId = newsItem.GetAttribute<int>(SystemCustomerAttributeNames.PictureCatalogId);
            if(model.PictureId != 0)
            {
                model.Picture = new PictureModel() { ImageUrl = _pictureService.GetPictureUrl(model.PictureId, showDefaultPicture: false) };
            }

            if (model.CatalogPictureId != 0)
            {
                model.CatalogPicture = new PictureModel() { ImageUrl = _pictureService.GetPictureUrl(model.CatalogPictureId, showDefaultPicture: false) };
            }

            model.Published = newsItem.Published;
            model.PublishingDate = newsItem.PublishingDate;
            if (newsItem.CustomerId.HasValue)
            {
                model.Company = newsItem.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, newsItem.LanguageId);
                model.CompanySeName = newsItem.Customer.CompanyInformation.GetSeName(newsItem.LanguageId);
                if (model.Company == null)
                {
                    model.Company = newsItem.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, _languageService.GetAllLanguages().Where(x => x.LanguageCulture == "de-DE").First().Id);
                    model.CompanySeName = newsItem.Customer.CompanyInformation.GetSeName(_languageService.GetAllLanguages().Where(x => x.LanguageCulture == "de-DE").First().Id);
                }
                if (model.Company == null)
                {
                    model.Company = newsItem.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, _languageService.GetAllLanguages().Where(x => x.LanguageCulture == "es-MX").First().Id);
                    model.CompanySeName = newsItem.Customer.CompanyInformation.GetSeName(_languageService.GetAllLanguages().Where(x => x.LanguageCulture == "es-MX").First().Id);
                }
                if (model.Company == null)
                {
                    model.Company = newsItem.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, _languageService.GetAllLanguages().Where(x => x.LanguageCulture == "en-US").First().Id);
                    model.CompanySeName = newsItem.Customer.CompanyInformation.GetSeName(_languageService.GetAllLanguages().Where(x => x.LanguageCulture == "en-US").First().Id);
                }
                if (model.Company == null)
                {
                    model.Company = newsItem.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, _languageService.GetAllLanguages().Where(x => x.LanguageCulture == "ru-RU").First().Id);
                    model.CompanySeName = newsItem.Customer.CompanyInformation.GetSeName(_languageService.GetAllLanguages().Where(x => x.LanguageCulture == "ru-RU").First().Id);
                }
                model.CompanyId = newsItem.Customer.CompanyInformationId.GetValueOrDefault();
            }

            if (prepareComments)
            {
                var newsComments = newsItem.NewsComments.Where(n => n.IsApproved).OrderBy(pr => pr.CreatedOnUtc);
                foreach (var nc in newsComments)
                {
                    var commentModel = new NewsCommentModel()
                    {
                        Id = nc.Id,
                        CustomerId = nc.CustomerId,
                        CustomerName = nc.Customer.FormatUserName(),
                        CommentTitle = nc.CommentTitle,
                        CommentText = nc.CommentText,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(nc.CreatedOnUtc, DateTimeKind.Utc),
                        AllowViewingProfiles = _customerSettings.AllowViewingProfiles && nc.Customer != null && !nc.Customer.IsGuest(),
                    };
                    if (_customerSettings.AllowCustomersToUploadAvatars)
                    {
                        var customer = nc.Customer;
                        string avatarUrl = _pictureService.GetPictureUrl(customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId), _mediaSettings.AvatarPictureSize, false);
                        if (String.IsNullOrEmpty(avatarUrl) && _customerSettings.DefaultAvatarEnabled)
                            avatarUrl = _pictureService.GetDefaultPictureUrl(_mediaSettings.AvatarPictureSize, PictureType.Avatar);
                        commentModel.CustomerAvatarUrl = avatarUrl;
                    }
                    model.Comments.Add(commentModel);
                }
            }
        }
        
        #endregion

        #region Methods

        public ActionResult HomePageNews()
        {
            if (!_newsSettings.Enabled || !_newsSettings.ShowNewsOnMainPage)
                return Content("");
            
            var cacheKey = string.Format(ModelCacheEventConsumer.HOMEPAGE_NEWSMODEL_KEY, _workContext.WorkingLanguage.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var newsItems = _newsService.GetAllNews(_workContext.WorkingLanguage.Id, 0, _newsSettings.MainPageNewsCount);
                return new HomePageNewsItemsModel()
                {
                    WorkingLanguageId = _workContext.WorkingLanguage.Id,
                    NewsItems = newsItems
                        .Select(x =>
                                    {
                                        var newsModel = new NewsItemModel();
                                        PrepareNewsItemModel(newsModel, x, false);
                                        return newsModel;
                                    })
                        .ToList()
                };
            });

            //"Comments" property of "NewsItemModel" object depends on the current customer.
            //Furthermore, we just don't need it for home page news. So let's reset it.
            //But first we need to clone the cached model (the updated one should not be cached)
            var model = (HomePageNewsItemsModel)cachedModel.Clone();
            foreach (var newsItemModel in model.NewsItems)
                newsItemModel.Comments.Clear();
            return PartialView(model);
        }

        public ActionResult List(NewsPagingFilteringModel command)
        {
            if (!_newsSettings.Enabled)
                return RedirectToRoute("HomePage");

            var model = new NewsItemListModel();
            model.WorkingLanguageId = _workContext.WorkingLanguage.Id;

            if (command.PageSize <= 0) command.PageSize = _newsSettings.NewsArchivePageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;

            var newsItems = _newsService.GetAllNews(_workContext.WorkingLanguage.Id,
                command.PageNumber - 1, command.PageSize);
            model.PagingFilteringContext.LoadPagedList(newsItems);
            if (newsItems.Count == 0 && command.PageNumber > 1)
            {
                return RedirectToAction("List");
            }
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
            model.PagingFilteringContext.Approved = null;
            model.PagingFilteringContext.CreationStartDate = null;
            model.PagingFilteringContext.CreationEndDate = null;
            model.PagingFilteringContext.PublishEndDate = null;
            model.PagingFilteringContext.PublishStartDate = null;
            return View(model);
        }

        public ActionResult LatestNews()
        {
            var newsItems = _newsService.GetAllNews(_workContext.WorkingLanguage.Id, 0, int.MaxValue).OrderByDescending(x=>x.PublishingDate).Take(4).ToList();
            if (newsItems.Where(x => x.Featured).FirstOrDefault() == null)
            {
                var featuredNew = _newsService.GetFeaturedNew(_workContext.WorkingLanguage.Id);
                if (featuredNew != null)
                {
                    //if (newsItems.Where(x => x.Id == featuredNew.Id).FirstOrDefault() == null)
                    //{
                        newsItems.Remove(newsItems.Last());
                        newsItems.Add(featuredNew);
                    //}
                }
            }
            var model = newsItems
                .Select(x =>
                {
                    var newsModel = new NewsItemModel();
                    PrepareNewsItemModel(newsModel, x, false);
                    if (newsModel.PictureId != 0)
                    {
                        newsModel.Picture = new PictureModel()
                        {
                            ImageUrl = _pictureService.GetPictureUrl(newsModel.PictureId,targetSize:220)
                        };
                    }
                    return newsModel;
                }).ToList();
            return View(model);
        }

        public ActionResult SellerNews(NewsPagingFilteringModel command, string seName)
        {
            if (!_newsSettings.Enabled)
                return RedirectToRoute("HomePage");
            var model = new NewsItemListModel();
            int sellerId = 0;
            if (String.Compare(seName, "Industry", true) != 0)
            {
                if (String.Compare(seName, "CompanyNews", true) != 0)
                {
                    var slug = _urlRecordService.GetBySlug(seName);
                    if (slug == null)
                    {
                        return RedirectToRoute("NewsArchive");
                    }
                    sellerId = _urlRecordService.GetBySlug(seName).EntityId;
                }else
                {
                    sellerId = -1;
                }
            }
            else
            {
                sellerId = 0;
            }
            if (sellerId > 0)
            {
                sellerId = _companyInformationService.GetCompanyInformation(sellerId).Customers.First().Id;
            }
            model.WorkingLanguageId = _workContext.WorkingLanguage.Id;

            if (command.PageSize <= 0) command.PageSize = _newsSettings.NewsArchivePageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;

            var newsItems = _newsService.GetAllCompanyNews(_workContext.WorkingLanguage.Id, sellerId, command.PageNumber - 1, command.PageSize);
            model.PagingFilteringContext.LoadPagedList(newsItems);

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

            return View("List",model);
        }

        public ActionResult ListRss(int languageId)
        {
            var feed = new SyndicationFeed(
                                    string.Format("{0}: News", _storeInformationSettings.StoreName),
                                    "News",
                                    new Uri(_webHelper.GetStoreLocation(false)),
                                    "NewsRSS",
                                    DateTime.UtcNow);

            if (!_newsSettings.Enabled)
                return new RssActionResult() { Feed = feed };

            var items = new List<SyndicationItem>();
            var newsItems = _newsService.GetAllNews(languageId, 0, int.MaxValue);
            foreach (var n in newsItems)
            {
                string newsUrl = Url.RouteUrl("NewsItem", new { SeName = n.GetSeName(n.LanguageId, ensureTwoPublishedLanguages: false) }, "http");
                items.Add(new SyndicationItem(n.Title, n.Short, new Uri(newsUrl), String.Format("Blog:{0}", n.Id), n.CreatedOnUtc));
            }
            feed.Items = items;
            return new RssActionResult() { Feed = feed };
        }

        public ActionResult NewsItem(int newsItemId)
        {
            if (!_newsSettings.Enabled)
                return RedirectToRoute("HomePage");

            var newsItem = _newsService.GetNewsById(newsItemId);
            if (newsItem.ExtendedProfileOnly == (int)NewsDisplayEnum.MiniSite)
            {
                throw new HttpException(404, "Not found");
            }

            if (newsItem == null)
            {
                throw new HttpException(404, "Not found");
            }
            if (newsItem.LanguageId != _workContext.WorkingLanguage.Id)
            {
                return RedirectToAction("List", "News");
            }
            if (newsItem == null || 
                !newsItem.Published ||
                (newsItem.StartDateUtc.HasValue && newsItem.StartDateUtc.Value >= DateTime.UtcNow) ||
                (newsItem.EndDateUtc.HasValue && newsItem.EndDateUtc.Value <= DateTime.UtcNow))
                return RedirectToRoute("HomePage");

            var model = new NewsItemModel();
            PrepareNewsItemModel(model, newsItem, true);

            return View(model);
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult AddNews()
        {
            var customer = _workContext.CurrentCustomer;
            if (customer.CustomerRoles.Where(x => x.SystemName == "Sellers").FirstOrDefault() == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var model =  new NewsItemModel();
            model.CustomerId = customer.Id;
            model.AviableLanguages = _languageService.GetAllLanguages().Select(x=>new LanguageModel()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();
            model.WorkingLanguageId = _workContext.WorkingLanguage.Id;
            return View(model);
        }

        [HttpPost]
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult AddNews(NewsItemModel model)
        {
            var customer = _workContext.CurrentCustomer;
            if (customer.CustomerRoles.Where(x => x.SystemName == "Sellers").FirstOrDefault() == null)
            {
                return RedirectToRoute("/Home/Index");
            }

            if (ModelState.IsValid)
            {
                var newItem = model.ToEntity(_workContext.CurrentCustomer.Id);
                if (!_newsSettings.EnablePremoderation)
                {
                    newItem.Published = true;
                    newItem.PublishingDate = DateTime.UtcNow;
                }
                else
                {
                    if (newItem.ExtendedProfileOnly == (int)NewsDisplayEnum.MiniSite)
                    {
                        newItem.Published = true;
                        newItem.PublishingDate = DateTime.UtcNow;
                    }
                }
                _newsService.InsertNews(newItem);
                var seName = newItem.ValidateSeName(null, newItem.Title, true);
                _urlRecordService.SaveSlug(newItem, seName, newItem.LanguageId);
                _genericAttributeService.SaveAttribute<int>(newItem, SystemCustomerAttributeNames.PictureId, model.PictureId);
                _genericAttributeService.SaveAttribute<int>(newItem, SystemCustomerAttributeNames.PictureCatalogId, model.CatalogPictureId);
                return RedirectToAction("ManageNews");
            }

            model.CustomerId = customer.Id;
            model.AviableLanguages = _languageService.GetAllLanguages().Select(x => new LanguageModel()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
            model.WorkingLanguageId = _workContext.WorkingLanguage.Id;
            return View(model);
        }

        [HttpPost, ActionName("NewsItem")]
        [FormValueRequired("add-comment")]
        [CaptchaValidator]
        public ActionResult NewsCommentAdd(int newsItemId, NewsItemModel model, bool captchaValid)
        {
            if (!_newsSettings.Enabled)
                return RedirectToRoute("HomePage");

            var newsItem = _newsService.GetNewsById(newsItemId);
            if (newsItem == null || !newsItem.Published || !newsItem.AllowComments)
                return RedirectToRoute("HomePage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnNewsCommentPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptcha"));
            }

            if (_workContext.CurrentCustomer.IsGuest() && !_newsSettings.AllowNotRegisteredUsersToLeaveComments)
            {
                ModelState.AddModelError("", _localizationService.GetResource("News.Comments.OnlyRegisteredUsersLeaveComments"));
            }

            if (ModelState.IsValid)
            {
                var comment = new NewsComment()
                {
                    NewsItemId = newsItem.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    IpAddress = _webHelper.GetCurrentIpAddress(),
                    CommentTitle = model.AddNewComment.CommentTitle,
                    CommentText = model.AddNewComment.CommentText,
                    IsApproved = true,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                };
                _customerContentService.InsertCustomerContent(comment);

                //update totals
                _newsService.UpdateCommentTotals(newsItem);

                //notify a store owner;
                if (_newsSettings.NotifyAboutNewNewsComments)
                    _workflowMessageService.SendNewsCommentNotificationMessage(comment, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                _customerActivityService.InsertActivity("PublicStore.AddNewsComment", _localizationService.GetResource("ActivityLog.PublicStore.AddNewsComment"));

                //The text boxes should be cleared after a comment has been posted
                //That' why we reload the page
                TempData["nop.news.addcomment.result"] = _localizationService.GetResource("News.Comments.SuccessfullyAdded");
                return RedirectToRoute("NewsItem", new {SeName = newsItem.GetSeName(newsItem.LanguageId, ensureTwoPublishedLanguages: false) });
            }


            //If we got this far, something failed, redisplay form
            PrepareNewsItemModel(model, newsItem, true);
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult RssHeaderLink()
        {
            if (!_newsSettings.Enabled || !_newsSettings.ShowHeaderRssUrl)
                return Content("");

            string link = string.Format("<link href=\"{0}\" rel=\"alternate\" type=\"application/rss+xml\" title=\"{1}: News\" />",
                Url.RouteUrl("NewsRSS", new { languageId = _workContext.WorkingLanguage.Id }, _webHelper.IsCurrentConnectionSecured() ? "https" : "http"), _storeInformationSettings.StoreName);

            return Content(link);
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult ManageNews(NewsPagingFilteringModel command)
        {
            if (_workContext.CurrentCustomer.CustomerRoles.Where(x => x.SystemName == "Sellers").FirstOrDefault() == null)
                return RedirectToRoute("homePage");
            if (command.PageSize <= 0) command.PageSize = _newsSettings.NewsArchivePageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;

            var model = new NewsItemListModel();
            model.WorkingLanguageId = _workContext.WorkingLanguage.Id;
            //var newsItems = _newsService.GetAllCompanyNews(_workContext.CurrentCustomer.Id, command.PageNumber - 1, command.PageSize);
            IPagedList<NewsItem> newsItems;
            if (command.DisplayPlace == 0)
            {
                newsItems = _newsService.GetAllCompanyNews(command.LanguageId,
                    _workContext.CurrentCustomer.Id,
                    command.PageNumber - 1,
                    command.PageSize,
                    command.CreationStartDate,
                    command.CreationEndDate,
                    command.PublishStartDate,
                    command.PublishEndDate,
                    command.Approved, miniSite: (int)(NewsDisplayEnum.MiniSite | NewsDisplayEnum.Main | NewsDisplayEnum.Both));
            }
            else
            {
                newsItems = _newsService.GetAllCompanyNews(command.LanguageId,
                    _workContext.CurrentCustomer.Id,
                    command.PageNumber - 1,
                    command.PageSize,
                    command.CreationStartDate,
                    command.CreationEndDate,
                    command.PublishStartDate,
                    command.PublishEndDate,
                    command.Approved, miniSite: command.DisplayPlace);
            }
            model.PagingFilteringContext.LoadPagedList<NewsItem>(newsItems);

            model.NewsItems = newsItems.Select(x =>
                {
                    var md = new NewsItemModel();
                    PrepareNewsItemModel(md, x, false);

                    if (md.PictureId != 0)
                    {
                        md.Picture = new PictureModel()
                        {
                            ImageUrl = _pictureService.GetPictureUrl(md.PictureId, 200)
                        };
                    }
                    return md;
                }).ToList();
            model.AviableLanguages = _languageService.GetAllLanguages().Select(x=>new LanguageModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    LanguageCulture = x.LanguageCulture,
                    FlagImageFileName = x.FlagImageFileName
                }).ToList();
            model.AviableLanguages.Add(new LanguageModel()
            {
                Id = 0,
                Name = _localizationService.GetResource("ETF.News.NotSet"),
                LanguageCulture = "",
                FlagImageFileName = ""
            });
            return View(model);
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Delete(int id,int pageNumber)
        {
            var newsItem = _newsService.GetNewsById(id);
            if (newsItem.CustomerId != _workContext.CurrentCustomer.Id)
            {
                return RedirectToRoute("HomePage");
            }
            //celar entity slug
            _urlRecordService.ClearEntitySlug<NewsItem>(newsItem, newsItem.LanguageId);
            _newsService.DeleteNews(newsItem);

            return RedirectToAction("ManageNews", new { sellerId = _workContext.CurrentCustomer.Id, pagenumber = pageNumber });
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Edit(int id, int pageNumber)
        {
            var newsItem = _newsService.GetNewsById(id);
            if (newsItem == null)
                return RedirectToAction("ManageNews");
            if (newsItem.CustomerId != _workContext.CurrentCustomer.Id)
                return RedirectToRoute("HomePage");
            var model = new NewsItemModel();
            PrepareNewsItemModel(model, newsItem, false);
            model.PageNumber = pageNumber;
            model.Language = newsItem.LanguageId;
            return View(model);
        }

        [HttpPost]
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Edit(NewsItemModel model)
        {
            if (model.CustomerId != _workContext.CurrentCustomer.Id)
                return RedirectToRoute("HomePage");
            if (ModelState.IsValid)
            {
                var newsItem = _newsService.GetNewsById(model.Id);
                newsItem = model.ToEntity(newsItem, model.CustomerId);
                newsItem.Published = false;
                newsItem.Id = model.Id;
                newsItem.LanguageId = model.Language;
                if (!_newsSettings.EnablePremoderation)
                {
                    newsItem.Published = true;
                    if(!newsItem.PublishingDate.HasValue)
                        newsItem.PublishingDate = DateTime.UtcNow;
                }
                else
                {
                    if (newsItem.ExtendedProfileOnly == (int)NewsDisplayEnum.MiniSite)
                    {
                        newsItem.Published = true;
                        if (!newsItem.PublishingDate.HasValue)
                            newsItem.PublishingDate = DateTime.UtcNow;
                    }
                }
                _newsService.UpdateNews(newsItem);

                //Clear old slug
                _urlRecordService.ClearEntitySlug<NewsItem>(newsItem, newsItem.LanguageId);
                var seName = newsItem.ValidateSeName(null, newsItem.Title, true);
                _urlRecordService.SaveSlug(newsItem, seName, newsItem.LanguageId);
                var oldPictureId = newsItem.GetAttribute<int>(SystemCustomerAttributeNames.PictureId);
                if (oldPictureId != 0 && oldPictureId != model.PictureId)
                {
                    var pict = _pictureService.GetPictureById(oldPictureId);
                    if (pict != null)
                        _pictureService.DeletePicture(pict);
                }
                _genericAttributeService.SaveAttribute<int>(newsItem, SystemCustomerAttributeNames.PictureId, model.PictureId);

                oldPictureId = newsItem.GetAttribute<int>(SystemCustomerAttributeNames.PictureCatalogId);
                
                if (oldPictureId != 0 && oldPictureId != model.CatalogPictureId)
                {
                    var pict = _pictureService.GetPictureById(oldPictureId);
                    if (pict != null)
                        _pictureService.DeletePicture(pict);
                }

                _genericAttributeService.SaveAttribute<int>(newsItem, SystemCustomerAttributeNames.PictureCatalogId, model.CatalogPictureId);
                return RedirectToAction("ManageNews", new { sellerId = _workContext.CurrentCustomer.Id, pagenumber = model.PageNumber});
            }

            return View(model);
        }

        public ActionResult RecentlyAddedNewsFeed()
        {
            TimeZoneInfo timeZone = _dateTimeHelper.CurrentTimeZone;
            var feed = new SyndicationFeed(
                                    string.Format("{0}:{1}", _storeInformationSettings.StoreName, _localizationService.GetResource("News.LatestNews")),
                                    _localizationService.GetResource("News.LatestNews"),
                                    new Uri(_webHelper.GetStoreLocation(false)),
                                    "NewsRSS",
                                    DateTime.UtcNow);

            var items = new List<SyndicationItem>();
            var newsItems = _newsService.GetAllNews(_workContext.WorkingLanguage.Id,
                0, _settingService.GetSettingByKey<int>("Rss.News.Count"));

            var model = newsItems
                .Select(x =>
                {
                    var newsModel = new NewsItemModel();
                    PrepareNewsItemModel(newsModel, x, false);
                    if (newsModel.PictureId != 0)
                    {
                        newsModel.Picture = new PictureModel()
                        {
                            ImageUrl = _pictureService.GetPictureUrl(newsModel.PictureId,200,false)
                        };
                    }
                    return newsModel;
                })
                .ToList();
            foreach (var newItem in model)
            {
                string newUrl = Url.RouteUrl("NewsItem", new { SeName = newItem.SeName }, "http");
                string content = "";
                if (newItem.Picture != null)
                    content += "<img src=\"" + newItem.Picture.ImageUrl + "\" /><br>";
                if (newItem.CompanyId == 0)
                {
                    content += "<a href=\"" + Url.RouteUrl("SellerNews", new { seName = "Industry" }) + "\">Industry</a><br>";
                }
                else
                {
                    content += "<a href=\"" + Url.RouteUrl("SellerNews", new { seName = newItem.CompanySeName }) + "\">" + newItem.Company + "</a><br>";
                }
                content += newItem.Short;
                items.Add(new SyndicationItem(newItem.Title, content, new Uri(newUrl), String.Format("RecentlyAddedProduct:{0}", newItem.Id), _dateTimeHelper.ConvertToUserTime(newItem.PublishingDate.Value,TimeZoneInfo.Utc,timeZone)));
            }
            feed.Items = items;
            return new RssActionResult() { Feed = feed };
        }

        [ChildActionOnly]
        public ActionResult LatestCompanyNews(int customerId)
        {
            var companyNews = _newsService.GetAllCompanyNews(_workContext.WorkingLanguage.Id, customerId, 0, _newsSettings.LatestNewsNumber);
            var model = companyNews
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

                   if (newsModel.CatalogPictureId != 0)
                   {
                       newsModel.CatalogPicture = new PictureModel()
                       {
                           ImageUrl = _pictureService.GetPictureUrl(newsModel.PictureId,targetSize:220)
                       };
                   }
                   //newsModel.Short = newsModel.Short.Length > 81 ? newsModel.Short.Substring(0, 80) : newsModel.Short;
                   newsModel.Short = newsModel.Short.Replace("<p>", "");
                   newsModel.Short = newsModel.Short.Replace("</p>", "");
                   return newsModel;
               }).ToList();
            return View(model);
        }
        #endregion
    }
}
