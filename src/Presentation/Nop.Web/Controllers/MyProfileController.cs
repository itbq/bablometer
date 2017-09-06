using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.UI.Captcha;
using Nop.Web.Models.Common;
using Nop.Web.Models.Customer;
using Nop.Services.Logging;
using Nop.Web.Models.Media;
using Nop.Web.Models.CompanyInformation;
using Nop.Web.Models.Regions;

namespace Nop.Web.Controllers
{
    public partial class CustomerController : BaseNopController
    {
        protected ProfileModel PrepareProfileModel()
        {
            var customer = _workContext.CurrentCustomer;
            var model = new ProfileModel();
            if (customer.BirthdayDate.HasValue)
            {
                model.DateOfBirthDay = customer.BirthdayDate.Value.Day;
                model.DateOfBirthMonth = customer.BirthdayDate.Value.Month;
                model.DateOfBirthYear = customer.BirthdayDate.Value.Year;
            }
            model.CurrencyId = customer.CurrencyId.GetValueOrDefault();
            model.AviableCurrencies = _currencyService.GetAllCurrencies().Select(x => new CurrencyModel() { Id = x.Id, Name = x.CurrencyCode }).ToList();
            model.Email = customer.Email;
            model.FirstName = customer.FirstName;
            model.LastName = customer.LastName;
            if (customer.CurrencyId.HasValue && customer.Income.HasValue)
            {
                var currency = _currencyService.GetCurrencyById(customer.CurrencyId.Value);
                model.Income = (int)(customer.Income * (double) currency.Rate);
                model.CurrencyId = customer.CurrencyId.Value;
            }
            model.Index = customer.PostIndex;
            model.Address = customer.Address;
            model.BirthdayDate = customer.BirthdayDate;
            model.CityId = customer.CityId.HasValue ? customer.CityId.Value : 0;
            model.RegionId = customer.RegionId.HasValue ? customer.RegionId.Value : 0;
            model.ExternalAutentificationRecord = customer.ExternalAuthenticationRecords.Count > 0 ? true : false;
            if (customer.Gender.HasValue)
            {
                model.Gender = customer.Gender.Value == 1 ? "M" : "W";
            }
            model.Regions = new List<RegionModel>();
            model.Regions.Add(new RegionModel() { Id = 0, Title = _localizationService.GetResource("ITB.Portal.Register.SelectRegion") });
            var regions = _regionService.GetAllRegions().OrderBy(x=>x.Title).Select(x => new RegionModel()
            {
                Title = x.Title,
                Id = x.Id
            }).ToList();
            foreach (var regionModel in regions)
            {
                model.Regions.Add(regionModel);
            }
            model.Cities = new List<CityModel>();
            model.Cities.Add(new CityModel() { Id = 0, Title = _localizationService.GetResource("ITB.Portal.Register.SelectCity") });

            if (model.RegionId == 0 || _regionService.GetById(model.RegionId) == null)
            {
                var cities = _cityService.GetAllCities().OrderBy(x=>x.Title).Select(x => new CityModel()
                {
                    Title = x.Title,
                    Id = x.Id
                }).ToList();

                foreach (var cityModel in cities)
                {
                    model.Cities.Add(cityModel);
                }
            }
            else
            {
                var cities = _regionService.GetById(model.RegionId).Cities.OrderBy(x=>x.Title).Select(x => new CityModel()
                {
                    Title = x.Title,
                    Id = x.Id
                }).ToList();

                foreach (var cityModel in cities)
                {
                    model.Cities.Add(cityModel);
                }
            } 
            return model;
        }

        [NopHttpsRequirement(Framework.Security.SslRequirement.Yes)]
        public ActionResult MyProfile(int? languageId)
        {
            if (!languageId.HasValue)
                languageId = _workContext.WorkingLanguage.Id;
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }
            var model = PrepareProfileModel();
            return View(model);
        }

        [NopHttpsRequirement(Framework.Security.SslRequirement.Yes)]
        public ActionResult CustomerProfile(ProfileModel model)
        {
            return View("MyProfile", model);
        }

        [NopHttpsRequirement(Framework.Security.SslRequirement.Yes)]
        [HttpPost]
        public ActionResult MyProfile(ProfileModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("HomePage");
            }

            if(!String.IsNullOrEmpty(model.Index))
            {
                if (model.Index.Length != 6)
                {
                    ModelState.AddModelError("Index", _localizationService.GetResource("ITB.Portal.Profile.Index.Length"));
                }
                else
                {
                    long index;
                    if (!long.TryParse(model.Index, out index))
                    {
                        ModelState.AddModelError("Index", _localizationService.GetResource("ITB.Portal.Profile.Index.Number"));
                    }
                }
            }


            if (model.Income!=null)
            {
                if (model.Income <= 0)
                {
                    ModelState.AddModelError("Income", _localizationService.GetResource("ITB.Portal.Profile.Income"));
                }
                else
                {
                    if(model.Income >= 4000000)
                        ModelState.AddModelError("Income",_localizationService.GetResource("ITB.Portal.Profile.Income.Max"));
                }
            }

            if (model.NewPassword != null && model.NewPassword.IndexOf(" ") >= 0)
            {
                ModelState.AddModelError("NewPassword", _localizationService.GetResource("Profile.Password.Space"));
            }
            var customer = _workContext.CurrentCustomer;
            if (customer.Email != model.Email)
            {
                var newEmailCustomer = _customerService.GetCustomerByEmail(model.Email);
                if (newEmailCustomer != null)
                {
                    ModelState.AddModelError("Email",_localizationService.GetResource("Account.Register.Errors.EmailAlreadyExists"));
                }
            }

            if (model.DateOfBirthDay.Value != 0 || model.DateOfBirthYear.Value != 0 || model.DateOfBirthMonth.Value != 0)
            {
                if (model.DateOfBirthDay.Value == 0)
                {
                    ModelState.AddModelError("DateOfBirthDay", _localizationService.GetResource("ITB.Portal.Profile.Day.Required"));
                }
                if (model.DateOfBirthYear.Value == 0)
                {
                    ModelState.AddModelError("DateOfBirthYear", _localizationService.GetResource("ITB.Portal.Profile.Year.Required"));
                }
                if (model.DateOfBirthMonth.Value == 0)
                {
                    ModelState.AddModelError("DateOfBirthMonth", _localizationService.GetResource("ITB.Portal.Profile.Month.Required"));
                }
                try
                {
                    customer.BirthdayDate = new DateTime(model.DateOfBirthYear.Value, model.DateOfBirthMonth.Value, model.DateOfBirthDay.Value);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("DateOfBirthDay", _localizationService.GetResource("ITB.Portal.Profile.Birthday.Wrong"));
                }

            }

            
            if (ModelState.IsValid)
            {
                customer.FirstName = model.FirstName;
                customer.LastName = model.LastName;
                customer.PostIndex = model.Index;
                if (model.Income.HasValue)
                {
                    var currency = _currencyService.GetCurrencyById(model.CurrencyId);
                    customer.Income = ((double)model.Income /(double) currency.Rate);
                    customer.CurrencyId = model.CurrencyId;
                }else
                {
                    customer.Income = null;
                    customer.CurrencyId = null;
                }
                if (model.DateOfBirthYear.Value != 0 && model.DateOfBirthMonth.Value != 0 && model.DateOfBirthDay.Value != 0)
                {
                    customer.BirthdayDate = new DateTime(model.DateOfBirthYear.Value, model.DateOfBirthMonth.Value, model.DateOfBirthDay.Value);
                }
                customer.Address = model.Address;
                if (!String.IsNullOrEmpty(model.Gender))
                {
                    customer.Gender = model.Gender == "M" ? 1 : 0;
                }
                else
                {
                    customer.Gender = null;
                }
                if (model.RegionId != 0)
                {
                    customer.RegionId = model.RegionId;
                }
                else
                {
                    customer.RegionId = null;
                }

                if (model.CityId != 0)
                {
                    customer.CityId = model.CityId;
                }
                else
                {
                    customer.CityId = null;
                }
                
                if (customer.Email != model.Email)
                {
                    customer.Email = model.Email;
                }
                _customerService.UpdateCustomer(customer);
                if (model.NewPassword != null)
                {
                    var changePasswordRequest = new ChangePasswordRequest(customer.Email,
                    false, _customerSettings.DefaultPasswordFormat, model.NewPassword);
                    var changePasswordResult = _customerRegistrationService.ChangePassword(changePasswordRequest);
                }
                return RedirectToAction("MyProfile");
            }

            model.AviableCurrencies = _currencyService.GetAllCurrencies().Select(x => new CurrencyModel() { Id = x.Id, Name = x.CurrencyCode }).ToList();
            model.Regions =new List<RegionModel>();
            model.Regions.Add(new RegionModel() { Id = 0, Title = _localizationService.GetResource("ITB.Portal.Register.SelectRegion") });
            var regions= _regionService.GetAllRegions().OrderBy(x=>x.Title).Select(x => new RegionModel()
            {
                Title = x.Title,
                Id = x.Id
            }).ToList();
            foreach (var regionModel in regions)
            {
                model.Regions.Add(regionModel);
            }
            model.Cities=new List<CityModel>();
            model.Cities.Add(new CityModel() { Id = 0, Title = _localizationService.GetResource("ITB.Portal.Register.SelectCity") });

            if (model.RegionId == 0)
            {
                var cities = _cityService.GetAllCities().OrderBy(x=>x.Title).Select(x => new CityModel()
                {
                    Title = x.Title,
                    Id = x.Id
                }).ToList();

                foreach (var cityModel in cities)
                {
                    model.Cities.Add(cityModel);
                }
            }
            else
            {
                var cities = _regionService.GetById(model.RegionId).Cities.OrderBy(x=>x.Title).Select(x => new CityModel()
                {
                    Title = x.Title,
                    Id = x.Id
                }).ToList();

                foreach (var cityModel in cities)
                {
                    model.Cities.Add(cityModel);
                }
            }


            return View(model);
        }


        //public ActionResult RecentlyAddedBuyers()
        //{
        //    var companies = _companyInformationService.GetAllCompanies().Where(x => x.Customers.First().IsBuyer() && !x.Customers.First().Deleted).OrderByDescending(x=>x.Customers.First().CreatedOnUtc).Take(12);
        //    var model = companies.Select(x => PrepareCompanyProfileOverview(x)).ToList();
        //    return View(model);
        //}

        //public ActionResult RecentlyAddedSellers()
        //{
        //    var companies = _companyInformationService.GetAllCompanies().Where(x => x.Customers.First().IsSeller() && !x.Customers.First().Deleted).OrderByDescending(x => x.Customers.First().CreatedOnUtc).Take(12);
        //    var model = companies.Select(x => PrepareCompanyProfileOverview(x)).ToList();
        //    return View(model);
        //}

        public ActionResult MyProfileMenuStrip(int activetab)
        {
            var model = new CustomerMenuModel() { IsSeller = _workContext.CurrentCustomer.IsSeller() };
            model.ActiveTab = (MenuTab)activetab;
            return View(model);
        }
    }
}
