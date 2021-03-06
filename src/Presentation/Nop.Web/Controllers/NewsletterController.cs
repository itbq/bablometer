﻿using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Models.Newsletter;

namespace Nop.Web.Controllers
{
    public partial class NewsletterController : BaseNopController
    {
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IWorkflowMessageService _workflowMessageService;

        private readonly CustomerSettings _customerSettings;

        public NewsletterController(ILocalizationService localizationService,
            IWorkContext workContext, INewsLetterSubscriptionService newsLetterSubscriptionService,
            IWorkflowMessageService workflowMessageService, CustomerSettings customerSettings)
        {
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._workflowMessageService = workflowMessageService;

            this._customerSettings = customerSettings;
        }

        [ChildActionOnly]
        public ActionResult NewsletterBox()
        {
            if (_customerSettings.HideNewsletterBlock)
                return Content("");

            return PartialView(new NewsletterBoxModel()
                {
                    LanguageId = _workContext.WorkingLanguage.Id
                });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SubscribeNewsletter(string email, int languageid, bool newProduct, bool newBuyingRequest)
        {
            string result;
            bool success = false;

            if (!CommonHelper.IsValidEmail(email))
            {
                result = _localizationService.GetResource("Newsletter.Email.Wrong");
                if (!(newProduct || newBuyingRequest))
                {
                    result += "<br />" + _localizationService.GetResource("Newsletter.Email.Select.Type");
                }
            }
            else
            {
                if (!(newProduct || newBuyingRequest))
                {
                    result =_localizationService.GetResource("Newsletter.Email.Select.Type");
                    return Json(new
                    {
                        Success = false,
                        Result = result,
                    });
                }
                //subscribe/unsubscribe
                email = email.Trim();

                var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmail(email, languageid);
                if (subscription != null)
                {
                    if (!subscription.Active)
                    {
                        _workflowMessageService.SendNewsLetterSubscriptionActivationMessage(subscription, _workContext.WorkingLanguage.Id);
                    }
                    result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                }
                else
                {
                    subscription = new NewsLetterSubscription()
                    {
                        NewsLetterSubscriptionGuid = Guid.NewGuid(),
                        Email = email,
                        Active = false,
                        CreatedOnUtc = DateTime.UtcNow,
                        LanguageId = languageid,
                        NewProduct = newProduct,
                        NewBuyingRequests = newBuyingRequest
                    };
                    _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);
                    _workflowMessageService.SendNewsLetterSubscriptionActivationMessage(subscription, _workContext.WorkingLanguage.Id);

                    result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                }
                success = true;
            }

            return Json(new
            {
                Success = success,
                Result = result,
            });
        }

        public ActionResult SubscriptionActivation(Guid token, bool active)
        {
            var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByGuid(token);
            if (subscription == null)
                return RedirectToRoute("HomePage");

            var model = new SubscriptionActivationModel();

            if (active)
            {
                subscription.Active = active;
                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscription);
            }
            else
                _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);

            if (active)
                model.Result = _localizationService.GetResource("Newsletter.ResultActivated");
            else
                model.Result = _localizationService.GetResource("Newsletter.ResultDeactivated");

            return View(model);
        }
    }
}
