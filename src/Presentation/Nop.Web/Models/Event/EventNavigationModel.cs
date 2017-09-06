using Nop.Core;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.UI.Paging;
using Nop.Web.Models.Banner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Event
{
    public partial class EventNavigationModel : BaseNopModel
    {
        public IPagedList<EventInfoModel> EventList { get; set; }
        public EventPagingModel PagingContext { get; set; }
        public IList<BannerModel> Banners { get; set; }
    }
}