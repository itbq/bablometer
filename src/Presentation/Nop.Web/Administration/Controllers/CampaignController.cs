using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Admin.Models.Messages;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;
using Nop.Web.Framework.UI;
using Telerik.Web.Mvc.UI;
using Nop.Services.Catalog;
using Nop.Services.Configuration;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
	public partial class CampaignController : BaseNopController
	{
        private readonly ICampaignService _campaignService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEmailAccountService _emailAccountService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IPermissionService _permissionService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizationEntityService;
        private readonly IWorkContext _workContext;
        private readonly ICategoryService _categoryService;
        private readonly CampaignSettings _campaignSettings;
        private readonly ISettingService _settingService;
        private readonly INewsletterDatesService _newsletterDatesService;

        public CampaignController(ICampaignService campaignService,
            IDateTimeHelper dateTimeHelper, IEmailAccountService emailAccountService,
            EmailAccountSettings emailAccountSettings,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            ILocalizationService localizationService, IMessageTokenProvider messageTokenProvider,
            IPermissionService permissionService,
            ILanguageService languageService,
            ILocalizedEntityService localizationEntityService,
            IWorkContext workContext,
            ICategoryService categoryService,
            CampaignSettings campaignSettings,
            ISettingService settingService,
            INewsletterDatesService newsletterDatesService)
		{
            this._campaignService = campaignService;
            this._dateTimeHelper = dateTimeHelper;
            this._emailAccountService = emailAccountService;
            this._emailAccountSettings = emailAccountSettings;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._localizationService = localizationService;
            this._messageTokenProvider = messageTokenProvider;
            this._permissionService = permissionService;
            this._languageService = languageService;
            this._localizationEntityService = localizationEntityService;
            this._workContext = workContext;
            this._categoryService = categoryService;
            this._campaignSettings = campaignSettings;
            this._settingService = settingService;
            this._newsletterDatesService = newsletterDatesService;
		}
        private string FormatTokens(string[] tokens)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];
                sb.Append(token);
                if (i != tokens.Length - 1)
                    sb.Append(", ");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Update campaign locales
        /// </summary>
        /// <param name="mt">campaign to update</param>
        /// <param name="model">model with locales</param>
        protected void UpdateLocales(Campaign mt, CampaignModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizationEntityService.SaveLocalizedValue(mt,
                                                           x => x.Subject,
                                                           localized.Subject,
                                                           localized.LanguageId);

                _localizationEntityService.SaveLocalizedValue(mt,
                                                           x => x.Body,
                                                           localized.Body,
                                                           localized.LanguageId);
            }
        }

        /// <summary>
        /// Check model locales
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected bool CheckLocales(CampaignModel model)
        {
            foreach (var lang in model.CompaignLanguages.Where(x => x.Selected))
            {
                var locale = model.Locales.Where(x => x.LanguageId == lang.LanguageId).FirstOrDefault();
                if (locale == null)
                {
                    lang.LanguageName = "locale";
                    return false;
                }
                if (locale.Subject == null)
                {
                    lang.LanguageName = _localizationService.GetResource("Admin.Promotions.Campaigns.Fields.Subject",_workContext.WorkingLanguage.Id);
                    return false;
                }

                if (locale.Body == null)
                {
                    lang.LanguageName = _localizationService.GetResource("Admin.Promotions.Campaigns.Fields.Body", _workContext.WorkingLanguage.Id);
                    return false;
                }
            }

            return true;
        }

        protected List<TreeViewItemModel> PrepareTreeView(List<TreeViewItemModel> tree, int rootCategory)
        {
            var newTree = _categoryService.GetAllCategoriesByParentCategoryId(rootCategory, true).Select(x =>
                {
                    return new TreeViewItemModel()
                    {
                        Text = x.GetLocalized(c=>c.Name,_workContext.WorkingLanguage.Id,true),
                        Value = x.Id.ToString(),
                        Enabled = true,
                        ImageUrl = Url.Content("~/Administration/Content/images/ico-content.png"),
                        Checked = true,
                    };
                }).ToList();
            foreach (var catId in newTree)
            {
                catId.Items = PrepareTreeView(catId.Items,int.Parse(catId.Value));
                tree.Add(catId);
            }
            return tree;
        }
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

		public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var campaigns = _campaignService.GetAllCampaigns();
            var gridModel = new GridModel<CampaignModel>
            {
                Data = campaigns.Select(x =>
                {
                    var model = x.ToModel();
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    return model;
                }),
                Total = campaigns.Count
            };
            return View(gridModel);
		}

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var campaigns = _campaignService.GetAllCampaigns();
            var gridModel = new GridModel<CampaignModel>
            {
                Data = campaigns.Select(x =>
                {
                    var model = x.ToModel();
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    return model;
                }),
                Total = campaigns.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var model = new CampaignModel();
            var compaign = new Campaign();
            model.Locales = new List<CompaignLocalizedModel>();
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Subject = compaign.GetLocalized(x => x.Subject, languageId, false, false);
                locale.Body = compaign.GetLocalized(x => x.Body, languageId, false, false);
            });
            model.CompaignLanguages = _languageService.GetAllLanguages()
                .Select(x => new CompaignLanguage()
                {
                    LanguageId = x.Id,
                    LanguageName = x.Name,
                    Selected = false
                }).ToList();
            model.CategoriesTree = new List<TreeViewItemModel>();
            model.CategoriesTree = PrepareTreeView(model.CategoriesTree, 0);
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfRecentProductsTokens());
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(CampaignModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var campaign = model.ToEntity();
                campaign.CreatedOnUtc = DateTime.UtcNow;
                _campaignService.InsertCampaign(campaign);

                UpdateLocales(campaign, model);
                SuccessNotification(_localizationService.GetResource("Admin.Promotions.Campaigns.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = campaign.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfCampaignAllowedTokens());

            model.CategoriesTree = new List<TreeViewItemModel>();
            model.CategoriesTree = PrepareTreeView(model.CategoriesTree, 0);
            return View(model);
        }

		public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var campaign = _campaignService.GetCampaignById(id);
            if (campaign == null)
                //No campaign found with the specified id
                return RedirectToAction("List");
            var model = campaign.ToModel();
            model.NumberOfItems = _campaignSettings.DefaultItemCount;
            model.Locales = new List<CompaignLocalizedModel>();
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Subject = campaign.GetLocalized(x => x.Subject, languageId, false, false);
                locale.Body = campaign.GetLocalized(x => x.Body, languageId, false, false);
            });

            model.CompaignLanguages = _languageService.GetAllLanguages()
                .Select(x=>new CompaignLanguage()
                {
                    LanguageId = x.Id,
                    LanguageName = x.Name,
                    Selected = false
                }).ToList();
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfCampaignAllowedTokens());
            model.CategoriesTree = new List<TreeViewItemModel>();
            model.CategoriesTree = PrepareTreeView(model.CategoriesTree,0);
            model.EndDate = DateTime.UtcNow;
            model.StartDate = model.EndDate.Value - TimeSpan.FromDays(7);
            return View(model);
		}

        [HttpPost]
        [ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(CampaignModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var campaign = _campaignService.GetCampaignById(model.Id);
            if (campaign == null)
                //No campaign found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                campaign = model.ToEntity(campaign);
                _campaignService.UpdateCampaign(campaign);

                UpdateLocales(campaign, model);
                SuccessNotification(_localizationService.GetResource("Admin.Promotions.Campaigns.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = campaign.Id }) : RedirectToAction("List");
            }

            
            //If we got this far, something failed, redisplay form
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfCampaignAllowedTokens());

            model.CategoriesTree = new List<TreeViewItemModel>();
            model.CategoriesTree = PrepareTreeView(model.CategoriesTree, 0);
            return View(model);
		}

        [HttpPost,ActionName("Edit")]
        [FormValueRequired("send-test-email")]
        public ActionResult SendTestEmail(CampaignModel model, List<TreeViewItem> category_treeview_checkedNodes)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();
            List<int> categoryList;
            if (model.EnableTimePeriod)
            {
                categoryList = category_treeview_checkedNodes.Where(x => x.Checked).Select(x => int.Parse(x.Value)).ToList();
            }
            else
            {
                categoryList = _categoryService.GetAllCategories(showHidden: true).Select(x=>x.Id).ToList();
            }
            if (!CheckLocales(model) || categoryList.Count == 0 || (ModelState["StartDate"] != null && ModelState["StartDate"].Errors.Count > 0) || model.CompaignLanguages.Where(x=>x.Selected).Count() == 0 || (model.EnableTimePeriod && !model.EndDate.HasValue))
            {
                if (model.CompaignLanguages.Where(x => x.Selected).Count() == 0)
                {
                    AddNotification(NotifyType.Error, _localizationService.GetResource("Admin.ETF.Languages.One"), false);
                }
                if (model.EnableTimePeriod && !model.EndDate.HasValue)
                {
                    AddNotification(NotifyType.Error, _localizationService.GetResource("Admin.ETF.Campaign.EndDate"), false);
                }
                var lang = model.CompaignLanguages.Where(x => x.LanguageName != null).FirstOrDefault();
                if (lang != null)
                {
                    string Error = String.Format(_localizationService.GetResource("Admin.Campaign.LocaleError", _workContext.WorkingLanguage.Id), lang.LanguageName, _languageService.GetLanguageById(lang.LanguageId).Name);
                    AddNotification(NotifyType.Error, Error, false);
                }
                model.CompaignLanguages = _languageService.GetAllLanguages()
               .Select(x => new CompaignLanguage()
               {
                   LanguageId = x.Id,
                   LanguageName = x.Name,
                   Selected = false
               }).ToList();
                model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfCampaignAllowedTokens());
                model.CategoriesTree = new List<TreeViewItemModel>();
                model.CategoriesTree = PrepareTreeView(model.CategoriesTree, 0);
                return View(model);
            }
            var campaign = _campaignService.GetCampaignById(model.Id);
            if (campaign == null)
                //No campaign found with the specified id
                return RedirectToAction("List");

            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfCampaignAllowedTokens());
            if (!model.EnableTimePeriod)
            {
                model.EndDate = null;
            }
            foreach (var lang in model.CompaignLanguages.Where(x => x.Selected))
            {
                campaign.Body = campaign.GetLocalized(x => x.Body, lang.LanguageId);
                campaign.Subject = campaign.GetLocalized(x => x.Subject, lang.LanguageId);
                try
                {
                    var emailAccount = _emailAccountService.GetAllEmailAccounts().Where(x => x.DisplayName == "TradeBel newsletter").FirstOrDefault();
                    if (emailAccount == null)
                        throw new NopException("Email account could not be loaded");
                    if (!model.EnableTimePeriod)
                    {
                        model.StartDate = _newsletterDatesService.GetAllNewsletterDates().Where(x=>x.LanguageId == lang.LanguageId).FirstOrDefault().LastSubmit;
                        model.EndDate = null;
                    }

                    //no subscription found
                    _campaignService.SendCampaign(campaign, emailAccount, model.TestEmail, lang.LanguageId, categoryList,model.StartDate, model.EndDate,model.NumberOfItems);

                    model.CompaignLanguages = _languageService.GetAllLanguages()
                            .Select(x => new CompaignLanguage()
                            {
                                LanguageId = x.Id,
                                LanguageName = x.Name,
                                Selected = false
                            }).ToList();
                    SuccessNotification(_localizationService.GetResource("Admin.Promotions.Campaigns.TestEmailSentToCustomers"), false);
                }
                catch (Exception exc)
                {
                    ErrorNotification(exc, false);
                }
            }

            model.CompaignLanguages = _languageService.GetAllLanguages()
               .Select(x => new CompaignLanguage()
               {
                   LanguageId = x.Id,
                   LanguageName = x.Name,
                   Selected = false
               }).ToList();
            model.CategoriesTree = new List<TreeViewItemModel>();
            model.CategoriesTree = PrepareTreeView(model.CategoriesTree, 0);
            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("send-mass-email")]
        public ActionResult SendMassEmail(CampaignModel model, List<TreeViewItem> category_treeview_checkedNodes)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            List<int> categoryList;
            if (model.EnableTimePeriod)
            {
                categoryList = category_treeview_checkedNodes.Where(x => x.Checked).Select(x => int.Parse(x.Value)).ToList();
            }
            else
            {
                categoryList = _categoryService.GetAllCategories(showHidden: true).Select(x => x.Id).ToList();
            }
            if (!CheckLocales(model) || categoryList.Count == 0 || (ModelState["StartDate"] != null && ModelState["StartDate"].Errors.Count > 0) || model.CompaignLanguages.Where(x => x.Selected).Count() == 0 || (model.EnableTimePeriod && !model.EndDate.HasValue))
            {
                if (model.CompaignLanguages.Where(x => x.Selected).Count() == 0)
                {
                    AddNotification(NotifyType.Error, _localizationService.GetResource("Admin.ETF.Languages.One"), false);
                }
                if (model.EnableTimePeriod && !model.EndDate.HasValue)
                {
                    AddNotification(NotifyType.Error, _localizationService.GetResource("Admin.ETF.Campaign.EndDate"), false);
                }
                var lang = model.CompaignLanguages.Where(x => x.LanguageName != null).FirstOrDefault();
                if (lang != null)
                {
                    string Error = String.Format(_localizationService.GetResource("Admin.Campaign.LocaleError", _workContext.WorkingLanguage.Id), lang.LanguageName, _languageService.GetLanguageById(lang.LanguageId).Name);
                    AddNotification(NotifyType.Error, Error, false);
                }
                model.CompaignLanguages = _languageService.GetAllLanguages()
               .Select(x => new CompaignLanguage()
               {
                   LanguageId = x.Id,
                   LanguageName = x.Name,
                   Selected = false
               }).ToList();
                model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfCampaignAllowedTokens());
                model.CategoriesTree = new List<TreeViewItemModel>();
                model.CategoriesTree = PrepareTreeView(model.CategoriesTree, 0);
                return View(model);
            }
            var campaign = _campaignService.GetCampaignById(model.Id);
            if (campaign == null)
                //No campaign found with the specified id
                return RedirectToAction("List");

            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfCampaignAllowedTokens());
            if (!model.EnableTimePeriod)
            {
                model.EndDate = null;
            }
            foreach (var lang in model.CompaignLanguages.Where(x => x.Selected))
            {
                campaign.Body = campaign.GetLocalized(x => x.Body, lang.LanguageId);
                campaign.Subject = campaign.GetLocalized(x => x.Subject, lang.LanguageId);
                try
                {
                    var emailAccount = _emailAccountService.GetAllEmailAccounts().Where(x => x.DisplayName == "TradeBel newsletter").FirstOrDefault();
                    if (emailAccount == null)
                        throw new NopException("Email account could not be loaded");
                    IList<NewsLetterSubscription> subscriptions;
                    if (campaign.Body.IndexOf("%Store.RecentProducts%") > 0)
                    {
                        subscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(null, 0, int.MaxValue, false)
                            .Where(x=>x.NewProduct).ToList();
                    }
                    else
                    {
                        subscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(null, 0, int.MaxValue, false)
                            .Where(x => x.NewBuyingRequests).ToList();
                    }

                    var totalEmailsSent = _campaignService.SendCampaign(campaign, emailAccount, subscriptions, lang.LanguageId,categoryList,model.StartDate,model.EndDate ,model.NumberOfItems);
                    SuccessNotification(string.Format(_localizationService.GetResource("Admin.Promotions.Campaigns.MassEmailSentToCustomers"), totalEmailsSent), false);
                    
                    model.CompaignLanguages = _languageService.GetAllLanguages()
                    .Select(x => new CompaignLanguage()
                    {
                        LanguageId = x.Id,
                        LanguageName = x.Name,
                        Selected = false
                    }).ToList();
                }
                catch (Exception exc)
                {
                    ErrorNotification(exc, false);
                }
            }

            model.CompaignLanguages = _languageService.GetAllLanguages()
               .Select(x => new CompaignLanguage()
               {
                   LanguageId = x.Id,
                   LanguageName = x.Name,
                   Selected = false
               }).ToList();
            //If we got this far, something failed, redisplay form
            model.CategoriesTree = new List<TreeViewItemModel>();
            model.CategoriesTree = PrepareTreeView(model.CategoriesTree, 0);
            return View(model);
        }

		[HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            var campaign = _campaignService.GetCampaignById(id);
            if (campaign == null)
                //No campaign found with the specified id
                return RedirectToAction("List");

            _campaignService.DeleteCampaign(campaign);

            SuccessNotification(_localizationService.GetResource("Admin.Promotions.Campaigns.Deleted"));
			return RedirectToAction("List");
		}
	}
}
