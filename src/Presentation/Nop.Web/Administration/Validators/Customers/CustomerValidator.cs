using FluentValidation;
using Nop.Admin.Models.Customers;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Customers
{
    public class CustomerValidator : AbstractValidator<CustomerModel>
    {
        public CustomerValidator(ILocalizationService localizationService, CustomerSettings customerSettings)
        {
            //form fields
            if (customerSettings.StreetAddressRequired && customerSettings.StreetAddressEnabled) 
                RuleFor(x => x.StreetAddress).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.StreetAddress.Required"));
            RuleFor(x => x.Income)
                .LessThan(4000000)
                .WithMessage(localizationService.GetResource("ITB.Portal.Profile.Income.Max"));
        }
    }
}