using System.Collections.Generic;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using System;
using Nop.Core.Domain;
using Nop.Core.Domain.MiniSite;

namespace Nop.Services.Messages
{
    public partial interface IMessageTokenProvider
    {
        void AddStoreTokens(IList<Token> tokens);

        void AddOrderTokens(IList<Token> tokens, Order order, int languageId);

        void AddShipmentTokens(IList<Token> tokens, Shipment shipment, int languageId);

        void AddOrderNoteTokens(IList<Token> tokens, OrderNote orderNote);

        void AddRecurringPaymentTokens(IList<Token> tokens, RecurringPayment recurringPayment);
        
        void AddReturnRequestTokens(IList<Token> tokens, ReturnRequest returnRequest, OrderProductVariant opv);

        void AddGiftCardTokens(IList<Token> tokens, GiftCard giftCard);

        void AddCustomerTokens(IList<Token> tokens, Customer customer);

        void AddNewsLetterSubscriptionTokens(IList<Token> tokens, NewsLetterSubscription subscription);

        void AddProductReviewTokens(IList<Token> tokens, ProductReview productReview);

        void AddBlogCommentTokens(IList<Token> tokens, BlogComment blogComment);

        void AddNewsCommentTokens(IList<Token> tokens, NewsComment newsComment);

        void AddProductTokens(IList<Token> tokens, Product product, int languageId);

        void AddProductVariantTokens(IList<Token> tokens, ProductVariant productVariant);

        void AddForumTokens(IList<Token> tokens, Forum forum);

        void AddForumTopicTokens(IList<Token> tokens, ForumTopic forumTopic,
            int? friendlyForumTopicPageIndex = null, int? appendedPostIdentifierAnchor = null);

        void AddForumPostTokens(IList<Token> tokens, ForumPost forumPost);

        void AddPrivateMessageTokens(IList<Token> tokens, PrivateMessage privateMessage);

        void AddBackInStockTokens(IList<Token> tokens, BackInStockSubscription subscription);

        void AddNewRequestTokens(IList<Token> tokens, Request request, int languageId);

        void AddRequestReqsponceTokens(IList<Token> tokens, Request request, int languageId);

        void AddNewPublishingToken(IList<Token> tokens, NewsItem newItem);

        string[] GetListOfCampaignAllowedTokens();

        string[] GetListOfAllowedTokens();

        string[] GetListOfRecentProductsTokens();

        void AddRecentProductsToken(IList<Token> tokens, int languageId, List<int> Categories, DateTime startDate, DateTime? endDate, int itemCount);
        void AddRecentBuyingRequestsToken(IList<Token> tokens, int languageId, List<int> Categories, DateTime startDate, DateTime? endDate, int itemCount);
        void AddNewMiniSiteTokens(IList<Token> tokens, UserMiniSite miniSite);
    }
}
