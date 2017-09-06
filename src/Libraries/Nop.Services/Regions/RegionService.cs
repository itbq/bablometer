using Nop.Core.Data;
using Nop.Core.Domain.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Regions
{
    public class RegionService : IRegionService
    {
        private readonly IRepository<Region> _regionRepository;

        public RegionService(IRepository<Region> regionRepository)
        {
            this._regionRepository = regionRepository;
        }

        public Region GetById(int id)
        {
            if (id == 0)
                return null;

            return _regionRepository.GetById(id);
        }

        /// <summary>
        /// Get region by region code
        /// </summary>
        /// <param name="code">region code</param>
        /// <returns>resulting region or null if not exists</returns>
        public Region GetByRegionCode(int code)
        {
            if (code == 0)
                return null;
            return _regionRepository.Table.Where(x => x.Code == code).FirstOrDefault();
        }

        public void Insert(Region region)
        {
            if (region == null)
                throw new ArgumentNullException("Regioin");

            _regionRepository.Insert(region);
        }

        public void Update(Region region)
        {
            if (region == null)
                throw new ArgumentNullException("Regioin");

            _regionRepository.Update(region);
        }

        public void Delete(Region region)
        {
            if (region == null)
                throw new ArgumentNullException("Regioin");

            _regionRepository.Delete(region);
        }

        public IList<Region> GetAllRegions()
        {
            return _regionRepository.Table.ToList();
        }
    }
}
