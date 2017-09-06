using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.ExternalAuth.Vkontakte.Models;
using Nop.Plugin.ExternalAuth.Vkontakte.Core;


namespace Nop.Plugin.ExternalAuth.Vkontakte.Controllers
{
    public class ExternalAuthVkontakteController : Controller
    {
        private readonly ISettingService _settingService;
        private readonly VkontakteExternalAuthSettings _vkontakteExternalAuthSettings;
        private readonly IOAuthProviderVkontakteAuthorizer _oAuthProviderVkontakteAuthorizer;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly IPermissionService _permissionService;

        public ExternalAuthVkontakteController(ISettingService settingService,
            VkontakteExternalAuthSettings vkontakteExternalAuthSettings,
            IOAuthProviderVkontakteAuthorizer oAuthProviderVkontakteAuthorizer,
            IOpenAuthenticationService openAuthenticationService,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            IPermissionService permissionService)
        {
            this._settingService = settingService;
            this._vkontakteExternalAuthSettings = vkontakteExternalAuthSettings;
            this._oAuthProviderVkontakteAuthorizer = oAuthProviderVkontakteAuthorizer;
            this._openAuthenticationService = openAuthenticationService;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._permissionService = permissionService;
        }
        
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return Content("Access denied");

            var model = new ConfigurationModel();
            model.ClientKeyIdentifier = _vkontakteExternalAuthSettings.ClientKeyIdentifier;
            model.ClientSecret = _vkontakteExternalAuthSettings.ClientSecret;
            
            return View("Nop.Plugin.ExternalAuth.Vkontakte.Views.ExternalAuthFacebook.Configure", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return Content("Access denied");

            if (!ModelState.IsValid)
                return Configure();
            
            //save settings
            _vkontakteExternalAuthSettings.ClientKeyIdentifier = model.ClientKeyIdentifier;
            _vkontakteExternalAuthSettings.ClientSecret = model.ClientSecret;
            _settingService.SaveSetting(_vkontakteExternalAuthSettings);

            return View("Nop.Plugin.ExternalAuth.Vkontakte.Views.ExternalAuthFacebook.Configure", model);
        }

        [ChildActionOnly]
        public ActionResult PublicInfo()
        {
            return View("Nop.Plugin.ExternalAuth.Vkontakte.Views.ExternalAuthFacebook.PublicInfo");
        }

        [NonAction]
        private ActionResult LoginInternal(string returnUrl, bool verifyResponse)
        {
            var processor = _openAuthenticationService.LoadExternalAuthenticationMethodBySystemName("ExternalAuth.Vkontakte");
            if (processor == null ||
                !processor.IsMethodActive(_externalAuthenticationSettings) || !processor.PluginDescriptor.Installed)
                throw new NopException("Vkontakte module cannot be loaded");

            var viewModel = new LoginModel();
            TryUpdateModel(viewModel);
            var result = _oAuthProviderVkontakteAuthorizer.Authorize(returnUrl, verifyResponse);
            
            switch (result.AuthenticationStatus)
            {
                case OpenAuthenticationStatus.Error:
                    {
                        if (!result.Success)
                            foreach (var error in result.Errors)
                                ExternalAuthorizerHelper.AddErrorsToDisplay(error);

                        return new RedirectResult(Url.LogOn(returnUrl));
                    }
                case OpenAuthenticationStatus.AssociateOnLogon:
                    {
                        return new RedirectResult(Url.LogOn(returnUrl));
                    }
                case OpenAuthenticationStatus.AutoRegisteredEmailValidation:
                    {
                        //result
                        return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation });
                    }
                case OpenAuthenticationStatus.AutoRegisteredAdminApproval:
                    {
                        return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval });
                    }
                case OpenAuthenticationStatus.AutoRegisteredStandard:
                    {
                        return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
                    }
                default:
                    break;
            }

            if (result.Result != null) return result.Result;
            return HttpContext.Request.IsAuthenticated ? new RedirectResult(!string.IsNullOrEmpty(returnUrl) ? returnUrl : "~/") : new RedirectResult(Url.LogOn(returnUrl));
        }
        
        public ActionResult Login(string returnUrl)
        {
            return LoginInternal(returnUrl, false);
        }

        public ActionResult LoginCallback(string returnUrl)
        {
            return LoginInternal(returnUrl, true);
        }
    }
}