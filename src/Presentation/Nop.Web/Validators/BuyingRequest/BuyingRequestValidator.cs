using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Validators.BuyingRequest
{
    public class BuyingRequestValidator:AbstractValidator<BuyingRequestModel>
    {
        public BuyingRequestValidator(ILocalizationService localizationSerice)
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(localizationSerice.GetResource("BuyingRequest.Validation.Title"));
            RuleFor(x => x.ShortDescription)
                .NotEmpty()
                .WithMessage(localizationSerice.GetResource("News.Validation.Short"));
            RuleFor(x=>x.FullDescription)
                .NotEmpty()
                .WithMessage(localizationSerice.GetResource("News.Validation.Full"));
        }
    }
}