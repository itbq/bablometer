using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Services.Orders;
using System.IO;
using System.Web;
using Nop.Core.Domain.Media;
using Nop.Services.Customers;

namespace Nop.Web.Controllers
{
    public partial class DownloadController : BaseNopController
    {
        private readonly IDownloadService _downloadService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly ICompanyInformationService _companyInformationService;

        private readonly CustomerSettings _customerSettings;

        public DownloadController(IDownloadService downloadService, IProductService productService,
            IOrderService orderService, IWorkContext workContext, CustomerSettings customerSettings,
            ICompanyInformationService companyInformationService)
        {
            this._downloadService = downloadService;
            this._productService = productService;
            this._orderService = orderService;
            this._workContext = workContext;
            this._customerSettings = customerSettings;
            this._companyInformationService = companyInformationService;
        }
        
        public ActionResult Sample(int productVariantId)
        {
            var productVariant = _productService.GetProductVariantById(productVariantId);
            if (productVariant == null)
                return RedirectToRoute("HomePage");

            if (!productVariant.HasSampleDownload)
                return Content("Product variant doesn't have a sample download.");

            var download = _downloadService.GetDownloadById(productVariant.SampleDownloadId);
            if (download == null)
                return Content("Sample download is not available any more.");

            if (download.UseDownloadUrl)
            {
                return new RedirectResult(download.DownloadUrl);
            }
            else
            {
                if (download.DownloadBinary == null)
                    return Content("Download data is not available any more.");

                string fileName = !String.IsNullOrWhiteSpace(download.Filename) ? download.Filename : productVariant.Id.ToString();
                string contentType = !String.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : "application/octet-stream";
                return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
            }
        }

        public ActionResult GetDownload(Guid opvId, bool agree = false)
        {
            var orderProductVariant = _orderService.GetOrderProductVariantByGuid(opvId);
            if (orderProductVariant == null)
                return RedirectToRoute("HomePage");

            var order = orderProductVariant.Order;
            var productVariant = orderProductVariant.ProductVariant;
            if (!_downloadService.IsDownloadAllowed(orderProductVariant))
                return Content("Downloads are not allowed");

            if (_customerSettings.DownloadableProductsValidateUser)
            {
                if (_workContext.CurrentCustomer == null)
                    return new HttpUnauthorizedResult();

                if (order.CustomerId != _workContext.CurrentCustomer.Id)
                    return Content("This is not your order");
            }

            var download = _downloadService.GetDownloadById(productVariant.DownloadId);
            if (download == null)
                return Content("Download is not available any more.");

            if (productVariant.HasUserAgreement)
            {
                if (!agree)
                    return RedirectToRoute("DownloadUserAgreement", new { opvid = opvId });
            }


            if (!productVariant.UnlimitedDownloads && orderProductVariant.DownloadCount >= productVariant.MaxNumberOfDownloads)
                return Content(string.Format("You have reached maximum number of downloads {0}", productVariant.MaxNumberOfDownloads));
            

            if (download.UseDownloadUrl)
            {
                //increase download
                orderProductVariant.DownloadCount++;
                _orderService.UpdateOrder(order);

                //return result
                return new RedirectResult(download.DownloadUrl);
            }
            else
            {
                if (download.DownloadBinary == null)
                    return Content("Download data is not available any more.");

                //increase download
                orderProductVariant.DownloadCount++;
                _orderService.UpdateOrder(order);

                //return result
                string fileName = !String.IsNullOrWhiteSpace(download.Filename) ? download.Filename : productVariant.Id.ToString();
                string contentType = !String.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : "application/octet-stream";
                return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
            }
        }

        public ActionResult GetLicense(Guid opvId)
        {
            var orderProductVariant = _orderService.GetOrderProductVariantByGuid(opvId);
            if (orderProductVariant == null)
                return RedirectToRoute("HomePage");

            var order = orderProductVariant.Order;
            var productVariant = orderProductVariant.ProductVariant;
            if (!_downloadService.IsLicenseDownloadAllowed(orderProductVariant))
                return Content("Downloads are not allowed");

            if (_customerSettings.DownloadableProductsValidateUser)
            {
                if (_workContext.CurrentCustomer == null)
                    return new HttpUnauthorizedResult();

                if (order.CustomerId != _workContext.CurrentCustomer.Id)
                    return Content("This is not your order");
            }

            var download = _downloadService.GetDownloadById(orderProductVariant.LicenseDownloadId.HasValue ? orderProductVariant.LicenseDownloadId.Value : 0);
            if (download == null)
                return Content("Download is not available any more.");
            
            if (download.UseDownloadUrl)
            {
                //return result
                return new RedirectResult(download.DownloadUrl);
            }
            else
            {
                if (download.DownloadBinary == null)
                    return Content("Download data is not available any more.");
                
                //return result
                string fileName = !String.IsNullOrWhiteSpace(download.Filename) ? download.Filename : productVariant.Id.ToString();
                string contentType = !String.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : "application/octet-stream";
                return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
            }
        }

        public ActionResult GetFileUpload(Guid downloadId)
        {
            var download = _downloadService.GetDownloadByGuid(downloadId);
            if (download == null)
                return Content("Download is not available any more.");

            if (download.UseDownloadUrl)
            {
                //return result
                return new RedirectResult(download.DownloadUrl);
            }
            else
            {
                if (download.DownloadBinary == null)
                    return Content("Download data is not available any more.");

                //return result
                string fileName = !String.IsNullOrWhiteSpace(download.Filename) ? download.Filename : downloadId.ToString();
                string contentType = !String.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : "application/octet-stream";
                return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
            }
        }

        [HttpPost]
        public ActionResult AsyncUpload()
        {
            //we process it distinct ways based on a browser
            //find more info here http://stackoverflow.com/questions/4884920/mvc3-valums-ajax-file-upload
            Stream stream = null;
            var fileName = "";
            var contentType = "";
            if (String.IsNullOrEmpty(Request["qqfile"]))
            {
                // IE
                HttpPostedFileBase httpPostedFile = Request.Files[0];
                if (httpPostedFile == null)
                    throw new ArgumentException("No file uploaded");
                stream = httpPostedFile.InputStream;
                fileName = Path.GetFileName(httpPostedFile.FileName);
                contentType = httpPostedFile.ContentType;
            }
            else
            {
                //Webkit, Mozilla
                stream = Request.InputStream;
                fileName = Request["qqfile"];
            }

            var fileBinary = new byte[stream.Length];
            if (fileBinary.LongLength > 10.48e6)
            {
                return Json(new
                {
                    success = false,
                    id = 0,
                    downloadId = 0,
                    downloadUrl = "",
                    errorString = "File Should be less than 10 mb"
                },
                "text/plain");
            }

            stream.Read(fileBinary, 0, fileBinary.Length);

            var fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            var download = new Download()
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = "",
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = Path.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true,
                FileSize = fileBinary.LongLength,
            };
            _downloadService.InsertDownload(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                id = download.Id,
                downloadId = download.DownloadGuid,
                downloadUrl = Url.Action("DownloadFile", new { downloadId = download.DownloadGuid })
            },
                "text/plain");
        }

        public ActionResult DownloadFile(Guid downloadId)
        {
            var download = _downloadService.GetDownloadByGuid(downloadId);
            if (download == null)
                return Content("No download record found with the specified id");

            if (download.UseDownloadUrl)
            {
                return new RedirectResult(download.DownloadUrl);
            }
            else
            {
                //use stored data
                if (download.DownloadBinary == null)
                    return Content(string.Format("Download data is not available any more. Download ID={0}", downloadId));

                string fileName = !String.IsNullOrWhiteSpace(download.Filename) ? download.Filename : downloadId.ToString();
                string contentType = !String.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : "application/octet-stream";
                return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
            }
        }

        /// <summary>
        /// Delete file by its guid
        /// </summary>
        /// <param name="id">file guid</param>
        /// <returns></returns>
        public ActionResult DeleteFile(Guid id)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return Json(new
                {
                    success = false,
                    id = 0
                }, "text/plain");
            }
            var download = _downloadService.GetDownloadByGuid(id);
            var newId = download.DownloadGuid;
            _downloadService.DeleteDownload(download);
            return Json(new
            {
                success = true,
                id = newId
            }, "text/plain");
        }
    }
}
