using Nop.Core.Domain.Messages;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Localization;
using Nop.Services.Customers;

namespace Nop.Services.Messages
{
    public class UploadCatalogEmailService : IUploadCatalogEmailService
    {
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly ITokenizer _tokenizer;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly ICategoryService _categoryService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly ICustomerService _customerService;

        public UploadCatalogEmailService(IMessageTemplateService messageTemplateService,
            ITokenizer tokenizer,
            IQueuedEmailService queuedEmailService,
            IEmailAccountService emailAccountService,
            EmailAccountSettings emailAccountSettings,
            ICategoryService categoryService,
            IMessageTokenProvider messageTokenProvider,
            ICustomerService customerService)
        {
            this._messageTemplateService = messageTemplateService;
            this._tokenizer = tokenizer;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountService = emailAccountService;
            this._emailAccountSettings = emailAccountSettings;
            this._categoryService = categoryService;
            this._messageTokenProvider = messageTokenProvider;
            this._customerService = customerService;
        }

        public void SendUploadCatalogEmail(int items, int categoryId, int languageId, int customerId, IDictionary<int, string> errorProducts)
        {
            var account = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            var template = _messageTemplateService.GetMessageTemplateByName("Customer.UploadCatalog");
            template.Subject = template.GetLocalized(x => x.Subject, languageId);
            template.Body = template.GetLocalized(x => x.Body, languageId);
            var tokens = new List<Token>();
            tokens.Add(new Token("Catalog.Upload.Items",items.ToString()));
            var category = _categoryService.GetCategoryById(categoryId);
            string categoryName = category.GetLocalized(x=>x.Name,languageId);
            tokens.Add(new Token("Catalog.Upload.Category", categoryName));
            var errorString = new StringBuilder("<br />");
            if (errorProducts.Count > 0)
            {
                foreach(var error in errorProducts)
                {
                    errorString.AppendLine(error.Key.ToString() + ":" + error.Value + "<br />");
                }
            }
            tokens.Add(new Token("Catalog.Upload.Error", errorString.ToString()));
            _messageTokenProvider.AddStoreTokens(tokens);
            template.Subject = _tokenizer.Replace(template.Subject, tokens, false);
            template.Body = _tokenizer.Replace(template.Body, tokens, false);
            var customer = _customerService.GetCustomerById(customerId);
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
