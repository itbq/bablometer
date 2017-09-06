using System;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Common
{
    public class Address : BaseEntity, ICloneable, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the first name
        /// </summary>
        public virtual string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name
        /// </summary>
        public virtual string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Second email
        /// </summary>
        public virtual string Email1 { get; set; }

        /// <summary>
        /// Third email
        /// </summary>
        public virtual string Email2 { get; set; }

        /// <summary>
        /// fourth email
        /// </summary>
        public virtual string Email3 { get; set; }

        /// <summary>
        /// Gets or sets the company
        /// </summary>
        public virtual string Company { get; set; }

        /// <summary>
        /// Gets or sets the country identifier
        /// </summary>
        public virtual int? CountryId { get; set; }

        /// <summary>
        /// Gets or sets the state/province identifier
        /// </summary>
        public virtual int? StateProvinceId { get; set; }

        /// <summary>
        /// Gets or sets the city
        /// </summary>
        public virtual string City { get; set; }

        /// <summary>
        /// Gets or sets the address 1
        /// </summary>
        public virtual string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the address 2
        /// </summary>
        public virtual string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the zip/postal code
        /// </summary>
        public virtual string ZipPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the phone number
        /// </summary>
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        /// second phone number
        /// </summary>
        public virtual string PhoneNumber1 { get; set; }

        /// <summary>
        /// third phone number
        /// </summary>
        public virtual string PhoneNumber2 { get; set; }

        /// <summary>
        /// fourth phone number
        /// </summary>
        public virtual string PhoneNumber3 { get; set; }

        /// <summary>
        /// Gets or sets the fax number
        /// </summary>
        public virtual string FaxNumber { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the country
        /// </summary>
        public virtual Country Country { get; set; }

        /// <summary>
        /// Gets or sets the state/province
        /// </summary>
        public virtual StateProvince StateProvince { get; set; }

        public object Clone()
        {
            var addr = new Address()
            {
                FirstName = this.FirstName,
                LastName = this.LastName,
                Email = this.Email,
                Email1 = this.Email1,
                Email2 = this.Email2,
                Email3 = this.Email3,
                Company = this.Company,
                Country = this.Country,
                CountryId = this.CountryId,
                StateProvince = this.StateProvince,
                StateProvinceId = this.StateProvinceId,
                City = this.City,
                Address1 = this.Address1,
                Address2 = this.Address2,
                ZipPostalCode = this.ZipPostalCode,
                PhoneNumber = this.PhoneNumber,
                PhoneNumber1 = this.PhoneNumber1,
                PhoneNumber2 = this.PhoneNumber2,
                PhoneNumber3 = this.PhoneNumber3,
                FaxNumber = this.FaxNumber,
                CreatedOnUtc = this.CreatedOnUtc,
            };
            return addr;
        }
    }
}
