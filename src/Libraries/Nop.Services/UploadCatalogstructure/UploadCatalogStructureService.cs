using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Services.Catalog;
using Nop.Services.Media;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Localization;
using Nop.Services.Directory;
using Nop.Services.Seo;

namespace Nop.Services.UploadCatalogstructure
{
    public class UploadCatalogStructureService : IUploadCatalogStructureService
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ICategoryProductAttributeService _categoryProductAttributeService;
        private readonly IDownloadService _downloadService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ICountryService _countryService;
        private readonly ILanguageService _languageService;
        private readonly IUrlRecordService _urlRecordService;

        public UploadCatalogStructureService(ICategoryService categoryService,
            IProductAttributeService productAttributeService,
            ICategoryProductAttributeService categoryProductAttributeService,
            IDownloadService downloadService,
            ILocalizedEntityService localizedEntityService,
            ICountryService countryService,
            ILanguageService languageService,
            IUrlRecordService urlRecordService)
        {
            this._categoryService = categoryService;
            this._productAttributeService = productAttributeService;
            this._categoryProductAttributeService = categoryProductAttributeService;
            this._downloadService = downloadService;
            this._localizedEntityService = localizedEntityService;
            this._countryService = countryService;
            this._languageService = languageService;
            this._urlRecordService = urlRecordService;
        }

        public void UploadStructure(Download download)
        {
            var productAttributeList = new List<ExportProductAttribute>();
            var productAttributeValueLists = new List<ExportProductAttributeValueList>();
            var categoryList = new List<ExportCategory>();
            var binary = download.DownloadBinary;
            if (binary.Length == 0)
                return;
            using (MemoryStream ms = new MemoryStream(binary))
            {
                ExcelPackage pck = new ExcelPackage(ms);
                
                ExportProductAttributeValueList currentAttributesValueList = null;
                var currentProductAttribute = new ExportProductAttribute();
                var rootCategory = new ExportCategory();
                var firstCategory = new ExportCategory();
                var secondCategory = new ExportCategory();
                var thirdCategory = new ExportCategory();
                int categoryIdIncrement = 1;
                int productAttributeValueListIdIncrement = 1;
                foreach (var workSheet in pck.Workbook.Worksheets)
                {
                    for (int i = 2; i <= workSheet.Dimension.End.Row - workSheet.Dimension.Start.Row + 1; i++)
                    {
                        if (workSheet.Cells[i, 1].Style.Font.Size == 20)
                        {
                            rootCategory = new ExportCategory()
                            {
                                CategoryId = categoryIdIncrement,
                                ParentCategoryId = 0,
                                Title = workSheet.GetValue<string>(i, 1).TrimEnd(),
                                TitleRu = workSheet.GetValue<string>(i, 2).TrimEnd(),
                                TitleDe = workSheet.GetValue<string>(i, 3).TrimEnd(),
                                TitleEs = workSheet.GetValue<string>(i, 4).TrimEnd()
                            };
                            firstCategory = null;
                            secondCategory = null;
                            thirdCategory = null;
                            categoryList.Add(rootCategory);
                            categoryIdIncrement++;
                        }
                        if (workSheet.Cells[i, 1].Style.Font.Size == 18)
                        {
                            firstCategory = new ExportCategory()
                            {
                                CategoryId = categoryIdIncrement,
                                ParentCategoryId = rootCategory.CategoryId,
                                Title = workSheet.GetValue<string>(i, 1).TrimEnd(),
                                TitleRu = workSheet.GetValue<string>(i, 2).TrimEnd(),
                                TitleDe = workSheet.GetValue<string>(i, 3).TrimEnd(),
                                TitleEs = workSheet.GetValue<string>(i, 4).TrimEnd()
                            };
                            if (secondCategory != null)
                            {
                                currentAttributesValueList = null;
                                secondCategory = null;
                                thirdCategory = null;
                            }
                            else
                            {
                                if (currentAttributesValueList != null)
                                {
                                    if (firstCategory.Title != "Other")
                                    {
                                        firstCategory.AttributeValueListId = currentAttributesValueList.Id;
                                    }
                                    else
                                    {
                                        firstCategory.AttributeValueListId = int.MaxValue;
                                    }
                                }
                            }
                            categoryList.Add(firstCategory);
                            categoryIdIncrement++;
                        }

                        if (workSheet.Cells[i, 1].Style.Font.Size == 16)
                        {
                            secondCategory = new ExportCategory()
                            {
                                CategoryId = categoryIdIncrement,
                                ParentCategoryId = firstCategory.CategoryId,
                                Title = workSheet.GetValue<string>(i, 1).TrimEnd(),
                                TitleRu = workSheet.GetValue<string>(i, 2).TrimEnd(),
                                TitleDe = workSheet.GetValue<string>(i, 3).TrimEnd(),
                                TitleEs = workSheet.GetValue<string>(i, 4).TrimEnd()
                            };
                            if (thirdCategory != null)
                            {
                                currentAttributesValueList = null;
                                thirdCategory = null;
                            }
                            else
                            {
                                if (currentAttributesValueList != null)
                                {
                                    if (secondCategory.Title != "Other")
                                    {
                                        secondCategory.AttributeValueListId = currentAttributesValueList.Id;
                                    }
                                    else
                                    {
                                        secondCategory.AttributeValueListId = int.MaxValue;
                                    }
                                }
                            }
                            categoryList.Add(secondCategory);
                            categoryIdIncrement++;
                        }

                        if (workSheet.Cells[i, 1].Style.Font.Size == 14)
                        {
                            thirdCategory = new ExportCategory()
                            {
                                CategoryId = categoryIdIncrement,
                                ParentCategoryId = secondCategory.CategoryId,
                                Title = workSheet.GetValue<string>(i, 1).TrimEnd(),
                                TitleRu = workSheet.GetValue<string>(i, 2).TrimEnd(),
                                TitleDe = workSheet.GetValue<string>(i, 3).TrimEnd(),
                                TitleEs = workSheet.GetValue<string>(i, 4).TrimEnd()
                            };
                            if (currentAttributesValueList != null)
                            {
                                if (thirdCategory.Title != "Other")
                                {
                                    thirdCategory.AttributeValueListId = currentAttributesValueList.Id;
                                }
                                else
                                {
                                    thirdCategory.AttributeValueListId = int.MaxValue;
                                }
                            }
                            categoryList.Add(thirdCategory);
                            if (thirdCategory.Title == "Other")
                                thirdCategory = null;
                            categoryIdIncrement++;
                        }

                        if (workSheet.Cells[i, 1].Style.Font.Size == 10 && workSheet.Cells[i, 1].Style.Font.Bold)
                        {
                            var fontSize = workSheet.Cells[i - 1, 1].Style.Font.Size;
                            if (fontSize > 10)
                            {
                                var cat = categoryList.Last();
                                var parent = categoryList.Where(x => x.CategoryId == cat.ParentCategoryId).FirstOrDefault();
                                string name = parent.Title + "-" + cat.Title;
                                string nameRu = parent.TitleRu + "-" + cat.TitleRu;
                                string nameDe = parent.TitleDe + "-" + cat.TitleDe;
                                string nameEs = parent.TitleEs + "-" + cat.TitleEs;
                                currentAttributesValueList = new ExportProductAttributeValueList()
                                {
                                    List = new List<ExportProductAttributeValue>(),
                                    Id = productAttributeValueListIdIncrement,
                                    Name = name,
                                    NameDe = nameDe,
                                    NameRu = nameRu,
                                    NameEs = nameEs
                                };
                                cat.AttributeValueListId = currentAttributesValueList.Id;
                                productAttributeValueListIdIncrement++;
                                productAttributeValueLists.Add(currentAttributesValueList);
                            }

                            var attrVal = workSheet.GetValue<string>(i, 1).TrimEnd();
                            var attr = productAttributeList.Where(x => x.Name == attrVal).FirstOrDefault();
                            if (attr == null)
                            {
                                productAttributeList.Add(new ExportProductAttribute()
                                {
                                    Name = attrVal,
                                    NameRu = workSheet.GetValue<string>(i, 2).TrimEnd(),
                                    NameDe = workSheet.GetValue<string>(i, 3).TrimEnd(),
                                    NameEs = workSheet.GetValue<string>(i, 4).TrimEnd()
                                });
                                currentProductAttribute = new ExportProductAttribute()
                                {
                                    Name = attrVal,
                                    NameRu = workSheet.GetValue<string>(i, 2).TrimEnd(),
                                    NameDe = workSheet.GetValue<string>(i, 3).TrimEnd(),
                                    NameEs = workSheet.GetValue<string>(i, 4).TrimEnd()
                                };
                            }
                            else
                            {
                                currentProductAttribute = attr;
                            }
                        }

                        if (workSheet.Cells[i, 1].Style.Font.Size == 10 && !workSheet.Cells[i, 1].Style.Font.Bold)
                        {
                            currentAttributesValueList.List.Add(new ExportProductAttributeValue()
                            {
                                AttributeTitle = currentProductAttribute.Name,
                                Title = workSheet.GetValue<string>(i, 1).TrimEnd(),
                                TitleRu = workSheet.GetValue<string>(i, 2).TrimEnd(),
                                TitleDe = workSheet.GetValue<string>(i, 3).TrimEnd(),
                                TitleEs = workSheet.GetValue<string>(i, 4).TrimEnd(),
                            });
                        }
                    }
                }

                var resetCategoryList = categoryList.Where(x => categoryList.Where(c => x.CategoryId == c.ParentCategoryId).FirstOrDefault() != null);
                foreach (var cats in resetCategoryList)
                {
                    cats.AttributeValueListId = 0;
                }
            }

            UpdateDb(categoryList, productAttributeValueLists, productAttributeList);
            _downloadService.DeleteDownload(download);
        }

        private void UpdateDb(List<ExportCategory> categoryList, List<ExportProductAttributeValueList> productAttributeValueLists, List<ExportProductAttribute> productAttributeList)
        {
            foreach (var attr in productAttributeList)
            {
                var productAttribute = new ProductAttribute()
                {
                    Name = attr.Name
                };
                _productAttributeService.InsertProductAttribute(productAttribute);
                _localizedEntityService.SaveLocalizedValue(productAttribute, x => x.Name, attr.Name, 1);
                _localizedEntityService.SaveLocalizedValue(productAttribute, x => x.Name, attr.NameRu, 2);
                _localizedEntityService.SaveLocalizedValue(productAttribute, x => x.Name, attr.NameDe, 3);
                _localizedEntityService.SaveLocalizedValue(productAttribute, x => x.Name, attr.NameEs, 4);
                attr.RecievedId = productAttribute.Id;
            }

            foreach (var attributeGroup in productAttributeValueLists)
            {
                var group = new CategoryProductAttributeGroup()
                {
                    Name = attributeGroup.Name
                };

                _categoryProductAttributeService.InsertCategoryProductAttributeGroup(group);
                _localizedEntityService.SaveLocalizedValue(group, x => x.Name, attributeGroup.Name, 1);
                _localizedEntityService.SaveLocalizedValue(group, x => x.Name, attributeGroup.NameRu, 2);
                _localizedEntityService.SaveLocalizedValue(group, x => x.Name, attributeGroup.NameDe, 3);
                _localizedEntityService.SaveLocalizedValue(group, x => x.Name, attributeGroup.NameEs, 4);
                attributeGroup.RecievedId = group.Id;
                var attrs = attributeGroup.List.GroupBy(x => x.AttributeTitle);
                foreach (var attr in attrs)
                {
                    int productAttributeId = productAttributeList.Where(x => x.Name == attr.Key).First().RecievedId;
                    var categoryProductAttribute = new CategoryProductAttribute()
                    {
                        AttributeControlTypeId = (int)AttributeControlType.Checkboxes,
                        CategoryProductGroupId = group.Id,
                        DisplayOrder = 0,
                        IsRequired = false,
                        ProductAttributeId = productAttributeId
                    };

                    _categoryProductAttributeService.InsertCategoryProductAttribute(categoryProductAttribute);
                    foreach (var value in attr)
                    {
                        var categoryProductAttributeValue = new CategoryProductAttributeValue()
                        {
                            DisplayOrder = 0,
                            CategoryProductAttributeId = categoryProductAttribute.Id,
                            Name = value.Title,
                            IsPreSelected = false
                        };
                        _categoryProductAttributeService.InsertCategoryProductAttributeValue(categoryProductAttributeValue);
                        _localizedEntityService.SaveLocalizedValue(categoryProductAttributeValue, x => x.Name, value.Title, 1);
                        _localizedEntityService.SaveLocalizedValue(categoryProductAttributeValue, x => x.Name, value.TitleRu, 2);
                        _localizedEntityService.SaveLocalizedValue(categoryProductAttributeValue, x => x.Name, value.TitleDe, 3);
                        _localizedEntityService.SaveLocalizedValue(categoryProductAttributeValue, x => x.Name, value.TitleEs, 4);
                    }
                }
            }

            foreach (var category in categoryList.Where(x=>x.ParentCategoryId == 0))
            {
                InsertChildCategories(category, categoryList, productAttributeValueLists);
            }
        }

        private void InsertChildCategories(ExportCategory category, List<ExportCategory> categoryList,List<ExportProductAttributeValueList> productAttributeValueLists)
        {
            InsertCategory(category, categoryList, productAttributeValueLists);
            var childCategories = categoryList.Where(x => x.ParentCategoryId == category.CategoryId).ToList();
            foreach(var childCategory in childCategories)
            {
                InsertChildCategories(childCategory, categoryList, productAttributeValueLists);
            }
        }

        private void InsertCategory(ExportCategory category, List<ExportCategory> categoryList,List<ExportProductAttributeValueList> productAttributeValueLists)
        {
            int parentCategoryId = 0;
            if (category.ParentCategoryId != 0)
            {
                parentCategoryId = categoryList.Where(x => x.CategoryId == category.ParentCategoryId).FirstOrDefault().RecievedId;
            }
            int groupId = 0;
            if (category.AttributeValueListId != 0 && category.AttributeValueListId != int.MaxValue)
            {
                groupId = productAttributeValueLists.Where(x => x.Id == category.AttributeValueListId).First().RecievedId;
            }
            var newCategory = new Category()
            {
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                Published = true,
                DisplayOrder = 0,
                Name = category.Title,
                ParentCategoryId = parentCategoryId,
            };
            _categoryService.InsertCategory(newCategory);
            _localizedEntityService.SaveLocalizedValue(newCategory, x => x.Name, category.Title, 1);
            _localizedEntityService.SaveLocalizedValue(newCategory, x => x.Name, category.TitleRu, 2);
            _localizedEntityService.SaveLocalizedValue(newCategory, x => x.Name, category.TitleDe, 3);
            _localizedEntityService.SaveLocalizedValue(newCategory, x => x.Name, category.TitleEs, 4);

            category.RecievedId = newCategory.Id;

            if (groupId != 0)
            {
                var categoryToCategoryAttributeGroup = new CategoryToCategoryProductAttributeGroup()
                {
                    CategoryId = category.RecievedId,
                    CategoryProductAttributeGroupId = groupId,
                    DisplayOrder = 0
                };
                _categoryProductAttributeService.InsertCategoryToCategoryProductAttributeGroup(categoryToCategoryAttributeGroup);
            }
        }

        public void UpdateCountriies(Download download)
        {
            var binary = download.DownloadBinary;
            if (binary.Length == 0)
                return;
            using (MemoryStream ms = new MemoryStream(binary))
            {
                ExcelPackage pck = new ExcelPackage(ms);
                var workSheet = pck.Workbook.Worksheets["countries"];
                for (int i = 2; i <= workSheet.Dimension.End.Row - workSheet.Dimension.Start.Row + 1; i++)
                {
                    int id = workSheet.GetValue<int>(i, 1);
                    var country = _countryService.GetCountryById(id);
                    if (country != null)
                    {
                        _localizedEntityService.SaveLocalizedValue(country, x => x.Name, workSheet.GetValue<string>(i, 2), 1);
                        _localizedEntityService.SaveLocalizedValue(country, x => x.Name, workSheet.GetValue<string>(i, 3), 2);
                        _localizedEntityService.SaveLocalizedValue(country, x => x.Name, workSheet.GetValue<string>(i, 4), 3);
                        _localizedEntityService.SaveLocalizedValue(country, x => x.Name, workSheet.GetValue<string>(i, 5), 4);
                    }
                }
            }
        }

        public void ProcessCategoriesUrls()
        {
            var categories = _categoryService.GetAllCategories();
            foreach (var category in categories)
            {
                foreach (var lang in _languageService.GetAllLanguages())
                {
                    string name = category.GetLocalized(x => x.Name, lang.Id, false);
                    if (name != null)
                    {
                        string SeName = category.ValidateSeName(name, null, true);
                        _urlRecordService.SaveSlug(category, SeName, lang.Id);
                    }
                }
            }
        }
    }
}
