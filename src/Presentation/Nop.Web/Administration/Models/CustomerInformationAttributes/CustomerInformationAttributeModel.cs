using Nop.Core.Domain.Customers;
using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.CustomerInformationAttributes
{
    public class CustomerInformationAttributeModel 
    {
        public int Id { get; set; }

        [NopResourceDisplayName("BSFA.Customer.Attribute.Alias")]
        public string Alias { get; set; }
        
        public int ProductAddControlTypeId { get; set; }
        
        public int ProductSearchControlTypeId { get; set; }
        public CustomerInformationProductAddControlType ProductAddControlType { get; set; }

        [NopResourceDisplayName("BSFA.Customer.Attribute.AddProductType")]
        [UIHint("CustomerAttributeControlType")]
        public string ProductAddControlTypeString { get; set; }
        public CustomerInformationProductSearchControlType ProductSearchControlType { get; set; }

        [NopResourceDisplayName("BSFA.Customer.Attribute.SearchControlType")]
        [UIHint("CustomerSearchAttributeControlType")]
        public string ProductSearchControlTypeString { get; set; }

        [NopResourceDisplayName("BSFA.Customer.Attribute.IncludeEmptyValues")]
        public bool IncludeEmptyValuesInSearchResults { get; set; }

        [NopResourceDisplayName("BSFA.Customer.Attribute.IsRequred")]
        public bool IsRequired { get; set; }

        [NopResourceDisplayName("BSFA.Customer.Attribute.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}