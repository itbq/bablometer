using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Customers
{
    public class CompanyInformationSettings: ISettings
    {
        /// <summary>
        /// Max number of company contacts
        /// </summary>
        public int MaxContactNumber { get; set; }

        /// <summary>
        /// Max number of company documents
        /// </summary>
        public int MaxCompanyDocumentsCount { get; set; }

        /// <summary>
        /// Max number of company legal documents
        /// </summary>
        public int MaxLegalDocumentsCount { get; set; }

        /// <summary>
        /// Number of recently viiewed companies to display
        /// </summary>
        public int CompanyInformationViewNumber { get; set; }

        /// <summary>
        /// Gets or sets value indicatinng how many company information views to store in DB
        /// </summary>
        public int CompanyInformationMaxViewNumber { get; set; }

        /// <summary>
        /// Gets or sets number of products to display in recently added products
        /// </summary>
        public int RecentlyAddedCompanyProducts { get; set; }
    }
}
