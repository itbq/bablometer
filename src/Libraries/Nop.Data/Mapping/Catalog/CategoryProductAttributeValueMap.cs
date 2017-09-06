using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class CategoryProductAttributeValueMap: EntityTypeConfiguration<CategoryProductAttributeValue>
    {
        public CategoryProductAttributeValueMap()
        {
            this.ToTable("CategoryProductAttributeValue");
            this.HasKey(pvav => pvav.Id);
            this.Property(pvav => pvav.Name).HasMaxLength(400);
            this.Property(pvav => pvav.ColorSquaresRgb).HasMaxLength(100);

            this.HasRequired(pvav => pvav.CategoryProductAttribute)
                .WithMany(pva => pva.CategoryProductAttributeValues)
                .HasForeignKey(pvav => pvav.CategoryProductAttributeId).WillCascadeOnDelete(true);

            this.HasMany(pvav=>pvav.Products)
                .WithMany(p=>p.ProductAttributes)
                .Map(map => map.ToTable("Product_ProductAttributeValue"));

            this.HasOptional(x => x.Currency)
                .WithMany()
                .HasForeignKey(x => x.CurrencyId);
        }
    }
}
