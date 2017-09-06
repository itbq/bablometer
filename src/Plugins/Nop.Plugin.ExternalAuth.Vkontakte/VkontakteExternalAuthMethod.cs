using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.ExternalAuth.Vkontakte
{
    /// <summary>
    /// Facebook externalAuth processor
    /// </summary>
    public class VkontakteExternalAuthMethod : BasePlugin, IExternalAuthenticationMethod
    {
        #region Fields

        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public VkontakteExternalAuthMethod(ISettingService settingService)
        {
            this._settingService = settingService;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ExternalAuthVkontakte";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.ExternalAuth.Vkontakte.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for displaying plugin in public store
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPublicInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "ExternalAuthVkontakte";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.ExternalAuth.Vkontakte.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new VkontakteExternalAuthSettings()
            {
                ClientKeyIdentifier = "",
                ClientSecret = "",
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Vkontakte.Login", "Войти с помощью аккаунта ВКонтакте");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Vkontakte.ClientKeyIdentifier", "Client key identifier");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Vkontakte.ClientKeyIdentifier.Hint", "Enter your client key identifier here.");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Vkontakte.ClientSecret", "Client secret");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Vkontakte.ClientSecret.Hint", "Enter your client secret here.");

            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<VkontakteExternalAuthSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.Vkontakte.Login");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.Vkontakte.ClientKeyIdentifier");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.Vkontakte.ClientKeyIdentifier.Hint");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.Vkontakte.ClientSecret");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.Vkontakte.ClientSecret.Hint");

            base.Uninstall();
        }

        #endregion
    }
}
