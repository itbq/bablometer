using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Validators.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using FluentValidation.Attributes;
namespace Nop.Admin.Models.Catalog
{
    public partial class CategoryProductAttributeGroupModel : BaseNopEntityModel, ILocalizedModel<CategoryProductAttributeGroupLocalizedModel>
    {
        public CategoryProductAttributeGroupModel()
        {
            Locales = new List<CategoryProductAttributeGroupLocalizedModel>();
        }


        [NopResourceDisplayName("Admin.Catalog.Products.Variants.CategoryProductAttributeGroupModel.Attributes.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Variants.CategoryProductAttributeGroupModel.Attributes.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        public IList<CategoryProductAttributeGroupLocalizedModel> Locales { get; set; }

        public IList<ConversionImageModel> ConversionImages { get; set; }

    }

    [Validator(typeof(CategoryProductAttributeGroupLocalizedModelValidator))]
    public partial class CategoryProductAttributeGroupLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Variants.CategoryProductAttributeGroupLocalizedModel.Attributes.Values.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }


        [NopResourceDisplayName("Admin.Catalog.Products.Variants.CategoryProductAttributeGroupLocalizedModel.Attributes.Values.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }
    }

    //ETF Section        

    public partial class CategoryProductAttributeModel : BaseNopEntityModel
    {
        public int CategoryProductGroupId { get; set; }

        public int ProductAttributeId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Products.Variants.CategoryProductAttributeModel.Attributes.Fields.Attribute")]
        [UIHint("CategoryProductAttribute")]
        public string ProductAttribute { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Variants.CategoryProductAttributeModel.Attributes.Fields.TextPrompt")]
        [AllowHtml]
        public string TextPrompt { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Variants.CategoryProductAttributeModel.Attributes.Fields.IsRequired")]
        public bool IsRequired { get; set; }

        public int AttributeControlTypeId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Products.Variants.CategoryProductAttributeModel.Attributes.Fields.AttributeControlType")]
        [UIHint("AttributeControlType")]
        public string AttributeControlType { get; set; }

        public int SearchAttributeControlTypeId { get; set; }
        [NopResourceDisplayName("BSFA.Admin.SearchAttributeControlType")]
        [UIHint("SearchAttributeControlType")]
        public string SearchAttributeControlType { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Variants.CategoryProductAttributeModel.Attributes.Fields.DisplayOrder")]
        //we don't name it DisplayOrder because Telerik has a small bug 
        //"if we have one more editor with the same name on a page, it doesn't allow editing"
        //in our case it's category.DisplayOrder
        public int DisplayOrder1 { get; set; }

        public string ViewEditUrl { get; set; }
        public string ViewEditText { get; set; }
        [NopResourceDisplayName("ITB.Admin.CategoryAttribute.Main")]
        public bool MainAttribute { get; set; }

        [NopResourceDisplayName("ITBSFA.Admin.ProductBoxAttribute")]
        public bool ProductBoxAttribute { get; set; }

        [NopResourceDisplayName("ITBSFA.Admin.AdditionalAttribute")]
        public bool AdditionalAttribute { get; set; }
    }

    [Validator(typeof(CategoryProductAttributeValueModelValidator))]
    public partial class CategoryProductAttributeValueModel : BaseNopEntityModel, ILocalizedModel<CategoryProductAttributeValueLocalizedModel>
    {
        public CategoryProductAttributeValueModel()
        {
            Locales = new List<CategoryProductAttributeValueLocalizedModel>();
        }

        public int CategoryProductAttributeId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Variants.CategoryProductAttributeValueModel.Attributes.Values.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Variants.CategoryProductAttributeValueModel.Attributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Variants.CategoryProductAttributeValueModel.Attributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("ITB.Admin.Attribute.PopularValue")]
        public bool Popularvalue { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Products.Variants.ProductVariantAttributes.Attributes.Values.Fields.ColorSquaresRgb")]
        [AllowHtml]
        public string ColorSquaresRgb { get; set; }

        [NopResourceDisplayName("ITBSFA.Product.Attribute.RealValue")]
        public double? Realvalue { get; set; }
        public bool DisplayColorSquaresRgb { get; set; }

        public IList<CategoryProductAttributeValueLocalizedModel> Locales { get; set; }
    }
    
    public partial class CategoryProductAttributeValueListModel : BaseNopModel
    {
        public int CategoryGroupId { get; set; }

        public string CategoryGroupName { get; set; }

        public int CategoryProductAttributeId { get; set; }

        public string CategoryProductAttributeName { get; set; }
    }

    [Validator(typeof(CategoryProductAttributeValueLocalizedModelValidator))]
    public partial class CategoryProductAttributeValueLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Variants.CategoryProductVariantAttributes.Attributes.Values.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }

    public partial class CategoryToCategoryProductAttributeGroupModel : BaseNopEntityModel
    {
        public int CategoryProductAttributeGroupId { get; set; }

        public int CategoryId { get; set; }

        //public int ProductAttributeId { get; set; }
        //[NopResourceDisplayName("Admin.Catalog.Products.Variants.CategoryProductAttributeModel.Attributes.Fields.Attribute")]
        //[UIHint("CategoryProductAttribute")]
        //public string ProductAttribute { get; set; }

        //[NopResourceDisplayName("Admin.Catalog.Products.Variants.CategoryProductAttributeModel.Attributes.Fields.TextPrompt")]
        //[AllowHtml]
        //public string TextPrompt { get; set; }

        //[NopResourceDisplayName("Admin.Catalog.Products.Variants.CategoryProductAttributeModel.Attributes.Fields.IsRequired")]
        //public bool IsRequired { get; set; }

        //public int AttributeControlTypeId { get; set; }
        //[NopResourceDisplayName("Admin.Catalog.Products.Variants.CategoryProductAttributeModel.Attributes.Fields.AttributeControlType")]
        //[UIHint("AttributeControlType")]
        //public string AttributeControlType { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Variants.CategoryProductAttributeModel.Attributes.Fields.DisplayOrder")]
        //we don't name it DisplayOrder because Telerik has a small bug 
        //"if we have one more editor with the same name on a page, it doesn't allow editing"
        //in our case it's category.DisplayOrder
        public int DisplayOrder1 { get; set; }

        public string ViewEditUrl { get; set; }
        public string ViewEditText { get; set; }
    }

}