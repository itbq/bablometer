using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Areas.MiniSite.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Areas.MiniSite.Validators
{
    public class ContactsModelValidator : AbstractValidator<ContactUsModel>
    {
        public ContactsModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("ContactUs.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
            RuleFor(x => x.FullName).NotEmpty().WithMessage(localizationService.GetResource("ContactUs.FullName.Required"));
            RuleFor(x => x.Enquiry).NotEmpty().WithMessage(localizationService.GetResource("ContactUs.Enquiry.Required"));
            //RuleFor(x => x.Company).NotEmpty().WithMessage(localizationService.GetResource("ContactUs.Company.Validation"));
        }
    }
}