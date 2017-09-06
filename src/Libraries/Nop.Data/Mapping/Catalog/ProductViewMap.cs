using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductViewMap:EntityTypeConfiguration<ProductView>
    {
        public ProductViewMap()
        {
            this.ToTable("ProductView");

            this.HasRequired(x => x.Customer)
                .WithMany(c => c.ProductViews)
                .HasForeignKey(x => x.CustomerId)
                .WillCascadeOnDelete(true);

            this.HasRequired(x => x.Product)
                .WithMany(p => p.ProductViews)
                .HasForeignKey(x => x.ProductId)
                .WillCascadeOnDelete(true);
        }
    }
}
