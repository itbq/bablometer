using Nop.Core.Domain.MiniSite;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.MiniSite
{
    public class SliderItemMap:EntityTypeConfiguration<SliderItem>
    {
        public SliderItemMap()
        {
            this.ToTable("SliderItem");

            this.HasOptional(x=>x.UserMiniSite)
                .WithMany(m=>m.SliderItems)
                .HasForeignKey(x=>x.UserMiniSiteId).WillCascadeOnDelete();
        }
    }
}
