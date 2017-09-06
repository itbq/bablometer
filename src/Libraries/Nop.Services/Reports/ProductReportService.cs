using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Services.BannerService;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Reports
{
    public class ProductReportService : IProductReportsService
    {
        private readonly IProductService _productService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;
        private readonly IBannerService _bannerService;
        private readonly IWorkContext _workContext;

        public ProductReportService(IProductService productService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ICustomerService customerService,
            IBannerService bannerService,
            IWorkContext workContext)
        {
            this._productService = productService;
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._customerService = customerService;
            this._bannerService = bannerService;
            this._workContext = workContext;
        }

        private void GenerateProductsOrdersReport(DateTime? startDate, DateTime? endDate, Stream stream)
        {
            var logType = _customerActivityService.GetActivityTypeBySystmeName("PublicStore.PlaceOrder");
            var logs = _customerActivityService.GetAllActivities(startDate, endDate, null, logType.Id, 0, 0);
            ExcelPackage pck = new ExcelPackage(stream);
            int i = 0;
            var ws = pck.Workbook.Worksheets.Add("Report");
            var propertis = new List<string>()
            {
                _localizationService.GetResource("ITBSFA.Reports.Product.ProductId"),
                _localizationService.GetResource("ITBSFA.Reports.Product.ProductName"),
                _localizationService.GetResource("ITBSFA.Reports.Product.CategoryName"),
                _localizationService.GetResource("ITBSFA.Reports.Product.BankName"),
                _localizationService.GetResource("ITBSFA.Reports.ProductOrder.Count"),
            };

            ws.DefaultColWidth = Pixel2ColumnWidth(ws, 250);
            for (i = 0; i < propertis.Count; i++)
            {
                ws.Cells[1, i + 1].Value = propertis[i];
            }

            var productLogs = logs.Where(x => x.ReferenceId.HasValue).GroupBy(x => x.ReferenceId.Value);
            int j = 2;
            foreach (var productLog in productLogs)
            {
                var product = _productService.GetProductById(productLog.Key);
                int count = productLog.Count();
                if (count > 0)
                {
                    ws.Cells[j, 1].Value = product.Id;
                    ws.Cells[j, 2].Value = product.Name;
                    ws.Cells[j, 3].Value = product.ProductCategories.First().Category.Name;
                    ws.Cells[j, 4].Value = product.Customer.CompanyName;
                    ws.Cells[j, 5].Value = count;
                    j++;
                }
            }

            pck.Save();
        }
        private void GenerateProductOrderReport(DateTime? startDate, DateTime? endDate, Stream stream, int productId)
        {
            var logType = _customerActivityService.GetActivityTypeBySystmeName("PublicStore.PlaceOrder");
            var logs = _customerActivityService.GetAllActivities(startDate, endDate, null, logType.Id, 0, 0).Where(x => x.ReferenceId == productId);
            ExcelPackage pck = new ExcelPackage(stream);
            int i = 0;
            var ws = pck.Workbook.Worksheets.Add("Report");
            var propertis = new List<string>()
            {
                _localizationService.GetResource("ITBSFA.Reports.Product.ProductId"),
                _localizationService.GetResource("ITBSFA.Reports.Product.ProductName"),
                _localizationService.GetResource("ITBSFA.Reports.Product.CategoryName"),
                _localizationService.GetResource("ITBSFA.Reports.Product.BankName"),
                _localizationService.GetResource("ITBSFA.Reports.Product.Customer.Email"),
                _localizationService.GetResource("ITBSFA.Report.ProductOrder.EventTime"),
                _localizationService.GetResource("ITBSFA.Report.ProductView.ReferenceUrl"),
            };

            ws.DefaultColWidth = Pixel2ColumnWidth(ws, 250);
            for (i = 0; i < propertis.Count; i++)
            {
                ws.Cells[1, i + 1].Value = propertis[i];
            }

            int j = 2;
            ws.Column(6).Style.Numberformat.Format = "dd-MM-yyyy h:mm";
            foreach (var productLog in logs)
            {
                var product = _productService.GetProductById(productLog.ReferenceId.Value);
                ws.Cells[j, 1].Value = product.Id;
                ws.Cells[j, 2].Value = product.Name;
                ws.Cells[j, 3].Value = product.ProductCategories.First().Category.Name;
                ws.Cells[j, 4].Value = product.Customer.CompanyName;
                ws.Cells[j, 5].Value = productLog.Customer == null ? "unauthorized" : productLog.Customer.Email;
                ws.Cells[j, 6].Value = productLog.CreatedOnUtc;
                ws.Cells[j, 7].Value = productLog.ReferenceUrl;
                j++;
            }

            pck.Save();
        }

        private void GenerateProductViewsReport(DateTime? startDate, DateTime? endDate, Stream stream, int productId)
        {
            var logType = _customerActivityService.GetActivityTypeBySystmeName("PublicStore.ViewProduct");
            var logs = _customerActivityService.GetAllActivities(startDate, endDate, null, logType.Id, 0, 0).Where(x=>x.ReferenceId == productId);
            ExcelPackage pck = new ExcelPackage(stream);
            int i = 0;
            var ws = pck.Workbook.Worksheets.Add("Report");
            var propertis = new List<string>()
            {
                _localizationService.GetResource("ITBSFA.Reports.Product.ProductId"),
                _localizationService.GetResource("ITBSFA.Reports.Product.ProductName"),
                _localizationService.GetResource("ITBSFA.Reports.Product.CategoryName"),
                _localizationService.GetResource("ITBSFA.Reports.Product.BankName"),
                _localizationService.GetResource("ITBSFA.Reports.Product.Customer.Email"),
                _localizationService.GetResource("ITBSFA.Report.ProductView.EventTime"),
                _localizationService.GetResource("ITBSFA.Report.ProductView.ReferenceUrl"),
            };

            ws.DefaultColWidth = Pixel2ColumnWidth(ws, 250);
            for (i = 0; i < propertis.Count; i++)
            {
                ws.Cells[1, i + 1].Value = propertis[i];
            }

            int j = 2;
            ws.Column(6).Style.Numberformat.Format = "dd-MM-yyyy h:mm"; 
            foreach (var productLog in logs)
            {
                var product = _productService.GetProductById(productLog.ReferenceId.Value);
                ws.Cells[j, 1].Value = product.Id;
                ws.Cells[j, 2].Value = product.Name;
                ws.Cells[j, 3].Value = product.ProductCategories.First().Category.Name;
                ws.Cells[j, 4].Value = product.Customer.CompanyName;
                ws.Cells[j, 5].Value = productLog.Customer == null ? "unauthorized" : productLog.Customer.Email;
                ws.Cells[j, 6].Value = productLog.CreatedOnUtc;
                ws.Cells[j, 7].Value = productLog.ReferenceUrl;
                j++;
            }

            pck.Save();
        }

        private void GenerateProductsViewsReport(DateTime? startDate, DateTime? endDate, Stream stream)
        {
            var logType = _customerActivityService.GetActivityTypeBySystmeName("PublicStore.ViewProduct");
            var logs = _customerActivityService.GetAllActivities(startDate, endDate, null, logType.Id, 0, 0);
            ExcelPackage pck = new ExcelPackage(stream);
            int i = 0;
            var ws = pck.Workbook.Worksheets.Add("Reports");
            var propertis = new List<string>()
            {
                _localizationService.GetResource("ITBSFA.Reports.Product.ProductId"),
                _localizationService.GetResource("ITBSFA.Reports.Product.ProductName"),
                _localizationService.GetResource("ITBSFA.Reports.Product.CategoryName"),
                _localizationService.GetResource("ITBSFA.Reports.Product.BankName"),
                _localizationService.GetResource("ITBSFA.Reports.Product.ProductViewCount"),
            };

            ws.DefaultColWidth = Pixel2ColumnWidth(ws, 250);
            for (i = 0; i < propertis.Count; i++)
            {
                ws.Cells[1, i + 1].Value = propertis[i];
            }

            var productLogs = logs.Where(x=>x.ReferenceId.HasValue).GroupBy(x => x.ReferenceId.Value);
            int j = 2;
            foreach (var productLog in productLogs)
            {
                var product = _productService.GetProductById(productLog.Key);
                int count = productLog.Count();
                if (count > 0)
                {
                    ws.Cells[j, 1].Value = product.Id;
                    ws.Cells[j, 2].Value = product.Name;
                    ws.Cells[j, 3].Value = product.ProductCategories.First().Category.Name;
                    ws.Cells[j, 4].Value = product.Customer.CompanyName;
                    ws.Cells[j, 5].Value = count;
                    j++;
                }
            }

            pck.Save();
        }


        private void GenerateUserAuthorizationReport(DateTime? startDate, DateTime? endDate, Stream stream)
        {
            var logType = _customerActivityService.GetActivityTypeBySystmeName("PublicStore.Login");
            var logs = _customerActivityService.GetAllActivities(startDate, endDate, null, logType.Id, 0, 0);
            var logTypeLogout = _customerActivityService.GetActivityTypeBySystmeName("PublicStore.Logout");
            var logsLogout = _customerActivityService.GetAllActivities(startDate, endDate, null, logTypeLogout.Id, 0, 0);
            ExcelPackage pck = new ExcelPackage(stream);
            int i = 0;
            var ws = pck.Workbook.Worksheets.Add("Report");
            var propertis = new List<string>()
            {
                _localizationService.GetResource("ITBSFA.Reports.UserAuthorization.Id"),
                _localizationService.GetResource("ITBSFA.Reports.UserAutorization.email"),
                _localizationService.GetResource("ITBSFA.Reports.UserAuthorization.lastAuthorization"),
                _localizationService.GetResource("ITBSFA.Reports.UserAuthorization.authorizationCount"),
                _localizationService.GetResource("ITBSFA.Reports.UserAuthorization.lastLogOut"),
            };

            ws.DefaultColWidth = Pixel2ColumnWidth(ws, 250);
            for (i = 0; i < propertis.Count; i++)
            {
                ws.Cells[1, i + 1].Value = propertis[i];
            }

            ws.Column(3).Style.Numberformat.Format = "dd-MM-yyyy h:mm";
            ws.Column(5).Style.Numberformat.Format = "dd-MM-yyyy h:mm";
            var userLogs = logs.Where(x => x.CustomerId.HasValue).GroupBy(x => x.CustomerId.Value);
            var userLogsLogout = logsLogout.Where(x => x.CustomerId.HasValue).GroupBy(x => x.CustomerId.Value);
            int j = 2;
            foreach (var userLog in userLogs)
            {
                if (userLogsLogout.FirstOrDefault(x => x.Key == userLog.Key) != null)
                {
                    var LastLogoutDateUtc = userLogsLogout.FirstOrDefault(x => x.Key == userLog.Key).OrderByDescending(x => x.CreatedOnUtc).Select(z => z.CreatedOnUtc).First();
                    ws.Cells[j, 5].Value = LastLogoutDateUtc;
                }
                else
                {
                    ws.Cells[j, 5].Value = null;
                }
                var user = _customerService.GetCustomerById(userLog.Key);
                ws.Cells[j, 1].Value = userLog.Key;
                if (String.IsNullOrEmpty(user.Email))
                    user.Email = "Unknown";
                ws.Cells[j, 2].Value = user.Email;
                ws.Cells[j, 3].Value = user.LastLoginDateUtc;
                ws.Cells[j, 4].Value = userLog.Count();
                j++;
            }


            pck.Save();
        }

        /// <summary>
        /// Single banner click report
        /// </summary>
        /// <param name="startDate">start date</param>
        /// <param name="endDate"> end date</param>
        /// <param name="stream">stream where to save results</param>
        /// <param name="bannerId">banner id</param>
        private void GenerateBannerClickReport(DateTime? startDate, DateTime? endDate, Stream stream, int bannerId)
        {
            var logType = _customerActivityService.GetActivityTypeBySystmeName("PublicStore.BannerClick");
            var logs = _customerActivityService.GetAllBannerActivities(startDate, endDate, null, logType.Id, 0, 0,bannerId);
            ExcelPackage pck = new ExcelPackage(stream);
            int i = 0;
            var ws = pck.Workbook.Worksheets.Add("Report");
            var propertis = new List<string>()
            {
                _localizationService.GetResource("ITBSFA.Admin.Banner.Id"),
                _localizationService.GetResource("ITBSFA.Admin.Reports.Banner.BannerType"),
                _localizationService.GetResource("ITBSFA.Admin.Reports.Banner.Customer"),
                _localizationService.GetResource("ITBSFA.Admin.Reporst.Banner.CurrenPageUrl"),
                _localizationService.GetResource("ITBSFA.Admin.Reporst.Banner.UrlReferrer"),
                _localizationService.GetResource("ITBSFA.Admin.Reports.Banner.Event"),
            };

            ws.DefaultColWidth = Pixel2ColumnWidth(ws, 250);
            for (i = 0; i < propertis.Count; i++)
            {
                ws.Cells[1, i + 1].Value = propertis[i];
            }

            int j = 2;
            ws.Column(6).Style.Numberformat.Format = "dd-MM-yyyy h:mm";
            foreach (var bannerLog in logs)
            {
                var banner = _bannerService.GetBannerById(bannerLog.ReferenceId.Value);
                string bannerType = banner.BannerType.GetLocalizedEnum(_localizationService, _workContext.WorkingLanguage.Id);
                ws.Cells[j, 1].Value = banner.Id;
                ws.Cells[j, 2].Value = bannerType;
                ws.Cells[j, 3].Value = bannerLog.Customer == null ? "unauthorized" : bannerLog.Customer.Email;
                ws.Cells[j, 4].Value = bannerLog.CurrentUrl;
                ws.Cells[j, 5].Value = bannerLog.ReferenceUrl;
                ws.Cells[j, 6].Value = bannerLog.CreatedOnUtc;
                j++;
            }

            pck.Save();
        }

        /// <summary>
        /// Banners clicks report
        /// </summary>
        /// <param name="startDate">start date</param>
        /// <param name="endDate">end date</param>
        /// <param name="stream">stream where to save results</param>
        private void GenerateBannerClicksReport(DateTime? startDate, DateTime? endDate, Stream stream)
        {
            var logType = _customerActivityService.GetActivityTypeBySystmeName("PublicStore.BannerClick");
            var logs = _customerActivityService.GetAllActivities(startDate, endDate, null, logType.Id, 0, 0);
            ExcelPackage pck = new ExcelPackage(stream);
            int i = 0;
            var ws = pck.Workbook.Worksheets.Add("Report");
            var propertis = new List<string>()
            {
                _localizationService.GetResource("ITBSFA.Admin.Banner.Id"),
                _localizationService.GetResource("ITBSFA.Admin.Reports.Banner.BannerType"),
                _localizationService.GetResource("ITBSFA.Admin.Reports.Banner.Count.Clicks"),
            };

            ws.DefaultColWidth = Pixel2ColumnWidth(ws, 250);
            for (i = 0; i < propertis.Count; i++)
            {
                ws.Cells[1, i + 1].Value = propertis[i];
            }

            var productLogs = logs.Where(x => x.ReferenceId.HasValue).GroupBy(x => x.ReferenceId.Value);
            int j = 2;
            foreach (var bannerLog in productLogs)
            {
                var banner = _bannerService.GetBannerById(bannerLog.Key);
                string bannerType = banner.BannerType.GetLocalizedEnum(_localizationService, _workContext.WorkingLanguage.Id);
                int count = bannerLog.Count();
                if (count > 0)
                {
                    ws.Cells[j, 1].Value = banner.Id;
                    ws.Cells[j, 2].Value = bannerType;
                    ws.Cells[j, 3].Value = count;
                    j++;
                }
            }

            pck.Save();
        }

        /// <summary>
        /// Single banner view report
        /// </summary>
        /// <param name="startDate">start date</param>
        /// <param name="endDate"> end date</param>
        /// <param name="stream">stream where to save results</param>
        /// <param name="bannerId">banner id</param>
        private void GenerateBannerViewReport(DateTime? startDate, DateTime? endDate, Stream stream, int bannerId)
        {
            var logType = _customerActivityService.GetActivityTypeBySystmeName("PublicStore.BannerView");
            var logs = _customerActivityService.GetAllBannerActivities(startDate, endDate, null, logType.Id, 0, 0, bannerId);
            ExcelPackage pck = new ExcelPackage(stream);
            int i = 0;
            var ws = pck.Workbook.Worksheets.Add("Report");
            var propertis = new List<string>()
            {
                _localizationService.GetResource("ITBSFA.Admin.Banner.Id"),
                _localizationService.GetResource("ITBSFA.Admin.Reports.Banner.BannerType"),
                _localizationService.GetResource("ITBSFA.Admin.Reports.Banner.Customer"),
                _localizationService.GetResource("ITBSFA.Admin.Reporst.Banner.CurrenPageUrl"),
                _localizationService.GetResource("ITBSFA.Admin.Reporst.Banner.UrlReferrer"),
                _localizationService.GetResource("ITBSFA.Admin.Reports.Banner.Event"),
            };

            ws.DefaultColWidth = Pixel2ColumnWidth(ws, 250);
            for (i = 0; i < propertis.Count; i++)
            {
                ws.Cells[1, i + 1].Value = propertis[i];
            }

            int j = 2;
            ws.Column(6).Style.Numberformat.Format = "dd-MM-yyyy h:mm";
            foreach (var bannerLog in logs)
            {
                var banner = _bannerService.GetBannerById(bannerLog.ReferenceId.Value);
                string bannerType = banner.BannerType.GetLocalizedEnum(_localizationService, _workContext.WorkingLanguage.Id);
                ws.Cells[j, 1].Value = banner.Id;
                ws.Cells[j, 2].Value = bannerType;
                ws.Cells[j, 3].Value = bannerLog.Customer == null ? "unauthorized" : bannerLog.Customer.Email;
                ws.Cells[j, 4].Value = bannerLog.CurrentUrl;
                ws.Cells[j, 5].Value = bannerLog.ReferenceUrl;
                ws.Cells[j, 6].Value = bannerLog.CreatedOnUtc;
                j++;
            }

            pck.Save();
        }

        /// <summary>
        /// Banners views report
        /// </summary>
        /// <param name="startDate">start date</param>
        /// <param name="endDate">end date</param>
        /// <param name="stream">stream where to save results</param>
        private void GenerateBannerViewsReport(DateTime? startDate, DateTime? endDate, Stream stream)
        {
            var logType = _customerActivityService.GetActivityTypeBySystmeName("PublicStore.BannerView");
            var logs = _customerActivityService.GetAllActivities(startDate, endDate, null, logType.Id, 0, 0);
            ExcelPackage pck = new ExcelPackage(stream);
            int i = 0;
            var ws = pck.Workbook.Worksheets.Add("Report");
            var propertis = new List<string>()
            {
                _localizationService.GetResource("ITBSFA.Admin.Banner.Id"),
                _localizationService.GetResource("ITBSFA.Admin.Reports.Banner.BannerType"),
                _localizationService.GetResource("ITBSFA.Admin.Reports.Banner.Count.Views"),
            };

            ws.DefaultColWidth = Pixel2ColumnWidth(ws, 250);
            for (i = 0; i < propertis.Count; i++)
            {
                ws.Cells[1, i + 1].Value = propertis[i];
            }

            var productLogs = logs.Where(x => x.ReferenceId.HasValue).GroupBy(x => x.ReferenceId.Value);
            int j = 2;
            foreach (var bannerLog in productLogs)
            {
                var banner = _bannerService.GetBannerById(bannerLog.Key);
                string bannerType = banner.BannerType.GetLocalizedEnum(_localizationService, _workContext.WorkingLanguage.Id);
                int count = bannerLog.Count();
                if (count > 0)
                {
                    ws.Cells[j, 1].Value = banner.Id;
                    ws.Cells[j, 2].Value = bannerType;
                    ws.Cells[j, 3].Value = count;
                    j++;
                }
            }

            pck.Save();
        }

        public void GenerateReport(ReportTypesEnum reportType, int referenceId, Stream stream, DateTime? startDate, DateTime? endDate)
        {
            switch (reportType)
            {
                case ReportTypesEnum.ProductViews:
                    {
                        GenerateProductsViewsReport(startDate, endDate, stream);
                        break;
                    }
                case ReportTypesEnum.UserAutorization:
                    {
                        GenerateUserAuthorizationReport(startDate, endDate, stream);
                        break;
                    }
                case ReportTypesEnum.ProductView:
                    {
                        GenerateProductViewsReport(startDate, endDate, stream, referenceId);
                        break;
                    }
                case ReportTypesEnum.ProductOrders:
                    {
                        GenerateProductsOrdersReport(startDate, endDate, stream);
                        break;
                    }
                case ReportTypesEnum.ProductOrder:
                    {
                        GenerateProductOrderReport(startDate, endDate, stream, referenceId);
                        break;
                    }
                case ReportTypesEnum.BannerView:
                    {
                        GenerateBannerViewReport(startDate, endDate, stream, referenceId);
                        break;
                    }
                case ReportTypesEnum.BannerViews:
                    {
                        GenerateBannerViewsReport(startDate, endDate, stream);
                        break;
                    }
                case ReportTypesEnum.BannerClick:
                    {
                        GenerateBannerClickReport(startDate, endDate, stream, referenceId);
                        break;
                    }
                case ReportTypesEnum.BannerClicks:
                    {
                        GenerateBannerClicksReport(startDate, endDate, stream);
                        break;
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
    }
}
