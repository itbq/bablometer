using Nop.Core.Domain.Messages;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Messages
{
    public class NewsletterDatesMap:EntityTypeConfiguration<NewsletterDates>
    {
        public NewsletterDatesMap()
        {
            this.ToTable("NewsletterDates");
            this.Property(x => x.LanguageId)
                .IsRequired();
            this.Property(x => x.LastSubmit)
                .IsRequired();
            this.HasRequired(x => x.Language)
                .WithMany()
                .HasForeignKey(x => x.LanguageId);
        }
    }
}
