using FluentValidation;
using Nop.Admin.Models.Notes_Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Validators.Note_Instructions
{
    public class NoteModelValidator : AbstractValidator<NoteLocalizedModel>
    {
        public NoteModelValidator()
        {
            RuleFor(x => x.TextValue)
                .NotEmpty()
                .WithMessage("Note text required");
        }
    }
}