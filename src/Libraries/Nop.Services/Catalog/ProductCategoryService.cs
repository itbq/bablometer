using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Catalog
{
    public partial class ProductCategoryService : IProductCategoryService
    {
        private readonly IRepository<ProductCategory> _repository;

        public ProductCategoryService(IRepository<ProductCategory> repository)
        {
            this._repository = repository;
        }

        public void Delete(ProductCategory category)
        {
            if (category == null)
                throw new ArgumentNullException("Category");

            _repository.Delete(category);
        }
    }
}
