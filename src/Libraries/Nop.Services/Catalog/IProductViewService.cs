using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Catalog
{
    public interface IProductViewService
    {
        IList<ProductView> GetCustomerProductViews(int customerId,int languageId);
        void AddProductToProductViews(int productId, int customerId);
        IList<int> GetPopularCustomerProducts(int customerId, int languageId);
    }
}
