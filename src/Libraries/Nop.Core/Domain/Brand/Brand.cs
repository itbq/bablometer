using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.BrandDomain
{
    public partial class Brand : BaseEntity, ILocalizedEntity
    {
        private ICollection<Customer> _customers;
        
        /// <summary>
        /// Customers that sells this brand
        /// </summary>
        public virtual ICollection<Customer> Customers 
        {
            get { return _customers ?? (_customers = new List<Customer>()); }
            protected set { _customers = value; }
        }

        /// <summary>
        /// Brand name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Brand description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Brand creation time
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Isthis brand approoved by administrator
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// Brand logoId
        /// </summary>
        public int LogoId { get; set; }
    }
}
