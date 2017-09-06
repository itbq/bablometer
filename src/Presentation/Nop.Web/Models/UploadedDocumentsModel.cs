using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models
{
    public partial class UploadedDocumentsModel : BaseNopModel
    {
        public int DownloadId { get; set; }
        public IList<int> Downloads { get; set; }
    }
}