using FluentValidation;
using Nop.Admin.Models.Paging;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Validators.Paging
{
    public partial class PagingValidator : AbstractValidator<PagingModel>
    {
        public PagingValidator(ILocalizationService _localizationService)
        {
            RuleFor(x => x.ActiveRequestsPageSize)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("Paging.Error"));
            RuleFor(x => x.EventsPageSize)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("Paging.Error"));
            RuleFor(x => x.ItemCatalogPageSize)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("Paging.Error"));
            RuleFor(x => x.ManageItemsPageSize)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("Paging.Error"));
            RuleFor(x => x.NewsListPageSize)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("Paging.Error"));
            RuleFor(x => x.NewsRssPageSize)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("Paging.Error"));
            RuleFor(x => x.RecentCompanyNewsPageSize)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("Paging.Error"));
            RuleFor(x => x.RecentlyViewedProductsNumber)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("Paging.Error"));
            RuleFor(x => x.RecentlyViewedSellersNumber)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("Paging.Error"));
            RuleFor(x => x.RequestsHistoryPageSize)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("Paging.Error"));
            RuleFor(x => x.SellerCatalogPageSize)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("Paging.Error"));
            RuleFor(x => x.SellerRssCount)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("Paging.Error")); ;
            RuleFor(x => x.ItemsRssCount)
                .NotEmpty()
                .WithMessage(_localizationService.GetResource("Paging.Error")); ;
        }
    }
}