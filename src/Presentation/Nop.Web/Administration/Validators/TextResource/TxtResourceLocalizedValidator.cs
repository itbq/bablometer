using FluentValidation;
using Nop.Admin.Models.TextResource;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Validators.TextResource
{
    public class TextResourceLocalizedValidator:AbstractValidator<TextResourceLocalizedModel>
    {
        public TextResourceLocalizedValidator(ILocalizationService localizationService)
        {
            //RuleFor(x=>x.ContactAddText).NotEmpty()
            //    .WithMessage(localizationService.GetResource("ETF.Excel.RequiredField.Error.Message"));
            //RuleFor(x => x.ItemAddText).NotEmpty()
            //    .WithMessage(localizationService.GetResource("ETF.Excel.RequiredField.Error.Message"));
            //RuleFor(x => x.JoinUsText).NotEmpty()
            //    .WithMessage(localizationService.GetResource("ETF.Excel.RequiredField.Error.Message"));
            //RuleFor(x => x.RequestAccept).NotEmpty()
            //    .WithMessage(localizationService.GetResource("ETF.Excel.RequiredField.Error.Message"));
            //RuleFor(x => x.RequestReject).NotEmpty()
            //    .WithMessage(localizationService.GetResource("ETF.Excel.RequiredField.Error.Message"));

            //RuleFor(x => x.UploadCatalogStep1Prompt).NotEmpty()
            //    .WithMessage(localizationService.GetResource("ETF.Excel.RequiredField.Error.Message"));
            //RuleFor(x => x.UploadCatalogStep1Title).NotEmpty()
            //    .WithMessage(localizationService.GetResource("ETF.Excel.RequiredField.Error.Message"));
            //RuleFor(x => x.UploadCatalogStep2Prompt).NotEmpty()
            //    .WithMessage(localizationService.GetResource("ETF.Excel.RequiredField.Error.Message"));
            //RuleFor(x => x.UploadCatalogStep2Title).NotEmpty()
            //    .WithMessage(localizationService.GetResource("ETF.Excel.RequiredField.Error.Message"));
            //RuleFor(x => x.UploadCatalogStep3Prompt).NotEmpty()
            //    .WithMessage(localizationService.GetResource("ETF.Excel.RequiredField.Error.Message"));
            //RuleFor(x => x.UploadCatalogStep3Title).NotEmpty()
            //    .WithMessage(localizationService.GetResource("ETF.Excel.RequiredField.Error.Message"));
            //RuleFor(x => x.UploadCatalogStep4Prompt).NotEmpty()
            //    .WithMessage(localizationService.GetResource("ETF.Excel.RequiredField.Error.Message"));
            //RuleFor(x => x.UploadCatalogStep4Title).NotEmpty()
            //    .WithMessage(localizationService.GetResource("ETF.Excel.RequiredField.Error.Message"));
            //RuleFor(x => x.UploadCatalogStep5Prompt).NotEmpty()
            //    .WithMessage(localizationService.GetResource("ETF.Excel.RequiredField.Error.Message"));
            //RuleFor(x => x.UploadCatalogStep5Title).NotEmpty()
            //    .WithMessage(localizationService.GetResource("ETF.Excel.RequiredField.Error.Message"));

        }
    }
}