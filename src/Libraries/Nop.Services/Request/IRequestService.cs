using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain;
using Nop.Core.Data;

namespace Nop.Services.RequestServices
{
    public interface IRequestService
    {
        List<Request> GetAllRequests();
        void UpdateRequest(Request request);
        void DeleteRequest(Request request);
        void InsertRequest(Request request);
        Request GetRequestById(int id);
        IList<Request> GetCustomerRequests(int customerId);
        IList<Request> GetProductRequests(int productId);
    }
}
