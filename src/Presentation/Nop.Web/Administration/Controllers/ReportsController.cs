using Nop.Admin.Models.Reports;
using Nop.Services.Localization;
using Nop.Services.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Admin.Controllers
{
    public class ReportsController : BaseNopController
    {
        private readonly IProductReportsService _productReportsService;
        private readonly ILocalizationService _localizationService;

        public ReportsController(IProductReportsService productReportsService,
            ILocalizationService localizationService)
        {
            this._productReportsService = productReportsService;
            this._localizationService = localizationService;
        }

        public ActionResult ReportsPopUp(int reportType, int referenceId, string reportName)
        {
            var model = new ReportModel()
            {
                ReportTypeId = reportType,
                ReferenceId = referenceId,
                ReportName = reportName
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ReportsPopUp(ReportModel model)
        {
            if(model.StartDate.HasValue && model.EndDate.HasValue)
            {
                if(model.StartDate >= model.EndDate)
                {
                    ModelState.AddModelError("StartDate",_localizationService.GetResource("ITBSFA.Reports.StartDateEndDate"));
                    ModelState.AddModelError("EndDate",_localizationService.GetResource("ITBSFA.Reports.StartDateEndDate"));
                    return View(model);
                }
            }
            MemoryStream stream = new MemoryStream();
            _productReportsService.GenerateReport((ReportTypesEnum)model.ReportTypeId, model.ReferenceId, stream, model.StartDate, model.EndDate);
            stream.Position = 0;
            //Response.Clear();
            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

    }
}
