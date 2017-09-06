using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Services.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Localization;

namespace Nop.Services.Messages
{
    public class NewsPublicationEmailSender : INewPublicationEmailSender
    {
        private readonly INewsService _newsService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IMessageTokenProvider _mesageTokenProvider;
        private readonly ITokenizer _tokenizer;
        private readonly IQueuedEmailService _queuedEmailService;

        public NewsPublicationEmailSender(INewsService newsService,
            IEmailAccountService emailAccountService,
            EmailAccountSettings emailAccountSettings,
            IMessageTemplateService messageTemplateService,
            IMessageTokenProvider mesageTokenProvider,
            ITokenizer tokenizer,
            IQueuedEmailService queuedEmailService)
        {
            this._newsService = newsService;
            this._emailAccountService = emailAccountService;
            this._emailAccountSettings = emailAccountSettings;
            this._messageTemplateService = messageTemplateService;
            this._mesageTokenProvider = mesageTokenProvider;
            this._tokenizer = tokenizer;
            this._queuedEmailService = queuedEmailService;
        }

        public void SendNewsPublicationNotification(NewsItem newItem)
        {
            if (newItem == null)
                throw new ArgumentNullException("new item");
            var account = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            var customer = newItem.Customer;
            var template = _messageTemplateService.GetMessageTemplateByName("NewPublishing");
            template.Subject = template.GetLocalized(x => x.Subject, newItem.LanguageId);
            template.Body = template.GetLocalized(x => x.Body, newItem.LanguageId);
            var tokens = new List<Token>();
            _mesageTokenProvider.AddNewPublishingToken(tokens, newItem);
            _mesageTokenProvider.AddStoreTokens(tokens);
            template.Subject = _tokenizer.Replace(template.Subject, tokens, false);
            template.Body = _tokenizer.Replace(template.Body, tokens, false);
            var email = new QueuedEmail()
            {
                Priority = 3,
                From = account.Email,
                FromName = account.DisplayName,
                To = customer.Email,
                Subject = template.Subject,
                Body = template.Body,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = account.Id
            };
            _queuedEmailService.InsertQueuedEmail(email);
        }
    }
}
