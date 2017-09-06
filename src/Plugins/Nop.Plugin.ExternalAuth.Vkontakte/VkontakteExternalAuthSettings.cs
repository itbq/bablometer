using Nop.Core.Configuration;

namespace Nop.Plugin.ExternalAuth.Vkontakte
{
    public class VkontakteExternalAuthSettings : ISettings
    {
        public string ClientKeyIdentifier { get; set; }
        public string ClientSecret { get; set; }
    }
}
