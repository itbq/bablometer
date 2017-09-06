using FluentValidation;
using Nop.Admin.Models.News;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.News
{
    public class NewsItemValidator : AbstractValidator<NewsItemModel>
    {
        public NewsItemValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Title)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Fields.Title.Required"));

            RuleFor(x => x.Short)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Fields.Short.Required"));

            RuleFor(x => x.Full)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Fields.Full.Required"));


            RuleFor(x => x.StartDate)
               .Must((x, dt) =>
               {
                   if (x.EndDate.HasValue)
                   {
                       return dt < x.EndDate;
                   }
                   return true;
               })
               .WithMessage(localizationService.GetResource("admin.event.startdate.before"));
        }
    }
}