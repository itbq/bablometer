using Nop.Core.Data;
using Nop.Core.Domain.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Regions
{
    public class CityService : ICityService
    {
        private readonly IRepository<City> _cityRepository;

        public CityService(IRepository<City> cityRepository)
        {
            this._cityRepository = cityRepository;
        }

        public City GetById(int id)
        {
            if (id == 0)
                return null;
            return _cityRepository.GetById(id);
        }

        public void Insert(City city)
        {
            if (city == null)
                throw new ArgumentNullException("City");

            _cityRepository.Insert(city);
        }

        public void Delete(City city)
        {
            if (city == null)
                throw new ArgumentNullException("City");

            _cityRepository.Delete(city);
        }

        public void Update(City city)
        {
            if (city == null)
                throw new ArgumentNullException("City");

            _cityRepository.Update(city);
        }

        public City GetCityByRegionAndTitle(string title, int regionId)
        {
            if (String.IsNullOrEmpty(title)) 
                return null;
            if (regionId == 0)
                return null;

            return _cityRepository.Table.Where(x => x.Title == title && x.RegionId == regionId).FirstOrDefault();
        }

        public IList<City> GetAllCities()
        {
            return _cityRepository.Table.ToList();
        }
    }
}
