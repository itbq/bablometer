using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Catalog
{
    public class ConversionImageMap:EntityTypeConfiguration<ConversionImage>
    {
        public ConversionImageMap()
        {
            this.ToTable("ConversionImage");
            this.HasRequired(x => x.AttributeGroup)
                .WithMany(g => g.ConversionImages)
                .HasForeignKey(x => x.CategoryAttributeGroupId)
                .WillCascadeOnDelete(true);
        }
    }
}
