using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.BuyingRequest
{
    public class CategoryAttributeValueModel
    {
        public bool Error { get; set; }
        public int CategoryAttributeId { get; set; }
        public string Value { get; set; }
        public int Id { get; set; }
    }
}