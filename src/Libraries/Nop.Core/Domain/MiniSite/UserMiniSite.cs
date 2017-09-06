using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.MiniSite
{
    public class UserMiniSite : BaseEntity, ILocalizedEntity
    {
        private ICollection<SliderItem> _sliderItems;
        private ICollection<BannerMiniSite> _banners;
        private ICollection<MiniSiteTextPage> _textPages;

        /// <summary>
        /// Customer id
        /// </summary>
        //public virtual int CustomerId { get; set; }

        /// <summary>
        /// Url of minisite logo
        /// </summary>
        public virtual string LogoUrl { get; set; }

        /// <summary>
        /// Minisite footer email
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Email where to send emails from contact us page
        /// </summary>
        public virtual string ContactEmail { get; set; }

        /// <summary>
        /// Footer copyriight information
        /// </summary>
        public virtual string CopyrightInfo { get; set; }

        /// <summary>
        /// MiniSite layoutId
        /// </summary>
        public virtual int LayoutId { get; set; }

        /// <summary>
        /// Mini site domain name
        /// </summary>
        public virtual string DomainName { get; set; }

        /// <summary>
        /// Indicates is this miniSite active
        /// </summary>
        public virtual bool Active { get; set; }
 
        /// <summary>
        /// Customer who owns minisiite
        /// </summary>
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// Html code of about Us page
        /// </summary>
        public virtual string AboutUsPage { get; set; }

        public virtual string AbouotUsTitle { get; set; }

        public virtual string AboutUsPageTitle { get; set; }

        /// <summary>
        /// Html code of about products page
        /// </summary>
        public virtual string AboutProductsPage { get; set; }

        public virtual string AboutProductsTitle { get; set; }

        public virtual string AboutProductsPageTitle { get; set; }

        /// <summary>
        /// Logo picture Id
        /// </summary>
        public int LogoId { get; set; }

        /// <summary>
        /// MiniSite layout
        /// </summary>
        public virtual MiniSiteLayout MiniSiteLayout { get; set; }

        /// <summary>
        /// Mini site slides
        /// </summary>
        public virtual ICollection<SliderItem> SliderItems
        {
            get { return _sliderItems ?? (_sliderItems = new List<SliderItem>()); }
            protected set { _sliderItems = value; }
        }

        /// <summary>
        /// Mini site banners
        /// </summary>
        public virtual ICollection<BannerMiniSite> Banners
        {
            get { return _banners ?? (_banners = new List<BannerMiniSite>()); }
            protected set { _banners = value; }
        }

        /// <summary>
        /// Mini site text pages
        /// </summary>
        public virtual ICollection<MiniSiteTextPage> Textpages
        {
            get { return _textPages ?? (_textPages = new List<MiniSiteTextPage>()); }
            protected set { _textPages = value; }
        }
    }
}
