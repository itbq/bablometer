using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Models.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Validators.Customer
{
    public class MyProfileValidator:AbstractValidator<ProfileModel>
    {
        public MyProfileValidator(ILocalizationService localizationService, CustomerSettings customerSettings)
        {
            //RuleFor(x => x.Income)
            //    .LessThanOrEqualTo(4000000)
            //    .WithMessage(localizationService.GetResource("ITB.Portal.Profile.Income.Max"));
            RuleFor(x => x.NewPassword)
                .Must(y=>{
                    if (y != null)
                    {
                        return y.Length >= customerSettings.PasswordMinLength && y.Length < 999;
                    }
                    else
                    {
                        return true;
                    }
                })
                .WithMessage(string.Format(localizationService.GetResource("Account.Fields.Password.LengthValidation"), customerSettings.PasswordMinLength));
            RuleFor(model => model.ConfirmNewPassword)
                .Equal(model => model.NewPassword)
                .WithMessage(localizationService.GetResource("Account.Fields.Password.EnteredPasswordsDoNotMatch"));
        }
    }
}