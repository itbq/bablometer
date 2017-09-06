using Nop.Web.Framework.UI.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Request
{
    public partial class RequestPagableModel: BasePageableModel, IPageableModel
    {
        public bool History { get; set; }
    }
}