using Nop.Admin.Models.Event;
using Nop.Core.Domain.Common;
using Nop.Services.EventService;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Nop.Services.Seo;
using Nop.Core.Domain.Event;
using Nop.Services.Localization;
using Nop.Admin.Extensions;
using Nop.Web.Framework.Localization;
using System.Threading;
using Nop.Admin.Validators.Event;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public partial class EventController : BaseNopController
    {
        private readonly IEventService _eventService;
        private readonly IPictureService _pictureService;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILanguageService _languageService;

        public EventController(IEventService eventService,
            IPictureService pictureService,
            AdminAreaSettings adminSettings,
            ICustomerActivityService customerActivityService,
            IUrlRecordService urlRecordService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            ILanguageService languageService)
        {
            this._eventService = eventService;
            this._pictureService = pictureService;
            this._adminAreaSettings = adminSettings;
            this._customerActivityService = customerActivityService;
            this._urlRecordService = urlRecordService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._languageService = languageService;
        }


        [NonAction]
        protected void UpdateLocales(Event mt, EventModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(mt,
                                                           x => x.ShortDescription,
                                                           localized.ShortDescription,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(mt,
                                                           x => x.Title,
                                                           localized.Title,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(mt,
                                                           x => x.FullDescription,
                                                           localized.FullDescription,
                                                           localized.LanguageId);
                _urlRecordService.ClearEntitySlug(mt, localized.LanguageId);
                //search engine name
                model.SeName = mt.ValidateSeName(model.SeName, mt.Title, true);
                _urlRecordService.SaveSlug(mt, model.SeName, localized.LanguageId);
            }
        }

        public ActionResult List()
        {
            var model = new EventListModel();
            var events = _eventService.GetAllEvents(0, _adminAreaSettings.GridPageSize);

            model.Events = new GridModel<EventModel>()
            {
                Data = events.Select(x => x.ToModel()),
                Total = events.TotalCount
            };
            return View(model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command, EventListModel model)
        {
            var events = _eventService.GetAllEvents(command.Page - 1, command.PageSize);

            var gridModel = new GridModel<EventModel>()
            {
                Data = events.Select(x => x.ToModel()),
                Total = events.TotalCount
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        public ActionResult Create()
        {
            var model = new EventModel();
            model.PictureId = 0;
            model.CatalogPictureId = 0;
            model.Locales = new List<EventLocalizedModel>();
            var evnt = new Event();
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.FullDescription = evnt.GetLocalized(x => x.FullDescription, languageId, false, false);
                locale.ShortDescription = evnt.GetLocalized(x => x.ShortDescription, languageId, false, false);
                locale.Title = evnt.GetLocalized(x => x.Title, languageId, false, false);
            });
            if (model.EndDate != null)
            {
                model.EndDateEnabled = true;
            }
            ViewBag.error = false;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(EventModel model, bool continueEditing)
        {
            if (!model.EndDateEnabled)
                model.EndDate = null;

            //check there any valid localized models 
            if (ModelState.IsValid && model.CheckLocales())
            {
                if ((ModelState["StartDate"] != null && ModelState["StartDate"].Errors.Count != 0))
                {
                    ModelState["Title"].Errors.Clear();
                    ModelState["ShortDescription"].Errors.Clear();
                    ModelState["FullDescription"].Errors.Clear();
                    ViewBag.error = true;
                    return View(model);
                }
                var evnt = model.ToEntity();
                if (!model.EndDateEnabled)
                {
                    evnt.EndDate = null;
                }
                _eventService.AddEvent(evnt);

                model.SeName = evnt.ValidateSeName(model.SeName, evnt.Title, true);
                _urlRecordService.SaveSlug(evnt, model.SeName, 0);
                UpdateLocales(evnt, model);

                foreach (var loc in model.Locales)
                {
                    if (loc.Title != null)
                    {
                        loc.SeName = evnt.ValidateSeName(null, loc.Title, true);
                        _urlRecordService.SaveSlug(evnt, loc.SeName, loc.LanguageId);
                    }
                }
                //activity log
                _customerActivityService.InsertActivity("AddNewCategory", "Category added sucesfully", evnt.Title);

                SuccessNotification("Event Added sucessfully");
                return continueEditing ? RedirectToAction("Edit", new { id = evnt.Id }) : RedirectToAction("List");
            }
            ViewBag.error = true;
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var evnt = _eventService.GetEventById(id);
            if (evnt == null)
            {
                return RedirectToAction("List");
            }

            var model = evnt.ToModel();
            model.Title = null;
            model.ShortDescription = null;
            model.FullDescription = null;
            model.Locales = new List<EventLocalizedModel>();
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.FullDescription = evnt.GetLocalized(x => x.FullDescription, languageId, false, false);
                locale.ShortDescription = evnt.GetLocalized(x => x.ShortDescription, languageId, false, false);
                locale.Title = evnt.GetLocalized(x => x.Title, languageId, false, false);
            });
            if (model.EndDate != null)
            {
                model.EndDateEnabled = true;
            }
            ViewBag.error = false;
            return View(model);
        }


        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(EventModel model, bool continueEditing)
        {
            var evnt = _eventService.GetEventById(model.Id);
            if (evnt == null)
                //No category found with the specified id
                return RedirectToAction("List");

            //check there any valid localized models 
            if (ModelState.IsValid && model.CheckLocales())
            {
                if (ModelState["StartDate"] != null && ModelState["StartDate"].Errors.Count != 0)
                {
                    ModelState["Title"].Errors.Clear();
                    ModelState["ShortDescription"].Errors.Clear();
                    ModelState["FullDescription"].Errors.Clear();
                    ViewBag.error = true;
                    return View(model);
                }
                int prevPictureId = evnt.PictureId;
                int prevCatalogPictureId = evnt.CatalogPictureId;
                evnt = model.ToEntity(evnt);
                if (!model.EndDateEnabled)
                {
                    evnt.EndDate = null;
                }
                _eventService.UpdateEvent(evnt);

                UpdateLocales(evnt, model);
                foreach (var loc in model.Locales)
                {
                    if (loc.Title != null)
                    {
                        loc.SeName = evnt.ValidateSeName(null,loc.Title,true);
                        _urlRecordService.SaveSlug(evnt, loc.SeName,loc.LanguageId);
                    }
                }
                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != evnt.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }
                //delete an old picture (if deleted or updated)
                if (prevCatalogPictureId > 0 && prevCatalogPictureId != evnt.CatalogPictureId)
                {
                    var prevCatalogPicture = _pictureService.GetPictureById(prevCatalogPictureId);
                    if (prevCatalogPicture != null)
                        _pictureService.DeletePicture(prevCatalogPicture);
                }
                
                //activity log
                _customerActivityService.InsertActivity("EditCategory", "Event edited", evnt.Title);

                SuccessNotification("Event saved scessfully");
                ViewBag.error = false;
                return continueEditing ? RedirectToAction("Edit", evnt.Id) : RedirectToAction("List");
            }
            ViewBag.error = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {

            var evnt = _eventService.GetEventById(id);
            if (evnt == null)
                //No category found with the specified id
                return RedirectToAction("List");
            _urlRecordService.ClearEntitySlug(evnt, 0);
            _eventService.DeleteEvent(evnt);

            //activity log
            _customerActivityService.InsertActivity("DeleteCategory", "Event deleted", evnt.Title);

            SuccessNotification("Event deleted sucessfully");
            return RedirectToAction("List");
        }
    }
}
