using Microsoft.Ajax.Utilities;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.CustomerInformationAttributes;
using Nop.Services.Directory;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Models.Common;
using Nop.Web.Models.SearchProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using Nop.Services.Regions;
using Nop.Web.Models.Regions;
using Nop.Web.Models.Catalog;
using Nop.Services.Logging;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;

namespace Nop.Web.Controllers
{
    public class SearchProductController : BaseNopController
    {
        private readonly ICategoryService _categoryService;
        private readonly ICategoryProductAttributeService _categoryProductAttributeService;
        private readonly ICurrencyService _currencyService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerInformationAttributeService _customerInformationAttributeService;
        private readonly ICityService _cityService;
        private readonly IProductService _productService;
        private readonly ICurrencyService _currenncyService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ISearchLogService _searchLogService;
        private readonly ILocalizationService _localizationService;

        public SearchProductController(ICategoryService categoryService,
            ICategoryProductAttributeService categoryProductAttributeService,
            ICurrencyService currencyService,
            IWorkContext workContext, ICustomerInformationAttributeService customerInformationAttributeService,
            ICityService cityService,
            IProductService productService,
            ICurrencyService currenncyService,
            IPictureService pictureService,
            IUrlRecordService urlRecordService,
            ICustomerActivityService customerActivityService,
            ISearchLogService searchLogService,
            ILocalizationService localizationService)
        {
            this._categoryService = categoryService;
            this._categoryProductAttributeService = categoryProductAttributeService;
            this._currencyService = currencyService;
            this._workContext = workContext;
            this._customerInformationAttributeService = customerInformationAttributeService;
            this._cityService = cityService;
            this._productService = productService;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._urlRecordService = urlRecordService;
            this._customerActivityService = customerActivityService;
            this._searchLogService = searchLogService;
            this._localizationService = localizationService;
        }

        [NonAction]
        private SearchProductAttributeModel PrepareProductAttributeModel(CategoryProductAttribute attribute)
        {
            var attributeModel = new SearchProductAttributeModel();
            attributeModel.AttributeTitle = attribute.ProductAttribute.Name;
            attributeModel.Id = attribute.Id;
            attributeModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(attribute.ProductAttribute.PictureId.GetValueOrDefault(), 100, false);
            attributeModel.Description=attribute.ProductAttribute.Description;
            attributeModel.AttributeControlTypeId = attribute.SearchControlTypeId;
            switch (attribute.SearchControlType)
            {
                case SearchAttributeControlType.CheckBox:
                case SearchAttributeControlType.CheckBoxGroup:
                    {
                        var attributeValues = attribute.CategoryProductAttributeValues;
                        if (attributeValues.Count > 0)
                        {
                            attributeModel.Values = new List<SearchProductAttributeValueModel>();
                            foreach (var value in attributeValues.OrderBy(x=>x.DisplayOrder))
                            {
                                var attributeValueModel = new SearchProductAttributeValueModel();
                                attributeValueModel.Id = value.Id;
                                attributeValueModel.Selected = value.IsPreSelected;
                                attributeValueModel.ValueText = value.Name;
                                attributeValueModel.Popularvalue = value.PopularValue;
                                attributeModel.Values.Add(attributeValueModel);
                            }
                        }
                        break;
                    }
                case SearchAttributeControlType.DropDown:
                    {
                        var attributeValues = attribute.CategoryProductAttributeValues;
                        if (attributeValues.Count > 0)
                        {
                            attributeModel.Values = new List<SearchProductAttributeValueModel>();
                            //if (attribute.CategoryProductAttributeValues.FirstOrDefault(x => x.IsPreSelected) == null)
                            //{
                            //    attributeModel.Values.Add(new SearchProductAttributeValueModel()
                            //    {
                            //        Id = 0,
                            //        ValueText = attribute.ProductAttribute.Name,
                            //        Selected = true
                            //    });
                            //}

                            if (attributeModel.Values.FirstOrDefault(x => x.Selected) == null)
                            {
                                attributeModel.Values.Add(new SearchProductAttributeValueModel()
                                {
                                    Id = 0,
                                    ValueText = attribute.MainAttribute ? attribute.ProductAttribute.Name : _localizationService.GetResource("ITBFA.Attribute.NoImportant"),
                                    Selected = true
                                });
                            }
                            else
                            {
                                if (attributeModel.Values.FirstOrDefault(x=>x.Id==0)==null)
                                {
                                    attributeModel.Values.Add(new SearchProductAttributeValueModel()
                                    {
                                        Id = 0,
                                        ValueText = attribute.MainAttribute ? attribute.ProductAttribute.Name : _localizationService.GetResource("ITBFA.Attribute.NoImportant"),
                                        Selected = false
                                       
                                    });
                                }
                            }


                            foreach (var value in attributeValues.OrderBy(x=>x.DisplayOrder))
                            {
                                var attributeValueModel = new SearchProductAttributeValueModel();
                                attributeValueModel.Id = value.Id;
                                attributeValueModel.ValueText = value.Name;
                                attributeValueModel.Popularvalue = value.PopularValue;
                                attributeModel.Values.Add(attributeValueModel);
                                if (value.IsPreSelected)
                                {
                                    attributeModel.SelectedAttributeId = value.Id;
                                }
                            }

                           
                        }

                        break;
                    }
                case SearchAttributeControlType.ToddlerIntBetween:
                case SearchAttributeControlType.ToddlerMax:
                case SearchAttributeControlType.ToddlerMin:
                    {
                        var values = attribute.CategoryProductAttributeValues.Where(x=>x.RealValue.HasValue)
                            .Where(x=>x.Products.Where(p=>!p.Deleted).Any() || x.Products.Count == 0);
                        if(!values.Any())
                        {
                            attributeModel.MinValue = 0;
                            attributeModel.MinValue = int.MaxValue;
                        }else
                        {
                            values = values.OrderByDescending(x=>x.RealValue).ToList();
                            attributeModel.MinValue = (int)values.Last().RealValue;
                            attributeModel.MaxValue = (int)values.First().RealValue;
                            attributeModel.Values = new List<SearchProductAttributeValueModel>();
                            foreach (var val in attribute.CategoryProductAttributeValues.Where(x => x.PopularValue).Where(x=>x.RealValue.HasValue))
                            {
                                var popularValue = new SearchProductAttributeValueModel();
                                popularValue.ValueDouble = ((int)val.RealValue).ToString();
                                attributeModel.Values.Add(popularValue);
                            }
                        }
                        break;
                    }
            }

            return attributeModel;
        }

        [NonAction]
        private SearchProductCategoryModel PrepareCustomerAttributes()
        {
            var model = new SearchProductCategoryModel();
            model.Currencies = _currencyService.GetAllCurrencies().Select(x => new CurrencyModel()
            {
                Id = x.Id,
                Name = x.CurrencyCode
            }).ToList();
            model.Attributes = new List<SearchProductAttributeModel>();
            model.Cities = _cityService.GetAllCities().OrderBy(x=>x.Title).Select(x => new CityModel()
            {
                Id = x.Id,
                Title = x.Title
            }).ToList();
            var customerAttributes = _customerInformationAttributeService.GetAllAttributes().OrderByDescending(x=>x.DisplayOrder);
            foreach (var attribute in customerAttributes)
            {
                var attributeModel = new SearchProductAttributeModel();
                attributeModel.Id = attribute.Id;
                attributeModel.AttributeControlTypeId = attribute.ProductSearchControlTypeId;
                attributeModel.AttributeTitle = typeof(Customer).GetProperty(attribute.CustomerFieldName)
                    .GetCustomAttribute<CustomerFieldIsInProductAttribute>().Alias;
                switch (attribute.ProductSearchControlType)
                {
                    case CustomerInformationProductSearchControlType.Gender:
                        {
                            attributeModel.Values = new List<SearchProductAttributeValueModel>();
                            var values = _customerInformationAttributeService.GetAttributeValuesForSearchProduct(attribute.Id);
                            foreach (var val in values)
                            {
                                var attributeValue = new SearchProductAttributeValueModel();
                                var referenceValue = _customerInformationAttributeService.GetValue(val.Id);
                                attributeValue.Id = val.Id;
                                attributeValue.ValueText = referenceValue.Text;
                                if (_workContext.CurrentCustomer.Gender.HasValue)
                                {
                                    attributeValue.Selected = _workContext.CurrentCustomer.Gender == referenceValue.Id;
                                }

                                attributeModel.Values.Add(attributeValue);
                            }
                            break;
                        }
                    case CustomerInformationProductSearchControlType.NumberTextBoxExact:
                    case CustomerInformationProductSearchControlType.NumberToddlerBetween:
                    case CustomerInformationProductSearchControlType.NumberToddlerLess:
                    case CustomerInformationProductSearchControlType.NumberToddlerMore:
                    case CustomerInformationProductSearchControlType.MoneyLess:
                    case CustomerInformationProductSearchControlType.MoneyMore:
                        {
                            if (_workContext.CurrentCustomer.IsRegistered())
                            {
                                var val = _customerInformationAttributeService.GetCustomerAttribute(attribute.Id, _workContext.CurrentCustomer);
                                attributeModel.AttributeValue = val.Text;
                                attributeModel.CurrencyId = val.Id;
                            }
                            break;
                        }
                }

                model.Attributes.Add(attributeModel);
            }

            return model;
        }

        [NonAction]
        private SearchProductCategoryModel RestoreSearchModel(SearchProductCategoryModel model)
        {
            foreach (var attributeModel in model.Attributes)
            {
                var attribute = _categoryProductAttributeService.GetCategoryProductAttributeById(attributeModel.Id);
                attributeModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(attribute.ProductAttribute.PictureId.GetValueOrDefault(), 100, false);
                attributeModel.Description = attribute.ProductAttribute.Description;
                switch (attributeModel.AttributeControlTypeId)
                {
                    case (int)SearchAttributeControlType.CheckBox:
                    case (int)SearchAttributeControlType.CheckBoxGroup:
                        {
                            //if(attributeModel
                                break;
                        }
                    case (int)SearchAttributeControlType.DropDown:
                        {
                            var attributeValues = attribute.CategoryProductAttributeValues;
                            if (attributeValues.Count > 0)
                            {
                                attributeModel.Values = new List<SearchProductAttributeValueModel>();

                                if (attributeModel.SelectedAttributeId == 0)
                                {
                                    attributeModel.Values.Add(new SearchProductAttributeValueModel()
                                                              {
                                                                  Id = 0,
                                                                  ValueText = attribute.MainAttribute ? attribute.ProductAttribute.Name : _localizationService.GetResource("ITBFA.Attribute.NoImportant"),
                                                                  Selected = true
                                                              });
                                }
                                //else
                                //{
                                //    if (attributeModel.Values.FirstOrDefault(x => x.Id == 0) == null)
                                //    {
                                //        attributeModel.Values.Add(new SearchProductAttributeValueModel()
                                //        {
                                //            Id = 0,
                                //            ValueText = attribute.ProductAttribute.Name,
                                //            Selected = false
                                //        });
                                //    }
                                //}

                                if (attributeModel.Values.FirstOrDefault(x => x.Selected) == null)
                                {
                                    attributeModel.Values.Add(new SearchProductAttributeValueModel()
                                    {
                                        Id = 0,
                                        ValueText = attribute.MainAttribute ? attribute.ProductAttribute.Name : _localizationService.GetResource("ITBFA.Attribute.NoImportant"),
                                        Selected = true
                                    });
                                }

                                foreach (var value in attributeValues.OrderBy(x=>x.DisplayOrder))
                                {
                                    var attributeValueModel = new SearchProductAttributeValueModel();
                                    attributeValueModel.Id = value.Id;
                                    attributeValueModel.ValueText = value.Name;
                                    attributeValueModel.Selected = value.Id == attributeModel.SelectedAttributeId;
                                    attributeValueModel.Popularvalue = value.PopularValue;
                                    attributeModel.Values.Add(attributeValueModel);
                                }

                                
                            }
                            break;
                        }
                    case (int)SearchAttributeControlType.ToddlerIntBetween:
                    case (int)SearchAttributeControlType.ToddlerMax:
                    case (int)SearchAttributeControlType.ToddlerMin:
                        {
                            var values = attribute.CategoryProductAttributeValues.Where(x => x.RealValue.HasValue)
                                .Where(x => x.Products.Where(p => !p.Deleted).Any() || x.Products.Count == 0);
                            if (values.Count() == 0)
                            {
                                attributeModel.MinValue = 0;
                                attributeModel.MinValue = int.MaxValue;
                            }
                            else
                            {
                                values = values.OrderByDescending(x => x.RealValue).ToList();
                                attributeModel.MinValue = (int)values.Last().RealValue;
                                attributeModel.MaxValue = (int)values.First().RealValue;
                                attributeModel.Values = new List<SearchProductAttributeValueModel>();
                                foreach (var val in attribute.CategoryProductAttributeValues.Where(x => x.PopularValue).Where(x => x.RealValue.HasValue))
                                {
                                    var popularValue = new SearchProductAttributeValueModel();
                                    popularValue.ValueDouble = ((int)val.RealValue).ToString();
                                    attributeModel.Values.Add(popularValue);
                                }
                            }

                            break;
                        }
                }
            }

            var currencies = _currencyService.GetAllCurrencies().Select(x => new CurrencyModel()
            {
                Id = x.Id,
                Name = x.CurrencyCode
            }).ToList();

            model.Currencies = currencies;
            return model;
        }

        [NonAction]
        private SearchProductCategoryModel RestoreCustomerSearchAttributes(SearchProductCategoryModel model)
        {
            foreach (var attributeModel in model.Attributes)
            {
                var attribute = _customerInformationAttributeService.GetAttributeById(attributeModel.Id);
                switch (attributeModel.AttributeControlTypeId)
                {
                    case (int)CustomerInformationProductSearchControlType.Gender:
                        {
                            attributeModel.Values = new List<SearchProductAttributeValueModel>();
                            var values = _customerInformationAttributeService.GetAttributeValuesForSearchProduct(attribute.Id);
                            foreach (var val in values)
                            {
                                var attributeValue = new SearchProductAttributeValueModel();
                                var referenceValue = _customerInformationAttributeService.GetValue(val.Id);
                                attributeValue.Id = val.Id;
                                attributeValue.ValueText = referenceValue.Text;
                                if (attributeModel.SelectedAttributeId != 0)
                                {
                                    attributeValue.Selected = attributeValue.Id == attributeModel.SelectedAttributeId;
                                }
                                else
                                {
                                    if (_workContext.CurrentCustomer.Gender.HasValue)
                                    {
                                        attributeValue.Selected = _workContext.CurrentCustomer.Gender == referenceValue.Id;
                                    }
                                }

                                attributeModel.Values.Add(attributeValue);
                            }
                            break;
                        }
                }
            }

            return model;
        }

        private SearchProductCategoryModel PrepareSearchCategoryModel(int categoryId, bool MainAttributes, bool additionalAttributes=false)
        {
            var currencies = _currencyService.GetAllCurrencies().Select(x => new CurrencyModel()
            {
                Id = x.Id,
                Name = x.CurrencyCode
            }).ToList();
            var category = _categoryService.GetCategoryById(categoryId);
            var categoryModel = new SearchProductCategoryModel();
            categoryModel.Currencies = currencies;
            categoryModel.Attributes = new List<SearchProductAttributeModel>();
            categoryModel.CateogyTitle = category.Name;
            categoryModel.CategoryId = category.Id;
            var attributeGroups = _categoryProductAttributeService.GetCategoryToCategoryProductAttributeGroupByCategoryId(category.Id);
            if (attributeGroups.Count > 0)
            {
                foreach (var group in attributeGroups)
                {
                    var attributes = group.CategoryProductAttributeGroup.CategoryProductAttributes;
                    attributes = attributes.Where(x => x.MainAttribute == MainAttributes && x.AdditionalAttribute == additionalAttributes).OrderByDescending(x => x.DisplayOrder).ToList();
                    if (attributes.Count > 0)
                    {
                        foreach (var attribute in attributes)
                        {
                            var attributeModel = PrepareProductAttributeModel(attribute);
                            categoryModel.Attributes.Add(attributeModel);
                        }
                    }
                }
            }

            return categoryModel;
        }
        
        
        public ActionResult HomePageSearchBlock()
        {
            var categories = _categoryService.GetAllCategories();
            var currencies = _currencyService.GetAllCurrencies().Select(x => new CurrencyModel()
            {
                Id = x.Id,
                Name = x.CurrencyCode
            }).ToList();
            var model = new SearchProductCategoriesModel();
            model.Categories = new List<SearchProductCategoryModel>();
            foreach (var category in categories)
            {
                var categoryModel = new SearchProductCategoryModel();
                categoryModel.Currencies = currencies;
                categoryModel.Attributes = new List<SearchProductAttributeModel>();
                categoryModel.CateogyTitle = category.Name;
                categoryModel.CategoryId = category.Id;
                categoryModel.SeoName = category.GetSeName(_workContext.WorkingLanguage.Id);
                var attributeGroups = _categoryProductAttributeService.GetCategoryToCategoryProductAttributeGroupByCategoryId(category.Id);
                if (attributeGroups.Count > 0)
                {
                    foreach (var group in attributeGroups)
                    {
                        var attributes = group.CategoryProductAttributeGroup.CategoryProductAttributes;
                        attributes = attributes.Where(x => x.MainAttribute).OrderByDescending(x=>x.DisplayOrder).ToList();
                        if (attributes.Count > 0)
                        {
                            foreach (var attribute in attributes)
                            {
                                var attributeModel = PrepareProductAttributeModel(attribute);
                                categoryModel.Attributes.Add(attributeModel);
                            }
                        }
                    }
                }

                model.Categories.Add(categoryModel);
            }

            model.CustomerAttributes = PrepareCustomerAttributes(); 
            return View(model);
        }



        [ParameterBasedOnFormNameAttribute("LowerButtonClick", "LowerButtonClick")]
        public ActionResult SearchResultsPage(SearchModel model, bool LowerButtonClick, string CategorySeName, int? tag = null)
        {
            var categoryId = _urlRecordService.GetBySlug(CategorySeName);
            Category categor;
            if (categoryId == null && model.SelectedCategoryAttributes == null && model.CustomerAttributes == null)
            {
                return RedirectToAction("HomePage");
            }
            else
            {
                if (model.SelectedCategoryAttributes != null && model.CustomerAttributes != null)
                {
                    categor = _categoryService.GetCategoryById(model.SelectedCategoryAttributes.CategoryId);
                }else
                {
                    categor = _categoryService.GetCategoryById(categoryId.EntityId);
                }
            }
            
            var a = 10;
            IList<SearchProductAttributeValue> attributesToSearch = null;// = ParceProductAttributesToSearch(model);
            IList<SearchProductAttributeValue> customerAttributes = null;// = ParceCustomerAttributesToSearch(model);

            var searchModelFull = new SearchModelFull();
            searchModelFull.LowerButtonClick = LowerButtonClick;
            if (model.SelectedCategoryAttributes != null)
            {
                attributesToSearch = ParceProductAttributesToSearch(model,true);
                searchModelFull.SelectedCategoryAttributes = RestoreSearchModel(model.SelectedCategoryAttributes);
                if (model.DetailedSelectedCategoryAttributes != null)
                {
                    ((List<SearchProductAttributeValue>)attributesToSearch).AddRange(ParceProductAttributesToSearch(model, false));
                    searchModelFull.DetailedSelectedCategoryAttributes = RestoreSearchModel(model.DetailedSelectedCategoryAttributes);
                }
                else
                {
                    searchModelFull.DetailedSelectedCategoryAttributes = PrepareSearchCategoryModel(categor.Id, false);
                    searchModelFull.DetailedSelectedCategoryAttributes.CategoryId = categor.Id;
                }

                if (model.SelectedAdditionalCategoryAttributes != null)
                {
                    ((List<SearchProductAttributeValue>)attributesToSearch).AddRange(ParceProductAttributesToSearch(model, false,true));
                    searchModelFull.SelectedAdditionalCategoryAttributes = RestoreSearchModel(model.SelectedAdditionalCategoryAttributes);
                }
                else
                {
                    searchModelFull.SelectedAdditionalCategoryAttributes = PrepareSearchCategoryModel(categor.Id, false, true);
                    searchModelFull.SelectedAdditionalCategoryAttributes.CategoryId = categor.Id;
                }
            }
            else
            {
                searchModelFull.DetailedSelectedCategoryAttributes = PrepareSearchCategoryModel(categor.Id, false);
                searchModelFull.DetailedSelectedCategoryAttributes.CategoryId = categor.Id;
                searchModelFull.SelectedAdditionalCategoryAttributes = PrepareSearchCategoryModel(categor.Id, false, true);
                searchModelFull.SelectedAdditionalCategoryAttributes.CategoryId = categor.Id;
                searchModelFull.SelectedCategoryAttributes = PrepareSearchCategoryModel(categor.Id,true);
            }

            if (model.CustomerAttributes == null)
            {
                searchModelFull.CustomerAttributes = PrepareCustomerAttributes();
            }
            else
            {
                customerAttributes = ParceCustomerAttributesToSearch(model);
                searchModelFull.CustomerAttributes = RestoreCustomerSearchAttributes(model.CustomerAttributes);
            }
            
            searchModelFull.CustomerAttributes.Cities = _cityService.GetAllCities().OrderBy(x=>x.Title).Select(x => new CityModel()
            {
                Id = x.Id,
                Title = x.Title
            }).ToList();

            searchModelFull.CustomerAttributes.Currencies = _currencyService.GetAllCurrencies().Select(x => new CurrencyModel()
            {
                Id = x.Id,
                Name = x.CurrencyCode
            }).ToList();
            int regionId = 0;
            if (model.CustomerAttributes != null && model.CustomerAttributes.CityId != 0)
            {
                var city = _cityService.GetById(model.CustomerAttributes.CityId);
                if (city != null)
                {
                    regionId = city.RegionId;
                }
            }

            var bestProducts = _productService.SearchProductsWithAttributes(0, ProductSortingEnum.CreatedOn, 0, short.MaxValue, 0, categor.Id, 0, 0, 0, null, null).Where(x => x.FeaturedProduct);


            var products = _productService.SearchProductsWithAttributes(0, ProductSortingEnum.CreatedOn, 0, short.MaxValue, tag.GetValueOrDefault(), categor.Id, 0, 0, regionId, attributesToSearch, customerAttributes);//дописать linq фильтр по параметрам


            var searchActivity = _customerActivityService.InsertActivity("PublicStore.SearchProduct", _localizationService.GetResource("ITBSFA.SearchLog.Message"), _workContext.CurrentCustomer, Request.UrlReferrer == null ? "" : Request.UrlReferrer.ToString(), 0);
            _searchLogService.LogSearchQuerryWithParameters(attributesToSearch, customerAttributes, searchActivity.Id, regionId, 0, categor.Id, tag.GetValueOrDefault(), 0, 0);

            var productsTags = products.SelectMany(x => x.ProductTags).DistinctBy(x => x.Name);

            foreach (var productsTag in productsTags)
                searchModelFull.ProductTags.Add(new ProductTagModel()
            {
                Name = productsTag.Name,
                Id = productsTag.Id
            });


            var categories = _categoryService.GetAllCategories();
         
            searchModelFull.Categories = new List<SearchCategoryModel>();
            foreach (var category in categories)
            {
                    var categoryModel = new SearchCategoryModel();
                    categoryModel.CateogyTitle = category.Name;
                    categoryModel.CategoryId = category.Id;
                    categoryModel.SeName = category.GetSeName(_workContext.WorkingLanguage.Id);
                    searchModelFull.Categories.Add(categoryModel);
            }

            searchModelFull.Products = new List<OffersProductModel>();
            if (products!=null && products.Any())
            {
                searchModelFull.Products = new List<OffersProductModel>();
                foreach (var i in products)
                {
                    var productModel = new OffersProductModel()
                    {
                        Id = i.Id,
                        Rating = Math.Round(i.Rating ?? 0),
                        BankRating = Math.Round(i.Customer.Rating ?? 0),
                        Name = i.Name,
                        ProductAttributeValue =
                            i.ProductAttributes.FirstOrDefault(x => x.CategoryProductAttribute.ProductBoxAttribute),
                        MetaTitle = i.MetaTitle,
                        PictureThumbnailUrl =
                            _pictureService.GetPictureUrl(i.Customer.ProviderLogoId.GetValueOrDefault(), 100, false),
                        ShortDescription = i.ShortDescription,
                        FullDescription = i.FullDescription,
                        SeName = i.GetSeName(),
                        OrderingLink = i.OrderLink,
                    };
                    searchModelFull.Products.Add(productModel);
                }
            }

            if (bestProducts.Any())
            {
                searchModelFull.BestProducts = new List<OffersProductModel>();
                foreach (var i in bestProducts)
                {
                    var productModel = new OffersProductModel()
                    {
                        Id = i.Id,
                        Rating = Math.Round(i.Rating ?? 0),
                        BankRating = Math.Round(i.Customer.Rating ?? 0),
                        Name = i.Name,
                        ProductAttributeValue =
                            i.ProductAttributes.FirstOrDefault(x => x.CategoryProductAttribute.ProductBoxAttribute),
                        MetaTitle = i.MetaTitle,
                        PictureThumbnailUrl =
                            _pictureService.GetPictureUrl(i.Customer.ProviderLogoId.GetValueOrDefault(), 100, false),
                        ShortDescription = i.ShortDescription,
                        FullDescription = i.FullDescription,
                        SeName = i.GetSeName(),
                        OrderingLink = i.OrderLink,
                    };
                    searchModelFull.BestProducts.Add(productModel);
                }
            }

            return View(searchModelFull);
        }

        [NonAction]
        private IList<SearchProductAttributeValue> ParceProductAttributesToSearch(SearchModel model, bool MainAttributes, bool additionalAttributes=false)
        {
            var list = new List<SearchProductAttributeValue>();
            IList<SearchProductAttributeModel> attributeModelList = new List<SearchProductAttributeModel>();
            if (MainAttributes)
            {
                if(model.SelectedCategoryAttributes.Attributes != null)
                    attributeModelList = model.SelectedCategoryAttributes.Attributes.Where(x => !String.IsNullOrEmpty(x.AttributeValue) ||
                    !String.IsNullOrEmpty(x.AttributeValueMax) ||
                    x.SelectedAttributeId != 0 || x.SelectedIntValue != 0 || x.SelectedMaxIntValue != 0 || (x.Values != null && x.Values.Where(v=>v.Selected).Count() > 0)).ToList();
            }
            else
            {
                if (!MainAttributes && !additionalAttributes)
                {
                    if(model.DetailedSelectedCategoryAttributes.Attributes != null)
                        attributeModelList =
                            model.DetailedSelectedCategoryAttributes.Attributes.Where(
                                x => !String.IsNullOrEmpty(x.AttributeValue) ||
                                     !String.IsNullOrEmpty(x.AttributeValueMax) ||
                                     x.SelectedAttributeId != 0 || x.SelectedIntValue != 0 || x.SelectedMaxIntValue != 0 ||
                                     (x.Values != null && x.Values.Where(v => v.Selected).Count() > 0)).ToList();
                }
                else
                {
                    if(model.SelectedAdditionalCategoryAttributes.Attributes != null)
                        attributeModelList =
                            model.SelectedAdditionalCategoryAttributes.Attributes.Where(
                                x => !String.IsNullOrEmpty(x.AttributeValue) ||
                                     !String.IsNullOrEmpty(x.AttributeValueMax) ||
                                     x.SelectedAttributeId != 0 || x.SelectedIntValue != 0 || x.SelectedMaxIntValue != 0 ||
                                     (x.Values != null && x.Values.Where(v => v.Selected).Count() > 0)).ToList();
                }
            }

            foreach (var attribute in attributeModelList)
            {
                list.AddRange(PrepareAttributeModelForSearch(attribute));
            }

            return list;
        }

        [NonAction]
        private IList<SearchProductAttributeValue> PrepareAttributeModelForSearch(SearchProductAttributeModel model)
        {
            var attributesForSearch = new List<SearchProductAttributeValue>();

            switch ((SearchAttributeControlType)model.AttributeControlTypeId)
            {
                case SearchAttributeControlType.CheckBox:
                case SearchAttributeControlType.CheckBoxGroup:
                    {
                        foreach (var value in model.Values)
                        {
                            if (value.Selected)
                            {
                                var attributeForSearch = new SearchProductAttributeValue();
                                attributeForSearch.CategoryProductAttributeId = model.Id;
                                attributeForSearch.IdValue = value.Id;
                                attributeForSearch.ControlType = (SearchAttributeControlType)model.AttributeControlTypeId;
                                attributesForSearch.Add(attributeForSearch);
                            }
                        }
                        break;
                    }
                case SearchAttributeControlType.DropDown:
                    {
                        if (model.SelectedAttributeId != 0)
                        {
                            var attributeForSearch = new SearchProductAttributeValue();
                            attributeForSearch.CategoryProductAttributeId = model.Id;
                            attributeForSearch.IdValue = model.SelectedAttributeId;
                            attributeForSearch.ControlType = (SearchAttributeControlType)model.AttributeControlTypeId;
                            attributesForSearch.Add(attributeForSearch);
                        }
                        break;
                    }
                case SearchAttributeControlType.TextBoxText:
                    {
                        if (!String.IsNullOrEmpty(model.AttributeValue))
                        {
                            var attributeForSearch = new SearchProductAttributeValue();
                            attributeForSearch.CategoryProductAttributeId = model.Id;
                            attributeForSearch.Textvalue = model.AttributeValue;
                            attributeForSearch.ControlType = (SearchAttributeControlType)model.AttributeControlTypeId;
                            attributesForSearch.Add(attributeForSearch);
                        }
                        break;
                    }
                case SearchAttributeControlType.TextBoxReal:
                    {
                        if (!String.IsNullOrEmpty(model.AttributeValue))
                        {
                            var attributeForSearch = new SearchProductAttributeValue();
                            attributeForSearch.CategoryProductAttributeId = model.Id;
                            double res = 0;
                            double.TryParse(model.AttributeValue, out res);
                            attributeForSearch.ExactValue = res;
                            attributeForSearch.ControlType = (SearchAttributeControlType)model.AttributeControlTypeId;
                            attributesForSearch.Add(attributeForSearch);
                        }
                        break;
                    }
                case SearchAttributeControlType.ToddlerMax:
                    {
                        if (model.MinValue != 0 && model.SelectedIntValue != 0)
                        {
                            var attributeForSearch = new SearchProductAttributeValue();
                            attributeForSearch.CategoryProductAttributeId = model.Id;
                            attributeForSearch.ControlType = (SearchAttributeControlType)model.AttributeControlTypeId;
                            attributeForSearch.MinValue = model.MinValue;
                            attributeForSearch.MaxValue = model.SelectedIntValue;
                            attributesForSearch.Add(attributeForSearch);
                        }

                        break;
                    }
                case SearchAttributeControlType.ToddlerMin:
                    {
                        if (model.MaxValue != 0 && model.SelectedIntValue != 0)
                        {
                            var attributeForSearch = new SearchProductAttributeValue();
                            attributeForSearch.CategoryProductAttributeId = model.Id;
                            attributeForSearch.ControlType = (SearchAttributeControlType)model.AttributeControlTypeId;
                            attributeForSearch.MinValue = model.SelectedIntValue;
                            attributeForSearch.MaxValue = model.MaxValue;
                            attributesForSearch.Add(attributeForSearch);
                        }

                        break;
                    }
                case SearchAttributeControlType.ToddlerIntBetween:
                    {
                        if (model.SelectedMaxIntValue != 0 && model.SelectedIntValue != 0)
                        {
                            var attributeForSearch = new SearchProductAttributeValue();
                            attributeForSearch.CategoryProductAttributeId = model.Id;
                            attributeForSearch.ControlType = (SearchAttributeControlType)model.AttributeControlTypeId;
                            attributeForSearch.MinValue = model.SelectedIntValue;
                            attributeForSearch.MaxValue = model.SelectedMaxIntValue;
                            attributesForSearch.Add(attributeForSearch);
                        }

                        break;
                    }
                case SearchAttributeControlType.Money:
                    {
                        if(!String.IsNullOrEmpty(model.AttributeValue) || !String.IsNullOrEmpty(model.AttributeValueMax))
                        {
                            var currency = _currencyService.GetCurrencyById(model.CurrencyId);
                            var attributeForSearch = new SearchProductAttributeValue();
                            attributeForSearch.CategoryProductAttributeId = model.Id;
                            attributeForSearch.ControlType = (SearchAttributeControlType)model.AttributeControlTypeId;
                            if (!String.IsNullOrEmpty(model.AttributeValue))
                            {
                                double res = 0;
                                double.TryParse(model.AttributeValue, out res);
                                attributeForSearch.MinValue = res/((double)currency.Rate);
                                attributeForSearch.MaxValue = res / ((double)currency.Rate);
                            }
                            attributesForSearch.Add(attributeForSearch);
                        }
                        break;
                    }
            }

            return attributesForSearch;
        }
        
        [NonAction]
        private IList<SearchProductAttributeValue> ParceCustomerAttributesToSearch(SearchModel model)
        {
            var list = new List<SearchProductAttributeValue>();
            foreach (var attribute in model.CustomerAttributes.Attributes.Where(x => !String.IsNullOrEmpty(x.AttributeValue) ||
                !String.IsNullOrEmpty(x.AttributeValueMax) ||
                x.SelectedAttributeId != 0))
            {
                var attributeForSearch = new SearchProductAttributeValue();
                attributeForSearch.CustomerControlType = (CustomerInformationProductSearchControlType)attribute.AttributeControlTypeId;

                if (!String.IsNullOrEmpty(attribute.AttributeValue))
                {
                    double res = 0;
                    double.TryParse(attribute.AttributeValue, out res);
                    if (attribute.CurrencyId != 0)
                    {
                        var currency = _currencyService.GetCurrencyById(attribute.CurrencyId);
                        res = res / (double)currency.Rate;
                    }
                    attributeForSearch.ExactValue = res;
                    attributeForSearch.MinValue = res;
                }
                attributeForSearch.IdValue = attribute.SelectedAttributeId;
                if (!String.IsNullOrEmpty(attribute.AttributeValueMax))
                {
                    double res = 0;
                    double.TryParse(attribute.AttributeValueMax, out res);
                    if (attribute.CurrencyId != 0)
                    {
                        var currency = _currencyService.GetCurrencyById(attribute.CurrencyId);
                        res = res / (double)currency.Rate;
                    }
                    attributeForSearch.MaxValue = res;
                }
                attributeForSearch.Textvalue = attribute.AttributeValue;
                attributeForSearch.CategoryProductAttributeId = attribute.Id;
                list.Add(attributeForSearch);
            }

            return list;
        }
    }
}
