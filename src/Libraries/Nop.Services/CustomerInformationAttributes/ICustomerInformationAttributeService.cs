using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.CustomerInformationAttributes
{
    public interface ICustomerInformationAttributeService
    {
        CustomerInformationProductAttribute GetAttributeById(int id);
        void InsertAttribute(CustomerInformationProductAttribute attribute);
        void UpdateAttribute(CustomerInformationProductAttribute attribute);
        void DeleteAttribute(CustomerInformationProductAttribute attribute);
        CustomerInformationProductAttribute GetByCustomerFieldName(string fieldName);
        CustomerInformationProductAttributeValue GetAttributeValueById(int id);
        void InsertAttributeValue(CustomerInformationProductAttributeValue attributeValue);
        void UpdateAttributeValue(CustomerInformationProductAttributeValue attributeValue);
        void DeleteAttributeValue(CustomerInformationProductAttributeValue attributeValue);
        CustomerInformationProductAttribute FillValues(string fieldName);
        IList<CustomerInformationProductAttributeValue> GetAttributeValuesForAddProduct(int attributeId);
        IList<CustomerInformationProductAttributeValue> GetAttributeValuesForSearchProduct(int attributeId);
        IList<CustomerInformationProductAttributeValue> GetValuesByAttributeId(int id);
        IList<CustomerInformationProductAttribute> GetAllAttributes();
        ReferenceValue GetValue(int attributeValueId);
        ReferenceValue GetCustomerAttribute(int attributeId, Customer customer);
    }
}
