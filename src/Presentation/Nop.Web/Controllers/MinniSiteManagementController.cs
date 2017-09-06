using Nop.Core;
using Nop.Core.Domain.MiniSite;
using Nop.Services.MiniSite;
using Nop.Web.Models.MiniSite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Services.Media;
using Nop.Services.Localization;
using Nop.Web.Models.Common;
using Nop.Services.Seo;
using Nop.Services.Common;
using Nop.Core.Domain;
using Nop.Services.Messages;
using Nop.Web.Models.Media;
using Nop.Web.Models.BuyingRequest;
using System.ServiceModel;
using System.Net;
using System.Text.RegularExpressions;

namespace Nop.Web.Controllers
{
    public class MiniSiteManagementController : Controller
    {
        private readonly IMiniSiteService _miniSiteService;
        private readonly IWorkContext _workContext;
        private readonly IPictureService _pictureService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILanguageService _languageService;
        private readonly IMiniSiteBannerService _miniSiteBannerService;
        private readonly IMiniSiteTextPageService _miniSiteTextPageService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ISliderItemService _sliderItemService;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly IMiniSiteEmailSender _miniSiteEmailSender;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;

        public MiniSiteManagementController(IMiniSiteService miniSiteService,
            IWorkContext workContext,
            IPictureService pictureService,
            ILocalizedEntityService localizedEntityService,
            ILanguageService languageService,
            IMiniSiteBannerService miniSiteBannerService,
            IMiniSiteTextPageService miniSiteTextPageService,
            IUrlRecordService urlRecordService,
            ILocalizedEntityService localizedEntityServer,
            ISliderItemService sliderItemService,
            StoreInformationSettings storeInformationSettings,
            IMiniSiteEmailSender miniSiteEmailSender,
            IWebHelper webHelper,
            ILocalizationService localizationService,
            IGenericAttributeService genericAttributeService)
        {
            this._miniSiteService = miniSiteService;
            this._workContext = workContext;
            this._pictureService = pictureService;
            this._localizedEntityService = localizedEntityService;
            this._languageService = languageService;
            this._miniSiteBannerService = miniSiteBannerService;
            this._miniSiteTextPageService = miniSiteTextPageService;
            this._urlRecordService = urlRecordService;
            this._localizedEntityService = localizedEntityService;
            this._sliderItemService = sliderItemService;
            this._storeInformationSettings = storeInformationSettings;
            this._miniSiteEmailSender = miniSiteEmailSender;
            this._webHelper = webHelper;
            this._localizationService = localizationService;
            this._genericAttributeService = genericAttributeService;
        }

        private void SetLanguage(int languageId, MiniSiteTextPagesModel txtpageModel)
        {
            txtpageModel.AviableLanguages = new List<LanguageModel>();
            var language = _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "ru").FirstOrDefault();
            var lang = _languageService.GetLanguageById(languageId);
            txtpageModel.CurrentLanguage = new LanguageModel()
            {
                Name = lang.Name,
                LanguageCulture = lang.LanguageCulture,
                Id = lang.Id
            };
            txtpageModel.AviableLanguages.Add(new LanguageModel()
            {
                Name = language.Name,
                LanguageCulture = language.LanguageCulture,
                Id = language.Id
            });

            language = _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "en").FirstOrDefault();
            txtpageModel.AviableLanguages.Add(new LanguageModel()
            {
                Name = language.Name,
                LanguageCulture = language.LanguageCulture,
                Id = language.Id
            });

            txtpageModel.SelectedLanguageId = languageId;
        }

        [NonAction]
        protected MiniSiteBannerModel PrepareBannerOverviewModel(BannerMiniSite banner, int languageId)
        {
            var bannerModel = new MiniSiteBannerModel();
            bannerModel.Id = banner.Id;
            bannerModel.Languages = new List<BuyingRequestLanguageModel>();
            foreach (var lang in _languageService.GetAllLanguages()
                .Where(x => x.UniqueSeoCode == "ru" || x.UniqueSeoCode == "en"))
            {
                if (banner.GetLocalized(x => x.BannerPictureId, lang.Id, false) != 0)
                {
                    var languageModel = new BuyingRequestLanguageModel()
                    {
                        LanguageId = lang.Id,
                        Selected = true,
                        LanguageName = lang.Name,
                        FlagImageUrl = lang.FlagImageFileName
                    };
                    bannerModel.Languages.Add(languageModel);
                }
                else
                {
                    var languageModel = new BuyingRequestLanguageModel()
                    {
                        LanguageId = lang.Id,
                        Selected = false,
                        LanguageName = lang.Name,
                        FlagImageUrl = lang.FlagImageFileName
                    };
                    bannerModel.Languages.Add(languageModel);
                }
            }
            bannerModel.AltTag = banner.GetLocalized(x => x.BannerAlt, languageId, false);
            bannerModel.TitleTag = banner.GetLocalized(x => x.BannerTitle, languageId, false);
            bannerModel.Url = banner.GetLocalized(x => x.BannerUrl, languageId, false);
            bannerModel.PictureId = banner.GetLocalized(x => x.BannerPictureId, languageId, false);
            if (bannerModel.PictureId != null)
            {
                bannerModel.Picture = new PictureModel()
                {
                    ImageUrl = _pictureService.GetPictureUrl(bannerModel.PictureId,showDefaultPicture:false)
                };
            }
            return bannerModel;
        }

        [NonAction]
        protected void SaveBannerModel(MiniSiteBannerModel model)
        {
            if (model.Id != 0)
            {
                var banner = _miniSiteBannerService.GetById(model.Id);
                int oldPictureId = banner.GetLocalized(x => x.BannerPictureId, model.WorkingLanguage, false);
                var equalpics = (banner.GetLocalized(x => x.BannerPictureId, _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "ru").First().Id, false) ==
                    banner.GetLocalized(x => x.BannerPictureId, _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "en").First().Id, false));
                if (oldPictureId != 0 && oldPictureId != model.PictureId && !equalpics)
                {
                    var oldPic = _pictureService.GetPictureById(oldPictureId);
                    if (oldPic != null)
                    {
                        _pictureService.DeletePicture(oldPic);
                    }
                }
                UpdateBannerLocale(banner,model, model.WorkingLanguage); 
            }
            else
            {
                var banner = new BannerMiniSite();
                banner.UserMiniSiteId = _workContext.CurrentCustomer.UserMiniSite.Id;
                _miniSiteBannerService.Insert(banner);
                foreach (var lang in model.Languages.Where(x=>x.Selected))
                {
                    UpdateBannerLocale(banner, model,lang.LanguageId);
                }
            }
        }

        [NonAction]
        protected void UpdateBannerLocale(BannerMiniSite banner, MiniSiteBannerModel model,int languageId)
        {
            _localizedEntityService.SaveLocalizedValue(banner, x => x.BannerTitle, model.TitleTag, languageId);
            _localizedEntityService.SaveLocalizedValue(banner, x => x.BannerAlt, model.AltTag, languageId);
            _localizedEntityService.SaveLocalizedValue(banner, x => x.BannerUrl, model.Url, languageId);

            _localizedEntityService.SaveLocalizedValue(banner, x => x.BannerPictureId, model.PictureId, languageId);
        }

        public ActionResult Add()
        {
            if (!_workContext.CurrentCustomer.IsSeller())
                return RedirectToRoute("HomePage");
            if (_workContext.CurrentCustomer.UserMiniSite != null)
                return RedirectToAction("MainSetup");
            var model = new MiniSiteActivationModel();
            string templatePath = Server.MapPath("~/Content/MiniSite/Templates");
            string[] templates = Directory.GetDirectories(templatePath);
            var names = templates.Select(x => x.Substring(x.LastIndexOf(@"\") + 1, x.Length - x.LastIndexOf(@"\") - 1)).ToList();
            model.AviableCssTemplate = names;
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(MiniSiteActivationModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return RedirectToRoute("HomePage");

            if (_workContext.CurrentCustomer.UserMiniSite != null && _workContext.CurrentCustomer.UserMiniSite.Active)
            {
                return RedirectToAction("MainSetup");
            }
            else
            {
                if(_workContext.CurrentCustomer.UserMiniSite != null)
                    return RedirectToAction("MyProfile", "Customer");
            }

            if (!String.IsNullOrEmpty(model.DomainName) && model.UseSecondLavelDomain)
            {
                if (!Regex.IsMatch(model.DomainName, @"^[a-z|A-Z|0-9][a-z|A-Z|0-9|-]+[a-z|A-Z|0-9]$"))
                {
                    ModelState.AddModelError("DomainName", _localizationService.GetResource("Etf.Domain.Invalid"));
                }
                //model.DomainName = model.DomainName.Replace("http:\\", "");
                //model.DomainName = model.DomainName.Replace("https:\\", "");
                //model.DomainName = model.DomainName.Replace("\\", "");
                //model.DomainName = model.DomainName.Replace("/", "");
            }

            if (!String.IsNullOrEmpty(model.OwnDomain) && !model.UseSecondLavelDomain)
            {
                if (!ValidateDomain(model.OwnDomain))
                {
                    ModelState.AddModelError("OwnDomain", _localizationService.GetResource("Etf.Domain.Invalid"));
                }
                //model.OwnDomain = model.OwnDomain.Replace("http:\\", "");
                //model.OwnDomain = model.OwnDomain.Replace("https:\\", "");
                //model.OwnDomain = model.OwnDomain.Replace("\\", "");
                //model.OwnDomain = model.OwnDomain.Replace("/", "");
            }
            string domainName = model.UseSecondLavelDomain ? model.DomainName + "." + (new Uri(_storeInformationSettings.StoreUrl).Host) : model.OwnDomain;
            if (!String.IsNullOrEmpty(domainName))
            {
                domainName = domainName.ToLower();
                var miniSite = _miniSiteService.GetMiniSiteByDomain(domainName);
                if (miniSite != null && miniSite.Customer.Id != _workContext.CurrentCustomer.Id)
                    ModelState.AddModelError("DomainName", "Minisite with specified domain name already exists");
            }

            if (ModelState.IsValid)
            {
                var UserMiniSite = new UserMiniSite();
                UserMiniSite.Active = false;
                UserMiniSite.DomainName = domainName.Replace("www.","");
                UserMiniSite.LayoutId = model.LayoutTemplateId;
                UserMiniSite.Customer = _workContext.CurrentCustomer;
                UserMiniSite.MiniSiteLayout = new MiniSiteLayout();
                UserMiniSite.MiniSiteLayout.StyleFolder = UserMiniSite.Customer.Username;
                UserMiniSite.MiniSiteLayout.CssTemplate = model.CssTemplateName;
                string MiniSitePath = Server.MapPath("~/Content/MiniSite");
                Directory.CreateDirectory(MiniSitePath + @"\" + UserMiniSite.Customer.Username);
                System.IO.File.Copy(MiniSitePath + @"\Templates\" + model.CssTemplateName + @"\images\logo.png", MiniSitePath + @"\" + UserMiniSite.Customer.Username + @"\logo.jpg",true);
                _miniSiteService.InsertMiniSite(UserMiniSite);

                _miniSiteEmailSender.SendMiniSiteActivationEmail(UserMiniSite.Id,_languageService.GetAllLanguages()
                    .Where(x=>x.UniqueSeoCode == "ru")
                    .FirstOrDefault().Id);

                model.Success = true;
            }
            string templatePath = Server.MapPath("~/Content/MiniSite/Templates");
            string[] templates = Directory.GetDirectories(templatePath);
            var names = templates.Select(x => x.Substring(x.LastIndexOf(@"\") + 1, x.Length - x.LastIndexOf(@"\") - 1)).ToList();
            model.AviableCssTemplate = names;
            return View(model);
        }

        public ActionResult MainSetup()
        {
            if(!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }
            if (_workContext.CurrentCustomer.UserMiniSite == null)
            {
                return RedirectToRoute("HomePage");
            }
            if (!_workContext.CurrentCustomer.UserMiniSite.Active)
            {
                return RedirectToAction("MyProfile", "Customer");
            }
            var model = new MiniSiteGeneralSetupModel();
            var miniSite = _workContext.CurrentCustomer.UserMiniSite;
            if (miniSite == null)
            {
                return RedirectToAction("Add");
            }

            model.LayoutId = miniSite.LayoutId;
            model.ContactEmail = miniSite.ContactEmail;
            model.CssTemplateName = miniSite.MiniSiteLayout.CssTemplate;
            model.UseSecondLavelDomain = miniSite.DomainName.IndexOf(new Uri(_storeInformationSettings.StoreUrl).Host) > 0 ? true : false;
            if (model.UseSecondLavelDomain)
            {
                model.DomainName = miniSite.DomainName.Replace("." + (new Uri(_storeInformationSettings.StoreUrl).Host),"");
            }
            else
            {
                model.OwnDomain = miniSite.DomainName;
            }
            model.Title = miniSite.MiniSiteLayout.GetLocalized(x => x.RootTitle, _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "en").FirstOrDefault().Id);
            model.TitleRus = miniSite.MiniSiteLayout.GetLocalized(x => x.RootTitle, _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "ru").FirstOrDefault().Id);
            model.LogoId = miniSite.LayoutId;
            model.LogoId = miniSite.LogoId;
            model.GoogleAnalytics = miniSite.GetAttribute<string>(SystemCustomerAttributeNames.Signature);
            string templatePath = Server.MapPath("~/Content/MiniSite/Templates");
            string[] templates = Directory.GetDirectories(templatePath);
            var names = templates.Select(x => x.Substring(x.LastIndexOf(@"\") + 1, x.Length - x.LastIndexOf(@"\") - 1)).ToList();
            model.AviableCssTemplates = names;

            return View(model);
        }

        [HttpPost]
        public ActionResult MainSetup(MiniSiteGeneralSetupModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }
            if (_workContext.CurrentCustomer.UserMiniSite == null)
            {
                return RedirectToRoute("HomePage");
            }

            if (!String.IsNullOrEmpty(model.DomainName) && model.UseSecondLavelDomain)
            {
                if (!Regex.IsMatch(model.DomainName, @"^[a-z|A-Z|0-9][a-z|A-Z|0-9|-]{1,61}[a-z|A-Z|0-9]$"))
                {
                    ModelState.AddModelError("DomainName", _localizationService.GetResource("Etf.Domain.Invalid"));
                }
                //model.DomainName = model.DomainName.Replace("http:\\", "");
                //model.DomainName = model.DomainName.Replace("https:\\", "");
                //model.DomainName = model.DomainName.Replace("\\", "");
                //model.DomainName = model.DomainName.Replace("/", "");
            }

            if (!String.IsNullOrEmpty(model.OwnDomain) && !model.UseSecondLavelDomain)
            {
                if (!ValidateDomain(model.OwnDomain))
                {
                    ModelState.AddModelError("OwnDomain", _localizationService.GetResource("Etf.Domain.Invalid"));
                }
                //model.OwnDomain = model.OwnDomain.Replace("http:\\", "");
                //model.OwnDomain = model.OwnDomain.Replace("https:\\", "");
                //model.OwnDomain = model.OwnDomain.Replace("\\", "");
                //model.OwnDomain = model.OwnDomain.Replace("/", "");
            }
            string storeUrl = (new Uri(_storeInformationSettings.StoreUrl).Host);
            string domainName = model.UseSecondLavelDomain ? model.DomainName +  "." + (new Uri(_storeInformationSettings.StoreUrl).Host) : model.OwnDomain;
            if (!String.IsNullOrEmpty(domainName) && domainName != _workContext.CurrentCustomer.UserMiniSite.DomainName)
            {
                domainName = domainName.ToLower();
                var miniSite = _miniSiteService.GetMiniSiteByDomain(domainName);
                if (miniSite != null && miniSite.Customer.Id != _workContext.CurrentCustomer.Id)
                    ModelState.AddModelError("DomainName", "Minisite with specified domain name already exists");
            }
            if (ModelState.IsValid)
            {
                var minisite = _workContext.CurrentCustomer.UserMiniSite;
                if (minisite.DomainName != domainName)
                {
                    if(minisite.Active)
                        try
                        {
                            _webHelper.DeleteWebSiteBinding(minisite.DomainName, storeUrl);
                        }
                        catch (FaultException ex)
                        {
                            if (ex.Message != "Binding not found!")
                            {
                                throw ex;
                            }
                        }
                    minisite.Active = false;
                    minisite.DomainName = domainName.Replace("www.","");
                }

                if (minisite.LogoId != model.LogoId)
                {
                    var logo = _pictureService.GetPictureById(model.LogoId);
                    string templatePath = Server.MapPath("~/Content/MiniSite/" + _workContext.CurrentCustomer.Username);
                    using (var fs = System.IO.File.OpenWrite(templatePath + @"\logo.jpg"))
                    {
                        if (logo != null)
                        {
                            var binary = _pictureService.LoadPictureBinary(logo);
                            fs.Write(binary, 0, binary.Length);
                            fs.Close();
                        }
                        else
                        {
                            fs.Write(new byte[1] { 0 }, 0, 1);
                        }
                    }

                    var pict = _pictureService.GetPictureById(minisite.LogoId);
                    if(pict != null)
                        _pictureService.DeletePicture(_pictureService.GetPictureById(minisite.LogoId));
                }
                minisite.LogoId = model.LogoId;
                minisite.LayoutId = model.LayoutId;
                _localizedEntityService.SaveLocalizedValue(minisite.MiniSiteLayout, x => x.RootTitle,model.Title,_languageService.GetAllLanguages().Where(x=>x.UniqueSeoCode == "en").FirstOrDefault().Id);
                _localizedEntityService.SaveLocalizedValue(minisite.MiniSiteLayout, x => x.RootTitle, model.TitleRus, _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "ru").FirstOrDefault().Id);
                minisite.MiniSiteLayout.CssTemplate = model.CssTemplateName;
                minisite.ContactEmail = model.ContactEmail;
                _miniSiteService.UpdateMiniSite(minisite);
                if (!String.IsNullOrEmpty(model.GoogleAnalytics))
                {
                    _genericAttributeService.SaveAttribute(minisite, SystemCustomerAttributeNames.Signature, model.GoogleAnalytics);
                }
                if (model.UseSecondLavelDomain)
                {
                    model.OwnDomain= null;
                }
                else
                {
                    model.DomainName = null;
                }
            }

            string tempPath = Server.MapPath("~/Content/MiniSite/Templates");
            string[] templates = Directory.GetDirectories(tempPath);
            var names = templates.Select(x => x.Substring(x.LastIndexOf(@"\") + 1, x.Length - x.LastIndexOf(@"\") - 1)).ToList();
            model.AviableCssTemplates = names;

            return View(model);
        }

        public ActionResult SliderSetup(int? languageId)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }

            if (_workContext.CurrentCustomer.UserMiniSite == null)
            {
                return RedirectToRoute("HomePage");
            }

            if (!languageId.HasValue)
            {
                languageId = _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "ru").FirstOrDefault().Id;
            }
            var model = new MiniSiteSliderModel();
            var slides = _workContext.CurrentCustomer.UserMiniSite.SliderItems;
            var sliderItem = slides.Where(x => x.SlideNumber == 1).FirstOrDefault();
            if (sliderItem != null)
            {
                model.Link1 = sliderItem.Url;
                model.Picture1Id = sliderItem.PictureId;
                model.Text1 = sliderItem.GetLocalized(x => x.ShortText, languageId.Value,false);
                model.Title1 = sliderItem.GetLocalized(x=>x.TitleText,languageId.Value,false);
            }

            sliderItem = slides.Where(x => x.SlideNumber == 2).FirstOrDefault();
            if (sliderItem != null)
            {
                model.Link2 = sliderItem.Url;
                model.Picture2Id = sliderItem.PictureId;
                model.Text2 = sliderItem.GetLocalized(x=>x.ShortText,languageId.Value,false);
                model.Title2 = sliderItem.GetLocalized(x=>x.TitleText,languageId.Value,false);
            }

            sliderItem = slides.Where(x => x.SlideNumber == 3).FirstOrDefault();
            if (sliderItem != null)
            {
                model.Link3 = sliderItem.Url;
                model.Picture3Id = sliderItem.PictureId;
                model.Text3 = sliderItem.GetLocalized(x=>x.ShortText,languageId.Value,false);
                model.Title3 = sliderItem.GetLocalized(x=>x.TitleText,languageId.Value,false);
            }

            sliderItem = slides.Where(x => x.SlideNumber == 4).FirstOrDefault();
            if (sliderItem != null)
            {
                model.Link4 = sliderItem.Url;
                model.Picture4Id = sliderItem.PictureId;
                model.Text4 = sliderItem.GetLocalized(x=>x.ShortText,languageId.Value,false);
                model.Title4 = sliderItem.GetLocalized(x=>x.TitleText,languageId.Value,false);
            }

            var lang = _languageService.GetLanguageById(languageId.Value);

            model.SelectedLanguage = new LanguageModel()
            {
                Name = lang.Name,
                LanguageCulture = lang.LanguageCulture,
                Id = lang.Id
            };

            model.AviableLanguages = new List<LanguageModel>();
            var language = _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "ru").FirstOrDefault();
            model.AviableLanguages.Add(new LanguageModel()
            {
                Name = language.Name,
                LanguageCulture = language.LanguageCulture,
                Id = language.Id
            });

            language = _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "en").FirstOrDefault();
            model.AviableLanguages.Add(new LanguageModel()
            {
                Name = language.Name,
                LanguageCulture = language.LanguageCulture,
                Id = language.Id
            });

            model.SelectedLanguageId = lang.Id;

            return View(model);
        }

        [HttpPost]
        public ActionResult SliderSetup(MiniSiteSliderModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }

            if (_workContext.CurrentCustomer.UserMiniSite == null)
            {
                return RedirectToRoute("HomePage");
            }

            if (ModelState.IsValid)
            {
                var miniSite = _workContext.CurrentCustomer.UserMiniSite;
                var sliderItems = miniSite.SliderItems;
                var item = sliderItems.Where(x => x.SlideNumber == 1).FirstOrDefault();
                if (item != null)
                {
                    if (item.PictureId != model.Picture1Id)
                    {
                        if (item.PictureId != 0)
                            _pictureService.DeletePicture(_pictureService.GetPictureById(item.PictureId));
                        item.PictureId = model.Picture1Id;
                    }

                    _localizedEntityService.SaveLocalizedValue(item, x => x.TitleText, model.Title1, model.SelectedLanguageId);
                    _localizedEntityService.SaveLocalizedValue(item, x => x.ShortText, model.Text1, model.SelectedLanguageId);
                    if (!String.IsNullOrEmpty(model.Link1))
                    {
                        Match match = Regex.Match(model.Link1, @"^http(s?)://\S+$");
                        model.Link1 = match.Success ? model.Link1 : "http://" + model.Link1;
                    }
                    item.Url = model.Link1;
                    _sliderItemService.Update(item);
                }
                else
                {
                    item = new SliderItem();
                    item.SlideNumber = 1;
                    item.PictureId = model.Picture1Id;

                    if (!String.IsNullOrEmpty(model.Link1))
                    {
                        Match match = Regex.Match(model.Link1, @"^http(s?)://\S+$");
                        model.Link1 = match.Success ? model.Link1 : "http://" + model.Link1;
                    }
                    item.Url = model.Link1;
                    item.UserMiniSiteId = miniSite.Id;
                    _sliderItemService.Insert(item);
                    _localizedEntityService.SaveLocalizedValue(item, x => x.TitleText, model.Title1, model.SelectedLanguageId);
                    _localizedEntityService.SaveLocalizedValue(item, x => x.ShortText, model.Text1, model.SelectedLanguageId);
                }

                item = sliderItems.Where(x => x.SlideNumber == 2).FirstOrDefault();
                if (item != null)
                {
                    if (item.PictureId != model.Picture2Id)
                    {
                        if (item.PictureId != 0)
                            _pictureService.DeletePicture(_pictureService.GetPictureById(item.PictureId));
                        item.PictureId = model.Picture2Id;
                    }

                    _localizedEntityService.SaveLocalizedValue(item, x => x.TitleText, model.Title2, model.SelectedLanguageId);
                    _localizedEntityService.SaveLocalizedValue(item, x => x.ShortText, model.Text2, model.SelectedLanguageId);
                    if (!String.IsNullOrEmpty(model.Link2))
                    {
                        Match match = Regex.Match(model.Link2, @"^http(s?)://\S+$");
                        model.Link2 = match.Success ? model.Link2 : "http://" + model.Link2;
                    }
                    item.Url = model.Link2;
                    _sliderItemService.Update(item);
                }
                else
                {
                    item = new SliderItem();
                    item.SlideNumber = 2;
                    item.PictureId = model.Picture2Id;
                    if (!String.IsNullOrEmpty(model.Link2))
                    {
                        Match match = Regex.Match(model.Link2, @"^http(s?)://\S+$");
                        model.Link2 = match.Success ? model.Link2 : "http://" + model.Link2;
                    }
                    item.Url = model.Link2;
                    item.UserMiniSiteId = miniSite.Id;
                    _sliderItemService.Insert(item);
                    _localizedEntityService.SaveLocalizedValue(item, x => x.TitleText, model.Title2, model.SelectedLanguageId);
                    _localizedEntityService.SaveLocalizedValue(item, x => x.ShortText,  model.Text2, model.SelectedLanguageId);
                }

                item = sliderItems.Where(x => x.SlideNumber == 3).FirstOrDefault();
                if (item != null)
                {
                    if (item.PictureId != model.Picture3Id)
                    {
                        if (item.PictureId != 0)
                            _pictureService.DeletePicture(_pictureService.GetPictureById(item.PictureId));
                        item.PictureId = model.Picture3Id;
                    }

                    _localizedEntityService.SaveLocalizedValue(item, x => x.TitleText, model.Title3, model.SelectedLanguageId);
                    _localizedEntityService.SaveLocalizedValue(item, x => x.ShortText, model.Text3, model.SelectedLanguageId);
                    if (!String.IsNullOrEmpty(model.Link3))
                    {
                        Match match = Regex.Match(model.Link3, @"^http(s?)://\S+$");
                        model.Link3 = match.Success ? model.Link3 : "http://" + model.Link3;
                    }
                    item.Url = model.Link3;
                    _sliderItemService.Update(item);
                }
                else
                {
                    item = new SliderItem();
                    item.SlideNumber = 3;
                    item.PictureId = model.Picture3Id;
                    if (!String.IsNullOrEmpty(model.Link3))
                    {
                        Match match = Regex.Match(model.Link3, @"^http(s?)://\S+$");
                        model.Link3 = match.Success ? model.Link3 : "http://" + model.Link3;
                    }
                    item.Url = model.Link3;
                    item.UserMiniSiteId = miniSite.Id;
                    _sliderItemService.Insert(item);
                    _localizedEntityService.SaveLocalizedValue(item, x => x.TitleText, model.Title3, model.SelectedLanguageId);
                    _localizedEntityService.SaveLocalizedValue(item, x => x.ShortText, model.Text3, model.SelectedLanguageId);
                }

                item = sliderItems.Where(x => x.SlideNumber == 4).FirstOrDefault();
                if (item != null)
                {
                    if (item.PictureId != model.Picture4Id)
                    {
                        if (item.PictureId != 0)
                            _pictureService.DeletePicture(_pictureService.GetPictureById(item.PictureId));
                        item.PictureId = model.Picture4Id;
                    }

                    _localizedEntityService.SaveLocalizedValue(item, x => x.TitleText, model.Title4, model.SelectedLanguageId);
                    _localizedEntityService.SaveLocalizedValue(item, x => x.ShortText, model.Text4, model.SelectedLanguageId);
                    if (!String.IsNullOrEmpty(model.Link4))
                    {
                        Match match = Regex.Match(model.Link4, @"^http(s?)://\S+$");
                        model.Link4 = match.Success ? model.Link4 : "http://" + model.Link4;
                    }
                    item.Url = model.Link4;
                    _sliderItemService.Update(item);
                }
                else
                {
                    item = new SliderItem();
                    item.SlideNumber = 4;
                    item.PictureId = model.Picture4Id;
                    if (!String.IsNullOrEmpty(model.Link4))
                    {
                        Match match = Regex.Match(model.Link4, @"^http(s?)://\S+$");
                        model.Link4 = match.Success ? model.Link4 : "http://" + model.Link4;
                    }
                    item.Url = model.Link4;
                    item.UserMiniSiteId = miniSite.Id;
                    _sliderItemService.Insert(item);
                    _localizedEntityService.SaveLocalizedValue(item, x => x.TitleText, model.Title4, model.SelectedLanguageId);
                    _localizedEntityService.SaveLocalizedValue(item, x => x.ShortText, model.Text4, model.SelectedLanguageId);
                }
            }

            var lang = _languageService.GetLanguageById(model.SelectedLanguageId);

            model.SelectedLanguage = new LanguageModel()
            {
                Name = lang.Name,
                LanguageCulture = lang.LanguageCulture,
                Id = lang.Id
            };

            model.AviableLanguages = new List<LanguageModel>();
            var language = _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "ru").FirstOrDefault();
            model.AviableLanguages.Add(new LanguageModel()
            {
                Name = language.Name,
                LanguageCulture = language.LanguageCulture,
                Id = language.Id
            });

            language = _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "en").FirstOrDefault();
            model.AviableLanguages.Add(new LanguageModel()
            {
                Name = language.Name,
                LanguageCulture = language.LanguageCulture,
                Id = language.Id
            });

            model.SelectedLanguageId = lang.Id;
            return View(model);
        }


        public ActionResult BannerSetup(int? languageId)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }

            if (_workContext.CurrentCustomer.UserMiniSite == null)
            {
                return RedirectToRoute("HomePage");
            }

            if(!languageId.HasValue)
            {
                if (_workContext.WorkingLanguage.Id == 2)
                {
                    languageId = 2;
                }
                else
                {
                    languageId = 1;
                }
                
            }
            var model = new List<MiniSiteBannerModel>();
            var banners = _workContext.CurrentCustomer.UserMiniSite.Banners;
            model = banners.Select(x=>PrepareBannerOverviewModel(x,languageId.Value)).ToList();
            
            return View(model);
        }

        public ActionResult BannerSetupAdd()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }
            if (_workContext.CurrentCustomer.UserMiniSite == null)
            {
                return RedirectToRoute("HomePage");
            }

            var model = new MiniSiteBannerModel();
            model.Languages = _languageService.GetAllLanguages().Where(x=>x.UniqueSeoCode == "ru" || x.UniqueSeoCode == "en")
                .Select(x=>new BuyingRequestLanguageModel()
                {
                    LanguageId = x.Id,
                    LanguageName = x.Name,
                    Selected = false,
                    FlagImageUrl = x.FlagImageFileName
                }).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult BannerSetupAdd(MiniSiteBannerModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }
            if (_workContext.CurrentCustomer.UserMiniSite == null)
            {
                return RedirectToRoute("HomePage");
            }
            if (model.Languages.Where(x => x.Selected).FirstOrDefault() == null)
            {
                ModelState.AddModelError("WorkingLanguage", _localizationService.GetResource("Etf.MiniSite.Banner.Language.Required"));
            }

            if (!String.IsNullOrEmpty(model.Url))
            {
                if (model.Url.IndexOf("http://") != 0)
                {
                    if (model.Url.IndexOf("https://") != 0)
                    {
                        ModelState.AddModelError("Url", _localizationService.GetResource("Etf.MiniSite.Banner.Http.Required"));
                    }
                }
            }

            if (ModelState.IsValid)
            {
                SaveBannerModel(model);
                return RedirectToAction("BannerSetup");
            }

            foreach (var lang in model.Languages)
            {
                lang.LanguageName = _languageService.GetLanguageById(lang.LanguageId).Name;
            }
            return View(model);
        }

        public ActionResult BannerSetupEdit(int Id, int languageId)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }
            if (_workContext.CurrentCustomer.UserMiniSite == null)
            {
                return RedirectToRoute("HomePage");
            }

            if(Id == 0)
                return RedirectToAction("BannerSetup");
            var banner = _miniSiteBannerService.GetById(Id); 
            if(banner == null)
                return RedirectToAction("BannerSetup");

            var model = new MiniSiteBannerModel();
            model.Languages = _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "ru" || x.UniqueSeoCode == "en")
                .Select(x => new BuyingRequestLanguageModel()
                {
                    LanguageId = x.Id,
                    LanguageName = x.Name,
                    Selected = x.Id == languageId,
                    FlagImageUrl = x.FlagImageFileName
                }).ToList();
            model.PictureId = banner.GetLocalized(x => x.BannerPictureId, languageId, false);
            model.TitleTag = banner.GetLocalized(x => x.BannerTitle, languageId, false);
            model.AltTag = banner.GetLocalized(x => x.BannerAlt, languageId, false);
            model.Url = banner.GetLocalized(x => x.BannerUrl, languageId, false);
            model.Id = banner.Id;
            model.WorkingLanguage = languageId;
            return View(model);
        }

        [HttpPost]
        public ActionResult BannerSetupEdit(MiniSiteBannerModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }
            if (_workContext.CurrentCustomer.UserMiniSite == null)
            {
                return RedirectToRoute("HomePage");
            }

            if (!String.IsNullOrEmpty(model.Url))
            {
                if (model.Url.IndexOf("http://") != 0)
                {
                    if (model.Url.IndexOf("https://") != 0)
                    {
                        ModelState.AddModelError("Url", _localizationService.GetResource("Etf.MiniSite.Banner.Http.Required"));
                    }
                }
            }

            if (ModelState.IsValid)
            {
                SaveBannerModel(model);
                return RedirectToAction("BannerSetup");
            }

            model.Languages = _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "ru" || x.UniqueSeoCode == "en")
                .Select(x => new BuyingRequestLanguageModel()
                {
                    LanguageId = x.Id,
                    LanguageName = x.Name,
                    Selected = x.Id == model.WorkingLanguage,
                    FlagImageUrl = x.FlagImageFileName
                }).ToList();

            return View(model);
        }

        public ActionResult BannerSetupDelete(int Id)
        {
            var banner = _miniSiteBannerService.GetById(Id);
            if(banner == null)
                return RedirectToAction("BannerSetup");
            foreach (var lang in _languageService.GetAllLanguages())
            {
                int pictureId = banner.GetLocalized(x => x.BannerPictureId, lang.Id, false);
                if (pictureId != 0)
                {
                    var picture = _pictureService.GetPictureById(pictureId);
                    if (picture != null)
                        _pictureService.DeletePicture(picture);
                }
            }
            _miniSiteBannerService.Delete(banner);

            return RedirectToAction("BannerSetup");
        }

        public ActionResult TextPageSetup(int? languageId)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }
            if (_workContext.CurrentCustomer.UserMiniSite == null)
            {
                return RedirectToRoute("HomePage");
            }

            if (!languageId.HasValue)
            {
                languageId = _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "ru").FirstOrDefault().Id;
            }

            var lang = _languageService.GetLanguageById(languageId.Value);
            var pages = _workContext.CurrentCustomer.UserMiniSite.Textpages.ToList();
            var model = new List<MiniSiteTextPagesModel>();
            if (pages.Count > 1)
            {
                foreach (var page in pages)
                {
                    var txtpageModel = new MiniSiteTextPagesModel();
                    txtpageModel.PageHtml = page.GetLocalized(x => x.Html, languageId.Value, false);
                    txtpageModel.PageMenuTitle = page.GetLocalized(x => x.MenuTitle, languageId.Value, false);
                    txtpageModel.PageTitle = page.GetLocalized(x => x.Title, languageId.Value, false);
                    txtpageModel.PageTitleTag = page.GetLocalized(x => x.PageTitle, languageId.Value, false);
                    txtpageModel.Id = page.Id;
                    txtpageModel.AviableLanguages = new List<LanguageModel>();
                    SetLanguage(languageId.Value, txtpageModel);
                    model.Add(txtpageModel);
                }
            }
            else
            {
                if (pages.Count == 1)
                {
                    var txtpageModel = new MiniSiteTextPagesModel();
                    txtpageModel.PageHtml = pages[0].GetLocalized(x => x.Html, languageId.Value, false);
                    txtpageModel.PageMenuTitle = pages[0].GetLocalized(x => x.MenuTitle, languageId.Value, false);
                    txtpageModel.PageTitle = pages[0].GetLocalized(x => x.Title, languageId.Value, false);
                    txtpageModel.PageTitleTag = pages[0].GetLocalized(x => x.PageTitle, languageId.Value, false);
                    txtpageModel.Id = pages[0].Id;
                    SetLanguage(languageId.Value, txtpageModel);

                    model.Add(txtpageModel);
                    txtpageModel = new MiniSiteTextPagesModel();
                    txtpageModel.PageTitle = "Page 2";
                    SetLanguage(languageId.Value, txtpageModel);
                    model.Add(txtpageModel);
                }
                else
                {
                    var txtpageModel = new MiniSiteTextPagesModel();
                    txtpageModel.PageTitle = "Page 1";
                    SetLanguage(languageId.Value, txtpageModel);
                    model.Add(txtpageModel);

                    txtpageModel = new MiniSiteTextPagesModel();
                    txtpageModel.PageTitle = "Page 2";
                    SetLanguage(languageId.Value, txtpageModel);
                    model.Add(txtpageModel);
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult TextPageSetup(MiniSiteTextPagesModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }
            if (_workContext.CurrentCustomer.UserMiniSite == null)
            {
                return RedirectToRoute("HomePage");
            }

            if (ModelState.IsValid)
            {
                if(model.Id != 0)
                {
                    var oldTextPage = _miniSiteTextPageService.GetById(model.Id);
                    _localizedEntityService.SaveLocalizedValue(oldTextPage, x => x.Html, model.PageHtml, model.SelectedLanguageId);
                    _localizedEntityService.SaveLocalizedValue(oldTextPage, x => x.MenuTitle, model.PageMenuTitle, model.SelectedLanguageId);
                    _localizedEntityService.SaveLocalizedValue(oldTextPage, x => x.PageTitle, model.PageTitleTag, model.SelectedLanguageId);
                    _localizedEntityService.SaveLocalizedValue(oldTextPage, x => x.Title, model.PageTitle, model.SelectedLanguageId);
                    var seName = oldTextPage.ValidateSeName(oldTextPage.GetLocalized(x=>x.Title,model.SelectedLanguageId),null, true);
                    _urlRecordService.SaveSlug(oldTextPage, seName, model.SelectedLanguageId);
                }else
                {
                    var textpage = new MiniSiteTextPage();
                    textpage.UserMiniSiteId = _workContext.CurrentCustomer.UserMiniSite.Id;
                    _miniSiteTextPageService.Insert(textpage);

                    _localizedEntityService.SaveLocalizedValue(textpage, x => x.Html, model.PageHtml, model.SelectedLanguageId);
                    _localizedEntityService.SaveLocalizedValue(textpage, x => x.MenuTitle, model.PageMenuTitle, model.SelectedLanguageId);
                    _localizedEntityService.SaveLocalizedValue(textpage, x => x.PageTitle, model.PageTitleTag, model.SelectedLanguageId);
                    _localizedEntityService.SaveLocalizedValue(textpage, x => x.Title, model.PageTitle, model.SelectedLanguageId);
                    var seName = textpage.ValidateSeName(textpage.GetLocalized(x => x.Title, model.SelectedLanguageId), null, true);
                    _urlRecordService.SaveSlug(textpage, seName, model.SelectedLanguageId);
                }
            }

            var lang = _languageService.GetLanguageById(model.SelectedLanguageId);

            model.CurrentLanguage = new LanguageModel()
            {
                Name = lang.Name,
                LanguageCulture = lang.LanguageCulture,
                Id = lang.Id
            };

            model.AviableLanguages = new List<LanguageModel>();
            var language = _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "ru").FirstOrDefault();
            model.AviableLanguages.Add(new LanguageModel()
            {
                Name = language.Name,
                LanguageCulture = language.LanguageCulture,
                Id = language.Id
            });

            language = _languageService.GetAllLanguages().Where(x => x.UniqueSeoCode == "en").FirstOrDefault();
            model.AviableLanguages.Add(new LanguageModel()
            {
                Name = language.Name,
                LanguageCulture = language.LanguageCulture,
                Id = language.Id
            });

            model.SelectedLanguageId = lang.Id;

            return RedirectToAction("TextPageSetup", new { languageId = model.SelectedLanguageId });
        }

        protected bool ValidateDomain(string domain)
        {
            return Regex.IsMatch(domain, @"^[a-z|A-Z|0-9][a-z|A-Z|0-9|-]{1,61}[a-z|A-Z|0-9](\.[a-z|A-Z|0-9][a-z|A-Z|0-9|-]{1,61}[a-z|A-Z|0-9])*\.[a-z|A-Z]{2,3}$");
        }
    }
}
