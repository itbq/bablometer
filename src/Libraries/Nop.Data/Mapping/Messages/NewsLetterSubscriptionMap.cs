using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Mapping.Messages
{
    public partial class NewsLetterSubscriptionMap : EntityTypeConfiguration<NewsLetterSubscription>
    {
        public NewsLetterSubscriptionMap()
        {
            this.ToTable("NewsLetterSubscription");
            this.HasKey(nls => nls.Id);

            this.Property(nls => nls.Email).IsRequired().HasMaxLength(255);
            this.HasRequired(nls => nls.Language)
                .WithMany()
                .HasForeignKey(nls => nls.LanguageId);
        }
    }
}