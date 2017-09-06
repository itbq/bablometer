using Nop.Core;
using Nop.Services.Catalog;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Localization;
using Nop.Core.Domain.Catalog;
using Nop.Services.Media;
using OfficeOpenXml.Drawing;
using Nop.Core.Domain.Media;
using Nop.Services.Directory;
using Nop.Core.Domain.BrandDomain;
using Nop.Services.Seo;
using Ionic.Zip;
using System.Collections.Specialized;
using System.Xml.Linq;
using Nop.Services.Messages;

namespace Nop.Services.ExportImport
{
    public class CatalogExportManager : ICatalogExportManager
    {
        private readonly ICategoryProductAttributeService _categoryProductAttributeService;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IPictureService _pictureService;
        private readonly ILanguageService _languageService;
        private readonly IDownloadService _downloadService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizationEntityService;
        private readonly IProductPriceService _productPriceService;
        private readonly ICurrencyService _currencyService;
        private readonly IBrandService _brandService;
        private readonly IProductTagService _productTagService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IUploadCatalogEmailService _uploadCatalogEmailService;

        private int codePage;


        public CatalogExportManager(ICategoryProductAttributeService categoryProductAttributeService,
            IProductService productService,
            IWorkContext workContext,
            IPictureService pictureService,
            ILanguageService languageService,
            IDownloadService downloadService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizationEntityService,
            IProductPriceService productPriceService,
            ICurrencyService currencyService,
            IBrandService brandService,
            IProductTagService productTagService,
            IUrlRecordService urlRecordService,
            IUploadCatalogEmailService uploadCatalogEmailService)
        {
            this._categoryProductAttributeService = categoryProductAttributeService;
            this._productService = productService;
            this._workContext = workContext;
            this._pictureService = pictureService;
            this._languageService = languageService;
            this._downloadService = downloadService;
            this._localizationService = localizationService;
            this._localizationEntityService = localizationEntityService;
            this._productPriceService = productPriceService;
            this._currencyService = currencyService;
            this._brandService = brandService;
            this._productTagService = productTagService;
            this._urlRecordService = urlRecordService;
            this._uploadCatalogEmailService = uploadCatalogEmailService;
        }


        public string GenerateExcel(int categoryId, Stream stream, int languageId)
        {
            List<ExportCategoryAttributeModel> attributes = new List<ExportCategoryAttributeModel>();
            var categoryAttributeGroup = _categoryProductAttributeService.GetCategoryToCategoryProductAttributeGroupByCategoryId(categoryId);
            //if (categoryAttributeGroup.Count == 0)
            //{
            //    return null;
            //}
            foreach (var grp in categoryAttributeGroup)
            {
                attributes.AddRange(grp.CategoryProductAttributeGroup.CategoryProductAttributes.Select(x => new ExportCategoryAttributeModel()
                {
                    ControlType = x.AttributeControlType,
                    Name = x.ProductAttribute.GetLocalized(a=>a.Name,languageId),
                    Values = x.CategoryProductAttributeValues.OrderBy(y => y.DisplayOrder).ToList(),
                    Id = x.Id,
                    IsRequired = x.IsRequired
                }).ToList());
            }
            
            attributes = attributes.GroupBy(x => x.Name).Select(y => y.First()).ToList();
            string path = GenerateExcel(attributes,categoryId, stream,languageId);
            return null;
        }

        //private string GenerateExcel(List<ExportCategoryAttributeModel> attributes, string path, int categoryId)
        //{
        //    path = Path.Combine(path, "template.xlsm");
        //    string moduleCode = MODULE_CODE;
        //    string workbookCode = WORKBOOK_OPEN_WRAPPER;
        //    string worksheetCode = SELECTION_CHANGE_WRAPPER;
        //    var selectionCode = new StringBuilder();
        //    var workbookCodeN = new StringBuilder();
        //    var eventsCode = new StringBuilder();
        //    var itemsCode = new StringBuilder();
        //    ExcelPackage pck = new ExcelPackage();
        //    int i = 0;
        //    var ws = pck.Workbook.Worksheets.Add("Product");
        //    var propertis = new List<string>()
        //    {
        //        "Entered",
        //        "Id",
        //        "Language",
        //        "Title",
        //        "ShortDescription",
        //        "FullDescription",
        //        "OrderingComment",
        //    };
        //    for (i = 0; i < propertis.Count; i++)
        //    {
        //        ws.Cells[1, i + 1].Value = propertis[i];
        //    }
        //    ws.Column(1).Width = Pixel2ColumnWidth(ws, 25);
        //    ws.Column(2).Width = Pixel2ColumnWidth(ws, 30);
        //    ws.Column(3).Width = Pixel2ColumnWidth(ws, 75);
        //    ws.Column(4).Width = Pixel2ColumnWidth(ws, 250);
        //    ws.Column(5).Width = Pixel2ColumnWidth(ws, 250);
        //    ws.Column(6).Width = Pixel2ColumnWidth(ws, 250);
        //    ws.Column(7).Width = Pixel2ColumnWidth(ws, 250);
        //    for (int j = 0; j < 12; j += 2)
        //    {
        //        ws.Cells[1, i + 1 + j].Value = String.Format("Picture{0}", (int)j / 2 + 1);
        //        ws.Column(i + 2 + j).Width = 0;
        //        ws.Column(i + 1 + j).Width = Pixel2ColumnWidth(ws, 100);
        //    }

        //    i += 12;
        //    foreach (var attribute in attributes)
        //    {
        //        ws.Cells[1, i + 1].Value = attribute.Name;
        //        ws.Cells[1, i + 2].Value = attribute.Id;
        //        var column = ws.Column(i + 2);
        //        column.Width = 0;
        //        ws.Column(i + 1).Width = Pixel2ColumnWidth(ws, 100);
        //        switch (attribute.ControlType)
        //        {
        //            case Core.Domain.Catalog.AttributeControlType.DropdownList:
        //            case Core.Domain.Catalog.AttributeControlType.RadioList:
        //                {
        //                    selectionCode.Append(String.Format(COMBOBOX_SELECTION_CHANGE_CODE_N, i + 1));
        //                    foreach (var attr in attribute.Values)
        //                    {
        //                        itemsCode.AppendLine(String.Format(ITEM_ADD_CODE, attr.Name));
        //                    }

        //                    workbookCodeN.Append(String.Format(COMBOBOX_WORKBOOK_OPEN_CODE_N, i + 1, itemsCode.ToString()));
        //                    itemsCode.Clear();
        //                    eventsCode.Append(String.Format(COMBOBOX_CHANGE_N, i + 1));
        //                    break;
        //                }
        //            case Core.Domain.Catalog.AttributeControlType.ColorSquares:
        //            case Core.Domain.Catalog.AttributeControlType.Checkboxes:
        //            case Core.Domain.Catalog.AttributeControlType.SizePicker:
        //                {
        //                    selectionCode.Append(String.Format(LISTBOX_SELECTION_CHANGE_CODE_N, i + 1));
        //                    foreach (var attr in attribute.Values)
        //                    {
        //                        itemsCode.AppendLine(String.Format(ITEM_ADD_CODE, attr.Name));
        //                    }
        //                    workbookCodeN.Append(String.Format(LISTBOX_WORKBOOK_OPEN_CODE_N, i + 1, itemsCode.ToString()));
        //                    eventsCode.Append(String.Format(LISTBOX_CHANGE_N, i + 1));
        //                    itemsCode.Clear();
        //                    break;
        //                }
        //        }
        //        i += 2;
        //    }
        //    ws.Workbook.CreateVBAProject();
        //    var module = ws.Workbook.VbaProject.Modules.AddModule("Module1");

        //    workbookCode = String.Format(workbookCode, workbookCodeN);
        //    ws.Workbook.CodeModule.Code = workbookCode;
        //    selectionCode.Append(PICTURE_SELECTION_CHANGE_PART);
        //    ws.CodeModule.Code = String.Format(worksheetCode, selectionCode, eventsCode);
        //    StringBuilder pictureHideCode = new StringBuilder();
        //    FillExcellWithProducts(ws, pictureHideCode, categoryId, _workContext.WorkingLanguage.Id);
        //    module.Code = String.Format(moduleCode, pictureHideCode);
        //    FileStream str = File.Create(path);
        //    pck.SaveAs(str);
        //    str.Close();

        //    return null;
        //}

        //private void FillExcellWithProducts(ExcelWorksheet workSheet, StringBuilder pictureHideCode, int categoryId, int languageId)
        //{
        //    var products = _productService.NewSearchProducts(_workContext.CurrentCustomer.Id, 0, 0, Core.Domain.Catalog.ProductSortingEnum.CreatedOn, 0, 1000, 0, categoryId, 0);
        //    for (int i = 0; i < products.Count; i++)
        //    {
        //        var languageList = _languageService.GetAllLanguages();
        //        workSheet.Row(i * languageList.Count + 2).Height = Pixel2RowHeight(100);
        //        workSheet.Cells[i * languageList.Count + 2, 1].Value = "$";
        //        workSheet.Cells[i * languageList.Count + 2, 2].Value = products[i].Id;

        //        for (int j = 0; j < languageList.Count; j++)
        //        {
        //            workSheet.Row(i * languageList.Count + j + 2).Height = Pixel2RowHeight(100);
        //            workSheet.Cells[i * languageList.Count + j + 2, 3].Value = languageList[j].Name;
        //            workSheet.Cells[i * languageList.Count + j + 2, 4].Value = products[i].GetLocalized(x => x.Name, languageList[j].Id, false);
        //            workSheet.Cells[i * languageList.Count + j + 2, 4].Style.WrapText = true;
        //            workSheet.Cells[i * languageList.Count + j + 2, 5].Value = products[i].GetLocalized(x => x.ShortDescription, languageList[j].Id, false);
        //            workSheet.Cells[i * languageList.Count + j + 2, 5].Style.WrapText = true;
        //            workSheet.Cells[i * languageList.Count + j + 2, 6].Value = products[i].GetLocalized(x => x.FullDescription, languageList[j].Id, false);
        //            workSheet.Cells[i * languageList.Count + j + 2, 6].Style.WrapText = true;
        //            workSheet.Cells[i * languageList.Count + j + 2, 7].Value = products[i].GetLocalized(x => x.AdminComment, languageList[j].Id, false);
        //            workSheet.Cells[i * languageList.Count + j + 2, 7].Style.WrapText = true;
        //        }
        //        if (products[i].ProductPictures.Count > 0)
        //        {
        //            ProcessPictures(products[i], i * languageList.Count + 2, workSheet, pictureHideCode);
        //        }
        //    }
        //}

        //private void ProcessPictures(Product product, int index, ExcelWorksheet worksheet, StringBuilder pictureHideCode)
        //{
        //    ExcelPicture picture, pictureSmall;
        //    int firstPicColumn = 7;
        //    var defaultPic = product.ProductPictures.Where(x => x.DisplayOrder == 0).FirstOrDefault();
        //    if (defaultPic != null)
        //    {
        //        System.Drawing.Image img;
        //        var bytes = _pictureService.LoadPictureBinary(defaultPic.Picture);
        //        using (MemoryStream ms = new MemoryStream(bytes))
        //        {
        //            img = System.Drawing.Image.FromStream(ms);
        //            picture = worksheet.Drawings.AddPicture(String.Format("{0}{1}", index, firstPicColumn), img);
        //            pictureHideCode.Append(String.Format(PICTURE_HIDE_CODE_N, String.Format("{0}{1}", index, firstPicColumn)));
        //            pictureSmall = worksheet.Drawings.AddPicture(Guid.NewGuid().ToString(), img);
        //            pictureSmall.SetPosition(index - 1, 0, firstPicColumn - 1, 0);
        //            pictureSmall.SetSize((int)Pixel2ColumnWidth(worksheet, (short)worksheet.Column(firstPicColumn).Width) / 2, (int)worksheet.Row(index).Height / 2);
        //            ms.Close();
        //        }
        //    }
        //    var list = product.ProductPictures.ToList();
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        if (list[i].DisplayOrder == 0)
        //            continue;
        //        System.Drawing.Image img;
        //        var bytes = _pictureService.LoadPictureBinary(list[i].Picture);
        //        using (MemoryStream ms = new MemoryStream(bytes))
        //        {
        //            img = System.Drawing.Image.FromStream(ms);

        //            picture = worksheet.Drawings.AddPicture(String.Format("{0}{1}", index, firstPicColumn + 2 + 2 * i), img);
        //            pictureHideCode.Append(String.Format(PICTURE_HIDE_CODE_N, String.Format("{0}{1}", index, firstPicColumn + 2 + 2 * i)));
        //            pictureSmall = worksheet.Drawings.AddPicture(Guid.NewGuid().ToString(), img);
        //            pictureSmall.SetPosition(index - 1, 0, firstPicColumn + 2 + 2 * i - 1, 0);
        //            pictureSmall.SetSize((int)Pixel2ColumnWidth(worksheet, (short)worksheet.Column(firstPicColumn + 2 + 2 * i).Width) / 2, (int)worksheet.Row(index).Height / 2);
        //            ms.Close();
        //        }
        //    }
        //}

        private string GenerateExcel(List<ExportCategoryAttributeModel> attributes, int categoryId, Stream stream, int languageId)
        {
            codePage = 1252;
            if (_workContext.WorkingLanguage.LanguageCulture == "ru-RU")
                codePage = 1251;
            string moduleCode = MODULE_CODE;
            string workbookCode = WORKBOOK_OPEN_WRAPPER;
            string worksheetCode = SELECTION_CHANGE_WRAPPER;
            var attriibuteValidationCode = new StringBuilder();
            var checkBoxResetCode = new StringBuilder();
            var selectionCode = new StringBuilder();
            var workbookCodeN = new StringBuilder();
            var eventsCode = new StringBuilder();
            var itemsCode = new StringBuilder();
            ExcelPackage pck = new ExcelPackage(stream);
            int i = 0;
            var ws = pck.Workbook.Worksheets.Add("Product");
            var propertis = new List<string>()
            {
                "Entered",
                _localizationService.GetResource("ETF.Product.Title"),
                _localizationService.GetResource("ETF.Product.Short"),
                _localizationService.GetResource("ETF.Product.Full"),
                _localizationService.GetResource("ETF.Product.Comment"),
                _localizationService.GetResource("Profile.Catalog.Brand",languageId),
                _localizationService.GetResource("ETF.Front.Product.Details.Keywords",languageId)
            };
            for (i = 0; i < propertis.Count; i++)
            {
                ws.Cells[1, i + 1].Value = propertis[i];
            }
            ws.Column(1).Width = Pixel2ColumnWidth(ws, 25);
            ws.Column(1).Style.WrapText = true;
            ws.Column(2).Width = Pixel2ColumnWidth(ws, 250);
            ws.Column(2).Style.WrapText = true;
            ws.Column(3).Width = Pixel2ColumnWidth(ws, 250);
            ws.Column(3).Style.WrapText = true;
            ws.Column(4).Width = Pixel2ColumnWidth(ws, 250);
            ws.Column(4).Style.WrapText = true;
            ws.Column(5).Width = Pixel2ColumnWidth(ws, 250);
            ws.Column(5).Style.WrapText = true;
            ws.Column(6).Width = Pixel2ColumnWidth(ws, 250);
            ws.Column(6).Style.WrapText = true;
            ws.Column(7).Width = Pixel2ColumnWidth(ws, 250);
            ws.Column(7).Style.WrapText = true;
            ws.Column(7).Width = Pixel2ColumnWidth(ws, 250);

            for (int j = 0; j < 12; j += 2)
            {
                ws.Cells[1, i + 1 + j].Value = String.Format("Picture{0}", (int)j / 2 + 1);
                ws.Cells[1, i + 2 + j].Value = categoryId;
                ws.Column(i + 2 + j).Width = 0;
                ws.Column(i + 1 + j).Width = Pixel2ColumnWidth(ws, 100);
            }

            i += 12;
            var currencies = _currencyService.GetAllCurrencies();
            foreach (var currency in currencies)
            {
                ws.Cells[1, i + 1].Value = currency.CurrencyCode;
                i++;
            }

            foreach (var attribute in attributes)
            {
                if (attribute.IsRequired)
                {
                    attriibuteValidationCode.Append(String.Format(VALIDATION_CODE, i + 1));
                }
                ws.Cells[1, i + 1].Value = attribute.Name;
                ws.Cells[1, i + 2].Value = attribute.Id;
                var column = ws.Column(i + 2);
                column.Width = 0;
                ws.Column(i + 1).Width = Pixel2ColumnWidth(ws, 150);
                ws.Column(i + 1).Style.WrapText = true;
                switch (attribute.ControlType)
                {
                    case Core.Domain.Catalog.AttributeControlType.DropdownList:
                        {
                            checkBoxResetCode.Append(String.Format(CHECK_BOX_ENABLE_CODE,i + 1));
                            selectionCode.Append(String.Format(COMBOBOX_SELECTION_CHANGE_CODE_N, i + 1));
                            foreach (var attr in attribute.Values)
                            {
                                itemsCode.AppendLine(String.Format(ITEM_ADD_CODE, ConvertString(codePage,attr.GetLocalized(x=>x.Name,languageId).Replace("\"",""))));
                            }

                            workbookCodeN.Append(String.Format(COMBOBOX_WORKBOOK_OPEN_CODE_N, i + 1, itemsCode.ToString()));
                            itemsCode.Clear();
                            eventsCode.Append(String.Format(COMBOBOX_CHANGE_N, i + 1));
                            break;
                        }
                    case Core.Domain.Catalog.AttributeControlType.Checkboxes:
                        {
                            checkBoxResetCode.Append(String.Format(CHECK_BOX_ENABLE_CODE, i + 1));
                            selectionCode.Append(String.Format(LISTBOX_SELECTION_CHANGE_CODE_N, i + 1));
                            foreach (var attr in attribute.Values)
                            {
                                itemsCode.AppendLine(String.Format(ITEM_ADD_CODE, ConvertString(codePage, attr.GetLocalized(x => x.Name, languageId).Replace("\"", ""))));
                            }
                            workbookCodeN.Append(String.Format(LISTBOX_WORKBOOK_OPEN_CODE_N, i + 1, itemsCode.ToString()));
                            eventsCode.Append(String.Format(LISTBOX_CHANGE_N, i + 1));
                            itemsCode.Clear();
                            break;
                        }
                    case AttributeControlType.TextBox:
                        {

                            break;
                        }
                }
                workbookCodeN.Append(String.Format(HIDE_COLUMN, i + 2));
                i += 2;
            }
            for (int k = 2; k <= 100; k++)
            {
                ws.Row(k).Height = Pixel2RowHeight(100);
            }
            ws.Workbook.CreateVBAProject();
            var module = ws.Workbook.VbaProject.Modules.AddModule("Module1");

            workbookCode = String.Format(workbookCode, workbookCodeN);
            ws.Workbook.CodeModule.Code = workbookCode;
            selectionCode.Append(PICTURE_SELECTION_CHANGE_PART);
            ws.CodeModule.Code = String.Format(worksheetCode, selectionCode, eventsCode);
            StringBuilder pictureHideCode = new StringBuilder();
            //FillExcellWithProducts(ws, pictureHideCode, categoryId, _workContext.WorkingLanguage.Id);
            module.Code = String.Format(moduleCode, "", attriibuteValidationCode.ToString(),checkBoxResetCode.ToString());
            pck.Save();
            return null;
        }

        public void ImportExcelFile(Download file, int customerId, int categoryId, int productItemType, IList<int> languages, string path, int languageid)
        {
            int count = 0;
            var notUploadedList = new Dictionary<int,string>();
            path = path + @"/" + Guid.NewGuid().ToString();

            var binary = file.DownloadBinary;
            if (binary.Length == 0)
                return;
            using (MemoryStream mstemp = new MemoryStream(binary))
            {
                ExcelPackage pck = new ExcelPackage(mstemp);
                var ws = pck.Workbook.Worksheets["Product"];
                int catId = ws.GetValue<int>(1,9);
                if (catId != categoryId)
                {
                    notUploadedList.Add(0, _localizationService.GetResource("ETF.Upload.Category.Error",languageid,false));
                    _uploadCatalogEmailService.SendUploadCatalogEmail(count, categoryId, languageid, customerId, notUploadedList);
                    return;
                }
            }
            ImportPictures(file, path);
            using (MemoryStream ms = new MemoryStream(binary))
            {
                ExcelPackage pck = new ExcelPackage(ms);
                var ws = pck.Workbook.Worksheets["Product"];
                for (int i = 1; i <= ws.Dimension.End.Row - ws.Dimension.Start.Row + 1; i++)
                {
                    int columnPosition = 2;
                    var product = new Product();
                    var val = ws.GetValue<string>(i + 1, 1);
                    if (ws.GetValue<string>(i + 1, 1) != "$$")
                    {
                        continue;
                    }
                    string title = ws.GetValue<string>(i + 1, 2);
                    if (title != null)
                        title = title.Trim();
                    columnPosition++;
                    string shortDescription = ws.GetValue<string>(i + 1, 3);
                    if (shortDescription != null)
                        shortDescription = shortDescription.Trim();
                    columnPosition++;
                    string fullDescription = ws.GetValue<string>(i + 1, 4);
                    if (fullDescription != null)
                        fullDescription = fullDescription.Trim();
                    columnPosition++;
                    string orderingComment = ws.GetValue<string>(i + 1, 5);
                    if (orderingComment != null)
                        orderingComment = orderingComment.Trim();
                    columnPosition++;
                    string brand = ws.GetValue<string>(i + 1, columnPosition);
                    if (brand != null)
                        brand = brand.Trim();
                    columnPosition++;
                    string productTags = ws.GetValue<string>(i + 1, columnPosition);
                    if (productTags != null)
                        productTags.Trim();
                    columnPosition++;
                    if (String.IsNullOrEmpty(title))
                    {
                        notUploadedList.Add(i + 1, _localizationService.GetResource("ETF.Product.Title", languageid, false) + " " + _localizationService.GetResource("ETF.Excel.RequiredField.Prompt.Message", languageid, false));
                        continue;
                    }

                    if (String.IsNullOrEmpty(shortDescription))
                    {
                        notUploadedList.Add(i + 1, _localizationService.GetResource("ETF.Product.Short", languageid, false) + " " + _localizationService.GetResource("ETF.Excel.RequiredField.Prompt.Message", languageid, false));
                        continue;
                    }

                    if (String.IsNullOrEmpty(fullDescription))
                    {
                        notUploadedList.Add(i + 1, _localizationService.GetResource("ETF.Product.Full", languageid, false) + " " + _localizationService.GetResource("ETF.Excel.RequiredField.Prompt.Message", languageid, false));
                        continue;
                    }

                    if (String.IsNullOrEmpty(brand))
                    {
                        notUploadedList.Add(i + 1, _localizationService.GetResource("ETF.Product.Brand", languageid, false) + " " + _localizationService.GetResource("ETF.Excel.RequiredField.Prompt.Message", languageid, false));
                        continue;
                    }
                    product.Name = title;
                    product.ShortDescription = shortDescription;
                    product.FullDescription = "<p>" + fullDescription + "</p>";
                    product.CustomerId = customerId;
                    product.AdminComment = "<p>" + orderingComment + "</p>";
                    product.Published = true;
                    product.Deleted = false;
                    product.UpdatedOnUtc = DateTime.UtcNow;
                    product.CreatedOnUtc = DateTime.UtcNow;
                    columnPosition += 12;
                    var atributes =new List<ExportCategoryAttributeValueModel>();
                    var currencies = _currencyService.GetAllCurrencies();
                    var prices = new List<ProductPrice>();
                    for (int m = 0; m < currencies.Count; m++)
                    {
                        var code = ws.GetValue<string>(1, columnPosition);
                        var currency = currencies.Where(x => x.CurrencyCode == code).FirstOrDefault();
                        if (currency == null)
                        {
                            continue;
                        }
                        var price = ws.GetValue<decimal>(i + 1, columnPosition);
                        var productPrice = new ProductPrice()
                        {
                            Currency = currency,
                            Price = price
                        };
                        prices.Add(productPrice);
                        columnPosition++;
                    }
                    if (!CheckAttributes(ws, categoryId,atributes, i + 1,columnPosition,languageid))
                    {
                        notUploadedList.Add(i + 1, _localizationService.GetResource("Export.Attributes.Error", languageid, false));
                        continue;
                    }
                    //save product
                    _productService.InsertProduct(product);
                    
                    prices = prices.Select(x => { x.ProductId = product.Id; return x; }).ToList();
                    product.ProductCategories.Add(new ProductCategory()
                    {
                        CategoryId = categoryId,
                        ProductId = product.Id
                    });
                    _productService.UpdateProduct(product);
                    foreach (var price in prices)
                    {
                        _productPriceService.InsertProductPrice(price);
                    }
                    //save attribute values
                    foreach (var attribute in atributes)
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

                    if (!String.IsNullOrEmpty(productTags))
                    {
                        foreach (var lang in languages)
                        {
                            string[] tags = productTags.Split(new char[] { ',' });
                            foreach (var tag in tags)
                            {
                                if (String.IsNullOrEmpty(tag) || String.IsNullOrWhiteSpace(tag))
                                {
                                    continue;
                                }
                                var tagEntity = _productTagService.GetAllProductTags().Where(x => x.Name == tag).FirstOrDefault();
                                if (tagEntity != null)
                                {
                                    //tagEntity.ProductCount++;
                                    product.ProductTags.Add(tagEntity);
                                    _productService.UpdateProduct(product);
                                    _productTagService.UpdateProductTagTotals(tagEntity);
                                }
                                else
                                {
                                    var productTag = new ProductTag()
                                    {
                                        Name = tag,
                                        ProductCount = 1,
                                    };
                                    _productTagService.InsertProductTag(productTag);
                                    product.ProductTags.Add(productTag);
                                    _productService.UpdateProduct(product);
                                    _productTagService.UpdateProductTagTotals(productTag);
                                }

                            }
                        }
                    }

                    foreach (var lang in languages)
                    {
                        var seName = product.ValidateSeName(product.Name, product.Name, true);
                        _urlRecordService.SaveSlug(product, seName, lang);
                        //_localizationService.sa
                        _localizationEntityService.SaveLocalizedValue(product, x => x.Name, product.Name, lang);
                        _localizationEntityService.SaveLocalizedValue(product, x => x.ShortDescription, product.ShortDescription, lang);
                        _localizationEntityService.SaveLocalizedValue(product, x => x.FullDescription, product.FullDescription, lang);
                        if(product.AdminComment != null)
                            _localizationEntityService.SaveLocalizedValue(product, x => x.AdminComment, product.AdminComment, lang);
                    }

                    AttachPictures(i + 1, path, product);
                    columnPosition = 2;
                    count++;
                    
                }
                _downloadService.DeleteDownload(file);
                System.IO.Directory.Delete(path, true);
            }

            _uploadCatalogEmailService.SendUploadCatalogEmail(count, categoryId, languageid, customerId,notUploadedList);
        }

        private void AttachPictures(int productNumber, string path, Product product)
        {
            string[] files = System.IO.Directory.GetFiles(path + @"\picsFormatted");
            for(int i = 0; i < files.Length; i++)
            {
                var fi = new FileInfo(files[i]);
                string productNum = fi.Name.Substring(0,productNumber.ToString().Length);
                if(productNum != productNumber.ToString())
                    continue;
                string number = fi.Name.Substring(productNumber.ToString().Length, fi.Name.Length - productNumber.ToString().Length);
                var picture = new Picture();
                using (var stream = fi.OpenRead())
                {
                    picture.PictureBinary = new byte[stream.Length];
                    stream.Read(picture.PictureBinary, 0, (int)stream.Length);
                    stream.Close();
                }
                if (fi.Extension == "gif")
                {
                    picture.MimeType = "image/gif";
                }
                else
                {
                    picture.MimeType = "image/jpeg";
                }
                picture = _pictureService.InsertPicture(picture.PictureBinary,picture.MimeType,"",true);
                if (number == "8")
                {
                    product.ProductPictures.Add(new ProductPicture()
                    {
                        PictureId = picture.Id,
                        ProductId = product.Id,
                        DisplayOrder = 0
                    });
                }
                else
                {
                    product.ProductPictures.Add(new ProductPicture()
                    {
                        PictureId = picture.Id,
                        ProductId = product.Id,
                        DisplayOrder = 1
                    });
                }
            }
            _productService.UpdateProduct(product);
        }

        private void ImportPictures(Download download, string path)
        {
            var binary = download.DownloadBinary;
            if (binary.Length == 0)
                return;
            string unpackDirectory = path;
            using (MemoryStream ms = new MemoryStream(binary))
            {
                var guid = Guid.NewGuid();
                using (ZipFile zip1 = ZipFile.Read(ms))
                {
                    foreach (ZipEntry e in zip1)
                    {
                        e.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }
            var picDir = System.IO.Directory.CreateDirectory(unpackDirectory + @"\picsFormatted");
            var indexesAndLinks = new NameValueCollection();
            var files = System.IO.Directory.GetFiles(unpackDirectory + @"\xl\drawings");
            var relsDirectory = unpackDirectory + @"\xl\drawings\_rels";

            foreach (var file in files)
            {
                int result = 0;
                XDocument doc = XDocument.Load(file);
                var pics = from z in doc.Descendants()
                           where z.Name.LocalName == "pic"
                           select z;
                var filteredPics = pics.Where(x => x.Descendants()
                                                .Where(z => z.Name.LocalName == "cNvPr" && (z.Attribute("name") != null && int.TryParse(z.Attribute("name").Value,out result)))
                                                .Count() > 0);
                var linkNumbers = pics.Where(x => x.Descendants()
                    .Where(z => z.Name.LocalName == "cNvPr" && (z.Attribute("name") != null))
                    .Where(z => z.Attribute("name").Value.IndexOf("_l") > 0 && int.TryParse(z.Attribute("name").Value.Replace("_l",""), out result)).Count() > 0);
                filteredPics = (from p in filteredPics
                               join l in linkNumbers on p.Descendants().Where(x => x.Name.LocalName == "cNvPr").Attributes("name").First().Value equals l.Descendants().Where(x => x.Name.LocalName == "cNvPr").Attributes("name").First().Value.Replace("_l","")
                               select p).Distinct();
                
                foreach (var pic in filteredPics)
                {
                    string index = pic.Descendants().Where(x => x.Name.LocalName == "cNvPr").Attributes("name").First().Value;
                    var relation = pic.Descendants().Where(x => x.Name.LocalName == "blip").First().Attributes().Where(x=>x.Value[0] == 'r').First().Value;
                    var relationDoc = XDocument.Load(relsDirectory + @"\" + (new FileInfo(file)).Name + ".rels");
                    var fileName = relationDoc.Root.Descendants()
                        .Where(x => x.Name.LocalName == "Relationship" && x.Attribute("Id").Value == relation)
                        .First()
                        .Attribute("Target").Value;
                    int indexTemp = fileName.LastIndexOf(@"/");
                    fileName = fileName.Substring(indexTemp + 1, fileName.Length - indexTemp - 1);
                    File.Copy(unpackDirectory + @"\xl\media\" + fileName, picDir.FullName + @"\" + index + ".jpg");
                }
            }
        }

        private bool CheckAttributes(ExcelWorksheet ws, int categoryId, List<ExportCategoryAttributeValueModel> attributesResult, int productNumber, int columnPosition, int languageId)
        {
            List<ExportCategoryAttributeModel> attributes = new List<ExportCategoryAttributeModel>();
            var categoryAttributeGroup = _categoryProductAttributeService.GetCategoryToCategoryProductAttributeGroupByCategoryId(categoryId);
            if (categoryAttributeGroup.Count == 0)
            {
                return true;
            }
            foreach (var grp in categoryAttributeGroup)
            {
                attributes.AddRange(grp.CategoryProductAttributeGroup.CategoryProductAttributes.Select(x => new ExportCategoryAttributeModel()
                {
                    ControlType = x.AttributeControlType,
                    Name = x.ProductAttribute.Name,
                    Values = x.CategoryProductAttributeValues.OrderBy(y => y.DisplayOrder).ToList(),
                    Id = x.Id,
                    IsRequired = x.IsRequired
                }).ToList());
            }

            attributes = attributes.GroupBy(x => x.Name).Select(y => y.First()).ToList();
            foreach (var attribute in attributes)
            {
                string attributeValue = String.Empty;
                for (int i = 0; i < attributes.Count*2; i+=2)
                {
                    int id = ws.Cells[1, columnPosition + i + 1].GetValue<int>();
                    if (id == attribute.Id)
                    {
                        attributeValue = ws.Cells[productNumber,columnPosition + i].GetValue<string>();
                        break;
                    }
                }
                if (attribute.IsRequired)
                {
                    if (String.IsNullOrEmpty(attributeValue))
                        return false;
                }
                if (String.IsNullOrEmpty(attributeValue))
                {
                    continue;
                }
                switch (attribute.ControlType)
                {
                    case AttributeControlType.TextBox:
                        {
                            var value = new ExportCategoryAttributeValueModel();
                            value.CategoryAttributeId = attribute.Id;
                            value.Value = attributeValue;
                            attributesResult.Add(value);
                            break;
                        }
                    case AttributeControlType.Checkboxes:
                        {

                            var values = new List<ExportCategoryAttributeValueModel>();
                            string[] vals = attributeValue.Split(new char[] { ';' });
                            for (int j = 0; j < vals.Length; j++)
                            {
                                if (String.IsNullOrEmpty(vals[j]) || vals[j] == " ")
                                    continue;
                                var attrVal = attribute.Values.Where(x=>x.GetLocalized(a=>a.Name,languageId,true).Replace("\"","") == vals[j]).FirstOrDefault();
                                if(attrVal == null)
                                    continue;
                                values.Add(new ExportCategoryAttributeValueModel()
                                {
                                    Id = attrVal.Id,
                                    CategoryAttributeId = attribute.Id
                                });
                            }

                            attributesResult.AddRange(values);
                            break;
                        }
                    case AttributeControlType.DropdownList:
                        {
                            var value = new ExportCategoryAttributeValueModel();
                            value.CategoryAttributeId = attribute.Id;
                            var attrVal = attribute.Values.Where(x => x.GetLocalized(a=>a.Name,languageId,true).Replace("\"","") == attributeValue).FirstOrDefault();
                            if (attrVal == null)
                                continue;
                            value.Id = attrVal.Id;
                            attributesResult.Add(value);
                            break;
                        }
                }
            }

            return true;
        }
        private void FillExcellWithProducts(ExcelWorksheet workSheet, StringBuilder pictureHideCode, int categoryId, int languageId)
        {
            var products = _productService.NewSearchProducts(_workContext.CurrentCustomer.Id, 0, 0, Core.Domain.Catalog.ProductSortingEnum.CreatedOn, 0, 1000, 0, categoryId, 0);
            for (int i = 0; i < products.Count; i++)
            {
                var languageList = _languageService.GetAllLanguages();
                workSheet.Row(i * languageList.Count + 2).Height = Pixel2RowHeight(100);
                workSheet.Cells[i * languageList.Count + 2, 1].Value = "$";
                workSheet.Cells[i * languageList.Count + 2, 2].Value = products[i].Id;

                for (int j = 0; j < languageList.Count; j++)
                {
                    workSheet.Row(i * languageList.Count + j + 2).Height = Pixel2RowHeight(100);
                    workSheet.Cells[i * languageList.Count + j + 2, 3].Value = languageList[j].Name;
                    workSheet.Cells[i * languageList.Count + j + 2, 4].Value = products[i].GetLocalized(x => x.Name, languageList[j].Id, false);
                    workSheet.Cells[i * languageList.Count + j + 2, 4].Style.WrapText = true;
                    workSheet.Cells[i * languageList.Count + j + 2, 5].Value = products[i].GetLocalized(x => x.ShortDescription, languageList[j].Id, false);
                    workSheet.Cells[i * languageList.Count + j + 2, 5].Style.WrapText = true;
                    workSheet.Cells[i * languageList.Count + j + 2, 6].Value = products[i].GetLocalized(x => x.FullDescription, languageList[j].Id, false);
                    workSheet.Cells[i * languageList.Count + j + 2, 6].Style.WrapText = true;
                    workSheet.Cells[i * languageList.Count + j + 2, 7].Value = products[i].GetLocalized(x => x.AdminComment, languageList[j].Id, false);
                    workSheet.Cells[i * languageList.Count + j + 2, 7].Style.WrapText = true;
                }
                if (products[i].ProductPictures.Count > 0)
                {
                    ProcessPictures(products[i], i * languageList.Count + 2, workSheet, pictureHideCode);
                }
            }
        }

        private void ProcessPictures(Product product, int index, ExcelWorksheet worksheet, StringBuilder pictureHideCode)
        {
            ExcelPicture picture, pictureSmall;
            int firstPicColumn = 7;
            var defaultPic = product.ProductPictures.Where(x => x.DisplayOrder == 0).FirstOrDefault();
            if (defaultPic != null)
            {
                System.Drawing.Image img;
                var bytes = _pictureService.LoadPictureBinary(defaultPic.Picture);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    img = System.Drawing.Image.FromStream(ms);
                    picture = worksheet.Drawings.AddPicture(String.Format("{0}{1}", index, firstPicColumn), img);
                    pictureHideCode.Append(String.Format(PICTURE_HIDE_CODE_N, String.Format("{0}{1}", index, firstPicColumn)));
                    pictureSmall = worksheet.Drawings.AddPicture(Guid.NewGuid().ToString(), img);
                    pictureSmall.SetPosition(index - 1, 0, firstPicColumn - 1, 0);
                    pictureSmall.SetSize((int)Pixel2ColumnWidth(worksheet, (short)worksheet.Column(firstPicColumn).Width) / 2, (int)worksheet.Row(index).Height / 2);
                    ms.Close();
                }
            }
            var list = product.ProductPictures.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].DisplayOrder == 0)
                    continue;
                System.Drawing.Image img;
                var bytes = _pictureService.LoadPictureBinary(list[i].Picture);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    img = System.Drawing.Image.FromStream(ms);

                    picture = worksheet.Drawings.AddPicture(String.Format("{0}{1}", index, firstPicColumn + 2 + 2 * i), img);
                    pictureHideCode.Append(String.Format(PICTURE_HIDE_CODE_N, String.Format("{0}{1}", index, firstPicColumn + 2 + 2 * i)));
                    pictureSmall = worksheet.Drawings.AddPicture(Guid.NewGuid().ToString(), img);
                    pictureSmall.SetPosition(index - 1, 0, firstPicColumn + 2 + 2 * i - 1, 0);
                    pictureSmall.SetSize((int)Pixel2ColumnWidth(worksheet, (short)worksheet.Column(firstPicColumn + 2 + 2 * i).Width) / 2, (int)worksheet.Row(index).Height / 2);
                    ms.Close();
                }
            }
        }

        #region DimensionsConversion
        public const int MTU_PER_PIXEL = 9525;

        public static int ColumnWidth2Pixel(ExcelWorksheet ws, double excelColumnWidth)
        {
            //The correct method to convert width to pixel is:
            //Pixel =Truncate(((256 * {width} + Truncate(128/{Maximum DigitWidth}))/256)*{Maximum Digit Width})

            //get the maximum digit width
            decimal mdw = ws.Workbook.MaxFontWidth;

            //convert width to pixel
            decimal pixels = decimal.Truncate(((256 * (decimal)excelColumnWidth + decimal.Truncate(128 / (decimal)mdw)) / 256) * mdw);
            //double columnWidthInTwips = (double)(pixels * (1440f / 96f));

            return Convert.ToInt32(pixels);

        }

        public static double Pixel2ColumnWidth(ExcelWorksheet ws, int pixels)
        {
            //The correct method to convert pixel to width is:
            //1. use the formula =Truncate(({pixels}-5)/{Maximum Digit Width} * 100+0.5)/100 
            //    to convert pixel to character number.
            //2. use the formula width = Truncate([{Number of Characters} * {Maximum Digit Width} + {5 pixel padding}]/{Maximum Digit Width}*256)/256 
            //    to convert the character number to width.

            //get the maximum digit width
            decimal mdw = ws.Workbook.MaxFontWidth;

            //convert pixel to character number
            decimal numChars = decimal.Truncate(decimal.Add((decimal)(pixels - 5) / mdw * 100, (decimal)0.5)) / 100;
            //convert the character number to width
            decimal excelColumnWidth = decimal.Truncate((decimal.Add(numChars * mdw, (decimal)5)) / mdw * 256) / 256;

            return Convert.ToDouble(excelColumnWidth);
        }

        public static int RowHeight2Pixel(double excelRowHeight)
        {
            //convert height to pixel
            decimal pixels = decimal.Truncate((decimal)(excelRowHeight / 0.75));

            return Convert.ToInt32(pixels);
        }

        public static double Pixel2RowHeight(int pixels)
        {
            //convert height to pixel
            double excelRowHeight = pixels * (double)0.75;

            return excelRowHeight;
        }

        public static int MTU2Pixel(int mtus)
        {
            //convert MTU to pixel
            decimal pixels = decimal.Truncate((decimal)(mtus / MTU_PER_PIXEL));

            return Convert.ToInt32(pixels);
        }

        public static int Pixel2MTU(int pixels)
        {
            //convert pixel to MTU
            int mtus = pixels * MTU_PER_PIXEL;

            return mtus;
        }
        #endregion
        private static string ConvertString(int codePage, string sourceString)
        {
            byte[] bytes = Encoding.GetEncoding(codePage).GetBytes(sourceString);
            return Encoding.Default.GetString(bytes);
        }
        #region Macroconstants
        private string MODULE_CODE
        {
            get
            {
                return @"
Dim EventFlag As Boolean
Public Declare Sub Sleep Lib ""kernel32"" (ByVal dwMilliseconds As Long)
Public Sub PutPicInCell()
'
Dim MyCell As Range
Dim vfile As Variant
Dim bit As Byte

    Set MyCell = ActiveCell
    vfile = Application.GetOpenFilename(""All Files (*.*),*.*"")
    If vfile = ""False"" Then Exit Sub
    
    Dim length As Long
    length = FileLen(vfile)
    If length > 524000# Then
        MsgBox (""File is greater than 500 Kb"")
        Exit Sub
    End If
    
    Dim MinExtensionX
    Dim Arr() As Variant
    Dim lngLoc As Variant

    'Retrieve extension of file
    MinExtensionX = Mid(vfile, InStrRev(vfile, ""."") + 1)
    
    Arr = Array(""jpg"", ""jpeg"", ""png"", ""gif"") 'define which extensions you want to allow
    On Error Resume Next
    lngLoc = Application.WorksheetFunction.Match(MinExtensionX, Arr(), 0)
    If IsEmpty(lngLoc) Then
        MsgBox (""Only jpg, jpeg, png, gif files are allowed"")
        Exit Sub
    End If
    
    Dim oldShape As Shape
    On Error Resume Next
    Set oldShape = ActiveSheet.Shapes(CStr(ActiveCell.Row) + CStr(ActiveCell.Column))
    If Not oldShape Is Nothing Then
        oldShape.Delete
        End If
    
    Dim oldShape1 As Shape
    On Error Resume Next
    Set oldShape1 = ActiveSheet.Shapes(CStr(ActiveCell.Row) + CStr(ActiveCell.Column))  + ""_l""
    If Not oldShape1 Is Nothing Then
        oldShape1.Delete
        End If

    Dim oPic As Shape
    Set oPic = ActiveSheet.Shapes.AddPicture(vfile, False, True, -1, -1, -1, -1)
    oPic.Name = CStr(ActiveCell.Row) + CStr(ActiveCell.Column)
    oPic.Title = CStr(ActiveCell.Row) + CStr(ActiveCell.Column)
    ActiveSheet.Pictures.Insert(vfile).Select
    'newCell.Value = data
    oPic.Visible = False
    Application.Selection.Top = MyCell.Top
    Application.Selection.Left = MyCell.Left
    Application.Selection.Width = MyCell.Width / 2
    Application.Selection.Height = MyCell.Height / 2
    Application.Selection.Name = CStr(ActiveCell.Row) + CStr(ActiveCell.Column) + ""_l""
    Application.Selection.Title = CStr(ActiveCell.Row) + CStr(ActiveCell.Column) + ""_l""
    ActiveSheet.Cells(MyCell.Row, MyCell.Column + 1).Value = 0
End Sub

Public Sub HideImages()
    Dim oPic As Shape
    {0}
End Sub

Public Sub AddValidation()
    With ActiveSheet.Cells(ActiveCell.Row, 2).Validation
        .Delete
        .Add Type:=xlValidateTextLength, AlertStyle:=xlValidAlertStop, _
        Operator:=xlBetween, Formula1:=""1"", Formula2:=""4000000""
        .IgnoreBlank = False
        .InCellDropdown = True
        .InputTitle = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Prompt.Title")) +  @"""
        .ErrorTitle = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Error.Title")) + @"""
        .InputMessage = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Prompt.Message")) + @"""
        .ErrorMessage = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Error.Message")) + @"""
        .ShowInput = True
        .ShowError = True
    End With
    With ActiveSheet.Cells(ActiveCell.Row, 3).Validation
        .Delete
        .Add Type:=xlValidateTextLength, AlertStyle:=xlValidAlertStop, _
        Operator:=xlBetween, Formula1:=""1"", Formula2:=""4000000""
        .IgnoreBlank = False
        .InCellDropdown = True
        .InputTitle = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Prompt.Title")) + @"""
        .ErrorTitle = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Error.Title")) + @"""
        .InputMessage = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Prompt.Message")) + @"""
        .ErrorMessage = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Error.Message")) + @"""
        .ShowInput = True
        .ShowError = True
    End With
    With ActiveSheet.Cells(ActiveCell.Row, 4).Validation
        .Delete
        .Add Type:=xlValidateTextLength, AlertStyle:=xlValidAlertStop, _
        Operator:=xlBetween, Formula1:=""1"", Formula2:=""4000000""
        .IgnoreBlank = False
        .InCellDropdown = True
        .InputTitle = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Prompt.Title")) + @"""
        .ErrorTitle = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Error.Title")) + @"""
        .InputMessage = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Prompt.Message")) + @"""
        .ErrorMessage = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Error.Message")) + @"""
        .ShowInput = True
        .ShowError = True
    End With

    With ActiveSheet.Cells(ActiveCell.Row, 6).Validation
        .Delete
        .Add Type:=xlValidateTextLength, AlertStyle:=xlValidAlertStop, _
        Operator:=xlBetween, Formula1:=""1"", Formula2:=""4000000""
        .IgnoreBlank = False
        .InCellDropdown = True
        .InputTitle = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Prompt.Title")) + @"""
        .ErrorTitle = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Error.Title")) + @"""
        .InputMessage = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Prompt.Message")) + @"""
        .ErrorMessage = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Error.Message")) + @"""
        .ShowInput = True
        .ShowError = True
    End With
    {1}
    ActiveSheet.CircleInvalid
End Sub
Private Sub Timer()
    EventFlag = False
    Dim Lb As MSForms.ListBox
    {2}
End Sub

Public Function GetFlag() As Boolean
    GetFlag = EventFlag
End Function

Public Sub SetFlag(FlagState As Boolean)
    EventFlag = FlagState
End Sub
";
            }
        }

        private const string PICTURE_SELECTION_CHANGE_PART = @"
If Target.Column = 8 And Target.Row > 1 Then Call PutPicInCell
If Target.Column = 10 And Target.Row > 1 Then Call PutPicInCell
If Target.Column = 12 And Target.Row > 1 Then Call PutPicInCell
If Target.Column = 14 And Target.Row > 1 Then Call PutPicInCell
If Target.Column = 16 And Target.Row > 1 Then Call PutPicInCell
If Target.Column = 18 And Target.Row > 1 Then Call PutPicInCell";

        private const string SELECTION_CHANGE_WRAPPER = @" 
Dim EventFlag As Boolean
Private Sub WorkSheet_Activate()
    SetFlag (True)
End Sub
Private Sub Worksheet_BeforeDoubleClick(ByVal Target As Range, Cancel As Boolean)
    Application.OnTime Now() + TimeValue(""00:00:01""), ""Timer""
    SetFlag (True)
    Cancel = True
    If Target.Column = 2 Or Target.Column = 3 Or Target.Column = 4 Or Target.Column = 5 Then
        Cancel = False   
    End If 
    If Target.Column = 6 Or Target.Column = 7 Or Target.Column = 20 Or Target.Column = 21 Then
        Cancel = False   
    End If 
    If Target.Column = 22 Then
        Cancel = False   
    End If
    On Error Resume Next
    activeSheet.OLEObjects.Visible = False
    Dim obj As OLEObject
    Dim Lb As MSForms.ListBox
    ActiveSheet.Cells(Target.Row, 1).Value = ""$$""
    AddValidation
    {0}
    EventFlag = False
End Sub
Private Sub Worksheet_SelectionChange(ByVal Target As Range)
    On Error Resume Next
    ActiveSheet.OLEObjects.Visible = False
    EventFlag = True
    ActiveSheet.CircleInvalid
End Sub
{1}";

        private const string WORKBOOK_OPEN_WRAPPER = @"
Private Sub WorkBook_Open()
    Set vbProj = ActiveWorkbook.VBProject
    On Error Resume Next
    vbProj.References.AddFromFile ""C:\WINDOWS\system32\FM20.dll""
    HideImages
End Sub

Private Sub WorkBook_Activate()
    Dim Lb As MSForms.ListBox
    Dim obj As OLEObject
    Dim data As String
    Dim i As Integer
    Set Ws = ActiveWorkbook.Worksheets(""Product"")
    ActiveSheet.Columns(1).Hidden = True
{0}
End Sub";
        private const string LISTBOX_CHANGE_N = @"
Private Sub List{0}_Change()
If GetFlag() Then
    Exit Sub
End If

Dim data As String
data = ""
Dim Lb As MSForms.ListBox
Dim SelectedCount As Integer
Set Lb = activeSheet.OLEObjects(""List{0}"").Object
For i = 0 To Lb.ListCount - 1 Step 1
    If Lb.Selected(i) Then
        data = data + CStr(Lb.List(i)) + "";""
    End If
Next i
ActiveCell.Value = data
End Sub";

        private const string LISTBOX_SELECTION_CHANGE_CODE_N = @"
If Target.Column = {0} Then
    Set obj = activeSheet.OLEObjects(""List{0}"")
    obj.Top = Target.Top
    obj.Left = Target.Left
    obj.Width = Target.Width
    obj.Visible = True
    Set Lb = obj.Object
    For i = 0 To Lb.ListCount - 1 Step 1
        Lb.Selected(i) = False
    Next i
    For i = 0 To Lb.ListCount - 1 Step 1
        If InStr(CStr(ActiveCell.Value), CStr(Lb.List(i))) > 0 Then
            Lb.Selected(i) = True
        End If
    Next i
    Lb.Enabled = False
End If";

        private const string LISTBOX_WORKBOOK_OPEN_CODE_N = @"
On Error Resume Next
Set obj = Ws.OLEObjects(""List{0}"")
Ws.OLEObjects(""List{0}"").Delete
activeSheet.OLEObjects.Add ClassType:=""Forms.ListBox.1"", Link:=False, DisplayAsIcon:=False, Left:=20, Top:=20, Width:=50, Height:=72
Set obj = activeSheet.OLEObjects(""ListBox1"")
obj.Name = ""List{0}""
obj.Visible = False
Set Lb = obj.Object
{1}
Lb.MultiSelect = fmMultiSelectMulti
Lb.ListStyle = fmListStyleOption";
        private const string COMBOBOX_CHANGE_N = @"
Private Sub List{0}_Change()
Dim Lb As MSForms.ListBox
Set Lb = activeSheet.OLEObjects(""List{0}"").Object
ActiveCell.Value = Lb.List(Lb.ListIndex)
End Sub";

        private const string COMBOBOX_SELECTION_CHANGE_CODE_N = @"
If Target.Column = {0} Then
    Set obj = activeSheet.OLEObjects(""List{0}"")
    obj.Top = Target.Top
    obj.Left = Target.Left
    obj.Width = Target.Width
    obj.Visible = True
    Set Lb = obj.Object
    For i = 0 To Lb.ListCount - 1 Step 1
        If InStr(CStr(ActiveCell.Value), CStr(Lb.List(i))) > 0 Then
            Lb.ListIndex = i
        End If
    Next i
    Lb.Enabled = False
End If";

        private const string COMBOBOX_WORKBOOK_OPEN_CODE_N = @"
On Error Resume Next
Set obj = Ws.OLEObjects(""List{0}"")
Ws.OLEObjects(""List{0}"").Delete
activeSheet.OLEObjects.Add ClassType:=""Forms.ListBox.1"", Link:=False, DisplayAsIcon:=False, Left:=20, Top:=20, Width:=50, Height:=72
Set obj = activeSheet.OLEObjects(""ListBox1"")
obj.Name = ""List{0}""
obj.Visible = False
Set Lb = obj.Object
{1}";

        private const string ITEM_ADD_CODE = @"Lb.AddItem (""{0}"")";

        private string PICTURE_HIDE_CODE_N = @"
Set oPic = ActiveSheet.Shapes(""{0}"")
oPic.Visible = False";

        private string VALIDATION_CODE
        {
            get
            {
                return @"
    With ActiveSheet.Cells(ActiveCell.Row, {0}).Validation
        .Delete
        .Add Type:=xlValidateTextLength, AlertStyle:=xlValidAlertStop, _
        Operator:=xlBetween, Formula1:=""1"", Formula2:=""4000000""
        .IgnoreBlank = False
        .InCellDropdown = True
        InputTitle = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Prompt.Title")) + @"""
        .ErrorTitle = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Error.Title")) + @"""
        .InputMessage = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Prompt.Message")) + @"""
        .ErrorMessage = """ + ConvertString(codePage, _localizationService.GetResource("ETF.Excel.RequiredField.Error.Message")) + @"""
        .ShowInput = True
        .ShowError = True
    End With";
            }
        }

        private const string HIDE_COLUMN = @"
ActiveSheet.Columns({0}).Hidden = True";

        private const string CHECK_BOX_ENABLE_CODE = @"
On Error Resume Next
Set obj = ActiveSheet.OLEObjects(""List{0}"")
Set Lb = obj.Object
Lb.Enabled = True";
        #endregion
    }
}
