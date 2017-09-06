using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.MiniSite
{
    public class SliderItem : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Slide picture id
        /// </summary>
        public virtual int PictureId { get; set; }

        /// <summary>
        /// slide title text
        /// </summary>
        public virtual string TitleText { get; set; }

        /// <summary>
        /// slide short text
        /// </summary>
        public virtual string ShortText { get; set; }

        /// <summary>
        /// Slide url
        /// </summary>
        public virtual string Url { get; set; }

        /// <summary>
        /// Slide number
        /// </summary>
        public virtual int SlideNumber { get; set; }

        /// <summary>
        /// Minisite id where this slide displayed
        /// </summary>
        public virtual int? UserMiniSiteId { get; set; }

        /// <summary>
        /// Minisit where this slide displayed
        /// </summary>
        public virtual UserMiniSite UserMiniSite { get; set; }
    }
}
