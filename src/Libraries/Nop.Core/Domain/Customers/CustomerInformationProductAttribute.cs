using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Customers
{
    public class CustomerInformationProductAttribute : BaseEntity
    {
        /// <summary>
        /// Customer field name for this attribute
        /// </summary>
        public virtual string CustomerFieldName { get; set; }

        /// <summary>
        /// Control type of add attribute to product
        /// </summary>
        public virtual CustomerInformationProductAddControlType ProductAddControlType
        {
            get
            {
                return (CustomerInformationProductAddControlType)this.ProductAddControlTypeId;
            }
            set
            {
                this.ProductAddControlTypeId = (int)value;
            }
        }

        /// <summary>
        /// Id of control type displayed when adding product
        /// </summary>
        public virtual int ProductAddControlTypeId { get; set; }

        /// <summary>
        /// Control type of search product by this attribute
        /// </summary>
        public virtual CustomerInformationProductSearchControlType ProductSearchControlType
        {
            get
            {
                return (CustomerInformationProductSearchControlType)this.ProductSearchControlTypeId;
            }
            set
            {
                this.ProductSearchControlTypeId = (int)value;
            }
        }

        /// <summary>
        /// Id of control type displayed when searching
        /// </summary>
        public virtual int ProductSearchControlTypeId { get; set; }

        /// <summary>
        /// If true, empty attribute values included iin search results
        /// </summary>
        public virtual bool IncludeEmptyValuesInSearchResults { get; set; }

        /// <summary>
        /// Is this attribute required
        /// </summary>
        public virtual bool IsRequired { get; set; }

        /// <summary>
        /// Display order of this attribute
        /// </summary>
        public virtual int DisplayOrder { get; set; }
    }
}
