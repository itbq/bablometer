using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Models.MiniSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Validators.MiniSite
{
    public class MiniSiteTextPagesModelValidator:AbstractValidator<MiniSiteTextPagesModel>
    {
        public MiniSiteTextPagesModelValidator(ILocalizationService _localizationService)
        {
            RuleFor(x => x.PageHtml)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("ETF.TextPages.PageHtml.Required"));

            RuleFor(x => x.PageTitleTag)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("ETF.TextPages.PageTitleTag.Required"));

            RuleFor(x => x.PageTitle)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("ETF.TextPages.PageHeader.Required"));

            RuleFor(x => x.PageMenuTitle)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("ETF.TextPages.MenuTitle.Required"));
        }
    }
}