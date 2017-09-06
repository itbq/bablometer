using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Newsletter
{
    public partial class NewsletterBoxModel : BaseNopModel
    {
        public string NewsletterEmail { get; set; }
        public int LanguageId { get; set; }
    }
}