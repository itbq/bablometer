using Nop.Admin.Models.Footer;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Web.Framework.Controllers;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public partial class FooterController : Controller
    {
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;

        public FooterController(ILocalizationService localizationService,
            ILanguageService languageService)
        {
            this._localizationService = localizationService;
            this._languageService = languageService;
        }
        protected FooterContentModel PrepareModel()
        {
            var model = new FooterContentModel();
            model.CopyrightText = _localizationService.GetResource("Content.CopyrightNotice");
            model.ContactText = _localizationService.GetResource("Content.Contact");
            model.CopyRightLink = _localizationService.GetResource("Content.CopyrightLink");

            return model;
        }

        public ActionResult FooterContent()
        {
            var model = PrepareModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult FooterContent(FooterContentModel model)
        {
            if (ModelState.IsValid)
            {
                var resource = _localizationService.GetLocaleStringResourceByName("Content.CopyrightNotice");
                resource.ResourceValue = model.CopyrightText;
                _localizationService.UpdateLocaleStringResource(resource);
                if (!String.IsNullOrEmpty(model.CopyRightLink))
                {
                    resource = _localizationService.GetLocaleStringResourceByName("Content.CopyrightLink");
                    resource.ResourceValue = model.CopyRightLink;
                    _localizationService.UpdateLocaleStringResource(resource);
                }

                resource = _localizationService.GetLocaleStringResourceByName("Content.Contact");
                resource.ResourceValue = model.ContactText;
                _localizationService.UpdateLocaleStringResource(resource);
                return View(model);
            }

            return View(model);
        }
    }
}
