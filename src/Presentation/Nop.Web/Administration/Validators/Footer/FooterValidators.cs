using FluentValidation;
using Nop.Admin.Models.Footer;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Validators.Footer
{
    public partial class FooterValidator : AbstractValidator<FooterContentModel>
    {
        public FooterValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CopyrightText)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("BSFA.Admin.CopyRightText.Required"));
        }
    }
}