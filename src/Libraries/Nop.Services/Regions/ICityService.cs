using Nop.Core.Domain.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Regions
{
    public interface ICityService
    {
        City GetById(int id);
        void Insert(City city);
        void Delete(City city);
        void Update(City city);
        City GetCityByRegionAndTitle(string title, int regionId);
        IList<City> GetAllCities();
    }
}
