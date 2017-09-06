using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            this.ToTable("Product");
            this.HasKey(p => p.Id);
            this.Property(p => p.Name).IsRequired().HasMaxLength(400);
            this.Property(p => p.ShortDescription);
            this.Property(p => p.FullDescription);
            this.Property(p => p.AdminComment);
            this.Property(p => p.MetaKeywords).HasMaxLength(400);
            this.Property(p => p.MetaDescription);
            this.Property(p => p.MetaTitle).HasMaxLength(400);
            this.Property(p => p.CustomerId);
            this.Property(p => p.MinimumOrderQuantity);
            this.HasRequired(p => p.Customer)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CustomerId)
                .WillCascadeOnDelete();
            this.HasMany(p => p.ProductAttributes)
                .WithMany(a => a.Products)
                .Map(map => map.ToTable("Product_ProductAttributeValue"));
            this.HasMany(p => p.CustomerInformationAttributes)
                .WithMany(a => a.Products)
                .Map(map => map.ToTable("Product_CustomerInformationAttributeValue"));
            this.HasMany(p => p.Regions)
                .WithMany(r => r.Products)
                .Map(x => x.ToTable("Product_Region"));
        }
    }
}