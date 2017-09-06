using Nop.Core.Domain.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Logging
{
    public class SearchLogMap : EntityTypeConfiguration<SearchLog>
    {
        public SearchLogMap()
        {
            this.ToTable("SearchLog");

            this.HasRequired(x => x.CustomerActivity)
                .WithMany(sq=>sq.SearchLogs)
                .HasForeignKey(x => x.CustomerActivityId)
                .WillCascadeOnDelete(true);
        }
    }
}
