using Nop.Core.Domain.MiniSite;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.MiniSite
{
    public class MiniSiteLayoutMap:EntityTypeConfiguration<MiniSiteLayout>
    {
        public MiniSiteLayoutMap()
        {
            this.ToTable("MiniSitelayout");
            this.HasOptional(x => x.UserMiniSite)
                .WithOptionalDependent(x => x.MiniSiteLayout)
                .Map(x => x.MapKey("UserMiniSiteId")).WillCascadeOnDelete(true);
        }
    }
}
