using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Web.Models.BuyingRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Web.Extensions;
using Nop.Services.Seo;
using Nop.Services.Localization;
using Nop.Web.Models.Media;
using Nop.Services.Media;
using Nop.Web.Models.Catalog;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Services.Directory;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Orders;
using Nop.Services.Messages;
using Nop.Services.Common;
using Nop.Services.Logging;
using Nop.Core.Domain.Media;
using Nop.Services.Events;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Core.Caching;
using Nop.Web.Framework.UI.Captcha;
using Nop.Web.Infrastructure.Cache;
using Nop.Services.RequestServices;
using Nop.Web.Models.Request;
using Nop.Web.Models.Brand;
using Nop.Web.Models.SellerCatalog;
using Nop.Web.Models.Customer;
using Nop.Web.Models.CompanyInformation;
using System.ServiceModel.Syndication;
using Nop.Web.Framework;
using Nop.Services.Configuration;
using Nop.Services.Favorits;
using Nop.Core.Domain.Favorit;
using Nop.Services.ExportImport;
using System.IO;

namespace Nop.Web.Controllers
{
    public class CatalogueController : BaseNopController
    {

        #region Fields
        private readonly ILanguageService _languageService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly IManufacturerTemplateService _manufacturerTemplateService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IWorkContext _workContext;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWebHelper _webHelper;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ICustomerContentService _customerContentService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly ICompareProductsService _compareProductsService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IProductTagService _productTagService;
        private readonly IOrderReportService _orderReportService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly IAclService _aclService;
        private readonly IPermissionService _permissionService;
        private readonly IDownloadService _downloadService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRequestService _requestService;
        private readonly ISettingService _settingService;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly ICacheManager _cacheManager;
        private readonly CaptchaSettings _captchaSettings;
        private readonly ICustomerService _customerService;
        private readonly IRequestEmailSender _requestEmailSender;
        private readonly IFavoritsService _favoritsService;
        private readonly IProductPdfService _productPdfService;
        private readonly IProductPriceService _productPriceService;
        private readonly ICategoryProductAttributeService _categoryProductAttributeService;
        private readonly IMobileDeviceHelper _mobileDeviceHelper;
        #endregion

        public CatalogueController(
            ILanguageService languageService,
            IUrlRecordService urlRecordService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService, IProductService productService,
            IProductTemplateService productTemplateService,
            ICategoryTemplateService categoryTemplateService,
            IManufacturerTemplateService manufacturerTemplateService,
            IProductAttributeService productAttributeService, IProductAttributeParser productAttributeParser,
            IWorkContext workContext, ITaxService taxService, ICurrencyService currencyService,
            IPictureService pictureService, ILocalizationService localizationService,
            IPriceCalculationService priceCalculationService, IPriceFormatter priceFormatter,
            IWebHelper webHelper, ISpecificationAttributeService specificationAttributeService,
            ICustomerContentService customerContentService, IDateTimeHelper dateTimeHelper,
            IShoppingCartService shoppingCartService,
            IRecentlyViewedProductsService recentlyViewedProductsService, ICompareProductsService compareProductsService,
            IWorkflowMessageService workflowMessageService, IProductTagService productTagService,
            IOrderReportService orderReportService, IGenericAttributeService genericAttributeService,
            IBackInStockSubscriptionService backInStockSubscriptionService, IAclService aclService,
            IPermissionService permissionService, IDownloadService downloadService,
            ICustomerActivityService customerActivityService, IEventPublisher eventPublisher,
            MediaSettings mediaSettings, CatalogSettings catalogSettings,
            ShoppingCartSettings shoppingCartSettings, StoreInformationSettings storeInformationSettings,
            LocalizationSettings localizationSettings, CustomerSettings customerSettings,
            CaptchaSettings captchaSettings,
            ICacheManager cacheManager,
            IRequestService requestService,
            ICustomerService customerService,
            ISettingService settingService,
            IRequestEmailSender requestEmailSender,
            IFavoritsService favoritsService,
            IProductPdfService productPdfService,
            IProductPriceService productPriceService,
            ICategoryProductAttributeService categoryProductAttributeService,
            IMobileDeviceHelper mobileDeviceHelper
            )
        {
            
            this._urlRecordService = urlRecordService;
            this._languageService = languageService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            this._productTemplateService = productTemplateService;
            this._categoryTemplateService = categoryTemplateService;
            this._manufacturerTemplateService = manufacturerTemplateService;
            this._productAttributeService = productAttributeService;
            this._productAttributeParser = productAttributeParser;
            this._workContext = workContext;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._webHelper = webHelper;
            this._specificationAttributeService = specificationAttributeService;
            this._customerContentService = customerContentService;
            this._dateTimeHelper = dateTimeHelper;
            this._shoppingCartService = shoppingCartService;
            this._recentlyViewedProductsService = recentlyViewedProductsService;
            this._compareProductsService = compareProductsService;
            this._workflowMessageService = workflowMessageService;
            this._productTagService = productTagService;
            this._orderReportService = orderReportService;
            this._genericAttributeService = genericAttributeService;
            this._backInStockSubscriptionService = backInStockSubscriptionService;
            this._aclService = aclService;
            this._permissionService = permissionService;
            this._downloadService = downloadService;
            this._customerActivityService = customerActivityService;
            this._eventPublisher = eventPublisher;
            this._requestService = requestService;
            this._customerService = customerService;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._localizationSettings = localizationSettings;
            this._customerSettings = customerSettings;
            this._captchaSettings = captchaSettings;
            this._settingService = settingService;
            this._cacheManager = cacheManager;
            this._requestEmailSender = requestEmailSender;
            this._favoritsService = favoritsService;
            this._productPdfService = productPdfService;
            this._productPriceService = productPriceService;
            this._categoryProductAttributeService = categoryProductAttributeService;
            this._mobileDeviceHelper = mobileDeviceHelper;
        }

        #region Utilities
        /// <summary>
        /// Prepare array of categories
        /// </summary>
        /// <param name="categoryId">last child category to prepare array from</param>
        /// <param name="model"></param>
        [NonAction]
        protected CategoryNavigationModel[] PrepareCategoryNavigationArray(int categoryId)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            int lavel = 1;
            while (category.ParentCategoryId != 0)
            {
                lavel++;
                category = _categoryService.GetCategoryById(category.ParentCategoryId);
            }
            int lvl1 = 1;
            int lvl2 = 1;
            var categories = new CategoryNavigationModel[lavel];
            category = _categoryService.GetCategoryById(_categoryService.GetCategoryById(categoryId).ParentCategoryId);
            int prevCategory = categoryId;
            for (int i = lavel - 1; i > 0; i--)
            {
                categories[i] = new CategoryNavigationModel();
                categories[i].CurrentCategoryId = prevCategory;
                categories[i].Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(category.Id,ref lvl1);
                if (category.ParentCategoryId != 0)
                {
                    prevCategory = category.Id;
                    category = _categoryService.GetCategoryById(category.ParentCategoryId);
                }
            }

            categories[0] = new CategoryNavigationModel();
            categories[0].CurrentCategoryId = category == null ? categoryId : category.Id;
            categories[0].Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(0,ref lvl2);

            return categories;
        }

        /// <summary>
        /// Prepare buiyng request catalog model
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        [NonAction]
        protected BuyingRequestCatalogModel PrepareBuyingRequestCatalogModel(Product entity, int languageId)
        {
            var model = new BuyingRequestCatalogModel();
            var timeZone = _dateTimeHelper.CurrentTimeZone;
            model.Id = entity.Id;
            model.CreatedOnUtc = _dateTimeHelper.ConvertToUserTime(entity.CreatedOnUtc, TimeZoneInfo.Utc, timeZone);
            model.ProductId = entity.Id;
            model.ProductSeName = entity.GetSeName(languageId);
            model.Title = entity.Name;
            model.ShortDescription = entity.ShortDescription;
            model.CompanyTitle = entity.Customer.CompanyInformation.CompanyName;
            model.CompanySeName = entity.Customer.CompanyInformation.GetSeName();

            if (entity.ProductPictures.Count > 0)
            {
                var produtPicture = entity.ProductPictures.Where(x => x.DisplayOrder == 0).FirstOrDefault();
                if (produtPicture != null)
                {
                    model.Picture = new PictureModel()
                    {
                        ImageUrl = _pictureService.GetPictureUrl(produtPicture.PictureId, showDefaultPicture: false, targetSize:180),
                    };
                }
                else
                {
                    model.Picture = new PictureModel()
                    {
                        ImageUrl = _pictureService.GetPictureUrl(entity.ProductPictures.First().PictureId, showDefaultPicture: false, targetSize: 180)
                    };
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare product details page model
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [NonAction]
        protected ProductDetailsModel PrepareProductDetailsPageModel(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var model = new ProductDetailsModel()
            {
                Id = product.Id,
                Name = product.Name,
                CategoryId = product.ProductCategories.First().CategoryId,
                ShortDescription = product.ShortDescription,
                FullDescription = product.FullDescription,
                OrderingComments = product.AdminComment,
                MetaKeywords = product.MetaDescription,
                Rating = Math.Round(product.Rating ?? 0),
                BankRating = Math.Round(product.Customer.Rating ?? 0),
                MetaDescription = product.MetaKeywords,
                MetaTitle = product.MetaTitle,
                SeName = product.GetSeName(),
                MinimumOrderQuantity = product.MinimumOrderQuantity.HasValue ? product.MinimumOrderQuantity.Value : 1,
                Favorit = _favoritsService.IsItemFavorit(_workContext.CurrentCustomer.Id, product.Id),
                OrderingLink = product.OrderLink
            };

            var bank = product.Customer;
            if (bank.ProviderLogoId.HasValue)
            {
                var pictureUrl = _pictureService.GetPictureUrl(bank.ProviderLogoId.Value, 200);
                model.DefaultPictureModel = new PictureModel()
                {
                    ImageUrl = pictureUrl
                };
            }

            model.CompanyInformationModel = new CompanyInformationModel();
            model.CompanyInformationModel.CompanyName = bank.CompanyName;
            //template

            var templateCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_TEMPLATE_MODEL_KEY, product.ProductTemplateId);
            model.ProductTemplateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _productTemplateService.GetProductTemplateById(product.ProductTemplateId);
                if (template == null)
                    template = _productTemplateService.GetAllProductTemplates().FirstOrDefault();
                return template.ViewPath;
            });

            //pictures
            //product variants
            //foreach (var variant in _productService.GetProductVariantsByProductId(product.Id))
            //    model.ProductVariantModels.Add(PrepareProductVariantModel(new ProductDetailsModel.ProductVariantModel(), variant));

            //product attributes

            //foreach (var item in product.ProductAttributes)
            //{
            //    model.ProductAttributes.Add(new ProductDetailsModel.ProductAttributesModel()
            //    {
            //        AttributeControlTypeId = item.CategoryProductAttribute.AttributeControlTypeId,
            //        ProductAttributeName = item.CategoryProductAttribute.ProductAttribute.Name,
            //        ProductAttributeValue = item.Name,
            //        ColorSquaresRgb = item.ColorSquaresRgb,
            //        DisplayOrder = item.DisplayOrder
            //    });
            //}
            List<CategoryProductAttributeGroup> _attrGroups = product.ProductAttributes.Select(x => x.CategoryProductAttribute.CategoryProductGroup).Distinct().ToList();
            List<CategoryAttributeModel> DisplayedAttributes = new List<CategoryAttributeModel>();
            foreach (var _aG in _attrGroups)
            {
                foreach (var cpa in _aG.CategoryProductAttributes)
                {
                    CategoryAttributeModel cam = new CategoryAttributeModel();
                    cam.Values = new List<CategoryProductAttributeValueModel>();
                    cam.Values.AddRange(cpa.CategoryProductAttributeValues.OrderBy(x => x.DisplayOrder)
                        .ThenBy(x => x.Name)
                        .Select(x=>{
                            var md = new CategoryProductAttributeValueModel();
                            if (x.CategoryProductAttribute.AttributeControlType != AttributeControlType.TextBox)
                            {
                                md.Name = x.Name;
                            }
                            else
                            {
                                md.Name = x.Name;
                            }
                            if (x.CategoryProductAttribute.AttributeControlType == AttributeControlType.Money || x.CategoryProductAttribute.AttributeControlType == AttributeControlType.MoneyRange)
                            {
                                if (x.CurrencyId.HasValue)
                                {
                                    var currency = _currencyService.GetCurrencyById(x.CurrencyId.Value);
                                    md.Name = ((int)(x.RealValue * (double)currency.Rate)).ToString("D");
                                    if(x.RealValueMax.HasValue)
                                    {
                                        md.NameMax = ((int)(x.RealValueMax * (double)currency.Rate)).ToString("D");
                                    }
                                }
                            }
                            md.IsPreSelected = product.ProductAttributes.Where(p => p.Id == x.Id).Count() > 0;
                            md.CategoryProductAttributeId = x.CategoryProductAttributeId;
                            md.Id = x.Id;
                            md.DisplayOrder = x.DisplayOrder;
                            md.ColorSquaresRgb = x.ColorSquaresRgb;
                            md.CategoryProductAttributeId = x.CategoryProductAttributeId;
                            cam.CurrencyCode = x.CurrencyId.HasValue && x.CurrencyId.Value != 0 ? x.Currency.CurrencyCode : "";
                            return md;
                        })
                        .ToList());
                    cam.Name = cpa.ProductAttribute.Name;
                    cam.ControlType = cpa.AttributeControlType;
                    //cam.Values.ForEach(x =>
                    //{
                    //    x.IsPreSelected = product.ProductAttributes.Where(i => i.Id == x.Id).Count() > 0;
                    //});
                    //foreach (var val in cam.Values)
                    //{
                    //    val.IsPreSelected = product.ProductAttributes.Where(p => p.Id == val.Id).Count() > 0;
                    //}
                    var attrValue = cam.Values.Where(i => i.IsPreSelected).FirstOrDefault();
                    cam.SelectedValue = attrValue;
                    cam.DisplayOrder = cpa.DisplayOrder;
                    //cam.SelectedValue.Name = attrValue.GetLocalized(v => v.Name, _workContext.WorkingLanguage.Id, true);
                    //cam.Values.ForEach(i => { i.Name = i.GetLocalized(xi => xi.Name, _workContext.WorkingLanguage.Id, true); });
                    DisplayedAttributes.Add(cam);
                }
            }
            model.CategoryAttributes = DisplayedAttributes.OrderBy(x=>x.DisplayOrder).ToList();
            model.HaveConversionImages = GetProductConversionImagesModels(product.ProductCategories.First()).Count > 0;
            //product tags
            foreach (var item in product.ProductTags)
            {
                model.ProductTags.Add(new ProductTagModel()
                {
                    Name = item.Name,
                    ProductCount = item.ProductCount,
                    Id = item.Id
                });
            }
            return model;
        }

        /// <summary>
        /// Prepare category navigation model
        /// </summary>
        /// <param name="rootCategoryId">root category</param>
        /// <returns></returns>
        [NonAction]
        protected IList<CategoryNavigationModel.CategoryModel> PrepareCategoryNavigationModel(int rootCategoryId, ref int lavel)
        {
            lavel--;
            int newLavel = lavel;
            var result = new List<CategoryNavigationModel.CategoryModel>();
            foreach (var category in _categoryService.GetAllCategoriesByParentCategoryId(rootCategoryId))
            {
                var categoryModel = new CategoryNavigationModel.CategoryModel()
                {
                    Id = category.Id,
                    Name = category.Name,
                    SeName = category.GetSeName()
                };
                //subcategories
                if(lavel > 0)
                    categoryModel.SubCategories.AddRange(PrepareCategoryNavigationModel(category.Id, ref newLavel));

                result.Add(categoryModel);
            }

            return result;
        }

        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="viewName">view name</param>
        /// <param name="model">model to pass to view</param>
        /// <returns></returns>
        [NonAction]
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

        /// <summary>
        /// Filter products tha do not belongs to provuded category
        /// </summary>
        /// <param name="products">products to filter</param>
        /// <param name="categoryid">category id</param>
        /// <returns></returns>
        [NonAction]
        protected List<Product> FilterProductsByCategoryId(List<Product> products, int categoryid, List<Product> resultproducts = null)
        {
            if (resultproducts == null)
                resultproducts = new List<Product>();
            var childCategories = _categoryService.GetAllCategoriesByParentCategoryId(categoryid);
            if (childCategories.Count == 0)
            {
                resultproducts.AddRange(products.Where(x => x.ProductCategories.First().CategoryId == categoryid));
            }
            else
            {
                foreach (var category in childCategories)
                {
                    FilterProductsByCategoryId(products, category.Id, resultproducts);
                }
            } return resultproducts;
        }

        [NonAction]
        protected IList<Category> GetCategoryBreadCrumb(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            var breadCrumb = new List<Category>();

            while (category != null && //category is not null
                !category.Deleted && //category is not deleted
                category.Published && //category is published
                _aclService.Authorize(category)) //ACL
            {
                breadCrumb.Add(category);
                category = _categoryService.GetCategoryById(category.ParentCategoryId);
            }
            breadCrumb.Reverse();
            return breadCrumb;
        }

        [ChildActionOnly]
        public ActionResult ProductBreadcrumb(int productId, int productItemType = 0)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            if (!_catalogSettings.CategoryBreadcrumbEnabled)
                return Content("");

            var customerRolesIds = _workContext.CurrentCustomer.CustomerRoles
                .Where(cr => cr.Active).Select(cr => cr.Id).ToList();
            var cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_BREADCRUMB_MODEL_KEY, product.Id, _workContext.WorkingLanguage.Id, string.Join(",", customerRolesIds));
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var model = new ProductDetailsModel.ProductBreadcrumbModel()
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductSeName = product.GetSeName(),
                    ProductItemTypeAnchor = productItemType > 0 ? ((ProductItemTypeEnum)productItemType).ToString() : "Product"
                };
                var productCategories = _categoryService.GetProductCategoriesByProductId(product.Id);
                if (productCategories.Count > 0)
                {
                    var category = productCategories[0].Category;
                    if (category != null)
                    {
                        foreach (var catBr in GetCategoryBreadCrumb(category))
                        {
                            model.CategoryBreadcrumb.Add(new CategoryModel()
                            {
                                Id = catBr.Id,
                                Name = catBr.Name,
                                SeName = catBr.GetSeName()
                            });
                        }
                    }
                }
                return model;
            });

            return PartialView("_ProductBreadcrumb", cacheModel);
        }

        public ActionResult SelectCategory(int productId)
        {
            object cacheModel = new object();
            return PartialView("_SelectCategory", cacheModel);
        }
        #endregion

        /// <summary>
        /// Display list of buying requests
        /// </summary>
        /// <param name="command">paging filtering command</param>
        /// <returns></returns>
        public ActionResult BuyingRequests(BuyingRequestPagableModel command)
        {
            command.PageSize = 5;
            command.ProductItemTypeId = (int)ProductItemTypeEnum.ProductBuyingRequest;
            var products = _productService.GetAllProducts()
                .Where(x => x.Published && !x.Deleted)
                .Where(x => x.Name != null).ToList();
            if (command.CategoryId != 0)
            {
                products = products.Where(x => x.ProductCategories.First().CategoryId == command.CategoryId).ToList();
            }
            var buyingrequestModel = products.Select(x => PrepareBuyingRequestCatalogModel(x, _workContext.WorkingLanguage.Id));
            var model = new BuyingRequestCatalogListModel();
            model.PagingContext = command;
            var list = new PagedList<BuyingRequestCatalogModel>(buyingrequestModel.ToList(), command.PageIndex, command.PageSize);
            model.PagingContext.LoadPagedList(list);
            model.BuyingRequestList = list;
            int lvl = 1;
            model.Categories = new CategoryNavigationModel[1];
            model.Categories[0] = new CategoryNavigationModel();
            model.Categories[0].Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(0,ref lvl);
            model.ItemType = (int)ProductItemTypeEnum.ProductBuyingRequest;
            return View("Category", model);
        }

        /// <summary>
        /// Display catalog of product/services/productbuyingrequests/servicebuyingrequests filtered by category
        /// </summary>
        /// <param name="SeName">category se name</param>
        /// <param name="itemtype">item type</param>
        /// <param name="command">paging command</param>
        /// <returns></returns>
        public ActionResult Category(string SeName, string itemtype, BuyingRequestPagableModel command)
        {
            command.PageSize = _catalogSettings.ItemCatalogPageSize;
            if (SeName == "all")
            {
                command.CategoryId = 0;
            }
            else
            {
                var slug = _urlRecordService.GetBySlug(SeName);
                if (slug == null)
                {
                    throw new HttpException(404, "Not found");
                }
                command.CategoryId = slug.EntityId;
            }
            ProductItemTypeEnum itemtypevlue;
            int producttype;
            if (!Enum.TryParse(itemtype, true, out itemtypevlue))
            {
                return RedirectToRoute("HomePage");
            }

            if (!_workContext.CurrentCustomer.IsRegistered() && (itemtypevlue == ProductItemTypeEnum.ServiceBuyingRequest || itemtypevlue == ProductItemTypeEnum.ProductBuyingRequest))
            {
                return RedirectToRoute("HomePage");
            }
            int productTagId = command.ProductTagId;
            producttype = (int)itemtypevlue;
            var products = _productService.NewSearchProducts(command.CustomerId, producttype, command.BrandId, ProductSortingEnum.CreatedOn, command.PageIndex, command.PageSize, productTagId, command.CategoryId, _workContext.WorkingLanguage.Id);
            //var products = _productService.GetAllProducts()
            //    .Where(x => x.ProductItemTypeId == producttype)
            //    .Where(x => x.Published && !x.Deleted)
            //    .Where(x => x.GetLocalized(z => z.Name, _workContext.WorkingLanguage.Id, false) != null)
            //    .OrderByDescending(x=>x.CreatedOnUtc).ToList();
            //
            //if (command.BrandId != 0)
            //{
            //    products = products.Where(x => x.BrandId == command.BrandId).ToList();
            //}
            //if (command.CategoryId != 0)
            //{
            //    products = FilterProductsByCategoryId(products, command.CategoryId);
            //}
            //
            //if (command.CustomerId != 0)
            //{
            //    products = products.Where(x => x.CustomerId == command.CustomerId).ToList();
            //}

            if (products.Count == 0 && command.PageNumber > 1)
            {
                return RedirectToRoute("CategoryItem", new { SeName = SeName, itemtype = itemtype });
            }
            int lvl = 1;
            var buyingrequestModel = products.Select(x => PrepareBuyingRequestCatalogModel(x, _workContext.WorkingLanguage.Id));
            var model = new BuyingRequestCatalogListModel();
            model.PagingContext = command;
            var list = new PagedList<BuyingRequestCatalogModel>(buyingrequestModel.ToList(), command.PageIndex, command.PageSize, products.TotalCount);
            model.PagingContext.LoadPagedList(list);
            model.BuyingRequestList = list;
            if (command.CategoryId == 0)
            {
                model.Categories = new CategoryNavigationModel[1];
                model.Categories[0] = new CategoryNavigationModel();
                model.Categories[0].Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(0,ref lvl);
            }
            else
            {
                model.Categories = PrepareCategoryNavigationArray(command.CategoryId);
            }
            model.ItemType = producttype;
            model.SelectedCategorySeName = SeName;
            return View(model);
        }

        public ActionResult Details(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                throw new HttpException(404, "Not found");

            if (!product.Published)
                return RedirectToRoute("HomePage");

            if (_workContext.CurrentMiniSite != null)
            {
                throw new HttpException(404, "Not found");
            }
            //ACL (access control list)
            //if (!_aclService.Authorize(product))
            //    return RedirectToRoute("HomePage");

            //prepare the model
            var model = PrepareProductDetailsPageModel(product);

            //if (model.ProductAttributes.Count == 0)
            //    return RedirectToRoute("HomePage");

            //save as recently viewed
            _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewProduct", _localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"),_workContext.CurrentCustomer,Request.UrlReferrer== null ? "" : Request.UrlReferrer.ToString(), product.Id , product.Name);

            model.Request = new RequestOverviewModel();
            return View(model);
        }

        public ActionResult Brand(int brandId)
        {
            //var product = _productService.GetProductById(productId);
            //if (product == null || product.Deleted)
            //    return RedirectToRoute("HomePage");

            //if (!product.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
            //    return RedirectToRoute("HomePage");

            ////ACL (access control list)
            ////if (!_aclService.Authorize(product))
            ////    return RedirectToRoute("HomePage");

            ////prepare the model
            //var model = PrepareProductDetailsPageModel(product);

            //if (model.ProductAttributes.Count == 0)
            //    return RedirectToRoute("HomePage");

            ////save as recently viewed
            //_recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);

            ////activity log
            //_customerActivityService.InsertActivity("PublicStore.ViewProduct", _localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name);

            //model.Request = new RequestOverviewModel();
            //return View(model);
            throw new Exception("Implement me please!");
        }


        [HttpPost]
        public ActionResult AddRequest(ProductDetailsModel model)
        {
            var product = _productService.GetProductById(model.Id);
            if (!ModelState.IsValid)
            {
                var model1 = PrepareProductDetailsPageModel(product);
                model1.Request = model.Request;
                return View("Details", model1);
            }

            var request = new Request();
            request.ProductId = product.Id;
            request.ProposeComment = model.Request.RequestComment;
            request.Quantity = model.Request.Quantity;
            request.CreatedOnUtc = DateTime.UtcNow;
            request.CustomerId = _workContext.CurrentCustomer.Id;
            request.IsNew = true;
            _requestService.InsertRequest(request);

            _requestEmailSender.SendNewRequestEmail(request.Id, _workContext.WorkingLanguage.Id);

            return RedirectToRoute("CategoryItem", new { SeName = product.ProductCategories.First().Category.GetSeName(_workContext.WorkingLanguage.Id) });
        }

        /// <summary>
        /// Display catalog tab on homepage
        /// </summary>
        /// <returns></returns>
        public ActionResult CatalogTab()
        {
            return View();
        }

        /// <summary>
        /// Display product/service/product buying request/serrapivice buyinng request tabs in catalog tab on homepage
        /// </summary>
        /// <param name="productItemType">ProductItemTypeEnum product/service/productbuyingrequest/servicebuyingrequest </param>
        /// <returns></returns>
        public ActionResult ProductCatalogTab(int productItemType)
        {      
            string cacheKey = String.Format(ModelCacheEventConsumer.HOMEPAGE_CATEGORIES_MODEL_KEY, _workContext.WorkingLanguage.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                int lvl = 2;
                var categories = PrepareCategoryNavigationModel(0, ref lvl).ToList();
                categories = categories.ToList();
                return categories.Select(x =>
                {
                    x.SubCategories = x.SubCategories.Take(6).ToList();
                    return x;
                }).ToList();
            });
            var cats = (List<CategoryNavigationModel.CategoryModel>)cachedModel;
            var model = new CategoryNavigationModel()
            {
                ItemType = ProductItemTypeEnum.Product,
                Categories = cats,
            };
            model.ItemType = (ProductItemTypeEnum)productItemType;
            return View(model);
        }

        /// <summary>
        /// Display seller/service provider catalogue
        /// </summary>
        /// <param name="SeName">category sename to filter selers or service providers</param>
        /// <param name="itemtype">iitem type: Product/Service</param>
        /// <param name="command">paging command</param>
        /// <returns></returns>
        public ActionResult SellerCatalogue(string SeName, string itemtype, SellerListPagableModel command)
        {
            command.PageSize = _catalogSettings.SellerCatalogPageSize;
            
            if (SeName == "all")
            {
                command.CategoryId = 0;
            }
            else
            {
                var slug = _urlRecordService.GetBySlug(SeName);
                if (slug == null)
                {
                    throw new HttpException(404, "Not found");
                }
                command.CategoryId = slug.EntityId;
            }
            ProductItemTypeEnum itemtypevlue;
            int producttype;
            if (!Enum.TryParse(itemtype, true, out itemtypevlue))
            {
                return RedirectToRoute("HomePage");
            }
            producttype = (int)itemtypevlue;
            //var items = _productService.GetAllProducts()
            //    .Where(x => x.Published && !x.Deleted)
            //    .Where(x => x.ProductItemTypeId == producttype).ToList();
            var items = _productService.NewSearchProducts(0, producttype, 0, ProductSortingEnum.CreatedOn, 0, 2147483644, 0, command.CategoryId, 0);
            //if (command.CategoryId != 0)
            //{
            //    items = FilterProductsByCategoryId(items, command.CategoryId);
            //}

            var products = items.GroupBy(x => x.CustomerId).ToList();

            var model = new SellerListModel();
            model.SelectedCategoryId = command.CategoryId;
            var sellersids = products.Select(x => x.Key).ToList();
            var lang = _workContext.WorkingLanguage.Id;
            var sellers = new List<ProfileModel>();

            sellers.AddRange(sellersids.Where(x => _customerService.GetCustomerById(x).CompanyInformation.CompanyName != null)
                .Select(x =>
                {
                    var company = _customerService.GetCustomerById(x).CompanyInformation;
                    var profileModel = new ProfileModel()
                    {
                        CompanyName = company.CompanyName,
                        CompanyDescription = company.CompanyDescription,
                        CompanySeName = company.GetSeName(),
                        Priority = 0,
                        PictureUrl = _pictureService.GetPictureUrl(company.GetAttribute<int>(SystemCustomerAttributeNames.PictureId),showDefaultPicture:false),
                        Id = x
                    };
                    return profileModel;
                }));

            var filteredids = sellersids.Except(sellers.Select(x => x.Id)).ToList();


            var cultures = new OrderedLanguageCultures();
            for (int i = 0; i < cultures.Cultures.Count; i++)
            {
                int priority = 0;
                lang = _languageService.GetAllLanguages().Where(x => x.LanguageCulture == cultures.Cultures[i]).First().Id;
                if (lang == _workContext.WorkingLanguage.Id)
                    continue;
                priority = i + 1;
                sellers.AddRange(filteredids.Where(x => _customerService.GetCustomerById(x).CompanyInformation.CompanyName != null)
                .Select(x =>
                {
                    var company = _customerService.GetCustomerById(x).CompanyInformation;
                    var profileModel = new ProfileModel()
                    {
                        CompanyName = company.CompanyName,
                        CompanyDescription = company.CompanyDescription,
                        CompanySeName = company.GetSeName(),
                        Priority = priority,
                        PictureUrl = _pictureService.GetPictureUrl(company.GetAttribute<int>(SystemCustomerAttributeNames.PictureId), showDefaultPicture: false),
                        Id = x
                    };
                    return profileModel;
                }));

                filteredids = sellersids.Except(sellers.Select(x => x.Id)).ToList();
            }

            model.Sellers = new PagedList<ProfileModel>(sellers.OrderBy(x => x.Priority).ToList(), command.PageNumber - 1, command.PageSize);
            model.PagingContext = new SellerListPagableModel();
            model.PagingContext.LoadPagedList(model.Sellers);
            var category = _categoryService.GetCategoryById(model.SelectedCategoryId);
            if (category != null)
            {
                model.CategoryBreadCrumb = GetCategoryBreadCrumb(category)
                    .Select(x => new CategoryModel()
                    {
                        Name = x.Name,
                        SeName = x.GetSeName()
                    }).ToList();
            }
            model.ItemType = producttype;

            if (command.CategoryId == 0)
            {
                int lvl = 1;
                model.Categories = new CategoryNavigationModel[1];
                model.Categories[0] = new CategoryNavigationModel();
                model.Categories[0].Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(0,ref lvl);
            }
            else
            {
                model.Categories = PrepareCategoryNavigationArray(command.CategoryId);
            }
            model.ItemType = producttype;
            model.SelectedCategorySeName = SeName;

            return View(model);
        }

        /// <summary>
        /// Category selector box
        /// </summary>
        /// <param name="CatalogUrl">Root Url of catalog for example:"Sellers" - seller catalogue,"Catalogue" - product catalogues</param>
        /// <param name="itemtype">ProductItemTypeEnum convertsd to string catalog item type</param>
        /// <returns></returns>
        public PartialViewResult CategorySelector(string CatalogUrl, int itemtype)
        {
            int lvl = 1;
            var model = new CategorySelectorModel();
            model.Categories = new CategoryNavigationModel[1];
            model.Categories[0] = new CategoryNavigationModel();
            model.Categories[0].Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(0,ref lvl);
            model.CatalogUrl = CatalogUrl;
            model.ItemType = itemtype;
            return PartialView(model);
        }

        public PartialViewResult DisplayCategoryAttributes(List<CategoryAttributeModel> categoryAttributesModel)
        {
            return PartialView("_DisplayCategoryAttributes", categoryAttributesModel);
        }
        /// <summary>
        /// Category selector box on manage item page
        /// </summary>
        /// <param name="CatalogUrl">Root Url of catalog for example:"Sellers" - seller catalogue,"Catalogue" - product catalogues</param>
        /// <param name="itemtype">ProductItemTypeEnum convertsd to string catalog item type</param>
        /// <returns></returns>
        public PartialViewResult ManageItemCategorySelector()
        {
            int lvl = 1;
            var model = new CategorySelectorModel();
            model.Categories = new CategoryNavigationModel[1];
            model.Categories[0] = new CategoryNavigationModel();
            model.Categories[0].Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(0,ref lvl);
            return PartialView(model);
        }


        /// <summary>
        /// Category selector box on manage item page
        /// </summary>
        /// <param name="CatalogUrl">Root Url of catalog for example:"Sellers" - seller catalogue,"Catalogue" - product catalogues</param>
        /// <param name="itemtype">ProductItemTypeEnum convertsd to string catalog item type</param>
        /// <returns></returns>
        public PartialViewResult UploadCatalogCategorySelector()
        {
            int lvl = 1;
            var model = new CategorySelectorModel();
            model.Categories = new CategoryNavigationModel[1];
            model.Categories[0] = new CategoryNavigationModel();
            model.Categories[0].Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(0, ref lvl);
            return PartialView(model);
        }


        /// <summary>
        /// Category selector box on product add page
        /// </summary>
        /// <param name="CatalogUrl">Root Url of catalog for example:"Sellers" - seller catalogue,"Catalogue" - product catalogues</param>
        /// <param name="itemtype">ProductItemTypeEnum convertsd to string catalog item type</param>
        /// <returns></returns>
        public PartialViewResult AddProductCategorySelector()
        {
            int lvl = 1;
            var model = new CategorySelectorModel();
            model.Categories = new CategoryNavigationModel[1];
            model.Categories[0] = new CategoryNavigationModel();
            model.Categories[0].Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(0,ref lvl);
            return PartialView(model);
        }

        public PartialViewResult EditProductCategorySelector(CategoryNavigationModel[] productCategories)
        {
            var model = new CategorySelectorModel();
            model.Categories = new CategoryNavigationModel[productCategories.Length];
            for (int i = 0; i < productCategories.Length; i++)
            {
                model.Categories[i] = new CategoryNavigationModel();
                model.Categories[i] = productCategories[i];
            }
            return PartialView(model);
        }
        
        /// <summary>
        /// Display seller/service provider tabs in catalog tab on homepage
        /// </summary>
        /// <param name="productItemType">ProductItemTypeEnum product/service </param>
        /// <returns></returns>
        public ActionResult SellerCatalogTab(int productItemType)
        {
            string cacheKey = String.Format(ModelCacheEventConsumer.HOMEPAGE_CATEGORIES_MODEL_KEY, _workContext.WorkingLanguage.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                int lvl = 2;
                var categories = PrepareCategoryNavigationModel(0, ref lvl).Where(x => x.SubCategories.Count > 0).ToList();
                categories = categories.ToList();
                return categories.Select(x =>
                {
                    x.SubCategories = x.SubCategories.Take(6).ToList();
                    return x;
                }).ToList();
            });
            var cats = (List<CategoryNavigationModel.CategoryModel>)cachedModel;

            
            var model = new CategoryNavigationModel()
            {
                ItemType = ProductItemTypeEnum.Product,
                Categories = cats,
            };
            model.ItemType = (ProductItemTypeEnum)productItemType;
            return View(model);
        }

        /// <summary>
        /// Display recently added items feeds
        /// </summary>
        /// <param name="itemtype">item type</param>
        /// <returns></returns>
        public ActionResult RecentlyAddedItemsFeed(string itemtype)
        {
            var feed = new SyndicationFeed();
            
            var lang = _workContext.WorkingLanguage.Id;
            var items = new List<SyndicationItem>();
            var products = _productService.GetAllProducts()
                .Where(x => !x.Deleted)
                .Where(x => x.Name != null)
                .OrderByDescending(x => x.CreatedOnUtc)
                .Take(_settingService.GetSettingByKey<int>("Rss.Product.Count"));
            var productModels = products.Select(x =>
            {
                var model = PrepareBuyingRequestCatalogModel(x, lang);
                if (x.ProductPictures.Count > 0)
                {
                    var produtPicture = x.ProductPictures.Where(p => p.DisplayOrder == 0).FirstOrDefault();
                    if (produtPicture != null)
                    {
                        model.Picture = new PictureModel()
                        {
                            ImageUrl = _pictureService.GetPictureUrl(produtPicture.PictureId,targetSize:200, showDefaultPicture: false),
                        };
                    }
                    else
                    {
                        model.Picture = new PictureModel()
                        {
                            ImageUrl = _pictureService.GetPictureUrl(x.ProductPictures.First().PictureId, targetSize: 200, showDefaultPicture: false)
                        };
                    }
                }

                return model;
            });
            foreach (var product in productModels)
            {
                string productUrl = Url.RouteUrl("Product", new { SeName = product.ProductSeName }, "http");
                string content = "";
                if (product.Picture != null && product.Picture.ImageUrl != null && product.Picture.ImageUrl != "")
                    content += "<img src=\"" + product.Picture.ImageUrl + "\" /><br>";
                if (product.Brand != null)
                {
                    content += product.Brand + " by ";
                }
                content += "<a href=\"" + Url.RouteUrl("CompanyInformation", new { SeName = product.CompanySeName }) + "\">" + product.CompanyTitle + "</a><br>";
                content += product.ShortDescription;
                items.Add(new SyndicationItem(product.Title, content, new Uri(productUrl), String.Format("RecentlyAddedProduct:{0}", product.Id), product.CreatedOnUtc));
            }
            feed.Items = items;
            return new RssActionResult() { Feed = feed };
        }

        /// <summary>
        /// Display recently added sellers feed
        /// </summary>
        /// <param name="itemtype">catalog type</param>
        /// <returns></returns>
        public ActionResult RecentlyAddedSellersFeed(string itemtype)
        {
            var feed = new SyndicationFeed();
            ProductItemTypeEnum itemtypevlue;
            int producttype;
            if (!Enum.TryParse(itemtype, true, out itemtypevlue))
            {
                return RedirectToRoute("HomePage");
            }
            producttype = (int)itemtypevlue;

            switch (producttype)
            {
                case (int)ProductItemTypeEnum.Product:
                    {
                        feed.Title = new TextSyndicationContent(string.Format("{0}: {1}", _storeInformationSettings.StoreName, _localizationService.GetResource("ETF.Feed.Seller.Title")));
                        feed.Description = new TextSyndicationContent(_localizationService.GetResource("ETF.Feed.Seller.Description"));
                        feed.Id = "RecentlyAddedSellersRss";
                        break;
                    }
                case (int)ProductItemTypeEnum.Service:
                    {
                        feed.Title = new TextSyndicationContent(string.Format("{0}: {1}", _storeInformationSettings.StoreName, _localizationService.GetResource("ETF.Feed.ServiceProvider.Title")));
                        feed.Description = new TextSyndicationContent(_localizationService.GetResource("ETF.Feed.ServiceProvider.Description"));
                        feed.Id = "RecentlyAddedServicesProvidersRss";
                        break;
                    }
            }
            var productitems = _productService.GetAllProducts()
                .Where(x => x.Published && !x.Deleted).ToList();

            var products = productitems.GroupBy(x => x.CustomerId).ToList();

            var model = new SellerListModel();
            var sellersids = products.Select(x => x.Key).ToList();
            sellersids = sellersids.OrderByDescending(x => _customerService.GetCustomerById(x).CreatedOnUtc).ToList();
            var lang = _workContext.WorkingLanguage.Id;
            var items = new List<SyndicationItem>();
            var sellers = new List<ProfileModel>();
            sellers.AddRange(sellersids.Where(x => _customerService.GetCustomerById(x).CompanyInformation.CompanyName != null)
                .Select(x =>
                {
                    var company = _customerService.GetCustomerById(x).CompanyInformation;
                    var profileModel = new ProfileModel()
                    {
                        CompanyName = company.CompanyName,
                        CompanyDescription = company.CompanyDescription,
                        CompanySeName = company.GetSeName(),
                        Priority = 0,
                        PictureUrl = _pictureService.GetPictureUrl(company.GetAttribute<int>(SystemCustomerAttributeNames.PictureId),targetSize:200, showDefaultPicture: false),
                        Id = x,
                        CreatedOnUtc = _dateTimeHelper.ConvertToUserTime(_customerService.GetCustomerById(x).CreatedOnUtc, TimeZoneInfo.Utc, _dateTimeHelper.CurrentTimeZone)
                    };
                    return profileModel;
                }).OrderByDescending(x=>x.CreatedOnUtc));

            var filteredids = sellersids.Except(sellers.Select(x => x.Id)).ToList();


            var cultures = new OrderedLanguageCultures();
            for (int i = 0; i < cultures.Cultures.Count; i++)
            {
                int priority = 0;
                lang = _languageService.GetAllLanguages().Where(x => x.LanguageCulture == cultures.Cultures[i]).First().Id;
                if (lang == _workContext.WorkingLanguage.Id)
                    continue;
                priority = i + 1;
                sellers.AddRange(filteredids.Where(x => _customerService.GetCustomerById(x).CompanyInformation.CompanyName != null)
                .Select(x =>
                {
                    var company = _customerService.GetCustomerById(x).CompanyInformation;
                    var profileModel = new ProfileModel()
                    {
                        CompanyName = company.CompanyName,
                        CompanyDescription = company.CompanyDescription,
                        CompanySeName = company.GetSeName(),
                        Priority = priority,
                        PictureUrl = _pictureService.GetPictureUrl(company.GetAttribute<int>(SystemCustomerAttributeNames.PictureId),targetSize:200, showDefaultPicture: false),
                        Id = x,
                        CreatedOnUtc = _customerService.GetCustomerById(x).CreatedOnUtc
                    };
                    return profileModel;
                }));
                filteredids = sellersids.Except(sellers.Select(x => x.Id)).ToList();
            }
            foreach (var seller in sellers.Take(_settingService.GetSettingByKey<int>("Rss.Seller.Count")))
            {
                string productUrl = Url.RouteUrl("CompanyInformation", new { SeName = seller.CompanySeName }, "http");
                string content = "";
                if (seller.PictureUrl != null && seller.PictureUrl != "")
                    content += "<img src=\"" + seller.PictureUrl + "\" /><br>";
                content += seller.CompanyDescription;
                items.Add(new SyndicationItem(seller.CompanyName, content, new Uri(productUrl), String.Format("RecentlyAddedSellers:{0}", seller.Id), seller.CreatedOnUtc));
            }
            feed.Items = items;
            return new RssActionResult() { Feed = feed };
        }

        /// <summary>
        /// Add item to favorites
        /// </summary>
        /// <param name="productId">item  to add id</param>
        /// <returns></returns>
        public ActionResult AddToFavorites(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("Topic", new { SystemName = "benefits" });
            }
            if (product == null)
            {
                return RedirectToRoute("HomePage");
            }
            var favoritItem = new FavoritItem()
            {
                ProductId = productId,
                CustomerId = _workContext.CurrentCustomer.Id,
                CreatedOnUtc = DateTime.UtcNow
            };


            if(!_favoritsService.IsItemFavorit(_workContext.CurrentCustomer.Id, productId))
                _favoritsService.Insert(favoritItem);
            var seName = product.GetSeName(_workContext.WorkingLanguage.Id);
            return RedirectToRoute("Product", new { seName = seName });
        }

        /// <summary>
        /// Remove item from favorites
        /// </summary>
        /// <param name="productId">item to remove id</param>
        /// <returns></returns>
        public ActionResult RemoveFromFavorites(int productId)
        {
            var product = _productService.GetProductById(productId);
            var seName = product.GetSeName(_workContext.WorkingLanguage.Id);
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("Topic", new { SystemName = "benefits" });
            }
            if (product == null)
            {
                return RedirectToRoute("HomePage");
            }
            var favorit = _favoritsService.GetCustomerFavorits(_workContext.CurrentCustomer.Id)
                .Where(x => x.ProductId == product.Id)
                .FirstOrDefault();
            if (favorit == null)
            {
                return RedirectToRoute("Product", new { seName = seName });
            }

            _favoritsService.DeleteFavorit(favorit);

            return RedirectToRoute("Product", new { seName = seName });
        }

        public ActionResult GetProductBroshure(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
            {
                if (_workContext.CurrentMiniSite != null)
                {
                    return RedirectToRoute("MiniSiteHomePage");
                }
                else
                {
                    return RedirectToRoute("HomePage");
                }
            }
            if (product.Deleted)
            {
                if (_workContext.CurrentMiniSite != null)
                {
                    return RedirectToRoute("MiniSiteHomePage");
                }
                else
                {
                    return RedirectToRoute("HomePage");
                }
            }

            if (_workContext.CurrentMiniSite != null)
            {
                if (_workContext.CurrentMiniSite.Customer.Id != product.CustomerId)
                {
                    throw new HttpException(404, "Not found");
                }
            }
            string path = Server.MapPath("~/Content/Broshure");
            MemoryStream stream = new MemoryStream();
            if (_mobileDeviceHelper.MyIsMobileDevice(HttpContext))
            {
                _productPdfService.GenerateMobileProductPdf(productId, _workContext.WorkingLanguage.Id, stream, path);
            }
            else
            {
                _productPdfService.GeneratePdfItextSharp(productId, _workContext.WorkingLanguage.Id, stream, path);
            }
            byte[] content = stream.ToArray();
            return new FileStreamResult(new MemoryStream(content, 0, content.Length), "application/pdf");
        }

        public ActionResult GetConversionImages(int productId)
        {
            if (productId == 0)
            {
                return View(new List<ConversionImageModel>());
            }
            var product = _productService.GetProductById(productId);
            var category = product.ProductCategories.First();
            var model = GetProductConversionImagesModels(category);

            return View(model);
        }

        private List<ConversionImageModel> GetProductConversionImagesModels(ProductCategory category)
        {
            var groups = _categoryProductAttributeService.GetCategoryToCategoryProductAttributeGroupByCategoryId(category.CategoryId).Select(x => x.CategoryProductAttributeGroup);
            var model = new List<ConversionImageModel>();
            foreach (var group in groups)
            {
                foreach (var conversionImage in group.ConversionImages)
                {
                    string name = conversionImage.GetLocalized(x => x.Name, _workContext.WorkingLanguage.Id, false);
                    int PictureId = conversionImage.GetLocalized(x => x.PictureId, _workContext.WorkingLanguage.Id, false);
                    if (!String.IsNullOrEmpty(name) && PictureId != 0)
                    {
                        model.Add(new ConversionImageModel()
                        {
                            Name = name,
                            PictureUrl = _pictureService.GetPictureUrl(PictureId, showDefaultPicture: false)
                        });
                    }
                }
            }
            return model;
        }

        [HttpPost]
        public ActionResult OrderProduct(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product != null)
            {
                _customerActivityService.InsertActivity("PublicStore.PlaceOrder", _localizationService.GetResource("ITBSFA.Admin.Customer.PlaseOrder"), _workContext.CurrentCustomer, Request.UrlReferrer == null ? "" : Request.UrlReferrer.ToString(), product.Id, product.Name);
            }

            return new JsonResult();
        }
    }
}
