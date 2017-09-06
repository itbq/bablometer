using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Customer
{
    public partial class CustomerMenuModel:BaseNopModel
    {
        public bool IsSeller { get; set; }
        public MenuTab ActiveTab { get; set; }
    }

    public enum MenuTab
    {
        Profile,
        Requests,
        BuyingRequests,
        Catalogue,
        News,
        Favourits,
        MiniSite
    }
}