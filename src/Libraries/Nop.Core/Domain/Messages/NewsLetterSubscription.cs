using Nop.Core.Domain.Localization;
using System;

namespace Nop.Core.Domain.Messages
{
    /// <summary>
    /// Represents NewsLetterSubscription entity
    /// </summary>
    public partial class NewsLetterSubscription : BaseEntity
    {       
        /// <summary>
        /// Gets or sets the newsletter subscription GUID
        /// </summary>
        public virtual Guid NewsLetterSubscriptionGuid { get; set; }

        /// <summary>
        /// Gets or sets the subcriber email
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether subscription is active
        /// </summary>
        public virtual bool Active { get; set; }

        /// <summary>
        /// Gets or sets the date and time when subscription was created
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Get or set subscription language
        /// </summary>
        public virtual Language Language { get; set; }

        /// <summary>
        /// Get or set subscription languageId
        /// </summary>
        public virtual int LanguageId { get; set; }

        /// <summary>
        /// Indicates is this subscribber subscribed to new product newsletter
        /// </summary>
        public virtual bool NewProduct { get; set; }

        /// <summary>
        /// Indicates is this subscribber subscribed to new buying requests newsletter
        /// </summary>
        public virtual bool NewBuyingRequests { get; set; }
    }
}
