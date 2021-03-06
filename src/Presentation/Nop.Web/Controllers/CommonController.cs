﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Web.Extensions;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI.Captcha;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using Nop.Web.Models.Topics;
using Nop.Web.Models.Banner;
using Nop.Services.BannerService;
using Nop.Services.Media;
using Nop.Admin.Models.Catalog;

namespace Nop.Web.Controllers
{
    public partial class CommonController : BaseNopController
    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ITopicService _topicService;
        private readonly ILanguageService _languageService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ISitemapGenerator _sitemapGenerator;
        private readonly IThemeContext _themeContext;
        private readonly IThemeProvider _themeProvider;
        private readonly IForumService _forumservice;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;
        private readonly IMobileDeviceHelper _mobileDeviceHelper;
        private readonly HttpContextBase _httpContext;
        private readonly ICacheManager _cacheManager;
        private readonly ICustomerActivityService _customerActivityService;

        private readonly CustomerSettings _customerSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly CommonSettings _commonSettings;
        private readonly BlogSettings _blogSettings;
        private readonly NewsSettings _newsSettings;
        private readonly ForumSettings _forumSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly IBannerService _bannerService;
        private readonly IPictureService _pictureService;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;

        #endregion

        #region Constructors

        public CommonController(ICategoryService categoryService, IProductService productService,
            IManufacturerService manufacturerService, ITopicService topicService,
            ILanguageService languageService,
            ICurrencyService currencyService, ILocalizationService localizationService,
            IWorkContext workContext,
            IQueuedEmailService queuedEmailService, IEmailAccountService emailAccountService,
            ISitemapGenerator sitemapGenerator, IThemeContext themeContext,
            IThemeProvider themeProvider, IForumService forumService,
            IGenericAttributeService genericAttributeService, IWebHelper webHelper,
            IPermissionService permissionService, IMobileDeviceHelper mobileDeviceHelper,
            HttpContextBase httpContext, ICacheManager cacheManager,
            ICustomerActivityService customerActivityService, CustomerSettings customerSettings,
            TaxSettings taxSettings, CatalogSettings catalogSettings,
            StoreInformationSettings storeInformationSettings, EmailAccountSettings emailAccountSettings,
            CommonSettings commonSettings, BlogSettings blogSettings,
            NewsSettings newsSettings, ForumSettings forumSettings,
            LocalizationSettings localizationSettings, CaptchaSettings captchaSettings,
            IBannerService bannerService,
             IRecentlyViewedProductsService recentlyViewedProductsService,
            IPictureService pictureService)
        {
            this._categoryService = categoryService;
            this._productService = productService;
            this._manufacturerService = manufacturerService;
            this._topicService = topicService;
            this._languageService = languageService;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountService = emailAccountService;
            this._sitemapGenerator = sitemapGenerator;
            this._themeContext = themeContext;
            this._themeProvider = themeProvider;
            this._forumservice = forumService;
            this._genericAttributeService = genericAttributeService;
            this._webHelper = webHelper;
            this._permissionService = permissionService;
            this._mobileDeviceHelper = mobileDeviceHelper;
            this._httpContext = httpContext;
            this._cacheManager = cacheManager;
            this._customerActivityService = customerActivityService;
            this._bannerService = bannerService;
            this._pictureService = pictureService;

            this._customerSettings = customerSettings;
            this._taxSettings = taxSettings;
            this._catalogSettings = catalogSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._emailAccountSettings = emailAccountSettings;
            this._commonSettings = commonSettings;
            this._blogSettings = blogSettings;
            this._newsSettings = newsSettings;
            this._forumSettings = forumSettings;
            this._localizationSettings = localizationSettings;
            this._captchaSettings = captchaSettings;
            this._recentlyViewedProductsService = recentlyViewedProductsService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected LanguageSelectorModel PrepareLanguageSelectorModel()
        {
            var availableLanguages = _cacheManager.Get(ModelCacheEventConsumer.AVAILABLE_LANGUAGES_MODEL_KEY, () =>
            {
                var result = _languageService
                    .GetAllLanguages()
                    .Select(x => new LanguageModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        FlagImageFileName = x.FlagImageFileName,
                    })
                    .ToList();
                return result;
            });

            var model = new LanguageSelectorModel()
            {
                CurrentLanguageId = _workContext.WorkingLanguage.Id,
                AvailableLanguages = availableLanguages,
                UseImages = _localizationSettings.UseImagesForLanguageSelection
            };
            return model;
        }

        [NonAction]
        protected CurrencySelectorModel PrepareCurrencySelectorModel()
        {
            var availableCurrencies = _cacheManager.Get(string.Format(ModelCacheEventConsumer.AVAILABLE_CURRENCIES_MODEL_KEY, _workContext.WorkingLanguage.Id), () =>
            {
                var result = _currencyService
                    .GetAllCurrencies()
                    .Select(x => new CurrencyModel()
                    {
                        Id = x.Id,
                        Name = x.GetLocalized(y => y.Name),
                    })
                    .ToList();
                return result;
            });

            var model = new CurrencySelectorModel()
            {
                CurrentCurrencyId = _workContext.WorkingCurrency.Id,
                AvailableCurrencies = availableCurrencies
            };
            return model;
        }

        [NonAction]
        protected TaxTypeSelectorModel PrepareTaxTypeSelectorModel()
        {
            var model = new TaxTypeSelectorModel()
            {
                Enabled = _taxSettings.AllowCustomersToSelectTaxDisplayType,
                CurrentTaxType = _workContext.TaxDisplayType
            };
            return model;
        }

        [NonAction]
        protected int GetUnreadPrivateMessages()
        {
            var result = 0;
            var customer = _workContext.CurrentCustomer;
            if (_forumSettings.AllowPrivateMessages && !customer.IsGuest())
            {
                var privateMessages = _forumservice.GetAllPrivateMessages(0, customer.Id, false, null, false, string.Empty, 0, 1);

                if (privateMessages.TotalCount > 0)
                {
                    result = privateMessages.TotalCount;
                }
            }

            return result;
        }

        #endregion

        #region Methods

        public ActionResult BannerList(BannerTypeEnum bannerType, int numberOfBanners, int? CategoryId)
        {
            var banners = GetRandomBanners(numberOfBanners, bannerType, CategoryId);
            var model = new List<BannerModel>();
            foreach (var banner in banners)
            {
                var bannerModel = new BannerModel();
                bannerModel.Id = banner.Id;
                bannerModel.BannerType = banner.BannerType;
                bannerModel.DisplayOnHomePage = banner.DisplayOnMain;
                int picId = banner.PictureId;
                bannerModel.AltText = banner.Alt;
                bannerModel.NetBanner = banner.NetBanner;
                bannerModel.TitleText = banner.Title;
                bannerModel.ImageUrl = _pictureService.GetPictureUrl(picId, showDefaultPicture: false);
                bannerModel.Url = banner.Url;

                _customerActivityService.InsertActivity("PublicStore.BannerView", _localizationService.GetResource("ITBSFA.Banner.View"),_workContext.CurrentCustomer,Request.Url.ToString() ,Request.UrlReferrer== null ? "" : Request.UrlReferrer.ToString(), bannerModel.Id, bannerModel.Id);
                model.Add(bannerModel);
            }
            return View(model);
        }

        private List<Banner> GetRandomBanners(int numberOfBanners,BannerTypeEnum bannerType, int? categoryId = null)
        {
            var allBanners = _bannerService.GetBannersByType(bannerType,false).ToList();
            if (categoryId.HasValue)
                allBanners = allBanners.Where(x => x.CategoryId == categoryId).ToList();
            if (allBanners.Count() <= numberOfBanners)
            {
                return allBanners;
            }
            else
            {
                var resultBanners = new List<Banner>();
                for (int i = 0; i < numberOfBanners; i++)
                {
                    Random rnd = new Random(DateTime.UtcNow.Millisecond);
                    int number = rnd.Next(1000);
                    resultBanners.Add(allBanners[number % (allBanners.Count())]);
                    allBanners.Remove(allBanners[number % (allBanners.Count())]);
                }
                return resultBanners;
            }
        }

        /// <summary>
        /// Banner click activity logging
        /// </summary>
        /// <param name="bannerId">banner id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BannerClick(int bannerId, string currentUrl)
        {
            var banner = _bannerService.GetBannerById(bannerId);
            if (banner != null)
            {
                _customerActivityService.InsertActivity("PublicStore.BannerClick", _localizationService.GetResource("ITBSFA.Banner.Click"), _workContext.CurrentCustomer, currentUrl, Request.UrlReferrer == null ? "" : Request.UrlReferrer.ToString(), bannerId, bannerId);
            }

            return new JsonResult();
        }

        private IList<SmCategoryModel> PrepareCategoriesList(Category parentCategory)
        {
            var categoriesList = new List<SmCategoryModel>();
            var categoryModel = new SmCategoryModel()
            {
                SeName = parentCategory.GetSeName(_workContext.WorkingLanguage.Id),
                Title = parentCategory.GetLocalized(x=>x.Name,_workContext.WorkingLanguage.Id,false)
            };
            var subCategories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategory.Id);
            categoryModel.SubCategories = new List<SmCategoryModel>();
            if (subCategories.Count > 0)
            {
                foreach (var subCategorie in subCategories)
                {
                    ((List<SmCategoryModel>)categoryModel.SubCategories).AddRange(PrepareCategoriesList(subCategorie));
                }
            }
            categoriesList.Add(categoryModel);

            return categoriesList;
        }
        /// <summary>
        /// Displays recently added products
        /// </summary>
        /// <param name="count">Count of items to take</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RecentlyAddedProducts(int count)
        {
            var products = _productService.NewSearchProducts(0, (int)ProductItemTypeEnum.Product, 0, ProductSortingEnum.CreatedOn, 0, count, 0, 0, _workContext.WorkingLanguage.Id);
            IEnumerable<ProductModel> model = new List<ProductModel>();
            if (products.Count() > 0)
            {
                model = from i in products
                        select new ProductModel()
                        {
                            Id = i.Id,
                            Name = i.Name,
                            MetaTitle = i.MetaTitle,
                            PictureThumbnailUrl = _pictureService.GetPictureUrl(i.ProductPictures.OrderBy(p => p.DisplayOrder).FirstOrDefault() != null ? i.ProductPictures.OrderBy(p => p.DisplayOrder).FirstOrDefault().PictureId : 0,showDefaultPicture:false, targetSize:100),
                            ShortDescription = i.ShortDescription,
                            SeName = i.GetSeName(),
                        };
            }
            return View(model);
        }
        /// <summary>
        /// Displays recently viewed items
        /// </summary>
        /// <param name="count">Count of items to take</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RecentlyViewedItems(int count)
        {

            //get recently viewed
            IList<Product> products = _recentlyViewedProductsService.GetRecentlyViewedProducts(count);
            IEnumerable<ProductModel> model = new List<ProductModel>();
            if (products.Count() > 0)
            {
                model = from i in products
                        where i.GetLocalized(x=>x.Name,_workContext.WorkingLanguage.Id,false) != null
                        select new ProductModel()
                        {
                            Id = i.Id,
                            Name = i.Name,
                            MetaTitle = i.MetaTitle,
                            PictureThumbnailUrl = _pictureService.GetPictureUrl(i.ProductPictures.OrderBy(p => p.DisplayOrder).FirstOrDefault() != null ? i.ProductPictures.OrderBy(p => p.DisplayOrder).FirstOrDefault().PictureId : 0,showDefaultPicture:false, targetSize:100),
                            ShortDescription = i.GetLocalized(p=>p.ShortDescription,_workContext.WorkingLanguage.Id,false),
                        };
            }

            return View(model);

        }



        /// <summary>
        /// <summary>
        /// Displays recently added products
        /// </summary>
        /// <param name="count">Count of items to take</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult HomePageOffersSlideBlock(int count, bool partialView=false, string categoryId=null)
        {
            var categories = _categoryService.GetAllCategories();

            var model = new OffersModel();
            model.Categories = new List<OffersCategoryModel>();
            foreach (var category in categories)
            {
                var categoryModel = new OffersCategoryModel();
                categoryModel.CateogyTitle = category.Name;
                categoryModel.CategoryId = category.Id;
                model.Categories.Add(categoryModel);
            }

            var products = _productService.SearchProductsWithAttributes(0, ProductSortingEnum.CreatedOn, 0, count, 0, 0, 0, 0, 0, null, null);
            
            if (!string.IsNullOrEmpty(categoryId))
            {
                products = _productService.SearchProductsWithAttributes(0, ProductSortingEnum.CreatedOn, 0, count, 0, Convert.ToInt32(categoryId), 0, 0, 0, null, null);
            }
            model.Products = new List<OffersProductModel>();
            if (products.Any())
            {
                model.Products = new List<OffersProductModel>();
                foreach (var i in products)
                {
                    var productModel = new OffersProductModel()
                    {
                        Id = i.Id,
                        Rating = Math.Round(i.Rating ?? 0),
                        BankRating = Math.Round(i.Customer.Rating ?? 0),
                        Name = i.Name,
                        ProductAttributeValue = i.ProductAttributes.FirstOrDefault(x => x.CategoryProductAttribute.ProductBoxAttribute),
                        MetaTitle = i.MetaTitle,
                        PictureThumbnailUrl =
                            _pictureService.GetPictureUrl(i.Customer.ProviderLogoId.GetValueOrDefault(), 100, false),
                        ShortDescription = i.ShortDescription,
                        FullDescription = i.FullDescription,
                        SeName = i.GetSeName(),
                        OrderingLink = i.OrderLink,
                    };
                    model.Products.Add(productModel);
                }
            }
            if (partialView)
            {
                return PartialView("OffersSlideBox", model);
            }
            else
            {
                return View(model);
            }
        }



       
        //language
        [ChildActionOnly]
        public ActionResult LanguageSelector()
        {
            var model = PrepareLanguageSelectorModel();
            return PartialView(model);
        }
        public ActionResult SetLanguage(int langid, string returnUrl = "")
        {
            if (_workContext.CurrentMiniSite != null)
            {
                if (langid > 2)
                    langid = 1;
            }
            var language = _languageService.GetLanguageById(langid);
            if (language != null && language.Published)
            {
                _workContext.WorkingLanguage = language;
                //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language.LanguageCulture);
            }

            
            //url referrer
            if (String.IsNullOrEmpty(returnUrl))
            {
                returnUrl = _webHelper.GetUrlReferrer();
            }

            //home page
            if (_workContext.CurrentMiniSite == null)
            {
                if (String.IsNullOrEmpty(returnUrl))
                    returnUrl = Url.RouteUrl("HomePage");
            }
            else
            {
                if (String.IsNullOrEmpty(returnUrl))
                    returnUrl = Url.RouteUrl("MiniSiteHomePage");
            }
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                string applicationPath = HttpContext.Request.ApplicationPath;
                if (returnUrl.IsLocalizedUrl(applicationPath, true))
                {
                    //already localized URL
                    returnUrl = returnUrl.RemoveLanguageSeoCodeFromRawUrl(applicationPath);
                }
                returnUrl = returnUrl.AddLanguageSeoCodeToRawUrl(applicationPath, _workContext.WorkingLanguage);
            }
            return Redirect(returnUrl);
        }

        //currency
        [ChildActionOnly]
        public ActionResult CurrencySelector()
        {
            var model = PrepareCurrencySelectorModel();
            return PartialView(model);
        }
        public ActionResult CurrencySelected(int customerCurrency, string returnUrl = "")
        {
            var currency = _currencyService.GetCurrencyById(customerCurrency);
            if (currency != null)
                _workContext.WorkingCurrency = currency;

            //url referrer
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = _webHelper.GetUrlReferrer();
            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");
            return Redirect(returnUrl);
        }

        //tax type
        [ChildActionOnly]
        public ActionResult TaxTypeSelector()
        {
            var model = PrepareTaxTypeSelectorModel();
            return PartialView(model);
        }
        public ActionResult TaxTypeSelected(int customerTaxType, string returnUrl = "")
        {
            var taxDisplayType = (TaxDisplayType)Enum.ToObject(typeof(TaxDisplayType), customerTaxType);
            _workContext.TaxDisplayType = taxDisplayType;

            //url referrer
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = _webHelper.GetUrlReferrer();
            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");
            return Redirect(returnUrl);
        }

        //Configuration page (used on mobile devices)
        [ChildActionOnly]
        public ActionResult ConfigButton()
        {
            var langModel = PrepareLanguageSelectorModel();
            var currModel = PrepareCurrencySelectorModel();
            var taxModel = PrepareTaxTypeSelectorModel();
            //should we display the button?
            if (langModel.AvailableLanguages.Count > 1 ||
                currModel.AvailableCurrencies.Count > 1 ||
                taxModel.Enabled)
                return PartialView();
            else
                return Content("");
        }
        public ActionResult Config()
        {
            return View();
        }

        //footer
        [ChildActionOnly]
        public ActionResult JavaScriptDisabledWarning()
        {
            if (!_commonSettings.DisplayJavaScriptDisabledWarning)
                return Content("");

            return PartialView();
        }

        //header links
        [ChildActionOnly]
        public ActionResult HeaderLinks()
        {
            var customer = _workContext.CurrentCustomer;

            var unreadMessageCount = GetUnreadPrivateMessages();
            var unreadMessage = string.Empty;
            var alertMessage = string.Empty;
            if (unreadMessageCount > 0)
            {
                unreadMessage = string.Format(_localizationService.GetResource("PrivateMessages.TotalUnread"), unreadMessageCount);

                //notifications here
                if (_forumSettings.ShowAlertForPM &&
                    !customer.GetAttribute<bool>(SystemCustomerAttributeNames.NotifiedAboutNewPrivateMessages))
                {
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.NotifiedAboutNewPrivateMessages, true);
                    alertMessage = string.Format(_localizationService.GetResource("PrivateMessages.YouHaveUnreadPM"), unreadMessageCount);
                }
            }

            var model = new HeaderLinksModel()
            {
                IsAuthenticated = customer.IsRegistered(),
                CustomerEmailUsername = customer.IsRegistered() ? (_customerSettings.UsernamesEnabled ? customer.Username : customer.Email) : "",
                ShoppingCartEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart),
                ShoppingCartItems = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList().GetTotalProducts(),
                WishlistEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableWishlist),
                WishlistItems = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).ToList().GetTotalProducts(),
                AllowPrivateMessages = _forumSettings.AllowPrivateMessages,
                UnreadPrivateMessages = unreadMessage,
                AlertMessage = alertMessage,
            };

            return PartialView(model);
        }
        [ChildActionOnly]
        public ActionResult AdminHeaderLinks()
        {
            var customer = _workContext.CurrentCustomer;

            var model = new AdminHeaderLinksModel()
            {
                ImpersonatedCustomerEmailUsername = customer.IsRegistered() ? (_customerSettings.UsernamesEnabled ? customer.Username : customer.Email) : "",
                IsCustomerImpersonated = _workContext.OriginalCustomerIfImpersonated != null,
                DisplayAdminLink = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel),
            };

            return PartialView(model);
        }

        //footer
        [ChildActionOnly]
        public ActionResult Footer()
        {
            var model = new FooterModel()
            {
                StoreName = _storeInformationSettings.StoreName
            };
            model.Topics = _topicService.GetFooterTopics().OrderBy(x=>x.Priority).Select(x => new TopicModel() { Title = x.MenuTitle, SystemName = x.SystemName }).ToList();
            return PartialView(model);
        }

        //Header menu
        [ChildActionOnly]
        public ActionResult HeaderMenu()
        {
            var model = new FooterModel()
            {
                StoreName = _storeInformationSettings.StoreName
            };
            model.Topics = _topicService.GetHeaderTopics().OrderBy(x=>x.Priority).Select(x => new TopicModel() { Title = x.MenuTitle, SystemName = x.SystemName }).ToList();
            return PartialView(model);
        }

        //menu
        [ChildActionOnly]
        public ActionResult Menu()
        {
            var model = new MenuModel()
            {
                RecentlyAddedProductsEnabled = _catalogSettings.RecentlyAddedProductsEnabled,
                BlogEnabled = _blogSettings.Enabled,
                ForumEnabled = _forumSettings.ForumsEnabled
            };

            return PartialView(model);
        }

        //info block
        [ChildActionOnly]
        public ActionResult InfoBlock()
        {
            var model = new InfoBlockModel()
            {
                RecentlyAddedProductsEnabled = _catalogSettings.RecentlyAddedProductsEnabled,
                RecentlyViewedProductsEnabled = _catalogSettings.RecentlyViewedProductsEnabled,
                CompareProductsEnabled = _catalogSettings.CompareProductsEnabled,
                BlogEnabled = _blogSettings.Enabled,
                NewsEnabled = _newsSettings.Enabled,
                SitemapEnabled = _commonSettings.SitemapEnabled,
                ForumEnabled = _forumSettings.ForumsEnabled,
                AllowPrivateMessages = _forumSettings.AllowPrivateMessages,
            };

            return PartialView(model);
        }

        //contact us page
        //[NopHttpsRequirement(SslRequirement.No)]
        public ActionResult ContactUs()
        {
            var model = new ContactUsModel()
            {
                Email = _workContext.CurrentCustomer.Email,
                Company = _workContext.CurrentCustomer.CompanyInformation == null ? null : _workContext.CurrentCustomer.CompanyInformation.CompanyName,
                DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage
            };
            return View(model);
        }
        [HttpPost, ActionName("ContactUs")]
        //[CaptchaValidator]
        public ActionResult ContactUsSend(ContactUsModel model, bool captchaValid = true)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptcha"));
            }

            if (ModelState.IsValid)
            {
                string email = model.Email.Trim();
                string fullName = model.FullName;
                string subject = string.Format(_localizationService.GetResource("ContactUs.EmailSubject"), _storeInformationSettings.StoreName);

                var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                if (emailAccount == null)
                    emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();

                string from = null;
                string fromName = null;
                string body = Core.Html.HtmlHelper.FormatText(model.Enquiry, false, true, false, false, false, false);
                //required for some SMTP servers
                if (_commonSettings.UseSystemEmailForContactUsForm)
                {
                    from = emailAccount.Email;
                    fromName = emailAccount.DisplayName;
                    body = string.Format("<strong>From</strong>: {0} - {1}<br />Company: {3} <br /><br />{2}",
                        Server.HtmlEncode(fullName),
                        Server.HtmlEncode(email), body, model.Company);
                }
                else
                {
                    from = email;
                    fromName = fullName;
                }
                var toEmail = _storeInformationSettings.ContactEmailAddress;
                _queuedEmailService.InsertQueuedEmail(new QueuedEmail()
                {
                    From = from,
                    FromName = fromName,
                    To = toEmail,
                    ToName = emailAccount.DisplayName,
                    Priority = 5,
                    Subject = subject,
                    Body = body,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id
                });

                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("ContactUs.YourEnquiryHasBeenSent");
                model.Company = null;
                model.Email = null;
                model.Enquiry = null;
                model.FullName = null;
                
                //activity log
                _customerActivityService.InsertActivity("PublicStore.ContactUs", _localizationService.GetResource("ActivityLog.PublicStore.ContactUs"));

                return View(model);
            }

            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage;
            return View(model);
        }

        //sitemap page
        [NopHttpsRequirement(SslRequirement.No)]
        public ActionResult Sitemap()
        {
            if (!_commonSettings.SitemapEnabled)
                return RedirectToRoute("HomePage");

            var model = new SitemapModel();
            if (_commonSettings.SitemapIncludeCategories)
            {
                var categories = _categoryService.GetAllCategoriesByParentCategoryId(0);
                foreach (var category in categories)
                {
                    ((List<SmCategoryModel>)model.Categories).AddRange(PrepareCategoriesList(category));
                }
            }
            return View(model);
        }

        //SEO sitemap page
        [NopHttpsRequirement(SslRequirement.No)]
        public ActionResult SitemapSeo()
        {
            if (!_commonSettings.SitemapEnabled)
                return RedirectToRoute("HomePage");

            string siteMap = _sitemapGenerator.Generate();
            return Content(siteMap, "text/xml");
        }

        //store theme
        [ChildActionOnly]
        public ActionResult StoreThemeSelector()
        {
            if (!_storeInformationSettings.AllowCustomerToSelectTheme)
                return Content("");

            var model = new StoreThemeSelectorModel();
            var currentTheme = _themeProvider.GetThemeConfiguration(_themeContext.WorkingDesktopTheme);
            model.CurrentStoreTheme = new StoreThemeModel()
            {
                Name = currentTheme.ThemeName,
                Title = currentTheme.ThemeTitle
            };
            model.AvailableStoreThemes = _themeProvider.GetThemeConfigurations()
                //do not display themes for mobile devices
                .Where(x => !x.MobileTheme)
                .Select(x =>
                {
                    return new StoreThemeModel()
                    {
                        Name = x.ThemeName,
                        Title = x.ThemeTitle
                    };
                })
                .ToList();
            return PartialView(model);
        }
        public ActionResult StoreThemeSelected(string themeName)
        {
            _themeContext.WorkingDesktopTheme = themeName;

            var model = new StoreThemeSelectorModel();
            var currentTheme = _themeProvider.GetThemeConfiguration(_themeContext.WorkingDesktopTheme);
            model.CurrentStoreTheme = new StoreThemeModel()
            {
                Name = currentTheme.ThemeName,
                Title = currentTheme.ThemeTitle
            };
            model.AvailableStoreThemes = _themeProvider.GetThemeConfigurations()
                //do not display themes for mobile devices
                .Where(x => !x.MobileTheme)
                .Select(x =>
                {
                    return new StoreThemeModel()
                    {
                        Name = x.ThemeName,
                        Title = x.ThemeTitle
                    };
                })
                .ToList();
            return PartialView("StoreThemeSelector", model);
        }

        //favicon
        [ChildActionOnly]
        public ActionResult Favicon()
        {
            var model = new FaviconModel()
            {
                Uploaded = System.IO.File.Exists(System.IO.Path.Combine(Request.PhysicalApplicationPath, "favicon.ico")),
                FaviconUrl = _webHelper.GetStoreLocation() + "favicon.ico"
            };

            return PartialView(model);
        }

        /// <summary>
        /// Change presentation layer (desktop or mobile version)
        /// </summary>
        /// <param name="dontUseMobileVersion">True - use desktop version; false - use version for mobile devices</param>
        /// <returns>Action result</returns>
        public ActionResult ChangeDevice(bool dontUseMobileVersion)
        {
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.DontUseMobileVersion, dontUseMobileVersion);

            string returnurl = _webHelper.GetUrlReferrer();
            if (String.IsNullOrEmpty(returnurl))
                returnurl = Url.RouteUrl("HomePage");
            return Redirect(returnurl);
        }
        [ChildActionOnly]
        public ActionResult ChangeDeviceBlock()
        {
            if (!_mobileDeviceHelper.MobileDevicesSupported())
                //mobile devices support is disabled
                return Content("");

            if (!_mobileDeviceHelper.IsMobileDevice(_httpContext))
                //request is made by a desktop computer
                return Content("");

            return View();
        }

        //EU Cookie law
        [ChildActionOnly]
        public ActionResult EuCookieLaw()
        {
            if (!_storeInformationSettings.DisplayEuCookieLawWarning)
                //disabled
                return Content("");

            if (_workContext.CurrentCustomer.GetAttribute<bool>("EuCookieLaw.Accepted"))
                //already accepted
                return Content("");

            return PartialView();
        }
        [HttpPost]
        public ActionResult EuCookieLawAccept()
        {
            if (!_storeInformationSettings.DisplayEuCookieLawWarning)
                //disabled
                return Json(new { stored = false });

            //save setting
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, "EuCookieLaw.Accepted", true);
            return Json(new { stored = true });
        }

        public ActionResult RobotsTextFile()
        {
            if (Request.Url.ToString().IndexOf(_storeInformationSettings.StoreUrl + "robots.txt") < 0)
            {
                throw new HttpException(404,"Not found");
            }
            var disallowPaths = new List<string>()
                                    {
                                        "/bin/",
                                        "/content/",
                                        "/download/",
                                        "/install",
                                        "/setproductreviewhelpfulness",
                                        "/Customer/PasswordRecovery‎",
                                    };
            var localizableDisallowPaths = new List<string>()
                                               {
                                                   "/addproducttocart",
                                                   "/addproductvarianttocart",
                                                   "/boards",
                                                   "/cart",
                                                   "/checkout",
                                                   "/clearcomparelist",
                                                   "/compareproducts",
                                                   "/customer/",
                                                   "/deletepm",
                                                   "/emailwishlist",
                                                   "/inboxupdate",
                                                   "/newsletter/subscriptionactivation",
                                                   "/onepagecheckout",
                                                   "/orderdetails",
                                                   "/passwordrecovery/confirm",
                                                   "/poll/vote",
                                                   "/privatemessages",
                                                   "/returnrequest",
                                                   "/sendpm",
                                                   "/sentupdate",
                                                   "/subscribenewsletter",
                                                   "/topic/authenticate",
                                                   "/viewpm",
                                                   "/wishlist",
                                                   "/feed",
                                                   "/myprofile",
                                                   "/managenews",
                                                   "/Favorit",
                                                   "/*My*",
                                               };


            const string newLine = "\r\n"; //Environment.NewLine
            var sb = new StringBuilder();
            sb.Append("User-agent: *");
            sb.Append(newLine);
            //usual paths
            foreach (var path in disallowPaths)
            {
                sb.AppendFormat("Disallow: {0}", path);
                sb.Append(newLine);
            }
            //localizable paths (without SEO code)
            foreach (var path in localizableDisallowPaths)
            {
                sb.AppendFormat("Disallow: {0}", path);
                sb.Append(newLine);
            }
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                //URLs are localizable. Append SEO code
                foreach (var language in _languageService.GetAllLanguages())
                {
                    foreach (var path in localizableDisallowPaths)
                    {
                        sb.AppendFormat("Disallow: {0}{1}", language.UniqueSeoCode, path);
                        sb.Append(newLine);
                    }
                }
            }
            sb.AppendLine();
            //sb.AppendLine("Sitemap: http://tradebel.com/en/sitemap.xml");
            Response.ContentType = "text/plain";
            Response.Write(sb.ToString());
            return null;
        }

        //public ActionResult SiteMap()
        //{
        //    var mode = new 
        //}

        public ActionResult GenericUrl()
        {
            //seems that no entity was found
            return RedirectToRoute("HomePage");
        }

        //store is closed
        public ActionResult StoreClosed()
        {
            return View();
        }

        #endregion
    }
}
