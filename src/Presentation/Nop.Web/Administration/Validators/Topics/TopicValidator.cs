using FluentValidation;
using Nop.Admin.Models.Topics;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Topics
{
    public class TopicValidator : AbstractValidator<TopicModel>
    {
        public TopicValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SystemName).NotNull().WithMessage(localizationService.GetResource("Admin.ContentManagement.Topics.Fields.SystemName.Required"));
            RuleFor(x => x.Title).NotEmpty().WithMessage(localizationService.GetResource("ITB.Admin.Topics.Title.Required"));
            RuleFor(x => x.Body).NotEmpty().WithMessage(localizationService.GetResource("ITB.Admin.Topics.Body.Required"));
            RuleFor(x => x.MenuTitle).NotEmpty().WithMessage(localizationService.GetResource("ITB.Admin.Topics.MenuTitle.Required"));
        }
    }
}