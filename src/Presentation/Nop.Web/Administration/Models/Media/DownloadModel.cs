using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.Media
{
    public partial class DownloadModel:BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Download.FileName")]
        public string FileName { get; set; }

        [NopResourceDisplayName("Admin.Download.FileExtension")]
        public string FileExtension { get; set; }

        [NopResourceDisplayName("Admin.Download.FileSize")]
        public long FileSize { get; set; }

        public Guid FileGuid { get; set; }
    }
}