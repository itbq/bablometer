using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Validators.Request
{
    public partial class RequestValidator : AbstractValidator<RequestOverviewModel>
    {
        public RequestValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.RequestComment)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Request.Validation.RequestComment"));
            RuleFor(x => x.Quantity)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Request.Validation.Quantity"));
            //RuleFor(x => x.Quantity)
            //    .NotEmpty()
            //    .WithMessage("Quantity should be greater than {0}");
        }
    }
}