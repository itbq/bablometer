using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Favorit
{
    public partial class FavoritItem:BaseEntity
    {
        /// <summary>
        /// Get or set customer id
        /// </summary>
        public virtual int CustomerId { get; set; }

        /// <summary>
        /// Get or set Customer
        /// </summary>
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// Get or set product id
        /// </summary>
        public virtual int ProductId { get; set; }

        /// <summary>
        /// Get or set Product
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// get or set creation date
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }
}
