using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.SearchProduct
{
    public class SearchProductAttributeValueModel
    {
        public string ValueText { get; set; }
        public string ValueDouble { get; set; }
        public int Id { get; set; }
        public string ValueMax { get; set; }
        public bool Selected { get; set; }
        public bool Popularvalue { get; set; }
    }
}