using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Favorit;

namespace Nop.Data.Mapping.Favorits
{
    public partial class FavoritsMap : EntityTypeConfiguration<FavoritItem>
    {
        public FavoritsMap()
        {
            this.ToTable("Favorits");

            this.HasRequired(x => x.Customer)
                .WithMany(c => c.Favorits)
                .HasForeignKey(x => x.CustomerId)
                .WillCascadeOnDelete();

            this.HasRequired(x => x.Product)
                .WithMany(p => p.Favorits)
                .HasForeignKey(x => x.ProductId)
                .WillCascadeOnDelete();
        }
    }
}
