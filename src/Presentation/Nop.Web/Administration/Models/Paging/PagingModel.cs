using FluentValidation.Attributes;
using Nop.Admin.Validators.Paging;
using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.Paging
{
    [Validator(typeof (PagingValidator))]
    public class PagingModel
    {
        /// <summary>
        /// Number of news per page on news list page
        /// </summary>
        [NopResourceDisplayName("Paging.NewsList")]
        public int NewsListPageSize { get; set; }

        /// <summary>
        /// Number of news in Rss feed
        /// </summary>
        [NopResourceDisplayName("Paging.NewsRssPageSize")]
        public int NewsRssPageSize { get; set; }

        /// <summary>
        /// Number of recnt company news to display
        /// </summary>
        [NopResourceDisplayName("Paging.RecentCompanyNewsPageSize")]
        public int RecentCompanyNewsPageSize { get; set; }

        /// <summary>
        /// Number of events per page on events list page
        /// </summary>
        [NopResourceDisplayName("Paging.EventsPageSize")]
        public int EventsPageSize { get; set; }

        /// <summary>
        /// Number of items(products/services/buying requests) to display on catalog page
        /// </summary>
        [NopResourceDisplayName("Paging.ItemCatalogPageSize")]
        public int ItemCatalogPageSize { get; set; }

        /// <summary>
        /// Number of items to display on Seller/service provider catalog page
        /// </summary>
        [NopResourceDisplayName("Paging.SellerCatalogPageSize")]
        public int SellerCatalogPageSize { get; set; }

        /// <summary>
        /// Number of active requests to display
        /// </summary>
        [NopResourceDisplayName("Paging.ActiveRequestsPageSize")]
        public int ActiveRequestsPageSize { get; set; }

        /// <summary>
        /// Number of requests on history page
        /// </summary>
        [NopResourceDisplayName("Paging.RequestsHistoryPageSize")]
        public int RequestsHistoryPageSize { get; set; }

        /// <summary>
        /// Number of items to display on Manage items page
        /// </summary>
        [NopResourceDisplayName("Paging.ManageItemsPageSize")]
        public int ManageItemsPageSize { get; set; }

        /// <summary>
        /// Number of displayed recently viewed selllers
        /// </summary>
        [NopResourceDisplayName("Paging.RecentlyViewedSellersNumber")]
        public int RecentlyViewedSellersNumber { get; set; }

        /// <summary>
        /// Number of displayed recently viewed products
        /// </summary>
        [NopResourceDisplayName("Paging.RecentlyViewedProductsNumber")]
        public int RecentlyViewedProductsNumber { get; set; }

        /// <summary>
        /// Number of items ir catalog rss
        /// </summary>
        [NopResourceDisplayName("Paging.ItemsRssCount")]
        public int ItemsRssCount { get; set; }

        /// <summary>
        /// Number of sellers in seller RssFeed
        /// </summary>
        [NopResourceDisplayName("Paging.SellerRssCount")]
        public int SellerRssCount { get; set; }
    }
}