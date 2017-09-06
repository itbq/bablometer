using FluentValidation;
using Nop.Admin.Models.Regions;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Validators.Regions
{
    public class CityModelValidator : AbstractValidator<CityModel>
    {
        public CityModelValidator(ILocalizationService _localizationService)
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("ITB.Admin.Region.Title.Required"));
        }
    }
}