using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.CustomerInformationAttributes
{
    public class CustomerInformationAttributeService : ICustomerInformationAttributeService
    {
        private readonly IRepository<CustomerInformationProductAttribute> _attributesRepository;
        private readonly IRepository<CustomerInformationProductAttributeValue> _attributeValueRepository;
        private readonly ICityService _cityService;
        private readonly ICurrencyService _currencyService;

        public CustomerInformationAttributeService(IRepository<CustomerInformationProductAttribute> attributesRepository,
            IRepository<CustomerInformationProductAttributeValue> attributeValueRepository,
            ICityService cityService,
            ICurrencyService currencyService)
        {
            this._attributesRepository = attributesRepository;
            this._attributeValueRepository = attributeValueRepository;
            this._cityService = cityService;
            this._currencyService = currencyService;
        }

        public CustomerInformationProductAttribute GetAttributeById(int id)
        {
            if (id == null)
                return null;

            return _attributesRepository.GetById(id);
        }

        public IList<CustomerInformationProductAttribute> GetAllAttributes()
        {
            return _attributesRepository.Table.ToList();
        }
        public void InsertAttribute(CustomerInformationProductAttribute attribute)
        {
            if(attribute == null)
                throw new ArgumentNullException("attribute");

            _attributesRepository.Insert(attribute);
        }


        public void UpdateAttribute(CustomerInformationProductAttribute attribute)
        {
            if(attribute == null)
                throw new ArgumentNullException("attribute");

            _attributesRepository.Update(attribute);
        }

        public void DeleteAttribute(CustomerInformationProductAttribute attribute)
        {
            if(attribute == null)
                throw new ArgumentNullException("attribute");

            _attributesRepository.Delete(attribute);
        }

        public CustomerInformationProductAttribute GetByCustomerFieldName(string fieldName)
        {
            if (String.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException("field name");
            return _attributesRepository.Table.Where(x => x.CustomerFieldName == fieldName).FirstOrDefault();
        }

        public CustomerInformationProductAttribute FillValues(string fieldName)
        {
            var attribute = _attributesRepository.Table.Where(x => x.CustomerFieldName == fieldName).FirstOrDefault();
            if (attribute == null)
            {
                CustomerInformationProductAttribute newAttribute = null;
                switch (fieldName)
                {
                    case "Gender":
                        {
                            newAttribute = new CustomerInformationProductAttribute()
                            {
                                CustomerFieldName = "Gender",
                                DisplayOrder = 0,
                                IncludeEmptyValuesInSearchResults = false,
                                IsRequired = false,
                                ProductAddControlTypeId = (int)CustomerInformationProductAddControlType.CheckBoxes,
                                ProductSearchControlTypeId = (int)CustomerInformationProductSearchControlType.Gender,
                            };
                            _attributesRepository.Insert(newAttribute);

                            var value = new CustomerInformationProductAttributeValue()
                            {
                                CustomerInformationProductAttributeId = newAttribute.Id,
                                ReferenceValueInt = 0,
                                ValueString = "Ж"
                            };
                            _attributeValueRepository.Insert(value);
                            value = new CustomerInformationProductAttributeValue()
                            {
                                CustomerInformationProductAttributeId = newAttribute.Id,
                                ReferenceValueInt = 1,
                                ValueString = "М"
                            };
                            _attributeValueRepository.Insert(value);
                            break;
                        }
                    case "Income":
                        {
                            newAttribute = new CustomerInformationProductAttribute()
                            {
                                CustomerFieldName = "Income",
                                DisplayOrder = 0,
                                IncludeEmptyValuesInSearchResults = false,
                                IsRequired = false,
                                ProductAddControlTypeId = (int)CustomerInformationProductAddControlType.NumberTextBoxValue,
                                ProductSearchControlTypeId = (int)CustomerInformationProductSearchControlType.NumberToddlerMore,
                            };
                            _attributesRepository.Insert(newAttribute);
                            break;
                        }
                    case "BirthdayDate":
                        {
                            newAttribute = new CustomerInformationProductAttribute()
                            {
                                CustomerFieldName = "BirthdayDate",
                                DisplayOrder = 0,
                                IncludeEmptyValuesInSearchResults = false,
                                IsRequired = false,
                                ProductAddControlTypeId = (int)CustomerInformationProductAddControlType.NumberTextBoxRange,
                                ProductSearchControlTypeId = (int)CustomerInformationProductSearchControlType.NumberTextBoxExact,
                            };
                            _attributesRepository.Insert(newAttribute);
                            break;
                        }
                    case "CityId":
                        {
                            newAttribute = new CustomerInformationProductAttribute()
                            {
                                CustomerFieldName = "CityId",
                                DisplayOrder = 0,
                                IncludeEmptyValuesInSearchResults = false,
                                IsRequired = false,
                                ProductAddControlTypeId = (int)CustomerInformationProductAddControlType.ReferenceValue,
                                ProductSearchControlTypeId = (int)CustomerInformationProductSearchControlType.DropDownList,
                            };
                            _attributesRepository.Insert(newAttribute);

                            var cities = _cityService.GetAllCities();
                            foreach (var city in cities)
                            {
                                var value = new CustomerInformationProductAttributeValue()
                                {
                                    CustomerInformationProductAttributeId = newAttribute.Id,
                                    ReferenceValueInt = city.Id
                                };
                                _attributeValueRepository.Insert(value);
                            }
                            break;
                        }
                }

                return newAttribute;
            }
            else
            {
                switch (attribute.CustomerFieldName)
                {
                    case "CityId":
                        {
                            var values = GetValuesByAttributeId(attribute.Id);
                            var cities = _cityService.GetAllCities();
                            foreach (var city in cities)
                            {
                                if (values.Where(x => x.ReferenceValueInt == city.Id).FirstOrDefault() == null)
                                {
                                    var value = new CustomerInformationProductAttributeValue()
                                    {
                                        CustomerInformationProductAttributeId = attribute.Id,
                                        ReferenceValueInt = city.Id
                                    };
                                    _attributeValueRepository.Insert(value);
                                }
                            }
                            break;
                        }
                }

                return attribute;
            }
        }

        public ReferenceValue GetValue(int attributeValueId)
        {
            var attributeValue = _attributeValueRepository.GetById(attributeValueId);
            if (attributeValue != null)
            {
                var value = new ReferenceValue();
                switch (attributeValue.CustomerInformationProductAttribute.CustomerFieldName)
                {
                    case "CityId":
                        {
                            var city = _cityService.GetById(attributeValue.ReferenceValueInt.Value);
                            if (city == null)
                                return null;

                            value.Id = city.Id;
                            value.Text = city.Title;
                            return value;
                        }
                    case "Gender":
                        {
                            if (attributeValue.ReferenceValueInt == 0)
                            {
                                value.Id = 0;
                                value.Text = "Ж";
                            }
                            else
                            {
                                value.Id = 1;
                                value.Text = "М";
                            }

                            return value;
                        }
                }
            }

            return null;
        }

        public ReferenceValue GetCustomerAttribute(int attributeId, Customer customer)
        {
            var attribute = _attributesRepository.GetById(attributeId);
            var referenceValue = new ReferenceValue();
            switch (attribute.CustomerFieldName)
            {
                case "Income":
                    {
                        if (customer.Income.HasValue)
                        {
                            var currency = _currencyService.GetCurrencyById(customer.CurrencyId.Value);
                            int value = (int)(customer.Income.Value * (double)currency.Rate);
                            referenceValue.Text = value.ToString();
                            referenceValue.Id = currency.Id;
                        }
                        break;
                    }
                case "BirthdayDate":
                    {
                        if(customer.BirthdayDate.HasValue)
                        {
                            DateTime today = DateTime.Today;
                            int age = today.Year - customer.BirthdayDate.Value.Year;
                            if (customer.BirthdayDate.Value > today.AddYears(-age)) age--;
                            referenceValue.Text = age.ToString();
                        }
                        break;
                    }
            }
            return referenceValue;
        }
        public IList<CustomerInformationProductAttributeValue> GetAttributeValuesForAddProduct(int attributeId)
        {
            var attribute = _attributesRepository.GetById(attributeId);
            switch (attribute.ProductAddControlType)
            {
                case CustomerInformationProductAddControlType.CheckBoxes:
                case CustomerInformationProductAddControlType.DropDownList:
                case CustomerInformationProductAddControlType.ReferenceValue:
                    {
                        return GetValuesByAttributeId(attributeId);
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        public IList<CustomerInformationProductAttributeValue> GetAttributeValuesForSearchProduct(int attributeId)
        {
            var attribute = _attributesRepository.GetById(attributeId);
            switch (attribute.ProductSearchControlType)
            {
                case CustomerInformationProductSearchControlType.Checkboxes:
                case CustomerInformationProductSearchControlType.DropDownList:
                case CustomerInformationProductSearchControlType.Gender:
                    {
                        return GetValuesByAttributeId(attributeId);
                    }
                default:
                    {
                        return null;
                    }
            }
        }
        public CustomerInformationProductAttributeValue GetAttributeValueById(int id)
        {
            if (id == null)
                return null;

            return _attributeValueRepository.GetById(id);
        }


        public void InsertAttributeValue(CustomerInformationProductAttributeValue attributeValue)
        {
            if(attributeValue == null)
                throw new ArgumentNullException("attribute value");

            _attributeValueRepository.Insert(attributeValue);
        }

        public void UpdateAttributeValue(CustomerInformationProductAttributeValue attributeValue)
        {
            if (attributeValue == null)
                throw new ArgumentNullException("attribute value");

            _attributeValueRepository.Update(attributeValue);
        }


        public void DeleteAttributeValue(CustomerInformationProductAttributeValue attributeValue)
        {
            if (attributeValue == null)
                throw new ArgumentNullException("attribute value");

            _attributeValueRepository.Delete(attributeValue);
        }

        public IList<CustomerInformationProductAttributeValue> GetValuesByAttributeId(int id)
        {
            if (id == 0)
                return new List<CustomerInformationProductAttributeValue>();
            return _attributeValueRepository.Table.Where(x => x.CustomerInformationProductAttributeId == id).ToList();
        }
    }

    public class ReferenceValue
    {
        public int Id {get; set;}
        public string Text {get; set;}
    }
}
