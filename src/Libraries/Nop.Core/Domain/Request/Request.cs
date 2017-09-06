using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain
{
    public partial class Request:BaseEntity
    {
        /// <summary>
        /// Comment leaved by person who responded request
        /// </summary>
        public virtual string ProposeComment { get; set; }

        /// <summary>
        /// Owner comment about reason why he accepted or rejected request
        /// </summary>
        public virtual string ResponseComment { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Product id of this deal
        /// </summary>
        public virtual int ProductId { get; set; }

        /// <summary>
        /// Product of this deal
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// Id of customer that made request
        /// </summary>
        public virtual int CustomerId { get; set; }

        /// <summary>
        /// Customer that made request
        /// </summary>
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// Indicates is this request accepted
        /// </summary>
        public virtual bool? Accepted { get; set; }

        /// <summary>
        /// Item quantity
        /// </summary>
        public virtual int? Quantity { get; set; }

        /// <summary>
        /// Time of responce
        /// </summary>
        public virtual DateTime? ResponsedOnUtc { get; set; }

        /// <summary>
        /// Is this request new
        /// </summary>
        public virtual bool IsNew { get; set; }
    }
}
