using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Customers
{
    public class CustomerInformationProductAttributeMap : EntityTypeConfiguration<CustomerInformationProductAttribute>
    {
        public CustomerInformationProductAttributeMap()
        {
            this.ToTable("CustomerInformationProductAttribute");
            this.Ignore(x => x.ProductAddControlType);
            this.Ignore(x => x.ProductSearchControlType);
        }
    }
}
