using Nop.Services.Catalog;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Media;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Catalog;
using Nop.Web.Models.Brand;
using Nop.Web.Infrastructure.Cache;
using Nop.Services.Favorits;
using Nop.Core.Caching;
using Nop.Core.Domain.Media;
using Nop.Web.Models.Media;
using Nop.Web.Models.BuyingRequest;
using Nop.Web.Models.CompanyInformation;
using Nop.Services.Directory;
using Nop.Web.Models.Request;
using Nop.Web.Models.SellerCatalog;
using Nop.Services.Helpers;
using System.IO;
using Nop.Web.Controllers;

namespace Nop.Web.Areas.MiniSite.Controllers
{
    public class CatalogueController : BaseNopController
    {
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly IWorkContext _workContext;
        private readonly CatalogSettings _catalogSettings;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IFavoritsService _favoritsService;
        private readonly ICacheManager _cacheManager;
        private readonly MediaSettings _mediaSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IProductPriceService _productPriceService;
        private readonly ICurrencyService _currencyService;
        private readonly ICategoryService _categoryService;
        private readonly IAclService _aclService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ICategoryProductAttributeService _categoryProductAttributeService;

        public CatalogueController(IProductService productService,
            IPictureService pictureService,
            IWorkContext workContext,
            CatalogSettings catalogSettings,
            IProductTemplateService productTemplateService,
            IFavoritsService favoritsService,
            ICacheManager cacheManager,
            MediaSettings mediaSettings,
            ILocalizationService localizationService,
            ILanguageService languageService,
            IProductPriceService productPriceService,
            ICurrencyService currencyService,
            ICategoryService categoryService,
            IAclService aclService,
            IDateTimeHelper dateTimeHelper,
            IUrlRecordService urlRecordService,
            ICategoryProductAttributeService categoryProductAttributeService)
        {
            this._pictureService = pictureService;
            this._productService = productService;
            this._workContext = workContext;
            this._catalogSettings = catalogSettings;
            this._productTemplateService = productTemplateService;
            this._favoritsService = favoritsService;
            this._cacheManager = cacheManager;
            this._mediaSettings = mediaSettings;
            this._localizationService = localizationService;
            this._productPriceService = productPriceService;
            this._currencyService = currencyService;
            this._categoryService = categoryService;
            this._languageService = languageService;
            this._aclService = aclService;
            this._dateTimeHelper = dateTimeHelper;
            this._urlRecordService = urlRecordService;
            this._categoryProductAttributeService = categoryProductAttributeService;
        }

        /// <summary>
        /// Prepare productShortModel
        /// </summary>
        /// <param name="productId">product id</param>
        /// <param name="languageId">language id</param>
        /// <returns></returns>
        [NonAction]
        protected ProductShortModel PrepareProductModel(int productId, int languageId)
        {
            var product = _productService.GetProductById(productId);
            var model = new ProductShortModel()
            {
                ProductName = product.GetLocalized(x => x.Name),
                ProductSeName = product.GetSeName(languageId),
            };

            var pictureId = product.ProductPictures.Where(x => x.DisplayOrder == 0).Select(x => x.PictureId).FirstOrDefault();
            if (pictureId == 0)
            {
                pictureId = product.ProductPictures.Select(x => x.PictureId).FirstOrDefault();
            }


            if (pictureId != 0)
            {
                model.PictureUrl = _pictureService.GetPictureUrl(pictureId, showDefaultPicture: false,targetSize:150);
            }
            return model;

        }

        [NonAction]
        private List<ConversionImageModel> GetProductConversionImagesModels(int categoryId)
        {
            var groups = _categoryProductAttributeService.GetCategoryToCategoryProductAttributeGroupByCategoryId(categoryId).Select(x => x.CategoryProductAttributeGroup);
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
                Name = product.GetLocalized(x => x.Name, _workContext.WorkingLanguage.Id),
                ShortDescription = product.GetLocalized(x => x.ShortDescription, _workContext.WorkingLanguage.Id),
                FullDescription = product.GetLocalized(x => x.FullDescription, _workContext.WorkingLanguage.Id),
                OrderingComments = product.GetLocalized(x => x.AdminComment, _workContext.WorkingLanguage.Id, false),
                MetaKeywords = product.GetLocalized(x => x.MetaKeywords, _workContext.WorkingLanguage.Id),
                MetaDescription = product.GetLocalized(x => x.MetaDescription, _workContext.WorkingLanguage.Id),
                MetaTitle = product.GetLocalized(x => x.MetaTitle, _workContext.WorkingLanguage.Id),
                SeName = product.GetSeName(),
                MinimumOrderQuantity = product.MinimumOrderQuantity.HasValue ? product.MinimumOrderQuantity.Value : 1,
                Favorit = _favoritsService.IsItemFavorit(_workContext.CurrentCustomer.Id, product.Id)

            };

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
            model.DefaultPictureZoomEnabled = _mediaSettings.DefaultPictureZoomEnabled;
            var pictures = _pictureService.GetPicturesByProductId(product.Id);
            if (pictures.Count > 0)
            {
                //default picture
                model.DefaultPictureModel = new PictureModel()
                {
                    ImageUrl = _pictureService.GetPictureUrl(pictures.FirstOrDefault()),
                    FullSizeImageUrl = _pictureService.GetPictureUrl(pictures.FirstOrDefault()),
                    Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.Name),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.Name),
                };
                //all pictures
                int i = 0;
                foreach (var picture in pictures)
                {
                    model.PictureModels.Add(new PictureModel()
                    {
                        ImageUrl = _pictureService.GetPictureUrl(picture),
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.Name),
                        Default = i == 0
                    });
                    i++;
                }
            }
            else
            {
                //no images. set the default one
                model.DefaultPictureModel = new PictureModel()
                {
                    ImageUrl = _pictureService.GetDefaultPictureUrl(_mediaSettings.ProductDetailsPictureSize),
                    FullSizeImageUrl = _pictureService.GetDefaultPictureUrl(),
                    Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.Name),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.Name),
                };
            }

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
                        .Select(x =>
                        {
                            var md = new CategoryProductAttributeValueModel();
                            if (x.CategoryProductAttribute.AttributeControlType != AttributeControlType.TextBox)
                            {
                                md.Name = x.GetLocalized(z => z.Name, _workContext.WorkingLanguage.Id, false);
                            }
                            else
                            {
                                md.Name = x.Name;
                            }
                            md.IsPreSelected = product.ProductAttributes.Where(p => p.Id == x.Id).Count() > 0;
                            md.CategoryProductAttributeId = x.CategoryProductAttributeId;
                            md.Id = x.Id;
                            md.DisplayOrder = x.DisplayOrder;
                            md.ColorSquaresRgb = x.ColorSquaresRgb;
                            md.CategoryProductAttributeId = x.CategoryProductAttributeId;
                            return md;
                        })
                        .ToList());
                    //cam.Values.ForEach(i =>
                    //{
                    //    i.Name = i.GetLocalized(xi => xi.Name, _workContext.WorkingLanguage.Id, true);
                    //});

                    cam.Name = cpa.ProductAttribute.GetLocalized(n => n.Name, _workContext.WorkingLanguage.Id, false);
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
            model.CategoryAttributes = DisplayedAttributes.OrderBy(x => x.DisplayOrder).ToList();

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
            model.CompanyInformationModel = new CompanyInformationModel();
            model.CompanyInformationModel.CompanyName = product.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, _workContext.WorkingLanguage.Id, false);
            model.CompanyInformationModel.CompanySeName = product.Customer.CompanyInformation.GetSeName(_workContext.WorkingLanguage.Id);
            if (model.CompanyInformationModel.CompanyName == null)
            {
                var languages = _languageService.GetAllLanguages().ToList();
                model.CompanyInformationModel.CompanyName = product.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, languages.Where(l => l.LanguageCulture == "es-MX").FirstOrDefault().Id, false);
                model.CompanyInformationModel.CompanyName = product.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, languages.Where(l => l.LanguageCulture == "de-DE").FirstOrDefault().Id, false);
                model.CompanyInformationModel.CompanyName = product.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, languages.Where(l => l.LanguageCulture == "en-US").FirstOrDefault().Id, false);
                model.CompanyInformationModel.CompanyName = product.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, languages.Where(l => l.LanguageCulture == "ru-RU").FirstOrDefault().Id, false);
            }

            var prices = _productPriceService.GetAllProductPrices(model.Id);
            var currencies = _currencyService.GetAllCurrencies().Where(c => c.Published).ToList();
            model.ProductPrices = new List<ProductDetailsModel.ProductPriceModel>();
            var prices_to_delete = prices.Where(p => !currencies.Contains(p.Currency)).ToList();
            prices = prices.Where(p => currencies.Contains(p.Currency)).ToList();
            foreach (var p in prices_to_delete)
                _productPriceService.DeleteProductPriceById(p.Id);
            model.ProductPrices = new List<ProductDetailsModel.ProductPriceModel>();
            foreach (var price in prices)
                model.ProductPrices.Add(new ProductDetailsModel.ProductPriceModel()
                {
                    CurrencyId = price.CurrencyId,
                    Id = price.Id,
                    Price = price.Price,
                    PriceUpdatedOn = price.PriceUpdatedOn,
                    PriceValue = price.Price.ToString("N2"),
                    ProductId = price.ProductId,
                    Currency = new Core.Domain.Directory.Currency()
                    {
                        CurrencyCode = price.Currency.CurrencyCode
                    }

                });

            return model;
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

        /// <summary>
        /// Prepare category navigation model
        /// </summary>
        /// <param name="rootCategoryId">root category</param>
        /// <returns></returns>
        [NonAction]
        protected IList<CategoryNavigationModel.CategoryModel> PrepareCategoryNavigationModel(int rootCategoryId, ref int lavel, IList<int> customerCategories)
        {
            lavel--;
            int newLavel = lavel;
            var result = new List<CategoryNavigationModel.CategoryModel>();
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(rootCategoryId);
            categories = (from p in categories
                         join x in customerCategories on p.Id equals x
                         select p).ToList();
            foreach (var category in categories)
            {
                var categoryModel = new CategoryNavigationModel.CategoryModel()
                {
                    Id = category.Id,
                    Name = category.GetLocalized(x => x.Name),
                    SeName = category.GetSeName(_workContext.WorkingLanguage.Id)
                };
                //subcategories
                if (lavel > 0)
                    categoryModel.SubCategories.AddRange(PrepareCategoryNavigationModel(category.Id, ref newLavel, customerCategories));

                result.Add(categoryModel);
            }

            return result;
        }

        /// <summary>
        /// Prepare array of categories
        /// </summary>
        /// <param name="categoryId">last child category to prepare array from</param>
        /// <param name="model"></param>
        [NonAction]
        protected CategoryNavigationModel[] PrepareCategoryNavigationArray(int categoryId, IList<int> customerCategoreis)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            int lavel = 1;
            while (category.ParentCategoryId != 0)
            {
                lavel++;
                category = _categoryService.GetCategoryById(category.ParentCategoryId);
            }
            var categories = new CategoryNavigationModel[lavel];
            category = _categoryService.GetCategoryById(_categoryService.GetCategoryById(categoryId).ParentCategoryId);
            int prevCategory = categoryId;
            int lvl = 1;
            for (int i = lavel - 1; i > 0; i--)
            {
                categories[i] = new CategoryNavigationModel();
                categories[i].CurrentCategoryId = prevCategory;
                categories[i].Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(category.Id, ref lvl, customerCategoreis);
                if (category.ParentCategoryId != 0)
                {
                    prevCategory = category.Id;
                    category = _categoryService.GetCategoryById(category.ParentCategoryId);
                }
            }
            lvl = 1;
            categories[0] = new CategoryNavigationModel();
            categories[0].CurrentCategoryId = category == null ? categoryId : category.Id;
            categories[0].Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(0, ref lvl, customerCategoreis);

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
            model.Title = entity.GetLocalized(x => x.Name, languageId);
            model.ShortDescription = entity.GetLocalized(x => x.ShortDescription, languageId);
            model.CompanyTitle = entity.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, languageId, false);
            model.CompanySeName = entity.Customer.CompanyInformation.GetSeName(languageId);
            if (model.CompanyTitle == null)
            {
                var languages = _languageService.GetAllLanguages().ToList();
                model.CompanyTitle = entity.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, languages.Where(l => l.LanguageCulture == "es-MX").FirstOrDefault().Id, false);
                model.CompanyTitle = entity.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, languages.Where(l => l.LanguageCulture == "de-DE").FirstOrDefault().Id, false);
                model.CompanyTitle = entity.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, languages.Where(l => l.LanguageCulture == "en-US").FirstOrDefault().Id, false);
                model.CompanyTitle = entity.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, languages.Where(l => l.LanguageCulture == "ru-RU").FirstOrDefault().Id, false);
            }

            if (entity.ProductPictures.Count > 0)
            {
                var produtPicture = entity.ProductPictures.Where(x => x.DisplayOrder == 0).FirstOrDefault();
                if (produtPicture != null)
                {
                    model.Picture = new PictureModel()
                    {
                        ImageUrl = _pictureService.GetPictureUrl(produtPicture.PictureId, showDefaultPicture: false,targetSize:220),
                    };
                }
                else
                {
                    model.Picture = new PictureModel()
                    {
                        ImageUrl = _pictureService.GetPictureUrl(entity.ProductPictures.First().PictureId, showDefaultPicture: false,targetSize:220)
                    };
                }
            }

            return model;
        }

        [NonAction]
        protected IList<int> GetCustomerCategoryIds(bool product, int languageId, int rootCategoryId)
        {
            var productType = (int)(product ? ProductItemTypeEnum.Product : ProductItemTypeEnum.Service);
            var products = _productService.NewSearchProducts(_workContext.CurrentMiniSite.Customer.Id, productType, 0, ProductSortingEnum.CreatedOn, 0, int.MaxValue / 2, 0, rootCategoryId, languageId);
            var categories = products.GroupBy(x => x.ProductCategories.First().CategoryId).Select(x => x.Key);
            var resultCategoriesList = new List<int>();
            foreach (var category in categories)
            {
                var cat = _categoryService.GetCategoryById(category);
                while (cat.ParentCategoryId != 0)
                {
                    resultCategoriesList.Add(cat.Id);
                    cat = _categoryService.GetCategoryById(cat.ParentCategoryId);
                }

                resultCategoriesList.Add(cat.Id);
            }

            return resultCategoriesList.GroupBy(x=>x).Select(x=>x.Key).ToList();
        }

        [NonAction]
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

        /// <summary>
        /// Display recenntly added products block
        /// </summary>
        /// <returns></returns>
        public ActionResult RecentlyAdded()
        {
            var customer = _workContext.CurrentMiniSite.Customer;
            //var products = _productService.GetAllProducts().Where(x => !x.Deleted && x.Published && x.CustomerId == customer.Id && x.ProductItemTypeId == (int)ProductItemTypeEnum.Product)
            //    .Where(x => x.GetLocalized(p => p.Name, _workContext.WorkingLanguage.Id, false) != null)
            //    .OrderByDescending(x => x.CreatedOnUtc)
            //    .Select(x => x.Id);
            var products = _productService.NewSearchProducts(_workContext.CurrentMiniSite.Customer.Id, 0, 0, ProductSortingEnum.CreatedOn, 0, _catalogSettings.RecentlyViewedProductsNumber, 0, 0, _workContext.WorkingLanguage.Id);

            var model = products.ToList().Select(x => PrepareProductModel(x.Id, _workContext.WorkingLanguage.Id)).Take(_catalogSettings.RecentlyViewedProductsNumber).ToList();
            return View(model);
        }

        public ActionResult CatalogTabs()
        {
            return View();
        }

        public ActionResult Details(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                return RedirectToRoute("MiniSiteHomePage");

            if (product.CustomerId != _workContext.CurrentMiniSite.Customer.Id)
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
            //_recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);

            //activity log
            //_customerActivityService.InsertActivity("PublicStore.ViewProduct", _localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name);

            //model.Request = new RequestOverviewModel();
            return View(model);
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
                    ProductName = product.GetLocalized(x => x.Name),
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
                                Name = catBr.GetLocalized(x => x.Name),
                                SeName = catBr.GetSeName()
                            });
                        }
                    }
                }
                return model;
            });

            return PartialView("_ProductBreadcrumb", cacheModel);
        }

        public PartialViewResult DisplayCategoryAttributes(List<CategoryAttributeModel> categoryAttributesModel)
        {
            return PartialView("_DisplayCategoryAttributes", categoryAttributesModel);
        }


        /// <summary>
        /// Category selector box
        /// </summary>
        /// <param name="CatalogUrl">Root Url of catalog for example:"Sellers" - seller catalogue,"Catalogue" - product catalogues</param>
        /// <param name="itemtype">ProductItemTypeEnum convertsd to string catalog item type</param>
        /// <returns></returns>
        public PartialViewResult CategorySelector(string CatalogUrl, int itemtype)
        {
            var customerCategories = GetCustomerCategoryIds(itemtype == (int)ProductItemTypeEnum.Product, _workContext.WorkingLanguage.Id, 0);
            int lvl = 1;
            var model = new CategorySelectorModel();
            model.Categories = new CategoryNavigationModel[1];
            model.Categories[0] = new CategoryNavigationModel();
            model.Categories[0].Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(0, ref lvl, customerCategories);
            model.CatalogUrl = CatalogUrl;
            model.ItemType = itemtype;
            return PartialView(model);
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

            command.CustomerId = _workContext.CurrentMiniSite.Customer.Id;
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
                return RedirectToRoute("MiniSiteHomePage");
            }

            var customerCategories = GetCustomerCategoryIds(itemtypevlue == ProductItemTypeEnum.Product, _workContext.WorkingLanguage.Id, command.CategoryId);
            if (!_workContext.CurrentCustomer.IsRegistered() && (itemtypevlue == ProductItemTypeEnum.ServiceBuyingRequest || itemtypevlue == ProductItemTypeEnum.ProductBuyingRequest))
            {
                return RedirectToRoute("MiniSiteHomePage");
            }
            int productTagId = command.ProductTagId;
            producttype = (int)itemtypevlue;
            var products = _productService.NewSearchProducts(_workContext.CurrentMiniSite.Customer.Id, producttype, command.BrandId, ProductSortingEnum.CreatedOn, command.PageIndex, command.PageSize, productTagId, command.CategoryId, _workContext.WorkingLanguage.Id);

            if (products.Count == 0 && command.PageNumber > 1)
            {
                return RedirectToRoute("MiniCategoryItem", new { SeName = SeName, itemtype = itemtype });
            }
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

            if (products.Count == 0 && command.CategoryId != 0)
            {
                throw new HttpException(404, "Not found");
            }
            var buyingrequestModel = products.Select(x => PrepareBuyingRequestCatalogModel(x, _workContext.WorkingLanguage.Id));
            var model = new BuyingRequestCatalogListModel();
            model.PagingContext = command;
            var list = new PagedList<BuyingRequestCatalogModel>(buyingrequestModel.ToList(), command.PageIndex, command.PageSize, products.TotalCount);
            model.PagingContext.LoadPagedList(list);
            model.BuyingRequestList = list;
            if (command.CategoryId == 0)
            {
                int lvl = 1;
                model.Categories = new CategoryNavigationModel[1];
                model.Categories[0] = new CategoryNavigationModel();
                model.Categories[0].Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(0, ref lvl, customerCategories);
            }
            else
            {
                model.Categories = PrepareCategoryNavigationArray(command.CategoryId, customerCategories);
            }
            model.ItemType = producttype;
            model.SelectedCategorySeName = SeName;
            return View(model);
        }


        /// <summary>
        /// Display product/service/product buying request/service buyinng request tabs in catalog tab on homepage
        /// </summary>
        /// <param name="productItemType">ProductItemTypeEnum product/service/productbuyingrequest/servicebuyingrequest </param>
        /// <returns></returns>
        public ActionResult ProductCatalogTab(int productItemType)
        {
            int lvl = 2;
            var customerCategories = GetCustomerCategoryIds(productItemType == (int)ProductItemTypeEnum.Product, _workContext.WorkingLanguage.Id, 0);
            var categories = PrepareCategoryNavigationModel(0, ref lvl, customerCategories).Where(x => x.SubCategories.Count > 0).ToList();
            categories = categories.Take(2).ToList();
            categories = categories.Select(x =>
            {
                x.SubCategories = x.SubCategories.Take(5).ToList();
                return x;
            }).ToList();
            var model = new CategoryNavigationModel()
            {
                ItemType = ProductItemTypeEnum.Product,
                Categories = categories,
            };
            model.ItemType = (ProductItemTypeEnum)productItemType;
            return View(model);
        }

        /// <summary>
        /// Display conversion images of product category
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Get subcategories by specified category id
        /// </summary>
        /// <param name="categoryId">category id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSubCategories(int categoryId, bool product)
        {
            var customerCategories = GetCustomerCategoryIds(product, _workContext.WorkingLanguage.Id, categoryId);
            var model = new CategoryNavigationModel();
            bool subCategories = false;
            int lvl = 1;
            model.Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(categoryId, ref lvl, customerCategories);
            if (model.Categories.Count > 0)
                subCategories = true;

            var category = _categoryService.GetCategoryById(categoryId);
            int lavel = 1;
            while (category.ParentCategoryId != 0)
            {
                lavel++;
                category = _categoryService.GetCategoryById(category.ParentCategoryId);
            }

            model.CurrentCategoryId = lavel;
            string html;
            html = RenderPartialViewToString("_categoryEditor", model);
            return Json(new
            {
                haveConversionImages = GetProductConversionImagesModels(categoryId).Count > 0,
                lavel = lavel,
                id = categoryId,
                subCategories = subCategories,
                htmlString = html,
                seName = _categoryService.GetCategoryById(categoryId).GetSeName(_workContext.WorkingLanguage.Id)
            });
        }
    }
}
