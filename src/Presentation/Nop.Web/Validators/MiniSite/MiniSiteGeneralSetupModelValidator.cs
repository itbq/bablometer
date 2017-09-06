using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Models.MiniSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Validators.MiniSite
{
    public class MiniSiteGeneralSetupModelValidator:AbstractValidator<MiniSiteGeneralSetupModel>
    {
        public MiniSiteGeneralSetupModelValidator(ILocalizationService _localizationService)
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("ETF.MiniSite.EnglishTitle.Required"));

            RuleFor(x => x.TitleRus)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("ETF.MiniSite.RussianTitle.Required"));
            RuleFor(x => x.ContactEmail)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("ETF.MiniSite.Email.Required"));

            RuleFor(x => x.ContactEmail)
                .EmailAddress()
                .WithMessage(_localizationService.GetResource("ETF.MiniSite.Email.Incorrect"));

            RuleFor(x => x.LayoutId)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("ETF.MiniSite.Layout.Required"));
            RuleFor(x => x.CssTemplateName)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("ETF.MiniSite.CssTemplate.Required"));
            RuleFor(x => x.OwnDomain)
                .Must((x, OwnDomain) => { return !x.UseSecondLavelDomain? !String.IsNullOrEmpty(x.OwnDomain) : true; })
                .WithMessage(_localizationService.GetResource("ETF.MiniSite.DomainName.Required"));
            RuleFor(x => x.OwnDomain)
                .Must((x, OwnDomain) => { return x.UseSecondLavelDomain ? !String.IsNullOrEmpty(x.DomainName) : true; })
                .WithMessage(_localizationService.GetResource("ETF.MiniSite.DomainName.Required"));
        }
    }
}