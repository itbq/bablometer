using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Directory;

namespace Nop.Core.Domain.Catalog
{
    public partial class ProductPrice : BaseEntity
    {
        public virtual decimal Price { get; set; }
        public virtual DateTime PriceUpdatedOn { get; set; }
        public virtual int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual int ProductId { get; set; }
        public virtual Product Product { get; set; }

    }
}
