using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Topics
{
    /// <summary>
    /// Represents a topic
    /// </summary>
    public partial class Topic : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this topic should be included in sitemap
        /// </summary>
        public virtual bool IncludeInSitemap { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this topic is password protected
        /// </summary>
        public virtual bool IsPasswordProtected { get; set; }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public virtual string Password { get; set; }

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public virtual string Body { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public virtual string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public virtual string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public virtual string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets topic priority in menu
        /// </summary>
        public virtual int Priority { get; set; }

        /// <summary>
        /// Gets or sets display this topic in menu or not
        /// </summary>
        public virtual bool DisplayInMenu { get; set; }

        /// <summary>
        /// Gets or sets display this topic in footer menu or not
        /// </summary>
        public virtual bool DisplayInFooterMenu { get; set; }

        /// <summary>
        /// Topic menu title
        /// </summary>
        public virtual string MenuTitle { get; set; }
    }
}
