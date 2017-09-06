using Nop.Core.Domain.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.UploadCatalogstructure
{
    public interface IUploadCatalogStructureService
    {
        void UploadStructure(Download download);
        void UpdateCountriies(Download download);
        void ProcessCategoriesUrls();
    }
}
