using Nop.Core.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.RequestMap
{
    public class RequestMap:EntityTypeConfiguration<Request>
    {
        public RequestMap()
        {
            this.ToTable("Request");

            this.HasRequired(x => x.Customer)
                .WithMany(c => c.Requests)
                .HasForeignKey(x => x.CustomerId);

            this.HasRequired(x => x.Product)
                .WithMany(p => p.Requests)
                .HasForeignKey(x => x.ProductId);
        }
    }
}
