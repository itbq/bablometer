using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.BrandDomain;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// BrandService interface
    /// </summary>
    public partial interface IBrandService
    {
        IList<Brand> GetAllBrands();
        void DeleteBrand(Brand Brand);
        Brand GetBrandById(int BrandId);
        void InsertBrand(Brand Brand);
        void UpdateBrand(Brand Brand);
        Brand GetBrandByName(string name);
    }
}
