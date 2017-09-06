using Nop.Core.Domain.Messages;
using Nop.Services.Customers;
using Nop.Services.MiniSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Localization;
using System.Net.Mail;

namespace Nop.Services.Messages
{
    public class MiniSiteEmailSender: IMiniSiteEmailSender
    {
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly ITokenizer _tokenizer;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ICustomerService _customerService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailSender _emailSender;
        private readonly IMiniSiteService _miniSiteService;

        public MiniSiteEmailSender(IMessageTemplateService messageTemplateService,
            IMessageTokenProvider messageTokenProvider,
            ITokenizer tokenizer,
            IQueuedEmailService queuedEmailService,
            ICustomerService customerService,
            IEmailAccountService emailAccountService,
            EmailAccountSettings emailAccountSettings,
            IEmailSender emailSender,
            IMiniSiteService miniSiteService)
        {
            this._messageTemplateService = messageTemplateService;
            this._messageTokenProvider = messageTokenProvider;
            this._tokenizer = tokenizer;
            this._queuedEmailService = queuedEmailService;
            this._customerService = customerService;
            this._emailAccountService = emailAccountService;
            this._emailAccountSettings = emailAccountSettings;
            this._emailSender = emailSender;
            this._miniSiteService = miniSiteService;
        }

        public void SendMiniSiteActivationEmail(int miniSiteId, int languageId)
        {
            var miniSite = _miniSiteService.GetMiniSiteById(miniSiteId);
            //MiniSiteNew

            var template = _messageTemplateService.GetMessageTemplateByName("MiniSiteNew");
            template.Subject = template.GetLocalized(x => x.Subject, languageId, false, false);
            template.Body = template.GetLocalized(x => x.Body, languageId, false, false);
            
            //template
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddNewMiniSiteTokens(tokens, miniSite);
            string subject = _tokenizer.Replace(template.Subject, tokens, true);
            string body = _tokenizer.Replace(template.Body, tokens, true);

            var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            var email = new QueuedEmail()
            {
                Priority = 3,
                From = emailAccount.Email,
                FromName = emailAccount.DisplayName,
                To = emailAccount.Email,
                Subject = subject,
                Body = body,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id
            };

            _queuedEmailService.InsertQueuedEmail(email);
        }

        public void SendMiniSiteChangeDomainEmail(int miniSiteId, string oldDomainName, int languageId)
        {
            var miniSite = _miniSiteService.GetMiniSiteById(miniSiteId);
            //MiniSiteNew

            var template = _messageTemplateService.GetMessageTemplateByName("MiniSiteChangeDomain");
            template.Subject = template.GetLocalized(x => x.Subject, languageId, false, false);
            template.Body = template.GetLocalized(x => x.Body, languageId, false, false);

            //template
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddNewMiniSiteTokens(tokens, miniSite);
            tokens.Add(new Token("MiniSite.OldDomain",oldDomainName));
            string subject = _tokenizer.Replace(template.Subject, tokens, true);
            string body = _tokenizer.Replace(template.Body, tokens, true);

            var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            var email = new QueuedEmail()
            {
                Priority = 3,
                From = emailAccount.Email,
                FromName = emailAccount.DisplayName,
                To = emailAccount.Email,
                Subject = subject,
                Body = body,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id
            };

            _queuedEmailService.InsertQueuedEmail(email);
        }


        public void SendMiniSiteAcceptEmail(int miniSiteId, int languageId)
        {
            var miniSite = _miniSiteService.GetMiniSiteById(miniSiteId);
            //MiniSiteNew

            var template = _messageTemplateService.GetMessageTemplateByName("MiniSiteConfirmation");
            template.Subject = template.GetLocalized(x => x.Subject, languageId, false, false);
            template.Body = template.GetLocalized(x => x.Body, languageId, false, false);

            //template
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddNewMiniSiteTokens(tokens, miniSite);
            string subject = _tokenizer.Replace(template.Subject, tokens, true);
            string body = _tokenizer.Replace(template.Body, tokens, true);

            var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            var email = new QueuedEmail()
            {
                Priority = 3,
                From = emailAccount.Email,
                FromName = emailAccount.DisplayName,
                To = miniSite.Customer.Email,
                Subject = subject,
                Body = body,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id
            };

            _queuedEmailService.InsertQueuedEmail(email);
        }


        public void SendMiniSiteRejectEmail(int miniSiteId, int languageId)
        {
            var miniSite = _miniSiteService.GetMiniSiteById(miniSiteId);
            //MiniSiteNew

            var template = _messageTemplateService.GetMessageTemplateByName("MiniSiteRejection");
            template.Subject = template.GetLocalized(x => x.Subject, languageId, false, false);
            template.Body = template.GetLocalized(x => x.Body, languageId, false, false);

            //template
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddNewMiniSiteTokens(tokens, miniSite);
            string subject = _tokenizer.Replace(template.Subject, tokens, true);
            string body = _tokenizer.Replace(template.Body, tokens, true);

            var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            var email = new QueuedEmail()
            {
                Priority = 3,
                From = emailAccount.Email,
                FromName = emailAccount.DisplayName,
                To = miniSite.Customer.Email,
                Subject = subject,
                Body = body,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id
            };

            _queuedEmailService.InsertQueuedEmail(email);
        }
    }
}
