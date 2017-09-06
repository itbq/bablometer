using Nop.Core.Domain.Localization;
using System.Collections.Generic;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product attribute
    /// </summary>
    public partial class ProductAttribute : BaseEntity, ILocalizedEntity
    {
        private ICollection<CategoryProductAttribute> _categoryProductAttribute;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Attribute image id
        /// </summary>
        public virtual int? PictureId { get; set; }
        public virtual ICollection<CategoryProductAttribute> CategoryProductAttribute
        {
            get { return _categoryProductAttribute ?? (_categoryProductAttribute = new List<CategoryProductAttribute>()); }
            protected set { _categoryProductAttribute = value; }
        }
    }
}
