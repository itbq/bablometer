using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Event
{
    public partial class Event : BaseEntity, ISlugSupported, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets event title
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets value of full event description
        /// </summary>
        public virtual string ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets value of event picture id for home page
        /// </summary>
        public virtual int PictureId { get; set; }

        /// <summary>
        /// Gets or sets value of event picture id for catalog and details page
        /// </summary>
        public virtual int CatalogPictureId { get; set; }

        /// <summary>
        /// Gets or sets full description of event
        /// </summary>
        public virtual string FullDescription { get; set; }

        /// <summary>
        /// Gets or sets event start date
        /// </summary>
        public virtual DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets value of end date of event
        /// If event is one day long this field is null
        /// </summary>
        public virtual DateTime? EndDate { get; set; }
    }
}
