
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class ProductAttributeExtensions
    {
        /// <summary>
        /// A value indicating whether this product variant attribute should have values
        /// </summary>
        /// <param name="productVariantAttribute">Product variant attribute</param>
        /// <returns>Result</returns>
        public static bool ShouldHaveValues(this ProductVariantAttribute productVariantAttribute)
        {
            if (productVariantAttribute == null)
                return false;

            if (productVariantAttribute.AttributeControlType == AttributeControlType.TextBox)
                return false;

            //other attribute controle types support values
            return true;
        }
    }

    public static class CategoryProductAttributeExtensions
    {
        /// <summary>
        /// A value indicating whether this product variant attribute should have values
        /// </summary>
        /// <param name="categoryProductAttribute">Product variant attribute</param>
        /// <returns>Result</returns>
        public static bool ShouldHaveValues(this CategoryProductAttribute categoryProductAttribute)
        {
            if (categoryProductAttribute == null)
                return false;

            if (categoryProductAttribute.AttributeControlType == AttributeControlType.TextBox)
                return false;

            if (categoryProductAttribute.AttributeControlType == AttributeControlType.Money)
                return false;
            if (categoryProductAttribute.AttributeControlType == AttributeControlType.MoneyRange)
                return false;
            //other attribute controle types support values
            return true;
        }
    }
}
