using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using Nop.Admin.Models.Brand;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Brand
{
    public class BrandValidator : AbstractValidator<BrandModel>
    {
        public BrandValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.Brand.Fields.Name.Required"));
        }
    }
}