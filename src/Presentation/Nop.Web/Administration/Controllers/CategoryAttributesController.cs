using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models.Catalog;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;
using Nop.Services.Media;
using System.Text;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class CategoryAttributesController : BaseNopController
    {

        #region Fields

        private readonly ICategoryProductAttributeService _categoryProductAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IWorkContext _workContext;
        private readonly ICategoryService _categoryService;
        private readonly IConversionImageService _conversionImageService;
        private readonly IPictureService _pictureService;

        #endregion

        #region constructor

        public CategoryAttributesController(ICategoryProductAttributeService categoryProductAttributeService,
            IProductAttributeService productAttributeService, IWorkContext workContext,
            ILanguageService languageService, ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService, ICustomerActivityService customerActivityService,
            IPermissionService permissionService,
            ICategoryService categoryService,
            IConversionImageService conversionImageService,
            IPictureService pictureService)
        {
            this._categoryProductAttributeService = categoryProductAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._productAttributeService = productAttributeService;
            this._workContext = workContext;
            this._categoryService = categoryService;
            this._conversionImageService = conversionImageService;
            this._pictureService = pictureService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected void UpdateLocales(CategoryProductAttributeGroup productAttribute, CategoryProductAttributeGroupModel model)
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
        
        [NonAction]
        protected void UpdateAttributeValueLocales(CategoryProductAttributeValue pvav, CategoryProductAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(pvav,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        protected List<ConversionImageModel> PrepareConversionImagesList(int categoryAttributeGroupId)
        {
            var imagesList = new List<ConversionImageModel>();
            if (categoryAttributeGroupId != 0)
            {
                var group = _categoryProductAttributeService.GetCategoryProductAttributeGroupById(categoryAttributeGroupId);
                foreach (var image in group.ConversionImages)
                {
                    var model = new ConversionImageModel()
                    {
                        Name = image.GetLocalized(x => x.Name, _workContext.WorkingLanguage.Id, false),
                        Id = image.Id,
                        GroupModelId = categoryAttributeGroupId
                    };
                    model.Locales = new List<ConversionImageLocalizedModel>();
                    AddLocales(_languageService, model.Locales, (locale, languageId) =>
                    {
                        locale.Name = image.GetLocalized(x => x.Name, languageId, false, false);
                        locale.PictureId = image.GetLocalized(x => x.PictureId, languageId, false, false);
                    });
                    imagesList.Add(model);
                }
                return imagesList;
            }
            return imagesList;
        }

        #endregion

        #region Main Actions
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var categoryProductAttributeGroupss = _categoryProductAttributeService.GetAllCategoryProductAttributeGroups();
            var data = categoryProductAttributeGroupss.Select(x => { x.Name = x.GetLocalized(z => z.Name,_workContext.WorkingLanguage.Id); return x; }).ToList();
            var gridModel = new GridModel<CategoryProductAttributeGroupModel>
            {
                Data = data.Select(x => x.ToModel()),
                Total = categoryProductAttributeGroupss.Count()
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var categoryProductAttributeGroupss = _categoryProductAttributeService.GetAllCategoryProductAttributeGroups();
            var data = categoryProductAttributeGroupss.Select(x => { x.Name = x.GetLocalized(z => z.Name, _workContext.WorkingLanguage.Id); return x; }).ToList();
            var gridModel = new GridModel<CategoryProductAttributeGroupModel>
            {
                Data = data.Select(x => x.ToModel()),
                Total = categoryProductAttributeGroupss.Count()
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

            var model = new CategoryProductAttributeGroupModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(CategoryProductAttributeGroupModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var categoryProductAttributeGroup = model.ToEntity();
                _categoryProductAttributeService.InsertCategoryProductAttributeGroup(categoryProductAttributeGroup);
                UpdateLocales(categoryProductAttributeGroup, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewCategoryProductAttributeGroup", _localizationService.GetResource("ActivityLog.AddNewCategoryProductAttributeGroup"), categoryProductAttributeGroup.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.CategoryProductAttributeGroup.Added"));
                model.ConversionImages = PrepareConversionImagesList(0);
                return continueEditing ? RedirectToAction("Edit", new { id = categoryProductAttributeGroup.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productAttribute = _categoryProductAttributeService.GetCategoryProductAttributeGroupById(id);
            if (productAttribute == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            var model = productAttribute.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = productAttribute.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = productAttribute.GetLocalized(x => x.Description, languageId, false, false);
            });
            model.ConversionImages = PrepareConversionImagesList(id);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(CategoryProductAttributeGroupModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var categoryProductAttributeGroup = _categoryProductAttributeService.GetCategoryProductAttributeGroupById(model.Id);
            if (categoryProductAttributeGroup == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                categoryProductAttributeGroup = model.ToEntity(categoryProductAttributeGroup);
                _categoryProductAttributeService.UpdateCategoryProductAttributeGroup(categoryProductAttributeGroup);

                UpdateLocales(categoryProductAttributeGroup, model);

                //activity log
                _customerActivityService.InsertActivity("EditCategoryProductAttributeGroup", _localizationService.GetResource("ActivityLog.EditCategoryProductAttributeGroup"), categoryProductAttributeGroup.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.ProductAttributes.Updated"));
                model.ConversionImages = PrepareConversionImagesList(model.Id);
                return continueEditing ? RedirectToAction("Edit", categoryProductAttributeGroup.Id) : RedirectToAction("List");
            }
            model.ConversionImages = PrepareConversionImagesList(model.Id);
            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var productAttribute = _categoryProductAttributeService.GetCategoryProductAttributeGroupById(id);
            if (productAttribute == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            _categoryProductAttributeService.DeleteCategoryProductAttributeGroup(productAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteCategoryProductAttribute", _localizationService.GetResource("ActivityLog.DeleteCategoryProductAttribute"), productAttribute.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.ProductAttributes.Deleted"));
            return RedirectToAction("List");
        }
        #endregion

        #region Product variant attributes

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult CategoryProductAttributeList(GridCommand command, int categoryProductGroupId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var categoryProductAttributes = _categoryProductAttributeService.GetCategoryProductAttributesByCategoryProductGroupId(categoryProductGroupId);
            var categoryProductAttributesModel = categoryProductAttributes
                .Select(x =>
                {
                    var pvaModel = new CategoryProductAttributeModel()
                    {
                        Id = x.Id,
                        CategoryProductGroupId = x.CategoryProductGroupId,
                        ProductAttribute = _productAttributeService.GetProductAttributeById(x.ProductAttributeId).Name,
                        ProductAttributeId = x.ProductAttributeId,
                        SearchAttributeControlTypeId = x.SearchControlTypeId,
                        SearchAttributeControlType = x.SearchControlType.GetLocalizedEnum(_localizationService,_workContext),
                        TextPrompt = x.TextPrompt,
                        IsRequired = x.IsRequired,
                        AdditionalAttribute = x.AdditionalAttribute,
                        MainAttribute = x.MainAttribute,
                        AttributeControlType = x.AttributeControlType.GetLocalizedEnum(_localizationService, _workContext),
                        AttributeControlTypeId = x.AttributeControlTypeId,
                        DisplayOrder1 = x.DisplayOrder,
                        ProductBoxAttribute = x.ProductBoxAttribute
                    };

                    if (x.ShouldHaveValues())
                    {
                        pvaModel.ViewEditUrl = Url.Action("EditCategoryProductAttributeValues", "CategoryAttributes", new { categoryProductAttributeId = x.Id });
                        pvaModel.ViewEditText = string.Format(_localizationService.GetResource("Admin.Catalog.CategoryProductAttributes.Attributes.Values.ViewLink"), x.CategoryProductAttributeValues != null ? x.CategoryProductAttributeValues.Count : 0);
                    }
                    return pvaModel;
                })
                .ToList();

            var model = new GridModel<CategoryProductAttributeModel>
            {
                Data = categoryProductAttributesModel,
                Total = categoryProductAttributesModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult CategoryProductAttributeInsert(GridCommand command, CategoryProductAttributeModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var pva = new CategoryProductAttribute()
            {
                CategoryProductGroupId = model.CategoryProductGroupId,
                ProductAttributeId = Int32.Parse(model.ProductAttribute), //use ProductAttribute property (not ProductAttributeId) because appropriate property is stored in it
                TextPrompt = model.TextPrompt,
                IsRequired = model.IsRequired,
                MainAttribute = model.MainAttribute,
                ProductBoxAttribute = model.ProductBoxAttribute,
                AdditionalAttribute = model.AdditionalAttribute,
                AttributeControlTypeId = Int32.Parse(model.AttributeControlType), //use AttributeControlType property (not AttributeControlTypeId) because appropriate property is stored in it
                SearchControlTypeId = Int32.Parse(model.SearchAttributeControlType),
                DisplayOrder = model.DisplayOrder1
            };

            bool flag = false;
            var categories = _categoryService.GetAllCategories().Where(x=>x.CategoryToCategoryProductAttributeGroups.Where(g=>g.CategoryProductAttributeGroupId == model.CategoryProductGroupId).FirstOrDefault() != null);
            foreach (var category in categories)
            {
                foreach (var group in category.CategoryToCategoryProductAttributeGroups.Where(x => x.CategoryProductAttributeGroupId != model.CategoryProductGroupId)
                    .Select(x => x.CategoryProductAttributeGroup))
                {
                    var attr = group.CategoryProductAttributes.Where(x => x.ProductAttributeId == pva.ProductAttributeId).FirstOrDefault();
                    if (attr != null)
                    {
                        flag = true;
                    }
                }
            }

            string conflictMessage = "";
            conflictMessage = ValidateForConflict(pva);
            //productAttribute.
            if (!flag)
            {
                if (!String.IsNullOrEmpty(conflictMessage))
                {
                    ModelState.AddModelError("ProductAttribute", conflictMessage);
                    var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                    return Content(modelStateErrors.FirstOrDefault());
                }
                _categoryProductAttributeService.InsertCategoryProductAttribute(pva);
            }
            else
            {
                ModelState.AddModelError("ProductAttribute", "Category attribute group conflict");
                var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            return CategoryProductAttributeList(command, model.CategoryProductGroupId);
        }

        private string ValidateForConflict(CategoryProductAttribute pva)
        {
            var sb = new StringBuilder();
            if (pva.MainAttribute && pva.AdditionalAttribute)
            {
                sb.AppendLine(_localizationService.GetResource("ITBSFA.Admin.MainAttributeAdditionalAttribute.Conflict"));
            }
            if (pva.MainAttribute)
            {
                if (pva.SearchControlType == SearchAttributeControlType.CheckBox ||
                    pva.SearchControlType == SearchAttributeControlType.CheckBoxGroup ||
                    pva.SearchControlType == SearchAttributeControlType.ToddlerIntBetween ||
                    pva.SearchControlType == SearchAttributeControlType.ToddlerMax ||
                    pva.SearchControlType == SearchAttributeControlType.ToddlerMin)
                {
                    sb.AppendLine(_localizationService.GetResource("ITBSFA.Admin.MainAttribute.Conflict"));
                }
            }

            if (pva.AdditionalAttribute)
            {
                if (!(pva.SearchControlType == SearchAttributeControlType.ToddlerIntBetween ||
                    pva.SearchControlType == SearchAttributeControlType.ToddlerMax ||
                    pva.SearchControlType == SearchAttributeControlType.ToddlerMin))
                {
                    sb.AppendLine(_localizationService.GetResource("ITBSFA.Admin.AdditionalAttribute.Conflict"));
                }
            }

            if (pva.AttributeControlType == AttributeControlType.Money ||
                pva.AttributeControlType == AttributeControlType.MoneyRange)
            {
                if (pva.SearchControlType != SearchAttributeControlType.Money)
                {
                    sb.AppendLine(_localizationService.GetResource("ITBSFA.Admin.Money.Conflict"));
                }
            }

            if (pva.AttributeControlType == AttributeControlType.ToddlerInt)
            {
                if (!(pva.SearchControlType == SearchAttributeControlType.TextBoxReal ||
                     pva.SearchControlType == SearchAttributeControlType.ToddlerIntBetween ||
                    pva.SearchControlType == SearchAttributeControlType.ToddlerMax ||
                    pva.SearchControlType == SearchAttributeControlType.ToddlerMin))
                {
                    sb.AppendLine(_localizationService.GetResource("ITBSFA.Admin.ToddlerInt.Conflict"));
                }
            }

            if (pva.AttributeControlType == AttributeControlType.DropdownList)
            {
                if (pva.SearchControlType != SearchAttributeControlType.DropDown)
                {
                    sb.AppendLine(_localizationService.GetResource("ITBSFA.Admin.DropDownList.Conflict"));
                }
            }

            if (pva.AttributeControlType == AttributeControlType.TextBox)
            {
                if (pva.SearchControlType != SearchAttributeControlType.TextBoxText)
                {
                    sb.AppendLine(_localizationService.GetResource("ITBSFA.Admin.TextBox.Conflict"));
                }
            }

            if (pva.AttributeControlType == AttributeControlType.Checkboxes)
            {
                if (!(pva.SearchControlType == SearchAttributeControlType.CheckBoxGroup ||
                    pva.SearchControlType == SearchAttributeControlType.CheckBox))
                {
                    sb.AppendLine(_localizationService.GetResource("ITBSFA.Admin.CheckBox.Conflict"));
                }
            }

            return sb.ToString();
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult CategoryProductAttributeUpdate(GridCommand command, CategoryProductAttributeModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var pva = _categoryProductAttributeService.GetCategoryProductAttributeById(model.Id);
            if (pva == null)
                throw new ArgumentException("No product variant attribute found with the specified id");

            //use ProductAttribute property (not ProductAttributeId) because appropriate property is stored in it
            pva.ProductAttributeId = Int32.Parse(model.ProductAttribute);
            pva.TextPrompt = model.TextPrompt;
            pva.IsRequired = model.IsRequired;
            pva.MainAttribute = model.MainAttribute;
            pva.ProductBoxAttribute = model.ProductBoxAttribute;
            pva.AdditionalAttribute = model.AdditionalAttribute;
            //use AttributeControlType property (not AttributeControlTypeId) because appropriate property is stored in it
            if (pva.AttributeControlTypeId != Int32.Parse(model.AttributeControlType))
            {
                foreach (var val in pva.CategoryProductAttributeValues.ToList())
                {
                    _categoryProductAttributeService.DeleteCategoryProductAttributeValue(val);
                }
            }
            pva.AttributeControlTypeId = Int32.Parse(model.AttributeControlType);

            pva.SearchControlTypeId = Int32.Parse(model.SearchAttributeControlType);
            pva.DisplayOrder = model.DisplayOrder1;
            
            bool flag = false;
            var categories = _categoryService.GetAllCategories().Where(x => x.CategoryToCategoryProductAttributeGroups.Where(g => g.CategoryProductAttributeGroupId == pva.CategoryProductGroupId).FirstOrDefault() != null);
            foreach (var category in categories)
            {
                foreach (var group in category.CategoryToCategoryProductAttributeGroups.Where(x => x.CategoryProductAttributeGroupId != pva.CategoryProductGroupId)
                    .Select(x => x.CategoryProductAttributeGroup))
                {
                    var attr = group.CategoryProductAttributes.Where(x => x.ProductAttributeId == pva.ProductAttributeId).FirstOrDefault();
                    if (attr != null)
                    {
                        flag = true;
                    }
                }
            }
            //productAttribute.
            string conflictMessage = "";
            conflictMessage = ValidateForConflict(pva);
            if (!flag)
            {
                if (!String.IsNullOrEmpty(conflictMessage))
                {
                    ModelState.AddModelError("ProductAttribute", conflictMessage);
                    var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                    return Content(modelStateErrors.FirstOrDefault());
                }
                _categoryProductAttributeService.UpdateCategoryProductAttribute(pva);
            }
            else
            {
                ModelState.AddModelError("ProductAttribute", "Category attribute group conflict");
                var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }
            return CategoryProductAttributeList(command, pva.CategoryProductGroupId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult CategoryProductAttributeDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var pva = _categoryProductAttributeService.GetCategoryProductAttributeById(id);
            if (pva == null)
                throw new ArgumentException("No product variant attribute found with the specified id");

            var categoryProductGroupId = pva.CategoryProductGroupId;
            _categoryProductAttributeService.DeleteCategoryProductAttribute(pva);

            return CategoryProductAttributeList(command, categoryProductGroupId);
        }

        #endregion

        #region Conversion images

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ConversionImagesList(GridCommand command, int Id)
        {
            var conversionImages = _categoryProductAttributeService.GetCategoryProductAttributeGroupById(Id).ConversionImages;
            var conversionImagesListModel = PrepareConversionImagesList(Id);

            var model = new GridModel<ConversionImageModel>
            {
                Data = conversionImagesListModel,
                Total = conversionImagesListModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ConversionImageInsert(GridCommand command, ConversionImageModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();
            var group = _categoryProductAttributeService.GetCategoryProductAttributeGroupById(model.GroupModelId);
            if (!ModelState.IsValid)
            {
                var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var conversionImage = new ConversionImage()
            {
                CategoryAttributeGroupId = model.GroupModelId,
                Name = model.Name,
                PictureId = 0,
            };
            _conversionImageService.Insert(conversionImage);
            _localizedEntityService.SaveLocalizedValue(conversionImage, x => x.Name,conversionImage.Name, _workContext.WorkingLanguage.Id);
            group.ConversionImages.Add(conversionImage);

            return ConversionImagesList(command, model.GroupModelId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ConversionImageUpdate(GridCommand command, ConversionImageModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();
            if (!ModelState.IsValid)
            {
                var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var conversionImage = _conversionImageService.GetConversionImageById(model.Id);
            _localizedEntityService.SaveLocalizedValue(conversionImage, x => x.Name, model.Name, _workContext.WorkingLanguage.Id);
            
            return ConversionImagesList(command, model.GroupModelId);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ConversionImageDelete(GridCommand command, int id)
        {
            var conversionImage = _conversionImageService.GetConversionImageById(id);
            if (conversionImage != null)
            {
                int groupId = conversionImage.CategoryAttributeGroupId;
                foreach (var lang in _languageService.GetAllLanguages())
                {
                    int pictureId = conversionImage.GetLocalized(x => x.PictureId, lang.Id, false);
                    if (pictureId != 0)
                    {
                        _pictureService.DeletePicture(_pictureService.GetPictureById(pictureId));
                    }
                }
                _conversionImageService.Delete(conversionImage);
                return ConversionImagesList(command, groupId);
            }

            var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
            return Content(modelStateErrors.FirstOrDefault());
        }

        #endregion


        public ActionResult EditConversionImageValuePopUp(int Id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();
            var conversionImage = _conversionImageService.GetConversionImageById(Id);
            var model = new ConversionImageModel()
            {
                Id = conversionImage.Id,
                GroupModelId = conversionImage.CategoryAttributeGroupId,
                PictureId = conversionImage.PictureId
            };

            model.Locales = new List<ConversionImageLocalizedModel>();
            AddLocales(_languageService,model.Locales,(locale,languageId)=>
                {
                    locale.Name = conversionImage.GetLocalized(x => x.Name, languageId, false);
                    locale.PictureId = conversionImage.GetLocalized(x => x.PictureId, languageId, false);
                });

            return View(model);
        }

        [HttpPost]
        public ActionResult EditConversionImageValuePopUp(ConversionImageModel model, string btnId, string formId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}

            var conversionImage = _conversionImageService.GetConversionImageById(model.Id);
            foreach (var locale in model.Locales)
            {
                if (String.IsNullOrEmpty(locale.Name) || locale.PictureId == 0)
                    continue;

                _localizedEntityService.SaveLocalizedValue(conversionImage, x => x.Name, locale.Name, locale.LanguageId);
                _localizedEntityService.SaveLocalizedValue(conversionImage, x => x.PictureId, locale.PictureId, locale.LanguageId);
            }

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        #region Product variant attribute values

        //list
        public ActionResult EditCategoryProductAttributeValues(int categoryProductAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var pva = _categoryProductAttributeService.GetCategoryProductAttributeById(categoryProductAttributeId);
            var model = new CategoryProductAttributeValueListModel()
            {
                CategoryGroupName = pva.CategoryProductGroup.Name,
                CategoryGroupId = pva.CategoryProductGroupId,
                CategoryProductAttributeName = pva.ProductAttribute.Name,
                CategoryProductAttributeId = pva.Id,
            };

            return View(model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult CategoryProductAttributeValueList(int categoryProductAttributeId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var values = _categoryProductAttributeService.GetCategoryProductAttributeValues(categoryProductAttributeId);
            var data = values.Select(x => { x.Name = x.GetLocalized(z => z.Name,_workContext.WorkingLanguage.Id); return x; }).ToList();
            var gridModel = new GridModel<CategoryProductAttributeValueModel>
            {
                Data = values.Select(x =>
                {
                    return new CategoryProductAttributeValueModel()
                    {
                        Id = x.Id,
                        CategoryProductAttributeId = x.CategoryProductAttributeId,
                        ColorSquaresRgb = x.ColorSquaresRgb,
                        Name = x.Name,
                        IsPreSelected = x.IsPreSelected,
                        DisplayOrder = x.DisplayOrder,
                        Popularvalue = x.PopularValue
                    };
                }),
                Total = values.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        //create
        public ActionResult CategoryProductAttributeValueCreatePopup(int categoryProductAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var pva = _categoryProductAttributeService.GetCategoryProductAttributeById(categoryProductAttributeId);
            var model = new CategoryProductAttributeValueModel();
            model.CategoryProductAttributeId = categoryProductAttributeId;


            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public ActionResult CategoryProductAttributeValueCreatePopup(string btnId, string formId, CategoryProductAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var pva = _categoryProductAttributeService.GetCategoryProductAttributeById(model.CategoryProductAttributeId);
            if (pva == null)
                //No product variant attribute found with the specified id
                return RedirectToAction("List", "Product");


            if (ModelState.IsValid)
            {
                var pvav = new CategoryProductAttributeValue()
                {
                    CategoryProductAttributeId = model.CategoryProductAttributeId,
                    Name = model.Name,
                    ColorSquaresRgb = model.ColorSquaresRgb,
                    //PriceAdjustment = model.PriceAdjustment,
                    //WeightAdjustment = model.WeightAdjustment,
                    IsPreSelected = model.IsPreSelected,
                    DisplayOrder = model.DisplayOrder,
                    PopularValue = model.Popularvalue
                };

                if (model.Realvalue.HasValue)
                {
                    pvav.RealValue = model.Realvalue;
                    if (String.IsNullOrEmpty(model.Name))
                    {
                        pvav.Name = ((int)model.Realvalue).ToString();
                    }
                }
                _categoryProductAttributeService.InsertCategoryProductAttributeValue(pvav);
                UpdateAttributeValueLocales(pvav, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public ActionResult CategoryProductAttributeValueEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var pvav = _categoryProductAttributeService.GetCategoryProductAttributeValueById(id);
            if (pvav == null)
                //No attribute value found with the specified id
                return RedirectToAction("List", "Product");
            
            var model = new CategoryProductAttributeValueModel()
            {
                CategoryProductAttributeId = pvav.CategoryProductAttributeId,
                Name = pvav.Name,
                ColorSquaresRgb = pvav.ColorSquaresRgb,
                DisplayColorSquaresRgb = false,
                //PriceAdjustment = pvav.PriceAdjustment,
                //WeightAdjustment = pvav.WeightAdjustment,
                IsPreSelected = pvav.IsPreSelected,
                DisplayOrder = pvav.DisplayOrder,
                Popularvalue = pvav.PopularValue,
                Realvalue = pvav.RealValue
            };
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = pvav.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost]
        public ActionResult CategoryProductAttributeValueEditPopup(string btnId, string formId, CategoryProductAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var pvav = _categoryProductAttributeService.GetCategoryProductAttributeValueById(model.Id);
            if (pvav == null)
                //No attribute value found with the specified id
                return RedirectToAction("List", "Product");


            if (ModelState.IsValid)
            {
                pvav.Name = model.Name;
                pvav.ColorSquaresRgb = model.ColorSquaresRgb;
                //pvav.PriceAdjustment = model.PriceAdjustment;
                //pvav.WeightAdjustment = model.WeightAdjustment;
                pvav.IsPreSelected = model.IsPreSelected;
                pvav.DisplayOrder = model.DisplayOrder;
                pvav.PopularValue = model.Popularvalue;
                if (model.Realvalue.HasValue)
                {
                    pvav.RealValue = model.Realvalue;
                    if (String.IsNullOrEmpty(model.Name))
                    {
                        pvav.Name = ((int)model.Realvalue).ToString();
                    }
                }
                _categoryProductAttributeService.UpdateCategoryProductAttributeValue(pvav);
                UpdateAttributeValueLocales(pvav, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [GridAction(EnableCustomBinding = true)]
        public ActionResult CategoryProductAttributeValueDelete(int pvavId, int categoryProductAttributeId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedView();

            var pvav = _categoryProductAttributeService.GetCategoryProductAttributeValueById(pvavId);
            if (pvav == null)
                throw new ArgumentException("No product variant attribute value found with the specified id");

            _categoryProductAttributeService.DeleteCategoryProductAttributeValue(pvav);

            return CategoryProductAttributeValueList(categoryProductAttributeId, command);
        }

        #endregion

        #region Copy attributes
        public ActionResult CopyAttributePopUp(int categoryProductAttributeId)
        {
            var categoryProductAttribute = _categoryProductAttributeService.GetCategoryProductAttributeById(categoryProductAttributeId);
            string attributeName = categoryProductAttribute.ProductAttribute.Name;
            var categoryProductAttributeGroups = _categoryProductAttributeService.GetAllCategoryProductAttributeGroups()
                .Where(x=>x.CategoryProductAttributes.Where(a=>a.ProductAttribute.Name == attributeName).FirstOrDefault() != null &&
                x.Id != categoryProductAttribute.CategoryProductGroupId);
            var model = new CategoryAttributeCopyModel()
            {
                CategoryProductAttributeId = categoryProductAttributeId
            };
            model.AviableGroups = categoryProductAttributeGroups.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.GetLocalized(c => c.Name, _workContext.WorkingLanguage.Id) }).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult CopyAttributePopUp(CategoryAttributeCopyModel model, string btnId, string formId)
        {
            var categoryProductAttributeGroup = _categoryProductAttributeService.GetCategoryProductAttributeGroupById(model.CategoryProductAttributeGroupId);
            var categoryProductAttribute = _categoryProductAttributeService.GetCategoryProductAttributeById(model.CategoryProductAttributeId);
            string attributeName = categoryProductAttribute.ProductAttribute.Name;
            var sourceCategoryProductAttribute = categoryProductAttributeGroup.CategoryProductAttributes.Where(x => x.ProductAttribute.Name == attributeName).FirstOrDefault();
            if (sourceCategoryProductAttribute == null)
            {
                ModelState.AddModelError("CategoryProductAttributeGroupId", _localizationService.GetResource(""));
            }
           
            foreach (var val in categoryProductAttribute.CategoryProductAttributeValues.ToList())
            {
                _categoryProductAttributeService.DeleteCategoryProductAttributeValue(val);
            }

            var values = categoryProductAttributeGroup.CategoryProductAttributes.Where(x => x.ProductAttribute.Name == attributeName).FirstOrDefault().CategoryProductAttributeValues;
            foreach (var val in values)
            {
                var newAttributeValue = new CategoryProductAttributeValue()
                {
                    CategoryProductAttributeId = categoryProductAttribute.Id,
                    DisplayOrder = val.DisplayOrder,
                    IsPreSelected = val.IsPreSelected,
                    Name = val.Name,
                    ColorSquaresRgb = val.ColorSquaresRgb,
                    CurrencyId = val.CurrencyId,
                    PopularValue = val.PopularValue,
                    RealValue = val.RealValue,
                    RealValueMax = val.RealValueMax,
                };
                _categoryProductAttributeService.InsertCategoryProductAttributeValue(newAttributeValue);
            }

            var categoryProductAttributeGroups = _categoryProductAttributeService.GetAllCategoryProductAttributeGroups();
            model.AviableGroups = categoryProductAttributeGroups.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.GetLocalized(c => c.Name, _workContext.WorkingLanguage.Id) }).ToList();
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }
        #endregion
    }
}
