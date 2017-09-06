using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Customers
{
    public class CustomerInformationProductAttributeValueMap : EntityTypeConfiguration<CustomerInformationProductAttributeValue>
    {
        public CustomerInformationProductAttributeValueMap()
        {
            this.ToTable("CustomerInformationProductAttributeValue");

            this.HasRequired(x => x.CustomerInformationProductAttribute)
                .WithMany()
                .HasForeignKey(x => x.CustomerInformationProductAttributeId);
        }
    }
}
