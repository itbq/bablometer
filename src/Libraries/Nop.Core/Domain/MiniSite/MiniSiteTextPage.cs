using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.MiniSite
{
    public class MiniSiteTextPage : BaseEntity, ILocalizedEntity, ISlugSupported
    {
        /// <summary>
        /// Page header title
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Page title tag
        /// </summary>
        public virtual string PageTitle { get; set; }

        /// <summary>
        /// Page name in meenu
        /// </summary>
        public virtual string MenuTitle { get; set; }

        /// <summary>
        /// page html code
        /// </summary>
        public virtual string Html { get; set; }

        /// <summary>
        /// User mini site where this page displayed
        /// </summary>
        public virtual int UserMiniSiteId { get; set; }

        public virtual UserMiniSite UserMiniSite { get; set; }
    }
}
