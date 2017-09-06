using Nop.Services.Catalog;
using Nop.Services.Media;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Localization;
using PdfSharp.Drawing;
using Nop.Core.Domain.Media;
using System.IO;
using Nop.Core.Domain.Catalog;
using PdfSharp.Drawing.Layout;
using Nop.Services.Directory;
using Nop.Core.Domain.Localization;
using System.Web;
using Nop.Services.Seo;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Nop.Core.Domain;
using System.Net;
using Nop.Core;

namespace Nop.Services.ExportImport
{
    public class ProductPdfService : IProductPdfService
    {
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductPriceService _productPriceService;
        private readonly ICurrencyService _currencyService;
        private readonly ILanguageService _languageService;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly IWorkContext _workContext;

        public ProductPdfService(IProductService productService,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IProductPriceService productPriceService,
            ICurrencyService currencyService,
            ILanguageService languageService,
            StoreInformationSettings storeInformationSettings,
            IWorkContext workContext)
        {
            this._productService = productService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._productPriceService = productPriceService;
            this._currencyService = currencyService;
            this._languageService = languageService;
            this._storeInformationSettings = storeInformationSettings;
            this._workContext = workContext;
        }

        //public void GenerateProductPdf(int productId, int languageId, MemoryStream stream)
        //{
        //    var product = _productService.GetProductById(productId);
        //    var document = new PdfDocument();
        //    document.Info.Title = product.GetLocalized(x => x.Name, languageId);
        //    int i;
        //    int yposition = 0;
        //    PdfPage page = document.AddPage();

        //    XGraphics gfx = XGraphics.FromPdfPage(page);

        //    XFont font = new XFont("Veranda", 20, XFontStyle.Bold, new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Default));
        //    //Display title
        //    XTextFormatter tfTitle = new XTextFormatter(gfx);
        //    tfTitle.Alignment = XParagraphAlignment.Center;
        //    int titleHeight;
        //    XSize attrSize = gfx.MeasureString(document.Info.Title, font);
        //    if (attrSize.Width > page.Width - 35)
        //    {
        //        titleHeight = ((int)(attrSize.Width / (page.Width - 35)) + 1) * ((int)attrSize.Height + 1);
        //        tfTitle.DrawString(document.Info.Title, font, XBrushes.Black,
        //            new XRect(20, 15, page.Width - 35, titleHeight + 16));
        //    }
        //    else
        //    {
        //        gfx.DrawString(document.Info.Title, font, XBrushes.Black,
        //            new XRect(20, 15, page.Width - 35, 50),
        //            XStringFormats.TopCenter);
        //        titleHeight = 50;
        //    }
        //    yposition = titleHeight + 15;
        //    //Display pictures
        //    if (product.ProductPictures.Count > 0)
        //    {
        //        var defaultPict = product.ProductPictures.Where(x => x.DisplayOrder == 0).FirstOrDefault();
        //        Picture pict;
        //        if (defaultPict == null)
        //        {
        //            pict = _pictureService.GetPictureById(product.ProductPictures.First().PictureId);
        //            DrawPicture(pict, 20, yposition, 200, 200, gfx);
        //        }
        //        else
        //        {
        //            DrawPicture(defaultPict.Picture, 20, yposition, 200, 200, gfx);
        //        }
        //        var pictureList = product.ProductPictures.ToList();
        //        for (i = 0; i < product.ProductPictures.Count; i++)
        //        {
        //            if (i < 3)
        //            {
        //                DrawPicture(pictureList[i].Picture, 240 + (i) * 120, 65, 110, 110, gfx);
        //            }
        //            else
        //            {
        //                DrawPicture(pictureList[i].Picture, 240 + (i - 3) * 120, 185, 110, 110, gfx);
        //            }
        //        }
        //        yposition = 310;
        //    }

        //    //Display short description
        //    font = new XFont("Veranda", 14, XFontStyle.Regular, new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Default));
        //    string full = product.GetLocalized(x => x.FullDescription, languageId);
        //    full = full.Replace("</br>", "\n");
        //    full = full.Replace("<p>", "\n\t");
        //    int index = 0;
        //    while (full.IndexOf("<") >= 0)
        //    {
        //        index = full.IndexOf("<");
        //        full = full.Remove(index, full.IndexOf(">", index) - index + 1);
        //    }

        //    var boldFont = new XFont("Veranda", 14, XFontStyle.Bold, new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Default));

        //    var languages = new OrderedLanguageCultures();
        //    int height = 0;
        //    var tempString = new StringBuilder();
        //    string companyName = product.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, languageId);
        //    if (companyName == null)
        //    {
        //        for (int k = 0; k < languages.Cultures.Count; k++)
        //        {
        //            var langid = _languageService.GetAllLanguages().Where(x => x.LanguageCulture == languages.Cultures[k]).FirstOrDefault().Id;
        //            companyName = product.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, langid, false);
        //            if (companyName != null)
        //            {
        //                break;
        //            }
        //        }
        //    }
        //    //gfx.DrawString(companyName, boldFont, XBrushes.Black,
        //    //    new XRect(100, yposition, page.Width - 35, 20),
        //    //    XStringFormats.TopLeft);
        //    attrSize = gfx.MeasureString(_localizationService.GetResource("Profile.Request.Company") + ":\t" + companyName, font);
        //    if (attrSize.Width > page.Width - 35)
        //    {
        //        height += (int)((((int)(attrSize.Width / (page.Width - 35)) + 1)) * attrSize.Height);
        //    }
        //    else
        //    {
        //        height += (int)attrSize.Height + 17;
        //    }
        //    tempString.AppendLine(_localizationService.GetResource("Profile.Request.Company") + ":\t" + companyName);
        //    //gfx.DrawString(_localizationService.GetResource("Profile.Request.Company") + ": ", boldFont, XBrushes.Black,
        //    //    new XRect(20, yposition, 80, 20),
        //    //    XStringFormats.TopLeft);
        //    //yposition += 20;
        //    if (product.Brand != null)
        //    {
        //        //gfx.DrawString(_localizationService.GetResource("Profile.Catalog.Brand") + ": ", boldFont, XBrushes.Black,
        //        //new XRect(20, yposition, 80, 20),
        //        //XStringFormats.TopLeft);
        //        //gfx.DrawString(product.Brand.Name, boldFont, XBrushes.Black,
        //        //new XRect(100, yposition, page.Width - 35, 20),
        //        //XStringFormats.TopLeft);
        //        attrSize = gfx.MeasureString(_localizationService.GetResource("Profile.Catalog.Brand") + ":\t" + product.Brand.Name, font);
        //        if (attrSize.Width > page.Width - 35)
        //        {
        //            height += (int)((((int)(attrSize.Width / (page.Width - 35))  + 2)) * attrSize.Height);
        //        }
        //        else
        //        {
        //            height += (int)attrSize.Height + 16;
        //        }
        //        tempString.AppendLine(_localizationService.GetResource("Profile.Catalog.Brand") + ":\t" + product.Brand.Name);
        //        //yposition += 20;
        //    }

        //    //Price processing
        //    var prices = _productPriceService.GetAllProductPrices(product.Id);
        //    var currencies = _currencyService.GetAllCurrencies().Where(c => c.Published).ToList();
        //    var prices_to_delete = prices.Where(p => !currencies.Contains(p.Currency)).ToList();
        //    prices = prices.Where(p => currencies.Contains(p.Currency)).OrderBy(p => p.Currency.Name).ToList();
        //    foreach (var p in prices_to_delete)
        //        _productPriceService.DeleteProductPriceById(p.Id);
        //    StringBuilder priceBuilder = new StringBuilder();
        //    if (prices.Where(x => x.Price != 0).Count() > 0)
        //    {
        //        priceBuilder.Append(_localizationService.GetResource("ETF.Front.Product.Add.ProductPrices") + ": ");

        //        foreach (var price in prices)
        //        {
        //            if (price.Price == 0)
        //                continue;
        //            priceBuilder.Append(price.Price.ToString("F2"));
        //            priceBuilder.Append(" ");
        //            priceBuilder.Append(price.Currency.CurrencyCode);
        //            priceBuilder.Append(", ");
        //        }
        //        int indexColon = priceBuilder.ToString().LastIndexOf(", ");
        //        if (index > 0)
        //            priceBuilder.Remove(priceBuilder.Length - 2, 2);
        //        //gfx.DrawString(priceBuilder.ToString(), boldFont, XBrushes.Black,
        //        //    new XRect(20, yposition, 80, 20),
        //        //    XStringFormats.TopLeft);
        //        attrSize = gfx.MeasureString(priceBuilder.ToString(), font);
        //        if (attrSize.Width > page.Width - 35)
        //        {
        //            height += (int)((((int)(attrSize.Width / (page.Width - 35)) + 2)) * attrSize.Height);
        //        }
        //        else
        //        {
        //            height += (int)attrSize.Height + 16;
        //        }
        //        tempString.AppendLine(priceBuilder.ToString() + "\n");
        //        //yposition += 20;
        //    }
        //    //Attributes processing
        //    List<CategoryProductAttributeGroup> _attrGroups = product.ProductAttributes.Select(x => x.CategoryProductAttribute.CategoryProductGroup).Distinct().ToList();
        //    List<AttributesModel> DisplayedAttributes = new List<AttributesModel>();
        //    foreach (var _aG in _attrGroups)
        //    {
        //        foreach (var cpa in _aG.CategoryProductAttributes)
        //        {
        //            AttributesModel cam = new AttributesModel();
        //            cam.Values = new List<CategoryProductAttributeValue>();
        //            cam.Values.AddRange(cpa.CategoryProductAttributeValues.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToList());
        //            if (cpa.AttributeControlType != AttributeControlType.TextBox)
        //            {
        //                cam.Name = cpa.ProductAttribute == null ? null : cpa.ProductAttribute.GetLocalized(x => x.Name, languageId);
        //            }
        //            else
        //            {
        //                cam.Name = cpa.ProductAttribute == null ? null : cpa.ProductAttribute.Name;
        //            }

        //            cam.ControlType = cpa.AttributeControlType;
        //            foreach (var val in cam.Values)
        //            {
        //                val.IsPreSelected = product.ProductAttributes.Where(p => p.Id == val.Id).Count() > 0;
        //            }
        //            cam.SelectedValue = cam.Values.Where(z => z.IsPreSelected).FirstOrDefault();
        //            DisplayedAttributes.Add(cam);
        //        }
        //    }

        //    DisplayedAttributes = DisplayedAttributes.Where(x => x.Name != null && ((x.SelectedValue != null && (x.ControlType == AttributeControlType.DropdownList || x.ControlType == AttributeControlType.RadioList || x.ControlType == AttributeControlType.TextBox)) || x.ControlType == AttributeControlType.Checkboxes)).ToList();
        //    var sb = new StringBuilder();
        //    for (i = 0; i < DisplayedAttributes.Count; i++)
        //    {
        //        var attribute = DisplayedAttributes[i];
        //        //gfx.DrawString(attribute.Name + ":", boldFont, XBrushes.Black,
        //        //new XRect(20, yposition + i * 20, 80, 20),
        //        //XStringFormats.TopLeft);
        //        tempString.Append(attribute.Name + ":\t");
        //        switch (attribute.ControlType)
        //        {
        //            case AttributeControlType.SizePicker:
        //            case AttributeControlType.Checkboxes:
        //            case AttributeControlType.ColorSquares:
        //                {
        //                    for (int j = 0; j < attribute.Values.Count; j++)
        //                    {
        //                        //if(attribute.Values[j])
        //                        if (attribute.Values[j].IsPreSelected)
        //                            sb.Append(attribute.Values[j].GetLocalized(x => x.Name, languageId) + ", ");
        //                    }
        //                    if (sb.Length > 0)
        //                    {
        //                        sb.Remove(sb.Length - 2, 2);
        //                    }
        //                    //gfx.DrawString(sb.ToString(), font, XBrushes.Black,
        //                    //new XRect(150, yposition + i * 20, page.Width - 185, 20),
        //                    //XStringFormats.TopLeft);
        //                    tempString.Append(sb.ToString() + "\n\n");
        //                    attrSize = gfx.MeasureString(sb.ToString(), font);
        //                    if (attrSize.Width > page.Width - 35)
        //                    {
        //                        height += (int)((((int)(attrSize.Width / (page.Width - 35)) + 2))*attrSize.Height);
        //                    }
        //                    else
        //                    {
        //                        height += (int)attrSize.Height + 16;
        //                    }
        //                    sb.Clear();
        //                    break;
        //                }
        //            case AttributeControlType.DropdownList:
        //            case AttributeControlType.RadioList:
        //                {
        //                    gfx.DrawString(attribute.SelectedValue.GetLocalized(x => x.Name, languageId), font, XBrushes.Black,
        //                    new XRect(150, yposition + i * 20, page.Width - 185, 20),
        //                    XStringFormats.TopLeft);
        //                    tempString.Append(attribute.SelectedValue.GetLocalized(x => x.Name, languageId) + "\n\n");
        //                    break;
        //                }
        //            case AttributeControlType.TextBox:
        //                {
        //                    gfx.DrawString(attribute.SelectedValue.Name, font, XBrushes.Black,
        //                    new XRect(150, yposition + i * 20, page.Width - 185, 20),
        //                    XStringFormats.TopLeft);
        //                    attrSize = gfx.MeasureString(attribute.SelectedValue.Name, font);
        //                    if (attrSize.Width > page.Width - 35)
        //                    {
        //                        height += (int)((((int)(attrSize.Width / (page.Width - 35)) + 2)) * attrSize.Height);
        //                    }
        //                    else
        //                    {
        //                        height += (int)attrSize.Height + 16;
        //                    }
        //                    tempString.Append(attribute.SelectedValue.Name + "\n\n");
        //                    break;
        //                }
        //        }
        //    }


        //    //if (full[0] == '\n')
        //    //{
        //    //    full = full.Remove(0, 1);
        //    //}
        //    //if (full[0] == '\t')
        //    //{
        //    //    full = full.Remove(0, 1);
        //    //}
        //    full = full.Replace("\t", "    ");
        //    XTextFormatter tfAttrr = new XTextFormatter(gfx);
        //    tfAttrr.Alignment = XParagraphAlignment.Left;
        //    tfAttrr.DrawString(tempString.ToString(), font, XBrushes.Black,
        //            new XRect(20, yposition, page.Width - 35, height), XStringFormats.TopLeft);
        //    yposition += height - 25;
        //    //process description
        //    var strings = full.Split(new char[] { '\n' });
        //    XTextFormatter tf = new XTextFormatter(gfx);
        //    tf.Alignment = XParagraphAlignment.Justify;
        //    XSize size; 
        //    int koef = 1;
        //    foreach (var str in strings)
        //    {
        //        koef = 1;
        //        string strToWrite;
        //        if (str == "")
        //            continue;
        //        size = gfx.MeasureString(str, font);
        //        if (size.Width > page.Width - 35)
        //        {
        //            koef = (int)(size.Width / (page.Width - 35)) + 1;
        //        }
        //        int textHeight = (int)(size.Height * koef) + 16;
        //        if (yposition + textHeight > page.Height - 30)
        //        {
        //            int endIndex;
        //            endIndex = (int)(str.Length * ((textHeight) / (page.Height - yposition - 30)));
                     
        //            if (endIndex > str.Length)
        //            {
        //                endIndex = str.Length;
        //            }
        //            else
        //            {
        //                int endIndex1 = str.LastIndexOf(" ", endIndex);
        //                if (endIndex1 > 0)
        //                {
        //                    endIndex = endIndex1;
        //                }
        //            }
        //            //endIndex = str.LastIndexOf(" ", endIndex);
        //            strToWrite = str.Substring(0, endIndex);
        //            strToWrite = HttpUtility.HtmlDecode(strToWrite);
        //            tf.DrawString(strToWrite, font, XBrushes.Black, new XRect(20, yposition, page.Width - 35, page.Height - 30 - yposition));
        //            textHeight = (int)(textHeight * (1 - 1.0 / koef));
        //            yposition = 15;
        //            page = document.AddPage();
        //            gfx = XGraphics.FromPdfPage(page);
        //            tf = new XTextFormatter(gfx);
        //            strToWrite = str.Substring(endIndex, str.Length - endIndex);
        //            strToWrite = HttpUtility.HtmlDecode(strToWrite);
        //            tf.DrawString(strToWrite, font, XBrushes.Black, new XRect(20, yposition, page.Width - 35, textHeight));
        //            yposition += textHeight + 32;
        //        }
        //        else
        //        {
        //            strToWrite = HttpUtility.HtmlDecode(str);
        //            tf.DrawString(strToWrite, font, XBrushes.Black, new XRect(20, yposition, page.Width - 35, textHeight));
        //            yposition += textHeight;

        //            if (yposition > page.Height - 30)
        //            {
        //                yposition = 15;
        //                page = document.AddPage();
        //                gfx = XGraphics.FromPdfPage(page);
        //                tf = new XTextFormatter(gfx);
        //            }
        //        }
        //    }

            
        //    //XTextFormatter tf = new XTextFormatter(gfx);
        //    //tf.Alignment = XParagraphAlignment.Justify;
        //    //XSize size = gfx.MeasureString(full, font);
        //    //int koef = 1;
        //    //if (size.Width > page.Width)
        //    //{
        //    //    koef = (int)(size.Width / page.Width) + 1;
        //    //}
        //    //if (size.Height * koef > page.Height - (yposition + 15))
        //    //{
        //    //    int endindex = (int)(full.Length / (size.Height * koef / (page.Height - yposition - 75)));
        //    //    endindex = full.LastIndexOf(" ", endindex);

        //    //    var str = HttpUtility.HtmlDecode(full.Substring(0, endindex));
        //    //    tf.DrawString(HttpUtility.HtmlDecode(full.Substring(0, endindex)), font, XBrushes.Black,
        //    //        new XRect(20, yposition, page.Width - 35, page.Height - yposition - 15), XStringFormats.TopLeft);
        //    //    PdfPage page2 = document.AddPage();
        //    //    XGraphics gfx2 = XGraphics.FromPdfPage(page2);
        //    //    var tf1 = new XTextFormatter(gfx2);
        //    //    tf1.DrawString(HttpUtility.HtmlDecode(full.Substring(endindex, full.Length - endindex)), font, XBrushes.Black,
        //    //        new XRect(20, 15, page.Width - 35, page.Height - 30), XStringFormats.TopLeft);
        //    //}
        //    //else
        //    //{
        //    //    var str = HttpUtility.HtmlDecode(full);
        //    //    tf.DrawString(HttpUtility.HtmlDecode(full), font, XBrushes.Black,
        //    //        new XRect(20, yposition, page.Width - 35, page.Height - 30), XStringFormats.TopLeft);
        //    //}
        //    //document.Save(Path.Combine(fileName,"template.pdf"));

        //    document.Save(stream, false);
        //}


        
        private void DrawPicture(Picture pic, int x, int y, int width, int height, XGraphics gfx)
        {
            System.Drawing.Image img;
            var bytes = _pictureService.LoadPictureBinary(pic);
            if (bytes.Length == 0)
                return;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                img = System.Drawing.Image.FromStream(ms);
                int imageWidth = img.Width;
                int imageHeight = img.Height;
                using (XImage image = XImage.FromGdiPlusImage(img))
                {
                    if (imageWidth >= imageHeight)
                    {
                        double k = (double)imageWidth / imageHeight;
                        gfx.DrawImage(image, x, y, width, height / k);
                        image.Dispose();
                    }
                    else
                    {
                        double k = (double)imageHeight / imageWidth;
                        gfx.DrawImage(image, x, y, width / k, height);
                        image.Dispose();
                    }

                }
                ms.Close();
            }
        }


        public void GeneratePdfItextSharp(int productId, int languageId, MemoryStream stream, string imageFolderPath)
        {
            var product = _productService.GetProductById(productId);
            string productTitle = product.GetLocalized(x => x.Name, languageId);
            string productUrl =product.GetSeName(languageId);
            if (_workContext.CurrentMiniSite != null)
            {
                productUrl = "http://" + _workContext.CurrentMiniSite.DomainName + "/" + WebUtility.UrlEncode(productUrl);
            }
            else
            {
                productUrl = _storeInformationSettings.StoreUrl + WebUtility.UrlEncode(productUrl);
            }
            var languages = new OrderedLanguageCultures();
            string companyName = product.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, languageId);
            string companyLink = "";
            if (companyName == null)
            {
                for (int k = 0; k < languages.Cultures.Count; k++)
                {
                    var langid = _languageService.GetAllLanguages().Where(x => x.LanguageCulture == languages.Cultures[k]).FirstOrDefault().Id;
                    companyName = product.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, langid, false);
                    if (companyName != null)
                    {
                        companyLink = product.Customer.CompanyInformation.GetSeName(langid);
                        companyLink = _storeInformationSettings.StoreUrl + WebUtility.UrlEncode(companyLink);
                        break;
                    }
                }
            }
            else
            {
                companyLink = product.Customer.CompanyInformation.GetSeName(languageId);
                companyLink = _storeInformationSettings.StoreUrl + WebUtility.UrlEncode(companyLink);
            }

            string rootUrl = _storeInformationSettings.StoreUrl;
            var picturesCount = product.ProductPictures.Count;
            var headeEventHandler = new MyPageEventHanndler(
                productTitle, productUrl, companyName, "", companyLink, picturesCount > 0, imageFolderPath,_storeInformationSettings.StoreUrl);
            
            float headeWidth = headeEventHandler.HeaderHeight(PageSize.A4.Width);
            Document document = new Document(PageSize.A4, 0f, 0f, headeWidth + 20f, 40f);
            var writer = PdfWriter.GetInstance(document, stream);
            writer.PageEvent = headeEventHandler;
            document.Open();
            if (picturesCount > 0)
            {
                var picturesIds = product.ProductPictures.Select(x=>x.PictureId).ToList();
                var imageTable = new PdfPTable(2);
                imageTable.HorizontalAlignment = Element.ALIGN_LEFT;
                imageTable.TotalWidth = document.PageSize.Width;
                imageTable.WidthPercentage = 100f;
                imageTable.SetWidths(new float[] { 50f, 50f });
                imageTable.LockedWidth = true;
                imageTable.HorizontalAlignment = 0;
                imageTable.SpacingAfter = 20f;
                if (picturesCount % 2 == 0)
                {
                    for (int i = 0; i < picturesCount; i += 2)
                    {
                        var cell = new PdfPCell();
                        cell.Border = 0;
                        cell.PaddingTop = 10f;
                        cell.PaddingBottom = 10f;
                        cell.BackgroundColor = new BaseColor(211, 238, 226);
                        PutPictureInCell(picturesIds[i], 202, cell, (int)document.PageSize.Width, true);
                        imageTable.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        cell.PaddingTop = 10f;
                        cell.PaddingBottom = 10f;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.BackgroundColor = new BaseColor(211, 238, 226);
                        PutPictureInCell(picturesIds[i + 1], 202, cell, (int)document.PageSize.Width, false);
                        imageTable.AddCell(cell);
                    }
                }
                else
                {
                    for (int i = 0; i < picturesCount; i += 2)
                    {
                        if (i + 2 > picturesCount)
                        {
                            var cell = new PdfPCell();
                            cell.Colspan = 2;
                            cell.Border = 0;
                            cell.PaddingTop = 10f;
                            cell.PaddingBottom = 10f;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.BackgroundColor = new BaseColor(211, 238, 226);
                            PutPictureInCell(picturesIds[i], 202, cell, (int)document.PageSize.Width);
                            imageTable.AddCell(cell);
                        }
                        else
                        {
                            var cell = new PdfPCell();
                            cell.Border = 0;
                            cell.PaddingTop = 10f;
                            cell.PaddingBottom = 10f;
                            cell.BackgroundColor = new BaseColor(211, 238, 226);
                            PutPictureInCell(picturesIds[i], 202, cell, (int)document.PageSize.Width, true);
                            imageTable.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            cell.PaddingTop = 10f;
                            cell.PaddingBottom = 10f;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.BackgroundColor = new BaseColor(211, 238, 226);
                            PutPictureInCell(picturesIds[i + 1], 202, cell, (int)document.PageSize.Width, false);
                            imageTable.AddCell(cell);
                        }
                    }
                }


                document.Add(imageTable);
            }

            var attributeTable = new PdfPTable(2);
            attributeTable.HorizontalAlignment = Element.ALIGN_LEFT;
            attributeTable.TotalWidth = document.PageSize.Width;
            attributeTable.WidthPercentage = 80f;
            attributeTable.SetWidths(new float[] { 30f, 70f });
            attributeTable.LockedWidth = true;
            attributeTable.HorizontalAlignment = 0;

            FontFactory.Register(imageFolderPath + @"\arial.ttf","Arial");
            Font simpleBlackFont = FontFactory.GetFont("Arial",BaseFont.IDENTITY_H, 14, Font.NORMAL, new BaseColor(0, 0, 0));
            Font boldRedFont = FontFactory.GetFont("Arial", BaseFont.IDENTITY_H, 14, Font.BOLD, new BaseColor(255, 42, 72));
            //Price processing
            var prices = _productPriceService.GetAllProductPrices(product.Id);
            var currencies = _currencyService.GetAllCurrencies().Where(c => c.Published).ToList();
            var prices_to_delete = prices.Where(p => !currencies.Contains(p.Currency)).ToList();
            prices = prices.Where(p => currencies.Contains(p.Currency)).OrderBy(p => p.Currency.Name).ToList();
            foreach (var p in prices_to_delete)
                _productPriceService.DeleteProductPriceById(p.Id);
            StringBuilder priceBuilder = new StringBuilder();
            if (prices.Where(x => x.Price != 0).Count() > 0)
            {
                var cell = new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_BOTTOM;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.PaddingLeft = 40f;
                cell.AddElement(new Paragraph(_localizationService.GetResource("ETF.Front.Product.Add.ProductPrices") + ":", simpleBlackFont));
                attributeTable.AddCell(cell);

                bool first = true;
                var paragraph = new Paragraph();
                var phrase = new Phrase();
                foreach (var price in prices)
                {
                    if (price.Price == 0)
                        continue;
                    if (first)
                    {
                        if (price.Currency.CurrencyCode.ToLower() != "byr")
                        {
                            phrase.Add(new Chunk(price.Price.ToString("F2") + " ", boldRedFont));
                            phrase.Add(new Chunk(price.Currency.CurrencyCode.ToUpper(), simpleBlackFont));
                            first = false;
                        }
                        else
                        {
                            phrase.Add(new Chunk(price.Price.ToString("F0") + " ", boldRedFont));
                            phrase.Add(new Chunk(price.Currency.CurrencyCode.ToUpper(), simpleBlackFont));
                            first = false;
                        }
                    }
                    else
                    {
                        if (price.Currency.CurrencyCode.ToLower() != "byr")
                        {
                            phrase.Add(new Chunk(",    ", simpleBlackFont));
                            phrase.Add(new Chunk(price.Price.ToString("F2") + " ", boldRedFont));
                            phrase.Add(new Chunk(price.Currency.CurrencyCode.ToUpper(), simpleBlackFont));
                        }
                        else
                        {
                            phrase.Add(new Chunk(", ", simpleBlackFont));
                            phrase.Add(new Chunk(price.Price.ToString("F0") + " ", boldRedFont));
                            phrase.Add(new Chunk(price.Currency.CurrencyCode.ToUpper(), simpleBlackFont));
                        }
                    }
                }

                paragraph.Add(phrase);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.PaddingRight = 40f;
                cell.VerticalAlignment = Element.ALIGN_BOTTOM;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                attributeTable.AddCell(cell);
            }


            //Attributes processing
            List<CategoryProductAttributeGroup> _attrGroups = product.ProductAttributes.Select(x => x.CategoryProductAttribute.CategoryProductGroup).Distinct().ToList();
            List<AttributesModel> DisplayedAttributes = new List<AttributesModel>();
            foreach (var _aG in _attrGroups)
            {
                foreach (var cpa in _aG.CategoryProductAttributes)
                {
                    AttributesModel cam = new AttributesModel();
                    cam.Values = new List<CategoryProductAttributeValue>();
                    cam.Values.AddRange(cpa.CategoryProductAttributeValues.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToList());
                    if (cpa.AttributeControlType != AttributeControlType.TextBox)
                    {
                        cam.Name = cpa.ProductAttribute == null ? null : cpa.ProductAttribute.GetLocalized(x => x.Name, languageId);
                    }
                    else
                    {
                        cam.Name = cpa.ProductAttribute == null ? null : cpa.ProductAttribute.Name;
                    }

                    cam.ControlType = cpa.AttributeControlType;
                    foreach (var val in cam.Values)
                    {
                        val.IsPreSelected = product.ProductAttributes.Where(p => p.Id == val.Id).Count() > 0;
                    }
                    cam.SelectedValue = cam.Values.Where(z => z.IsPreSelected).FirstOrDefault();
                    DisplayedAttributes.Add(cam);
                }
            }

            DisplayedAttributes = DisplayedAttributes.Where(x => x.Name != null && ((x.SelectedValue != null && (x.ControlType == AttributeControlType.DropdownList || x.ControlType == AttributeControlType.TextBox)) || x.ControlType == AttributeControlType.Checkboxes)).ToList();
            var sb = new StringBuilder();
            for (int i = 0; i < DisplayedAttributes.Count; i++)
            {
                var attribute = DisplayedAttributes[i];
                var cell = new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.AddElement(new Paragraph(attribute.Name + ":", simpleBlackFont));
                cell.Border = 0;
                cell.PaddingLeft = 40f;
                attributeTable.AddCell(cell);

                //gfx.DrawString(attribute.Name + ":", boldFont, XBrushes.Black,
                //new XRect(20, yposition + i * 20, 80, 20),
                //XStringFormats.TopLeft);
                switch (attribute.ControlType)
                {
                    case AttributeControlType.Checkboxes:
                        {
                            for (int j = 0; j < attribute.Values.Count; j++)
                            {
                                //if(attribute.Values[j])
                                if (attribute.Values[j].IsPreSelected)
                                    sb.Append(attribute.Values[j].GetLocalized(x => x.Name, languageId) + ", ");
                            }
                            if (sb.Length > 0)
                            {
                                sb.Remove(sb.Length - 2, 2);
                            }
                            //gfx.DrawString(sb.ToString(), font, XBrushes.Black,
                            //new XRect(150, yposition + i * 20, page.Width - 185, 20),
                            //XStringFormats.TopLeft);
                            cell = new PdfPCell();
                            cell.VerticalAlignment = Element.ALIGN_TOP;
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.AddElement(new Paragraph(sb.ToString(), simpleBlackFont));
                            cell.Border = 0;
                            cell.PaddingRight = 40f;
                            attributeTable.AddCell(cell);
                            sb.Clear();
                            break;
                        }
                    case AttributeControlType.DropdownList:
                        {
                            cell = new PdfPCell();
                            cell.VerticalAlignment = Element.ALIGN_TOP;
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.AddElement(new Paragraph(attribute.SelectedValue.GetLocalized(x => x.Name, languageId), simpleBlackFont));
                            cell.Border = 0;
                            cell.PaddingRight = 40f;
                            attributeTable.AddCell(cell);
                            break;
                        }
                    case AttributeControlType.TextBox:
                        {
                            cell = new PdfPCell();
                            cell.VerticalAlignment = Element.ALIGN_TOP;
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.AddElement(new Paragraph(attribute.SelectedValue.Name, simpleBlackFont));
                            cell.Border = 0;
                            cell.PaddingRight = 40f;
                            attributeTable.AddCell(cell);
                            break;
                        }
                }
            }

            document.Add(attributeTable);

            string full = product.GetLocalized(x => x.FullDescription, languageId);
            full = WebUtility.HtmlDecode(full);
            full = full.Replace("\n", "");
            full = full.Replace("</br>", "\n");
            full = full.Replace("<br />", "\n");
            full = full.Replace("<p>", "\t");
            //full = full.Replace("<ul>", "\n");
            full = full.Replace("<li>", "\t - ");
            full = full.Replace("<tr>", "\n");
            full = full.Replace("</td>", "\t");
            int index = 0;
            while (full.IndexOf("<") >= 0)
            {
                index = full.IndexOf("<");
                full = full.Remove(index, full.IndexOf(">", index) - index + 1);
            }

            Font simpleBlackBoldFont = FontFactory.GetFont("Arial",BaseFont.IDENTITY_H, 16, Font.BOLD, new BaseColor(0, 0, 0));
            var paragraphFull = new Paragraph(_localizationService.GetResource("ETF.Full"), simpleBlackBoldFont);
            paragraphFull.IndentationLeft = 40f;
            paragraphFull.SpacingBefore = 40f;
            document.Add(paragraphFull);
            
            paragraphFull = new Paragraph(full, simpleBlackFont);
            paragraphFull.IndentationLeft = 40f;
            paragraphFull.IndentationRight = 40f;
            paragraphFull.SpacingBefore = 10f;

            string orderingComments = product.GetLocalized(x => x.AdminComment, languageId, false);
            orderingComments = WebUtility.HtmlDecode(orderingComments);
            if (!String.IsNullOrEmpty(orderingComments))
            {
                paragraphFull.SpacingAfter = 10f;
                document.Add(paragraphFull);
                orderingComments = orderingComments.Replace("\n", "");
                orderingComments = orderingComments.Replace("</br>", "\n");
                orderingComments = orderingComments.Replace("<br />", "\n");
                orderingComments = orderingComments.Replace("<p>", "\t");
                //orderingComments = orderingComments.Replace("<ul>", "\n");
                orderingComments = orderingComments.Replace("<li>", "\t - ");
                orderingComments = orderingComments.Replace("<tr>", "\n");
                orderingComments = orderingComments.Replace("</td>", "\t");
                index = 0;
                while (orderingComments.IndexOf("<") >= 0)
                {
                    index = orderingComments.IndexOf("<");
                    orderingComments = orderingComments.Remove(index, orderingComments.IndexOf(">", index) - index + 1);
                }
                paragraphFull = new Paragraph(_localizationService.GetResource("ETF.OrderingComments"), simpleBlackBoldFont);
                paragraphFull.IndentationLeft = 40f;
                paragraphFull.SpacingBefore = 10f;
                document.Add(paragraphFull);

                paragraphFull = new Paragraph(orderingComments, simpleBlackFont);
                paragraphFull.IndentationLeft = 40f;
                paragraphFull.IndentationRight = 40f;
                paragraphFull.SpacingBefore = 10f;
                paragraphFull.SpacingAfter = 0f;
                document.Add(paragraphFull);
            }
            else
            {
                paragraphFull.SpacingAfter = 0f;
                document.Add(paragraphFull);
            }
            document.Close();
        }

        private void GetImage(byte[] imageBynary, out bool widthScale, out double scaleFactor)
        {
            using(var ms = new MemoryStream(imageBynary))
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                int imageWidth = img.Width;
                int imageHeight = img.Height;
                if (imageWidth >= imageHeight)
                {
                    scaleFactor = (double)imageWidth / imageHeight;
                    widthScale = false;
                }
                else
                {
                    scaleFactor = (double)imageHeight / imageWidth;
                    widthScale = true;
                }
            }
        }

        public void PutPictureInCell(int pictureId, float imgWidth, PdfPCell cell, int pageWidth, bool? left = null)
        {
            var picture = _pictureService.GetPictureById(pictureId);

            var pictureBinary = _pictureService.LoadPictureBinary(picture);
            var table = new PdfPTable(3);
            if (left.HasValue)
            {
                if (left.Value)
                {
                    table.SetWidths(new[] { 20f, 70f, 10f });
                }
                else
                {
                    table.SetWidths(new[] { 10f, 70f, 20f });
                }
                table.WidthPercentage = 100f;
            }
            else
            {
                table.SetWidths(new[] { 15f, 70f, 15f });
                table.WidthPercentage = 50f;
            }
            table.TotalWidth = pageWidth / 2;
            var newCell = new PdfPCell();
            newCell.Border = 0;
            newCell.BackgroundColor = new BaseColor(211, 238, 226);
            table.AddCell(newCell);
            double scaleFactor;
            bool scaleWidth;
            GetImage(pictureBinary, out scaleWidth, out scaleFactor);
            var img = Image.GetInstance(pictureBinary);
            float newWidth = scaleWidth ? imgWidth / (float)scaleFactor : imgWidth;
            float newHeight = scaleWidth ? imgWidth : imgWidth / (float)scaleFactor;
            img.ScaleAbsolute(newWidth, newHeight);
            newCell = new PdfPCell(img);
            newCell.HorizontalAlignment = Element.ALIGN_CENTER;
            newCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            newCell.BorderWidth = 2f;
            newCell.Padding = 2f;
            newCell.FixedHeight = imgWidth + 4;
            newCell.BorderColor = new BaseColor(156, 220, 194);
            newCell.BackgroundColor = new BaseColor(255, 255, 255);
            table.AddCell(newCell);

            newCell = new PdfPCell();
            newCell.Border = 0;
            newCell.BackgroundColor = new BaseColor(211, 238, 226);
            table.AddCell(newCell);

            cell.AddElement(table);
        }

        private void PutPicInCellMobile(int pictureId, float imgWidth, PdfPCell cell, int pageWidth, bool? left = null)
        {
            var picture = _pictureService.GetPictureById(pictureId);

            var pictureBinary = _pictureService.LoadPictureBinary(picture);
            var table = new PdfPTable(3);
            table.SetWidths(new[] { 14.5f, 71f, 14.5f });
            table.TotalWidth = pageWidth;
            table.WidthPercentage = 100f;
            var newCell = new PdfPCell();
            newCell.Border = 0;
            newCell.BackgroundColor = new BaseColor(211, 238, 226);
            table.AddCell(newCell);
            double scaleFactor;
            bool scaleWidth;
            GetImage(pictureBinary, out scaleWidth, out scaleFactor);
            var img = Image.GetInstance(pictureBinary);
            float newWidth =(float)(scaleWidth ? (pageWidth * 0.7) / (float)scaleFactor : (pageWidth * 0.7));
            float newHeight = (float)(scaleWidth ? (pageWidth * 0.7) : (pageWidth * 0.7) / (float)scaleFactor);
            img.ScaleAbsolute(newWidth, newHeight);
            newCell = new PdfPCell(img);
            newCell.HorizontalAlignment = Element.ALIGN_CENTER;
            newCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            newCell.BorderWidth = 2f;
            newCell.Padding = 2f;
            newCell.FixedHeight = (float)((pageWidth * 0.7) + 4);
            newCell.BorderColor = new BaseColor(156, 220, 194);
            newCell.BackgroundColor = new BaseColor(255, 255, 255);
            table.AddCell(newCell);

            newCell = new PdfPCell();
            newCell.Border = 0;
            newCell.BackgroundColor = new BaseColor(211, 238, 226);
            table.AddCell(newCell);

            cell.AddElement(table);
        }


        public class MyPageEventHanndler : PdfPageEventHelper
        {
            private readonly string _productTitle;
            private readonly string _productLink;
            private readonly string _company;
            private readonly string _brand;
            private readonly string _companyLink;
            private readonly bool _haveImages;
            private readonly string _imageFolderPath;
            private readonly string _webSiteUrl;

            public float HeaderHeight(float totalWidth)
            {
                string imageFolderPath = _imageFolderPath;
                var table = new PdfPTable(2);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                table.TotalWidth = totalWidth;
                table.WidthPercentage = 100f;
                table.SetWidths(new float[] { 80f, 20f });
                table.LockedWidth = true;
                table.HorizontalAlignment = 0;
                FontFactory.Register(imageFolderPath + @"\arial.ttf", "Arial");
                Font headerWhiteFont = FontFactory.GetFont("Arial", 22, Font.NORMAL, new BaseColor(255, 255, 255));
                Font simpleWhiteFont = FontFactory.GetFont("Arial", 16, Font.NORMAL, new BaseColor(255, 255, 255));
                Font simpleYellowFont = FontFactory.GetFont("Arial", 16, Font.NORMAL, new BaseColor(206, 208, 130));
                Anchor anchor = new Anchor(_productTitle, headerWhiteFont);
                anchor.Reference = _productLink;
                var cell = new PdfPCell(anchor);
                cell.Border = 2;
                cell.BorderColor = new BaseColor(46, 52, 68);
                cell.PaddingLeft = 30f;
                cell.PaddingTop = 10f;
                cell.PaddingBottom = 10f;
                cell.BackgroundColor = new BaseColor(46, 52, 68);
                table.AddCell(cell);
                var image = Image.GetInstance(imageFolderPath + @"\broshure_copyright.png");
                image.ScaleToFit(115, 35);
                ///var chunk = new Chunk(image,0 ,150,false);
                ///chunk.SetAnchor(_webSiteUrl);
                ///image.Chunks.Add(chunk);
                cell = new PdfPCell(image);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = new BaseColor(46, 52, 68);
                cell.Border = 2;
                cell.BorderColor = new BaseColor(46, 52, 68);
                table.AddCell(cell);

                var paragraph = new Paragraph();
                var phrase = new Phrase();
                phrase.Add(new Chunk(_brand + " ", simpleYellowFont));
                phrase.Add(new Chunk("by ", simpleWhiteFont));
                anchor = new Anchor(_company, simpleYellowFont);
                anchor.Reference = _companyLink;
                phrase.Add(anchor);
                paragraph.Add(phrase);
                cell = new PdfPCell(paragraph);
                cell.Colspan = 2;
                cell.BackgroundColor = new BaseColor(46, 52, 68);
                cell.PaddingLeft = 30f;
                cell.PaddingBottom = 10f;
                cell.Border = 0;
                table.AddCell(cell);

                return table.TotalHeight;
            }

            public MyPageEventHanndler(string productTitle,
                string productLink,
                string company,
                string brand,
                string companyLink,
                bool haveImages,
                string imageFolderPath,
                string webSiteUrl)
            {
                this._productLink = productLink;
                this._productTitle = productTitle;
                this._brand = brand;
                this._company = company;
                this._companyLink = companyLink;
                this._haveImages = haveImages;
                this._imageFolderPath = imageFolderPath;
                this._webSiteUrl = webSiteUrl;
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                var table = new PdfPTable(2);
                table.DefaultCell.Border = Rectangle.NO_BORDER;
                table.DefaultCell.BorderColor = new BaseColor(46, 52, 68);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                table.TotalWidth = document.PageSize.Width;
                table.WidthPercentage = 100f;
                table.SetWidths(new float[] { 80f, 20f });
                table.LockedWidth = true;
                table.HorizontalAlignment = 0;
                FontFactory.Register(_imageFolderPath + @"\arial.ttf", "Arial");
                Font headerWhiteFont = FontFactory.GetFont("Arial",BaseFont.IDENTITY_H, 22, Font.NORMAL, new BaseColor(255, 255, 255));
                Font simpleWhiteFont = FontFactory.GetFont("Arial", BaseFont.IDENTITY_H, 16, Font.NORMAL, new BaseColor(255, 255, 255));
                Font simpleYellowFont = FontFactory.GetFont("Arial", BaseFont.IDENTITY_H, 16, Font.NORMAL, new BaseColor(206, 208, 130));
                Anchor anchor = new Anchor(_productTitle, headerWhiteFont);
                anchor.Reference = _productLink;
                var cell = new PdfPCell(anchor);
                cell.Border = Rectangle.NO_BORDER;
                cell.BorderColor = new BaseColor(46, 52, 68);
                cell.PaddingLeft = 30f;
                cell.PaddingTop = 10f;
                cell.PaddingBottom = 10f;
                cell.BackgroundColor = new BaseColor(46, 52, 68);
                table.AddCell(cell);
                var image = Image.GetInstance(_imageFolderPath + @"\broshure_copyright.png");
                image.ScaleToFit(115, 35);
                var chunk = new Chunk(image, 0, -15, false);
                chunk.SetBackground(new BaseColor(46, 52, 68));
                anchor = new Anchor(chunk);
                anchor.Reference = _webSiteUrl;
                cell = new PdfPCell(anchor);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = new BaseColor(46, 52, 68);
                cell.Border = 2;
                cell.BorderColor = new BaseColor(46, 52, 68);
                table.AddCell(cell);

                Paragraph paragraph = new Paragraph();
                Phrase phrase = new Phrase();
                phrase.Add(new Chunk(_brand + " ", simpleYellowFont));
                phrase.Add(new Chunk("by ", simpleWhiteFont));
                anchor = new Anchor(_company, simpleYellowFont);
                anchor.Reference = _companyLink;
                //phrase.Add(anchor);
                paragraph.Add(phrase);
                paragraph.Add(anchor);
                cell = new PdfPCell(paragraph);
                cell.Colspan = 2;
                cell.BackgroundColor = new BaseColor(46, 52, 68);
                cell.PaddingLeft = 30f;
                cell.PaddingBottom = 10f;
                cell.Border = 0;
                table.AddCell(cell);
                if (_haveImages)
                {
                    cell = new PdfPCell();
                    cell.FixedHeight = 20f;
                    cell.Colspan = 2;
                    cell.Border = 0;
                    if (writer.CurrentPageNumber == 1)
                    {
                        cell.BackgroundColor = new BaseColor(211, 238, 226);
                    }
                    else
                    {
                        cell.BackgroundColor = new BaseColor(255, 255, 255);
                    }
                }
                else
                {
                    cell = new PdfPCell();
                    cell.FixedHeight = 20f;
                    cell.Colspan = 2;
                    cell.Border = 0;
                    cell.BackgroundColor = new BaseColor(255, 255, 255);
                }
                table.AddCell(cell);

                table.WriteSelectedRows(0, -1, 0, document.PageSize.Height, writer.DirectContent);
            }
        }

        public class MyMobilePageEventHanndler : PdfPageEventHelper
        {
            private readonly string _webSiteUrl;
            private readonly string _imageFolderPath;

            public MyMobilePageEventHanndler(string webSiteUrl,
                 string imageFolderPath)
            {
                this._webSiteUrl = webSiteUrl;
                this._imageFolderPath = imageFolderPath;
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                var table = new PdfPTable(1);
                table.TotalWidth = document.PageSize.Width;
                table.WidthPercentage = 100f;
                table.SetWidths(new float[] { 100f });
                table.LockedWidth = true;
                table.HorizontalAlignment = 0;
                FontFactory.Register(_imageFolderPath + @"\arial.ttf", "Arial");
                Font headerWhiteFont = FontFactory.GetFont("Arial", 22, Font.NORMAL, new BaseColor(255, 255, 255));
                Font simpleWhiteFont = FontFactory.GetFont("Arial", 16, Font.NORMAL, new BaseColor(255, 255, 255));
                Font simpleYellowFont = FontFactory.GetFont("Arial", 16, Font.NORMAL, new BaseColor(206, 208, 130));
                var image = Image.GetInstance(_imageFolderPath + @"\bro_iphone.jpg");
                image.ScaleToFit(350, 100);
                var chunk = new Chunk(image, 0, 50, false);
                //chunk.SetBackground(new BaseColor(176, 219, 194));
                var anchor = new Anchor(chunk);
                anchor.Reference = _webSiteUrl;
                var cell = new PdfPCell(anchor);
                cell.FixedHeight = 180f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_BOTTOM;
                cell.BackgroundColor = new BaseColor(156, 220, 194);
                //cell.AddElement(anchor);
                cell.Border = 0;
                table.AddCell(cell);

                table.WriteSelectedRows(0, -1, 0, 180, writer.DirectContent);
            }
        }


        public void GenerateProductPdf(int productId, int languageId, MemoryStream stream)
        {
            throw new NotImplementedException();
        }

        public void GenerateMobileProductPdf(int productId, int languageId, MemoryStream stream, string imageFolderpath)
        {
            var product = _productService.GetProductById(productId);
            string productTitle = product.GetLocalized(x => x.Name, languageId);
            string productUrl = product.GetSeName(languageId);
            if (_workContext.CurrentMiniSite != null)
            {
                productUrl = "http://" + _workContext.CurrentMiniSite.DomainName + "/" + WebUtility.UrlEncode(productUrl);
            }
            else
            {
                productUrl = _storeInformationSettings.StoreUrl + WebUtility.UrlEncode(productUrl);
            }
            var languages = new OrderedLanguageCultures();
            string companyName = product.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, languageId);
            string companyLink = "";
            if (companyName == null)
            {
                for (int k = 0; k < languages.Cultures.Count; k++)
                {
                    var langid = _languageService.GetAllLanguages().Where(x => x.LanguageCulture == languages.Cultures[k]).FirstOrDefault().Id;
                    companyName = product.Customer.CompanyInformation.GetLocalized(x => x.CompanyName, langid, false);
                    if (companyName != null)
                    {
                        companyLink = product.Customer.CompanyInformation.GetSeName(langid);
                        companyLink = _storeInformationSettings.StoreUrl + WebUtility.UrlEncode(companyLink);
                        break;
                    }
                }
            }
            else
            {
                companyLink = product.Customer.CompanyInformation.GetSeName(languageId);
                companyLink = _storeInformationSettings.StoreUrl + WebUtility.UrlEncode(companyLink);
            }

            string rootUrl = _storeInformationSettings.StoreUrl;
            var pageHeaderEventHandler = new MyMobilePageEventHanndler(rootUrl, imageFolderpath);
            var picturesCount = product.ProductPictures.Count;
            Document document = new Document(PageSize.A4, 0f, 0f, 0f, 180f);
            var writer = PdfWriter.GetInstance(document, stream);
            writer.PageEvent = pageHeaderEventHandler;
            document.Open();

            var table = new PdfPTable(1);
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.TotalWidth = document.PageSize.Width;
            table.WidthPercentage = 100f;
            table.SetWidths(new float[] { 100f});
            table.LockedWidth = true;
            table.HorizontalAlignment = 0;
            FontFactory.Register(imageFolderpath + @"\arial.ttf", "Arial");
            Font headerBlackFont = FontFactory.GetFont("Arial", BaseFont.IDENTITY_H, 30, Font.NORMAL, new BaseColor(0, 0, 0));
            Font simpleBlackFont = FontFactory.GetFont("Arial", BaseFont.IDENTITY_H, 26, Font.NORMAL, new BaseColor(0, 0, 0));
            Font simpleRedFont = FontFactory.GetFont("Arial", BaseFont.IDENTITY_H, 26, Font.NORMAL, new BaseColor(235, 60, 85));
            Anchor anchor = new Anchor(productTitle, headerBlackFont);
            anchor.Reference = productUrl;
            var cell = new PdfPCell(anchor);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = 0;
            cell.PaddingLeft = 40f;
            cell.PaddingRight = 40f;
            cell.PaddingTop = 40f;
            cell.PaddingBottom = 20f;
            cell.BackgroundColor = new BaseColor(255, 255, 255);
            table.AddCell(cell);

            Paragraph paragraph = new Paragraph();
            Phrase phrase = new Phrase();
            phrase.Add(new Chunk("" + " ", simpleRedFont));
            phrase.Add(new Chunk("by ", simpleBlackFont));
            anchor = new Anchor(companyName, simpleRedFont);
            anchor.Reference = companyLink;
            //phrase.Add(anchor);
            paragraph.Add(phrase);
            paragraph.Add(anchor);
            cell = new PdfPCell(paragraph);
            cell.BackgroundColor = new BaseColor(255, 255, 255);
            cell.PaddingLeft = 40f;
            cell.PaddingRight = 40f;
            cell.PaddingBottom = 20f;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            document.Add(table);
            if (picturesCount > 0)
            {
                var picturesIds = product.ProductPictures.Select(x => x.PictureId).ToList();
                var imageTable = new PdfPTable(1);
                imageTable.HorizontalAlignment = Element.ALIGN_CENTER;
                imageTable.TotalWidth = document.PageSize.Width;
                imageTable.WidthPercentage = 100f;
                imageTable.SetWidths(new float[] { 100f });
                imageTable.LockedWidth = true;
                imageTable.HorizontalAlignment = 0;
                imageTable.SpacingAfter = 20f;
                for (int i = 0; i < picturesCount; i += 1)
                {
                    cell = new PdfPCell();
                    cell.Border = 0;
                    cell.PaddingTop = 30f;
                    cell.PaddingBottom = 30f;
                    cell.BackgroundColor = new BaseColor(211, 238, 226);
                    PutPicInCellMobile(picturesIds[i], 404, cell, (int)document.PageSize.Width, true);
                    imageTable.AddCell(cell);
                }
                document.Add(imageTable);
                document.NewPage();
            }

            var attributeTable = new PdfPTable(2);
            attributeTable.HorizontalAlignment = Element.ALIGN_LEFT;
            attributeTable.TotalWidth = document.PageSize.Width;
            attributeTable.WidthPercentage = 80f;
            attributeTable.SetWidths(new float[] { 40f, 60f });
            attributeTable.LockedWidth = true;
            attributeTable.HorizontalAlignment = 0;
            attributeTable.SpacingBefore = 50f;

            FontFactory.Register(imageFolderpath + @"\arial.ttf", "Arial");
            Font simpleBlackFontSmall = FontFactory.GetFont("Arial", BaseFont.IDENTITY_H, 26, Font.NORMAL, new BaseColor(0, 0, 0));
            Font boldRedFont = FontFactory.GetFont("Arial", BaseFont.IDENTITY_H, 26, Font.BOLD, new BaseColor(255, 42, 72));
            //Price processing
            var prices = _productPriceService.GetAllProductPrices(product.Id);
            var currencies = _currencyService.GetAllCurrencies().Where(c => c.Published).ToList();
            var prices_to_delete = prices.Where(p => !currencies.Contains(p.Currency)).ToList();
            prices = prices.Where(p => currencies.Contains(p.Currency)).OrderBy(p => p.Currency.Name).ToList();
            foreach (var p in prices_to_delete)
                _productPriceService.DeleteProductPriceById(p.Id);
            StringBuilder priceBuilder = new StringBuilder();
            if (prices.Where(x => x.Price != 0).Count() > 0)
            {
                cell = new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                cell.PaddingLeft = 40f;
                cell.AddElement(new Paragraph(_localizationService.GetResource("ETF.Front.Product.Add.ProductPrices") + ":", simpleBlackFontSmall));
                attributeTable.AddCell(cell);

                bool first = true;
                paragraph = new Paragraph();
                phrase = new Phrase();
                foreach (var price in prices)
                {
                    if (price.Price == 0)
                        continue;
                    if (first)
                    {
                        if (price.Currency.CurrencyCode.ToLower() != "byr")
                        {
                            phrase.Add(new Chunk(price.Price.ToString("F2") + " ", boldRedFont));
                            phrase.Add(new Chunk(price.Currency.CurrencyCode.ToUpper(), simpleBlackFontSmall));
                            first = false;
                        }
                        else
                        {
                            phrase.Add(new Chunk(price.Price.ToString("F0") + " ", boldRedFont));
                            phrase.Add(new Chunk(price.Currency.CurrencyCode.ToUpper(), simpleBlackFontSmall));
                            first = false;
                        }
                    }
                    else
                    {
                        if (price.Currency.CurrencyCode.ToLower() != "byr")
                        {
                            phrase.Add(new Chunk("\n" + price.Price.ToString("F2") + " ", boldRedFont));
                            phrase.Add(new Chunk(price.Currency.CurrencyCode.ToUpper(), simpleBlackFontSmall));
                        }
                        else
                        {
                            phrase.Add(new Chunk("\n" + price.Price.ToString("F0") + " ", boldRedFont));
                            phrase.Add(new Chunk(price.Currency.CurrencyCode.ToUpper(), simpleBlackFontSmall));
                        }
                    }
                }
                paragraph.Add(phrase);
                paragraph.SetLeading(30f, 0);
                cell = new PdfPCell();
                cell.Border = 0;
                cell.PaddingRight = 40f;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                attributeTable.AddCell(cell);
            }


            //Attributes processing
            List<CategoryProductAttributeGroup> _attrGroups = product.ProductAttributes.Select(x => x.CategoryProductAttribute.CategoryProductGroup).Distinct().ToList();
            List<AttributesModel> DisplayedAttributes = new List<AttributesModel>();
            foreach (var _aG in _attrGroups)
            {
                foreach (var cpa in _aG.CategoryProductAttributes)
                {
                    AttributesModel cam = new AttributesModel();
                    cam.Values = new List<CategoryProductAttributeValue>();
                    cam.Values.AddRange(cpa.CategoryProductAttributeValues.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToList());
                    if (cpa.AttributeControlType != AttributeControlType.TextBox)
                    {
                        cam.Name = cpa.ProductAttribute == null ? null : cpa.ProductAttribute.GetLocalized(x => x.Name, languageId);
                    }
                    else
                    {
                        cam.Name = cpa.ProductAttribute == null ? null : cpa.ProductAttribute.Name;
                    }

                    cam.ControlType = cpa.AttributeControlType;
                    foreach (var val in cam.Values)
                    {
                        val.IsPreSelected = product.ProductAttributes.Where(p => p.Id == val.Id).Count() > 0;
                    }
                    cam.SelectedValue = cam.Values.Where(z => z.IsPreSelected).FirstOrDefault();
                    DisplayedAttributes.Add(cam);
                }
            }

            DisplayedAttributes = DisplayedAttributes.Where(x => x.Name != null && ((x.SelectedValue != null && (x.ControlType == AttributeControlType.DropdownList || x.ControlType == AttributeControlType.TextBox)) || x.ControlType == AttributeControlType.Checkboxes)).ToList();
            var sb = new StringBuilder();
            for (int i = 0; i < DisplayedAttributes.Count; i++)
            {
                var attribute = DisplayedAttributes[i];
                cell = new PdfPCell();
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.AddElement(new Paragraph(attribute.Name + ":", simpleBlackFontSmall));
                cell.Border = 0;
                cell.PaddingLeft = 40f;
                attributeTable.AddCell(cell);

                //gfx.DrawString(attribute.Name + ":", boldFont, XBrushes.Black,
                //new XRect(20, yposition + i * 20, 80, 20),
                //XStringFormats.TopLeft);
                switch (attribute.ControlType)
                {
                    case AttributeControlType.Checkboxes:
                        {
                            for (int j = 0; j < attribute.Values.Count; j++)
                            {
                                //if(attribute.Values[j])
                                if (attribute.Values[j].IsPreSelected)
                                    sb.Append(attribute.Values[j].GetLocalized(x => x.Name, languageId) + ", ");
                            }
                            if (sb.Length > 0)
                            {
                                sb.Remove(sb.Length - 2, 2);
                            }
                            //gfx.DrawString(sb.ToString(), font, XBrushes.Black,
                            //new XRect(150, yposition + i * 20, page.Width - 185, 20),
                            //XStringFormats.TopLeft);
                            cell = new PdfPCell();
                            cell.VerticalAlignment = Element.ALIGN_TOP;
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.AddElement(new Paragraph(sb.ToString(), simpleBlackFontSmall));
                            cell.Border = 0;
                            cell.PaddingRight = 40f;
                            attributeTable.AddCell(cell);
                            sb.Clear();
                            break;
                        }
                    case AttributeControlType.DropdownList:
                        {
                            cell = new PdfPCell();
                            cell.VerticalAlignment = Element.ALIGN_TOP;
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.AddElement(new Paragraph(attribute.SelectedValue.GetLocalized(x => x.Name, languageId), simpleBlackFontSmall));
                            cell.Border = 0;
                            cell.PaddingRight = 40f;
                            attributeTable.AddCell(cell);
                            break;
                        }
                    case AttributeControlType.TextBox:
                        {
                            cell = new PdfPCell();
                            cell.VerticalAlignment = Element.ALIGN_TOP;
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.AddElement(new Paragraph(attribute.SelectedValue.Name, simpleBlackFontSmall));
                            cell.Border = 0;
                            cell.PaddingRight = 40f;
                            attributeTable.AddCell(cell);
                            break;
                        }
                }
            }

            document.Add(attributeTable);

            string full = product.GetLocalized(x => x.FullDescription, languageId);
            full = WebUtility.HtmlDecode(full);
            full = full.Replace("\n", "");
            full = full.Replace("</br>", "\n");
            full = full.Replace("<br />", "\n");
            full = full.Replace("<p>", "\t");
            //full = full.Replace("<ul>", "\n");
            full = full.Replace("<li>", "\t - ");
            full = full.Replace("<tr>", "\n");
            full = full.Replace("</td>", "\t");
            int index = 0;
            while (full.IndexOf("<") >= 0)
            {
                index = full.IndexOf("<");
                full = full.Remove(index, full.IndexOf(">", index) - index + 1);
            }

            Font simpleBlackBoldFont = FontFactory.GetFont("Arial", BaseFont.IDENTITY_H, 26, Font.BOLD, new BaseColor(0, 0, 0));
            var paragraphFull = new Paragraph(_localizationService.GetResource("ETF.Full"), simpleBlackBoldFont);
            paragraphFull.IndentationLeft = 40f;
            paragraphFull.SpacingBefore = 40f;
            paragraphFull.SpacingAfter = 0f;
            document.Add(paragraphFull);

            paragraphFull = new Paragraph(full, simpleBlackFontSmall);
            paragraphFull.IndentationLeft = 40f;
            paragraphFull.IndentationRight = 40f;
            paragraphFull.SpacingBefore = 10f;
            

            string orderingComments = product.GetLocalized(x => x.AdminComment, languageId, false);
            if (!String.IsNullOrEmpty(orderingComments))
            {
                paragraphFull.SpacingAfter = 10f;
                document.Add(paragraphFull);

                orderingComments = WebUtility.HtmlDecode(orderingComments);
                orderingComments = orderingComments.Replace("\n", "");
                orderingComments = orderingComments.Replace("</br>", "\n");
                orderingComments = orderingComments.Replace("<br />", "\n");
                orderingComments = orderingComments.Replace("<p>", "\t");
                //orderingComments = orderingComments.Replace("<ul>", "\n");
                orderingComments = orderingComments.Replace("<li>", "\t - ");
                orderingComments = orderingComments.Replace("<tr>", "\n");
                orderingComments = orderingComments.Replace("</td>", "\t");
                index = 0;
                while (orderingComments.IndexOf("<") >= 0)
                {
                    index = orderingComments.IndexOf("<");
                    orderingComments = orderingComments.Remove(index, orderingComments.IndexOf(">", index) - index + 1);
                }
                paragraphFull = new Paragraph(_localizationService.GetResource("ETF.OrderingComments"), simpleBlackBoldFont);
                paragraphFull.IndentationLeft = 40f;
                paragraphFull.SpacingBefore = 10f;
                document.Add(paragraphFull);

                paragraphFull = new Paragraph(orderingComments, simpleBlackFontSmall);
                paragraphFull.IndentationLeft = 40f;
                paragraphFull.IndentationRight = 40f;
                paragraphFull.SpacingBefore = 10f;
                paragraphFull.SpacingAfter = 0f;
                document.Add(paragraphFull);
            }
            else
            {
                paragraphFull.SpacingAfter = 0f;
                document.Add(paragraphFull);
            }
            document.Close();
        }

    }
}
