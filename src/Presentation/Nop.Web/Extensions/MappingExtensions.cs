using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using Nop.Core.Domain.Event;
using Nop.Web.Models.Event;
using Nop.Web.Models.CompanyInformation;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.News;
using Nop.Web.Models.News;
using Nop.Core.Domain.Favorit;

namespace Nop.Web.Extensions
{
    public static class MappingExtensions
    {
        //category
        public static CategoryModel ToModel(this Category entity)
        {
            if (entity == null)
                return null;

            var model = new CategoryModel()
            {
                Id = entity.Id,
                Name = entity.GetLocalized(x => x.Name),
                Description = entity.GetLocalized(x => x.Description),
                MetaKeywords = entity.GetLocalized(x => x.MetaKeywords),
                MetaDescription = entity.GetLocalized(x => x.MetaDescription),
                MetaTitle = entity.GetLocalized(x => x.MetaTitle),
                SeName = entity.GetSeName(),
            };
            return model;
        }

        //manufacturer
        public static ManufacturerModel ToModel(this Manufacturer entity)
        {
            if (entity == null)
                return null;

            var model = new ManufacturerModel()
            {
                Id = entity.Id,
                Name = entity.GetLocalized(x => x.Name),
                Description = entity.GetLocalized(x => x.Description),
                MetaKeywords = entity.GetLocalized(x => x.MetaKeywords),
                MetaDescription = entity.GetLocalized(x => x.MetaDescription),
                MetaTitle = entity.GetLocalized(x => x.MetaTitle),
                SeName = entity.GetSeName(),
            };
            return model;
        }

        /// <summary>
        /// Prepare address model
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="address">Address</param>
        /// <param name="excludeProperties">A value indicating whether to exclude properties</param>
        /// <param name="addressSettings">Address settings</param>
        /// <param name="localizationService">Localization service (used to prepare a select list)</param>
        /// <param name="stateProvinceService">State service (used to prepare a select list). null to don't prepare the list.</param>
        /// <param name="loadCountries">A function to load countries  (used to prepare a select list). null to don't prepare the list.</param>
        public static void PrepareModel(this AddressModel model,
            Address address, bool excludeProperties, 
            AddressSettings addressSettings,
            ILocalizationService localizationService = null,
            IStateProvinceService stateProvinceService = null,
            Func<IList<Country>> loadCountries = null,
            int? languageId = null)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (addressSettings == null)
                throw new ArgumentNullException("addressSettings");

            if (!excludeProperties && address != null)
            {
                model.Id = address.Id;
                model.FirstName = address.FirstName;
                model.LastName = address.LastName;
                model.Email = address.Email;
                model.Company = address.Company;
                model.CountryId = address.CountryId;
                model.CountryName = address.Country != null 
                    ? address.Country.GetLocalized(x => x.Name) 
                    : null;
                model.StateProvinceId = address.StateProvinceId;
                model.StateProvinceName = address.StateProvince != null 
                    ? address.StateProvince.GetLocalized(x => x.Name)
                    : null;
                model.City = address.City;
                model.Address1 = address.Address1;
                model.Address2 = address.Address2;
                model.ZipPostalCode = address.ZipPostalCode;
                model.PhoneNumber = address.PhoneNumber;
                model.FaxNumber = address.FaxNumber;
                model.Email1 = address.Email1 != null
                    ? address.Email1
                    : null;
                model.Email2 = address.Email2 != null
                    ? address.Email2
                    : null;
                model.Email3 = address.Email3 != null
                    ? address.Email3
                    : null;
                model.PhoneNumber1 = address.PhoneNumber1 != null
                    ? address.PhoneNumber1
                    : null;
                model.PhoneNumber2 = address.PhoneNumber2 != null
                   ? address.PhoneNumber2
                   : null;
                model.PhoneNumber3 = address.PhoneNumber3 != null
                   ? address.PhoneNumber3
                   : null;

                model.FirstName = address.FirstName;
                model.City = address.City;
                model.Address1 = address.Address1;
                model.LanguageId = languageId;
            }

            //countries and states
            if (addressSettings.CountryEnabled && loadCountries != null)
            {
                if (localizationService == null)
                    throw new ArgumentNullException("localizationService");

                model.AvailableCountries.Add(new SelectListItem() { Text = localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in loadCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem()
                    {
                        Text = c.GetLocalized(x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (addressSettings.StateProvinceEnabled)
                {
                    //states
                    if (stateProvinceService == null)
                        throw new ArgumentNullException("stateProvinceService");

                    var states = stateProvinceService
                        .GetStateProvincesByCountryId(model.CountryId.HasValue ? model.CountryId.Value : 0)
                        .ToList();
                    if (states.Count > 0)
                    {
                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem()
                            {
                                Text = s.GetLocalized(x => x.Name),
                                Value = s.Id.ToString(), 
                                Selected = (s.Id == model.StateProvinceId)
                            });
                        }
                    }
                    else
                    {
                        model.AvailableStates.Add(new SelectListItem()
                        {
                            Text = localizationService.GetResource("Address.OtherNonUS"),
                            Value = "0"
                        });
                    }
                }
            }

            //form fields
            model.CompanyEnabled = addressSettings.CompanyEnabled;
            model.CompanyRequired = addressSettings.CompanyRequired;
            model.StreetAddressEnabled = addressSettings.StreetAddressEnabled;
            model.StreetAddressRequired = addressSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = addressSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = addressSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = addressSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = addressSettings.ZipPostalCodeRequired;
            model.CityEnabled = addressSettings.CityEnabled;
            model.CityRequired = addressSettings.CityRequired;
            model.CountryEnabled = addressSettings.CountryEnabled;
            model.StateProvinceEnabled = addressSettings.StateProvinceEnabled;
            model.PhoneEnabled = addressSettings.PhoneEnabled;
            model.PhoneRequired = addressSettings.PhoneRequired;
            model.FaxEnabled = addressSettings.FaxEnabled;
            model.FaxRequired = addressSettings.FaxRequired;
        }
        public static Address ToEntity(this AddressModel model)
        {
            if (model == null)
                return null;

            var entity = new Address();
            return ToEntity(model, entity);
        }
        public static Address ToEntity(this AddressModel model, Address destination)
        {
            if (model == null)
                return destination;

            destination.Id = model.Id;
            destination.FirstName = model.FirstName;
            destination.LastName = model.LastName;
            destination.Email = model.Email;
            destination.Company = model.Company;
            destination.CountryId = model.CountryId;
            destination.StateProvinceId = model.StateProvinceId;
            destination.City = model.City;
            destination.Address1 = model.Address1;
            destination.Address2 = model.Address2;
            destination.ZipPostalCode = model.ZipPostalCode;
            destination.PhoneNumber = model.PhoneNumber;
            destination.FaxNumber = model.FaxNumber;
            destination.Email1 = model.Email1;
            destination.Email2 = model.Email2;
            destination.Email3 = model.Email3;
            destination.PhoneNumber1 = model.PhoneNumber1;
            destination.PhoneNumber2 = model.PhoneNumber2;
            destination.PhoneNumber3 = model.PhoneNumber3;
            return destination;
        }

        public static EventInfoModel ToModel(this  Event entity,int languageId)
        {
            if(entity == null)
                throw(new ArgumentNullException("event"));
            var model = new EventInfoModel()
            {
                Title = entity.GetLocalized(x => x.Title, languageId: languageId, returnDefaultValue: false),
                ShortDescription = entity.GetLocalized(x => x.ShortDescription, languageId: languageId, returnDefaultValue: false),
                FullDescription = entity.GetLocalized(x => x.FullDescription, languageId: languageId, returnDefaultValue: false),
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                SeName = entity.GetSeName(languageId),
                PictureId = entity.PictureId,
                CatalogPictureid = entity.CatalogPictureId
            };

            return model;
        }

        public static CompanyInformationModel ToModel(this CompanyInformation entity, int languageId)
        {
            if (entity == null)
                throw new ArgumentNullException("company");
            var model = new CompanyInformationModel()
            {
                CompanyName = entity.CompanyName,
                CompanyDescription = entity.CompanyDescription,
                AccountNumbers = entity.AccountNumbers,
                BankAddress = entity.BankAddress,
                BankName = entity.BankName,
                LegalAddress = entity.LegalAddress,
                SWIFT = entity.SWIFT,
                TaxRegistrationNumber = entity.TaxRegistrationNumber,
                TopExecutiveName = entity.TopExecutiveName,
                SeName = entity.GetSeName(languageId,returnDefaultValue:true),
            };
            var customer = entity.Customers.FirstOrDefault();
            model.CustomerId = customer.Id;
            if (customer.IsSeller())
            {
                model.Seller = true;
            }

            return model;
        }

        public static NewsItem ToEntity(this NewsItemModel model, int customerId)
        {
            var newItem = new NewsItem()
            {
                CustomerId = customerId,
                CreatedOnUtc = DateTime.UtcNow,
                Published = false,
                Title = model.Title,
                Full = model.Full,
                Short = model.Short,
                LanguageId = model.Language,
                StartDateUtc = DateTime.UtcNow.AddMinutes(-1),
                EndDateUtc = DateTime.MaxValue,
                ExtendedProfileOnly = model.ExtendedProfileDisplay
            };

            return newItem;
        }

        public static NewsItem ToEntity(this NewsItemModel model, NewsItem entity, int customerId)
        {
            entity.CustomerId = customerId;
            entity.Title = model.Title;
            entity.Short = model.Short;
            entity.Full = model.Full;
            entity.Short = model.Short;
            entity.StartDateUtc = DateTime.UtcNow.AddMinutes(-1);
            entity.EndDateUtc = DateTime.MaxValue;
            entity.ExtendedProfileOnly = model.ExtendedProfileDisplay;
            return entity;
        }
    }
}