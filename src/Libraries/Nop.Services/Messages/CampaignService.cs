using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Nop.Core.Data;
using Nop.Core.Domain.Messages;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Catalog;

namespace Nop.Services.Messages
{
    public partial class CampaignService : ICampaignService
    {
        private readonly IRepository<Campaign> _campaignRepository;
        private readonly IEmailSender _emailSender;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly ITokenizer _tokenizer;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ICustomerService _customerService;
        private readonly IEventPublisher _eventPublisher;
        private readonly INewsletterDatesService _newsletterDatesService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="campaignRepository">Campaign repository</param>
        /// <param name="emailSender">Email sender</param>
        /// <param name="messageTokenProvider">Message token provider</param>
        /// <param name="tokenizer">Tokenizer</param>
        /// <param name="queuedEmailService">Queued email service</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="eventPublisher">Event published</param>
        public CampaignService(IRepository<Campaign> campaignRepository,
            IEmailSender emailSender, IMessageTokenProvider messageTokenProvider,
            ITokenizer tokenizer, IQueuedEmailService queuedEmailService,
            ICustomerService customerService, IEventPublisher eventPublisher,
            INewsletterDatesService newsletterDatesService)
        {
            this._campaignRepository = campaignRepository;
            this._emailSender = emailSender;
            this._messageTokenProvider = messageTokenProvider;
            this._tokenizer = tokenizer;
            this._queuedEmailService = queuedEmailService;
            this._customerService = customerService;
            this._eventPublisher = eventPublisher;
            this._newsletterDatesService = newsletterDatesService;
        }

        /// <summary>
        /// Inserts a campaign
        /// </summary>
        /// <param name="campaign">Campaign</param>        
        public virtual void InsertCampaign(Campaign campaign)
        {
            if (campaign == null)
                throw new ArgumentNullException("campaign");

            _campaignRepository.Insert(campaign);

            //event notification
            _eventPublisher.EntityInserted(campaign);
        }

        /// <summary>
        /// Updates a campaign
        /// </summary>
        /// <param name="campaign">Campaign</param>
        public virtual void UpdateCampaign(Campaign campaign)
        {
            if (campaign == null)
                throw new ArgumentNullException("campaign");

            _campaignRepository.Update(campaign);

            //event notification
            _eventPublisher.EntityUpdated(campaign);
        }

        /// <summary>
        /// Deleted a queued email
        /// </summary>
        /// <param name="campaign">Campaign</param>
        public virtual void DeleteCampaign(Campaign campaign)
        {
            if (campaign == null)
                throw new ArgumentNullException("campaign");

            _campaignRepository.Delete(campaign);

            //event notification
            _eventPublisher.EntityDeleted(campaign);
        }

        /// <summary>
        /// Gets a campaign by identifier
        /// </summary>
        /// <param name="campaignId">Campaign identifier</param>
        /// <returns>Campaign</returns>
        public virtual Campaign GetCampaignById(int campaignId)
        {
            if (campaignId == 0)
                return null;

            var campaign = _campaignRepository.GetById(campaignId);
            return campaign;

        }

        /// <summary>
        /// Gets all campaigns
        /// </summary>
        /// <returns>Campaign collection</returns>
        public virtual IList<Campaign> GetAllCampaigns()
        {

            var query = from c in _campaignRepository.Table
                        orderby c.CreatedOnUtc
                        select c;
            var campaigns = query.ToList();

            return campaigns;
        }
        
        /// <summary>
        /// Sends a campaign to specified emails
        /// </summary>
        /// <param name="campaign">Campaign</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>Total emails sent</returns>
        public virtual int SendCampaign(Campaign campaign, EmailAccount emailAccount,
            IEnumerable<NewsLetterSubscription> subscriptions, int languageId, List<int> Categories, DateTime startDate, DateTime? endDate, int itemCount)
        {
            if (campaign == null)
                throw new ArgumentNullException("campaign");

            if (emailAccount == null)
                throw new ArgumentNullException("emailAccount");

            int totalEmailsSent = 0;
            bool hasProductsToken = campaign.Body.IndexOf("%Store.RecentProducts%") > 0;
            bool hasBuyingRequests = campaign.Body.IndexOf("%Store.RecentProductBuyingRequests%") > 0;
            var productDates = new NewsletterDates();
            var buyingRequestDates = new NewsletterDates();
            var tokens = new List<Token>();
            if (hasProductsToken)
            {
                productDates = _newsletterDatesService.GetAllNewsletterDates().Where(x => x.LanguageId == languageId && x.IsProduct).FirstOrDefault();
                if (endDate == null)
                    startDate = productDates.LastSubmit;
                _messageTokenProvider.AddRecentProductsToken(tokens, languageId, Categories, startDate, endDate, itemCount);
            }
            if (hasBuyingRequests)
            {
                buyingRequestDates = _newsletterDatesService.GetAllNewsletterDates().Where(x => x.LanguageId == languageId && !x.IsProduct).FirstOrDefault();
                if (endDate == null)
                    startDate = buyingRequestDates.LastSubmit;
                _messageTokenProvider.AddRecentBuyingRequestsToken(tokens, languageId, Categories, startDate, endDate, itemCount);
            }

            foreach (var subscription in subscriptions.Where(x=>x.LanguageId == languageId))
            {
                var tokensTemp = new List<Token>();
                tokensTemp.AddRange(tokens);
                _messageTokenProvider.AddStoreTokens(tokensTemp);
                _messageTokenProvider.AddNewsLetterSubscriptionTokens(tokensTemp, subscription);
                
                var customer = _customerService.GetCustomerByEmail(subscription.Email);
                if (customer != null)
                    _messageTokenProvider.AddCustomerTokens(tokensTemp, customer);

                string subject = _tokenizer.Replace(campaign.Subject, tokensTemp, false);
                string body = _tokenizer.Replace(campaign.Body, tokensTemp, true);

                var email = new QueuedEmail()
                {
                    Priority = 3,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = subscription.Email,
                    Subject = subject,
                    Body = body,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id
                };
                _queuedEmailService.InsertQueuedEmail(email);
                totalEmailsSent++;
            }
            if (hasProductsToken)
            {
                productDates.LastSubmit = DateTime.UtcNow;
                _newsletterDatesService.UpdateNewsletterDates(productDates);
            }
            if (hasBuyingRequests)
            {
                buyingRequestDates.LastSubmit = DateTime.UtcNow;
                _newsletterDatesService.UpdateNewsletterDates(buyingRequestDates);
            }
            
            return totalEmailsSent;
        }

        /// <summary>
        /// Sends a campaign to specified email
        /// </summary>
        /// <param name="campaign">Campaign</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="email">Email</param>
        public virtual void SendCampaign(Campaign campaign, EmailAccount emailAccount, string email, int languageId, List<int> Categories, DateTime startDate, DateTime? endDate, int itemCount)
        {
            if (campaign == null)
                throw new ArgumentNullException("campaign");

            if (emailAccount == null)
                throw new ArgumentNullException("emailAccount");

            bool hasProductsToken = campaign.Body.IndexOf("%Store.RecentProducts%") > 0;
            bool hasBuyingRequests = campaign.Body.IndexOf("%Store.RecentProductBuyingRequests%") > 0;
            var productDates = new NewsletterDates();
            var buyingRequestDates = new NewsletterDates();
            var tokens = new List<Token>();

            _messageTokenProvider.AddStoreTokens(tokens);
            if (hasProductsToken)
            {
                productDates = _newsletterDatesService.GetAllNewsletterDates().Where(x=>x.LanguageId == languageId && x.IsProduct).FirstOrDefault();
                if(endDate == null)
                    startDate = productDates.LastSubmit;
                _messageTokenProvider.AddRecentProductsToken(tokens, languageId, Categories, startDate, endDate, itemCount);
            }
            if(hasBuyingRequests)
            {
                buyingRequestDates = _newsletterDatesService.GetAllNewsletterDates().Where(x=>x.LanguageId == languageId && !x.IsProduct).FirstOrDefault();
                if(endDate == null)
                    startDate = buyingRequestDates.LastSubmit;
                _messageTokenProvider.AddRecentBuyingRequestsToken(tokens, languageId, Categories, startDate, endDate, itemCount);
            }

            var customer = _customerService.GetCustomerByEmail(email);
            if (customer != null)
                _messageTokenProvider.AddCustomerTokens(tokens, customer);
            
            string subject = _tokenizer.Replace(campaign.Subject, tokens, false);
            string body = _tokenizer.Replace(campaign.Body, tokens, true);

            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(email);
            _emailSender.SendEmail(emailAccount, subject, body, from, to);
        }
    }
}
