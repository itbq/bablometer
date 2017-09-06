using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class CategoryToCategoryProductAttributeGroupMap : EntityTypeConfiguration<CategoryToCategoryProductAttributeGroup>
    {
        public CategoryToCategoryProductAttributeGroupMap()
        {
            this.ToTable("Category_CategoryProductAttributeGroup_Map");
            this.HasKey(pva => pva.Id);

            this.Property(pva => pva.CategoryProductAttributeGroupId);

            this.Property(pva => pva.CategoryId);

            this.Property(pva => pva.DisplayOrder);

            this.HasRequired(c => c.Category).WithMany(c => c.CategoryToCategoryProductAttributeGroups).HasForeignKey(c => c.CategoryId);

            this.HasRequired(c => c.CategoryProductAttributeGroup).WithMany(c => c.CategoryToCategoryProductAttributeGroups).HasForeignKey(c => c.CategoryProductAttributeGroupId);

            
        }
    }
}
