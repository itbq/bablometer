using Nop.Core.Domain.Customers;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Admin.Models.Customers
{
    public partial class CompanyInformationModel:BaseNopEntityModel,ILocalizedModel<CompanyInformationLocalizedModel>
    {
        [NopResourceDisplayName("Company Name")]
        [AllowHtml]
        public string CompanyName { get; set; }

        [NopResourceDisplayName("Company Description")]
        [AllowHtml]
        public string CompanyDescription { get; set; }

        [NopResourceDisplayName("Legal Address")]
        [AllowHtml]
        public string LegalAddress { get; set; }

        [NopResourceDisplayName("Top Executive Name")]
        [AllowHtml]
        public string TopExecutiveName { get; set; }

        [NopResourceDisplayName("Tax Registration Number")]
        [AllowHtml]
        public string TaxRegistrationNumber { get; set; }

        [NopResourceDisplayName("Bank Name")]
        [AllowHtml]
        public string BankName { get; set; }

        [NopResourceDisplayName("Bank Address")]
        [AllowHtml]
        public string BankAddress { get; set; }

        [NopResourceDisplayName("SWIFT")]
        [AllowHtml]
        public string SWIFT { get; set; }

        [NopResourceDisplayName("Account Numbers")]
        [AllowHtml]
        public string AccountNumbers { get; set; }

        [NopResourceDisplayName("Company Name")]
        [AllowHtml]
        public virtual Customer Customer { get; set; }

        [NopResourceDisplayName("Admin.Customer.PublicUrl")]
        public virtual string PublicProfileUrl { get; set; }

        public IList<CompanyInformationLocalizedModel> Locales { get; set; }
    }

    public partial class CompanyInformationLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Company Name")]
        [AllowHtml]
        public string CompanyName { get; set; }

        [NopResourceDisplayName("Company Description")]
        [AllowHtml]
        public string CompanyDescription { get; set; }

        [NopResourceDisplayName("Legal Address")]
        [AllowHtml]
        public string LegalAddress { get; set; }

        [NopResourceDisplayName("Top Executive Name")]
        [AllowHtml]
        public string TopExecutiveName { get; set; }

        [NopResourceDisplayName("Tax Registration Number")]
        [AllowHtml]
        public string TaxRegistrationNumber { get; set; }

        [NopResourceDisplayName("Bank Name")]
        [AllowHtml]
        public string BankName { get; set; }

        [NopResourceDisplayName("Bank Address")]
        [AllowHtml]
        public string BankAddress { get; set; }

        [NopResourceDisplayName("SWIFT")]
        [AllowHtml]
        public string SWIFT { get; set; }

        [NopResourceDisplayName("Account Numbers")]
        [AllowHtml]
        public string AccountNumbers { get; set; }
    }
}