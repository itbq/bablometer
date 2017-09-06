using Nop.Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.MiniSite
{
    public class MiniSiteLayout : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Mini site id
        /// </summary>
        //public virtual int UserMiniSiteId { get; set; }

        /// <summary>
        /// Layout Html type
        /// </summary>
        public virtual int LayoutHtmlType { get; set; }

        /// <summary>
        /// Path to style folder
        /// </summary>
        public virtual string StyleFolder { get; set; }

        /// <summary>
        /// Root page title
        /// </summary>
        public virtual string RootTitle { get; set; }

        /// <summary>
        /// Css template user in current mini site
        /// </summary>
        public virtual string CssTemplate { get; set; }

        public virtual UserMiniSite UserMiniSite { get; set; }
    }

    public enum LayoutHtmlType
    {
        First = 1,
        Second = 2,
        Third = 3
    }
}
