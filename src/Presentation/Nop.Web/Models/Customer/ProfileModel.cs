using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Common;
using Nop.Web.Models.CompanyInformation;
using Nop.Web.Models.Regions;
using Nop.Web.Validators.Customer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.Models.Customer
{
    [Validator(typeof(MyProfileValidator))]
    public partial class ProfileModel : BaseNopEntityModel
    {
        public LanguageModel CurenntLanguage { get; set; }
        public LanguageModel DefaultUserLanguage { get; set; }

        [NopResourceDisplayName("Account.Fields.Company")]
        [AllowHtml]
        public string CompanyName { get; set; }
        public string CompanySeName { get; set; }

        [NopResourceDisplayName("Company Description")]
        [AllowHtml]
        public string CompanyDescription { get; set; }
        public CompanyInformationModel CompanyInformation { get; set; }
        public IList<LanguageModel> AviableLanguages { get; set; }

        [DataType(DataType.Password)]
        [NopResourceDisplayName("ITBSFA.Account.Fields.NewPassword")]
        [AllowHtml]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [NopResourceDisplayName("Account.Fields.ConfirmPassword")]
        [AllowHtml]
        public string ConfirmNewPassword { get; set; }
        public int Priority { get; set; }
        public string PictureUrl { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Company Logo")]
        [AllowHtml]
        public int PictureId { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public bool IsSeller { get; set; }
        public int CurrentLanguageId { get; set; }
        public int DefaultUserLanguageId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
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

        [NopResourceDisplayName("ITB.Portal.Register.Gender")]
        public string Gender { get; set; }

        [NopResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthDay { get; set; }
        [NopResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthMonth { get; set; }
        [NopResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthYear { get; set; }

        /// <summary>
        /// Does this customer use externa autentification record to log in
        /// </summary>
        public bool ExternalAutentificationRecord { get; set; }

        /// <summary>
        /// Aviable currencies
        /// </summary>
        public IList<CurrencyModel> AviableCurrencies { get; set; }

        /// <summary>
        /// Customer currency id
        /// </summary>
        public int CurrencyId { get; set; }
    }
}