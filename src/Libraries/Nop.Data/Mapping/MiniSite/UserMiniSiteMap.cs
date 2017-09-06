using Nop.Core.Domain.MiniSite;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.MiniSite
{
    public class UserMiniSiteMap:EntityTypeConfiguration<UserMiniSite>
    {
        public UserMiniSiteMap()
        {
            this.ToTable("UserMiniSite");
            this.HasOptional(x => x.Customer)
                .WithOptionalDependent(c => c.UserMiniSite)
                .Map(x => x.MapKey("CustomerId"));
            this.HasOptional(x=>x.MiniSiteLayout)
                .WithOptionalPrincipal(l=>l.UserMiniSite)
                .Map(x=>x.MapKey("UserMiniSiteId")).WillCascadeOnDelete(true);
        }
    }
}
