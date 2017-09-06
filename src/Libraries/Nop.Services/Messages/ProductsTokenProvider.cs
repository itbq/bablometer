using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Html;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;

namespace Nop.Services.Messages
{
    public partial class MessageTokenProvider : IMessageTokenProvider
    {
        /// <summary>
        /// Process new Recently Added products token
        /// </summary>
        /// <param name="tokens">List of tokens, where to add Recently.Added.Prooducts token</param>
        /// <param name="languageId">id of product language</param>
        /// <param name="itemCount">number of products to add to newsletter</param>
        public virtual void AddRecentProductsToken(IList<Token> tokens,int languageId, List<int> Categories, DateTime startDate, DateTime? endDate, int itemCount)
        {
            var tempTokens = new List<Token>();
            var template = _messageTemplateService.GetMessageTemplateByName("ProductTemplate");
            template.Body = template.GetLocalized(x => x.Body, languageId);
            List<Product> products = new List<Product>();
            var id = _productItemTypeService.GetAllProductItemTypes().Where(y => y.Name == "Product").FirstOrDefault().Id;
            foreach (var category in Categories)
            {
                var categoryProducts = _productService.Table.Where(x => x.ProductCategories.Where(y => y.CategoryId == category).FirstOrDefault() != null && x.Published && !x.Deleted).OrderByDescending(x=>x.CreatedOnUtc).ToList();
                var languageFilteredProducts = categoryProducts.ToList()
                    .Where(x => x.GetLocalized(p => p.Name, languageId, false) != null && x.GetLocalized(p => p.ShortDescription, languageId, false) != null && x.GetLocalized(p => p.FullDescription, languageId, false) != null)
                    .OrderByDescending(x => x.CreatedOnUtc);
                if (endDate != null)
                {
                    //endDate.Value.la
                    products.AddRange(languageFilteredProducts.Where(x => x.CreatedOnUtc.Date > startDate.Date && x.CreatedOnUtc.Date <= endDate.Value.Date).Take(itemCount));
                }
                else
                {
                    products.AddRange(languageFilteredProducts.Where(x => x.CreatedOnUtc.Date > startDate.Date).Take(itemCount));
                }
            }

            products = products.OrderByDescending(x => x.CreatedOnUtc).ToList();
            string tokenString   = "";
            foreach (var product in products)
            {
                AddProductTokens(tempTokens, product,languageId);
                AddCustomProductTokens(tempTokens, product,languageId);
                tokenString += _tokenizer.Replace(template.Body, tempTokens, false);
                tempTokens.Clear();
            }
            tokens.Add(new Token("Store.RecentProducts",tokenString,true));
        }

        /// <summary>
        /// Process new Recently Added buying request token
        /// </summary>
        /// <param name="tokens">List of tokens, where to add Recently.Added.Buying request token</param>
        /// <param name="languageId">id of buying request language</param>
        /// <param name="itemCount">number of buuying requests to add to newsletter</param>
        public virtual void AddRecentBuyingRequestsToken(IList<Token> tokens, int languageId, List<int> Categories, DateTime startDate, DateTime? endDate, int itemCount)
        {
            var tempTokens = new List<Token>();
            var template = _messageTemplateService.GetMessageTemplateByName("BuyingRequestTemplate");
            template.Body = template.GetLocalized(x => x.Body, languageId);
            List<Product> products = new List<Product>();

            var id = _productItemTypeService.GetAllProductItemTypes().Where(y => y.Name == "Product Buying Request").FirstOrDefault().Id;
            foreach (var category in Categories)
            {
                var categoryProducts = _productService.Table.Where(x => x.ProductCategories.Where(y => y.CategoryId == category).FirstOrDefault() != null).OrderByDescending(x=>x.CreatedOnUtc);

                var languageFilteredProducts = categoryProducts.ToList()
                    .Where(x => x.GetLocalized(p => p.Name, languageId, false) != null && x.GetLocalized(p => p.ShortDescription, languageId, false) != null && x.GetLocalized(p => p.FullDescription, languageId, false) != null)
                    .OrderByDescending(x => x.CreatedOnUtc);

                if (endDate != null)
                {
                    products.AddRange(categoryProducts.Where(x => x.CreatedOnUtc > startDate && x.CreatedOnUtc <= endDate).Take(itemCount));
                }
                else
                {
                    products.AddRange(categoryProducts.Where(x => x.CreatedOnUtc > startDate).Take(itemCount));
                }
            }

            products = products.OrderByDescending(x => x.CreatedOnUtc).ToList();
            string tokenString = "";
            foreach (var product in products)
            {
                AddProductTokens(tempTokens, product, languageId);
                AddCustomProductTokens(tempTokens, product, languageId);
                tokenString += _tokenizer.Replace(template.Body, tempTokens, false);
                tempTokens.Clear();
            }
            tokens.Add(new Token("Store.RecentProductBuyingRequests", tokenString, true));
        }
    }
}
