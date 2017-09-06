using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.UploadCatalogstructure
{
    public class ExportProductAttributeValueList
    {
        public List<ExportProductAttributeValue> List { get; set; }
        public int RecievedId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameRu { get; set; }
        public string NameDe { get; set; }
        public string NameEs { get; set; }
    }
}
