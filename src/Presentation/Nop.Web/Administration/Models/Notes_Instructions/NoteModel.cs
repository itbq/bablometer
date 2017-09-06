using FluentValidation.Attributes;
using Nop.Admin.Validators.Note_Instructions;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Admin.Models.Notes_Instructions
{
    public partial class NoteModel:BaseNopModel,ILocalizedModel<NoteLocalizedModel>
    {
        [NopResourceDisplayName("Admin.ContentManagement.Notes.Text")]
        [AllowHtml]
        public string Text {get; set;}

        [NopResourceDisplayName("Admin.ContentManagement.Notes.Title")]
        public string Name { get; set; }
        public IList<NoteLocalizedModel> Locales { get; set; }
    }

    [Validator(typeof(NoteModelValidator))]
    public partial class NoteLocalizedModel : ILocalizedModelLocal
    {
        [NopResourceDisplayName("Admin.ContentManagement.Notes.Text")]
        [AllowHtml]
        public string TextValue { get; set; }

        public int LanguageId { get; set; }
    }
}