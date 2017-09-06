using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Catalog
{
    public partial class CategoryProductAttribute : BaseEntity, ILocalizedEntity
    {
        private ICollection<CategoryProductAttributeValue> _categoryProductAttributeValues;

        /// <summary>
        /// Gets or sets the product variant identifier
        /// </summary>
        public virtual int CategoryProductGroupId { get; set; }

        /// <summary>
        /// Gets or sets the product attribute identifier
        /// </summary>
        public virtual int ProductAttributeId { get; set; }

        /// <summary>
        /// Gets or sets a value a text prompt
        /// </summary>
        public virtual string TextPrompt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is required
        /// </summary>
        public virtual bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets value indicating that this attribute is main
        /// </summary>
        public virtual bool MainAttribute { get; set; }

        /// <summary>
        /// Display this attribute in product box in catalogue
        /// </summary>
        public virtual bool ProductBoxAttribute { get; set; }

        /// <summary>
        /// Display this attribute in toddler block
        /// </summary>
        public virtual bool AdditionalAttribute { get; set; }

        /// <summary>
        /// Gets or sets the attribute control type identifier
        /// </summary>
        public virtual int AttributeControlTypeId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public virtual int DisplayOrder { get; set; }

        /// <summary>
        /// Gets the attribute control type
        /// </summary>
        public virtual AttributeControlType AttributeControlType
        {
            get
            {
                return (AttributeControlType)this.AttributeControlTypeId;
            }
            set
            {
                this.AttributeControlTypeId = (int)value;
            }
        }

        /// <summary>
        /// Attribute control type id for product search
        /// </summary>
        public virtual int SearchControlTypeId {get; set;}

        /// <summary>
        /// Attribute control type for product search
        /// </summary>
        public virtual SearchAttributeControlType SearchControlType
        {
            get
            {
                return (SearchAttributeControlType)this.SearchControlTypeId;
            }
            set
            {
                this.SearchControlTypeId = (int)value;
            }
        }


        /// <summary>
        /// Gets the product attribute
        /// </summary>
        public virtual ProductAttribute ProductAttribute { get; set; }

        /// <summary>
        /// Gets the product variant
        /// </summary>
        public virtual CategoryProductAttributeGroup CategoryProductGroup { get; set; }

        /// <summary>
        /// Gets the product variant attribute values
        /// </summary>
        public virtual ICollection<CategoryProductAttributeValue> CategoryProductAttributeValues
        {
            get { return _categoryProductAttributeValues ?? (_categoryProductAttributeValues = new List<CategoryProductAttributeValue>()); }
            protected set { _categoryProductAttributeValues = value; }
        }

    }
}
