using Nop.Core.Domain.Regions;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Regions
{
    public class CityMap : EntityTypeConfiguration<City>
    {
        public CityMap()
        {
            this.ToTable("City");

            this.HasRequired(x => x.Region)
                .WithMany(r => r.Cities)
                .HasForeignKey(x => x.RegionId).WillCascadeOnDelete(true);
        }
    }
}
