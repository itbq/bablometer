using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class CategoryProductAttributeGroupMap : EntityTypeConfiguration<CategoryProductAttributeGroup>
    {
        public CategoryProductAttributeGroupMap()
        {
            this.ToTable("CategoryProductAttributeGroup");
            this.HasKey(pva => pva.Id);
            this.Property(pvav => pvav.Name).HasMaxLength(1200);
            this.Property(p => p.Description);
            
        }
    }
}
