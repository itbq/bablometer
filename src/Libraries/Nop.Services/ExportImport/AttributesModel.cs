using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.ExportImport
{
    public class AttributesModel
    {
        public List<CategoryProductAttributeValue> Values { get; set; }
        public string Name { get; set; }
        public AttributeControlType ControlType { get; set; }
        public CategoryProductAttributeValue SelectedValue { get; set; }
    }
}
