using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalAuth.Vkontakte.Core
{
    public interface IVkontakteApplication
    {
        string AppId { get; }
        string AppSecret { get; }
        string CancelUrlPath { get; }
        string CanvasPage { get; }
        string CanvasUrl { get; }
        string SecureCanvasUrl { get; }
        string SiteUrl { get; }
        bool UseFacebookBeta { get; }
    }
}
