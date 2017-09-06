using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Web.Mvc;

namespace Nop.Admin.Models.Event
{
    public partial class EventListModel : BaseNopModel
    {
        public GridModel<EventModel> Events { get; set; }
    }
}