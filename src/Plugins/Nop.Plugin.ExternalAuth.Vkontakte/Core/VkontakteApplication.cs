//Contributor:  Nicholas Mayne

//using Facebook;
using Nop.Plugin.ExternalAuth.Vkontakte.Core;

namespace Nop.Plugin.ExternalAuth.Vkontakte.Core
{
    public class VkontakteApplication : IVkontakteApplication
    {
        public VkontakteApplication(string clientKeyIdentifier, string clientSecret)
        {
            AppId = clientKeyIdentifier;
            AppSecret = clientSecret;
        }

        public string AppId { get; private set; }
        public string AppSecret { get; private set; }
        public string SiteUrl { get { return null; } }
        public string CanvasPage { get { return null; } }
        public string CanvasUrl { get { return null; } }
        public string SecureCanvasUrl { get { return null; } }
        public string CancelUrlPath { get { return null; } }
        public bool UseFacebookBeta { get { return false; } }
    }
}