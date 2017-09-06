using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Logging
{
    public class SearchQueryLog : BaseEntity
    {
        private ICollection<SearchLog> _searchLogs = new List<SearchLog>();

        public virtual ActivityLog ActivityLog { get; set; }
        public virtual int ActivityLogId { get; set; }
        public virtual int RegionId { get; set; }
        public virtual int CustomerId { get; set; }
        public virtual int CategoryId { get; set; }
        public virtual int ProductTagId { get; set; }
        public virtual int BankRating { get; set; }
        public virtual int ProductRating { get; set; }
        public virtual ICollection<SearchLog> SearchLogs
        {
            get { return _searchLogs ?? new List<SearchLog>(); }
            set { this._searchLogs = value; }
        }
    }
}
