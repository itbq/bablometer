using Nop.Admin.Models.Media;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.Customers
{
    public partial class CustomerDocumentsModel:BaseNopModel
    {
        public int CompanyId { get; set; }
        public IList<DownloadModel> LegalDocuments { get; set; }
        public IList<DownloadModel> CompanyDocuments { get; set; }
    }
}