using Nop.Core.Domain.BrandDomain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.BranMap
{
    public partial class BrandMap : EntityTypeConfiguration<Brand>
    {
        public BrandMap()
        {
            this.ToTable("Brand");

            this.HasMany(x => x.Customers)
                .WithMany(c => c.Brands)
                .Map(x =>
                    {
                        x.ToTable("Customer_Brand");
                    });
        }

    }
}
