using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.ExportImport
{
    public class ExportCategoryAttributeModel
    {
        public List<CategoryProductAttributeValue> Values { get; set; }
        public string Name { get; set; }
        public AttributeControlType ControlType { get; set; }
        public int Id { get; set; }
        public bool IsRequired { get; set; }
        public CategoryProductAttributeValue SelectedValue { get; set; }
    }
}
