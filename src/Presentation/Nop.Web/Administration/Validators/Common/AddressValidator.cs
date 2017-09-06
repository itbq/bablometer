using FluentValidation;
using Nop.Admin.Models.Common;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Common
{
    public class AddressValidator : AbstractValidator<AddressModel>
    {
        public AddressValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FirstName)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.FirstName.Required"))
                .When(x => x.FirstNameEnabled && x.FirstNameRequired);
            //RuleFor(x => x.Email)
            //    .NotNull()
            //    .WithMessage(localizationService.GetResource("Admin.Address.Fields.Email.Required"))
            //    .When(x => x.EmailEnabled && x.EmailRequired);
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.Email.Required"));
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"));
            RuleFor(x => x.CountryId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.Country.Required"))
                .When(x => x.CountryEnabled);
            RuleFor(x => x.CountryId)
                .NotEqual(0)
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.Country.Required"))
                .When(x => x.CountryEnabled);
            RuleFor(x => x.City)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.City.Required"))
                .When(x => x.CityEnabled && x.CityRequired);
            RuleFor(x => x.Address1)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.Address1.Required"))
                .When(x => x.StreetAddressEnabled && x.StreetAddressRequired);
            RuleFor(x => x.Address2)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.Address2.Required"))
                .When(x => x.StreetAddress2Enabled && x.StreetAddress2Required);
            RuleFor(x => x.ZipPostalCode)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.ZipPostalCode.Required"))
                .When(x => x.ZipPostalCodeEnabled && x.ZipPostalCodeRequired);
            //RuleFor(x => x.PhoneNumber)
            //    .NotNull()
            //    .WithMessage(localizationService.GetResource("Admin.Address.Fields.PhoneNumber.Required"))
            //    .When(x => x.PhoneEnabled && x.PhoneRequired);
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.PhoneNumber.Required"));
            RuleFor(x => x.FaxNumber)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.FaxNumber.Required"));
            RuleFor(x => x.Email1)
                .EmailAddress();
            RuleFor(x => x.Email2)
                .EmailAddress();
            RuleFor(x => x.Email3)
                .EmailAddress();
        }
    }
}