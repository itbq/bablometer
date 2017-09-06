using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Customers
{
    public partial class CompanyInformationViewMap:EntityTypeConfiguration<CompanyInformationView>
    {
        public CompanyInformationViewMap()
        {
            this.ToTable("CompanyInformationView");

            this.HasRequired(x => x.Customer)
                .WithMany(c => c.CompanyInformationViews)
                .HasForeignKey(x => x.CustomerId)
                .WillCascadeOnDelete(true);

            this.HasRequired(x => x.CompanyInformation)
                .WithMany(c => c.CompanyInformationViews)
                .HasForeignKey(x => x.CompanyInformationId)
                .WillCascadeOnDelete(true);
        }
    }
}
