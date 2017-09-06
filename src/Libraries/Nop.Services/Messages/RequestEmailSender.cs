using Nop.Services.Catalog;
using Nop.Services.RequestServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Localization;
using Nop.Core.Domain.Messages;
using System.Net.Mail;

namespace Nop.Services.Messages
{
    public class RequestEmailSender : IRequestEmailSender
    {
        private readonly IRequestService _requestService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly ITokenizer _tokenizer;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IProductService _productService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailSender _emailSender;

        public RequestEmailSender(IRequestService requestService,
            IMessageTemplateService messageTemplateService,
            IMessageTokenProvider messageTokenProvider,
            ITokenizer tokenizer,
            IQueuedEmailService queuedEmailService,
            IProductService productService,
            IEmailAccountService emailAccountService,
            EmailAccountSettings emailAccountSettings,
            IEmailSender emailSender)
        {
            this._requestService = requestService;
            this._messageTemplateService = messageTemplateService;
            this._messageTokenProvider = messageTokenProvider;
            this._tokenizer = tokenizer;
            this._queuedEmailService = queuedEmailService;
            this._productService = productService;
            this._emailAccountService = emailAccountService;
            this._emailAccountSettings = emailAccountSettings;
            this._emailSender = emailSender;
        }


        public void SendNewRequestEmail(int requestId, int languageId)
        {
            var request = _requestService.GetRequestById(requestId);
            var template = _messageTemplateService.GetMessageTemplateByName("ResponceNew");
            template.Subject = template.GetLocalized(x => x.Subject, languageId, false, false);
            template.Body = template.GetLocalized(x => x.Body, languageId, false, false);
            //template
            var tokens = new List<Token>();
            _messageTokenProvider.AddProductTokens(tokens,_productService.GetProductById(request.ProductId),languageId);
            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddNewRequestTokens(tokens,request,languageId);
            string subject = _tokenizer.Replace(template.Subject, tokens, true);
            string body = _tokenizer.Replace(template.Body, tokens, true);

            string email = request.Product.Customer.Email;
            var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(email);
            _emailSender.SendEmail(emailAccount, subject, body, from, to);
        }

        public void SendRequestResponceEmail(int requestId, int languageId)
        {
            var request = _requestService.GetRequestById(requestId);
            var template = _messageTemplateService.GetMessageTemplateByName("RequestAccept");
            template.Subject = template.GetLocalized(x => x.Subject, languageId, false, false);
            template.Body = template.GetLocalized(x => x.Body, languageId, false, false);
            //template
            var tokens = new List<Token>();
            _messageTokenProvider.AddProductTokens(tokens, _productService.GetProductById(request.ProductId), languageId);
            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddRequestReqsponceTokens(tokens, request, languageId);
            string subject = _tokenizer.Replace(template.Subject, tokens, true);
            string body = _tokenizer.Replace(template.Body, tokens, true);

            string email = request.Customer.Email;
            var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(email);
            _emailSender.SendEmail(emailAccount, subject, body, from, to);
        }
    }
}
