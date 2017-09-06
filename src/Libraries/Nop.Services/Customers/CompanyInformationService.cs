using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Customers
{
    public partial class CompanyInformationService:ICompanyInformationService
    {

        private const string COMPANYINFORMATION_ALL_KEY = "Nop.CompanyInformation.all-{0}";
        private const string COMPANYINFORMATION_PATTERN_KEY = "Nop.CompanyInformation.";
        IRepository<CompanyInformation> _companyRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        public CompanyInformationService(IRepository<CompanyInformation> repository,
            IEventPublisher eventPublisher,
            ICacheManager cacheManager)
        {
            this._companyRepository = repository;
            this._eventPublisher = eventPublisher;
            this._cacheManager = cacheManager;
        }

        /// <summary>
        /// Get company information by id
        /// </summary>
        /// <param name="id">company id</param>
        /// <returns>company infromation</returns>
        public CompanyInformation GetCompanyInformation(int id)
        {
            return _companyRepository.GetById(id);
        }

        /// <summary>
        /// Update company information
        /// </summary>
        /// <param name="company">company to update</param>
        public void UpdateCompany(CompanyInformation company)
        {
            if (company == null)
                throw new ArgumentNullException("content");

            _companyRepository.Update(company);

            //event notification
            _eventPublisher.EntityUpdated(company);
        }

        /// <summary>
        /// Get all compaies
        /// </summary>
        /// <returns>List of all companies</returns>
        public IList<CompanyInformation> GetAllCompanies()
        {
            string key = string.Format(COMPANYINFORMATION_ALL_KEY, false);
            return _cacheManager.Get(key, () =>
                {
                    var query = _companyRepository.Table;
                    var companies = query.ToList();
                    return companies;
                });
        }
    }
}
