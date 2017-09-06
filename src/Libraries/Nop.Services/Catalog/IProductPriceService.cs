using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    public partial interface IProductPriceService
    {
        #region Methods

        void DeleteProductPrice(ProductPrice productPrice);
        void DeleteProductPriceById(int productPriceId);
        IList<ProductPrice> GetAllProductPrices(int ProductId);
        ProductPrice GetProductPriceById(int productPriceId);
        ProductPrice GetProductPriceByIdAndCurrencyId(int productId, int currencyId);
        void InsertProductPrice(ProductPrice productPrice);
        void UpdateProductPrice(ProductPrice productPrice);

        #endregion
    }
}
