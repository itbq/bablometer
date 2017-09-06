using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Catalog
{
    public partial class CategoryProductAttributeGroup : BaseEntity, ILocalizedEntity
    {
        private ICollection<CategoryProductAttribute> _categoryProductAttributes;
        private ICollection<ConversionImage> _conversionImages;

        private ICollection<CategoryToCategoryProductAttributeGroup> _categoryToCategoryProductAttributeGroup;
        /// <summary>
        /// Gets or sets name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets descripton
        /// </summary>
        public virtual string Description { get; set; }       

        /// <summary>
        /// Gets the product variant attribute values
        /// </summary>
        public virtual ICollection<CategoryProductAttribute> CategoryProductAttributes
        {
            get { return _categoryProductAttributes ?? (_categoryProductAttributes = new List<CategoryProductAttribute>()); }
            protected set { _categoryProductAttributes = value; }
        }

        /// <summary>
        /// Gets the product category to category product attribute groups
        /// </summary>
        public virtual ICollection<CategoryToCategoryProductAttributeGroup> CategoryToCategoryProductAttributeGroups
        {
            get { return _categoryToCategoryProductAttributeGroup ?? (_categoryToCategoryProductAttributeGroup = new List<CategoryToCategoryProductAttributeGroup>()); }
            protected set { _categoryToCategoryProductAttributeGroup = value; }
        }

        /// <summary>
        /// Gets the product category group to conversion images 
        /// </summary>
        public virtual ICollection<ConversionImage> ConversionImages
        {
            get { return _conversionImages ?? (_conversionImages = new List<ConversionImage>()); }
            protected set { _conversionImages = value; }
        }
    }
}
