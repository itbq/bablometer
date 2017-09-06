using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.UploadCatalogstructure
{
    public class ExportCategory
    {
        public int RecievedId { get; set; }
        public string Title { get; set; }
        public string TitleRu { get; set; }
        public string TitleDe { get; set; }
        public string TitleEs { get; set; }
        public int CategoryId { get; set; }
        public int ParentCategoryId { get; set; }
        public int AttributeValueListId { get; set; }
    }
}
