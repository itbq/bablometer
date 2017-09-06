using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Customers
{
    public class CustomerInformationProductAttributeValue : BaseEntity
    {
        private ICollection<Product> _products;
        /// <summary>
        /// Attribute value string
        /// </summary>
        public virtual string ValueString { get; set; }

        /// <summary>
        /// Attribute value double
        /// </summary>
        public virtual double? ValueDouble { get; set; }

        /// <summary>
        /// max double value for range values
        /// </summary>
        public virtual double? ValueMax { get; set; }

        /// <summary>
        /// Id of value currency
        /// </summary>
        public virtual int? CurrencyId { get; set; }

        /// <summary>
        /// Attribute value that references some other table (region, city e.t.c.)
        /// </summary>
        public virtual int? ReferenceValueInt { get; set; }

        public virtual CustomerInformationProductAttribute CustomerInformationProductAttribute { get; set; }

        public virtual int CustomerInformationProductAttributeId { get; set; }
        
        public virtual ICollection<Product> Products
        {
            get
            {
                return this._products ?? (_products = new List<Product>());
            }
            set
            {
                this._products = value;
            }
        }
    }
}
