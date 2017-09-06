using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Directory;

namespace Nop.Core.Domain.Catalog
{
    public partial class CategoryProductAttributeValue : BaseEntity, ILocalizedEntity
    {
        private ICollection<Product> _products;

        /// <summary>
        /// Gets or sets the product variant attribute mapping identifier
        /// </summary>
        public virtual int CategoryProductAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the product variant attribute name
        /// </summary>
        public virtual string Name { get; set; }      
      
        /// <summary>
        /// Gets or sets a value indicating whether the value is pre-selected
        /// </summary>
        public virtual bool IsPreSelected { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public virtual int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the color RGB value (used with "Color squares" attribute type)
        /// </summary> 
        public virtual string ColorSquaresRgb { get; set; }

        /// <summary>
        /// Gets or sets is this value a popular value 
        /// </summary>
        public virtual bool PopularValue { get; set; }
        /// <summary>
        /// Gets the product variant attribute
        /// </summary>
        public virtual CategoryProductAttribute CategoryProductAttribute { get; set; }

        /// <summary>
        /// Price currency id
        /// </summary>
        public virtual int? CurrencyId { get; set; }

        /// <summary>
        /// Numeric attribute value
        /// </summary>
        public virtual double? RealValue { get; set; }

        /// <summary>
        /// Maximum numeric attribute value for range values
        /// </summary>
        public virtual double? RealValueMax { get; set; }

        /// <summary>
        /// Price currency
        /// </summary>
        public virtual Currency Currency { get; set; }
        
        public virtual ICollection<Product> Products
        {
            get { return _products ?? (_products = new List<Product>()); }
            protected set { _products = value; }
        }
    }
}
