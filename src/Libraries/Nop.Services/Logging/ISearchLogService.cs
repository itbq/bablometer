using Nop.Core.Domain.Logging;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Logging
{
    public interface ISearchLogService
    {
        SearchLog GetById(int id);
        void Insert(SearchLog log);
        void Delete(SearchLog log);
        void Update(SearchLog log);
        void LogSearchQuery(IList<SearchProductAttributeValue> searchAttributes, IList<SearchProductAttributeValue> customerAttributes, SearchQueryLog query);
        void LogSearchQuerryWithParameters(IList<SearchProductAttributeValue> searchAttributes,
            IList<SearchProductAttributeValue> customerAttributes,
            int activityLogId,
            int regionId,
            int customerId,
            int categoryId,
            int productTagId,
            int bankRating,
            int productRating);
    }
}
