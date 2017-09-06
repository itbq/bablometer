using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Localization;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Customers
{
    public class CompanyInformationViewService : ICompanyInformationViewService
    {
        private readonly IRepository<CompanyInformationView> _repository;
        private readonly ICompanyInformationService _companyInfromationService;
        private readonly CompanyInformationSettings _companyInformationSettings;
        private readonly CatalogSettings _catalogSettings;

        public CompanyInformationViewService(IRepository<CompanyInformationView> repository,
            CompanyInformationSettings companyInformationSettings,
            ICompanyInformationService companyInfromationService,
            CatalogSettings catalogSettings)
        {
            this._catalogSettings = catalogSettings;
            this._repository = repository;
            this._companyInformationSettings = companyInformationSettings;
            this._companyInfromationService = companyInfromationService;
        }

        /// <summary>
        /// Check if specified product has specified language locale
        /// </summary>
        /// <param name="productId">product id</param>
        /// <param name="languageId">language id</param>
        /// <returns></returns>
        protected bool CheckCompany(int companyId, int languageId)
        {
            var company = _companyInfromationService.GetCompanyInformation(companyId);
            if (company.GetLocalized(x => x.CompanyName, languageId, false, false) == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// get recently viewed companies public profiles by specified customer
        /// </summary>
        /// <param name="customerId">customer id</param>
        /// <returns></returns>
        public IList<CompanyInformation> RecentlyViewevCompaniesSellers(int customerId, int languageId)
        {
            var companies = _repository.Table.Where(x => x.CustomerId == customerId).ToList();

            return companies.Where(x => CheckCompany(x.CompanyInformationId, languageId) && !x.Customer.Deleted)
                .OrderByDescending(x=>x.LastViewOnUtc)
                .Select(x => x.CompanyInformation)
                .Take(_catalogSettings.RecentlyViewedSellersNumber)
                .ToList();
        }

        /// <summary>
        /// Add company to recently viewed companies
        /// </summary>
        /// <param name="companyInformationId">company information id</param>
        /// <param name="customerId">customer id</param>
        public void AddCompanyInformationView(int companyInformationId, int customerId)
        {
            if (companyInformationId == 0 || customerId == 0)
                throw new ArgumentNullException("companyInformation view");
            var companyInformationViews = _repository.Table.Where(x => x.CustomerId == customerId).OrderBy(x => x.LastViewOnUtc);
            var companyInformationView = companyInformationViews.Where(x => x.CompanyInformationId == companyInformationId).FirstOrDefault();
            if (companyInformationView == null)
            {
                if (companyInformationViews.Count() > _companyInformationSettings.CompanyInformationMaxViewNumber)
                    _repository.Delete(companyInformationViews.Last());

                _repository.Insert(new CompanyInformationView() { CustomerId = customerId, CompanyInformationId = companyInformationId,LastViewOnUtc = DateTime.UtcNow });
            }
            else
            {
                companyInformationView.LastViewOnUtc = DateTime.UtcNow;
                _repository.Update(companyInformationView);
            }
        }
    }
}
