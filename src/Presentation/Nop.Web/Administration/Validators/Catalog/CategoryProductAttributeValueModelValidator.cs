using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluentValidation;
using Nop.Admin.Models.Catalog;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Catalog
{
    public class CategoryProductAttributeValueModelValidator : AbstractValidator<CategoryProductAttributeValueModel>
    {

        public CategoryProductAttributeValueModelValidator(ILocalizationService localizationService)
        {
            //RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Variants.CategoryProductVariantAttributes.Attributes.Values.Fields.Name.Required"));
        }
    }

    public class CategoryProductAttributeValueLocalizedModelValidator : AbstractValidator<CategoryProductAttributeValueLocalizedModel>
    {
        public CategoryProductAttributeValueLocalizedModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Variants.CategoryProductVariantAttributes.Attributes.Values.Fields.Name.Required"));
        }
    }
}
