using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.UploadCatalogstructure
{
    public class ExportProductAttribute
    {
        public int RecievedId { get; set; }
        public string Name { get; set; }
        public string NameRu { get; set; }
        public string NameEs { get; set; }
        public string NameDe { get; set; }
    }
}
