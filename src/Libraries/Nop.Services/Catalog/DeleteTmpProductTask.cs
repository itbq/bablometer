using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Catalog
{
    public partial class DeleteTmpProductTask : ITask
    {
        private readonly IRepository<Product> _productRepository;
        public DeleteTmpProductTask(IRepository<Product> productRepository)
        {
            this._productRepository = productRepository;
        }

        public void Execute()
        {
            //60*24 = 1 day
            var olderThanMinutes = 60; //TODO move to settings
            var endDate = DateTime.UtcNow.AddMinutes(-olderThanMinutes);
            var products = _productRepository.Table.Where(x => x.Name == "tmp_product" && x.CreatedOnUtc < endDate).ToList();
            foreach (var product in products)
            {
                _productRepository.Delete(product);
            }
        }
    }
}
