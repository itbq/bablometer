using FluentValidation.Attributes;
using Nop.Admin.Validators.TextResource;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Admin.Models.TextResource
{
    public class TextResourceModel:BaseNopModel,ILocalizedModel<TextResourceLocalizedModel>
    {
        /// <summary>
        /// Join us block text
        /// </summary>
        [NopResourceDisplayName("BSFA.JoinUs.Title")]
        [UIHint("RichEditor")]
        [AllowHtml]
        public string JoinUsText { get; set; }

        ///// <summary>
        ///// Add contact success notification
        ///// </summary>
        //[NopResourceDisplayName("TextResource.ContactAdd.Title")]
        //public string ContactAddText { get; set; }

        ///// <summary>
        ///// Text telling about language specific information on item add page
        ///// </summary>
        //[NopResourceDisplayName("TextResource.ItemAdd.Title")]
        //public string ItemAddText { get; set; }

        ///// <summary>
        ///// Text prompt in accept request window
        ///// </summary>
        //[NopResourceDisplayName("TextResource.AcceptRequestPrompt")]
        //public string RequestAccept { get; set; }

        ///// <summary>
        ///// Text prompt in reject request window
        ///// </summary>
        //[NopResourceDisplayName("TextResource.RejectRequestPrompt")]
        //public string RequestReject { get; set; }

        //[NopResourceDisplayName("TextResource.UploadCatalog.Step1.Title")]
        //public string UploadCatalogStep1Title { get; set; }

        //[NopResourceDisplayName("TextResource.UploadCatalog.Step1.Prompt")]
        //public string UploadCatalogStep1Prompt { get; set; }

        //[NopResourceDisplayName("TextResource.UploadCatalog.Step2.Title")]
        //public string UploadCatalogStep2Title { get; set; }

        //[NopResourceDisplayName("TextResource.UploadCatalog.Step2.Prompt")]
        //public string UploadCatalogStep2Prompt { get; set; }

        //[NopResourceDisplayName("TextResource.UploadCatalog.Step3.Title")]
        //public string UploadCatalogStep3Title { get; set; }

        //[NopResourceDisplayName("TextResource.UploadCatalog.Step3.Prompt")]
        //public string UploadCatalogStep3Prompt { get; set; }

        //[NopResourceDisplayName("TextResource.UploadCatalog.Step4.Title")]
        //public string UploadCatalogStep4Title { get; set; }

        //[NopResourceDisplayName("TextResource.UploadCatalog.Step4.Prompt")]
        //public string UploadCatalogStep4Prompt { get; set; }

        //[NopResourceDisplayName("TextResource.UploadCatalog.Step5.Title")]
        //public string UploadCatalogStep5Title { get; set; }

        //[NopResourceDisplayName("TextResource.UploadCatalog.Step5.Prompt")]
        //public string UploadCatalogStep5Prompt { get; set; }


        public IList<TextResourceLocalizedModel> Locales { get; set; }
    }

    [Validator(typeof(TextResourceLocalizedValidator))]
    public partial class TextResourceLocalizedModel : ILocalizedModelLocal
    {
        /// <summary>
        /// Join us block text
        /// </summary>
        [NopResourceDisplayName("BSFA.JoinUs.Title")]
        [UIHint("RichEditor")]
        public string JoinUsText { get; set; }

        ///// <summary>
        ///// Add contact success notification
        ///// </summary>
        //[NopResourceDisplayName("TextResource.ContactAdd.Title")]
        //public string ContactAddText { get; set; }

        ///// <summary>
        ///// Text telling about language specific information on item add page
        ///// </summary>
        //[NopResourceDisplayName("TextResource.ItemAdd.Title")]
        //public string ItemAddText { get; set; }

        ///// <summary>
        ///// Text prompt in accept request window
        ///// </summary>
        //[NopResourceDisplayName("TextResource.AcceptRequestPrompt")]
        //public string RequestAccept { get; set; }

        ///// <summary>
        ///// Text prompt in reject request window
        ///// </summary>
        //[NopResourceDisplayName("TextResource.RejectRequestPrompt")]
        //public string RequestReject { get; set; }


        //[NopResourceDisplayName("TextResource.UploadCatalog.Step1.Title")]
        //public string UploadCatalogStep1Title { get; set; }

        //[NopResourceDisplayName("TextResource.UploadCatalog.Step1.Prompt")]
        //public string UploadCatalogStep1Prompt { get; set; }

        //[NopResourceDisplayName("TextResource.UploadCatalog.Step2.Title")]
        //public string UploadCatalogStep2Title { get; set; }

        //[NopResourceDisplayName("TextResource.UploadCatalog.Step2.Prompt")]
        //public string UploadCatalogStep2Prompt { get; set; }

        //[NopResourceDisplayName("TextResource.UploadCatalog.Step3.Title")]
        //public string UploadCatalogStep3Title { get; set; }

        //[NopResourceDisplayName("TextResource.UploadCatalog.Step3.Prompt")]
        //public string UploadCatalogStep3Prompt { get; set; }

        //[NopResourceDisplayName("TextResource.UploadCatalog.Step4.Title")]
        //public string UploadCatalogStep4Title { get; set; }

        //[NopResourceDisplayName("TextResource.UploadCatalog.Step4.Prompt")]
        //public string UploadCatalogStep4Prompt { get; set; }

        //[NopResourceDisplayName("TextResource.UploadCatalog.Step5.Title")]
        //public string UploadCatalogStep5Title { get; set; }

        //[NopResourceDisplayName("TextResource.UploadCatalog.Step5.Prompt")]
        //public string UploadCatalogStep5Prompt { get; set; }


        public int LanguageId { get; set; }
    }

}