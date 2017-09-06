using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.UI.Captcha;
using Nop.Web.Models.Common;
using Nop.Web.Models.Customer;
using Nop.Services.Logging;
using System.IO;

namespace Nop.Web.Controllers
{
    public partial class CustomerController : BaseNopController
    {
        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        protected string RenderViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindView(ControllerContext, viewName,"");
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
        
        #region Add/DiplayContacts

        /// <summary>
        /// First add contact person block load
        /// </summary>
        /// <returns></returns>
        public PartialViewResult AddContactPerson(int? languageId)
        {
            var customer = _workContext.CurrentCustomer;
            var model = new CustomerAddressEditModel();
            model.Address = new AddressModel();
            model.Address.LanguageId = languageId;
            if (!languageId.HasValue)
            {
                model.Address.LanguageId = _workContext.WorkingLanguage.Id;
            }
            if (!customer.IsRegistered())
            {
                customer.Addresses.Clear();
                _customerService.UpdateCustomer(customer);
            }
            return PartialView(model);
        }

        /// <summary>
        /// Show add contact person block after submitting new contact
        /// </summary>
        /// <returns></returns>
        public PartialViewResult AddContactPersonPostback(int languageId)
        {
            return PartialView("AddContactPerson", new CustomerAddressEditModel() { Address = new AddressModel() { LanguageId = languageId } });
        }

        /// <summary>
        /// Adding new contact via AJAX request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult AddContactPerson(AddressModel model)
        {
            if (!model.LanguageId.HasValue)
            {
                model.LanguageId = _workContext.WorkingLanguage.Id;
            }
            var customer = _workContext.CurrentCustomer;
            if (model.Id != 0)
            {
                //find address (ensure that it belongs to the current customer)
                var address = customer.Addresses.Where(a => a.Id == model.Id).FirstOrDefault();

                if (ModelState.IsValid)
                {
                    address = model.ToEntity(address);
                    _addressService.UpdateAddress(address);

                }
                return AddContactPersonPostback(model.LanguageId.Value);
            }
            if (ModelState.IsValid)
            {
                var address = model.ToEntity();
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                customer.Addresses.Add(address);
                _customerService.UpdateCustomer(customer);
                return AddContactPersonPostback(model.LanguageId.Value);
            }
            return AddContactPersonPostback(model.LanguageId.Value);
        }

        /// <summary>
        /// Remove adress from contacts
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult RemoveAddress(int Id)
        {
            var customer = _workContext.CurrentCustomer;
            var address = _addressService.GetAddressById(Id);
            customer.Addresses.Remove(address);
            _customerService.UpdateCustomer(customer);
            _addressService.DeleteAddress(address);
            return null;
        }

        /// <summary>
        /// Show contact addresses
        /// </summary>
        /// <returns></returns>
        [NopHttpsRequirement(SslRequirement.Yes)]
        public PartialViewResult ContactAddresses(int? languageId)
        {
            var customer = _workContext.CurrentCustomer;
            
            var model = new CustomerAddressListModel();
            foreach (var address in customer.Addresses)
            {
                if (address.Email == null)
                {
                    _addressService.DeleteAddress(address);
                    continue;
                }
                var addressModel = new AddressModel();
                addressModel.PrepareModel(address, false, _addressSettings, _localizationService,
                    _stateProvinceService, () => _countryService.GetAllCountries(),languageId: languageId);
                model.Addresses.Add(addressModel);
            }
            model.MaxContactCount = _addressSettings.MaxContactCount;

            model.AvailableCountries = new List<SelectListItem>();
            model.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries())
            {
                model.AvailableCountries.Add(new SelectListItem() { Text = c.GetLocalized(x => x.Name), Value = c.Id.ToString() });
            }
            model.LanguageId = languageId.Value;
            return PartialView(model);
        }

        /// <summary>
        /// Show form to add contact address
        /// </summary>
        /// <returns></returns>
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult ContactAddressAdd()
        {
            var customer = _workContext.CurrentCustomer;

            var model = new CustomerAddressEditModel();
            model.NavigationModel = GetCustomerNavigationModel(customer);
            model.NavigationModel.SelectedTab = CustomerNavigationEnum.Addresses;
            model.Address.PrepareModel(null, false, _addressSettings, _localizationService,
                    _stateProvinceService, () => _countryService.GetAllCountries(), languageId: _workContext.WorkingLanguage.Id);
            return View(model);
        }

        /// <summary>
        /// Edit contact address
        /// </summary>
        /// <param name="id">address id</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult EditContact(int id)
        {
            var address = _addressService.GetAddressById(id);
            return Json(address,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Show uploaded documents
        /// </summary>
        /// <param name="isLegal">Indicatas what kind of company documents to show</param>
        /// <returns></returns>
        public PartialViewResult Uploads(bool isLegal)
        {
            var model = new UploadListModel();
            model.IsLegal = isLegal;
            model.Uploads = new List<UploadModel>();
            if (_workContext.CurrentCustomer.CompanyInformation == null)
            {
                _workContext.CurrentCustomer.CompanyInformation = new CompanyInformation();
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
            }
            model.Uploads = _workContext.CurrentCustomer.CompanyInformation.CompanyDocuments.Where(x => x.IsLegal == isLegal)
                   .Select(x => new UploadModel()
                   {
                       CompanyId = x.CompanyId.GetValueOrDefault(),
                       Id = x.Id,
                       DownloadId = x.DownloadGuid,
                       FileExtension = x.Extension,
                       FileSize = x.FileSize,
                       FileName = x.Filename,
                       IsLegal = x.IsLegal
                   }).ToList();
            return PartialView(model);
        }

        /// <summary>
        /// Show add document form
        /// </summary>
        /// <param name="isLegal">Indicates what kind of document would be added</param>
        /// <returns></returns>
        public ActionResult AddDocumentForm(bool isLegal)
        {
            var model = new UploadModel();
            model.IsLegal = isLegal;
            return View("AddDocument",model);
        }

        /// <summary>
        /// Add ducument to customer documents
        /// </summary>
        /// <param name="id">id of document to add</param>
        /// <param name="Legal">Indicates is current document is legal document</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddDocument(int id, bool Legal)
        {
            var customer = _workContext.CurrentCustomer;
            bool exceedMaxLimit = false;
            if (id != 0)
            {
                var download = _downloadService.GetDownloadById(id);
                if (_workContext.CurrentCustomer.CompanyInformation.CompanyDocuments.Where(x => x.IsLegal == Legal).Count() <= 4)
                {
                    download.IsLegal = Legal;
                    download.CompanyId = _workContext.CurrentCustomer.CompanyInformationId;
                    _downloadService.UpdateDownload(download);
                }
                else
                {
                    if (!_workContext.CurrentCustomer.IsRegistered())
                    {
                        exceedMaxLimit = true;
                        _downloadService.DeleteDownload(download);
                    }
                    else
                    {
                        exceedMaxLimit = true;
                        _downloadService.DeleteDownload(download);
                    }
                }
            }
            else
            {
                return PartialView(new UploadModel());
            }
            var model1 = new UploadListModel();
            model1.Uploads = customer.CompanyInformation.CompanyDocuments.Where(x => x.IsLegal == Legal)
                   .Select(x => new UploadModel()
                   {
                       CompanyId = x.CompanyId.GetValueOrDefault(),
                       Id = x.Id,
                       DownloadId = x.DownloadGuid,
                       FileExtension = x.Extension,
                       FileSize = x.FileSize,
                       FileName = x.Filename
                   }).ToList();
            model1.IsLegal = Legal;
            return Json (new
                {
                    htmlstr = RenderPartialViewToString("Uploads",model1),
                    IsLegal = Legal,
                    exceedMaxLimit = exceedMaxLimit
                });
        }
        #endregion


        [ChildActionOnly]
        public ActionResult CompanyContacts(int customerId)
        {
            var customer = _customerService.GetCustomerById(customerId);

            var model = new CustomerAddressListModel();
            foreach (var address in customer.Addresses)
            {
                if (address.Email == null)
                {
                    _addressService.DeleteAddress(address);
                    continue;
                }
                var addressModel = new AddressModel();
                addressModel.PrepareModel(address, false, _addressSettings, _localizationService,
                    _stateProvinceService, () => _countryService.GetAllCountries(), languageId: _workContext.WorkingLanguage.Id);
                model.Addresses.Add(addressModel);
            }

            return View(model);
        }
    }
}