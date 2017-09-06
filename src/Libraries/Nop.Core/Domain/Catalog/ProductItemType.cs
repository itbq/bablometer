using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product item type
    /// </summary>
    public partial class ProductItemType : BaseEntity
    {
        /// <summary>
        /// Gets or sets the ite type name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the view path
        /// </summary>
        public virtual string ViewPath { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public virtual int DisplayOrder { get; set; }
    }

    public enum ProductItemTypeEnum
    {
        Product = 1,
        Service = 2,
        ProductBuyingRequest = 5,
        ServiceBuyingRequest = 6
    }
}
