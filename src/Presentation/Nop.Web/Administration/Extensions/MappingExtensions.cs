using Nop.Admin.Models.CustomerInformationAttributes;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Extensions
{
    public static class MappingExtensions
    {
        public static CustomerInformationAttributeModel ToModel(this CustomerInformationProductAttribute attribute, string alias, ILocalizationService localizationService, IWorkContext _workContext)
        {
            var model = new CustomerInformationAttributeModel();
            model.Id = attribute.Id;
            model.Alias = alias;
            model.DisplayOrder = attribute.DisplayOrder;
            model.IncludeEmptyValuesInSearchResults = attribute.IncludeEmptyValuesInSearchResults;
            model.IsRequired = attribute.IsRequired;
            model.ProductAddControlTypeId = attribute.ProductAddControlTypeId;
            model.ProductAddControlType = attribute.ProductAddControlType;
            model.ProductAddControlTypeString = attribute.ProductAddControlType.GetLocalizedEnum(localizationService, _workContext);
            model.ProductSearchControlType = attribute.ProductSearchControlType;
            model.ProductSearchControlTypeId = attribute.ProductSearchControlTypeId;
            model.ProductSearchControlTypeString = attribute.ProductSearchControlType.GetLocalizedEnum(localizationService, _workContext);

            return model;
        }
    }
}