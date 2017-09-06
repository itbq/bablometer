using Nop.Core.Data;
using Nop.Core.Domain.Logging;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Logging
{
    public class SearchLogService : ISearchLogService
    {
        private readonly IRepository<SearchLog> _repository;
        private readonly IRepository<SearchQueryLog> _searchQueryRepository;

        public SearchLogService(IRepository<SearchLog> repository,
            IRepository<SearchQueryLog> searchQueryRepository)
        {
            this._repository = repository;
            this._searchQueryRepository = searchQueryRepository;
        }

        public SearchLog GetById(int id)
        {
            if (id == 0)
                return null;

            return _repository.GetById(id);
        }

        public void Insert(SearchLog log)
        {
            if (log == null)
                throw new ArgumentNullException("Search log");

            _repository.Insert(log);
        }

        public void Delete(SearchLog log)
        {
            if (log == null)
                throw new ArgumentNullException("Search log");

            _repository.Delete(log);
        }

        public void Update(SearchLog log)
        {
            if (log == null)
                throw new ArgumentNullException("Search log");

            _repository.Update(log);
        }

        public void LogSearchQuery(IList<SearchProductAttributeValue> searchAttributes, IList<SearchProductAttributeValue> customerAttributes, SearchQueryLog query)
        {
            if (searchAttributes != null)
            {
                foreach (var attribute in searchAttributes)
                {
                    var log = new SearchLog()
                    {
                        CategoryProductAttributeId = attribute.CategoryProductAttributeId,
                        ExactValue = attribute.ExactValue,
                        IdValue = attribute.IdValue,
                        MaxValue = attribute.MaxValue,
                        MinValue = attribute.MinValue,
                        Textvalue = attribute.Textvalue,
                        CustomerAttribute = false
                    };

                    query.SearchLogs.Add(log);
                    _searchQueryRepository.Update(query);
                }
            }

            if (customerAttributes != null)
            {
                foreach (var attribute in customerAttributes)
                {
                    var log = new SearchLog()
                    {
                        CategoryProductAttributeId = attribute.CategoryProductAttributeId,
                        ExactValue = attribute.ExactValue,
                        IdValue = attribute.IdValue,
                        MaxValue = attribute.MaxValue,
                        MinValue = attribute.MinValue,
                        Textvalue = attribute.Textvalue,
                        CustomerAttribute = true
                    };

                    query.SearchLogs.Add(log);
                    _searchQueryRepository.Update(query);
                }
            }
        }
        
        public void LogSearchQuerryWithParameters(IList<SearchProductAttributeValue> searchAttributes,
            IList<SearchProductAttributeValue> customerAttributes,
            int activityLogId,
            int regionId,
            int customerId,
            int categoryId,
            int productTagId,
            int bankRating,
            int productRating)
        {
            var searchQuery = new SearchQueryLog()
            {
                ActivityLogId = activityLogId,
                RegionId = regionId,
                BankRating = bankRating,
                CategoryId = categoryId,
                ProductTagId = productTagId,
                ProductRating = productRating,
                CustomerId = customerId
            };

            _searchQueryRepository.Insert(searchQuery);

            LogSearchQuery(searchAttributes, customerAttributes, searchQuery);
        }
    }
}
