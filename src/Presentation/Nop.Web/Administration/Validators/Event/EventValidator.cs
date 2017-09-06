using FluentValidation;
using Nop.Admin.Models.Event;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Validators.Event
{
    public class EventValidator: AbstractValidator<EventModel>
    {
        public EventValidator(ILocalizationService localizationService)
        {
            //RuleFor(x => x.Title)
            //    .NotEmpty()
            //    .WithMessage(localizationService.GetResource("admin.event.title.required"));
            //RuleFor(x => x.ShortDescription)
            //    .NotEmpty()
            //    .WithMessage(localizationService.GetResource("admin.event.shortdescription.required"));
            //RuleFor(x => x.FullDescription)
            //    .NotEmpty()
            //    .WithMessage(localizationService.GetResource("admin.event.fulldescription.required"));
            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("admin.event.startdate.required"));
            RuleFor(x => x.StartDate)
                .Must((x, dt) => 
                {
                    if (x.EndDateEnabled)
                    {
                        return dt < x.EndDate;
                    }
                    return true;
                })
                .WithMessage(localizationService.GetResource("admin.event.startdate.before"));
        }
    }


    public class EventLocalizedValidator : AbstractValidator<EventLocalizedModel>
    {
        public EventLocalizedValidator(ILocalizationService localizationService)
        {
            //RuleFor(x => x.Title)
            //    .NotNull()
            //    .WithMessage(localizationService.GetResource("admin.event.title.required"));
            //RuleFor(x => x.ShortDescription)
            //    .NotNull()
            //    .WithMessage(localizationService.GetResource("admin.event.title.required"));
            //RuleFor(x => x.FullDescription)
            //    .NotNull()
            //    .WithMessage(localizationService.GetResource("admin.event.title.required"));
        }
    }
}