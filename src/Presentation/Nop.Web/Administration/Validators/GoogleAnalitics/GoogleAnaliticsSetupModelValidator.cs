using FluentValidation;
using Nop.Admin.Models.GoogleAnalitics;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Validators.GoogleAnalitics
{
    public class GoogleAnaliticsSetupModelValidator:AbstractValidator<AnaliticsSetupModel>
    {
        public GoogleAnaliticsSetupModelValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.AccountId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("ETF.Admin.Analitics.AccountId.Error"));
            RuleFor(model => model.ClientId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("ETF.Admin.Analitics.ClientId.Error"));
        }
    }
}