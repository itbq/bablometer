using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Media;

namespace Nop.Data.Mapping.Media
{
    public partial class DownloadMap : EntityTypeConfiguration<Download>
    {
        public DownloadMap()
        {
            this.ToTable("Download");
            this.HasKey(p => p.Id);
            this.Property(p => p.DownloadBinary).IsMaxLength();
            this.HasOptional(p => p.Company)
                .WithMany(p => p.CompanyDocuments)
                .HasForeignKey(p => p.CompanyId).WillCascadeOnDelete();
        }
    }
}