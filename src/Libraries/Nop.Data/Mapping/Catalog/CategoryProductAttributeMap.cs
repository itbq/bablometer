using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{

    public partial class CategoryProductAttributeMap : EntityTypeConfiguration<CategoryProductAttribute>
    {
        public CategoryProductAttributeMap()
        {
            this.ToTable("Category_ProductAttribute_Mapping");
            this.HasKey(pva => pva.Id);

            this.Property(pva => pva.CategoryProductGroupId);

            this.Property(pva => pva.ProductAttributeId);
            //this.Ignore(pva => pva.ProductAttribute);

            this.Property(pva => pva.TextPrompt);

            this.Property(pva => pva.IsRequired);

            this.Property(pva => pva.AttributeControlTypeId);
            this.Ignore(pva => pva.AttributeControlType);
            this.Ignore(pva => pva.SearchControlType);
            this.Property(pva => pva.DisplayOrder);

            this.HasRequired(pva => pva.CategoryProductGroup)
                .WithMany(x => x.CategoryProductAttributes)
                .HasForeignKey(pva => pva.CategoryProductGroupId).WillCascadeOnDelete(true);

            this.HasRequired(pva => pva.ProductAttribute)
                .WithMany(a => a.CategoryProductAttribute)
                .HasForeignKey(pva => pva.ProductAttributeId);
        }
    }
}
