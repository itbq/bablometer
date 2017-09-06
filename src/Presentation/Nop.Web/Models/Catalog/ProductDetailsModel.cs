using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Brand;
using Nop.Web.Models.BuyingRequest;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;
using Nop.Web.Models.Request;

namespace Nop.Web.Models.Catalog
{
    public partial class ProductDetailsModel : BaseNopEntityModel
    {
        public ProductDetailsModel()
        {
            DefaultPictureModel = new PictureModel();
            PictureModels = new List<PictureModel>();
            ProductVariantModels = new List<ProductVariantModel>();
            SpecificationAttributeModels = new List<ProductSpecificationModel>();
            ProductAttributes = new List<ProductAttributesModel>();
            ProductTags = new List<ProductTagModel>();
            CategoryAttributes = new List<CategoryAttributeModel>();
            ProductPrices = new List<ProductPriceModel>();
        }

        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string ProductTemplateViewPath { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public int? MinimumOrderQuantity { get; set; }
        public int BrandId { get; set; }
        public double? Rating { get; set; }
        public double? BankRating { get; set; }
        public int ProductItemType { get; set; }
        public string OrderingComments { get; set; }
        public BrandModel Brand { get; set; }
        //picture(s)
        public bool DefaultPictureZoomEnabled { get; set; }
        public PictureModel DefaultPictureModel { get; set; }
        public bool Favorit { get; set; }
        public IList<PictureModel> PictureModels { get; set; }

        public string OrderingLink { get; set; }
        public CompanyInformation.CompanyInformationModel CompanyInformationModel { get; set; }
        //product variant(s)
        public IList<ProductVariantModel> ProductVariantModels { get; set; }
        //specification attributes
        public IList<ProductSpecificationModel> SpecificationAttributeModels { get; set; }

        public IList<ProductAttributesModel> ProductAttributes { get; set; }

        public IList<ProductTagModel> ProductTags { get; set; }

        public RequestOverviewModel Request { get; set; }

        public List<CategoryAttributeModel> CategoryAttributes { get; set; }

        public List<ProductPriceModel> ProductPrices { get; set; }

        public bool HaveConversionImages { get; set; }
        public int CustomerId { get; set; }
        #region Nested Classes

        public partial class ProductBreadcrumbModel : BaseNopModel
        {
            public ProductBreadcrumbModel()
            {
                CategoryBreadcrumb = new List<CategoryModel>();
            }

            public int ProductId { get; set; }
            public string ProductItemTypeAnchor { get; set; }
            public string ProductName { get; set; }
            public string ProductSeName { get; set; }
            public IList<CategoryModel> CategoryBreadcrumb { get; set; }
        }

        public partial class ProductVariantModel : BaseNopEntityModel
        {
            public ProductVariantModel()
            {
                GiftCard = new GiftCardModel();
                ProductVariantPrice = new ProductVariantPriceModel();
                PictureModel = new PictureModel();
                AddToCart = new AddToCartModel();
                ProductVariantAttributes = new List<ProductVariantAttributeModel>();
            }

            public string Name { get; set; }

            public bool ShowSku { get; set; }
            public string Sku { get; set; }

            public string Description { get; set; }

            public bool ShowManufacturerPartNumber { get; set; }
            public string ManufacturerPartNumber { get; set; }

            public bool ShowGtin { get; set; }
            public string Gtin { get; set; }

            public bool HasSampleDownload { get; set; }

            public GiftCardModel GiftCard { get; set; }

            public string StockAvailability { get; set; }

            public bool IsCurrentCustomerRegistered { get; set; }
            public bool DisplayBackInStockSubscription { get; set; }
            public bool BackInStockAlreadySubscribed { get; set; }

            public ProductVariantPriceModel ProductVariantPrice { get; set; }

            public AddToCartModel AddToCart { get; set; }

            public PictureModel PictureModel { get; set; }

            public IList<ProductVariantAttributeModel> ProductVariantAttributes { get; set; }

            #region Nested Classes

            public partial class AddToCartModel : BaseNopModel
            {
                public AddToCartModel()
                {
                    this.AllowedQuantities = new List<SelectListItem>();
                }
                public int ProductVariantId { get; set; }

                [NopResourceDisplayName("Products.Qty")]
                public int EnteredQuantity { get; set; }

                [NopResourceDisplayName("Products.EnterProductPrice")]
                public bool CustomerEntersPrice { get; set; }
                [NopResourceDisplayName("Products.EnterProductPrice")]
                public decimal CustomerEnteredPrice { get; set; }
                public String CustomerEnteredPriceRange { get; set; }

                public bool DisableBuyButton { get; set; }
                public bool DisableWishlistButton { get; set; }
                public List<SelectListItem> AllowedQuantities { get; set; }
                public bool AvailableForPreOrder { get; set; }
            }

            public partial class ProductVariantPriceModel : BaseNopModel
            {
                public string OldPrice { get; set; }

                public string Price { get; set; }
                public string PriceWithDiscount { get; set; }

                public decimal PriceValue { get; set; }
                public decimal PriceWithDiscountValue { get; set; }

                public bool CustomerEntersPrice { get; set; }

                public bool CallForPrice { get; set; }

                public int ProductVariantId { get; set; }

                public bool HidePrices { get; set; }

                public bool DynamicPriceUpdate { get; set; }
            }

            public partial class GiftCardModel : BaseNopModel
            {
                public bool IsGiftCard { get; set; }

                [NopResourceDisplayName("Products.GiftCard.RecipientName")]
                [AllowHtml]
                public string RecipientName { get; set; }
                [NopResourceDisplayName("Products.GiftCard.RecipientEmail")]
                [AllowHtml]
                public string RecipientEmail { get; set; }
                [NopResourceDisplayName("Products.GiftCard.SenderName")]
                [AllowHtml]
                public string SenderName { get; set; }
                [NopResourceDisplayName("Products.GiftCard.SenderEmail")]
                [AllowHtml]
                public string SenderEmail { get; set; }
                [NopResourceDisplayName("Products.GiftCard.Message")]
                [AllowHtml]
                public string Message { get; set; }

                public GiftCardType GiftCardType { get; set; }
            }

            public partial class TierPriceModel : BaseNopModel
            {
                public string Price { get; set; }

                public int Quantity { get; set; }
            }

            public partial class ProductVariantAttributeModel : BaseNopEntityModel
            {
                public ProductVariantAttributeModel()
                {
                    AllowedFileExtensions = new List<string>();
                    Values = new List<ProductVariantAttributeValueModel>();
                }

                public int ProductVariantId { get; set; }

                public int ProductAttributeId { get; set; }

                public string Name { get; set; }

                public string Description { get; set; }

                public string TextPrompt { get; set; }

                public bool IsRequired { get; set; }

                /// <summary>
                /// Selected value for textboxes
                /// </summary>
                public string TextValue { get; set; }
                /// <summary>
                /// Selected day value for datepicker
                /// </summary>
                public int? SelectedDay { get; set; }
                /// <summary>
                /// Selected month value for datepicker
                /// </summary>
                public int? SelectedMonth { get; set; }
                /// <summary>
                /// Selected year value for datepicker
                /// </summary>
                public int? SelectedYear { get; set; }
                /// <summary>
                /// Allowed file extensions for customer uploaded files
                /// </summary>
                public IList<string> AllowedFileExtensions { get; set; }

                public AttributeControlType AttributeControlType { get; set; }

                public IList<ProductVariantAttributeValueModel> Values { get; set; }

            }

            public partial class ProductVariantAttributeValueModel : BaseNopEntityModel
            {
                public string Name { get; set; }

                public string ColorSquaresRgb { get; set; }

                public string PriceAdjustment { get; set; }

                public decimal PriceAdjustmentValue { get; set; }

                public bool IsPreSelected { get; set; }
            }
            #endregion
        }

        /// <summary>
        /// Selects all attribute within which was created
        /// </summary>
        public partial class ProductAttributesModel : BaseNopEntityModel
        {
            /// <summary>
            /// Combobox, checkbox & etc.
            /// </summary>
            public int AttributeControlTypeId { get; set; }

            /// <summary>
            /// Value of the concreete product attribute
            /// </summary>
            public string ProductAttributeValue { get; set; }

            /// <summary>
            /// Name of the concreete product attribute
            /// </summary>
            public string ProductAttributeName { get; set; }

            /// <summary>
            /// Color Squares Value
            /// </summary>
            public string ColorSquaresRgb { get; set; }

            /// <summary>
            /// Display order of attribute
            /// </summary>
            public int DisplayOrder { get; set; }

        }

        public partial class ProductPriceModel : BaseNopEntityModel
        {
            public string PriceValue { get; set; }
            public decimal Price { get; set; }
            public DateTime PriceUpdatedOn { get; set; }
            public int CurrencyId { get; set; }
            public Currency Currency { get; set; }
            public int ProductId { get; set; }
            public Product Product { get; set; }
        }
        #endregion
    }
}