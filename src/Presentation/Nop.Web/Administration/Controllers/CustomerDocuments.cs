using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.ModelBinding;
using System.Web.Mvc;
using Nop.Admin.Models.Common;
using Nop.Admin.Models.Customers;
using Nop.Admin.Models.ShoppingCart;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Media;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public partial class CustomerController : BaseNopController
    {
        public PartialViewResult Documents(int customerId)
        {
            var customer = _customerService.GetCustomerById(customerId);
            var model = new CustomerDocumentsModel();
            if (customer.CustomerRoles.Where(x => x.SystemName == "Sellers").Select(x => x.SystemName).FirstOrDefault() != null)
            {
                model.LegalDocuments = customer.CompanyInformation.CompanyDocuments.Where(x => x.IsLegal).Select(x =>
                {
                    var docModel = new DownloadModel()
                    {
                        Id = x.Id,
                        FileName = x.Filename,
                        FileExtension = x.Extension,
                        FileSize = x.FileSize,
                        FileGuid = x.DownloadGuid
                    };
                    return docModel;
                }).ToList();
            }
            model.CompanyDocuments = customer.CompanyInformation.CompanyDocuments.Where(x => !x.IsLegal).Select(x =>
            {
                var docModel = new DownloadModel()
                {
                    Id = x.Id,
                    FileName = x.Filename,
                    FileExtension = x.Extension,
                    FileSize = x.FileSize,
                    FileGuid = x.DownloadGuid
                };
                return docModel;
            }).ToList();

            model.CompanyId = customer.CompanyInformationId.Value;
            return PartialView(model);
        }

        public ActionResult DeleteDocument(int id, int companyid)
        {
            var company = _companyInformationService.GetCompanyInformation(companyid);
            company.CompanyDocuments.Remove(_downloadService.GetDownloadById(id));
            _companyInformationService.UpdateCompany(company);

            return Json(new
            {
                success = true,
                downloadId = id
            },"text/plain");
        }
    }
}