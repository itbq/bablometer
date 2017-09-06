using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Models.MiniSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Validators.MiniSite
{

    public class MiniSiteBannerModelValidator : AbstractValidator<MiniSiteBannerModel>
    {
        public MiniSiteBannerModelValidator(ILocalizationService _localizationService)
        {
            RuleFor(x => x.PictureId)
                .NotEqual(0)
                .WithMessage(_localizationService.GetResource("MiniSite.Banner.Picture.Required"));
        }
    }
}