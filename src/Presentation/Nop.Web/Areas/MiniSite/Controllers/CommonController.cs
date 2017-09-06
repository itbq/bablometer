using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Messages;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Common;
using Nop.Web.Models.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Web.Extensions;
using Nop.Web.Models.MiniSite;
using Nop.Services.Media;
using Nop.Web.Areas.MiniSite.Models.Common;
using Nop.Services.Seo;
using Nop.Services.MiniSite;
using Nop.Web.Models.Media;
using Nop.Core.Domain.MiniSite;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Catalog;
using Nop.Web.Controllers;
using System.Net;
using Nop.Core.Domain.Customers;

namespace Nop.Web.Areas.MiniSite.Controllers
{
    public class CommonController : BaseNopController
    {
        private readonly ICacheManager _cacheManager;
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly CommonSettings _commonSettings;
        private readonly IAddressService _addressService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly AddressSettings _addressSettings;
        private readonly ICountryService _countryService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IMiniSiteTextPageService _miniSiteTextPageService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IWebHelper _webHelper;

        public CommonController(ICacheManager cacheManager,
            ILanguageService languageService,
            IWorkContext workContext,
            ILocalizationService localizationService,
            IEmailAccountService emailAccountService,
            IQueuedEmailService queuedEmailService,
            EmailAccountSettings emailAccountSettings,
            CommonSettings commonSettings,
            IAddressService addressService,
            IStateProvinceService stateProvinceService,
            AddressSettings addressSettings,
            ICountryService countryService,
            IPictureService pictureService,
            IUrlRecordService urlRecordService,
            IMiniSiteTextPageService miniSiteTextPageService,
            CatalogSettings catalogSettings,
            IWebHelper webHelper)
        {
            this._cacheManager = cacheManager;
            this._languageService = languageService;
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._emailAccountService = emailAccountService;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountSettings = emailAccountSettings;
            this._commonSettings = commonSettings;
            this._addressService = addressService;
            this._stateProvinceService = stateProvinceService;
            this._addressSettings = addressSettings;
            this._countryService = countryService;
            this._pictureService = pictureService;
            this._urlRecordService = urlRecordService;
            this._miniSiteTextPageService = miniSiteTextPageService;
            this._catalogSettings = catalogSettings;
            this._webHelper = webHelper;
        }


        [NonAction]
        protected LanguageSelectorModel PrepareLanguageSelectorModel()
        {
            var availableLanguages = _cacheManager.Get(ModelCacheEventConsumer.AVAILABLE_LANGUAGES_MODEL_KEY, () =>
            {
                var result = _languageService
                    .GetAllLanguages()
                    .Where(x => x.LanguageCulture == "en-US" || x.LanguageCulture == "ru-RU")
                    .Select(x => new LanguageModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        FlagImageFileName = x.FlagImageFileName,
                    })
                    .ToList();
                return result;
            });
            var currentLanguage = _workContext.WorkingLanguage;
            if (_workContext.WorkingLanguage.LanguageCulture != "en-US" && _workContext.WorkingLanguage.LanguageCulture != "ru-RU")
            {
                currentLanguage = _languageService.GetAllLanguages().Where(x => x.Id == 1).FirstOrDefault();
            }
            var model = new LanguageSelectorModel()
            {
                CurrentLanguageId = currentLanguage.Id,
                AvailableLanguages = availableLanguages,
                UseImages = true
            };
            return model;
        }

        [NonAction]
        protected IList<BannerMiniSite> GetRandomBanners(int languageId)
        {
            var allBanners = _workContext.CurrentMiniSite.Banners.Where(x => x.GetLocalized(b => b.BannerPictureId, languageId, false) != 0).ToList();
            int numberOfBanners = 3;
            if (allBanners.Count() <= numberOfBanners)
            {
                return allBanners;
            }
            else
            {
                var resultBanners = new List<BannerMiniSite>();
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

        public ActionResult Footer()
        {
            return View();
        }

        public ActionResult LanguageSelector()
        {
            var model = PrepareLanguageSelectorModel();
            return PartialView(model);
        }

        public ActionResult Contacts()
        {
            var model = new Nop.Web.Areas.MiniSite.Models.Common.ContactUsModel()
            {
                Email = null,
                Company = null,
                DisplayCaptcha = false
            };
            return View(model);
        }

        [HttpPost, ActionName("Contacts")]
        public ActionResult Contacts(Nop.Web.Areas.MiniSite.Models.Common.ContactUsModel model, bool captchaValid = true)
        {
            if (ModelState.IsValid)
            {
                string email = model.Email.Trim();
                string fullName = model.FullName;
                string StoreName = _workContext.CurrentMiniSite.MiniSiteLayout.RootTitle;
                string subject = string.Format(_localizationService.GetResource("ContactUs.EmailSubject"), StoreName);

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
                _queuedEmailService.InsertQueuedEmail(new QueuedEmail()
                {
                    From = from,
                    FromName = fromName,
                    To = _workContext.CurrentMiniSite.ContactEmail ?? _workContext.CurrentMiniSite.Customer.Email,
                    ToName = _workContext.CurrentMiniSite.Customer.Username,
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
                return View("Contacts", model);
            }
            return View("Contacts",model);
        }

        public ActionResult CompanyContacts()
        {
            var customer = _workContext.CurrentMiniSite.Customer;

            var model = new CustomerAddressListModel();
            foreach (var address in customer.Addresses)
            {
                if (address.Email == null)
                {
                    _addressService.DeleteAddress(address);
                    continue;
                }
                var addressModel = new AddressModel();
                addressModel.PrepareModel(address, false, _addressSettings, _localizationService,
                    _stateProvinceService, () => _countryService.GetAllCountries(), languageId: _workContext.WorkingLanguage.Id);
                model.Addresses.Add(addressModel);
            }

            return View(model);
        }


        public ActionResult AboutUsPage()
        {
            string pageHtml = _workContext.CurrentMiniSite.AboutUsPage;
            return View((object)pageHtml);
        }

        public ActionResult AboutProductPage()
        {
            string pageHtml = _workContext.CurrentMiniSite.AboutProductsPage;
            return View((object)pageHtml);
        }

        public ActionResult Banners()
        {
            var model = new List<MiniSiteBannerModel>();
            var banners = GetRandomBanners(_workContext.WorkingLanguage.Id);
            if(banners == null)
                return View(model);
            var langId = _workContext.WorkingLanguage.Id;
            foreach (var banner in banners)
            {
                var bannerModel = new MiniSiteBannerModel();
                bannerModel.AltTag = banner.GetLocalized(x => x.BannerAlt, langId, false);
                bannerModel.TitleTag = banner.GetLocalized(x => x.BannerTitle, langId, false);
                bannerModel.Url = banner.GetLocalized(x => x.BannerUrl, langId, false);
                bannerModel.PictureId = banner.GetLocalized(x => x.BannerPictureId, langId, false);
                if (bannerModel.PictureId != 0)
                {
                    bannerModel.Picture = new PictureModel()
                    {
                        ImageUrl = _pictureService.GetPictureUrl(bannerModel.PictureId, showDefaultPicture: false)
                    };
                }
                model.Add(bannerModel);
            }

            return View(model);
        }

        public ActionResult Menu()
        {
            var pages = _workContext.CurrentMiniSite.Textpages.Where(x=>x.GetLocalized(p=>p.MenuTitle,_workContext.WorkingLanguage.Id,false)!= null);
            var model = new List<MenuItemModel>();
            model = pages.OrderByDescending(x=>x.Id).Select(x => new MenuItemModel()
            {
                PageName = x.GetLocalized(p=>p.MenuTitle,_workContext.WorkingLanguage.Id),
                PageSeName = x.GetSeName(_workContext.WorkingLanguage.Id)
            }).ToList();
            return View(model);
        }

        public ActionResult FooterMenu()
        {
            var pages = _workContext.CurrentMiniSite.Textpages.Where(x => x.GetLocalized(p => p.MenuTitle, _workContext.WorkingLanguage.Id, false) != null);
            var model = new List<MenuItemModel>();
            model = pages.OrderByDescending(x => x.Id).Select(x => new MenuItemModel()
            {
                PageName = x.GetLocalized(p => p.MenuTitle, _workContext.WorkingLanguage.Id),
                PageSeName = x.GetSeName(_workContext.WorkingLanguage.Id)
            }).ToList();
            return View(model);
        }

        public ActionResult TextPage(string SeName)
        {
            var slug = _urlRecordService.GetBySlug(SeName);
            if (slug == null)
            {
                return Redirect("/");
            }

            var page = _miniSiteTextPageService.GetById(slug.EntityId);
            if (page == null)
            {
                return Redirect("/");
            }

            if (page.UserMiniSiteId != _workContext.CurrentMiniSite.Id)
            {
                throw new HttpException(404, "Not found");
            }

            var model = new TextPageModel();
            model.Header = page.GetLocalized(x => x.Title, _workContext.WorkingLanguage.Id);
            model.Html = page.GetLocalized(x => x.Html, _workContext.WorkingLanguage.Id);
            model.Title = page.GetLocalized(x => x.PageTitle, _workContext.WorkingLanguage.Id);

            return View(model);
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

        [ChildActionOnly]
        public PartialViewResult GoogleAnalytics()
        {
            string scriptValue = _workContext.CurrentMiniSite.GetAttribute<string>(SystemCustomerAttributeNames.Signature);
            
            return PartialView("GoogleAnalytics",scriptValue);
        }
    }
}
