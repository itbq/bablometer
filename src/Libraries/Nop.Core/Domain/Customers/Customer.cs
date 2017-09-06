using System;
using System.Collections.Generic;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.BrandDomain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Favorit;
using Nop.Core.Domain.MiniSite;
using Nop.Core.Domain.Regions;
namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a customer
    /// </summary>
    public partial class Customer : BaseEntity
    {
        private ICollection<ExternalAuthenticationRecord> _externalAuthenticationRecords;
        private ICollection<CustomerContent> _customerContent;
        private ICollection<CustomerRole> _customerRoles;
        private ICollection<ShoppingCartItem> _shoppingCartItems;
        private ICollection<Order> _orders;
        private ICollection<RewardPointsHistory> _rewardPointsHistory;
        private ICollection<ReturnRequest> _returnRequests;
        private ICollection<Address> _addresses;
        private ICollection<ForumTopic> _forumTopics;
        private ICollection<ForumPost> _forumPosts;
        private ICollection<NewsItem> _news;
        private ICollection<Download> _downloads;
        private ICollection<Brand> _brands;
        private ICollection<Product> _products;
        private ICollection<Request> _requests;
        private ICollection<FavoritItem> _favorits;
        private ICollection<ProductView> _productViews;
        private ICollection<CompanyInformationView> _companyInformationViews;

        public Customer()
        {
            this.CustomerGuid = Guid.NewGuid();
            this.PasswordFormat = PasswordFormat.Clear;
        }

        /// <summary>
        /// Gets or sets the customer Guid
        /// </summary>
        public virtual Guid CustomerGuid { get; set; }

        public virtual string Username { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }

        public virtual int PasswordFormatId { get; set; }
        public virtual PasswordFormat PasswordFormat
        {
            get { return (PasswordFormat)PasswordFormatId; }
            set { this.PasswordFormatId = (int)value; }
        }

        public virtual string PasswordSalt { get; set; }
        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public virtual string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public virtual int? LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the currency identifier
        /// </summary>
        public virtual int? CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the tax display type identifier
        /// </summary>
        public virtual int TaxDisplayTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is tax exempt
        /// </summary>
        public virtual bool IsTaxExempt { get; set; }

        /// <summary>
        /// Gets or sets a VAT number (including counry code)
        /// </summary>
        public virtual string VatNumber { get; set; }

        /// <summary>
        /// Gets or sets the VAT number status identifier
        /// </summary>
        public virtual int VatNumberStatusId { get; set; }

        /// <summary>
        /// Gets or sets the last payment method system name (selected one)
        /// </summary>
        public virtual string SelectedPaymentMethodSystemName { get; set; }

        /// <summary>
        /// Gets or sets the selected checkout attributes (serialized)
        /// </summary>
        public virtual string CheckoutAttributes { get; set; }

        /// <summary>
        /// Gets or sets the applied discount coupon code
        /// </summary>
        public virtual string DiscountCouponCode { get; set; }

        /// <summary>
        /// Gets or sets the applied gift card coupon codes (serialized)
        /// </summary>
        public virtual string GiftCardCouponCodes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use reward points during checkout
        /// </summary>
        public virtual bool UseRewardPointsDuringCheckout { get; set; }

        /// <summary>
        /// Gets or sets the time zone identifier
        /// </summary>
        public virtual string TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets the affiliate identifier
        /// </summary>
        public virtual int? AffiliateId { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the customer is active
        /// </summary>
        public virtual bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer has been deleted
        /// </summary>
        public virtual bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer account is system
        /// </summary>
        public virtual bool IsSystemAccount { get; set; }

        /// <summary>
        /// Gets or sets the customer system name
        /// </summary>
        public virtual string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the last IP address
        /// </summary>
        public virtual string LastIpAddress { get; set; }

        /// <summary>
        /// Provider company name
        /// </summary>
        public virtual string CompanyName { get; set; }

        /// <summary>
        /// Customer post index
        /// </summary>
        public virtual string PostIndex { get; set; }

        /// <summary>
        /// Customer Address
        /// </summary>
        public virtual string Address { get; set; }

        /// <summary>
        /// Customer region id
        /// </summary>
        public virtual int? RegionId { get; set; }

        /// <summary>
        /// Customer region
        /// </summary>
        public virtual Region Region { get; set; }

        /// <summary>
        /// Customer City
        /// </summary>
        public virtual City City { get; set; }

        /// <summary>
        /// Customer City Id
        /// </summary>
        public virtual int? CityId { get; set; } 

        /// <summary>
        /// Provider logo
        /// </summary>
        public virtual int? ProviderLogoId { get; set; }
        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Customer date of birth
        /// </summary>

        [CustomerFieldIsInProductAttribute("Возраст (лет)")]
        public virtual DateTime? BirthdayDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last login
        /// </summary>
        public virtual DateTime? LastLoginDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public virtual DateTime LastActivityDateUtc { get; set; }

        /// <summary>
        /// Gets or sets customer gender
        /// </summary>
        [CustomerFieldIsInProductAttribute("Пол")]
        public virtual int? Gender { get; set; }
        /// <summary>
        /// Get or set CompanyInnformation
        /// </summary>
        public virtual CompanyInformation CompanyInformation { get; set; }

        /// <summary>
        /// Customer Income
        /// </summary>
        [CustomerFieldIsInProductAttribute("Доход в месяц")]
        public virtual double? Income { get; set; }

        /// <summary>
        /// Customer first name
        /// </summary>
        public virtual string FirstName {get; set;}

        /// <summary>
        /// Bank rating
        /// </summary>
        public virtual double? Rating { get; set; }
        /// <summary>
        /// Customer Last name
        /// </summary>
        public virtual string LastName { get; set; }

        public virtual int? CompanyInformationId { get; set; }
        #region Custom properties

        /// <summary>
        /// Gets the tax display type
        /// </summary>
        public virtual TaxDisplayType TaxDisplayType
        {
            get
            {
                return (TaxDisplayType)this.TaxDisplayTypeId;
            }
            set
            {
                this.TaxDisplayTypeId = (int)value;
            }
        }

        /// <summary>
        /// Gets the VAT number status
        /// </summary>
        public virtual VatNumberStatus VatNumberStatus
        {
            get
            {
                return (VatNumberStatus)this.VatNumberStatusId;
            }
            set
            {
                this.VatNumberStatusId = (int)value;
            }
        }
        
        #endregion

        #region Navigation properties

        /// <summary>
        /// Gets or sets the affiliate
        /// </summary>
        public virtual Affiliate Affiliate { get; set; }

        /// <summary>
        /// Gets or sets the language
        /// </summary>
        public virtual Language Language { get; set; }

        /// <summary>
        /// Gets or sets the currency
        /// </summary>
        public virtual Currency Currency { get; set; }

        /// <summary>
        /// Gets or sets customer generated content
        /// </summary>
        public virtual ICollection<ExternalAuthenticationRecord> ExternalAuthenticationRecords
        {
            get { return _externalAuthenticationRecords ?? (_externalAuthenticationRecords = new List<ExternalAuthenticationRecord>()); }
            protected set { _externalAuthenticationRecords = value; }
        }

        /// <summary>
        /// Gets or sets customer generated content
        /// </summary>
        public virtual ICollection<CustomerContent> CustomerContent
        {
            get { return _customerContent ?? (_customerContent = new List<CustomerContent>()); }
            protected set { _customerContent = value; }
        }

        /// <summary>
        /// Gets or sets the customer roles
        /// </summary>
        public virtual ICollection<CustomerRole> CustomerRoles
        {
            get { return _customerRoles ?? (_customerRoles = new List<CustomerRole>()); }
            protected set { _customerRoles = value; }
        }

        /// <summary>
        /// Gets or sets shopping cart items
        /// </summary>
        public virtual ICollection<ShoppingCartItem> ShoppingCartItems
        {
            get { return _shoppingCartItems ?? (_shoppingCartItems = new List<ShoppingCartItem>()); }
            protected set { _shoppingCartItems = value; }            
        }

        /// <summary>
        /// Gets or sets orders
        /// </summary>
        public virtual ICollection<Order> Orders
        {
            get { return _orders ?? (_orders = new List<Order>()); }
            protected set { _orders = value; }            
        }

        /// <summary>
        /// Gets or sets reward points history
        /// </summary>
        public virtual ICollection<RewardPointsHistory> RewardPointsHistory
        {
            get { return _rewardPointsHistory ?? (_rewardPointsHistory = new List<RewardPointsHistory>()); }
            protected set { _rewardPointsHistory = value; }            
        }

        /// <summary>
        /// Gets or sets return request of this customer
        /// </summary>
        public virtual ICollection<ReturnRequest> ReturnRequests
        {
            get { return _returnRequests ?? (_returnRequests = new List<ReturnRequest>()); }
            protected set { _returnRequests = value; }            
        }
        
        /// <summary>
        /// Default billing address
        /// </summary>
        public virtual Address BillingAddress { get; set; }

        /// <summary>
        /// Default shipping address
        /// </summary>
        public virtual Address ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets customer addresses
        /// </summary>
        public virtual ICollection<Address> Addresses
        {
            get { return _addresses ?? (_addresses = new List<Address>()); }
            protected set { _addresses = value; }            
        }
        
        /// <summary>
        /// Gets or sets the created forum topics
        /// </summary>
        public virtual ICollection<ForumTopic> ForumTopics
        {
            get { return _forumTopics ?? (_forumTopics = new List<ForumTopic>()); }
            protected set { _forumTopics = value; }
        }

        /// <summary>
        /// Gets or sets the created forum posts
        /// </summary>
        public virtual ICollection<ForumPost> ForumPosts
        {
            get { return _forumPosts ?? (_forumPosts = new List<ForumPost>()); }
            protected set { _forumPosts = value; }
        }

        /// <summary>
        /// Customer news
        /// </summary>
        public virtual ICollection<NewsItem> News
        {
            get { return _news ?? (_news = new List<NewsItem>()); }
            protected set { _news = value; }
        }

        /// <summary>
        /// Customer brands
        /// </summary>
        public virtual ICollection<Brand> Brands
        {
            get { return _brands ?? (_brands = new List<Brand>()); }
            protected set { _brands = value; }
        }

        /// <summary>
        /// products added by customer
        /// </summary>
        public virtual ICollection<Product> Products
        {
            get { return _products ?? (_products = new List<Product>()); }
            protected set { _products = value; }
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
        /// Gets or sets favorits of this customer
        /// </summary>
        public virtual ICollection<FavoritItem> Favorits
        {
            get { return _favorits ?? (_favorits = new List<FavoritItem>()); }
            protected set { _favorits = value; }
        }

        /// <summary>
        /// Gets or sets product views of this customer
        /// </summary>
        public virtual ICollection<ProductView> ProductViews
        {
            get { return _productViews ?? (_productViews = new List<ProductView>()); }
            protected set { _productViews = value; }
        }

        /// <summary>
        /// Gets or sets company informations viewed by this customer
        /// </summary>
        public virtual ICollection<CompanyInformationView> CompanyInformationViews
        {
            get { return _companyInformationViews ?? (_companyInformationViews = new List<CompanyInformationView>()); }
            protected set { _companyInformationViews = value; }
        }
        #endregion

        public virtual UserMiniSite UserMiniSite { get; set; }
    }
}