using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Catalog
{
    public partial class ProductView: BaseEntity
    {
        /// <summary>
        /// Customer id that viewed product
        /// </summary>
        public virtual int CustomerId { get; set; }
        
        /// <summary>
        /// Customer that viewed product
        /// </summary>
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// Gets or sets product id
        /// </summary>
        public virtual int ProductId { get; set; }

        /// <summary>
        /// Gets or setc Product
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// Date of last product view on UTC
        /// </summary>
        public virtual DateTime LastViewOnUtc { get; set; }
    }
}
