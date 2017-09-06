using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Customers
{
    public partial class CompanyInformationView:BaseEntity
    {
        /// <summary>
        /// Gets or sets viewed company infromation id 
        /// </summary>
        public virtual int CompanyInformationId { get; set; }

        /// <summary>
        /// Gets or sets Viewed company information
        /// </summary>
        public virtual CompanyInformation CompanyInformation { get; set; }

        /// <summary>
        /// Get or sets id of customer id that viewd company profile
        /// </summary>
        public virtual int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets customer that viewd company profile
        /// </summary>
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// Gets or sets last view date
        /// </summary>
        public virtual DateTime LastViewOnUtc { get; set; }
    }
}
