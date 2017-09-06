using Nop.Core.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.BannerMap
{
    public partial class BannerMap : EntityTypeConfiguration<Banner>
    {
        public BannerMap()
        {
            this.ToTable("Banner");
            this.Property(x => x.PictureId)
                .IsRequired();
            this.Property(x => x.Height)
                .IsRequired();
            this.Property(x => x.Size)
                .IsRequired();
            this.Property(x => x.DisplayOnMain)
                .IsRequired();

            this.Ignore(x => x.BannerType);
        }
    }
}
