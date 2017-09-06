using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    public partial interface IProductItemTypeService
    {
        /// <summary>
        /// Delete product item type
        /// </summary>
        /// <param name="productTemplate">Product item type</param>
        void DeleteProductItemType(ProductItemType productItemType);

        /// <summary>
        /// Gets all product item types
        /// </summary>
        /// <returns>Product item types</returns>
        IList<ProductItemType> GetAllProductItemTypes();

        /// <summary>
        /// Gets a product item type
        /// </summary>
        /// <param name="productTemplateId">Product item type identifier</param>
        /// <returns>Product template</returns>
        ProductItemType GetProductItemTypeById(int productItemTypeId);

        /// <summary>
        /// Inserts product item type
        /// </summary>
        /// <param name="productTemplate">Product item type</param>
        void InsertProductItemType(ProductItemType productItemType);

        /// <summary>
        /// Updates the product item type
        /// </summary>
        /// <param name="productTemplate">Product item type</param>
        void UpdateProductItemType(ProductItemType productItemType);
    }
}
