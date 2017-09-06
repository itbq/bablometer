using FluentValidation;
using Nop.Admin.Models.Regions;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Validators.Regions
{
    public class RegionModelValidator : AbstractValidator<RegionModel>
    {
        public RegionModelValidator(ILocalizationService _localizationService)
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("ITB.Admin.Region.Title.Required"));
            RuleFor(x => x.Title)
               .NotEmpty()
               .WithMessage(_localizationService.GetResource("ITB.Admin.Region.Code.Required"));

        }
    }
}