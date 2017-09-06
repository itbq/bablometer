using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Models.MiniSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Validators.MiniSite
{
    public class MiniSiteActivationModelValidator:AbstractValidator<MiniSiteActivationModel>
    {
        public MiniSiteActivationModelValidator(ILocalizationService _localizationService)
        {
            RuleFor(x => x.LayoutTemplateId)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("ETF.MiniSite.Layout.Required"));
            RuleFor(x => x.CssTemplateName)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("ETF.MiniSite.CssTemplate.Required"));
            RuleFor(x => x.OwnDomain)
                .Must((x, OwnDomain) => { return !x.UseSecondLavelDomain ? !String.IsNullOrEmpty(x.OwnDomain) : true; })
                .WithMessage(_localizationService.GetResource("ETF.MiniSite.DomainName.Required"));
            RuleFor(x => x.OwnDomain)
                .Must((x, OwnDomain) => { return x.UseSecondLavelDomain ? !String.IsNullOrEmpty(x.DomainName) : true; })
                .WithMessage(_localizationService.GetResource("ETF.MiniSite.DomainName.Required"));
        }
    }
}