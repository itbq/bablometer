using Nop.Admin.Models.Directory;
using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.Catalog
{
    public class EditProductAttributeListModel
    {
        public IList<EditProductAttributeModel> AttributeList { get; set; }
        public IList<CurrencyModel> AviableCurrencies { get; set; }
        /// <summary>
        /// Product id
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Category id
        /// </summary>
        public int CategoryId { get; set; }
    }
    public class EditProductAttributeModel
    {
        /// <summary>
        /// Attribute Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Attribute name
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        /// Attribute control type
        /// </summary>
        public AttributeControlType AttributeControlType { get; set; }

        public int AttributeControlTypeId { get; set; }
        /// <summary>
        /// AttributeValue
        /// </summary>
        public string AttributeValue { get; set; }

        public int CurrencyId { get; set; }

        public string AdditionalValue { get; set; }

        /// <summary>
        /// Id of attribute value
        /// </summary>
        public int AttributeValueId { get; set; }

        public string ValidationMessage { get; set; }

        public string AttributeValueMax { get; set; }
        /// <summary>
        /// Aviable attribute values
        /// </summary>
        public IList<EditProductAttributeValueModel> AttributeValues { get; set; }
    }

    public class EditProductAttributeValueModel
    {
        public int Id { get; set; }
        public string AttributeValue { get; set; }
        public int ReferencedValue { get; set; }
        public bool Selected { get; set; }
    }
}