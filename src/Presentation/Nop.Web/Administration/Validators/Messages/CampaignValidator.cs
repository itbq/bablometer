using FluentValidation;
using Nop.Admin.Models.Messages;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Messages
{
    public class CampaignValidator : AbstractValidator<CampaignModel>
    {
        public CampaignValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Promotions.Campaigns.Fields.Name.Required"));
            RuleFor(x => x.StartDate)
                .Must((x, dt) => 
                {
                    if (x.EndDate.HasValue)
                    {
                        if (x.EnableTimePeriod)
                        {
                            return dt < x.EndDate.Value;
                        }
                    }
                    return true;
                })
                .WithMessage(localizationService.GetResource("admin.event.startdate.before"));
            
        }
    }
}