using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Catalog
{
    public class SearchProductAttributeValue
    {
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
        public double ExactValue { get; set; }
        public string Textvalue { get; set; }
        public int IdValue { get; set; }
        public int CategoryProductAttributeId { get; set; }
        public SearchAttributeControlType ControlType { get; set; }
        public CustomerInformationProductSearchControlType CustomerControlType { get; set; }
    }
}
