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
using Nop.Web.Models.Media;
using Nop.Web.Models.Catalog;
using Nop.Web.Models;
using Nop.Core.Domain.Catalog;
using System.IO;
using Nop.Web.Models.Brand;
using Nop.Web.Models.BuyingRequest;
using Nop.Core.Domain.BrandDomain;
using Nop.Web.Models.Request;
using Nop.Services;
using Nop.Services.RequestServices;
using Nop.Core.Domain;
using Nop.Web.Framework.Mvc;
using Nop.Services.ExportImport;
using Nop.Web.Models.SellerCatalog;

namespace Nop.Web.Controllers
{
    public partial class BuyingRequestController : BaseNopController
    {
        private readonly IWorkContext _workContext;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IPictureService _pictureService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly ICategoryProductAttributeService _categoryProductAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IRequestService _requestService;
        private readonly IRequestEmailSender _requestEmailSender;
        private readonly CatalogSettings _catalogSettings;
        private readonly ICatalogExportManager _catalogExportManager;
        private readonly IProductPriceService _productPriceService;
        private readonly ICurrencyService _currencyService;
        private readonly IDownloadService _downloadService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductCategoryService _productCategoryService;

        public BuyingRequestController(IWorkContext workContext,
            IProductService productService,
            IProductTagService productTagService,
            IPictureService pictureService,
            ICategoryService categoryService,
            IBrandService brandService,
            ICategoryTemplateService categoryTemplateService,
            ICategoryProductAttributeService categoryProductAttributeService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IUrlRecordService urlRecordService,
            IRequestService requestService,
            IRequestEmailSender requestEmailSender,
            CatalogSettings catalogSettings,
            IProductPriceService productPriceService,
            ICurrencyService currencyService,
            ICatalogExportManager catalogExportManager,
            IDownloadService downloadService,
            ILocalizationService localizationService,
            IProductCategoryService productCategoryService)
        {
            this._workContext = workContext;
            this._productService = productService;
            this._productTagService = productTagService;

            this._pictureService = pictureService;
            this._categoryService = categoryService;
            this._brandService = brandService;
            this._categoryTemplateService = categoryTemplateService;
            this._categoryProductAttributeService = categoryProductAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._urlRecordService = urlRecordService;
            this._requestService = requestService;
            this._requestEmailSender = requestEmailSender;
            this._catalogSettings = catalogSettings;
            this._productPriceService = productPriceService;
            this._currencyService = currencyService;
            this._catalogExportManager = catalogExportManager;
            this._downloadService = downloadService;
            this._localizationService = localizationService;
            this._productCategoryService = productCategoryService;
        }

        #region Utilites

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
        /// Prepare request overview model for specified language
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="languageId">language id</param>
        /// <returns></returns>
        [NonAction]
        protected RequestOverviewModel PrepareRequestModel(Request request, int languageId,bool buyingrequest)
        {
            var model = new RequestOverviewModel();
            var languages = new OrderedLanguageCultures();
            model.ProductTitle = request.Product.Name;
            //process product language specific information
            #region
            if (model.ProductTitle != null)
            {
                model.ProductDescription = request.Product.ShortDescription;
                model.ProductSeName = request.Product.GetSeName(languageId);
            }
            else
            {
                for (int i = 0; i < languages.Cultures.Count; i++)
                {
                    var langid = _languageService.GetAllLanguages().Where(x => x.LanguageCulture == languages.Cultures[i]).FirstOrDefault().Id;
                    model.ProductTitle = request.Product.Name;
                    if (model.ProductTitle != null)
                    {
                        model.ProductDescription = request.Product.ShortDescription;
                        model.ProductSeName = request.Product.GetSeName(langid);
                        break;
                    }
                }
            }
            #endregion

            //process offerer company information
            #region
            var company = buyingrequest ? request.Customer.CompanyInformation : request.Product.Customer.CompanyInformation;
            model.CompanyName = company.AccountNumbers;
            if (model.CompanyName != null)
            {
                model.CompanySeName = company.GetSeName();
            }
            else
            {
                for (int i = 0; i < languages.Cultures.Count; i++)
                {
                    var langid = _languageService.GetAllLanguages().Where(x => x.LanguageCulture == languages.Cultures[i]).FirstOrDefault().Id;
                    model.CompanyName = company.CompanyName;
                    if (model.CompanyName != null)
                    {
                        model.CompanySeName = company.GetSeName();
                        break;
                    }
                }
            }

            #endregion

            model.ProductId = request.ProductId;
            model.RequestComment = request.ProposeComment;
            model.RequestAnswer = request.ResponseComment;
            model.RequestDate = request.CreatedOnUtc;
            model.Id = request.Id;
            model.Status = request.Accepted;
            model.IsNew = request.IsNew;
            return model;
        }

        [NonAction]
        protected IList<CategoryNavigationModel.CategoryModel> PrepareCategoryNavigationModel(int rootCategoryId,ref int lavel)
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
                if (lavel > 0)
                    categoryModel.SubCategories.AddRange(PrepareCategoryNavigationModel(category.Id, ref newLavel));

                result.Add(categoryModel);
            }

            return result;
        }

        /// <summary>
        /// Prepare buying request picture list model
        /// </summary>
        /// <param name="productId">id of buying request</param>
        /// <returns></returns>
        [NonAction]
        protected IList<PictureModel> PreparePictureListMode(int productId)
        {
            var pictures = _productService.GetProductById(productId).ProductPictures
                .OrderBy(x => x.DisplayOrder)
                .Select(x => x.PictureId).ToList();
            var model = new List<PictureModel>();
            model = pictures.Select(x =>
            {
                return new PictureModel()
                {
                    Id = x,
                    ImageUrl = _pictureService.GetPictureUrl(x, 100),
                    FullSizeImageUrl = _pictureService.GetDefaultPictureUrl(x),
                    Default = _pictureService.GetPictureById(x).ProductPictures.Where(y => y.ProductId == productId).FirstOrDefault().DisplayOrder == 0
                };
            }).ToList();

            return model;
        }


        /// <summary>
        /// Check if all required buying reuest attributes filled in
        /// </summary>
        /// <param name="attributes">filled attributes</param>
        /// <param name="categoryId">id of categoryto get attributes</param>
        /// <returns></returns>
        [NonAction]
        protected bool CheckAttributes(List<CategoryAttributeValueModel> attributes, int categoryId)
        {
            bool globalRes = true;

            var categoryAttributeGroup = _categoryProductAttributeService.GetCategoryToCategoryProductAttributeGroupByCategoryId(categoryId);
            if (categoryAttributeGroup.Count == 0)
            {
                return true;
            }

            var categoryAttributes = new List<CategoryProductAttribute>();
            foreach (var group in categoryAttributeGroup)
            {
                categoryAttributes.AddRange(group.CategoryProductAttributeGroup.CategoryProductAttributes);
            }
            foreach (var attribute in categoryAttributes)
            {
                var attr_s = attributes.Where(x => x.CategoryAttributeId == attribute.Id).ToList();
                if (attr_s == null)
                    continue;
                if (attribute.IsRequired)
                {
                    bool res = false;
                    foreach (var attr in attr_s)
                    {
                        if (!(attr.Id == 0 && (attr.Value == null || attr.Value == "" || attr.Value == "0")))
                        {
                            res = true;
                        }
                    }
                    if (res)
                    {
                        continue;
                    }
                    else
                    {
                        var first_attr = attr_s.FirstOrDefault();
                        if (first_attr != null)
                        {
                            first_attr.Error = true;
                        }
                        //attr_s.First().Error = true;
                        globalRes = false;
                    }
                }
            }

            return globalRes;
        }

        /// <summary>
        /// Update buying request information
        /// </summary>
        /// <param name="product">buying request to update</param>
        /// <param name="model"></param>
        [NonAction]
        protected void UpdateProduct(Product product, BuyingRequestModel model)
        {
            product.Name = model.Title;
            product.ShortDescription = model.ShortDescription;
            product.FullDescription = model.FullDescription;
            product.CustomerId = _workContext.CurrentCustomer.Id;
            //product.ProductCategories.Clear();
            product.AdminComment = model.OrderingComments;
            product.Published = true;
            _productService.UpdateProduct(product);
            //if (product.ProductCategories.First().Id != model.SelectedCategoryId)
            //{
                var cats = product.ProductCategories.ToList();
                foreach (var categ in cats)
                {
                    _productCategoryService.Delete(categ);
                }

                product.ProductCategories.Add(new ProductCategory()
                {
                    CategoryId = model.SelectedCategoryId,
                    ProductId = product.Id
                });
                _productService.UpdateProduct(product);
            //}

                var tagsToRemove = new List<ProductTag>();
                if (!String.IsNullOrEmpty(model.Keywords))
                {
                    var tags = model.Keywords.Split(',');
                    var languages = new List<int>();
                    if (model.AviableLanguages != null && model.AviableLanguages.Count > 0)
                    {
                        languages = model.AviableLanguages.Where(x => x.Selected).Select(x => x.LanguageId).ToList();
                    }
                    else
                    {
                        languages.Add(model.WorkingLanguage);
                    }
                    foreach (var langId in languages)
                    {
                        tagsToRemove.AddRange(product.ProductTags);
                        foreach (var tag in tags)
                        {
                            if (tag == "")
                                continue;
                            var tg = new ProductTag();
                            tag.Trim();
                            var tagOld = product.ProductTags.FirstOrDefault();
                            if (tagOld != null)
                            {
                                tagsToRemove.Remove(tagOld);
                            }
                            var productTag = _productTagService.GetProductTagByName(tag);
                            if (productTag == null)
                            {
                                tg.Name = tag;
                                tg.ProductCount = 1;
                                _productTagService.InsertProductTag(tg);
                            }
                            else
                            {
                                tg = productTag;
                                tg.ProductCount++;
                                _productTagService.UpdateProductTag(tg);
                            }
                            product.ProductTags.Add(tg);
                        }
                    }
                }
                else
                {
                    tagsToRemove.AddRange(product.ProductTags);
                }

                foreach (var tgOld in tagsToRemove)
                {
                    product.ProductTags.Remove(tgOld);
                    _productService.UpdateProduct(product);
                    _productTagService.UpdateProductTagTotals(tgOld);
                }
        }
        
        [NonAction]
        protected void UpdatePrices(Product product, List<ProductDetailsModel.ProductPriceModel> ProductPrices)
        {
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

        /// <summary>
        /// Update buying request locales
        /// </summary>
        /// <param name="product">buying request to update</param>
        /// <param name="languages">buying request languages</param>
        [NonAction]
        protected void UpdateLocales(Product product, IEnumerable<int> languages)
        {
            foreach (var langId in languages)
            {
                _localizedEntityService.SaveLocalizedValue(product, x => x.Name, product.Name, langId);
                _localizedEntityService.SaveLocalizedValue(product, x => x.ShortDescription, product.ShortDescription, langId);
                _localizedEntityService.SaveLocalizedValue(product, x => x.FullDescription, product.FullDescription, langId);
                _localizedEntityService.SaveLocalizedValue(product, x => x.AdminComment, product.AdminComment, langId);
                foreach (var tag in product.ProductTags)
                {
                    _localizedEntityService.SaveLocalizedValue(tag, x => x.Name, tag.Name, langId);
                }
                _urlRecordService.ClearEntitySlug(product, langId);
                var seName = product.ValidateSeName(product.Name, null, true);
                _urlRecordService.SaveSlug(product, seName, langId);
            }
        }

        /// <summary>
        /// Prepare buying request overview model
        /// </summary>
        /// <param name="product">buying request to prepare model</param>
        /// <returns></returns>
        [NonAction]
        protected BuyingRequestOverviewModel PrepareBuyingRequestOverviewModel(Product product)
        {
            var model = new BuyingRequestOverviewModel();
            model.Languages = new List<BuyingRequestLanguageModel>();
            foreach (var lang in _languageService.GetAllLanguages())
            {
                if (product.Name != null &&
                    product.ShortDescription != null &&
                    product.FullDescription != null)
                {
                    model.Languages.Add(new BuyingRequestLanguageModel()
                    {
                        LanguageId = lang.Id,
                        LanguageName = lang.Name,
                        FlagImageUrl = lang.FlagImageFileName,
                        Selected = true
                    });
                }
                else
                {
                    model.Languages.Add(new BuyingRequestLanguageModel()
                    {
                        LanguageId = lang.Id,
                        LanguageName = lang.Name,
                        FlagImageUrl = lang.FlagImageFileName,
                        Selected = false
                    });
                }
            }
            //Language processing
            #region LanguageProcessing
            model.ProductTitle = product.Name;
            model.ProductDescription = product.ShortDescription;
            model.ProductSeName = product.GetSeName();
            #endregion
            var category = product.ProductCategories.First().Category;
            model.CategoryString = category.Name;
            while (category.ParentCategoryId != 0)
            {
                category = _categoryService.GetCategoryById(category.ParentCategoryId);

                model.CategoryString = model.CategoryString.Insert(0, category.Name + "->");

            }
            if (product.ProductPictures.Count > 0)
            {
                ProductPicture pp = product.ProductPictures.Where(x => x.DisplayOrder == 0).FirstOrDefault();
                if (pp == null)
                {
                    pp = product.ProductPictures.FirstOrDefault();
                }
                int pictureId = pp != null ? pp.PictureId : 0;
                if (pictureId != 0)
                {
                    model.Picture = new PictureModel()
                        {
                            Id = pictureId,
                            ImageUrl = _pictureService.GetPictureUrl(pictureId),
                        };
                }
            }
            model.ProductId = product.Id;
            return model;
        }

        /// <summary>
        /// Prepare categories array from child categorie to root parent categorie
        /// </summary>
        /// <param name="categorieId">child categorie id</param>
        /// <returns></returns>
        [NonAction]
        protected CategoryNavigationModel[] PrepareCategoriesLinked(int categorieId)
        {

            var category = _categoryService.GetCategoryById(categorieId);
            int lavel = 1;
            while (category.ParentCategoryId != 0)
            {
                lavel++;
                category = _categoryService.GetCategoryById(category.ParentCategoryId);
            }
            var categories = new CategoryNavigationModel[lavel];
            if (lavel != 1)
            {
                category = _categoryService.GetCategoryById(_categoryService.GetCategoryById(categorieId).ParentCategoryId);
            }
            int lvl = 1;
            int lvl1 = 1;
            int prevCategory = categorieId;
            for (int i = lavel - 1; i > 0; i--)
            {
                categories[i] = new CategoryNavigationModel();
                categories[i].CurrentCategoryId = prevCategory;
                categories[i].Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(category.Id,ref lvl);
                if (category.ParentCategoryId != 0)
                {
                    prevCategory = category.Id;
                    category = _categoryService.GetCategoryById(category.ParentCategoryId);
                }
            }
            categories[0] = new CategoryNavigationModel();
            categories[0].CurrentCategoryId = category.Id;
            categories[0].Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(0,ref lvl1);

            return categories;
        }

        /// <summary>
        /// Prepare edit model of selected product attributes values
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [NonAction]
        protected List<CategoryAttributeValueModel> PrepareSelectedAttributesModel(Product product)
        {
            var model = new List<CategoryAttributeValueModel>();
            model = product.ProductAttributes.Select(x =>
                new CategoryAttributeValueModel()
                {
                    CategoryAttributeId = x.CategoryProductAttributeId,
                    Value = x.Name,
                    Id = x.Id
                }).ToList();
            return model;
        }

        /// <summary>
        /// Save entered product attributes values
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="model">model with entered product attributes valuess</param>
        [NonAction]
        protected void SaveAttributes(Product product, List<CategoryAttributeValueModel> model)
        {


            if (product.ProductAttributes.Count == 0)
            {
                foreach (var attribute in model)
                {
                    if (attribute.Id == 0 && attribute.Value != null)
                    {
                        var attributeValue = new CategoryProductAttributeValue()
                        {
                            CategoryProductAttributeId = attribute.CategoryAttributeId,
                            Name = attribute.Value
                        };
                        _categoryProductAttributeService.InsertCategoryProductAttributeValue(attributeValue);
                        product.ProductAttributes.Add(attributeValue);
                    }
                    else
                    {
                        if (attribute.Id > 0)
                        {
                            var attributeValue = _categoryProductAttributeService.GetCategoryProductAttributeValueById(attribute.Id);
                            product.ProductAttributes.Add(attributeValue);
                        }
                    }
                }
            }
            else
            {
                List<CategoryProductAttributeValue> existing_attr = new List<CategoryProductAttributeValue>();
                foreach (var attr in product.ProductAttributes)
                    if (model.Where(m => m.Id == attr.Id).Count() < 1)
                        existing_attr.Add(attr);

                foreach (var ext_attr in existing_attr)
                    product.ProductAttributes.Remove(ext_attr);

                foreach (var attribute in model)
                {
                    if (attribute.Id == 0 && attribute.Value != null)
                    {
                        var attributeValue = product.ProductAttributes.Where(x => x.CategoryProductAttributeId == attribute.CategoryAttributeId).FirstOrDefault();
                        if (attributeValue != null)
                        {
                            attributeValue.Name = attribute.Value;
                            _categoryProductAttributeService.UpdateCategoryProductAttributeValue(attributeValue);
                        }
                        else
                        {
                            attributeValue = new CategoryProductAttributeValue()
                            {
                                CategoryProductAttributeId = attribute.CategoryAttributeId,
                                Name = attribute.Value
                            };
                            _categoryProductAttributeService.InsertCategoryProductAttributeValue(attributeValue);
                            product.ProductAttributes.Add(attributeValue);
                        }
                    }
                    else
                    {
                        if (attribute.Id > 0)
                        {
                            var attributeValue = product.ProductAttributes.Where(x => x.Id == attribute.Id).FirstOrDefault();
                            if (attributeValue != null)
                            {
                                product.ProductAttributes.Remove(attributeValue);
                                attributeValue = _categoryProductAttributeService.GetCategoryProductAttributeValueById(attribute.Id);
                                product.ProductAttributes.Add(attributeValue);
                            }
                            else
                            {
                                attributeValue = _categoryProductAttributeService.GetCategoryProductAttributeValueById(attribute.Id);
                                product.ProductAttributes.Add(attributeValue);
                            }
                        }
                    }
                }
            }

            _productService.UpdateProduct(product);
        }


        [NonAction]
        protected void UpdateProductTagTotals(Product product)
        {
            var productTags = product.ProductTags.ToList();
            foreach (var productTag in productTags)
                _productTagService.UpdateProductTagTotals(productTag);
        }

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

        #endregion


        /// <summary>
        /// Display add buying request form 
        /// </summary>
        /// <returns></returns>
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Add()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("Login");
            }
            if (!_workContext.CurrentCustomer.IsBuyerOrSeller())
            {
                return RedirectToRoute("HomePage");
            }
            var product = new Product();
            product.Name = "tmp_product";
            product.CreatedOnUtc = DateTime.UtcNow;
            product.UpdatedOnUtc = DateTime.UtcNow;
            product.CustomerId = _workContext.CurrentCustomer.Id;
            _productService.InsertProduct(product);
            var model = new BuyingRequestModel();
            model.ProductPrices = new List<ProductDetailsModel.ProductPriceModel>();
            model.ProductId = product.Id;
            model.AviableTags = _productTagService.GetAllProductTags().Select(x => new ProductTagModel()
                {
                    Id = x.Id,
                    Name = x.Name
                });
            //model.CategoryModel = new CategoryNavigationModel();
            //model.CategoryModel.Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(0);
            model.AviableLanguages = _languageService.GetAllLanguages().Select(x => new BuyingRequestLanguageModel()
            {
                LanguageId = x.Id,
                LanguageName = x.Name,
                FlagImageUrl = x.FlagImageFileName,
                Selected = false
            }).ToList();
            int lvl = 1;
            model.Categories = new CategoryNavigationModel[1];
            model.Categories[0] = new CategoryNavigationModel();
            model.Categories[0].Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(0,ref lvl);
            return View(model);
        }

        /// <summary>
        /// category selector after postback
        /// </summary>
        /// <param name="productCategories"></param>
        /// <returns></returns>
        public ActionResult EditProductCategorySelector(int selectedCategoryId)
        {
            var model = new CategorySelectorModel();
            model.Categories = PrepareCategoriesLinked(selectedCategoryId);
            var category = _categoryService.GetCategoryById(selectedCategoryId);
            model.HaveConversionImages = GetProductConversionImagesModels(category.Id).Count > 0;
            return View(model);
        }

        /// <summary>
        /// Add buying request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Add(BuyingRequestModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered() || !_workContext.CurrentCustomer.IsBuyerOrSeller())
            {
                return RedirectToRoute("Login");
            }

            if (model.AviableLanguages.Where(x => x.Selected).FirstOrDefault() == null)
            {
                ModelState.AddModelError("WorkingLanguage",_localizationService.GetResource("ETF.Product.Language.One"));
            }

            if (CheckAttributes(model.SelectedAttributes, model.SelectedCategoryId))
            {
                if (ModelState.IsValid)
                {
                    var product = _productService.GetProductById(model.ProductId);
                    UpdateProduct(product, model);
                    //UpdateLocales(product, model.AviableLanguages.Where(x => x.Selected).Select(x => x.LanguageId));
                    if(model.SelectedAttributes != null)
                        SaveAttributes(product, model.SelectedAttributes);
                    UpdatePrices(product, model.ProductPrices);
                    UpdateProductTagTotals(product);
                    string seName = product.ValidateSeName(product.Name, "",true);
                    _urlRecordService.SaveSlug(product, seName, 0);
                    return RedirectToAction("Manage");
                }
            }

            //Process selected categories
            if (model.SelectedCategoryId == 0)
            {
                int lvl = 1;
                model.Categories = new CategoryNavigationModel[1];
                model.Categories[0] = new CategoryNavigationModel();
                model.Categories[0].Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(0, ref lvl);
            }
            else
            {
                model.Categories = PrepareCategoriesLinked(model.SelectedCategoryId);
            }

            model.AviableTags = _productTagService.GetAllProductTags().Select(x => new ProductTagModel()
            {
                Id = x.Id,
                Name = x.Name
            });

            model.AviableBrands = _brandService.GetAllBrands()
                .Where(x => x.IsApproved)
                .Select(x => new BrandModel()
                {
                    Name = x.Name,
                    Id = x.Id
                });

            model.AviableLanguages = _languageService.GetAllLanguages().Select(x => new BuyingRequestLanguageModel()
            {
                LanguageId = x.Id,
                LanguageName = x.Name,
                FlagImageUrl = x.FlagImageFileName,
                Selected = false
            }).ToList();
            model.PostBack = true;

            return View(model);
        }

        /// <summary>
        /// Display manage buying requests tab
        /// </summary>
        /// <param name="pageModel">filter values for buying request tab</param>
        /// <returns></returns>
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Manage(BuyingRequestPagableModel pageModel)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }
            //if (_workContext.CurrentCustomer.IsSeller())
            //{
            //    return RedirectToRoute("HomePage");
            //}

            var model = new BuyingRequestListModel();
            model.PagingContext = new BuyingRequestPagableModel();
            pageModel.PageSize = _catalogSettings.ManageItemsPageSize;
            //var products = _productService.GetAllProducts()
            //    .Where(x => x.CustomerId == _workContext.CurrentCustomer.Id)
            //    .Where(x => !x.Deleted);
            var products = _productService.NewSearchProducts(_workContext.CurrentCustomer.Id, pageModel.ProductItemTypeId, pageModel.BrandId, ProductSortingEnum.CreatedOn, pageModel.PageIndex, pageModel.PageSize, 0, pageModel.SelectedCategoryId, 0);
            //if (pageModel.BrandId != 0)
            //{
            //    products = products.Where(x => x.BrandId == pageModel.BrandId);
            //}
            //if (pageModel.ProductItemTypeId != 0)
            //{
            //    products = products.Where(x => x.ProductItemTypeId == pageModel.ProductItemTypeId);
            //}
            //
            //if (pageModel.SelectedCategoryId != 0)
            //{
            //    products = FilterProductsByCategoryId(products.ToList(), pageModel.SelectedCategoryId);
            //}
            var buyingRequests = products
                .Select(x => PrepareBuyingRequestOverviewModel(x));
            IPagedList<BuyingRequestOverviewModel> list = new PagedList<BuyingRequestOverviewModel>(buyingRequests.ToList(), pageModel.PageIndex, pageModel.PageSize, products.TotalCount);

            model.PagingContext.LoadPagedList(list);
            model.BuyingRequestList = list;
            return View(model);
        }

        /// <summary>
        /// Delete Buying request
        /// </summary>
        /// <param name="id">buying request id</param>
        /// <returns></returns>
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Delete(int id)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("Login");
            }
            //if (_workContext.CurrentCustomer.IsSeller())
            //{
            //    return RedirectToRoute("HomePage");
            //}
            var product = _productService.GetProductById(id);
            if (product.CustomerId != _workContext.CurrentCustomer.Id)
                return RedirectToAction("Manage");
            product.Deleted = true;
            _urlRecordService.ClearEntitySlug(product, 0);
            _productService.UpdateProduct(product);
            UpdateProductTagTotals(product);

            return RedirectToAction("Manage");
        }

        /// <summary>
        /// Edit buying request
        /// </summary>
        /// <param name="id">buying request id</param>
        /// <param name="languageid">id of languuage to edit</param>
        /// <returns></returns>
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Edit(int id, int languageid)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("Login");
            }
            if (!_workContext.CurrentCustomer.IsBuyerOrSeller())
            {
                return RedirectToRoute("HomePage");
            }
            var product = _productService.GetProductById(id);

            var model = new BuyingRequestModel();
            model.ProductId = product.Id;
            model.AviableTags = _productTagService.GetAllProductTags().Select(x => new ProductTagModel()
            {
                Id = x.Id,
                Name = x.Name
            });
            model.PictureCount = product.ProductPictures.Count;
            model.ShortDescription = product.ShortDescription;
            model.Title = product.Name;
            model.FullDescription = product.FullDescription;
            model.OrderingComments = product.AdminComment;
            model.WorkingLanguage = languageid;
            foreach (var tag in product.ProductTags)
            {
                model.Keywords += tag.Name + ",";
            }
            model.Categories = PrepareCategoriesLinked(product.ProductCategories.First().CategoryId);

            model.SelectedCategoryId = product.ProductCategories.First().CategoryId;
            model.AviableBrands = _brandService.GetAllBrands()
                .Where(x => x.IsApproved)
                .Select(x => new BrandModel()
                {
                    Name = x.Name,
                    Id = x.Id
                });
            var prices = _productPriceService.GetAllProductPrices(id);
            var currencies = _currencyService.GetAllCurrencies().Where(c => c.Published).ToList();
            model.ProductPrices = new List<ProductDetailsModel.ProductPriceModel>();
            var prices_to_delete = prices.Where(p => !currencies.Contains(p.Currency)).ToList();
            prices = prices.Where(p => currencies.Contains(p.Currency)).ToList();
            foreach (var p in prices_to_delete)
                _productPriceService.DeleteProductPriceById(p.Id);
            foreach (var price in prices)
                model.ProductPrices.Add(new ProductDetailsModel.ProductPriceModel()
                {
                    CurrencyId = price.CurrencyId,
                    Id = price.Id,
                    Price = price.Price,
                    PriceUpdatedOn = price.PriceUpdatedOn,
                    PriceValue = price.Price.ToString("N2"),
                    ProductId = price.ProductId

                });

            model.AviableLanguages = _languageService.GetAllLanguages().Select(x => new BuyingRequestLanguageModel()
            {
                LanguageId = x.Id,
                LanguageName = x.Name,
                FlagImageUrl = x.FlagImageFileName,
                Selected = x.Id == model.WorkingLanguage

            }).ToList();
            List<CategoryProductAttributeGroup> _attrGroups = product.ProductAttributes.Select(x => x.CategoryProductAttribute.CategoryProductGroup).Distinct().ToList();
            List<CategoryAttributeValueModel> DisplayedAttributes = new List<CategoryAttributeValueModel>();
            foreach (var _aG in _attrGroups)
                foreach (var cpa in _aG.CategoryProductAttributes)
                    foreach (var avm in cpa.CategoryProductAttributeValues)
                        DisplayedAttributes.Add(new CategoryAttributeValueModel()
                        {
                            CategoryAttributeId = avm.CategoryProductAttributeId,
                            Id = avm.Id,
                            Value = avm.Name
                        });

            model.HaveConverionsImages = GetProductConversionImagesModels(model.SelectedCategoryId).Count > 0;
            model.SelectedAttributes = DisplayedAttributes;
            return View(model);
        }

        /// <summary>
        /// Edit buying request
        /// </summary>
        /// <param name="model">Model of buying request to edit</param>
        /// <returns></returns>
        [HttpPost]
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Edit(BuyingRequestModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("Login");
            }
            if (!_workContext.CurrentCustomer.IsBuyerOrSeller())
            {
                return RedirectToRoute("HomePage");
            }
            var product = _productService.GetProductById(model.ProductId);
            if (ModelState.IsValid && CheckAttributes(model.SelectedAttributes, model.SelectedCategoryId))
            {
                model.AviableLanguages = null;
                UpdateProduct(product, model);
                UpdateLocales(product, new List<int>()
                {
                    model.WorkingLanguage
                });
                if(model.SelectedAttributes != null)
                    SaveAttributes(product, model.SelectedAttributes);
                UpdatePrices(product, model.ProductPrices);
                UpdateProductTagTotals(product);
                return RedirectToAction("Manage");
            }
            model.AviableTags = _productTagService.GetAllProductTags().Select(x => new ProductTagModel()
            {
                Id = x.Id,
                Name = x.Name
            });
            
            if (model.SelectedCategoryId == 0)
            {
                int lvl = 1;
                model.Categories = new CategoryNavigationModel[1];
                model.Categories[0] = new CategoryNavigationModel();
                model.Categories[0].Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(0,ref lvl);
            }
            else
            {
                model.Categories = PrepareCategoriesLinked(model.SelectedCategoryId);
                model.HaveConverionsImages = GetProductConversionImagesModels(model.SelectedCategoryId).Count > 0;
            }
            //model.SelectedCategoryId = product.ProductCategories.First().CategoryId;
            model.AviableBrands = _brandService.GetAllBrands()
                .Where(x => x.IsApproved)
                .Select(x => new BrandModel()
                {
                    Name = x.Name,
                    Id = x.Id,
                });
            model.PictureCount = product.ProductPictures.Count;
            model.PostBack = true;
            model.AviableLanguages = _languageService.GetAllLanguages().Select(x => new BuyingRequestLanguageModel()
            {
                LanguageId = x.Id,
                LanguageName = x.Name,
                FlagImageUrl = x.FlagImageFileName,
                Selected = x.Id == model.WorkingLanguage

            }).ToList();
            return View(model);
        }

        /// <summary>
        /// Translate buying reauest to specific language
        /// </summary>
        /// <param name="id">Buying request id</param>
        /// <param name="languageid">Language to translate</param>
        /// <returns></returns>
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Translate(int id, int languageid)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("Login");
            }
            if (!_workContext.CurrentCustomer.IsBuyerOrSeller())
            {
                return RedirectToRoute("HomePage");
            }
            return RedirectToAction("Edit", new { id = id, languageid = languageid });
        }

        /// <summary>
        /// Display ist of product pictures
        /// </summary>
        /// <param name="productId">id of product</param>
        /// <returns></returns>
        public PartialViewResult ProductPictureList(int productId)
        {
            var model = PreparePictureListMode(productId);

            return PartialView(model);
        }

        /// <summary>
        /// Add picture to product pictures
        /// </summary>
        /// <param name="productId">product id</param>
        /// <param name="pictureId">picture id</param>
        /// <returns></returns>
        [HttpPost]
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult AddProductPicture(int productId, int pictureId)
        {
            var product = _productService.GetProductById(productId);
            var count = product.ProductPictures.Count;
            if (count < 6)
            {
                _productService.InsertProductPicture(new ProductPicture()
                {
                    PictureId = pictureId,
                    ProductId = productId,
                    DisplayOrder = 1,
                });
            }
            else
            {
                var picture = _pictureService.GetPictureById(pictureId);
                if (picture != null)
                    _pictureService.DeletePicture(picture);
            }
            var model = PreparePictureListMode(productId);

            var view = RenderPartialViewToString("ProductPictureList", model);
            return Json(new
            {
                success = true,
                picturesHtml = view
            });
        }

        /// <summary>
        /// Delete product picture
        /// </summary>
        /// <param name="pictureId">picture id</param>
        /// <param name="productId">product id</param>
        /// <returns></returns>
        [HttpPost]
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult DeleteProductPicture(int pictureId, int productId)
        {
            var pic = _pictureService.GetPictureById(pictureId);
            var productPicture = _productService.GetProductPicturesByProductId(productId)
                .Where(x => x.PictureId == pictureId).FirstOrDefault();
            _productService.DeleteProductPicture(productPicture);

            var model = PreparePictureListMode(productId);

            var view = RenderPartialViewToString("ProductPictureList", model);
            return Json(new
            {
                success = true,
                picturesHtml = view
            });
        }

        /// <summary>
        /// Set picture as default product picture
        /// </summary>
        /// <param name="pictureId"> picture id</param>
        /// <param name="productId">product id</param>
        /// <returns></returns>
        [HttpPost]
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult SetPictureDefault(int pictureId, int productId)
        {
            var defaultPicture = _productService.GetProductPicturesByProductId(productId)
                .Where(x => x.DisplayOrder == 0).FirstOrDefault();
            if (defaultPicture != null)
            {
                defaultPicture.DisplayOrder = 1;
                _productService.UpdateProductPicture(defaultPicture);
            }
            defaultPicture = _productService.GetProductPicturesByProductId(productId)
                .Where(x => x.PictureId == pictureId).FirstOrDefault();
            defaultPicture.DisplayOrder = 0;
            _productService.UpdateProductPicture(defaultPicture);


            var model = PreparePictureListMode(productId);

            var view = RenderPartialViewToString("ProductPictureList", model);
            return Json(new
            {
                success = true,
                picturesHtml = view
            });
        }

        /// <summary>
        /// Get subcategories by specified category id
        /// </summary>
        /// <param name="categoryId">category id</param>
        /// <returns></returns>
        [HttpPost]
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult GetSubCategories(int categoryId, int IsForAddProduct = 0, int NumberInRow = 0)
        {
            var model = new CategoryNavigationModel();
            bool subCategories = false;
            int lvl = 1;
            model.Categories = (List<CategoryNavigationModel.CategoryModel>)PrepareCategoryNavigationModel(categoryId,ref lvl);
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
            if (IsForAddProduct < 1)
                html = RenderPartialViewToString("_categoryEditor", model);
            else
                html = RenderPartialViewToString("_categoryEditorAddProduct", model);
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

        /// <summary>
        /// Get attributes editor for specified category asinchronously
        /// </summary>
        /// <param name="categoryId">category id</param>
        /// <returns></returns>
        [HttpPost]
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult CategoryAttributesEditor(int categoryId)
        {
            List<CategoryAttributeModel> attributes = new List<CategoryAttributeModel>();
            var categoryAttributeGroup = _categoryProductAttributeService.GetCategoryToCategoryProductAttributeGroupByCategoryId(categoryId);
            if (categoryAttributeGroup.Count == 0)
            {
                return Json(new
                {
                    htmlString = ""
                });
            }
            foreach (var grp in categoryAttributeGroup)
            {
                attributes.AddRange(grp.CategoryProductAttributeGroup.CategoryProductAttributes.Select(x =>
                    {
                        var attr = new CategoryAttributeModel()
                           {
                               ControlType = x.AttributeControlType,
                               Name = x.ProductAttribute.Name,
                               Values = x.CategoryProductAttributeValues.OrderBy(y => y.DisplayOrder)
                               .Select(a =>
                               {
                                   var md = new CategoryProductAttributeValueModel();
                                   md.IsPreSelected = a.IsPreSelected;
                                   md.Name = a.Name;
                                   md.CategoryProductAttributeId = a.CategoryProductAttributeId;
                                   md.Id = a.Id;
                                   md.DisplayOrder = a.DisplayOrder;
                                   md.ColorSquaresRgb = a.ColorSquaresRgb;
                                   md.CategoryProductAttributeId = a.CategoryProductAttributeId;
                                   return md;
                               }).ToList(),
                               Id = x.Id
                           };
                        return attr;
                    }).ToList());
            }
            attributes = attributes.GroupBy(x => x.Name).Select(y => y.First()).ToList();

            string html = RenderPartialViewToString("_CategoryAttributes", attributes);
            return Json(new
            {
                htmlString = html
            });
        }

        public ActionResult ProductPrices(int? productId, IList<Nop.Web.Models.Catalog.ProductDetailsModel.ProductPriceModel> ProductPrices)
        {
            List<Nop.Web.Models.Catalog.ProductDetailsModel.ProductPriceModel> prices = new List<ProductDetailsModel.ProductPriceModel>();
            if (productId.HasValue)
            {
                var items = _productPriceService.GetAllProductPrices(productId.Value);
                foreach (var item in items)
                {
                    decimal correct_price = 0;
                    Decimal.TryParse(ProductPrices.Where(x => x.CurrencyId == item.CurrencyId).First().PriceValue, out correct_price);
                    
                    prices.Add(new ProductDetailsModel.ProductPriceModel()
                    {
                        Currency = item.Currency,
                        CurrencyId = item.CurrencyId,
                        Id = item.Id,
                        Price = correct_price == 0 ? item.Price : correct_price,
                        PriceValue = correct_price == 0 ? item.Price.ToString() : correct_price.ToString(),
                        PriceUpdatedOn = item.PriceUpdatedOn,
                        Product = item.Product,
                        ProductId = item.ProductId
                    });
                }

                var activeCurrencies = _currencyService.GetAllCurrencies(false);
                foreach (var c in activeCurrencies)
                {
                    if (prices.Where(i => i.CurrencyId == c.Id).Count() < 1)
                    {
                        var ppm = new ProductPrice()
                        {
                            CurrencyId = c.Id,
                            Price = 0,
                            PriceUpdatedOn = DateTime.Now,
                            ProductId = productId.Value
                        };
                        _productPriceService.InsertProductPrice(ppm);
                        //ppm = _productPriceService.GetProductPriceById(ppm.Id);
                        prices.Add(new ProductDetailsModel.ProductPriceModel()
                        {
                            Currency = ppm.Currency,
                            CurrencyId = ppm.CurrencyId,
                            Id = ppm.Id,
                            Price = ppm.Price,
                            PriceUpdatedOn = ppm.PriceUpdatedOn,
                            Product = ppm.Product,
                            ProductId = ppm.ProductId,
                            PriceValue = ppm.Price.ToString()
                        });
                    }
                }
            }

            prices = prices.OrderBy(p => p.Currency.CurrencyCode).ToList();
            return View("_productPrices", prices);
        }
        
        /// <summary>
        /// Get attributes of specified category synchronously
        /// </summary>
        /// <param name="categoryId">category id</param>
        /// <returns></returns>
        public ActionResult CategoryAttributes(int categoryId, int? productId, IList<CategoryAttributeValueModel> SelectedAttributes, bool PostBack)
        {

            List<CategoryAttributeModel> attributes = new List<CategoryAttributeModel>();
            var categoryAttributeGroup = _categoryProductAttributeService.GetCategoryToCategoryProductAttributeGroupByCategoryId(categoryId);
            if (categoryAttributeGroup.Count == 0)
            {
                return null;
            }
            foreach (var grp in categoryAttributeGroup)
            {
                attributes.AddRange(grp.CategoryProductAttributeGroup.CategoryProductAttributes.Select(x =>
                    {
                        if (productId.HasValue && !PostBack)
                        {
                            var product = _productService.GetProductById(productId.Value);
                            if (product.ProductAttributes.Count > 0)
                            {
                                var value = product.ProductAttributes.Where(y => y.CategoryProductAttributeId == x.Id).FirstOrDefault();
                                if (value == null)
                                {
                                    var elsemodel2 = new CategoryAttributeModel()
                                    {
                                        ControlType = x.AttributeControlType,
                                        Name = x.ProductAttribute.Name,
                                        Values = x.CategoryProductAttributeValues.OrderBy(y => y.DisplayOrder)
                                        .Select(a =>
                                        {
                                            var md = new CategoryProductAttributeValueModel();
                                            md.IsPreSelected = product.ProductAttributes.Where(pa => pa.Id == a.Id).FirstOrDefault() != null;
                                            md.Name = a.Name;
                                            md.CategoryProductAttributeId = a.CategoryProductAttributeId;
                                            md.Id = a.Id;
                                            md.DisplayOrder = a.DisplayOrder;
                                            md.ColorSquaresRgb = a.ColorSquaresRgb;
                                            md.CategoryProductAttributeId = a.CategoryProductAttributeId;
                                            return md;
                                        }).ToList(),
                                        Id = x.Id,
                                    };
                                    return elsemodel2;
                                }
                                if (value.CategoryProductAttribute.AttributeControlType != AttributeControlType.TextBox)
                                {
                                    value.Name = value.Name;
                                    var model = new CategoryAttributeModel()
                                        {
                                            ControlType = x.AttributeControlType,
                                            Name = x.ProductAttribute.Name,
                                            Values = x.CategoryProductAttributeValues.OrderBy(y => y.DisplayOrder)
                                            .Select(a =>
                                            {
                                                var md = new CategoryProductAttributeValueModel();
                                                md.IsPreSelected = product.ProductAttributes.Where(z => z.Id == a.Id).FirstOrDefault() != null;
                                                md.Name = a.Name;
                                                md.Id = a.Id;
                                                md.DisplayOrder = a.DisplayOrder;
                                                md.ColorSquaresRgb = a.ColorSquaresRgb;
                                                md.CategoryProductAttributeId = a.CategoryProductAttributeId;
                                                return md;
                                            }).ToList(),
                                            Id = x.Id,
                                            SelectedValue = new CategoryProductAttributeValueModel()
                                            {
                                                Name = value.Name,
                                                DisplayOrder = value.DisplayOrder,
                                                ColorSquaresRgb = value.ColorSquaresRgb,
                                                CategoryProductAttributeId = value.CategoryProductAttributeId,
                                                IsPreSelected = value.IsPreSelected,
                                                Id = value.Id
                                            }
                                        };
                                    return model;
                                }
                                else
                                {
                                    var model = new CategoryAttributeModel()
                                    {
                                        ControlType = x.AttributeControlType,
                                        Name = x.ProductAttribute.Name,
                                        Id = x.Id,
                                        SelectedValue = new CategoryProductAttributeValueModel()
                                        {
                                            Name = value.Name,
                                            DisplayOrder = value.DisplayOrder,
                                            ColorSquaresRgb = value.ColorSquaresRgb,
                                            CategoryProductAttributeId = value.CategoryProductAttributeId,
                                            IsPreSelected = value.IsPreSelected,
                                            Id = value.Id
                                        }
                                    };
                                    return model;
                                }
                            }
                        }
                        if (SelectedAttributes != null && SelectedAttributes.Count > 0)
                        {
                            var attrValue = SelectedAttributes.Where(z=>z.CategoryAttributeId == x.Id).FirstOrDefault();
                            var elsemodel1 = new CategoryAttributeModel()
                            {
                                ControlType = x.AttributeControlType,
                                Name = x.ProductAttribute.Name,
                                Values = x.CategoryProductAttributeValues.OrderBy(y => y.DisplayOrder)
                                .Select(a=>{
                                    
                                    var md = new CategoryProductAttributeValueModel();
                                    md.IsPreSelected =SelectedAttributes.Where(z=>z.Id == a.Id).FirstOrDefault() != null;
                                    md.Name = a.Name;
                                    md.Id = a.Id;
                                    md.DisplayOrder = a.DisplayOrder;
                                    md.ColorSquaresRgb = a.ColorSquaresRgb;
                                    md.CategoryProductAttributeId = a.CategoryProductAttributeId;
                                    return md;
                                }).ToList(),
                                Error = attrValue == null ? true : attrValue.Error,
                                SelectedValue = attrValue == null ? null : new CategoryProductAttributeValueModel()
                                {
                                    Id = attrValue.Id,
                                    Name = attrValue.Value,
                                    CategoryProductAttributeId = attrValue.CategoryAttributeId
                                },
                                Id = x.Id,
                            };
                            return elsemodel1;
                        }
                        var elsemodel = new CategoryAttributeModel()
                        {
                            ControlType = x.AttributeControlType,
                            Name = x.ProductAttribute.Name,
                            Values = x.CategoryProductAttributeValues.OrderBy(y => y.DisplayOrder)
                            .Select(a =>
                            {
                                var md = new CategoryProductAttributeValueModel();
                                md.IsPreSelected = a.IsPreSelected;
                                md.Name = a.Name;
                                md.CategoryProductAttributeId = a.CategoryProductAttributeId;
                                md.Id = a.Id;
                                md.DisplayOrder = a.DisplayOrder;
                                md.ColorSquaresRgb = a.ColorSquaresRgb;
                                md.CategoryProductAttributeId = a.CategoryProductAttributeId;
                                return md;
                            }).ToList(),
                            Id = x.Id,
                        };
                        return elsemodel;
                    }).ToList());
            }

            //attributes = attributes.GroupBy(x => x.Name).Select(y => y.First()).ToList();
            //if (SelectedAttributes != null)
            //{
            //    attributes = attributes.Select(x =>
            //    {
            //        var attr = SelectedAttributes.Where(a => a.CategoryAttributeId == x.Id).FirstOrDefault();
            //        if (attr != null)
            //        {
            //            if (attr.Id != 0)
            //            {
            //                var val = _categoryProductAttributeService.GetCategoryProductAttributeValueById(attr.Id);
            //                x.SelectedValue = new CategoryProductAttributeValueModel()
            //                {
            //                    CategoryProductAttributeId = val.CategoryProductAttributeId,
            //                    Id = val.Id,
            //                    DisplayOrder = val.DisplayOrder,
            //                    ColorSquaresRgb = val.ColorSquaresRgb,
            //                    Name = val.GetLocalized(z => z.Name, _workContext.WorkingLanguage.Id, false),
            //                    IsPreSelected = val.IsPreSelected,
            //                };
            //            }
            //            else
            //            {

            //            }
            //        }
            //        x.Error = attr.Error;
            //        return x;
            //    }).ToList();
            //}
            return View("_CategoryAttributes", attributes);
        }

        [HttpPost]
        public ActionResult CheckRequests()
        {
            var customer = _workContext.CurrentCustomer;
            var products = _productService.GetAllProducts()
                .Where(x => x.CustomerId == _workContext.CurrentCustomer.Id && !x.Deleted);
            List<Request> requests = new List<Request>();
            foreach (var product in products)
            {
                requests.AddRange(product.Requests.Where(x => x.Accepted == null && x.IsNew));
            }

            return Json(new { count = requests.Count });
        }

        public string CheckRequestsSync()
        {
            var customer = _workContext.CurrentCustomer;
            var products = _productService.GetAllProducts()
                .Where(x => x.CustomerId == _workContext.CurrentCustomer.Id && !x.Deleted);
            List<Request> requests = new List<Request>();
            foreach (var product in products)
            {
                requests.AddRange(product.Requests.Where(x => x.Accepted == null && x.IsNew));
            }

            return requests.Count.ToString();
        }

        /// <summary>
        /// Display my requests that has no responce yet
        /// </summary>
        /// <param name="command">paging command</param>
        /// <returns></returns>
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult MyRequests(RequestPagableModel command)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }
            command.PageSize = _catalogSettings.ActiveRequestsPageSize;
            var model = new RequestListModel();
            int langId = _workContext.WorkingLanguage.Id;
            model.Requests = new PagedList<RequestOverviewModel>(_requestService.GetCustomerRequests(_workContext.CurrentCustomer.Id)
                .Where(x => !x.Accepted.HasValue && !x.Product.Deleted).Select(x => PrepareRequestModel(x, _workContext.WorkingLanguage.Id,false))
                .OrderByDescending(x => x.RequestDate)
                .ToList(), command.PageIndex, command.PageSize);
            model.PagingContext = new RequestPagableModel();
            model.PagingContext.LoadPagedList(model.Requests);
            model.PagingContext.History = false;
            return View(model);
        }

        /// <summary>
        /// Display completed my requests
        /// </summary>
        /// <param name="command">paging command</param>
        /// <returns></returns>
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult MyRequestsHistory(RequestPagableModel command)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }
            command.PageSize = _catalogSettings.RequestHistoryPageSize;
            var model = new RequestListModel();
            int langId = _workContext.WorkingLanguage.Id;
            model.Requests = new PagedList<RequestOverviewModel>(_requestService.GetCustomerRequests(_workContext.CurrentCustomer.Id)
                .Where(x => x.Accepted.HasValue).Select(x => PrepareRequestModel(x, _workContext.WorkingLanguage.Id,false))
                .OrderByDescending(x => x.RequestDate)
                .ToList(), command.PageIndex, command.PageSize);
            model.PagingContext = new RequestPagableModel();
            model.PagingContext.LoadPagedList(model.Requests);
            model.PagingContext.History = true;
            return View(model);
        }

        /// <summary>
        /// Show my buying requests with ability to accept or decline request
        /// </summary>
        /// <param name="command">paging command</param>
        /// <returns></returns>
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult MyBuyingRequests(RequestPagableModel command)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }
            command.PageSize = _catalogSettings.ActiveRequestsPageSize;
            var model = new RequestListModel();
            int langId = _workContext.WorkingLanguage.Id;
            var products = _productService.GetAllProducts()
                .Where(x => x.CustomerId == _workContext.CurrentCustomer.Id && !x.Deleted);
            List<Request> customerRequests = new List<Request>();
            foreach (var product in products)
            {
                customerRequests.AddRange(product.Requests.Where(x => x.Accepted == null));
            }

            var requests = customerRequests
                .OrderByDescending(x => x.CreatedOnUtc)
                .Select(x => PrepareRequestModel(x, _workContext.WorkingLanguage.Id,true)).ToList();
            customerRequests.Where(x => x.IsNew)
                .ToList()
                .ForEach(x => { x.IsNew = false; _requestService.UpdateRequest(x); });
            model.Requests = new PagedList<RequestOverviewModel>(requests, command.PageIndex, command.PageSize);
            model.PagingContext = new RequestPagableModel();
            model.PagingContext.LoadPagedList(model.Requests);
            return View(model);
        }

        /// <summary>
        /// Show my buying requests with request messages
        /// </summary>
        /// <param name="command">paging command</param>
        /// <returns></returns>
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult MyBuyingRequestsHistory(RequestPagableModel command)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }
            command.PageSize = _catalogSettings.RequestHistoryPageSize;
            var model = new RequestListModel();
            int langId = _workContext.WorkingLanguage.Id;
            var products = _productService.GetAllProducts()
                .Where(x => x.CustomerId == _workContext.CurrentCustomer.Id);
            List<RequestOverviewModel> requests = new List<RequestOverviewModel>();
            foreach (var product in products)
            {
                requests.AddRange(product.Requests.Where(x => x.Accepted != null).Select(x => PrepareRequestModel(x, _workContext.WorkingLanguage.Id,true)));
            }

            model.Requests = new PagedList<RequestOverviewModel>(requests.OrderByDescending(x => x.RequestDate).ToList(), command.PageIndex, command.PageSize);
            model.PagingContext = new RequestPagableModel();
            model.PagingContext.LoadPagedList(model.Requests);
            model.PagingContext.History = true;
            return View(model);
        }

        [HttpPost]
        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult AnswerRequest(int requestId, string requestcomment, bool Status)
        {
            var customer = _workContext.CurrentCustomer;
            if (!customer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }
            var request = _requestService.GetRequestById(requestId);

            if (request.Product.CustomerId != customer.Id)
            {
                return RedirectToRoute("HomePage");
            }

            if (request.Accepted.HasValue)
            {
                return RedirectToAction("MyBuyingRequests");
            }

            request.ResponseComment = requestcomment;
            request.Accepted = Status;
            request.ResponsedOnUtc = DateTime.UtcNow;
            _requestService.UpdateRequest(request);

            _requestEmailSender.SendRequestResponceEmail(request.Id, _workContext.WorkingLanguage.Id);

            return RedirectToAction("MyBuyingRequests");
        }

        public ActionResult UploadCatalog()
        {
            if (!_workContext.CurrentCustomer.IsBuyerOrSeller())
            {
                return RedirectToRoute("HomePage");
            }
            var model = new UploadCatalogModel();
            model.AviableLanguages = _languageService.GetAllLanguages().Select(x => new LanguageModel()
            {
                Name = x.Name,
                Id = x.Id,
                LanguageCulture = x.LanguageCulture,
            }).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult UploadCatalog(UploadCatalogModel model)
        {
            if (!_workContext.CurrentCustomer.IsBuyerOrSeller())
            {
                return RedirectToRoute("HomePage");
            }
            if (model.DownloadId == 0)
            {
                MemoryStream stream = new MemoryStream();
                string path = _catalogExportManager.GenerateExcel(model.SelectedCategoryId, stream,_workContext.WorkingLanguage.Id);
                stream.Position = 0;
                return new FileStreamResult(stream, "application/vnd.ms-excel.sheet.macroEnabled.12");
            }
            else
            {
                var download = _downloadService.GetDownloadById(model.DownloadId);
                if(download == null)
                    return RedirectToAction("Manage");
                var path = Server.MapPath("/Content/Images/uploaded");
                Action<Download,int,int,int,IList<int>,string,int> import = _catalogExportManager.ImportExcelFile;
                import.BeginInvoke(download, _workContext.CurrentCustomer.Id, model.SelectedCategoryId, model.ProductItemType, model.SelectedLanguages.Where(z=>z.Selected).Select(z=>z.Id).ToList(), path, _workContext.WorkingLanguage.Id, null, null);
                //_catalogExportManager.ImportExcelFile(download, _workContext.CurrentCustomer.Id, model.SelectedCategoryId, model.ProductItemType, model.SelectedLanguages, path);
                return RedirectToAction("Manage");
            }
        }


        public ActionResult GetCatalogExcel(int selectedCategoryId)
        {
            if (!_workContext.CurrentCustomer.IsBuyerOrSeller())
            {
                return RedirectToRoute("HomePage");
            }

            MemoryStream stream = new MemoryStream();
            string path = _catalogExportManager.GenerateExcel(selectedCategoryId, stream, _workContext.WorkingLanguage.Id);
            stream.Position = 0;
            return new FileStreamResult(stream, "application/vnd.ms-excel.sheet.macroEnabled.12");
        }

        [HttpPost]
        public ActionResult GetConversionImagesOfCategory(int categoryId)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            var groups = _categoryProductAttributeService.GetCategoryToCategoryProductAttributeGroupByCategoryId(category.Id).Select(x => x.CategoryProductAttributeGroup);
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

            string html = RenderPartialViewToString("GetConversionImages", model);
            return new JsonResult()
            {
                Data = new { html = html }
            };
        }
    }
}
