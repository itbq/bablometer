using Nop.Admin.Models.CustomerInformationAttributes;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.CustomerInformationAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Services.Localization;
namespace Nop.Admin.Controllers
{
    public class CustomerInformationAttributeController : BaseNopController
    {
        private readonly ICustomerInformationAttributeService _customerInformationAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        public CustomerInformationAttributeController(ICustomerInformationAttributeService customerInformationAttributeService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            this._customerInformationAttributeService = customerInformationAttributeService;
            this._localizationService = localizationService;
            this._workContext = workContext;
        }

        public ActionResult List()
        {
            var list = new GridModel<CustomerInformationAttributeModel>();
            var fieldNames = new List<CustomerInformationAttributeModel>();
            var properties = typeof(Customer).GetProperties()
                .Where(prop => prop.IsDefined(typeof(CustomerFieldIsInProductAttribute), false));
            foreach (var property in properties)
            {
                string alias = property.GetCustomAttribute<CustomerFieldIsInProductAttribute>().Alias;
                var attribute = _customerInformationAttributeService.GetByCustomerFieldName(property.Name);
                if (attribute == null)
                {
                    attribute = _customerInformationAttributeService.FillValues(property.Name);
                    fieldNames.Add(attribute.ToModel(alias, _localizationService,_workContext));
                }
                else
                {
                    _customerInformationAttributeService.FillValues(attribute.CustomerFieldName);
                    fieldNames.Add(attribute.ToModel(alias, _localizationService, _workContext));
                }
            }

            list.Data = fieldNames;
            list.Total = fieldNames.Count;
            return View(list);
        }

        [HttpPost,GridAction]
        public ActionResult List(GridCommand command)
        {
            var list = new GridModel<CustomerInformationAttributeModel>();
            var fieldNames = new List<CustomerInformationAttributeModel>();
            var properties = typeof(Customer).GetProperties()
                .Where(prop => prop.IsDefined(typeof(CustomerFieldIsInProductAttribute), false));
            foreach (var property in properties)
            {
                string alias = property.GetCustomAttribute<CustomerFieldIsInProductAttribute>().Alias;
                var attribute = _customerInformationAttributeService.GetByCustomerFieldName(property.Name);
                if (attribute == null)
                {
                    attribute = _customerInformationAttributeService.FillValues(property.Name);
                    fieldNames.Add(attribute.ToModel(alias, _localizationService, _workContext));
                }
                else
                {
                    _customerInformationAttributeService.FillValues(attribute.CustomerFieldName);
                    fieldNames.Add(attribute.ToModel(alias, _localizationService, _workContext));
                }
            }

            list.Data = fieldNames;
            list.Total = fieldNames.Count;
            return new JsonResult()
            {
                Data = list
            };
        }

        [HttpPost, GridAction]
        public ActionResult Update(GridCommand command, CustomerInformationAttributeModel model)
        {
            var attribute = _customerInformationAttributeService.GetAttributeById(model.Id);
            attribute.ProductAddControlTypeId = Int32.Parse(model.ProductAddControlTypeString);
            attribute.ProductSearchControlTypeId = Int32.Parse(model.ProductSearchControlTypeString);
            attribute.IsRequired = model.IsRequired;
            attribute.IncludeEmptyValuesInSearchResults = model.IncludeEmptyValuesInSearchResults;
            attribute.DisplayOrder = model.DisplayOrder;
            _customerInformationAttributeService.UpdateAttribute(attribute);

            return List(command);
        }
    }
}
