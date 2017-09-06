using Nop.Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Nop.Core.Domain
{
    public partial class Banner : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// get or set banner picture id
        /// </summary>
        public virtual int PictureId { get; set; }

        /// <summary>
        /// get or set bannet title text
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// get or set bannet alt attribute text
        /// </summary>
        public virtual string Alt { get; set; }

        /// <summary>
        /// Get or set Banner size
        /// </summary>
        public virtual int Size { get; set; }

        /// <summary>
        /// Get or set Banner height
        /// </summary>
        public virtual int Height { get; set; }
        
        /// <summary>
        /// get or set banner type id 
        /// </summary>
        public virtual int BannerTypeId { get; set; }

        public virtual int? CategoryId { get; set; }
        public BannerTypeEnum BannerType
        {
            get { return (BannerTypeEnum)BannerTypeId; }
            set { this.BannerTypeId = (int)value; }
        }

        /// <summary>
        /// get or set is banner displayed on home page
        /// </summary>
        public virtual bool DisplayOnMain { get; set; }

        /// <summary>
        /// get or set banner url
        /// </summary>
        public virtual string Url { get; set; }

        /// <summary>
        /// get or set banner net
        /// </summary>
        public virtual string NetBanner { get; set; }
    }
}
