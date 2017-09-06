using Nop.Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Nop.Core.Domain.MiniSite
{
    public class BannerMiniSite:BaseEntity,ILocalizedEntity
    {
        /// <summary>
        ///Banner url
        /// </summary>
        public virtual string BannerUrl { get; set; }

        /// <summary>
        /// Bannet img title text
        /// </summary>
        public virtual string BannerTitle { get; set; }

        /// <summary>
        /// /Banner picture id
        /// </summary>
        public virtual int BannerPictureId { get; set; }

        /// <summary>
        /// Banner alt tag attribute
        /// </summary>
        public virtual string BannerAlt { get; set; }
        /// <summary>
        /// Id of user mini site where this banner displays
        /// </summary>
        public virtual int? UserMiniSiteId { get; set; }

        public virtual UserMiniSite UserMiniSite { get; set; }
    }
}
