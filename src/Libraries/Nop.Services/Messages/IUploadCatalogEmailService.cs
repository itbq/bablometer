using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Messages
{
    public interface IUploadCatalogEmailService
    {
        void SendUploadCatalogEmail(int items, int categoryId, int languageId, int customerId,IDictionary<int,string> errorProducts);
    }
}
