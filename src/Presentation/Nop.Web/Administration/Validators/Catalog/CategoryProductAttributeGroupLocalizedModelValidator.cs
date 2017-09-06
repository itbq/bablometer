using FluentValidation;
using Nop.Admin.Models.Catalog;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Validators.Catalog
{
    public class CategoryProductAttributeGroupLocalizedModelValidator : AbstractValidator<CategoryProductAttributeGroupLocalizedModel>
    {
        public CategoryProductAttributeGroupLocalizedModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Catalog.Products.Variants.CategoryProductVariantAttributes.Attributes.Values.Fields.Name.Required"));
        }
    }
}