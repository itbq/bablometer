using FluentValidation;
using Nop.Admin.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Validators.Settings
{
    public partial class RssSettingsValidator : AbstractValidator<RssSettingsModel>
    {
        public RssSettingsValidator()
        {
            RuleFor(x => x.EventFeedCount)
                .NotEmpty()
                .WithMessage("Event count required");
            RuleFor(x => x.NewsFeedCount)
                .NotEmpty()
                .WithMessage("News count required");
            RuleFor(x => x.ProductFeedCount)
                .NotEmpty()
                .WithMessage("Product count required");
            RuleFor(x => x.SellerFeedCount)
                .NotEmpty()
                .WithMessage("Seller count required");
        }
    }
}