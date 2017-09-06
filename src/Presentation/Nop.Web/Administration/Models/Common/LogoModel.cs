using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.Common
{
    public partial class LogoModel
    {
        public HttpPostedFileBase File { get; set; }
        public string ImageUrl { get; set; }
    }
}