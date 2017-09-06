using Nop.Core.Domain.MiniSite;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.MiniSite
{
    public class BannerMiniSiteMap:EntityTypeConfiguration<BannerMiniSite>
    {
        public BannerMiniSiteMap()
        {
            this.ToTable("BannerMiniSite");
            this.HasRequired(x => x.UserMiniSite)
                .WithMany(b => b.Banners)
                .HasForeignKey(x => x.UserMiniSiteId).WillCascadeOnDelete();
        }
    }
}
