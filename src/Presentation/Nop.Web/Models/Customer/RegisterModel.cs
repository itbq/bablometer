using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Customer;
using Nop.Web.Models.Media;
using Nop.Web.Models.Common;
using Nop.Web.Models.Regions;
using System;

namespace Nop.Web.Models.Customer
{
    [Validator(typeof(RegisterValidator))]
    public partial class RegisterModel : BaseNopModel
    {
        public RegisterModel()
        {
        }

        [NopResourceDisplayName("Account.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        public bool UsernamesEnabled { get; set; }
        [NopResourceDisplayName("Account.Fields.Username")]
        [AllowHtml]
        public string Username { get; set; }

        public bool CheckUsernameAvailabilityEnabled { get; set; }

        [DataType(DataType.Password)]
        [NopResourceDisplayName("Account.Fields.Password")]
        [AllowHtml]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [NopResourceDisplayName("Account.Fields.ConfirmPassword")]
        [AllowHtml]
        public string ConfirmPassword { get; set; }

        //form fields & properties
        [NopResourceDisplayName("Account.Fields.Gender")]
        public string Gender { get; set; }

        [NopResourceDisplayName("Account.Fields.FirstName")]
        [AllowHtml]
        public string FirstName { get; set; }

        [NopResourceDisplayName("Account.Fields.LastName")]
        [AllowHtml]
        public string LastName { get; set; }


        public bool DateOfBirthEnabled { get; set; }
        [NopResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthDay { get; set; }
        [NopResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthMonth { get; set; }
        [NopResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthYear { get; set; }
            
        public bool AcceptPrivacyPolicyEnabled { get; set; }

        public bool DisplayCaptcha { get; set; }


        [NopResourceDisplayName("Account type")]
        [AllowHtml]
        public string AccountType { get; set; }

        [NopResourceDisplayName("ITB.Portal.Register.Income")]
        [AllowHtml]
        public int? Income { get; set; }

        [NopResourceDisplayName("ITB.Portal.Register.Region")]
        public int RegionId { get; set; }

        [NopResourceDisplayName("ITB.Portal.Register.City")]
        public int CityId { get; set; }

        [NopResourceDisplayName("ITB.Portal.Register.Index")]
        [RegularExpression(@"\b[0-9]{6}\b", ErrorMessage = "Должно быть 6 цифр")]
        [AllowHtml]
        public string Index { get; set; }

        [NopResourceDisplayName("ITB.Portal.Register.Address")]
        [AllowHtml]
        public string Address { get; set; }

        public IList<RegionModel> Regions { get; set; }

        public IList<CityModel> Cities { get; set; }

        [NopResourceDisplayName("ITB.Portal.Register.Birthday")]
        [UIHint("DateNullable")]
        public DateTime? BirthdayDate { get; set; }

        public IList<CurrencyModel> AviableCurrencies { get; set; }

        public int CurrencyId { get; set; }
    }
}