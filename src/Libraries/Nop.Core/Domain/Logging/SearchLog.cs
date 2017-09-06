using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Logging
{
    public class SearchLog : BaseEntity
    {
        public virtual double MaxValue { get; set; }
        public virtual double MinValue { get; set; }
        public virtual double ExactValue { get; set; }
        public virtual string Textvalue { get; set; }
        public virtual int IdValue { get; set; }
        public virtual int CategoryProductAttributeId { get; set; }
        public virtual bool CustomerAttribute { get; set; }
        public virtual int CustomerActivityId { get; set; }
        public virtual SearchQueryLog CustomerActivity { get; set; }
    }
}
