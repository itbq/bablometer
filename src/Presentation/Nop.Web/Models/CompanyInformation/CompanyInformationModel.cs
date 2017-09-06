using Nop.Core;
using Nop.Core.Domain.News;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Common;
using Nop.Web.Models.Customer;
using Nop.Web.Models.Media;
using Nop.Web.Models.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.Models.CompanyInformation
{
    public partial class CompanyInformationModel:BaseNopEntityModel
    {
        [NopResourceDisplayName("Profile.Company.Name")]
        [AllowHtml]
        public string CompanyName { get; set; }

        [NopResourceDisplayName("Profile.Company.Description")]
        [AllowHtml]
        public string CompanyDescription { get; set; }

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

        public string CompanySeName { get; set; }
        public PictureModel Picture { get; set; }

        public int CustomerId { get; set; }
        public Nop.Core.Domain.Customers.Customer Customer {get; set;} 
        public bool Seller { get; set; }

        public string SeName { get; set; }

        public IPagedList<NewsItem> CompanyNews { get; set; }
        public IList<UploadModel> CompanyDocuments { get; set; }
        public IList<UploadModel> LegalDocumennts { get; set; }
    }
}