using FluentValidation;
using Nop.Admin.Models.Catalog;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Catalog
{
    public class ProductValidator : AbstractValidator<ProductModel>
    {
        public ProductValidator(ILocalizationService localizationService)
        {
            RuleFor(x=>x.OrderLink).NotEmpty().WithMessage(localizationService.GetResource("ITB.Admin.Product.OrderLink.Required"));
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("ITB.Admin.Product.TitleRequired"));
            RuleFor(x => x.ShortDescription).NotEmpty().WithMessage(localizationService.GetResource("ITB.Admin.Product.ShortRequired"));
            RuleFor(x => x.FullDescription).NotEmpty().WithMessage(localizationService.GetResource("ITB.Admin.Product.FullRequired"));
            RuleFor(x => x.BankId).NotEmpty().WithMessage(localizationService.GetResource("ITB.Admin.Product.FullRequired"));
            RuleFor(x => x.Rating).LessThanOrEqualTo(5).WithMessage(localizationService.GetResource("ITBSFA.Admin.Rating.LessThan"));
        }
    }
}