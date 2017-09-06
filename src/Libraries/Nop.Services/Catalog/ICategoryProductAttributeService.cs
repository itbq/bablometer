using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    public partial interface ICategoryProductAttributeService
    {
        #region Category product group mappings (CategoryProductAttributeGroup)

        void DeleteCategoryProductAttributeGroup(CategoryProductAttributeGroup categoryProductAttributeGroup);     

        CategoryProductAttributeGroup GetCategoryProductAttributeGroupById(int categoryProductAttributeGroupId);

        void InsertCategoryProductAttributeGroup(CategoryProductAttributeGroup categoryProductAttributeGroup);

        void UpdateCategoryProductAttributeGroup(CategoryProductAttributeGroup categoryProductAttributeGroup);

        IList<CategoryProductAttributeGroup> GetAllCategoryProductAttributeGroups();

        #endregion

        #region Category product attributes mappings (CategoryProductAttribute)

        /// <summary>
        /// Deletes a product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttribute">Product variant attribute mapping</param>
        void DeleteCategoryProductAttribute(CategoryProductAttribute categoryProductAttribute);

        /// <summary>
        /// Gets product variant attribute mappings by product identifier
        /// </summary>
        /// <param name="productVariantId">The product variant identifier</param>
        /// <returns>Product variant attribute mapping collection</returns>
        IList<CategoryProductAttribute> GetCategoryProductAttributesByCategoryProductGroupId(int categoryProductGroupId);

        /// <summary>
        /// Gets a product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttributeId">Product variant attribute mapping identifier</param>
        /// <returns>Product variant attribute mapping</returns>
        CategoryProductAttribute GetCategoryProductAttributeById(int categoryProductAttributeId);

        /// <summary>
        /// Inserts a product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttribute">The product variant attribute mapping</param>
        void InsertCategoryProductAttribute(CategoryProductAttribute categoryProductAttribute);

        /// <summary>
        /// Updates the product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttribute">The product variant attribute mapping</param>
        void UpdateCategoryProductAttribute(CategoryProductAttribute categoryProductAttribute);

        #endregion

        #region Category product attribute values (CategoryProductAttributeValue)

        /// <summary>
        /// Deletes a product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValue">Product variant attribute value</param>
        void DeleteCategoryProductAttributeValue(CategoryProductAttributeValue categoryProductAttributeValue);

        /// <summary>
        /// Gets product variant attribute values by product identifier
        /// </summary>
        /// <param name="productVariantAttributeId">The product variant attribute mapping identifier</param>
        /// <returns>Product variant attribute mapping collection</returns>
        IList<CategoryProductAttributeValue> GetCategoryProductAttributeValues(int categoryProductAttributeId);

        /// <summary>
        /// Gets a product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValueId">Product variant attribute value identifier</param>
        /// <returns>Product variant attribute value</returns>
        CategoryProductAttributeValue GetCategoryProductAttributeValueById(int categoryProductAttributeValueId);

        /// <summary>
        /// Inserts a product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValue">The product variant attribute value</param>
        void InsertCategoryProductAttributeValue(CategoryProductAttributeValue categoryProductAttributeValue);

        /// <summary>
        /// Updates the product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValue">The product variant attribute value</param>
        void UpdateCategoryProductAttributeValue(CategoryProductAttributeValue categoryProductAttributeValue);

        #endregion

        #region Category to  category product attribute group (CategoryToCategoryProductAttributeGroup)

        void DeleteCategoryToCategoryProductAttributeGroup(CategoryToCategoryProductAttributeGroup item);

        CategoryToCategoryProductAttributeGroup GetCategoryToCategoryProductAttributeGroupById(int id);

        void InsertCategoryToCategoryProductAttributeGroup(CategoryToCategoryProductAttributeGroup item);

        void UpdateCategoryToCategoryProductAttributeGroup(CategoryToCategoryProductAttributeGroup item);

        CategoryToCategoryProductAttributeGroup GetCategoryToCategoryProductAttributeGroupByCategoryIdAndGroupId(int categoryId, int groupId);

        IList<CategoryToCategoryProductAttributeGroup> GetCategoryToCategoryProductAttributeGroupByCategoryId(int categoryId);
        #endregion
    }
}
