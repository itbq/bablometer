using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Seo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Customers
{
    public partial class CompanyInformation:BaseEntity,ILocalizedEntity,ISlugSupported
    {

        private ICollection<Download> _downloads;
        private ICollection<Customer> _customers;
        private ICollection<CompanyInformationView> _companyInformationViews;

        /// <summary>
        /// Company name
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Company description
        /// </summary>
        public string CompanyDescription { get; set; }

        /// <summary>
        /// Company Legal address
        /// </summary>
        public string LegalAddress { get; set; }

        /// <summary>
        /// Compcany top executive name
        /// </summary>
        public string TopExecutiveName { get; set; }

        /// <summary>
        /// Company tax registration number
        /// </summary>
        public string TaxRegistrationNumber { get; set; }

        /// <summary>
        /// Company bank name
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// Company bank address
        /// </summary>
        public string BankAddress { get; set; }

        /// <summary>
        /// Company SWIFT
        /// </summary>
        public string SWIFT { get; set; }

        /// <summary>
        /// Company account numbers
        /// </summary>
        public string AccountNumbers { get; set; }

        /// <summary>
        /// Customer
        /// </summary>
        public virtual ICollection<Customer> Customers
        {
            get { return _customers ?? (_customers = new List<Customer>()); }
            protected set { _customers = value; }
        }

        /// <summary>
        /// Company documents collection
        /// </summary>
        public virtual ICollection<Download> CompanyDocuments
        {
            get { return _downloads ?? (_downloads = new List<Download>()); }
            protected set { _downloads = value; }
        }

        public virtual ICollection<CompanyInformationView> CompanyInformationViews
        {
            get { return _companyInformationViews ?? (_companyInformationViews = new List<CompanyInformationView>()); }
            protected set { _companyInformationViews = value; }
        }
    }
}
