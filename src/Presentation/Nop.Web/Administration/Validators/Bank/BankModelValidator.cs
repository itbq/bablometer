
using FluentValidation;
using Nop.Admin.Models.Bank;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Validators.Bank
{
    public class BankModelValidator : AbstractValidator<BankModel>
    {
        public BankModelValidator(ILocalizationService _localizationService)
        {
            RuleFor(x => x.BankTitle)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("ITB.Admin.Bank.Title.Required"));
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("ITB.Admin.Bank.Email.Require"));
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage(_localizationService.GetResource("ITB.Admin.Bank.Email.Format"));
            RuleFor(x => x.Rating)
               .LessThanOrEqualTo(5)
               .WithMessage(_localizationService.GetResource("ITB.Admin.Bank.Email.Rating.Max"));
            RuleFor(x => x.Rating)
               .GreaterThanOrEqualTo(0)
               .WithMessage(_localizationService.GetResource("ITB.Admin.Bank.Email.Rating.Min"));
        }
    }
}