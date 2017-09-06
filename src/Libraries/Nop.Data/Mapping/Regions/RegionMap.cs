using Nop.Core.Domain.Regions;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Regions
{
    public class RegionMap : EntityTypeConfiguration<Region>
    {
        public RegionMap()
        {
            this.ToTable("Region");

            this.Property(x => x.Title).IsRequired();
            this.Property(x => x.Code).IsRequired();
            this.HasMany(x=>x.Products)
                .WithMany(p=>p.Regions)
                .Map(x=>x.ToTable("Product_Region"));
        }
    }
}
