using Nop.Core.Domain.MiniSite;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.MiniSite
{
    public class MiniSiteTextPageMap:EntityTypeConfiguration<MiniSiteTextPage>
    {
        public MiniSiteTextPageMap()
        {
            this.ToTable("MiniSiteTextPage");

            this.HasRequired(x => x.UserMiniSite)
                .WithMany(m => m.Textpages)
                .HasForeignKey(x => x.UserMiniSiteId).WillCascadeOnDelete();
        }
    }
}
