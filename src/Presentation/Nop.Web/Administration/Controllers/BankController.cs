using Nop.Admin.Models.Bank;
using Nop.Services.Customers;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Services.Media;
using Telerik.Web.Mvc;
using Nop.Services.Catalog;
using Nop.Core.Domain.Catalog;

namespace Nop.Admin.Controllers
{
    public class BankController : BaseNopController
    {
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;
        private readonly IPictureService _pictureService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly CustomerSettings _customerSettings;
        private readonly IProductService _productService;

        public BankController(ILocalizationService localizationService,
            ICustomerService customerService,
            IPictureService pictureService,
            ICustomerRegistrationService customerRegistrationService,
            CustomerSettings customerSettings,
            IProductService productService)
        {
            this._localizationService = localizationService;
            this._customerService = customerService;
            this._pictureService = pictureService;
            this._customerRegistrationService = customerRegistrationService;
            this._customerSettings = customerSettings;
            this._productService = productService;
        }

        public ActionResult List()
        {
            var model = new BankListModel();
            model.BankList = new GridModel<BankModel>();
            model.BankList.Data = _customerService.GetCustomersByCustomerRoleId(_customerService.GetCustomerRoleBySystemName("Sellers").Id)
                .Select(x=>new BankModel(){
                    Id = x.Id,
                    Email = x.Email,
                    BankTitle = x.CompanyName,
                    PictureId = x.ProviderLogoId.HasValue ? x.ProviderLogoId.Value: 0,
                    LogoUrl = x.ProviderLogoId.HasValue ? _pictureService.GetPictureUrl(x.ProviderLogoId.Value,100) : null,
                    Rating = x.Rating ?? 0
                }).ToList();

            model.BankList.Total = model.BankList.Data.Count();

            return View(model);
        }


        [HttpPost, GridAction]
        public ActionResult List(GridCommand command)
        {
            var model = new BankListModel();
            var bankList = _customerService.GetCustomersByCustomerRoleId(_customerService.GetCustomerRoleBySystemName("Sellers").Id);
            model.BankList = new GridModel<BankModel>();
            model.BankList.Data = bankList.Select(x => new BankModel()
                {
                    Id = x.Id,
                    BankTitle = x.CompanyName,
                    Email = x.Email,
                    PictureId = x.ProviderLogoId.HasValue ? x.ProviderLogoId.Value : 0,
                    LogoUrl = x.ProviderLogoId.HasValue ? _pictureService.GetPictureUrl(x.ProviderLogoId.Value, 100) : null,
                    Rating = x.Rating ?? 0
                }).ToList();

            model.BankList.Total = model.BankList.Data.Count();

            return new JsonResult()
            {
                Data = model.BankList
            };
        }

        [HttpPost, GridAction]
        public ActionResult Insert(BankModel model, GridCommand command)
        {
            ModelState.Remove("Id");
            if(!ModelState.IsValid)
            {
                var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }
            var customer = new Customer();
            customer.CreatedOnUtc = DateTime.UtcNow;
            customer.CustomerGuid = Guid.NewGuid();
            customer.LastActivityDateUtc = DateTime.UtcNow;
            _customerService.InsertCustomer(customer);

            var request = new CustomerRegistrationRequest(customer, model.Email, model.Email, "123456", _customerSettings.DefaultPasswordFormat);
            var result = _customerRegistrationService.RegisterCustomer(request);
            if (result.Success)
            {
                customer.CompanyName = model.BankTitle;
                customer.Email = model.Email;
                customer.Username = model.Email;
                if (model.Rating != 0)
                {
                    customer.Rating = model.Rating;
                }
                if(model.PictureId != 0)
                    customer.ProviderLogoId = model.PictureId;
                customer.CustomerRoles.Add(_customerService.GetCustomerRoleBySystemName("Sellers"));
                _customerService.UpdateCustomer(customer);
            }
            else
            {
                _customerService.DeleteCustomerPermamently(customer);
                foreach (var error in result.Errors)
                {
                    if (error == _localizationService.GetResource("Account.Register.Errors.EmailAlreadyExists"))
                    {
                        ModelState.AddModelError("Email", error);
                    }

                    ModelState.AddModelError("", error);
                }

                var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            return List(command);
        }

        [HttpPost,GridAction]
        public ActionResult UpdateBank(BankModel model, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var customer = _customerService.GetCustomerById(model.Id);
            customer.Email = model.Email;
            customer.Username = model.Email;
            customer.CompanyName = model.BankTitle;
            if (model.Rating != 0)
            {
                customer.Rating = model.Rating;
            }
            else
            {
                customer.Rating = null;
            }
            int oldPIctureId = 0;
            if (customer.ProviderLogoId.HasValue && customer.ProviderLogoId.Value != model.PictureId)
            {
                oldPIctureId = customer.ProviderLogoId.Value;
                if (model.PictureId != 0)
                {
                    customer.ProviderLogoId = model.PictureId;
                }
                else
                {
                    customer.ProviderLogoId = null;
                }
            }
            else
            {
                if(model.PictureId != 0)
                    customer.ProviderLogoId = model.PictureId;
            }
            _customerService.UpdateCustomer(customer);
            var picture = _pictureService.GetPictureById(oldPIctureId);
            if (picture != null)
                _pictureService.DeletePicture(picture);
            return List(command);
        }

        [HttpPost,GridAction]
        public ActionResult DeleteBank(GridCommand command, int Id)
        {
            var customer = _customerService.GetCustomerById(Id);
            if (customer != null)
            {
                int logoId = 0;
                if (customer.ProviderLogoId.HasValue && customer.ProviderLogoId.Value != 0)
                {
                    logoId = customer.ProviderLogoId.Value;
                    customer.ProviderLogoId = null;
                    _customerService.UpdateCustomer(customer);
                }

                var products = _productService.NewSearchProducts(customer.Id, 0, 0, ProductSortingEnum.CreatedOn, 0, short.MaxValue, 0, 0, 0);
                if (products.Count > 0)
                {
                    foreach (var product in products)
                    {
                        _productService.DeleteProduct(product);
                    }
                }
                _customerService.DeleteCustomer(customer);
                var picture = _pictureService.GetPictureById(logoId);
                if(picture != null)
                    _pictureService.DeletePicture(picture);
            }

            return List(command);
        }

    }
}
