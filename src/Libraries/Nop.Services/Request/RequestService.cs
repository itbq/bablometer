using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain;
using Nop.Core.Data;
using Nop.Core.Caching;
using Nop.Services.Events;

namespace Nop.Services.RequestServices
{
    public partial class RequestService:IRequestService
    {

        private const string REQUEST_BY_ID_KEY = "Nop.request.id-{0}";
        private const string REQUEST_BY_CUSTOMER_ID_KEY = "Nop.request.cuctomerid-{0}";
        private const string REQUEST_BY_PRODUCT_ID_KEY = "Nop.request.productid-{0}";
        private const string REQUEST_PATTERN_KEY = "Nop.request.";

        private readonly IRepository<Request> _repository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        public RequestService(IRepository<Request> repository,
            ICacheManager cacheManager,
            IEventPublisher eventPublisher)
        {
            this._repository = repository;
            this._cacheManager = cacheManager;
            this._eventPublisher = eventPublisher;
        }

        public List<Request> GetAllRequests()
        {
            return _repository.Table.ToList();
        }

        public Request GetRequestById(int id)
        {
            string key = string.Format(REQUEST_BY_ID_KEY, id);

            return _cacheManager.Get(key, () =>
            {
                var n = _repository.GetById(id);
                return n;
            });
        }

        public void InsertRequest(Request request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            _repository.Insert(request);

            _eventPublisher.EntityInserted(request);
        }

        public void DeleteRequest(Request request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            _repository.Delete(request);

            _cacheManager.RemoveByPattern(REQUEST_PATTERN_KEY);

            _eventPublisher.EntityDeleted(request);
        }

        public void UpdateRequest(Request request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            _repository.Update(request);

            _cacheManager.RemoveByPattern(REQUEST_PATTERN_KEY);

            _eventPublisher.EntityUpdated(request);
        }

        /// <summary>
        /// Get all customer requests
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public IList<Request> GetCustomerRequests(int customerId)
        {
            string key = string.Format(REQUEST_BY_CUSTOMER_ID_KEY, customerId);

            return _cacheManager.Get(key, () =>
            {
                var n = _repository.Table.Where(x => x.CustomerId == customerId).ToList();
                return n;
            });
        }

        /// <summary>
        /// get all product requests
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public IList<Request> GetProductRequests(int productId)
        {
            string key = string.Format(REQUEST_BY_PRODUCT_ID_KEY, productId);

            return _cacheManager.Get(key, () =>
            {
                var n = _repository.Table.Where(x => x.ProductId == productId).ToList();
                return n;
            });
        }
    }
}
