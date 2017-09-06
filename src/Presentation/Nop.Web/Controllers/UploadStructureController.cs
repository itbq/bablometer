using Nop.Core.Domain.Media;
using Nop.Services.Media;
using Nop.Services.UploadCatalogstructure;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.Controllers
{
    public class UploadStructureController : Controller
    {

        private readonly IDownloadService _downloadService;
        private readonly IUploadCatalogStructureService _uploadCatalogService;
        //
        // GET: /UploadStructure/
        public UploadStructureController(IDownloadService downloadService,
            IUploadCatalogStructureService uploadCatalogService)
        {
            this._downloadService = downloadService;
            this._uploadCatalogService = uploadCatalogService;
        }

        public ActionResult Index()
        {
            var model = new UploadModel();
            model.Id = 0;
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(UploadModel model)
        {
            var download = _downloadService.GetDownloadById(model.Id);
            Action<Download> uploadDelegate = _uploadCatalogService.UploadStructure;
            uploadDelegate.BeginInvoke(download, null, null);
            return View(model);
        }

        public ActionResult Countries()
        {
            var model = new UploadModel();
            model.Id = 0;
            return View(model);
        }

        [HttpPost]
        public ActionResult Countries(UploadModel model)
        {
            var download = _downloadService.GetDownloadById(model.Id);
            Action<Download> uploadDelegate = _uploadCatalogService.UpdateCountriies;
            uploadDelegate.BeginInvoke(download, null, null);
            return View(model);
        }

        public string AddSeName()
        {
            _uploadCatalogService.ProcessCategoriesUrls();
            return "Process started";
        }

    }
}
