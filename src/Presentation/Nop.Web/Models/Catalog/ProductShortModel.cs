using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Catalog
{
    public partial class ProductShortModel:BaseNopModel
    {
        public string ProductName { get; set; }
        public string ProductSeName { get; set; }
        public string PictureUrl { get; set; }
        public string BrandName { get; set; }
        public int BrandId { get; set; }
    }
}