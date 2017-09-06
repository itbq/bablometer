using Nop.Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Messages
{
    public partial class NewsletterDates:BaseEntity
    {
        /// <summary>
        /// Language id of newsletter submission
        /// </summary>
        public virtual int LanguageId { get; set; }

        /// <summary>
        /// Date of this newsletter in current languge submit
        /// </summary>
        public virtual DateTime LastSubmit { get; set; }

        /// <summary>
        /// Language of newsletter subscription
        /// </summary>
        public virtual Language Language { get; set; }

        /// <summary>
        /// Shows is this newsletter - product newsletter
        /// </summary>
        public virtual bool IsProduct { get; set; }
    }
}
