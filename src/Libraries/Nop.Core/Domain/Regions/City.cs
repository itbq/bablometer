using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Regions
{
    public class City : BaseEntity
    {
        /// <summary>
        /// City name
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Id of city region
        /// </summary>
        public virtual int RegionId { get; set; }

        /// <summary>
        /// City region
        /// </summary>
        public virtual Region Region { get; set; }
    }
}
