using System.Web.Mvc;
using System.Collections.Generic;
using Nop.Plugin.Widgets.NivoSlider.Models;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Web.Framework.Controllers;
using Nop.Services.MiniSite;
using Nop.Services.Localization;
using Nop.Core;
using System;
using Nop.Web.Framework.Localization;
using System.Linq;

namespace Nop.Plugin.Widgets.NivoSlider.Controllers
{
    public class WidgetsNivoSliderController : Controller
    {
        private readonly IPictureService _pictureService;
        private readonly NivoSliderSettings _nivoSliderSettings;
        private readonly ISettingService _settingService;
        private readonly ISliderItemService _sliderItemService;
        private readonly IWorkContext _workContext;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;

        public WidgetsNivoSliderController(IPictureService pictureService, 
            NivoSliderSettings nivoSliderSettings, ISettingService settingService,
            ISliderItemService sliderItemService,
            IWorkContext workContext,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService)
        {
            this._pictureService = pictureService;
            this._nivoSliderSettings = nivoSliderSettings;
            this._settingService = settingService;
            this._sliderItemService = sliderItemService;
            this._workContext = workContext;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
        }

        /// <summary>
        /// Add locales for localizable entities
        /// </summary>
        /// <typeparam name="TLocalizedModelLocal">Localizable model</typeparam>
        /// <param name="languageService">Language service</param>
        /// <param name="locales">Locales</param>
        /// <param name="configure">Configure action</param>
        public virtual void AddLocales<TLocalizedModelLocal>(ILanguageService languageService, IList<TLocalizedModelLocal> locales, Action<TLocalizedModelLocal, int> configure) where TLocalizedModelLocal : ILocalizedModelLocal
        {
            foreach (var language in languageService.GetAllLanguages(true))
            {
                var locale = Activator.CreateInstance<TLocalizedModelLocal>();
                locale.LanguageId = language.Id;
                if (configure != null)
                {
                    configure.Invoke(locale, locale.LanguageId);
                }
                locales.Add(locale);
            }
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new SliderItemListModel();
            model.Slides = new List<SliderItemModel>();
            var items = _sliderItemService.GetMailPageSlides();
            foreach (var item in items)
            {
                var tmpModel = new SliderItemModel();
                tmpModel.Locales = new List<SliderItemLocalizedModel>();
                tmpModel.Id = item.Id;
                tmpModel.PictureId = item.PictureId;
                tmpModel.Link = item.Url;
                AddLocales(_languageService, tmpModel.Locales, (locale, languageId) =>
                {
                    locale.SubText = item.GetLocalized(x => x.ShortText, languageId, false, false);
                    locale.Text = item.GetLocalized(x => x.TitleText, languageId, false, false);
                });
                model.Slides.Add(tmpModel);
            }
            //var model = new ConfigurationModel();
            //model.Picture1Id = _nivoSliderSettings.Picture1Id;
            //model.Text1 = _nivoSliderSettings.Text1;
            //model.SubText1 = _nivoSliderSettings.SubText1;
            //model.Link1 = _nivoSliderSettings.Link1;

            //model.Picture2Id = _nivoSliderSettings.Picture2Id;
            //model.Text2 = _nivoSliderSettings.Text2;
            //model.SubText2 = _nivoSliderSettings.SubText2;
            //model.Link2 = _nivoSliderSettings.Link2;

            //model.Picture3Id = _nivoSliderSettings.Picture3Id;
            //model.Text3 = _nivoSliderSettings.Text3;
            //model.SubText3 = _nivoSliderSettings.SubText3;
            //model.Link3 = _nivoSliderSettings.Link3;

            //model.Picture4Id = _nivoSliderSettings.Picture4Id;
            //model.Text4 = _nivoSliderSettings.Text4;
            //model.SubText4 = _nivoSliderSettings.SubText4;
            //model.Link4 = _nivoSliderSettings.Link4;

            return View("Nop.Plugin.Widgets.NivoSlider.Views.WidgetsNivoSlider.Configure", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(SliderItemModel model)
        {
            if (!ModelState.IsValid)
                return Configure();
            var item = _sliderItemService.GetMailPageSlides().Where(x => x.Id == model.Id).FirstOrDefault();
            if (item.PictureId != model.PictureId)
            {
                if (item.PictureId != 0)
                {
                    var pic = _pictureService.GetPictureById(item.PictureId);
                    if(pic != null)
                        _pictureService.DeletePicture(pic);
                }
                item.PictureId = model.PictureId;
            }

            foreach (var lang in _languageService.GetAllLanguages())
            {
                var locale = model.Locales.Where(x => x.LanguageId == lang.Id).FirstOrDefault();
                _localizedEntityService.SaveLocalizedValue(item, x => x.TitleText, locale.Text, lang.Id);
                _localizedEntityService.SaveLocalizedValue(item, x => x.ShortText, locale.SubText, lang.Id);
            }


            item.Url = model.Link;
            _sliderItemService.Update(item);
            var newModel = new SliderItemListModel();
            newModel.Slides = new List<SliderItemModel>();
            var items = _sliderItemService.GetMailPageSlides();
            foreach (var newItem in items)
            {
                var tmpModel = new SliderItemModel();
                tmpModel.Locales = new List<SliderItemLocalizedModel>();
                tmpModel.Id = newItem.Id;
                tmpModel.PictureId = newItem.PictureId;
                tmpModel.Link = newItem.Url;
                AddLocales(_languageService, tmpModel.Locales, (locale, languageId) =>
                {
                    locale.SubText = newItem.GetLocalized(x => x.ShortText, languageId, false, false);
                    locale.Text = newItem.GetLocalized(x => x.TitleText, languageId, false, false);
                });
                newModel.Slides.Add(tmpModel);
            }
            newModel.Returning = true;
            return View("Nop.Plugin.Widgets.NivoSlider.Views.WidgetsNivoSlider.Configure", newModel);
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone)
        {
            var model = new List<SliderItemModel>();
            var items = _sliderItemService.GetMailPageSlides();
            foreach (var item in items)
            {
                if (item.PictureId == 0)
                    continue;
                var tmpModel = new SliderItemModel();
                tmpModel.PictureUrl = _pictureService.GetPictureUrl(item.PictureId, showDefaultPicture: false);
                tmpModel.Text = "<strong><a href=\"" + item.Url + "\" >" + item.GetLocalized(x => x.TitleText, _workContext.WorkingLanguage.Id, false) + "</a></strong><div>" + item.GetLocalized(x => x.ShortText, _workContext.WorkingLanguage.Id, false) + "</div>";
                tmpModel.SubText = item.Url;
                model.Add(tmpModel);
            }
            return View("Nop.Plugin.Widgets.NivoSlider.Views.WidgetsNivoSlider.PublicInfo", model);
        }
    }
}