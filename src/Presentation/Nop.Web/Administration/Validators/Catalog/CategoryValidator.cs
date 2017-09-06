using FluentValidation;
using Nop.Admin.Models.Catalog;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Catalog
{
    public class CategoryValidator : AbstractValidator<CategoryModel>
    {
        public CategoryValidator(ILocalizationService localizationService)
        {
            //RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.Catalog.Categories.Fields.Name.Required"));
        }
    }

    public class CategoryLocalizedValidator : AbstractValidator<CategoryLocalizedModel>
    {
        public CategoryLocalizedValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Categories.Fields.Name.Required"));
        }
    }
}