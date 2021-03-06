﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Admin.Models.Catalog;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;
using Nop.Services.Seo;
using Nop.Services.Customers;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.BrandDomain;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Bank;
using Nop.Admin.Models.Directory;
using Nop.Services.Regions;
using Nop.Admin.Models.Regions;
using Nop.Services.CustomerInformationAttributes;
using System.Reflection;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public partial class ProductController : BaseNopController
    {
		#region Fields

        private readonly IProductService _productService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IProductItemTypeService _productItemTypeService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ICustomerService _customerService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IPictureService _pictureService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IProductTagService _productTagService;
        private readonly ICopyProductService _copyProductService;
        private readonly IPdfService _pdfService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;
        private readonly PdfSettings _pdfSettings;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IBrandService _brandService;
        private readonly ICompanyInformationService _companyInformationService;
        private readonly IProductPriceService _productPriceService;
        private readonly ICategoryProductAttributeService _categoryProductAttributeService;
        private readonly IRegionService _regionService;
        private readonly ICustomerInformationAttributeService _customerInformationAttributeService;
        #endregion

		#region Constructors

        public ProductController(IProductService productService, 
            IProductTemplateService productTemplateService,
            IProductItemTypeService productItemTypeService,
            ICategoryService categoryService, IManufacturerService manufacturerService,
            ICustomerService customerService,
            IUrlRecordService urlRecordService, IWorkContext workContext, ILanguageService languageService, 
            ILocalizationService localizationService, ILocalizedEntityService localizedEntityService,
            ISpecificationAttributeService specificationAttributeService, IPictureService pictureService,
            ITaxCategoryService taxCategoryService, IProductTagService productTagService,
            ICopyProductService copyProductService, IPdfService pdfService,
            IExportManager exportManager, IImportManager importManager,
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService, IAclService aclService,
            ICurrencyService currencyService, CurrencySettings currencySettings,
            IMeasureService measureService, MeasureSettings measureSettings,
            PdfSettings pdfSettings, AdminAreaSettings adminAreaSettings,
            IBrandService brandService,
            ICompanyInformationService companyInformationService,
            IProductPriceService productPriceService,
            ICategoryProductAttributeService categoryProductAttributeService,
            IRegionService regionService,
            ICustomerInformationAttributeService customerInformationAttributeService)
        {
            this._productService = productService;
            this._productTemplateService = productTemplateService;
            this._productItemTypeService = productItemTypeService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._customerService = customerService;
            this._urlRecordService = urlRecordService;
            this._workContext = workContext;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._specificationAttributeService = specificationAttributeService;
            this._pictureService = pictureService;
            this._taxCategoryService = taxCategoryService;
            this._productTagService = productTagService;
            this._copyProductService = copyProductService;
            this._pdfService = pdfService;
            this._exportManager = exportManager;
            this._importManager = importManager;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._aclService = aclService;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._measureService = measureService;
            this._measureSettings = measureSettings;
            this._pdfSettings = pdfSettings;
            this._adminAreaSettings = adminAreaSettings;
            this._brandService = brandService;
            this._companyInformationService = companyInformationService;
            this._productPriceService = productPriceService;
            this._categoryProductAttributeService = categoryProductAttributeService;
            this._regionService = regionService;
            this._customerInformationAttributeService = customerInformationAttributeService;
        }

        #endregion 

        #region Utitilies

        [NonAction]
        private void UpdatePictureSeoNames(Product product)
        {
            foreach (var pp in product.ProductPictures)
                _pictureService.SetSeoFilename(pp.PictureId, _pictureService.GetPictureSeName(product.Name));
        }

        [NonAction]
        protected void UpdateProductTagTotals(Product product)
        {
            var productTags = product.ProductTags;
            foreach (var productTag in productTags)
                _productTagService.UpdateProductTagTotals(productTag);
        }
        
        [NonAction]
        private void PrepareTemplatesModel(ProductModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var templates = _productTemplateService.GetAllProductTemplates();
            foreach (var template in templates)
            {
                model.AvailableProductTemplates.Add(new SelectListItem()
                {
                    Text =  template.Name,
                    Value = template.Id.ToString()
                });
            }
        }

        [NonAction]
        private void PrepareItemTypeModel(ProductModel model)
        {
            if(model==null)
                throw new ArgumentNullException("model");

            var itemtypes = _productItemTypeService.GetAllProductItemTypes();
            if (model.ProductItemTypeId == (int)ProductItemTypeEnum.Product || model.ProductItemTypeId == (int)ProductItemTypeEnum.Service)
            {
                model.AvailableProductItemTypes.Add(new SelectListItem()
                {
                    Text = _localizationService.GetResource("ETF.Catalog.Product",_workContext.WorkingLanguage.Id),
                    Value = ((int)ProductItemTypeEnum.Product).ToString()
                });
                model.AvailableProductItemTypes.Add(new SelectListItem()
                {
                    Text = _localizationService.GetResource("ETF.Catalog.Service",_workContext.WorkingLanguage.Id),
                    Value = ((int)ProductItemTypeEnum.Service).ToString()
                });
            }else
            {
                model.AvailableProductItemTypes.Add(new SelectListItem()
                {
                    Text = _localizationService.GetResource("ETF.Catalog.ProductBuyingRequest", _workContext.WorkingLanguage.Id),
                    Value = ((int)ProductItemTypeEnum.ProductBuyingRequest).ToString()
                });
                model.AvailableProductItemTypes.Add(new SelectListItem()
                {
                    Text = _localizationService.GetResource("ETF.Catalog.ServiceBuyingRequest", _workContext.WorkingLanguage.Id),
                    Value = ((int)ProductItemTypeEnum.ServiceBuyingRequest).ToString()
                });
            }
        }

        [NonAction]
        private void PrepareAddSpecificationAttributeModel(ProductModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (model.AddSpecificationAttributeModel == null)
                model.AddSpecificationAttributeModel = new ProductModel.AddProductSpecificationAttributeModel();
            
            //attributes
            var specificationAttributes = _specificationAttributeService.GetSpecificationAttributes();
            for (int i = 0; i < specificationAttributes.Count; i++)
            {
                var sa = specificationAttributes[i];
                model.AddSpecificationAttributeModel.AvailableAttributes.Add(new SelectListItem() { Text = sa.Name, Value = sa.Id.ToString() });
                if (i == 0)
                {
                    //attribute options
                    foreach (var sao in _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(sa.Id))
                        model.AddSpecificationAttributeModel.AvailableOptions.Add(new SelectListItem() { Text = sao.Name, Value = sao.Id.ToString() });
                }
            }
        }

        [NonAction]
        private void PrepareAddProductPictureModel(ProductModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (model.AddPictureModel == null)
                model.AddPictureModel = new ProductModel.ProductPictureModel();
        }

        [NonAction]
        private void PrepareCategoryMapping(ProductModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.NumberOfAvailableCategories = _categoryService.GetAllCategories(showHidden: true).Count;
        }

        [NonAction]
        private void PrepareManufacturerMapping(ProductModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.NumberOfAvailableManufacturers = _manufacturerService.GetAllManufacturers(true).Count;
        }

        [NonAction]
        private void PrepareAclModel(ProductModel model, Product product, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableCustomerRoles = _customerService
                .GetAllCustomerRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();
            if (!excludeProperties)
            {
                if (product != null)
                {
                    model.SelectedCustomerRoleIds = _aclService.GetCustomerRoleIdsWithAccess(product);
                }
                else
                {
                    model.SelectedCustomerRoleIds = new int[0];
                }
            }
        }

        [NonAction]
        protected void SaveProductAcl(Product product, ProductModel model)
        {
            var existingAclRecords = _aclService.GetAclRecords(product);
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds != null && model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Where(acl => acl.CustomerRoleId == customerRole.Id).Count() == 0)
                        _aclService.InsertAclRecord(product, customerRole.Id);
                }
                else
                {
                    //removed role
                    var aclRecordToDelete = existingAclRecords.Where(acl => acl.CustomerRoleId == customerRole.Id).FirstOrDefault();
                    if (aclRecordToDelete != null)
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }

        [NonAction]
        private void PrepareVariantsModel(ProductModel model, Product product)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (product != null)
            {
                var variants = _productService.GetProductVariantsByProductId(product.Id, true);
                foreach (var variant in variants)
                {
                    var variantModel = variant.ToModel();
                    if (String.IsNullOrEmpty(variantModel.Name))
                        variantModel.Name = "Unnamed";
                    model.ProductVariantModels.Add(variantModel);
                }
            }
        }

        [NonAction]
        private void PrepareProductPictureThumbnailModel(ProductModel model, Product product)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (product != null)
            {
                if (product.Customer.ProviderLogoId.HasValue)
                {
                    var defaultProductPicture = _pictureService.GetPictureById(product.Customer.ProviderLogoId.Value);
                    model.PictureThumbnailUrl = _pictureService.GetPictureUrl(defaultProductPicture, 75, true);
                }
            }
        }

        [NonAction]
        private void PrepareCopyProductModel(ProductModel model, Product product)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (product != null)
            {
                model.CopyProductModel.Id = product.Id;
                model.CopyProductModel.Name = "Copy of " + product.Name;
                model.CopyProductModel.Published = true;
                model.CopyProductModel.CopyImages = true;
            }
        }

        [NonAction]
        private void PrepareTags(ProductModel model, Product product)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            if (product != null)
            {
                var result = new StringBuilder();
                var productTags = product.ProductTags;
                for (int i = 0; i < productTags.Count(); i++)
                {
                    var pt = productTags.ToList()[i];
                    result.Append(pt.Name);
                    if (i != productTags.Count() - 1)
                        result.Append(", ");
                }
                model.ProductTags = result.ToString();
            }
        }

        [NonAction]
        private void UpdateLocales(ProductTag productTag, ProductTagModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(productTag,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        [NonAction]
        private void FirstVariant_UpdateLocales(ProductVariant variant, ProductVariantModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(variant,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(variant,
                                                               x => x.Description,
                                                               localized.Description,
                                                               localized.LanguageId);
            }
        }
        
        [NonAction]
        private void FirstVariant_PrepareProductVariantModel(ProductVariantModel model, ProductVariant variant, bool setPredefinedValues)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            //tax categories
            var taxCategories = _taxCategoryService.GetAllTaxCategories();
            model.AvailableTaxCategories.Add(new SelectListItem() { Text = "---", Value = "0" });
            foreach (var tc in taxCategories)
                model.AvailableTaxCategories.Add(new SelectListItem() { Text = tc.Name, Value = tc.Id.ToString(), Selected = variant != null && !setPredefinedValues && tc.Id == variant.TaxCategoryId });

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name;
            model.BaseDimensionIn = _measureService.GetMeasureDimensionById(_measureSettings.BaseDimensionId).Name;


            if (setPredefinedValues)
            {
                model.MaximumCustomerEnteredPrice = 1000;
                model.MaxNumberOfDownloads = 10;
                model.RecurringCycleLength = 100;
                model.RecurringTotalCycles = 10;
                model.StockQuantity = 10000;
                model.NotifyAdminForQuantityBelow = 1;
                model.OrderMinimumQuantity = 1;
                model.OrderMaximumQuantity = 10000;
                model.DisplayOrder = 1;

                model.UnlimitedDownloads = true;
                model.IsShipEnabled = true;
                model.Published = true;
            }

            //little hack here in order to hide some of properties of the first product variant
            //we do it because they dublicate some properties of a product
            model.HideNameAndDescriptionProperties = true;
            model.HidePublishedProperty = true;
            model.HideDisplayOrderProperty = true;
        }

        [NonAction]
        private string[] ParseProductTags(string productTags)
        {
            var result = new List<string>();
            if (!String.IsNullOrWhiteSpace(productTags))
            {
                string[] values = productTags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string val1 in values)
                    if (!String.IsNullOrEmpty(val1.Trim()))
                        result.Add(val1.Trim());
            }
            return result.ToArray();
        }

        [NonAction]
        private void SaveProductTags(Product product, string[] productTags)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            //product tags
            var existingProductTags = product.ProductTags.OrderByDescending(pt => pt.ProductCount).ToList();
            var productTagsToRemove = new List<ProductTag>();
            foreach (var existingProductTag in existingProductTags)
            {
                bool found = false;
                foreach (string newProductTag in productTags)
                {
                    if (existingProductTag.Name.Equals(newProductTag, StringComparison.InvariantCultureIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    productTagsToRemove.Add(existingProductTag);
                }
            }
            foreach (var productTag in productTagsToRemove)
            {
                product.ProductTags.Remove(productTag);
                //ensure product is saved before updating totals
                _productService.UpdateProduct(product);
                _productTagService.UpdateProductTagTotals(productTag);
            }
            foreach (string productTagName in productTags)
            {
                ProductTag productTag = null;
                var productTag2 = _productTagService.GetProductTagByName(productTagName);
                if (productTag2 == null)
                {
                    //add new product tag
                    productTag = new ProductTag()
                    {
                        Name = productTagName,
                        ProductCount = 0,
                    };
                    _productTagService.InsertProductTag(productTag);
                }
                else
                {
                    productTag = productTag2;
                }
                if (!product.ProductTagExists(productTag.Id))
                {
                    product.ProductTags.Add(productTag);
                    //ensure product is saved before updating totals
                    _productService.UpdateProduct(product);
                }
                //update product tag totals 
                _productTagService.UpdateProductTagTotals(productTag);
            }
        }

        [NonAction]
        protected void UpdatePrices(Product product, List<Nop.Admin.Models.Catalog.ProductModel.ProductPriceModel> ProductPrices)
        {
            if (ProductPrices == null)
            {
                var currencies = _currencyService.GetAllCurrencies().Where(x=>x.Published);
                foreach (var currency in currencies)
                {
                    ProductPrice pp = new ProductPrice()
                    {
                        ProductId = product.Id,
                        Price = 0,
                        CurrencyId = currency.Id,
                        PriceUpdatedOn = DateTime.UtcNow,
                    };
                    _productPriceService.InsertProductPrice(pp);
                }

                return;
            }
            foreach (var item in ProductPrices)
            {
                ProductPrice pp = _productPriceService.GetProductPriceById(item.Id);
                pp.ProductId = product.Id;
                pp.PriceUpdatedOn = DateTime.Now;
                pp.CurrencyId = item.CurrencyId;
                decimal correct_price = 0;
                if (Decimal.TryParse(item.PriceValue.Replace(",", "."), out correct_price))
                {
                    if (correct_price <= 100000000)
                    {
                        pp.Price = correct_price;
                    }
                }
                _productPriceService.UpdateProductPrice(pp);
            }
        }
        #endregion

        #region Methods

        #region Product list / create / edit / delete

        //list products
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            IList<int> filterableSpecificationAttributeOptionIds = null;
            var products = _productService.SearchProducts(0, 0, null, null, null, 0, string.Empty, false, false,
                _workContext.WorkingLanguage.Id, new List<int>(),
                ProductSortingEnum.CreatedOn, 0, _adminAreaSettings.GridPageSize,
                false, out filterableSpecificationAttributeOptionIds, true);

            var model = new ProductListModel();
            model.DisplayProductPictures = _adminAreaSettings.DisplayProductPictures;
            model.DisplayPdfDownloadCatalog = _pdfSettings.Enabled;
            model.Products = new GridModel<ProductModel>
            {
                Data = products.Select(x =>
                {
                    var productModel = x.ToModel();
                    productModel.Name = x.GetLocalized(p => p.Name, _workContext.WorkingLanguage.Id);
                    PrepareProductPictureThumbnailModel(productModel, x);
                    PrepareVariantsModel(productModel, x);
                    return productModel;
                }),
                Total = products.TotalCount
            };
            //categories
            model.AvailableCategories.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var c in _categoryService.GetAllCategories(showHidden: true))
                model.AvailableCategories.Add(new SelectListItem() { Text = c.GetCategoryNameWithPrefix(_categoryService), Value = c.Id.ToString() });
            model.AviableBrands = _brandService.GetAllBrands().Select(x => x.Name).ToList();
            //model.AviableCompanyNames
            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var m in _manufacturerService.GetAllManufacturers(true))
                model.AvailableManufacturers.Add(new SelectListItem() { Text = m.Name, Value = m.Id.ToString() });

            return View(model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ProductList(GridCommand command, ProductListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var gridModel = new GridModel();
            var products = new List<Product>();
            //if (String.IsNullOrEmpty(model.SearchBrand) &&
            //    String.IsNullOrEmpty(model.SearchProductCompany) &&
            //    !String.IsNullOrEmpty(model.SearchProductName) &&
            //    model.SearchCategoryId == 0)
            //{
            //    IList<int> filterableSpecificationAttributeOptionIds = null;
            //    products = _productService.SearchProducts(0,
            //        0, null, null, null, 0, model.SearchProductName, false, false,
            //        _workContext.WorkingLanguage.Id, new List<int>(),
            //        ProductSortingEnum.Position, command.Page - 1, command.PageSize,
            //        false, out filterableSpecificationAttributeOptionIds, true);
            //}

            int BrandId = 0;
            if (!String.IsNullOrEmpty(model.SearchBrand))
            {
                var brand = _brandService.GetBrandByName(model.SearchBrand);
                if (brand != null)
                    BrandId = brand.Id;
            }

            var companies = new List<CompanyInformation>();
            if (!String.IsNullOrEmpty(model.SearchProductCompany))
            {
                companies = _companyInformationService.GetAllCompanies().Where(x =>
                {
                    foreach (var lang in _languageService.GetAllLanguages())
                    {
                        var companyName = x.GetLocalized(c => c.CompanyName, lang.Id, false);
                        if (companyName != null && companyName.IndexOf(model.SearchProductCompany) >= 0)
                        {
                            return true;
                        }
                    }
                    return false;
                }).ToList();

                foreach (var company in companies)
                {
                    products.AddRange(_productService.NewSearchProducts(company.Customers.First().Id,
                        model.SearchItemType,
                        BrandId,
                        ProductSortingEnum.CreatedOn,
                        0,
                        short.MaxValue,
                        0,
                        model.SearchCategoryId,
                        0));
                }
            }
            else
            {
                products.AddRange(_productService.NewSearchProducts(0,
                        model.SearchItemType,
                        BrandId,
                        ProductSortingEnum.CreatedOn,
                        0,
                        short.MaxValue,
                        0,
                        model.SearchCategoryId,
                        0));
            }

            if (!String.IsNullOrEmpty(model.SearchProductName))
            {
                products = products.Where(x => x.Name.IndexOf(model.SearchProductName,StringComparison.InvariantCultureIgnoreCase) >= 0).ToList();
            }
            var productsPaged = new PagedList<Product>(products, command.Page - 1, command.PageSize);
            gridModel.Data = productsPaged.Select(x =>
                                                 {
                                                     var productModel = x.ToModel();
                                                     productModel.Name = x.GetLocalized(p => p.Name, _workContext.WorkingLanguage.Id);
                                                     PrepareProductPictureThumbnailModel(productModel, x);
                                                     return productModel;
                                                 });
            gridModel.Total = productsPaged.TotalCount;
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-product-by-sku")]
        public ActionResult GoToSku(ProductListModel model)
        {
            string sku = model.GoDirectlyToSku;
            var pv = _productService.GetProductVariantBySku(sku);
            if (pv != null)
                return RedirectToAction("Edit", "ProductVariant", new { id = pv.Id });
            
            //not found
            return List();
        }

        //create product
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var model = new ProductModel();

            //default values
            model.Published = true;
            model.AllowCustomerReviews = true;
         
            //first product variant
            model.FirstProductVariantModel = new ProductVariantModel();
            model.AviableBanks = _customerService.GetCustomersByCustomerRoleId(_customerService.GetCustomerRoleBySystemName("Sellers").Id)
                .Select(x => new BankModel() { BankTitle = x.CompanyName, Id = x.Id }).ToList();
            model.AviableBanks.Add(new BankModel() { BankTitle = _localizationService.GetResource("ITB.Admin.Product.Select"), Id = 0 });
            model.AviableTags = _productTagService.GetAllProductTags().Select(x => x.Name).ToList();
            AddLocales(_languageService, model.FirstProductVariantModel.Locales);
            FirstVariant_PrepareProductVariantModel(model.FirstProductVariantModel, null, true);
            model.AviableRegions = _regionService.GetAllRegions().OrderBy(x=>x.Title).Select(x => new ProductRegionModel() { Id = x.Id, Title = x.Title }).ToList();
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(ProductModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            if (!String.IsNullOrEmpty(model.OrderLink))
            {
                if (model.OrderLink.IndexOf("http") != 0)
                {
                    ModelState.AddModelError("OrderLink", _localizationService.GetResource("ITB.Admin.Product.OrderLink.Http"));
                }
            }
            if (ModelState.IsValid)
            {
                //product
                //model.CustomerId = _workContext.CurrentCustomer.Id;
                var product = model.ToEntity();
                product.CustomerId = model.BankId;
                product.CreatedOnUtc = DateTime.UtcNow;
                product.UpdatedOnUtc = DateTime.UtcNow;
                product.OrderLink = model.OrderLink;
                _productService.InsertProduct(product);
                //search engine name
                model.SeName = product.ValidateSeName(model.SeName, product.Name, true);
                _urlRecordService.SaveSlug(product, model.SeName, 0);

                //tags (after variant because it can effect product count)
                SaveProductTags(product, ParseProductTags(model.ProductTags));
                foreach (var region in model.AviableRegions.Where(x => x.Selected))
                {
                    var reg = _regionService.GetById(region.Id);
                    product.Regions.Add(reg);
                    _productService.UpdateProduct(product);
                }
                //activity log
                _customerActivityService.InsertActivity("AddNewProduct", _localizationService.GetResource("ActivityLog.AddNewProduct"), product.Name);
                
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = product.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model.AviableTags = _productTagService.GetAllProductTags().Select(x => x.Name).ToList();
            model.AviableBanks = _customerService.GetCustomersByCustomerRoleId(_customerService.GetCustomerRoleBySystemName("Sellers").Id)
               .Select(x => new BankModel() { BankTitle = x.CompanyName, Id = x.Id }).ToList();
            model.AviableBanks.Add(new BankModel() { BankTitle = _localizationService.GetResource("ITB.Admin.Product.Select"), Id = 0 });
            return View(model);
        }

        //edit product
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var product = _productService.GetProductById(id);
            if (product == null || product.Deleted)
                //No product found with the specified id
                return RedirectToAction("List");

            var model = product.ToModel();
            model.BankId = product.CustomerId;
            PrepareTags(model, product);
            PrepareAddSpecificationAttributeModel(model);
            PrepareAddProductPictureModel(model);
            PrepareCategoryMapping(model);
            model.AviableTags = _productTagService.GetAllProductTags().Select(x => x.Name).ToList();
            model.AviableBanks = _customerService.GetCustomersByCustomerRoleId(_customerService.GetCustomerRoleBySystemName("Sellers").Id)
               .Select(x => new BankModel() { BankTitle = x.CompanyName, Id = x.Id }).ToList();
            model.AviableBanks.Add(new BankModel() { BankTitle = _localizationService.GetResource("ITB.Admin.Product.Select"), Id = 0 });
            model.AviableBrands = _brandService.GetAllBrands().Select(x => x.Name).ToList();
            ViewBag.error = false;
            model.AviableRegions = _regionService.GetAllRegions().OrderBy(x => x.Title).Select(x => new ProductRegionModel() { Id = x.Id, Title = x.Title, Selected = product.Regions.Where(r => r.Id == x.Id).Any() }).ToList();
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(ProductModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var product = _productService.GetProductById(model.Id);
            if (product == null || product.Deleted)
                //No product found with the specified id
                return RedirectToAction("List");
            ViewBag.error = false;

            if (!String.IsNullOrEmpty(model.OrderLink))
            {
                if (model.OrderLink.IndexOf("http") != 0)
                {
                    ModelState.AddModelError("OrderLink", _localizationService.GetResource("ITB.Admin.Product.OrderLink.Http"));
                }
            }
            if (ModelState.IsValid)
            {
                //product
                model.CustomerId = product.CustomerId;

                //model.BrandId = product.BrandId.HasValue ? product.BrandId.Value : 0;
                product = model.ToEntity(product);
                product.UpdatedOnUtc = DateTime.UtcNow;
                //if(model.
                _productService.UpdateProduct(product);
                //search engine name
                //model.SeName = product.ValidateSeName(model.SeName, product.Name, true);
                //_urlRecordService.SaveSlug(product, model.SeName, 0);
                //tags
                SaveProductTags(product, ParseProductTags(model.ProductTags));
                //ACL (customer roles)
                SaveProductAcl(product, model);
                //picture seo names
                UpdatePictureSeoNames(product);

                UpdatePrices(product, model.ProductPrices);

                foreach (var region in model.AviableRegions)
                {
                    if (region.Selected)
                    {
                        if (!product.Regions.Where(x => x.Id == region.Id).Any())
                        {
                            var reg = _regionService.GetById(region.Id);
                            product.Regions.Add(reg);
                            _productService.UpdateProduct(product);
                        }
                    }
                    else
                    {
                        if (product.Regions.Where(x => x.Id == region.Id).Any())
                        {
                            var reg = product.Regions.Where(x => x.Id == region.Id).First();
                            product.Regions.Remove(reg);
                            _productService.UpdateProduct(product);
                        }
                    }
                }

                //activity log
                _customerActivityService.InsertActivity("EditProduct", _localizationService.GetResource("ActivityLog.EditProduct"), product.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = product.Id }) : RedirectToAction("List");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    ViewBag.error = true;
                }
            }
            //If we got this far, something failed, redisplay form
            model.AviableBanks = _customerService.GetCustomersByCustomerRoleId(_customerService.GetCustomerRoleBySystemName("Sellers").Id)
               .Select(x => new BankModel() { BankTitle = x.CompanyName, Id = x.Id }).ToList();
            model.AviableBanks.Add(new BankModel() { BankTitle = _localizationService.GetResource("ITB.Admin.Product.Select"), Id = 0 });
            PrepareTags(model, product);
            model.AviableTags = _productTagService.GetAllProductTags().Select(x => x.Name).ToList();
            PrepareCopyProductModel(model, product);
            PrepareVariantsModel(model, product);
            PrepareTemplatesModel(model);
            PrepareAddSpecificationAttributeModel(model);
            PrepareAddProductPictureModel(model);
            PrepareCategoryMapping(model);
            PrepareItemTypeModel(model);
            PrepareManufacturerMapping(model);
            PrepareAclModel(model, product, true);
            model.AviableBrands = _brandService.GetAllBrands().Select(x => x.Name).ToList();
            //foreach (var price in model.ProductPrices)
            //{
            //    price.Currency = _currencyService.GetCurrencyById(price.CurrencyId);
            //}
            return View(model);
        }

        //delete product
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var product = _productService.GetProductById(id);
            _urlRecordService.ClearEntitySlug(product, 0);
            _productService.DeleteProduct(product);
            //update product tag totals
            UpdateProductTagTotals(product);

            //activity log
            _customerActivityService.InsertActivity("DeleteProduct", _localizationService.GetResource("ActivityLog.DeleteProduct"), product.Name);
                
            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Deleted"));
            return RedirectToAction("List");
        }

        public ActionResult DeleteSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var products = new List<Product>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                products.AddRange(_productService.GetProductsByIds(ids));

                for (int i = 0; i < products.Count; i++)
                {
                    var product = products[i];
                    _urlRecordService.ClearEntitySlug(product, 0);
                    _productService.DeleteProduct(product);
                    //update product tag totals
                    UpdateProductTagTotals(product);
                }
            }

            return RedirectToAction("List");
        }


        [GridAction(EnableCustomBinding = true)]
        public ActionResult GetVariants(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var variants = _productService.GetProductVariantsByProductId(productId, true);

            var gridModel = new GridModel<ProductVariantModel>()
            {
                Data = variants.Select(x =>
                {
                    var variantModel = x.ToModel();
                    if (String.IsNullOrEmpty(variantModel.Name))
                        variantModel.Name = "Unnamed";
                    return variantModel;
                }),
                Total = variants.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [HttpPost]
        public ActionResult CopyProduct(ProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var copyModel = model.CopyProductModel;
            try
            {
                var originalProduct = _productService.GetProductById(copyModel.Id);
                var newProduct = _copyProductService.CopyProduct(originalProduct, 
                    copyModel.Name, copyModel.Published, copyModel.CopyImages);
                SuccessNotification("The product has been copied successfully");
                return RedirectToAction("Edit", new { id = newProduct.Id });
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = copyModel.Id });
            }
        }

        #endregion
        
        #region Product categories

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ProductCategoryList(GridCommand command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productCategories = _categoryService.GetProductCategoriesByProductId(productId, true);
            var productCategoriesModel = productCategories
                .Select(x =>
                {
                    return new ProductModel.ProductCategoryModel()
                    {
                        Id = x.Id,
                        Category = _categoryService.GetCategoryById(x.CategoryId).GetCategoryBreadCrumb(_categoryService,_workContext),
                        ProductId = x.ProductId,
                        CategoryId = x.CategoryId,
                        IsFeaturedProduct = x.IsFeaturedProduct,
                        DisplayOrder  = x.DisplayOrder
                    };
                })
                .ToList();

            var model = new GridModel<ProductModel.ProductCategoryModel>
            {
                Data = productCategoriesModel,
                Total = productCategoriesModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductCategoryInsert(GridCommand command, ProductModel.ProductCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productId = model.ProductId;
            var categoryId = Int32.Parse(model.Category); //use Category property (not CategoryId) because appropriate property is stored in it
              
            var existingProductCategories = _categoryService.GetProductCategoriesByCategoryId(categoryId, 0, int.MaxValue, true);
            if (existingProductCategories.FindProductCategory(productId, categoryId) == null)
            {
                var productCategory = new ProductCategory()
                {
                    ProductId = productId,
                    CategoryId = categoryId,
                    IsFeaturedProduct = model.IsFeaturedProduct,
                    DisplayOrder = model.DisplayOrder
                };
                _categoryService.InsertProductCategory(productCategory);
            }
            
            return ProductCategoryList(command, productId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductCategoryUpdate(GridCommand command, ProductModel.ProductCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productCategory = _categoryService.GetProductCategoryById(model.Id);
            if (productCategory == null)
                throw new ArgumentException("No product category mapping found with the specified id");
            var product = productCategory.Product;
            product.ProductAttributes.Clear();
            _productService.UpdateProduct(product);
            //use Category property (not CategoryId) because appropriate property is stored in it
            productCategory.CategoryId = Int32.Parse(model.Category);
            productCategory.IsFeaturedProduct = model.IsFeaturedProduct;
            productCategory.DisplayOrder = model.DisplayOrder;
            _categoryService.UpdateProductCategory(productCategory);

            return ProductCategoryList(command, productCategory.ProductId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductCategoryDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productCategory = _categoryService.GetProductCategoryById(id);
            if (productCategory == null)
                throw new ArgumentException("No product category mapping found with the specified id");

            var productId = productCategory.ProductId;
            _categoryService.DeleteProductCategory(productCategory);

            return ProductCategoryList(command, productId);
        }

        #endregion

        #region Product manufacturers

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ProductManufacturerList(GridCommand command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productManufacturers = _manufacturerService.GetProductManufacturersByProductId(productId, true);
            var productManufacturersModel = productManufacturers
                .Select(x =>
                {
                    return new ProductModel.ProductManufacturerModel()
                    {
                        Id = x.Id,
                        Manufacturer = _manufacturerService.GetManufacturerById(x.ManufacturerId).Name,
                        ProductId = x.ProductId,
                        ManufacturerId = x.ManufacturerId,
                        IsFeaturedProduct = x.IsFeaturedProduct,
                        DisplayOrder = x.DisplayOrder
                    };
                })
                .ToList();

            var model = new GridModel<ProductModel.ProductManufacturerModel>
            {
                Data = productManufacturersModel,
                Total = productManufacturersModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductManufacturerInsert(GridCommand command, ProductModel.ProductManufacturerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productId = model.ProductId;
            var manufacturerId = Int32.Parse(model.Manufacturer); //use Manufacturer property (not ManufacturerId) because appropriate property is stored in it

            var existingProductmanufacturers = _manufacturerService.GetProductManufacturersByManufacturerId(manufacturerId, 0, int.MaxValue, true);
            if (existingProductmanufacturers.FindProductManufacturer(productId, manufacturerId) == null)
            {
                var productManufacturer = new ProductManufacturer()
                {
                    ProductId = productId,
                    ManufacturerId = manufacturerId,
                    IsFeaturedProduct = model.IsFeaturedProduct,
                    DisplayOrder = model.DisplayOrder
                };
                _manufacturerService.InsertProductManufacturer(productManufacturer);
            }
            
            return ProductManufacturerList(command, productId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductManufacturerUpdate(GridCommand command, ProductModel.ProductManufacturerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productManufacturer = _manufacturerService.GetProductManufacturerById(model.Id);
            if (productManufacturer == null)
                throw new ArgumentException("No product manufacturer mapping found with the specified id");

            //use Manufacturer property (not ManufacturerId) because appropriate property is stored in it
            productManufacturer.ManufacturerId = Int32.Parse(model.Manufacturer);
            productManufacturer.IsFeaturedProduct = model.IsFeaturedProduct;
            productManufacturer.DisplayOrder = model.DisplayOrder;
            _manufacturerService.UpdateProductManufacturer(productManufacturer);

            return ProductManufacturerList(command, productManufacturer.ProductId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductManufacturerDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productManufacturer = _manufacturerService.GetProductManufacturerById(id);
            if (productManufacturer == null)
                throw new ArgumentException("No product manufacturer mapping found with the specified id");

            var productId = productManufacturer.ProductId;
            _manufacturerService.DeleteProductManufacturer(productManufacturer);

            return ProductManufacturerList(command, productId);
        }
        
        #endregion

        #region Related products

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult RelatedProductList(GridCommand command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var relatedProducts = _productService.GetRelatedProductsByProductId1(productId, true);
            var relatedProductsModel = relatedProducts
                .Select(x =>
                {
                    return new ProductModel.RelatedProductModel()
                    {
                        Id = x.Id,
                        ProductId1 = x.ProductId1,
                        ProductId2 = x.ProductId2,
                        Product2Name = _productService.GetProductById(x.ProductId2).Name,
                        DisplayOrder = x.DisplayOrder
                    };
                })
                .ToList();

            var model = new GridModel<ProductModel.RelatedProductModel>
            {
                Data = relatedProductsModel,
                Total = relatedProductsModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }
        
        [GridAction(EnableCustomBinding = true)]
        public ActionResult RelatedProductUpdate(GridCommand command, ProductModel.RelatedProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var relatedProduct = _productService.GetRelatedProductById(model.Id);
            if (relatedProduct == null)
                throw new ArgumentException("No related product found with the specified id");

            relatedProduct.DisplayOrder = model.DisplayOrder;
            _productService.UpdateRelatedProduct(relatedProduct);

            return RelatedProductList(command, relatedProduct.ProductId1);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult RelatedProductDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var relatedProduct = _productService.GetRelatedProductById(id);
            if (relatedProduct == null)
                throw new ArgumentException("No related product found with the specified id");

            var productId = relatedProduct.ProductId1;
            _productService.DeleteRelatedProduct(relatedProduct);

            return RelatedProductList(command, productId);
        }
        
        public ActionResult RelatedProductAddPopup(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            IList<int> filterableSpecificationAttributeOptionIds = null;
            var products = _productService.SearchProducts(0, 0, null, null, null, 0, string.Empty, false, false,
                _workContext.WorkingLanguage.Id, new List<int>(),
                ProductSortingEnum.Position, 0, _adminAreaSettings.GridPageSize,
                false, out filterableSpecificationAttributeOptionIds, true);

            var model = new ProductModel.AddRelatedProductModel();
            model.Products = new GridModel<ProductModel>
            {
                Data = products.Select(x => x.ToModel()),
                Total = products.TotalCount
            };
            //categories
            model.AvailableCategories.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var c in _categoryService.GetAllCategories(showHidden: true))
                model.AvailableCategories.Add(new SelectListItem() { Text = c.GetCategoryNameWithPrefix(_categoryService), Value = c.Id.ToString() });

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var m in _manufacturerService.GetAllManufacturers(true))
                model.AvailableManufacturers.Add(new SelectListItem() { Text = m.Name, Value = m.Id.ToString() });

            return View(model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult RelatedProductAddPopupList(GridCommand command, ProductModel.AddRelatedProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var gridModel = new GridModel();
            IList<int> filterableSpecificationAttributeOptionIds = null;
            var products = _productService.SearchProducts(model.SearchCategoryId, model.SearchManufacturerId, 
                null, null, null, 0, model.SearchProductName, false, false,
                _workContext.WorkingLanguage.Id, new List<int>(),
                ProductSortingEnum.Position, command.Page - 1, command.PageSize,
                false, out filterableSpecificationAttributeOptionIds, true);
            gridModel.Data = products.Select(x => x.ToModel());
            gridModel.Total = products.TotalCount;
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult RelatedProductAddPopup(string btnId, string formId, ProductModel.AddRelatedProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            if (model.SelectedProductIds != null)
            {
                foreach (int id in model.SelectedProductIds)
                {
                    var product = _productService.GetProductById(id);
                    if (product != null)
                    {
                        var existingRelatedProducts = _productService.GetRelatedProductsByProductId1(model.ProductId);
                        if (existingRelatedProducts.FindRelatedProduct(model.ProductId, id) == null)
                        {
                            _productService.InsertRelatedProduct(
                                new RelatedProduct()
                                {
                                    ProductId1 = model.ProductId,
                                    ProductId2 = id,
                                    DisplayOrder = 1
                                });
                        }
                    }
                }
            }

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            model.Products = new GridModel<ProductModel>();
            return View(model);
        }
        
        #endregion

        #region Cross-sell products

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult CrossSellProductList(GridCommand command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var crossSellProducts = _productService.GetCrossSellProductsByProductId1(productId, true);
            var crossSellProductsModel = crossSellProducts
                .Select(x =>
                {
                    return new ProductModel.CrossSellProductModel()
                    {
                        Id = x.Id,
                        ProductId1 = x.ProductId1,
                        ProductId2 = x.ProductId2,
                        Product2Name = _productService.GetProductById(x.ProductId2).Name,
                    };
                })
                .ToList();

            var model = new GridModel<ProductModel.CrossSellProductModel>
            {
                Data = crossSellProductsModel,
                Total = crossSellProductsModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult CrossSellProductDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var crossSellProduct = _productService.GetCrossSellProductById(id);
            if (crossSellProduct == null)
                throw new ArgumentException("No cross-sell product found with the specified id");

            var productId = crossSellProduct.ProductId1;
            _productService.DeleteCrossSellProduct(crossSellProduct);

            return CrossSellProductList(command, productId);
        }

        public ActionResult CrossSellProductAddPopup(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            IList<int> filterableSpecificationAttributeOptionIds = null;
            var products = _productService.SearchProducts(0, 0, null, null, null, 0, string.Empty, false, false,
                _workContext.WorkingLanguage.Id, new List<int>(),
                ProductSortingEnum.Position, 0, _adminAreaSettings.GridPageSize,
                false, out filterableSpecificationAttributeOptionIds, true);

            var model = new ProductModel.AddCrossSellProductModel();
            model.Products = new GridModel<ProductModel>
            {
                Data = products.Select(x => x.ToModel()),
                Total = products.TotalCount
            };
            //categories
            model.AvailableCategories.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var c in _categoryService.GetAllCategories(showHidden: true))
                model.AvailableCategories.Add(new SelectListItem() { Text = c.GetCategoryNameWithPrefix(_categoryService), Value = c.Id.ToString() });

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var m in _manufacturerService.GetAllManufacturers(true))
                model.AvailableManufacturers.Add(new SelectListItem() { Text = m.Name, Value = m.Id.ToString() });

            return View(model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult CrossSellProductAddPopupList(GridCommand command, ProductModel.AddCrossSellProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var gridModel = new GridModel();
            IList<int> filterableSpecificationAttributeOptionIds = null;
            var products = _productService.SearchProducts(model.SearchCategoryId,
                model.SearchManufacturerId, null, null, null, 0, model.SearchProductName, false, false,
                _workContext.WorkingLanguage.Id, new List<int>(),
                ProductSortingEnum.Position, command.Page - 1, command.PageSize,
                false, out filterableSpecificationAttributeOptionIds, true);
            gridModel.Data = products.Select(x => x.ToModel());
            gridModel.Total = products.TotalCount;
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult CrossSellProductAddPopup(string btnId, string formId, ProductModel.AddCrossSellProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            if (model.SelectedProductIds != null)
            {
                foreach (int id in model.SelectedProductIds)
                {
                    var product = _productService.GetProductById(id);
                    if (product != null)
                    {
                        var existingCrossSellProducts = _productService.GetCrossSellProductsByProductId1(model.ProductId);
                        if (existingCrossSellProducts.FindCrossSellProduct(model.ProductId, id) == null)
                        {
                            _productService.InsertCrossSellProduct(
                                new CrossSellProduct()
                                {
                                    ProductId1 = model.ProductId,
                                    ProductId2 = id,
                                });
                        }
                    }
                }
            }

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            model.Products = new GridModel<ProductModel>();
            return View(model);
        }

        #endregion

        #region Product pictures

        public ActionResult ProductPictureAdd(int pictureId, int displayOrder, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            if (pictureId == 0)
                throw new ArgumentException();

            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            _productService.InsertProductPicture(new ProductPicture()
            {
                PictureId = pictureId,
                ProductId = productId,
                DisplayOrder = displayOrder,
            });

            _pictureService.SetSeoFilename(pictureId, _pictureService.GetPictureSeName(product.Name));

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ProductPictureList(GridCommand command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productPictures = _productService.GetProductPicturesByProductId(productId);
            var productPicturesModel = productPictures
                .Select(x =>
                {
                    return new ProductModel.ProductPictureModel()
                    {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        PictureId = x.PictureId,
                        PictureUrl = _pictureService.GetPictureUrl(x.PictureId),
                        DisplayOrder = x.DisplayOrder
                    };
                })
                .ToList();

            var model = new GridModel<ProductModel.ProductPictureModel>
            {
                Data = productPicturesModel,
                Total = productPicturesModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductPictureUpdate(ProductModel.ProductPictureModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productPicture = _productService.GetProductPictureById(model.Id);
            if (productPicture == null)
                throw new ArgumentException("No product picture found with the specified id");

            productPicture.DisplayOrder = model.DisplayOrder;
            _productService.UpdateProductPicture(productPicture);

            return ProductPictureList(command, productPicture.ProductId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductPictureDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productPicture = _productService.GetProductPictureById(id);
            if (productPicture == null)
                throw new ArgumentException("No product picture found with the specified id");

            var productId = productPicture.ProductId;
            _productService.DeleteProductPicture(productPicture);

            var picture = _pictureService.GetPictureById(productPicture.PictureId);
            _pictureService.DeletePicture(picture);
            
            return ProductPictureList(command, productId);
        }

        #endregion

        #region Product specification attributes

        [ValidateInput(false)]
        public ActionResult ProductSpecificationAttributeAdd(int specificationAttributeOptionId,
            string customValue, bool allowFiltering, bool showOnProductPage, 
            int displayOrder, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var psa = new ProductSpecificationAttribute()
            {
                SpecificationAttributeOptionId = specificationAttributeOptionId,
                ProductId = productId,
                CustomValue = customValue,
                AllowFiltering = allowFiltering,
                ShowOnProductPage = showOnProductPage,
                DisplayOrder = displayOrder,
            };
            _specificationAttributeService.InsertProductSpecificationAttribute(psa);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ProductSpecAttrList(GridCommand command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productrSpecs = _specificationAttributeService.GetProductSpecificationAttributesByProductId(productId);

            var productrSpecsModel = productrSpecs
                .Select(x =>
                {
                    var psaModel = new ProductSpecificationAttributeModel()
                    {
                        Id = x.Id,
                        SpecificationAttributeName = x.SpecificationAttributeOption.SpecificationAttribute.Name,
                        SpecificationAttributeOptionName = x.SpecificationAttributeOption.Name,
                        CustomValue = x.CustomValue,
                        AllowFiltering = x.AllowFiltering,
                        ShowOnProductPage = x.ShowOnProductPage,
                        DisplayOrder = x.DisplayOrder
                    };
                    return psaModel;
                })
                .ToList();

            var model = new GridModel<ProductSpecificationAttributeModel>
            {
                Data = productrSpecsModel,
                Total = productrSpecsModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductSpecAttrUpdate(int psaId, ProductSpecificationAttributeModel model,
            GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var psa = _specificationAttributeService.GetProductSpecificationAttributeById(psaId);
            psa.CustomValue = model.CustomValue;
            psa.AllowFiltering = model.AllowFiltering;
            psa.ShowOnProductPage = model.ShowOnProductPage;
            psa.DisplayOrder = model.DisplayOrder;
            _specificationAttributeService.UpdateProductSpecificationAttribute(psa);

            return ProductSpecAttrList(command, psa.ProductId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductSpecAttrDelete(int psaId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var psa = _specificationAttributeService.GetProductSpecificationAttributeById(psaId);
            if (psa == null)
                throw new ArgumentException("No specification attribute found with the specified id");

            var productId = psa.ProductId;
            _specificationAttributeService.DeleteProductSpecificationAttribute(psa);

            return ProductSpecAttrList(command, productId);
        }

        #endregion

        #region Product tags

        public ActionResult ProductTags()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            return View();
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ProductTags(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var tags = _productTagService.GetAllProductTags(true)
                .Select(x =>
                {
                    return new ProductTagModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ProductCount = x.ProductCount,
                    };
                })
                .ForCommand(command);

            var model = new GridModel<ProductTagModel>
            {
                Data = tags.PagedForCommand(command),
                Total = tags.Count()
            };
            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductTagDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var tag = _productTagService.GetProductTagById(id);
            if (tag == null)
                throw new ArgumentException("No product tag found with the specified id");
            _productTagService.DeleteProductTag(tag);

            return ProductTags(command);
        }

        //edit
        public ActionResult EditProductTag(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productTag = _productTagService.GetProductTagById(id);
            if (productTag == null)
                //No product tag found with the specified id
                return RedirectToAction("List");

            var model = new ProductTagModel()
            {
                Id = productTag.Id,
                Name = productTag.Name,
                ProductCount = productTag.ProductCount
            };
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = productTag.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost]
        public ActionResult EditProductTag(string btnId, string formId, ProductTagModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productTag = _productTagService.GetProductTagById(model.Id);
            if (productTag == null)
                //No product tag found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                productTag.Name = model.Name;
                _productTagService.UpdateProductTag(productTag);
                //locales
                UpdateLocales(productTag, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProductTagUpdate(ProductTagModel model, GridCommand command)
        {
            var productTag = _productTagService.GetProductTagById(model.Id);
            if (productTag == null)
            {
                return RedirectToAction("List");
            }

            if (ModelState.IsValid)
            {
                productTag.Name = model.Name;
                _productTagService.UpdateProductTag(productTag);
            }

            return ProductTags(command);
        }
        #endregion

        #region Export / Import

        public ActionResult DownloadCatalogAsPdf()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            try
            {
                IList<int> filterableSpecificationAttributeOptionIds = null;
                var products = _productService.SearchProducts(0, 0, null, null, null, 0, string.Empty, false, false,
                    _workContext.WorkingLanguage.Id, new List<int>(),
                    ProductSortingEnum.Position, 0, int.MaxValue,
                    false, out filterableSpecificationAttributeOptionIds, true);


                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    _pdfService.PrintProductsToPdf(stream, products, _workContext.WorkingLanguage);
                    bytes = stream.ToArray();
                }
                return File(bytes, "application/pdf", "pdfcatalog.pdf");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public ActionResult ExportXmlAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            try
            {
                IList<int> filterableSpecificationAttributeOptionIds = null;
                var products = _productService.SearchProducts(0, 0, null, null, null, 0, string.Empty, false, false,
                    _workContext.WorkingLanguage.Id, new List<int>(),
                    ProductSortingEnum.Position, 0, int.MaxValue,
                    false, out filterableSpecificationAttributeOptionIds, true);

                var xml = _exportManager.ExportProductsToXml(products);
                return new XmlDownloadResult(xml, "products.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public ActionResult ExportXmlSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var products = new List<Product>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                products.AddRange(_productService.GetProductsByIds(ids));
            }

            var xml = _exportManager.ExportProductsToXml(products);
            return new XmlDownloadResult(xml, "products.xml");
        }

        public ActionResult ExportExcelAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            try
            {
                IList<int> filterableSpecificationAttributeOptionIds = null;
                var products = _productService.SearchProducts(0, 0, null, null, null, 0, string.Empty, false, false,
                    _workContext.WorkingLanguage.Id, new List<int>(),
                    ProductSortingEnum.Position, 0, int.MaxValue,
                    false, out filterableSpecificationAttributeOptionIds, true);
                
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    _exportManager.ExportProductsToXlsx(stream, products);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "products.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public ActionResult ExportExcelSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var products = new List<Product>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                products.AddRange(_productService.GetProductsByIds(ids));
            }

            byte[] bytes = null;
            using (var stream = new MemoryStream())
            {
                _exportManager.ExportProductsToXlsx(stream, products);
                bytes = stream.ToArray();
            }
            return File(bytes, "text/xls", "products.xlsx");
        }

        [HttpPost]
        public ActionResult ImportExcel(FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            try
            {
                var file = Request.Files["importexcelfile"];
                if (file != null && file.ContentLength > 0)
                {
                    _importManager.ImportProductsFromXlsx(file.InputStream);
                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Imported"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }

        }

        #endregion

        #endregion

        public ActionResult EditProductAttributeValues(int categoryId, int productId)
        {
            var model = new EditProductAttributeListModel();
            var currencies = _currencyService.GetAllCurrencies();
            model.AviableCurrencies = currencies.Select(x => new CurrencyModel()
            {
                Id = x.Id,
                CurrencyCode = x.CurrencyCode
            }).ToList();
            model.AttributeList = new List<EditProductAttributeModel>();
            var attributeGroups = _categoryProductAttributeService.GetCategoryToCategoryProductAttributeGroupByCategoryId(categoryId);
            if (attributeGroups == null)
            {
                ViewBag.Close = true;
            }else
            {
                if(attributeGroups.Count == 0)
                {
                    ViewBag.Close = true;
                }
            }

            var product = _productService.GetProductById(productId);
            if(product == null)
            {
                ViewBag.Close = true;
            }
            List<CategoryProductAttribute> productAttributes = new List<CategoryProductAttribute>();
            foreach (var group in attributeGroups)
            {
                productAttributes.AddRange(group.CategoryProductAttributeGroup.CategoryProductAttributes);
            }
            foreach (var attribute in productAttributes)
            {
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.Money:
                        {
                            var attributeModel = new EditProductAttributeModel();
                            attributeModel.AttributeName = attribute.ProductAttribute.Name;
                            var attributeValue = product.ProductAttributes.Where(x => x.CategoryProductAttributeId == attribute.Id).FirstOrDefault();
                            if (attributeValue != null)
                            {
                                attributeModel.AttributeValueId = attributeValue.Id;
                                attributeModel.CurrencyId = attributeValue.CurrencyId.Value;
                                var currency = _currencyService.GetCurrencyById(attributeValue.CurrencyId.Value);
                                attributeModel.AttributeValue = attributeValue.RealValue.HasValue ? String.Format("{0:0.00}", attributeValue.RealValue.Value * (double)currency.Rate) : "";
                            }
                            attributeModel.Id = attribute.Id;
                            attributeModel.AttributeControlType = attribute.AttributeControlType;
                            attributeModel.AttributeControlTypeId = attribute.AttributeControlTypeId;
                            model.AttributeList.Add(attributeModel);
                            break;
                        }
                    case AttributeControlType.MoneyRange:
                        {
                            var attributeModel = new EditProductAttributeModel();
                            attributeModel.AttributeName = attribute.ProductAttribute.Name;
                            var attributeValue = product.ProductAttributes.Where(x => x.CategoryProductAttributeId == attribute.Id).FirstOrDefault();
                            if (attributeValue != null)
                            {
                                attributeModel.AttributeValueId = attributeValue.Id;
                                attributeModel.CurrencyId = attributeValue.CurrencyId.Value;
                                var currency = _currencyService.GetCurrencyById(attributeValue.CurrencyId.Value);
                                attributeModel.AttributeValue = attributeValue.RealValue.HasValue ? String.Format("{0:0.00}", attributeValue.RealValue.Value * (double)currency.Rate) : "";
                                attributeModel.AttributeValueMax = attributeValue.RealValueMax.HasValue ? String.Format("{0:0.00}", attributeValue.RealValueMax.Value * (double)currency.Rate) : "";
                            }
                            attributeModel.Id = attribute.Id;
                            attributeModel.AttributeControlType = attribute.AttributeControlType;
                            attributeModel.AttributeControlTypeId = attribute.AttributeControlTypeId;
                            model.AttributeList.Add(attributeModel);
                            break;
                        }
                    case AttributeControlType.TextBox:
                    case AttributeControlType.ToddlerInt:
                        {
                            var attributeModel = new EditProductAttributeModel();
                            attributeModel.AttributeName = attribute.ProductAttribute.Name;
                            var attributeValue = product.ProductAttributes.Where(x => x.CategoryProductAttributeId == attribute.Id).FirstOrDefault();
                            if (attributeValue != null)
                            {
                                attributeModel.AttributeValue = attributeValue.Name;
                                attributeModel.AttributeValueId = attributeValue.Id;
                            }
                            attributeModel.Id = attribute.Id;
                            attributeModel.AttributeControlType = attribute.AttributeControlType;
                            attributeModel.AttributeControlTypeId = attribute.AttributeControlTypeId;
                            model.AttributeList.Add(attributeModel);
                            break;
                        }
                    case AttributeControlType.Checkboxes:
                        {
                            var attributeModel = new EditProductAttributeModel();
                            attributeModel.AttributeName = attribute.ProductAttribute.Name;
                            attributeModel.AttributeControlType = attribute.AttributeControlType;
                            attributeModel.AttributeControlTypeId = attribute.AttributeControlTypeId;
                            var attributeValues = product.ProductAttributes.Where(x => x.CategoryProductAttributeId == attribute.Id);
                            var aviableAttrbuteValues = _categoryProductAttributeService.GetCategoryProductAttributeValues(attribute.Id);
                            if (aviableAttrbuteValues.Count() > 0)
                            {
                                attributeModel.AttributeValues = new List<EditProductAttributeValueModel>();
                                foreach (var value in aviableAttrbuteValues)
                                {
                                    var attributeValue = new EditProductAttributeValueModel()
                                    {
                                        Id = value.Id,
                                        Selected = attributeValues.Where(x=>x.Id == value.Id).FirstOrDefault() != null,
                                        AttributeValue = value.Name
                                    };
                                    attributeModel.AttributeValues.Add(attributeValue);
                                }

                            }
                            attributeModel.Id = attribute.Id;
                            model.AttributeList.Add(attributeModel);
                            break;
                        }
                    case AttributeControlType.DropdownList:
                        {
                            var attributeModel = new EditProductAttributeModel();
                            attributeModel.AttributeName = attribute.ProductAttribute.Name;
                            attributeModel.AttributeControlType = attribute.AttributeControlType;
                            attributeModel.AttributeControlTypeId = attribute.AttributeControlTypeId;
                            var attributeValues = product.ProductAttributes.Where(x => x.CategoryProductAttributeId == attribute.Id);
                            var aviableAttrbuteValues = _categoryProductAttributeService.GetCategoryProductAttributeValues(attribute.Id);
                            if (aviableAttrbuteValues.Count() > 0)
                            {
                                attributeModel.AttributeValues = new List<EditProductAttributeValueModel>();
                                foreach (var value in aviableAttrbuteValues)
                                {
                                    var attributeValue = new EditProductAttributeValueModel()
                                    {
                                        Id = value.Id,
                                        AttributeValue = value.Name
                                    };
                                    attributeModel.AttributeValues.Add(attributeValue);
                                }
                                attributeModel.AttributeValues.Add(new EditProductAttributeValueModel()
                                    {
                                        Id = 0,
                                        AttributeValue = _localizationService.GetResource("ITB.Attribute.SelectValue")
                                    });
                                var selected = attributeValues.FirstOrDefault();
                                if (selected != null)
                                {
                                    attributeModel.AttributeValueId = selected.Id;
                                }

                            }
                            attributeModel.Id = attribute.Id;
                            model.AttributeList.Add(attributeModel);
                            break;
                        }
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult EditProductAttributeValues(EditProductAttributeListModel model)
        {
            if (ValidateProductAttributes(model))
            {
                UpdateProductAttributes(model);
                ViewBag.Close = true;
            }
            foreach (var attr in model.AttributeList)
            {
                attr.AttributeControlType = (AttributeControlType)attr.AttributeControlTypeId;
            }
            var currencies = _currencyService.GetAllCurrencies();
            model.AviableCurrencies = currencies.Select(x => new CurrencyModel()
            {
                Id = x.Id,
                CurrencyCode = x.CurrencyCode
            }).ToList();
            return View(model);
        }

        [NonAction]
        private void UpdateProductAttributes(EditProductAttributeListModel model)
        {
            var product = _productService.GetProductById(model.ProductId);
            foreach (var attribute in model.AttributeList)
            {
                var categoryAttribute = _categoryProductAttributeService.GetCategoryProductAttributeById(attribute.Id);
                switch (categoryAttribute.AttributeControlType)
                {
                    case AttributeControlType.Money:
                    case AttributeControlType.ToddlerInt:
                        {
                            if (attribute.AttributeValueId != 0)
                            {
                                if (String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    var existingAttribute = product.ProductAttributes.Where(x => x.CategoryProductAttributeId == attribute.Id).FirstOrDefault();
                                    if (existingAttribute != null)
                                    {
                                        product.ProductAttributes.Remove(existingAttribute);
                                    }
                                    _categoryProductAttributeService.DeleteCategoryProductAttributeValue(existingAttribute);
                                }
                                else
                                {
                                    double value = double.Parse(attribute.AttributeValue);
                                    var attr = _categoryProductAttributeService.GetCategoryProductAttributeValueById(attribute.AttributeValueId);
                                    if (attribute.CurrencyId != 0)
                                    {
                                        attr.CurrencyId = attribute.CurrencyId;
                                        var currency = _currencyService.GetCurrencyById(attribute.CurrencyId);
                                        value = value / (double)currency.Rate;
                                    }
                                    else
                                    {
                                        attr.CurrencyId = null;
                                    }
                                    attr.Name = attribute.AttributeValue;
                                    attr.RealValue = value;
                                    _categoryProductAttributeService.UpdateCategoryProductAttributeValue(attr);
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    double value = double.Parse(attribute.AttributeValue);
                                    var attributeValue = new CategoryProductAttributeValue()
                                    {
                                        CategoryProductAttributeId = attribute.Id,
                                        Name = attribute.AttributeValue,
                                        RealValue = value
                                    };
                                    if (attribute.CurrencyId != 0)
                                    {
                                        attributeValue.CurrencyId = attribute.CurrencyId;
                                        var currency = _currencyService.GetCurrencyById(attribute.CurrencyId);
                                        attributeValue.RealValue = value / (double)currency.Rate;
                                    }
                                    else
                                    {
                                        attributeValue.CurrencyId = null;
                                    }
                                    _categoryProductAttributeService.InsertCategoryProductAttributeValue(attributeValue);
                                    product.ProductAttributes.Add(attributeValue);
                                    _productService.UpdateProduct(product);
                                }
                            }
                            break;
                        }
                    case AttributeControlType.MoneyRange:
                        {
                            if (attribute.AttributeValueId != 0)
                            {
                                if (String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    var existingAttribute = product.ProductAttributes.Where(x => x.CategoryProductAttributeId == attribute.Id).FirstOrDefault();
                                    if (existingAttribute != null)
                                    {
                                        product.ProductAttributes.Remove(existingAttribute);
                                    }
                                    _categoryProductAttributeService.DeleteCategoryProductAttributeValue(existingAttribute);
                                }
                                else
                                {
                                    double value = double.Parse(attribute.AttributeValue);
                                    double? valueMax = null;
                                    if (!String.IsNullOrEmpty(attribute.AttributeValueMax))
                                    {
                                        valueMax = double.Parse(attribute.AttributeValueMax);
                                    }
                                    var attr = _categoryProductAttributeService.GetCategoryProductAttributeValueById(attribute.AttributeValueId);
                                    attr.CurrencyId = attribute.CurrencyId;
                                    var currency = _currencyService.GetCurrencyById(attribute.CurrencyId);
                                    value = value / (double)currency.Rate;
                                    if (valueMax.HasValue)
                                    {
                                        valueMax = valueMax / (double)currency.Rate;
                                    }
                                    attr.Name = attribute.AttributeValue;
                                    attr.RealValue = value;
                                    attr.RealValueMax = valueMax;
                                    _categoryProductAttributeService.UpdateCategoryProductAttributeValue(attr);
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    double value = double.Parse(attribute.AttributeValue);
                                    var attributeValue = new CategoryProductAttributeValue()
                                    {
                                        CategoryProductAttributeId = attribute.Id,
                                        Name = attribute.AttributeValue,
                                        RealValue = value
                                    };
                                    attributeValue.CurrencyId = attribute.CurrencyId;
                                    var currency = _currencyService.GetCurrencyById(attribute.CurrencyId);
                                    attributeValue.RealValue = value / (double)currency.Rate;
                                    if (!String.IsNullOrEmpty(attribute.AttributeValueMax))
                                    {
                                        double valueMax = double.Parse(attribute.AttributeValueMax);
                                        attributeValue.RealValueMax = valueMax / (double)currency.Rate;
                                    }
                                    _categoryProductAttributeService.InsertCategoryProductAttributeValue(attributeValue);
                                    product.ProductAttributes.Add(attributeValue);
                                    _productService.UpdateProduct(product);
                                }
                            }
                            break;
                        }
                    case AttributeControlType.TextBox:
                        {
                            if (attribute.AttributeValueId != 0)
                            {
                                if (String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    var attr = _categoryProductAttributeService.GetCategoryProductAttributeValueById(attribute.AttributeValueId);
                                    product.ProductAttributes.Remove(attr);
                                    _productService.UpdateProduct(product);
                                    _categoryProductAttributeService.DeleteCategoryProductAttributeValue(attr);
                                }
                                else
                                {
                                    var attr = _categoryProductAttributeService.GetCategoryProductAttributeValueById(attribute.AttributeValueId);
                                    attr.Name = attribute.AttributeValue;
                                    _categoryProductAttributeService.UpdateCategoryProductAttributeValue(attr);
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    var attributeValue = new CategoryProductAttributeValue()
                                    {
                                        CategoryProductAttributeId = attribute.Id,
                                        Name = attribute.AttributeValue,
                                        CurrencyId = attribute.CurrencyId
                                    };
                                    _categoryProductAttributeService.InsertCategoryProductAttributeValue(attributeValue);
                                    product.ProductAttributes.Add(attributeValue);
                                    _productService.UpdateProduct(product);
                                }
                            }
                            break;
                        }
                    case AttributeControlType.DropdownList:
                        {
                            if (!String.IsNullOrEmpty(attribute.AdditionalValue))
                            {
                                var attributes = _categoryProductAttributeService.GetCategoryProductAttributeValues(attribute.Id);
                                var oldAttribute = attributes.Where(x => x.Name == attribute.AdditionalValue).FirstOrDefault();
                                if (oldAttribute == null)
                                {
                                    var attributeValue = new CategoryProductAttributeValue()
                                    {
                                        CategoryProductAttributeId = attribute.Id,
                                        Name = attribute.AdditionalValue,
                                        CurrencyId = attribute.CurrencyId
                                    };
                                    _categoryProductAttributeService.InsertCategoryProductAttributeValue(attributeValue);
                                    var existingAttribute = product.ProductAttributes.Where(x => x.CategoryProductAttributeId == attribute.Id).FirstOrDefault();
                                    if (existingAttribute != null)
                                    {
                                        product.ProductAttributes.Remove(existingAttribute);
                                        _productService.UpdateProduct(product);
                                    }
                                    product.ProductAttributes.Add(attributeValue);
                                    _productService.UpdateProduct(product);
                                }
                                else
                                {
                                    var existingAttribute = product.ProductAttributes.Where(x => x.CategoryProductAttributeId == attribute.Id).FirstOrDefault();
                                    if (existingAttribute != null && existingAttribute.Id != oldAttribute.Id)
                                    {
                                        product.ProductAttributes.Remove(existingAttribute);
                                        _productService.UpdateProduct(product);
                                        var attr = _categoryProductAttributeService.GetCategoryProductAttributeValueById(attribute.AttributeValueId);
                                        product.ProductAttributes.Add(attr);
                                        _productService.UpdateProduct(product);
                                    }
                                }
                            }
                            else
                            {
                                var existingAttribute = product.ProductAttributes.Where(x=>x.CategoryProductAttributeId == attribute.Id).FirstOrDefault();
                                if (existingAttribute != null && existingAttribute.Id != attribute.AttributeValueId)
                                {
                                    product.ProductAttributes.Remove(existingAttribute);
                                    _productService.UpdateProduct(product);
                                    var attr = _categoryProductAttributeService.GetCategoryProductAttributeValueById(attribute.AttributeValueId);
                                    product.ProductAttributes.Add(attr);
                                    _productService.UpdateProduct(product);
                                }
                                else
                                {
                                    var attrValue = _categoryProductAttributeService.GetCategoryProductAttributeValueById(attribute.AttributeValueId);
                                    if (attrValue != null)
                                    {
                                        product.ProductAttributes.Add(attrValue);
                                        _productService.UpdateProduct(product);
                                    }
                                }
                            }
                            break;
                        }
                    case AttributeControlType.Checkboxes:
                        {
                            foreach (var attr in attribute.AttributeValues)
                            {
                                if (attr.Selected)
                                {
                                    var existingAttribute = product.ProductAttributes.Where(x => x.Id == attr.Id).FirstOrDefault();
                                    if (existingAttribute == null)
                                    {
                                        var attributeNew = _categoryProductAttributeService.GetCategoryProductAttributeValueById(attr.Id);
                                        product.ProductAttributes.Add(attributeNew);
                                        _productService.UpdateProduct(product);
                                    }
                                }
                                else
                                {
                                    var existingAttribute = product.ProductAttributes.Where(x => x.Id == attr.Id).FirstOrDefault();
                                    if (existingAttribute != null)
                                    {
                                        product.ProductAttributes.Remove(existingAttribute);
                                        _productService.UpdateProduct(product);
                                    }
                                }
                            }
                            break;
                        }
                }
            }
        }

        [NonAction]
        private bool ValidateProductAttributes(EditProductAttributeListModel model)
        {
            var category = _categoryService.GetCategoryById(model.CategoryId);

            bool result = true;
            foreach (var attribute in model.AttributeList)
            {
                var categoryAttribute = _categoryProductAttributeService.GetCategoryProductAttributeById(attribute.Id);
                if (categoryAttribute.IsRequired)
                {
                    switch (categoryAttribute.AttributeControlType)
                    {
                        case AttributeControlType.TextBox:
                            {
                                if (String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Required"), attribute.AttributeName);
                                    result = false;
                                }
                                break;
                            }
                        case AttributeControlType.MoneyRange:
                            {
                                if (String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Required"), attribute.AttributeName);
                                    result = false;
                                }
                                else
                                {
                                    double resultValue;
                                    double resultvalueMax;
                                    if (!Double.TryParse(attribute.AttributeValue, out resultValue))
                                    {
                                        attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int"), attribute.AttributeName);
                                        result = false;
                                    }
                                    else
                                    {
                                        if (resultValue == 0)
                                        {
                                            attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int.GreaterThanZero"), attribute.AttributeName);
                                            result = false;
                                        }
                                        else
                                        {
                                            if (!String.IsNullOrEmpty(attribute.AttributeValueMax))
                                            {
                                                if (!Double.TryParse(attribute.AttributeValueMax, out resultvalueMax))
                                                {
                                                    attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int"), attribute.AttributeName);
                                                    result = false;
                                                }
                                                else
                                                {
                                                    if (resultvalueMax < resultValue)
                                                    {
                                                        attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITBFA.Attributes.MoreThan.Error"), attribute.AttributeName);
                                                        result = false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        case AttributeControlType.Money:
                            {
                                if (String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Required"), attribute.AttributeName);
                                    result = false;
                                }
                                else
                                {
                                    double resultValue;
                                    if (!Double.TryParse(attribute.AttributeValue, out resultValue))
                                    {
                                        attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int"), attribute.AttributeName);
                                        result = false;
                                    }
                                    else
                                    {
                                        if (resultValue == 0)
                                        {
                                            attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int.GreaterThanZero"), attribute.AttributeName);
                                            result = false;
                                        }
                                    }
                                }
                                break;
                            }
                        case AttributeControlType.DropdownList:
                            {
                                if (attribute.AttributeValueId == 0)
                                {
                                    attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Required"), attribute.AttributeName);
                                    result = false;
                                }
                                break;
                            }
                        case AttributeControlType.Checkboxes:
                            {
                                bool valueChecked = false;
                                foreach (var value in attribute.AttributeValues)
                                {
                                    if (value.Selected)
                                    {
                                        valueChecked = true;
                                        break;
                                    }
                                }

                                if (!valueChecked)
                                {
                                    attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Required"), attribute.AttributeName);
                                    result = false;
                                }
                                break;
                            }
                        case AttributeControlType.ToddlerInt:
                            {
                                if (String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Required"), attribute.AttributeName);
                                    result = false;
                                }
                                else
                                {
                                    double val;
                                    if (!double.TryParse(attribute.AttributeValue, out val))
                                    {
                                        attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int"), attribute.AttributeName);
                                        result = false;
                                    }
                                    else
                                    {
                                        if (val == 0)
                                        {
                                            attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int.GreaterThanZero"), attribute.AttributeName);
                                            result = false;
                                        }
                                    }
                                }

                                break;
                            }
                    }
                }
                else
                {
                    switch (categoryAttribute.AttributeControlType)
                    {
                        case AttributeControlType.MoneyRange:
                            {
                                if (!String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    double resultValue;
                                    double resultvalueMax;
                                    if (!Double.TryParse(attribute.AttributeValue, out resultValue))
                                    {
                                        attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int"), attribute.AttributeName);
                                        result = false;
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(attribute.AttributeValueMax))
                                        {
                                            if (!Double.TryParse(attribute.AttributeValueMax, out resultvalueMax))
                                            {
                                                attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int"), attribute.AttributeName);
                                                result = false;
                                            }
                                            else
                                            {
                                                if (resultvalueMax < resultValue)
                                                {
                                                    attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITBFA.Attributes.MoreThan.Error"), attribute.AttributeName);
                                                    result = false;
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        case AttributeControlType.Money:
                            {
                                if (!String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    double resultValue;
                                    if (!Double.TryParse(attribute.AttributeValue, out resultValue))
                                    {
                                        attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int"), attribute.AttributeName);
                                        result = false;
                                    }
                                    else
                                    {
                                        if (resultValue == 0)
                                        {
                                            attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int.GreaterThanZero"), attribute.AttributeName);
                                            result = false;
                                        }
                                    }
                                }
                                break;
                            }
                        case AttributeControlType.ToddlerInt:
                            {
                                if (!String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    double val;
                                    if (!double.TryParse(attribute.AttributeValue, out val))
                                    {
                                        attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int"), attribute.AttributeName);
                                        result = false;
                                    }
                                    else
                                    {
                                        if (val == 0)
                                        {
                                            attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int.GreaterThanZero"), attribute.AttributeName);
                                            result = false;
                                        }
                                    }
                                }

                                break;
                            }
                    }
                }
            }

            return result;
        }

        public ActionResult EditCustomerInformationProductAttributes(int productId)
        {
            var customerAttributes = _customerInformationAttributeService.GetAllAttributes();
            var product = _productService.GetProductById(productId);
            var model = new EditProductAttributeListModel();
            var currencies = _currencyService.GetAllCurrencies();
            model.AviableCurrencies = currencies.Select(x => new CurrencyModel()
            {
                Id = x.Id,
                CurrencyCode = x.CurrencyCode
            }).ToList();
            model.AttributeList = new List<EditProductAttributeModel>();
            foreach (var attribute in customerAttributes)
            {
                switch (attribute.ProductAddControlType)
                {
                    case CustomerInformationProductAddControlType.TextBox:
                        {
                            var attributeModel = new EditProductAttributeModel();
                            attributeModel.AttributeName = typeof(Customer).GetProperty(attribute.CustomerFieldName).GetCustomAttribute<CustomerFieldIsInProductAttribute>().Alias;
                            var attributeValue = product.CustomerInformationAttributes.Where(x=>x.CustomerInformationProductAttributeId == attribute.Id).FirstOrDefault();
                            if (attributeValue != null)
                            {
                                attributeModel.AttributeValue = attributeValue.ValueString;
                                attributeModel.AttributeValueId = attributeValue.Id;
                            }
                            attributeModel.Id = attribute.Id;
                            attributeModel.AttributeControlTypeId = attribute.ProductAddControlTypeId;
                            model.AttributeList.Add(attributeModel);
                            break;
                        }
                    case CustomerInformationProductAddControlType.CheckBoxes:
                        {
                            var attributeModel = new EditProductAttributeModel();
                            attributeModel.AttributeName = typeof(Customer).GetProperty(attribute.CustomerFieldName).GetCustomAttribute<CustomerFieldIsInProductAttribute>().Alias;
                            attributeModel.AttributeControlTypeId = attribute.ProductAddControlTypeId;
                            var attributeValues = product.CustomerInformationAttributes.Where(x => x.CustomerInformationProductAttributeId == attribute.Id);
                            var aviableAttrbuteValues = _customerInformationAttributeService.GetValuesByAttributeId(attribute.Id);
                            if (aviableAttrbuteValues.Count() > 0)
                            {
                                attributeModel.AttributeValues = new List<EditProductAttributeValueModel>();
                                foreach (var value in aviableAttrbuteValues)
                                {
                                    var attributeValue = new EditProductAttributeValueModel()
                                    {
                                        Id = value.Id,
                                        Selected = attributeValues.Where(x => x.Id == value.Id).FirstOrDefault() != null,
                                        AttributeValue = value.ValueString,
                                    };
                                    attributeModel.AttributeValues.Add(attributeValue);
                                }

                            }
                            attributeModel.Id = attribute.Id;
                            model.AttributeList.Add(attributeModel);
                            break;
                        }
                    case CustomerInformationProductAddControlType.DropDownList:
                        {
                            if (attribute.CustomerFieldName == "CityId")
                                break;
                            var attributeModel = new EditProductAttributeModel();
                            attributeModel.AttributeName = typeof(Customer).GetProperty(attribute.CustomerFieldName).GetCustomAttribute<CustomerFieldIsInProductAttribute>().Alias;
                            attributeModel.AttributeControlTypeId = attribute.ProductAddControlTypeId;
                            var attributeValues = product.CustomerInformationAttributes.Where(x => x.CustomerInformationProductAttributeId == attribute.Id);
                            var aviableAttrbuteValues = _customerInformationAttributeService.GetValuesByAttributeId(attribute.Id);
                            if (aviableAttrbuteValues.Count() > 0)
                            {
                                attributeModel.AttributeValues = new List<EditProductAttributeValueModel>();
                                foreach (var value in aviableAttrbuteValues)
                                {
                                    var referenceValue = _customerInformationAttributeService.GetValue(value.Id);
                                    var attributeValue = new EditProductAttributeValueModel()
                                    {
                                        Id = value.Id,
                                        ReferencedValue = referenceValue.Id,
                                        AttributeValue = referenceValue.Text
                                    };
                                    attributeModel.AttributeValues.Add(attributeValue);
                                }
                                attributeModel.AttributeValues.Add(new EditProductAttributeValueModel()
                                {
                                    Id = 0,
                                    AttributeValue = _localizationService.GetResource("ITB.Attribute.SelectValue")
                                });
                                var selected = attributeValues.FirstOrDefault();
                                if (selected != null)
                                {
                                    attributeModel.AttributeValueId = selected.Id;
                                }

                            }
                            attributeModel.Id = attribute.Id;
                            model.AttributeList.Add(attributeModel);
                            break;
                        }
                    case CustomerInformationProductAddControlType.NumberTextBoxValue:
                        {
                            var attributeModel = new EditProductAttributeModel();
                            attributeModel.AttributeName = typeof(Customer).GetProperty(attribute.CustomerFieldName).GetCustomAttribute<CustomerFieldIsInProductAttribute>().Alias;
                            var attributeValue = product.CustomerInformationAttributes.Where(x => x.CustomerInformationProductAttributeId == attribute.Id).FirstOrDefault();
                            if (attributeValue != null)
                            {
                                attributeModel.AttributeValue = attributeValue.ValueDouble.ToString();
                                attributeModel.AttributeValueId = attributeValue.Id;
                            }
                            attributeModel.Id = attribute.Id;
                            attributeModel.AttributeControlTypeId = attribute.ProductAddControlTypeId;
                            model.AttributeList.Add(attributeModel);
                            break;
                        }
                    case CustomerInformationProductAddControlType.NumberTextBoxLess:
                        {
                            var attributeModel = new EditProductAttributeModel();
                            attributeModel.AttributeName = typeof(Customer).GetProperty(attribute.CustomerFieldName).GetCustomAttribute<CustomerFieldIsInProductAttribute>().Alias;
                            var attributeValue = product.CustomerInformationAttributes.Where(x => x.CustomerInformationProductAttributeId == attribute.Id).FirstOrDefault();
                            if (attributeValue != null)
                            {
                                attributeModel.AttributeValue = attributeValue.ValueMax.ToString();
                                attributeModel.AttributeValueId = attributeValue.Id;
                            }
                            attributeModel.Id = attribute.Id;
                            attributeModel.AttributeControlTypeId = attribute.ProductAddControlTypeId;
                            model.AttributeList.Add(attributeModel);
                            break;
                        }
                    case CustomerInformationProductAddControlType.NumberTetBoxMore:
                        {
                            var attributeModel = new EditProductAttributeModel();
                            attributeModel.AttributeName = typeof(Customer).GetProperty(attribute.CustomerFieldName).GetCustomAttribute<CustomerFieldIsInProductAttribute>().Alias;
                            var attributeValue = product.CustomerInformationAttributes.Where(x => x.CustomerInformationProductAttributeId == attribute.Id).FirstOrDefault();
                            if (attributeValue != null)
                            {
                                attributeModel.AttributeValue = attributeValue.ValueDouble.ToString();
                                attributeModel.AttributeValueId = attributeValue.Id;
                            }
                            attributeModel.Id = attribute.Id;
                            attributeModel.AttributeControlTypeId = attribute.ProductAddControlTypeId;
                            model.AttributeList.Add(attributeModel);
                            break;
                        }
                    case CustomerInformationProductAddControlType.MoneyLess:
                        {
                            var attributeModel = new EditProductAttributeModel();
                            attributeModel.AttributeName = typeof(Customer).GetProperty(attribute.CustomerFieldName).GetCustomAttribute<CustomerFieldIsInProductAttribute>().Alias;
                            var attributeValue = product.CustomerInformationAttributes.Where(x => x.CustomerInformationProductAttributeId == attribute.Id).FirstOrDefault();
                            if (attributeValue != null)
                            {
                                var val = attributeValue.ValueMax;
                                var currency = _currencyService.GetCurrencyById(attributeValue.CurrencyId.Value);
                                val = val * (double)currency.Rate;
                                attributeModel.AttributeValue = val.ToString();
                                attributeModel.AttributeValueId = attributeValue.Id;
                                attributeModel.CurrencyId = currency.Id;
                            }
                            attributeModel.Id = attribute.Id;
                            attributeModel.AttributeControlTypeId = attribute.ProductAddControlTypeId;
                            model.AttributeList.Add(attributeModel);
                            break;
                        }
                    case CustomerInformationProductAddControlType.MoneyMore:
                        {
                            var attributeModel = new EditProductAttributeModel();
                            attributeModel.AttributeName = typeof(Customer).GetProperty(attribute.CustomerFieldName).GetCustomAttribute<CustomerFieldIsInProductAttribute>().Alias;
                            var attributeValue = product.CustomerInformationAttributes.Where(x => x.CustomerInformationProductAttributeId == attribute.Id).FirstOrDefault();
                            if (attributeValue != null)
                            {
                                var val = attributeValue.ValueDouble;
                                var currency = _currencyService.GetCurrencyById(attributeValue.CurrencyId.Value);
                                val = val * (double)currency.Rate;
                                attributeModel.AttributeValue = val.ToString();
                                attributeModel.AttributeValueId = attributeValue.Id;
                                attributeModel.CurrencyId = currency.Id;
                            }
                            attributeModel.Id = attribute.Id;
                            attributeModel.AttributeControlTypeId = attribute.ProductAddControlTypeId;
                            model.AttributeList.Add(attributeModel);
                            break;
                        }
                    case CustomerInformationProductAddControlType.NumberTextBoxRange:
                        {
                            var attributeModel = new EditProductAttributeModel();
                            attributeModel.AttributeName = typeof(Customer).GetProperty(attribute.CustomerFieldName).GetCustomAttribute<CustomerFieldIsInProductAttribute>().Alias;
                            var attributeValue = product.CustomerInformationAttributes.Where(x => x.CustomerInformationProductAttributeId == attribute.Id).FirstOrDefault();
                            if (attributeValue != null)
                            {
                                attributeModel.AttributeValueMax = attributeValue.ValueMax.ToString();
                                attributeModel.AttributeValue = attributeValue.ValueDouble.ToString();
                                attributeModel.AttributeValueId = attributeValue.Id;
                            }
                            attributeModel.Id = attribute.Id;
                            attributeModel.AttributeControlTypeId = attribute.ProductAddControlTypeId;
                            model.AttributeList.Add(attributeModel);
                            break;
                        }
                    case CustomerInformationProductAddControlType.MoneyBetween:
                        {
                            var attributeModel = new EditProductAttributeModel();
                            attributeModel.AttributeName = typeof(Customer).GetProperty(attribute.CustomerFieldName).GetCustomAttribute<CustomerFieldIsInProductAttribute>().Alias;
                            var attributeValue = product.CustomerInformationAttributes.Where(x => x.CustomerInformationProductAttributeId == attribute.Id).FirstOrDefault();
                            if (attributeValue != null)
                            {
                                var val = attributeValue.ValueDouble;
                                var currency = _currencyService.GetCurrencyById(attributeValue.CurrencyId.Value);
                                val = val * (double)currency.Rate;
                                var valmax = attributeValue.ValueMax * (double)currency.Rate;
                                attributeModel.AttributeValueMax = valmax.ToString();
                                attributeModel.AttributeValue = val.ToString();
                                attributeModel.AttributeValueId = attributeValue.Id;
                                attributeModel.CurrencyId = currency.Id;
                            }
                            attributeModel.Id = attribute.Id;
                            attributeModel.AttributeControlTypeId = attribute.ProductAddControlTypeId;
                            model.AttributeList.Add(attributeModel);
                            break;
                        }
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult EditCustomerInformationProductAttributes(EditProductAttributeListModel model)
        {
            if (ValidateCustomerProductAttributes(model))
            {
                UpdateCustomerProductAttributes(model);
                ViewBag.Close = true;
            }
            foreach (var attr in model.AttributeList)
            {
                attr.AttributeControlType = (AttributeControlType)attr.AttributeControlTypeId;
            }
            var currencies = _currencyService.GetAllCurrencies();
            model.AviableCurrencies = currencies.Select(x => new CurrencyModel()
            {
                Id = x.Id,
                CurrencyCode = x.CurrencyCode
            }).ToList();
            return View(model);
        }

        private void UpdateCustomerProductAttributes(EditProductAttributeListModel model)
        {
            var product = _productService.GetProductById(model.ProductId);
            foreach (var attribute in model.AttributeList)
            {
                var categoryAttribute = _customerInformationAttributeService.GetAttributeById(attribute.Id);
                switch (categoryAttribute.ProductAddControlType)
                {
                    case CustomerInformationProductAddControlType.NumberTextBoxRange:
                        {
                            if (attribute.AttributeValueId != 0)
                            {
                                if (String.IsNullOrEmpty(attribute.AttributeValue) || String.IsNullOrEmpty(attribute.AttributeValueMax))
                                {
                                    var existingAttribute = product.CustomerInformationAttributes.Where(x => x.CustomerInformationProductAttributeId == attribute.Id).FirstOrDefault();
                                    if (existingAttribute != null)
                                    {
                                        product.CustomerInformationAttributes.Remove(existingAttribute);
                                    }
                                    _customerInformationAttributeService.DeleteAttributeValue(existingAttribute);
                                }
                                else
                                {
                                    double value = double.Parse(attribute.AttributeValue);
                                    double maxValue = double.Parse(attribute.AttributeValueMax);
                                    var attr = _customerInformationAttributeService.GetAttributeValueById(attribute.AttributeValueId);
                                    attr.ValueString = attribute.AttributeValue;
                                    attr.ValueDouble = value;
                                    attr.ValueMax = maxValue;
                                    _customerInformationAttributeService.UpdateAttributeValue(attr);
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(attribute.AttributeValue) && !String.IsNullOrEmpty(attribute.AttributeValueMax))
                                {
                                    double value = double.Parse(attribute.AttributeValue);
                                    double valueMax = double.Parse(attribute.AttributeValueMax);
                                    var attributeValue = new CustomerInformationProductAttributeValue()
                                    {
                                        CustomerInformationProductAttributeId = attribute.Id,
                                        ValueDouble = value,
                                        ValueString = value.ToString(),
                                        ValueMax = valueMax
                                    };
                                    _customerInformationAttributeService.InsertAttributeValue(attributeValue);
                                    product.CustomerInformationAttributes.Add(attributeValue);
                                    _productService.UpdateProduct(product);
                                }
                            }
                            break;
                        }
                    case CustomerInformationProductAddControlType.NumberTextBoxValue:
                        {
                            if (attribute.AttributeValueId != 0)
                            {
                                if (String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    var existingAttribute = product.CustomerInformationAttributes.Where(x => x.CustomerInformationProductAttributeId == attribute.Id).FirstOrDefault();
                                    if (existingAttribute != null)
                                    {
                                        product.CustomerInformationAttributes.Remove(existingAttribute);
                                    }
                                    _customerInformationAttributeService.DeleteAttributeValue(existingAttribute);
                                }
                                else
                                {
                                    double value = double.Parse(attribute.AttributeValue);
                                    var attr = _customerInformationAttributeService.GetAttributeValueById(attribute.AttributeValueId);
                                    attr.ValueString = attribute.AttributeValue;
                                    attr.ValueDouble = value;
                                    _customerInformationAttributeService.UpdateAttributeValue(attr);
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    double value = double.Parse(attribute.AttributeValue);
                                    var attributeValue = new CustomerInformationProductAttributeValue()
                                    {
                                        CustomerInformationProductAttributeId = attribute.Id,
                                        ValueDouble = value,
                                        ValueString = value.ToString(),
                                    };
                                    _customerInformationAttributeService.InsertAttributeValue(attributeValue);
                                    product.CustomerInformationAttributes.Add(attributeValue);
                                    _productService.UpdateProduct(product);
                                }
                            }
                            break;
                        }
                    case CustomerInformationProductAddControlType.NumberTextBoxLess:
                        {
                            if (attribute.AttributeValueId != 0)
                            {
                                if (String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    var existingAttribute = product.CustomerInformationAttributes.Where(x => x.CustomerInformationProductAttributeId == attribute.Id).FirstOrDefault();
                                    if (existingAttribute != null)
                                    {
                                        product.CustomerInformationAttributes.Remove(existingAttribute);
                                    }
                                    _customerInformationAttributeService.DeleteAttributeValue(existingAttribute);
                                }
                                else
                                {
                                    double value = double.Parse(attribute.AttributeValue);
                                    var attr = _customerInformationAttributeService.GetAttributeValueById(attribute.AttributeValueId);
                                    attr.ValueString = attribute.AttributeValue;
                                    attr.ValueMax = value;
                                    _customerInformationAttributeService.UpdateAttributeValue(attr);
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    double value = double.Parse(attribute.AttributeValue);
                                    var attributeValue = new CustomerInformationProductAttributeValue()
                                    {
                                        CustomerInformationProductAttributeId = attribute.Id,
                                        ValueMax = value,
                                        ValueString = value.ToString(),
                                    };
                                    _customerInformationAttributeService.InsertAttributeValue(attributeValue);
                                    product.CustomerInformationAttributes.Add(attributeValue);
                                    _productService.UpdateProduct(product);
                                }
                            }
                            break;
                        }
                    case CustomerInformationProductAddControlType.NumberTetBoxMore:
                        {
                            if (attribute.AttributeValueId != 0)
                            {
                                if (String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    var existingAttribute = product.CustomerInformationAttributes.Where(x => x.CustomerInformationProductAttributeId == attribute.Id).FirstOrDefault();
                                    if (existingAttribute != null)
                                    {
                                        product.CustomerInformationAttributes.Remove(existingAttribute);
                                    }
                                    _customerInformationAttributeService.DeleteAttributeValue(existingAttribute);
                                }
                                else
                                {
                                    double value = double.Parse(attribute.AttributeValue);
                                    var attr = _customerInformationAttributeService.GetAttributeValueById(attribute.AttributeValueId);
                                    attr.ValueString = attribute.AttributeValue;
                                    attr.ValueDouble = value;
                                    _customerInformationAttributeService.UpdateAttributeValue(attr);
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    double value = double.Parse(attribute.AttributeValue);
                                    var attributeValue = new CustomerInformationProductAttributeValue()
                                    {
                                        CustomerInformationProductAttributeId = attribute.Id,
                                        ValueDouble = value,
                                        ValueString = value.ToString(),
                                    };
                                    _customerInformationAttributeService.InsertAttributeValue(attributeValue);
                                    product.CustomerInformationAttributes.Add(attributeValue);
                                    _productService.UpdateProduct(product);
                                }
                            }
                            break;
                        }
                    case CustomerInformationProductAddControlType.MoneyLess:
                        {
                            if (attribute.AttributeValueId != 0)
                            {
                                if (String.IsNullOrEmpty(attribute.AdditionalValue))
                                {
                                    var existingAttribute = product.CustomerInformationAttributes.Where(x => x.CustomerInformationProductAttributeId == attribute.Id).FirstOrDefault();
                                    if (existingAttribute != null)
                                    {
                                        product.CustomerInformationAttributes.Remove(existingAttribute);
                                    }
                                    _customerInformationAttributeService.DeleteAttributeValue(existingAttribute);
                                }
                                else
                                {
                                    double value = double.Parse(attribute.AttributeValue);
                                    var currency = _currencyService.GetCurrencyById(attribute.CurrencyId);
                                    value = value / (double)currency.Rate;
                                    var attr = _customerInformationAttributeService.GetAttributeValueById(attribute.AttributeValueId);
                                    attr.ValueString = attribute.AttributeValue;
                                    attr.ValueMax = value;
                                    attr.ValueDouble = value;
                                    attr.CurrencyId = attribute.CurrencyId;
                                    _customerInformationAttributeService.UpdateAttributeValue(attr);
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    double value = double.Parse(attribute.AttributeValue);
                                    var currency = _currencyService.GetCurrencyById(attribute.CurrencyId);
                                    value = value / (double)currency.Rate;
                                    var attributeValue = new CustomerInformationProductAttributeValue()
                                    {
                                        CustomerInformationProductAttributeId = attribute.Id,
                                        ValueMax = value,
                                        ValueDouble = value,
                                        CurrencyId = attribute.CurrencyId,
                                        ValueString = value.ToString(),
                                    };
                                    _customerInformationAttributeService.InsertAttributeValue(attributeValue);
                                    product.CustomerInformationAttributes.Add(attributeValue);
                                    _productService.UpdateProduct(product);
                                }
                            }
                            break;
                        }
                    case CustomerInformationProductAddControlType.MoneyMore:
                        {
                            if (attribute.AttributeValueId != 0)
                            {
                                if (String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    var existingAttribute = product.CustomerInformationAttributes.Where(x => x.CustomerInformationProductAttributeId == attribute.Id).FirstOrDefault();
                                    if (existingAttribute != null)
                                    {
                                        product.CustomerInformationAttributes.Remove(existingAttribute);
                                    }
                                    _customerInformationAttributeService.DeleteAttributeValue(existingAttribute);
                                }
                                else
                                {
                                    double value = double.Parse(attribute.AttributeValue);
                                    var currency = _currencyService.GetCurrencyById(attribute.CurrencyId);
                                    value = value / (double)currency.Rate;
                                    var attr = _customerInformationAttributeService.GetAttributeValueById(attribute.AttributeValueId);
                                    attr.ValueString = attribute.AttributeValue;
                                    attr.ValueDouble = value;
                                    attr.ValueMax = value;
                                    attr.CurrencyId = attribute.CurrencyId;
                                    _customerInformationAttributeService.UpdateAttributeValue(attr);
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    double value = double.Parse(attribute.AttributeValue);
                                    var currency = _currencyService.GetCurrencyById(attribute.CurrencyId);
                                    value = value / (double)currency.Rate;
                                    var attributeValue = new CustomerInformationProductAttributeValue()
                                    {
                                        CustomerInformationProductAttributeId = attribute.Id,
                                        ValueDouble = value,
                                        ValueMax = value,
                                        CurrencyId = attribute.CurrencyId,
                                        ValueString = value.ToString(),
                                    };
                                    _customerInformationAttributeService.InsertAttributeValue(attributeValue);
                                    product.CustomerInformationAttributes.Add(attributeValue);
                                    _productService.UpdateProduct(product);
                                }
                            }
                            break;
                        }
                    case CustomerInformationProductAddControlType.MoneyBetween:
                        {
                            if (attribute.AttributeValueId != 0)
                            {
                                if (String.IsNullOrEmpty(attribute.AttributeValue) || String.IsNullOrEmpty(attribute.AttributeValueMax))
                                {
                                    var existingAttribute = product.CustomerInformationAttributes.Where(x => x.CustomerInformationProductAttributeId == attribute.Id).FirstOrDefault();
                                    if (existingAttribute != null)
                                    {
                                        product.CustomerInformationAttributes.Remove(existingAttribute);
                                    }
                                    _customerInformationAttributeService.DeleteAttributeValue(existingAttribute);
                                }
                                else
                                {
                                    double value = double.Parse(attribute.AttributeValue);
                                    
                                    double maxValue = double.Parse(attribute.AttributeValueMax);
                                    var currency = _currencyService.GetCurrencyById(attribute.CurrencyId);
                                    value = value / (double)currency.Rate;
                                    maxValue = maxValue / (double)currency.Rate;
                                    var attr = _customerInformationAttributeService.GetAttributeValueById(attribute.AttributeValueId);
                                    attr.ValueString = attribute.AttributeValue;
                                    attr.ValueDouble = value;
                                    attr.ValueMax = maxValue;
                                    attr.CurrencyId = attribute.CurrencyId;
                                    _customerInformationAttributeService.UpdateAttributeValue(attr);
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(attribute.AttributeValue) && !String.IsNullOrEmpty(attribute.AttributeValueMax))
                                {
                                    double value = double.Parse(attribute.AttributeValue);
                                    double valueMax = double.Parse(attribute.AttributeValueMax);
                                    var currency = _currencyService.GetCurrencyById(attribute.CurrencyId);
                                    value = value / (double)currency.Rate;
                                    valueMax = valueMax / (double)currency.Rate;
                                    var attributeValue = new CustomerInformationProductAttributeValue()
                                    {
                                        CustomerInformationProductAttributeId = attribute.Id,
                                        ValueDouble = value,
                                        ValueString = value.ToString(),
                                        CurrencyId = attribute.CurrencyId,
                                        ValueMax = valueMax
                                    };
                                    _customerInformationAttributeService.InsertAttributeValue(attributeValue);
                                    product.CustomerInformationAttributes.Add(attributeValue);
                                    _productService.UpdateProduct(product);
                                }
                            }
                            break;
                        }
                    case CustomerInformationProductAddControlType.TextBox:
                        {
                            if (attribute.AttributeValueId != 0)
                            {
                                if (String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    var attr = _customerInformationAttributeService.GetAttributeValueById(attribute.AttributeValueId);
                                    product.CustomerInformationAttributes.Remove(attr);
                                    _productService.UpdateProduct(product);
                                    _customerInformationAttributeService.DeleteAttributeValue(attr);
                                }
                                else
                                {
                                    var attr = _customerInformationAttributeService.GetAttributeValueById(attribute.AttributeValueId);
                                    attr.ValueString = attribute.AttributeValue;
                                    _customerInformationAttributeService.UpdateAttributeValue(attr);
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    var attributeValue = new CustomerInformationProductAttributeValue()
                                    {
                                        CustomerInformationProductAttributeId = attribute.Id,
                                        ValueString = attribute.AttributeValue,
                                    };
                                    _customerInformationAttributeService.InsertAttributeValue(attributeValue);
                                    product.CustomerInformationAttributes.Add(attributeValue);
                                    _productService.UpdateProduct(product);
                                }
                            }
                            break;
                        }
                    case CustomerInformationProductAddControlType.DropDownList:
                        {
                            var existingAttribute = product.CustomerInformationAttributes.Where(x => x.CustomerInformationProductAttributeId == attribute.Id).FirstOrDefault();
                            if (existingAttribute != null && existingAttribute.Id != attribute.AttributeValueId)
                            {
                                product.CustomerInformationAttributes.Remove(existingAttribute);
                                _productService.UpdateProduct(product);
                                var attr = _customerInformationAttributeService.GetAttributeValueById(attribute.AttributeValueId);
                                product.CustomerInformationAttributes.Add(attr);
                                _productService.UpdateProduct(product);
                            }
                            else
                            {
                                var attrValue = _customerInformationAttributeService.GetAttributeValueById(attribute.AttributeValueId);
                                if (attrValue != null)
                                {
                                    product.CustomerInformationAttributes.Add(attrValue);
                                    _productService.UpdateProduct(product);
                                }
                            }
                            break;
                        }
                    case CustomerInformationProductAddControlType.CheckBoxes:
                        {
                            foreach (var attr in attribute.AttributeValues)
                            {
                                if (attr.Selected)
                                {
                                    var existingAttribute = product.CustomerInformationAttributes.Where(x => x.Id == attr.Id).FirstOrDefault();
                                    if (existingAttribute == null)
                                    {
                                        var attributeNew = _customerInformationAttributeService.GetAttributeValueById(attr.Id);
                                        product.CustomerInformationAttributes.Add(attributeNew);
                                        _productService.UpdateProduct(product);
                                    }
                                }
                                else
                                {
                                    var existingAttribute = product.CustomerInformationAttributes.Where(x => x.Id == attr.Id).FirstOrDefault();
                                    if (existingAttribute != null)
                                    {
                                        product.CustomerInformationAttributes.Remove(existingAttribute);
                                        _productService.UpdateProduct(product);
                                    }
                                }
                            }
                            break;
                        }
                }
            }
        }

        private bool ValidateCustomerProductAttributes(EditProductAttributeListModel model)
        {
            bool result = true;
            foreach (var attribute in model.AttributeList)
            {
                var categoryAttribute = _customerInformationAttributeService.GetAttributeById(attribute.Id);
                if (categoryAttribute.IsRequired)
                {
                    switch (categoryAttribute.ProductAddControlType)
                    {
                        case CustomerInformationProductAddControlType.TextBox:
                            {
                                if (String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Required"), attribute.AttributeName);
                                    result = false;
                                }
                                break;
                            }
                        case CustomerInformationProductAddControlType.NumberTextBoxRange:
                        case CustomerInformationProductAddControlType.MoneyBetween:
                            {
                                double min = 0, max = 0;
                                if (String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Required"), attribute.AttributeName);
                                    result = false;
                                }
                                else
                                {
                                    double resultValue;
                                    if (!Double.TryParse(attribute.AttributeValue, out resultValue))
                                    {
                                        attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int"), attribute.AttributeName);
                                        result = false;
                                    }
                                    else
                                    {
                                        min = resultValue;
                                    }
                                }

                                if (String.IsNullOrEmpty(attribute.AttributeValueMax))
                                {
                                    attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Required"), attribute.AttributeName);
                                    result = false;
                                }
                                else
                                {
                                    double resultValue;
                                    if (!Double.TryParse(attribute.AttributeValueMax, out resultValue))
                                    {
                                        attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int"), attribute.AttributeName);
                                        result = false;
                                    }
                                    else
                                    {
                                        max = resultValue;
                                    }
                                }

                                if (min != 0 || max != 0)
                                {
                                    if (min >= max)
                                    {
                                        attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int.Greater"), attribute.AttributeName);
                                        result = false;
                                    }
                                }
                                break;
                            }
                        case CustomerInformationProductAddControlType.NumberTextBoxValue:
                        case CustomerInformationProductAddControlType.MoneyLess:
                        case CustomerInformationProductAddControlType.MoneyMore:
                            {
                                if (String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Required"), attribute.AttributeName);
                                    result = false;
                                }
                                else
                                {
                                    double resultValue;
                                    if (!Double.TryParse(attribute.AttributeValue, out resultValue))
                                    {
                                        attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int"), attribute.AttributeName);
                                        result = false;
                                    }
                                    else
                                    {
                                        if (resultValue == 0)
                                        {
                                            attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int.GreaterThanZero"), attribute.AttributeName);
                                            result = false;
                                        }
                                    }
                                }
                                break;
                            }
                        case CustomerInformationProductAddControlType.DropDownList:
                            {
                                if (attribute.AttributeValueId == 0)
                                {
                                    attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Required"), attribute.AttributeName);
                                    result = false;
                                }
                                break;
                            }
                        case CustomerInformationProductAddControlType.CheckBoxes:
                            {
                                bool valueChecked = false;
                                foreach (var value in attribute.AttributeValues)
                                {
                                    if (value.Selected)
                                    {
                                        valueChecked = true;
                                        break;
                                    }
                                }

                                if (!valueChecked)
                                {
                                    attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Required"), attribute.AttributeName);
                                    result = false;
                                }
                                break;
                            }
                        case CustomerInformationProductAddControlType.ReferenceValue:
                            {
                                bool valueChecked = false;
                                foreach (var value in attribute.AttributeValues)
                                {
                                    if (value.Selected)
                                    {
                                        valueChecked = true;
                                        break;
                                    }
                                }

                                if (!valueChecked)
                                {
                                    attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Required"), attribute.AttributeName);
                                    result = false;
                                }
                                break;
                            }
                    }
                }
                else
                {
                    switch (categoryAttribute.ProductAddControlType)
                    {
                        case CustomerInformationProductAddControlType.NumberTextBoxRange:
                        case CustomerInformationProductAddControlType.MoneyBetween:
                            {
                                double min = 0, max = 0;
                                if (!String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    double resultValue;
                                    if (!Double.TryParse(attribute.AttributeValue, out resultValue))
                                    {
                                        attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int"), attribute.AttributeName);
                                        result = false;
                                    }
                                    else
                                    {
                                        min = resultValue;
                                    }
                                }

                                if (!String.IsNullOrEmpty(attribute.AttributeValueMax))
                                {
                                    double resultValue;
                                    if (!Double.TryParse(attribute.AttributeValueMax, out resultValue))
                                    {
                                        attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int"), attribute.AttributeName);
                                        result = false;
                                    }
                                    else
                                    {
                                        if (resultValue == 0)
                                        {
                                            attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int.GreaterThanZero"), attribute.AttributeName);
                                            result = false;
                                        }
                                        else
                                        {
                                            max = resultValue;
                                        }
                                    }
                                }
                                if (min != 0 || max != 0)
                                {
                                    if (min >= max)
                                    {
                                        attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int.Greater"), attribute.AttributeName);
                                        result = false;
                                    }
                                }
                                break;
                            }
                        case CustomerInformationProductAddControlType.NumberTextBoxValue:
                        case CustomerInformationProductAddControlType.MoneyLess:
                        case CustomerInformationProductAddControlType.MoneyMore:
                            {
                                if (!String.IsNullOrEmpty(attribute.AttributeValue))
                                {
                                    double resultValue;
                                    if (!Double.TryParse(attribute.AttributeValue, out resultValue))
                                    {
                                        attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int"), attribute.AttributeName);
                                        result = false;
                                    }
                                    else
                                    {
                                        if (resultValue == 0)
                                        {
                                            attribute.ValidationMessage = String.Format(_localizationService.GetResource("ITB.Attribute.Int.GreaterThanZero"), attribute.AttributeName);
                                            result = false;
                                        }
                                    }
                                }
                                break;
                            }
                    }
                }
            }

            return result;
        }
    }
}
