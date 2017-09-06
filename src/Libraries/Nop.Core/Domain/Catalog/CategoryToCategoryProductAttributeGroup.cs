using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Catalog
{
    public partial class CategoryToCategoryProductAttributeGroup : BaseEntity, ILocalizedEntity
    {    
        /// <summary>
        /// Gets or sets the product variant identifier
        /// </summary>
        public virtual int CategoryProductAttributeGroupId { get; set; }

        /// <summary>
        /// Gets or sets the product attribute identifier
        /// </summary>
        public virtual int CategoryId { get; set; }       

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public virtual int DisplayOrder { get; set; }

       
        /// <summary>
        /// Gets the product attribute
        /// </summary>
        public virtual Category Category{ get; set; }

        /// <summary>
        /// Gets the product variant
        /// </summary>
        public virtual CategoryProductAttributeGroup CategoryProductAttributeGroup { get; set; }

      

    }
}
