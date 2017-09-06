using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.Models.Customer
{
    public partial class LegalInformationModel
    {
        [NopResourceDisplayName("Profile.LegalInformation.LegalAddress")]
        [AllowHtml]
        public string LegalAddress { get; set; }

        [NopResourceDisplayName("Profile.LegalInfromation.TopExecutive")]
        [AllowHtml]
        public string TopExecutiveName { get; set; }

        [NopResourceDisplayName("Profile.LegalInfromation.TaxRegistration")]
        [AllowHtml]
        public string TaxRegistrationNumber { get; set; }

        [NopResourceDisplayName("Profile.LegalInformation.BankName")]
        [AllowHtml]
        public string BankName { get; set; }

        [NopResourceDisplayName("Profile.LegalInformation.BankAddress")]
        [AllowHtml]
        public string BankAddress { get; set; }

        [NopResourceDisplayName("Profile.LegalInformation.SWIFT")]
        [AllowHtml]
        public string SWIFT { get; set; }

        [NopResourceDisplayName("Profile.LegalInformation.AccountNumbers")]
        [AllowHtml]
        public string AccountNumbers { get; set; }
    }
}