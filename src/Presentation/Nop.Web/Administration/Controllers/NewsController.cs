using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.News;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.News;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.News;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;
using Nop.Services.Common;
using Nop.Core.Domain.Customers;
using Nop.Core;
using Nop.Services.Messages;
using Nop.Core.Domain;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public partial class NewsController : BaseNopController
	{
		#region Fields

        private readonly INewsService _newsService;
        private readonly ILanguageService _languageService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerContentService _customerContentService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly INewPublicationEmailSender _newPublicationEmailSender;
        private readonly StoreInformationSettings _storeInformationSettings;

		#endregion

		#region Constructors

        public NewsController(INewsService newsService, ILanguageService languageService,
            IDateTimeHelper dateTimeHelper, ICustomerContentService customerContentService,
            ILocalizationService localizationService, IPermissionService permissionService,
            IUrlRecordService urlRecordService, AdminAreaSettings adminAreaSettings,
            IGenericAttributeService genericAttributeService,
            ICustomerService customerService,
            IWorkContext workContext,
            INewPublicationEmailSender newPublicationEmailSender,
            StoreInformationSettings storeInformationSettings)
        {
            this._newsService = newsService;
            this._languageService = languageService;
            this._dateTimeHelper = dateTimeHelper;
            this._customerContentService = customerContentService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._urlRecordService = urlRecordService;
            this._adminAreaSettings = adminAreaSettings;
            this._genericAttributeService = genericAttributeService;
            this._customerService = customerService;
            this._workContext = workContext;
            this._newPublicationEmailSender = newPublicationEmailSender;
            this._storeInformationSettings = storeInformationSettings;
		}

		#endregion 
        
        #region News items

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var news = _newsService.GetNews();
            var gridModel = new GridModel<NewsItemModel>
            {
                Data = news.Select(x =>
                {
                    var m = x.ToModel();
                    string miniSiteString = ((x.ExtendedProfileOnly & (int)NewsDisplayEnum.Both) == (int)NewsDisplayEnum.Both ? _localizationService.GetResource("News.Manage.MiniSite.Location.Both"): "") +
                                            ((x.ExtendedProfileOnly & (int)NewsDisplayEnum.MiniSite) == (int)NewsDisplayEnum.MiniSite ? _localizationService.GetResource("News.Manage.MiniSite.Location.MiniSite") : "") +
                                             ((x.ExtendedProfileOnly & (int)NewsDisplayEnum.Main) == (int)NewsDisplayEnum.Main ? _storeInformationSettings.StoreName : "");
                    m.MiniSiteString = miniSiteString;
                    if (x.StartDateUtc.HasValue)
                        m.StartDate = _dateTimeHelper.ConvertToUserTime(x.StartDateUtc.Value, DateTimeKind.Utc);
                    if (x.EndDateUtc.HasValue)
                        m.EndDate = _dateTimeHelper.ConvertToUserTime(x.EndDateUtc.Value, DateTimeKind.Utc);
                    m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    m.LanguageName = x.Language.Name;
                    m.Comments = x.ApprovedCommentCount + x.NotApprovedCommentCount;
                    if (x.Customer != null)
                    {
                        m.Company = x.Customer.CompanyInformation.GetLocalized(y => y.CompanyName, x.LanguageId);
                        if (m.Company == null)
                        {
                            m.Company = _localizationService.GetResource("Admin.Company.LocaleUndefined",_workContext.WorkingLanguage.Id);
                        }
                    }
                    else
                    {
                        m.Company = null;
                    }
                    return m;
                }).OrderByDescending(x=>x.CreatedOn),
                Total = news.Count
            };

            return View(gridModel);
        }

        [HttpPost, GridAction]
        public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //var news = _newsService.GetAllNews(0, command.Page - 1, command.PageSize, true);
            var news = _newsService.GetNews();
            var gridModel = new GridModel<NewsItemModel>
            {
                Data = news.Select(x =>
                {
                    var m = x.ToModel();
                    string miniSiteString = ((x.ExtendedProfileOnly & (int)NewsDisplayEnum.Both) == (int)NewsDisplayEnum.Both ? _localizationService.GetResource("News.Manage.MiniSite.Location.Both") : "") +
                                            ((x.ExtendedProfileOnly & (int)NewsDisplayEnum.MiniSite) == (int)NewsDisplayEnum.MiniSite ? _localizationService.GetResource("News.Manage.MiniSite.Location.MiniSite") : "") +
                                             ((x.ExtendedProfileOnly & (int)NewsDisplayEnum.Main) == (int)NewsDisplayEnum.Main ? _storeInformationSettings.StoreName : "");
                    m.MiniSiteString = miniSiteString;
                    if (x.StartDateUtc.HasValue)
                        m.StartDate = _dateTimeHelper.ConvertToUserTime(x.StartDateUtc.Value, DateTimeKind.Utc);
                    if (x.EndDateUtc.HasValue)
                        m.EndDate = _dateTimeHelper.ConvertToUserTime(x.EndDateUtc.Value, DateTimeKind.Utc);
                    m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    m.LanguageName = x.Language.Name;
                    m.Comments = x.ApprovedCommentCount + x.NotApprovedCommentCount;
                    if (x.Customer != null)
                    {
                        m.Company = x.Customer.CompanyInformation.GetLocalized(y => y.CompanyName, x.LanguageId);
                        if (m.Company == null)
                        {
                            m.Company = _localizationService.GetResource("Admin.Company.LocaleUndefined", _workContext.WorkingLanguage.Id);
                        }
                    }
                    else
                    {
                        m.Company = _localizationService.GetResource("Admin.News.DefaultCompany",_workContext.WorkingLanguage.Id);
                    }
                    return m;
                }).ToList(),
                Total = news.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);
            var model = new NewsItemModel();
            //default values
            model.Published = true;
            model.AllowComments = true;
            model.Company = "Industrial";
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(NewsItemModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var newsItem = model.ToEntity();
                newsItem.StartDateUtc = DateTime.UtcNow.AddMinutes(-1);
                newsItem.EndDateUtc = DateTime.MaxValue;
                newsItem.CreatedOnUtc = DateTime.UtcNow;
                if (model.CompanyId != 0)
                {
                    var sellers = _customerService.GetCustomersByCustomerRoleId(_customerService.GetCustomerRoleBySystemName("Sellers").Id);
                    var seller = sellers.Where(x => x.CompanyInformationId == model.CompanyId).FirstOrDefault();
                    newsItem.Customer = seller;
                }
                if (model.Published)
                {
                    newsItem.PublishingDate = DateTime.UtcNow;
                }
                newsItem.ExtendedProfileOnly = (int)NewsDisplayEnum.Main;
                _newsService.InsertNews(newsItem);
                
                //search engine name
                var seName = newsItem.ValidateSeName(model.SeName, model.Title, true);
                _urlRecordService.SaveSlug(newsItem, seName, newsItem.LanguageId);
                _genericAttributeService.SaveAttribute<int>(newsItem, SystemCustomerAttributeNames.PictureId, model.PictureId);
                _genericAttributeService.SaveAttribute<int>(newsItem, SystemCustomerAttributeNames.PictureCatalogId, model.CatalogPictureId);
                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = newsItem.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var newsItem = _newsService.GetNewsById(id);
            if (newsItem == null)
                //No news item found with the specified id
                return RedirectToAction("List");

            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);
            var model = newsItem.ToModel();
            model.CompanyId = newsItem.Customer == null ? 0 : newsItem.Customer.CompanyInformationId.Value;
            model.PublishingDate = newsItem.PublishingDate;
            model.StartDate = newsItem.StartDateUtc;
            model.EndDate = newsItem.EndDateUtc;
            model.PictureId = newsItem.GetAttribute<int>(SystemCustomerAttributeNames.PictureId);
            model.CatalogPictureId = newsItem.GetAttribute<int>(SystemCustomerAttributeNames.PictureCatalogId);
            return View(model);
        }

        public ActionResult Translate(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var newsItem = _newsService.GetNewsById(id);
            var model = newsItem.ToModel();
            model.Short = null;
            model.Full = null;
            model.SeName = null;
            model.Title = null;
            model.StartDate = newsItem.StartDateUtc;
            model.EndDate = newsItem.EndDateUtc;
            model.CreatedOn = newsItem.CreatedOnUtc;
            model.Id = 0;
            if(newsItem.Customer != null)
                model.CompanyId = newsItem.Customer.CompanyInformation.Id;
            model.PublishingDate = newsItem.PublishingDate;
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);
            model.PictureId = newsItem.GetAttribute<int>(SystemCustomerAttributeNames.PictureId);
            model.CatalogPictureId = newsItem.GetAttribute<int>(SystemCustomerAttributeNames.PictureCatalogId);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Translate(NewsItemModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (model.Published)
                {
                    if (model.PublishingDate == null)
                    {
                        model.PublishingDate = DateTime.UtcNow;
                    }
                }
                var newsItem = model.ToEntity();
                newsItem.StartDateUtc = model.StartDate;
                newsItem.EndDateUtc = model.EndDate;
                newsItem.CreatedOnUtc = model.CreatedOn;
                if (model.CompanyId != 0)
                {
                    var sellers = _customerService.GetCustomersByCustomerRoleId(_customerService.GetCustomerRoleBySystemName("Sellers").Id);
                    var seller = sellers.Where(x => x.CompanyInformationId == model.CompanyId).FirstOrDefault();
                    newsItem.Customer = seller;
                }

                _newsService.InsertNews(newsItem);

                //search engine name
                var seName = newsItem.ValidateSeName(model.SeName, model.Title, true);
                _urlRecordService.SaveSlug(newsItem, seName, newsItem.LanguageId);
                _genericAttributeService.SaveAttribute<int>(newsItem, SystemCustomerAttributeNames.PictureId, model.PictureId);
                _genericAttributeService.SaveAttribute<int>(newsItem, SystemCustomerAttributeNames.PictureCatalogId, model.CatalogPictureId);
                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Added"));
                if (!continueEditing)
                {
                    return RedirectToAction("List");
                }
                else
                {
                    return RedirectToAction("Edit", new { id = newsItem.Id });
                }
            }

            //If we got this far, something failed, redisplay form
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(NewsItemModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var newsItem = _newsService.GetNewsById(model.Id);
            if (newsItem == null)
                //No news item found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                bool featured = newsItem.Featured;
                bool published = newsItem.Published;
                model.StartDate = newsItem.StartDateUtc;
                model.EndDate = newsItem.EndDateUtc;
                model.PublishingDate = newsItem.PublishingDate;
                newsItem = model.ToEntity(newsItem);
                newsItem.Featured = featured;
                newsItem.StartDateUtc = model.StartDate;
                newsItem.EndDateUtc = model.EndDate;

                if (!model.Published)
                {
                    newsItem.Published = false;
                }
                else
                {
                    if (model.PublishingDate == null)
                    {
                        newsItem.PublishingDate = DateTime.UtcNow;
                        if (newsItem.Customer != null && !published)
                        {
                            _newPublicationEmailSender.SendNewsPublicationNotification(newsItem);
                        }
                    }
                    else
                    {
                        if (newsItem.Customer != null && !published)
                        {
                            _newPublicationEmailSender.SendNewsPublicationNotification(newsItem);
                        }
                    }
                    newsItem.Published = true;
                }
                _urlRecordService.ClearEntitySlug(newsItem, newsItem.LanguageId);
                _newsService.UpdateNews(newsItem);
                //search engine name
                var seName = newsItem.ValidateSeName(model.SeName, model.Title, true);
                _urlRecordService.SaveSlug(newsItem, seName, newsItem.LanguageId);
                _genericAttributeService.SaveAttribute<int>(newsItem, SystemCustomerAttributeNames.PictureId, model.PictureId);
                _genericAttributeService.SaveAttribute<int>(newsItem, SystemCustomerAttributeNames.PictureCatalogId, model.CatalogPictureId);
                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = newsItem.Id }) : RedirectToAction("List");
            }
            model.CompanyId = newsItem.Customer == null ? 0 : newsItem.Customer.CompanyInformationId.Value;
            //If we got this far, something failed, redisplay form
            ViewBag.AllLanguages = _languageService.GetAllLanguages(true);
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var newsItem = _newsService.GetNewsById(id);
            if (newsItem == null)
                //No news item found with the specified id
                return RedirectToAction("List");

            _urlRecordService.ClearEntitySlug(newsItem, 0);
            _newsService.DeleteNews(newsItem);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Deleted"));
            return RedirectToAction("List");
        }

        public ActionResult Feature(int id)
        {
            var newNewItem = _newsService.GetNewsById(id);
            if ((newNewItem.ExtendedProfileOnly & (int)NewsDisplayEnum.MiniSite) == (int)NewsDisplayEnum.MiniSite)
            {
                return RedirectToAction("List");
            }
            var newItem = _newsService.GetFeaturedNew(newNewItem.LanguageId);
            if (newItem != null)
            {
                newItem.Featured = false;
                _newsService.UpdateNews(newItem);
            }
            newNewItem.Featured = true;
            newNewItem.Published = true;
            if (!newNewItem.PublishingDate.HasValue)
                newNewItem.PublishingDate = DateTime.UtcNow.AddMinutes(-1);
            _newsService.UpdateNews(newNewItem);

            return RedirectToAction("List");
        }
        #endregion

        #region Comments

        public ActionResult Comments(int? filterByNewsItemId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            ViewBag.FilterByNewsItemId = filterByNewsItemId;
            var model = new GridModel<NewsCommentModel>();
            return View(model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult Comments(int? filterByNewsItemId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            IList<NewsComment> comments;
            if (filterByNewsItemId.HasValue)
            {
                //filter comments by news item
                var newsItem = _newsService.GetNewsById(filterByNewsItemId.Value);
                comments = newsItem.NewsComments.OrderBy(bc => bc.CreatedOnUtc).ToList();
            }
            else
            {
                //load all news comments
                comments = _customerContentService.GetAllCustomerContent<NewsComment>(0, null);
            }

            var gridModel = new GridModel<NewsCommentModel>
            {
                Data = comments.PagedForCommand(command).Select(newsComment =>
                {
                    var commentModel = new NewsCommentModel();
                    commentModel.Id = newsComment.Id;
                    commentModel.NewsItemId = newsComment.NewsItemId;
                    commentModel.NewsItemTitle = newsComment.NewsItem.Title;
                    commentModel.CustomerId = newsComment.CustomerId;
                    commentModel.IpAddress = newsComment.IpAddress;
                    commentModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(newsComment.CreatedOnUtc, DateTimeKind.Utc);
                    commentModel.CommentTitle = newsComment.CommentTitle;
                    commentModel.CommentText = Core.Html.HtmlHelper.FormatText(newsComment.CommentText, false, true, false, false, false, false);
                    return commentModel;
                }),
                Total = comments.Count,
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult CommentDelete(int? filterByNewsItemId, int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var comment = _customerContentService.GetCustomerContentById(id) as NewsComment;
            if (comment == null)
                throw new ArgumentException("No comment found with the specified id");

            var newsItem = comment.NewsItem;
            _customerContentService.DeleteCustomerContent(comment);
            //update totals
            _newsService.UpdateCommentTotals(newsItem);

            return Comments(filterByNewsItemId, command);
        }


        #endregion
    }
}
