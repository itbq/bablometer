using Nop.Admin.Models.Regions;
using Nop.Core.Domain.Regions;
using Nop.Services.Regions;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class RegionController : BaseNopController
    {
        private readonly IRegionService _regionService;
        private readonly ICityService _cityService;

        public RegionController(IRegionService regionService,
            ICityService cityService)
        {
            this._regionService = regionService;
            this._cityService = cityService;
        }

        #region Region part
        public ActionResult List()
        {
            var model = new GridModel<RegionModel>();
            model.Data = _regionService.GetAllRegions().Select(x=>new RegionModel()
            {
                Code = x.Code,
                Id = x.Id,
                Title = x.Title
            });

            model.Total = model.Data.Count();

            return View(model);
        }

        [HttpPost, GridAction]
        public ActionResult List(GridCommand command)
        {
            var model = new GridModel<RegionModel>();
            model.Data = _regionService.GetAllRegions().Select(x => new RegionModel()
            {
                Code = x.Code,
                Id = x.Id,
                Title = x.Title
            });

            model.Total = model.Data.Count();

            return new JsonResult()
            {
                Data = model
            };
        }

        [HttpPost,GridAction]
        public ActionResult Insert(RegionModel model, GridCommand command)
        {
            ModelState.Remove("Id");
            if (model.Code != 0)
            {
                var regionOld = _regionService.GetByRegionCode(model.Code);
                if (regionOld != null)
                {
                    ModelState.AddModelError("Code", "ITB.Admin.Region.Code.Exists");
                }
            }

            if (!ModelState.IsValid)
            {
                var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var region = new Region()
            {
                Title = model.Title,
                Code = model.Code
            };

            _regionService.Insert(region);

            return List(command);
        }

        [HttpPost, GridAction]
        public ActionResult Update(RegionModel model, GridCommand command)
        {
            if (model.Code != 0)
            {
                var regionOld = _regionService.GetByRegionCode(model.Code);
                if (regionOld != null)
                {
                    ModelState.AddModelError("Code", "ITB.Admin.Region.Code.Exists");
                }
            }

            if (!ModelState.IsValid)
            {
                var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var region = _regionService.GetById(model.Id);
            if (region == null)
                return List(command);

            region.Code = model.Code;
            region.Title = model.Title;
            _regionService.Update(region);

            return List(command);
        }

        [HttpPost, GridAction]
        public ActionResult Delete(int Id, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var region = _regionService.GetById(Id);
            if (region == null)
                return List(command);

            _regionService.Delete(region);

            return List(command);
        }

        #endregion

        #region
        public ActionResult CityList(int regionId)
        {
            var model = new GridModel<CityModel>();
            var region = _regionService.GetById(regionId);
            if (regionId == 0)
                return RedirectToAction("List");

            model.Data = region.Cities.Select(x => new CityModel()
            {
                Title = x.Title,
                Id = x.Id
            });
            model.Total = model.Data.Count();

            ViewBag.RegionTitle = region.Title;
            ViewBag.RegionId = regionId;

            return View(model);
        }

        [HttpPost,GridAction]
        public ActionResult CityList(int regionId, GridCommand command)
        {
            var model = new GridModel<CityModel>();
            var region = _regionService.GetById(regionId);
            if (regionId == 0)
                return RedirectToAction("List");

            model.Data = region.Cities.Select(x => new CityModel()
            {
                Title = x.Title,
                Id = x.Id
            });
            model.Total = model.Data.Count();

            return new JsonResult()
                {
                    Data = model.Data
                };
        }

        [HttpPost, GridAction]
        public ActionResult InsertCity(CityModel model, GridCommand command)
        {
            ModelState.Remove("Id");
            var oldCity = _cityService.GetCityByRegionAndTitle(model.Title, model.RegionId);
            if (oldCity != null)
            {
                ModelState.AddModelError("Title", "ITB.Admin.City.Title.Exists");
            }
            if (!ModelState.IsValid)
            {
                var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var city = new City()
            {
                RegionId = model.RegionId,
                Title = model.Title
            };

            _cityService.Insert(city);
            return CityList(model.RegionId, command);
        }

        [HttpPost, GridAction]
        public ActionResult UpdateCity(CityModel model, GridCommand command)
        {
            var oldCity = _cityService.GetCityByRegionAndTitle(model.Title, model.RegionId);
            if (oldCity != null)
            {
                ModelState.AddModelError("Title", "ITB.Admin.City.Title.Exists");
            }
            if (!ModelState.IsValid)
            {
                var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var city = _cityService.GetById(model.Id);
            if (city == null)
                return CityList(model.RegionId, command);

            city.Title = model.Title;
            _cityService.Update(city);

            return CityList(model.RegionId, command);
        }

        [HttpPost, GridAction]
        public ActionResult DeleteCity(CityModel model, GridCommand command)
        {
            var oldCity = _cityService.GetCityByRegionAndTitle(model.Title, model.RegionId);
            if (oldCity != null)
            {
                ModelState.AddModelError("Title", "ITB.Admin.City.Title.Exists");
            }
            if (!ModelState.IsValid)
            {
                var modelStateErrors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var city = _cityService.GetById(model.Id);
            if (city == null)
                return CityList(model.RegionId, command);

            city.Title = model.Title;
            _cityService.Update(city);

            return CityList(model.RegionId, command);
        }

        [HttpPost, GridAction]
        public ActionResult DeleteCity(int Id,int regionId, GridCommand command)
        {
            var city = _cityService.GetById(Id);
            if (city == null)
                return CityList(regionId, command);

            _cityService.Delete(city);

            return CityList(regionId, command);
        }
        #endregion
    }
}
