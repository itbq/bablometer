using Nop.Core.Domain.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Regions
{
    public interface IRegionService
    {
        Region GetById(int id);

        /// <summary>
        /// Get region by region code
        /// </summary>
        /// <param name="code">region code</param>
        /// <returns>resulting region or null if not exists</returns>
        Region GetByRegionCode(int code);
        void Insert(Region region);
        void Update(Region region);
        void Delete(Region region);
        IList<Region> GetAllRegions();
    }
}
