using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Customers
{
    public interface ICompanyInformationService
    {
        CompanyInformation GetCompanyInformation(int id);
        void UpdateCompany(CompanyInformation company);
        IList<CompanyInformation> GetAllCompanies();
    }
}
