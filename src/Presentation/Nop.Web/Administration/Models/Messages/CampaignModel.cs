using System;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Localization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Telerik.Web.Mvc.UI;

namespace Nop.Admin.Models.Messages
{
    [Validator(typeof(CampaignValidator))]
    public partial class CampaignModel : BaseNopEntityModel,ILocalizedModel<CompaignLocalizedModel>
    {
        [NopResourceDisplayName("Admin.Promotions.Campaigns.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Campaigns.Fields.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Campaigns.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }
        
        [NopResourceDisplayName("Admin.Promotions.Campaigns.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Campaigns.Fields.AllowedTokens")]
        public string AllowedTokens { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Campaigns.Fields.TestEmail")]
        [AllowHtml]
        public string TestEmail { get; set; }

        public bool IsBuyingRequest { get; set; }

        public IList<CompaignLocalizedModel> Locales { get; set; }

        [NopResourceDisplayName("Admin.Campaign.Lannguage")]
        public IList<CompaignLanguage> CompaignLanguages { get; set; }

        [NopResourceDisplayName("Admin.Campaign.ItemNumber")]
        public int NumberOfItems { get; set; }

        [NopResourceDisplayName("Admin.Campaign.StartDate")]
        [UIHint("Date")]
        public DateTime StartDate { get; set; }

        [NopResourceDisplayName("Admin.Campaign.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [NopResourceDisplayName("Admin.Campaign.EnableTime")]
        public bool EnableTimePeriod { get; set; }

        [NopResourceDisplayName("Admin.Campaign.Categories")]
        public List<TreeViewItemModel> CategoriesTree { get; set; }
    }

    public class CompaignLanguage
    {
        public int LanguageId { get; set; }
        public string LanguageName { get; set; }
        public bool Selected { get; set; }
    }
    public partial class CompaignLocalizedModel : ILocalizedModelLocal
    {
        [NopResourceDisplayName("Admin.Promotions.Campaigns.Fields.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Campaigns.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        public int LanguageId { get; set; }
    }
}