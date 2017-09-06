using Nop.Core.Domain.Media;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Common
{
    public partial class UploadModel:BaseNopEntityModel
    {
        [NopResourceDisplayName("Document")]
        [UIHint("Download")]
        public override int Id { get; set; }
        public Guid DownloadId { get; set; }
        public int CompanyId { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public long FileSize { get; set; }
        public bool IsLegal { get; set; }
    }
}