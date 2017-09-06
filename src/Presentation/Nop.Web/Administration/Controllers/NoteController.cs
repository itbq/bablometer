using Nop.Admin.Models.Notes_Instructions;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public partial class NoteController : BaseNopController
    {
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;

        public NoteController(ILocalizationService localizationService,
            ILanguageService languageService,
            IWorkContext workContext)
        {
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._workContext = workContext;
        }

        protected NoteListModel PrepareModel()
        {
            var model = new NoteListModel();

            var notes = new List<NoteModel>();
            foreach (var lang in _languageService.GetAllLanguages())
            {
                var resources = _localizationService.GetAllResources(lang.Id)
                    .Where(x => x.ResourceName.StartsWith("Content.Notes."));
                foreach (var resource in resources)
                {
                    var note = notes.Where(x => x.Name == resource.ResourceName).FirstOrDefault();
                    if (note == null)
                    {
                        notes.Add(new NoteModel()
                        {
                            Name = resource.ResourceName,
                            Text = resource.ResourceValue,
                            Locales = new List<NoteLocalizedModel>()
                            {
                                new NoteLocalizedModel()
                                {
                                    TextValue = resource.ResourceValue,
                                    LanguageId = resource.LanguageId
                                }
                            }
                        });
                    }
                    else
                    {
                        note.Locales.Add(new NoteLocalizedModel()
                                {
                                    TextValue = resource.ResourceValue,
                                    LanguageId = resource.LanguageId
                                });
                    }
                }
            }

            model.Notes = new GridModel<NoteModel>(notes);
            return model;
        }


        public void UpdateNote(NoteModel model)
        {
            foreach (var locale in model.Locales)
            {
                var resource = _localizationService.GetLocaleStringResourceByName(model.Name, locale.LanguageId);
                if (resource != null)
                {
                    resource.ResourceValue = locale.TextValue;
                    _localizationService.UpdateLocaleStringResource(resource);
                }
                else
                {
                    var res = new LocaleStringResource()
                    {
                        ResourceName = model.Name,
                        ResourceValue = locale.TextValue,
                        LanguageId = locale.LanguageId
                    };
                    _localizationService.InsertLocaleStringResource(res);
                }
            }
        }

        public ActionResult List()
        {
            var model = PrepareModel();
            model.WorkLanguageId = _workContext.WorkingLanguage.Id;
            return View(model);
        }

        public ActionResult Edit(string name)
        {
            var model = new NoteModel()
            {
                Name = name,
                Locales = new List<NoteLocalizedModel>()
            };
            foreach (var lang in _languageService.GetAllLanguages())
            {
                model.Locales.Add(new NoteLocalizedModel()
                {
                    TextValue = _localizationService.GetResource(name, lang.Id,returnEmptyIfNotFound:true),
                    LanguageId = lang.Id
                });
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(NoteModel model)
        {
            if (ModelState.IsValid)
            {
                UpdateNote(model);
                return RedirectToAction("List");
            }

            return View(model);
        }
    }
}
