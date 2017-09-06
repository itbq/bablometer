using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Common
{
    public partial class UploadListModel:BaseNopModel
    {
        public IList<UploadModel> Uploads { get; set; }
        public bool IsLegal { get; set; }
    }
}