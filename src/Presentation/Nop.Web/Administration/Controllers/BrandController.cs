using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models.Brand;
using Nop.Core.Domain.BrandDomain;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class BrandController : BaseNopController
    {
          #region Fields

        private readonly IBrandService _brandService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;

        #endregion Fields

        #region Constructors

        public BrandController(IBrandService brandService,
            ILanguageService languageService, ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService, ICustomerActivityService customerActivityService,
            IPermissionService permissionService,
            IProductService productService)
        {
            this._brandService = brandService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._productService = productService;
        }

        #endregion
        
        #region Utilities

        [NonAction]
        protected void UpdateLocales(Brand productAttribute, BrandModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(productAttribute,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(productAttribute,
                                                           x => x.Description,
                                                           localized.Description,
                                                           localized.LanguageId);
            }
        }
        
        #endregion
        
        #region Methods

        //list
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productAttributes = _brandService.GetAllBrands();
            var gridModel = new GridModel<BrandModel>
            {
                Data = productAttributes.Select(x =>
                {
                    var model = x.ToModel();
                    return model;
                }),
                Total = productAttributes.Count()
            };
            return View(gridModel);
        }

        [HttpPost, GridAction]
        public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productAttributes = _brandService.GetAllBrands();
            var gridModel = new GridModel<BrandModel>
            {
                Data = productAttributes.Select(x =>
                {
                    var model = x.ToModel();
                    return model;
                }),
                Total = productAttributes.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }
        
        //create
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var model = new BrandModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(BrandModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var brand = model.ToEntity();
                brand.CreatedOnUtc = DateTime.UtcNow;
                _brandService.InsertBrand(brand);
                UpdateLocales(brand, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewBrand", _localizationService.GetResource("ActivityLog.AddNewBrand"), brand.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Brand.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = brand.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var brand = _brandService.GetBrandById(id);
            if (brand == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            var model = brand.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = brand.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = brand.GetLocalized(x => x.Description, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(BrandModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var brand = _brandService.GetBrandById(model.Id);
            if (brand == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");
            
            if (ModelState.IsValid)
            {
                brand = model.ToEntity(brand);
                _brandService.UpdateBrand(brand);

                UpdateLocales(brand, model);

                //activity log
                _customerActivityService.InsertActivity("EditBrand", _localizationService.GetResource("ActivityLog.EditBrand"), brand.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Brand.Updated"));
                return continueEditing ? RedirectToAction("Edit", brand.Id) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var brand = _brandService.GetBrandById(id);
            if (brand == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            _brandService.DeleteBrand(brand);

            //activity log
            _customerActivityService.InsertActivity("DeleteBrand", _localizationService.GetResource("ActivityLog.DeleteBrand"), brand.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Brand.Deleted"));
            return RedirectToAction("List");
        }


        [GridAction(EnableCustomBinding = true)]
        public ActionResult BrandUpdate(BrandModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var brand = _brandService.GetBrandById(model.Id);
            brand.Name = model.Name;
            brand.IsApproved = model.IsApproved;
            _brandService.UpdateBrand(brand);
            brand.Name = model.Name;
            brand.IsApproved = model.IsApproved;
            _brandService.UpdateBrand(brand);
            return List(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult BrandDelete(int id, GridCommand command)
        {
            var brand = _brandService.GetBrandById(id);
            brand.Customers.Clear();
            _brandService.UpdateBrand(brand);
            _brandService.DeleteBrand(brand);

            return List(command);
        }
        #endregion

    }
}
