using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Customers
{
    public partial class CompanyMap : EntityTypeConfiguration<CompanyInformation>
    {
        public CompanyMap()
        {
            this.ToTable("CompanyInformation");
        }
    }
}
