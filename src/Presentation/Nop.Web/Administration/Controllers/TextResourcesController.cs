using Nop.Admin.Models.TextResource;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public partial class TextResourcesController : BaseNopController
    {
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        public TextResourcesController(ILocalizationService localizationService,
            ILanguageService languageService,
            IWorkContext workContext)
        {
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._workContext = workContext;
        }

        public ActionResult Index()
        {
            var model = new TextResourceModel();
            model.JoinUsText = _localizationService.GetResource("TextResource.JoinUs",_workContext.WorkingLanguage.Id,returnEmptyIfNotFound:true);
            //locale.ContactAddText = _localizationService.GetResource("TextResource.ContactAdd", languageId, returnEmptyIfNotFound: true);
            //locale.ItemAddText = _localizationService.GetResource("TextResource.ItemAdd", languageId, returnEmptyIfNotFound: true);
            //locale.RequestAccept = _localizationService.GetResource("TextResource.AcceptRequestPrompt.Value", languageId, returnEmptyIfNotFound: true);
            //locale.RequestReject = _localizationService.GetResource("TextResource.RejectRequestPrompt.Value", languageId, returnEmptyIfNotFound: true);
            //locale.UploadCatalogStep1Title = _localizationService.GetResource("ETF.Profile.UploadCatalog.Step1.Title", languageId, returnEmptyIfNotFound: true);
            //locale.UploadCatalogStep1Prompt = _localizationService.GetResource("ETF.Profile.UploadCatalog.Step1.Prompt", languageId, returnEmptyIfNotFound: true);
            //locale.UploadCatalogStep2Title = _localizationService.GetResource("ETF.Profile.UploadCatalog.Step2.Title", languageId, returnEmptyIfNotFound: true);
            //locale.UploadCatalogStep2Prompt = _localizationService.GetResource("ETF.Profile.UploadCatalog.Step2.Prompt", languageId, returnEmptyIfNotFound: true);
            //locale.UploadCatalogStep3Title = _localizationService.GetResource("ETF.Profile.UploadCatalog.Step3.Title", languageId, returnEmptyIfNotFound: true);
            //locale.UploadCatalogStep3Prompt = _localizationService.GetResource("ETF.Profile.UploadCatalog.Step3.Prompt", languageId, returnEmptyIfNotFound: true);
            //locale.UploadCatalogStep4Title = _localizationService.GetResource("ETF.Profile.UploadCatalog.Step4.Title", languageId, returnEmptyIfNotFound: true);
            //locale.UploadCatalogStep4Prompt = _localizationService.GetResource("ETF.Profile.UploadCatalog.Step4.Prompt", languageId, returnEmptyIfNotFound: true);
            //locale.UploadCatalogStep5Title = _localizationService.GetResource("ETF.Profile.UploadCatalog.Step5.Title", languageId, returnEmptyIfNotFound: true);
            //locale.UploadCatalogStep5Prompt = _localizationService.GetResource("ETF.Profile.UploadCatalog.Step5.Prompt", languageId, returnEmptyIfNotFound: true);
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(TextResourceModel model)
        {
            if (ModelState.IsValid)
            {
                //UpdateResource(lang, locale.ContactAddText, "TextResource.ContactAdd");
                //UpdateResource(lang, locale.ItemAddText, "TextResource.ItemAdd");
                UpdateResource(model.JoinUsText, "TextResource.JoinUs");
                //UpdateResource(lang, locale.RequestAccept, "TextResource.AcceptRequestPrompt.Value");
                //UpdateResource(lang, locale.RequestReject, "TextResource.RejectRequestPrompt.Value");
                //UpdateResource(lang, locale.UploadCatalogStep1Title, "ETF.Profile.UploadCatalog.Step1.Title");
                //UpdateResource(lang, locale.UploadCatalogStep1Prompt, "ETF.Profile.UploadCatalog.Step1.Prompt");
                //UpdateResource(lang, locale.UploadCatalogStep2Title, "ETF.Profile.UploadCatalog.Step2.Title");
                //UpdateResource(lang, locale.UploadCatalogStep2Prompt, "ETF.Profile.UploadCatalog.Step2.Prompt");
                //UpdateResource(lang, locale.UploadCatalogStep3Title, "ETF.Profile.UploadCatalog.Step3.Title");
                //UpdateResource(lang, locale.UploadCatalogStep3Prompt, "ETF.Profile.UploadCatalog.Step3.Prompt");
                //UpdateResource(lang, locale.UploadCatalogStep4Title, "ETF.Profile.UploadCatalog.Step4.Title");
                //UpdateResource(lang, locale.UploadCatalogStep4Prompt, "ETF.Profile.UploadCatalog.Step4.Prompt");
                //UpdateResource(lang, locale.UploadCatalogStep5Title, "ETF.Profile.UploadCatalog.Step5.Title");
                //UpdateResource(lang, locale.UploadCatalogStep5Prompt, "ETF.Profile.UploadCatalog.Step5.Prompt");
            }
            return View(model);
        }

        private void UpdateResource(string localizedValue, string name)
        {
            var resource = _localizationService.GetLocaleStringResourceByName(name);
            if (resource == null)
            {
                resource = new Core.Domain.Localization.LocaleStringResource();
                resource.LanguageId = _workContext.WorkingLanguage.Id;
                resource.ResourceName = name;
                resource.ResourceValue = localizedValue;
                _localizationService.InsertLocaleStringResource(resource);
            }
            else
            {
                resource.ResourceValue = localizedValue;
                _localizationService.UpdateLocaleStringResource(resource);
            }
        }
    }

}
