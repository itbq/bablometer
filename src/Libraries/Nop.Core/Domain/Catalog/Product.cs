using System;
using System.Collections.Generic;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.BrandDomain;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Favorit;
using Nop.Core.Domain.Regions;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product
    /// </summary>
    public partial class Product : BaseEntity, ILocalizedEntity, ISlugSupported, IAclSupported
    {
        private ICollection<ProductVariant> _productVariants;
        private ICollection<ProductCategory> _productCategories;
        private ICollection<ProductManufacturer> _productManufacturers;
        private ICollection<ProductPicture> _productPictures;
        private ICollection<ProductReview> _productReviews;
        private ICollection<ProductSpecificationAttribute> _productSpecificationAttributes;
        private ICollection<ProductTag> _productTags;
        private ICollection<Request> _requests;
        private ICollection<CategoryProductAttributeValue> _productAttributes;
        private ICollection<FavoritItem> _favorits;
        private ICollection<ProductView> _productViews;
        private ICollection<CustomerInformationProductAttributeValue> _customerInformationAttributes;
        private ICollection<Region> _regions;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the short description
        /// </summary>
        public virtual string ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the full description
        /// </summary>
        public virtual string FullDescription { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public virtual string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a value of used product template identifier
        /// </summary>
        public virtual int ProductTemplateId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the product on home page
        /// </summary>
        public virtual bool ShowOnHomePage { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public virtual string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public virtual string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public virtual string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product allows customer reviews
        /// </summary>
        public virtual bool AllowCustomerReviews { get; set; }

        /// <summary>
        /// Gets or sets the rating sum (approved reviews)
        /// </summary>
        public virtual int ApprovedRatingSum { get; set; }

        /// <summary>
        /// Gets or sets the rating sum (not approved reviews)
        /// </summary>
        public virtual int NotApprovedRatingSum { get; set; }

        /// <summary>
        /// Gets or sets the total rating votes (approved reviews)
        /// </summary>
        public virtual int ApprovedTotalReviews { get; set; }

        /// <summary>
        /// Gets or sets the total rating votes (not approved reviews)
        /// </summary>
        public virtual int NotApprovedTotalReviews { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is subject to ACL
        /// </summary>
        public virtual bool SubjectToAcl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public virtual bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public virtual bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time of product creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of product update
        /// </summary>
        public virtual DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// Customer, who owns this product
        /// </summary>
        public virtual int CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        //Link to order the product
        public virtual string OrderLink { get; set; }

        public virtual bool FeaturedProduct { get; set; }

        /// <summary>
        /// Product rating
        /// </summary>
        public virtual double? Rating { get; set; }
        /// <summary>
        /// Minimum order quantity
        /// </summary>
        public virtual int? MinimumOrderQuantity { get; set; }
        /// <summary>
        /// Gets or sets the product variants
        /// </summary>
        public virtual ICollection<ProductVariant> ProductVariants
        {
            get { return _productVariants ?? (_productVariants = new List<ProductVariant>()); }
            protected set { _productVariants = value; }
        }
        /// <summary>
        /// Gets or sets the collection of ProductCategory
        /// </summary>
        public virtual ICollection<ProductCategory> ProductCategories
        {
            get { return _productCategories ?? (_productCategories = new List<ProductCategory>()); }
            protected set { _productCategories = value; }
        }

        /// <summary>
        /// Gets or sets the collection of ProductManufacturer
        /// </summary>
        public virtual ICollection<ProductManufacturer> ProductManufacturers
        {
            get { return _productManufacturers ?? (_productManufacturers = new List<ProductManufacturer>()); }
            protected set { _productManufacturers = value; }
        }

        /// <summary>
        /// Gets or sets the collection of ProductPicture
        /// </summary>
        public virtual ICollection<ProductPicture> ProductPictures
        {
            get { return _productPictures ?? (_productPictures = new List<ProductPicture>()); }
            protected set { _productPictures = value; }
        }

        /// <summary>
        /// Gets or sets the collection of product reviews
        /// </summary>
        public virtual ICollection<ProductReview> ProductReviews
        {
            get { return _productReviews ?? (_productReviews = new List<ProductReview>()); }
            protected set { _productReviews = value; }
        }

        /// <summary>
        /// Gets or sets the product specification attribute
        /// </summary>
        public virtual ICollection<ProductSpecificationAttribute> ProductSpecificationAttributes
        {
            get { return _productSpecificationAttributes ?? (_productSpecificationAttributes = new List<ProductSpecificationAttribute>()); }
            protected set { _productSpecificationAttributes = value; }
        }

        /// <summary>
        /// Gets or sets the product specification attribute
        /// </summary>
        public virtual ICollection<ProductTag> ProductTags
        {
            get { return _productTags ?? (_productTags = new List<ProductTag>()); }
            protected set { _productTags = value; }
        }

        /// <summary>
        /// Gets or sets requests related to this product
        /// </summary>
        public virtual ICollection<Request> Requests
        {
            get { return _requests ?? (_requests = new List<Request>()); }
            protected set { _requests = value; }
        }

        /// <summary>
        /// Gets or sets product attributes collection
        /// </summary>
        public virtual ICollection<CategoryProductAttributeValue> ProductAttributes
        {
            get { return _productAttributes ?? (_productAttributes = new List<CategoryProductAttributeValue>()); }
            protected set { _productAttributes = value; }
        }

        /// <summary>
        /// Favorits of this product
        /// </summary>
        public virtual ICollection<FavoritItem> Favorits
        {
            get { return _favorits ?? (_favorits = new List<FavoritItem>()); }
            protected set { _favorits = value; }
        }


        /// <summary>
        /// Gets or sets information about this product viewers
        /// </summary>
        public virtual ICollection<ProductView> ProductViews
        {
            get { return _productViews ?? (_productViews = new List<ProductView>()); }
            protected set { _productViews = value; }
        }

        public virtual ICollection<CustomerInformationProductAttributeValue> CustomerInformationAttributes
        {
            get { return _customerInformationAttributes ?? (_customerInformationAttributes = new List<CustomerInformationProductAttributeValue>()); }
            set { this._customerInformationAttributes = value; }
        }

        public virtual ICollection<Region> Regions
        {
            get { return _regions ?? (_regions = new List<Region>()); }
            set { this._regions = value; }
        }
    }
}