using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductPriceMap : EntityTypeConfiguration<ProductPrice>
    {
        public ProductPriceMap()
        {
            this.ToTable("Product_Price");
            this.HasKey(pt => pt.Id);
            this.Property(pt => pt.Price).IsRequired();
            this.Property(pt => pt.PriceUpdatedOn).IsRequired();
            this.HasRequired(pc => pc.Currency)
               .WithMany()
               .HasForeignKey(pc => pc.CurrencyId);
            this.HasRequired(pc => pc.Product)
              .WithMany()
              .HasForeignKey(pc => pc.ProductId)
              .WillCascadeOnDelete();
        }
    }
}
