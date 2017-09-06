using Nop.Core.Domain.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Logging
{
    public class SearchQueryLogMap : EntityTypeConfiguration<SearchQueryLog>
    {
        public SearchQueryLogMap()
        {
            this.ToTable("SearchQueryLog");

            this.HasRequired(x => x.ActivityLog)
                .WithMany()
                .HasForeignKey(x => x.ActivityLogId).WillCascadeOnDelete(true);
        }
    }
}
